namespace ARKBreedingStats.uiControls
{
    partial class StatPotential
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
            labelDomLevels = new System.Windows.Forms.Label();
            labelImprinting = new System.Windows.Forms.Label();
            labelWildLevels = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // labelDomLevels
            // 
            labelDomLevels.BackColor = System.Drawing.Color.Gold;
            labelDomLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            labelDomLevels.Dock = System.Windows.Forms.DockStyle.Left;
            labelDomLevels.Location = new System.Drawing.Point(232, 0);
            labelDomLevels.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelDomLevels.Name = "labelDomLevels";
            labelDomLevels.Size = new System.Drawing.Size(86, 28);
            labelDomLevels.TabIndex = 3;
            labelDomLevels.Text = "label3";
            labelDomLevels.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelImprinting
            // 
            labelImprinting.BackColor = System.Drawing.Color.SkyBlue;
            labelImprinting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            labelImprinting.Dock = System.Windows.Forms.DockStyle.Left;
            labelImprinting.Location = new System.Drawing.Point(151, 0);
            labelImprinting.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelImprinting.Name = "labelImprinting";
            labelImprinting.Size = new System.Drawing.Size(81, 28);
            labelImprinting.TabIndex = 2;
            labelImprinting.Text = "label2";
            labelImprinting.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelWildLevels
            // 
            labelWildLevels.BackColor = System.Drawing.Color.LightCoral;
            labelWildLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            labelWildLevels.Dock = System.Windows.Forms.DockStyle.Left;
            labelWildLevels.Location = new System.Drawing.Point(40, 0);
            labelWildLevels.Name = "labelWildLevels";
            labelWildLevels.Size = new System.Drawing.Size(111, 28);
            labelWildLevels.TabIndex = 1;
            labelWildLevels.Text = "label1";
            labelWildLevels.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            label1.Dock = System.Windows.Forms.DockStyle.Left;
            label1.Location = new System.Drawing.Point(0, 0);
            label1.Name = "label1";
            label1.Padding = new System.Windows.Forms.Padding(5);
            label1.Size = new System.Drawing.Size(40, 28);
            label1.TabIndex = 4;
            label1.Text = "HP";
            // 
            // StatPotential
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(labelDomLevels);
            Controls.Add(labelImprinting);
            Controls.Add(labelWildLevels);
            Controls.Add(label1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "StatPotential";
            Size = new System.Drawing.Size(320, 28);
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelWildLevels;
        private System.Windows.Forms.Label labelDomLevels;
        private System.Windows.Forms.Label labelImprinting;
        private System.Windows.Forms.Label label1;
    }
}
