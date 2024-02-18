namespace ARKBreedingStats.uiControls
{
    partial class ColorPickerWindow
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
            this.ColorPickerControl1 = new ARKBreedingStats.uiControls.ColorPickerControl();
            this.SuspendLayout();
            // 
            // ColorPickerControl1
            // 
            this.ColorPickerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ColorPickerControl1.Location = new System.Drawing.Point(0, 0);
            this.ColorPickerControl1.Name = "ColorPickerControl1";
            this.ColorPickerControl1.Padding = new System.Windows.Forms.Padding(1);
            this.ColorPickerControl1.Size = new System.Drawing.Size(450, 414);
            this.ColorPickerControl1.TabIndex = 0;
            // 
            // ColorPickerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 414);
            this.Controls.Add(this.ColorPickerControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ColorPickerWindow";
            this.ShowInTaskbar = false;
            this.Text = "ColorPickerWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private ColorPickerControl ColorPickerControl1;
    }
}