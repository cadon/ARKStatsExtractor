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
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxAddTimer = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dhmInputTimer = new ARKBreedingStats.uiControls.dhmInput();
            this.button10m = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.button1h = new System.Windows.Forms.Button();
            this.buttonSet = new System.Windows.Forms.Button();
            this.button5h = new System.Windows.Forms.Button();
            this.button10h = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePickerTimerFinish = new System.Windows.Forms.DateTimePicker();
            this.textBoxTimerName = new System.Windows.Forms.TextBox();
            this.buttonAddTimer = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxAddTimer.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewTimer
            // 
            this.listViewTimer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderFinishedAt,
            this.columnHeaderTimeLeft});
            this.listViewTimer.ContextMenuStrip = this.contextMenuStrip1;
            this.listViewTimer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewTimer.FullRowSelect = true;
            this.listViewTimer.Location = new System.Drawing.Point(243, 3);
            this.listViewTimer.Name = "listViewTimer";
            this.listViewTimer.Size = new System.Drawing.Size(402, 488);
            this.listViewTimer.TabIndex = 0;
            this.listViewTimer.UseCompatibleStateImageBehavior = false;
            this.listViewTimer.View = System.Windows.Forms.View.Details;
            this.listViewTimer.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewTimer_ColumnClick);
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
            this.removeToolStripMenuItem,
            this.addToOverlayToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(156, 48);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // addToOverlayToolStripMenuItem
            // 
            this.addToOverlayToolStripMenuItem.Name = "addToOverlayToolStripMenuItem";
            this.addToOverlayToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.addToOverlayToolStripMenuItem.Text = "Add To Overlay";
            this.addToOverlayToolStripMenuItem.Click += new System.EventHandler(this.addToOverlayToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.listViewTimer, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxAddTimer, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(648, 494);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBoxAddTimer
            // 
            this.groupBoxAddTimer.Controls.Add(this.groupBox1);
            this.groupBoxAddTimer.Controls.Add(this.label2);
            this.groupBoxAddTimer.Controls.Add(this.label1);
            this.groupBoxAddTimer.Controls.Add(this.dateTimePickerTimerFinish);
            this.groupBoxAddTimer.Controls.Add(this.textBoxTimerName);
            this.groupBoxAddTimer.Controls.Add(this.buttonAddTimer);
            this.groupBoxAddTimer.Location = new System.Drawing.Point(3, 3);
            this.groupBoxAddTimer.Name = "groupBoxAddTimer";
            this.groupBoxAddTimer.Size = new System.Drawing.Size(234, 186);
            this.groupBoxAddTimer.TabIndex = 1;
            this.groupBoxAddTimer.TabStop = false;
            this.groupBoxAddTimer.Text = "Add Manual Timer";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dhmInputTimer);
            this.groupBox1.Controls.Add(this.button10m);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.button1h);
            this.groupBox1.Controls.Add(this.buttonSet);
            this.groupBox1.Controls.Add(this.button5h);
            this.groupBox1.Controls.Add(this.button10h);
            this.groupBox1.Location = new System.Drawing.Point(6, 71);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(222, 79);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Set Timer to end in";
            // 
            // dhmInputTimer
            // 
            this.dhmInputTimer.BackColor = System.Drawing.SystemColors.Window;
            this.dhmInputTimer.ForeColor = System.Drawing.SystemColors.GrayText;
            this.dhmInputTimer.Location = new System.Drawing.Point(87, 50);
            this.dhmInputTimer.Mask = "00\\:00\\:00";
            this.dhmInputTimer.Name = "dhmInputTimer";
            this.dhmInputTimer.Size = new System.Drawing.Size(73, 20);
            this.dhmInputTimer.TabIndex = 13;
            this.dhmInputTimer.Text = "000000";
            this.dhmInputTimer.Timespan = System.TimeSpan.Parse("00:00:00");
            // 
            // button10m
            // 
            this.button10m.Location = new System.Drawing.Point(6, 19);
            this.button10m.Name = "button10m";
            this.button10m.Size = new System.Drawing.Size(48, 23);
            this.button10m.TabIndex = 6;
            this.button10m.Text = "10 m";
            this.button10m.UseVisualStyleBackColor = true;
            this.button10m.Click += new System.EventHandler(this.button10m_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Custom [d:h:m]";
            // 
            // button1h
            // 
            this.button1h.Location = new System.Drawing.Point(60, 19);
            this.button1h.Name = "button1h";
            this.button1h.Size = new System.Drawing.Size(48, 23);
            this.button1h.TabIndex = 7;
            this.button1h.Text = "1 h";
            this.button1h.UseVisualStyleBackColor = true;
            this.button1h.Click += new System.EventHandler(this.button1h_Click);
            // 
            // buttonSet
            // 
            this.buttonSet.Location = new System.Drawing.Point(166, 48);
            this.buttonSet.Name = "buttonSet";
            this.buttonSet.Size = new System.Drawing.Size(50, 23);
            this.buttonSet.TabIndex = 11;
            this.buttonSet.Text = "Set";
            this.buttonSet.UseVisualStyleBackColor = true;
            this.buttonSet.Click += new System.EventHandler(this.buttonSet_Click);
            // 
            // button5h
            // 
            this.button5h.Location = new System.Drawing.Point(114, 19);
            this.button5h.Name = "button5h";
            this.button5h.Size = new System.Drawing.Size(48, 23);
            this.button5h.TabIndex = 8;
            this.button5h.Text = "5 h";
            this.button5h.UseVisualStyleBackColor = true;
            this.button5h.Click += new System.EventHandler(this.button5h_Click);
            // 
            // button10h
            // 
            this.button10h.Location = new System.Drawing.Point(168, 19);
            this.button10h.Name = "button10h";
            this.button10h.Size = new System.Drawing.Size(48, 23);
            this.button10h.TabIndex = 9;
            this.button10h.Text = "10 h";
            this.button10h.UseVisualStyleBackColor = true;
            this.button10h.Click += new System.EventHandler(this.button10h_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Finish Time";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
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
            this.textBoxTimerName.Location = new System.Drawing.Point(72, 19);
            this.textBoxTimerName.Name = "textBoxTimerName";
            this.textBoxTimerName.Size = new System.Drawing.Size(156, 20);
            this.textBoxTimerName.TabIndex = 1;
            // 
            // buttonAddTimer
            // 
            this.buttonAddTimer.Location = new System.Drawing.Point(6, 156);
            this.buttonAddTimer.Name = "buttonAddTimer";
            this.buttonAddTimer.Size = new System.Drawing.Size(222, 23);
            this.buttonAddTimer.TabIndex = 4;
            this.buttonAddTimer.Text = "Add Timer";
            this.buttonAddTimer.UseVisualStyleBackColor = true;
            this.buttonAddTimer.Click += new System.EventHandler(this.buttonAddTimer_Click);
            // 
            // TimerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TimerControl";
            this.Size = new System.Drawing.Size(648, 494);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBoxAddTimer.ResumeLayout(false);
            this.groupBoxAddTimer.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.Button button10h;
        private System.Windows.Forms.Button button5h;
        private System.Windows.Forms.Button button1h;
        private System.Windows.Forms.Button button10m;
        private System.Windows.Forms.ToolStripMenuItem addToOverlayToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonSet;
        private System.Windows.Forms.GroupBox groupBox1;
        private uiControls.dhmInput dhmInputTimer;
    }
}
