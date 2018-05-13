namespace ARKBreedingStats
{
    partial class ExportedCreatureList
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.chooseFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateDataOfLibraryCreaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadServerSettingsOfFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.importAllUnimportedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chooseFolderToolStripMenuItem,
            this.updateDataOfLibraryCreaturesToolStripMenuItem,
            this.loadServerSettingsOfFolderToolStripMenuItem,
            this.importAllUnimportedToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(810, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // chooseFolderToolStripMenuItem
            // 
            this.chooseFolderToolStripMenuItem.Name = "chooseFolderToolStripMenuItem";
            this.chooseFolderToolStripMenuItem.Size = new System.Drawing.Size(104, 20);
            this.chooseFolderToolStripMenuItem.Text = "Choose Folder…";
            this.chooseFolderToolStripMenuItem.Click += new System.EventHandler(this.chooseFolderToolStripMenuItem_Click);
            // 
            // updateDataOfLibraryCreaturesToolStripMenuItem
            // 
            this.updateDataOfLibraryCreaturesToolStripMenuItem.Name = "updateDataOfLibraryCreaturesToolStripMenuItem";
            this.updateDataOfLibraryCreaturesToolStripMenuItem.Size = new System.Drawing.Size(196, 20);
            this.updateDataOfLibraryCreaturesToolStripMenuItem.Text = "Update Data of library Creatures…";
            this.updateDataOfLibraryCreaturesToolStripMenuItem.ToolTipText = "Use the exported data to update the Ancestors of library creatures";
            this.updateDataOfLibraryCreaturesToolStripMenuItem.Click += new System.EventHandler(this.updateDataOfLibraryCreaturesToolStripMenuItem_Click);
            // 
            // loadServerSettingsOfFolderToolStripMenuItem
            // 
            this.loadServerSettingsOfFolderToolStripMenuItem.Name = "loadServerSettingsOfFolderToolStripMenuItem";
            this.loadServerSettingsOfFolderToolStripMenuItem.Size = new System.Drawing.Size(170, 20);
            this.loadServerSettingsOfFolderToolStripMenuItem.Text = "load Server Settings of folder";
            this.loadServerSettingsOfFolderToolStripMenuItem.Click += new System.EventHandler(this.loadServerSettingsOfFolderToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(810, 376);
            this.panel1.TabIndex = 1;
            // 
            // importAllUnimportedToolStripMenuItem
            // 
            this.importAllUnimportedToolStripMenuItem.Name = "importAllUnimportedToolStripMenuItem";
            this.importAllUnimportedToolStripMenuItem.Size = new System.Drawing.Size(136, 20);
            this.importAllUnimportedToolStripMenuItem.Text = "Import all unimported";
            this.importAllUnimportedToolStripMenuItem.Click += new System.EventHandler(this.importAllUnimportedToolStripMenuItem_Click);
            // 
            // ExportedCreatureList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 400);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ExportedCreatureList";
            this.Text = "Exported Creatures";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem chooseFolderToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem updateDataOfLibraryCreaturesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadServerSettingsOfFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importAllUnimportedToolStripMenuItem;
    }
}