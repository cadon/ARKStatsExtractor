
namespace ARKBreedingStats.library
{
    partial class AddDummyCreaturesSettings
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
            this.BtOk = new System.Windows.Forms.Button();
            this.BtCancel = new System.Windows.Forms.Button();
            this.CbOnlySelectedSpecies = new System.Windows.Forms.CheckBox();
            this.NudAmount = new ARKBreedingStats.uiControls.Nud();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.NudSpeciesAmount = new ARKBreedingStats.uiControls.Nud();
            ((System.ComponentModel.ISupportInitialize)(this.NudAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSpeciesAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // BtOk
            // 
            this.BtOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtOk.Location = new System.Drawing.Point(122, 96);
            this.BtOk.Name = "BtOk";
            this.BtOk.Size = new System.Drawing.Size(104, 23);
            this.BtOk.TabIndex = 0;
            this.BtOk.Text = "Add creatures";
            this.BtOk.UseVisualStyleBackColor = true;
            this.BtOk.Click += new System.EventHandler(this.BtOk_Click);
            // 
            // BtCancel
            // 
            this.BtCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtCancel.Location = new System.Drawing.Point(12, 96);
            this.BtCancel.Name = "BtCancel";
            this.BtCancel.Size = new System.Drawing.Size(104, 23);
            this.BtCancel.TabIndex = 1;
            this.BtCancel.Text = "Cancel";
            this.BtCancel.UseVisualStyleBackColor = true;
            this.BtCancel.Click += new System.EventHandler(this.BtCancel_Click);
            // 
            // CbOnlySelectedSpecies
            // 
            this.CbOnlySelectedSpecies.AutoSize = true;
            this.CbOnlySelectedSpecies.Location = new System.Drawing.Point(12, 38);
            this.CbOnlySelectedSpecies.Name = "CbOnlySelectedSpecies";
            this.CbOnlySelectedSpecies.Size = new System.Drawing.Size(129, 17);
            this.CbOnlySelectedSpecies.TabIndex = 2;
            this.CbOnlySelectedSpecies.Text = "Only selected species";
            this.CbOnlySelectedSpecies.UseVisualStyleBackColor = true;
            // 
            // NudAmount
            // 
            this.NudAmount.ForeColor = System.Drawing.SystemColors.WindowText;
            this.NudAmount.Location = new System.Drawing.Point(142, 12);
            this.NudAmount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NudAmount.Name = "NudAmount";
            this.NudAmount.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.NudAmount.Size = new System.Drawing.Size(67, 20);
            this.NudAmount.TabIndex = 3;
            this.NudAmount.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Amount of creatures";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Different random species";
            // 
            // NudSpeciesAmount
            // 
            this.NudSpeciesAmount.ForeColor = System.Drawing.SystemColors.WindowText;
            this.NudSpeciesAmount.Location = new System.Drawing.Point(142, 61);
            this.NudSpeciesAmount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NudSpeciesAmount.Name = "NudSpeciesAmount";
            this.NudSpeciesAmount.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.NudSpeciesAmount.Size = new System.Drawing.Size(67, 20);
            this.NudSpeciesAmount.TabIndex = 6;
            this.NudSpeciesAmount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // AddDummyCreaturesSettings
            // 
            this.AcceptButton = this.BtOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtCancel;
            this.ClientSize = new System.Drawing.Size(238, 131);
            this.Controls.Add(this.NudSpeciesAmount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NudAmount);
            this.Controls.Add(this.CbOnlySelectedSpecies);
            this.Controls.Add(this.BtCancel);
            this.Controls.Add(this.BtOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AddDummyCreaturesSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add random creatures";
            ((System.ComponentModel.ISupportInitialize)(this.NudAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSpeciesAmount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtOk;
        private System.Windows.Forms.Button BtCancel;
        private System.Windows.Forms.CheckBox CbOnlySelectedSpecies;
        private uiControls.Nud NudAmount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private uiControls.Nud NudSpeciesAmount;
    }
}