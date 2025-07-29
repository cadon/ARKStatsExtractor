namespace ARKBreedingStats.uiControls
{
    partial class CurrentBreeds
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
            this.FlpBreedingPairs = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // FlpBreedingPairs
            // 
            this.FlpBreedingPairs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FlpBreedingPairs.Location = new System.Drawing.Point(0, 13);
            this.FlpBreedingPairs.Name = "FlpBreedingPairs";
            this.FlpBreedingPairs.Size = new System.Drawing.Size(598, 322);
            this.FlpBreedingPairs.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Current breeding pairs";
            // 
            // CurrentBreeds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FlpBreedingPairs);
            this.Controls.Add(this.label1);
            this.Name = "CurrentBreeds";
            this.Size = new System.Drawing.Size(598, 335);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FlpBreedingPairs;
        private System.Windows.Forms.Label label1;
    }
}
