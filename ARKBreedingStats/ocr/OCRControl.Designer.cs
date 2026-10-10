using System;
using ARKBreedingStats.uiControls;

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
            tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            OCRDebugLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            tabControlManage = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            groupBox11 = new GroupBoxC();
            LbReplacingsFileStatus = new System.Windows.Forms.Label();
            BtReplacingLoadFile = new System.Windows.Forms.Button();
            BtReplacingOpenFile = new System.Windows.Forms.Button();
            label18 = new System.Windows.Forms.Label();
            LlOcrManual = new System.Windows.Forms.LinkLabel();
            groupBox6 = new GroupBoxC();
            BtNewOcrConfig = new System.Windows.Forms.Button();
            BtUnloadOCR = new System.Windows.Forms.Button();
            labelOCRFile = new System.Windows.Forms.Label();
            BtSaveOCRconfig = new System.Windows.Forms.Button();
            BtLoadOCRTemplate = new System.Windows.Forms.Button();
            BtSaveOCRConfigAs = new System.Windows.Forms.Button();
            label12 = new System.Windows.Forms.Label();
            tabPage2 = new System.Windows.Forms.TabPage();
            label1 = new System.Windows.Forms.Label();
            ListBoxPatternsOfString = new System.Windows.Forms.ListBox();
            btnSaveOCRConfigFile2 = new System.Windows.Forms.Button();
            cbEnableOutput = new System.Windows.Forms.CheckBox();
            label3 = new System.Windows.Forms.Label();
            txtOCROutput = new System.Windows.Forms.TextBox();
            label11 = new System.Windows.Forms.Label();
            listBoxRecognized = new System.Windows.Forms.ListBox();
            groupBox1 = new GroupBoxC();
            BtRemovePattern = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            ocrLetterEditTemplate = new OCRLetterEdit();
            btnSaveTemplate = new System.Windows.Forms.Button();
            textBoxTemplate = new System.Windows.Forms.TextBox();
            label13 = new System.Windows.Forms.Label();
            nudWhiteTreshold = new System.Windows.Forms.NumericUpDown();
            groupBox2 = new GroupBoxC();
            buttonSaveAsTemplate = new System.Windows.Forms.Button();
            labelMatching = new System.Windows.Forms.Label();
            ocrLetterEditRecognized = new OCRLetterEdit();
            BtCopyPatternRecognizedToTemplate = new System.Windows.Forms.Button();
            tabPage4 = new System.Windows.Forms.TabPage();
            groupBox7 = new GroupBoxC();
            BtRemoveSelectedPatterns = new System.Windows.Forms.Button();
            TbRemovePatterns = new System.Windows.Forms.TextBox();
            BtRemoveAllPatterns = new System.Windows.Forms.Button();
            groupBox5 = new GroupBoxC();
            buttonGetResFromScreenshot = new System.Windows.Forms.Button();
            nudResolutionHeight = new Nud();
            label16 = new System.Windows.Forms.Label();
            nudResolutionWidth = new Nud();
            label15 = new System.Windows.Forms.Label();
            groupBox10 = new GroupBoxC();
            CbSkipNameRecognition = new System.Windows.Forms.CheckBox();
            CbSkipTribeRecognition = new System.Windows.Forms.CheckBox();
            CbSkipOwnerRecognition = new System.Windows.Forms.CheckBox();
            CbTrainRecognition = new System.Windows.Forms.CheckBox();
            groupBox9 = new GroupBoxC();
            lbResizeResult = new System.Windows.Forms.Label();
            nudResizing = new Nud();
            groupBox8 = new GroupBoxC();
            BtCreateOcrPatternsFromManualChars = new System.Windows.Forms.Button();
            label17 = new System.Windows.Forms.Label();
            nudFontSizeCalibration = new Nud();
            BtCreateOcrPatternsForLabels = new System.Windows.Forms.Button();
            label14 = new System.Windows.Forms.Label();
            textBoxCalibrationText = new System.Windows.Forms.TextBox();
            tabPage3 = new System.Windows.Forms.TabPage();
            groupBox12 = new GroupBoxC();
            TbLabelSetName = new System.Windows.Forms.TextBox();
            BtDeleteLabelSet = new System.Windows.Forms.Button();
            BtNewLabelSet = new System.Windows.Forms.Button();
            CbbLabelSets = new System.Windows.Forms.ComboBox();
            groupBox4 = new GroupBoxC();
            BtSetStatPositionBasedOnFirstTwo = new System.Windows.Forms.Button();
            chkbSetAllStatLabels = new System.Windows.Forms.CheckBox();
            label9 = new System.Windows.Forms.Label();
            nudHeightT = new Nud();
            label10 = new System.Windows.Forms.Label();
            nudWidthL = new Nud();
            label8 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            nudHeight = new Nud();
            label6 = new System.Windows.Forms.Label();
            nudWidth = new Nud();
            label5 = new System.Windows.Forms.Label();
            nudY = new Nud();
            label4 = new System.Windows.Forms.Label();
            nudX = new Nud();
            groupBox3 = new GroupBoxC();
            listBoxLabelRectangles = new System.Windows.Forms.ListBox();
            tableLayoutPanel4.SuspendLayout();
            tabControlManage.SuspendLayout();
            tabPage1.SuspendLayout();
            groupBox11.SuspendLayout();
            groupBox6.SuspendLayout();
            tabPage2.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ocrLetterEditTemplate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWhiteTreshold).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ocrLetterEditRecognized).BeginInit();
            tabPage4.SuspendLayout();
            groupBox7.SuspendLayout();
            groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudResolutionHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudResolutionWidth).BeginInit();
            groupBox10.SuspendLayout();
            groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudResizing).BeginInit();
            groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudFontSizeCalibration).BeginInit();
            tabPage3.SuspendLayout();
            groupBox12.SuspendLayout();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudHeightT).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWidthL).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudX).BeginInit();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 360F));
            tableLayoutPanel4.Controls.Add(OCRDebugLayoutPanel, 0, 0);
            tableLayoutPanel4.Controls.Add(tabControlManage, 1, 0);
            tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel4.Size = new System.Drawing.Size(807, 726);
            tableLayoutPanel4.TabIndex = 2;
            // 
            // OCRDebugLayoutPanel
            // 
            OCRDebugLayoutPanel.AllowDrop = true;
            OCRDebugLayoutPanel.AutoScroll = true;
            OCRDebugLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            OCRDebugLayoutPanel.Location = new System.Drawing.Point(3, 3);
            OCRDebugLayoutPanel.Name = "OCRDebugLayoutPanel";
            OCRDebugLayoutPanel.Size = new System.Drawing.Size(441, 720);
            OCRDebugLayoutPanel.TabIndex = 0;
            OCRDebugLayoutPanel.DragDrop += OCRDebugLayoutPanel_DragDrop;
            OCRDebugLayoutPanel.DragEnter += OCRDebugLayoutPanel_DragEnter;
            // 
            // tabControlManage
            // 
            tabControlManage.Controls.Add(tabPage1);
            tabControlManage.Controls.Add(tabPage2);
            tabControlManage.Controls.Add(tabPage4);
            tabControlManage.Controls.Add(tabPage3);
            tabControlManage.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControlManage.Location = new System.Drawing.Point(450, 3);
            tabControlManage.Name = "tabControlManage";
            tabControlManage.SelectedIndex = 0;
            tabControlManage.Size = new System.Drawing.Size(354, 720);
            tabControlManage.TabIndex = 15;
            // 
            // tabPage1
            // 
            tabPage1.AutoScroll = true;
            tabPage1.Controls.Add(groupBox11);
            tabPage1.Controls.Add(LlOcrManual);
            tabPage1.Controls.Add(groupBox6);
            tabPage1.Controls.Add(label12);
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(346, 692);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "OCR Info";
            // 
            // groupBox11
            // 
            groupBox11.Controls.Add(LbReplacingsFileStatus);
            groupBox11.Controls.Add(BtReplacingLoadFile);
            groupBox11.Controls.Add(BtReplacingOpenFile);
            groupBox11.Controls.Add(label18);
            groupBox11.Location = new System.Drawing.Point(6, 420);
            groupBox11.Name = "groupBox11";
            groupBox11.Size = new System.Drawing.Size(337, 185);
            groupBox11.TabIndex = 3;
            groupBox11.TabStop = false;
            groupBox11.Text = "Manual corrections";
            // 
            // LbReplacingsFileStatus
            // 
            LbReplacingsFileStatus.AutoSize = true;
            LbReplacingsFileStatus.Location = new System.Drawing.Point(6, 157);
            LbReplacingsFileStatus.Name = "LbReplacingsFileStatus";
            LbReplacingsFileStatus.Size = new System.Drawing.Size(0, 15);
            LbReplacingsFileStatus.TabIndex = 3;
            // 
            // BtReplacingLoadFile
            // 
            BtReplacingLoadFile.Location = new System.Drawing.Point(87, 128);
            BtReplacingLoadFile.Name = "BtReplacingLoadFile";
            BtReplacingLoadFile.Size = new System.Drawing.Size(113, 23);
            BtReplacingLoadFile.TabIndex = 2;
            BtReplacingLoadFile.Text = "Load replacings";
            BtReplacingLoadFile.UseVisualStyleBackColor = true;
            BtReplacingLoadFile.Click += BtReplacingLoadFile_Click;
            // 
            // BtReplacingOpenFile
            // 
            BtReplacingOpenFile.Location = new System.Drawing.Point(6, 128);
            BtReplacingOpenFile.Name = "BtReplacingOpenFile";
            BtReplacingOpenFile.Size = new System.Drawing.Size(75, 23);
            BtReplacingOpenFile.TabIndex = 1;
            BtReplacingOpenFile.Text = "Open file";
            BtReplacingOpenFile.UseVisualStyleBackColor = true;
            BtReplacingOpenFile.Click += BtReplacingOpenFile_Click;
            // 
            // label18
            // 
            label18.Location = new System.Drawing.Point(6, 16);
            label18.Name = "label18";
            label18.Size = new System.Drawing.Size(325, 109);
            label18.TabIndex = 0;
            label18.Text = resources.GetString("label18.Text");
            // 
            // LlOcrManual
            // 
            LlOcrManual.AutoSize = true;
            LlOcrManual.Location = new System.Drawing.Point(6, 152);
            LlOcrManual.Name = "LlOcrManual";
            LlOcrManual.Size = new System.Drawing.Size(74, 15);
            LlOcrManual.TabIndex = 2;
            LlOcrManual.TabStop = true;
            LlOcrManual.Text = "OCR manual";
            LlOcrManual.LinkClicked += LlOcrManual_LinkClicked;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(BtNewOcrConfig);
            groupBox6.Controls.Add(BtUnloadOCR);
            groupBox6.Controls.Add(labelOCRFile);
            groupBox6.Controls.Add(BtSaveOCRconfig);
            groupBox6.Controls.Add(BtLoadOCRTemplate);
            groupBox6.Controls.Add(BtSaveOCRConfigAs);
            groupBox6.Location = new System.Drawing.Point(6, 219);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new System.Drawing.Size(334, 195);
            groupBox6.TabIndex = 1;
            groupBox6.TabStop = false;
            groupBox6.Text = "OCR config file";
            // 
            // BtNewOcrConfig
            // 
            BtNewOcrConfig.Location = new System.Drawing.Point(6, 19);
            BtNewOcrConfig.Name = "BtNewOcrConfig";
            BtNewOcrConfig.Size = new System.Drawing.Size(94, 23);
            BtNewOcrConfig.TabIndex = 0;
            BtNewOcrConfig.Text = "new";
            BtNewOcrConfig.UseVisualStyleBackColor = true;
            BtNewOcrConfig.Click += BtNewOcrConfig_Click;
            // 
            // BtUnloadOCR
            // 
            BtUnloadOCR.Location = new System.Drawing.Point(106, 48);
            BtUnloadOCR.Name = "BtUnloadOCR";
            BtUnloadOCR.Size = new System.Drawing.Size(94, 23);
            BtUnloadOCR.TabIndex = 3;
            BtUnloadOCR.Text = "unload";
            BtUnloadOCR.UseVisualStyleBackColor = true;
            BtUnloadOCR.Click += btUnloadOCR_Click;
            // 
            // labelOCRFile
            // 
            labelOCRFile.Location = new System.Drawing.Point(6, 74);
            labelOCRFile.Name = "labelOCRFile";
            labelOCRFile.Size = new System.Drawing.Size(322, 116);
            labelOCRFile.TabIndex = 5;
            labelOCRFile.Click += labelOCRFile_Click;
            // 
            // BtSaveOCRconfig
            // 
            BtSaveOCRconfig.Location = new System.Drawing.Point(206, 19);
            BtSaveOCRconfig.Name = "BtSaveOCRconfig";
            BtSaveOCRconfig.Size = new System.Drawing.Size(94, 23);
            BtSaveOCRconfig.TabIndex = 2;
            BtSaveOCRconfig.Text = "save";
            BtSaveOCRconfig.UseVisualStyleBackColor = true;
            BtSaveOCRconfig.Click += btnSaveOCRconfig_Click;
            // 
            // BtLoadOCRTemplate
            // 
            BtLoadOCRTemplate.Location = new System.Drawing.Point(106, 19);
            BtLoadOCRTemplate.Name = "BtLoadOCRTemplate";
            BtLoadOCRTemplate.Size = new System.Drawing.Size(94, 23);
            BtLoadOCRTemplate.TabIndex = 1;
            BtLoadOCRTemplate.Text = "load";
            BtLoadOCRTemplate.UseVisualStyleBackColor = true;
            BtLoadOCRTemplate.Click += buttonLoadOCRTemplate_Click;
            // 
            // BtSaveOCRConfigAs
            // 
            BtSaveOCRConfigAs.Location = new System.Drawing.Point(206, 48);
            BtSaveOCRConfigAs.Name = "BtSaveOCRConfigAs";
            BtSaveOCRConfigAs.Size = new System.Drawing.Size(94, 23);
            BtSaveOCRConfigAs.TabIndex = 4;
            BtSaveOCRConfigAs.Text = "save as…";
            BtSaveOCRConfigAs.UseVisualStyleBackColor = true;
            BtSaveOCRConfigAs.Click += btnSaveOCRConfigAs_Click;
            // 
            // label12
            // 
            label12.Location = new System.Drawing.Point(6, 14);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(334, 122);
            label12.TabIndex = 0;
            label12.Text = resources.GetString("label12.Text");
            // 
            // tabPage2
            // 
            tabPage2.AutoScroll = true;
            tabPage2.Controls.Add(label1);
            tabPage2.Controls.Add(ListBoxPatternsOfString);
            tabPage2.Controls.Add(btnSaveOCRConfigFile2);
            tabPage2.Controls.Add(cbEnableOutput);
            tabPage2.Controls.Add(label3);
            tabPage2.Controls.Add(txtOCROutput);
            tabPage2.Controls.Add(label11);
            tabPage2.Controls.Add(listBoxRecognized);
            tabPage2.Controls.Add(groupBox1);
            tabPage2.Controls.Add(label13);
            tabPage2.Controls.Add(nudWhiteTreshold);
            tabPage2.Controls.Add(groupBox2);
            tabPage2.Location = new System.Drawing.Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(3);
            tabPage2.Size = new System.Drawing.Size(346, 692);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Output";
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(6, 564);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(287, 65);
            label1.TabIndex = 17;
            label1.Text = "To fix a wrong character recognition, select it in the list on the right, enter the correct Text in the Template Character input, then in the Recognized GroupBox click on Save as template.";
            // 
            // ListBoxPatternsOfString
            // 
            ListBoxPatternsOfString.FormattingEnabled = true;
            ListBoxPatternsOfString.Location = new System.Drawing.Point(260, 268);
            ListBoxPatternsOfString.Name = "ListBoxPatternsOfString";
            ListBoxPatternsOfString.Size = new System.Drawing.Size(33, 244);
            ListBoxPatternsOfString.TabIndex = 4;
            ListBoxPatternsOfString.SelectedIndexChanged += ListBoxPatternsOfString_SelectedIndexChanged;
            // 
            // btnSaveOCRConfigFile2
            // 
            btnSaveOCRConfigFile2.Location = new System.Drawing.Point(6, 632);
            btnSaveOCRConfigFile2.Name = "btnSaveOCRConfigFile2";
            btnSaveOCRConfigFile2.Size = new System.Drawing.Size(130, 39);
            btnSaveOCRConfigFile2.TabIndex = 8;
            btnSaveOCRConfigFile2.Text = "save current OCR config-file";
            btnSaveOCRConfigFile2.UseVisualStyleBackColor = true;
            btnSaveOCRConfigFile2.Click += btnSaveOCRconfig_Click;
            // 
            // cbEnableOutput
            // 
            cbEnableOutput.AutoSize = true;
            cbEnableOutput.Location = new System.Drawing.Point(137, 6);
            cbEnableOutput.Name = "cbEnableOutput";
            cbEnableOutput.Size = new System.Drawing.Size(171, 19);
            cbEnableOutput.TabIndex = 0;
            cbEnableOutput.Text = "Enable Logging and Editing";
            cbEnableOutput.UseVisualStyleBackColor = true;
            cbEnableOutput.CheckedChanged += cbEnableOutput_CheckedChanged;
            // 
            // label3
            // 
            label3.Location = new System.Drawing.Point(6, 221);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(287, 44);
            label3.TabIndex = 2;
            label3.Text = "Here you can edit the OCR-character-templates. Click on a recognized character in the list on the right to see as which character it was read and to edit it.";
            // 
            // txtOCROutput
            // 
            txtOCROutput.Location = new System.Drawing.Point(6, 32);
            txtOCROutput.Multiline = true;
            txtOCROutput.Name = "txtOCROutput";
            txtOCROutput.ReadOnly = true;
            txtOCROutput.Size = new System.Drawing.Size(287, 186);
            txtOCROutput.TabIndex = 1;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(6, 16);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(72, 15);
            label11.TabIndex = 16;
            label11.Text = "OCR Output";
            // 
            // listBoxRecognized
            // 
            listBoxRecognized.Dock = System.Windows.Forms.DockStyle.Right;
            listBoxRecognized.FormattingEnabled = true;
            listBoxRecognized.Location = new System.Drawing.Point(305, 3);
            listBoxRecognized.Name = "listBoxRecognized";
            listBoxRecognized.Size = new System.Drawing.Size(44, 669);
            listBoxRecognized.TabIndex = 9;
            listBoxRecognized.SelectedIndexChanged += listBoxRecognized_SelectedIndexChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(BtRemovePattern);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(ocrLetterEditTemplate);
            groupBox1.Controls.Add(btnSaveTemplate);
            groupBox1.Controls.Add(textBoxTemplate);
            groupBox1.Location = new System.Drawing.Point(6, 268);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(248, 126);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Template";
            // 
            // BtRemovePattern
            // 
            BtRemovePattern.Location = new System.Drawing.Point(6, 45);
            BtRemovePattern.Name = "BtRemovePattern";
            BtRemovePattern.Size = new System.Drawing.Size(59, 36);
            BtRemovePattern.TabIndex = 2;
            BtRemovePattern.Text = "Remove template";
            BtRemovePattern.UseVisualStyleBackColor = true;
            BtRemovePattern.Click += BtRemovePattern_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(6, 22);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(58, 15);
            label2.TabIndex = 0;
            label2.Text = "Character";
            // 
            // ocrLetterEditTemplate
            // 
            ocrLetterEditTemplate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ocrLetterEditTemplate.Location = new System.Drawing.Point(136, 19);
            ocrLetterEditTemplate.Name = "ocrLetterEditTemplate";
            ocrLetterEditTemplate.Size = new System.Drawing.Size(102, 102);
            ocrLetterEditTemplate.TabIndex = 12;
            ocrLetterEditTemplate.TabStop = false;
            // 
            // btnSaveTemplate
            // 
            btnSaveTemplate.Location = new System.Drawing.Point(71, 45);
            btnSaveTemplate.Name = "btnSaveTemplate";
            btnSaveTemplate.Size = new System.Drawing.Size(59, 36);
            btnSaveTemplate.TabIndex = 3;
            btnSaveTemplate.Text = "Save template";
            btnSaveTemplate.UseVisualStyleBackColor = true;
            btnSaveTemplate.Click += btnSaveTemplate_Click;
            // 
            // textBoxTemplate
            // 
            textBoxTemplate.Location = new System.Drawing.Point(71, 19);
            textBoxTemplate.MaxLength = 1;
            textBoxTemplate.Name = "textBoxTemplate";
            textBoxTemplate.Size = new System.Drawing.Size(59, 23);
            textBoxTemplate.TabIndex = 1;
            textBoxTemplate.TextChanged += textBoxTemplate_TextChanged;
            textBoxTemplate.Enter += textBoxTemplate_Enter;
            // 
            // label13
            // 
            label13.Location = new System.Drawing.Point(61, 528);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(227, 33);
            label13.TabIndex = 7;
            label13.Text = "White Threshold (adjust until the characters that should be read are clearly distinguishable)";
            // 
            // nudWhiteTreshold
            // 
            nudWhiteTreshold.Location = new System.Drawing.Point(6, 531);
            nudWhiteTreshold.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudWhiteTreshold.Name = "nudWhiteTreshold";
            nudWhiteTreshold.Size = new System.Drawing.Size(49, 23);
            nudWhiteTreshold.TabIndex = 6;
            nudWhiteTreshold.Value = new decimal(new int[] { 155, 0, 0, 0 });
            nudWhiteTreshold.ValueChanged += nudWhiteTreshold_ValueChanged;
            nudWhiteTreshold.Leave += nudWhiteTreshold_Leave;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(buttonSaveAsTemplate);
            groupBox2.Controls.Add(labelMatching);
            groupBox2.Controls.Add(ocrLetterEditRecognized);
            groupBox2.Controls.Add(BtCopyPatternRecognizedToTemplate);
            groupBox2.Location = new System.Drawing.Point(6, 400);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(248, 125);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "Recognized";
            // 
            // buttonSaveAsTemplate
            // 
            buttonSaveAsTemplate.Location = new System.Drawing.Point(71, 16);
            buttonSaveAsTemplate.Name = "buttonSaveAsTemplate";
            buttonSaveAsTemplate.Size = new System.Drawing.Size(59, 36);
            buttonSaveAsTemplate.TabIndex = 1;
            buttonSaveAsTemplate.Text = "Save as template";
            buttonSaveAsTemplate.UseVisualStyleBackColor = true;
            buttonSaveAsTemplate.Click += buttonSaveAsTemplate_Click;
            // 
            // labelMatching
            // 
            labelMatching.Location = new System.Drawing.Point(6, 16);
            labelMatching.Name = "labelMatching";
            labelMatching.Size = new System.Drawing.Size(59, 36);
            labelMatching.TabIndex = 0;
            labelMatching.Text = "match";
            // 
            // ocrLetterEditRecognized
            // 
            ocrLetterEditRecognized.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ocrLetterEditRecognized.Location = new System.Drawing.Point(136, 16);
            ocrLetterEditRecognized.Name = "ocrLetterEditRecognized";
            ocrLetterEditRecognized.Size = new System.Drawing.Size(102, 102);
            ocrLetterEditRecognized.TabIndex = 13;
            ocrLetterEditRecognized.TabStop = false;
            // 
            // BtCopyPatternRecognizedToTemplate
            // 
            BtCopyPatternRecognizedToTemplate.Location = new System.Drawing.Point(71, 58);
            BtCopyPatternRecognizedToTemplate.Name = "BtCopyPatternRecognizedToTemplate";
            BtCopyPatternRecognizedToTemplate.Size = new System.Drawing.Size(59, 36);
            BtCopyPatternRecognizedToTemplate.TabIndex = 2;
            BtCopyPatternRecognizedToTemplate.Text = "Copy to template";
            BtCopyPatternRecognizedToTemplate.UseVisualStyleBackColor = true;
            BtCopyPatternRecognizedToTemplate.Click += BtCopyPatternRecognizedToTemplateClick;
            // 
            // tabPage4
            // 
            tabPage4.AutoScroll = true;
            tabPage4.Controls.Add(groupBox7);
            tabPage4.Controls.Add(groupBox5);
            tabPage4.Controls.Add(groupBox10);
            tabPage4.Controls.Add(groupBox9);
            tabPage4.Controls.Add(groupBox8);
            tabPage4.Location = new System.Drawing.Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new System.Windows.Forms.Padding(3);
            tabPage4.Size = new System.Drawing.Size(346, 692);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Manage";
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(BtRemoveSelectedPatterns);
            groupBox7.Controls.Add(TbRemovePatterns);
            groupBox7.Controls.Add(BtRemoveAllPatterns);
            groupBox7.Location = new System.Drawing.Point(6, 538);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new System.Drawing.Size(334, 100);
            groupBox7.TabIndex = 0;
            groupBox7.TabStop = false;
            groupBox7.Text = "Remove Patterns";
            // 
            // BtRemoveSelectedPatterns
            // 
            BtRemoveSelectedPatterns.BackColor = System.Drawing.Color.LightSalmon;
            BtRemoveSelectedPatterns.ForeColor = System.Drawing.Color.Black;
            BtRemoveSelectedPatterns.Location = new System.Drawing.Point(94, 17);
            BtRemoveSelectedPatterns.Name = "BtRemoveSelectedPatterns";
            BtRemoveSelectedPatterns.Size = new System.Drawing.Size(234, 23);
            BtRemoveSelectedPatterns.TabIndex = 1;
            BtRemoveSelectedPatterns.Text = "Remove Patterns of these characters";
            BtRemoveSelectedPatterns.UseVisualStyleBackColor = false;
            BtRemoveSelectedPatterns.Click += BtRemoveSelectedPatterns_Click;
            // 
            // TbRemovePatterns
            // 
            TbRemovePatterns.Location = new System.Drawing.Point(6, 19);
            TbRemovePatterns.Name = "TbRemovePatterns";
            TbRemovePatterns.Size = new System.Drawing.Size(82, 23);
            TbRemovePatterns.TabIndex = 0;
            // 
            // BtRemoveAllPatterns
            // 
            BtRemoveAllPatterns.BackColor = System.Drawing.Color.LightSalmon;
            BtRemoveAllPatterns.ForeColor = System.Drawing.Color.Black;
            BtRemoveAllPatterns.Location = new System.Drawing.Point(94, 71);
            BtRemoveAllPatterns.Name = "BtRemoveAllPatterns";
            BtRemoveAllPatterns.Size = new System.Drawing.Size(134, 23);
            BtRemoveAllPatterns.TabIndex = 2;
            BtRemoveAllPatterns.Text = "Remove all Patterns";
            BtRemoveAllPatterns.UseVisualStyleBackColor = false;
            BtRemoveAllPatterns.Click += BtRemoveAllPatterns_Click;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(buttonGetResFromScreenshot);
            groupBox5.Controls.Add(nudResolutionHeight);
            groupBox5.Controls.Add(label16);
            groupBox5.Controls.Add(nudResolutionWidth);
            groupBox5.Controls.Add(label15);
            groupBox5.Location = new System.Drawing.Point(6, 151);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new System.Drawing.Size(334, 77);
            groupBox5.TabIndex = 2;
            groupBox5.TabStop = false;
            groupBox5.Text = "Resolution";
            // 
            // buttonGetResFromScreenshot
            // 
            buttonGetResFromScreenshot.Location = new System.Drawing.Point(130, 19);
            buttonGetResFromScreenshot.Name = "buttonGetResFromScreenshot";
            buttonGetResFromScreenshot.Size = new System.Drawing.Size(103, 46);
            buttonGetResFromScreenshot.TabIndex = 4;
            buttonGetResFromScreenshot.Text = "Take resolution from screenshot";
            buttonGetResFromScreenshot.UseVisualStyleBackColor = true;
            buttonGetResFromScreenshot.Click += buttonGetResFromScreenshot_Click;
            // 
            // nudResolutionHeight
            // 
            nudResolutionHeight.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            nudResolutionHeight.Location = new System.Drawing.Point(47, 45);
            nudResolutionHeight.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudResolutionHeight.Name = "nudResolutionHeight";
            nudResolutionHeight.Size = new System.Drawing.Size(77, 23);
            nudResolutionHeight.TabIndex = 3;
            nudResolutionHeight.ValueChanged += nudResolutionHeight_ValueChanged;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new System.Drawing.Point(6, 47);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(43, 15);
            label16.TabIndex = 2;
            label16.Text = "Height";
            // 
            // nudResolutionWidth
            // 
            nudResolutionWidth.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            nudResolutionWidth.Location = new System.Drawing.Point(47, 19);
            nudResolutionWidth.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudResolutionWidth.Name = "nudResolutionWidth";
            nudResolutionWidth.Size = new System.Drawing.Size(77, 23);
            nudResolutionWidth.TabIndex = 1;
            nudResolutionWidth.ValueChanged += nudResolutionWidth_ValueChanged;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(6, 21);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(39, 15);
            label15.TabIndex = 0;
            label15.Text = "Width";
            // 
            // groupBox10
            // 
            groupBox10.Controls.Add(CbSkipNameRecognition);
            groupBox10.Controls.Add(CbSkipTribeRecognition);
            groupBox10.Controls.Add(CbSkipOwnerRecognition);
            groupBox10.Controls.Add(CbTrainRecognition);
            groupBox10.Location = new System.Drawing.Point(6, 6);
            groupBox10.Name = "groupBox10";
            groupBox10.Size = new System.Drawing.Size(334, 139);
            groupBox10.TabIndex = 1;
            groupBox10.TabStop = false;
            groupBox10.Text = "Pattern recognition settings";
            // 
            // CbSkipNameRecognition
            // 
            CbSkipNameRecognition.AutoSize = true;
            CbSkipNameRecognition.Location = new System.Drawing.Point(6, 42);
            CbSkipNameRecognition.Name = "CbSkipNameRecognition";
            CbSkipNameRecognition.Size = new System.Drawing.Size(145, 19);
            CbSkipNameRecognition.TabIndex = 1;
            CbSkipNameRecognition.Text = "Skip name recognition";
            CbSkipNameRecognition.UseVisualStyleBackColor = true;
            CbSkipNameRecognition.CheckedChanged += CbSkipNameRecognition_CheckedChanged;
            // 
            // CbSkipTribeRecognition
            // 
            CbSkipTribeRecognition.AutoSize = true;
            CbSkipTribeRecognition.Location = new System.Drawing.Point(6, 65);
            CbSkipTribeRecognition.Name = "CbSkipTribeRecognition";
            CbSkipTribeRecognition.Size = new System.Drawing.Size(139, 19);
            CbSkipTribeRecognition.TabIndex = 2;
            CbSkipTribeRecognition.Text = "Skip tribe recognition";
            CbSkipTribeRecognition.UseVisualStyleBackColor = true;
            CbSkipTribeRecognition.CheckedChanged += CbSkipTribeRecognition_CheckedChanged;
            // 
            // CbSkipOwnerRecognition
            // 
            CbSkipOwnerRecognition.AutoSize = true;
            CbSkipOwnerRecognition.Location = new System.Drawing.Point(6, 88);
            CbSkipOwnerRecognition.Name = "CbSkipOwnerRecognition";
            CbSkipOwnerRecognition.Size = new System.Drawing.Size(148, 19);
            CbSkipOwnerRecognition.TabIndex = 3;
            CbSkipOwnerRecognition.Text = "Skip owner recognition";
            CbSkipOwnerRecognition.UseVisualStyleBackColor = true;
            CbSkipOwnerRecognition.CheckedChanged += CbSkipOwnerRecognition_CheckedChanged;
            // 
            // CbTrainRecognition
            // 
            CbTrainRecognition.AutoSize = true;
            CbTrainRecognition.Location = new System.Drawing.Point(6, 19);
            CbTrainRecognition.Name = "CbTrainRecognition";
            CbTrainRecognition.Size = new System.Drawing.Size(116, 19);
            CbTrainRecognition.TabIndex = 0;
            CbTrainRecognition.Text = "Train recognition";
            CbTrainRecognition.UseVisualStyleBackColor = true;
            CbTrainRecognition.CheckedChanged += CbTrainRecognition_CheckedChanged;
            // 
            // groupBox9
            // 
            groupBox9.Controls.Add(lbResizeResult);
            groupBox9.Controls.Add(nudResizing);
            groupBox9.Location = new System.Drawing.Point(6, 234);
            groupBox9.Name = "groupBox9";
            groupBox9.Size = new System.Drawing.Size(334, 94);
            groupBox9.TabIndex = 3;
            groupBox9.TabStop = false;
            groupBox9.Text = "Resize the captured screenshot";
            // 
            // lbResizeResult
            // 
            lbResizeResult.Location = new System.Drawing.Point(107, 19);
            lbResizeResult.Name = "lbResizeResult";
            lbResizeResult.Size = new System.Drawing.Size(221, 72);
            lbResizeResult.TabIndex = 1;
            lbResizeResult.Text = "->";
            // 
            // nudResizing
            // 
            nudResizing.DecimalPlaces = 6;
            nudResizing.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            nudResizing.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            nudResizing.Location = new System.Drawing.Point(6, 19);
            nudResizing.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            nudResizing.Name = "nudResizing";
            nudResizing.Size = new System.Drawing.Size(95, 23);
            nudResizing.TabIndex = 0;
            nudResizing.ValueChanged += nudResizing_ValueChanged;
            // 
            // groupBox8
            // 
            groupBox8.Controls.Add(BtCreateOcrPatternsFromManualChars);
            groupBox8.Controls.Add(label17);
            groupBox8.Controls.Add(nudFontSizeCalibration);
            groupBox8.Controls.Add(BtCreateOcrPatternsForLabels);
            groupBox8.Controls.Add(label14);
            groupBox8.Controls.Add(textBoxCalibrationText);
            groupBox8.Location = new System.Drawing.Point(6, 334);
            groupBox8.Name = "groupBox8";
            groupBox8.Size = new System.Drawing.Size(334, 198);
            groupBox8.TabIndex = 4;
            groupBox8.TabStop = false;
            groupBox8.Text = "Add OCR Patterns";
            // 
            // BtCreateOcrPatternsFromManualChars
            // 
            BtCreateOcrPatternsFromManualChars.Location = new System.Drawing.Point(144, 168);
            BtCreateOcrPatternsFromManualChars.Name = "BtCreateOcrPatternsFromManualChars";
            BtCreateOcrPatternsFromManualChars.Size = new System.Drawing.Size(184, 23);
            BtCreateOcrPatternsFromManualChars.TabIndex = 5;
            BtCreateOcrPatternsFromManualChars.Text = "Create custom OCR patterns";
            BtCreateOcrPatternsFromManualChars.UseVisualStyleBackColor = true;
            BtCreateOcrPatternsFromManualChars.Click += BtCreateOcrPatternsFromManualChars_Click;
            // 
            // label17
            // 
            label17.Location = new System.Drawing.Point(6, 83);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(322, 56);
            label17.TabIndex = 1;
            label17.Text = resources.GetString("label17.Text");
            // 
            // nudFontSizeCalibration
            // 
            nudFontSizeCalibration.ForeColor = System.Drawing.Color.Black;
            nudFontSizeCalibration.Location = new System.Drawing.Point(58, 171);
            nudFontSizeCalibration.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            nudFontSizeCalibration.Name = "nudFontSizeCalibration";
            nudFontSizeCalibration.Size = new System.Drawing.Size(59, 23);
            nudFontSizeCalibration.TabIndex = 4;
            nudFontSizeCalibration.Value = new decimal(new int[] { 18, 0, 0, 0 });
            // 
            // BtCreateOcrPatternsForLabels
            // 
            BtCreateOcrPatternsForLabels.Location = new System.Drawing.Point(6, 19);
            BtCreateOcrPatternsForLabels.Name = "BtCreateOcrPatternsForLabels";
            BtCreateOcrPatternsForLabels.Size = new System.Drawing.Size(322, 37);
            BtCreateOcrPatternsForLabels.TabIndex = 0;
            BtCreateOcrPatternsForLabels.Text = "Automatic creation of OCR patterns from a font file considering the label sizes";
            BtCreateOcrPatternsForLabels.UseVisualStyleBackColor = true;
            BtCreateOcrPatternsForLabels.Click += buttonLoadCalibrationImage_Click;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(6, 173);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(51, 15);
            label14.TabIndex = 3;
            label14.Text = "font size";
            // 
            // textBoxCalibrationText
            // 
            textBoxCalibrationText.Location = new System.Drawing.Point(6, 142);
            textBoxCalibrationText.Name = "textBoxCalibrationText";
            textBoxCalibrationText.Size = new System.Drawing.Size(322, 23);
            textBoxCalibrationText.TabIndex = 2;
            textBoxCalibrationText.Text = "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            // 
            // tabPage3
            // 
            tabPage3.AutoScroll = true;
            tabPage3.Controls.Add(groupBox12);
            tabPage3.Controls.Add(BtDeleteLabelSet);
            tabPage3.Controls.Add(BtNewLabelSet);
            tabPage3.Controls.Add(CbbLabelSets);
            tabPage3.Controls.Add(groupBox4);
            tabPage3.Controls.Add(groupBox3);
            tabPage3.Location = new System.Drawing.Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new System.Windows.Forms.Padding(3);
            tabPage3.Size = new System.Drawing.Size(346, 692);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Labels";
            // 
            // groupBox12
            // 
            groupBox12.Controls.Add(TbLabelSetName);
            groupBox12.Location = new System.Drawing.Point(6, 35);
            groupBox12.Name = "groupBox12";
            groupBox12.Size = new System.Drawing.Size(334, 45);
            groupBox12.TabIndex = 5;
            groupBox12.TabStop = false;
            groupBox12.Text = "Label set name";
            // 
            // TbLabelSetName
            // 
            TbLabelSetName.Location = new System.Drawing.Point(6, 19);
            TbLabelSetName.Name = "TbLabelSetName";
            TbLabelSetName.Size = new System.Drawing.Size(322, 23);
            TbLabelSetName.TabIndex = 0;
            TbLabelSetName.Leave += TbLabelSetName_Leave;
            // 
            // BtDeleteLabelSet
            // 
            BtDeleteLabelSet.Location = new System.Drawing.Point(277, 6);
            BtDeleteLabelSet.Name = "BtDeleteLabelSet";
            BtDeleteLabelSet.Size = new System.Drawing.Size(63, 23);
            BtDeleteLabelSet.TabIndex = 4;
            BtDeleteLabelSet.Text = "delete";
            BtDeleteLabelSet.UseVisualStyleBackColor = true;
            BtDeleteLabelSet.Click += BtDeleteLabelSet_Click;
            // 
            // BtNewLabelSet
            // 
            BtNewLabelSet.Location = new System.Drawing.Point(214, 6);
            BtNewLabelSet.Name = "BtNewLabelSet";
            BtNewLabelSet.Size = new System.Drawing.Size(57, 23);
            BtNewLabelSet.TabIndex = 3;
            BtNewLabelSet.Text = "new";
            BtNewLabelSet.UseVisualStyleBackColor = true;
            BtNewLabelSet.Click += BtNewLabelSet_Click;
            // 
            // CbbLabelSets
            // 
            CbbLabelSets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CbbLabelSets.Location = new System.Drawing.Point(6, 8);
            CbbLabelSets.Name = "CbbLabelSets";
            CbbLabelSets.Size = new System.Drawing.Size(202, 23);
            CbbLabelSets.TabIndex = 2;
            CbbLabelSets.SelectedIndexChanged += CbbLabelSets_SelectedIndexChanged;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(BtSetStatPositionBasedOnFirstTwo);
            groupBox4.Controls.Add(chkbSetAllStatLabels);
            groupBox4.Controls.Add(label9);
            groupBox4.Controls.Add(nudHeightT);
            groupBox4.Controls.Add(label10);
            groupBox4.Controls.Add(nudWidthL);
            groupBox4.Controls.Add(label8);
            groupBox4.Controls.Add(label7);
            groupBox4.Controls.Add(nudHeight);
            groupBox4.Controls.Add(label6);
            groupBox4.Controls.Add(nudWidth);
            groupBox4.Controls.Add(label5);
            groupBox4.Controls.Add(nudY);
            groupBox4.Controls.Add(label4);
            groupBox4.Controls.Add(nudX);
            groupBox4.Location = new System.Drawing.Point(6, 355);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new System.Drawing.Size(293, 214);
            groupBox4.TabIndex = 1;
            groupBox4.TabStop = false;
            groupBox4.Text = "Rectangle";
            // 
            // BtSetStatPositionBasedOnFirstTwo
            // 
            BtSetStatPositionBasedOnFirstTwo.Location = new System.Drawing.Point(6, 176);
            BtSetStatPositionBasedOnFirstTwo.Name = "BtSetStatPositionBasedOnFirstTwo";
            BtSetStatPositionBasedOnFirstTwo.Size = new System.Drawing.Size(281, 23);
            BtSetStatPositionBasedOnFirstTwo.TabIndex = 14;
            BtSetStatPositionBasedOnFirstTwo.Text = "Set stat-positions based on HP and Stamina";
            BtSetStatPositionBasedOnFirstTwo.UseVisualStyleBackColor = true;
            BtSetStatPositionBasedOnFirstTwo.Click += BtSetStatPositionBasedOnFirstTwo_Click;
            // 
            // chkbSetAllStatLabels
            // 
            chkbSetAllStatLabels.AutoSize = true;
            chkbSetAllStatLabels.Location = new System.Drawing.Point(6, 153);
            chkbSetAllStatLabels.Name = "chkbSetAllStatLabels";
            chkbSetAllStatLabels.Size = new System.Drawing.Size(223, 19);
            chkbSetAllStatLabels.TabIndex = 13;
            chkbSetAllStatLabels.Text = "Set Values (except Y) for all stat-labels";
            chkbSetAllStatLabels.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(139, 119);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(55, 15);
            label9.TabIndex = 11;
            label9.Text = "Height-T";
            // 
            // nudHeightT
            // 
            nudHeightT.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            nudHeightT.Location = new System.Drawing.Point(193, 117);
            nudHeightT.Maximum = new decimal(new int[] { 4000, 0, 0, 0 });
            nudHeightT.Name = "nudHeightT";
            nudHeightT.Size = new System.Drawing.Size(77, 23);
            nudHeightT.TabIndex = 12;
            nudHeightT.ValueChanged += nudHeightT_ValueChanged;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(6, 119);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(50, 15);
            label10.TabIndex = 5;
            label10.Text = "Width-L";
            // 
            // nudWidthL
            // 
            nudWidthL.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            nudWidthL.Location = new System.Drawing.Point(56, 117);
            nudWidthL.Maximum = new decimal(new int[] { 4000, 0, 0, 0 });
            nudWidthL.Name = "nudWidthL";
            nudWidthL.Size = new System.Drawing.Size(77, 23);
            nudWidthL.TabIndex = 6;
            nudWidthL.ValueChanged += nudWidthL_ValueChanged;
            // 
            // label8
            // 
            label8.Location = new System.Drawing.Point(6, 16);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(281, 43);
            label8.TabIndex = 0;
            label8.Text = "The Height has to be the same for all texts in the same size. The text-baseline has to be exact in the same position for all labels with the same text-size.";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(149, 93);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(43, 15);
            label7.TabIndex = 9;
            label7.Text = "Height";
            // 
            // nudHeight
            // 
            nudHeight.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            nudHeight.Location = new System.Drawing.Point(193, 91);
            nudHeight.Maximum = new decimal(new int[] { 4000, 0, 0, 0 });
            nudHeight.Name = "nudHeight";
            nudHeight.Size = new System.Drawing.Size(77, 23);
            nudHeight.TabIndex = 10;
            nudHeight.ValueChanged += nudHeight_ValueChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(15, 93);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(39, 15);
            label6.TabIndex = 3;
            label6.Text = "Width";
            // 
            // nudWidth
            // 
            nudWidth.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            nudWidth.Location = new System.Drawing.Point(56, 91);
            nudWidth.Maximum = new decimal(new int[] { 4000, 0, 0, 0 });
            nudWidth.Name = "nudWidth";
            nudWidth.Size = new System.Drawing.Size(77, 23);
            nudWidth.TabIndex = 4;
            nudWidth.ValueChanged += nudWidth_ValueChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(173, 67);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(14, 15);
            label5.TabIndex = 7;
            label5.Text = "Y";
            // 
            // nudY
            // 
            nudY.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            nudY.Location = new System.Drawing.Point(193, 65);
            nudY.Maximum = new decimal(new int[] { 4000, 0, 0, 0 });
            nudY.Name = "nudY";
            nudY.Size = new System.Drawing.Size(77, 23);
            nudY.TabIndex = 8;
            nudY.ValueChanged += nudY_ValueChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(36, 67);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(14, 15);
            label4.TabIndex = 1;
            label4.Text = "X";
            // 
            // nudX
            // 
            nudX.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            nudX.Location = new System.Drawing.Point(56, 65);
            nudX.Maximum = new decimal(new int[] { 4000, 0, 0, 0 });
            nudX.Name = "nudX";
            nudX.Size = new System.Drawing.Size(77, 23);
            nudX.TabIndex = 2;
            nudX.ValueChanged += nudX_ValueChanged;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(listBoxLabelRectangles);
            groupBox3.Location = new System.Drawing.Point(6, 86);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(221, 263);
            groupBox3.TabIndex = 0;
            groupBox3.TabStop = false;
            groupBox3.Text = "Labelpositions";
            // 
            // listBoxLabelRectangles
            // 
            listBoxLabelRectangles.FormattingEnabled = true;
            listBoxLabelRectangles.Location = new System.Drawing.Point(6, 19);
            listBoxLabelRectangles.Name = "listBoxLabelRectangles";
            listBoxLabelRectangles.Size = new System.Drawing.Size(209, 229);
            listBoxLabelRectangles.TabIndex = 0;
            listBoxLabelRectangles.SelectedIndexChanged += listBoxLabelRectangles_SelectedIndexChanged;
            // 
            // OCRControl
            // 
            Controls.Add(tableLayoutPanel4);
            Name = "OCRControl";
            Size = new System.Drawing.Size(807, 726);
            tableLayoutPanel4.ResumeLayout(false);
            tabControlManage.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            groupBox11.ResumeLayout(false);
            groupBox11.PerformLayout();
            groupBox6.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ocrLetterEditTemplate).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWhiteTreshold).EndInit();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ocrLetterEditRecognized).EndInit();
            tabPage4.ResumeLayout(false);
            groupBox7.ResumeLayout(false);
            groupBox7.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudResolutionHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudResolutionWidth).EndInit();
            groupBox10.ResumeLayout(false);
            groupBox10.PerformLayout();
            groupBox9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)nudResizing).EndInit();
            groupBox8.ResumeLayout(false);
            groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudFontSizeCalibration).EndInit();
            tabPage3.ResumeLayout(false);
            groupBox12.ResumeLayout(false);
            groupBox12.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudHeightT).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWidthL).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudY).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudX).EndInit();
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.FlowLayoutPanel OCRDebugLayoutPanel;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ListBox listBoxRecognized;
        private GroupBoxC groupBox1;
        private System.Windows.Forms.Button btnSaveTemplate;
        private System.Windows.Forms.TextBox textBoxTemplate;
        private GroupBoxC groupBox2;
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
        private GroupBoxC groupBox3;
        private System.Windows.Forms.ListBox listBoxLabelRectangles;
        private GroupBoxC groupBox4;
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
        private GroupBoxC groupBox6;
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
        private GroupBoxC groupBox8;
        private GroupBoxC groupBox9;
        private System.Windows.Forms.Label lbResizeResult;
        private uiControls.Nud nudResizing;
        private System.Windows.Forms.Button BtUnloadOCR;
        private GroupBoxC groupBox10;
        private System.Windows.Forms.CheckBox CbSkipNameRecognition;
        private System.Windows.Forms.CheckBox CbSkipTribeRecognition;
        private System.Windows.Forms.CheckBox CbSkipOwnerRecognition;
        private System.Windows.Forms.CheckBox CbTrainRecognition;
        private System.Windows.Forms.ListBox ListBoxPatternsOfString;
        private System.Windows.Forms.Button BtRemovePattern;
        private System.Windows.Forms.Button BtNewOcrConfig;
        private System.Windows.Forms.Button BtCreateOcrPatternsFromManualChars;
        private GroupBoxC groupBox5;
        private System.Windows.Forms.Button buttonGetResFromScreenshot;
        private uiControls.Nud nudResolutionHeight;
        private System.Windows.Forms.Label label16;
        private uiControls.Nud nudResolutionWidth;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button BtSetStatPositionBasedOnFirstTwo;
        private GroupBoxC groupBox7;
        private System.Windows.Forms.Button BtRemoveSelectedPatterns;
        private System.Windows.Forms.TextBox TbRemovePatterns;
        private System.Windows.Forms.Button BtRemoveAllPatterns;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel LlOcrManual;
        private GroupBoxC groupBox11;
        private System.Windows.Forms.Label LbReplacingsFileStatus;
        private System.Windows.Forms.Button BtReplacingLoadFile;
        private System.Windows.Forms.Button BtReplacingOpenFile;
        private System.Windows.Forms.Label label18;
        private GroupBoxC groupBox12;
        private System.Windows.Forms.TextBox TbLabelSetName;
        private System.Windows.Forms.Button BtDeleteLabelSet;
        private System.Windows.Forms.Button BtNewLabelSet;
        private System.Windows.Forms.ComboBox CbbLabelSets;
    }
}
