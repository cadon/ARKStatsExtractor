﻿namespace ARKBreedingStats.uiControls
{
    partial class ScrollForm
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
            this.SuspendLayout();
            // 
            // ScrollForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(800, 800);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ScrollForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ScrollForm";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Black;
            this.MouseLeave += new System.EventHandler(this.ScrollForm_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ScrollForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScrollForm_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}