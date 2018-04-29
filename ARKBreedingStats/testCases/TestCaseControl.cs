using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats.testCases
{
    public partial class TestCaseControl : UserControl
    {
        public delegate void CopyTestToExtractorEventHandler(string species, int level, double[] statValues, bool postTamed, bool bred, double imprintingBonus, bool gotoExtractor, TestCaseControl tcc);
        public delegate void CopyTestToTesterEventHandler(string species, int[] wildLevels, int[] domLevels, bool postTamed, bool bred, double te, double imprintingBonus, bool gotoTester, TestCaseControl tcc);
        public delegate void RemoveTestCaseEventHandler(TestCaseControl tcc);
        public event CopyTestToExtractorEventHandler CopyToExtractor;
        public event CopyTestToTesterEventHandler CopyToTester;
        public event RemoveTestCaseEventHandler RemoveTestCase;

        public ExtractionTestCase testCase;
        public bool? success;

        public TestCaseControl()
        {
            InitializeComponent();
        }

        public TestCaseControl(ExtractionTestCase testcase)
        {
            InitializeComponent();
            setTestCase(testcase);
        }

        public void setTestCase(ExtractionTestCase testCase)
        {
            this.testCase = testCase;
            updateTestCaseTitle();
            lbTestResult.BackColor = SystemColors.Control;
            success = null;
        }

        private void updateTestCaseTitle()
        {
            groupBox1.Text = this.testCase.species + " (Lv " + testCase.totalLevel + ", " + (testCase.bred ? "B" : (testCase.postTamed ? "T" : "W")) + "), " + this.testCase.testName;
        }

        private void bt2Ex_Click(object sender, EventArgs e)
        {
            CopyToExtractor?.Invoke(testCase.species, testCase.levelsWild[7] + 1 + testCase.levelsDom.Sum(), testCase.statValues, testCase.postTamed, testCase.bred, testCase.imprintingBonus, true, this);
        }

        private void bt2Te_Click(object sender, EventArgs e)
        {
            CopyToTester?.Invoke(testCase.species, testCase.levelsWild, testCase.levelsDom, testCase.postTamed, testCase.bred, testCase.tamingEff, testCase.imprintingBonus, true, this);
        }

        private void btRunTest_Click(object sender, EventArgs e)
        {
            runTest();
        }

        public void runTest()
        {
            ClearTestResult();
            CopyToExtractor?.Invoke(testCase.species, testCase.levelsWild[7] + 1 + testCase.levelsDom.Sum(), testCase.statValues, testCase.postTamed, testCase.bred, testCase.imprintingBonus, false, this);
        }

        public void setTestResult(bool success, int time, int additionalResults = 0, string info = "")
        {
            this.success = success;
            if (success == true)
            {
                lbTestResult.Text = "Check" + (info.Length > 0 ? " | " + info : "");
                lbTestResult.BackColor = Color.LightGreen;
            }
            else
            {
                lbTestResult.Text = info;
                lbTestResult.BackColor = Color.LightSalmon;
            }

            lbTime.Text = time.ToString() + " ms";
            lbTime.BackColor = Utils.getColorFromPercent(100 - time);
            lbTime.ForeColor = Utils.ForeColor(lbTime.BackColor);

            if (additionalResults > 0)
            {
                lbAdditionalResults.Text = "additional Results: " + additionalResults.ToString();
                lbAdditionalResults.BackColor = Utils.getColorFromPercent(60 - additionalResults / 4);
                lbAdditionalResults.ForeColor = Utils.ForeColor(lbAdditionalResults.BackColor);
            }
            else
            {
                lbAdditionalResults.Text = "";
                lbAdditionalResults.BackColor = Color.Transparent;
            }
        }

        public void ClearTestResult()
        {
            lbTestResult.Text = "untested";
            lbTime.Text = "";
            lbTestResult.BackColor = Color.Transparent;
        }

        private void lbTestResult_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Utils.ShowTextInput("Testcase-info", out string name, "Name of the testcase", testCase.testName))
                {
                    testCase.testName = name;
                    updateTestCaseTitle();
                }
            }
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete this testcase?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                RemoveTestCase?.Invoke(this);
        }
    }
}
