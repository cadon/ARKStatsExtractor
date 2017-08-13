namespace ARKBreedingStats.raising
{
    partial class ParentStats
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
            this.labelMother = new System.Windows.Forms.Label();
            this.labelFather = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelMother
            // 
            this.labelMother.Location = new System.Drawing.Point(51, 16);
            this.labelMother.Name = "labelMother";
            this.labelMother.Size = new System.Drawing.Size(73, 30);
            this.labelMother.TabIndex = 0;
            this.labelMother.Text = "Mother";
            // 
            // labelFather
            // 
            this.labelFather.Location = new System.Drawing.Point(130, 16);
            this.labelFather.Name = "labelFather";
            this.labelFather.Size = new System.Drawing.Size(75, 30);
            this.labelFather.TabIndex = 1;
            this.labelFather.Text = "Father";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelFather);
            this.groupBox1.Controls.Add(this.labelMother);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(211, 245);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parent Stats";
            // 
            // ParentStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "ParentStats";
            this.Size = new System.Drawing.Size(211, 245);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelMother;
        private System.Windows.Forms.Label labelFather;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
