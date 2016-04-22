namespace ARKBreedingStats
{
    partial class TimerList
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
            this.listViewTimer = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFinishedAt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTimeLeft = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listViewTimer
            // 
            this.listViewTimer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderFinishedAt,
            this.columnHeaderTimeLeft});
            this.listViewTimer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewTimer.Location = new System.Drawing.Point(0, 0);
            this.listViewTimer.Name = "listViewTimer";
            this.listViewTimer.Size = new System.Drawing.Size(528, 292);
            this.listViewTimer.TabIndex = 0;
            this.listViewTimer.UseCompatibleStateImageBehavior = false;
            this.listViewTimer.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 104;
            // 
            // columnHeaderFinishedAt
            // 
            this.columnHeaderFinishedAt.Text = "Finished at";
            this.columnHeaderFinishedAt.Width = 84;
            // 
            // columnHeaderTimeLeft
            // 
            this.columnHeaderTimeLeft.Text = "Time Left";
            this.columnHeaderTimeLeft.Width = 78;
            // 
            // TimerList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewTimer);
            this.Name = "TimerList";
            this.Size = new System.Drawing.Size(528, 292);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewTimer;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderFinishedAt;
        private System.Windows.Forms.ColumnHeader columnHeaderTimeLeft;
    }
}
