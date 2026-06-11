namespace ARKBreedingStats.uiControls
{
    partial class StatPotentials
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatPotentials));
            groupBox1 = new GroupBoxC();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            flpStats = new System.Windows.Forms.FlowLayoutPanel();
            label4 = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            LbWildLevels = new System.Windows.Forms.Label();
            LbDomLevels = new System.Windows.Forms.Label();
            LbImprinting = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(tableLayoutPanel1);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox1.Location = new System.Drawing.Point(0, 0);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(342, 552);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Potentials of the genes of the current creature";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(flpStats, 0, 1);
            tableLayoutPanel1.Controls.Add(label4, 0, 2);
            tableLayoutPanel1.Controls.Add(panel1, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(4, 19);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            tableLayoutPanel1.Size = new System.Drawing.Size(334, 530);
            tableLayoutPanel1.TabIndex = 5;
            // 
            // flpStats
            // 
            flpStats.Dock = System.Windows.Forms.DockStyle.Fill;
            flpStats.Location = new System.Drawing.Point(4, 47);
            flpStats.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            flpStats.Name = "flpStats";
            flpStats.Size = new System.Drawing.Size(326, 388);
            flpStats.TabIndex = 4;
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(4, 438);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(326, 65);
            label4.TabIndex = 4;
            label4.Text = resources.GetString("label4.Text");
            // 
            // panel1
            // 
            panel1.Controls.Add(LbWildLevels);
            panel1.Controls.Add(LbDomLevels);
            panel1.Controls.Add(LbImprinting);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(4, 3);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(326, 38);
            panel1.TabIndex = 5;
            // 
            // LbWildLevels
            // 
            LbWildLevels.BackColor = System.Drawing.Color.LightCoral;
            LbWildLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            LbWildLevels.Location = new System.Drawing.Point(4, 0);
            LbWildLevels.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbWildLevels.Name = "LbWildLevels";
            LbWildLevels.Size = new System.Drawing.Size(93, 26);
            LbWildLevels.TabIndex = 1;
            LbWildLevels.Text = "Wild-Levels";
            LbWildLevels.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LbDomLevels
            // 
            LbDomLevels.BackColor = System.Drawing.Color.Gold;
            LbDomLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            LbDomLevels.Location = new System.Drawing.Point(204, 0);
            LbDomLevels.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbDomLevels.Name = "LbDomLevels";
            LbDomLevels.Size = new System.Drawing.Size(93, 26);
            LbDomLevels.TabIndex = 3;
            LbDomLevels.Text = "Dom-Leveling";
            LbDomLevels.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LbImprinting
            // 
            LbImprinting.BackColor = System.Drawing.Color.SkyBlue;
            LbImprinting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            LbImprinting.Location = new System.Drawing.Point(104, 0);
            LbImprinting.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbImprinting.Name = "LbImprinting";
            LbImprinting.Size = new System.Drawing.Size(93, 26);
            LbImprinting.TabIndex = 2;
            LbImprinting.Text = "Imprinting";
            LbImprinting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // StatPotentials
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(groupBox1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "StatPotentials";
            Size = new System.Drawing.Size(342, 552);
            groupBox1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private GroupBoxC groupBox1;
        private System.Windows.Forms.Label LbDomLevels;
        private System.Windows.Forms.Label LbImprinting;
        private System.Windows.Forms.Label LbWildLevels;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flpStats;
        private System.Windows.Forms.Panel panel1;
    }
}
