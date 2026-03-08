namespace ARKBreedingStats.settings
{
    partial class customSoundChooser
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
            this.labelName = new System.Windows.Forms.Label();
            this.buttonFileChooser = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.Location = new System.Drawing.Point(3, 5);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(285, 18);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Event";
            // 
            // buttonFileChooser
            // 
            this.buttonFileChooser.Location = new System.Drawing.Point(294, 0);
            this.buttonFileChooser.Name = "buttonFileChooser";
            this.buttonFileChooser.Size = new System.Drawing.Size(75, 23);
            this.buttonFileChooser.TabIndex = 1;
            this.buttonFileChooser.Text = "choose file...";
            this.buttonFileChooser.UseVisualStyleBackColor = true;
            this.buttonFileChooser.Click += new System.EventHandler(this.buttonFileChooser_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Location = new System.Drawing.Point(375, 0);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(23, 23);
            this.buttonPlay.TabIndex = 2;
            this.buttonPlay.Text = "▶";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // customSoundChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.buttonFileChooser);
            this.Controls.Add(this.labelName);
            this.Name = "customSoundChooser";
            this.Size = new System.Drawing.Size(401, 23);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Button buttonFileChooser;
        private System.Windows.Forms.Button buttonPlay;
    }
}
