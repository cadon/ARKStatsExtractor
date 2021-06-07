
namespace ARKBreedingStats.uiControls
{
    partial class CreatureAnalysis
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pColors = new System.Windows.Forms.Panel();
            this.LbColorAnalysis = new System.Windows.Forms.Label();
            this.LbColorStatus = new System.Windows.Forms.Label();
            this.pStats = new System.Windows.Forms.Panel();
            this.LbStatAnalysis = new System.Windows.Forms.Label();
            this.LbStatsStatus = new System.Windows.Forms.Label();
            this.pResult = new System.Windows.Forms.Panel();
            this.LbConclusion = new System.Windows.Forms.Label();
            this.LbIcon = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.pColors.SuspendLayout();
            this.pStats.SuspendLayout();
            this.pResult.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pColors);
            this.groupBox1.Controls.Add(this.pStats);
            this.groupBox1.Controls.Add(this.pResult);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5);
            this.groupBox1.Size = new System.Drawing.Size(346, 199);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Analysis";
            // 
            // pColors
            // 
            this.pColors.Controls.Add(this.LbColorAnalysis);
            this.pColors.Controls.Add(this.LbColorStatus);
            this.pColors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pColors.Location = new System.Drawing.Point(5, 123);
            this.pColors.Name = "pColors";
            this.pColors.Size = new System.Drawing.Size(336, 71);
            this.pColors.TabIndex = 5;
            // 
            // LbColorAnalysis
            // 
            this.LbColorAnalysis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LbColorAnalysis.Location = new System.Drawing.Point(0, 0);
            this.LbColorAnalysis.Name = "LbColorAnalysis";
            this.LbColorAnalysis.Size = new System.Drawing.Size(319, 71);
            this.LbColorAnalysis.TabIndex = 2;
            this.LbColorAnalysis.Text = "This creature has new colors";
            // 
            // LbColorStatus
            // 
            this.LbColorStatus.AutoSize = true;
            this.LbColorStatus.Dock = System.Windows.Forms.DockStyle.Right;
            this.LbColorStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbColorStatus.Location = new System.Drawing.Point(319, 0);
            this.LbColorStatus.Name = "LbColorStatus";
            this.LbColorStatus.Size = new System.Drawing.Size(17, 17);
            this.LbColorStatus.TabIndex = 4;
            this.LbColorStatus.Text = "✓";
            // 
            // pStats
            // 
            this.pStats.Controls.Add(this.LbStatAnalysis);
            this.pStats.Controls.Add(this.LbStatsStatus);
            this.pStats.Dock = System.Windows.Forms.DockStyle.Top;
            this.pStats.Location = new System.Drawing.Point(5, 68);
            this.pStats.Margin = new System.Windows.Forms.Padding(3, 3, 3, 30);
            this.pStats.Name = "pStats";
            this.pStats.Size = new System.Drawing.Size(336, 55);
            this.pStats.TabIndex = 6;
            // 
            // LbStatAnalysis
            // 
            this.LbStatAnalysis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LbStatAnalysis.Location = new System.Drawing.Point(0, 0);
            this.LbStatAnalysis.Name = "LbStatAnalysis";
            this.LbStatAnalysis.Size = new System.Drawing.Size(319, 55);
            this.LbStatAnalysis.TabIndex = 1;
            this.LbStatAnalysis.Text = "HP is good";
            // 
            // LbStatsStatus
            // 
            this.LbStatsStatus.AutoSize = true;
            this.LbStatsStatus.Dock = System.Windows.Forms.DockStyle.Right;
            this.LbStatsStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbStatsStatus.Location = new System.Drawing.Point(319, 0);
            this.LbStatsStatus.Name = "LbStatsStatus";
            this.LbStatsStatus.Size = new System.Drawing.Size(17, 17);
            this.LbStatsStatus.TabIndex = 4;
            this.LbStatsStatus.Text = "✓";
            // 
            // pResult
            // 
            this.pResult.Controls.Add(this.LbConclusion);
            this.pResult.Controls.Add(this.LbIcon);
            this.pResult.Dock = System.Windows.Forms.DockStyle.Top;
            this.pResult.Location = new System.Drawing.Point(5, 18);
            this.pResult.Name = "pResult";
            this.pResult.Size = new System.Drawing.Size(336, 50);
            this.pResult.TabIndex = 4;
            // 
            // LbConclusion
            // 
            this.LbConclusion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LbConclusion.Location = new System.Drawing.Point(0, 0);
            this.LbConclusion.Name = "LbConclusion";
            this.LbConclusion.Size = new System.Drawing.Size(291, 50);
            this.LbConclusion.TabIndex = 0;
            this.LbConclusion.Text = "Keep creature";
            // 
            // LbIcon
            // 
            this.LbIcon.AutoSize = true;
            this.LbIcon.Dock = System.Windows.Forms.DockStyle.Right;
            this.LbIcon.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbIcon.Location = new System.Drawing.Point(291, 0);
            this.LbIcon.Name = "LbIcon";
            this.LbIcon.Size = new System.Drawing.Size(45, 46);
            this.LbIcon.TabIndex = 3;
            this.LbIcon.Text = "✓";
            // 
            // CreatureAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "CreatureAnalysis";
            this.Size = new System.Drawing.Size(346, 199);
            this.groupBox1.ResumeLayout(false);
            this.pColors.ResumeLayout(false);
            this.pColors.PerformLayout();
            this.pStats.ResumeLayout(false);
            this.pStats.PerformLayout();
            this.pResult.ResumeLayout(false);
            this.pResult.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label LbColorAnalysis;
        private System.Windows.Forms.Label LbStatAnalysis;
        private System.Windows.Forms.Label LbConclusion;
        private System.Windows.Forms.Label LbIcon;
        private System.Windows.Forms.Panel pResult;
        private System.Windows.Forms.Panel pStats;
        private System.Windows.Forms.Label LbStatsStatus;
        private System.Windows.Forms.Panel pColors;
        private System.Windows.Forms.Label LbColorStatus;
    }
}
