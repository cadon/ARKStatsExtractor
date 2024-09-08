using ARKBreedingStats.uiControls;

namespace ARKBreedingStats.StatsOptions.LevelColorSettings
{
    partial class HueControl
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
            this.CbReverseGradient = new System.Windows.Forms.CheckBox();
            this.PbColorGradient = new System.Windows.Forms.PictureBox();
            this.BtColorHigh = new System.Windows.Forms.Button();
            this.BtColorLow = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.NudLevelHigh = new ARKBreedingStats.uiControls.Nud();
            this.NudLevelLow = new ARKBreedingStats.uiControls.Nud();
            ((System.ComponentModel.ISupportInitialize)(this.PbColorGradient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudLevelHigh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudLevelLow)).BeginInit();
            this.SuspendLayout();
            // 
            // CbReverseGradient
            // 
            this.CbReverseGradient.AutoSize = true;
            this.CbReverseGradient.Location = new System.Drawing.Point(287, 4);
            this.CbReverseGradient.Name = "CbReverseGradient";
            this.CbReverseGradient.Size = new System.Drawing.Size(62, 17);
            this.CbReverseGradient.TabIndex = 12;
            this.CbReverseGradient.Text = "rev hue";
            this.CbReverseGradient.UseVisualStyleBackColor = true;
            this.CbReverseGradient.CheckedChanged += new System.EventHandler(this.CbReverseGradient_CheckedChanged);
            // 
            // PbColorGradient
            // 
            this.PbColorGradient.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PbColorGradient.Location = new System.Drawing.Point(92, 0);
            this.PbColorGradient.Margin = new System.Windows.Forms.Padding(0);
            this.PbColorGradient.Name = "PbColorGradient";
            this.PbColorGradient.Size = new System.Drawing.Size(100, 23);
            this.PbColorGradient.TabIndex = 9;
            this.PbColorGradient.TabStop = false;
            this.PbColorGradient.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PbColorGradient_MouseDown);
            // 
            // BtColorHigh
            // 
            this.BtColorHigh.Location = new System.Drawing.Point(192, 0);
            this.BtColorHigh.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.BtColorHigh.Name = "BtColorHigh";
            this.BtColorHigh.Size = new System.Drawing.Size(38, 23);
            this.BtColorHigh.TabIndex = 8;
            this.BtColorHigh.UseVisualStyleBackColor = true;
            this.BtColorHigh.Click += new System.EventHandler(this.BtColorClick);
            // 
            // BtColorLow
            // 
            this.BtColorLow.Location = new System.Drawing.Point(54, 0);
            this.BtColorLow.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.BtColorLow.Name = "BtColorLow";
            this.BtColorLow.Size = new System.Drawing.Size(38, 23);
            this.BtColorLow.TabIndex = 7;
            this.BtColorLow.UseVisualStyleBackColor = true;
            this.BtColorLow.Click += new System.EventHandler(this.BtColorClick);
            // 
            // NudLevelHigh
            // 
            this.NudLevelHigh.ForeColor = System.Drawing.SystemColors.GrayText;
            this.NudLevelHigh.Location = new System.Drawing.Point(236, 3);
            this.NudLevelHigh.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NudLevelHigh.Name = "NudLevelHigh";
            this.NudLevelHigh.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.NudLevelHigh.Size = new System.Drawing.Size(45, 20);
            this.NudLevelHigh.TabIndex = 11;
            this.NudLevelHigh.ValueChanged += new System.EventHandler(this.NudLevelValueChanged);
            // 
            // NudLevelLow
            // 
            this.NudLevelLow.ForeColor = System.Drawing.SystemColors.GrayText;
            this.NudLevelLow.Location = new System.Drawing.Point(3, 3);
            this.NudLevelLow.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NudLevelLow.Name = "NudLevelLow";
            this.NudLevelLow.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.NudLevelLow.Size = new System.Drawing.Size(45, 20);
            this.NudLevelLow.TabIndex = 10;
            this.NudLevelLow.ValueChanged += new System.EventHandler(this.NudLevelValueChanged);
            // 
            // HueControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CbReverseGradient);
            this.Controls.Add(this.NudLevelHigh);
            this.Controls.Add(this.NudLevelLow);
            this.Controls.Add(this.PbColorGradient);
            this.Controls.Add(this.BtColorHigh);
            this.Controls.Add(this.BtColorLow);
            this.Name = "HueControl";
            this.Size = new System.Drawing.Size(351, 24);
            ((System.ComponentModel.ISupportInitialize)(this.PbColorGradient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudLevelHigh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudLevelLow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox CbReverseGradient;
        private Nud NudLevelHigh;
        private Nud NudLevelLow;
        private System.Windows.Forms.PictureBox PbColorGradient;
        private System.Windows.Forms.Button BtColorHigh;
        private System.Windows.Forms.Button BtColorLow;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}
