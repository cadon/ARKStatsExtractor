namespace ARKBreedingStats.uiControls
{
    partial class ExportedCreatureControl
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
            this.lbStatus = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btLoadValues = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Location = new System.Drawing.Point(161, 24);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(84, 13);
            this.lbStatus.TabIndex = 0;
            this.lbStatus.Text = "Not yet imported";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btLoadValues);
            this.groupBox1.Controls.Add(this.lbStatus);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(463, 47);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // btLoadValues
            // 
            this.btLoadValues.Location = new System.Drawing.Point(6, 19);
            this.btLoadValues.Name = "btLoadValues";
            this.btLoadValues.Size = new System.Drawing.Size(149, 23);
            this.btLoadValues.TabIndex = 1;
            this.btLoadValues.Text = "Copy Values to Extractor";
            this.btLoadValues.UseVisualStyleBackColor = true;
            this.btLoadValues.Click += new System.EventHandler(this.btLoadValues_Click);
            // 
            // ExportedCreatureControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "ExportedCreatureControl";
            this.Size = new System.Drawing.Size(463, 47);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btLoadValues;
    }
}
