
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
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelMessage
            // 
            this.LabelMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LabelMessage.Location = new System.Drawing.Point(0, 0);
            this.LabelMessage.Name = "LabelMessage";
            this.LabelMessage.Padding = new System.Windows.Forms.Padding(60, 40, 20, 5);
            this.LabelMessage.Size = new System.Drawing.Size(398, 146);
            this.LabelMessage.TabIndex = 0;
            this.LabelMessage.Text = "Message";
            // 
            // BtRight
            // 
            this.ButtonCancel.AutoSize = true;
            this.ButtonCancel.Location = new System.Drawing.Point(335, 5);
            this.ButtonCancel.Margin = new System.Windows.Forms.Padding(5);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(58, 30);
            this.ButtonCancel.TabIndex = 2;
            this.ButtonCancel.Text = "BtRight";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // BtLeft
            // 
            this.ButtonYes.AutoSize = true;
            this.ButtonYes.Location = new System.Drawing.Point(199, 5);
            this.ButtonYes.Margin = new System.Windows.Forms.Padding(5);
            this.ButtonYes.Name = "ButtonYes";
            this.ButtonYes.Size = new System.Drawing.Size(58, 30);
            this.ButtonYes.TabIndex = 0;
            this.ButtonYes.Text = "BtLeft";
            this.ButtonYes.UseVisualStyleBackColor = true;
            // 
            // BtMiddle
            // 
            this.ButtonNo.AutoSize = true;
            this.ButtonNo.Location = new System.Drawing.Point(267, 5);
            this.ButtonNo.Margin = new System.Windows.Forms.Padding(5);
            this.ButtonNo.Name = "ButtonNo";
            this.ButtonNo.Size = new System.Drawing.Size(58, 30);
            this.ButtonNo.TabIndex = 1;
            this.ButtonNo.Text = "BtMiddle";
            this.ButtonNo.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanel1.Controls.Add(this.ButtonCancel);
            this.flowLayoutPanel1.Controls.Add(this.ButtonNo);
            this.flowLayoutPanel1.Controls.Add(this.ButtonYes);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 146);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(398, 41);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // CustomMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(398, 187);
            this.Controls.Add(this.LabelMessage);
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CustomMessageBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CustomMessageBox";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LabelMessage;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonYes;
        private System.Windows.Forms.Button ButtonNo;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}