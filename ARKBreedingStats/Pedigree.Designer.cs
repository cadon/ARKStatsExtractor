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
            this.components = new System.ComponentModel.Container();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.labelEmptyInfo = new System.Windows.Forms.Label();
            this.pedigreeCreature1 = new ARKBreedingStats.PedigreeCreature();
            this.listViewCreatures = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(525, 180);
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
            this.labelEmptyInfo.Size = new System.Drawing.Size(1043, 520);
            this.labelEmptyInfo.TabIndex = 1;
            this.labelEmptyInfo.Text = "Select a creature in the Library to see its pedigree here.";
            this.labelEmptyInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pedigreeCreature1
            // 
            this.pedigreeCreature1.IsVirtual = false;
            this.pedigreeCreature1.Location = new System.Drawing.Point(525, 10);
            this.pedigreeCreature1.Name = "pedigreeCreature1";
            this.pedigreeCreature1.Size = new System.Drawing.Size(296, 35);
            this.pedigreeCreature1.TabIndex = 2;
            // 
            // listViewCreatures
            // 
            this.listViewCreatures.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewCreatures.Dock = System.Windows.Forms.DockStyle.Left;
            this.listViewCreatures.FullRowSelect = true;
            this.listViewCreatures.HideSelection = false;
            this.listViewCreatures.Location = new System.Drawing.Point(0, 0);
            this.listViewCreatures.MultiSelect = false;
            this.listViewCreatures.Name = "listViewCreatures";
            this.listViewCreatures.Size = new System.Drawing.Size(153, 520);
            this.listViewCreatures.TabIndex = 3;
            this.listViewCreatures.UseCompatibleStateImageBehavior = false;
            this.listViewCreatures.View = System.Windows.Forms.View.Details;
            this.listViewCreatures.SelectedIndexChanged += new System.EventHandler(this.listViewCreatures_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Lvl";
            this.columnHeader2.Width = 31;
            // 
            // Pedigree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.listViewCreatures);
            this.Controls.Add(this.pedigreeCreature1);
            this.Controls.Add(this.labelEmptyInfo);
            this.Controls.Add(this.pictureBox);
            this.Name = "Pedigree";
            this.Size = new System.Drawing.Size(1043, 520);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label labelEmptyInfo;
        private PedigreeCreature pedigreeCreature1;
        private System.Windows.Forms.ListView listViewCreatures;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}
