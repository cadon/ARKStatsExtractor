namespace ARKBreedingStats.uiControls
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
            this.panelBarWildLevels = new System.Windows.Forms.Panel();
            this.labelLevelDom = new System.Windows.Forms.Label();
            this.labelBreedingValue = new System.Windows.Forms.Label();
            this.labelDomValue = new System.Windows.Forms.Label();
            this.panelBarDomLevels = new System.Windows.Forms.Panel();
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
            // panelBarWildLevels
            // 
            this.panelBarWildLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBarWildLevels.Location = new System.Drawing.Point(0, 14);
            this.panelBarWildLevels.Name = "panelBarWildLevels";
            this.panelBarWildLevels.Size = new System.Drawing.Size(175, 5);
            this.panelBarWildLevels.TabIndex = 2;
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
            // panelBarDomLevels
            // 
            this.panelBarDomLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBarDomLevels.Location = new System.Drawing.Point(0, 17);
            this.panelBarDomLevels.Name = "panelBarDomLevels";
            this.panelBarDomLevels.Size = new System.Drawing.Size(175, 3);
            this.panelBarDomLevels.TabIndex = 3;
            // 
            // StatDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelBarDomLevels);
            this.Controls.Add(this.labelDomValue);
            this.Controls.Add(this.labelBreedingValue);
            this.Controls.Add(this.labelLevelDom);
            this.Controls.Add(this.panelBarWildLevels);
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
        private System.Windows.Forms.Panel panelBarWildLevels;
        private System.Windows.Forms.Label labelLevelDom;
        private System.Windows.Forms.Label labelBreedingValue;
        private System.Windows.Forms.Label labelDomValue;
        private System.Windows.Forms.Panel panelBarDomLevels;
    }
}
