using ARKBreedingStats.uiControls;

namespace ARKBreedingStats.Pedigree
{
    partial class PedigreeCreature
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
            components = new System.ComponentModel.Container();
            groupBox1 = new GroupBoxC();
            labelMutations = new System.Windows.Forms.Label();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            labelCr = new System.Windows.Forms.Label();
            labelSp = new System.Windows.Forms.Label();
            labelDm = new System.Windows.Forms.Label();
            labelWe = new System.Windows.Forms.Label();
            labelFo = new System.Windows.Forms.Label();
            labelOx = new System.Windows.Forms.Label();
            labelSt = new System.Windows.Forms.Label();
            labelHP = new System.Windows.Forms.Label();
            labelSex = new System.Windows.Forms.Label();
            LbCrMut = new System.Windows.Forms.Label();
            LbHpMut = new System.Windows.Forms.Label();
            LbSpMut = new System.Windows.Forms.Label();
            LbStMut = new System.Windows.Forms.Label();
            LbDmMut = new System.Windows.Forms.Label();
            LbOxMut = new System.Windows.Forms.Label();
            LbWeMut = new System.Windows.Forms.Label();
            LbFoMut = new System.Windows.Forms.Label();
            panelHighlight = new System.Windows.Forms.Panel();
            panel1 = new System.Windows.Forms.Panel();
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            bestBreedingPartnersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            setCooldownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            removeCooldownGrowingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            TsMiViewInPedigree = new System.Windows.Forms.ToolStripMenuItem();
            editTraitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            copyNameToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItemCopyGeneratedNameToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            exportToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            plainTextbreedingValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            plainTextcurrentValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copyInfoGraphicToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openWikipageInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panelHighlight.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.BackColor = System.Drawing.SystemColors.Control;
            groupBox1.Controls.Add(labelMutations);
            groupBox1.Controls.Add(pictureBox1);
            groupBox1.Controls.Add(labelCr);
            groupBox1.Controls.Add(labelSp);
            groupBox1.Controls.Add(labelDm);
            groupBox1.Controls.Add(labelWe);
            groupBox1.Controls.Add(labelFo);
            groupBox1.Controls.Add(labelOx);
            groupBox1.Controls.Add(labelSt);
            groupBox1.Controls.Add(labelHP);
            groupBox1.Controls.Add(labelSex);
            groupBox1.Controls.Add(LbCrMut);
            groupBox1.Controls.Add(LbHpMut);
            groupBox1.Controls.Add(LbSpMut);
            groupBox1.Controls.Add(LbStMut);
            groupBox1.Controls.Add(LbDmMut);
            groupBox1.Controls.Add(LbOxMut);
            groupBox1.Controls.Add(LbWeMut);
            groupBox1.Controls.Add(LbFoMut);
            groupBox1.Controls.Add(panelHighlight);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox1.Location = new System.Drawing.Point(0, 0);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(0);
            groupBox1.Size = new System.Drawing.Size(379, 58);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.MouseClick += element_MouseClick;
            // 
            // labelMutations
            // 
            labelMutations.Location = new System.Drawing.Point(332, 18);
            labelMutations.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelMutations.Name = "labelMutations";
            labelMutations.Size = new System.Drawing.Size(43, 15);
            labelMutations.TabIndex = 10;
            labelMutations.Text = "Muta";
            labelMutations.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            labelMutations.MouseClick += element_MouseClick;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new System.Drawing.Point(299, 9);
            pictureBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(28, 28);
            pictureBox1.TabIndex = 9;
            pictureBox1.TabStop = false;
            pictureBox1.MouseClick += element_MouseClick;
            // 
            // labelCr
            // 
            labelCr.Location = new System.Drawing.Point(263, 18);
            labelCr.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelCr.Name = "labelCr";
            labelCr.Size = new System.Drawing.Size(33, 15);
            labelCr.TabIndex = 11;
            labelCr.Text = "Cr";
            labelCr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSp
            // 
            labelSp.Location = new System.Drawing.Point(229, 18);
            labelSp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelSp.Name = "labelSp";
            labelSp.Size = new System.Drawing.Size(33, 15);
            labelSp.TabIndex = 7;
            labelSp.Text = "Sp";
            labelSp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelDm
            // 
            labelDm.Location = new System.Drawing.Point(195, 18);
            labelDm.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelDm.Name = "labelDm";
            labelDm.Size = new System.Drawing.Size(33, 15);
            labelDm.TabIndex = 6;
            labelDm.Text = "Dm";
            labelDm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelWe
            // 
            labelWe.Location = new System.Drawing.Point(161, 18);
            labelWe.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelWe.Name = "labelWe";
            labelWe.Size = new System.Drawing.Size(33, 15);
            labelWe.TabIndex = 5;
            labelWe.Text = "We";
            labelWe.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelFo
            // 
            labelFo.Location = new System.Drawing.Point(127, 18);
            labelFo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelFo.Name = "labelFo";
            labelFo.Size = new System.Drawing.Size(33, 15);
            labelFo.TabIndex = 4;
            labelFo.Text = "Fo";
            labelFo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelOx
            // 
            labelOx.Location = new System.Drawing.Point(93, 18);
            labelOx.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelOx.Name = "labelOx";
            labelOx.Size = new System.Drawing.Size(33, 15);
            labelOx.TabIndex = 3;
            labelOx.Text = "Ox";
            labelOx.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSt
            // 
            labelSt.Location = new System.Drawing.Point(59, 18);
            labelSt.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelSt.Name = "labelSt";
            labelSt.Size = new System.Drawing.Size(33, 15);
            labelSt.TabIndex = 2;
            labelSt.Text = "St";
            labelSt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelHP
            // 
            labelHP.Location = new System.Drawing.Point(25, 18);
            labelHP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelHP.Name = "labelHP";
            labelHP.Size = new System.Drawing.Size(33, 15);
            labelHP.TabIndex = 1;
            labelHP.Text = "HP";
            labelHP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSex
            // 
            labelSex.Location = new System.Drawing.Point(7, 18);
            labelSex.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelSex.Name = "labelSex";
            labelSex.Size = new System.Drawing.Size(15, 15);
            labelSex.TabIndex = 0;
            labelSex.Text = "S";
            labelSex.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            labelSex.MouseClick += element_MouseClick;
            // 
            // LbCrMut
            // 
            LbCrMut.Location = new System.Drawing.Point(263, 35);
            LbCrMut.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbCrMut.Name = "LbCrMut";
            LbCrMut.Size = new System.Drawing.Size(33, 15);
            LbCrMut.TabIndex = 19;
            LbCrMut.Text = "Cr";
            LbCrMut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LbHpMut
            // 
            LbHpMut.BackColor = System.Drawing.SystemColors.Control;
            LbHpMut.Location = new System.Drawing.Point(25, 35);
            LbHpMut.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbHpMut.Name = "LbHpMut";
            LbHpMut.Size = new System.Drawing.Size(33, 15);
            LbHpMut.TabIndex = 12;
            LbHpMut.Text = "HP";
            LbHpMut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LbSpMut
            // 
            LbSpMut.Location = new System.Drawing.Point(229, 35);
            LbSpMut.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbSpMut.Name = "LbSpMut";
            LbSpMut.Size = new System.Drawing.Size(33, 15);
            LbSpMut.TabIndex = 18;
            LbSpMut.Text = "Sp";
            LbSpMut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LbStMut
            // 
            LbStMut.Location = new System.Drawing.Point(59, 35);
            LbStMut.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbStMut.Name = "LbStMut";
            LbStMut.Size = new System.Drawing.Size(33, 15);
            LbStMut.TabIndex = 13;
            LbStMut.Text = "St";
            LbStMut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LbDmMut
            // 
            LbDmMut.Location = new System.Drawing.Point(195, 35);
            LbDmMut.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbDmMut.Name = "LbDmMut";
            LbDmMut.Size = new System.Drawing.Size(33, 15);
            LbDmMut.TabIndex = 17;
            LbDmMut.Text = "Dm";
            LbDmMut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LbOxMut
            // 
            LbOxMut.Location = new System.Drawing.Point(93, 35);
            LbOxMut.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbOxMut.Name = "LbOxMut";
            LbOxMut.Size = new System.Drawing.Size(33, 15);
            LbOxMut.TabIndex = 14;
            LbOxMut.Text = "Ox";
            LbOxMut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LbWeMut
            // 
            LbWeMut.Location = new System.Drawing.Point(161, 35);
            LbWeMut.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbWeMut.Name = "LbWeMut";
            LbWeMut.Size = new System.Drawing.Size(33, 15);
            LbWeMut.TabIndex = 16;
            LbWeMut.Text = "We";
            LbWeMut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LbFoMut
            // 
            LbFoMut.Location = new System.Drawing.Point(127, 35);
            LbFoMut.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbFoMut.Name = "LbFoMut";
            LbFoMut.Size = new System.Drawing.Size(33, 15);
            LbFoMut.TabIndex = 15;
            LbFoMut.Text = "Fo";
            LbFoMut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelHighlight
            // 
            panelHighlight.BackColor = System.Drawing.SystemColors.Highlight;
            panelHighlight.Controls.Add(panel1);
            panelHighlight.Dock = System.Windows.Forms.DockStyle.Fill;
            panelHighlight.Location = new System.Drawing.Point(0, 16);
            panelHighlight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelHighlight.Name = "panelHighlight";
            panelHighlight.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelHighlight.Size = new System.Drawing.Size(379, 42);
            panelHighlight.TabIndex = 8;
            panelHighlight.Visible = false;
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(4, 3);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(371, 36);
            panel1.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { editToolStripMenuItem, bestBreedingPartnersToolStripMenuItem, setCooldownToolStripMenuItem, removeCooldownGrowingToolStripMenuItem, TsMiViewInPedigree, editTraitsToolStripMenuItem, toolStripSeparator1, copyNameToClipboardToolStripMenuItem, toolStripMenuItemCopyGeneratedNameToClipboard, exportToClipboardToolStripMenuItem, copyInfoGraphicToClipboardToolStripMenuItem, openWikipageInBrowserToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(259, 252);
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            editToolStripMenuItem.Text = "Edit...";
            editToolStripMenuItem.Click += editToolStripMenuItem_Click;
            // 
            // bestBreedingPartnersToolStripMenuItem
            // 
            bestBreedingPartnersToolStripMenuItem.Name = "bestBreedingPartnersToolStripMenuItem";
            bestBreedingPartnersToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            bestBreedingPartnersToolStripMenuItem.Text = "Best Breeding Partners...";
            bestBreedingPartnersToolStripMenuItem.Click += bestBreedingPartnersToolStripMenuItem_Click;
            // 
            // setCooldownToolStripMenuItem
            // 
            setCooldownToolStripMenuItem.Name = "setCooldownToolStripMenuItem";
            setCooldownToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            setCooldownToolStripMenuItem.Text = "Set Cooldown for next 2h";
            setCooldownToolStripMenuItem.Click += setCooldownToolStripMenuItem_Click;
            // 
            // removeCooldownGrowingToolStripMenuItem
            // 
            removeCooldownGrowingToolStripMenuItem.Name = "removeCooldownGrowingToolStripMenuItem";
            removeCooldownGrowingToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            removeCooldownGrowingToolStripMenuItem.Text = "Remove Cooldown / Growing";
            removeCooldownGrowingToolStripMenuItem.Click += removeCooldownGrowingToolStripMenuItem_Click;
            // 
            // TsMiViewInPedigree
            // 
            TsMiViewInPedigree.Name = "TsMiViewInPedigree";
            TsMiViewInPedigree.Size = new System.Drawing.Size(258, 22);
            TsMiViewInPedigree.Text = "View in Pedigree";
            TsMiViewInPedigree.Click += TsMiViewInPedigree_Click;
            // 
            // editTraitsToolStripMenuItem
            // 
            editTraitsToolStripMenuItem.Name = "editTraitsToolStripMenuItem";
            editTraitsToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            editTraitsToolStripMenuItem.Text = "Edit Traits…";
            editTraitsToolStripMenuItem.Click += editTraitsToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(255, 6);
            // 
            // copyNameToClipboardToolStripMenuItem
            // 
            copyNameToClipboardToolStripMenuItem.Name = "copyNameToClipboardToolStripMenuItem";
            copyNameToClipboardToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            copyNameToClipboardToolStripMenuItem.Text = "Copy Name to Clipboard";
            copyNameToClipboardToolStripMenuItem.Click += copyNameToClipboardToolStripMenuItem_Click;
            // 
            // toolStripMenuItemCopyGeneratedNameToClipboard
            // 
            toolStripMenuItemCopyGeneratedNameToClipboard.Name = "toolStripMenuItemCopyGeneratedNameToClipboard";
            toolStripMenuItemCopyGeneratedNameToClipboard.Size = new System.Drawing.Size(258, 22);
            toolStripMenuItemCopyGeneratedNameToClipboard.Text = "Copy generated name to clipboard";
            // 
            // exportToClipboardToolStripMenuItem
            // 
            exportToClipboardToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { plainTextbreedingValuesToolStripMenuItem, plainTextcurrentValuesToolStripMenuItem });
            exportToClipboardToolStripMenuItem.Name = "exportToClipboardToolStripMenuItem";
            exportToClipboardToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            exportToClipboardToolStripMenuItem.Text = "Export to Clipboard";
            // 
            // plainTextbreedingValuesToolStripMenuItem
            // 
            plainTextbreedingValuesToolStripMenuItem.Name = "plainTextbreedingValuesToolStripMenuItem";
            plainTextbreedingValuesToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            plainTextbreedingValuesToolStripMenuItem.Text = "Breeding Values";
            plainTextbreedingValuesToolStripMenuItem.Click += plainTextbreedingValuesToolStripMenuItem_Click;
            // 
            // plainTextcurrentValuesToolStripMenuItem
            // 
            plainTextcurrentValuesToolStripMenuItem.Name = "plainTextcurrentValuesToolStripMenuItem";
            plainTextcurrentValuesToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            plainTextcurrentValuesToolStripMenuItem.Text = "Current Values";
            plainTextcurrentValuesToolStripMenuItem.Click += plainTextcurrentValuesToolStripMenuItem_Click;
            // 
            // copyInfoGraphicToClipboardToolStripMenuItem
            // 
            copyInfoGraphicToClipboardToolStripMenuItem.Name = "copyInfoGraphicToClipboardToolStripMenuItem";
            copyInfoGraphicToClipboardToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            copyInfoGraphicToClipboardToolStripMenuItem.Text = "Copy InfoGraphic to Clipboard";
            copyInfoGraphicToClipboardToolStripMenuItem.Click += copyInfoGraphicToClipboardToolStripMenuItem_Click;
            // 
            // openWikipageInBrowserToolStripMenuItem
            // 
            openWikipageInBrowserToolStripMenuItem.Name = "openWikipageInBrowserToolStripMenuItem";
            openWikipageInBrowserToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            openWikipageInBrowserToolStripMenuItem.Text = "Open Wiki-page in Browser";
            openWikipageInBrowserToolStripMenuItem.Click += OpenWikipageInBrowserToolStripMenuItem_Click;
            // 
            // PedigreeCreature
            // 
            ContextMenuStrip = contextMenuStrip1;
            Controls.Add(groupBox1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "PedigreeCreature";
            Size = new System.Drawing.Size(379, 58);
            MouseClick += PedigreeCreature_MouseClick;
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panelHighlight.ResumeLayout(false);
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private GroupBoxC groupBox1;
        private System.Windows.Forms.Label labelCr;
        private System.Windows.Forms.Label labelSp;
        private System.Windows.Forms.Label labelDm;
        private System.Windows.Forms.Label labelWe;
        private System.Windows.Forms.Label labelFo;
        private System.Windows.Forms.Label labelOx;
        private System.Windows.Forms.Label labelSt;
        private System.Windows.Forms.Label labelHP;
        private System.Windows.Forms.Label labelSex;
        private System.Windows.Forms.Panel panelHighlight;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setCooldownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bestBreedingPartnersToolStripMenuItem;
        private System.Windows.Forms.Label labelMutations;
        private System.Windows.Forms.ToolStripMenuItem exportToClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plainTextbreedingValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plainTextcurrentValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeCooldownGrowingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openWikipageInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem TsMiViewInPedigree;
        private System.Windows.Forms.ToolStripMenuItem copyNameToClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyInfoGraphicToClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopyGeneratedNameToClipboard;
        private System.Windows.Forms.Label LbCrMut;
        private System.Windows.Forms.Label LbHpMut;
        private System.Windows.Forms.Label LbSpMut;
        private System.Windows.Forms.Label LbStMut;
        private System.Windows.Forms.Label LbDmMut;
        private System.Windows.Forms.Label LbOxMut;
        private System.Windows.Forms.Label LbWeMut;
        private System.Windows.Forms.Label LbFoMut;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem editTraitsToolStripMenuItem;
    }
}
