namespace ARKBreedingStats.uiControls
{
    partial class MultiSetter
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
            label1 = new System.Windows.Forms.Label();
            buttonStatus = new System.Windows.Forms.Button();
            buttonSex = new System.Windows.Forms.Button();
            checkBoxOwner = new System.Windows.Forms.CheckBox();
            checkBoxStatus = new System.Windows.Forms.CheckBox();
            checkBoxSex = new System.Windows.Forms.CheckBox();
            checkBoxIsBred = new System.Windows.Forms.CheckBox();
            checkBoxBred = new System.Windows.Forms.CheckBox();
            checkBoxMother = new System.Windows.Forms.CheckBox();
            checkBoxFather = new System.Windows.Forms.CheckBox();
            buttonApply = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            checkBoxNote = new System.Windows.Forms.CheckBox();
            textBoxNote = new System.Windows.Forms.TextBox();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            buttonColor6 = new System.Windows.Forms.Button();
            buttonColor5 = new System.Windows.Forms.Button();
            buttonColor4 = new System.Windows.Forms.Button();
            buttonColor3 = new System.Windows.Forms.Button();
            buttonColor2 = new System.Windows.Forms.Button();
            buttonColor1 = new System.Windows.Forms.Button();
            checkBoxColor1 = new System.Windows.Forms.CheckBox();
            checkBoxColor2 = new System.Windows.Forms.CheckBox();
            checkBoxColor3 = new System.Windows.Forms.CheckBox();
            checkBoxColor4 = new System.Windows.Forms.CheckBox();
            checkBoxColor5 = new System.Windows.Forms.CheckBox();
            checkBoxColor6 = new System.Windows.Forms.CheckBox();
            groupBoxTags = new GroupBoxC();
            flowLayoutPanelTags = new System.Windows.Forms.FlowLayoutPanel();
            tbNewTag = new System.Windows.Forms.TextBox();
            bAddTag = new System.Windows.Forms.Button();
            cbServer = new System.Windows.Forms.CheckBox();
            cbbSpecies = new System.Windows.Forms.ComboBox();
            checkBoxSpecies = new System.Windows.Forms.CheckBox();
            cbbServer = new System.Windows.Forms.ComboBox();
            cbbOwner = new System.Windows.Forms.ComboBox();
            cbbTribe = new System.Windows.Forms.ComboBox();
            cbTribe = new System.Windows.Forms.CheckBox();
            parentComboBoxFather = new ParentComboBox();
            parentComboBoxMother = new ParentComboBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBoxTags.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label1.Location = new System.Drawing.Point(14, 10);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(980, 45);
            label1.TabIndex = 28;
            label1.Text = "Checked properties will overwrite the current ones of all selected Creatures.";
            // 
            // buttonStatus
            // 
            buttonStatus.Location = new System.Drawing.Point(340, 115);
            buttonStatus.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonStatus.Name = "buttonStatus";
            buttonStatus.Size = new System.Drawing.Size(36, 27);
            buttonStatus.TabIndex = 1;
            buttonStatus.Text = "?";
            buttonStatus.UseVisualStyleBackColor = true;
            buttonStatus.Click += buttonStatus_Click;
            // 
            // buttonSex
            // 
            buttonSex.Location = new System.Drawing.Point(340, 149);
            buttonSex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonSex.Name = "buttonSex";
            buttonSex.Size = new System.Drawing.Size(36, 27);
            buttonSex.TabIndex = 2;
            buttonSex.Text = "?";
            buttonSex.UseVisualStyleBackColor = true;
            buttonSex.Click += buttonSex_Click;
            // 
            // checkBoxOwner
            // 
            checkBoxOwner.AutoSize = true;
            checkBoxOwner.Location = new System.Drawing.Point(383, 60);
            checkBoxOwner.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxOwner.Name = "checkBoxOwner";
            checkBoxOwner.Size = new System.Drawing.Size(61, 19);
            checkBoxOwner.TabIndex = 15;
            checkBoxOwner.Text = "Owner";
            checkBoxOwner.UseVisualStyleBackColor = true;
            // 
            // checkBoxStatus
            // 
            checkBoxStatus.AutoSize = true;
            checkBoxStatus.Location = new System.Drawing.Point(383, 120);
            checkBoxStatus.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxStatus.Name = "checkBoxStatus";
            checkBoxStatus.Size = new System.Drawing.Size(58, 19);
            checkBoxStatus.TabIndex = 16;
            checkBoxStatus.Text = "Status";
            checkBoxStatus.UseVisualStyleBackColor = true;
            // 
            // checkBoxSex
            // 
            checkBoxSex.AutoSize = true;
            checkBoxSex.Location = new System.Drawing.Point(383, 153);
            checkBoxSex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxSex.Name = "checkBoxSex";
            checkBoxSex.Size = new System.Drawing.Size(43, 19);
            checkBoxSex.TabIndex = 17;
            checkBoxSex.Text = "Sex";
            checkBoxSex.UseVisualStyleBackColor = true;
            // 
            // checkBoxIsBred
            // 
            checkBoxIsBred.AutoSize = true;
            checkBoxIsBred.Location = new System.Drawing.Point(358, 185);
            checkBoxIsBred.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxIsBred.Name = "checkBoxIsBred";
            checkBoxIsBred.Size = new System.Drawing.Size(15, 14);
            checkBoxIsBred.TabIndex = 3;
            checkBoxIsBred.UseVisualStyleBackColor = true;
            checkBoxIsBred.CheckedChanged += checkBoxIsBred_CheckedChanged;
            // 
            // checkBoxBred
            // 
            checkBoxBred.AutoSize = true;
            checkBoxBred.Location = new System.Drawing.Point(383, 185);
            checkBoxBred.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxBred.Name = "checkBoxBred";
            checkBoxBred.Size = new System.Drawing.Size(50, 19);
            checkBoxBred.TabIndex = 18;
            checkBoxBred.Text = "Bred";
            checkBoxBred.UseVisualStyleBackColor = true;
            // 
            // checkBoxMother
            // 
            checkBoxMother.AutoSize = true;
            checkBoxMother.Location = new System.Drawing.Point(383, 215);
            checkBoxMother.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxMother.Name = "checkBoxMother";
            checkBoxMother.Size = new System.Drawing.Size(65, 19);
            checkBoxMother.TabIndex = 19;
            checkBoxMother.Text = "Mother";
            checkBoxMother.UseVisualStyleBackColor = true;
            // 
            // checkBoxFather
            // 
            checkBoxFather.AutoSize = true;
            checkBoxFather.Location = new System.Drawing.Point(383, 246);
            checkBoxFather.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxFather.Name = "checkBoxFather";
            checkBoxFather.Size = new System.Drawing.Size(59, 19);
            checkBoxFather.TabIndex = 20;
            checkBoxFather.Text = "Father";
            checkBoxFather.UseVisualStyleBackColor = true;
            // 
            // buttonApply
            // 
            buttonApply.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonApply.Location = new System.Drawing.Point(907, 465);
            buttonApply.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonApply.Name = "buttonApply";
            buttonApply.Size = new System.Drawing.Size(88, 27);
            buttonApply.TabIndex = 13;
            buttonApply.Text = "Apply";
            buttonApply.UseVisualStyleBackColor = true;
            buttonApply.Click += buttonApply_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Location = new System.Drawing.Point(812, 465);
            buttonCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(88, 27);
            buttonCancel.TabIndex = 14;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // checkBoxNote
            // 
            checkBoxNote.AutoSize = true;
            checkBoxNote.Location = new System.Drawing.Point(383, 307);
            checkBoxNote.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxNote.Name = "checkBoxNote";
            checkBoxNote.Size = new System.Drawing.Size(52, 19);
            checkBoxNote.TabIndex = 21;
            checkBoxNote.Text = "Note";
            checkBoxNote.UseVisualStyleBackColor = true;
            // 
            // textBoxNote
            // 
            textBoxNote.Location = new System.Drawing.Point(18, 306);
            textBoxNote.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxNote.Multiline = true;
            textBoxNote.Name = "textBoxNote";
            textBoxNote.Size = new System.Drawing.Size(358, 49);
            textBoxNote.TabIndex = 6;
            textBoxNote.TextChanged += textBoxNote_TextChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new System.Drawing.Point(479, 58);
            pictureBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(149, 148);
            pictureBox1.TabIndex = 19;
            pictureBox1.TabStop = false;
            // 
            // buttonColor6
            // 
            buttonColor6.Location = new System.Drawing.Point(636, 205);
            buttonColor6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonColor6.Name = "buttonColor6";
            buttonColor6.Size = new System.Drawing.Size(27, 27);
            buttonColor6.TabIndex = 12;
            buttonColor6.Text = "5";
            buttonColor6.UseVisualStyleBackColor = true;
            buttonColor6.Click += buttonColor6_Click;
            // 
            // buttonColor5
            // 
            buttonColor5.Location = new System.Drawing.Point(636, 175);
            buttonColor5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonColor5.Name = "buttonColor5";
            buttonColor5.Size = new System.Drawing.Size(27, 27);
            buttonColor5.TabIndex = 11;
            buttonColor5.Text = "4";
            buttonColor5.UseVisualStyleBackColor = true;
            buttonColor5.Click += buttonColor5_Click;
            // 
            // buttonColor4
            // 
            buttonColor4.Location = new System.Drawing.Point(636, 145);
            buttonColor4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonColor4.Name = "buttonColor4";
            buttonColor4.Size = new System.Drawing.Size(27, 27);
            buttonColor4.TabIndex = 10;
            buttonColor4.Text = "3";
            buttonColor4.UseVisualStyleBackColor = true;
            buttonColor4.Click += buttonColor4_Click;
            // 
            // buttonColor3
            // 
            buttonColor3.Location = new System.Drawing.Point(636, 115);
            buttonColor3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonColor3.Name = "buttonColor3";
            buttonColor3.Size = new System.Drawing.Size(27, 27);
            buttonColor3.TabIndex = 9;
            buttonColor3.Text = "2";
            buttonColor3.UseVisualStyleBackColor = true;
            buttonColor3.Click += buttonColor3_Click;
            // 
            // buttonColor2
            // 
            buttonColor2.Location = new System.Drawing.Point(636, 85);
            buttonColor2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonColor2.Name = "buttonColor2";
            buttonColor2.Size = new System.Drawing.Size(27, 27);
            buttonColor2.TabIndex = 8;
            buttonColor2.Text = "1";
            buttonColor2.UseVisualStyleBackColor = true;
            buttonColor2.Click += buttonColor2_Click;
            // 
            // buttonColor1
            // 
            buttonColor1.Location = new System.Drawing.Point(636, 55);
            buttonColor1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonColor1.Name = "buttonColor1";
            buttonColor1.Size = new System.Drawing.Size(27, 27);
            buttonColor1.TabIndex = 7;
            buttonColor1.Text = "0";
            buttonColor1.UseVisualStyleBackColor = true;
            buttonColor1.Click += buttonColor1_Click;
            // 
            // checkBoxColor1
            // 
            checkBoxColor1.AutoSize = true;
            checkBoxColor1.Location = new System.Drawing.Point(670, 61);
            checkBoxColor1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxColor1.Name = "checkBoxColor1";
            checkBoxColor1.Size = new System.Drawing.Size(15, 14);
            checkBoxColor1.TabIndex = 22;
            checkBoxColor1.UseVisualStyleBackColor = true;
            // 
            // checkBoxColor2
            // 
            checkBoxColor2.AutoSize = true;
            checkBoxColor2.Location = new System.Drawing.Point(670, 91);
            checkBoxColor2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxColor2.Name = "checkBoxColor2";
            checkBoxColor2.Size = new System.Drawing.Size(15, 14);
            checkBoxColor2.TabIndex = 23;
            checkBoxColor2.UseVisualStyleBackColor = true;
            // 
            // checkBoxColor3
            // 
            checkBoxColor3.AutoSize = true;
            checkBoxColor3.Location = new System.Drawing.Point(670, 121);
            checkBoxColor3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxColor3.Name = "checkBoxColor3";
            checkBoxColor3.Size = new System.Drawing.Size(15, 14);
            checkBoxColor3.TabIndex = 24;
            checkBoxColor3.UseVisualStyleBackColor = true;
            // 
            // checkBoxColor4
            // 
            checkBoxColor4.AutoSize = true;
            checkBoxColor4.Location = new System.Drawing.Point(670, 151);
            checkBoxColor4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxColor4.Name = "checkBoxColor4";
            checkBoxColor4.Size = new System.Drawing.Size(15, 14);
            checkBoxColor4.TabIndex = 25;
            checkBoxColor4.UseVisualStyleBackColor = true;
            // 
            // checkBoxColor5
            // 
            checkBoxColor5.AutoSize = true;
            checkBoxColor5.Location = new System.Drawing.Point(670, 181);
            checkBoxColor5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxColor5.Name = "checkBoxColor5";
            checkBoxColor5.Size = new System.Drawing.Size(15, 14);
            checkBoxColor5.TabIndex = 26;
            checkBoxColor5.UseVisualStyleBackColor = true;
            // 
            // checkBoxColor6
            // 
            checkBoxColor6.AutoSize = true;
            checkBoxColor6.Location = new System.Drawing.Point(670, 211);
            checkBoxColor6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxColor6.Name = "checkBoxColor6";
            checkBoxColor6.Size = new System.Drawing.Size(15, 14);
            checkBoxColor6.TabIndex = 27;
            checkBoxColor6.UseVisualStyleBackColor = true;
            // 
            // groupBoxTags
            // 
            groupBoxTags.Controls.Add(flowLayoutPanelTags);
            groupBoxTags.Location = new System.Drawing.Point(716, 13);
            groupBoxTags.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxTags.Name = "groupBoxTags";
            groupBoxTags.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxTags.Size = new System.Drawing.Size(275, 382);
            groupBoxTags.TabIndex = 29;
            groupBoxTags.TabStop = false;
            groupBoxTags.Text = "Tags";
            // 
            // flowLayoutPanelTags
            // 
            flowLayoutPanelTags.AutoScroll = true;
            flowLayoutPanelTags.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanelTags.Location = new System.Drawing.Point(4, 19);
            flowLayoutPanelTags.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            flowLayoutPanelTags.Name = "flowLayoutPanelTags";
            flowLayoutPanelTags.Size = new System.Drawing.Size(267, 360);
            flowLayoutPanelTags.TabIndex = 37;
            // 
            // tbNewTag
            // 
            tbNewTag.Location = new System.Drawing.Point(716, 404);
            tbNewTag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tbNewTag.Name = "tbNewTag";
            tbNewTag.Size = new System.Drawing.Size(231, 23);
            tbNewTag.TabIndex = 30;
            // 
            // bAddTag
            // 
            bAddTag.Location = new System.Drawing.Point(955, 402);
            bAddTag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bAddTag.Name = "bAddTag";
            bAddTag.Size = new System.Drawing.Size(36, 27);
            bAddTag.TabIndex = 31;
            bAddTag.Text = "+";
            bAddTag.UseVisualStyleBackColor = true;
            bAddTag.Click += bAddTag_Click;
            // 
            // cbServer
            // 
            cbServer.AutoSize = true;
            cbServer.Location = new System.Drawing.Point(383, 277);
            cbServer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbServer.Name = "cbServer";
            cbServer.Size = new System.Drawing.Size(58, 19);
            cbServer.TabIndex = 33;
            cbServer.Text = "Server";
            cbServer.UseVisualStyleBackColor = true;
            // 
            // cbbSpecies
            // 
            cbbSpecies.FormattingEnabled = true;
            cbbSpecies.Location = new System.Drawing.Point(18, 362);
            cbbSpecies.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbbSpecies.Name = "cbbSpecies";
            cbbSpecies.Size = new System.Drawing.Size(358, 23);
            cbbSpecies.TabIndex = 35;
            cbbSpecies.SelectedIndexChanged += cbbSpecies_SelectedIndexChanged;
            // 
            // checkBoxSpecies
            // 
            checkBoxSpecies.AutoSize = true;
            checkBoxSpecies.Location = new System.Drawing.Point(383, 363);
            checkBoxSpecies.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxSpecies.Name = "checkBoxSpecies";
            checkBoxSpecies.Size = new System.Drawing.Size(65, 19);
            checkBoxSpecies.TabIndex = 36;
            checkBoxSpecies.Text = "Species";
            checkBoxSpecies.UseVisualStyleBackColor = true;
            checkBoxSpecies.CheckedChanged += checkBoxSpecies_CheckedChanged;
            // 
            // cbbServer
            // 
            cbbServer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            cbbServer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            cbbServer.FormattingEnabled = true;
            cbbServer.Location = new System.Drawing.Point(18, 273);
            cbbServer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbbServer.Name = "cbbServer";
            cbbServer.Size = new System.Drawing.Size(358, 23);
            cbbServer.TabIndex = 37;
            cbbServer.SelectedIndexChanged += cbbServer_SelectedIndexChanged;
            cbbServer.TextUpdate += cbbServer_TextUpdate;
            // 
            // cbbOwner
            // 
            cbbOwner.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            cbbOwner.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            cbbOwner.FormattingEnabled = true;
            cbbOwner.Location = new System.Drawing.Point(18, 58);
            cbbOwner.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbbOwner.Name = "cbbOwner";
            cbbOwner.Size = new System.Drawing.Size(358, 23);
            cbbOwner.TabIndex = 38;
            cbbOwner.SelectedIndexChanged += cbbOwner_SelectedIndexChanged;
            cbbOwner.TextUpdate += cbbOwner_TextUpdate;
            // 
            // cbbTribe
            // 
            cbbTribe.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            cbbTribe.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            cbbTribe.FormattingEnabled = true;
            cbbTribe.Location = new System.Drawing.Point(18, 89);
            cbbTribe.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbbTribe.Name = "cbbTribe";
            cbbTribe.Size = new System.Drawing.Size(358, 23);
            cbbTribe.TabIndex = 40;
            cbbTribe.SelectedIndexChanged += cbbTribe_SelectedIndexChanged;
            cbbTribe.TextUpdate += cbbTribe_TextUpdate;
            // 
            // cbTribe
            // 
            cbTribe.AutoSize = true;
            cbTribe.Location = new System.Drawing.Point(383, 91);
            cbTribe.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbTribe.Name = "cbTribe";
            cbTribe.Size = new System.Drawing.Size(52, 19);
            cbTribe.TabIndex = 39;
            cbTribe.Text = "Tribe";
            cbTribe.UseVisualStyleBackColor = true;
            // 
            // parentComboBoxFather
            // 
            parentComboBoxFather.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            parentComboBoxFather.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            parentComboBoxFather.FormattingEnabled = true;
            parentComboBoxFather.Location = new System.Drawing.Point(18, 245);
            parentComboBoxFather.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            parentComboBoxFather.Name = "parentComboBoxFather";
            parentComboBoxFather.Size = new System.Drawing.Size(358, 24);
            parentComboBoxFather.TabIndex = 5;
            parentComboBoxFather.SelectedIndexChanged += parentComboBoxFather_SelectedIndexChanged;
            // 
            // parentComboBoxMother
            // 
            parentComboBoxMother.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            parentComboBoxMother.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            parentComboBoxMother.FormattingEnabled = true;
            parentComboBoxMother.Location = new System.Drawing.Point(18, 213);
            parentComboBoxMother.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            parentComboBoxMother.Name = "parentComboBoxMother";
            parentComboBoxMother.Size = new System.Drawing.Size(358, 24);
            parentComboBoxMother.TabIndex = 4;
            parentComboBoxMother.SelectedIndexChanged += parentComboBoxMother_SelectedIndexChanged;
            // 
            // MultiSetter
            // 
            AcceptButton = buttonApply;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(1008, 505);
            Controls.Add(cbbTribe);
            Controls.Add(cbTribe);
            Controls.Add(cbbOwner);
            Controls.Add(cbbServer);
            Controls.Add(checkBoxSpecies);
            Controls.Add(cbbSpecies);
            Controls.Add(cbServer);
            Controls.Add(bAddTag);
            Controls.Add(tbNewTag);
            Controls.Add(groupBoxTags);
            Controls.Add(checkBoxColor6);
            Controls.Add(checkBoxColor5);
            Controls.Add(checkBoxColor4);
            Controls.Add(checkBoxColor3);
            Controls.Add(checkBoxColor2);
            Controls.Add(checkBoxColor1);
            Controls.Add(buttonColor6);
            Controls.Add(buttonColor5);
            Controls.Add(buttonColor4);
            Controls.Add(buttonColor3);
            Controls.Add(buttonColor2);
            Controls.Add(buttonColor1);
            Controls.Add(pictureBox1);
            Controls.Add(textBoxNote);
            Controls.Add(checkBoxNote);
            Controls.Add(buttonCancel);
            Controls.Add(buttonApply);
            Controls.Add(parentComboBoxFather);
            Controls.Add(parentComboBoxMother);
            Controls.Add(checkBoxFather);
            Controls.Add(checkBoxMother);
            Controls.Add(checkBoxBred);
            Controls.Add(checkBoxIsBred);
            Controls.Add(checkBoxSex);
            Controls.Add(checkBoxStatus);
            Controls.Add(checkBoxOwner);
            Controls.Add(buttonSex);
            Controls.Add(buttonStatus);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MultiSetter";
            ShowInTaskbar = false;
            Text = "MultiSetter";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBoxTags.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonStatus;
        private System.Windows.Forms.Button buttonSex;
        private System.Windows.Forms.CheckBox checkBoxOwner;
        private System.Windows.Forms.CheckBox checkBoxStatus;
        private System.Windows.Forms.CheckBox checkBoxSex;
        private System.Windows.Forms.CheckBox checkBoxIsBred;
        private System.Windows.Forms.CheckBox checkBoxBred;
        private System.Windows.Forms.CheckBox checkBoxMother;
        private System.Windows.Forms.CheckBox checkBoxFather;
        private ParentComboBox parentComboBoxMother;
        private ParentComboBox parentComboBoxFather;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxNote;
        private System.Windows.Forms.TextBox textBoxNote;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonColor6;
        private System.Windows.Forms.Button buttonColor5;
        private System.Windows.Forms.Button buttonColor4;
        private System.Windows.Forms.Button buttonColor3;
        private System.Windows.Forms.Button buttonColor2;
        private System.Windows.Forms.Button buttonColor1;
        private System.Windows.Forms.CheckBox checkBoxColor1;
        private System.Windows.Forms.CheckBox checkBoxColor2;
        private System.Windows.Forms.CheckBox checkBoxColor3;
        private System.Windows.Forms.CheckBox checkBoxColor4;
        private System.Windows.Forms.CheckBox checkBoxColor5;
        private System.Windows.Forms.CheckBox checkBoxColor6;
        private GroupBoxC groupBoxTags;
        private System.Windows.Forms.TextBox tbNewTag;
        private System.Windows.Forms.Button bAddTag;
        private System.Windows.Forms.CheckBox cbServer;
        private System.Windows.Forms.ComboBox cbbSpecies;
        private System.Windows.Forms.CheckBox checkBoxSpecies;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelTags;
        private System.Windows.Forms.ComboBox cbbServer;
        private System.Windows.Forms.ComboBox cbbOwner;
        private System.Windows.Forms.ComboBox cbbTribe;
        private System.Windows.Forms.CheckBox cbTribe;
    }
}