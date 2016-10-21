namespace ARKBreedingStats
{
    partial class BreedingInfo
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonHatching = new System.Windows.Forms.Button();
            this.labelBreedingInfos = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelBreedingInfos);
            this.groupBox1.Controls.Add(this.buttonHatching);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(444, 435);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Breeding";
            // 
            // buttonHatching
            // 
            this.buttonHatching.Location = new System.Drawing.Point(98, 220);
            this.buttonHatching.Name = "buttonHatching";
            this.buttonHatching.Size = new System.Drawing.Size(75, 23);
            this.buttonHatching.TabIndex = 0;
            this.buttonHatching.Text = "hatching";
            this.buttonHatching.UseVisualStyleBackColor = true;
            // 
            // labelBreedingInfos
            // 
            this.labelBreedingInfos.AutoSize = true;
            this.labelBreedingInfos.Location = new System.Drawing.Point(6, 16);
            this.labelBreedingInfos.Name = "labelBreedingInfos";
            this.labelBreedingInfos.Size = new System.Drawing.Size(72, 13);
            this.labelBreedingInfos.TabIndex = 1;
            this.labelBreedingInfos.Text = "BreedingInfos";
            // 
            // BreedingInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "BreedingInfo";
            this.Size = new System.Drawing.Size(444, 435);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonHatching;
        private System.Windows.Forms.Label labelBreedingInfos;
    }
}
