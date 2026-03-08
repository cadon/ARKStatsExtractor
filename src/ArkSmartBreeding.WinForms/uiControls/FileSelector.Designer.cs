namespace ARKBreedingStats.uiControls
{
    partial class FileSelector
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
            this.btChooseFile = new System.Windows.Forms.Button();
            this.btDeleteLink = new System.Windows.Forms.Button();
            this.lbLink = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btChooseFile
            // 
            this.btChooseFile.Dock = System.Windows.Forms.DockStyle.Left;
            this.btChooseFile.Location = new System.Drawing.Point(0, 0);
            this.btChooseFile.Name = "btChooseFile";
            this.btChooseFile.Size = new System.Drawing.Size(89, 23);
            this.btChooseFile.TabIndex = 0;
            this.btChooseFile.Text = "Choose Folder…";
            this.btChooseFile.UseVisualStyleBackColor = true;
            this.btChooseFile.Click += new System.EventHandler(this.btChooseFile_Click);
            // 
            // btDeleteLink
            // 
            this.btDeleteLink.Dock = System.Windows.Forms.DockStyle.Right;
            this.btDeleteLink.Location = new System.Drawing.Point(357, 0);
            this.btDeleteLink.Name = "btDeleteLink";
            this.btDeleteLink.Size = new System.Drawing.Size(23, 23);
            this.btDeleteLink.TabIndex = 1;
            this.btDeleteLink.Text = "×";
            this.btDeleteLink.UseVisualStyleBackColor = true;
            this.btDeleteLink.Click += new System.EventHandler(this.btDeleteLink_Click);
            // 
            // lbLink
            // 
            this.lbLink.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbLink.Location = new System.Drawing.Point(89, 0);
            this.lbLink.Name = "lbLink";
            this.lbLink.Size = new System.Drawing.Size(268, 23);
            this.lbLink.TabIndex = 2;
            this.lbLink.Text = "filename";
            this.lbLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FileSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbLink);
            this.Controls.Add(this.btDeleteLink);
            this.Controls.Add(this.btChooseFile);
            this.Name = "FileSelector";
            this.Size = new System.Drawing.Size(380, 23);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btChooseFile;
        private System.Windows.Forms.Button btDeleteLink;
        private System.Windows.Forms.Label lbLink;
    }
}
