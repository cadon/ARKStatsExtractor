namespace ARKBreedingStats.duplicates
{
    partial class MergingDuplicatesUI
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.gbLeftCreature = new System.Windows.Forms.GroupBox();
            this.statsDisplay2 = new ARKBreedingStats.uiControls.StatsDisplay();
            this.label1 = new System.Windows.Forms.Label();
            this.gbRightCreature = new System.Windows.Forms.GroupBox();
            this.statsDisplay1 = new ARKBreedingStats.uiControls.StatsDisplay();
            this.lbCreatureInfosLeft = new System.Windows.Forms.Label();
            this.lbCreatureInfosRight = new System.Windows.Forms.Label();
            this.btUseLeft = new System.Windows.Forms.Button();
            this.btUseRight = new System.Windows.Forms.Button();
            this.btKeepBoth = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.gbLeftCreature.SuspendLayout();
            this.gbRightCreature.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btUseRight, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.gbLeftCreature, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gbRightCreature, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btUseLeft, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btKeepBoth, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(822, 490);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // gbLeftCreature
            // 
            this.gbLeftCreature.Controls.Add(this.lbCreatureInfosLeft);
            this.gbLeftCreature.Controls.Add(this.statsDisplay2);
            this.gbLeftCreature.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbLeftCreature.Location = new System.Drawing.Point(3, 43);
            this.gbLeftCreature.Name = "gbLeftCreature";
            this.gbLeftCreature.Size = new System.Drawing.Size(405, 344);
            this.gbLeftCreature.TabIndex = 4;
            this.gbLeftCreature.TabStop = false;
            this.gbLeftCreature.Text = "groupBox2";
            // 
            // statsDisplay2
            // 
            this.statsDisplay2.Location = new System.Drawing.Point(217, 19);
            this.statsDisplay2.Name = "statsDisplay2";
            this.statsDisplay2.Size = new System.Drawing.Size(182, 214);
            this.statsDisplay2.TabIndex = 1;
            // 
            // label1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(816, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "Duplicate found";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbRightCreature
            // 
            this.gbRightCreature.Controls.Add(this.lbCreatureInfosRight);
            this.gbRightCreature.Controls.Add(this.statsDisplay1);
            this.gbRightCreature.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbRightCreature.Location = new System.Drawing.Point(414, 43);
            this.gbRightCreature.Name = "gbRightCreature";
            this.gbRightCreature.Size = new System.Drawing.Size(405, 344);
            this.gbRightCreature.TabIndex = 3;
            this.gbRightCreature.TabStop = false;
            this.gbRightCreature.Text = "groupBox1";
            // 
            // statsDisplay1
            // 
            this.statsDisplay1.Location = new System.Drawing.Point(6, 19);
            this.statsDisplay1.Name = "statsDisplay1";
            this.statsDisplay1.Size = new System.Drawing.Size(182, 214);
            this.statsDisplay1.TabIndex = 1;
            // 
            // lbCreatureInfosLeft
            // 
            this.lbCreatureInfosLeft.AutoSize = true;
            this.lbCreatureInfosLeft.Location = new System.Drawing.Point(364, 236);
            this.lbCreatureInfosLeft.Name = "lbCreatureInfosLeft";
            this.lbCreatureInfosLeft.Size = new System.Drawing.Size(35, 13);
            this.lbCreatureInfosLeft.TabIndex = 2;
            this.lbCreatureInfosLeft.Text = "label2";
            // 
            // lbCreatureInfosRight
            // 
            this.lbCreatureInfosRight.AutoSize = true;
            this.lbCreatureInfosRight.Location = new System.Drawing.Point(6, 236);
            this.lbCreatureInfosRight.Name = "lbCreatureInfosRight";
            this.lbCreatureInfosRight.Size = new System.Drawing.Size(35, 13);
            this.lbCreatureInfosRight.TabIndex = 2;
            this.lbCreatureInfosRight.Text = "label3";
            // 
            // btUseLeft
            // 
            this.btUseLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btUseLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btUseLeft.Location = new System.Drawing.Point(3, 393);
            this.btUseLeft.Name = "btUseLeft";
            this.btUseLeft.Size = new System.Drawing.Size(405, 44);
            this.btUseLeft.TabIndex = 5;
            this.btUseLeft.Text = "Use left, overwrite right";
            this.btUseLeft.UseVisualStyleBackColor = true;
            this.btUseLeft.Click += new System.EventHandler(this.btUseLeft_Click);
            // 
            // btUseRight
            // 
            this.btUseRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btUseRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btUseRight.Location = new System.Drawing.Point(414, 393);
            this.btUseRight.Name = "btUseRight";
            this.btUseRight.Size = new System.Drawing.Size(405, 44);
            this.btUseRight.TabIndex = 6;
            this.btUseRight.Text = "Use right, overwrite left";
            this.btUseRight.UseVisualStyleBackColor = true;
            this.btUseRight.Click += new System.EventHandler(this.btUseRight_Click);
            // 
            // btKeepBoth
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.btKeepBoth, 2);
            this.btKeepBoth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btKeepBoth.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btKeepBoth.Location = new System.Drawing.Point(3, 443);
            this.btKeepBoth.Name = "btKeepBoth";
            this.btKeepBoth.Size = new System.Drawing.Size(816, 44);
            this.btKeepBoth.TabIndex = 7;
            this.btKeepBoth.Text = "Keep Both (do nothing)";
            this.btKeepBoth.UseVisualStyleBackColor = true;
            this.btKeepBoth.Click += new System.EventHandler(this.btKeepBoth_Click);
            // 
            // MergingDuplicates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MergingDuplicates";
            this.Size = new System.Drawing.Size(822, 490);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.gbLeftCreature.ResumeLayout(false);
            this.gbLeftCreature.PerformLayout();
            this.gbRightCreature.ResumeLayout(false);
            this.gbRightCreature.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbLeftCreature;
        private uiControls.StatsDisplay statsDisplay2;
        private System.Windows.Forms.GroupBox gbRightCreature;
        private uiControls.StatsDisplay statsDisplay1;
        private System.Windows.Forms.Button btUseRight;
        private System.Windows.Forms.Label lbCreatureInfosLeft;
        private System.Windows.Forms.Label lbCreatureInfosRight;
        private System.Windows.Forms.Button btUseLeft;
        private System.Windows.Forms.Button btKeepBoth;
    }
}
