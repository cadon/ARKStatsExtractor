namespace ARKBreedingStats.raising
{
    partial class ParentStatValues
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
            this.label1 = new System.Windows.Forms.Label();
            this.labelM = new System.Windows.Forms.Label();
            this.labelF = new System.Windows.Forms.Label();
            this.lbBestLevel = new System.Windows.Forms.Label();
            this.lbBest = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Dm %";
            // 
            // labelM
            // 
            this.labelM.Location = new System.Drawing.Point(43, 0);
            this.labelM.Name = "labelM";
            this.labelM.Size = new System.Drawing.Size(49, 13);
            this.labelM.TabIndex = 1;
            this.labelM.Text = "1000000";
            // 
            // labelF
            // 
            this.labelF.Location = new System.Drawing.Point(120, 0);
            this.labelF.Name = "labelF";
            this.labelF.Size = new System.Drawing.Size(49, 13);
            this.labelF.TabIndex = 2;
            this.labelF.Text = "1000000";
            // 
            // lbBestLevel
            // 
            this.lbBestLevel.Location = new System.Drawing.Point(274, 0);
            this.lbBestLevel.Name = "lbBestLevel";
            this.lbBestLevel.Size = new System.Drawing.Size(49, 13);
            this.lbBestLevel.TabIndex = 3;
            this.lbBestLevel.Text = "[Lv 100]";
            // 
            // lbBest
            // 
            this.lbBest.Location = new System.Drawing.Point(219, 0);
            this.lbBest.Name = "lbBest";
            this.lbBest.Size = new System.Drawing.Size(49, 13);
            this.lbBest.TabIndex = 4;
            this.lbBest.Text = "1000000";
            // 
            // ParentStatValues
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbBest);
            this.Controls.Add(this.lbBestLevel);
            this.Controls.Add(this.labelF);
            this.Controls.Add(this.labelM);
            this.Controls.Add(this.label1);
            this.Name = "ParentStatValues";
            this.Size = new System.Drawing.Size(326, 15);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelM;
        private System.Windows.Forms.Label labelF;
        private System.Windows.Forms.Label lbBestLevel;
        private System.Windows.Forms.Label lbBest;
    }
}
