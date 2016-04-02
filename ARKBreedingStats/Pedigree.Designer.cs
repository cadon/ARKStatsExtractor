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
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.labelEmptyInfo = new System.Windows.Forms.Label();
            this.pedigreeCreature1 = new ARKBreedingStats.PedigreeCreature();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(278, 180);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(256, 256);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // labelEmptyInfo
            // 
            this.labelEmptyInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelEmptyInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEmptyInfo.Location = new System.Drawing.Point(0, 0);
            this.labelEmptyInfo.Name = "labelEmptyInfo";
            this.labelEmptyInfo.Size = new System.Drawing.Size(534, 436);
            this.labelEmptyInfo.TabIndex = 1;
            this.labelEmptyInfo.Text = "Select a creature in the Library to see its pedigree here.";
            this.labelEmptyInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pedigreeCreature1
            // 
            this.pedigreeCreature1.Location = new System.Drawing.Point(278, 10);
            this.pedigreeCreature1.Name = "pedigreeCreature1";
            this.pedigreeCreature1.Size = new System.Drawing.Size(249, 35);
            this.pedigreeCreature1.TabIndex = 2;
            // 
            // Pedigree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.pedigreeCreature1);
            this.Controls.Add(this.labelEmptyInfo);
            this.Controls.Add(this.pictureBox);
            this.Name = "Pedigree";
            this.Size = new System.Drawing.Size(517, 342);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label labelEmptyInfo;
        private PedigreeCreature pedigreeCreature1;
    }
}
