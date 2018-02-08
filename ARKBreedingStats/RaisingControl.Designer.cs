namespace ARKBreedingStats
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
            this.nudCurrentWeight = new System.Windows.Forms.NumericUpDown();
            this.labelAmountFoodAdult = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.labelAmountFoodBaby = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelTimeLeftGrowing = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelTimeLeftBaby = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelMaturationProgress = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nudTotalWeight = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listViewBabies = new System.Windows.Forms.ListView();
            this.columnHeaderBabyName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSpecies = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderIncubation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderBabyTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderGrowingTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStripBabyList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractValuesOfHatchedbornBabyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteTimerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllExpiredTimersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageMaturationProgress = new System.Windows.Forms.TabPage();
            this.tabPageEditTimer = new System.Windows.Forms.TabPage();
            this.bSaveTimerEdit = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.lEditTimerName = new System.Windows.Forms.Label();
            this.dhmsInputTimerEditTimer = new ARKBreedingStats.uiControls.dhmsInput();
            this.label7 = new System.Windows.Forms.Label();
            this.dateTimePickerEditTimerFinish = new System.Windows.Forms.DateTimePicker();
            this.parentStats1 = new ARKBreedingStats.raising.ParentStats();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudCurrentWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTotalWeight)).BeginInit();
            this.contextMenuStripBabyList.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageMaturationProgress.SuspendLayout();
            this.tabPageEditTimer.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.listViewRaisingTimes.Location = new System.Drawing.Point(6, 122);
            this.listViewRaisingTimes.Name = "listViewRaisingTimes";
            this.listViewRaisingTimes.Size = new System.Drawing.Size(317, 92);
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
            this.columnHeaderUntil.Width = 107;
            // 
            // nudCurrentWeight
            // 
            this.nudCurrentWeight.DecimalPlaces = 2;
            this.nudCurrentWeight.Location = new System.Drawing.Point(90, 4);
            this.nudCurrentWeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudCurrentWeight.Name = "nudCurrentWeight";
            this.nudCurrentWeight.Size = new System.Drawing.Size(73, 20);
            this.nudCurrentWeight.TabIndex = 4;
            this.nudCurrentWeight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudCurrentWeight.ValueChanged += new System.EventHandler(this.nudCurrentWeight_ValueChanged);
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
            // labelMaturationProgress
            // 
            this.labelMaturationProgress.AutoSize = true;
            this.labelMaturationProgress.Location = new System.Drawing.Point(133, 39);
            this.labelMaturationProgress.Name = "labelMaturationProgress";
            this.labelMaturationProgress.Size = new System.Drawing.Size(21, 13);
            this.labelMaturationProgress.TabIndex = 8;
            this.labelMaturationProgress.Text = "0%";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Maturation-progress:";
            // 
            // nudTotalWeight
            // 
            this.nudTotalWeight.DecimalPlaces = 1;
            this.nudTotalWeight.Location = new System.Drawing.Point(187, 4);
            this.nudTotalWeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudTotalWeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudTotalWeight.Name = "nudTotalWeight";
            this.nudTotalWeight.Size = new System.Drawing.Size(73, 20);
            this.nudTotalWeight.TabIndex = 6;
            this.nudTotalWeight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudTotalWeight.ValueChanged += new System.EventHandler(this.nudTotalWeight_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(169, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "/";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current Weight";
            // 
            // listViewBabies
            // 
            this.listViewBabies.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderBabyName,
            this.columnHeaderSpecies,
            this.columnHeaderIncubation,
            this.columnHeaderBabyTime,
            this.columnHeaderGrowingTime});
            this.listViewBabies.ContextMenuStrip = this.contextMenuStripBabyList;
            this.listViewBabies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewBabies.FullRowSelect = true;
            this.listViewBabies.GridLines = true;
            this.listViewBabies.HideSelection = false;
            this.listViewBabies.Location = new System.Drawing.Point(347, 3);
            this.listViewBabies.Name = "listViewBabies";
            this.listViewBabies.Size = new System.Drawing.Size(568, 543);
            this.listViewBabies.TabIndex = 6;
            this.listViewBabies.UseCompatibleStateImageBehavior = false;
            this.listViewBabies.View = System.Windows.Forms.View.Details;
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
            // contextMenuStripBabyList
            // 
            this.contextMenuStripBabyList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractValuesOfHatchedbornBabyToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteTimerToolStripMenuItem,
            this.removeAllExpiredTimersToolStripMenuItem});
            this.contextMenuStripBabyList.Name = "contextMenuStripBabyList";
            this.contextMenuStripBabyList.Size = new System.Drawing.Size(265, 76);
            // 
            // extractValuesOfHatchedbornBabyToolStripMenuItem
            // 
            this.extractValuesOfHatchedbornBabyToolStripMenuItem.Name = "extractValuesOfHatchedbornBabyToolStripMenuItem";
            this.extractValuesOfHatchedbornBabyToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.extractValuesOfHatchedbornBabyToolStripMenuItem.Text = "Extract values of hatched/born baby";
            this.extractValuesOfHatchedbornBabyToolStripMenuItem.Click += new System.EventHandler(this.extractValuesOfHatchedbornBabyToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(261, 6);
            // 
            // deleteTimerToolStripMenuItem
            // 
            this.deleteTimerToolStripMenuItem.Name = "deleteTimerToolStripMenuItem";
            this.deleteTimerToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.deleteTimerToolStripMenuItem.Text = "Remove selected Timers...";
            this.deleteTimerToolStripMenuItem.Click += new System.EventHandler(this.deleteTimerToolStripMenuItem_Click);
            // 
            // removeAllExpiredTimersToolStripMenuItem
            // 
            this.removeAllExpiredTimersToolStripMenuItem.Name = "removeAllExpiredTimersToolStripMenuItem";
            this.removeAllExpiredTimersToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.removeAllExpiredTimersToolStripMenuItem.Text = "Remove all expired Timers...";
            this.removeAllExpiredTimersToolStripMenuItem.Click += new System.EventHandler(this.removeAllExpiredTimersToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 344F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.listViewBabies, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(918, 549);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Controls.Add(this.parentStats1);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(338, 543);
            this.panel1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageMaturationProgress);
            this.tabControl1.Controls.Add(this.tabPageEditTimer);
            this.tabControl1.Location = new System.Drawing.Point(6, 229);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(323, 166);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPageMaturationProgress
            // 
            this.tabPageMaturationProgress.Controls.Add(this.labelAmountFoodAdult);
            this.tabPageMaturationProgress.Controls.Add(this.label8);
            this.tabPageMaturationProgress.Controls.Add(this.label1);
            this.tabPageMaturationProgress.Controls.Add(this.labelAmountFoodBaby);
            this.tabPageMaturationProgress.Controls.Add(this.nudCurrentWeight);
            this.tabPageMaturationProgress.Controls.Add(this.label5);
            this.tabPageMaturationProgress.Controls.Add(this.label2);
            this.tabPageMaturationProgress.Controls.Add(this.labelTimeLeftGrowing);
            this.tabPageMaturationProgress.Controls.Add(this.nudTotalWeight);
            this.tabPageMaturationProgress.Controls.Add(this.label6);
            this.tabPageMaturationProgress.Controls.Add(this.label3);
            this.tabPageMaturationProgress.Controls.Add(this.labelTimeLeftBaby);
            this.tabPageMaturationProgress.Controls.Add(this.labelMaturationProgress);
            this.tabPageMaturationProgress.Controls.Add(this.label4);
            this.tabPageMaturationProgress.Location = new System.Drawing.Point(4, 22);
            this.tabPageMaturationProgress.Name = "tabPageMaturationProgress";
            this.tabPageMaturationProgress.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMaturationProgress.Size = new System.Drawing.Size(315, 140);
            this.tabPageMaturationProgress.TabIndex = 0;
            this.tabPageMaturationProgress.Text = "Maturation Progress";
            this.tabPageMaturationProgress.UseVisualStyleBackColor = true;
            // 
            // tabPageEditTimer
            // 
            this.tabPageEditTimer.Controls.Add(this.bSaveTimerEdit);
            this.tabPageEditTimer.Controls.Add(this.label9);
            this.tabPageEditTimer.Controls.Add(this.lEditTimerName);
            this.tabPageEditTimer.Controls.Add(this.dhmsInputTimerEditTimer);
            this.tabPageEditTimer.Controls.Add(this.label7);
            this.tabPageEditTimer.Controls.Add(this.dateTimePickerEditTimerFinish);
            this.tabPageEditTimer.Location = new System.Drawing.Point(4, 22);
            this.tabPageEditTimer.Name = "tabPageEditTimer";
            this.tabPageEditTimer.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEditTimer.Size = new System.Drawing.Size(315, 140);
            this.tabPageEditTimer.TabIndex = 1;
            this.tabPageEditTimer.Text = "Edit Timer";
            this.tabPageEditTimer.UseVisualStyleBackColor = true;
            // 
            // bSaveTimerEdit
            // 
            this.bSaveTimerEdit.Location = new System.Drawing.Point(9, 90);
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
            this.label9.Location = new System.Drawing.Point(6, 38);
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
            // dhmsInputTimerEditTimer
            // 
            this.dhmsInputTimerEditTimer.Location = new System.Drawing.Point(93, 58);
            this.dhmsInputTimerEditTimer.Name = "dhmsInputTimerEditTimer";
            this.dhmsInputTimerEditTimer.Size = new System.Drawing.Size(136, 26);
            this.dhmsInputTimerEditTimer.TabIndex = 15;
            this.dhmsInputTimerEditTimer.Timespan = System.TimeSpan.Parse("00:00:00");
            this.dhmsInputTimerEditTimer.ValueChanged += new ARKBreedingStats.uiControls.dhmsInput.ValueChangedEventHandler(this.dhmsInputTimerEditTimer_ValueChanged);
            this.dhmsInputTimerEditTimer.TextChanged += new System.EventHandler(this.dhmsInputTimerEditTimer_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 64);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Duration";
            // 
            // dateTimePickerEditTimerFinish
            // 
            this.dateTimePickerEditTimerFinish.CustomFormat = "";
            this.dateTimePickerEditTimerFinish.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerEditTimerFinish.Location = new System.Drawing.Point(96, 32);
            this.dateTimePickerEditTimerFinish.Name = "dateTimePickerEditTimerFinish";
            this.dateTimePickerEditTimerFinish.Size = new System.Drawing.Size(156, 20);
            this.dateTimePickerEditTimerFinish.TabIndex = 4;
            this.dateTimePickerEditTimerFinish.ValueChanged += new System.EventHandler(this.dateTimePickerEditTimerFinish_ValueChanged);
            // 
            // parentStats1
            // 
            this.parentStats1.Location = new System.Drawing.Point(6, 397);
            this.parentStats1.Name = "parentStats1";
            this.parentStats1.Size = new System.Drawing.Size(329, 245);
            this.parentStats1.TabIndex = 7;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelRaisingInfos);
            this.groupBox2.Controls.Add(this.listViewRaisingTimes);
            this.groupBox2.Location = new System.Drawing.Point(6, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(329, 220);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General Infos";
            // 
            // RaisingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RaisingControl";
            this.Size = new System.Drawing.Size(918, 549);
            ((System.ComponentModel.ISupportInitialize)(this.nudCurrentWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTotalWeight)).EndInit();
            this.contextMenuStripBabyList.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageMaturationProgress.ResumeLayout(false);
            this.tabPageMaturationProgress.PerformLayout();
            this.tabPageEditTimer.ResumeLayout(false);
            this.tabPageEditTimer.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelRaisingInfos;
        private System.Windows.Forms.ListView listViewRaisingTimes;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderDuration;
        private System.Windows.Forms.ColumnHeader columnHeaderTotalTime;
        private System.Windows.Forms.ColumnHeader columnHeaderUntil;
        private System.Windows.Forms.NumericUpDown nudCurrentWeight;
        private System.Windows.Forms.Label labelMaturationProgress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudTotalWeight;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTimeLeftGrowing;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelTimeLeftBaby;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelAmountFoodBaby;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListView listViewBabies;
        private System.Windows.Forms.ColumnHeader columnHeaderBabyName;
        private System.Windows.Forms.ColumnHeader columnHeaderSpecies;
        private System.Windows.Forms.ColumnHeader columnHeaderBabyTime;
        private System.Windows.Forms.ColumnHeader columnHeaderGrowingTime;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ColumnHeader columnHeaderIncubation;
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
    }
}
