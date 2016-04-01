namespace ARKBreedingStats
{
    partial class PedigreeCreature
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelSp = new System.Windows.Forms.Label();
            this.labelDm = new System.Windows.Forms.Label();
            this.labelWe = new System.Windows.Forms.Label();
            this.labelFo = new System.Windows.Forms.Label();
            this.labelOx = new System.Windows.Forms.Label();
            this.labelSt = new System.Windows.Forms.Label();
            this.labelHP = new System.Windows.Forms.Label();
            this.labelGender = new System.Windows.Forms.Label();
            this.panelHighlight = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.labelSp);
            this.groupBox1.Controls.Add(this.labelDm);
            this.groupBox1.Controls.Add(this.labelWe);
            this.groupBox1.Controls.Add(this.labelFo);
            this.groupBox1.Controls.Add(this.labelOx);
            this.groupBox1.Controls.Add(this.labelSt);
            this.groupBox1.Controls.Add(this.labelHP);
            this.groupBox1.Controls.Add(this.labelGender);
            this.groupBox1.Controls.Add(this.panelHighlight);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(249, 35);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CreatureName";
            this.groupBox1.Click += new System.EventHandler(this.element_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(223, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 24);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.element_Click);
            // 
            // labelSp
            // 
            this.labelSp.Location = new System.Drawing.Point(190, 16);
            this.labelSp.Name = "labelSp";
            this.labelSp.Size = new System.Drawing.Size(27, 13);
            this.labelSp.TabIndex = 7;
            this.labelSp.Text = "Sp";
            this.labelSp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelSp.Click += new System.EventHandler(this.element_Click);
            // 
            // labelDm
            // 
            this.labelDm.Location = new System.Drawing.Point(162, 16);
            this.labelDm.Name = "labelDm";
            this.labelDm.Size = new System.Drawing.Size(27, 13);
            this.labelDm.TabIndex = 6;
            this.labelDm.Text = "Dm";
            this.labelDm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelDm.Click += new System.EventHandler(this.element_Click);
            // 
            // labelWe
            // 
            this.labelWe.Location = new System.Drawing.Point(134, 16);
            this.labelWe.Name = "labelWe";
            this.labelWe.Size = new System.Drawing.Size(27, 13);
            this.labelWe.TabIndex = 5;
            this.labelWe.Text = "We";
            this.labelWe.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelWe.Click += new System.EventHandler(this.element_Click);
            // 
            // labelFo
            // 
            this.labelFo.Location = new System.Drawing.Point(106, 16);
            this.labelFo.Name = "labelFo";
            this.labelFo.Size = new System.Drawing.Size(27, 13);
            this.labelFo.TabIndex = 4;
            this.labelFo.Text = "Fo";
            this.labelFo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelFo.Click += new System.EventHandler(this.element_Click);
            // 
            // labelOx
            // 
            this.labelOx.Location = new System.Drawing.Point(78, 16);
            this.labelOx.Name = "labelOx";
            this.labelOx.Size = new System.Drawing.Size(27, 13);
            this.labelOx.TabIndex = 3;
            this.labelOx.Text = "Ox";
            this.labelOx.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelOx.Click += new System.EventHandler(this.element_Click);
            // 
            // labelSt
            // 
            this.labelSt.Location = new System.Drawing.Point(50, 16);
            this.labelSt.Name = "labelSt";
            this.labelSt.Size = new System.Drawing.Size(27, 13);
            this.labelSt.TabIndex = 2;
            this.labelSt.Text = "St";
            this.labelSt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelSt.Click += new System.EventHandler(this.element_Click);
            // 
            // labelHP
            // 
            this.labelHP.Location = new System.Drawing.Point(22, 16);
            this.labelHP.Name = "labelHP";
            this.labelHP.Size = new System.Drawing.Size(27, 13);
            this.labelHP.TabIndex = 1;
            this.labelHP.Text = "HP";
            this.labelHP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelHP.Click += new System.EventHandler(this.element_Click);
            // 
            // labelGender
            // 
            this.labelGender.Location = new System.Drawing.Point(6, 16);
            this.labelGender.Name = "labelGender";
            this.labelGender.Size = new System.Drawing.Size(13, 13);
            this.labelGender.TabIndex = 0;
            this.labelGender.Text = "G";
            this.labelGender.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelGender.Click += new System.EventHandler(this.element_Click);
            // 
            // panelHighlight
            // 
            this.panelHighlight.BackColor = System.Drawing.SystemColors.Highlight;
            this.panelHighlight.Location = new System.Drawing.Point(3, 13);
            this.panelHighlight.Name = "panelHighlight";
            this.panelHighlight.Size = new System.Drawing.Size(217, 19);
            this.panelHighlight.TabIndex = 8;
            this.panelHighlight.Visible = false;
            // 
            // PedigreeCreature
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "PedigreeCreature";
            this.Size = new System.Drawing.Size(249, 35);
            this.Click += new System.EventHandler(this.PedigreeCreature_Click);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelSp;
        private System.Windows.Forms.Label labelDm;
        private System.Windows.Forms.Label labelWe;
        private System.Windows.Forms.Label labelFo;
        private System.Windows.Forms.Label labelOx;
        private System.Windows.Forms.Label labelSt;
        private System.Windows.Forms.Label labelHP;
        private System.Windows.Forms.Label labelGender;
        private System.Windows.Forms.Panel panelHighlight;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
