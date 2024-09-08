namespace ARKBreedingStats.StatsOptions.LevelColorSettings
{
    partial class StatLevelGraphOptionsControl
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
            this.LbStatName = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.CbUseDifferentColorsForOddLevels = new System.Windows.Forms.CheckBox();
            this.hueControlOdd = new HueControl();
            this.hueControl = new HueControl();
            this.CbOverrideGraphSettings = new System.Windows.Forms.CheckBox();
            this.HueControlMutations = new HueControl();
            this.CbUseDifferentColorsForMutationLevels = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // LbStatName
            // 
            this.LbStatName.AutoSize = true;
            this.LbStatName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbStatName.Location = new System.Drawing.Point(3, 3);
            this.LbStatName.Name = "LbStatName";
            this.LbStatName.Size = new System.Drawing.Size(61, 24);
            this.LbStatName.TabIndex = 0;
            this.LbStatName.Text = "[0] HP";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gray;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1362, 1);
            this.panel1.TabIndex = 7;
            // 
            // CbUseDifferentColorsForOddLevels
            // 
            this.CbUseDifferentColorsForOddLevels.AutoSize = true;
            this.CbUseDifferentColorsForOddLevels.Enabled = false;
            this.CbUseDifferentColorsForOddLevels.Location = new System.Drawing.Point(531, 8);
            this.CbUseDifferentColorsForOddLevels.Name = "CbUseDifferentColorsForOddLevels";
            this.CbUseDifferentColorsForOddLevels.Size = new System.Drawing.Size(44, 17);
            this.CbUseDifferentColorsForOddLevels.TabIndex = 8;
            this.CbUseDifferentColorsForOddLevels.Text = "odd";
            this.CbUseDifferentColorsForOddLevels.UseVisualStyleBackColor = true;
            this.CbUseDifferentColorsForOddLevels.CheckedChanged += new System.EventHandler(this.CbUseDifferentColorsForOddLevels_CheckedChanged);
            // 
            // hueControlOdd
            // 
            this.hueControlOdd.Location = new System.Drawing.Point(581, 3);
            this.hueControlOdd.Name = "hueControlOdd";
            this.hueControlOdd.Size = new System.Drawing.Size(347, 24);
            this.hueControlOdd.TabIndex = 10;
            // 
            // hueControl
            // 
            this.hueControl.Location = new System.Drawing.Point(175, 3);
            this.hueControl.Name = "hueControl";
            this.hueControl.Size = new System.Drawing.Size(350, 24);
            this.hueControl.TabIndex = 9;
            // 
            // CbOverrideGraphSettings
            // 
            this.CbOverrideGraphSettings.AutoSize = true;
            this.CbOverrideGraphSettings.Location = new System.Drawing.Point(70, 10);
            this.CbOverrideGraphSettings.Name = "CbOverrideGraphSettings";
            this.CbOverrideGraphSettings.Size = new System.Drawing.Size(99, 17);
            this.CbOverrideGraphSettings.TabIndex = 11;
            this.CbOverrideGraphSettings.Text = "Override parent";
            this.CbOverrideGraphSettings.UseVisualStyleBackColor = true;
            this.CbOverrideGraphSettings.CheckedChanged += new System.EventHandler(this.CbOverrideGraphSettings_CheckedChanged);
            // 
            // HueControlMutations
            // 
            this.HueControlMutations.Location = new System.Drawing.Point(1011, 3);
            this.HueControlMutations.Name = "HueControlMutations";
            this.HueControlMutations.Size = new System.Drawing.Size(347, 24);
            this.HueControlMutations.TabIndex = 13;
            // 
            // CbUseDifferentColorsForMutationLevels
            // 
            this.CbUseDifferentColorsForMutationLevels.AutoSize = true;
            this.CbUseDifferentColorsForMutationLevels.Enabled = false;
            this.CbUseDifferentColorsForMutationLevels.Location = new System.Drawing.Point(934, 8);
            this.CbUseDifferentColorsForMutationLevels.Name = "CbUseDifferentColorsForMutationLevels";
            this.CbUseDifferentColorsForMutationLevels.Size = new System.Drawing.Size(71, 17);
            this.CbUseDifferentColorsForMutationLevels.TabIndex = 12;
            this.CbUseDifferentColorsForMutationLevels.Text = "mutations";
            this.CbUseDifferentColorsForMutationLevels.UseVisualStyleBackColor = true;
            this.CbUseDifferentColorsForMutationLevels.CheckedChanged += new System.EventHandler(this.CbUseDifferentColorsForMutationLevels_CheckedChanged);
            // 
            // StatLevelGraphOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.HueControlMutations);
            this.Controls.Add(this.CbUseDifferentColorsForMutationLevels);
            this.Controls.Add(this.CbOverrideGraphSettings);
            this.Controls.Add(this.hueControlOdd);
            this.Controls.Add(this.hueControl);
            this.Controls.Add(this.CbUseDifferentColorsForOddLevels);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.LbStatName);
            this.Name = "StatLevelGraphOptionsControl";
            this.Size = new System.Drawing.Size(1362, 29);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LbStatName;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox CbUseDifferentColorsForOddLevels;
        private HueControl hueControl;
        private HueControl hueControlOdd;
        private System.Windows.Forms.CheckBox CbOverrideGraphSettings;
        private HueControl HueControlMutations;
        private System.Windows.Forms.CheckBox CbUseDifferentColorsForMutationLevels;
    }
}
