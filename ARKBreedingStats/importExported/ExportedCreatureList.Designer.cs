namespace ARKBreedingStats.importExported
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
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAllImportedFilesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteALLFilesInSelectedFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateDataOfLibraryCreaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadServerSettingsOfFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importAllUnimportedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripCbHideImported = new System.Windows.Forms.ToolStripMenuItem();
            this.setUserSuffixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chooseFolderToolStripMenuItem,
            this.openFolderToolStripMenuItem,
            this.toolStripMenuItem1,
            this.updateDataOfLibraryCreaturesToolStripMenuItem,
            this.loadServerSettingsOfFolderToolStripMenuItem,
            this.importAllUnimportedToolStripMenuItem,
            this.toolStripCbHideImported,
            this.setUserSuffixToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(956, 24);
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
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(82, 20);
            this.openFolderToolStripMenuItem.Text = "Open folder";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteAllImportedFilesToolStripMenuItem1,
            this.deleteALLFilesInSelectedFolderToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(42, 20);
            this.toolStripMenuItem1.Text = "Files";
            // 
            // deleteAllImportedFilesToolStripMenuItem1
            // 
            this.deleteAllImportedFilesToolStripMenuItem1.Name = "deleteAllImportedFilesToolStripMenuItem1";
            this.deleteAllImportedFilesToolStripMenuItem1.Size = new System.Drawing.Size(247, 22);
            this.deleteAllImportedFilesToolStripMenuItem1.Text = "Delete all imported files";
            this.deleteAllImportedFilesToolStripMenuItem1.Click += new System.EventHandler(this.deleteAllImportedFilesToolStripMenuItem_Click);
            // 
            // deleteALLFilesInSelectedFolderToolStripMenuItem
            // 
            this.deleteALLFilesInSelectedFolderToolStripMenuItem.Name = "deleteALLFilesInSelectedFolderToolStripMenuItem";
            this.deleteALLFilesInSelectedFolderToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.deleteALLFilesInSelectedFolderToolStripMenuItem.Text = "Delete ALL files in selected folder";
            this.deleteALLFilesInSelectedFolderToolStripMenuItem.Click += new System.EventHandler(this.deleteAllFilesToolStripMenuItem_Click);
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
            // importAllUnimportedToolStripMenuItem
            // 
            this.importAllUnimportedToolStripMenuItem.Name = "importAllUnimportedToolStripMenuItem";
            this.importAllUnimportedToolStripMenuItem.Size = new System.Drawing.Size(136, 20);
            this.importAllUnimportedToolStripMenuItem.Text = "Import all unimported";
            this.importAllUnimportedToolStripMenuItem.Click += new System.EventHandler(this.importAllUnimportedToolStripMenuItem_Click);
            // 
            // toolStripCbHideImported
            // 
            this.toolStripCbHideImported.CheckOnClick = true;
            this.toolStripCbHideImported.Name = "toolStripCbHideImported";
            this.toolStripCbHideImported.Size = new System.Drawing.Size(96, 20);
            this.toolStripCbHideImported.Text = "Hide Imported";
            this.toolStripCbHideImported.Click += new System.EventHandler(this.toolStripCbHideImported_Click);
            // 
            // setUserSuffixToolStripMenuItem
            // 
            this.setUserSuffixToolStripMenuItem.Name = "setUserSuffixToolStripMenuItem";
            this.setUserSuffixToolStripMenuItem.Size = new System.Drawing.Size(92, 20);
            this.setUserSuffixToolStripMenuItem.Text = "Set User suffix";
            this.setUserSuffixToolStripMenuItem.Click += new System.EventHandler(this.setUserSuffixToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 620);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(956, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(956, 596);
            this.panel1.TabIndex = 4;
            // 
            // ExportedCreatureList
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(956, 642);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ExportedCreatureList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Exported Creatures";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ExportedCreatureList_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ExportedCreatureList_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem chooseFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateDataOfLibraryCreaturesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadServerSettingsOfFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importAllUnimportedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripCbHideImported;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteAllImportedFilesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteALLFilesInSelectedFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setUserSuffixToolStripMenuItem;
    }
}