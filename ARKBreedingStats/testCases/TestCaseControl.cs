using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void setTestCase(ExtractionTestCase testcase)
        {
            this.testCase = testcase;
            groupBox1.Text = testCase.species + " (Lv " + testcase.totalLevel + ", " + (testcase.postTamed ? (testcase.bred ? "B" : "T") : "W") + "), " + testCase.testName;
            lbTestResult.BackColor = SystemColors.Control;
            success = null;
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

        public void setTestResult(bool success, int time, string info = "")
        {
            this.success = success;
            if (success == true)
            {
                lbTestResult.Text = "Check" + info;
                lbTestResult.BackColor = Color.LightGreen;
            }
            else
            {
                lbTestResult.Text = info;
                lbTestResult.BackColor = Color.LightSalmon;
            }
            lbTime.Text = time.ToString() + " ms";
        }

        public void ClearTestResult()
        {
            lbTestResult.Text = "untested";
            lbTime.Text = "";
            lbTestResult.BackColor = SystemColors.Control;
        }

        private void lbTestResult_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Utils.ShowTextInput("Testcase-info", out string name, testCase.testName))
                {
                    testCase.testName = name;
                    groupBox1.Text = testCase.species + ", " + testCase.testName;
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
