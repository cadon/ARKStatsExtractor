
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
            LabelMessage = new System.Windows.Forms.Label();
            ButtonCancel = new System.Windows.Forms.Button();
            ButtonYes = new System.Windows.Forms.Button();
            ButtonNo = new System.Windows.Forms.Button();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            BtCopyToClipboard = new System.Windows.Forms.Button();
            PbIcon = new System.Windows.Forms.PictureBox();
            panel1 = new System.Windows.Forms.Panel();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PbIcon).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // LabelMessage
            // 
            LabelMessage.AutoSize = true;
            LabelMessage.Location = new System.Drawing.Point(0, 0);
            LabelMessage.MaximumSize = new System.Drawing.Size(380, 0);
            LabelMessage.Name = "LabelMessage";
            LabelMessage.Padding = new System.Windows.Forms.Padding(65, 40, 15, 10);
            LabelMessage.Size = new System.Drawing.Size(133, 65);
            LabelMessage.TabIndex = 0;
            LabelMessage.Text = "message";
            // 
            // ButtonCancel
            // 
            ButtonCancel.AutoSize = true;
            ButtonCancel.Location = new System.Drawing.Point(335, 5);
            ButtonCancel.Margin = new System.Windows.Forms.Padding(5);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new System.Drawing.Size(58, 30);
            ButtonCancel.TabIndex = 3;
            ButtonCancel.Text = "BtRight";
            ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonYes
            // 
            ButtonYes.AutoSize = true;
            ButtonYes.Location = new System.Drawing.Point(192, 5);
            ButtonYes.Margin = new System.Windows.Forms.Padding(5);
            ButtonYes.Name = "ButtonYes";
            ButtonYes.Size = new System.Drawing.Size(58, 30);
            ButtonYes.TabIndex = 1;
            ButtonYes.Text = "BtLeft";
            ButtonYes.UseVisualStyleBackColor = true;
            // 
            // ButtonNo
            // 
            ButtonNo.AutoSize = true;
            ButtonNo.Location = new System.Drawing.Point(260, 5);
            ButtonNo.Margin = new System.Windows.Forms.Padding(5);
            ButtonNo.Name = "ButtonNo";
            ButtonNo.Size = new System.Drawing.Size(65, 30);
            ButtonNo.TabIndex = 2;
            ButtonNo.Text = "BtMiddle";
            ButtonNo.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            flowLayoutPanel1.Controls.Add(ButtonCancel);
            flowLayoutPanel1.Controls.Add(ButtonNo);
            flowLayoutPanel1.Controls.Add(ButtonYes);
            flowLayoutPanel1.Controls.Add(BtCopyToClipboard);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 147);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(398, 40);
            flowLayoutPanel1.TabIndex = 1;
            // 
            // BtCopyToClipboard
            // 
            BtCopyToClipboard.Location = new System.Drawing.Point(73, 5);
            BtCopyToClipboard.Margin = new System.Windows.Forms.Padding(5);
            BtCopyToClipboard.Name = "BtCopyToClipboard";
            BtCopyToClipboard.Size = new System.Drawing.Size(109, 30);
            BtCopyToClipboard.TabIndex = 0;
            BtCopyToClipboard.Text = "Copy to Clipboard";
            BtCopyToClipboard.UseVisualStyleBackColor = true;
            BtCopyToClipboard.Click += BtCopyToClipboard_Click;
            // 
            // PbIcon
            // 
            PbIcon.Location = new System.Drawing.Point(12, 41);
            PbIcon.Name = "PbIcon";
            PbIcon.Size = new System.Drawing.Size(45, 45);
            PbIcon.TabIndex = 3;
            PbIcon.TabStop = false;
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.Controls.Add(LabelMessage);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(398, 147);
            panel1.TabIndex = 4;
            // 
            // CustomMessageBox
            // 
            BackColor = System.Drawing.SystemColors.Window;
            ClientSize = new System.Drawing.Size(398, 187);
            Controls.Add(PbIcon);
            Controls.Add(panel1);
            Controls.Add(flowLayoutPanel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Name = "CustomMessageBox";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "CustomMessageBox";
            SizeChanged += CustomMessageBox_SizeChanged;
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PbIcon).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

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