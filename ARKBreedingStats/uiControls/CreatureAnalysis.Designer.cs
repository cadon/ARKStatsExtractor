
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TlpRegionInfo = new System.Windows.Forms.TableLayoutPanel();
            this.LbCreatureCountHeader = new System.Windows.Forms.Label();
            this.LbColorIdHeader = new System.Windows.Forms.Label();
            this.LbRegionHeader = new System.Windows.Forms.Label();
            this.LbViewInLibraryHeader = new System.Windows.Forms.Label();
            this.LbColorStatus = new System.Windows.Forms.Label();
            this.LbColorAnalysis = new System.Windows.Forms.Label();
            this.LbStatsStatus = new System.Windows.Forms.Label();
            this.LbStatAnalysis = new System.Windows.Forms.Label();
            this.LbIcon = new System.Windows.Forms.Label();
            this.LbConclusion = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.TlpRegionInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5);
            this.groupBox1.Size = new System.Drawing.Size(261, 271);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Analysis";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.TlpRegionInfo, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.LbColorStatus, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.LbColorAnalysis, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.LbStatsStatus, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.LbStatAnalysis, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.LbIcon, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.LbConclusion, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 18);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(251, 248);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // TlpRegionInfo
            // 
            this.TlpRegionInfo.AutoSize = true;
            this.TlpRegionInfo.ColumnCount = 5;
            this.tableLayoutPanel1.SetColumnSpan(this.TlpRegionInfo, 2);
            this.TlpRegionInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpRegionInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpRegionInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpRegionInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpRegionInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpRegionInfo.Controls.Add(this.LbCreatureCountHeader, 2, 0);
            this.TlpRegionInfo.Controls.Add(this.LbColorIdHeader, 1, 0);
            this.TlpRegionInfo.Controls.Add(this.LbRegionHeader, 0, 0);
            this.TlpRegionInfo.Controls.Add(this.LbViewInLibraryHeader, 3, 0);
            this.TlpRegionInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.TlpRegionInfo.Location = new System.Drawing.Point(3, 95);
            this.TlpRegionInfo.Name = "TlpRegionInfo";
            this.TlpRegionInfo.RowCount = 1;
            this.TlpRegionInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpRegionInfo.Size = new System.Drawing.Size(245, 13);
            this.TlpRegionInfo.TabIndex = 8;
            // 
            // LbCreatureCountHeader
            // 
            this.LbCreatureCountHeader.AutoSize = true;
            this.LbCreatureCountHeader.Location = new System.Drawing.Point(98, 0);
            this.LbCreatureCountHeader.Name = "LbCreatureCountHeader";
            this.LbCreatureCountHeader.Size = new System.Drawing.Size(52, 13);
            this.LbCreatureCountHeader.TabIndex = 1;
            this.LbCreatureCountHeader.Text = "Creatures";
            // 
            // LbColorIdHeader
            // 
            this.LbColorIdHeader.AutoSize = true;
            this.LbColorIdHeader.Location = new System.Drawing.Point(50, 0);
            this.LbColorIdHeader.Name = "LbColorIdHeader";
            this.LbColorIdHeader.Size = new System.Drawing.Size(42, 13);
            this.LbColorIdHeader.TabIndex = 2;
            this.LbColorIdHeader.Text = "Color id";
            // 
            // LbRegionHeader
            // 
            this.LbRegionHeader.AutoSize = true;
            this.LbRegionHeader.Location = new System.Drawing.Point(3, 0);
            this.LbRegionHeader.Name = "LbRegionHeader";
            this.LbRegionHeader.Size = new System.Drawing.Size(41, 13);
            this.LbRegionHeader.TabIndex = 3;
            this.LbRegionHeader.Text = "Region";
            // 
            // LbViewInLibraryHeader
            // 
            this.LbViewInLibraryHeader.AutoSize = true;
            this.LbViewInLibraryHeader.Location = new System.Drawing.Point(156, 0);
            this.LbViewInLibraryHeader.Name = "LbViewInLibraryHeader";
            this.LbViewInLibraryHeader.Size = new System.Drawing.Size(71, 13);
            this.LbViewInLibraryHeader.TabIndex = 4;
            this.LbViewInLibraryHeader.Text = "View in library";
            // 
            // LbColorStatus
            // 
            this.LbColorStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LbColorStatus.AutoSize = true;
            this.LbColorStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbColorStatus.Location = new System.Drawing.Point(232, 73);
            this.LbColorStatus.Margin = new System.Windows.Forms.Padding(2);
            this.LbColorStatus.Name = "LbColorStatus";
            this.LbColorStatus.Size = new System.Drawing.Size(17, 17);
            this.LbColorStatus.TabIndex = 4;
            this.LbColorStatus.Text = "✓";
            // 
            // LbColorAnalysis
            // 
            this.LbColorAnalysis.AutoSize = true;
            this.LbColorAnalysis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LbColorAnalysis.Location = new System.Drawing.Point(3, 71);
            this.LbColorAnalysis.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this.LbColorAnalysis.Name = "LbColorAnalysis";
            this.LbColorAnalysis.Size = new System.Drawing.Size(196, 16);
            this.LbColorAnalysis.TabIndex = 2;
            this.LbColorAnalysis.Text = "This creature has new colors";
            // 
            // LbStatsStatus
            // 
            this.LbStatsStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LbStatsStatus.AutoSize = true;
            this.LbStatsStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbStatsStatus.Location = new System.Drawing.Point(232, 52);
            this.LbStatsStatus.Margin = new System.Windows.Forms.Padding(2);
            this.LbStatsStatus.Name = "LbStatsStatus";
            this.LbStatsStatus.Size = new System.Drawing.Size(17, 17);
            this.LbStatsStatus.TabIndex = 4;
            this.LbStatsStatus.Text = "✓";
            // 
            // LbStatAnalysis
            // 
            this.LbStatAnalysis.AutoSize = true;
            this.LbStatAnalysis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LbStatAnalysis.Location = new System.Drawing.Point(3, 50);
            this.LbStatAnalysis.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this.LbStatAnalysis.Name = "LbStatAnalysis";
            this.LbStatAnalysis.Size = new System.Drawing.Size(196, 16);
            this.LbStatAnalysis.TabIndex = 1;
            this.LbStatAnalysis.Text = "HP is good";
            // 
            // LbIcon
            // 
            this.LbIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LbIcon.AutoSize = true;
            this.LbIcon.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbIcon.Location = new System.Drawing.Point(204, 2);
            this.LbIcon.Margin = new System.Windows.Forms.Padding(2);
            this.LbIcon.Name = "LbIcon";
            this.LbIcon.Size = new System.Drawing.Size(45, 46);
            this.LbIcon.TabIndex = 3;
            this.LbIcon.Text = "✓";
            // 
            // LbConclusion
            // 
            this.LbConclusion.AutoSize = true;
            this.LbConclusion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LbConclusion.Location = new System.Drawing.Point(3, 0);
            this.LbConclusion.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this.LbConclusion.Name = "LbConclusion";
            this.LbConclusion.Size = new System.Drawing.Size(196, 45);
            this.LbConclusion.TabIndex = 0;
            this.LbConclusion.Text = "Keep creature";
            // 
            // CreatureAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "CreatureAnalysis";
            this.Size = new System.Drawing.Size(261, 271);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.TlpRegionInfo.ResumeLayout(false);
            this.TlpRegionInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label LbColorAnalysis;
        private System.Windows.Forms.Label LbStatAnalysis;
        private System.Windows.Forms.Label LbConclusion;
        private System.Windows.Forms.Label LbIcon;
        private System.Windows.Forms.Label LbStatsStatus;
        private System.Windows.Forms.Label LbColorStatus;
        private System.Windows.Forms.TableLayoutPanel TlpRegionInfo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label LbRegionHeader;
        private System.Windows.Forms.Label LbCreatureCountHeader;
        private System.Windows.Forms.Label LbColorIdHeader;
        private System.Windows.Forms.Label LbViewInLibraryHeader;
    }
}
