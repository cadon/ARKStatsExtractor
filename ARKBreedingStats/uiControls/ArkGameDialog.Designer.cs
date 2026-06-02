namespace ARKBreedingStats.uiControls
{
    partial class ArkGameDialog
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
            Lb = new System.Windows.Forms.Label();
            BtAsa = new System.Windows.Forms.Button();
            BtAse = new System.Windows.Forms.Button();
            CbRememberSelection = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // Lb
            // 
            Lb.AutoSize = true;
            Lb.Location = new System.Drawing.Point(12, 9);
            Lb.Name = "Lb";
            Lb.Padding = new System.Windows.Forms.Padding(5);
            Lb.Size = new System.Drawing.Size(180, 25);
            Lb.TabIndex = 0;
            Lb.Text = "Select game for the new library";
            // 
            // BtAsa
            // 
            BtAsa.Location = new System.Drawing.Point(12, 37);
            BtAsa.Name = "BtAsa";
            BtAsa.Size = new System.Drawing.Size(183, 68);
            BtAsa.TabIndex = 1;
            BtAsa.Text = "ARK: Survival Ascended";
            BtAsa.UseVisualStyleBackColor = true;
            BtAsa.Click += BtAsa_Click;
            // 
            // BtAse
            // 
            BtAse.Location = new System.Drawing.Point(201, 37);
            BtAse.Name = "BtAse";
            BtAse.Size = new System.Drawing.Size(183, 68);
            BtAse.TabIndex = 2;
            BtAse.Text = "ARK: Survival Evolved";
            BtAse.UseVisualStyleBackColor = true;
            BtAse.Click += BtAse_Click;
            // 
            // CbRememberSelection
            // 
            CbRememberSelection.AutoSize = true;
            CbRememberSelection.Location = new System.Drawing.Point(12, 111);
            CbRememberSelection.Name = "CbRememberSelection";
            CbRememberSelection.Size = new System.Drawing.Size(306, 19);
            CbRememberSelection.TabIndex = 3;
            CbRememberSelection.Text = "Remember selection (can be changed in the settings)";
            CbRememberSelection.UseVisualStyleBackColor = true;
            // 
            // ArkGameDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(396, 142);
            Controls.Add(CbRememberSelection);
            Controls.Add(BtAse);
            Controls.Add(BtAsa);
            Controls.Add(Lb);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Name = "ArkGameDialog";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label Lb;
        private System.Windows.Forms.Button BtAsa;
        private System.Windows.Forms.Button BtAse;
        private System.Windows.Forms.CheckBox CbRememberSelection;
    }
}