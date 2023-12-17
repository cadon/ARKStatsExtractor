using ARKBreedingStats.uiControls;

namespace ARKBreedingStats
{
    partial class CreatureBox
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonStatus = new System.Windows.Forms.Button();
            this.checkBoxIsBred = new System.Windows.Forms.CheckBox();
            this.textBoxNote = new System.Windows.Forms.TextBox();
            this.panelParents = new System.Windows.Forms.Panel();
            this.parentComboBoxFather = new ARKBreedingStats.uiControls.ParentComboBox();
            this.parentComboBoxMother = new ARKBreedingStats.uiControls.ParentComboBox();
            this.labelEditParents = new System.Windows.Forms.Label();
            this.labelF = new System.Windows.Forms.Label();
            this.labelM = new System.Windows.Forms.Label();
            this.buttonSex = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxOwner = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.statsDisplay1 = new ARKBreedingStats.uiControls.StatsDisplay();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelSpecies = new System.Windows.Forms.Label();
            this.LbFather = new System.Windows.Forms.Label();
            this.LbMotherAndWildInfo = new System.Windows.Forms.Label();
            this.labelNotes = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.regionColorChooser1 = new ARKBreedingStats.uiControls.RegionColorChooser();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelParents.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.buttonEdit);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(195, 406);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Creature";
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEdit.Image = global::ARKBreedingStats.Properties.Resources.pen;
            this.buttonEdit.Location = new System.Drawing.Point(178, 0);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(18, 18);
            this.buttonEdit.TabIndex = 0;
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonStatus);
            this.panel1.Controls.Add(this.checkBoxIsBred);
            this.panel1.Controls.Add(this.textBoxNote);
            this.panel1.Controls.Add(this.panelParents);
            this.panel1.Controls.Add(this.buttonSex);
            this.panel1.Controls.Add(this.textBoxName);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.buttonSave);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBoxOwner);
            this.panel1.Location = new System.Drawing.Point(6, 18);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(183, 229);
            this.panel1.TabIndex = 15;
            this.panel1.Visible = false;
            // 
            // buttonStatus
            // 
            this.buttonStatus.Location = new System.Drawing.Point(40, 205);
            this.buttonStatus.Name = "buttonStatus";
            this.buttonStatus.Size = new System.Drawing.Size(28, 19);
            this.buttonStatus.TabIndex = 48;
            this.buttonStatus.Text = "?";
            this.buttonStatus.UseVisualStyleBackColor = true;
            this.buttonStatus.Click += new System.EventHandler(this.buttonStatus_Click);
            // 
            // checkBoxIsBred
            // 
            this.checkBoxIsBred.AutoSize = true;
            this.checkBoxIsBred.Location = new System.Drawing.Point(129, 57);
            this.checkBoxIsBred.Name = "checkBoxIsBred";
            this.checkBoxIsBred.Size = new System.Drawing.Size(48, 17);
            this.checkBoxIsBred.TabIndex = 45;
            this.checkBoxIsBred.Text = "Bred";
            this.checkBoxIsBred.UseVisualStyleBackColor = true;
            this.checkBoxIsBred.CheckedChanged += new System.EventHandler(this.checkBoxIsBred_CheckedChanged);
            // 
            // textBoxNote
            // 
            this.textBoxNote.Location = new System.Drawing.Point(6, 123);
            this.textBoxNote.Multiline = true;
            this.textBoxNote.Name = "textBoxNote";
            this.textBoxNote.Size = new System.Drawing.Size(171, 73);
            this.textBoxNote.TabIndex = 37;
            // 
            // panelParents
            // 
            this.panelParents.Controls.Add(this.parentComboBoxFather);
            this.panelParents.Controls.Add(this.parentComboBoxMother);
            this.panelParents.Controls.Add(this.labelEditParents);
            this.panelParents.Controls.Add(this.labelF);
            this.panelParents.Controls.Add(this.labelM);
            this.panelParents.Location = new System.Drawing.Point(6, 55);
            this.panelParents.Name = "panelParents";
            this.panelParents.Size = new System.Drawing.Size(174, 62);
            this.panelParents.TabIndex = 36;
            this.panelParents.Visible = false;
            // 
            // parentComboBoxFather
            // 
            this.parentComboBoxFather.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.parentComboBoxFather.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parentComboBoxFather.FormattingEnabled = true;
            this.parentComboBoxFather.Location = new System.Drawing.Point(49, 41);
            this.parentComboBoxFather.Name = "parentComboBoxFather";
            this.parentComboBoxFather.PreselectedCreatureGuid = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.parentComboBoxFather.Size = new System.Drawing.Size(124, 21);
            this.parentComboBoxFather.TabIndex = 39;
            // 
            // parentComboBoxMother
            // 
            this.parentComboBoxMother.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.parentComboBoxMother.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parentComboBoxMother.FormattingEnabled = true;
            this.parentComboBoxMother.Location = new System.Drawing.Point(49, 19);
            this.parentComboBoxMother.Name = "parentComboBoxMother";
            this.parentComboBoxMother.PreselectedCreatureGuid = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.parentComboBoxMother.Size = new System.Drawing.Size(124, 21);
            this.parentComboBoxMother.TabIndex = 38;
            // 
            // labelEditParents
            // 
            this.labelEditParents.AutoSize = true;
            this.labelEditParents.Location = new System.Drawing.Point(3, 3);
            this.labelEditParents.Name = "labelEditParents";
            this.labelEditParents.Size = new System.Drawing.Size(43, 13);
            this.labelEditParents.TabIndex = 30;
            this.labelEditParents.Text = "Parents";
            // 
            // labelF
            // 
            this.labelF.AutoSize = true;
            this.labelF.Location = new System.Drawing.Point(1, 44);
            this.labelF.Name = "labelF";
            this.labelF.Size = new System.Drawing.Size(37, 13);
            this.labelF.TabIndex = 35;
            this.labelF.Text = "Father";
            // 
            // labelM
            // 
            this.labelM.AutoSize = true;
            this.labelM.Location = new System.Drawing.Point(1, 22);
            this.labelM.Name = "labelM";
            this.labelM.Size = new System.Drawing.Size(40, 13);
            this.labelM.TabIndex = 34;
            this.labelM.Text = "Mother";
            // 
            // buttonSex
            // 
            this.buttonSex.Location = new System.Drawing.Point(6, 205);
            this.buttonSex.Name = "buttonSex";
            this.buttonSex.Size = new System.Drawing.Size(28, 19);
            this.buttonSex.TabIndex = 33;
            this.buttonSex.Text = "?";
            this.buttonSex.UseVisualStyleBackColor = true;
            this.buttonSex.Click += new System.EventHandler(this.buttonSex_Click);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(44, 3);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(136, 20);
            this.textBoxName.TabIndex = 1;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(74, 203);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(53, 23);
            this.buttonCancel.TabIndex = 21;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(133, 203);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(47, 23);
            this.buttonSave.TabIndex = 20;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Owner";
            // 
            // textBoxOwner
            // 
            this.textBoxOwner.Location = new System.Drawing.Point(44, 29);
            this.textBoxOwner.Name = "textBoxOwner";
            this.textBoxOwner.Size = new System.Drawing.Size(136, 20);
            this.textBoxOwner.TabIndex = 14;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.statsDisplay1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(189, 387);
            this.tableLayoutPanel1.TabIndex = 30;
            // 
            // statsDisplay1
            // 
            this.statsDisplay1.AutoSize = true;
            this.statsDisplay1.Location = new System.Drawing.Point(0, 0);
            this.statsDisplay1.Margin = new System.Windows.Forms.Padding(0);
            this.statsDisplay1.Name = "statsDisplay1";
            this.statsDisplay1.Size = new System.Drawing.Size(189, 196);
            this.statsDisplay1.TabIndex = 28;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.labelSpecies);
            this.panel2.Controls.Add(this.LbFather);
            this.panel2.Controls.Add(this.LbMotherAndWildInfo);
            this.panel2.Controls.Add(this.labelNotes);
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.regionColorChooser1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 196);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(189, 191);
            this.panel2.TabIndex = 29;
            // 
            // labelSpecies
            // 
            this.labelSpecies.AutoSize = true;
            this.labelSpecies.Location = new System.Drawing.Point(3, 174);
            this.labelSpecies.Name = "labelSpecies";
            this.labelSpecies.Size = new System.Drawing.Size(0, 13);
            this.labelSpecies.TabIndex = 26;
            // 
            // LbFather
            // 
            this.LbFather.AutoSize = true;
            this.LbFather.Location = new System.Drawing.Point(0, 14);
            this.LbFather.Name = "LbFather";
            this.LbFather.Size = new System.Drawing.Size(0, 13);
            this.LbFather.TabIndex = 29;
            this.LbFather.Click += new System.EventHandler(this.LbFatherClick);
            // 
            // LbMotherAndWildInfo
            // 
            this.LbMotherAndWildInfo.Location = new System.Drawing.Point(0, 0);
            this.LbMotherAndWildInfo.Name = "LbMotherAndWildInfo";
            this.LbMotherAndWildInfo.Size = new System.Drawing.Size(183, 35);
            this.LbMotherAndWildInfo.TabIndex = 17;
            this.LbMotherAndWildInfo.Click += new System.EventHandler(this.LbMotherClick);
            // 
            // labelNotes
            // 
            this.labelNotes.Location = new System.Drawing.Point(0, 26);
            this.labelNotes.Margin = new System.Windows.Forms.Padding(0);
            this.labelNotes.Name = "labelNotes";
            this.labelNotes.Size = new System.Drawing.Size(183, 22);
            this.labelNotes.TabIndex = 18;
            this.labelNotes.Text = "Notes";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 56);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // regionColorChooser1
            // 
            this.regionColorChooser1.ColorIdsAlsoPossible = new byte[] {
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0))};
            this.regionColorChooser1.Location = new System.Drawing.Point(129, 96);
            this.regionColorChooser1.Margin = new System.Windows.Forms.Padding(0);
            this.regionColorChooser1.Name = "regionColorChooser1";
            this.regionColorChooser1.Size = new System.Drawing.Size(58, 88);
            this.regionColorChooser1.TabIndex = 27;
            this.regionColorChooser1.VerboseButtonTexts = false;
            // 
            // CreatureBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "CreatureBox";
            this.Size = new System.Drawing.Size(195, 406);
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelParents.ResumeLayout(false);
            this.panelParents.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxOwner;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSex;
        private System.Windows.Forms.Label labelEditParents;
        private System.Windows.Forms.Label labelF;
        private System.Windows.Forms.Label labelM;
        private System.Windows.Forms.Panel panelParents;
        private System.Windows.Forms.Label LbMotherAndWildInfo;
        private System.Windows.Forms.TextBox textBoxNote;
        private System.Windows.Forms.CheckBox checkBoxIsBred;
        private System.Windows.Forms.Label labelNotes;
        private System.Windows.Forms.Button buttonStatus;
        private System.Windows.Forms.PictureBox pictureBox1;
        private ParentComboBox parentComboBoxFather;
        private ParentComboBox parentComboBoxMother;
        private System.Windows.Forms.Label labelSpecies;
        private uiControls.RegionColorChooser regionColorChooser1;
        private uiControls.StatsDisplay statsDisplay1;
        private System.Windows.Forms.Label LbFather;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
    }
}
