namespace ARKBreedingStats
{
    partial class CreatureInfoInput
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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonGender = new System.Windows.Forms.Button();
            this.textBoxOwner = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.buttonAdd2Library = new System.Windows.Forms.Button();
            this.buttonStatus = new System.Windows.Forms.Button();
            this.textBoxNote = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.parentComboBoxFather = new ARKBreedingStats.ParentComboBox();
            this.parentComboBoxMother = new ARKBreedingStats.ParentComboBox();
            this.buttonSaveChanges = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonSaveChanges);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBoxNote);
            this.groupBox1.Controls.Add(this.buttonStatus);
            this.groupBox1.Controls.Add(this.parentComboBoxFather);
            this.groupBox1.Controls.Add(this.parentComboBoxMother);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.buttonGender);
            this.groupBox1.Controls.Add(this.textBoxOwner);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.textBoxName);
            this.groupBox1.Controls.Add(this.buttonAdd2Library);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(229, 191);
            this.groupBox1.TabIndex = 42;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Creature-info";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 48;
            this.label2.Text = "Father";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 47;
            this.label1.Text = "Mother";
            // 
            // buttonGender
            // 
            this.buttonGender.Location = new System.Drawing.Point(47, 151);
            this.buttonGender.Name = "buttonGender";
            this.buttonGender.Size = new System.Drawing.Size(35, 22);
            this.buttonGender.TabIndex = 44;
            this.buttonGender.Text = "?";
            this.buttonGender.UseVisualStyleBackColor = true;
            this.buttonGender.Click += new System.EventHandler(this.buttonGender_Click);
            // 
            // textBoxOwner
            // 
            this.textBoxOwner.Location = new System.Drawing.Point(50, 45);
            this.textBoxOwner.Name = "textBoxOwner";
            this.textBoxOwner.Size = new System.Drawing.Size(172, 20);
            this.textBoxOwner.TabIndex = 42;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 48);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(38, 13);
            this.label12.TabIndex = 41;
            this.label12.Text = "Owner";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 13);
            this.label11.TabIndex = 40;
            this.label11.Text = "Name";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(50, 19);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(172, 20);
            this.textBoxName.TabIndex = 39;
            // 
            // buttonAdd2Library
            // 
            this.buttonAdd2Library.Location = new System.Drawing.Point(88, 151);
            this.buttonAdd2Library.Name = "buttonAdd2Library";
            this.buttonAdd2Library.Size = new System.Drawing.Size(134, 37);
            this.buttonAdd2Library.TabIndex = 15;
            this.buttonAdd2Library.Text = "Add new to Library";
            this.buttonAdd2Library.UseVisualStyleBackColor = true;
            this.buttonAdd2Library.Click += new System.EventHandler(this.buttonAdd2Library_Click);
            // 
            // buttonStatus
            // 
            this.buttonStatus.Location = new System.Drawing.Point(6, 151);
            this.buttonStatus.Name = "buttonStatus";
            this.buttonStatus.Size = new System.Drawing.Size(35, 22);
            this.buttonStatus.TabIndex = 51;
            this.buttonStatus.UseVisualStyleBackColor = true;
            this.buttonStatus.Click += new System.EventHandler(this.buttonStatus_Click);
            // 
            // textBoxNote
            // 
            this.textBoxNote.Location = new System.Drawing.Point(50, 125);
            this.textBoxNote.Name = "textBoxNote";
            this.textBoxNote.Size = new System.Drawing.Size(172, 20);
            this.textBoxNote.TabIndex = 52;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 53;
            this.label3.Text = "Note";
            // 
            // parentComboBoxFather
            // 
            this.parentComboBoxFather.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.parentComboBoxFather.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parentComboBoxFather.FormattingEnabled = true;
            this.parentComboBoxFather.Location = new System.Drawing.Point(50, 98);
            this.parentComboBoxFather.Name = "parentComboBoxFather";
            this.parentComboBoxFather.Size = new System.Drawing.Size(172, 21);
            this.parentComboBoxFather.TabIndex = 50;
            // 
            // parentComboBoxMother
            // 
            this.parentComboBoxMother.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.parentComboBoxMother.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parentComboBoxMother.FormattingEnabled = true;
            this.parentComboBoxMother.Location = new System.Drawing.Point(50, 71);
            this.parentComboBoxMother.Name = "parentComboBoxMother";
            this.parentComboBoxMother.Size = new System.Drawing.Size(172, 21);
            this.parentComboBoxMother.TabIndex = 49;
            // 
            // buttonSaveChanges
            // 
            this.buttonSaveChanges.Location = new System.Drawing.Point(88, 151);
            this.buttonSaveChanges.Name = "buttonSaveChanges";
            this.buttonSaveChanges.Size = new System.Drawing.Size(60, 37);
            this.buttonSaveChanges.TabIndex = 54;
            this.buttonSaveChanges.Text = "Save Changes";
            this.buttonSaveChanges.UseVisualStyleBackColor = true;
            this.buttonSaveChanges.Visible = false;
            this.buttonSaveChanges.Click += new System.EventHandler(this.buttonSaveChanges_Click);
            // 
            // CreatureInfoInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "CreatureInfoInput";
            this.Size = new System.Drawing.Size(229, 191);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonGender;
        private System.Windows.Forms.TextBox textBoxOwner;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button buttonAdd2Library;
        private ParentComboBox parentComboBoxMother;
        private ParentComboBox parentComboBoxFather;
        private System.Windows.Forms.Button buttonStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxNote;
        private System.Windows.Forms.Button buttonSaveChanges;
    }
}
