namespace ARKBreedingStats.uiControls
{
    partial class StatPotential
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
            this.labelDomLevels = new System.Windows.Forms.Label();
            this.labelImprinting = new System.Windows.Forms.Label();
            this.labelWildLevels = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelDomLevels
            // 
            this.labelDomLevels.BackColor = System.Drawing.Color.Gold;
            this.labelDomLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelDomLevels.Location = new System.Drawing.Point(286, 0);
            this.labelDomLevels.Name = "labelDomLevels";
            this.labelDomLevels.Size = new System.Drawing.Size(92, 23);
            this.labelDomLevels.TabIndex = 3;
            this.labelDomLevels.Text = "label3";
            this.labelDomLevels.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelImprinting
            // 
            this.labelImprinting.BackColor = System.Drawing.Color.SkyBlue;
            this.labelImprinting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelImprinting.Location = new System.Drawing.Point(165, 0);
            this.labelImprinting.Name = "labelImprinting";
            this.labelImprinting.Size = new System.Drawing.Size(115, 23);
            this.labelImprinting.TabIndex = 2;
            this.labelImprinting.Text = "label2";
            this.labelImprinting.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelWildLevels
            // 
            this.labelWildLevels.BackColor = System.Drawing.Color.LightCoral;
            this.labelWildLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelWildLevels.Location = new System.Drawing.Point(31, 0);
            this.labelWildLevels.Name = "labelWildLevels";
            this.labelWildLevels.Size = new System.Drawing.Size(128, 23);
            this.labelWildLevels.TabIndex = 1;
            this.labelWildLevels.Text = "label1";
            this.labelWildLevels.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "HP";
            // 
            // StatPotential
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelDomLevels);
            this.Controls.Add(this.labelImprinting);
            this.Controls.Add(this.labelWildLevels);
            this.Name = "StatPotential";
            this.Size = new System.Drawing.Size(386, 24);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelWildLevels;
        private System.Windows.Forms.Label labelDomLevels;
        private System.Windows.Forms.Label labelImprinting;
        private System.Windows.Forms.Label label1;
    }
}
