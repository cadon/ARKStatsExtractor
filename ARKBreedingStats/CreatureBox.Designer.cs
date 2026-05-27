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
            groupBox1 = new System.Windows.Forms.GroupBox();
            buttonEdit = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            buttonStatus = new System.Windows.Forms.Button();
            checkBoxIsBred = new System.Windows.Forms.CheckBox();
            textBoxNote = new System.Windows.Forms.TextBox();
            panelParents = new System.Windows.Forms.Panel();
            parentComboBoxFather = new ParentComboBox();
            parentComboBoxMother = new ParentComboBox();
            labelEditParents = new System.Windows.Forms.Label();
            labelF = new System.Windows.Forms.Label();
            labelM = new System.Windows.Forms.Label();
            buttonSex = new System.Windows.Forms.Button();
            textBoxName = new System.Windows.Forms.TextBox();
            buttonCancel = new System.Windows.Forms.Button();
            buttonSave = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            textBoxOwner = new System.Windows.Forms.TextBox();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            statsDisplay1 = new StatsDisplay();
            panel2 = new System.Windows.Forms.Panel();
            labelNotes = new System.Windows.Forms.Label();
            LbFather = new System.Windows.Forms.Label();
            LbMotherAndWildInfo = new System.Windows.Forms.Label();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            regionColorChooser1 = new RegionColorChooser();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            panelParents.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.AutoSize = true;
            groupBox1.Controls.Add(buttonEdit);
            groupBox1.Controls.Add(panel1);
            groupBox1.Controls.Add(tableLayoutPanel1);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox1.Location = new System.Drawing.Point(0, 0);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(227, 468);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Creature";
            // 
            // buttonEdit
            // 
            buttonEdit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonEdit.Image = Properties.Resources.pen;
            buttonEdit.Location = new System.Drawing.Point(206, 0);
            buttonEdit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonEdit.Name = "buttonEdit";
            buttonEdit.Size = new System.Drawing.Size(21, 21);
            buttonEdit.TabIndex = 0;
            buttonEdit.UseVisualStyleBackColor = true;
            buttonEdit.Click += buttonEdit_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(buttonStatus);
            panel1.Controls.Add(checkBoxIsBred);
            panel1.Controls.Add(textBoxNote);
            panel1.Controls.Add(panelParents);
            panel1.Controls.Add(buttonSex);
            panel1.Controls.Add(textBoxName);
            panel1.Controls.Add(buttonCancel);
            panel1.Controls.Add(buttonSave);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(textBoxOwner);
            panel1.Location = new System.Drawing.Point(7, 21);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(214, 264);
            panel1.TabIndex = 15;
            panel1.Visible = false;
            // 
            // buttonStatus
            // 
            buttonStatus.Location = new System.Drawing.Point(47, 237);
            buttonStatus.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonStatus.Name = "buttonStatus";
            buttonStatus.Size = new System.Drawing.Size(33, 22);
            buttonStatus.TabIndex = 48;
            buttonStatus.Text = "?";
            buttonStatus.UseVisualStyleBackColor = true;
            buttonStatus.Click += buttonStatus_Click;
            // 
            // checkBoxIsBred
            // 
            checkBoxIsBred.AutoSize = true;
            checkBoxIsBred.Location = new System.Drawing.Point(150, 66);
            checkBoxIsBred.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxIsBred.Name = "checkBoxIsBred";
            checkBoxIsBred.Size = new System.Drawing.Size(50, 19);
            checkBoxIsBred.TabIndex = 45;
            checkBoxIsBred.Text = "Bred";
            checkBoxIsBred.UseVisualStyleBackColor = true;
            checkBoxIsBred.CheckedChanged += checkBoxIsBred_CheckedChanged;
            // 
            // textBoxNote
            // 
            textBoxNote.Location = new System.Drawing.Point(7, 142);
            textBoxNote.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxNote.Multiline = true;
            textBoxNote.Name = "textBoxNote";
            textBoxNote.Size = new System.Drawing.Size(199, 84);
            textBoxNote.TabIndex = 37;
            // 
            // panelParents
            // 
            panelParents.Controls.Add(parentComboBoxFather);
            panelParents.Controls.Add(parentComboBoxMother);
            panelParents.Controls.Add(labelEditParents);
            panelParents.Controls.Add(labelF);
            panelParents.Controls.Add(labelM);
            panelParents.Location = new System.Drawing.Point(7, 63);
            panelParents.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelParents.Name = "panelParents";
            panelParents.Size = new System.Drawing.Size(203, 72);
            panelParents.TabIndex = 36;
            panelParents.Visible = false;
            // 
            // parentComboBoxFather
            // 
            parentComboBoxFather.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            parentComboBoxFather.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            parentComboBoxFather.FormattingEnabled = true;
            parentComboBoxFather.Location = new System.Drawing.Point(57, 47);
            parentComboBoxFather.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            parentComboBoxFather.Name = "parentComboBoxFather";
            parentComboBoxFather.Size = new System.Drawing.Size(144, 24);
            parentComboBoxFather.TabIndex = 39;
            // 
            // parentComboBoxMother
            // 
            parentComboBoxMother.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            parentComboBoxMother.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            parentComboBoxMother.FormattingEnabled = true;
            parentComboBoxMother.Location = new System.Drawing.Point(57, 22);
            parentComboBoxMother.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            parentComboBoxMother.Name = "parentComboBoxMother";
            parentComboBoxMother.Size = new System.Drawing.Size(144, 24);
            parentComboBoxMother.TabIndex = 38;
            // 
            // labelEditParents
            // 
            labelEditParents.AutoSize = true;
            labelEditParents.Location = new System.Drawing.Point(4, 3);
            labelEditParents.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelEditParents.Name = "labelEditParents";
            labelEditParents.Size = new System.Drawing.Size(46, 15);
            labelEditParents.TabIndex = 30;
            labelEditParents.Text = "Parents";
            // 
            // labelF
            // 
            labelF.AutoSize = true;
            labelF.Location = new System.Drawing.Point(1, 51);
            labelF.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelF.Name = "labelF";
            labelF.Size = new System.Drawing.Size(40, 15);
            labelF.TabIndex = 35;
            labelF.Text = "Father";
            // 
            // labelM
            // 
            labelM.AutoSize = true;
            labelM.Location = new System.Drawing.Point(1, 25);
            labelM.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelM.Name = "labelM";
            labelM.Size = new System.Drawing.Size(46, 15);
            labelM.TabIndex = 34;
            labelM.Text = "Mother";
            // 
            // buttonSex
            // 
            buttonSex.Location = new System.Drawing.Point(7, 237);
            buttonSex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonSex.Name = "buttonSex";
            buttonSex.Size = new System.Drawing.Size(33, 22);
            buttonSex.TabIndex = 33;
            buttonSex.Text = "?";
            buttonSex.UseVisualStyleBackColor = true;
            buttonSex.Click += buttonSex_Click;
            // 
            // textBoxName
            // 
            textBoxName.Location = new System.Drawing.Point(51, 3);
            textBoxName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new System.Drawing.Size(158, 23);
            textBoxName.TabIndex = 1;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new System.Drawing.Point(86, 234);
            buttonCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(62, 27);
            buttonCancel.TabIndex = 21;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonSave
            // 
            buttonSave.Location = new System.Drawing.Point(155, 234);
            buttonSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(55, 27);
            buttonSave.TabIndex = 20;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(4, 7);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(39, 15);
            label2.TabIndex = 16;
            label2.Text = "Name";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(4, 37);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(42, 15);
            label1.TabIndex = 15;
            label1.Text = "Owner";
            // 
            // textBoxOwner
            // 
            textBoxOwner.Location = new System.Drawing.Point(51, 33);
            textBoxOwner.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxOwner.Name = "textBoxOwner";
            textBoxOwner.Size = new System.Drawing.Size(158, 23);
            textBoxOwner.TabIndex = 14;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(statsDisplay1, 0, 0);
            tableLayoutPanel1.Controls.Add(panel2, 0, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(4, 19);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(219, 446);
            tableLayoutPanel1.TabIndex = 30;
            // 
            // statsDisplay1
            // 
            statsDisplay1.AutoSize = true;
            statsDisplay1.Dock = System.Windows.Forms.DockStyle.Top;
            statsDisplay1.Location = new System.Drawing.Point(0, 0);
            statsDisplay1.Margin = new System.Windows.Forms.Padding(0);
            statsDisplay1.Name = "statsDisplay1";
            statsDisplay1.Size = new System.Drawing.Size(219, 21);
            statsDisplay1.TabIndex = 28;
            // 
            // panel2
            // 
            panel2.AutoSize = true;
            panel2.Controls.Add(labelNotes);
            panel2.Controls.Add(LbFather);
            panel2.Controls.Add(LbMotherAndWildInfo);
            panel2.Controls.Add(pictureBox1);
            panel2.Controls.Add(regionColorChooser1);
            panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            panel2.Location = new System.Drawing.Point(0, 21);
            panel2.Margin = new System.Windows.Forms.Padding(0);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(219, 425);
            panel2.TabIndex = 29;
            // 
            // labelNotes
            // 
            labelNotes.Location = new System.Drawing.Point(0, 30);
            labelNotes.Margin = new System.Windows.Forms.Padding(0);
            labelNotes.Name = "labelNotes";
            labelNotes.Size = new System.Drawing.Size(214, 25);
            labelNotes.TabIndex = 18;
            labelNotes.Text = "Notes";
            // 
            // LbFather
            // 
            LbFather.AutoSize = true;
            LbFather.Location = new System.Drawing.Point(0, 16);
            LbFather.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbFather.Name = "LbFather";
            LbFather.Size = new System.Drawing.Size(0, 15);
            LbFather.TabIndex = 29;
            LbFather.Click += LbFatherClick;
            // 
            // LbMotherAndWildInfo
            // 
            LbMotherAndWildInfo.Location = new System.Drawing.Point(0, 0);
            LbMotherAndWildInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbMotherAndWildInfo.Name = "LbMotherAndWildInfo";
            LbMotherAndWildInfo.Size = new System.Drawing.Size(214, 40);
            LbMotherAndWildInfo.TabIndex = 17;
            LbMotherAndWildInfo.Click += LbMotherClick;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new System.Drawing.Point(0, 65);
            pictureBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(149, 148);
            pictureBox1.TabIndex = 19;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // regionColorChooser1
            // 
            regionColorChooser1.Location = new System.Drawing.Point(150, 111);
            regionColorChooser1.Margin = new System.Windows.Forms.Padding(0);
            regionColorChooser1.Name = "regionColorChooser1";
            regionColorChooser1.Size = new System.Drawing.Size(68, 102);
            regionColorChooser1.TabIndex = 27;
            // 
            // CreatureBox
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(groupBox1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "CreatureBox";
            Size = new System.Drawing.Size(227, 468);
            groupBox1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panelParents.ResumeLayout(false);
            panelParents.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();

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
        private uiControls.RegionColorChooser regionColorChooser1;
        private uiControls.StatsDisplay statsDisplay1;
        private System.Windows.Forms.Label LbFather;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
    }
}
