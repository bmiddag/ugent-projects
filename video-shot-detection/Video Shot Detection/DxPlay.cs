/****************************************************************************
While the underlying libraries are covered by LGPL, this sample is released 
as public domain.  It is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
or FITNESS FOR A PARTICULAR PURPOSE.  
*****************************************************************************/

// Two very important things to know when using this class:
// 1) Make sure you DxPlay.Dispose before shutting down the app (to avoid hanging)
// 2) Make sure you Marshal.FreeCoTaskMem items returned from DxPlay.SnapShot (to avoid leaking)

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

using DirectShowLib;


namespace DirectShow {
    internal class DxPlayer : IDisposable {
        enum GraphState {
            Stopped,
            Paused,
            Running,
            Exiting
        }

        #region Member variables

        // File name we are playing
        private string m_sFileName;

        // graph builder interfaces
        private IFilterGraph2 m_FilterGraph;
        private IMediaControl m_mediaCtrl;
        private IMediaEvent m_mediaEvent;

        // Used to grab current snapshots
        ISampleGrabber m_sampGrabber = null;

        // Grab once.  Used to create bitmap
        private int m_videoWidth;
        private int m_videoHeight;
        private int m_stride;
        private int m_ImageSize; // In bytes
        private double m_frameDuration;
        private Control owner;

        // Event used by Media Event thread
        private ManualResetEvent m_mre;

        // Current state of the graph (can change async)
        volatile private GraphState m_State = GraphState.Stopped;

#if DEBUG
        // Allow you to "Connect to remote graph" from GraphEdit
        DsROTEntry m_DsRot;
#endif

        #endregion

        // Release everything.
        public void Dispose() {
            CloseInterfaces();
        }
        ~DxPlayer() {
            CloseInterfaces();
        }

        // Event that is called when a clip finishs playing
        public event DxPlayEvent StopPlay;
        public delegate void DxPlayEvent(Object sender);

        // Play an avi file into a window.  Allow for snapshots.
        // (Control to show video in, Avi file to play
        public DxPlayer(Control hWin, string FileName) {
            try {
                int hr;
                IntPtr hEvent;

                // Save off the file name
                m_sFileName = FileName;

                owner = hWin;
             

                // Set up the graph
                SetupGraph(hWin, FileName);

                // Get the event handle the graph will use to signal
                // when events occur
                hr = m_mediaEvent.GetEventHandle(out hEvent);
                DsError.ThrowExceptionForHR(hr);

                // Wrap the graph event with a ManualResetEvent
                m_mre = new ManualResetEvent(false);
#if USING_NET11
                m_mre.Handle = hEvent;
#else
                m_mre.SafeWaitHandle = new Microsoft.Win32.SafeHandles.SafeWaitHandle(hEvent, true);
#endif
                IMediaPosition mediaPos = m_FilterGraph as IMediaPosition;
                OABool canSeek;
                hr = mediaPos.CanSeekForward(out canSeek);
                if (canSeek == OABool.False) {
                    Console.WriteLine("You suck Star Wars!");
                } 
                hr = mediaPos.get_Duration(out duration);
                DsError.ThrowExceptionForHR(hr);

                // Create a new thread to wait for events
                Thread t = new Thread(new ThreadStart(this.EventWait));
                t.Name = "Media Event Thread";
                t.Start();
            } catch {
                Dispose();
                throw;
            }
        }

        // Wait for events to happen.  This approach uses waiting on an event handle.
        // The nice thing about doing it this way is that you aren't in the windows message
        // loop, and don't have to worry about re-entrency or taking too long.  Plus, being
        // in a class as we are, we don't have access to the message loop.
        // Alternately, you can receive your events as windows messages.  See
        // IMediaEventEx.SetNotifyWindow.
        private void EventWait() {
            // Returned when GetEvent is called but there are no events
            const int E_ABORT = unchecked((int)0x80004004);

            int hr;
            IntPtr p1, p2;
            EventCode ec;

            do {
                // Wait for an event
                m_mre.WaitOne(-1, true);

                // Avoid contention for m_State
                lock(this) {
                    // If we are not shutting down
                    if (m_State != GraphState.Exiting) {
                        // Read the event
                        for(hr = m_mediaEvent.GetEvent(out ec, out p1, out p2, 0); hr >= 0; hr = m_mediaEvent.GetEvent(out ec, out p1, out p2, 0)) {
                            // Write the event name to the debug window
                            Debug.WriteLine(ec.ToString());

                            // If the clip is finished playing
                            if (ec == EventCode.Complete) {
                                // Call Stop() to set state
                                Stop();

                                // Throw event
                                if (StopPlay != null) {
                                    StopPlay(this);
                                }
                            }

                            // Release any resources the message allocated
                            hr = m_mediaEvent.FreeEventParams(ec, p1, p2);
                            DsError.ThrowExceptionForHR(hr);
                        }

                        // If the error that exited the loop wasn't due to running out of events
                        if (hr != E_ABORT) {
                            DsError.ThrowExceptionForHR(hr);
                        }
                    } else {
                        // We are shutting down
                        break;
                    }
                }
            } while (true);
        }

