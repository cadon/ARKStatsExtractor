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
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownHoursGrowing = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownHoursCooldown = new System.Windows.Forms.NumericUpDown();
            this.buttonSaveChanges = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxNote = new System.Windows.Forms.TextBox();
            this.buttonStatus = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonGender = new System.Windows.Forms.Button();
            this.textBoxOwner = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.buttonAdd2Library = new System.Windows.Forms.Button();
            this.dateTimePickerAdded = new System.Windows.Forms.DateTimePicker();
            this.parentComboBoxFather = new ARKBreedingStats.ParentComboBox();
            this.parentComboBoxMother = new ARKBreedingStats.ParentComboBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHoursGrowing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHoursCooldown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dateTimePickerAdded);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numericUpDownHoursGrowing);
            this.groupBox1.Controls.Add(this.numericUpDownHoursCooldown);
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
            this.groupBox1.Size = new System.Drawing.Size(229, 230);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Creature-info";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 153);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Cooldown [h]";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(120, 153);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Grown in [h]";
            // 
            // numericUpDownHoursGrowing
            // 
            this.numericUpDownHoursGrowing.Location = new System.Drawing.Point(184, 151);
            this.numericUpDownHoursGrowing.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownHoursGrowing.Name = "numericUpDownHoursGrowing";
            this.numericUpDownHoursGrowing.Size = new System.Drawing.Size(38, 20);
            this.numericUpDownHoursGrowing.TabIndex = 6;
            this.numericUpDownHoursGrowing.ValueChanged += new System.EventHandler(this.numericUpDownHoursGrowing_ValueChanged);
            // 
            // numericUpDownHoursCooldown
            // 
            this.numericUpDownHoursCooldown.Location = new System.Drawing.Point(76, 151);
            this.numericUpDownHoursCooldown.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownHoursCooldown.Name = "numericUpDownHoursCooldown";
            this.numericUpDownHoursCooldown.Size = new System.Drawing.Size(38, 20);
            this.numericUpDownHoursCooldown.TabIndex = 5;
            this.numericUpDownHoursCooldown.ValueChanged += new System.EventHandler(this.numericUpDownHoursCooldown_ValueChanged);
            // 
            // buttonSaveChanges
            // 
            this.buttonSaveChanges.Location = new System.Drawing.Point(89, 187);
            this.buttonSaveChanges.Name = "buttonSaveChanges";
            this.buttonSaveChanges.Size = new System.Drawing.Size(60, 37);
            this.buttonSaveChanges.TabIndex = 9;
            this.buttonSaveChanges.Text = "Save Changes";
            this.buttonSaveChanges.UseVisualStyleBackColor = true;
            this.buttonSaveChanges.Visible = false;
            this.buttonSaveChanges.Click += new System.EventHandler(this.buttonSaveChanges_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Note";
            // 
            // textBoxNote
            // 
            this.textBoxNote.Location = new System.Drawing.Point(50, 125);
            this.textBoxNote.Name = "textBoxNote";
            this.textBoxNote.Size = new System.Drawing.Size(172, 20);
            this.textBoxNote.TabIndex = 4;
            // 
            // buttonStatus
            // 
            this.buttonStatus.Location = new System.Drawing.Point(6, 175);
            this.buttonStatus.Name = "buttonStatus";
            this.buttonStatus.Size = new System.Drawing.Size(35, 22);
            this.buttonStatus.TabIndex = 7;
            this.buttonStatus.UseVisualStyleBackColor = true;
            this.buttonStatus.Click += new System.EventHandler(this.buttonStatus_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Father";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Mother";
            // 
            // buttonGender
            // 
            this.buttonGender.Location = new System.Drawing.Point(47, 175);
            this.buttonGender.Name = "buttonGender";
            this.buttonGender.Size = new System.Drawing.Size(35, 22);
            this.buttonGender.TabIndex = 8;
            this.buttonGender.Text = "?";
            this.buttonGender.UseVisualStyleBackColor = true;
            this.buttonGender.Click += new System.EventHandler(this.buttonGender_Click);
            // 
            // textBoxOwner
            // 
            this.textBoxOwner.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxOwner.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textBoxOwner.Location = new System.Drawing.Point(50, 45);
            this.textBoxOwner.Name = "textBoxOwner";
            this.textBoxOwner.Size = new System.Drawing.Size(172, 20);
            this.textBoxOwner.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 48);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(38, 13);
            this.label12.TabIndex = 12;
            this.label12.Text = "Owner";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 13);
            this.label11.TabIndex = 11;
            this.label11.Text = "Name";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(50, 19);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(172, 20);
            this.textBoxName.TabIndex = 0;
            // 
            // buttonAdd2Library
            // 
            this.buttonAdd2Library.Location = new System.Drawing.Point(89, 187);
            this.buttonAdd2Library.Name = "buttonAdd2Library";
            this.buttonAdd2Library.Size = new System.Drawing.Size(134, 37);
            this.buttonAdd2Library.TabIndex = 10;
            this.buttonAdd2Library.Text = "Add new to Library";
            this.buttonAdd2Library.UseVisualStyleBackColor = true;
            this.buttonAdd2Library.Click += new System.EventHandler(this.buttonAdd2Library_Click);
            // 
            // dateTimePickerAdded
            // 
            this.dateTimePickerAdded.Checked = false;
            this.dateTimePickerAdded.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerAdded.Location = new System.Drawing.Point(6, 203);
            this.dateTimePickerAdded.MinDate = new System.DateTime(2014, 12, 31, 0, 0, 0, 0);
            this.dateTimePickerAdded.Name = "dateTimePickerAdded";
            this.dateTimePickerAdded.Size = new System.Drawing.Size(76, 20);
            this.dateTimePickerAdded.TabIndex = 18;
            // 
            // parentComboBoxFather
            // 
            this.parentComboBoxFather.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.parentComboBoxFather.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parentComboBoxFather.FormattingEnabled = true;
            this.parentComboBoxFather.Location = new System.Drawing.Point(50, 98);
            this.parentComboBoxFather.Name = "parentComboBoxFather";
            this.parentComboBoxFather.Size = new System.Drawing.Size(172, 21);
            this.parentComboBoxFather.TabIndex = 3;
            // 
            // parentComboBoxMother
            // 
            this.parentComboBoxMother.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.parentComboBoxMother.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parentComboBoxMother.FormattingEnabled = true;
            this.parentComboBoxMother.Location = new System.Drawing.Point(50, 71);
            this.parentComboBoxMother.Name = "parentComboBoxMother";
            this.parentComboBoxMother.Size = new System.Drawing.Size(172, 21);
            this.parentComboBoxMother.TabIndex = 2;
            // 
            // CreatureInfoInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "CreatureInfoInput";
            this.Size = new System.Drawing.Size(229, 230);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHoursGrowing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHoursCooldown)).EndInit();
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
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDownHoursGrowing;
        private System.Windows.Forms.NumericUpDown numericUpDownHoursCooldown;
        private System.Windows.Forms.DateTimePicker dateTimePickerAdded;
    }
}
