namespace ARKBreedingStats
{
    partial class AboutBox1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            labelProductName = new System.Windows.Forms.Label();
            labelVersion = new System.Windows.Forms.Label();
            labelCopyright = new System.Windows.Forms.Label();
            linkLabel = new System.Windows.Forms.LinkLabel();
            okButton = new System.Windows.Forms.Button();
            labelDescription = new System.Windows.Forms.Label();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            textBoxContributors = new System.Windows.Forms.TextBox();
            tabPage2 = new System.Windows.Forms.TabPage();
            TbDependencies = new System.Windows.Forms.TextBox();
            tableLayoutPanel.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 117F));
            tableLayoutPanel.Controls.Add(labelProductName, 0, 0);
            tableLayoutPanel.Controls.Add(labelVersion, 0, 1);
            tableLayoutPanel.Controls.Add(labelCopyright, 0, 2);
            tableLayoutPanel.Controls.Add(linkLabel, 0, 3);
            tableLayoutPanel.Controls.Add(okButton, 1, 6);
            tableLayoutPanel.Controls.Add(labelDescription, 0, 4);
            tableLayoutPanel.Controls.Add(tabControl1, 0, 5);
            tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel.Location = new System.Drawing.Point(10, 10);
            tableLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 7;
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            tableLayoutPanel.Size = new System.Drawing.Size(600, 634);
            tableLayoutPanel.TabIndex = 0;
            // 
            // labelProductName
            // 
            tableLayoutPanel.SetColumnSpan(labelProductName, 2);
            labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            labelProductName.Location = new System.Drawing.Point(7, 0);
            labelProductName.Margin = new System.Windows.Forms.Padding(7, 0, 4, 0);
            labelProductName.MaximumSize = new System.Drawing.Size(0, 20);
            labelProductName.Name = "labelProductName";
            labelProductName.Size = new System.Drawing.Size(589, 20);
            labelProductName.TabIndex = 19;
            labelProductName.Text = "Produktname";
            labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelVersion
            // 
            tableLayoutPanel.SetColumnSpan(labelVersion, 2);
            labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            labelVersion.Location = new System.Drawing.Point(7, 35);
            labelVersion.Margin = new System.Windows.Forms.Padding(7, 0, 4, 0);
            labelVersion.MaximumSize = new System.Drawing.Size(0, 20);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new System.Drawing.Size(589, 20);
            labelVersion.TabIndex = 0;
            labelVersion.Text = "Version";
            labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCopyright
            // 
            tableLayoutPanel.SetColumnSpan(labelCopyright, 2);
            labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
            labelCopyright.Location = new System.Drawing.Point(7, 70);
            labelCopyright.Margin = new System.Windows.Forms.Padding(7, 0, 4, 0);
            labelCopyright.MaximumSize = new System.Drawing.Size(0, 20);
            labelCopyright.Name = "labelCopyright";
            labelCopyright.Size = new System.Drawing.Size(589, 20);
            labelCopyright.TabIndex = 21;
            labelCopyright.Text = "Copyright";
            labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel
            // 
            linkLabel.AutoSize = true;
            tableLayoutPanel.SetColumnSpan(linkLabel, 2);
            linkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            linkLabel.Location = new System.Drawing.Point(7, 105);
            linkLabel.Margin = new System.Windows.Forms.Padding(7, 0, 4, 0);
            linkLabel.MaximumSize = new System.Drawing.Size(0, 20);
            linkLabel.Name = "linkLabel";
            linkLabel.Size = new System.Drawing.Size(589, 20);
            linkLabel.TabIndex = 25;
            linkLabel.TabStop = true;
            linkLabel.Text = "ARK Smart Breeding: Check for more info and new versions";
            linkLabel.LinkClicked += linkLabel_LinkClicked;
            // 
            // okButton
            // 
            okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            okButton.Location = new System.Drawing.Point(508, 604);
            okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(88, 27);
            okButton.TabIndex = 24;
            okButton.Text = "Close";
            okButton.Click += okButton_Click;
            // 
            // labelDescription
            // 
            labelDescription.AutoSize = true;
            tableLayoutPanel.SetColumnSpan(labelDescription, 2);
            labelDescription.Location = new System.Drawing.Point(4, 140);
            labelDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelDescription.Name = "labelDescription";
            labelDescription.Size = new System.Drawing.Size(67, 15);
            labelDescription.TabIndex = 26;
            labelDescription.Text = "Description";
            // 
            // tabControl1
            // 
            tableLayoutPanel.SetColumnSpan(tabControl1, 2);
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(4, 178);
            tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(592, 407);
            tabControl1.TabIndex = 27;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(textBoxContributors);
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage1.Size = new System.Drawing.Size(584, 379);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Contributors";
            // 
            // textBoxContributors
            // 
            textBoxContributors.AcceptsReturn = true;
            textBoxContributors.Dock = System.Windows.Forms.DockStyle.Fill;
            textBoxContributors.Location = new System.Drawing.Point(4, 3);
            textBoxContributors.Margin = new System.Windows.Forms.Padding(7, 3, 4, 3);
            textBoxContributors.Multiline = true;
            textBoxContributors.Name = "textBoxContributors";
            textBoxContributors.ReadOnly = true;
            textBoxContributors.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            textBoxContributors.Size = new System.Drawing.Size(576, 373);
            textBoxContributors.TabIndex = 23;
            textBoxContributors.TabStop = false;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(TbDependencies);
            tabPage2.Location = new System.Drawing.Point(4, 24);
            tabPage2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage2.Size = new System.Drawing.Size(584, 379);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Dependencies";
            // 
            // TbDependencies
            // 
            TbDependencies.AcceptsReturn = true;
            TbDependencies.Dock = System.Windows.Forms.DockStyle.Fill;
            TbDependencies.Location = new System.Drawing.Point(4, 3);
            TbDependencies.Margin = new System.Windows.Forms.Padding(7, 3, 4, 3);
            TbDependencies.Multiline = true;
            TbDependencies.Name = "TbDependencies";
            TbDependencies.ReadOnly = true;
            TbDependencies.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            TbDependencies.Size = new System.Drawing.Size(576, 373);
            TbDependencies.TabIndex = 24;
            TbDependencies.TabStop = false;
            // 
            // AboutBox1
            // 
            AcceptButton = okButton;
            ClientSize = new System.Drawing.Size(620, 654);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutBox1";
            Padding = new System.Windows.Forms.Padding(10);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "AboutBox1";
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.TextBox textBoxContributors;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.LinkLabel linkLabel;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox TbDependencies;
    }
}