        // Return the currently playing file name
        public string FileName {
            get {
                return m_sFileName;
            }
        }

        public double CurrentTime {
            set {
                IMediaPosition mediaPos = m_FilterGraph as IMediaPosition;
                int hr;

                if (value >= 0 && value <= Duration) {
                    lock (this) {
                        hr = mediaPos.put_CurrentPosition(value);
                    }
                    DsError.ThrowExceptionForHR(hr);
                }
            }

            get {
                IMediaPosition mediaPos = m_FilterGraph as IMediaPosition;
                int hr;

                double currentTime;
                lock (this) {
                    hr = mediaPos.get_CurrentPosition(out currentTime);
                }
                DsError.ThrowExceptionForHR(hr);

                return currentTime;
            }
        }
        private double duration;

        public double Duration {
            get {
                return duration;
            }
        }

        public int TotalFrames {
            get {
                return (int)(duration / m_frameDuration);
            }
        }

        public double FrameDuration {
            get {
                return m_frameDuration;
            }
        }

        public int CurrentFrameFast {
            get {
                return Convert.ToInt32(Math.Min(TotalFrames - 1, Math.Floor(CurrentTime / m_frameDuration)));
            }
            set {
                CurrentTime = value * m_frameDuration;
            }
        }

        public int CurrentFrame {
            get {              
                IMediaSeeking mediaPos = m_FilterGraph as IMediaSeeking;
                int hr;
                hr = mediaPos.IsFormatSupported(TimeFormat.Frame);
                if (hr == 0) {
                    hr = mediaPos.SetTimeFormat(TimeFormat.Frame);
                    DsError.ThrowExceptionForHR(hr);
                    long currentFrame;
                    lock (this) {
                        hr = mediaPos.GetCurrentPosition(out currentFrame);
                    }
                    DsError.ThrowExceptionForHR(hr);

                    return (int)currentFrame;
                } else return CurrentFrameFast;
            }
            set {
                IMediaSeeking mediaPos = m_FilterGraph as IMediaSeeking;
                int hr;
                hr = mediaPos.IsFormatSupported(TimeFormat.Frame);
                if (hr == 0) {
                    if (value >= 0 && value <= duration / m_frameDuration) {
                        lock (this) {
                            hr = mediaPos.SetTimeFormat(TimeFormat.Frame);
                            DsError.ThrowExceptionForHR(hr);
                            hr = mediaPos.SetPositions(value, AMSeekingSeekingFlags.AbsolutePositioning, null, AMSeekingSeekingFlags.NoPositioning);
                        }
                        DsError.ThrowExceptionForHR(hr);
                    }
                } else CurrentFrameFast = value;
            }
        }

        // start playing
        public void Start() {
            // If we aren't already playing (or shutting down)
            if (m_State == GraphState.Stopped || m_State == GraphState.Paused) {
                int hr = m_mediaCtrl.Run();
                DsError.ThrowExceptionForHR( hr );

                // Make the window visible
                IVideoWindow videoWindow = m_FilterGraph as IVideoWindow;
                hr = videoWindow.put_Visible(OABool.True);
                DsError.ThrowExceptionForHR(hr);

                m_State = GraphState.Running;
            }
        }

        // Pause the capture graph.
        public void Pause() {
            // If we are playing
            if (m_State == GraphState.Running) {
                int hr = m_mediaCtrl.Pause();
                DsError.ThrowExceptionForHR(hr);

                m_State = GraphState.Paused;
            }
        }
        // Pause the capture graph.
        public void Stop() {
            // Can only Stop when playing or paused
            if (m_State == GraphState.Running || m_State == GraphState.Paused) {
                Rewind();
                int hr = m_mediaCtrl.Stop();
                DsError.ThrowExceptionForHR( hr );

                // Make the window invisible
                IVideoWindow videoWindow = m_FilterGraph as IVideoWindow;
                hr = videoWindow.put_Visible(OABool.False);
                DsError.ThrowExceptionForHR(hr);

                m_State = GraphState.Stopped;
            }
        }

