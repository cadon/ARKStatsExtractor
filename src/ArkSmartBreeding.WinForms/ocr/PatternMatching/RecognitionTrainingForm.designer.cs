namespace ARKBreedingStats.ocr.PatternMatching
{
    partial class RecognitionTrainingForm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BtAbort = new System.Windows.Forms.Button();
            this.BtFemaleSign = new System.Windows.Forms.Button();
            this.BtMale = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(150, 150);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(191, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(428, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Please enter the character to train the recognition module (leave empty to skip c" +
    "haracter):";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(335, 97);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(158, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(499, 95);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.Location = new System.Drawing.Point(282, 9);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(354, 39);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(191, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Captured Image:";
            // 
            // BtAbort
            // 
            this.BtAbort.Location = new System.Drawing.Point(499, 124);
            this.BtAbort.Name = "BtAbort";
            this.BtAbort.Size = new System.Drawing.Size(75, 23);
            this.BtAbort.TabIndex = 6;
            this.BtAbort.Text = "Abort";
            this.BtAbort.UseVisualStyleBackColor = true;
            this.BtAbort.Click += new System.EventHandler(this.BtAbort_Click);
            // 
            // BtFemaleSign
            // 
            this.BtFemaleSign.Location = new System.Drawing.Point(335, 123);
            this.BtFemaleSign.Name = "BtFemaleSign";
            this.BtFemaleSign.Size = new System.Drawing.Size(23, 23);
            this.BtFemaleSign.TabIndex = 7;
            this.BtFemaleSign.Text = "♀";
            this.BtFemaleSign.UseVisualStyleBackColor = true;
            this.BtFemaleSign.Click += new System.EventHandler(this.BtFemaleSign_Click);
            // 
            // BtMale
            // 
            this.BtMale.Location = new System.Drawing.Point(364, 123);
            this.BtMale.Name = "BtMale";
            this.BtMale.Size = new System.Drawing.Size(23, 23);
            this.BtMale.TabIndex = 8;
            this.BtMale.Text = "♂";
            this.BtMale.UseVisualStyleBackColor = true;
            this.BtMale.Click += new System.EventHandler(this.BtMale_Click);
            // 
            // RecognitionTrainingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 175);
            this.Controls.Add(this.BtMale);
            this.Controls.Add(this.BtFemaleSign);
            this.Controls.Add(this.BtAbort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "RecognitionTrainingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RecognitionTrainingForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BtAbort;
        private System.Windows.Forms.Button BtFemaleSign;
        private System.Windows.Forms.Button BtMale;
    }
}