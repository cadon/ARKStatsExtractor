namespace ARKBreedingStats.uiControls
{
    partial class MultiSetterTag
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
            this.cbConsider = new System.Windows.Forms.CheckBox();
            this.cbTagChecked = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbConsider
            // 
            this.cbConsider.AutoSize = true;
            this.cbConsider.Location = new System.Drawing.Point(3, 4);
            this.cbConsider.Name = "cbConsider";
            this.cbConsider.Size = new System.Drawing.Size(15, 14);
            this.cbConsider.TabIndex = 0;
            this.cbConsider.UseVisualStyleBackColor = true;
            this.cbConsider.CheckedChanged += new System.EventHandler(this.cbConsider_CheckedChanged);
            // 
            // cbTagChecked
            // 
            this.cbTagChecked.AutoSize = true;
            this.cbTagChecked.Location = new System.Drawing.Point(24, 3);
            this.cbTagChecked.Name = "cbTagChecked";
            this.cbTagChecked.Size = new System.Drawing.Size(15, 14);
            this.cbTagChecked.TabIndex = 1;
            this.cbTagChecked.UseVisualStyleBackColor = true;
            this.cbTagChecked.CheckedChanged += new System.EventHandler(this.cbTagChecked_CheckedChanged);
            // 
            // MultiSetterTag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbTagChecked);
            this.Controls.Add(this.cbConsider);
            this.Name = "MultiSetterTag";
            this.Size = new System.Drawing.Size(202, 19);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbConsider;
        private System.Windows.Forms.CheckBox cbTagChecked;
    }
}
