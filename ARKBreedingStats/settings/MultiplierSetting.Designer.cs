namespace ARKBreedingStats.settings
{
    partial class MultiplierSetting
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
            this.numericUpDownWildLevel = new ARKBreedingStats.uiControls.Nud();
            this.numericUpDownTameMult = new ARKBreedingStats.uiControls.Nud();
            this.numericUpDownTameAdd = new ARKBreedingStats.uiControls.Nud();
            this.numericUpDownDomLevel = new ARKBreedingStats.uiControls.Nud();
            this.labelStatName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWildLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTameMult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTameAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDomLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownWildLevel
            // 
            this.numericUpDownWildLevel.DecimalPlaces = 3;
            this.numericUpDownWildLevel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.numericUpDownWildLevel.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownWildLevel.Location = new System.Drawing.Point(65, 3);
            this.numericUpDownWildLevel.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownWildLevel.Name = "numericUpDownWildLevel";
            this.numericUpDownWildLevel.Size = new System.Drawing.Size(54, 20);
            this.numericUpDownWildLevel.TabIndex = 0;
            this.numericUpDownWildLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericUpDownTameMult
            // 
            this.numericUpDownTameMult.DecimalPlaces = 3;
            this.numericUpDownTameMult.ForeColor = System.Drawing.SystemColors.WindowText;
            this.numericUpDownTameMult.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownTameMult.Location = new System.Drawing.Point(245, 3);
            this.numericUpDownTameMult.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownTameMult.Name = "numericUpDownTameMult";
            this.numericUpDownTameMult.Size = new System.Drawing.Size(54, 20);
            this.numericUpDownTameMult.TabIndex = 3;
            this.numericUpDownTameMult.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericUpDownTameAdd
            // 
            this.numericUpDownTameAdd.DecimalPlaces = 3;
            this.numericUpDownTameAdd.ForeColor = System.Drawing.SystemColors.WindowText;
            this.numericUpDownTameAdd.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownTameAdd.Location = new System.Drawing.Point(185, 3);
            this.numericUpDownTameAdd.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownTameAdd.Name = "numericUpDownTameAdd";
            this.numericUpDownTameAdd.Size = new System.Drawing.Size(54, 20);
            this.numericUpDownTameAdd.TabIndex = 2;
            this.numericUpDownTameAdd.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericUpDownDomLevel
            // 
            this.numericUpDownDomLevel.DecimalPlaces = 3;
            this.numericUpDownDomLevel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.numericUpDownDomLevel.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownDomLevel.Location = new System.Drawing.Point(125, 3);
            this.numericUpDownDomLevel.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownDomLevel.Name = "numericUpDownDomLevel";
            this.numericUpDownDomLevel.Size = new System.Drawing.Size(54, 20);
            this.numericUpDownDomLevel.TabIndex = 1;
            this.numericUpDownDomLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labelStatName
            // 
            this.labelStatName.AutoSize = true;
            this.labelStatName.Location = new System.Drawing.Point(3, 5);
            this.labelStatName.Name = "labelStatName";
            this.labelStatName.Size = new System.Drawing.Size(26, 13);
            this.labelStatName.TabIndex = 4;
            this.labelStatName.Text = "Stat";
            // 
            // MultiplierSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelStatName);
            this.Controls.Add(this.numericUpDownWildLevel);
            this.Controls.Add(this.numericUpDownDomLevel);
            this.Controls.Add(this.numericUpDownTameMult);
            this.Controls.Add(this.numericUpDownTameAdd);
            this.Name = "MultiplierSetting";
            this.Size = new System.Drawing.Size(302, 26);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWildLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTameMult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTameAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDomLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ARKBreedingStats.uiControls.Nud numericUpDownWildLevel;
        private ARKBreedingStats.uiControls.Nud numericUpDownTameMult;
        private ARKBreedingStats.uiControls.Nud numericUpDownTameAdd;
        private ARKBreedingStats.uiControls.Nud numericUpDownDomLevel;
        private System.Windows.Forms.Label labelStatName;
    }
}
