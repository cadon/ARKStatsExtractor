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
            this.LbTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // FlpBreedingPairs
            // 
            this.FlpBreedingPairs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FlpBreedingPairs.Location = new System.Drawing.Point(0, 30);
            this.FlpBreedingPairs.Name = "FlpBreedingPairs";
            this.FlpBreedingPairs.Size = new System.Drawing.Size(598, 305);
            this.FlpBreedingPairs.TabIndex = 0;
            // 
            // LbTitle
            // 
            this.LbTitle.AutoSize = true;
            this.LbTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.LbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbTitle.Location = new System.Drawing.Point(0, 0);
            this.LbTitle.Name = "LbTitle";
            this.LbTitle.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.LbTitle.Size = new System.Drawing.Size(188, 30);
            this.LbTitle.TabIndex = 1;
            this.LbTitle.Text = "Current breeding pairs";
            // 
            // CurrentBreeds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FlpBreedingPairs);
            this.Controls.Add(this.LbTitle);
            this.Name = "CurrentBreeds";
            this.Size = new System.Drawing.Size(598, 335);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FlpBreedingPairs;
        private System.Windows.Forms.Label LbTitle;
    }
}
