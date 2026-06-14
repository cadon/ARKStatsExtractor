using ARKBreedingStats.uiControls;

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
            components = new System.ComponentModel.Container();
            listViewTimer = new System.Windows.Forms.ListView();
            columnHeaderName = new System.Windows.Forms.ColumnHeader();
            columnHeaderFinishedAt = new System.Windows.Forms.ColumnHeader();
            columnHeaderTimeLeft = new System.Windows.Forms.ColumnHeader();
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            addToOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            addAllTimersToOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            hideAllTimersFromOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            removeAllExpiredTimersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            groupBox2 = new GroupBoxC();
            LbTimerPresets = new System.Windows.Forms.ListBox();
            panel1 = new System.Windows.Forms.Panel();
            BtRemovePreset = new System.Windows.Forms.Button();
            BtAddPreset = new System.Windows.Forms.Button();
            groupBoxAddTimer = new GroupBoxC();
            label4 = new System.Windows.Forms.Label();
            btPlaySelectedSound = new System.Windows.Forms.Button();
            btOpenSoundFolder = new System.Windows.Forms.Button();
            SoundListBox = new System.Windows.Forms.ComboBox();
            SoundLabel = new System.Windows.Forms.Label();
            groupBox1 = new GroupBoxC();
            bSetTimerNow = new System.Windows.Forms.Button();
            dhmsInputTimer = new dhmsInput();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            dateTimePickerTimerFinish = new System.Windows.Forms.DateTimePicker();
            textBoxTimerName = new System.Windows.Forms.TextBox();
            buttonAddTimer = new System.Windows.Forms.Button();
            BtStartPauseTimers = new System.Windows.Forms.Button();
            contextMenuStripTimerHeader = new System.Windows.Forms.ContextMenuStrip(components);
            toolStripMenuItemResetTimerColumnWidths = new System.Windows.Forms.ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            groupBox2.SuspendLayout();
            panel1.SuspendLayout();
            groupBoxAddTimer.SuspendLayout();
            groupBox1.SuspendLayout();
            contextMenuStripTimerHeader.SuspendLayout();
            SuspendLayout();
            // 
            // listViewTimer
            // 
            listViewTimer.AllowColumnReorder = true;
            listViewTimer.CheckBoxes = true;
            listViewTimer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderName, columnHeaderFinishedAt, columnHeaderTimeLeft });
            listViewTimer.ContextMenuStrip = contextMenuStrip1;
            listViewTimer.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewTimer.FullRowSelect = true;
            listViewTimer.Location = new System.Drawing.Point(243, 3);
            listViewTimer.Name = "listViewTimer";
            tableLayoutPanel1.SetRowSpan(listViewTimer, 3);
            listViewTimer.Size = new System.Drawing.Size(402, 584);
            listViewTimer.TabIndex = 0;
            listViewTimer.UseCompatibleStateImageBehavior = false;
            listViewTimer.View = System.Windows.Forms.View.Details;
            listViewTimer.ColumnClick += listViewTimer_ColumnClick;
            listViewTimer.ItemChecked += listViewTimer_ItemChecked;
            listViewTimer.KeyUp += listViewTimer_KeyUp;
            // 
            // columnHeaderName
            // 
            columnHeaderName.Text = "Name";
            columnHeaderName.Width = 123;
            // 
            // columnHeaderFinishedAt
            // 
            columnHeaderFinishedAt.Text = "Finished at";
            columnHeaderFinishedAt.Width = 116;
            // 
            // columnHeaderTimeLeft
            // 
            columnHeaderTimeLeft.Text = "Time Left";
            columnHeaderTimeLeft.Width = 78;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { addToOverlayToolStripMenuItem, addAllTimersToOverlayToolStripMenuItem, hideAllTimersFromOverlayToolStripMenuItem, toolStripSeparator1, removeToolStripMenuItem, removeAllExpiredTimersToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(222, 120);
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            // 
            // addToOverlayToolStripMenuItem
            // 
            addToOverlayToolStripMenuItem.Name = "addToOverlayToolStripMenuItem";
            addToOverlayToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            addToOverlayToolStripMenuItem.Text = "Add to overlay";
            addToOverlayToolStripMenuItem.Click += addToOverlayToolStripMenuItem_Click;
            // 
            // addAllTimersToOverlayToolStripMenuItem
            // 
            addAllTimersToOverlayToolStripMenuItem.Name = "addAllTimersToOverlayToolStripMenuItem";
            addAllTimersToOverlayToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            addAllTimersToOverlayToolStripMenuItem.Text = "Add all timers to overlay";
            addAllTimersToOverlayToolStripMenuItem.Click += addAllTimersToOverlayToolStripMenuItem_Click;
            // 
            // hideAllTimersFromOverlayToolStripMenuItem
            // 
            hideAllTimersFromOverlayToolStripMenuItem.Name = "hideAllTimersFromOverlayToolStripMenuItem";
            hideAllTimersFromOverlayToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            hideAllTimersFromOverlayToolStripMenuItem.Text = "Hide all timers from overlay";
            hideAllTimersFromOverlayToolStripMenuItem.Click += hideAllTimersFromOverlayToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(218, 6);
            // 
            // removeToolStripMenuItem
            // 
            removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            removeToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            removeToolStripMenuItem.Text = "Remove selected Timers...";
            removeToolStripMenuItem.Click += removeToolStripMenuItem_Click;
            // 
            // removeAllExpiredTimersToolStripMenuItem
            // 
            removeAllExpiredTimersToolStripMenuItem.Name = "removeAllExpiredTimersToolStripMenuItem";
            removeAllExpiredTimersToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            removeAllExpiredTimersToolStripMenuItem.Text = "Remove all expired Timers...";
            removeAllExpiredTimersToolStripMenuItem.Click += removeAllExpiredTimersToolStripMenuItem_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(groupBox2, 0, 2);
            tableLayoutPanel1.Controls.Add(listViewTimer, 1, 0);
            tableLayoutPanel1.Controls.Add(groupBoxAddTimer, 0, 0);
            tableLayoutPanel1.Controls.Add(BtStartPauseTimers, 0, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(648, 590);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(LbTimerPresets);
            groupBox2.Controls.Add(panel1);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox2.Location = new System.Drawing.Point(3, 448);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(234, 139);
            groupBox2.TabIndex = 15;
            groupBox2.TabStop = false;
            groupBox2.Text = "Presets";
            // 
            // LbTimerPresets
            // 
            LbTimerPresets.Dock = System.Windows.Forms.DockStyle.Fill;
            LbTimerPresets.FormattingEnabled = true;
            LbTimerPresets.Location = new System.Drawing.Point(3, 49);
            LbTimerPresets.Name = "LbTimerPresets";
            LbTimerPresets.Size = new System.Drawing.Size(228, 87);
            LbTimerPresets.TabIndex = 3;
            LbTimerPresets.SelectedIndexChanged += LbTimerPresets_SelectedIndexChanged;
            LbTimerPresets.MouseDoubleClick += LbTimerPresets_MouseDoubleClick;
            // 
            // panel1
            // 
            panel1.Controls.Add(BtRemovePreset);
            panel1.Controls.Add(BtAddPreset);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(3, 19);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(228, 30);
            panel1.TabIndex = 15;
            // 
            // BtRemovePreset
            // 
            BtRemovePreset.Location = new System.Drawing.Point(105, 3);
            BtRemovePreset.Name = "BtRemovePreset";
            BtRemovePreset.Size = new System.Drawing.Size(120, 23);
            BtRemovePreset.TabIndex = 19;
            BtRemovePreset.Text = "Remove Preset";
            BtRemovePreset.UseVisualStyleBackColor = true;
            BtRemovePreset.Click += BtRemovePreset_Click;
            // 
            // BtAddPreset
            // 
            BtAddPreset.Location = new System.Drawing.Point(3, 3);
            BtAddPreset.Name = "BtAddPreset";
            BtAddPreset.Size = new System.Drawing.Size(96, 23);
            BtAddPreset.TabIndex = 18;
            BtAddPreset.Text = "Add Preset";
            BtAddPreset.UseVisualStyleBackColor = true;
            BtAddPreset.Click += BtAddPreset_Click;
            // 
            // groupBoxAddTimer
            // 
            groupBoxAddTimer.Controls.Add(label4);
            groupBoxAddTimer.Controls.Add(btPlaySelectedSound);
            groupBoxAddTimer.Controls.Add(btOpenSoundFolder);
            groupBoxAddTimer.Controls.Add(SoundListBox);
            groupBoxAddTimer.Controls.Add(SoundLabel);
            groupBoxAddTimer.Controls.Add(groupBox1);
            groupBoxAddTimer.Controls.Add(label2);
            groupBoxAddTimer.Controls.Add(label1);
            groupBoxAddTimer.Controls.Add(dateTimePickerTimerFinish);
            groupBoxAddTimer.Controls.Add(textBoxTimerName);
            groupBoxAddTimer.Controls.Add(buttonAddTimer);
            groupBoxAddTimer.Location = new System.Drawing.Point(3, 3);
            groupBoxAddTimer.Name = "groupBoxAddTimer";
            groupBoxAddTimer.Size = new System.Drawing.Size(234, 399);
            groupBoxAddTimer.TabIndex = 1;
            groupBoxAddTimer.TabStop = false;
            groupBoxAddTimer.Text = "Add Manual Timer";
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(6, 335);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(222, 36);
            label4.TabIndex = 17;
            label4.Text = "The checkboxes can be used to display timers in the overlay.";
            // 
            // btPlaySelectedSound
            // 
            btPlaySelectedSound.Location = new System.Drawing.Point(6, 97);
            btPlaySelectedSound.Name = "btPlaySelectedSound";
            btPlaySelectedSound.Size = new System.Drawing.Size(60, 23);
            btPlaySelectedSound.TabIndex = 16;
            btPlaySelectedSound.Text = "⏵";
            btPlaySelectedSound.UseVisualStyleBackColor = true;
            btPlaySelectedSound.Click += btPlaySelectedSound_Click;
            // 
            // btOpenSoundFolder
            // 
            btOpenSoundFolder.Location = new System.Drawing.Point(72, 97);
            btOpenSoundFolder.Name = "btOpenSoundFolder";
            btOpenSoundFolder.Size = new System.Drawing.Size(156, 23);
            btOpenSoundFolder.TabIndex = 15;
            btOpenSoundFolder.Text = "Open custom sounds folder";
            btOpenSoundFolder.UseVisualStyleBackColor = true;
            btOpenSoundFolder.Click += btOpenSoundFolder_Click;
            // 
            // SoundListBox
            // 
            SoundListBox.FormattingEnabled = true;
            SoundListBox.Location = new System.Drawing.Point(80, 70);
            SoundListBox.Name = "SoundListBox";
            SoundListBox.Size = new System.Drawing.Size(148, 23);
            SoundListBox.TabIndex = 15;
            // 
            // SoundLabel
            // 
            SoundLabel.AutoSize = true;
            SoundLabel.Location = new System.Drawing.Point(6, 75);
            SoundLabel.Name = "SoundLabel";
            SoundLabel.Size = new System.Drawing.Size(41, 15);
            SoundLabel.TabIndex = 14;
            SoundLabel.Text = "Sound";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(bSetTimerNow);
            groupBox1.Controls.Add(dhmsInputTimer);
            groupBox1.Controls.Add(label3);
            groupBox1.Location = new System.Drawing.Point(6, 176);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(222, 143);
            groupBox1.TabIndex = 13;
            groupBox1.TabStop = false;
            groupBox1.Text = "Set Timer to end in";
            // 
            // bSetTimerNow
            // 
            bSetTimerNow.Location = new System.Drawing.Point(6, 19);
            bSetTimerNow.Name = "bSetTimerNow";
            bSetTimerNow.Size = new System.Drawing.Size(90, 23);
            bSetTimerNow.TabIndex = 14;
            bSetTimerNow.Text = "now (reset)";
            bSetTimerNow.UseVisualStyleBackColor = true;
            bSetTimerNow.Click += bSetTimerNow_Click;
            // 
            // dhmsInputTimer
            // 
            dhmsInputTimer.Location = new System.Drawing.Point(53, 111);
            dhmsInputTimer.Name = "dhmsInputTimer";
            dhmsInputTimer.Size = new System.Drawing.Size(138, 26);
            dhmsInputTimer.TabIndex = 13;
            dhmsInputTimer.ValueChanged += dhmsInputTimer_ValueChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(5, 119);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(49, 15);
            label3.TabIndex = 12;
            label3.Text = "Custom";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(6, 50);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(68, 15);
            label2.TabIndex = 2;
            label2.Text = "Finish Time";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 25);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(39, 15);
            label1.TabIndex = 0;
            label1.Text = "Name";
            // 
            // dateTimePickerTimerFinish
            // 
            dateTimePickerTimerFinish.CustomFormat = "";
            dateTimePickerTimerFinish.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            dateTimePickerTimerFinish.Location = new System.Drawing.Point(80, 45);
            dateTimePickerTimerFinish.Name = "dateTimePickerTimerFinish";
            dateTimePickerTimerFinish.Size = new System.Drawing.Size(148, 23);
            dateTimePickerTimerFinish.TabIndex = 3;
            // 
            // textBoxTimerName
            // 
            textBoxTimerName.Location = new System.Drawing.Point(80, 20);
            textBoxTimerName.Name = "textBoxTimerName";
            textBoxTimerName.Size = new System.Drawing.Size(148, 23);
            textBoxTimerName.TabIndex = 1;
            // 
            // buttonAddTimer
            // 
            buttonAddTimer.Location = new System.Drawing.Point(6, 126);
            buttonAddTimer.Name = "buttonAddTimer";
            buttonAddTimer.Size = new System.Drawing.Size(222, 44);
            buttonAddTimer.TabIndex = 4;
            buttonAddTimer.Text = "Add Timer";
            buttonAddTimer.UseVisualStyleBackColor = true;
            buttonAddTimer.Click += buttonAddTimer_Click;
            // 
            // BtStartPauseTimers
            // 
            BtStartPauseTimers.Dock = System.Windows.Forms.DockStyle.Fill;
            BtStartPauseTimers.Font = new System.Drawing.Font("Segoe UI", 15.75F);
            BtStartPauseTimers.Location = new System.Drawing.Point(3, 408);
            BtStartPauseTimers.Name = "BtStartPauseTimers";
            BtStartPauseTimers.Size = new System.Drawing.Size(234, 34);
            BtStartPauseTimers.TabIndex = 2;
            BtStartPauseTimers.Text = "⏯";
            BtStartPauseTimers.UseVisualStyleBackColor = true;
            BtStartPauseTimers.Click += BtStartPauseTimers_Click;
            // 
            // contextMenuStripTimerHeader
            // 
            contextMenuStripTimerHeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMenuItemResetTimerColumnWidths });
            contextMenuStripTimerHeader.Name = "contextMenuStrip2";
            contextMenuStripTimerHeader.Size = new System.Drawing.Size(189, 26);
            // 
            // toolStripMenuItemResetTimerColumnWidths
            // 
            toolStripMenuItemResetTimerColumnWidths.Name = "toolStripMenuItemResetTimerColumnWidths";
            toolStripMenuItemResetTimerColumnWidths.Size = new System.Drawing.Size(188, 22);
            toolStripMenuItemResetTimerColumnWidths.Text = "Reset Column Widths";
            toolStripMenuItemResetTimerColumnWidths.Click += toolStripMenuItemResetLibraryColumnWidths_Click;
            // 
            // TimerControl
            // 
            Controls.Add(tableLayoutPanel1);
            Name = "TimerControl";
            Size = new System.Drawing.Size(648, 590);
            contextMenuStrip1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            groupBoxAddTimer.ResumeLayout(false);
            groupBoxAddTimer.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            contextMenuStripTimerHeader.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewTimer;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderFinishedAt;
        private System.Windows.Forms.ColumnHeader columnHeaderTimeLeft;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private GroupBoxC groupBoxAddTimer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePickerTimerFinish;
        private System.Windows.Forms.TextBox textBoxTimerName;
        private System.Windows.Forms.Button buttonAddTimer;
        private System.Windows.Forms.ToolStripMenuItem addToOverlayToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private GroupBoxC groupBox1;
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
        private GroupBoxC groupBox2;
        private System.Windows.Forms.Panel panel1;
    }
}
