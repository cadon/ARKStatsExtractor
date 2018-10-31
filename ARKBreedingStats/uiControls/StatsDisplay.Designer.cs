namespace ARKBreedingStats.uiControls
{
    partial class StatsDisplay
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
            this.labelSex = new System.Windows.Forms.Label();
            this.labelStatHeader = new System.Windows.Forms.Label();
            this.statDisplayTo = new ARKBreedingStats.StatDisplay();
            this.statDisplaySp = new ARKBreedingStats.StatDisplay();
            this.statDisplayDm = new ARKBreedingStats.StatDisplay();
            this.statDisplayWe = new ARKBreedingStats.StatDisplay();
            this.statDisplayFo = new ARKBreedingStats.StatDisplay();
            this.statDisplayOx = new ARKBreedingStats.StatDisplay();
            this.statDisplaySt = new ARKBreedingStats.StatDisplay();
            this.statDisplayHP = new ARKBreedingStats.StatDisplay();
            this.SuspendLayout();
            // 
            // labelSex
            // 
            this.labelSex.AutoSize = true;
            this.labelSex.Location = new System.Drawing.Point(3, 0);
            this.labelSex.Name = "labelSex";
            this.labelSex.Size = new System.Drawing.Size(13, 13);
            this.labelSex.TabIndex = 26;
            this.labelSex.Text = "?";
            // 
            // labelStatHeader
            // 
            this.labelStatHeader.AutoSize = true;
            this.labelStatHeader.Location = new System.Drawing.Point(31, 0);
            this.labelStatHeader.Name = "labelStatHeader";
            this.labelStatHeader.Size = new System.Drawing.Size(145, 13);
            this.labelStatHeader.TabIndex = 25;
            this.labelStatHeader.Text = "W      D      Breed       Current";
            // 
            // statDisplayTo
            // 
            this.statDisplayTo.Location = new System.Drawing.Point(3, 180);
            this.statDisplayTo.Name = "statDisplayTo";
            this.statDisplayTo.Size = new System.Drawing.Size(183, 20);
            this.statDisplayTo.TabIndex = 24;
            // 
            // statDisplaySp
            // 
            this.statDisplaySp.Location = new System.Drawing.Point(3, 157);
            this.statDisplaySp.Name = "statDisplaySp";
            this.statDisplaySp.Size = new System.Drawing.Size(183, 20);
            this.statDisplaySp.TabIndex = 23;
            // 
            // statDisplayDm
            // 
            this.statDisplayDm.Location = new System.Drawing.Point(3, 134);
            this.statDisplayDm.Name = "statDisplayDm";
            this.statDisplayDm.Size = new System.Drawing.Size(183, 20);
            this.statDisplayDm.TabIndex = 22;
            // 
            // statDisplayWe
            // 
            this.statDisplayWe.Location = new System.Drawing.Point(3, 111);
            this.statDisplayWe.Name = "statDisplayWe";
            this.statDisplayWe.Size = new System.Drawing.Size(183, 20);
            this.statDisplayWe.TabIndex = 21;
            // 
            // statDisplayFo
            // 
            this.statDisplayFo.Location = new System.Drawing.Point(3, 88);
            this.statDisplayFo.Name = "statDisplayFo";
            this.statDisplayFo.Size = new System.Drawing.Size(183, 20);
            this.statDisplayFo.TabIndex = 20;
            // 
            // statDisplayOx
            // 
            this.statDisplayOx.Location = new System.Drawing.Point(3, 65);
            this.statDisplayOx.Name = "statDisplayOx";
            this.statDisplayOx.Size = new System.Drawing.Size(183, 20);
            this.statDisplayOx.TabIndex = 19;
            // 
            // statDisplaySt
            // 
            this.statDisplaySt.Location = new System.Drawing.Point(3, 42);
            this.statDisplaySt.Name = "statDisplaySt";
            this.statDisplaySt.Size = new System.Drawing.Size(183, 20);
            this.statDisplaySt.TabIndex = 18;
            // 
            // statDisplayHP
            // 
            this.statDisplayHP.Location = new System.Drawing.Point(3, 19);
            this.statDisplayHP.Name = "statDisplayHP";
            this.statDisplayHP.Size = new System.Drawing.Size(183, 20);
            this.statDisplayHP.TabIndex = 17;
            // 
            // StatsDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelSex);
            this.Controls.Add(this.labelStatHeader);
            this.Controls.Add(this.statDisplayTo);
            this.Controls.Add(this.statDisplaySp);
            this.Controls.Add(this.statDisplayDm);
            this.Controls.Add(this.statDisplayWe);
            this.Controls.Add(this.statDisplayFo);
            this.Controls.Add(this.statDisplayOx);
            this.Controls.Add(this.statDisplaySt);
            this.Controls.Add(this.statDisplayHP);
            this.Name = "StatsDisplay";
            this.Size = new System.Drawing.Size(182, 203);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSex;
        private System.Windows.Forms.Label labelStatHeader;
        private ARKBreedingStats.StatDisplay statDisplayTo;
        private ARKBreedingStats.StatDisplay statDisplaySp;
        private ARKBreedingStats.StatDisplay statDisplayDm;
        private ARKBreedingStats.StatDisplay statDisplayWe;
        private ARKBreedingStats.StatDisplay statDisplayFo;
        private ARKBreedingStats.StatDisplay statDisplayOx;
        private ARKBreedingStats.StatDisplay statDisplaySt;
        private ARKBreedingStats.StatDisplay statDisplayHP;
    }
}
