namespace ARKBreedingStats.testCases
{
    partial class TestCaseControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lbTestResult = new System.Windows.Forms.Label();
            this.lbTime = new System.Windows.Forms.Label();
            this.btRunTest = new System.Windows.Forms.Button();
            this.bt2Te = new System.Windows.Forms.Button();
            this.bt2Ex = new System.Windows.Forms.Button();
            this.btDelete = new System.Windows.Forms.Button();
            this.lbAdditionalResults = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Controls.Add(this.btRunTest);
            this.groupBox1.Controls.Add(this.bt2Te);
            this.groupBox1.Controls.Add(this.bt2Ex);
            this.groupBox1.Controls.Add(this.btDelete);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(564, 53);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "species, info";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.lbTestResult, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbTime, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbAdditionalResults, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(228, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(309, 34);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // lbTestResult
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.lbTestResult, 2);
            this.lbTestResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbTestResult.Location = new System.Drawing.Point(3, 0);
            this.lbTestResult.Name = "lbTestResult";
            this.lbTestResult.Size = new System.Drawing.Size(303, 17);
            this.lbTestResult.TabIndex = 3;
            this.lbTestResult.Text = "not yet tested";
            this.lbTestResult.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lbTestResult_MouseClick);
            // 
            // lbTime
            // 
            this.lbTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbTime.Location = new System.Drawing.Point(3, 17);
            this.lbTime.Name = "lbTime";
            this.lbTime.Size = new System.Drawing.Size(148, 17);
            this.lbTime.TabIndex = 4;
            this.lbTime.Text = "testing-time";
            // 
            // btRunTest
            // 
            this.btRunTest.Dock = System.Windows.Forms.DockStyle.Left;
            this.btRunTest.Location = new System.Drawing.Point(153, 16);
            this.btRunTest.Name = "btRunTest";
            this.btRunTest.Size = new System.Drawing.Size(75, 34);
            this.btRunTest.TabIndex = 2;
            this.btRunTest.Text = "Run Test";
            this.btRunTest.UseVisualStyleBackColor = true;
            this.btRunTest.Click += new System.EventHandler(this.btRunTest_Click);
            // 
            // bt2Te
            // 
            this.bt2Te.Dock = System.Windows.Forms.DockStyle.Left;
            this.bt2Te.Location = new System.Drawing.Point(78, 16);
            this.bt2Te.Name = "bt2Te";
            this.bt2Te.Size = new System.Drawing.Size(75, 34);
            this.bt2Te.TabIndex = 1;
            this.bt2Te.Text = "to Tester";
            this.bt2Te.UseVisualStyleBackColor = true;
            this.bt2Te.Click += new System.EventHandler(this.bt2Te_Click);
            // 
            // bt2Ex
            // 
            this.bt2Ex.Dock = System.Windows.Forms.DockStyle.Left;
            this.bt2Ex.Location = new System.Drawing.Point(3, 16);
            this.bt2Ex.Name = "bt2Ex";
            this.bt2Ex.Size = new System.Drawing.Size(75, 34);
            this.bt2Ex.TabIndex = 0;
            this.bt2Ex.Text = "to Extractor";
            this.bt2Ex.UseVisualStyleBackColor = true;
            this.bt2Ex.Click += new System.EventHandler(this.bt2Ex_Click);
            // 
            // btDelete
            // 
            this.btDelete.Dock = System.Windows.Forms.DockStyle.Right;
            this.btDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btDelete.Location = new System.Drawing.Point(537, 16);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(24, 34);
            this.btDelete.TabIndex = 6;
            this.btDelete.Text = "×";
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // lbAdditionalResults
            // 
            this.lbAdditionalResults.AutoSize = true;
            this.lbAdditionalResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbAdditionalResults.Location = new System.Drawing.Point(157, 17);
            this.lbAdditionalResults.Name = "lbAdditionalResults";
            this.lbAdditionalResults.Size = new System.Drawing.Size(149, 17);
            this.lbAdditionalResults.TabIndex = 5;
            this.lbAdditionalResults.Text = "0";
            // 
            // TestCaseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "TestCaseControl";
            this.Size = new System.Drawing.Size(564, 53);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbTestResult;
        private System.Windows.Forms.Button btRunTest;
        private System.Windows.Forms.Button bt2Te;
        private System.Windows.Forms.Button bt2Ex;
        private System.Windows.Forms.Label lbTime;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Label lbAdditionalResults;
    }
}
