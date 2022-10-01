namespace ARKBreedingStats.settings {
    partial class ATImportFileLocationDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ATImportFileLocationDialog));
            this.label_ConvenientName = new System.Windows.Forms.Label();
            this.textBox_ConvenientName = new System.Windows.Forms.TextBox();
            this.label_ServerName = new System.Windows.Forms.Label();
            this.textBox_ServerName = new System.Windows.Forms.TextBox();
            this.label_FileLocation = new System.Windows.Forms.Label();
            this.textBox_FileLocation = new System.Windows.Forms.TextBox();
            this.button_FileSelect = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_Ok = new System.Windows.Forms.Button();
            this.LlFtpHelp = new System.Windows.Forms.LinkLabel();
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
            this.textBox_ConvenientName.TabIndex = 1;
            // 
            // label_ServerName
            // 
            this.label_ServerName.AutoSize = true;
            this.label_ServerName.Location = new System.Drawing.Point(230, 9);
            this.label_ServerName.Name = "label_ServerName";
            this.label_ServerName.Size = new System.Drawing.Size(195, 13);
            this.label_ServerName.TabIndex = 2;
            this.label_ServerName.Text = "Server name (set for imported creatures)";
            // 
            // textBox_ServerName
            // 
            this.textBox_ServerName.Location = new System.Drawing.Point(233, 25);
            this.textBox_ServerName.Name = "textBox_ServerName";
            this.textBox_ServerName.Size = new System.Drawing.Size(203, 20);
            this.textBox_ServerName.TabIndex = 3;
            // 
            // label_FileLocation
            // 
            this.label_FileLocation.AutoSize = true;
            this.label_FileLocation.Location = new System.Drawing.Point(12, 48);
            this.label_FileLocation.Name = "label_FileLocation";
            this.label_FileLocation.Size = new System.Drawing.Size(361, 39);
            this.label_FileLocation.TabIndex = 4;
            this.label_FileLocation.Text = "File location, local path or ftp-adress (required). Examples are\r\nD:\\…\\ARK\\Shoote" +
    "rGame\\Saved\\SavedArksLocal\\TheIsland.ark\r\nftp://203.0.113.12:27015/ShooterGame/S" +
    "aved/SavedArks/TheIsland.ark";
            // 
            // textBox_FileLocation
            // 
            this.textBox_FileLocation.Location = new System.Drawing.Point(12, 96);
            this.textBox_FileLocation.Name = "textBox_FileLocation";
            this.textBox_FileLocation.Size = new System.Drawing.Size(395, 20);
            this.textBox_FileLocation.TabIndex = 5;
            // 
            // button_FileSelect
            // 
            this.button_FileSelect.Image = ((System.Drawing.Image)(resources.GetObject("button_FileSelect.Image")));
            this.button_FileSelect.Location = new System.Drawing.Point(413, 94);
            this.button_FileSelect.Name = "button_FileSelect";
            this.button_FileSelect.Size = new System.Drawing.Size(23, 23);
            this.button_FileSelect.TabIndex = 6;
            this.button_FileSelect.UseVisualStyleBackColor = true;
            this.button_FileSelect.Click += new System.EventHandler(this.button_FileSelect_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(280, 123);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 7;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            // 
            // button_Ok
            // 
            this.button_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_Ok.Location = new System.Drawing.Point(361, 123);
            this.button_Ok.Name = "button_Ok";
            this.button_Ok.Size = new System.Drawing.Size(75, 23);
            this.button_Ok.TabIndex = 8;
            this.button_Ok.Text = "OK";
            this.button_Ok.UseVisualStyleBackColor = true;
            // 
            // LlFtpHelp
            // 
            this.LlFtpHelp.AutoSize = true;
            this.LlFtpHelp.Location = new System.Drawing.Point(12, 128);
            this.LlFtpHelp.Name = "LlFtpHelp";
            this.LlFtpHelp.Size = new System.Drawing.Size(164, 13);
            this.LlFtpHelp.TabIndex = 9;
            this.LlFtpHelp.TabStop = true;
            this.LlFtpHelp.Text = "Manual for configuring ftp access";
            this.LlFtpHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LlFtpHelp_LinkClicked);
            // 
            // ATImportFileLocationDialog
            // 
            this.AcceptButton = this.button_Ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(448, 158);
            this.Controls.Add(this.LlFtpHelp);
            this.Controls.Add(this.button_Ok);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_FileSelect);
            this.Controls.Add(this.textBox_FileLocation);
            this.Controls.Add(this.label_FileLocation);
            this.Controls.Add(this.textBox_ServerName);
            this.Controls.Add(this.label_ServerName);
            this.Controls.Add(this.textBox_ConvenientName);
            this.Controls.Add(this.label_ConvenientName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ATImportFileLocationDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit File Location";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_ConvenientName;
        private System.Windows.Forms.TextBox textBox_ConvenientName;
        private System.Windows.Forms.Label label_ServerName;
        private System.Windows.Forms.TextBox textBox_ServerName;
        private System.Windows.Forms.Label label_FileLocation;
        private System.Windows.Forms.TextBox textBox_FileLocation;
        private System.Windows.Forms.Button button_FileSelect;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Button button_Ok;
        private System.Windows.Forms.LinkLabel LlFtpHelp;
    }
}