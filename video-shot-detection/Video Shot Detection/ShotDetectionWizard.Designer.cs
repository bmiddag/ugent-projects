namespace DirectShow {
    partial class ShotDetectionWizard {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabMethod = new System.Windows.Forms.TabPage();
            this.lblDescr1 = new System.Windows.Forms.Label();
            this.radioGeneral = new System.Windows.Forms.RadioButton();
            this.radioLocal = new System.Windows.Forms.RadioButton();
            this.radioGlobal = new System.Windows.Forms.RadioButton();
            this.radioMotion = new System.Windows.Forms.RadioButton();
            this.radioPixel = new System.Windows.Forms.RadioButton();
            this.tabParameters = new System.Windows.Forms.TabPage();
            this.parameter3 = new System.Windows.Forms.Label();
            this.parameter2 = new System.Windows.Forms.Label();
            this.parameter1 = new System.Windows.Forms.Label();
            this.parameterBox3 = new System.Windows.Forms.TextBox();
            this.parameterBox2 = new System.Windows.Forms.TextBox();
            this.parameterBox1 = new System.Windows.Forms.TextBox();
            this.lblDescr2 = new System.Windows.Forms.Label();
            this.tabExecuting = new System.Windows.Forms.TabPage();
            this.lblTimeElapsedNum = new System.Windows.Forms.Label();
            this.lblTimeElapsedTxt = new System.Windows.Forms.Label();
            this.lblProcessing = new System.Windows.Forms.Label();
            this.detectProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblDescr3 = new System.Windows.Forms.Label();
            this.tabFinish = new System.Windows.Forms.TabPage();
            this.lblDescr6 = new System.Windows.Forms.Label();
            this.lblDescr5 = new System.Windows.Forms.Label();
            this.lblDescr4 = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnFinish = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dgvShots = new System.Windows.Forms.DataGridView();
            this.colBeginFrame = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEndFrame = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblDetectedShots = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.detectionTimer = new System.Windows.Forms.Timer(this.components);
            this.lblPDescr1 = new System.Windows.Forms.Label();
            this.lblPDescr2 = new System.Windows.Forms.Label();
            this.lblPDescr3 = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabMethod.SuspendLayout();
            this.tabParameters.SuspendLayout();
            this.tabExecuting.SuspendLayout();
            this.tabFinish.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvShots)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabMethod);
            this.tabControl.Controls.Add(this.tabParameters);
            this.tabControl.Controls.Add(this.tabExecuting);
            this.tabControl.Controls.Add(this.tabFinish);
            this.tabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl.Location = new System.Drawing.Point(251, 89);
            this.tabControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(421, 304);
            this.tabControl.TabIndex = 0;
            this.tabControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl_DrawItem);
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            this.tabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl_Selecting);
            // 
            // tabMethod
            // 
            this.tabMethod.Controls.Add(this.lblDescr1);
            this.tabMethod.Controls.Add(this.radioGeneral);
            this.tabMethod.Controls.Add(this.radioLocal);
            this.tabMethod.Controls.Add(this.radioGlobal);
            this.tabMethod.Controls.Add(this.radioMotion);
            this.tabMethod.Controls.Add(this.radioPixel);
            this.tabMethod.Location = new System.Drawing.Point(4, 25);
            this.tabMethod.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabMethod.Name = "tabMethod";
            this.tabMethod.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabMethod.Size = new System.Drawing.Size(413, 275);
            this.tabMethod.TabIndex = 0;
            this.tabMethod.Text = "1. Method";
            this.tabMethod.UseVisualStyleBackColor = true;
            // 
            // lblDescr1
            // 
            this.lblDescr1.AutoSize = true;
            this.lblDescr1.Location = new System.Drawing.Point(37, 41);
            this.lblDescr1.Name = "lblDescr1";
            this.lblDescr1.Size = new System.Drawing.Size(339, 34);
            this.lblDescr1.TabIndex = 30;
            this.lblDescr1.Text = "Welcome to the Shot Detection wizard. Please select\r\nthe method you wish to use f" +
    "or shot detection.";
            // 
            // radioGeneral
            // 
            this.radioGeneral.AutoSize = true;
            this.radioGeneral.Location = new System.Drawing.Point(85, 206);
            this.radioGeneral.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioGeneral.Name = "radioGeneral";
            this.radioGeneral.Size = new System.Drawing.Size(106, 21);
            this.radioGeneral.TabIndex = 29;
            this.radioGeneral.TabStop = true;
            this.radioGeneral.Text = "Generalized";
            this.radioGeneral.UseVisualStyleBackColor = true;
            // 
            // radioLocal
            // 
            this.radioLocal.AutoSize = true;
            this.radioLocal.Location = new System.Drawing.Point(85, 178);
            this.radioLocal.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioLocal.Name = "radioLocal";
            this.radioLocal.Size = new System.Drawing.Size(129, 21);
            this.radioLocal.TabIndex = 28;
            this.radioLocal.TabStop = true;
            this.radioLocal.Text = "Local histogram";
            this.radioLocal.UseVisualStyleBackColor = true;
            // 
            // radioGlobal
            // 
            this.radioGlobal.AutoSize = true;
            this.radioGlobal.Location = new System.Drawing.Point(85, 153);
            this.radioGlobal.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioGlobal.Name = "radioGlobal";
            this.radioGlobal.Size = new System.Drawing.Size(136, 21);
            this.radioGlobal.TabIndex = 27;
            this.radioGlobal.TabStop = true;
            this.radioGlobal.Text = "Global histogram";
            this.radioGlobal.UseVisualStyleBackColor = true;
            // 
            // radioMotion
            // 
            this.radioMotion.AutoSize = true;
            this.radioMotion.Location = new System.Drawing.Point(85, 126);
            this.radioMotion.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioMotion.Name = "radioMotion";
            this.radioMotion.Size = new System.Drawing.Size(139, 21);
            this.radioMotion.TabIndex = 26;
            this.radioMotion.TabStop = true;
            this.radioMotion.Text = "Motion estimation";
            this.radioMotion.UseVisualStyleBackColor = true;
            // 
            // radioPixel
            // 
            this.radioPixel.AutoSize = true;
            this.radioPixel.Checked = true;
            this.radioPixel.Location = new System.Drawing.Point(85, 98);
            this.radioPixel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioPixel.Name = "radioPixel";
            this.radioPixel.Size = new System.Drawing.Size(125, 21);
            this.radioPixel.TabIndex = 25;
            this.radioPixel.TabStop = true;
            this.radioPixel.Text = "Pixel difference";
            this.radioPixel.UseVisualStyleBackColor = true;
            // 
            // tabParameters
            // 
            this.tabParameters.Controls.Add(this.lblPDescr3);
            this.tabParameters.Controls.Add(this.lblPDescr2);
            this.tabParameters.Controls.Add(this.lblPDescr1);
            this.tabParameters.Controls.Add(this.parameter3);
            this.tabParameters.Controls.Add(this.parameter2);
            this.tabParameters.Controls.Add(this.parameter1);
            this.tabParameters.Controls.Add(this.parameterBox3);
            this.tabParameters.Controls.Add(this.parameterBox2);
            this.tabParameters.Controls.Add(this.parameterBox1);
            this.tabParameters.Controls.Add(this.lblDescr2);
            this.tabParameters.Location = new System.Drawing.Point(4, 25);
            this.tabParameters.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabParameters.Name = "tabParameters";
            this.tabParameters.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabParameters.Size = new System.Drawing.Size(413, 275);
            this.tabParameters.TabIndex = 1;
            this.tabParameters.Text = "2. Parameters";
            this.tabParameters.UseVisualStyleBackColor = true;
            // 
            // parameter3
            // 
            this.parameter3.AutoSize = true;
            this.parameter3.Location = new System.Drawing.Point(38, 206);
            this.parameter3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.parameter3.Name = "parameter3";
            this.parameter3.Size = new System.Drawing.Size(86, 17);
            this.parameter3.TabIndex = 37;
            this.parameter3.Text = "Parameter 3";
            // 
            // parameter2
            // 
            this.parameter2.AutoSize = true;
            this.parameter2.Location = new System.Drawing.Point(38, 139);
            this.parameter2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.parameter2.Name = "parameter2";
            this.parameter2.Size = new System.Drawing.Size(86, 17);
            this.parameter2.TabIndex = 36;
            this.parameter2.Text = "Parameter 2";
            // 
            // parameter1
            // 
            this.parameter1.AutoSize = true;
            this.parameter1.Location = new System.Drawing.Point(38, 73);
            this.parameter1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.parameter1.Name = "parameter1";
            this.parameter1.Size = new System.Drawing.Size(86, 17);
            this.parameter1.TabIndex = 35;
            this.parameter1.Text = "Parameter 1";
            // 
            // parameterBox3
            // 
            this.parameterBox3.Enabled = false;
            this.parameterBox3.Location = new System.Drawing.Point(192, 203);
            this.parameterBox3.Margin = new System.Windows.Forms.Padding(4);
            this.parameterBox3.Name = "parameterBox3";
            this.parameterBox3.Size = new System.Drawing.Size(132, 22);
            this.parameterBox3.TabIndex = 34;
            // 
            // parameterBox2
            // 
            this.parameterBox2.Location = new System.Drawing.Point(192, 136);
            this.parameterBox2.Margin = new System.Windows.Forms.Padding(4);
            this.parameterBox2.Name = "parameterBox2";
            this.parameterBox2.Size = new System.Drawing.Size(132, 22);
            this.parameterBox2.TabIndex = 33;
            // 
            // parameterBox1
            // 
            this.parameterBox1.Location = new System.Drawing.Point(192, 70);
            this.parameterBox1.Margin = new System.Windows.Forms.Padding(4);
            this.parameterBox1.Name = "parameterBox1";
            this.parameterBox1.Size = new System.Drawing.Size(132, 22);
            this.parameterBox1.TabIndex = 32;
            // 
            // lblDescr2
            // 
            this.lblDescr2.AutoSize = true;
            this.lblDescr2.Location = new System.Drawing.Point(20, 41);
            this.lblDescr2.Name = "lblDescr2";
            this.lblDescr2.Size = new System.Drawing.Size(294, 17);
            this.lblDescr2.TabIndex = 31;
            this.lblDescr2.Text = "Please select the parameters for this method.";
            // 
            // tabExecuting
            // 
            this.tabExecuting.Controls.Add(this.lblTimeElapsedNum);
            this.tabExecuting.Controls.Add(this.lblTimeElapsedTxt);
            this.tabExecuting.Controls.Add(this.lblProcessing);
            this.tabExecuting.Controls.Add(this.detectProgressBar);
            this.tabExecuting.Controls.Add(this.lblDescr3);
            this.tabExecuting.Location = new System.Drawing.Point(4, 25);
            this.tabExecuting.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabExecuting.Name = "tabExecuting";
            this.tabExecuting.Size = new System.Drawing.Size(413, 275);
            this.tabExecuting.TabIndex = 2;
            this.tabExecuting.Text = "3. Shot Detection";
            this.tabExecuting.UseVisualStyleBackColor = true;
            // 
            // lblTimeElapsedNum
            // 
            this.lblTimeElapsedNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTimeElapsedNum.Location = new System.Drawing.Point(307, 204);
            this.lblTimeElapsedNum.Name = "lblTimeElapsedNum";
            this.lblTimeElapsedNum.Size = new System.Drawing.Size(72, 17);
            this.lblTimeElapsedNum.TabIndex = 35;
            this.lblTimeElapsedNum.Text = "0:00.000";
            this.lblTimeElapsedNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTimeElapsedTxt
            // 
            this.lblTimeElapsedTxt.AutoSize = true;
            this.lblTimeElapsedTxt.Location = new System.Drawing.Point(204, 204);
            this.lblTimeElapsedTxt.Name = "lblTimeElapsedTxt";
            this.lblTimeElapsedTxt.Size = new System.Drawing.Size(97, 17);
            this.lblTimeElapsedTxt.TabIndex = 34;
            this.lblTimeElapsedTxt.Text = "Time elapsed:";
            // 
            // lblProcessing
            // 
            this.lblProcessing.AutoSize = true;
            this.lblProcessing.Location = new System.Drawing.Point(37, 135);
            this.lblProcessing.Name = "lblProcessing";
            this.lblProcessing.Size = new System.Drawing.Size(150, 17);
            this.lblProcessing.TabIndex = 33;
            this.lblProcessing.Text = "Processing frame 0 / 0";
            // 
            // detectProgressBar
            // 
            this.detectProgressBar.Location = new System.Drawing.Point(40, 167);
            this.detectProgressBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.detectProgressBar.Name = "detectProgressBar";
            this.detectProgressBar.Size = new System.Drawing.Size(339, 23);
            this.detectProgressBar.TabIndex = 32;
            // 
            // lblDescr3
            // 
            this.lblDescr3.AutoSize = true;
            this.lblDescr3.Location = new System.Drawing.Point(37, 41);
            this.lblDescr3.Name = "lblDescr3";
            this.lblDescr3.Size = new System.Drawing.Size(342, 51);
            this.lblDescr3.TabIndex = 31;
            this.lblDescr3.Text = "We will now process your video file. This could take a\r\nwhile. You can follow the" +
    " progress below. Detected\r\nshots will appear in the table on the left.";
            // 
            // tabFinish
            // 
            this.tabFinish.Controls.Add(this.lblDescr6);
            this.tabFinish.Controls.Add(this.lblDescr5);
            this.tabFinish.Controls.Add(this.lblDescr4);
            this.tabFinish.Location = new System.Drawing.Point(4, 25);
            this.tabFinish.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabFinish.Name = "tabFinish";
            this.tabFinish.Size = new System.Drawing.Size(413, 275);
            this.tabFinish.TabIndex = 3;
            this.tabFinish.Text = "4. Overview";
            this.tabFinish.UseVisualStyleBackColor = true;
            // 
            // lblDescr6
            // 
            this.lblDescr6.AutoSize = true;
            this.lblDescr6.Location = new System.Drawing.Point(37, 138);
            this.lblDescr6.Name = "lblDescr6";
            this.lblDescr6.Size = new System.Drawing.Size(219, 17);
            this.lblDescr6.TabIndex = 36;
            this.lblDescr6.Text = "The execution time was 0:00.000.";
            // 
            // lblDescr5
            // 
            this.lblDescr5.AutoSize = true;
            this.lblDescr5.Location = new System.Drawing.Point(37, 121);
            this.lblDescr5.Name = "lblDescr5";
            this.lblDescr5.Size = new System.Drawing.Size(201, 17);
            this.lblDescr5.TabIndex = 33;
            this.lblDescr5.Text = "In total, 0 shots were detected.";
            // 
            // lblDescr4
            // 
            this.lblDescr4.AutoSize = true;
            this.lblDescr4.Location = new System.Drawing.Point(37, 41);
            this.lblDescr4.Name = "lblDescr4";
            this.lblDescr4.Size = new System.Drawing.Size(334, 34);
            this.lblDescr4.TabIndex = 32;
            this.lblDescr4.Text = "We have finished processing the video. Please take\r\na moment to view the results." +
    "";
            // 
            // btnBack
            // 
            this.btnBack.Enabled = false;
            this.btnBack.Location = new System.Drawing.Point(399, 406);
            this.btnBack.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(85, 27);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = "˂ Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(491, 406);
            this.btnNext.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(85, 27);
            this.btnNext.TabIndex = 2;
            this.btnNext.Text = "Next ˃";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnFinish
            // 
            this.btnFinish.Enabled = false;
            this.btnFinish.Location = new System.Drawing.Point(583, 406);
            this.btnFinish.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(85, 27);
            this.btnFinish.TabIndex = 3;
            this.btnFinish.Text = "Finish";
            this.btnFinish.UseVisualStyleBackColor = true;
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(307, 406);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 27);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dgvShots
            // 
            this.dgvShots.AllowUserToAddRows = false;
            this.dgvShots.AllowUserToDeleteRows = false;
            this.dgvShots.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvShots.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colBeginFrame,
            this.colEndFrame});
            this.dgvShots.Location = new System.Drawing.Point(13, 57);
            this.dgvShots.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvShots.Name = "dgvShots";
            this.dgvShots.ReadOnly = true;
            this.dgvShots.RowTemplate.Height = 24;
            this.dgvShots.Size = new System.Drawing.Size(223, 331);
            this.dgvShots.TabIndex = 5;
            // 
            // colBeginFrame
            // 
            this.colBeginFrame.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colBeginFrame.HeaderText = "From";
            this.colBeginFrame.MaxInputLength = 7;
            this.colBeginFrame.Name = "colBeginFrame";
            this.colBeginFrame.ReadOnly = true;
            this.colBeginFrame.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colEndFrame
            // 
            this.colEndFrame.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colEndFrame.HeaderText = "To";
            this.colEndFrame.MaxInputLength = 7;
            this.colEndFrame.Name = "colEndFrame";
            this.colEndFrame.ReadOnly = true;
            this.colEndFrame.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // lblDetectedShots
            // 
            this.lblDetectedShots.AutoSize = true;
            this.lblDetectedShots.Location = new System.Drawing.Point(47, 26);
            this.lblDetectedShots.Name = "lblDetectedShots";
            this.lblDetectedShots.Size = new System.Drawing.Size(162, 17);
            this.lblDetectedShots.TabIndex = 6;
            this.lblDetectedShots.Text = "Detected Shots (frames)";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(249, 26);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(333, 36);
            this.lblTitle.TabIndex = 7;
            this.lblTitle.Text = "Shot Detection Wizard";
            // 
            // detectionTimer
            // 
            this.detectionTimer.Interval = 75;
            this.detectionTimer.Tick += new System.EventHandler(this.detectionTimer_Tick);
            // 
            // lblPDescr1
            // 
            this.lblPDescr1.AutoSize = true;
            this.lblPDescr1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPDescr1.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblPDescr1.Location = new System.Drawing.Point(38, 96);
            this.lblPDescr1.Name = "lblPDescr1";
            this.lblPDescr1.Size = new System.Drawing.Size(161, 34);
            this.lblPDescr1.TabIndex = 38;
            this.lblPDescr1.Text = "Parameter 1 Description\r\nRange: 0 - 0";
            // 
            // lblPDescr2
            // 
            this.lblPDescr2.AutoSize = true;
            this.lblPDescr2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPDescr2.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblPDescr2.Location = new System.Drawing.Point(38, 162);
            this.lblPDescr2.Name = "lblPDescr2";
            this.lblPDescr2.Size = new System.Drawing.Size(161, 34);
            this.lblPDescr2.TabIndex = 39;
            this.lblPDescr2.Text = "Parameter 2 Description\r\nRange: 0 - 0";
            // 
            // lblPDescr3
            // 
            this.lblPDescr3.AutoSize = true;
            this.lblPDescr3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPDescr3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblPDescr3.Location = new System.Drawing.Point(38, 229);
            this.lblPDescr3.Name = "lblPDescr3";
            this.lblPDescr3.Size = new System.Drawing.Size(161, 34);
            this.lblPDescr3.TabIndex = 40;
            this.lblPDescr3.Text = "Parameter 3 Description\r\nRange: 0 - 0";
            // 
            // ShotDetectionWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 439);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblDetectedShots);
            this.Controls.Add(this.dgvShots);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnFinish);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ShotDetectionWizard";
            this.Text = "Shot Detection Wizard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ShotDetectionWizard_FormClosing);
            this.tabControl.ResumeLayout(false);
            this.tabMethod.ResumeLayout(false);
            this.tabMethod.PerformLayout();
            this.tabParameters.ResumeLayout(false);
            this.tabParameters.PerformLayout();
            this.tabExecuting.ResumeLayout(false);
            this.tabExecuting.PerformLayout();
            this.tabFinish.ResumeLayout(false);
            this.tabFinish.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvShots)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabMethod;
        private System.Windows.Forms.TabPage tabParameters;
        private System.Windows.Forms.TabPage tabExecuting;
        private System.Windows.Forms.TabPage tabFinish;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnFinish;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton radioGeneral;
        private System.Windows.Forms.RadioButton radioLocal;
        private System.Windows.Forms.RadioButton radioGlobal;
        private System.Windows.Forms.RadioButton radioMotion;
        private System.Windows.Forms.RadioButton radioPixel;
        private System.Windows.Forms.DataGridView dgvShots;
        private System.Windows.Forms.Label lblDetectedShots;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDescr1;
        private System.Windows.Forms.Label lblDescr2;
        private System.Windows.Forms.Label lblProcessing;
        private System.Windows.Forms.ProgressBar detectProgressBar;
        private System.Windows.Forms.Label lblDescr3;
        private System.Windows.Forms.Label lblDescr5;
        private System.Windows.Forms.Label lblDescr4;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBeginFrame;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEndFrame;
        private System.Windows.Forms.TextBox parameterBox3;
        private System.Windows.Forms.TextBox parameterBox2;
        private System.Windows.Forms.TextBox parameterBox1;
        private System.Windows.Forms.Label parameter3;
        private System.Windows.Forms.Label parameter2;
        private System.Windows.Forms.Label parameter1;
        private System.Windows.Forms.Label lblTimeElapsedTxt;
        private System.Windows.Forms.Label lblTimeElapsedNum;
        private System.Windows.Forms.Timer detectionTimer;
        private System.Windows.Forms.Label lblDescr6;
        private System.Windows.Forms.Label lblPDescr3;
        private System.Windows.Forms.Label lblPDescr2;
        private System.Windows.Forms.Label lblPDescr1;
    }
}