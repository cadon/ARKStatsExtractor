namespace ARKBreedingStats
{
    partial class Pedigree
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelPedigreeInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelPedigreeInfo
            // 
            this.labelPedigreeInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPedigreeInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPedigreeInfo.Location = new System.Drawing.Point(0, 0);
            this.labelPedigreeInfo.Name = "labelPedigreeInfo";
            this.labelPedigreeInfo.Size = new System.Drawing.Size(601, 416);
            this.labelPedigreeInfo.TabIndex = 0;
            this.labelPedigreeInfo.Text = "Select a creature in the Library to see its pedigree here.";
            this.labelPedigreeInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Pedigree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.labelPedigreeInfo);
            this.Name = "Pedigree";
            this.Size = new System.Drawing.Size(601, 416);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelPedigreeInfo;
    }
}
