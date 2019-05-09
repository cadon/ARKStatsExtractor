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
            this.nudWildLevel = new ARKBreedingStats.uiControls.Nud();
            this.nudTameMult = new ARKBreedingStats.uiControls.Nud();
            this.nudTameAdd = new ARKBreedingStats.uiControls.Nud();
            this.nudDomLevel = new ARKBreedingStats.uiControls.Nud();
            this.labelStatName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudWildLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTameMult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTameAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDomLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // nudWildLevel
            // 
            this.nudWildLevel.DecimalPlaces = 3;
            this.nudWildLevel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nudWildLevel.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudWildLevel.Location = new System.Drawing.Point(133, 3);
            this.nudWildLevel.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudWildLevel.Name = "nudWildLevel";
            this.nudWildLevel.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudWildLevel.Size = new System.Drawing.Size(54, 20);
            this.nudWildLevel.TabIndex = 0;
            this.nudWildLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudTameMult
            // 
            this.nudTameMult.DecimalPlaces = 3;
            this.nudTameMult.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nudTameMult.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudTameMult.Location = new System.Drawing.Point(313, 3);
            this.nudTameMult.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudTameMult.Name = "nudTameMult";
            this.nudTameMult.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudTameMult.Size = new System.Drawing.Size(54, 20);
            this.nudTameMult.TabIndex = 3;
            this.nudTameMult.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudTameAdd
            // 
            this.nudTameAdd.DecimalPlaces = 3;
            this.nudTameAdd.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nudTameAdd.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudTameAdd.Location = new System.Drawing.Point(253, 3);
            this.nudTameAdd.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudTameAdd.Name = "nudTameAdd";
            this.nudTameAdd.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudTameAdd.Size = new System.Drawing.Size(54, 20);
            this.nudTameAdd.TabIndex = 2;
            this.nudTameAdd.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudDomLevel
            // 
            this.nudDomLevel.DecimalPlaces = 3;
            this.nudDomLevel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nudDomLevel.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudDomLevel.Location = new System.Drawing.Point(193, 3);
            this.nudDomLevel.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudDomLevel.Name = "nudDomLevel";
            this.nudDomLevel.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudDomLevel.Size = new System.Drawing.Size(54, 20);
            this.nudDomLevel.TabIndex = 1;
            this.nudDomLevel.Value = new decimal(new int[] {
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
            this.Controls.Add(this.nudWildLevel);
            this.Controls.Add(this.nudDomLevel);
            this.Controls.Add(this.nudTameMult);
            this.Controls.Add(this.nudTameAdd);
            this.Controls.Add(this.labelStatName);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "MultiplierSetting";
            this.Size = new System.Drawing.Size(372, 26);
            ((System.ComponentModel.ISupportInitialize)(this.nudWildLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTameMult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTameAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDomLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ARKBreedingStats.uiControls.Nud nudWildLevel;
        private ARKBreedingStats.uiControls.Nud nudTameMult;
        private ARKBreedingStats.uiControls.Nud nudTameAdd;
        private ARKBreedingStats.uiControls.Nud nudDomLevel;
        private System.Windows.Forms.Label labelStatName;
    }
}
