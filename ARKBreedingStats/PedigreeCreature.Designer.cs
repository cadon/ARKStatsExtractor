﻿namespace ARKBreedingStats
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelMutations = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelSp = new System.Windows.Forms.Label();
            this.labelDm = new System.Windows.Forms.Label();
            this.labelWe = new System.Windows.Forms.Label();
            this.labelFo = new System.Windows.Forms.Label();
            this.labelOx = new System.Windows.Forms.Label();
            this.labelSt = new System.Windows.Forms.Label();
            this.labelHP = new System.Windows.Forms.Label();
            this.labelSex = new System.Windows.Forms.Label();
            this.panelHighlight = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bestBreedingPartnersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setCooldownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeCooldownGrowingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aRKChatbreedingValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aRKChatcurrentValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plainTextbreedingValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plainTextcurrentValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelMutations);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.labelSp);
            this.groupBox1.Controls.Add(this.labelDm);
            this.groupBox1.Controls.Add(this.labelWe);
            this.groupBox1.Controls.Add(this.labelFo);
            this.groupBox1.Controls.Add(this.labelOx);
            this.groupBox1.Controls.Add(this.labelSt);
            this.groupBox1.Controls.Add(this.labelHP);
            this.groupBox1.Controls.Add(this.labelSex);
            this.groupBox1.Controls.Add(this.panelHighlight);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(296, 35);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.element_MouseClick);
            // 
            // labelMutations
            // 
            this.labelMutations.Location = new System.Drawing.Point(253, 13);
            this.labelMutations.Name = "labelMutations";
            this.labelMutations.Size = new System.Drawing.Size(37, 16);
            this.labelMutations.TabIndex = 10;
            this.labelMutations.Text = "Muta";
            this.labelMutations.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelMutations.MouseClick += new System.Windows.Forms.MouseEventHandler(this.element_MouseClick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(227, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 24);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.element_MouseClick);
            // 
            // labelSp
            // 
            this.labelSp.Location = new System.Drawing.Point(196, 16);
            this.labelSp.Name = "labelSp";
            this.labelSp.Size = new System.Drawing.Size(28, 13);
            this.labelSp.TabIndex = 7;
            this.labelSp.Text = "Sp";
            this.labelSp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelSp.MouseClick += new System.Windows.Forms.MouseEventHandler(this.element_MouseClick);
            // 
            // labelDm
            // 
            this.labelDm.Location = new System.Drawing.Point(167, 16);
            this.labelDm.Name = "labelDm";
            this.labelDm.Size = new System.Drawing.Size(28, 13);
            this.labelDm.TabIndex = 6;
            this.labelDm.Text = "Dm";
            this.labelDm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelDm.MouseClick += new System.Windows.Forms.MouseEventHandler(this.element_MouseClick);
            // 
            // labelWe
            // 
            this.labelWe.Location = new System.Drawing.Point(138, 16);
            this.labelWe.Name = "labelWe";
            this.labelWe.Size = new System.Drawing.Size(28, 13);
            this.labelWe.TabIndex = 5;
            this.labelWe.Text = "We";
            this.labelWe.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelWe.MouseClick += new System.Windows.Forms.MouseEventHandler(this.element_MouseClick);
            // 
            // labelFo
            // 
            this.labelFo.Location = new System.Drawing.Point(109, 16);
            this.labelFo.Name = "labelFo";
            this.labelFo.Size = new System.Drawing.Size(28, 13);
            this.labelFo.TabIndex = 4;
            this.labelFo.Text = "Fo";
            this.labelFo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelFo.MouseClick += new System.Windows.Forms.MouseEventHandler(this.element_MouseClick);
            // 
            // labelOx
            // 
            this.labelOx.Location = new System.Drawing.Point(80, 16);
            this.labelOx.Name = "labelOx";
            this.labelOx.Size = new System.Drawing.Size(28, 13);
            this.labelOx.TabIndex = 3;
            this.labelOx.Text = "Ox";
            this.labelOx.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelOx.MouseClick += new System.Windows.Forms.MouseEventHandler(this.element_MouseClick);
            // 
            // labelSt
            // 
            this.labelSt.Location = new System.Drawing.Point(51, 16);
            this.labelSt.Name = "labelSt";
            this.labelSt.Size = new System.Drawing.Size(28, 13);
            this.labelSt.TabIndex = 2;
            this.labelSt.Text = "St";
            this.labelSt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelSt.MouseClick += new System.Windows.Forms.MouseEventHandler(this.element_MouseClick);
            // 
            // labelHP
            // 
            this.labelHP.Location = new System.Drawing.Point(22, 16);
            this.labelHP.Name = "labelHP";
            this.labelHP.Size = new System.Drawing.Size(28, 13);
            this.labelHP.TabIndex = 1;
            this.labelHP.Text = "HP";
            this.labelHP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelHP.MouseClick += new System.Windows.Forms.MouseEventHandler(this.element_MouseClick);
            // 
            // labelSex
            // 
            this.labelSex.Location = new System.Drawing.Point(6, 16);
            this.labelSex.Name = "labelSex";
            this.labelSex.Size = new System.Drawing.Size(13, 13);
            this.labelSex.TabIndex = 0;
            this.labelSex.Text = "S";
            this.labelSex.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelSex.MouseClick += new System.Windows.Forms.MouseEventHandler(this.element_MouseClick);
            // 
            // panelHighlight
            // 
            this.panelHighlight.BackColor = System.Drawing.SystemColors.Highlight;
            this.panelHighlight.Location = new System.Drawing.Point(3, 13);
            this.panelHighlight.Name = "panelHighlight";
            this.panelHighlight.Size = new System.Drawing.Size(223, 19);
            this.panelHighlight.TabIndex = 8;
            this.panelHighlight.Visible = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.bestBreedingPartnersToolStripMenuItem,
            this.setCooldownToolStripMenuItem,
            this.removeCooldownGrowingToolStripMenuItem,
            this.exportToClipboardToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(232, 114);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.editToolStripMenuItem.Text = "Edit...";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // bestBreedingPartnersToolStripMenuItem
            // 
            this.bestBreedingPartnersToolStripMenuItem.Name = "bestBreedingPartnersToolStripMenuItem";
            this.bestBreedingPartnersToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.bestBreedingPartnersToolStripMenuItem.Text = "Best Breeding Partners...";
            this.bestBreedingPartnersToolStripMenuItem.Click += new System.EventHandler(this.bestBreedingPartnersToolStripMenuItem_Click);
            // 
            // setCooldownToolStripMenuItem
            // 
            this.setCooldownToolStripMenuItem.Name = "setCooldownToolStripMenuItem";
            this.setCooldownToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.setCooldownToolStripMenuItem.Text = "Set Cooldown for next 2h";
            this.setCooldownToolStripMenuItem.Click += new System.EventHandler(this.setCooldownToolStripMenuItem_Click);
            // 
            // removeCooldownGrowingToolStripMenuItem
            // 
            this.removeCooldownGrowingToolStripMenuItem.Name = "removeCooldownGrowingToolStripMenuItem";
            this.removeCooldownGrowingToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.removeCooldownGrowingToolStripMenuItem.Text = "Remove Cooldown / Growing";
            this.removeCooldownGrowingToolStripMenuItem.Click += new System.EventHandler(this.removeCooldownGrowingToolStripMenuItem_Click);
            // 
            // exportToClipboardToolStripMenuItem
            // 
            this.exportToClipboardToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aRKChatbreedingValuesToolStripMenuItem,
            this.aRKChatcurrentValuesToolStripMenuItem,
            this.plainTextbreedingValuesToolStripMenuItem,
            this.plainTextcurrentValuesToolStripMenuItem});
            this.exportToClipboardToolStripMenuItem.Name = "exportToClipboardToolStripMenuItem";
            this.exportToClipboardToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.exportToClipboardToolStripMenuItem.Text = "Export to Clipboard";
            // 
            // aRKChatbreedingValuesToolStripMenuItem
            // 
            this.aRKChatbreedingValuesToolStripMenuItem.Name = "aRKChatbreedingValuesToolStripMenuItem";
            this.aRKChatbreedingValuesToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.aRKChatbreedingValuesToolStripMenuItem.Text = "ARK-Chat (breeding values)";
            this.aRKChatbreedingValuesToolStripMenuItem.Click += new System.EventHandler(this.aRKChatbreedingValuesToolStripMenuItem_Click);
            // 
            // aRKChatcurrentValuesToolStripMenuItem
            // 
            this.aRKChatcurrentValuesToolStripMenuItem.Name = "aRKChatcurrentValuesToolStripMenuItem";
            this.aRKChatcurrentValuesToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.aRKChatcurrentValuesToolStripMenuItem.Text = "ARK-Chat (current values)";
            this.aRKChatcurrentValuesToolStripMenuItem.Click += new System.EventHandler(this.aRKChatcurrentValuesToolStripMenuItem_Click);
            // 
            // plainTextbreedingValuesToolStripMenuItem
            // 
            this.plainTextbreedingValuesToolStripMenuItem.Name = "plainTextbreedingValuesToolStripMenuItem";
            this.plainTextbreedingValuesToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.plainTextbreedingValuesToolStripMenuItem.Text = "Plain Text (breeding values)";
            this.plainTextbreedingValuesToolStripMenuItem.Click += new System.EventHandler(this.plainTextbreedingValuesToolStripMenuItem_Click);
            // 
            // plainTextcurrentValuesToolStripMenuItem
            // 
            this.plainTextcurrentValuesToolStripMenuItem.Name = "plainTextcurrentValuesToolStripMenuItem";
            this.plainTextcurrentValuesToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.plainTextcurrentValuesToolStripMenuItem.Text = "Plain Text (current values)";
            this.plainTextcurrentValuesToolStripMenuItem.Click += new System.EventHandler(this.plainTextcurrentValuesToolStripMenuItem_Click);
            // 
            // PedigreeCreature
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.groupBox1);
            this.Name = "PedigreeCreature";
            this.Size = new System.Drawing.Size(296, 35);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PedigreeCreature_MouseClick);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
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
        private System.Windows.Forms.ToolStripMenuItem aRKChatbreedingValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aRKChatcurrentValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plainTextbreedingValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plainTextcurrentValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeCooldownGrowingToolStripMenuItem;
    }
}
