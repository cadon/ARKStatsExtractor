using ARKBreedingStats.uiControls;

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
            this.gbCreatureInfo = new System.Windows.Forms.GroupBox();
            this.btNamingPatternEditor = new System.Windows.Forms.Button();
            this.btnGenerateUniqueName = new System.Windows.Forms.Button();
            this.tbArkIdIngame = new System.Windows.Forms.TextBox();
            this.tbARKID = new System.Windows.Forms.TextBox();
            this.cbServer = new System.Windows.Forms.ComboBox();
            this.textBoxTribe = new System.Windows.Forms.TextBox();
            this.textBoxNote = new System.Windows.Forms.TextBox();
            this.parentComboBoxFather = new ARKBreedingStats.uiControls.ParentComboBox();
            this.parentComboBoxMother = new ARKBreedingStats.uiControls.ParentComboBox();
            this.textBoxOwner = new System.Windows.Forms.TextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.nudMutationsFather = new ARKBreedingStats.uiControls.Nud();
            this.nudMutationsMother = new ARKBreedingStats.uiControls.Nud();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dhmsInputGrown = new ARKBreedingStats.uiControls.dhmsInput();
            this.dhmsInputCooldown = new ARKBreedingStats.uiControls.dhmsInput();
            this.nudMaturation = new ARKBreedingStats.uiControls.Nud();
            this.cbNeutered = new System.Windows.Forms.CheckBox();
            this.dateTimePickerAdded = new System.Windows.Forms.DateTimePicker();
            this.buttonStatus = new System.Windows.Forms.Button();
            this.buttonSex = new System.Windows.Forms.Button();
            this.lbNewMutations = new System.Windows.Forms.Label();
            this.lbArkIdIngame = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btClearColors = new System.Windows.Forms.Button();
            this.regionColorChooser1 = new ARKBreedingStats.uiControls.RegionColorChooser();
            this.lbServer = new System.Windows.Forms.Label();
            this.lbTribe = new System.Windows.Forms.Label();
            this.lbStatus = new System.Windows.Forms.Label();
            this.lbSex = new System.Windows.Forms.Label();
            this.lbMutations = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lbMaturationPerc = new System.Windows.Forms.Label();
            this.lbCooldown = new System.Windows.Forms.Label();
            this.lbGrownIn = new System.Windows.Forms.Label();
            this.btSaveChanges = new System.Windows.Forms.Button();
            this.lbNote = new System.Windows.Forms.Label();
            this.lbFather = new System.Windows.Forms.Label();
            this.lbMother = new System.Windows.Forms.Label();
            this.lbOwner = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.btAdd2Library = new System.Windows.Forms.Button();
            this.btNamingPattern2 = new System.Windows.Forms.Button();
            this.btNamingPattern3 = new System.Windows.Forms.Button();
            this.btNamingPattern4 = new System.Windows.Forms.Button();
            this.btNamingPattern5 = new System.Windows.Forms.Button();
            this.btNamingPattern6 = new System.Windows.Forms.Button();
            this.gbCreatureInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMutationsFather)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMutationsMother)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaturation)).BeginInit();
            this.SuspendLayout();
            // 
            // gbCreatureInfo
            // 
            this.gbCreatureInfo.Controls.Add(this.btNamingPattern6);
            this.gbCreatureInfo.Controls.Add(this.btNamingPattern5);
            this.gbCreatureInfo.Controls.Add(this.btNamingPattern4);
            this.gbCreatureInfo.Controls.Add(this.btNamingPattern3);
            this.gbCreatureInfo.Controls.Add(this.btNamingPattern2);
            this.gbCreatureInfo.Controls.Add(this.btNamingPatternEditor);
            this.gbCreatureInfo.Controls.Add(this.btnGenerateUniqueName);
            this.gbCreatureInfo.Controls.Add(this.tbArkIdIngame);
            this.gbCreatureInfo.Controls.Add(this.tbARKID);
            this.gbCreatureInfo.Controls.Add(this.cbServer);
            this.gbCreatureInfo.Controls.Add(this.textBoxTribe);
            this.gbCreatureInfo.Controls.Add(this.textBoxNote);
            this.gbCreatureInfo.Controls.Add(this.parentComboBoxFather);
            this.gbCreatureInfo.Controls.Add(this.parentComboBoxMother);
            this.gbCreatureInfo.Controls.Add(this.textBoxOwner);
            this.gbCreatureInfo.Controls.Add(this.textBoxName);
            this.gbCreatureInfo.Controls.Add(this.nudMutationsFather);
            this.gbCreatureInfo.Controls.Add(this.nudMutationsMother);
            this.gbCreatureInfo.Controls.Add(this.label12);
            this.gbCreatureInfo.Controls.Add(this.label11);
            this.gbCreatureInfo.Controls.Add(this.dhmsInputGrown);
            this.gbCreatureInfo.Controls.Add(this.dhmsInputCooldown);
            this.gbCreatureInfo.Controls.Add(this.nudMaturation);
            this.gbCreatureInfo.Controls.Add(this.cbNeutered);
            this.gbCreatureInfo.Controls.Add(this.dateTimePickerAdded);
            this.gbCreatureInfo.Controls.Add(this.buttonStatus);
            this.gbCreatureInfo.Controls.Add(this.buttonSex);
            this.gbCreatureInfo.Controls.Add(this.lbNewMutations);
            this.gbCreatureInfo.Controls.Add(this.lbArkIdIngame);
            this.gbCreatureInfo.Controls.Add(this.label1);
            this.gbCreatureInfo.Controls.Add(this.btClearColors);
            this.gbCreatureInfo.Controls.Add(this.regionColorChooser1);
            this.gbCreatureInfo.Controls.Add(this.lbServer);
            this.gbCreatureInfo.Controls.Add(this.lbTribe);
            this.gbCreatureInfo.Controls.Add(this.lbStatus);
            this.gbCreatureInfo.Controls.Add(this.lbSex);
            this.gbCreatureInfo.Controls.Add(this.lbMutations);
            this.gbCreatureInfo.Controls.Add(this.label7);
            this.gbCreatureInfo.Controls.Add(this.lbMaturationPerc);
            this.gbCreatureInfo.Controls.Add(this.lbCooldown);
            this.gbCreatureInfo.Controls.Add(this.lbGrownIn);
            this.gbCreatureInfo.Controls.Add(this.btSaveChanges);
            this.gbCreatureInfo.Controls.Add(this.lbNote);
            this.gbCreatureInfo.Controls.Add(this.lbFather);
            this.gbCreatureInfo.Controls.Add(this.lbMother);
            this.gbCreatureInfo.Controls.Add(this.lbOwner);
            this.gbCreatureInfo.Controls.Add(this.lbName);
            this.gbCreatureInfo.Controls.Add(this.btAdd2Library);
            this.gbCreatureInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbCreatureInfo.Location = new System.Drawing.Point(0, 0);
            this.gbCreatureInfo.Name = "gbCreatureInfo";
            this.gbCreatureInfo.Size = new System.Drawing.Size(229, 518);
            this.gbCreatureInfo.TabIndex = 0;
            this.gbCreatureInfo.TabStop = false;
            this.gbCreatureInfo.Text = "Creature-info";
            this.gbCreatureInfo.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // btNamingPatternEditor
            // 
            this.btNamingPatternEditor.Location = new System.Drawing.Point(201, 19);
            this.btNamingPatternEditor.Name = "btNamingPatternEditor";
            this.btNamingPatternEditor.Size = new System.Drawing.Size(22, 20);
            this.btNamingPatternEditor.TabIndex = 43;
            this.btNamingPatternEditor.Text = "⚙";
            this.btNamingPatternEditor.UseVisualStyleBackColor = true;
            this.btNamingPatternEditor.Click += new System.EventHandler(this.btNamingPatternEditor_Click);
            // 
            // btnGenerateUniqueName
            // 
            this.btnGenerateUniqueName.Location = new System.Drawing.Point(177, 19);
            this.btnGenerateUniqueName.Name = "btnGenerateUniqueName";
            this.btnGenerateUniqueName.Size = new System.Drawing.Size(22, 20);
            this.btnGenerateUniqueName.TabIndex = 1;
            this.btnGenerateUniqueName.TabStop = false;
            this.btnGenerateUniqueName.Text = "Generate";
            this.btnGenerateUniqueName.UseVisualStyleBackColor = true;
            // 
            // tbArkIdIngame
            // 
            this.tbArkIdIngame.Location = new System.Drawing.Point(83, 255);
            this.tbArkIdIngame.Name = "tbArkIdIngame";
            this.tbArkIdIngame.ReadOnly = true;
            this.tbArkIdIngame.Size = new System.Drawing.Size(138, 20);
            this.tbArkIdIngame.TabIndex = 40;
            // 
            // tbARKID
            // 
            this.tbARKID.Location = new System.Drawing.Point(50, 229);
            this.tbARKID.Name = "tbARKID";
            this.tbARKID.Size = new System.Drawing.Size(172, 20);
            this.tbARKID.TabIndex = 8;
            // 
            // cbServer
            // 
            this.cbServer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cbServer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cbServer.FormattingEnabled = true;
            this.cbServer.Location = new System.Drawing.Point(50, 122);
            this.cbServer.Name = "cbServer";
            this.cbServer.Size = new System.Drawing.Size(172, 21);
            this.cbServer.TabIndex = 4;
            // 
            // textBoxTribe
            // 
            this.textBoxTribe.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxTribe.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textBoxTribe.Location = new System.Drawing.Point(50, 96);
            this.textBoxTribe.Name = "textBoxTribe";
            this.textBoxTribe.Size = new System.Drawing.Size(172, 20);
            this.textBoxTribe.TabIndex = 3;
            // 
            // textBoxNote
            // 
            this.textBoxNote.Location = new System.Drawing.Point(50, 203);
            this.textBoxNote.Name = "textBoxNote";
            this.textBoxNote.Size = new System.Drawing.Size(172, 20);
            this.textBoxNote.TabIndex = 7;
            // 
            // parentComboBoxFather
            // 
            this.parentComboBoxFather.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.parentComboBoxFather.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parentComboBoxFather.FormattingEnabled = true;
            this.parentComboBoxFather.Location = new System.Drawing.Point(50, 176);
            this.parentComboBoxFather.Name = "parentComboBoxFather";
            this.parentComboBoxFather.PreselectedCreatureGuid = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.parentComboBoxFather.Size = new System.Drawing.Size(172, 21);
            this.parentComboBoxFather.TabIndex = 6;
            this.parentComboBoxFather.SelectedIndexChanged += new System.EventHandler(this.parentComboBox_SelectedIndexChanged);
            // 
            // parentComboBoxMother
            // 
            this.parentComboBoxMother.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.parentComboBoxMother.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parentComboBoxMother.FormattingEnabled = true;
            this.parentComboBoxMother.Location = new System.Drawing.Point(50, 149);
            this.parentComboBoxMother.Name = "parentComboBoxMother";
            this.parentComboBoxMother.PreselectedCreatureGuid = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.parentComboBoxMother.Size = new System.Drawing.Size(172, 21);
            this.parentComboBoxMother.TabIndex = 5;
            this.parentComboBoxMother.SelectedIndexChanged += new System.EventHandler(this.parentComboBox_SelectedIndexChanged);
            // 
            // textBoxOwner
            // 
            this.textBoxOwner.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxOwner.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textBoxOwner.Location = new System.Drawing.Point(50, 70);
            this.textBoxOwner.Name = "textBoxOwner";
            this.textBoxOwner.Size = new System.Drawing.Size(172, 20);
            this.textBoxOwner.TabIndex = 2;
            this.textBoxOwner.Leave += new System.EventHandler(this.textBoxOwner_Leave);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(50, 19);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(125, 20);
            this.textBoxName.TabIndex = 0;
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // nudMutationsFather
            // 
            this.nudMutationsFather.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudMutationsFather.Location = new System.Drawing.Point(162, 360);
            this.nudMutationsFather.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.nudMutationsFather.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.nudMutationsFather.Name = "nudMutationsFather";
            this.nudMutationsFather.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudMutationsFather.Size = new System.Drawing.Size(60, 20);
            this.nudMutationsFather.TabIndex = 13;
            this.nudMutationsFather.ValueChanged += new System.EventHandler(this.NudMutations_ValueChanged);
            // 
            // nudMutationsMother
            // 
            this.nudMutationsMother.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudMutationsMother.Location = new System.Drawing.Point(80, 360);
            this.nudMutationsMother.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.nudMutationsMother.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.nudMutationsMother.Name = "nudMutationsMother";
            this.nudMutationsMother.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudMutationsMother.Size = new System.Drawing.Size(60, 20);
            this.nudMutationsMother.TabIndex = 12;
            this.nudMutationsMother.ValueChanged += new System.EventHandler(this.NudMutations_ValueChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(147, 362);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(17, 13);
            this.label12.TabIndex = 34;
            this.label12.Text = "♂";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(65, 362);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(16, 13);
            this.label11.TabIndex = 33;
            this.label11.Text = "♀";
            // 
            // dhmsInputGrown
            // 
            this.dhmsInputGrown.Location = new System.Drawing.Point(86, 305);
            this.dhmsInputGrown.Name = "dhmsInputGrown";
            this.dhmsInputGrown.Size = new System.Drawing.Size(136, 26);
            this.dhmsInputGrown.TabIndex = 10;
            this.dhmsInputGrown.Timespan = System.TimeSpan.Parse("00:00:00");
            this.dhmsInputGrown.ValueChanged += new ARKBreedingStats.uiControls.dhmsInput.ValueChangedEventHandler(this.dhmsInputGrown_ValueChanged);
            // 
            // dhmsInputCooldown
            // 
            this.dhmsInputCooldown.Location = new System.Drawing.Point(86, 278);
            this.dhmsInputCooldown.Name = "dhmsInputCooldown";
            this.dhmsInputCooldown.Size = new System.Drawing.Size(136, 26);
            this.dhmsInputCooldown.TabIndex = 9;
            this.dhmsInputCooldown.Timespan = System.TimeSpan.Parse("00:00:00");
            // 
            // nudMaturation
            // 
            this.nudMaturation.DecimalPlaces = 2;
            this.nudMaturation.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nudMaturation.Location = new System.Drawing.Point(89, 334);
            this.nudMaturation.Name = "nudMaturation";
            this.nudMaturation.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudMaturation.Size = new System.Drawing.Size(76, 20);
            this.nudMaturation.TabIndex = 11;
            this.nudMaturation.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudMaturation.ValueChanged += new System.EventHandler(this.nudMaturation_ValueChanged);
            // 
            // cbNeutered
            // 
            this.cbNeutered.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbNeutered.AutoSize = true;
            this.cbNeutered.Location = new System.Drawing.Point(91, 386);
            this.cbNeutered.Name = "cbNeutered";
            this.cbNeutered.Size = new System.Drawing.Size(61, 23);
            this.cbNeutered.TabIndex = 15;
            this.cbNeutered.Text = "Neutered";
            this.cbNeutered.UseVisualStyleBackColor = true;
            // 
            // dateTimePickerAdded
            // 
            this.dateTimePickerAdded.Checked = false;
            this.dateTimePickerAdded.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerAdded.Location = new System.Drawing.Point(146, 414);
            this.dateTimePickerAdded.MinDate = new System.DateTime(2014, 12, 31, 0, 0, 0, 0);
            this.dateTimePickerAdded.Name = "dateTimePickerAdded";
            this.dateTimePickerAdded.Size = new System.Drawing.Size(76, 20);
            this.dateTimePickerAdded.TabIndex = 18;
            // 
            // buttonStatus
            // 
            this.buttonStatus.Location = new System.Drawing.Point(49, 415);
            this.buttonStatus.Name = "buttonStatus";
            this.buttonStatus.Size = new System.Drawing.Size(35, 23);
            this.buttonStatus.TabIndex = 16;
            this.buttonStatus.UseVisualStyleBackColor = true;
            this.buttonStatus.Click += new System.EventHandler(this.buttonStatus_Click);
            // 
            // buttonSex
            // 
            this.buttonSex.Location = new System.Drawing.Point(50, 386);
            this.buttonSex.Name = "buttonSex";
            this.buttonSex.Size = new System.Drawing.Size(35, 23);
            this.buttonSex.TabIndex = 14;
            this.buttonSex.Text = "?";
            this.buttonSex.UseVisualStyleBackColor = true;
            this.buttonSex.Click += new System.EventHandler(this.buttonSex_Click);
            // 
            // lbNewMutations
            // 
            this.lbNewMutations.AutoSize = true;
            this.lbNewMutations.Location = new System.Drawing.Point(186, 391);
            this.lbNewMutations.Name = "lbNewMutations";
            this.lbNewMutations.Size = new System.Drawing.Size(30, 13);
            this.lbNewMutations.TabIndex = 42;
            this.lbNewMutations.Text = "+mut";
            this.lbNewMutations.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbArkIdIngame
            // 
            this.lbArkIdIngame.AutoSize = true;
            this.lbArkIdIngame.Location = new System.Drawing.Point(5, 258);
            this.lbArkIdIngame.Name = "lbArkIdIngame";
            this.lbArkIdIngame.Size = new System.Drawing.Size(72, 13);
            this.lbArkIdIngame.TabIndex = 41;
            this.lbArkIdIngame.Text = "Ark-Id ingame";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 232);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 39;
            this.label1.Text = "Ark-Id";
            // 
            // btClearColors
            // 
            this.btClearColors.Location = new System.Drawing.Point(2, 444);
            this.btClearColors.Name = "btClearColors";
            this.btClearColors.Size = new System.Drawing.Size(45, 23);
            this.btClearColors.TabIndex = 37;
            this.btClearColors.Text = "Colors";
            this.btClearColors.UseVisualStyleBackColor = true;
            this.btClearColors.Click += new System.EventHandler(this.btClearColors_Click);
            // 
            // regionColorChooser1
            // 
            this.regionColorChooser1.Location = new System.Drawing.Point(48, 441);
            this.regionColorChooser1.Margin = new System.Windows.Forms.Padding(0);
            this.regionColorChooser1.Name = "regionColorChooser1";
            this.regionColorChooser1.Size = new System.Drawing.Size(174, 29);
            this.regionColorChooser1.TabIndex = 19;
            // 
            // lbServer
            // 
            this.lbServer.AutoSize = true;
            this.lbServer.Location = new System.Drawing.Point(6, 125);
            this.lbServer.Name = "lbServer";
            this.lbServer.Size = new System.Drawing.Size(38, 13);
            this.lbServer.TabIndex = 30;
            this.lbServer.Text = "Server";
            // 
            // lbTribe
            // 
            this.lbTribe.AutoSize = true;
            this.lbTribe.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTribe.Location = new System.Drawing.Point(6, 99);
            this.lbTribe.Name = "lbTribe";
            this.lbTribe.Size = new System.Drawing.Size(31, 13);
            this.lbTribe.TabIndex = 29;
            this.lbTribe.Text = "Tribe";
            this.lbTribe.Click += new System.EventHandler(this.lblTribe_Click);
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Location = new System.Drawing.Point(6, 420);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(37, 13);
            this.lbStatus.TabIndex = 27;
            this.lbStatus.Text = "Status";
            // 
            // lbSex
            // 
            this.lbSex.AutoSize = true;
            this.lbSex.Location = new System.Drawing.Point(6, 391);
            this.lbSex.Name = "lbSex";
            this.lbSex.Size = new System.Drawing.Size(25, 13);
            this.lbSex.TabIndex = 26;
            this.lbSex.Text = "Sex";
            // 
            // lbMutations
            // 
            this.lbMutations.AutoSize = true;
            this.lbMutations.Location = new System.Drawing.Point(6, 362);
            this.lbMutations.Name = "lbMutations";
            this.lbMutations.Size = new System.Drawing.Size(53, 13);
            this.lbMutations.TabIndex = 25;
            this.lbMutations.Text = "Mutations";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(105, 420);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Added";
            // 
            // lbMaturationPerc
            // 
            this.lbMaturationPerc.AutoSize = true;
            this.lbMaturationPerc.Location = new System.Drawing.Point(6, 336);
            this.lbMaturationPerc.Name = "lbMaturationPerc";
            this.lbMaturationPerc.Size = new System.Drawing.Size(74, 13);
            this.lbMaturationPerc.TabIndex = 22;
            this.lbMaturationPerc.Text = "Maturation [%]";
            // 
            // lbCooldown
            // 
            this.lbCooldown.AutoSize = true;
            this.lbCooldown.Location = new System.Drawing.Point(6, 285);
            this.lbCooldown.Name = "lbCooldown";
            this.lbCooldown.Size = new System.Drawing.Size(54, 13);
            this.lbCooldown.TabIndex = 20;
            this.lbCooldown.Text = "Cooldown";
            // 
            // lbGrownIn
            // 
            this.lbGrownIn.AutoSize = true;
            this.lbGrownIn.Location = new System.Drawing.Point(6, 311);
            this.lbGrownIn.Name = "lbGrownIn";
            this.lbGrownIn.Size = new System.Drawing.Size(49, 13);
            this.lbGrownIn.TabIndex = 21;
            this.lbGrownIn.Text = "Grown in";
            // 
            // btSaveChanges
            // 
            this.btSaveChanges.Location = new System.Drawing.Point(89, 473);
            this.btSaveChanges.Name = "btSaveChanges";
            this.btSaveChanges.Size = new System.Drawing.Size(60, 37);
            this.btSaveChanges.TabIndex = 20;
            this.btSaveChanges.Text = "Save Changes";
            this.btSaveChanges.UseVisualStyleBackColor = true;
            this.btSaveChanges.Visible = false;
            this.btSaveChanges.Click += new System.EventHandler(this.buttonSaveChanges_Click);
            // 
            // lbNote
            // 
            this.lbNote.AutoSize = true;
            this.lbNote.Location = new System.Drawing.Point(6, 206);
            this.lbNote.Name = "lbNote";
            this.lbNote.Size = new System.Drawing.Size(30, 13);
            this.lbNote.TabIndex = 19;
            this.lbNote.Text = "Note";
            // 
            // lbFather
            // 
            this.lbFather.AutoSize = true;
            this.lbFather.Location = new System.Drawing.Point(6, 179);
            this.lbFather.Name = "lbFather";
            this.lbFather.Size = new System.Drawing.Size(37, 13);
            this.lbFather.TabIndex = 18;
            this.lbFather.Text = "Father";
            // 
            // lbMother
            // 
            this.lbMother.AutoSize = true;
            this.lbMother.Location = new System.Drawing.Point(6, 152);
            this.lbMother.Name = "lbMother";
            this.lbMother.Size = new System.Drawing.Size(40, 13);
            this.lbMother.TabIndex = 17;
            this.lbMother.Text = "Mother";
            // 
            // lbOwner
            // 
            this.lbOwner.AutoSize = true;
            this.lbOwner.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbOwner.Location = new System.Drawing.Point(6, 73);
            this.lbOwner.Name = "lbOwner";
            this.lbOwner.Size = new System.Drawing.Size(38, 13);
            this.lbOwner.TabIndex = 16;
            this.lbOwner.Text = "Owner";
            this.lbOwner.Click += new System.EventHandler(this.lblOwner_Click);
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.Location = new System.Drawing.Point(6, 22);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(35, 13);
            this.lbName.TabIndex = 15;
            this.lbName.Text = "Name";
            this.lbName.Click += new System.EventHandler(this.lblName_Click);
            // 
            // btAdd2Library
            // 
            this.btAdd2Library.Location = new System.Drawing.Point(89, 473);
            this.btAdd2Library.Name = "btAdd2Library";
            this.btAdd2Library.Size = new System.Drawing.Size(134, 37);
            this.btAdd2Library.TabIndex = 21;
            this.btAdd2Library.Text = "Add new to Library";
            this.btAdd2Library.UseVisualStyleBackColor = true;
            this.btAdd2Library.Click += new System.EventHandler(this.buttonAdd2Library_Click);
            // 
            // btNamingPattern2
            // 
            this.btNamingPattern2.Location = new System.Drawing.Point(50, 44);
            this.btNamingPattern2.Name = "btNamingPattern2";
            this.btNamingPattern2.Size = new System.Drawing.Size(30, 20);
            this.btNamingPattern2.TabIndex = 44;
            this.btNamingPattern2.TabStop = false;
            this.btNamingPattern2.Text = "G2";
            this.btNamingPattern2.UseVisualStyleBackColor = true;
            // 
            // btNamingPattern3
            // 
            this.btNamingPattern3.Location = new System.Drawing.Point(85, 44);
            this.btNamingPattern3.Name = "btNamingPattern3";
            this.btNamingPattern3.Size = new System.Drawing.Size(30, 20);
            this.btNamingPattern3.TabIndex = 45;
            this.btNamingPattern3.TabStop = false;
            this.btNamingPattern3.Text = "G3";
            this.btNamingPattern3.UseVisualStyleBackColor = true;
            // 
            // btNamingPattern4
            // 
            this.btNamingPattern4.Location = new System.Drawing.Point(120, 44);
            this.btNamingPattern4.Name = "btNamingPattern4";
            this.btNamingPattern4.Size = new System.Drawing.Size(30, 20);
            this.btNamingPattern4.TabIndex = 46;
            this.btNamingPattern4.TabStop = false;
            this.btNamingPattern4.Text = "G4";
            this.btNamingPattern4.UseVisualStyleBackColor = true;
            // 
            // btNamingPattern5
            // 
            this.btNamingPattern5.Location = new System.Drawing.Point(155, 44);
            this.btNamingPattern5.Name = "btNamingPattern5";
            this.btNamingPattern5.Size = new System.Drawing.Size(30, 20);
            this.btNamingPattern5.TabIndex = 47;
            this.btNamingPattern5.TabStop = false;
            this.btNamingPattern5.Text = "G5";
            this.btNamingPattern5.UseVisualStyleBackColor = true;
            // 
            // btNamingPattern6
            // 
            this.btNamingPattern6.Location = new System.Drawing.Point(190, 44);
            this.btNamingPattern6.Name = "btNamingPattern6";
            this.btNamingPattern6.Size = new System.Drawing.Size(33, 20);
            this.btNamingPattern6.TabIndex = 48;
            this.btNamingPattern6.TabStop = false;
            this.btNamingPattern6.Text = "G6";
            this.btNamingPattern6.UseVisualStyleBackColor = true;
            // 
            // CreatureInfoInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbCreatureInfo);
            this.Name = "CreatureInfoInput";
            this.Size = new System.Drawing.Size(229, 518);
            this.gbCreatureInfo.ResumeLayout(false);
            this.gbCreatureInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMutationsFather)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMutationsMother)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaturation)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbCreatureInfo;
        private System.Windows.Forms.Label lbFather;
        private System.Windows.Forms.Label lbMother;
        private System.Windows.Forms.Button buttonSex;
        private System.Windows.Forms.TextBox textBoxOwner;
        private System.Windows.Forms.Label lbOwner;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button btAdd2Library;
        private ParentComboBox parentComboBoxMother;
        private ParentComboBox parentComboBoxFather;
        private System.Windows.Forms.Button buttonStatus;
        private System.Windows.Forms.Label lbNote;
        private System.Windows.Forms.TextBox textBoxNote;
        private System.Windows.Forms.Button btSaveChanges;
        private System.Windows.Forms.Label lbCooldown;
        private System.Windows.Forms.Label lbGrownIn;
        private System.Windows.Forms.DateTimePicker dateTimePickerAdded;
        private System.Windows.Forms.CheckBox cbNeutered;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbMaturationPerc;
        private uiControls.Nud nudMaturation;
        private uiControls.dhmsInput dhmsInputGrown;
        private uiControls.dhmsInput dhmsInputCooldown;
        private uiControls.Nud nudMutationsMother;
        private System.Windows.Forms.Label lbMutations;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Label lbSex;
        private System.Windows.Forms.TextBox textBoxTribe;
        private System.Windows.Forms.Label lbTribe;
        private System.Windows.Forms.Button btnGenerateUniqueName;
        private System.Windows.Forms.ComboBox cbServer;
        private System.Windows.Forms.Label lbServer;
        private uiControls.Nud nudMutationsFather;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private uiControls.RegionColorChooser regionColorChooser1;
        private System.Windows.Forms.Button btClearColors;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbARKID;
        private System.Windows.Forms.Label lbArkIdIngame;
        private System.Windows.Forms.TextBox tbArkIdIngame;
        private System.Windows.Forms.Label lbNewMutations;
        private System.Windows.Forms.Button btNamingPatternEditor;
        private System.Windows.Forms.Button btNamingPattern6;
        private System.Windows.Forms.Button btNamingPattern5;
        private System.Windows.Forms.Button btNamingPattern4;
        private System.Windows.Forms.Button btNamingPattern3;
        private System.Windows.Forms.Button btNamingPattern2;
    }
}
