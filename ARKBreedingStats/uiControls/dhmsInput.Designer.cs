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
            this.mTBD = new System.Windows.Forms.TextBox();
            this.mTBH = new System.Windows.Forms.TextBox();
            this.mTBM = new System.Windows.Forms.TextBox();
            this.mTBS = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mTBD
            // 
            this.mTBD.Location = new System.Drawing.Point(3, 3);
            this.mTBD.Name = "mTBD";
            this.mTBD.Size = new System.Drawing.Size(31, 20);
            this.mTBD.TabIndex = 0;
            this.mTBD.Text = "0";
            this.mTBD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mTBD.TextChanged += new System.EventHandler(this.mTB_TextChanged);
            this.mTBD.Enter += new System.EventHandler(this.mTB_Enter);
            this.mTBD.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mTB_KeyUp);
            // 
            // mTBH
            // 
            this.mTBH.Location = new System.Drawing.Point(43, 3);
            this.mTBH.Name = "mTBH";
            this.mTBH.Size = new System.Drawing.Size(21, 20);
            this.mTBH.TabIndex = 1;
            this.mTBH.Text = "0";
            this.mTBH.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mTBH.TextChanged += new System.EventHandler(this.mTB_TextChanged);
            this.mTBH.Enter += new System.EventHandler(this.mTB_Enter);
            this.mTBH.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mTB_KeyUp);
            // 
            // mTBM
            // 
            this.mTBM.Location = new System.Drawing.Point(73, 3);
            this.mTBM.Name = "mTBM";
            this.mTBM.Size = new System.Drawing.Size(21, 20);
            this.mTBM.TabIndex = 2;
            this.mTBM.Text = "0";
            this.mTBM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mTBM.TextChanged += new System.EventHandler(this.mTB_TextChanged);
            this.mTBM.Enter += new System.EventHandler(this.mTB_Enter);
            this.mTBM.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mTB_KeyUp);
            // 
            // mTBS
            // 
            this.mTBS.Location = new System.Drawing.Point(105, 3);
            this.mTBS.Name = "mTBS";
            this.mTBS.Size = new System.Drawing.Size(21, 20);
            this.mTBS.TabIndex = 3;
            this.mTBS.Text = "0";
            this.mTBS.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mTBS.TextChanged += new System.EventHandler(this.mTB_TextChanged);
            this.mTBS.Enter += new System.EventHandler(this.mTB_Enter);
            this.mTBS.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mTB_KeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "d";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(63, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "h";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(93, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(15, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "m";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(124, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(12, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "s";
            // 
            // dhmsInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mTBS);
            this.Controls.Add(this.mTBM);
            this.Controls.Add(this.mTBH);
            this.Controls.Add(this.mTBD);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "dhmsInput";
            this.Size = new System.Drawing.Size(136, 26);
            this.ResumeLayout(false);
            this.PerformLayout();

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
