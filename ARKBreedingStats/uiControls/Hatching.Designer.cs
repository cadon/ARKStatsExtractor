
namespace ARKBreedingStats.uiControls
{
    partial class Hatching
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
            this.LbHeader = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.LbStatNames = new System.Windows.Forms.Label();
            this.LbStatValues = new System.Windows.Forms.Label();
            this.LbStatLevels = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LbHeader
            // 
            this.LbHeader.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.LbHeader, 4);
            this.LbHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbHeader.Location = new System.Drawing.Point(3, 0);
            this.LbHeader.Name = "LbHeader";
            this.LbHeader.Padding = new System.Windows.Forms.Padding(10);
            this.LbHeader.Size = new System.Drawing.Size(142, 40);
            this.LbHeader.TabIndex = 0;
            this.LbHeader.Text = "select a species";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.LbHeader, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LbStatNames, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.LbStatValues, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.LbStatLevels, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(598, 385);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // LbStatNames
            // 
            this.LbStatNames.AutoSize = true;
            this.LbStatNames.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbStatNames.Location = new System.Drawing.Point(3, 40);
            this.LbStatNames.Name = "LbStatNames";
            this.LbStatNames.Padding = new System.Windows.Forms.Padding(5);
            this.LbStatNames.Size = new System.Drawing.Size(10, 27);
            this.LbStatNames.TabIndex = 1;
            // 
            // LbStatValues
            // 
            this.LbStatValues.AutoSize = true;
            this.LbStatValues.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbStatValues.Location = new System.Drawing.Point(19, 40);
            this.LbStatValues.Name = "LbStatValues";
            this.LbStatValues.Padding = new System.Windows.Forms.Padding(5);
            this.LbStatValues.Size = new System.Drawing.Size(10, 27);
            this.LbStatValues.TabIndex = 2;
            this.LbStatValues.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // LbStatLevels
            // 
            this.LbStatLevels.AutoSize = true;
            this.LbStatLevels.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbStatLevels.Location = new System.Drawing.Point(35, 40);
            this.LbStatLevels.Name = "LbStatLevels";
            this.LbStatLevels.Padding = new System.Windows.Forms.Padding(5);
            this.LbStatLevels.Size = new System.Drawing.Size(10, 27);
            this.LbStatLevels.TabIndex = 3;
            this.LbStatLevels.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Hatching
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Hatching";
            this.Size = new System.Drawing.Size(598, 385);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LbHeader;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label LbStatNames;
        private System.Windows.Forms.Label LbStatValues;
        private System.Windows.Forms.Label LbStatLevels;
    }
}
