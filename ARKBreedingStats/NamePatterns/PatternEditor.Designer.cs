namespace ARKBreedingStats.NamePatterns
{
    partial class PatternEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatternEditor));
            this.txtboxPattern = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbPreview = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TabPageKeysFunctions = new System.Windows.Forms.TabPage();
            this.TlpKeysFunctions = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.TbFilterFunctions = new System.Windows.Forms.TextBox();
            this.BtClearFilterFunctions = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.TbFilterKeys = new System.Windows.Forms.TextBox();
            this.BtClearFilterKey = new System.Windows.Forms.Button();
            this.TabPagePatternTemplates = new System.Windows.Forms.TabPage();
            this.CbPatternNameToClipboardAfterManualApplication = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.TabPageKeysFunctions.SuspendLayout();
            this.TlpKeysFunctions.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtboxPattern
            // 
            this.txtboxPattern.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtboxPattern.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxPattern.Location = new System.Drawing.Point(0, 0);
            this.txtboxPattern.Multiline = true;
            this.txtboxPattern.Name = "txtboxPattern";
            this.txtboxPattern.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtboxPattern.Size = new System.Drawing.Size(678, 40);
            this.txtboxPattern.TabIndex = 0;
            this.txtboxPattern.TextChanged += new System.EventHandler(this.txtboxPattern_TextChanged);
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(696, 32);
            this.label2.TabIndex = 3;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonOK.Location = new System.Drawing.Point(621, 0);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 40);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonCancel.Location = new System.Drawing.Point(546, 0);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 40);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.Maroon;
            this.btnClear.Location = new System.Drawing.Point(678, 0);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(24, 40);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "×";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.linkLabel1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.CbPatternNameToClipboardAfterManualApplication, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(702, 630);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.buttonOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(696, 40);
            this.panel1.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbPreview);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(546, 40);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Preview";
            // 
            // cbPreview
            // 
            this.cbPreview.AutoSize = true;
            this.cbPreview.Checked = true;
            this.cbPreview.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPreview.Location = new System.Drawing.Point(9, 15);
            this.cbPreview.Name = "cbPreview";
            this.cbPreview.Size = new System.Drawing.Size(15, 14);
            this.cbPreview.TabIndex = 11;
            this.cbPreview.UseMnemonic = false;
            this.cbPreview.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(3, 106);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(185, 13);
            this.linkLabel1.TabIndex = 10;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "More infos about the Name-Generator";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.TabPageKeysFunctions);
            this.tabControl1.Controls.Add(this.TabPagePatternTemplates);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 129);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(696, 498);
            this.tabControl1.TabIndex = 16;
            // 
            // TabPageKeysFunctions
            // 
            this.TabPageKeysFunctions.Controls.Add(this.TlpKeysFunctions);
            this.TabPageKeysFunctions.Location = new System.Drawing.Point(4, 22);
            this.TabPageKeysFunctions.Name = "TabPageKeysFunctions";
            this.TabPageKeysFunctions.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageKeysFunctions.Size = new System.Drawing.Size(688, 472);
            this.TabPageKeysFunctions.TabIndex = 0;
            this.TabPageKeysFunctions.Text = "Keys and Functions";
            this.TabPageKeysFunctions.UseVisualStyleBackColor = true;
            // 
            // TlpKeysFunctions
            // 
            this.TlpKeysFunctions.ColumnCount = 2;
            this.TlpKeysFunctions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TlpKeysFunctions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TlpKeysFunctions.Controls.Add(this.panel3, 1, 0);
            this.TlpKeysFunctions.Controls.Add(this.panel2, 0, 0);
            this.TlpKeysFunctions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpKeysFunctions.Location = new System.Drawing.Point(3, 3);
            this.TlpKeysFunctions.Name = "TlpKeysFunctions";
            this.TlpKeysFunctions.RowCount = 2;
            this.TlpKeysFunctions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.TlpKeysFunctions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpKeysFunctions.Size = new System.Drawing.Size(682, 466);
            this.TlpKeysFunctions.TabIndex = 15;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.TbFilterFunctions);
            this.panel3.Controls.Add(this.BtClearFilterFunctions);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(344, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(335, 20);
            this.panel3.TabIndex = 14;
            // 
            // TbFilterFunctions
            // 
            this.TbFilterFunctions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TbFilterFunctions.Location = new System.Drawing.Point(0, 0);
            this.TbFilterFunctions.Name = "TbFilterFunctions";
            this.TbFilterFunctions.Size = new System.Drawing.Size(315, 20);
            this.TbFilterFunctions.TabIndex = 11;
            this.TbFilterFunctions.TextChanged += new System.EventHandler(this.TbFilterFunctions_TextChanged);
            // 
            // BtClearFilterFunctions
            // 
            this.BtClearFilterFunctions.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtClearFilterFunctions.Location = new System.Drawing.Point(315, 0);
            this.BtClearFilterFunctions.Name = "BtClearFilterFunctions";
            this.BtClearFilterFunctions.Size = new System.Drawing.Size(20, 20);
            this.BtClearFilterFunctions.TabIndex = 12;
            this.BtClearFilterFunctions.Text = "×";
            this.BtClearFilterFunctions.UseVisualStyleBackColor = true;
            this.BtClearFilterFunctions.Click += new System.EventHandler(this.BtClearFilterFunctions_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.TbFilterKeys);
            this.panel2.Controls.Add(this.BtClearFilterKey);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(335, 20);
            this.panel2.TabIndex = 13;
            // 
            // TbFilterKeys
            // 
            this.TbFilterKeys.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TbFilterKeys.Location = new System.Drawing.Point(0, 0);
            this.TbFilterKeys.Name = "TbFilterKeys";
            this.TbFilterKeys.Size = new System.Drawing.Size(315, 20);
            this.TbFilterKeys.TabIndex = 11;
            this.TbFilterKeys.TextChanged += new System.EventHandler(this.TbFilterKeys_TextChanged);
            // 
            // BtClearFilterKey
            // 
            this.BtClearFilterKey.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtClearFilterKey.Location = new System.Drawing.Point(315, 0);
            this.BtClearFilterKey.Name = "BtClearFilterKey";
            this.BtClearFilterKey.Size = new System.Drawing.Size(20, 20);
            this.BtClearFilterKey.TabIndex = 12;
            this.BtClearFilterKey.Text = "×";
            this.BtClearFilterKey.UseVisualStyleBackColor = true;
            this.BtClearFilterKey.Click += new System.EventHandler(this.BtClearFilterKey_Click);
            // 
            // TabPagePatternTemplates
            // 
            this.TabPagePatternTemplates.Location = new System.Drawing.Point(4, 22);
            this.TabPagePatternTemplates.Name = "TabPagePatternTemplates";
            this.TabPagePatternTemplates.Padding = new System.Windows.Forms.Padding(3);
            this.TabPagePatternTemplates.Size = new System.Drawing.Size(688, 472);
            this.TabPagePatternTemplates.TabIndex = 1;
            this.TabPagePatternTemplates.Text = "Templates";
            this.TabPagePatternTemplates.UseVisualStyleBackColor = true;
            // 
            // CbPatternNameToClipboardAfterManualApplication
            // 
            this.CbPatternNameToClipboardAfterManualApplication.AutoSize = true;
            this.CbPatternNameToClipboardAfterManualApplication.Location = new System.Drawing.Point(3, 81);
            this.CbPatternNameToClipboardAfterManualApplication.Name = "CbPatternNameToClipboardAfterManualApplication";
            this.CbPatternNameToClipboardAfterManualApplication.Size = new System.Drawing.Size(339, 17);
            this.CbPatternNameToClipboardAfterManualApplication.TabIndex = 17;
            this.CbPatternNameToClipboardAfterManualApplication.Text = "Copy generated name to clipboard after pattern is applied manually";
            this.CbPatternNameToClipboardAfterManualApplication.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtboxPattern);
            this.splitContainer1.Panel1.Controls.Add(this.btnClear);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(702, 674);
            this.splitContainer1.SplitterDistance = 40;
            this.splitContainer1.TabIndex = 8;
            // 
            // PatternEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(702, 674);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "PatternEditor";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Pattern Editor";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.TabPageKeysFunctions.ResumeLayout(false);
            this.TlpKeysFunctions.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtboxPattern;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbPreview;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox TbFilterFunctions;
        private System.Windows.Forms.Button BtClearFilterFunctions;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox TbFilterKeys;
        private System.Windows.Forms.Button BtClearFilterKey;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabPageKeysFunctions;
        private System.Windows.Forms.TableLayoutPanel TlpKeysFunctions;
        private System.Windows.Forms.TabPage TabPagePatternTemplates;
        private System.Windows.Forms.CheckBox CbPatternNameToClipboardAfterManualApplication;
    }
}