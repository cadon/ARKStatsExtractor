namespace ARKBreedingStats
{
    partial class TimerControl
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
            this.listViewTimer = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFinishedAt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTimeLeft = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAllTimersToOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideAllTimersFromOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllExpiredTimersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxAddTimer = new System.Windows.Forms.GroupBox();
            this.BtRemovePreset = new System.Windows.Forms.Button();
            this.BtAddPreset = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btPlaySelectedSound = new System.Windows.Forms.Button();
            this.btOpenSoundFolder = new System.Windows.Forms.Button();
            this.SoundListBox = new System.Windows.Forms.ComboBox();
            this.SoundLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bSetTimerNow = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePickerTimerFinish = new System.Windows.Forms.DateTimePicker();
            this.textBoxTimerName = new System.Windows.Forms.TextBox();
            this.buttonAddTimer = new System.Windows.Forms.Button();
            this.BtStartPauseTimers = new System.Windows.Forms.Button();
            this.LbTimerPresets = new System.Windows.Forms.ListBox();
            this.contextMenuStripTimerHeader = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemResetTimerColumnWidths = new System.Windows.Forms.ToolStripMenuItem();
            this.dhmsInputTimer = new ARKBreedingStats.uiControls.dhmsInput();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxAddTimer.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.contextMenuStripTimerHeader.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewTimer
            // 
            this.listViewTimer.AllowColumnReorder = true;
            this.listViewTimer.CheckBoxes = true;
            this.listViewTimer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderFinishedAt,
            this.columnHeaderTimeLeft});
            this.listViewTimer.ContextMenuStrip = this.contextMenuStrip1;
            this.listViewTimer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewTimer.FullRowSelect = true;
            this.listViewTimer.HideSelection = false;
            this.listViewTimer.Location = new System.Drawing.Point(243, 3);
            this.listViewTimer.Name = "listViewTimer";
            this.tableLayoutPanel1.SetRowSpan(this.listViewTimer, 3);
            this.listViewTimer.Size = new System.Drawing.Size(402, 584);
            this.listViewTimer.TabIndex = 0;
            this.listViewTimer.UseCompatibleStateImageBehavior = false;
            this.listViewTimer.View = System.Windows.Forms.View.Details;
            this.listViewTimer.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewTimer_ColumnClick);
            this.listViewTimer.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewTimer_ItemChecked);
            this.listViewTimer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listViewTimer_KeyUp);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 123;
            // 
            // columnHeaderFinishedAt
            // 
            this.columnHeaderFinishedAt.Text = "Finished at";
            this.columnHeaderFinishedAt.Width = 116;
            // 
            // columnHeaderTimeLeft
            // 
            this.columnHeaderTimeLeft.Text = "Time Left";
            this.columnHeaderTimeLeft.Width = 78;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToOverlayToolStripMenuItem,
            this.addAllTimersToOverlayToolStripMenuItem,
            this.hideAllTimersFromOverlayToolStripMenuItem,
            this.toolStripSeparator1,
            this.removeToolStripMenuItem,
            this.removeAllExpiredTimersToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(222, 120);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // addToOverlayToolStripMenuItem
            // 
            this.addToOverlayToolStripMenuItem.Name = "addToOverlayToolStripMenuItem";
            this.addToOverlayToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.addToOverlayToolStripMenuItem.Text = "Add to overlay";
            this.addToOverlayToolStripMenuItem.Click += new System.EventHandler(this.addToOverlayToolStripMenuItem_Click);
            // 
            // addAllTimersToOverlayToolStripMenuItem
            // 
            this.addAllTimersToOverlayToolStripMenuItem.Name = "addAllTimersToOverlayToolStripMenuItem";
            this.addAllTimersToOverlayToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.addAllTimersToOverlayToolStripMenuItem.Text = "Add all timers to overlay";
            this.addAllTimersToOverlayToolStripMenuItem.Click += new System.EventHandler(this.addAllTimersToOverlayToolStripMenuItem_Click);
            // 
            // hideAllTimersFromOverlayToolStripMenuItem
            // 
            this.hideAllTimersFromOverlayToolStripMenuItem.Name = "hideAllTimersFromOverlayToolStripMenuItem";
            this.hideAllTimersFromOverlayToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.hideAllTimersFromOverlayToolStripMenuItem.Text = "Hide all timers from overlay";
            this.hideAllTimersFromOverlayToolStripMenuItem.Click += new System.EventHandler(this.hideAllTimersFromOverlayToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(218, 6);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.removeToolStripMenuItem.Text = "Remove selected Timers...";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // removeAllExpiredTimersToolStripMenuItem
            // 
            this.removeAllExpiredTimersToolStripMenuItem.Name = "removeAllExpiredTimersToolStripMenuItem";
            this.removeAllExpiredTimersToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.removeAllExpiredTimersToolStripMenuItem.Text = "Remove all expired Timers...";
            this.removeAllExpiredTimersToolStripMenuItem.Click += new System.EventHandler(this.removeAllExpiredTimersToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.listViewTimer, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxAddTimer, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.BtStartPauseTimers, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(648, 590);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBoxAddTimer
            // 
            this.groupBoxAddTimer.Controls.Add(this.label4);
            this.groupBoxAddTimer.Controls.Add(this.btPlaySelectedSound);
            this.groupBoxAddTimer.Controls.Add(this.btOpenSoundFolder);
            this.groupBoxAddTimer.Controls.Add(this.SoundListBox);
            this.groupBoxAddTimer.Controls.Add(this.SoundLabel);
            this.groupBoxAddTimer.Controls.Add(this.groupBox1);
            this.groupBoxAddTimer.Controls.Add(this.label2);
            this.groupBoxAddTimer.Controls.Add(this.label1);
            this.groupBoxAddTimer.Controls.Add(this.dateTimePickerTimerFinish);
            this.groupBoxAddTimer.Controls.Add(this.textBoxTimerName);
            this.groupBoxAddTimer.Controls.Add(this.buttonAddTimer);
            this.groupBoxAddTimer.Location = new System.Drawing.Point(3, 3);
            this.groupBoxAddTimer.Name = "groupBoxAddTimer";
            this.groupBoxAddTimer.Size = new System.Drawing.Size(234, 399);
            this.groupBoxAddTimer.TabIndex = 1;
            this.groupBoxAddTimer.TabStop = false;
            this.groupBoxAddTimer.Text = "Add Manual Timer";
            // 
            // BtRemovePreset
            // 
            this.BtRemovePreset.Location = new System.Drawing.Point(105, 3);
            this.BtRemovePreset.Name = "BtRemovePreset";
            this.BtRemovePreset.Size = new System.Drawing.Size(120, 23);
            this.BtRemovePreset.TabIndex = 19;
            this.BtRemovePreset.Text = "Remove Preset";
            this.BtRemovePreset.UseVisualStyleBackColor = true;
            this.BtRemovePreset.Click += new System.EventHandler(this.BtRemovePreset_Click);
            // 
            // BtAddPreset
            // 
            this.BtAddPreset.Location = new System.Drawing.Point(3, 3);
            this.BtAddPreset.Name = "BtAddPreset";
            this.BtAddPreset.Size = new System.Drawing.Size(96, 23);
            this.BtAddPreset.TabIndex = 18;
            this.BtAddPreset.Text = "Add Preset";
            this.BtAddPreset.UseVisualStyleBackColor = true;
            this.BtAddPreset.Click += new System.EventHandler(this.BtAddPreset_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 335);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(222, 36);
            this.label4.TabIndex = 17;
            this.label4.Text = "The checkboxes can be used to display timers in the overlay.";
            // 
            // btPlaySelectedSound
            // 
            this.btPlaySelectedSound.Location = new System.Drawing.Point(6, 97);
            this.btPlaySelectedSound.Name = "btPlaySelectedSound";
            this.btPlaySelectedSound.Size = new System.Drawing.Size(60, 23);
            this.btPlaySelectedSound.TabIndex = 16;
            this.btPlaySelectedSound.Text = "⏵";
            this.btPlaySelectedSound.UseVisualStyleBackColor = true;
            this.btPlaySelectedSound.Click += new System.EventHandler(this.btPlaySelectedSound_Click);
            // 
            // btOpenSoundFolder
            // 
            this.btOpenSoundFolder.Location = new System.Drawing.Point(72, 97);
            this.btOpenSoundFolder.Name = "btOpenSoundFolder";
            this.btOpenSoundFolder.Size = new System.Drawing.Size(156, 23);
            this.btOpenSoundFolder.TabIndex = 15;
            this.btOpenSoundFolder.Text = "Open custom sounds folder";
            this.btOpenSoundFolder.UseVisualStyleBackColor = true;
            this.btOpenSoundFolder.Click += new System.EventHandler(this.btOpenSoundFolder_Click);
            // 
            // SoundListBox
            // 
            this.SoundListBox.FormattingEnabled = true;
            this.SoundListBox.Location = new System.Drawing.Point(72, 70);
            this.SoundListBox.Name = "SoundListBox";
            this.SoundListBox.Size = new System.Drawing.Size(156, 21);
            this.SoundListBox.TabIndex = 15;
            // 
            // SoundLabel
            // 
            this.SoundLabel.AutoSize = true;
            this.SoundLabel.Location = new System.Drawing.Point(6, 75);
            this.SoundLabel.Name = "SoundLabel";
            this.SoundLabel.Size = new System.Drawing.Size(38, 13);
            this.SoundLabel.TabIndex = 14;
            this.SoundLabel.Text = "Sound";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bSetTimerNow);
            this.groupBox1.Controls.Add(this.dhmsInputTimer);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(6, 176);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(222, 143);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Set Timer to end in";
            // 
            // bSetTimerNow
            // 
            this.bSetTimerNow.Location = new System.Drawing.Point(6, 19);
            this.bSetTimerNow.Name = "bSetTimerNow";
            this.bSetTimerNow.Size = new System.Drawing.Size(90, 23);
            this.bSetTimerNow.TabIndex = 14;
            this.bSetTimerNow.Text = "now (reset)";
            this.bSetTimerNow.UseVisualStyleBackColor = true;
            this.bSetTimerNow.Click += new System.EventHandler(this.bSetTimerNow_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Custom";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Finish Time";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // dateTimePickerTimerFinish
            // 
            this.dateTimePickerTimerFinish.CustomFormat = "";
            this.dateTimePickerTimerFinish.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerTimerFinish.Location = new System.Drawing.Point(72, 45);
            this.dateTimePickerTimerFinish.Name = "dateTimePickerTimerFinish";
            this.dateTimePickerTimerFinish.Size = new System.Drawing.Size(156, 20);
            this.dateTimePickerTimerFinish.TabIndex = 3;
            // 
            // textBoxTimerName
            // 
            this.textBoxTimerName.Location = new System.Drawing.Point(72, 20);
            this.textBoxTimerName.Name = "textBoxTimerName";
            this.textBoxTimerName.Size = new System.Drawing.Size(156, 20);
            this.textBoxTimerName.TabIndex = 1;
            // 
            // buttonAddTimer
            // 
            this.buttonAddTimer.Location = new System.Drawing.Point(6, 126);
            this.buttonAddTimer.Name = "buttonAddTimer";
            this.buttonAddTimer.Size = new System.Drawing.Size(222, 44);
            this.buttonAddTimer.TabIndex = 4;
            this.buttonAddTimer.Text = "Add Timer";
            this.buttonAddTimer.UseVisualStyleBackColor = true;
            this.buttonAddTimer.Click += new System.EventHandler(this.buttonAddTimer_Click);
            // 
            // BtStartPauseTimers
            // 
            this.BtStartPauseTimers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtStartPauseTimers.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.BtStartPauseTimers.Location = new System.Drawing.Point(3, 408);
            this.BtStartPauseTimers.Name = "BtStartPauseTimers";
            this.BtStartPauseTimers.Size = new System.Drawing.Size(234, 34);
            this.BtStartPauseTimers.TabIndex = 2;
            this.BtStartPauseTimers.Text = "⏯";
            this.BtStartPauseTimers.UseVisualStyleBackColor = true;
            this.BtStartPauseTimers.Click += new System.EventHandler(this.BtStartPauseTimers_Click);
            // 
            // LbTimerPresets
            // 
            this.LbTimerPresets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LbTimerPresets.FormattingEnabled = true;
            this.LbTimerPresets.Location = new System.Drawing.Point(3, 46);
            this.LbTimerPresets.Name = "LbTimerPresets";
            this.LbTimerPresets.Size = new System.Drawing.Size(228, 90);
            this.LbTimerPresets.TabIndex = 3;
            this.LbTimerPresets.SelectedIndexChanged += new System.EventHandler(this.LbTimerPresets_SelectedIndexChanged);
            this.LbTimerPresets.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LbTimerPresets_MouseDoubleClick);
            // 
            // contextMenuStripTimerHeader
            // 
            this.contextMenuStripTimerHeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemResetTimerColumnWidths});
            this.contextMenuStripTimerHeader.Name = "contextMenuStrip2";
            this.contextMenuStripTimerHeader.Size = new System.Drawing.Size(189, 26);
            // 
            // toolStripMenuItemResetTimerColumnWidths
            // 
            this.toolStripMenuItemResetTimerColumnWidths.Name = "toolStripMenuItemResetTimerColumnWidths";
            this.toolStripMenuItemResetTimerColumnWidths.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItemResetTimerColumnWidths.Text = "Reset Column Widths";
            this.toolStripMenuItemResetTimerColumnWidths.Click += new System.EventHandler(this.toolStripMenuItemResetLibraryColumnWidths_Click);
            // 
            // dhmsInputTimer
            // 
            this.dhmsInputTimer.Location = new System.Drawing.Point(53, 111);
            this.dhmsInputTimer.Name = "dhmsInputTimer";
            this.dhmsInputTimer.Size = new System.Drawing.Size(136, 26);
            this.dhmsInputTimer.TabIndex = 13;
            this.dhmsInputTimer.Timespan = System.TimeSpan.Parse("00:00:00");
            this.dhmsInputTimer.ValueChanged += new ARKBreedingStats.uiControls.dhmsInput.ValueChangedEventHandler(this.dhmsInputTimer_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.LbTimerPresets);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 448);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(234, 139);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Presets";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtRemovePreset);
            this.panel1.Controls.Add(this.BtAddPreset);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(228, 30);
            this.panel1.TabIndex = 15;
            // 
            // TimerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TimerControl";
            this.Size = new System.Drawing.Size(648, 590);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBoxAddTimer.ResumeLayout(false);
            this.groupBoxAddTimer.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMenuStripTimerHeader.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewTimer;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderFinishedAt;
        private System.Windows.Forms.ColumnHeader columnHeaderTimeLeft;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBoxAddTimer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePickerTimerFinish;
        private System.Windows.Forms.TextBox textBoxTimerName;
        private System.Windows.Forms.Button buttonAddTimer;
        private System.Windows.Forms.ToolStripMenuItem addToOverlayToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private uiControls.dhmsInput dhmsInputTimer;
        private System.Windows.Forms.Button bSetTimerNow;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem removeAllExpiredTimersToolStripMenuItem;
        private System.Windows.Forms.ComboBox SoundListBox;
        private System.Windows.Forms.Label SoundLabel;
        private System.Windows.Forms.Button btOpenSoundFolder;
        private System.Windows.Forms.Button btPlaySelectedSound;
        private System.Windows.Forms.ToolStripMenuItem addAllTimersToOverlayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideAllTimersFromOverlayToolStripMenuItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTimerHeader;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemResetTimerColumnWidths;
        private System.Windows.Forms.Button BtStartPauseTimers;
        private System.Windows.Forms.Button BtRemovePreset;
        private System.Windows.Forms.Button BtAddPreset;
        private System.Windows.Forms.ListBox LbTimerPresets;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel1;
    }
}
