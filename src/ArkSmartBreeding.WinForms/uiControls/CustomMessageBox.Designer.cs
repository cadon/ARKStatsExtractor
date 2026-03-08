
namespace ARKBreedingStats.uiControls
{
    partial class CustomMessageBox
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
            this.LabelMessage = new System.Windows.Forms.Label();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonYes = new System.Windows.Forms.Button();
            this.ButtonNo = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.BtCopyToClipboard = new System.Windows.Forms.Button();
            this.PbIcon = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PbIcon)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelMessage
            // 
            this.LabelMessage.AutoSize = true;
            this.LabelMessage.Location = new System.Drawing.Point(0, 0);
            this.LabelMessage.MaximumSize = new System.Drawing.Size(380, 0);
            this.LabelMessage.Name = "LabelMessage";
            this.LabelMessage.Padding = new System.Windows.Forms.Padding(65, 40, 15, 10);
            this.LabelMessage.Size = new System.Drawing.Size(129, 63);
            this.LabelMessage.TabIndex = 0;
            this.LabelMessage.Text = "message";
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.AutoSize = true;
            this.ButtonCancel.Location = new System.Drawing.Point(335, 5);
            this.ButtonCancel.Margin = new System.Windows.Forms.Padding(5);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(58, 30);
            this.ButtonCancel.TabIndex = 3;
            this.ButtonCancel.Text = "BtRight";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonYes
            // 
            this.ButtonYes.AutoSize = true;
            this.ButtonYes.Location = new System.Drawing.Point(199, 5);
            this.ButtonYes.Margin = new System.Windows.Forms.Padding(5);
            this.ButtonYes.Name = "ButtonYes";
            this.ButtonYes.Size = new System.Drawing.Size(58, 30);
            this.ButtonYes.TabIndex = 1;
            this.ButtonYes.Text = "BtLeft";
            this.ButtonYes.UseVisualStyleBackColor = true;
            // 
            // ButtonNo
            // 
            this.ButtonNo.AutoSize = true;
            this.ButtonNo.Location = new System.Drawing.Point(267, 5);
            this.ButtonNo.Margin = new System.Windows.Forms.Padding(5);
            this.ButtonNo.Name = "ButtonNo";
            this.ButtonNo.Size = new System.Drawing.Size(58, 30);
            this.ButtonNo.TabIndex = 2;
            this.ButtonNo.Text = "BtMiddle";
            this.ButtonNo.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanel1.Controls.Add(this.ButtonCancel);
            this.flowLayoutPanel1.Controls.Add(this.ButtonNo);
            this.flowLayoutPanel1.Controls.Add(this.ButtonYes);
            this.flowLayoutPanel1.Controls.Add(this.BtCopyToClipboard);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 146);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(398, 41);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // BtCopyToClipboard
            // 
            this.BtCopyToClipboard.Location = new System.Drawing.Point(80, 5);
            this.BtCopyToClipboard.Margin = new System.Windows.Forms.Padding(5);
            this.BtCopyToClipboard.Name = "BtCopyToClipboard";
            this.BtCopyToClipboard.Size = new System.Drawing.Size(109, 30);
            this.BtCopyToClipboard.TabIndex = 0;
            this.BtCopyToClipboard.Text = "Copy to Clipboard";
            this.BtCopyToClipboard.UseVisualStyleBackColor = true;
            this.BtCopyToClipboard.Click += new System.EventHandler(this.BtCopyToClipboard_Click);
            // 
            // PbIcon
            // 
            this.PbIcon.Location = new System.Drawing.Point(12, 41);
            this.PbIcon.Name = "PbIcon";
            this.PbIcon.Size = new System.Drawing.Size(45, 45);
            this.PbIcon.TabIndex = 3;
            this.PbIcon.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.LabelMessage);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(398, 146);
            this.panel1.TabIndex = 4;
            // 
            // CustomMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(398, 187);
            this.Controls.Add(this.PbIcon);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "CustomMessageBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CustomMessageBox";
            this.SizeChanged += new System.EventHandler(this.CustomMessageBox_SizeChanged);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PbIcon)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LabelMessage;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonYes;
        private System.Windows.Forms.Button ButtonNo;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.PictureBox PbIcon;
        private System.Windows.Forms.Button BtCopyToClipboard;
        private System.Windows.Forms.Panel panel1;
    }
}