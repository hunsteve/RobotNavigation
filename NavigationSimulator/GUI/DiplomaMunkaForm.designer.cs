namespace OnlabNeuralis
{
    partial class NavigationSimulatorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.buttonStartSim = new System.Windows.Forms.Button();
            this.textBoxPosX = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxPosY = new System.Windows.Forms.TextBox();
            this.textBoxOriAng = new System.Windows.Forms.TextBox();
            this.textBoxOriX = new System.Windows.Forms.TextBox();
            this.textBoxOriY = new System.Windows.Forms.TextBox();
            this.buttonStopTraining = new System.Windows.Forms.Button();
            this.checkBoxSerial = new System.Windows.Forms.CheckBox();
            this.comboBoxSerial = new System.Windows.Forms.ComboBox();
            this.textBoxSerial = new System.Windows.Forms.TextBox();
            this.buttonStopSim = new System.Windows.Forms.Button();
            this.buttonStartTraining = new System.Windows.Forms.Button();
            this.pictureBoxCamPreview = new System.Windows.Forms.PictureBox();
            this.checkBoxShowCamImage = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newObstacleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteObstacleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.carModelGraphicControl1 = new OnlabNeuralis.CarModelGraphicControl();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.buttonProcessSingleFrame = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBoxCarMarker = new System.Windows.Forms.PictureBox();
            this.pictureBoxFinishMarker = new System.Windows.Forms.PictureBox();
            this.pictureBoxBackground = new System.Windows.Forms.PictureBox();
            this.checkBoxCameraEnabled = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxCarEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxAdaptiveMu = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxShowNNOutput = new System.Windows.Forms.CheckBox();
            this.textBoxMu = new System.Windows.Forms.TextBox();
            this.trackBarMu = new System.Windows.Forms.TrackBar();
            this.buttonReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamPreview)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCarMarker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFinishMarker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMu)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // buttonStartSim
            // 
            this.buttonStartSim.Location = new System.Drawing.Point(7, 17);
            this.buttonStartSim.Name = "buttonStartSim";
            this.buttonStartSim.Size = new System.Drawing.Size(82, 23);
            this.buttonStartSim.TabIndex = 1;
            this.buttonStartSim.Text = "Start Sim";
            this.buttonStartSim.UseVisualStyleBackColor = true;
            this.buttonStartSim.Click += new System.EventHandler(this.buttonStartSim_Click);
            // 
            // textBoxPosX
            // 
            this.textBoxPosX.Location = new System.Drawing.Point(24, 19);
            this.textBoxPosX.Name = "textBoxPosX";
            this.textBoxPosX.ReadOnly = true;
            this.textBoxPosX.Size = new System.Drawing.Size(94, 20);
            this.textBoxPosX.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "X";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Y";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Angle";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Or. X";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 74);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Or. Y";
            // 
            // textBoxPosY
            // 
            this.textBoxPosY.Location = new System.Drawing.Point(24, 44);
            this.textBoxPosY.Name = "textBoxPosY";
            this.textBoxPosY.ReadOnly = true;
            this.textBoxPosY.Size = new System.Drawing.Size(94, 20);
            this.textBoxPosY.TabIndex = 5;
            // 
            // textBoxOriAng
            // 
            this.textBoxOriAng.Location = new System.Drawing.Point(42, 19);
            this.textBoxOriAng.Name = "textBoxOriAng";
            this.textBoxOriAng.ReadOnly = true;
            this.textBoxOriAng.Size = new System.Drawing.Size(76, 20);
            this.textBoxOriAng.TabIndex = 5;
            // 
            // textBoxOriX
            // 
            this.textBoxOriX.Location = new System.Drawing.Point(42, 45);
            this.textBoxOriX.Name = "textBoxOriX";
            this.textBoxOriX.ReadOnly = true;
            this.textBoxOriX.Size = new System.Drawing.Size(76, 20);
            this.textBoxOriX.TabIndex = 5;
            // 
            // textBoxOriY
            // 
            this.textBoxOriY.Location = new System.Drawing.Point(42, 71);
            this.textBoxOriY.Name = "textBoxOriY";
            this.textBoxOriY.ReadOnly = true;
            this.textBoxOriY.Size = new System.Drawing.Size(76, 20);
            this.textBoxOriY.TabIndex = 5;
            // 
            // buttonStopTraining
            // 
            this.buttonStopTraining.Location = new System.Drawing.Point(95, 49);
            this.buttonStopTraining.Name = "buttonStopTraining";
            this.buttonStopTraining.Size = new System.Drawing.Size(81, 23);
            this.buttonStopTraining.TabIndex = 6;
            this.buttonStopTraining.Text = "Stop Training";
            this.buttonStopTraining.UseVisualStyleBackColor = true;
            this.buttonStopTraining.Click += new System.EventHandler(this.buttonStopTraining_Click);
            // 
            // checkBoxSerial
            // 
            this.checkBoxSerial.AutoSize = true;
            this.checkBoxSerial.Location = new System.Drawing.Point(6, 18);
            this.checkBoxSerial.Name = "checkBoxSerial";
            this.checkBoxSerial.Size = new System.Drawing.Size(114, 17);
            this.checkBoxSerial.TabIndex = 7;
            this.checkBoxSerial.Text = "Serial port enabled";
            this.checkBoxSerial.UseVisualStyleBackColor = true;
            this.checkBoxSerial.CheckedChanged += new System.EventHandler(this.checkBoxSerial_CheckedChanged);
            // 
            // comboBoxSerial
            // 
            this.comboBoxSerial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSerial.FormattingEnabled = true;
            this.comboBoxSerial.Location = new System.Drawing.Point(185, 16);
            this.comboBoxSerial.Name = "comboBoxSerial";
            this.comboBoxSerial.Size = new System.Drawing.Size(73, 21);
            this.comboBoxSerial.TabIndex = 8;
            // 
            // textBoxSerial
            // 
            this.textBoxSerial.Location = new System.Drawing.Point(6, 41);
            this.textBoxSerial.Multiline = true;
            this.textBoxSerial.Name = "textBoxSerial";
            this.textBoxSerial.ReadOnly = true;
            this.textBoxSerial.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxSerial.Size = new System.Drawing.Size(252, 53);
            this.textBoxSerial.TabIndex = 9;
            // 
            // buttonStopSim
            // 
            this.buttonStopSim.Location = new System.Drawing.Point(95, 17);
            this.buttonStopSim.Name = "buttonStopSim";
            this.buttonStopSim.Size = new System.Drawing.Size(81, 23);
            this.buttonStopSim.TabIndex = 10;
            this.buttonStopSim.Text = "Stop Sim";
            this.buttonStopSim.UseVisualStyleBackColor = true;
            this.buttonStopSim.Click += new System.EventHandler(this.buttonStopSim_Click);
            // 
            // buttonStartTraining
            // 
            this.buttonStartTraining.Location = new System.Drawing.Point(7, 49);
            this.buttonStartTraining.Name = "buttonStartTraining";
            this.buttonStartTraining.Size = new System.Drawing.Size(82, 23);
            this.buttonStartTraining.TabIndex = 6;
            this.buttonStartTraining.Text = "Start Training";
            this.buttonStartTraining.UseVisualStyleBackColor = true;
            this.buttonStartTraining.Click += new System.EventHandler(this.buttonStartTraining_Click);
            // 
            // pictureBoxCamPreview
            // 
            this.pictureBoxCamPreview.Location = new System.Drawing.Point(191, 19);
            this.pictureBoxCamPreview.Name = "pictureBoxCamPreview";
            this.pictureBoxCamPreview.Size = new System.Drawing.Size(64, 48);
            this.pictureBoxCamPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxCamPreview.TabIndex = 13;
            this.pictureBoxCamPreview.TabStop = false;
            // 
            // checkBoxShowCamImage
            // 
            this.checkBoxShowCamImage.AutoSize = true;
            this.checkBoxShowCamImage.Checked = true;
            this.checkBoxShowCamImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowCamImage.Location = new System.Drawing.Point(6, 111);
            this.checkBoxShowCamImage.Name = "checkBoxShowCamImage";
            this.checkBoxShowCamImage.Size = new System.Drawing.Size(124, 17);
            this.checkBoxShowCamImage.TabIndex = 14;
            this.checkBoxShowCamImage.Text = "Show Camera Image";
            this.checkBoxShowCamImage.UseVisualStyleBackColor = true;
            this.checkBoxShowCamImage.CheckedChanged += new System.EventHandler(this.checkBoxShowCamImage_CheckedChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.contextMenuStrip1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(16, 8);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newObstacleToolStripMenuItem,
            this.deleteObstacleToolStripMenuItem,
            this.cancelToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip1.ShowCheckMargin = true;
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(162, 70);
            // 
            // newObstacleToolStripMenuItem
            // 
            this.newObstacleToolStripMenuItem.Name = "newObstacleToolStripMenuItem";
            this.newObstacleToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.newObstacleToolStripMenuItem.Text = "New Obstacle";
            // 
            // deleteObstacleToolStripMenuItem
            // 
            this.deleteObstacleToolStripMenuItem.Name = "deleteObstacleToolStripMenuItem";
            this.deleteObstacleToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.deleteObstacleToolStripMenuItem.Text = "Delete Obstacle";
            // 
            // cancelToolStripMenuItem
            // 
            this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
            this.cancelToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.cancelToolStripMenuItem.Text = "Cancel";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.carModelGraphicControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox5);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox4);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(744, 681);
            this.splitContainer1.SplitterDistance = 471;
            this.splitContainer1.TabIndex = 15;
            // 
            // carModelGraphicControl1
            // 
            this.carModelGraphicControl1.ContextMenuStrip = this.contextMenuStrip1;
            this.carModelGraphicControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.carModelGraphicControl1.Location = new System.Drawing.Point(0, 0);
            this.carModelGraphicControl1.Name = "carModelGraphicControl1";
            this.carModelGraphicControl1.Size = new System.Drawing.Size(471, 681);
            this.carModelGraphicControl1.TabIndex = 0;
            this.carModelGraphicControl1.Text = "carModelGraphicControl1";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.buttonProcessSingleFrame);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.pictureBoxCarMarker);
            this.groupBox5.Controls.Add(this.pictureBoxFinishMarker);
            this.groupBox5.Controls.Add(this.pictureBoxBackground);
            this.groupBox5.Controls.Add(this.checkBoxCameraEnabled);
            this.groupBox5.Controls.Add(this.checkBoxShowCamImage);
            this.groupBox5.Controls.Add(this.pictureBoxCamPreview);
            this.groupBox5.Location = new System.Drawing.Point(3, 375);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(264, 167);
            this.groupBox5.TabIndex = 19;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Camera";
            // 
            // buttonProcessSingleFrame
            // 
            this.buttonProcessSingleFrame.Location = new System.Drawing.Point(6, 134);
            this.buttonProcessSingleFrame.Name = "buttonProcessSingleFrame";
            this.buttonProcessSingleFrame.Size = new System.Drawing.Size(124, 23);
            this.buttonProcessSingleFrame.TabIndex = 20;
            this.buttonProcessSingleFrame.Text = "Process Single Frame";
            this.buttonProcessSingleFrame.UseVisualStyleBackColor = true;
            this.buttonProcessSingleFrame.Click += new System.EventHandler(this.buttonProcessSingleFrame_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(188, 72);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(27, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "Live";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(118, 72);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Background";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(62, 72);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Finish";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Car";
            // 
            // pictureBoxCarMarker
            // 
            this.pictureBoxCarMarker.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxCarMarker.Location = new System.Drawing.Point(8, 19);
            this.pictureBoxCarMarker.Name = "pictureBoxCarMarker";
            this.pictureBoxCarMarker.Size = new System.Drawing.Size(50, 50);
            this.pictureBoxCarMarker.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxCarMarker.TabIndex = 18;
            this.pictureBoxCarMarker.TabStop = false;
            this.pictureBoxCarMarker.Click += new System.EventHandler(this.pictureBoxCarMarker_Click);
            // 
            // pictureBoxFinishMarker
            // 
            this.pictureBoxFinishMarker.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxFinishMarker.Location = new System.Drawing.Point(65, 19);
            this.pictureBoxFinishMarker.Name = "pictureBoxFinishMarker";
            this.pictureBoxFinishMarker.Size = new System.Drawing.Size(50, 50);
            this.pictureBoxFinishMarker.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxFinishMarker.TabIndex = 18;
            this.pictureBoxFinishMarker.TabStop = false;
            this.pictureBoxFinishMarker.Click += new System.EventHandler(this.pictureBoxFinishMarker_Click);
            // 
            // pictureBoxBackground
            // 
            this.pictureBoxBackground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxBackground.Location = new System.Drawing.Point(121, 19);
            this.pictureBoxBackground.Name = "pictureBoxBackground";
            this.pictureBoxBackground.Size = new System.Drawing.Size(64, 48);
            this.pictureBoxBackground.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxBackground.TabIndex = 18;
            this.pictureBoxBackground.TabStop = false;
            this.pictureBoxBackground.Click += new System.EventHandler(this.pictureBoxBackground_Click);
            // 
            // checkBoxCameraEnabled
            // 
            this.checkBoxCameraEnabled.AutoSize = true;
            this.checkBoxCameraEnabled.Location = new System.Drawing.Point(6, 88);
            this.checkBoxCameraEnabled.Name = "checkBoxCameraEnabled";
            this.checkBoxCameraEnabled.Size = new System.Drawing.Size(166, 17);
            this.checkBoxCameraEnabled.TabIndex = 16;
            this.checkBoxCameraEnabled.Text = "Enable Camera Marker Finder";
            this.checkBoxCameraEnabled.UseVisualStyleBackColor = true;
            this.checkBoxCameraEnabled.CheckedChanged += new System.EventHandler(this.checkBoxCameraEnabled_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBoxSerial);
            this.groupBox4.Controls.Add(this.comboBoxSerial);
            this.groupBox4.Controls.Add(this.textBoxSerial);
            this.groupBox4.Location = new System.Drawing.Point(3, 269);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(264, 100);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Serial Port";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxOriAng);
            this.groupBox3.Controls.Add(this.textBoxOriY);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.textBoxOriX);
            this.groupBox3.Location = new System.Drawing.Point(134, 166);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(133, 97);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Orientation";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxPosX);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBoxPosY);
            this.groupBox2.Location = new System.Drawing.Point(3, 166);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(127, 97);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Position";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxCarEnable);
            this.groupBox1.Controls.Add(this.checkBoxAdaptiveMu);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.checkBoxShowNNOutput);
            this.groupBox1.Controls.Add(this.textBoxMu);
            this.groupBox1.Controls.Add(this.trackBarMu);
            this.groupBox1.Controls.Add(this.buttonReset);
            this.groupBox1.Controls.Add(this.buttonStopTraining);
            this.groupBox1.Controls.Add(this.buttonStartTraining);
            this.groupBox1.Controls.Add(this.buttonStopSim);
            this.groupBox1.Controls.Add(this.buttonStartSim);
            this.groupBox1.Location = new System.Drawing.Point(3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(264, 158);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Control";
            // 
            // checkBoxCarEnable
            // 
            this.checkBoxCarEnable.AutoSize = true;
            this.checkBoxCarEnable.Location = new System.Drawing.Point(185, 54);
            this.checkBoxCarEnable.Name = "checkBoxCarEnable";
            this.checkBoxCarEnable.Size = new System.Drawing.Size(77, 17);
            this.checkBoxCarEnable.TabIndex = 17;
            this.checkBoxCarEnable.Text = "Car enable";
            this.checkBoxCarEnable.UseVisualStyleBackColor = true;
            // 
            // checkBoxAdaptiveMu
            // 
            this.checkBoxAdaptiveMu.AutoSize = true;
            this.checkBoxAdaptiveMu.Location = new System.Drawing.Point(9, 110);
            this.checkBoxAdaptiveMu.Name = "checkBoxAdaptiveMu";
            this.checkBoxAdaptiveMu.Size = new System.Drawing.Size(77, 17);
            this.checkBoxAdaptiveMu.TabIndex = 16;
            this.checkBoxAdaptiveMu.Text = "Adaptive μ";
            this.checkBoxAdaptiveMu.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(156, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "μ";
            // 
            // checkBoxShowNNOutput
            // 
            this.checkBoxShowNNOutput.AutoSize = true;
            this.checkBoxShowNNOutput.Checked = true;
            this.checkBoxShowNNOutput.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowNNOutput.Location = new System.Drawing.Point(9, 133);
            this.checkBoxShowNNOutput.Name = "checkBoxShowNNOutput";
            this.checkBoxShowNNOutput.Size = new System.Drawing.Size(107, 17);
            this.checkBoxShowNNOutput.TabIndex = 14;
            this.checkBoxShowNNOutput.Text = "Show NN Output";
            this.checkBoxShowNNOutput.UseVisualStyleBackColor = true;
            this.checkBoxShowNNOutput.CheckedChanged += new System.EventHandler(this.checkBoxShowNNOutput_CheckedChanged);
            // 
            // textBoxMu
            // 
            this.textBoxMu.Location = new System.Drawing.Point(173, 108);
            this.textBoxMu.Name = "textBoxMu";
            this.textBoxMu.ReadOnly = true;
            this.textBoxMu.Size = new System.Drawing.Size(71, 20);
            this.textBoxMu.TabIndex = 13;
            // 
            // trackBarMu
            // 
            this.trackBarMu.Location = new System.Drawing.Point(9, 77);
            this.trackBarMu.Maximum = 100;
            this.trackBarMu.Name = "trackBarMu";
            this.trackBarMu.Size = new System.Drawing.Size(235, 42);
            this.trackBarMu.TabIndex = 12;
            this.trackBarMu.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarMu.Value = 85;
            this.trackBarMu.Scroll += new System.EventHandler(this.trackBarMu_Scroll);
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(182, 17);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(76, 23);
            this.buttonReset.TabIndex = 11;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // NavigationSimulatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 681);
            this.Controls.Add(this.splitContainer1);
            this.Name = "NavigationSimulatorForm";
            this.Text = "Neural Network Car Controller";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamPreview)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCarMarker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFinishMarker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private CarModelGraphicControl carModelGraphicControl1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button buttonStartSim;
        private System.Windows.Forms.TextBox textBoxPosX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxPosY;
        private System.Windows.Forms.TextBox textBoxOriAng;
        private System.Windows.Forms.TextBox textBoxOriX;
        private System.Windows.Forms.TextBox textBoxOriY;
        private System.Windows.Forms.Button buttonStopTraining;
        private System.Windows.Forms.CheckBox checkBoxSerial;
        private System.Windows.Forms.ComboBox comboBoxSerial;
        private System.Windows.Forms.TextBox textBoxSerial;
        private System.Windows.Forms.Button buttonStopSim;
        private System.Windows.Forms.Button buttonStartTraining;
        private System.Windows.Forms.PictureBox pictureBoxCamPreview;
        private System.Windows.Forms.CheckBox checkBoxShowCamImage;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem newObstacleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteObstacleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.TrackBar trackBarMu;
        private System.Windows.Forms.TextBox textBoxMu;
        private System.Windows.Forms.CheckBox checkBoxShowNNOutput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxAdaptiveMu;
        private System.Windows.Forms.CheckBox checkBoxCameraEnabled;
        private System.Windows.Forms.PictureBox pictureBoxBackground;
        private System.Windows.Forms.PictureBox pictureBoxCarMarker;
        private System.Windows.Forms.PictureBox pictureBoxFinishMarker;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonProcessSingleFrame;
        private System.Windows.Forms.CheckBox checkBoxCarEnable;
    }
}

