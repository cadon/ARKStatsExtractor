namespace ARKBreedingStats.uiControls
{
    partial class StatsDisplay
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
            this.labelSex = new System.Windows.Forms.Label();
            this.labelStatHeader = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelSex
            // 
            this.labelSex.AutoSize = true;
            this.labelSex.Location = new System.Drawing.Point(3, 0);
            this.labelSex.Name = "labelSex";
            this.labelSex.Size = new System.Drawing.Size(13, 13);
            this.labelSex.TabIndex = 26;
            this.labelSex.Text = "?";
            // 
            // labelStatHeader
            // 
            this.labelStatHeader.AutoSize = true;
            this.labelStatHeader.Location = new System.Drawing.Point(31, 0);
            this.labelStatHeader.Name = "labelStatHeader";
            this.labelStatHeader.Size = new System.Drawing.Size(145, 13);
            this.labelStatHeader.TabIndex = 25;
            this.labelStatHeader.Text = "W      D      Breed       Current";
            // 
            // StatsDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelSex);
            this.Controls.Add(this.labelStatHeader);
            this.Name = "StatsDisplay";
            this.Size = new System.Drawing.Size(182, 203);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSex;
        private System.Windows.Forms.Label labelStatHeader;
    }
}
