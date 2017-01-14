namespace ARKBreedingStats
{
    partial class OffspringPossibilities
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
            this.labelMin = new System.Windows.Forms.Label();
            this.labelMax = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelMaxProb = new System.Windows.Forms.Label();
            this.panelLine = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // labelMin
            // 
            this.labelMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelMin.Location = new System.Drawing.Point(3, 187);
            this.labelMin.Name = "labelMin";
            this.labelMin.Size = new System.Drawing.Size(105, 13);
            this.labelMin.TabIndex = 0;
            this.labelMin.Text = "0";
            this.labelMin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelMax
            // 
            this.labelMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMax.Location = new System.Drawing.Point(205, 187);
            this.labelMax.Name = "labelMax";
            this.labelMax.Size = new System.Drawing.Size(92, 13);
            this.labelMax.TabIndex = 1;
            this.labelMax.Text = "0";
            this.labelMax.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(128, 188);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Level";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Probability";
            // 
            // labelMaxProb
            // 
            this.labelMaxProb.AutoSize = true;
            this.labelMaxProb.Location = new System.Drawing.Point(3, 3);
            this.labelMaxProb.Name = "labelMaxProb";
            this.labelMaxProb.Size = new System.Drawing.Size(13, 13);
            this.labelMaxProb.TabIndex = 4;
            this.labelMaxProb.Text = "0";
            // 
            // panelLine
            // 
            this.panelLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLine.Location = new System.Drawing.Point(0, 0);
            this.panelLine.Name = "panelLine";
            this.panelLine.Size = new System.Drawing.Size(1, 200);
            this.panelLine.TabIndex = 5;
            // 
            // OffspringPossibilities
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelLine);
            this.Controls.Add(this.labelMaxProb);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelMax);
            this.Controls.Add(this.labelMin);
            this.Name = "OffspringPossibilities";
            this.Size = new System.Drawing.Size(300, 200);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMin;
        private System.Windows.Forms.Label labelMax;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelMaxProb;
        private System.Windows.Forms.Panel panelLine;
    }
}
