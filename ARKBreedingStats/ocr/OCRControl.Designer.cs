using System;

namespace ARKBreedingStats.ocr
{
    partial class OCRControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OCRControl));
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.OCRDebugLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tabControlManage = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.LbReplacingsFileStatus = new System.Windows.Forms.Label();
            this.BtReplacingLoadFile = new System.Windows.Forms.Button();
            this.BtReplacingOpenFile = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.LlOcrManual = new System.Windows.Forms.LinkLabel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.BtNewOcrConfig = new System.Windows.Forms.Button();
            this.BtUnloadOCR = new System.Windows.Forms.Button();
            this.labelOCRFile = new System.Windows.Forms.Label();
            this.BtSaveOCRconfig = new System.Windows.Forms.Button();
            this.BtLoadOCRTemplate = new System.Windows.Forms.Button();
            this.BtSaveOCRConfigAs = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.ListBoxPatternsOfString = new System.Windows.Forms.ListBox();
            this.btnSaveOCRConfigFile2 = new System.Windows.Forms.Button();
            this.cbEnableOutput = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOCROutput = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.listBoxRecognized = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BtRemovePattern = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSaveTemplate = new System.Windows.Forms.Button();
            this.textBoxTemplate = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.nudWhiteTreshold = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonSaveAsTemplate = new System.Windows.Forms.Button();
            this.labelMatching = new System.Windows.Forms.Label();
            this.BtCopyPatternRecognizedToTemplate = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.BtRemoveSelectedPatterns = new System.Windows.Forms.Button();
            this.TbRemovePatterns = new System.Windows.Forms.TextBox();
            this.BtRemoveAllPatterns = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.buttonGetResFromScreenshot = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.CbSkipNameRecognition = new System.Windows.Forms.CheckBox();
            this.CbSkipTribeRecognition = new System.Windows.Forms.CheckBox();
            this.CbSkipOwnerRecognition = new System.Windows.Forms.CheckBox();
            this.CbTrainRecognition = new System.Windows.Forms.CheckBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.lbResizeResult = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.BtCreateOcrPatternsFromManualChars = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.BtCreateOcrPatternsForLabels = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxCalibrationText = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.TbLabelSetName = new System.Windows.Forms.TextBox();
            this.BtDeleteLabelSet = new System.Windows.Forms.Button();
            this.BtNewLabelSet = new System.Windows.Forms.Button();
            this.CbbLabelSets = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.BtSetStatPositionBasedOnFirstTwo = new System.Windows.Forms.Button();
            this.chkbSetAllStatLabels = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.listBoxLabelRectangles = new System.Windows.Forms.ListBox();
            this.ocrLetterEditTemplate = new ARKBreedingStats.ocr.OCRLetterEdit();
            this.ocrLetterEditRecognized = new ARKBreedingStats.ocr.OCRLetterEdit();
            this.nudResolutionHeight = new ARKBreedingStats.uiControls.Nud();
            this.nudResolutionWidth = new ARKBreedingStats.uiControls.Nud();
            this.nudResizing = new ARKBreedingStats.uiControls.Nud();
            this.nudFontSizeCalibration = new ARKBreedingStats.uiControls.Nud();
            this.nudHeightT = new ARKBreedingStats.uiControls.Nud();
            this.nudWidthL = new ARKBreedingStats.uiControls.Nud();
            this.nudHeight = new ARKBreedingStats.uiControls.Nud();
            this.nudWidth = new ARKBreedingStats.uiControls.Nud();
            this.nudY = new ARKBreedingStats.uiControls.Nud();
            this.nudX = new ARKBreedingStats.uiControls.Nud();
            this.tableLayoutPanel4.SuspendLayout();
            this.tabControlManage.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWhiteTreshold)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ocrLetterEditTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ocrLetterEditRecognized)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudResolutionHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudResolutionWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudResizing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontSizeCalibration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeightT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidthL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudX)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 360F));
            this.tableLayoutPanel4.Controls.Add(this.OCRDebugLayoutPanel, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tabControlManage, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(807, 726);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // OCRDebugLayoutPanel
            // 
            this.OCRDebugLayoutPanel.AllowDrop = true;
            this.OCRDebugLayoutPanel.AutoScroll = true;
            this.OCRDebugLayoutPanel.BackColor = System.Drawing.Color.LightSteelBlue;
            this.OCRDebugLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OCRDebugLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.OCRDebugLayoutPanel.Name = "OCRDebugLayoutPanel";
            this.OCRDebugLayoutPanel.Size = new System.Drawing.Size(441, 720);
            this.OCRDebugLayoutPanel.TabIndex = 0;
            this.OCRDebugLayoutPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.OCRDebugLayoutPanel_DragDrop);
            this.OCRDebugLayoutPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.OCRDebugLayoutPanel_DragEnter);
            // 
            // tabControlManage
            // 
            this.tabControlManage.Controls.Add(this.tabPage1);
            this.tabControlManage.Controls.Add(this.tabPage2);
            this.tabControlManage.Controls.Add(this.tabPage4);
            this.tabControlManage.Controls.Add(this.tabPage3);
            this.tabControlManage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlManage.Location = new System.Drawing.Point(450, 3);
            this.tabControlManage.Name = "tabControlManage";
            this.tabControlManage.SelectedIndex = 0;
            this.tabControlManage.Size = new System.Drawing.Size(354, 720);
            this.tabControlManage.TabIndex = 15;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add(this.groupBox11);
            this.tabPage1.Controls.Add(this.LlOcrManual);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(346, 694);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "OCR Info";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.LbReplacingsFileStatus);
            this.groupBox11.Controls.Add(this.BtReplacingLoadFile);
            this.groupBox11.Controls.Add(this.BtReplacingOpenFile);
            this.groupBox11.Controls.Add(this.label18);
            this.groupBox11.Location = new System.Drawing.Point(6, 420);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(337, 185);
            this.groupBox11.TabIndex = 3;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Manual corrections";
            // 
            // LbReplacingsFileStatus
            // 
            this.LbReplacingsFileStatus.AutoSize = true;
            this.LbReplacingsFileStatus.Location = new System.Drawing.Point(6, 157);
            this.LbReplacingsFileStatus.Name = "LbReplacingsFileStatus";
            this.LbReplacingsFileStatus.Size = new System.Drawing.Size(0, 13);
            this.LbReplacingsFileStatus.TabIndex = 3;
            // 
            // BtReplacingLoadFile
            // 
            this.BtReplacingLoadFile.Location = new System.Drawing.Point(87, 128);
            this.BtReplacingLoadFile.Name = "BtReplacingLoadFile";
            this.BtReplacingLoadFile.Size = new System.Drawing.Size(113, 23);
            this.BtReplacingLoadFile.TabIndex = 2;
            this.BtReplacingLoadFile.Text = "Load replacings";
            this.BtReplacingLoadFile.UseVisualStyleBackColor = true;
            this.BtReplacingLoadFile.Click += new System.EventHandler(this.BtReplacingLoadFile_Click);
            // 
            // BtReplacingOpenFile
            // 
            this.BtReplacingOpenFile.Location = new System.Drawing.Point(6, 128);
            this.BtReplacingOpenFile.Name = "BtReplacingOpenFile";
            this.BtReplacingOpenFile.Size = new System.Drawing.Size(75, 23);
            this.BtReplacingOpenFile.TabIndex = 1;
            this.BtReplacingOpenFile.Text = "Open file";
            this.BtReplacingOpenFile.UseVisualStyleBackColor = true;
            this.BtReplacingOpenFile.Click += new System.EventHandler(this.BtReplacingOpenFile_Click);
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(6, 16);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(325, 109);
            this.label18.TabIndex = 0;
            this.label18.Text = resources.GetString("label18.Text");
            // 
            // LlOcrManual
            // 
            this.LlOcrManual.AutoSize = true;
            this.LlOcrManual.Location = new System.Drawing.Point(6, 152);
            this.LlOcrManual.Name = "LlOcrManual";
            this.LlOcrManual.Size = new System.Drawing.Size(67, 13);
            this.LlOcrManual.TabIndex = 2;
            this.LlOcrManual.TabStop = true;
            this.LlOcrManual.Text = "OCR manual";
            this.LlOcrManual.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LlOcrManual_LinkClicked);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.BtNewOcrConfig);
            this.groupBox6.Controls.Add(this.BtUnloadOCR);
            this.groupBox6.Controls.Add(this.labelOCRFile);
            this.groupBox6.Controls.Add(this.BtSaveOCRconfig);
            this.groupBox6.Controls.Add(this.BtLoadOCRTemplate);
            this.groupBox6.Controls.Add(this.BtSaveOCRConfigAs);
            this.groupBox6.Location = new System.Drawing.Point(6, 219);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(334, 195);
            this.groupBox6.TabIndex = 1;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "OCR config file";
            // 
            // BtNewOcrConfig
            // 
            this.BtNewOcrConfig.Location = new System.Drawing.Point(6, 19);
            this.BtNewOcrConfig.Name = "BtNewOcrConfig";
            this.BtNewOcrConfig.Size = new System.Drawing.Size(94, 23);
            this.BtNewOcrConfig.TabIndex = 0;
            this.BtNewOcrConfig.Text = "new";
            this.BtNewOcrConfig.UseVisualStyleBackColor = true;
            this.BtNewOcrConfig.Click += new System.EventHandler(this.BtNewOcrConfig_Click);
            // 
            // BtUnloadOCR
            // 
            this.BtUnloadOCR.Location = new System.Drawing.Point(106, 48);
            this.BtUnloadOCR.Name = "BtUnloadOCR";
            this.BtUnloadOCR.Size = new System.Drawing.Size(94, 23);
            this.BtUnloadOCR.TabIndex = 3;
            this.BtUnloadOCR.Text = "unload";
            this.BtUnloadOCR.UseVisualStyleBackColor = true;
            this.BtUnloadOCR.Click += new System.EventHandler(this.btUnloadOCR_Click);
            // 
            // labelOCRFile
            // 
            this.labelOCRFile.Location = new System.Drawing.Point(6, 74);
            this.labelOCRFile.Name = "labelOCRFile";
            this.labelOCRFile.Size = new System.Drawing.Size(322, 116);
            this.labelOCRFile.TabIndex = 5;
            this.labelOCRFile.Click += new System.EventHandler(this.labelOCRFile_Click);
            // 
            // BtSaveOCRconfig
            // 
            this.BtSaveOCRconfig.Location = new System.Drawing.Point(206, 19);
            this.BtSaveOCRconfig.Name = "BtSaveOCRconfig";
            this.BtSaveOCRconfig.Size = new System.Drawing.Size(94, 23);
            this.BtSaveOCRconfig.TabIndex = 2;
            this.BtSaveOCRconfig.Text = "save";
            this.BtSaveOCRconfig.UseVisualStyleBackColor = true;
            this.BtSaveOCRconfig.Click += new System.EventHandler(this.btnSaveOCRconfig_Click);
            // 
            // BtLoadOCRTemplate
            // 
            this.BtLoadOCRTemplate.Location = new System.Drawing.Point(106, 19);
            this.BtLoadOCRTemplate.Name = "BtLoadOCRTemplate";
            this.BtLoadOCRTemplate.Size = new System.Drawing.Size(94, 23);
            this.BtLoadOCRTemplate.TabIndex = 1;
            this.BtLoadOCRTemplate.Text = "load";
            this.BtLoadOCRTemplate.UseVisualStyleBackColor = true;
            this.BtLoadOCRTemplate.Click += new System.EventHandler(this.buttonLoadOCRTemplate_Click);
            // 
            // BtSaveOCRConfigAs
            // 
            this.BtSaveOCRConfigAs.Location = new System.Drawing.Point(206, 48);
            this.BtSaveOCRConfigAs.Name = "BtSaveOCRConfigAs";
            this.BtSaveOCRConfigAs.Size = new System.Drawing.Size(94, 23);
            this.BtSaveOCRConfigAs.TabIndex = 4;
            this.BtSaveOCRConfigAs.Text = "save as…";
            this.BtSaveOCRConfigAs.UseVisualStyleBackColor = true;
            this.BtSaveOCRConfigAs.Click += new System.EventHandler(this.btnSaveOCRConfigAs_Click);
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(6, 14);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(334, 122);
            this.label12.TabIndex = 0;
            this.label12.Text = resources.GetString("label12.Text");
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.ListBoxPatternsOfString);
            this.tabPage2.Controls.Add(this.btnSaveOCRConfigFile2);
            this.tabPage2.Controls.Add(this.cbEnableOutput);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.txtOCROutput);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.listBoxRecognized);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.nudWhiteTreshold);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(346, 694);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Output";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 564);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(287, 65);
            this.label1.TabIndex = 17;
            this.label1.Text = "To fix a wrong character recognition, select it in the list on the right, enter t" +
    "he correct Text in the Template Character input, then in the Recognized GroupBox" +
    " click on Save as template.";
            // 
            // ListBoxPatternsOfString
            // 
            this.ListBoxPatternsOfString.FormattingEnabled = true;
            this.ListBoxPatternsOfString.Location = new System.Drawing.Point(260, 268);
            this.ListBoxPatternsOfString.Name = "ListBoxPatternsOfString";
            this.ListBoxPatternsOfString.Size = new System.Drawing.Size(33, 251);
            this.ListBoxPatternsOfString.TabIndex = 4;
            this.ListBoxPatternsOfString.SelectedIndexChanged += new System.EventHandler(this.ListBoxPatternsOfString_SelectedIndexChanged);
            // 
            // btnSaveOCRConfigFile2
            // 
            this.btnSaveOCRConfigFile2.Location = new System.Drawing.Point(6, 632);
            this.btnSaveOCRConfigFile2.Name = "btnSaveOCRConfigFile2";
            this.btnSaveOCRConfigFile2.Size = new System.Drawing.Size(130, 39);
            this.btnSaveOCRConfigFile2.TabIndex = 8;
            this.btnSaveOCRConfigFile2.Text = "save current OCR config-file";
            this.btnSaveOCRConfigFile2.UseVisualStyleBackColor = true;
            this.btnSaveOCRConfigFile2.Click += new System.EventHandler(this.btnSaveOCRconfig_Click);
            // 
            // cbEnableOutput
            // 
            this.cbEnableOutput.AutoSize = true;
            this.cbEnableOutput.Location = new System.Drawing.Point(137, 6);
            this.cbEnableOutput.Name = "cbEnableOutput";
            this.cbEnableOutput.Size = new System.Drawing.Size(156, 17);
            this.cbEnableOutput.TabIndex = 0;
            this.cbEnableOutput.Text = "Enable Logging and Editing";
            this.cbEnableOutput.UseVisualStyleBackColor = true;
            this.cbEnableOutput.CheckedChanged += new System.EventHandler(this.cbEnableOutput_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 221);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(287, 44);
            this.label3.TabIndex = 2;
            this.label3.Text = "Here you can edit the OCR-character-templates. Click on a recognized character in" +
    " the list on the right to see as which character it was read and to edit it.";
            // 
            // txtOCROutput
            // 
            this.txtOCROutput.Location = new System.Drawing.Point(6, 32);
            this.txtOCROutput.Multiline = true;
            this.txtOCROutput.Name = "txtOCROutput";
            this.txtOCROutput.ReadOnly = true;
            this.txtOCROutput.Size = new System.Drawing.Size(287, 186);
            this.txtOCROutput.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "OCR Output";
            // 
            // listBoxRecognized
            // 
            this.listBoxRecognized.Dock = System.Windows.Forms.DockStyle.Right;
            this.listBoxRecognized.FormattingEnabled = true;
            this.listBoxRecognized.Location = new System.Drawing.Point(299, 3);
            this.listBoxRecognized.Name = "listBoxRecognized";
            this.listBoxRecognized.Size = new System.Drawing.Size(44, 688);
            this.listBoxRecognized.TabIndex = 9;
            this.listBoxRecognized.SelectedIndexChanged += new System.EventHandler(this.listBoxRecognized_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BtRemovePattern);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.ocrLetterEditTemplate);
            this.groupBox1.Controls.Add(this.btnSaveTemplate);
            this.groupBox1.Controls.Add(this.textBoxTemplate);
            this.groupBox1.Location = new System.Drawing.Point(6, 268);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(248, 126);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Template";
            // 
            // BtRemovePattern
            // 
            this.BtRemovePattern.Location = new System.Drawing.Point(6, 45);
            this.BtRemovePattern.Name = "BtRemovePattern";
            this.BtRemovePattern.Size = new System.Drawing.Size(59, 36);
            this.BtRemovePattern.TabIndex = 2;
            this.BtRemovePattern.Text = "Remove template";
            this.BtRemovePattern.UseVisualStyleBackColor = true;
            this.BtRemovePattern.Click += new System.EventHandler(this.BtRemovePattern_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Character";
            // 
            // btnSaveTemplate
            // 
            this.btnSaveTemplate.Location = new System.Drawing.Point(71, 45);
            this.btnSaveTemplate.Name = "btnSaveTemplate";
            this.btnSaveTemplate.Size = new System.Drawing.Size(59, 36);
            this.btnSaveTemplate.TabIndex = 3;
            this.btnSaveTemplate.Text = "Save template";
            this.btnSaveTemplate.UseVisualStyleBackColor = true;
            this.btnSaveTemplate.Click += new System.EventHandler(this.btnSaveTemplate_Click);
            // 
            // textBoxTemplate
            // 
            this.textBoxTemplate.Location = new System.Drawing.Point(71, 19);
            this.textBoxTemplate.MaxLength = 1;
            this.textBoxTemplate.Name = "textBoxTemplate";
            this.textBoxTemplate.Size = new System.Drawing.Size(59, 20);
            this.textBoxTemplate.TabIndex = 1;
            this.textBoxTemplate.TextChanged += new System.EventHandler(this.textBoxTemplate_TextChanged);
            this.textBoxTemplate.Enter += new System.EventHandler(this.textBoxTemplate_Enter);
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(61, 528);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(227, 33);
            this.label13.TabIndex = 7;
            this.label13.Text = "White Threshold (adjust until the characters that should be read are clearly dist" +
    "inguishable)";
            // 
            // nudWhiteTreshold
            // 
            this.nudWhiteTreshold.Location = new System.Drawing.Point(6, 531);
            this.nudWhiteTreshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudWhiteTreshold.Name = "nudWhiteTreshold";
            this.nudWhiteTreshold.Size = new System.Drawing.Size(49, 20);
            this.nudWhiteTreshold.TabIndex = 6;
            this.nudWhiteTreshold.Value = new decimal(new int[] {
            155,
            0,
            0,
            0});
            this.nudWhiteTreshold.ValueChanged += new System.EventHandler(this.nudWhiteTreshold_ValueChanged);
            this.nudWhiteTreshold.Leave += new System.EventHandler(this.nudWhiteTreshold_Leave);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonSaveAsTemplate);
            this.groupBox2.Controls.Add(this.labelMatching);
            this.groupBox2.Controls.Add(this.ocrLetterEditRecognized);
            this.groupBox2.Controls.Add(this.BtCopyPatternRecognizedToTemplate);
            this.groupBox2.Location = new System.Drawing.Point(6, 400);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(248, 125);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Recognized";
            // 
            // buttonSaveAsTemplate
            // 
            this.buttonSaveAsTemplate.Location = new System.Drawing.Point(71, 16);
            this.buttonSaveAsTemplate.Name = "buttonSaveAsTemplate";
            this.buttonSaveAsTemplate.Size = new System.Drawing.Size(59, 36);
            this.buttonSaveAsTemplate.TabIndex = 1;
            this.buttonSaveAsTemplate.Text = "Save as template";
            this.buttonSaveAsTemplate.UseVisualStyleBackColor = true;
            this.buttonSaveAsTemplate.Click += new System.EventHandler(this.buttonSaveAsTemplate_Click);
            // 
            // labelMatching
            // 
            this.labelMatching.Location = new System.Drawing.Point(6, 16);
            this.labelMatching.Name = "labelMatching";
            this.labelMatching.Size = new System.Drawing.Size(59, 36);
            this.labelMatching.TabIndex = 0;
            this.labelMatching.Text = "match";
            // 
            // BtCopyPatternRecognizedToTemplate
            // 
            this.BtCopyPatternRecognizedToTemplate.Location = new System.Drawing.Point(71, 58);
            this.BtCopyPatternRecognizedToTemplate.Name = "BtCopyPatternRecognizedToTemplate";
            this.BtCopyPatternRecognizedToTemplate.Size = new System.Drawing.Size(59, 36);
            this.BtCopyPatternRecognizedToTemplate.TabIndex = 2;
            this.BtCopyPatternRecognizedToTemplate.Text = "Copy to template";
            this.BtCopyPatternRecognizedToTemplate.UseVisualStyleBackColor = true;
            this.BtCopyPatternRecognizedToTemplate.Click += new System.EventHandler(this.BtCopyPatternRecognizedToTemplateClick);
            // 
            // tabPage4
            // 
            this.tabPage4.AutoScroll = true;
            this.tabPage4.Controls.Add(this.groupBox7);
            this.tabPage4.Controls.Add(this.groupBox5);
            this.tabPage4.Controls.Add(this.groupBox10);
            this.tabPage4.Controls.Add(this.groupBox9);
            this.tabPage4.Controls.Add(this.groupBox8);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(346, 694);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Manage";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.BtRemoveSelectedPatterns);
            this.groupBox7.Controls.Add(this.TbRemovePatterns);
            this.groupBox7.Controls.Add(this.BtRemoveAllPatterns);
            this.groupBox7.Location = new System.Drawing.Point(6, 538);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(334, 100);
            this.groupBox7.TabIndex = 0;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Remove Patterns";
            // 
            // BtRemoveSelectedPatterns
            // 
            this.BtRemoveSelectedPatterns.BackColor = System.Drawing.Color.LightSalmon;
            this.BtRemoveSelectedPatterns.ForeColor = System.Drawing.Color.Black;
            this.BtRemoveSelectedPatterns.Location = new System.Drawing.Point(94, 17);
            this.BtRemoveSelectedPatterns.Name = "BtRemoveSelectedPatterns";
            this.BtRemoveSelectedPatterns.Size = new System.Drawing.Size(234, 23);
            this.BtRemoveSelectedPatterns.TabIndex = 1;
            this.BtRemoveSelectedPatterns.Text = "Remove Patterns of these characters";
            this.BtRemoveSelectedPatterns.UseVisualStyleBackColor = false;
            this.BtRemoveSelectedPatterns.Click += new System.EventHandler(this.BtRemoveSelectedPatterns_Click);
            // 
            // TbRemovePatterns
            // 
            this.TbRemovePatterns.Location = new System.Drawing.Point(6, 19);
            this.TbRemovePatterns.Name = "TbRemovePatterns";
            this.TbRemovePatterns.Size = new System.Drawing.Size(82, 20);
            this.TbRemovePatterns.TabIndex = 0;
            // 
            // BtRemoveAllPatterns
            // 
            this.BtRemoveAllPatterns.BackColor = System.Drawing.Color.LightSalmon;
            this.BtRemoveAllPatterns.ForeColor = System.Drawing.Color.Black;
            this.BtRemoveAllPatterns.Location = new System.Drawing.Point(94, 71);
            this.BtRemoveAllPatterns.Name = "BtRemoveAllPatterns";
            this.BtRemoveAllPatterns.Size = new System.Drawing.Size(134, 23);
            this.BtRemoveAllPatterns.TabIndex = 2;
            this.BtRemoveAllPatterns.Text = "Remove all Patterns";
            this.BtRemoveAllPatterns.UseVisualStyleBackColor = false;
            this.BtRemoveAllPatterns.Click += new System.EventHandler(this.BtRemoveAllPatterns_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.buttonGetResFromScreenshot);
            this.groupBox5.Controls.Add(this.nudResolutionHeight);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.nudResolutionWidth);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Location = new System.Drawing.Point(6, 151);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(334, 77);
            this.groupBox5.TabIndex = 2;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Resolution";
            // 
            // buttonGetResFromScreenshot
            // 
            this.buttonGetResFromScreenshot.Location = new System.Drawing.Point(130, 19);
            this.buttonGetResFromScreenshot.Name = "buttonGetResFromScreenshot";
            this.buttonGetResFromScreenshot.Size = new System.Drawing.Size(103, 46);
            this.buttonGetResFromScreenshot.TabIndex = 4;
            this.buttonGetResFromScreenshot.Text = "Take resolution from screenshot";
            this.buttonGetResFromScreenshot.UseVisualStyleBackColor = true;
            this.buttonGetResFromScreenshot.Click += new System.EventHandler(this.buttonGetResFromScreenshot_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 47);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(38, 13);
            this.label16.TabIndex = 2;
            this.label16.Text = "Height";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 21);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(35, 13);
            this.label15.TabIndex = 0;
            this.label15.Text = "Width";
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.CbSkipNameRecognition);
            this.groupBox10.Controls.Add(this.CbSkipTribeRecognition);
            this.groupBox10.Controls.Add(this.CbSkipOwnerRecognition);
            this.groupBox10.Controls.Add(this.CbTrainRecognition);
            this.groupBox10.Location = new System.Drawing.Point(6, 6);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(334, 139);
            this.groupBox10.TabIndex = 1;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Pattern recognition settings";
            // 
            // CbSkipNameRecognition
            // 
            this.CbSkipNameRecognition.AutoSize = true;
            this.CbSkipNameRecognition.Location = new System.Drawing.Point(6, 42);
            this.CbSkipNameRecognition.Name = "CbSkipNameRecognition";
            this.CbSkipNameRecognition.Size = new System.Drawing.Size(131, 17);
            this.CbSkipNameRecognition.TabIndex = 1;
            this.CbSkipNameRecognition.Text = "Skip name recognition";
            this.CbSkipNameRecognition.UseVisualStyleBackColor = true;
            this.CbSkipNameRecognition.CheckedChanged += new System.EventHandler(this.CbSkipNameRecognition_CheckedChanged);
            // 
            // CbSkipTribeRecognition
            // 
            this.CbSkipTribeRecognition.AutoSize = true;
            this.CbSkipTribeRecognition.Location = new System.Drawing.Point(6, 65);
            this.CbSkipTribeRecognition.Name = "CbSkipTribeRecognition";
            this.CbSkipTribeRecognition.Size = new System.Drawing.Size(125, 17);
            this.CbSkipTribeRecognition.TabIndex = 2;
            this.CbSkipTribeRecognition.Text = "Skip tribe recognition";
            this.CbSkipTribeRecognition.UseVisualStyleBackColor = true;
            this.CbSkipTribeRecognition.CheckedChanged += new System.EventHandler(this.CbSkipTribeRecognition_CheckedChanged);
            // 
            // CbSkipOwnerRecognition
            // 
            this.CbSkipOwnerRecognition.AutoSize = true;
            this.CbSkipOwnerRecognition.Location = new System.Drawing.Point(6, 88);
            this.CbSkipOwnerRecognition.Name = "CbSkipOwnerRecognition";
            this.CbSkipOwnerRecognition.Size = new System.Drawing.Size(134, 17);
            this.CbSkipOwnerRecognition.TabIndex = 3;
            this.CbSkipOwnerRecognition.Text = "Skip owner recognition";
            this.CbSkipOwnerRecognition.UseVisualStyleBackColor = true;
            this.CbSkipOwnerRecognition.CheckedChanged += new System.EventHandler(this.CbSkipOwnerRecognition_CheckedChanged);
            // 
            // CbTrainRecognition
            // 
            this.CbTrainRecognition.AutoSize = true;
            this.CbTrainRecognition.Location = new System.Drawing.Point(6, 19);
            this.CbTrainRecognition.Name = "CbTrainRecognition";
            this.CbTrainRecognition.Size = new System.Drawing.Size(105, 17);
            this.CbTrainRecognition.TabIndex = 0;
            this.CbTrainRecognition.Text = "Train recognition";
            this.CbTrainRecognition.UseVisualStyleBackColor = true;
            this.CbTrainRecognition.CheckedChanged += new System.EventHandler(this.CbTrainRecognition_CheckedChanged);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.lbResizeResult);
            this.groupBox9.Controls.Add(this.nudResizing);
            this.groupBox9.Location = new System.Drawing.Point(6, 234);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(334, 94);
            this.groupBox9.TabIndex = 3;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Resize the captured screenshot";
            // 
            // lbResizeResult
            // 
            this.lbResizeResult.Location = new System.Drawing.Point(107, 19);
            this.lbResizeResult.Name = "lbResizeResult";
            this.lbResizeResult.Size = new System.Drawing.Size(221, 72);
            this.lbResizeResult.TabIndex = 1;
            this.lbResizeResult.Text = "->";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.BtCreateOcrPatternsFromManualChars);
            this.groupBox8.Controls.Add(this.label17);
            this.groupBox8.Controls.Add(this.nudFontSizeCalibration);
            this.groupBox8.Controls.Add(this.BtCreateOcrPatternsForLabels);
            this.groupBox8.Controls.Add(this.label14);
            this.groupBox8.Controls.Add(this.textBoxCalibrationText);
            this.groupBox8.Location = new System.Drawing.Point(6, 334);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(334, 198);
            this.groupBox8.TabIndex = 4;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Add OCR Patterns";
            // 
            // BtCreateOcrPatternsFromManualChars
            // 
            this.BtCreateOcrPatternsFromManualChars.Location = new System.Drawing.Point(144, 168);
            this.BtCreateOcrPatternsFromManualChars.Name = "BtCreateOcrPatternsFromManualChars";
            this.BtCreateOcrPatternsFromManualChars.Size = new System.Drawing.Size(184, 23);
            this.BtCreateOcrPatternsFromManualChars.TabIndex = 5;
            this.BtCreateOcrPatternsFromManualChars.Text = "Create custom OCR patterns";
            this.BtCreateOcrPatternsFromManualChars.UseVisualStyleBackColor = true;
            this.BtCreateOcrPatternsFromManualChars.Click += new System.EventHandler(this.BtCreateOcrPatternsFromManualChars_Click);
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(6, 83);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(322, 56);
            this.label17.TabIndex = 1;
            this.label17.Text = resources.GetString("label17.Text");
            // 
            // BtCreateOcrPatternsForLabels
            // 
            this.BtCreateOcrPatternsForLabels.Location = new System.Drawing.Point(6, 19);
            this.BtCreateOcrPatternsForLabels.Name = "BtCreateOcrPatternsForLabels";
            this.BtCreateOcrPatternsForLabels.Size = new System.Drawing.Size(322, 37);
            this.BtCreateOcrPatternsForLabels.TabIndex = 0;
            this.BtCreateOcrPatternsForLabels.Text = "Automatic creation of OCR patterns from a font file considering the label sizes";
            this.BtCreateOcrPatternsForLabels.UseVisualStyleBackColor = true;
            this.BtCreateOcrPatternsForLabels.Click += new System.EventHandler(this.buttonLoadCalibrationImage_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 173);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(46, 13);
            this.label14.TabIndex = 3;
            this.label14.Text = "font size";
            // 
            // textBoxCalibrationText
            // 
            this.textBoxCalibrationText.Location = new System.Drawing.Point(6, 142);
            this.textBoxCalibrationText.Name = "textBoxCalibrationText";
            this.textBoxCalibrationText.Size = new System.Drawing.Size(322, 20);
            this.textBoxCalibrationText.TabIndex = 2;
            this.textBoxCalibrationText.Text = "!#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqr" +
    "stuvwxyz{|}~";
            // 
            // tabPage3
            // 
            this.tabPage3.AutoScroll = true;
            this.tabPage3.Controls.Add(this.groupBox12);
            this.tabPage3.Controls.Add(this.BtDeleteLabelSet);
            this.tabPage3.Controls.Add(this.BtNewLabelSet);
            this.tabPage3.Controls.Add(this.CbbLabelSets);
            this.tabPage3.Controls.Add(this.groupBox4);
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(346, 694);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Labels";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.TbLabelSetName);
            this.groupBox12.Location = new System.Drawing.Point(6, 35);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(334, 45);
            this.groupBox12.TabIndex = 5;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Label set name";
            // 
            // TbLabelSetName
            // 
            this.TbLabelSetName.Location = new System.Drawing.Point(6, 19);
            this.TbLabelSetName.Name = "TbLabelSetName";
            this.TbLabelSetName.Size = new System.Drawing.Size(322, 20);
            this.TbLabelSetName.TabIndex = 0;
            this.TbLabelSetName.Leave += new System.EventHandler(this.TbLabelSetName_Leave);
            // 
            // BtDeleteLabelSet
            // 
            this.BtDeleteLabelSet.Location = new System.Drawing.Point(277, 6);
            this.BtDeleteLabelSet.Name = "BtDeleteLabelSet";
            this.BtDeleteLabelSet.Size = new System.Drawing.Size(63, 23);
            this.BtDeleteLabelSet.TabIndex = 4;
            this.BtDeleteLabelSet.Text = "delete";
            this.BtDeleteLabelSet.UseVisualStyleBackColor = true;
            this.BtDeleteLabelSet.Click += new System.EventHandler(this.BtDeleteLabelSet_Click);
            // 
            // BtNewLabelSet
            // 
            this.BtNewLabelSet.Location = new System.Drawing.Point(214, 6);
            this.BtNewLabelSet.Name = "BtNewLabelSet";
            this.BtNewLabelSet.Size = new System.Drawing.Size(57, 23);
            this.BtNewLabelSet.TabIndex = 3;
            this.BtNewLabelSet.Text = "new";
            this.BtNewLabelSet.UseVisualStyleBackColor = true;
            this.BtNewLabelSet.Click += new System.EventHandler(this.BtNewLabelSet_Click);
            // 
            // CbbLabelSets
            // 
            this.CbbLabelSets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbbLabelSets.Location = new System.Drawing.Point(6, 8);
            this.CbbLabelSets.Name = "CbbLabelSets";
            this.CbbLabelSets.Size = new System.Drawing.Size(202, 21);
            this.CbbLabelSets.TabIndex = 2;
            this.CbbLabelSets.SelectedIndexChanged += new System.EventHandler(this.CbbLabelSets_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.BtSetStatPositionBasedOnFirstTwo);
            this.groupBox4.Controls.Add(this.chkbSetAllStatLabels);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.nudHeightT);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.nudWidthL);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.nudHeight);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.nudWidth);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.nudY);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.nudX);
            this.groupBox4.Location = new System.Drawing.Point(6, 355);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(293, 214);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Rectangle";
            // 
            // BtSetStatPositionBasedOnFirstTwo
            // 
            this.BtSetStatPositionBasedOnFirstTwo.Location = new System.Drawing.Point(6, 176);
            this.BtSetStatPositionBasedOnFirstTwo.Name = "BtSetStatPositionBasedOnFirstTwo";
            this.BtSetStatPositionBasedOnFirstTwo.Size = new System.Drawing.Size(281, 23);
            this.BtSetStatPositionBasedOnFirstTwo.TabIndex = 14;
            this.BtSetStatPositionBasedOnFirstTwo.Text = "Set stat-positions based on HP and Stamina";
            this.BtSetStatPositionBasedOnFirstTwo.UseVisualStyleBackColor = true;
            this.BtSetStatPositionBasedOnFirstTwo.Click += new System.EventHandler(this.BtSetStatPositionBasedOnFirstTwo_Click);
            // 
            // chkbSetAllStatLabels
            // 
            this.chkbSetAllStatLabels.AutoSize = true;
            this.chkbSetAllStatLabels.Location = new System.Drawing.Point(6, 153);
            this.chkbSetAllStatLabels.Name = "chkbSetAllStatLabels";
            this.chkbSetAllStatLabels.Size = new System.Drawing.Size(206, 17);
            this.chkbSetAllStatLabels.TabIndex = 13;
            this.chkbSetAllStatLabels.Text = "Set Values (except Y) for all stat-labels";
            this.chkbSetAllStatLabels.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(139, 119);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Height-T";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 119);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Width-L";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(6, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(281, 43);
            this.label8.TabIndex = 0;
            this.label8.Text = "The Height has to be the same for all texts in the same size. The text-baseline h" +
    "as to be exact in the same position for all labels with the same text-size.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(149, 93);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Height";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 93);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Width";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(173, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Y";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "X";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.listBoxLabelRectangles);
            this.groupBox3.Location = new System.Drawing.Point(6, 86);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(221, 263);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Labelpositions";
            // 
            // listBoxLabelRectangles
            // 
            this.listBoxLabelRectangles.FormattingEnabled = true;
            this.listBoxLabelRectangles.Location = new System.Drawing.Point(6, 19);
            this.listBoxLabelRectangles.Name = "listBoxLabelRectangles";
            this.listBoxLabelRectangles.Size = new System.Drawing.Size(209, 238);
            this.listBoxLabelRectangles.TabIndex = 0;
            this.listBoxLabelRectangles.SelectedIndexChanged += new System.EventHandler(this.listBoxLabelRectangles_SelectedIndexChanged);
            // 
            // ocrLetterEditTemplate
            // 
            this.ocrLetterEditTemplate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ocrLetterEditTemplate.Location = new System.Drawing.Point(136, 19);
            this.ocrLetterEditTemplate.Name = "ocrLetterEditTemplate";
            this.ocrLetterEditTemplate.PatternDisplay = null;
            this.ocrLetterEditTemplate.Size = new System.Drawing.Size(102, 102);
            this.ocrLetterEditTemplate.TabIndex = 12;
            this.ocrLetterEditTemplate.TabStop = false;
            // 
            // ocrLetterEditRecognized
            // 
            this.ocrLetterEditRecognized.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ocrLetterEditRecognized.Location = new System.Drawing.Point(136, 16);
            this.ocrLetterEditRecognized.Name = "ocrLetterEditRecognized";
            this.ocrLetterEditRecognized.PatternDisplay = null;
            this.ocrLetterEditRecognized.Size = new System.Drawing.Size(102, 102);
            this.ocrLetterEditRecognized.TabIndex = 13;
            this.ocrLetterEditRecognized.TabStop = false;
            // 
            // nudResolutionHeight
            // 
            this.nudResolutionHeight.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudResolutionHeight.Location = new System.Drawing.Point(47, 45);
            this.nudResolutionHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudResolutionHeight.Name = "nudResolutionHeight";
            this.nudResolutionHeight.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudResolutionHeight.Size = new System.Drawing.Size(77, 20);
            this.nudResolutionHeight.TabIndex = 3;
            this.nudResolutionHeight.ValueChanged += new System.EventHandler(this.nudResolutionHeight_ValueChanged);
            // 
            // nudResolutionWidth
            // 
            this.nudResolutionWidth.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudResolutionWidth.Location = new System.Drawing.Point(47, 19);
            this.nudResolutionWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudResolutionWidth.Name = "nudResolutionWidth";
            this.nudResolutionWidth.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudResolutionWidth.Size = new System.Drawing.Size(77, 20);
            this.nudResolutionWidth.TabIndex = 1;
            this.nudResolutionWidth.ValueChanged += new System.EventHandler(this.nudResolutionWidth_ValueChanged);
            // 
            // nudResizing
            // 
            this.nudResizing.DecimalPlaces = 6;
            this.nudResizing.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudResizing.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudResizing.Location = new System.Drawing.Point(6, 19);
            this.nudResizing.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudResizing.Name = "nudResizing";
            this.nudResizing.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudResizing.Size = new System.Drawing.Size(95, 20);
            this.nudResizing.TabIndex = 0;
            this.nudResizing.ValueChanged += new System.EventHandler(this.nudResizing_ValueChanged);
            // 
            // nudFontSizeCalibration
            // 
            this.nudFontSizeCalibration.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nudFontSizeCalibration.Location = new System.Drawing.Point(58, 171);
            this.nudFontSizeCalibration.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudFontSizeCalibration.Name = "nudFontSizeCalibration";
            this.nudFontSizeCalibration.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudFontSizeCalibration.Size = new System.Drawing.Size(59, 20);
            this.nudFontSizeCalibration.TabIndex = 4;
            this.nudFontSizeCalibration.Value = new decimal(new int[] {
            18,
            0,
            0,
            0});
            // 
            // nudHeightT
            // 
            this.nudHeightT.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudHeightT.Location = new System.Drawing.Point(193, 117);
            this.nudHeightT.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.nudHeightT.Name = "nudHeightT";
            this.nudHeightT.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudHeightT.Size = new System.Drawing.Size(77, 20);
            this.nudHeightT.TabIndex = 12;
            this.nudHeightT.ValueChanged += new System.EventHandler(this.nudHeightT_ValueChanged);
            // 
            // nudWidthL
            // 
            this.nudWidthL.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudWidthL.Location = new System.Drawing.Point(56, 117);
            this.nudWidthL.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.nudWidthL.Name = "nudWidthL";
            this.nudWidthL.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudWidthL.Size = new System.Drawing.Size(77, 20);
            this.nudWidthL.TabIndex = 6;
            this.nudWidthL.ValueChanged += new System.EventHandler(this.nudWidthL_ValueChanged);
            // 
            // nudHeight
            // 
            this.nudHeight.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudHeight.Location = new System.Drawing.Point(193, 91);
            this.nudHeight.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.nudHeight.Name = "nudHeight";
            this.nudHeight.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudHeight.Size = new System.Drawing.Size(77, 20);
            this.nudHeight.TabIndex = 10;
            this.nudHeight.ValueChanged += new System.EventHandler(this.nudHeight_ValueChanged);
            // 
            // nudWidth
            // 
            this.nudWidth.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudWidth.Location = new System.Drawing.Point(56, 91);
            this.nudWidth.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.nudWidth.Name = "nudWidth";
            this.nudWidth.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudWidth.Size = new System.Drawing.Size(77, 20);
            this.nudWidth.TabIndex = 4;
            this.nudWidth.ValueChanged += new System.EventHandler(this.nudWidth_ValueChanged);
            // 
            // nudY
            // 
            this.nudY.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudY.Location = new System.Drawing.Point(193, 65);
            this.nudY.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.nudY.Name = "nudY";
            this.nudY.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudY.Size = new System.Drawing.Size(77, 20);
            this.nudY.TabIndex = 8;
            this.nudY.ValueChanged += new System.EventHandler(this.nudY_ValueChanged);
            // 
            // nudX
            // 
            this.nudX.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudX.Location = new System.Drawing.Point(56, 65);
            this.nudX.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.nudX.Name = "nudX";
            this.nudX.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudX.Size = new System.Drawing.Size(77, 20);
            this.nudX.TabIndex = 2;
            this.nudX.ValueChanged += new System.EventHandler(this.nudX_ValueChanged);
            // 
            // OCRControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel4);
            this.Name = "OCRControl";
            this.Size = new System.Drawing.Size(807, 726);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tabControlManage.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWhiteTreshold)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ocrLetterEditTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ocrLetterEditRecognized)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudResolutionHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudResolutionWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudResizing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontSizeCalibration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeightT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidthL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudX)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.FlowLayoutPanel OCRDebugLayoutPanel;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ListBox listBoxRecognized;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSaveTemplate;
        private System.Windows.Forms.TextBox textBoxTemplate;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button BtCopyPatternRecognizedToTemplate;
        private OCRLetterEdit ocrLetterEditTemplate;
        private OCRLetterEdit ocrLetterEditRecognized;
        private System.Windows.Forms.Label labelMatching;
        private System.Windows.Forms.Button buttonSaveAsTemplate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControlManage;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown nudWhiteTreshold;
        private System.Windows.Forms.TextBox txtOCROutput;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox listBoxLabelRectangles;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private uiControls.Nud nudHeight;
        private System.Windows.Forms.Label label6;
        private uiControls.Nud nudWidth;
        private System.Windows.Forms.Label label5;
        private uiControls.Nud nudY;
        private System.Windows.Forms.Label label4;
        private uiControls.Nud nudX;
        private System.Windows.Forms.CheckBox chkbSetAllStatLabels;
        private System.Windows.Forms.Label label9;
        private uiControls.Nud nudHeightT;
        private System.Windows.Forms.Label label10;
        private uiControls.Nud nudWidthL;
        private System.Windows.Forms.CheckBox cbEnableOutput;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button BtSaveOCRconfig;
        private System.Windows.Forms.Button BtLoadOCRTemplate;
        private System.Windows.Forms.Label labelOCRFile;
        private System.Windows.Forms.Button BtSaveOCRConfigAs;
        private System.Windows.Forms.Button btnSaveOCRConfigFile2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxCalibrationText;
        private System.Windows.Forms.Button BtCreateOcrPatternsForLabels;
        private uiControls.Nud nudFontSizeCalibration;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Label lbResizeResult;
        private uiControls.Nud nudResizing;
        private System.Windows.Forms.Button BtUnloadOCR;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.CheckBox CbSkipNameRecognition;
        private System.Windows.Forms.CheckBox CbSkipTribeRecognition;
        private System.Windows.Forms.CheckBox CbSkipOwnerRecognition;
        private System.Windows.Forms.CheckBox CbTrainRecognition;
        private System.Windows.Forms.ListBox ListBoxPatternsOfString;
        private System.Windows.Forms.Button BtRemovePattern;
        private System.Windows.Forms.Button BtNewOcrConfig;
        private System.Windows.Forms.Button BtCreateOcrPatternsFromManualChars;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button buttonGetResFromScreenshot;
        private uiControls.Nud nudResolutionHeight;
        private System.Windows.Forms.Label label16;
        private uiControls.Nud nudResolutionWidth;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button BtSetStatPositionBasedOnFirstTwo;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button BtRemoveSelectedPatterns;
        private System.Windows.Forms.TextBox TbRemovePatterns;
        private System.Windows.Forms.Button BtRemoveAllPatterns;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel LlOcrManual;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Label LbReplacingsFileStatus;
        private System.Windows.Forms.Button BtReplacingLoadFile;
        private System.Windows.Forms.Button BtReplacingOpenFile;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.TextBox TbLabelSetName;
        private System.Windows.Forms.Button BtDeleteLabelSet;
        private System.Windows.Forms.Button BtNewLabelSet;
        private System.Windows.Forms.ComboBox CbbLabelSets;
    }
}
