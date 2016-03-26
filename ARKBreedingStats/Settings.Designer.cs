namespace ARKBreedingStats
{
    partial class Settings
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxMultiplier = new System.Windows.Forms.GroupBox();
            this.buttonAllToOne = new System.Windows.Forms.Button();
            this.buttonSetToOfficial = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.multiplierSettingTo = new ARKBreedingStats.MultiplierSetting();
            this.multiplierSettingSp = new ARKBreedingStats.MultiplierSetting();
            this.multiplierSettingDm = new ARKBreedingStats.MultiplierSetting();
            this.multiplierSettingWe = new ARKBreedingStats.MultiplierSetting();
            this.multiplierSettingFo = new ARKBreedingStats.MultiplierSetting();
            this.multiplierSettingOx = new ARKBreedingStats.MultiplierSetting();
            this.multiplierSettingSt = new ARKBreedingStats.MultiplierSetting();
            this.multiplierSettingHP = new ARKBreedingStats.MultiplierSetting();
            this.labelInfo = new System.Windows.Forms.Label();
            this.groupBoxMultiplier.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxMultiplier
            // 
            this.groupBoxMultiplier.Controls.Add(this.label4);
            this.groupBoxMultiplier.Controls.Add(this.label3);
            this.groupBoxMultiplier.Controls.Add(this.label2);
            this.groupBoxMultiplier.Controls.Add(this.label1);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingTo);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingSp);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingDm);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingWe);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingFo);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingOx);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingSt);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingHP);
            this.groupBoxMultiplier.Location = new System.Drawing.Point(12, 63);
            this.groupBoxMultiplier.Name = "groupBoxMultiplier";
            this.groupBoxMultiplier.Size = new System.Drawing.Size(301, 249);
            this.groupBoxMultiplier.TabIndex = 0;
            this.groupBoxMultiplier.TabStop = false;
            this.groupBoxMultiplier.Text = "Multipliers";
            // 
            // buttonAllToOne
            // 
            this.buttonAllToOne.Location = new System.Drawing.Point(319, 69);
            this.buttonAllToOne.Name = "buttonAllToOne";
            this.buttonAllToOne.Size = new System.Drawing.Size(87, 23);
            this.buttonAllToOne.TabIndex = 1;
            this.buttonAllToOne.Text = "Set all to 1";
            this.buttonAllToOne.UseVisualStyleBackColor = true;
            this.buttonAllToOne.Click += new System.EventHandler(this.buttonAllToOne_Click);
            // 
            // buttonSetToOfficial
            // 
            this.buttonSetToOfficial.Location = new System.Drawing.Point(319, 95);
            this.buttonSetToOfficial.Name = "buttonSetToOfficial";
            this.buttonSetToOfficial.Size = new System.Drawing.Size(87, 23);
            this.buttonSetToOfficial.TabIndex = 2;
            this.buttonSetToOfficial.Text = "Set to official";
            this.buttonSetToOfficial.UseVisualStyleBackColor = true;
            this.buttonSetToOfficial.Visible = false;
            this.buttonSetToOfficial.Click += new System.EventHandler(this.buttonSetToOfficial_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(331, 319);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(250, 319);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "TameAdd";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(115, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "TameAff";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(175, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "WildLevel";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(234, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "DomLevel";
            // 
            // multiplierSettingTo
            // 
            this.multiplierSettingTo.Location = new System.Drawing.Point(6, 214);
            this.multiplierSettingTo.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingTo.Name = "multiplierSettingTo";
            this.multiplierSettingTo.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingTo.TabIndex = 8;
            // 
            // multiplierSettingSp
            // 
            this.multiplierSettingSp.Location = new System.Drawing.Point(6, 188);
            this.multiplierSettingSp.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingSp.Name = "multiplierSettingSp";
            this.multiplierSettingSp.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingSp.TabIndex = 7;
            // 
            // multiplierSettingDm
            // 
            this.multiplierSettingDm.Location = new System.Drawing.Point(6, 162);
            this.multiplierSettingDm.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingDm.Name = "multiplierSettingDm";
            this.multiplierSettingDm.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingDm.TabIndex = 6;
            // 
            // multiplierSettingWe
            // 
            this.multiplierSettingWe.Location = new System.Drawing.Point(6, 136);
            this.multiplierSettingWe.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingWe.Name = "multiplierSettingWe";
            this.multiplierSettingWe.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingWe.TabIndex = 5;
            // 
            // multiplierSettingFo
            // 
            this.multiplierSettingFo.Location = new System.Drawing.Point(6, 110);
            this.multiplierSettingFo.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingFo.Name = "multiplierSettingFo";
            this.multiplierSettingFo.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingFo.TabIndex = 4;
            // 
            // multiplierSettingOx
            // 
            this.multiplierSettingOx.Location = new System.Drawing.Point(6, 84);
            this.multiplierSettingOx.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingOx.Name = "multiplierSettingOx";
            this.multiplierSettingOx.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingOx.TabIndex = 3;
            // 
            // multiplierSettingSt
            // 
            this.multiplierSettingSt.Location = new System.Drawing.Point(6, 58);
            this.multiplierSettingSt.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingSt.Name = "multiplierSettingSt";
            this.multiplierSettingSt.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingSt.TabIndex = 2;
            // 
            // multiplierSettingHP
            // 
            this.multiplierSettingHP.Location = new System.Drawing.Point(6, 32);
            this.multiplierSettingHP.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingHP.Name = "multiplierSettingHP";
            this.multiplierSettingHP.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingHP.TabIndex = 1;
            // 
            // labelInfo
            // 
            this.labelInfo.Location = new System.Drawing.Point(12, 9);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(308, 48);
            this.labelInfo.TabIndex = 5;
            this.labelInfo.Text = "The multipliers are saved with each library. If the server you play on changes it" +
    "s multipliers, you can adjust them here.";
            // 
            // Settings
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(419, 354);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonSetToOfficial);
            this.Controls.Add(this.buttonAllToOne);
            this.Controls.Add(this.groupBoxMultiplier);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Settings";
            this.Text = "Settings";
            this.groupBoxMultiplier.ResumeLayout(false);
            this.groupBoxMultiplier.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxMultiplier;
        private MultiplierSetting multiplierSettingTo;
        private MultiplierSetting multiplierSettingSp;
        private MultiplierSetting multiplierSettingDm;
        private MultiplierSetting multiplierSettingWe;
        private MultiplierSetting multiplierSettingFo;
        private MultiplierSetting multiplierSettingOx;
        private MultiplierSetting multiplierSettingSt;
        private MultiplierSetting multiplierSettingHP;
        private System.Windows.Forms.Button buttonAllToOne;
        private System.Windows.Forms.Button buttonSetToOfficial;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelInfo;
    }
}