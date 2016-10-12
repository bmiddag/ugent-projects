/****************************************************************************
While the underlying libraries are covered by LGPL, this sample is released 
as public domain.  It is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
or FITNESS FOR A PARTICULAR PURPOSE.  
*****************************************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data;
using System.Xml;
using System.Drawing.Imaging;
using System.Threading;

namespace DirectShow {
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
    public class MainForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnScreenshot;
        private Button btnDetect;
        private Button btnOpenFile;
        private Label lblFilename;
        private String filename = "";
        private TrackBar videoTrackBar;
        private Panel panel2;
        private Label lblNumber1;
        private Label lblWelcome;
        private Label lblNumber2;
        private Label lblDurationNum;
        private Label lblDurationTxt;
        private Label lblCurrentTimeNum;
        private Label lblCurrentTimeTxt;
        private System.Windows.Forms.Timer playbackTimer;
        private Label lblCurrentFrameTxt;
        private Label lblCurrentFrameNumber;
        private Label lblTotalFramesTxt;
        private Label lblTotalFramesNum;
        private Label lblNumber4;
        private Button btnExportXML;
        private Label lblNumber3;
        private DataGridView dgvShots;
        private Label lblShotsDetected;
        private Label lblFilter;
        private TextBox tbFilter;
        private IContainer components;
        private DataGridViewTextBoxColumn colBeginFrame;
        private DataGridViewTextBoxColumn colEndFrame;
        private DataGridViewTextBoxColumn colKeywords;
        private Label lvlDoubleClick;
        private ArrayList shots;
        private int shotDetectionMethod;
        private double[] param = new double[3];
        private String[] paramNames = new String[3];

        public void addShots(ArrayList shots, int shotDetectionMethod, double[] param, String[] paramNames){
            this.shots = shots;
            int amountShots = shots.Count;
            dgvShots.Rows.Clear();

            if (amountShots == 1) lblShotsDetected.Text = "1 shot";
            else lblShotsDetected.Text = String.Format("{0} shots", amountShots);

            for (int i = 0; i < amountShots; i++) {
                Shot shot = (Shot)shots[i];
                dgvShots.Rows.Add(shot.StartFrame, shot.EndFrame, "");
            }
            this.shotDetectionMethod = shotDetectionMethod;
            for (int i = 0; i < 3; i++) {
                this.param[i] = param[i];
                this.paramNames[i] = paramNames[i];
            }
            tbFilter.Text = "";

            if (amountShots < 1) {
                btnExportXML.Enabled = false;
            } else btnExportXML.Enabled = true;
        }

		public MainForm() {
			InitializeComponent();
            // Initialize shots ArrayList
            shots = new ArrayList();
            // Add line numbers in shots table
            this.dgvShots.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvShots_RowPostPaint);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
            // Make sure to release the DxPlay object to avoid hanging
            if (m_play != null) {
                m_play.Dispose();
            }
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.btnStart = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnScreenshot = new System.Windows.Forms.Button();
            this.btnDetect = new System.Windows.Forms.Button();
            this.lblFilename = new System.Windows.Forms.Label();
            this.videoTrackBar = new System.Windows.Forms.TrackBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lvlDoubleClick = new System.Windows.Forms.Label();
            this.lblFilter = new System.Windows.Forms.Label();
            this.tbFilter = new System.Windows.Forms.TextBox();
            this.lblShotsDetected = new System.Windows.Forms.Label();
            this.lblNumber4 = new System.Windows.Forms.Label();
            this.btnExportXML = new System.Windows.Forms.Button();
            this.lblNumber3 = new System.Windows.Forms.Label();
            this.dgvShots = new System.Windows.Forms.DataGridView();
            this.colBeginFrame = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEndFrame = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colKeywords = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblNumber2 = new System.Windows.Forms.Label();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblNumber1 = new System.Windows.Forms.Label();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.lblDurationNum = new System.Windows.Forms.Label();
            this.lblDurationTxt = new System.Windows.Forms.Label();
            this.lblCurrentTimeNum = new System.Windows.Forms.Label();
            this.lblCurrentTimeTxt = new System.Windows.Forms.Label();
            this.playbackTimer = new System.Windows.Forms.Timer(this.components);
            this.lblCurrentFrameTxt = new System.Windows.Forms.Label();
            this.lblCurrentFrameNumber = new System.Windows.Forms.Label();
            this.lblTotalFramesTxt = new System.Windows.Forms.Label();
            this.lblTotalFramesNum = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.videoTrackBar)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvShots)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(10, 423);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(64, 39);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(678, 353);
            this.panel1.TabIndex = 10;
            // 
            // btnPause
            // 
            this.btnPause.Enabled = false;
            this.btnPause.Location = new System.Drawing.Point(80, 423);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(66, 39);
            this.btnPause.TabIndex = 11;
            this.btnPause.Text = "Pause";
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnScreenshot
            // 
            this.btnScreenshot.Enabled = false;
            this.btnScreenshot.Location = new System.Drawing.Point(152, 423);
            this.btnScreenshot.Name = "btnScreenshot";
            this.btnScreenshot.Size = new System.Drawing.Size(96, 39);
            this.btnScreenshot.TabIndex = 12;
            this.btnScreenshot.Text = "Screenshot";
            this.btnScreenshot.Click += new System.EventHandler(this.btnScreenshot_Click);
            // 
            // btnDetect
            // 
            this.btnDetect.BackColor = System.Drawing.SystemColors.Control;
            this.btnDetect.Enabled = false;
            this.btnDetect.Location = new System.Drawing.Point(40, 98);
            this.btnDetect.Name = "btnDetect";
            this.btnDetect.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnDetect.Size = new System.Drawing.Size(112, 39);
            this.btnDetect.TabIndex = 13;
            this.btnDetect.Text = "Detect Shots";
            this.btnDetect.UseVisualStyleBackColor = false;
            this.btnDetect.Click += new System.EventHandler(this.btnDetect_Click);
            // 
            // lblFilename
            // 
            this.lblFilename.AutoSize = true;
            this.lblFilename.Location = new System.Drawing.Point(134, 58);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(104, 17);
            this.lblFilename.TabIndex = 15;
            this.lblFilename.Text = "No File Loaded";
            // 
            // videoTrackBar
            // 
            this.videoTrackBar.Enabled = false;
            this.videoTrackBar.LargeChange = 50;
            this.videoTrackBar.Location = new System.Drawing.Point(10, 372);
            this.videoTrackBar.Maximum = 677;
            this.videoTrackBar.Name = "videoTrackBar";
            this.videoTrackBar.Size = new System.Drawing.Size(676, 56);
            this.videoTrackBar.TabIndex = 16;
            this.videoTrackBar.TickFrequency = 50;
            this.videoTrackBar.Scroll += new System.EventHandler(this.videoTrackBar_Scroll);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.lvlDoubleClick);
            this.panel2.Controls.Add(this.lblFilter);
            this.panel2.Controls.Add(this.tbFilter);
            this.panel2.Controls.Add(this.lblShotsDetected);
            this.panel2.Controls.Add(this.lblNumber4);
            this.panel2.Controls.Add(this.btnExportXML);
            this.panel2.Controls.Add(this.lblNumber3);
            this.panel2.Controls.Add(this.dgvShots);
            this.panel2.Controls.Add(this.lblNumber2);
            this.panel2.Controls.Add(this.lblWelcome);
            this.panel2.Controls.Add(this.lblNumber1);
            this.panel2.Controls.Add(this.btnOpenFile);
            this.panel2.Controls.Add(this.lblFilename);
            this.panel2.Controls.Add(this.btnDetect);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(702, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(281, 468);
            this.panel2.TabIndex = 17;
            // 
            // lvlDoubleClick
            // 
            this.lvlDoubleClick.AutoSize = true;
            this.lvlDoubleClick.Location = new System.Drawing.Point(40, 150);
            this.lvlDoubleClick.Name = "lvlDoubleClick";
            this.lvlDoubleClick.Size = new System.Drawing.Size(209, 34);
            this.lvlDoubleClick.TabIndex = 27;
            this.lvlDoubleClick.Text = "Double click frame to play,\r\nor double click keywords to edit.";
            // 
            // lblFilter
            // 
            this.lblFilter.AutoSize = true;
            this.lblFilter.Location = new System.Drawing.Point(40, 366);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(83, 17);
            this.lblFilter.TabIndex = 26;
            this.lblFilter.Text = "Filter Shots:";
            // 
            // tbFilter
            // 
            this.tbFilter.Location = new System.Drawing.Point(40, 387);
            this.tbFilter.Name = "tbFilter";
            this.tbFilter.Size = new System.Drawing.Size(223, 22);
            this.tbFilter.TabIndex = 25;
            this.tbFilter.TextChanged += new System.EventHandler(this.tbFilter_TextChanged);
            // 
            // lblShotsDetected
            // 
            this.lblShotsDetected.AutoSize = true;
            this.lblShotsDetected.Location = new System.Drawing.Point(170, 108);
            this.lblShotsDetected.Name = "lblShotsDetected";
            this.lblShotsDetected.Size = new System.Drawing.Size(54, 17);
            this.lblShotsDetected.TabIndex = 24;
            this.lblShotsDetected.Text = "0 shots";
            // 
            // lblNumber4
            // 
            this.lblNumber4.AutoSize = true;
            this.lblNumber4.Location = new System.Drawing.Point(8, 425);
            this.lblNumber4.Name = "lblNumber4";
            this.lblNumber4.Size = new System.Drawing.Size(20, 17);
            this.lblNumber4.TabIndex = 23;
            this.lblNumber4.Text = "4.";
            // 
            // btnExportXML
            // 
            this.btnExportXML.BackColor = System.Drawing.SystemColors.Control;
            this.btnExportXML.Enabled = false;
            this.btnExportXML.Location = new System.Drawing.Point(40, 414);
            this.btnExportXML.Name = "btnExportXML";
            this.btnExportXML.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnExportXML.Size = new System.Drawing.Size(223, 39);
            this.btnExportXML.TabIndex = 21;
            this.btnExportXML.Text = "Export XML and images";
            this.btnExportXML.UseVisualStyleBackColor = false;
            this.btnExportXML.Click += new System.EventHandler(this.btnExportXML_Click);
            // 
            // lblNumber3
            // 
            this.lblNumber3.AutoSize = true;
            this.lblNumber3.Location = new System.Drawing.Point(8, 150);
            this.lblNumber3.Name = "lblNumber3";
            this.lblNumber3.Size = new System.Drawing.Size(20, 17);
            this.lblNumber3.TabIndex = 20;
            this.lblNumber3.Text = "3.";
            // 
            // dgvShots
            // 
            this.dgvShots.AllowUserToAddRows = false;
            this.dgvShots.AllowUserToDeleteRows = false;
            this.dgvShots.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvShots.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colBeginFrame,
            this.colEndFrame,
            this.colKeywords});
            this.dgvShots.Location = new System.Drawing.Point(40, 187);
            this.dgvShots.Name = "dgvShots";
            this.dgvShots.RowTemplate.Height = 24;
            this.dgvShots.Size = new System.Drawing.Size(223, 176);
            this.dgvShots.TabIndex = 19;
            this.dgvShots.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvShots_CellDoubleClick);
            this.dgvShots.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvShots_CellEndEdit);
            // 
            // colBeginFrame
            // 
            this.colBeginFrame.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colBeginFrame.HeaderText = "From";
            this.colBeginFrame.MaxInputLength = 7;
            this.colBeginFrame.Name = "colBeginFrame";
            this.colBeginFrame.ReadOnly = true;
            this.colBeginFrame.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colBeginFrame.Width = 46;
            // 
            // colEndFrame
            // 
            this.colEndFrame.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colEndFrame.HeaderText = "To";
            this.colEndFrame.MaxInputLength = 7;
            this.colEndFrame.Name = "colEndFrame";
            this.colEndFrame.ReadOnly = true;
            this.colEndFrame.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colEndFrame.Width = 31;
            // 
            // colKeywords
            // 
            this.colKeywords.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colKeywords.HeaderText = "Keywords";
            this.colKeywords.MaxInputLength = 10000;
            this.colKeywords.Name = "colKeywords";
            this.colKeywords.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // lblNumber2
            // 
            this.lblNumber2.AutoSize = true;
            this.lblNumber2.Location = new System.Drawing.Point(8, 108);
            this.lblNumber2.Name = "lblNumber2";
            this.lblNumber2.Size = new System.Drawing.Size(20, 17);
            this.lblNumber2.TabIndex = 18;
            this.lblNumber2.Text = "2.";
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Location = new System.Drawing.Point(37, 21);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(176, 17);
            this.lblWelcome.TabIndex = 17;
            this.lblWelcome.Text = "Welcome to Shot Detector!";
            // 
            // lblNumber1
            // 
            this.lblNumber1.AutoSize = true;
            this.lblNumber1.Location = new System.Drawing.Point(8, 58);
            this.lblNumber1.Name = "lblNumber1";
            this.lblNumber1.Size = new System.Drawing.Size(20, 17);
            this.lblNumber1.TabIndex = 16;
            this.lblNumber1.Text = "1.";
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.BackColor = System.Drawing.SystemColors.Control;
            this.btnOpenFile.Image = global::DirectShow.Properties.Resources.open;
            this.btnOpenFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenFile.Location = new System.Drawing.Point(40, 46);
            this.btnOpenFile.Margin = new System.Windows.Forms.Padding(8);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(82, 41);
            this.btnOpenFile.TabIndex = 14;
            this.btnOpenFile.Text = "Open";
            this.btnOpenFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOpenFile.UseVisualStyleBackColor = false;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // lblDurationNum
            // 
            this.lblDurationNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDurationNum.Location = new System.Drawing.Point(622, 440);
            this.lblDurationNum.Name = "lblDurationNum";
            this.lblDurationNum.Size = new System.Drawing.Size(72, 17);
            this.lblDurationNum.TabIndex = 18;
            this.lblDurationNum.Text = "0:00.000";
            this.lblDurationNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDurationTxt
            // 
            this.lblDurationTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDurationTxt.Location = new System.Drawing.Point(546, 440);
            this.lblDurationTxt.Name = "lblDurationTxt";
            this.lblDurationTxt.Size = new System.Drawing.Size(70, 17);
            this.lblDurationTxt.TabIndex = 19;
            this.lblDurationTxt.Text = "Duration:";
            this.lblDurationTxt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCurrentTimeNum
            // 
            this.lblCurrentTimeNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentTimeNum.Location = new System.Drawing.Point(622, 418);
            this.lblCurrentTimeNum.Name = "lblCurrentTimeNum";
            this.lblCurrentTimeNum.Size = new System.Drawing.Size(72, 16);
            this.lblCurrentTimeNum.TabIndex = 20;
            this.lblCurrentTimeNum.Text = "0:00.000";
            this.lblCurrentTimeNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCurrentTimeTxt
            // 
            this.lblCurrentTimeTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentTimeTxt.Location = new System.Drawing.Point(516, 418);
            this.lblCurrentTimeTxt.Name = "lblCurrentTimeTxt";
            this.lblCurrentTimeTxt.Size = new System.Drawing.Size(100, 16);
            this.lblCurrentTimeTxt.TabIndex = 21;
            this.lblCurrentTimeTxt.Text = "Current Time:";
            this.lblCurrentTimeTxt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // playbackTimer
            // 
            this.playbackTimer.Interval = 50;
            this.playbackTimer.Tick += new System.EventHandler(this.playbackTimer_Tick);
            // 
            // lblCurrentFrameTxt
            // 
            this.lblCurrentFrameTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentFrameTxt.AutoSize = true;
            this.lblCurrentFrameTxt.Location = new System.Drawing.Point(336, 418);
            this.lblCurrentFrameTxt.Name = "lblCurrentFrameTxt";
            this.lblCurrentFrameTxt.Size = new System.Drawing.Size(103, 17);
            this.lblCurrentFrameTxt.TabIndex = 22;
            this.lblCurrentFrameTxt.Text = "Current Frame:";
            this.lblCurrentFrameTxt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCurrentFrameNumber
            // 
            this.lblCurrentFrameNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentFrameNumber.Location = new System.Drawing.Point(446, 418);
            this.lblCurrentFrameNumber.Name = "lblCurrentFrameNumber";
            this.lblCurrentFrameNumber.Size = new System.Drawing.Size(48, 16);
            this.lblCurrentFrameNumber.TabIndex = 23;
            this.lblCurrentFrameNumber.Text = "0";
            this.lblCurrentFrameNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTotalFramesTxt
            // 
            this.lblTotalFramesTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalFramesTxt.Location = new System.Drawing.Point(340, 440);
            this.lblTotalFramesTxt.Name = "lblTotalFramesTxt";
            this.lblTotalFramesTxt.Size = new System.Drawing.Size(100, 17);
            this.lblTotalFramesTxt.TabIndex = 24;
            this.lblTotalFramesTxt.Text = "Total Frames:";
            this.lblTotalFramesTxt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTotalFramesNum
            // 
            this.lblTotalFramesNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalFramesNum.Location = new System.Drawing.Point(446, 440);
            this.lblTotalFramesNum.Name = "lblTotalFramesNum";
            this.lblTotalFramesNum.Size = new System.Drawing.Size(48, 17);
            this.lblTotalFramesNum.TabIndex = 25;
            this.lblTotalFramesNum.Text = "0";
            this.lblTotalFramesNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(983, 468);
            this.Controls.Add(this.lblTotalFramesNum);
            this.Controls.Add(this.lblTotalFramesTxt);
            this.Controls.Add(this.lblCurrentFrameNumber);
            this.Controls.Add(this.lblCurrentFrameTxt);
            this.Controls.Add(this.lblCurrentTimeTxt);
            this.Controls.Add(this.lblCurrentTimeNum);
            this.Controls.Add(this.lblDurationTxt);
            this.Controls.Add(this.lblDurationNum);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnScreenshot);
            this.Controls.Add(this.videoTrackBar);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Shot Detector";
            ((System.ComponentModel.ISupportInitialize)(this.videoTrackBar)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvShots)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.Run(new MainForm());
		}

        private void dgvShots_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e) {
            using (SolidBrush b = new SolidBrush(dgvShots.RowHeadersDefaultCellStyle.ForeColor)) {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4);
            }
        }


        enum State {
            Uninit,
            Stopped,
            Paused,
            Playing
        }
        State m_State = State.Uninit;
        DxPlayer m_play = null;

        private void btnStart_Click(object sender, System.EventArgs e) {
            if (filename.Equals("")) {
                btnOpenFile_Click(null, null);
                if(filename.Equals("")) MessageBox.Show("No file selected", "Play Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // If we were stopped, start
            if (m_State == State.Stopped) {
                btnStart.Text = "Stop";
                m_play.Start();
                btnScreenshot.Enabled = true;
                btnPause.Enabled = true;
                btnOpenFile.Enabled = false;
                playbackTimer.Start();
                videoTrackBar.Enabled = true;
                m_State = State.Playing;
            } else if (m_State == State.Playing || m_State == State.Paused) {
                playbackTimer.Stop();
                videoTrackBar.Enabled = false;
                resetCurrentTime();
                m_play.Stop();
                btnPause.Enabled = false;
                btnOpenFile.Enabled = true;
                btnStart.Text = "Start";
                btnPause.Text = "Pause";
                m_State = State.Stopped;
            }
        }

        private void btnPause_Click(object sender, System.EventArgs e) {
            // If we are playing, pause, else, start
            if (m_State == State.Playing) {
                playbackTimer.Stop();
                m_play.Pause();
                btnPause.Text = "Resume";
                m_State = State.Paused;
            } else {
                m_play.Start();
                btnPause.Text = "Pause";
                m_State = State.Playing;
                playbackTimer.Start();
            }
        }

        private void btnScreenshot_Click(object sender, System.EventArgs e) {
            if (m_play == null || m_State == State.Stopped || m_State == State.Uninit
                || filename == null || filename.Equals("")) return;
            // Get current open file to get the directory
            String directory = System.IO.Path.GetDirectoryName(filename);
            String file = System.IO.Path.GetFileNameWithoutExtension(filename);
            takeScreenshot(directory + "\\" + file + "_screenshot_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".png");
        }

        private void takeScreenshot(String filename) {
            // Grab a copy of the current bitmap.  Graph can be paused or playing
            IntPtr ip = m_play.SnapShot();
            try {
                // Turn the raw pixels into a Bitmap
                Bitmap bmp = m_play.IPToBmp(ip);

                // Save the bitmap to a file
                bmp.Save(@filename, ImageFormat.Png);
            } finally {
                // Free the raw pixels
                Marshal.FreeCoTaskMem(ip);
            }
        }

        // Called when the video is finished playing
        private void m_play_StopPlay(Object sender) {
            // This isn't the right way to do this, but heck, it's only a sample
            CheckForIllegalCrossThreadCalls = false;

            btnPause.Enabled = false;
            btnOpenFile.Enabled = true;
            playbackTimer.Stop();
            videoTrackBar.Enabled = false;
            resetCurrentTime();

            btnStart.Text = "Start";
            btnPause.Text = "Pause";

            CheckForIllegalCrossThreadCalls = true;

            m_State = State.Stopped;

            // Rewind clip to beginning to allow DxPlay.Start to work again.
            m_play.Rewind();
        }

        private void btnOpenFile_Click(object sender, EventArgs e) {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "AVI Video Files (.avi)|*.avi|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = (openFileDialog1.ShowDialog() == DialogResult.OK);

            // Process input if the user clicked OK.
            if (userClickedOK == true) {
                // If necessary, close the old file
                if (m_State == State.Stopped) {
                    // Did the filename change?
                    filename = openFileDialog1.FileName;
                    if (filename != m_play.FileName) {
                        // File name changed, close the old file
                        m_play.Dispose();
                        m_play = null;
                        m_State = State.Uninit;
                        btnScreenshot.Enabled = false;
                        btnDetect.Enabled = false;
                        filename = "";
                        lblFilename.Text = "No file loaded";
                        dgvShots.Rows.Clear();
                        shots.Clear();
                        lblShotsDetected.Text = "0 shots";
                        lblDurationNum.Text = "0:00.000";
                        lblTotalFramesNum.Text = "0";
                        resetCurrentTime();
                        btnExportXML.Enabled = false;
                    }
                }

                // If we have no file open
                if (m_play == null) {
                    try {
                        filename = openFileDialog1.FileName;
                        lblFilename.Text = System.IO.Path.GetFileName(filename);

                        // Open the file, provide a handle to play it in
                        m_play = new DxPlayer(panel1, filename);

                        // Let us know when the file is finished playing
                        m_play.StopPlay += new DxPlayer.DxPlayEvent(m_play_StopPlay);
                        m_State = State.Stopped;
                        btnDetect.Enabled = true;
                        btnExportXML.Enabled = false;

                        double duration = m_play.Duration;
                        lblDurationNum.Text = Math.Floor(duration / 60.0) + ":" +
                            string.Format("{0:00.000}", duration % 60.0);
                        lblTotalFramesNum.Text = m_play.TotalFrames.ToString();

                        btnStart_Click(null, null);

                    } catch (COMException ce) {
                        MessageBox.Show("Failed to open file: " + ce.Message, "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        filename = "";
                        lblFilename.Text = "No file loaded";
                        lblDurationNum.Text = "0:00.000";
                        lblTotalFramesNum.Text = "0";
                        btnDetect.Enabled = false;
                        btnExportXML.Enabled = false;
                    }
                }
            }
        }

        private void playbackTimer_Tick(object sender, EventArgs e) {
            if (m_play == null || m_State == State.Stopped || m_State == State.Uninit) {
                playbackTimer.Stop();
                resetCurrentTime();
            } else if (m_State == State.Playing) {
                double currentTime = m_play.CurrentTime;
                lblCurrentTimeNum.Text = Math.Floor(currentTime / 60.0) + ":" +
                    string.Format("{0:00.000}", currentTime % 60.0);
                lblCurrentFrameNumber.Text = m_play.CurrentFrameFast.ToString();
                videoTrackBar.Value = (int)Math.Floor((currentTime / m_play.Duration) * videoTrackBar.Maximum);
            }
        }

        private void resetCurrentTime() {
            lblCurrentTimeNum.Text = "0:00.000";
            lblCurrentFrameNumber.Text = "0";
            videoTrackBar.Value = 0;
        }

        private void videoTrackBar_Scroll(object sender, EventArgs e) {
            if (m_play == null || m_State == State.Uninit || m_State == State.Stopped) {
                resetCurrentTime();
                videoTrackBar.Enabled = false;
            } else if(m_State == State.Paused || m_State == State.Playing) {
                m_play.CurrentTime = ((double)videoTrackBar.Value / (double)videoTrackBar.Maximum) * (double)m_play.Duration;
                double currentTime = m_play.CurrentTime;
                lblCurrentTimeNum.Text = Math.Floor(currentTime / 60.0) + ":" +
                    string.Format("{0:00.000}", currentTime % 60.0);
                lblCurrentFrameNumber.Text = m_play.CurrentFrameFast.ToString();
            }
        }

        private void btnDetect_Click(object sender, EventArgs e) {
            if (m_play != null) {
                ShotDetectionWizard form2 = new ShotDetectionWizard(filename, m_play.TotalFrames, this);
                form2.Show();
            } else {
                btnDetect.Enabled = false;
            }
        }

        private void dgvShots_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            // Add keywords to shot
            if (e.ColumnIndex != dgvShots.ColumnCount - 1) return;
            DataGridViewCell cell = dgvShots.Rows[e.RowIndex].Cells[e.ColumnIndex];
            int startFrame = int.Parse(dgvShots.Rows[e.RowIndex].Cells[0].Value.ToString());
            Shot shot = null;
            for (int i = 0; i < shots.Count; i++) {
                Shot curShot = (Shot)shots[i];
                if (curShot.StartFrame.Equals(startFrame)) {
                    shot = curShot;
                    break;
                }
            }
            if (shot == null) return;
            String[] value = cell.Value.ToString().Split(' ');
            shot.emptyKeywords();
            cell.Value = "";
            for (int i = 0; i < value.Length; i++) {
                if (!value[i].Equals("")) {
                    shot.addKeyword(value[i]);
                    if (!cell.Value.Equals("")) cell.Value += " ";
                    cell.Value += value[i];
                }
            }
        }

        private void tbFilter_TextChanged(object sender, EventArgs e) {
            // Filter shots on keywords
            String[] value = tbFilter.Text.Split(' ');
            ArrayList keyShots = getShotsWithKeywords(value);
            int shotsCount = keyShots.Count;
            dgvShots.Rows.Clear();
            for (int i = 0; i < shotsCount; i++) {
                Shot shot = (Shot)keyShots[i];
                String strKeywords = "";
                ArrayList keywords = shot.Keywords;
                for(int j = 0; j < keywords.Count; j++) {
                    if (!strKeywords.Equals("")) strKeywords += " ";
                    strKeywords += keywords[j];
                }
                dgvShots.Rows.Add(shot.StartFrame, shot.EndFrame, strKeywords);
            }
            if (shotsCount < 1) {
                btnExportXML.Enabled = false;
            } else btnExportXML.Enabled = true;

            if (shotsCount == 1) {
                this.lblShotsDetected.Text = "1 shot";
            } else {
                this.lblShotsDetected.Text = String.Format("{0} shots", shotsCount);
            }
        }

        private ArrayList getShotsWithKeywords(String[] value) {
            int shotsCount = shots.Count;
            ArrayList keyShots = new ArrayList();
            for (int i = 0; i < shotsCount; i++) {
                Shot shot = (Shot)shots[i];
                String strKeywords = "";
                ArrayList keywords = shot.Keywords;
                for (int j = 0; j < keywords.Count; j++) {
                    if (!strKeywords.Equals("")) strKeywords += " ";
                    strKeywords += keywords[j];
                }
                bool containsEverything = true;
                for (int j = 0; j < value.Length; j++) {
                    if (!value[j].Equals("")) {
                        containsEverything = containsEverything && strKeywords.Contains(value[j]);
                        if (!containsEverything) break;
                    }
                }
                if (containsEverything) keyShots.Add(shot);
            }
            return keyShots;
        }

        private void dgvShots_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == dgvShots.ColumnCount - 1) return;
            if (e.RowIndex == -1) return; // We don't want to do anything if header is clicked
            DataGridViewCell cell;
            if (e.ColumnIndex == -1) cell = dgvShots.Rows[e.RowIndex].Cells[0];
            else cell = dgvShots.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (m_play == null || m_State == State.Uninit) {
                resetCurrentTime();
                videoTrackBar.Enabled = false;
            } else if (m_State == State.Stopped) {
                btnStart_Click(null, null);
                m_play.CurrentFrame = int.Parse(cell.Value.ToString());
                lblCurrentFrameNumber.Text = cell.Value.ToString();
                double currentTime = m_play.CurrentTime;
                videoTrackBar.Value = (int)Math.Floor((currentTime / m_play.Duration) * videoTrackBar.Maximum);
                lblCurrentTimeNum.Text = Math.Floor(currentTime / 60.0) + ":" +
                    string.Format("{0:00.000}", currentTime % 60.0);
            } else if (m_State == State.Paused || m_State == State.Playing) {
                m_play.CurrentFrame = int.Parse(cell.Value.ToString());
                lblCurrentFrameNumber.Text = cell.Value.ToString();
                double currentTime = m_play.CurrentTime;
                videoTrackBar.Value = (int)Math.Floor((currentTime / m_play.Duration) * videoTrackBar.Maximum);
                lblCurrentTimeNum.Text = Math.Floor(currentTime / 60.0) + ":" +
                    string.Format("{0:00.000}", currentTime % 60.0);
            }

        }

        private void btnExportXML_Click(object sender, EventArgs e) {
            if (shots.Count < 1) {
                btnExportXML.Enabled = false;
                return;
            }
            // Create an instance of the open file dialog box.
            SaveFileDialog saveDialog = new SaveFileDialog();

            // Set filter options and filter index.
            saveDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(filename) + "_shots.xml";
            saveDialog.Filter = "XML Files (.xml)|*.xml|All Files (*.*)|*.*";
            saveDialog.FilterIndex = 1;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = (saveDialog.ShowDialog() == DialogResult.OK);
            if (userClickedOK == true) {
                ArrayList keyShots;
                XmlWriterSettings settings = new XmlWriterSettings { Indent = true, IndentChars = "\t", NewLineOnAttributes = false };
                using (XmlWriter writer = XmlWriter.Create(saveDialog.FileName, settings)) {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("ShotDetection");
                    writer.WriteAttributeString("file", System.IO.Path.GetFileName(filename));
                    writer.WriteStartElement("method");
                    writer.WriteAttributeString("nr", shotDetectionMethod.ToString());
                    for (int i = 0; i < 3; i++) {
                        if (!paramNames[i].Equals("")) {
                            writer.WriteStartElement("param" + (i+1)); // 1-indexed parameters
                            writer.WriteAttributeString("name", paramNames[i]);
                            writer.WriteString(param[i].ToString());
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteStartElement("shots");
                    // Filter shots
                    String[] value = tbFilter.Text.Split(' ');
                    keyShots = getShotsWithKeywords(value);
                    int keyShotCount = keyShots.Count;
                    for(int i = 0; i < keyShotCount; i++) {
                        Shot shot = (Shot)keyShots[i];
                        writer.WriteStartElement("shot");
                        writer.WriteAttributeString("nr", (i+1).ToString());
                        writer.WriteAttributeString("start", shot.StartFrame.ToString());
                        writer.WriteAttributeString("end", shot.EndFrame.ToString());
                        ArrayList keywords = shot.Keywords;
                        for (int j = 0; j < keywords.Count; j++) {
                            writer.WriteElementString("keyword", (String)keywords[j]);
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                if (keyShots.Count > 0) {
                    DialogResult confirmResult = MessageBox.Show("Save representative frames for each shot?\nImages will be saved in the same folder.",
                        "Export images", MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.Yes) {
                        exportImages(keyShots, System.IO.Path.GetDirectoryName(saveDialog.FileName));
                    }
                }
            }
        }

        private void exportImages(ArrayList shotsToExport, String directory) {
            if (shotsToExport.Count < 1) {
                return;
            }
            // Show loading window so user knows stuff is happening
            LoadingForm loading = new LoadingForm();
            loading.Show(this);

            // Disable buttons
            btnStart.Text = "Stop";
            m_play.Start();
            Application.DoEvents();
            Thread.Sleep(250);
            btnOpenFile.Enabled = false;
            if(playbackTimer.Enabled) playbackTimer.Stop();
            videoTrackBar.Enabled = false;
            m_State = State.Paused;
            btnPause.Enabled = false;
            btnStart.Enabled = false;
            m_play.Pause();
            Application.DoEvents();
            Thread.Sleep(250);
            // Do the actual image exporting
            int shotCount = shotsToExport.Count;
            String namePrefix = directory + "\\" + System.IO.Path.GetFileNameWithoutExtension(filename) + "_shot";
            for (int i = 0; i < shotCount; i++) {
                Shot shot = (Shot)shotsToExport[i];
                int start = shot.StartFrame;
                int end = shot.EndFrame;

                m_play.CurrentFrame = (start + end) / 2;
                Application.DoEvents();
                Thread.Sleep(250);

                takeScreenshot(namePrefix + (i+1) + ".png");
            }

            // Re-enable buttons
            btnStart.Enabled = true;
            btnStart_Click(this, EventArgs.Empty);

            // Close that pesky loading window now
            loading.Close();
        }

    }
}
