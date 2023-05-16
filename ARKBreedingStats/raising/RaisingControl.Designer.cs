namespace ARKBreedingStats.raising
{
    partial class RaisingControl
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
            this.components = new System.ComponentModel.Container();
            this.labelRaisingInfos = new System.Windows.Forms.Label();
            this.listViewRaisingTimes = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDuration = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTotalTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderUntil = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelAmountFoodAdult = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.labelAmountFoodBaby = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelTimeLeftGrowing = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelTimeLeftBaby = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.contextMenuStripBabyList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractValuesOfHatchedbornBabyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteTimerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllExpiredTimersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btStartPauseTimer = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageMaturationProgress = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageEditTimer = new System.Windows.Forms.TabPage();
            this.bSaveTimerEdit = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.lEditTimerName = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dateTimePickerEditTimerFinish = new System.Windows.Forms.DateTimePicker();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.cbSubtractOffsetToAllTimers = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btAdjustAllTimers = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.LbFoodInfoGeneral = new System.Windows.Forms.Label();
            this.CbGrowingFood = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.listViewBabies = new System.Windows.Forms.ListView();
            this.columnHeaderBabyName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSpecies = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderIncubation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderBabyTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderGrowingTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.parentStats1 = new ARKBreedingStats.raising.ParentStats();
            this.nudMaturationProgress = new ARKBreedingStats.uiControls.Nud();
            this.dhmsInputTimerEditTimer = new ARKBreedingStats.uiControls.dhmsInput();
            this.dhmsInputOffsetAllTimers = new ARKBreedingStats.uiControls.dhmsInput();
            this.contextMenuStripBabyList.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageMaturationProgress.SuspendLayout();
            this.tabPageEditTimer.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaturationProgress)).BeginInit();
            this.SuspendLayout();
            // 
            // labelRaisingInfos
            // 
            this.labelRaisingInfos.AutoSize = true;
            this.labelRaisingInfos.Location = new System.Drawing.Point(6, 16);
            this.labelRaisingInfos.Name = "labelRaisingInfos";
            this.labelRaisingInfos.Size = new System.Drawing.Size(30, 13);
            this.labelRaisingInfos.TabIndex = 2;
            this.labelRaisingInfos.Text = "Infos";
            // 
            // listViewRaisingTimes
            // 
            this.listViewRaisingTimes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderDuration,
            this.columnHeaderTotalTime,
            this.columnHeaderUntil});
            this.listViewRaisingTimes.HideSelection = false;
            this.listViewRaisingTimes.Location = new System.Drawing.Point(6, 149);
            this.listViewRaisingTimes.Name = "listViewRaisingTimes";
            this.listViewRaisingTimes.Size = new System.Drawing.Size(346, 116);
            this.listViewRaisingTimes.TabIndex = 3;
            this.listViewRaisingTimes.UseCompatibleStateImageBehavior = false;
            this.listViewRaisingTimes.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "";
            this.columnHeaderName.Width = 72;
            // 
            // columnHeaderDuration
            // 
            this.columnHeaderDuration.Text = "Duration";
            this.columnHeaderDuration.Width = 67;
            // 
            // columnHeaderTotalTime
            // 
            this.columnHeaderTotalTime.Text = "Total Time";
            this.columnHeaderTotalTime.Width = 67;
            // 
            // columnHeaderUntil
            // 
            this.columnHeaderUntil.Text = "Until";
            this.columnHeaderUntil.Width = 132;
            // 
            // labelAmountFoodAdult
            // 
            this.labelAmountFoodAdult.AutoSize = true;
            this.labelAmountFoodAdult.Location = new System.Drawing.Point(133, 111);
            this.labelAmountFoodAdult.Name = "labelAmountFoodAdult";
            this.labelAmountFoodAdult.Size = new System.Drawing.Size(13, 13);
            this.labelAmountFoodAdult.TabIndex = 14;
            this.labelAmountFoodAdult.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 111);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(114, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Food for adult needed:";
            // 
            // labelAmountFoodBaby
            // 
            this.labelAmountFoodBaby.AutoSize = true;
            this.labelAmountFoodBaby.Location = new System.Drawing.Point(133, 93);
            this.labelAmountFoodBaby.Name = "labelAmountFoodBaby";
            this.labelAmountFoodBaby.Size = new System.Drawing.Size(13, 13);
            this.labelAmountFoodBaby.TabIndex = 6;
            this.labelAmountFoodBaby.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Food for baby needed:";
            // 
            // labelTimeLeftGrowing
            // 
            this.labelTimeLeftGrowing.AutoSize = true;
            this.labelTimeLeftGrowing.Location = new System.Drawing.Point(133, 75);
            this.labelTimeLeftGrowing.Name = "labelTimeLeftGrowing";
            this.labelTimeLeftGrowing.Size = new System.Drawing.Size(13, 13);
            this.labelTimeLeftGrowing.TabIndex = 12;
            this.labelTimeLeftGrowing.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Time left until grown:";
            // 
            // labelTimeLeftBaby
            // 
            this.labelTimeLeftBaby.AutoSize = true;
            this.labelTimeLeftBaby.Location = new System.Drawing.Point(133, 57);
            this.labelTimeLeftBaby.Name = "labelTimeLeftBaby";
            this.labelTimeLeftBaby.Size = new System.Drawing.Size(13, 13);
            this.labelTimeLeftBaby.TabIndex = 10;
            this.labelTimeLeftBaby.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Time left as Baby:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Maturation-progress:";
            // 
            // contextMenuStripBabyList
            // 
            this.contextMenuStripBabyList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractValuesOfHatchedbornBabyToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteTimerToolStripMenuItem,
            this.removeAllExpiredTimersToolStripMenuItem});
            this.contextMenuStripBabyList.Name = "contextMenuStripBabyList";
            this.contextMenuStripBabyList.Size = new System.Drawing.Size(266, 76);
            // 
            // extractValuesOfHatchedbornBabyToolStripMenuItem
            // 
            this.extractValuesOfHatchedbornBabyToolStripMenuItem.Name = "extractValuesOfHatchedbornBabyToolStripMenuItem";
            this.extractValuesOfHatchedbornBabyToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.extractValuesOfHatchedbornBabyToolStripMenuItem.Text = "Extract values of hatched/born baby";
            this.extractValuesOfHatchedbornBabyToolStripMenuItem.Click += new System.EventHandler(this.extractValuesOfHatchedbornBabyToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(262, 6);
            // 
            // deleteTimerToolStripMenuItem
            // 
            this.deleteTimerToolStripMenuItem.Name = "deleteTimerToolStripMenuItem";
            this.deleteTimerToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.deleteTimerToolStripMenuItem.Text = "Remove selected Timers...";
            this.deleteTimerToolStripMenuItem.Click += new System.EventHandler(this.deleteTimerToolStripMenuItem_Click);
            // 
            // removeAllExpiredTimersToolStripMenuItem
            // 
            this.removeAllExpiredTimersToolStripMenuItem.Name = "removeAllExpiredTimersToolStripMenuItem";
            this.removeAllExpiredTimersToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.removeAllExpiredTimersToolStripMenuItem.Text = "Remove all expired Timers...";
            this.removeAllExpiredTimersToolStripMenuItem.Click += new System.EventHandler(this.removeAllExpiredTimersToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.AutoScrollMinSize = new System.Drawing.Size(0, 700);
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 364F));
            this.tableLayoutPanel1.Controls.Add(this.parentStats1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btStartPauseTimer, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 277F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 165F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(364, 853);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // btStartPauseTimer
            // 
            this.btStartPauseTimer.Dock = System.Windows.Forms.DockStyle.Top;
            this.btStartPauseTimer.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btStartPauseTimer.Location = new System.Drawing.Point(3, 280);
            this.btStartPauseTimer.Name = "btStartPauseTimer";
            this.btStartPauseTimer.Size = new System.Drawing.Size(358, 34);
            this.btStartPauseTimer.TabIndex = 4;
            this.btStartPauseTimer.Text = "⏯";
            this.btStartPauseTimer.UseVisualStyleBackColor = true;
            this.btStartPauseTimer.Click += new System.EventHandler(this.btStartPauseTimer_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageMaturationProgress);
            this.tabControl1.Controls.Add(this.tabPageEditTimer);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 320);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(358, 159);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPageMaturationProgress
            // 
            this.tabPageMaturationProgress.Controls.Add(this.label1);
            this.tabPageMaturationProgress.Controls.Add(this.nudMaturationProgress);
            this.tabPageMaturationProgress.Controls.Add(this.labelAmountFoodAdult);
            this.tabPageMaturationProgress.Controls.Add(this.label8);
            this.tabPageMaturationProgress.Controls.Add(this.labelAmountFoodBaby);
            this.tabPageMaturationProgress.Controls.Add(this.label5);
            this.tabPageMaturationProgress.Controls.Add(this.labelTimeLeftGrowing);
            this.tabPageMaturationProgress.Controls.Add(this.label6);
            this.tabPageMaturationProgress.Controls.Add(this.label3);
            this.tabPageMaturationProgress.Controls.Add(this.labelTimeLeftBaby);
            this.tabPageMaturationProgress.Controls.Add(this.label4);
            this.tabPageMaturationProgress.Location = new System.Drawing.Point(4, 22);
            this.tabPageMaturationProgress.Name = "tabPageMaturationProgress";
            this.tabPageMaturationProgress.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMaturationProgress.Size = new System.Drawing.Size(350, 133);
            this.tabPageMaturationProgress.TabIndex = 0;
            this.tabPageMaturationProgress.Text = "Maturation Progress";
            this.tabPageMaturationProgress.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(219, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "%";
            // 
            // tabPageEditTimer
            // 
            this.tabPageEditTimer.Controls.Add(this.bSaveTimerEdit);
            this.tabPageEditTimer.Controls.Add(this.label9);
            this.tabPageEditTimer.Controls.Add(this.lEditTimerName);
            this.tabPageEditTimer.Controls.Add(this.label7);
            this.tabPageEditTimer.Controls.Add(this.dateTimePickerEditTimerFinish);
            this.tabPageEditTimer.Controls.Add(this.dhmsInputTimerEditTimer);
            this.tabPageEditTimer.Location = new System.Drawing.Point(4, 22);
            this.tabPageEditTimer.Name = "tabPageEditTimer";
            this.tabPageEditTimer.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEditTimer.Size = new System.Drawing.Size(350, 133);
            this.tabPageEditTimer.TabIndex = 1;
            this.tabPageEditTimer.Text = "Edit Timer";
            this.tabPageEditTimer.UseVisualStyleBackColor = true;
            // 
            // bSaveTimerEdit
            // 
            this.bSaveTimerEdit.Location = new System.Drawing.Point(6, 104);
            this.bSaveTimerEdit.Name = "bSaveTimerEdit";
            this.bSaveTimerEdit.Size = new System.Drawing.Size(158, 23);
            this.bSaveTimerEdit.TabIndex = 18;
            this.bSaveTimerEdit.Text = "Save Changes";
            this.bSaveTimerEdit.UseVisualStyleBackColor = true;
            this.bSaveTimerEdit.Click += new System.EventHandler(this.bSaveTimerEdit_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 52);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(58, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Finished at";
            // 
            // lEditTimerName
            // 
            this.lEditTimerName.AutoSize = true;
            this.lEditTimerName.Location = new System.Drawing.Point(6, 3);
            this.lEditTimerName.Name = "lEditTimerName";
            this.lEditTimerName.Size = new System.Drawing.Size(79, 13);
            this.lEditTimerName.TabIndex = 16;
            this.lEditTimerName.Text = "EditTimerName";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 78);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Duration";
            // 
            // dateTimePickerEditTimerFinish
            // 
            this.dateTimePickerEditTimerFinish.CustomFormat = "";
            this.dateTimePickerEditTimerFinish.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerEditTimerFinish.Location = new System.Drawing.Point(93, 46);
            this.dateTimePickerEditTimerFinish.Name = "dateTimePickerEditTimerFinish";
            this.dateTimePickerEditTimerFinish.Size = new System.Drawing.Size(156, 20);
            this.dateTimePickerEditTimerFinish.TabIndex = 4;
            this.dateTimePickerEditTimerFinish.ValueChanged += new System.EventHandler(this.dateTimePickerEditTimerFinish_ValueChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cbSubtractOffsetToAllTimers);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.btAdjustAllTimers);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.dhmsInputOffsetAllTimers);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(350, 133);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Edit all Timers";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // cbSubtractOffsetToAllTimers
            // 
            this.cbSubtractOffsetToAllTimers.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbSubtractOffsetToAllTimers.AutoSize = true;
            this.cbSubtractOffsetToAllTimers.Location = new System.Drawing.Point(61, 73);
            this.cbSubtractOffsetToAllTimers.Name = "cbSubtractOffsetToAllTimers";
            this.cbSubtractOffsetToAllTimers.Size = new System.Drawing.Size(23, 23);
            this.cbSubtractOffsetToAllTimers.TabIndex = 25;
            this.cbSubtractOffsetToAllTimers.Text = "+";
            this.cbSubtractOffsetToAllTimers.UseVisualStyleBackColor = true;
            this.cbSubtractOffsetToAllTimers.CheckedChanged += new System.EventHandler(this.cbAddOffsetToAllTimers_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(318, 52);
            this.label2.TabIndex = 24;
            this.label2.Text = "Change all timers by the following offset.\r\nThis can be used to synchronize the t" +
    "imers with the server if there were lags.";
            // 
            // btAdjustAllTimers
            // 
            this.btAdjustAllTimers.Location = new System.Drawing.Point(6, 104);
            this.btAdjustAllTimers.Name = "btAdjustAllTimers";
            this.btAdjustAllTimers.Size = new System.Drawing.Size(158, 23);
            this.btAdjustAllTimers.TabIndex = 23;
            this.btAdjustAllTimers.Text = "Adjust ALL timers";
            this.btAdjustAllTimers.UseVisualStyleBackColor = true;
            this.btAdjustAllTimers.Click += new System.EventHandler(this.btAdjustAllTimers_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 78);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Offest";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.LbFoodInfoGeneral);
            this.groupBox2.Controls.Add(this.CbGrowingFood);
            this.groupBox2.Controls.Add(this.labelRaisingInfos);
            this.groupBox2.Controls.Add(this.listViewRaisingTimes);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(358, 271);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General Infos";
            // 
            // LbFoodInfoGeneral
            // 
            this.LbFoodInfoGeneral.AutoSize = true;
            this.LbFoodInfoGeneral.Location = new System.Drawing.Point(6, 47);
            this.LbFoodInfoGeneral.Name = "LbFoodInfoGeneral";
            this.LbFoodInfoGeneral.Size = new System.Drawing.Size(49, 13);
            this.LbFoodInfoGeneral.TabIndex = 5;
            this.LbFoodInfoGeneral.Text = "FoodInfo";
            // 
            // CbGrowingFood
            // 
            this.CbGrowingFood.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CbGrowingFood.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CbGrowingFood.Location = new System.Drawing.Point(6, 122);
            this.CbGrowingFood.Name = "CbGrowingFood";
            this.CbGrowingFood.Size = new System.Drawing.Size(346, 21);
            this.CbGrowingFood.TabIndex = 4;
            this.CbGrowingFood.SelectedIndexChanged += new System.EventHandler(this.CbGrowingFood_SelectedIndexChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 370F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.listViewBabies, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(918, 859);
            this.tableLayoutPanel2.TabIndex = 8;
            // 
            // listViewBabies
            // 
            this.listViewBabies.CheckBoxes = true;
            this.listViewBabies.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderBabyName,
            this.columnHeaderSpecies,
            this.columnHeaderIncubation,
            this.columnHeaderBabyTime,
            this.columnHeaderGrowingTime,
            this.columnHeaderStatus});
            this.listViewBabies.ContextMenuStrip = this.contextMenuStripBabyList;
            this.listViewBabies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewBabies.FullRowSelect = true;
            this.listViewBabies.GridLines = true;
            this.listViewBabies.HideSelection = false;
            this.listViewBabies.Location = new System.Drawing.Point(373, 3);
            this.listViewBabies.Name = "listViewBabies";
            this.listViewBabies.Size = new System.Drawing.Size(542, 853);
            this.listViewBabies.TabIndex = 6;
            this.listViewBabies.UseCompatibleStateImageBehavior = false;
            this.listViewBabies.View = System.Windows.Forms.View.Details;
            this.listViewBabies.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewBabies_ColumnClick);
            this.listViewBabies.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewBabies_ItemChecked);
            this.listViewBabies.SelectedIndexChanged += new System.EventHandler(this.listViewBabies_SelectedIndexChanged);
            // 
            // columnHeaderBabyName
            // 
            this.columnHeaderBabyName.Text = "Name";
            this.columnHeaderBabyName.Width = 74;
            // 
            // columnHeaderSpecies
            // 
            this.columnHeaderSpecies.Text = "Species";
            this.columnHeaderSpecies.Width = 89;
            // 
            // columnHeaderIncubation
            // 
            this.columnHeaderIncubation.Text = "Incubation/Gestation";
            this.columnHeaderIncubation.Width = 109;
            // 
            // columnHeaderBabyTime
            // 
            this.columnHeaderBabyTime.Text = "Baby Time left";
            this.columnHeaderBabyTime.Width = 99;
            // 
            // columnHeaderGrowingTime
            // 
            this.columnHeaderGrowingTime.Text = "Growing Time left";
            this.columnHeaderGrowingTime.Width = 108;
            // 
            // columnHeaderStatus
            // 
            this.columnHeaderStatus.Text = "Status";
            // 
            // parentStats1
            // 
            this.parentStats1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parentStats1.Location = new System.Drawing.Point(3, 485);
            this.parentStats1.Name = "parentStats1";
            this.parentStats1.Size = new System.Drawing.Size(358, 365);
            this.parentStats1.TabIndex = 7;
            // 
            // nudMaturationProgress
            // 
            this.nudMaturationProgress.DecimalPlaces = 2;
            this.nudMaturationProgress.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudMaturationProgress.Location = new System.Drawing.Point(136, 6);
            this.nudMaturationProgress.Name = "nudMaturationProgress";
            this.nudMaturationProgress.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudMaturationProgress.Size = new System.Drawing.Size(77, 20);
            this.nudMaturationProgress.TabIndex = 16;
            this.nudMaturationProgress.ValueChanged += new System.EventHandler(this.nudMaturationProgress_ValueChanged);
            // 
            // dhmsInputTimerEditTimer
            // 
            this.dhmsInputTimerEditTimer.Location = new System.Drawing.Point(90, 72);
            this.dhmsInputTimerEditTimer.Name = "dhmsInputTimerEditTimer";
            this.dhmsInputTimerEditTimer.Size = new System.Drawing.Size(136, 26);
            this.dhmsInputTimerEditTimer.TabIndex = 15;
            this.dhmsInputTimerEditTimer.Timespan = System.TimeSpan.Parse("00:00:00");
            this.dhmsInputTimerEditTimer.ValueChanged += new ARKBreedingStats.uiControls.dhmsInput.ValueChangedEventHandler(this.dhmsInputTimerEditTimer_ValueChanged);
            this.dhmsInputTimerEditTimer.TextChanged += new System.EventHandler(this.dhmsInputTimerEditTimer_TextChanged);
            // 
            // dhmsInputOffsetAllTimers
            // 
            this.dhmsInputOffsetAllTimers.Location = new System.Drawing.Point(90, 72);
            this.dhmsInputOffsetAllTimers.Name = "dhmsInputOffsetAllTimers";
            this.dhmsInputOffsetAllTimers.Size = new System.Drawing.Size(136, 26);
            this.dhmsInputOffsetAllTimers.TabIndex = 21;
            this.dhmsInputOffsetAllTimers.Timespan = System.TimeSpan.Parse("00:00:00");
            // 
            // RaisingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "RaisingControl";
            this.Size = new System.Drawing.Size(918, 859);
            this.contextMenuStripBabyList.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageMaturationProgress.ResumeLayout(false);
            this.tabPageMaturationProgress.PerformLayout();
            this.tabPageEditTimer.ResumeLayout(false);
            this.tabPageEditTimer.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudMaturationProgress)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelRaisingInfos;
        private System.Windows.Forms.ListView listViewRaisingTimes;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderDuration;
        private System.Windows.Forms.ColumnHeader columnHeaderTotalTime;
        private System.Windows.Forms.ColumnHeader columnHeaderUntil;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelTimeLeftGrowing;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelTimeLeftBaby;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelAmountFoodBaby;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripBabyList;
        private System.Windows.Forms.ToolStripMenuItem extractValuesOfHatchedbornBabyToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelAmountFoodAdult;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem deleteTimerToolStripMenuItem;
        private raising.ParentStats parentStats1;
        private System.Windows.Forms.ToolStripMenuItem removeAllExpiredTimersToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageMaturationProgress;
        private System.Windows.Forms.TabPage tabPageEditTimer;
        private System.Windows.Forms.DateTimePicker dateTimePickerEditTimerFinish;
        private System.Windows.Forms.Button bSaveTimerEdit;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lEditTimerName;
        private uiControls.dhmsInput dhmsInputTimerEditTimer;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private uiControls.Nud nudMaturationProgress;
        private System.Windows.Forms.Button btStartPauseTimer;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btAdjustAllTimers;
        private System.Windows.Forms.Label label10;
        private uiControls.dhmsInput dhmsInputOffsetAllTimers;
        private System.Windows.Forms.CheckBox cbSubtractOffsetToAllTimers;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ListView listViewBabies;
        private System.Windows.Forms.ColumnHeader columnHeaderBabyName;
        private System.Windows.Forms.ColumnHeader columnHeaderSpecies;
        private System.Windows.Forms.ColumnHeader columnHeaderIncubation;
        private System.Windows.Forms.ColumnHeader columnHeaderBabyTime;
        private System.Windows.Forms.ColumnHeader columnHeaderGrowingTime;
        private System.Windows.Forms.ColumnHeader columnHeaderStatus;
        private System.Windows.Forms.ComboBox CbGrowingFood;
        private System.Windows.Forms.Label LbFoodInfoGeneral;
    }
}
