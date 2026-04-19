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
            FlpBreedingPairs = new System.Windows.Forms.FlowLayoutPanel();
            LbTitle = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // FlpBreedingPairs
            // 
            FlpBreedingPairs.AutoScroll = true;
            FlpBreedingPairs.Dock = System.Windows.Forms.DockStyle.Fill;
            FlpBreedingPairs.Location = new System.Drawing.Point(0, 32);
            FlpBreedingPairs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            FlpBreedingPairs.Name = "FlpBreedingPairs";
            FlpBreedingPairs.Size = new System.Drawing.Size(698, 355);
            FlpBreedingPairs.TabIndex = 0;
            // 
            // LbTitle
            // 
            LbTitle.AutoSize = true;
            LbTitle.Dock = System.Windows.Forms.DockStyle.Top;
            LbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            LbTitle.Location = new System.Drawing.Point(0, 0);
            LbTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbTitle.Name = "LbTitle";
            LbTitle.Padding = new System.Windows.Forms.Padding(0, 0, 0, 12);
            LbTitle.Size = new System.Drawing.Size(188, 32);
            LbTitle.TabIndex = 1;
            LbTitle.Text = "Current breeding pairs";
            // 
            // CurrentBreeds
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(FlpBreedingPairs);
            Controls.Add(LbTitle);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "CurrentBreeds";
            Size = new System.Drawing.Size(698, 387);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FlpBreedingPairs;
        private System.Windows.Forms.Label LbTitle;
    }
}
