/****************************************************************************
While the underlying libraries are covered by LGPL, this sample is released 
as public domain.  It is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
or FITNESS FOR A PARTICULAR PURPOSE.  
*****************************************************************************/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

using DirectShowLib;

namespace DirectShow {
    /// <summary> Summary description for MainForm. </summary>
    internal class DxScanner : ISampleGrabberCB, IDisposable {
        #region Member variables

        /// <summary> graph builder interface. </summary>
        private IFilterGraph2 m_FilterGraph = null;
        IMediaControl m_mediaCtrl = null;
        IMediaEvent m_MediaEvent = null;

        /// <summary> Dimensions of the image, calculated once in constructor. </summary>
        private int m_videoWidth;
        private int m_videoHeight;
        private int m_stride;
        private int detectionMethod;
        private double param1 = 0.0;
        private double param2 = 0.0;
        private double param3 = 0.0;
        public int m_Count = 0;
        private int[][] histogramOld1;
        private int[][] histogramNew1;
        private int[][][] histogramOld2;
        private int[][][] histogramNew2;
        private byte[] previousFrame;
        public Shot currentShot;
        private ShotDetectionWizard wizard;
        private bool continueScan;
        public bool disposing;

#if DEBUG
        // Allow you to "Connect to remote graph" from GraphEdit
        DsROTEntry m_rot = null;
#endif

        #endregion

