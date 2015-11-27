namespace ARKBreedingStats
{
    partial class CreatureBox
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
            this.buttonEdit = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.statDisplayTo = new ARKBreedingStats.StatDisplay();
            this.statDisplaySp = new ARKBreedingStats.StatDisplay();
            this.statDisplayDm = new ARKBreedingStats.StatDisplay();
            this.statDisplayWe = new ARKBreedingStats.StatDisplay();
            this.statDisplayFo = new ARKBreedingStats.StatDisplay();
            this.statDisplayOx = new ARKBreedingStats.StatDisplay();
            this.statDisplaySt = new ARKBreedingStats.StatDisplay();
            this.statDisplayHP = new ARKBreedingStats.StatDisplay();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonEdit);
            this.groupBox1.Controls.Add(this.textBoxName);
            this.groupBox1.Controls.Add(this.statDisplayTo);
            this.groupBox1.Controls.Add(this.statDisplaySp);
            this.groupBox1.Controls.Add(this.statDisplayDm);
            this.groupBox1.Controls.Add(this.statDisplayWe);
            this.groupBox1.Controls.Add(this.statDisplayFo);
            this.groupBox1.Controls.Add(this.statDisplayOx);
            this.groupBox1.Controls.Add(this.statDisplaySt);
            this.groupBox1.Controls.Add(this.statDisplayHP);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(147, 210);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Creature";
            // 
            // buttonEdit
            // 
            this.buttonEdit.Location = new System.Drawing.Point(124, 187);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(21, 20);
            this.buttonEdit.TabIndex = 9;
            this.buttonEdit.Text = "e";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(6, 22);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(129, 20);
            this.textBoxName.TabIndex = 8;
            this.textBoxName.Visible = false;
            this.textBoxName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxName_KeyDown);
            this.textBoxName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxName_KeyPress);
            // 
            // statDisplayTo
            // 
            this.statDisplayTo.Location = new System.Drawing.Point(6, 183);
            this.statDisplayTo.Name = "statDisplayTo";
            this.statDisplayTo.Size = new System.Drawing.Size(129, 20);
            this.statDisplayTo.TabIndex = 7;
            // 
            // statDisplaySp
            // 
            this.statDisplaySp.Location = new System.Drawing.Point(6, 160);
            this.statDisplaySp.Name = "statDisplaySp";
            this.statDisplaySp.Size = new System.Drawing.Size(129, 20);
            this.statDisplaySp.TabIndex = 6;
            // 
            // statDisplayDm
            // 
            this.statDisplayDm.Location = new System.Drawing.Point(6, 137);
            this.statDisplayDm.Name = "statDisplayDm";
            this.statDisplayDm.Size = new System.Drawing.Size(129, 20);
            this.statDisplayDm.TabIndex = 5;
            // 
            // statDisplayWe
            // 
            this.statDisplayWe.Location = new System.Drawing.Point(6, 114);
            this.statDisplayWe.Name = "statDisplayWe";
            this.statDisplayWe.Size = new System.Drawing.Size(129, 20);
            this.statDisplayWe.TabIndex = 4;
            // 
            // statDisplayFo
            // 
            this.statDisplayFo.Location = new System.Drawing.Point(6, 91);
            this.statDisplayFo.Name = "statDisplayFo";
            this.statDisplayFo.Size = new System.Drawing.Size(129, 20);
            this.statDisplayFo.TabIndex = 3;
            // 
            // statDisplayOx
            // 
            this.statDisplayOx.Location = new System.Drawing.Point(6, 68);
            this.statDisplayOx.Name = "statDisplayOx";
            this.statDisplayOx.Size = new System.Drawing.Size(129, 20);
            this.statDisplayOx.TabIndex = 2;
            // 
            // statDisplaySt
            // 
            this.statDisplaySt.Location = new System.Drawing.Point(6, 45);
            this.statDisplaySt.Name = "statDisplaySt";
            this.statDisplaySt.Size = new System.Drawing.Size(129, 20);
            this.statDisplaySt.TabIndex = 1;
            // 
            // statDisplayHP
            // 
            this.statDisplayHP.Location = new System.Drawing.Point(6, 22);
            this.statDisplayHP.Name = "statDisplayHP";
            this.statDisplayHP.Size = new System.Drawing.Size(129, 20);
            this.statDisplayHP.TabIndex = 0;
            // 
            // CreatureBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "CreatureBox";
            this.Size = new System.Drawing.Size(147, 210);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private StatDisplay statDisplayTo;
        private StatDisplay statDisplaySp;
        private StatDisplay statDisplayDm;
        private StatDisplay statDisplayWe;
        private StatDisplay statDisplayFo;
        private StatDisplay statDisplayOx;
        private StatDisplay statDisplaySt;
        private StatDisplay statDisplayHP;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button buttonEdit;
    }
}
