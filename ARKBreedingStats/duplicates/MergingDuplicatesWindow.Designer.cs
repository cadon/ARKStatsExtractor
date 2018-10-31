namespace ARKBreedingStats.duplicates
{
    partial class MergingDuplicatesWindow
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btUseRight = new System.Windows.Forms.Button();
            this.gbLeftCreature = new System.Windows.Forms.GroupBox();
            this.lbCreatureInfosLeft = new System.Windows.Forms.Label();
            this.statsDisplay2 = new ARKBreedingStats.uiControls.StatsDisplay();
            this.label1 = new System.Windows.Forms.Label();
            this.gbRightCreature = new System.Windows.Forms.GroupBox();
            this.lbCreatureInfosRight = new System.Windows.Forms.Label();
            this.statsDisplay1 = new ARKBreedingStats.uiControls.StatsDisplay();
            this.btUseLeft = new System.Windows.Forms.Button();
            this.btKeepBoth = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
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
            this.tableLayoutPanel1.Controls.Add(this.btUseRight, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.gbLeftCreature, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.gbRightCreature, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btUseLeft, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btKeepBoth, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(769, 539);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // btUseRight
            // 
            this.btUseRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btUseRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btUseRight.Location = new System.Drawing.Point(387, 442);
            this.btUseRight.Name = "btUseRight";
            this.btUseRight.Size = new System.Drawing.Size(379, 44);
            this.btUseRight.TabIndex = 6;
            this.btUseRight.Text = "Use right, overwrite left";
            this.btUseRight.UseVisualStyleBackColor = true;
            this.btUseRight.Click += new System.EventHandler(this.btUseRight_Click);
            // 
            // gbLeftCreature
            // 
            this.gbLeftCreature.Controls.Add(this.lbCreatureInfosLeft);
            this.gbLeftCreature.Controls.Add(this.statsDisplay2);
            this.gbLeftCreature.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbLeftCreature.Location = new System.Drawing.Point(3, 73);
            this.gbLeftCreature.Name = "gbLeftCreature";
            this.gbLeftCreature.Size = new System.Drawing.Size(378, 363);
            this.gbLeftCreature.TabIndex = 4;
            this.gbLeftCreature.TabStop = false;
            this.gbLeftCreature.Text = "groupBox2";
            // 
            // lbCreatureInfosLeft
            // 
            this.lbCreatureInfosLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCreatureInfosLeft.AutoSize = true;
            this.lbCreatureInfosLeft.Location = new System.Drawing.Point(337, 236);
            this.lbCreatureInfosLeft.Name = "lbCreatureInfosLeft";
            this.lbCreatureInfosLeft.Size = new System.Drawing.Size(35, 13);
            this.lbCreatureInfosLeft.TabIndex = 2;
            this.lbCreatureInfosLeft.Text = "label2";
            // 
            // statsDisplay2
            // 
            this.statsDisplay2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.statsDisplay2.Location = new System.Drawing.Point(190, 19);
            this.statsDisplay2.Name = "statsDisplay2";
            this.statsDisplay2.Size = new System.Drawing.Size(182, 214);
            this.statsDisplay2.TabIndex = 1;
            // 
            // label1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(763, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "Duplicate found";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbRightCreature
            // 
            this.gbRightCreature.Controls.Add(this.lbCreatureInfosRight);
            this.gbRightCreature.Controls.Add(this.statsDisplay1);
            this.gbRightCreature.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbRightCreature.Location = new System.Drawing.Point(387, 73);
            this.gbRightCreature.Name = "gbRightCreature";
            this.gbRightCreature.Size = new System.Drawing.Size(379, 363);
            this.gbRightCreature.TabIndex = 3;
            this.gbRightCreature.TabStop = false;
            this.gbRightCreature.Text = "groupBox1";
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
            // statsDisplay1
            // 
            this.statsDisplay1.Location = new System.Drawing.Point(6, 19);
            this.statsDisplay1.Name = "statsDisplay1";
            this.statsDisplay1.Size = new System.Drawing.Size(182, 214);
            this.statsDisplay1.TabIndex = 1;
            // 
            // btUseLeft
            // 
            this.btUseLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btUseLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btUseLeft.Location = new System.Drawing.Point(3, 442);
            this.btUseLeft.Name = "btUseLeft";
            this.btUseLeft.Size = new System.Drawing.Size(378, 44);
            this.btUseLeft.TabIndex = 5;
            this.btUseLeft.Text = "Use left, overwrite right";
            this.btUseLeft.UseVisualStyleBackColor = true;
            this.btUseLeft.Click += new System.EventHandler(this.btUseLeft_Click);
            // 
            // btKeepBoth
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.btKeepBoth, 2);
            this.btKeepBoth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btKeepBoth.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btKeepBoth.Location = new System.Drawing.Point(3, 492);
            this.btKeepBoth.Name = "btKeepBoth";
            this.btKeepBoth.Size = new System.Drawing.Size(763, 44);
            this.btKeepBoth.TabIndex = 7;
            this.btKeepBoth.Text = "Keep Both (do nothing)";
            this.btKeepBoth.UseVisualStyleBackColor = true;
            this.btKeepBoth.Click += new System.EventHandler(this.btKeepBoth_Click);
            // 
            // progressBar1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.progressBar1, 2);
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(3, 3);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(763, 24);
            this.progressBar1.TabIndex = 8;
            // 
            // MergingDuplicatesWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 539);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MergingDuplicatesWindow";
            this.Text = "Merging Duplicates";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.gbLeftCreature.ResumeLayout(false);
            this.gbLeftCreature.PerformLayout();
            this.gbRightCreature.ResumeLayout(false);
            this.gbRightCreature.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btUseRight;
        private System.Windows.Forms.GroupBox gbLeftCreature;
        private System.Windows.Forms.Label lbCreatureInfosLeft;
        private uiControls.StatsDisplay statsDisplay2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbRightCreature;
        private System.Windows.Forms.Label lbCreatureInfosRight;
        private uiControls.StatsDisplay statsDisplay1;
        private System.Windows.Forms.Button btUseLeft;
        private System.Windows.Forms.Button btKeepBoth;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}