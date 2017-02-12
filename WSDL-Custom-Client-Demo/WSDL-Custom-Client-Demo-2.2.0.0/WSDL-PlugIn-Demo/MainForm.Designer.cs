namespace WsdlPlugInDemo
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lblOutside = new System.Windows.Forms.Label();
            this.lblInside = new System.Windows.Forms.Label();
            this.lblOutsideTemp = new System.Windows.Forms.Label();
            this.lblInsideTemp = new System.Windows.Forms.Label();
            this.btnQuit = new System.Windows.Forms.Button();
            this.lblTempOkHdr = new System.Windows.Forms.Label();
            this.lblCoolerHdr = new System.Windows.Forms.Label();
            this.lblTempOk = new System.Windows.Forms.Label();
            this.lblCooler = new System.Windows.Forms.Label();
            this.lblFanHdr = new System.Windows.Forms.Label();
            this.lblFanState = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nudSetPoint = new System.Windows.Forms.NumericUpDown();
            this.nudHysterisis = new System.Windows.Forms.NumericUpDown();
            this.nudThreshold = new System.Windows.Forms.NumericUpDown();
            this.lblAcTemp = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblAcState = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.nudAcSetPoint = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.lblAcTempOk = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.nudOutsideChannel = new System.Windows.Forms.NumericUpDown();
            this.nudInsideChannel = new System.Windows.Forms.NumericUpDown();
            this.nudAcChannel = new System.Windows.Forms.NumericUpDown();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.cbFanDisalbesAc = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.cbCoolingFanHouseCode = new System.Windows.Forms.ComboBox();
            this.cbMixingFanHouseCode = new System.Windows.Forms.ComboBox();
            this.cbAcHouseCode = new System.Windows.Forms.ComboBox();
            this.cbCoolingFanUnitCode = new System.Windows.Forms.ComboBox();
            this.cbAcUnitCode = new System.Windows.Forms.ComboBox();
            this.cbMixingFanUnitCode = new System.Windows.Forms.ComboBox();
            this.gbConfig = new System.Windows.Forms.GroupBox();
            this.lbSaveNeeded = new System.Windows.Forms.Label();
            this.cbAcMixingUnitCode = new System.Windows.Forms.ComboBox();
            this.cbAcMixingHouseCode = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudSetPoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHysterisis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAcSetPoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOutsideChannel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInsideChannel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAcChannel)).BeginInit();
            this.gbConfig.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblOutside
            // 
            this.lblOutside.AutoSize = true;
            this.lblOutside.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOutside.Location = new System.Drawing.Point(6, 26);
            this.lblOutside.Name = "lblOutside";
            this.lblOutside.Size = new System.Drawing.Size(63, 19);
            this.lblOutside.TabIndex = 0;
            this.lblOutside.Text = "Outside";
            // 
            // lblInside
            // 
            this.lblInside.AutoSize = true;
            this.lblInside.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInside.Location = new System.Drawing.Point(17, 57);
            this.lblInside.Name = "lblInside";
            this.lblInside.Size = new System.Drawing.Size(52, 19);
            this.lblInside.TabIndex = 1;
            this.lblInside.Text = "Inside";
            // 
            // lblOutsideTemp
            // 
            this.lblOutsideTemp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblOutsideTemp.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOutsideTemp.Location = new System.Drawing.Point(75, 23);
            this.lblOutsideTemp.Name = "lblOutsideTemp";
            this.lblOutsideTemp.Size = new System.Drawing.Size(54, 24);
            this.lblOutsideTemp.TabIndex = 2;
            this.lblOutsideTemp.Text = "-----";
            this.lblOutsideTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblInsideTemp
            // 
            this.lblInsideTemp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblInsideTemp.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInsideTemp.Location = new System.Drawing.Point(75, 54);
            this.lblInsideTemp.Name = "lblInsideTemp";
            this.lblInsideTemp.Size = new System.Drawing.Size(54, 24);
            this.lblInsideTemp.TabIndex = 3;
            this.lblInsideTemp.Text = "-----";
            this.lblInsideTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnQuit
            // 
            this.btnQuit.Location = new System.Drawing.Point(348, 415);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(46, 23);
            this.btnQuit.TabIndex = 6;
            this.btnQuit.Text = "EXIT";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // lblTempOkHdr
            // 
            this.lblTempOkHdr.AutoSize = true;
            this.lblTempOkHdr.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTempOkHdr.Location = new System.Drawing.Point(140, 58);
            this.lblTempOkHdr.Name = "lblTempOkHdr";
            this.lblTempOkHdr.Size = new System.Drawing.Size(78, 19);
            this.lblTempOkHdr.TabIndex = 7;
            this.lblTempOkHdr.Text = "Inside OK";
            // 
            // lblCoolerHdr
            // 
            this.lblCoolerHdr.AutoSize = true;
            this.lblCoolerHdr.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCoolerHdr.Location = new System.Drawing.Point(145, 26);
            this.lblCoolerHdr.Name = "lblCoolerHdr";
            this.lblCoolerHdr.Size = new System.Drawing.Size(73, 19);
            this.lblCoolerHdr.TabIndex = 8;
            this.lblCoolerHdr.Text = "Can Cool";
            // 
            // lblTempOk
            // 
            this.lblTempOk.BackColor = System.Drawing.Color.SlateGray;
            this.lblTempOk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTempOk.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTempOk.Location = new System.Drawing.Point(224, 57);
            this.lblTempOk.Name = "lblTempOk";
            this.lblTempOk.Size = new System.Drawing.Size(20, 20);
            this.lblTempOk.TabIndex = 9;
            this.lblTempOk.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCooler
            // 
            this.lblCooler.BackColor = System.Drawing.Color.SlateGray;
            this.lblCooler.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCooler.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCooler.Location = new System.Drawing.Point(224, 25);
            this.lblCooler.Name = "lblCooler";
            this.lblCooler.Size = new System.Drawing.Size(20, 20);
            this.lblCooler.TabIndex = 10;
            this.lblCooler.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFanHdr
            // 
            this.lblFanHdr.AutoSize = true;
            this.lblFanHdr.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFanHdr.Location = new System.Drawing.Point(17, 125);
            this.lblFanHdr.Name = "lblFanHdr";
            this.lblFanHdr.Size = new System.Drawing.Size(74, 19);
            this.lblFanHdr.TabIndex = 11;
            this.lblFanHdr.Text = "Fan State";
            // 
            // lblFanState
            // 
            this.lblFanState.BackColor = System.Drawing.Color.SlateGray;
            this.lblFanState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFanState.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFanState.Location = new System.Drawing.Point(97, 124);
            this.lblFanState.Name = "lblFanState";
            this.lblFanState.Size = new System.Drawing.Size(20, 20);
            this.lblFanState.TabIndex = 12;
            this.lblFanState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 19);
            this.label1.TabIndex = 13;
            this.label1.Text = "Inside Set Point";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 19);
            this.label2.TabIndex = 14;
            this.label2.Text = "Hysterisis";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(147, 19);
            this.label3.TabIndex = 15;
            this.label3.Text = "Fan Cool Threshold";
            // 
            // nudSetPoint
            // 
            this.nudSetPoint.BackColor = System.Drawing.Color.SlateGray;
            this.nudSetPoint.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudSetPoint.Location = new System.Drawing.Point(164, 27);
            this.nudSetPoint.Maximum = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.nudSetPoint.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudSetPoint.Name = "nudSetPoint";
            this.nudSetPoint.Size = new System.Drawing.Size(55, 27);
            this.nudSetPoint.TabIndex = 16;
            this.nudSetPoint.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudSetPoint.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudSetPoint.ValueChanged += new System.EventHandler(this.nudSetPoint_ValueChanged);
            // 
            // nudHysterisis
            // 
            this.nudHysterisis.BackColor = System.Drawing.Color.SlateGray;
            this.nudHysterisis.DecimalPlaces = 1;
            this.nudHysterisis.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudHysterisis.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudHysterisis.Location = new System.Drawing.Point(164, 93);
            this.nudHysterisis.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            65536});
            this.nudHysterisis.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.nudHysterisis.Name = "nudHysterisis";
            this.nudHysterisis.Size = new System.Drawing.Size(55, 27);
            this.nudHysterisis.TabIndex = 17;
            this.nudHysterisis.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudHysterisis.Value = new decimal(new int[] {
            20,
            0,
            0,
            65536});
            this.nudHysterisis.ValueChanged += new System.EventHandler(this.nudHysterisis_ValueChanged);
            // 
            // nudThreshold
            // 
            this.nudThreshold.BackColor = System.Drawing.Color.SlateGray;
            this.nudThreshold.DecimalPlaces = 1;
            this.nudThreshold.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudThreshold.Location = new System.Drawing.Point(164, 126);
            this.nudThreshold.Maximum = new decimal(new int[] {
            150,
            0,
            0,
            65536});
            this.nudThreshold.Name = "nudThreshold";
            this.nudThreshold.Size = new System.Drawing.Size(55, 27);
            this.nudThreshold.TabIndex = 18;
            this.nudThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudThreshold.Value = new decimal(new int[] {
            20,
            0,
            0,
            65536});
            this.nudThreshold.ValueChanged += new System.EventHandler(this.nudThreshold_ValueChanged);
            // 
            // lblAcTemp
            // 
            this.lblAcTemp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAcTemp.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAcTemp.Location = new System.Drawing.Point(75, 87);
            this.lblAcTemp.Name = "lblAcTemp";
            this.lblAcTemp.Size = new System.Drawing.Size(54, 24);
            this.lblAcTemp.TabIndex = 20;
            this.lblAcTemp.Text = "-----";
            this.lblAcTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(17, 90);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 19);
            this.label5.TabIndex = 19;
            this.label5.Text = "A/C";
            // 
            // lblAcState
            // 
            this.lblAcState.BackColor = System.Drawing.Color.SlateGray;
            this.lblAcState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAcState.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAcState.Location = new System.Drawing.Point(222, 124);
            this.lblAcState.Name = "lblAcState";
            this.lblAcState.Size = new System.Drawing.Size(20, 20);
            this.lblAcState.TabIndex = 22;
            this.lblAcState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(142, 125);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(76, 19);
            this.label7.TabIndex = 21;
            this.label7.Text = "A/C State";
            // 
            // nudAcSetPoint
            // 
            this.nudAcSetPoint.BackColor = System.Drawing.Color.SlateGray;
            this.nudAcSetPoint.DecimalPlaces = 1;
            this.nudAcSetPoint.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudAcSetPoint.Increment = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.nudAcSetPoint.Location = new System.Drawing.Point(164, 60);
            this.nudAcSetPoint.Maximum = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.nudAcSetPoint.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudAcSetPoint.Name = "nudAcSetPoint";
            this.nudAcSetPoint.Size = new System.Drawing.Size(55, 27);
            this.nudAcSetPoint.TabIndex = 24;
            this.nudAcSetPoint.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudAcSetPoint.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudAcSetPoint.ValueChanged += new System.EventHandler(this.nudAcSetPoint_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 19);
            this.label4.TabIndex = 23;
            this.label4.Text = "A/C Set Point";
            // 
            // lblAcTempOk
            // 
            this.lblAcTempOk.BackColor = System.Drawing.Color.SlateGray;
            this.lblAcTempOk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAcTempOk.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAcTempOk.Location = new System.Drawing.Point(222, 89);
            this.lblAcTempOk.Name = "lblAcTempOk";
            this.lblAcTempOk.Size = new System.Drawing.Size(20, 20);
            this.lblAcTempOk.TabIndex = 26;
            this.lblAcTempOk.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(154, 90);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 19);
            this.label8.TabIndex = 25;
            this.label8.Text = "A/C OK";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(37, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 19);
            this.label6.TabIndex = 27;
            this.label6.Text = "WSDL Channels";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // nudOutsideChannel
            // 
            this.nudOutsideChannel.BackColor = System.Drawing.Color.SlateGray;
            this.nudOutsideChannel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudOutsideChannel.Location = new System.Drawing.Point(106, 67);
            this.nudOutsideChannel.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.nudOutsideChannel.Name = "nudOutsideChannel";
            this.nudOutsideChannel.Size = new System.Drawing.Size(55, 27);
            this.nudOutsideChannel.TabIndex = 28;
            this.nudOutsideChannel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudOutsideChannel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudOutsideChannel.ValueChanged += new System.EventHandler(this.nudOutsideChannel_ValueChanged);
            // 
            // nudInsideChannel
            // 
            this.nudInsideChannel.BackColor = System.Drawing.Color.SlateGray;
            this.nudInsideChannel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudInsideChannel.Location = new System.Drawing.Point(106, 99);
            this.nudInsideChannel.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.nudInsideChannel.Name = "nudInsideChannel";
            this.nudInsideChannel.Size = new System.Drawing.Size(55, 27);
            this.nudInsideChannel.TabIndex = 29;
            this.nudInsideChannel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudInsideChannel.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.nudInsideChannel.ValueChanged += new System.EventHandler(this.nudInsideChannel_ValueChanged);
            // 
            // nudAcChannel
            // 
            this.nudAcChannel.BackColor = System.Drawing.Color.SlateGray;
            this.nudAcChannel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudAcChannel.Location = new System.Drawing.Point(106, 131);
            this.nudAcChannel.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.nudAcChannel.Name = "nudAcChannel";
            this.nudAcChannel.Size = new System.Drawing.Size(55, 27);
            this.nudAcChannel.TabIndex = 30;
            this.nudAcChannel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudAcChannel.Value = new decimal(new int[] {
            13,
            0,
            0,
            0});
            this.nudAcChannel.ValueChanged += new System.EventHandler(this.nudAcChannel_ValueChanged);
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveSettings.Location = new System.Drawing.Point(74, 414);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(119, 25);
            this.btnSaveSettings.TabIndex = 31;
            this.btnSaveSettings.Text = "Save Settings";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // cbFanDisalbesAc
            // 
            this.cbFanDisalbesAc.AutoSize = true;
            this.cbFanDisalbesAc.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbFanDisalbesAc.Location = new System.Drawing.Point(12, 159);
            this.cbFanDisalbesAc.Name = "cbFanDisalbesAc";
            this.cbFanDisalbesAc.Size = new System.Drawing.Size(151, 24);
            this.cbFanDisalbesAc.TabIndex = 32;
            this.cbFanDisalbesAc.Text = "Fan Disables A/C";
            this.cbFanDisalbesAc.UseVisualStyleBackColor = true;
            this.cbFanDisalbesAc.CheckedChanged += new System.EventHandler(this.cbFanDisalbesAc_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(48, 133);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 19);
            this.label9.TabIndex = 35;
            this.label9.Text = "A/C";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(48, 100);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 19);
            this.label10.TabIndex = 34;
            this.label10.Text = "Inside";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(37, 69);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 19);
            this.label11.TabIndex = 33;
            this.label11.Text = "Outside";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(236, 16);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(102, 19);
            this.label12.TabIndex = 36;
            this.label12.Text = "X10 Switches";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(237, 113);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(117, 19);
            this.label13.TabIndex = 39;
            this.label13.Text = "A/C Mixing Fan";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(269, 87);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(85, 19);
            this.label14.TabIndex = 38;
            this.label14.Text = "Mixing Fan";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(261, 57);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(93, 19);
            this.label15.TabIndex = 37;
            this.label15.Text = "Cooling Fan";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(350, 11);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(116, 38);
            this.label16.TabIndex = 40;
            this.label16.Text = "House     Unit\r\n Code      Code";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbCoolingFanHouseCode
            // 
            this.cbCoolingFanHouseCode.BackColor = System.Drawing.Color.SlateGray;
            this.cbCoolingFanHouseCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCoolingFanHouseCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbCoolingFanHouseCode.FormattingEnabled = true;
            this.cbCoolingFanHouseCode.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P"});
            this.cbCoolingFanHouseCode.Location = new System.Drawing.Point(360, 57);
            this.cbCoolingFanHouseCode.MaxDropDownItems = 16;
            this.cbCoolingFanHouseCode.Name = "cbCoolingFanHouseCode";
            this.cbCoolingFanHouseCode.Size = new System.Drawing.Size(38, 24);
            this.cbCoolingFanHouseCode.TabIndex = 41;
            // 
            // cbMixingFanHouseCode
            // 
            this.cbMixingFanHouseCode.BackColor = System.Drawing.Color.SlateGray;
            this.cbMixingFanHouseCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMixingFanHouseCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbMixingFanHouseCode.FormattingEnabled = true;
            this.cbMixingFanHouseCode.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P"});
            this.cbMixingFanHouseCode.Location = new System.Drawing.Point(360, 85);
            this.cbMixingFanHouseCode.MaxDropDownItems = 16;
            this.cbMixingFanHouseCode.Name = "cbMixingFanHouseCode";
            this.cbMixingFanHouseCode.Size = new System.Drawing.Size(38, 24);
            this.cbMixingFanHouseCode.TabIndex = 42;
            // 
            // cbAcHouseCode
            // 
            this.cbAcHouseCode.BackColor = System.Drawing.Color.SlateGray;
            this.cbAcHouseCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAcHouseCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbAcHouseCode.FormattingEnabled = true;
            this.cbAcHouseCode.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P"});
            this.cbAcHouseCode.Location = new System.Drawing.Point(360, 143);
            this.cbAcHouseCode.MaxDropDownItems = 16;
            this.cbAcHouseCode.Name = "cbAcHouseCode";
            this.cbAcHouseCode.Size = new System.Drawing.Size(38, 24);
            this.cbAcHouseCode.TabIndex = 43;
            // 
            // cbCoolingFanUnitCode
            // 
            this.cbCoolingFanUnitCode.BackColor = System.Drawing.Color.SlateGray;
            this.cbCoolingFanUnitCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCoolingFanUnitCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbCoolingFanUnitCode.FormattingEnabled = true;
            this.cbCoolingFanUnitCode.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16"});
            this.cbCoolingFanUnitCode.Location = new System.Drawing.Point(418, 57);
            this.cbCoolingFanUnitCode.MaxDropDownItems = 16;
            this.cbCoolingFanUnitCode.Name = "cbCoolingFanUnitCode";
            this.cbCoolingFanUnitCode.Size = new System.Drawing.Size(45, 24);
            this.cbCoolingFanUnitCode.TabIndex = 44;
            // 
            // cbAcUnitCode
            // 
            this.cbAcUnitCode.BackColor = System.Drawing.Color.SlateGray;
            this.cbAcUnitCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAcUnitCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbAcUnitCode.FormattingEnabled = true;
            this.cbAcUnitCode.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16"});
            this.cbAcUnitCode.Location = new System.Drawing.Point(418, 143);
            this.cbAcUnitCode.MaxDropDownItems = 16;
            this.cbAcUnitCode.Name = "cbAcUnitCode";
            this.cbAcUnitCode.Size = new System.Drawing.Size(45, 24);
            this.cbAcUnitCode.TabIndex = 45;
            // 
            // cbMixingFanUnitCode
            // 
            this.cbMixingFanUnitCode.BackColor = System.Drawing.Color.SlateGray;
            this.cbMixingFanUnitCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMixingFanUnitCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbMixingFanUnitCode.FormattingEnabled = true;
            this.cbMixingFanUnitCode.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16"});
            this.cbMixingFanUnitCode.Location = new System.Drawing.Point(418, 87);
            this.cbMixingFanUnitCode.MaxDropDownItems = 16;
            this.cbMixingFanUnitCode.Name = "cbMixingFanUnitCode";
            this.cbMixingFanUnitCode.Size = new System.Drawing.Size(45, 24);
            this.cbMixingFanUnitCode.TabIndex = 46;
            // 
            // gbConfig
            // 
            this.gbConfig.Controls.Add(this.lbSaveNeeded);
            this.gbConfig.Controls.Add(this.cbAcMixingUnitCode);
            this.gbConfig.Controls.Add(this.cbAcMixingHouseCode);
            this.gbConfig.Controls.Add(this.label17);
            this.gbConfig.Controls.Add(this.label6);
            this.gbConfig.Controls.Add(this.cbMixingFanUnitCode);
            this.gbConfig.Controls.Add(this.nudOutsideChannel);
            this.gbConfig.Controls.Add(this.cbAcUnitCode);
            this.gbConfig.Controls.Add(this.nudInsideChannel);
            this.gbConfig.Controls.Add(this.cbCoolingFanUnitCode);
            this.gbConfig.Controls.Add(this.nudAcChannel);
            this.gbConfig.Controls.Add(this.cbAcHouseCode);
            this.gbConfig.Controls.Add(this.label11);
            this.gbConfig.Controls.Add(this.cbMixingFanHouseCode);
            this.gbConfig.Controls.Add(this.label10);
            this.gbConfig.Controls.Add(this.cbCoolingFanHouseCode);
            this.gbConfig.Controls.Add(this.label9);
            this.gbConfig.Controls.Add(this.label16);
            this.gbConfig.Controls.Add(this.label12);
            this.gbConfig.Controls.Add(this.label13);
            this.gbConfig.Controls.Add(this.label15);
            this.gbConfig.Controls.Add(this.label14);
            this.gbConfig.Location = new System.Drawing.Point(12, 12);
            this.gbConfig.Name = "gbConfig";
            this.gbConfig.Size = new System.Drawing.Size(502, 200);
            this.gbConfig.TabIndex = 47;
            this.gbConfig.TabStop = false;
            this.gbConfig.Text = "WSDL and X10 Configuration";
            // 
            // lbSaveNeeded
            // 
            this.lbSaveNeeded.AutoSize = true;
            this.lbSaveNeeded.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.lbSaveNeeded.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbSaveNeeded.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSaveNeeded.Location = new System.Drawing.Point(24, 172);
            this.lbSaveNeeded.Name = "lbSaveNeeded";
            this.lbSaveNeeded.Size = new System.Drawing.Size(454, 21);
            this.lbSaveNeeded.TabIndex = 50;
            this.lbSaveNeeded.Text = "Modified X10 codes will not take effect until settings are saved.";
            // 
            // cbAcMixingUnitCode
            // 
            this.cbAcMixingUnitCode.BackColor = System.Drawing.Color.SlateGray;
            this.cbAcMixingUnitCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAcMixingUnitCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbAcMixingUnitCode.FormattingEnabled = true;
            this.cbAcMixingUnitCode.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16"});
            this.cbAcMixingUnitCode.Location = new System.Drawing.Point(418, 113);
            this.cbAcMixingUnitCode.MaxDropDownItems = 16;
            this.cbAcMixingUnitCode.Name = "cbAcMixingUnitCode";
            this.cbAcMixingUnitCode.Size = new System.Drawing.Size(45, 24);
            this.cbAcMixingUnitCode.TabIndex = 49;
            // 
            // cbAcMixingHouseCode
            // 
            this.cbAcMixingHouseCode.BackColor = System.Drawing.Color.SlateGray;
            this.cbAcMixingHouseCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAcMixingHouseCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbAcMixingHouseCode.FormattingEnabled = true;
            this.cbAcMixingHouseCode.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P"});
            this.cbAcMixingHouseCode.Location = new System.Drawing.Point(360, 113);
            this.cbAcMixingHouseCode.MaxDropDownItems = 16;
            this.cbAcMixingHouseCode.Name = "cbAcMixingHouseCode";
            this.cbAcMixingHouseCode.Size = new System.Drawing.Size(38, 24);
            this.cbAcMixingHouseCode.TabIndex = 48;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(318, 143);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(36, 19);
            this.label17.TabIndex = 47;
            this.label17.Text = "A/C";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbFanDisalbesAc);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.nudSetPoint);
            this.groupBox1.Controls.Add(this.nudHysterisis);
            this.groupBox1.Controls.Add(this.nudThreshold);
            this.groupBox1.Controls.Add(this.nudAcSetPoint);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(12, 218);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(233, 191);
            this.groupBox1.TabIndex = 48;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Setpoints";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblOutside);
            this.groupBox2.Controls.Add(this.lblInside);
            this.groupBox2.Controls.Add(this.lblOutsideTemp);
            this.groupBox2.Controls.Add(this.lblInsideTemp);
            this.groupBox2.Controls.Add(this.lblAcState);
            this.groupBox2.Controls.Add(this.lblAcTempOk);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.lblTempOkHdr);
            this.groupBox2.Controls.Add(this.lblFanState);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.lblFanHdr);
            this.groupBox2.Controls.Add(this.lblCoolerHdr);
            this.groupBox2.Controls.Add(this.lblTempOk);
            this.groupBox2.Controls.Add(this.lblCooler);
            this.groupBox2.Controls.Add(this.lblAcTemp);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(251, 218);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(262, 191);
            this.groupBox2.TabIndex = 49;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Status";
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.SlateGray;
            this.ClientSize = new System.Drawing.Size(531, 452);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbConfig);
            this.Controls.Add(this.btnSaveSettings);
            this.Controls.Add(this.btnQuit);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "WSDL X10 Fan Controller";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.nudSetPoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHysterisis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAcSetPoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOutsideChannel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInsideChannel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAcChannel)).EndInit();
            this.gbConfig.ResumeLayout(false);
            this.gbConfig.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblOutside;
        private System.Windows.Forms.Label lblInside;
        private System.Windows.Forms.Label lblOutsideTemp;
        private System.Windows.Forms.Label lblInsideTemp;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Label lblTempOkHdr;
        private System.Windows.Forms.Label lblCoolerHdr;
        private System.Windows.Forms.Label lblTempOk;
        private System.Windows.Forms.Label lblCooler;
        private System.Windows.Forms.Label lblFanHdr;
        private System.Windows.Forms.Label lblFanState;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudSetPoint;
        private System.Windows.Forms.NumericUpDown nudHysterisis;
        private System.Windows.Forms.NumericUpDown nudThreshold;
        private System.Windows.Forms.Label lblAcTemp;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblAcState;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nudAcSetPoint;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblAcTempOk;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudOutsideChannel;
        private System.Windows.Forms.NumericUpDown nudInsideChannel;
        private System.Windows.Forms.NumericUpDown nudAcChannel;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.CheckBox cbFanDisalbesAc;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cbCoolingFanHouseCode;
        private System.Windows.Forms.ComboBox cbMixingFanHouseCode;
        private System.Windows.Forms.ComboBox cbAcHouseCode;
        private System.Windows.Forms.ComboBox cbCoolingFanUnitCode;
        private System.Windows.Forms.ComboBox cbAcUnitCode;
        private System.Windows.Forms.ComboBox cbMixingFanUnitCode;
        private System.Windows.Forms.GroupBox gbConfig;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cbAcMixingUnitCode;
        private System.Windows.Forms.ComboBox cbAcMixingHouseCode;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lbSaveNeeded;
    }
}

