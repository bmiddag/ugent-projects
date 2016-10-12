using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DirectShow {
    public partial class ShotDetectionWizard : Form {

        String fileName = null;
        public int totalFrames = 0;
        bool shotDetectionDone = false;
        bool buttonClicked = false;
        bool finishClicked = false;
        private ArrayList shots;
        private Stopwatch stopwatch;
        private MainForm original;
        double param1 = 0.0, param2 = 0.0, param3 = 0.0;
        int shotDetectionMethod;

        public ShotDetectionWizard(String filename, int total, MainForm original) {
            InitializeComponent();
            shots = new ArrayList();
            this.fileName = filename;
            this.totalFrames = total;
            this.original = original;

            // Show row numbers
            this.dgvShots.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvShots_RowPostPaint);
        }

        private void dgvShots_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e) {
            using (SolidBrush b = new SolidBrush(dgvShots.RowHeadersDefaultCellStyle.ForeColor)) {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4);
            }
        }

        private void btnNext_Click(object sender, EventArgs e) {
            buttonClicked = true;
            tabControl.SelectTab(Math.Min(tabControl.TabCount - 1, tabControl.SelectedIndex + 1));
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e) {
            if (tabControl.SelectedIndex == tabControl.TabCount - 1) {
                btnNext.Enabled = false;
                btnFinish.Enabled = true;
                btnBack.Enabled = false;
            } else {
                btnNext.Enabled = true;
                btnFinish.Enabled = false;
                if (tabControl.SelectedIndex == 0) {
                    btnBack.Enabled = false;
                } else btnBack.Enabled = true;
            }
            if (tabControl.SelectedIndex == 1)
            {
                parameter3.Visible = true;
                parameterBox3.Visible = true;
                lblPDescr3.Visible = true;
                parameterBox3.Enabled = true;

                // When tab has changed, change parameters on tab 1
                if (radioPixel.Checked) {
                    parameter1.Text = "Pixel Threshold";
                    lblPDescr1.Text = "If the sum of all color differences is above this threshold,\nthis pixel is significantly different. "
                        + "Range: 0 - 768";
                    parameter2.Text = "Frame Threshold";
                    lblPDescr2.Text = "A new shot starts here if more than this fraction of pixels is\nsignificantly different. "
                        + "Range: 0.0 - 1.0";
                    parameterBox1.Text = "220";
                    parameterBox2.Text = "0.5";

                    lblPDescr3.Visible = false;
                    parameter3.Visible = false;
                    parameterBox3.Enabled = false;
                    parameterBox3.Visible = false;
                } else if (radioMotion.Checked) {
                    parameter1.Text = "Threshold";
                    lblPDescr1.Text = "Determines the sensitivity of the detection" + "\nRange: 0 - 1";
                    parameter2.Text = "Block size";
                    lblPDescr2.Text = "Size of the movement detection blocks" + "\nRange: 2 - 256, power of 2";
                    parameter3.Text = "Search window size";
                    lblPDescr3.Text = "Size of the search window";

                    parameterBox1.Text = "";
                    parameterBox2.Text = "";
                    parameterBox3.Text = "";
                } else if (this.radioGlobal.Checked) {
                    parameter1.Text = "Threshold";
                    this.parameterBox1.Text = "160";
                    lblPDescr1.Text = "Determines the sensitivity of the detection" + "\nRange: integer, default = 160";
                    parameter2.Text = "Number of bins";
                    this.parameterBox2.Text = "8";
                    lblPDescr2.Text = "Per colour, 2^x bins are considered" + "\nRange: 2 - 8, default = 8";

                    lblPDescr3.Visible = false;
                    parameter3.Visible = false;
                    parameterBox3.Enabled = false;
                    parameterBox3.Visible = false;
                } else if (this.radioLocal.Checked) {
                    this.parameter1.Text = "Threshold";
                    this.parameterBox1.Text = "450";
                    lblPDescr1.Text = "Determines the sensitivity of the detection" + "\nRange: integer, default = 450";
                    // Amount of bins is actually 2^x with x this number
                    this.parameter2.Text = "Number of bins";
                    this.parameterBox2.Text = "8";
                    lblPDescr2.Text = "Per colour, 2^x bins are considered" + "\nRange: 2 - 8, default = 8";
                    // Region size is the amount of times height and width is split
                    // The higher this parameter, the lower the actual size of the regions
                    // 2 -> 4 regions (split bot height and width in 2)
                    this.parameter3.Text = "Number of splits";
                    this.parameterBox3.Text = "35";
                    lblPDescr3.Text = "Amount of local areas is obtained as x^2, x splits along \nheight and width" + " (1 - 50, default = 35)";
                } else if (this.radioGeneral.Checked) {
                    parameter1.Text = "Threshold";
                    lblPDescr1.Text = "Determines the sensitivity of the detection" + "\nRange: 0 - 1";
                    parameter2.Text = "Block size";
                    lblPDescr2.Text = "Range: power of 2 (2 - 256)";
                    parameter3.Text = "Search window size";
                    lblPDescr3.Text = "Size of the search window";
                    parameterBox1.Text = "";
                    parameterBox2.Text = "";
                    parameterBox3.Text = "";
                }
            }
            if (this.tabControl.SelectedIndex == 2 && shotDetectionDone == false) {
                // The processing tab is located here
                try {
                    // Get the parameters
                    param1 = double.Parse(this.parameterBox1.Text, CultureInfo.InvariantCulture);
                    param2 = double.Parse(this.parameterBox2.Text, CultureInfo.InvariantCulture);
                    if (this.parameterBox3.Enabled)
                        param3 = Double.Parse(this.parameterBox3.Text, CultureInfo.InvariantCulture);
                    // Make sure we cannot change tab while processing
                    //this.tabControl.Enabled = false;
                    this.btnNext.Enabled = false;
                    this.btnBack.Enabled = false;
                    // Actual shot detection
                    if (this.radioPixel.Checked) {
                        shotDetectionMethod = 0;
                    } else if (this.radioMotion.Checked) {
                        shotDetectionMethod = 1;
                    } else if (this.radioGlobal.Checked) {
                        shotDetectionMethod = 2;
                    } else if (this.radioLocal.Checked) {
                        shotDetectionMethod = 3;
                    } else if (this.radioGeneral.Checked) {
                        shotDetectionMethod = 4;
                    }
                    shotDetection(shotDetectionMethod, param1, param2, param3);
                } catch (FormatException) {
                    lblDescr3.Text = "Wrong parameter input, please try again.";
                    this.btnNext.Enabled = true;
                    this.btnBack.Enabled = true;
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e) {
            buttonClicked = true;
            tabControl.SelectTab(Math.Max(tabControl.SelectedIndex - 1,0));
        }

        private void btnFinish_Click(object sender, EventArgs e) {
            double[] param = {param1, param2, param3};
            String[] paramNames = {"", "", ""};
            switch(shotDetectionMethod) {
                case 0:
                    paramNames[0] = "Pixel Threshold";
                    paramNames[1] = "Frame Threshold";
                    break;
                case 1:
                    paramNames[0] = "Threshold";
                    paramNames[1] = "Block size";
                    paramNames[2] = "Seach window size";
                    break;
                case 2:
                    paramNames[0] = "Threshold";
                    paramNames[1] = "Number of bins";
                    break;
                case 3:
                    paramNames[0] = "Threshold";
                    paramNames[1] = "Number of bins";
                    paramNames[2] = "Region size";
                    break;
                case 4:
                    paramNames[0] = "Threshold";
                    paramNames[1] = "Block size";
                    paramNames[2] = "Seach window size";
                    break;
            }
            original.addShots(shots, shotDetectionMethod, param, paramNames);
            finishClicked = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        enum State {
            uninit,
            busy
        }

        private DxScanner s_detect = null;

        private void shotDetection(int method, double param1, double param2, double param3) {
            Cursor.Current = Cursors.WaitCursor;
            s_detect = new DxScanner(fileName, method, param1, param2, param3, this);
            detectProgressBar.Maximum = totalFrames;

            stopwatch = Stopwatch.StartNew();
            detectionTimer.Enabled = true;
            s_detect.Start();
        }

        // Called when the video is finished playing
        public void detectionFinished() {
            lblProcessing.Text = "Processing collected data. Please wait.";
            afterProcessing();
            detectionTimer.Enabled = false;
            stopwatch.Stop();
            Cursor.Current = Cursors.Default;
            lblProcessing.Text = "Done processing " + totalFrames + " frames.";
            lblDescr5.Text = "In total, " + shots.Count + (shots.Count == 1 ? " shot was" : " shots were") + " detected.";
            double ticksSec = stopwatch.ElapsedMilliseconds / 1000.0;
            lblDescr6.Text = "The execution time was " + Math.Floor(ticksSec / 60.0) + ":" +
                    string.Format("{0:00.000}", ticksSec % 60.0) + ".";

            detectProgressBar.Value = totalFrames;
            shotDetectionDone = true;

            lock (this) {
                if (s_detect != null) {
                    s_detect.Dispose();
                    s_detect = null;
                }
                if (shotDetectionDone) {
                    this.btnNext.Enabled = true;
                    this.btnBack.Enabled = true;
                    buttonClicked = true;
                    tabControl.SelectTab(tabControl.TabCount - 1);
                }
            }
        }

        private void tabControl_Selecting(object sender, TabControlCancelEventArgs e) {
            if (!buttonClicked) {
                TabPage current = (sender as TabControl).SelectedTab;
                //validate the current page, to cancel the select use:
                e.Cancel = true;
            } else {
                buttonClicked = false;
            }
        }

        private void tabControl_DrawItem(object sender, DrawItemEventArgs e) {
            TabControl tabControl = sender as TabControl;
            TabPage page = tabControl.TabPages[e.Index];
            if (!tabControl.SelectedIndex.Equals(e.Index)) {
                //Draws disabled tab
                using (SolidBrush brush = new SolidBrush(SystemColors.GrayText)) {
                    e.Graphics.DrawString(page.Text, page.Font, brush, e.Bounds.X + 2, e.Bounds.Y + 3);
                }
            } else {
                // Draws normal tab
                using (SolidBrush brush = new SolidBrush(page.ForeColor)) {
                    e.Graphics.DrawString(page.Text, page.Font, brush, e.Bounds.X + 5, e.Bounds.Y + 3);
                }
            }
        }

        public void addShot(Shot shot) {
            shots.Add(shot);
            dgvShots.Rows.Add(shot.StartFrame, shot.EndFrame);
        }

        private void detectionTimer_Tick(object sender, EventArgs e) {
            if(s_detect.ContinueScan == false) {
                addShot(s_detect.currentShot);
                s_detect.ContinueScan = true;
            }
            lblProcessing.Text = string.Format("Processing frame: {0} / {1}:", s_detect.m_Count, totalFrames);
            detectProgressBar.Value = s_detect.m_Count;
            double ticksSec = stopwatch.ElapsedMilliseconds / 1000.0;
            lblTimeElapsedNum.Text = Math.Floor(ticksSec / 60.0) + ":" +
                    string.Format("{0:00.000}", ticksSec % 60.0);
            lock (s_detect) {
                if (!s_detect.disposing) {
                    if (s_detect.CheckIfDone()) {
                        detectionFinished();
                    }
                }
            }
        }

        private void afterProcessing() {
            if (s_detect.currentShot != null) {
                s_detect.currentShot.EndFrame = s_detect.m_Count - 1;
                addShot(s_detect.currentShot);
                s_detect.currentShot = null;
            }
        }

        private void ShotDetectionWizard_FormClosing(object sender, FormClosingEventArgs e) {
            if (tabControl.SelectedIndex >= 2 && finishClicked == false) {
                var confirmResult = MessageBox.Show("Are you sure you wish to cancel?\nAny detected shots will be lost.",
                    "Confirm Cancel", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.No) {
                    e.Cancel = true;
                    return;
                }
            }
            lock (this) {
                if (s_detect != null) {
                    s_detect.Dispose();
                    s_detect = null;
                }
            }
        }
    }
}
