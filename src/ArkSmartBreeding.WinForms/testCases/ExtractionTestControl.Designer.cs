namespace ARKBreedingStats.testCases
{
    partial class ExtractionTestControl
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
            this.components = new System.ComponentModel.Container();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.contextMenuStripFile = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newTestfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTestfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveTestfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveTestfileAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbTestFile = new System.Windows.Forms.Label();
            this.btSaveTestFile = new System.Windows.Forms.Button();
            this.flowLayoutPanelTestCases = new System.Windows.Forms.FlowLayoutPanel();
            this.btRunAllTests = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.contextMenuStripFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.ContextMenuStrip = this.contextMenuStripFile;
            this.groupBox3.Controls.Add(this.lbTestFile);
            this.groupBox3.Controls.Add(this.btSaveTestFile);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(721, 41);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Test Case File (right-click for file-options)";
            // 
            // contextMenuStripFile
            // 
            this.contextMenuStripFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTestfileToolStripMenuItem,
            this.loadTestfileToolStripMenuItem,
            this.saveTestfileToolStripMenuItem,
            this.saveTestfileAsToolStripMenuItem});
            this.contextMenuStripFile.Name = "contextMenuStripFile";
            this.contextMenuStripFile.Size = new System.Drawing.Size(162, 92);
            // 
            // newTestfileToolStripMenuItem
            // 
            this.newTestfileToolStripMenuItem.Name = "newTestfileToolStripMenuItem";
            this.newTestfileToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.newTestfileToolStripMenuItem.Text = "New Testfile";
            this.newTestfileToolStripMenuItem.Click += new System.EventHandler(this.newTestfileToolStripMenuItem_Click);
            // 
            // loadTestfileToolStripMenuItem
            // 
            this.loadTestfileToolStripMenuItem.Name = "loadTestfileToolStripMenuItem";
            this.loadTestfileToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.loadTestfileToolStripMenuItem.Text = "Load Testfile...";
            this.loadTestfileToolStripMenuItem.Click += new System.EventHandler(this.loadTestfileToolStripMenuItem_Click);
            // 
            // saveTestfileToolStripMenuItem
            // 
            this.saveTestfileToolStripMenuItem.Name = "saveTestfileToolStripMenuItem";
            this.saveTestfileToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.saveTestfileToolStripMenuItem.Text = "Save Testfile";
            this.saveTestfileToolStripMenuItem.Click += new System.EventHandler(this.saveTestfileToolStripMenuItem_Click);
            // 
            // saveTestfileAsToolStripMenuItem
            // 
            this.saveTestfileAsToolStripMenuItem.Name = "saveTestfileAsToolStripMenuItem";
            this.saveTestfileAsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.saveTestfileAsToolStripMenuItem.Text = "Save Testfile as...";
            this.saveTestfileAsToolStripMenuItem.Click += new System.EventHandler(this.saveTestfileAsToolStripMenuItem_Click);
            // 
            // lbTestFile
            // 
            this.lbTestFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbTestFile.Location = new System.Drawing.Point(78, 16);
            this.lbTestFile.Name = "lbTestFile";
            this.lbTestFile.Size = new System.Drawing.Size(640, 22);
            this.lbTestFile.TabIndex = 1;
            this.lbTestFile.Text = "testfile";
            // 
            // btSaveTestFile
            // 
            this.btSaveTestFile.Dock = System.Windows.Forms.DockStyle.Left;
            this.btSaveTestFile.Location = new System.Drawing.Point(3, 16);
            this.btSaveTestFile.Name = "btSaveTestFile";
            this.btSaveTestFile.Size = new System.Drawing.Size(75, 22);
            this.btSaveTestFile.TabIndex = 0;
            this.btSaveTestFile.Text = "Save";
            this.btSaveTestFile.UseVisualStyleBackColor = true;
            this.btSaveTestFile.Click += new System.EventHandler(this.btSaveTestFile_Click);
            // 
            // flowLayoutPanelTestCases
            // 
            this.flowLayoutPanelTestCases.AutoScroll = true;
            this.flowLayoutPanelTestCases.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelTestCases.Location = new System.Drawing.Point(0, 64);
            this.flowLayoutPanelTestCases.Name = "flowLayoutPanelTestCases";
            this.flowLayoutPanelTestCases.Size = new System.Drawing.Size(721, 467);
            this.flowLayoutPanelTestCases.TabIndex = 1;
            // 
            // btRunAllTests
            // 
            this.btRunAllTests.Dock = System.Windows.Forms.DockStyle.Top;
            this.btRunAllTests.Location = new System.Drawing.Point(0, 41);
            this.btRunAllTests.Name = "btRunAllTests";
            this.btRunAllTests.Size = new System.Drawing.Size(721, 23);
            this.btRunAllTests.TabIndex = 0;
            this.btRunAllTests.Text = "Run all";
            this.btRunAllTests.UseVisualStyleBackColor = true;
            this.btRunAllTests.Click += new System.EventHandler(this.btRunAllTests_Click);
            // 
            // ExtractionTestControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanelTestCases);
            this.Controls.Add(this.btRunAllTests);
            this.Controls.Add(this.groupBox3);
            this.Name = "ExtractionTestControl";
            this.Size = new System.Drawing.Size(721, 531);
            this.groupBox3.ResumeLayout(false);
            this.contextMenuStripFile.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lbTestFile;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelTestCases;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripFile;
        private System.Windows.Forms.ToolStripMenuItem newTestfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadTestfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveTestfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveTestfileAsToolStripMenuItem;
        private System.Windows.Forms.Button btRunAllTests;
        private System.Windows.Forms.Button btSaveTestFile;
    }
}