        #region API

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] uint Length);

        #endregion

        public bool ContinueScan {
            get {
                lock (this) {
                    return continueScan;
                }
            }
            set {
                lock (this) {
                    continueScan = value;
                }
            }
        }

        /// <summary> File name to scan</summary>
        public DxScanner(string FileName, int method, double p1, double p2, double p3, ShotDetectionWizard wizard) {
            try {
                // Set up the capture graph
                SetupGraph(FileName);

                continueScan = true;
                disposing = false;
                // Initialize the detection
                this.detectionMethod = method;
                this.param1 = p1;
                this.param2 = p2;
                this.param3 = p3;
                this.wizard = wizard;
                previousFrame = new byte[m_videoHeight * m_stride];
                if (detectionMethod == 2) {
                    this.param2 = Math.Pow(2, p2);
                    this.histogramOld1 = new int[(int)param2][];
                    this.histogramNew1 = new int[(int)param2][];
                    for (int i = 0; i < (int)param2; i++)
                    {
                        this.histogramOld1[i] = new int[3];
                        this.histogramNew1[i] = new int[3];
                    }
                } else if (detectionMethod == 3) {
                    this.param2 = Math.Pow(2, p2);
                    this.param3 = Math.Pow(p3, 2);

                    this.histogramOld2 = new int[(int)param2][][];
                    this.histogramNew2 = new int[(int)param2][][];
                    for (int i = 0; i < (int)param2; i++)
                    {
                        this.histogramOld2[i] = new int[3][];
                        this.histogramNew2[i] = new int[3][];
                        for (int j = 0; j < 3; j++)
                        {
                            this.histogramOld2[i][j] = new int[(int)param3];
                            this.histogramNew2[i][j] = new int[(int)param3];
                        }
                    }
                }

            } catch {
                Dispose();
                throw;
            }
        }
        /// <summary> release everything. </summary>
        public void Dispose() {
            CloseInterfaces();
        }
        // Destructor
        ~DxScanner() {
            CloseInterfaces();
        }


        /// <summary> capture the next image </summary>
        public void Start() {
            int hr = m_mediaCtrl.Run();
            DsError.ThrowExceptionForHR(hr);
        }

        public bool CheckIfDone() {
            int hr;
            EventCode evCode;
            const int E_Abort = unchecked((int)0x80004004);

            //System.Windows.Forms.Application.DoEvents();
            hr = this.m_MediaEvent.WaitForCompletion(100, out evCode);
            //DsError.ThrowExceptionForHR(hr);
            return (hr != E_Abort);
        }


        public void WaitUntilDone() {
            int hr;
            EventCode evCode;
            const int E_Abort = unchecked((int)0x80004004);

            do {
                System.Windows.Forms.Application.DoEvents();
                hr = this.m_MediaEvent.WaitForCompletion(100, out evCode);
            } while (hr == E_Abort);
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary> build the capture graph for grabber. </summary>
        private void SetupGraph(string FileName) {
            int hr;

            ISampleGrabber sampGrabber = null;
            IBaseFilter baseGrabFlt = null;
            IBaseFilter capFilter = null;
            IBaseFilter nullrenderer = null;

            // Get the graphbuilder object
            m_FilterGraph = new FilterGraph() as IFilterGraph2;
            m_mediaCtrl = m_FilterGraph as IMediaControl;
            m_MediaEvent = m_FilterGraph as IMediaEvent;

            IMediaFilter mediaFilt = m_FilterGraph as IMediaFilter;

            try {
#if DEBUG
                m_rot = new DsROTEntry(m_FilterGraph);
#endif

                // Add the video source
                hr = m_FilterGraph.AddSourceFilter(FileName, "Ds.NET FileFilter", out capFilter);
                DsError.ThrowExceptionForHR(hr);

                // Get the SampleGrabber interface
                sampGrabber = new SampleGrabber() as ISampleGrabber;
                baseGrabFlt = sampGrabber as IBaseFilter;

                ConfigureSampleGrabber(sampGrabber);

                // Add the frame grabber to the graph
                hr = m_FilterGraph.AddFilter(baseGrabFlt, "Ds.NET Grabber");
                DsError.ThrowExceptionForHR(hr);

                // ---------------------------------
                // Connect the file filter to the sample grabber

                // Hopefully this will be the video pin, we could check by reading it's mediatype
                IPin iPinOut = DsFindPin.ByDirection(capFilter, PinDirection.Output, 0);

                // Get the input pin from the sample grabber
                IPin iPinIn = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Input, 0);

                hr = m_FilterGraph.Connect(iPinOut, iPinIn);
                DsError.ThrowExceptionForHR(hr);

                // Add the null renderer to the graph
                nullrenderer = new NullRenderer() as IBaseFilter;
                hr = m_FilterGraph.AddFilter(nullrenderer, "Null renderer");
                DsError.ThrowExceptionForHR(hr);

                // ---------------------------------
                // Connect the sample grabber to the null renderer

                iPinOut = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Output, 0);
                iPinIn = DsFindPin.ByDirection(nullrenderer, PinDirection.Input, 0);

                hr = m_FilterGraph.Connect(iPinOut, iPinIn);
                DsError.ThrowExceptionForHR(hr);

                // Turn off the clock.  This causes the frames to be sent
                // thru the graph as fast as possible
                hr = mediaFilt.SetSyncSource(null);
                DsError.ThrowExceptionForHR(hr);

                // Read and cache the image sizes
                SaveSizeInfo(sampGrabber);
            } finally {
                if (capFilter != null) {
                    Marshal.ReleaseComObject(capFilter);
                    capFilter = null;
                }
                if (sampGrabber != null) {
                    Marshal.ReleaseComObject(sampGrabber);
                    sampGrabber = null;
                }
                if (nullrenderer != null) {
                    Marshal.ReleaseComObject(nullrenderer);
                    nullrenderer = null;
                }
            }
        }

        /// <summary> Read and store the properties </summary>
        private void SaveSizeInfo(ISampleGrabber sampGrabber) {
            int hr;

            // Get the media type from the SampleGrabber
            AMMediaType media = new AMMediaType();
            hr = sampGrabber.GetConnectedMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero)) {
                throw new NotSupportedException("Unknown Grabber Media Format");
            }

            // Grab the size info
            VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
            m_videoWidth = videoInfoHeader.BmiHeader.Width;
            m_videoHeight = videoInfoHeader.BmiHeader.Height;
            m_stride = m_videoWidth * (videoInfoHeader.BmiHeader.BitCount / 8);

            DsUtils.FreeAMMediaType(media);
            media = null;
        }

        /// <summary> Set the options on the sample grabber </summary>
        private void ConfigureSampleGrabber(ISampleGrabber sampGrabber) {
            AMMediaType media;
            int hr;

            // Set the media type to Video/RBG24
            media = new AMMediaType();
            media.majorType = MediaType.Video;
            media.subType = MediaSubType.RGB24;
            media.formatType = FormatType.VideoInfo;
            hr = sampGrabber.SetMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            DsUtils.FreeAMMediaType(media);
            media = null;

            // Choose to call BufferCB instead of SampleCB
            hr = sampGrabber.SetCallback(this, 1);
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary> Shut down capture </summary>
        private void CloseInterfaces() {
            int hr;

            disposing = true;
            ContinueScan = true;
            
            try {
                if (m_mediaCtrl != null) {
                    // Stop the graph
                    hr = m_mediaCtrl.Stop();
                    m_mediaCtrl = null;
                }
            }
            catch (Exception ex) {
                Debug.WriteLine(ex);
            }

#if DEBUG
            if (m_rot != null) {
                m_rot.Dispose();
            }
#endif

            if (m_FilterGraph != null) {
                Marshal.ReleaseComObject(m_FilterGraph);
                m_FilterGraph = null;
            }
            GC.Collect();
        }

        /// <summary> sample callback, NOT USED. </summary>
        int ISampleGrabberCB.SampleCB(double SampleTime, IMediaSample pSample) {
            Marshal.ReleaseComObject(pSample);
            return 0;
        }

        /// <summary> buffer callback, COULD BE FROM FOREIGN THREAD. </summary>
        unsafe int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen) {

            // m_stride = number of bytes per row (along the width)
            Debug.Assert(IntPtr.Size == 4, "Change all instances of IntPtr.ToInt32 to .ToInt64");

            // Calculate bytes per pixel
            // Commented out because we automatically set this to 3. One for every color component!
            // int m_pixellen = m_stride / m_videoWidth;
            // Debug.WriteLine(m_pixellen);
            if (currentShot == null) {
                // Create new shot starting this frame
                currentShot = new Shot(m_Count);
            }
            if (detectionMethod == 0) {
                int result = PixelDifferenceSD(pBuffer);
                if (result != 0) return result;
            } else if (detectionMethod == 1){
                int result = MotionEstimationSD(pBuffer);
                if (result != 0) return result;
            } else if (detectionMethod == 2) {
                int result = GlobalHistogramSD(pBuffer);
                if (result != 0) return result;
            } else if (detectionMethod == 3) {
                int result = LocalHistogramSD(pBuffer);
                if (result != 0) return result;
            } else if (detectionMethod == 4) {
                int result = GeneralizedSD(pBuffer);
                if (result != 0) return result;
            }
            // Increment frame number.  Done this way, frame are zero indexed.
            m_Count++;

            return 0;
        }

        unsafe byte RedValue(Byte* b, int x, int y){return *(b + x * m_stride + 3 * y);}
        unsafe byte GreenValue(Byte* b, int x, int y){return *(b + x * m_stride + 3 * y + 1);}
        unsafe byte BlueValue(Byte* b, int x, int y){return *(b + x * m_stride + 3 * y + 2);}
        unsafe byte RedValue(Byte[] b, int x, int y) { return b[x * m_stride + 3 * y]; }
        unsafe byte GreenValue(Byte[] b, int x, int y) { return b[x * m_stride + 3 * y + 1]; }
        unsafe byte BlueValue(Byte[] b, int x, int y) { return b[x * m_stride + 3 * y + 2]; }

        unsafe double BlockDifference(Byte* a, Byte[] b, int xa, int ya, int xb, int yb, int size) {
            double diff = 0;
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    diff += Math.Abs(RedValue(a, xa+i, ya+j) - RedValue(b, xb+i, yb+j));
                    diff += Math.Abs(GreenValue(a, xa+i, ya+j) - GreenValue(b, xb+i, yb+j));
                    diff += Math.Abs(BlueValue(a, xa+i, ya+j) - BlueValue(b, xb+i, yb+j));
                }
            }
            return diff/(3*255);
        }

        unsafe int MotionEstimationSD(IntPtr pBuffer) {
            double threshold = param1;
            int blockSize = (int)param2;
            int windowSize = (int)param3;
            int xBlocks = m_videoHeight / blockSize;
            int yBlocks = m_videoWidth / blockSize;
            byte* b = (byte*)pBuffer;

            if (m_Count > 0) {
                double total = 0;
                for (int xBlock = 0; xBlock < xBlocks; xBlock++) {
                    for (int yBlock = 0; yBlock < yBlocks; yBlock++) {
                        int xa = xBlock * blockSize;
                        int ya = yBlock * blockSize;
                        double best = BlockDifference(b,previousFrame,xa,ya,xa,ya,blockSize);
                        for (int xb = Math.Max(0, xa - windowSize); xb+blockSize < m_videoHeight && xb < xa+windowSize; xb++) {
                            for (int yb = Math.Max(0, ya - windowSize); yb+blockSize < m_videoWidth && yb < ya + windowSize; yb++) {
                                double difference = BlockDifference(b, previousFrame, xa, ya, xb, yb, blockSize);
                                if (difference < best) {
                                    best = difference;
                                }
                            }
                        }
                        total += best;
                    }
                }
                //Console.WriteLine(total + " " + total / (m_videoHeight * m_videoWidth));
                if (total / (m_videoHeight * m_videoWidth) > threshold) {
                    int saved = SaveShot();
                    if (saved != 0) return saved;
                }
            }
            // Save copy of pBuffer to compare next frame.
            Marshal.Copy(pBuffer, previousFrame, 0, m_videoHeight * m_stride);

            return 0;
        }

        int SaveShot() {
            // end shot here
            currentShot.EndFrame = m_Count-1;

            // Wait for form to register current shot
            continueScan = false;
            bool stop = false;
            do {
                Thread.Sleep(10);
                if (disposing) return -1;
                lock (this) {
                    stop = continueScan;
                }
            } while (!stop);

            currentShot = new Shot(m_Count);
            return 0;
        }

        unsafe double GrayValue(Byte* a, int x, int y) {
            if (x < 0 || y < 0 || x >= m_videoHeight || y >= m_videoWidth) return 0;
            return ((double)*(a + x * m_stride + 3 * y))/255;
        }
        unsafe double GrayValue(Byte[] a, int x, int y) {
            if (x < 0 || y < 0 || x >= m_videoHeight || y >= m_videoWidth) return 0;
            return ((double)a[x * m_stride + 3 * y]) / 255;
        }
        unsafe double DX(Byte* a, int x, int y) { return (GrayValue(a, x - 1, y) + GrayValue(a, x - 1, y - 1) + GrayValue(a, x - 1, y + 1) - GrayValue(a, x + 1, y) - GrayValue(a, x + 1, y - 1) - GrayValue(a, x + 1, y + 1))/3; }
        unsafe double DY(Byte* a, int x, int y) { return (GrayValue(a, x, y - 1) + GrayValue(a, x - 1, y - 1) + GrayValue(a, x + 1, y - 1) - GrayValue(a, x, y + 1) - GrayValue(a, x + 1, y + 1) - GrayValue(a, x - 1, y + 1))/3; }
        unsafe double DX(Byte[] a, int x, int y) { return (GrayValue(a, x - 1, y) + GrayValue(a, x - 1, y - 1) + GrayValue(a, x - 1, y + 1) - GrayValue(a, x + 1, y) - GrayValue(a, x + 1, y - 1) - GrayValue(a, x + 1, y + 1))/3; }
        unsafe double DY(Byte[] a, int x, int y) { return (GrayValue(a, x, y - 1) + GrayValue(a, x - 1, y - 1) + GrayValue(a, x + 1, y - 1) - GrayValue(a, x, y + 1) - GrayValue(a, x + 1, y + 1) - GrayValue(a, x - 1, y + 1))/3; }

        unsafe double BlockEdgeDifference(Byte* a, Byte[] b, int xa, int ya, int xb, int yb, int size) {
            double diff = 0;
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    double dxa = DX(a, xa, ya);
                    double dya = DY(a, xa, ya);
                    double dxb = DX(b, xb, yb);
                    double dyb = DY(b, xb, yb);
                    double strengtha = Math.Sqrt(dxa * dxa + dya * dya);
                    double strengthb = Math.Sqrt(dxb * dxb + dyb * dyb);
                    double anglea = strengtha * Math.Atan2(dya, dxa) / 2 / Math.PI + 1;
                    double angleb = strengthb * Math.Atan2(dyb, dxb) / 2 / Math.PI + 1;
                    diff += Math.Abs(strengtha - strengthb);
                    diff += Math.Abs(anglea - angleb);
                }
            }
            return diff/2;
        }

        unsafe int GlobalHistogramSD(IntPtr pBuffer) {
            Byte* b = (byte*)pBuffer;
            // Calculate the histogram of the current frame
            for (int x = 0; x < m_videoHeight; x++) {
                //Loop over all pixels in this row
                for (int y = 0; y < m_videoWidth; y++) {
                    // Calculate the gray value (weighted average) and add 1 to the histogram column 
                    double R = RedValue(b, x, y);
                    double G = GreenValue(b, x, y);
                    double B = BlueValue(b, x, y);
                    double border = 256 / param2;
                    this.histogramNew1[(int)(R / border)][0]++;
                    this.histogramNew1[(int)(G / border)][1]++;
                    this.histogramNew1[(int)(B / border)][2]++;
                }
            }
            // If we are at the first frame, backup the histogram and do nothing
            if (m_Count == 0) {
                histogramOld1 = histogramNew1;
                histogramNew1 = new int[(int)param2][];
                for (int i = 0; i < (int)param2; i++) {
                    histogramNew1[i] = new int[3];
                }
            }
            // In any other frame, we calculate the histogram difference and check for a cut
            else {
                int D = 0;
                for (int i = 0; i < param2; i++) {
                    D += (int)Math.Abs(histogramOld1[i][0] - histogramNew1[i][0]);
                    D += (int)Math.Abs(histogramOld1[i][1] - histogramNew1[i][1]);
                    D += (int)Math.Abs(histogramOld1[i][2] - histogramNew1[i][2]);
                }
                histogramOld1 = histogramNew1;
                histogramNew1 = new int[(int)param2][];
                for (int i = 0; i < (int)param2; i++) {
                    histogramNew1[i] = new int[3];
                }
                if ((double)(D) / (double)(m_videoHeight * m_videoWidth) > param1) {
                    int saved = SaveShot();
                    if (saved != 0) return saved;
                }
            }
            return 0;
        }

        unsafe int GeneralizedSD(IntPtr pBuffer) {
            double threshold = param1;
            int blockSize = (int)param2;
            int windowSize = (int)param3;
            int xBlocks = m_videoHeight / blockSize;
            int yBlocks = m_videoWidth / blockSize;
            byte* b = (byte*)pBuffer;

            if (m_Count > 0) {
                double total = 0;
                for (int xBlock = 0; xBlock < xBlocks; xBlock++) {
                    for (int yBlock = 0; yBlock < yBlocks; yBlock++) {
                        int xa = xBlock * blockSize;
                        int ya = yBlock * blockSize;
                        double best = BlockEdgeDifference(b, previousFrame, xa, ya, xa, ya, blockSize);
                        for (int xb = Math.Max(0, xa - windowSize); xb + blockSize < m_videoHeight && xb < xa + windowSize; xb++) {
                            for (int yb = Math.Max(0, ya - windowSize); yb + blockSize < m_videoWidth && yb < ya + windowSize; yb++) {
                                double difference = BlockEdgeDifference(b, previousFrame, xa, ya, xb, yb, blockSize);
                                if (difference < best) {
                                    best = difference;
                                }
                            }
                        }
                        total += best;
                    }
                }
                Console.WriteLine(total + " " + total / (m_videoHeight * m_videoWidth) + " "+ threshold);
                if (total / (m_videoHeight * m_videoWidth) > threshold) {
                    int saved = SaveShot();
                    if (saved != 0) return saved;
                }
            }
            // Save copy of pBuffer to compare next frame.
            Marshal.Copy(pBuffer, previousFrame, 0, m_videoHeight * m_stride);

            return 0;
        }

        unsafe int LocalHistogramSD(IntPtr pBuffer) {
            Byte* b = (byte*)pBuffer;
            for (int x = 0; x < m_videoHeight; x++) {
                //Loop over all pixels in this row
                for (int y = 0; y < m_videoWidth; y++) {
                    double R = RedValue(b, x, y);
                    double G = GreenValue(b, x, y);
                    double B = BlueValue(b, x, y);
                    double border = 256 / param2;
                    double sqr = Math.Sqrt(param3);
                    // We divide the height by the number of splits
                    double borderx = m_videoHeight / sqr;
                    // We divide the width by the number of splits
                    double bordery = m_videoWidth / sqr;
                    // Index is defined by x and (y/3) level
                    int index = (int)(Math.Floor(x / borderx)) + (int)(sqr * Math.Floor((y / 3) / bordery));
                    this.histogramNew2[(int)(R / border)][0][index]++;
                    this.histogramNew2[(int)(G / border)][1][index]++;
                    this.histogramNew2[(int)(B / border)][2][index]++;
                }
            }
            if (m_Count == 0) {
                histogramOld2 = histogramNew2;
                histogramNew2 = new int[(int)param2][][];
                for (int i = 0; i < param2; i++) {
                    histogramNew2[i] = new int[3][];
                    for (int j = 0; j < 3; j++) {
                        histogramNew2[i][j] = new int[(int)param3];
                    }
                }
            } else {
                double D = 0;
                double[] DP = new double[(int)param3];
                for (int k = 0; k < (int)param3; k++) {
                    for (int i = 0; i < param2; i++) {
                        DP[k] += Math.Abs(histogramOld2[i][0][k] - histogramNew2[i][0][k]) * 0.2989;
                        DP[k] += Math.Abs(histogramOld2[i][1][k] - histogramNew2[i][1][k]) * 0.5870;
                        DP[k] += Math.Abs(histogramOld2[i][2][k] - histogramNew2[i][2][k]) * 0.1140;
                    }
                    D += DP[k];
                }
                histogramOld2 = histogramNew2;
                histogramNew2 = new int[(int)param2][][];
                for (int i = 0; i < (int)param2; i++) {
                    histogramNew2[i] = new int[3][];
                    for (int j = 0; j < 3; j++) {
                        histogramNew2[i][j] = new int[(int)param3];
                    }
                }
                if (((double)(D) * 300 / (double)(m_videoHeight * m_videoWidth)) > param1) {
                    int saved = SaveShot();
                    if (saved != 0) return saved;
                }
            }
            return 0;
        }

        unsafe int PixelDifferenceSD(IntPtr pBuffer) {
            Byte* b = (byte*)pBuffer;
            if (m_Count > 0) {
                // Count large pixel differences
                int largePixelDifs = 0;
                int x, y, R, G, B;
                for (x = 0; x < m_videoHeight; x++) {
                    // Loop over all pixels in this row
                    for (y = 0; y < m_videoWidth; y++) {
                        // Get current frame RGB - previous frame RGB
                        R = Math.Abs((int)RedValue(b, x, y) - (int)RedValue(previousFrame, x, y));
                        G = Math.Abs((int)GreenValue(b, x, y) - (int)GreenValue(previousFrame, x, y));
                        B = Math.Abs((int)BlueValue(b, x, y) - (int)BlueValue(previousFrame, x, y));
                        if ((R+G+B) > param1) largePixelDifs++;
                    }
                }
                if ((double)(largePixelDifs) / (double)(m_videoHeight * m_videoWidth) > param2) {
                    int saved = SaveShot();
                    if (saved != 0) return saved;
                }
            }
            // Save copy of pBuffer to compare next frame.
            Marshal.Copy(pBuffer, previousFrame, 0, m_videoHeight * m_stride);
            return 0;
        }
    }
}