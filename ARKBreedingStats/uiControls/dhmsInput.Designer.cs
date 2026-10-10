namespace ARKBreedingStats.uiControls
{
    partial class dhmsInput
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
            mTBD = new System.Windows.Forms.TextBox();
            mTBH = new System.Windows.Forms.TextBox();
            mTBM = new System.Windows.Forms.TextBox();
            mTBS = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // mTBD
            // 
            mTBD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            mTBD.Location = new System.Drawing.Point(3, 3);
            mTBD.Name = "mTBD";
            mTBD.Size = new System.Drawing.Size(30, 23);
            mTBD.TabIndex = 0;
            mTBD.Text = "0";
            mTBD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            mTBD.TextChanged += mTB_TextChanged;
            mTBD.Enter += mTB_Enter;
            mTBD.KeyUp += mTB_KeyUp;
            // 
            // mTBH
            // 
            mTBH.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            mTBH.Location = new System.Drawing.Point(43, 3);
            mTBH.Name = "mTBH";
            mTBH.Size = new System.Drawing.Size(20, 23);
            mTBH.TabIndex = 1;
            mTBH.Text = "0";
            mTBH.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            mTBH.TextChanged += mTB_TextChanged;
            mTBH.Enter += mTB_Enter;
            mTBH.KeyUp += mTB_KeyUp;
            // 
            // mTBM
            // 
            mTBM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            mTBM.Location = new System.Drawing.Point(73, 3);
            mTBM.Name = "mTBM";
            mTBM.Size = new System.Drawing.Size(20, 23);
            mTBM.TabIndex = 2;
            mTBM.Text = "0";
            mTBM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            mTBM.TextChanged += mTB_TextChanged;
            mTBM.Enter += mTB_Enter;
            mTBM.KeyUp += mTB_KeyUp;
            // 
            // mTBS
            // 
            mTBS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            mTBS.Location = new System.Drawing.Point(105, 3);
            mTBS.Name = "mTBS";
            mTBS.Size = new System.Drawing.Size(20, 23);
            mTBS.TabIndex = 3;
            mTBS.Text = "0";
            mTBS.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            mTBS.TextChanged += mTB_TextChanged;
            mTBS.Enter += mTB_Enter;
            mTBS.KeyUp += mTB_KeyUp;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(31, 6);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(14, 15);
            label1.TabIndex = 4;
            label1.Text = "d";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(61, 6);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(14, 15);
            label2.TabIndex = 5;
            label2.Text = "h";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(91, 6);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(18, 15);
            label3.TabIndex = 6;
            label3.Text = "m";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(125, 6);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(12, 15);
            label4.TabIndex = 7;
            label4.Text = "s";
            // 
            // dhmsInput
            // 
            Controls.Add(mTBS);
            Controls.Add(mTBM);
            Controls.Add(mTBH);
            Controls.Add(mTBD);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "dhmsInput";
            Size = new System.Drawing.Size(136, 26);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mTBD;
        private System.Windows.Forms.TextBox mTBH;
        private System.Windows.Forms.TextBox mTBM;
        private System.Windows.Forms.TextBox mTBS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}