        // Reset the clip back to the beginning
        public void Rewind() {
            int hr;

            IMediaPosition imp = m_FilterGraph as IMediaPosition;
            hr = imp.put_CurrentPosition(0);
        }

        // Grab a snapshot of the most recent image played.
        // Returns A pointer to the raw pixel data. Caller must release this memory with
        // Marshal.FreeCoTaskMem when it is no longer needed.
        public IntPtr SnapShot() {
            int hr;
            IntPtr ip = IntPtr.Zero;
            int iBuffSize = 0;

            // Read the buffer size
            lock (this) {
                hr = m_sampGrabber.GetCurrentBuffer(ref iBuffSize, ip);
                DsError.ThrowExceptionForHR(hr);

                Debug.Assert(iBuffSize == m_ImageSize, "Unexpected buffer size");

                // Allocate the buffer and read it
                ip = Marshal.AllocCoTaskMem(iBuffSize);

                hr = m_sampGrabber.GetCurrentBuffer(ref iBuffSize, ip);
                DsError.ThrowExceptionForHR(hr);
            }

            return ip;
        }

        // Convert a point to the raw pixel data to a .NET bitmap
        public Bitmap IPToBmp(IntPtr ip) {
            // We know the Bits Per Pixel is 24 (3 bytes) because we forced it 
            // to be with sampGrabber.SetMediaType()
            int iBufSize = m_videoWidth * m_videoHeight * 3;

            return new Bitmap(
                m_videoWidth, 
                m_videoHeight, 
                -m_stride, 
                PixelFormat.Format24bppRgb, 
                (IntPtr)(ip.ToInt32() + iBufSize - m_stride)
                );
        }

        // Build the capture graph for grabber and renderer.</summary>
        // (Control to show video in, Filename to play)
        private void SetupGraph(Control hWin, string FileName) {
            int hr;

            // Get the graphbuilder object
            m_FilterGraph = new FilterGraph() as IFilterGraph2;

            // Get a ICaptureGraphBuilder2 to help build the graph
            ICaptureGraphBuilder2 icgb2 = new CaptureGraphBuilder2() as ICaptureGraphBuilder2;

            try {
                // Link the ICaptureGraphBuilder2 to the IFilterGraph2
                hr = icgb2.SetFiltergraph(m_FilterGraph);
                DsError.ThrowExceptionForHR( hr );

#if DEBUG
                // Allows you to view the graph with GraphEdit File/Connect
                m_DsRot = new DsROTEntry(m_FilterGraph);
#endif
                // Add the filters necessary to render the file.  This function will
                // work with a number of different file types.
                IBaseFilter	sourceFilter = null;
                hr = m_FilterGraph.AddSourceFilter(FileName, FileName, out sourceFilter);
                DsError.ThrowExceptionForHR( hr );

                // Get the SampleGrabber interface
                m_sampGrabber = (ISampleGrabber) new SampleGrabber();
                IBaseFilter baseGrabFlt = (IBaseFilter)	m_sampGrabber;

                // Configure the Sample Grabber
                ConfigureSampleGrabber(m_sampGrabber);

                // Add it to the filter
                hr = m_FilterGraph.AddFilter( baseGrabFlt, "Ds.NET Grabber" );
                DsError.ThrowExceptionForHR( hr );

                // Connect the pieces together, use the default renderer
                hr = icgb2.RenderStream(null, null, sourceFilter, baseGrabFlt, null);
                DsError.ThrowExceptionForHR( hr );

                // Now that the graph is built, read the dimensions of the bitmaps we'll be getting
                SaveSizeInfo(m_sampGrabber);

                // Configure the Video Window
                IVideoWindow videoWindow = m_FilterGraph as IVideoWindow;
                ConfigureVideoWindow(videoWindow, hWin);

                // Grab some other interfaces
                m_mediaEvent = m_FilterGraph as IMediaEvent;
                m_mediaCtrl = m_FilterGraph as IMediaControl;
            } finally {
                if (icgb2 != null) {
                    Marshal.ReleaseComObject(icgb2);
                    icgb2 = null;
                }
            }
#if DEBUG
            // Double check to make sure we aren't releasing something
            // important.
            GC.Collect();
            GC.WaitForPendingFinalizers();
#endif
        }

