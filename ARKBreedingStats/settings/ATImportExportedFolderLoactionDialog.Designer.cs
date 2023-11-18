namespace ARKBreedingStats.settings
{
    partial class ATImportExportedFolderLocationDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ATImportExportedFolderLocationDialog));
            this.label_ConvenientName = new System.Windows.Forms.Label();
            this.textBox_ConvenientName = new System.Windows.Forms.TextBox();
            this.label_FolderPath = new System.Windows.Forms.Label();
            this.textBox_FolderPath = new System.Windows.Forms.TextBox();
            this.button_FileSelect = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_Ok = new System.Windows.Forms.Button();
            this.textBox_ownerSuffix = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_ConvenientName
            // 
            this.label_ConvenientName.AutoSize = true;
            this.label_ConvenientName.Location = new System.Drawing.Point(12, 9);
            this.label_ConvenientName.Name = "label_ConvenientName";
            this.label_ConvenientName.Size = new System.Drawing.Size(136, 13);
            this.label_ConvenientName.TabIndex = 0;
            this.label_ConvenientName.Text = "Convenient name (arbitrary)";
            // 
            // textBox_ConvenientName
            // 
            this.textBox_ConvenientName.Location = new System.Drawing.Point(12, 25);
            this.textBox_ConvenientName.Name = "textBox_ConvenientName";
            this.textBox_ConvenientName.Size = new System.Drawing.Size(203, 20);
            this.textBox_ConvenientName.TabIndex = 0;
            // 
            // label_FolderPath
            // 
            this.label_FolderPath.AutoSize = true;
            this.label_FolderPath.Location = new System.Drawing.Point(12, 48);
            this.label_FolderPath.Name = "label_FolderPath";
            this.label_FolderPath.Size = new System.Drawing.Size(104, 13);
            this.label_FolderPath.TabIndex = 4;
            this.label_FolderPath.Text = "Folderpath (required)";
            // 
            // textBox_FolderPath
            // 
            this.textBox_FolderPath.Location = new System.Drawing.Point(12, 64);
            this.textBox_FolderPath.Name = "textBox_FolderPath";
            this.textBox_FolderPath.Size = new System.Drawing.Size(395, 20);
            this.textBox_FolderPath.TabIndex = 2;
            // 
            // button_FileSelect
            // 
            this.button_FileSelect.Image = ((System.Drawing.Image)(resources.GetObject("button_FileSelect.Image")));
            this.button_FileSelect.Location = new System.Drawing.Point(413, 62);
            this.button_FileSelect.Name = "button_FileSelect";
            this.button_FileSelect.Size = new System.Drawing.Size(23, 23);
            this.button_FileSelect.TabIndex = 3;
            this.button_FileSelect.UseVisualStyleBackColor = true;
            this.button_FileSelect.Click += new System.EventHandler(this.button_FileSelect_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(280, 98);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 4;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            // 
            // button_Ok
            // 
            this.button_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_Ok.Location = new System.Drawing.Point(361, 98);
            this.button_Ok.Name = "button_Ok";
            this.button_Ok.Size = new System.Drawing.Size(75, 23);
            this.button_Ok.TabIndex = 5;
            this.button_Ok.Text = "OK";
            this.button_Ok.UseVisualStyleBackColor = true;
            // 
            // textBox_ownerSuffix
            // 
            this.textBox_ownerSuffix.Location = new System.Drawing.Point(221, 25);
            this.textBox_ownerSuffix.Name = "textBox_ownerSuffix";
            this.textBox_ownerSuffix.Size = new System.Drawing.Size(203, 20);
            this.textBox_ownerSuffix.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(221, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Owner suffix";
            // 
            // ATImportExportedFolderLocationDialog
            // 
            this.AcceptButton = this.button_Ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(448, 133);
            this.Controls.Add(this.textBox_ownerSuffix);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Ok);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_FileSelect);
            this.Controls.Add(this.textBox_FolderPath);
            this.Controls.Add(this.label_FolderPath);
            this.Controls.Add(this.textBox_ConvenientName);
            this.Controls.Add(this.label_ConvenientName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ATImportExportedFolderLocationDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit File Location";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_ConvenientName;
        private System.Windows.Forms.TextBox textBox_ConvenientName;
        private System.Windows.Forms.Label label_FolderPath;
        private System.Windows.Forms.TextBox textBox_FolderPath;
        private System.Windows.Forms.Button button_FileSelect;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Button button_Ok;
        private System.Windows.Forms.TextBox textBox_ownerSuffix;
        private System.Windows.Forms.Label label1;
    }
}