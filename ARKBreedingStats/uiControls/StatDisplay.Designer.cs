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
            labelName = new System.Windows.Forms.Label();
            labelWildLevel = new System.Windows.Forms.Label();
            panelBarWildLevels = new System.Windows.Forms.Panel();
            labelLevelDom = new System.Windows.Forms.Label();
            labelBreedingValue = new System.Windows.Forms.Label();
            labelDomValue = new System.Windows.Forms.Label();
            panelBarMutLevels = new System.Windows.Forms.Panel();
            labelMutLevel = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // labelName
            // 
            labelName.AutoSize = true;
            labelName.Location = new System.Drawing.Point(4, 0);
            labelName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelName.Name = "labelName";
            labelName.Size = new System.Drawing.Size(23, 15);
            labelName.TabIndex = 0;
            labelName.Text = "HP";
            // 
            // labelWildLevel
            // 
            labelWildLevel.Location = new System.Drawing.Point(20, 0);
            labelWildLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelWildLevel.Name = "labelWildLevel";
            labelWildLevel.Size = new System.Drawing.Size(29, 15);
            labelWildLevel.TabIndex = 1;
            labelWildLevel.Text = "100";
            labelWildLevel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panelBarWildLevels
            // 
            panelBarWildLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelBarWildLevels.Location = new System.Drawing.Point(0, 14);
            panelBarWildLevels.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelBarWildLevels.Name = "panelBarWildLevels";
            panelBarWildLevels.Size = new System.Drawing.Size(210, 5);
            panelBarWildLevels.TabIndex = 2;
            // 
            // labelLevelDom
            // 
            labelLevelDom.Location = new System.Drawing.Point(62, 0);
            labelLevelDom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelLevelDom.Name = "labelLevelDom";
            labelLevelDom.Size = new System.Drawing.Size(29, 15);
            labelLevelDom.TabIndex = 3;
            labelLevelDom.Text = "100";
            labelLevelDom.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelBreedingValue
            // 
            labelBreedingValue.BackColor = System.Drawing.Color.Transparent;
            labelBreedingValue.Location = new System.Drawing.Point(89, 0);
            labelBreedingValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelBreedingValue.Name = "labelBreedingValue";
            labelBreedingValue.Size = new System.Drawing.Size(58, 15);
            labelBreedingValue.TabIndex = 4;
            labelBreedingValue.Text = "100000";
            labelBreedingValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelDomValue
            // 
            labelDomValue.BackColor = System.Drawing.Color.Transparent;
            labelDomValue.Location = new System.Drawing.Point(141, 0);
            labelDomValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelDomValue.Name = "labelDomValue";
            labelDomValue.Size = new System.Drawing.Size(58, 15);
            labelDomValue.TabIndex = 5;
            labelDomValue.Text = "100000";
            labelDomValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panelBarMutLevels
            // 
            panelBarMutLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelBarMutLevels.Location = new System.Drawing.Point(0, 16);
            panelBarMutLevels.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelBarMutLevels.Name = "panelBarMutLevels";
            panelBarMutLevels.Size = new System.Drawing.Size(210, 3);
            panelBarMutLevels.TabIndex = 3;
            // 
            // labelMutLevel
            // 
            labelMutLevel.Location = new System.Drawing.Point(41, 0);
            labelMutLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelMutLevel.Name = "labelMutLevel";
            labelMutLevel.Size = new System.Drawing.Size(29, 15);
            labelMutLevel.TabIndex = 6;
            labelMutLevel.Text = "100";
            labelMutLevel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // StatDisplay
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panelBarMutLevels);
            Controls.Add(panelBarWildLevels);
            Controls.Add(labelName);
            Controls.Add(labelWildLevel);
            Controls.Add(labelMutLevel);
            Controls.Add(labelLevelDom);
            Controls.Add(labelBreedingValue);
            Controls.Add(labelDomValue);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "StatDisplay";
            Size = new System.Drawing.Size(211, 20);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelWildLevel;
        private System.Windows.Forms.Panel panelBarWildLevels;
        private System.Windows.Forms.Label labelLevelDom;
        private System.Windows.Forms.Label labelBreedingValue;
        private System.Windows.Forms.Label labelDomValue;
        private System.Windows.Forms.Panel panelBarMutLevels;
        private System.Windows.Forms.Label labelMutLevel;
    }
}