        // Configure the video window
        private void ConfigureVideoWindow(IVideoWindow videoWindow, Control hWin) {
            int hr;

            // Set the output window
            hr = videoWindow.put_Owner( hWin.Handle );
            DsError.ThrowExceptionForHR( hr );

            // Set the window style
            hr = videoWindow.put_WindowStyle( (WindowStyle.Child | WindowStyle.ClipChildren | WindowStyle.ClipSiblings) );
            DsError.ThrowExceptionForHR( hr );

            // Make the window visible
            hr = videoWindow.put_Visible( OABool.True );
            DsError.ThrowExceptionForHR( hr );

            // Position the playing location
            Rectangle rc = hWin.ClientRectangle;

            int winWidth = rc.Right - rc.Left;
            int winHeight = rc.Bottom - rc.Top;
            double winRatio = (double)winWidth / (double)winHeight;
            double vidRatio = (double)m_videoWidth / (double)m_videoHeight;

            if(m_videoHeight < winHeight && m_videoWidth < winWidth) {
                int hOffset = winWidth - m_videoWidth;
                int vOffset = winHeight - m_videoHeight;
                hr = videoWindow.SetWindowPosition(rc.Left + hOffset / 2, rc.Top + vOffset / 2, m_videoWidth, m_videoHeight);
            } else if (vidRatio > winRatio) {
                // width from left to right, offset in vertical
                double scale = (double)winWidth / (double)m_videoWidth;
                int offset = (int)(winHeight - (m_videoHeight * scale));
                hr = videoWindow.SetWindowPosition(rc.Left, rc.Top + offset / 2, rc.Right, rc.Bottom - offset);
            } else {
                // height from top to buttom, offset in horizontal
                double scale = (double)winHeight / (double)m_videoHeight;
                int offset = (int)(winWidth - (m_videoWidth * scale));
                hr = videoWindow.SetWindowPosition(rc.Left + offset / 2, rc.Top, rc.Right - offset, rc.Bottom);
            }

            DsError.ThrowExceptionForHR( hr );
        }

        // Set the options on the sample grabber
        private void ConfigureSampleGrabber(ISampleGrabber sampGrabber) {
            AMMediaType media;
            int hr;

            // Set the media type to Video/RBG24
            media = new AMMediaType();
            media.majorType	= MediaType.Video;
            media.subType	= MediaSubType.RGB24;
            media.formatType = FormatType.VideoInfo;
            hr = sampGrabber.SetMediaType( media );
            DsError.ThrowExceptionForHR( hr );

            DsUtils.FreeAMMediaType(media);
            media = null;

            // Configure the samplegrabber
            hr = sampGrabber.SetBufferSamples( true );
            DsError.ThrowExceptionForHR( hr );
        }

        // Save the size parameters for use in SnapShot
        private void SaveSizeInfo(ISampleGrabber sampGrabber) {
            int hr;

            // Get the media type from the SampleGrabber
            AMMediaType media = new AMMediaType();

            hr = sampGrabber.GetConnectedMediaType( media );
            DsError.ThrowExceptionForHR( hr );

            try {
                if( (media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero) ) {
                    throw new NotSupportedException( "Unknown Grabber Media Format" );
                }

                // Get the struct
                VideoInfoHeader videoInfoHeader = new VideoInfoHeader();
                Marshal.PtrToStructure( media.formatPtr, videoInfoHeader);

                // Grab the size info
                m_videoWidth = videoInfoHeader.BmiHeader.Width;
                m_videoHeight = videoInfoHeader.BmiHeader.Height;
                m_stride = videoInfoHeader.BmiHeader.ImageSize / m_videoHeight;
                m_ImageSize = videoInfoHeader.BmiHeader.ImageSize;
                m_frameDuration = videoInfoHeader.AvgTimePerFrame / 10000000.0;
            } finally {
                DsUtils.FreeAMMediaType(media);
                media = null;
            }
        }

        // Shut down capture
        private void CloseInterfaces() {
            int hr;

            lock (this) {
                if (m_State != GraphState.Exiting) {
                    m_State = GraphState.Exiting;

                    // Release the thread (if the thread was started)
                    if (m_mre != null) {
                        m_mre.Set();
                    }
                }

                if( m_mediaCtrl != null ) {
                    // Stop the graph
                    hr = m_mediaCtrl.Stop();
                    m_mediaCtrl = null;
                }

                if (m_sampGrabber != null) {
                    Marshal.ReleaseComObject(m_sampGrabber);
                    m_sampGrabber = null;
                }

#if DEBUG
                if (m_DsRot != null) {
                    m_DsRot.Dispose();
                }
#endif

                if (m_FilterGraph != null) {
                    Marshal.ReleaseComObject(m_FilterGraph);
                    m_FilterGraph = null;
                }
            }
            GC.Collect();
        }
    }
}
