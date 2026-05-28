using ARKBreedingStats.uiControls;

namespace ARKBreedingStats.SpeciesOptions.LevelColorSettings
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
            CbReverseGradient = new System.Windows.Forms.CheckBox();
            PbColorGradient = new System.Windows.Forms.PictureBox();
            BtColorHigh = new System.Windows.Forms.Button();
            BtColorLow = new System.Windows.Forms.Button();
            colorDialog1 = new System.Windows.Forms.ColorDialog();
            NudLevelHigh = new Nud();
            NudLevelLow = new Nud();
            ((System.ComponentModel.ISupportInitialize)PbColorGradient).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NudLevelHigh).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NudLevelLow).BeginInit();
            SuspendLayout();
            // 
            // CbReverseGradient
            // 
            CbReverseGradient.AutoSize = true;
            CbReverseGradient.Location = new System.Drawing.Point(335, 5);
            CbReverseGradient.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CbReverseGradient.Name = "CbReverseGradient";
            CbReverseGradient.Size = new System.Drawing.Size(65, 19);
            CbReverseGradient.TabIndex = 12;
            CbReverseGradient.Text = "rev hue";
            CbReverseGradient.UseVisualStyleBackColor = true;
            CbReverseGradient.CheckedChanged += CbReverseGradient_CheckedChanged;
            // 
            // PbColorGradient
            // 
            PbColorGradient.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            PbColorGradient.Location = new System.Drawing.Point(107, 1);
            PbColorGradient.Margin = new System.Windows.Forms.Padding(0);
            PbColorGradient.Name = "PbColorGradient";
            PbColorGradient.Size = new System.Drawing.Size(116, 26);
            PbColorGradient.TabIndex = 9;
            PbColorGradient.TabStop = false;
            PbColorGradient.MouseDown += PbColorGradient_MouseDown;
            // 
            // BtColorHigh
            // 
            BtColorHigh.FlatAppearance.BorderColor = System.Drawing.SystemColors.WindowText;
            BtColorHigh.FlatAppearance.BorderSize = 2;
            BtColorHigh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            BtColorHigh.Location = new System.Drawing.Point(224, 1);
            BtColorHigh.Margin = new System.Windows.Forms.Padding(4);
            BtColorHigh.Name = "BtColorHigh";
            BtColorHigh.Size = new System.Drawing.Size(44, 25);
            BtColorHigh.TabIndex = 8;
            BtColorHigh.UseVisualStyleBackColor = true;
            BtColorHigh.Click += BtColorClick;
            // 
            // BtColorLow
            // 
            BtColorLow.FlatAppearance.BorderColor = System.Drawing.SystemColors.WindowText;
            BtColorLow.FlatAppearance.BorderSize = 2;
            BtColorLow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            BtColorLow.Location = new System.Drawing.Point(62, 1);
            BtColorLow.Margin = new System.Windows.Forms.Padding(4);
            BtColorLow.Name = "BtColorLow";
            BtColorLow.Size = new System.Drawing.Size(44, 25);
            BtColorLow.TabIndex = 7;
            BtColorLow.UseVisualStyleBackColor = true;
            BtColorLow.Click += BtColorClick;
            // 
            // NudLevelHigh
            // 
            NudLevelHigh.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            NudLevelHigh.Location = new System.Drawing.Point(275, 3);
            NudLevelHigh.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NudLevelHigh.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            NudLevelHigh.Name = "NudLevelHigh";
            NudLevelHigh.Size = new System.Drawing.Size(52, 23);
            NudLevelHigh.TabIndex = 11;
            NudLevelHigh.ValueChanged += NudLevelValueChanged;
            // 
            // NudLevelLow
            // 
            NudLevelLow.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            NudLevelLow.Location = new System.Drawing.Point(4, 3);
            NudLevelLow.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NudLevelLow.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            NudLevelLow.Name = "NudLevelLow";
            NudLevelLow.Size = new System.Drawing.Size(52, 23);
            NudLevelLow.TabIndex = 10;
            NudLevelLow.ValueChanged += NudLevelValueChanged;
            // 
            // HueControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(CbReverseGradient);
            Controls.Add(NudLevelHigh);
            Controls.Add(NudLevelLow);
            Controls.Add(PbColorGradient);
            Controls.Add(BtColorHigh);
            Controls.Add(BtColorLow);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "HueControl";
            Size = new System.Drawing.Size(410, 28);
            ((System.ComponentModel.ISupportInitialize)PbColorGradient).EndInit();
            ((System.ComponentModel.ISupportInitialize)NudLevelHigh).EndInit();
            ((System.ComponentModel.ISupportInitialize)NudLevelLow).EndInit();
            ResumeLayout(false);
            PerformLayout();

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
