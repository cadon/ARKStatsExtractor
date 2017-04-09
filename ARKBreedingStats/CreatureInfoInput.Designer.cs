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
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.labelMutations = new System.Windows.Forms.Label();
            this.numericUpDownMutations = new System.Windows.Forms.NumericUpDown();
            this.labelGrownPercent = new System.Windows.Forms.Label();
            this.dhmInputGrown = new ARKBreedingStats.uiControls.dhmInput();
            this.dhmInputCooldown = new ARKBreedingStats.uiControls.dhmInput();
            this.numericUpDownWeight = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxNeutered = new System.Windows.Forms.CheckBox();
            this.dateTimePickerAdded = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonSaveChanges = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxNote = new System.Windows.Forms.TextBox();
            this.buttonStatus = new System.Windows.Forms.Button();
            this.parentComboBoxFather = new ARKBreedingStats.ParentComboBox();
            this.parentComboBoxMother = new ARKBreedingStats.ParentComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSex = new System.Windows.Forms.Button();
            this.textBoxOwner = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.buttonAdd2Library = new System.Windows.Forms.Button();
            this.textBoxTribe = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMutations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWeight)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxTribe);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.labelMutations);
            this.groupBox1.Controls.Add(this.numericUpDownMutations);
            this.groupBox1.Controls.Add(this.labelGrownPercent);
            this.groupBox1.Controls.Add(this.dhmInputGrown);
            this.groupBox1.Controls.Add(this.dhmInputCooldown);
            this.groupBox1.Controls.Add(this.numericUpDownWeight);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.checkBoxNeutered);
            this.groupBox1.Controls.Add(this.dateTimePickerAdded);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.buttonSaveChanges);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBoxNote);
            this.groupBox1.Controls.Add(this.buttonStatus);
            this.groupBox1.Controls.Add(this.parentComboBoxFather);
            this.groupBox1.Controls.Add(this.parentComboBoxMother);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.buttonSex);
            this.groupBox1.Controls.Add(this.textBoxOwner);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.textBoxName);
            this.groupBox1.Controls.Add(this.buttonAdd2Library);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(229, 383);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Creature-info";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 315);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "Status";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 286);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(25, 13);
            this.label8.TabIndex = 26;
            this.label8.Text = "Sex";
            // 
            // labelMutations
            // 
            this.labelMutations.AutoSize = true;
            this.labelMutations.Location = new System.Drawing.Point(6, 257);
            this.labelMutations.Name = "labelMutations";
            this.labelMutations.Size = new System.Drawing.Size(53, 13);
            this.labelMutations.TabIndex = 25;
            this.labelMutations.Text = "Mutations";
            // 
            // numericUpDownMutations
            // 
            this.numericUpDownMutations.Location = new System.Drawing.Point(108, 255);
            this.numericUpDownMutations.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numericUpDownMutations.Name = "numericUpDownMutations";
            this.numericUpDownMutations.Size = new System.Drawing.Size(60, 20);
            this.numericUpDownMutations.TabIndex = 8;
            this.numericUpDownMutations.ValueChanged += new System.EventHandler(this.numericUpDownMutations_ValueChanged);
            // 
            // labelGrownPercent
            // 
            this.labelGrownPercent.AutoSize = true;
            this.labelGrownPercent.Location = new System.Drawing.Point(174, 206);
            this.labelGrownPercent.Name = "labelGrownPercent";
            this.labelGrownPercent.Size = new System.Drawing.Size(33, 13);
            this.labelGrownPercent.TabIndex = 23;
            this.labelGrownPercent.Text = "100%";
            // 
            // dhmInputGrown
            // 
            this.dhmInputGrown.BackColor = System.Drawing.SystemColors.Window;
            this.dhmInputGrown.ForeColor = System.Drawing.SystemColors.GrayText;
            this.dhmInputGrown.Location = new System.Drawing.Point(108, 203);
            this.dhmInputGrown.Mask = "00\\:00\\:00";
            this.dhmInputGrown.Name = "dhmInputGrown";
            this.dhmInputGrown.Size = new System.Drawing.Size(60, 20);
            this.dhmInputGrown.TabIndex = 6;
            this.dhmInputGrown.Text = "000000";
            this.dhmInputGrown.Timespan = System.TimeSpan.Parse("00:00:00");
            this.dhmInputGrown.TextChanged += new System.EventHandler(this.dhmInputGrown_TextChanged);
            // 
            // dhmInputCooldown
            // 
            this.dhmInputCooldown.BackColor = System.Drawing.SystemColors.Window;
            this.dhmInputCooldown.ForeColor = System.Drawing.SystemColors.GrayText;
            this.dhmInputCooldown.Location = new System.Drawing.Point(108, 177);
            this.dhmInputCooldown.Mask = "00\\:00\\:00";
            this.dhmInputCooldown.Name = "dhmInputCooldown";
            this.dhmInputCooldown.Size = new System.Drawing.Size(60, 20);
            this.dhmInputCooldown.TabIndex = 5;
            this.dhmInputCooldown.Text = "000000";
            this.dhmInputCooldown.Timespan = System.TimeSpan.Parse("00:00:00");
            // 
            // numericUpDownWeight
            // 
            this.numericUpDownWeight.DecimalPlaces = 2;
            this.numericUpDownWeight.Location = new System.Drawing.Point(146, 229);
            this.numericUpDownWeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownWeight.Name = "numericUpDownWeight";
            this.numericUpDownWeight.Size = new System.Drawing.Size(76, 20);
            this.numericUpDownWeight.TabIndex = 7;
            this.numericUpDownWeight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownWeight.ValueChanged += new System.EventHandler(this.numericUpDownWeight_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(105, 315);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 24;
            this.label7.Text = "Added";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(44, 231);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Current Weight";
            // 
            // checkBoxNeutered
            // 
            this.checkBoxNeutered.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxNeutered.AutoSize = true;
            this.checkBoxNeutered.Location = new System.Drawing.Point(108, 281);
            this.checkBoxNeutered.Name = "checkBoxNeutered";
            this.checkBoxNeutered.Size = new System.Drawing.Size(61, 23);
            this.checkBoxNeutered.TabIndex = 11;
            this.checkBoxNeutered.Text = "Neutered";
            this.checkBoxNeutered.UseVisualStyleBackColor = true;
            // 
            // dateTimePickerAdded
            // 
            this.dateTimePickerAdded.Checked = false;
            this.dateTimePickerAdded.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerAdded.Location = new System.Drawing.Point(146, 309);
            this.dateTimePickerAdded.MinDate = new System.DateTime(2014, 12, 31, 0, 0, 0, 0);
            this.dateTimePickerAdded.Name = "dateTimePickerAdded";
            this.dateTimePickerAdded.Size = new System.Drawing.Size(76, 20);
            this.dateTimePickerAdded.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 180);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Cooldown [d:h:m]";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 206);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Grown in [d:h:m]";
            // 
            // buttonSaveChanges
            // 
            this.buttonSaveChanges.Location = new System.Drawing.Point(88, 339);
            this.buttonSaveChanges.Name = "buttonSaveChanges";
            this.buttonSaveChanges.Size = new System.Drawing.Size(60, 37);
            this.buttonSaveChanges.TabIndex = 13;
            this.buttonSaveChanges.Text = "Save Changes";
            this.buttonSaveChanges.UseVisualStyleBackColor = true;
            this.buttonSaveChanges.Visible = false;
            this.buttonSaveChanges.Click += new System.EventHandler(this.buttonSaveChanges_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 154);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Note";
            // 
            // textBoxNote
            // 
            this.textBoxNote.Location = new System.Drawing.Point(50, 151);
            this.textBoxNote.Name = "textBoxNote";
            this.textBoxNote.Size = new System.Drawing.Size(172, 20);
            this.textBoxNote.TabIndex = 4;
            // 
            // buttonStatus
            // 
            this.buttonStatus.Location = new System.Drawing.Point(49, 310);
            this.buttonStatus.Name = "buttonStatus";
            this.buttonStatus.Size = new System.Drawing.Size(35, 23);
            this.buttonStatus.TabIndex = 10;
            this.buttonStatus.UseVisualStyleBackColor = true;
            this.buttonStatus.Click += new System.EventHandler(this.buttonStatus_Click);
            // 
            // parentComboBoxFather
            // 
            this.parentComboBoxFather.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.parentComboBoxFather.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parentComboBoxFather.FormattingEnabled = true;
            this.parentComboBoxFather.Location = new System.Drawing.Point(50, 124);
            this.parentComboBoxFather.Name = "parentComboBoxFather";
            this.parentComboBoxFather.Size = new System.Drawing.Size(172, 21);
            this.parentComboBoxFather.TabIndex = 3;
            this.parentComboBoxFather.SelectedIndexChanged += new System.EventHandler(this.parentComboBoxFather_SelectedIndexChanged);
            // 
            // parentComboBoxMother
            // 
            this.parentComboBoxMother.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.parentComboBoxMother.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parentComboBoxMother.FormattingEnabled = true;
            this.parentComboBoxMother.Location = new System.Drawing.Point(50, 97);
            this.parentComboBoxMother.Name = "parentComboBoxMother";
            this.parentComboBoxMother.Size = new System.Drawing.Size(172, 21);
            this.parentComboBoxMother.TabIndex = 2;
            this.parentComboBoxMother.SelectedIndexChanged += new System.EventHandler(this.parentComboBoxMother_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Father";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Mother";
            // 
            // buttonSex
            // 
            this.buttonSex.Location = new System.Drawing.Point(49, 281);
            this.buttonSex.Name = "buttonSex";
            this.buttonSex.Size = new System.Drawing.Size(35, 23);
            this.buttonSex.TabIndex = 9;
            this.buttonSex.Text = "?";
            this.buttonSex.UseVisualStyleBackColor = true;
            this.buttonSex.Click += new System.EventHandler(this.buttonGender_Click);
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
            this.label12.TabIndex = 16;
            this.label12.Text = "Owner";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 13);
            this.label11.TabIndex = 15;
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
            this.buttonAdd2Library.Location = new System.Drawing.Point(88, 339);
            this.buttonAdd2Library.Name = "buttonAdd2Library";
            this.buttonAdd2Library.Size = new System.Drawing.Size(134, 37);
            this.buttonAdd2Library.TabIndex = 14;
            this.buttonAdd2Library.Text = "Add new to Library";
            this.buttonAdd2Library.UseVisualStyleBackColor = true;
            this.buttonAdd2Library.Click += new System.EventHandler(this.buttonAdd2Library_Click);
            // 
            // textBoxTribe
            // 
            this.textBoxTribe.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxTribe.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textBoxTribe.Location = new System.Drawing.Point(50, 71);
            this.textBoxTribe.Name = "textBoxTribe";
            this.textBoxTribe.Size = new System.Drawing.Size(172, 20);
            this.textBoxTribe.TabIndex = 28;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 74);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(31, 13);
            this.label10.TabIndex = 29;
            this.label10.Text = "Tribe";
            // 
            // CreatureInfoInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "CreatureInfoInput";
            this.Size = new System.Drawing.Size(229, 383);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMutations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWeight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSex;
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
        private System.Windows.Forms.DateTimePicker dateTimePickerAdded;
        private System.Windows.Forms.CheckBox checkBoxNeutered;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDownWeight;
        private uiControls.dhmInput dhmInputGrown;
        private uiControls.dhmInput dhmInputCooldown;
        private System.Windows.Forms.Label labelGrownPercent;
        private System.Windows.Forms.NumericUpDown numericUpDownMutations;
        private System.Windows.Forms.Label labelMutations;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxTribe;
        private System.Windows.Forms.Label label10;
    }
}
