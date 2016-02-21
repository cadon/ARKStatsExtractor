namespace ARKBreedingStats
{
    partial class StatDisplay
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
            this.labelName = new System.Windows.Forms.Label();
            this.labelWildLevel = new System.Windows.Forms.Label();
            this.panelBar = new System.Windows.Forms.Panel();
            this.labelLevelDom = new System.Windows.Forms.Label();
            this.labelBreedingValue = new System.Windows.Forms.Label();
            this.labelDomValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(3, 0);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(22, 13);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "HP";
            // 
            // labelWildLevel
            // 
            this.labelWildLevel.AutoSize = true;
            this.labelWildLevel.Location = new System.Drawing.Point(26, 0);
            this.labelWildLevel.Name = "labelWildLevel";
            this.labelWildLevel.Size = new System.Drawing.Size(25, 13);
            this.labelWildLevel.TabIndex = 1;
            this.labelWildLevel.Text = "100";
            // 
            // panelBar
            // 
            this.panelBar.Location = new System.Drawing.Point(0, 14);
            this.panelBar.Name = "panelBar";
            this.panelBar.Size = new System.Drawing.Size(175, 5);
            this.panelBar.TabIndex = 2;
            // 
            // labelLevelDom
            // 
            this.labelLevelDom.AutoSize = true;
            this.labelLevelDom.Location = new System.Drawing.Point(54, 0);
            this.labelLevelDom.Name = "labelLevelDom";
            this.labelLevelDom.Size = new System.Drawing.Size(25, 13);
            this.labelLevelDom.TabIndex = 3;
            this.labelLevelDom.Text = "100";
            // 
            // labelBreedingValue
            // 
            this.labelBreedingValue.AutoSize = true;
            this.labelBreedingValue.Location = new System.Drawing.Point(82, 0);
            this.labelBreedingValue.Name = "labelBreedingValue";
            this.labelBreedingValue.Size = new System.Drawing.Size(43, 13);
            this.labelBreedingValue.TabIndex = 4;
            this.labelBreedingValue.Text = "100000";
            // 
            // labelDomValue
            // 
            this.labelDomValue.AutoSize = true;
            this.labelDomValue.Location = new System.Drawing.Point(132, 0);
            this.labelDomValue.Name = "labelDomValue";
            this.labelDomValue.Size = new System.Drawing.Size(43, 13);
            this.labelDomValue.TabIndex = 5;
            this.labelDomValue.Text = "100000";
            // 
            // StatDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelDomValue);
            this.Controls.Add(this.labelBreedingValue);
            this.Controls.Add(this.labelLevelDom);
            this.Controls.Add(this.panelBar);
            this.Controls.Add(this.labelWildLevel);
            this.Controls.Add(this.labelName);
            this.Name = "StatDisplay";
            this.Size = new System.Drawing.Size(183, 20);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelWildLevel;
        private System.Windows.Forms.Panel panelBar;
        private System.Windows.Forms.Label labelLevelDom;
        private System.Windows.Forms.Label labelBreedingValue;
        private System.Windows.Forms.Label labelDomValue;
    }
}
