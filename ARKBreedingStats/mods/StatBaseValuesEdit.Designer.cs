namespace ARKBreedingStats.mods
{
    partial class StatBaseValuesEdit
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
            this.cbOverride = new System.Windows.Forms.CheckBox();
            this.cbImprintingOverride = new System.Windows.Forms.CheckBox();
            this.nudImprintingOverride = new ARKBreedingStats.uiControls.Nud();
            this.nudTm = new ARKBreedingStats.uiControls.Nud();
            this.nudTa = new ARKBreedingStats.uiControls.Nud();
            this.nudId = new ARKBreedingStats.uiControls.Nud();
            this.nudIw = new ARKBreedingStats.uiControls.Nud();
            this.nudBase = new ARKBreedingStats.uiControls.Nud();
            ((System.ComponentModel.ISupportInitialize)(this.nudImprintingOverride)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTa)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBase)).BeginInit();
            this.SuspendLayout();
            // 
            // cbOverride
            // 
            this.cbOverride.AutoSize = true;
            this.cbOverride.Location = new System.Drawing.Point(3, 4);
            this.cbOverride.Name = "cbOverride";
            this.cbOverride.Size = new System.Drawing.Size(64, 17);
            this.cbOverride.TabIndex = 5;
            this.cbOverride.Text = "override";
            this.cbOverride.UseVisualStyleBackColor = true;
            this.cbOverride.CheckedChanged += new System.EventHandler(this.cbOverride_CheckedChanged);
            // 
            // cbImprintingOverride
            // 
            this.cbImprintingOverride.AutoSize = true;
            this.cbImprintingOverride.Location = new System.Drawing.Point(666, 5);
            this.cbImprintingOverride.Name = "cbImprintingOverride";
            this.cbImprintingOverride.Size = new System.Drawing.Size(15, 14);
            this.cbImprintingOverride.TabIndex = 6;
            this.cbImprintingOverride.UseVisualStyleBackColor = true;
            this.cbImprintingOverride.CheckedChanged += new System.EventHandler(this.cbImprintingOverride_CheckedChanged);
            // 
            // nudImprintingOverride
            // 
            this.nudImprintingOverride.DecimalPlaces = 3;
            this.nudImprintingOverride.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudImprintingOverride.Location = new System.Drawing.Point(687, 3);
            this.nudImprintingOverride.Name = "nudImprintingOverride";
            this.nudImprintingOverride.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudImprintingOverride.Size = new System.Drawing.Size(52, 20);
            this.nudImprintingOverride.TabIndex = 7;
            // 
            // nudTm
            // 
            this.nudTm.DecimalPlaces = 5;
            this.nudTm.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudTm.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudTm.Location = new System.Drawing.Point(538, 3);
            this.nudTm.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudTm.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.nudTm.Name = "nudTm";
            this.nudTm.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudTm.Size = new System.Drawing.Size(94, 20);
            this.nudTm.TabIndex = 4;
            // 
            // nudTa
            // 
            this.nudTa.DecimalPlaces = 5;
            this.nudTa.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudTa.Location = new System.Drawing.Point(438, 3);
            this.nudTa.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudTa.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.nudTa.Name = "nudTa";
            this.nudTa.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudTa.Size = new System.Drawing.Size(94, 20);
            this.nudTa.TabIndex = 3;
            // 
            // nudId
            // 
            this.nudId.DecimalPlaces = 5;
            this.nudId.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudId.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudId.Location = new System.Drawing.Point(338, 3);
            this.nudId.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudId.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nudId.Name = "nudId";
            this.nudId.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudId.Size = new System.Drawing.Size(94, 20);
            this.nudId.TabIndex = 2;
            // 
            // nudIw
            // 
            this.nudIw.DecimalPlaces = 5;
            this.nudIw.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudIw.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudIw.Location = new System.Drawing.Point(238, 3);
            this.nudIw.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudIw.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nudIw.Name = "nudIw";
            this.nudIw.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudIw.Size = new System.Drawing.Size(94, 20);
            this.nudIw.TabIndex = 1;
            // 
            // nudBase
            // 
            this.nudBase.DecimalPlaces = 5;
            this.nudBase.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudBase.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudBase.Location = new System.Drawing.Point(138, 3);
            this.nudBase.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudBase.Name = "nudBase";
            this.nudBase.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudBase.Size = new System.Drawing.Size(94, 20);
            this.nudBase.TabIndex = 0;
            // 
            // StatBaseValuesEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.nudImprintingOverride);
            this.Controls.Add(this.cbImprintingOverride);
            this.Controls.Add(this.nudTm);
            this.Controls.Add(this.nudTa);
            this.Controls.Add(this.nudId);
            this.Controls.Add(this.nudIw);
            this.Controls.Add(this.nudBase);
            this.Controls.Add(this.cbOverride);
            this.Name = "StatBaseValuesEdit";
            this.Size = new System.Drawing.Size(742, 26);
            ((System.ComponentModel.ISupportInitialize)(this.nudImprintingOverride)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTa)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBase)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private uiControls.Nud nudBase;
        private uiControls.Nud nudIw;
        private uiControls.Nud nudId;
        private uiControls.Nud nudTa;
        private uiControls.Nud nudTm;
        private System.Windows.Forms.CheckBox cbOverride;
        private System.Windows.Forms.CheckBox cbImprintingOverride;
        private uiControls.Nud nudImprintingOverride;
    }
}
