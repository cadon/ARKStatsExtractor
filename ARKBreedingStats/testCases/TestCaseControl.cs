using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.testCases
{
    public partial class TestCaseControl : UserControl
    {
        public delegate void CopyTestToExtractorEventHandler(string speciesBp, int level, double[] statValues, bool postTamed, bool bred, double imprintingBonus, bool gotoExtractor, TestCaseControl tcc);
        public delegate void CopyTestToTesterEventHandler(string speciesBp, int[] wildLevels, int[] domLevels, int[] mutLevels, bool postTamed, bool bred, double te, double imprintingBonus, bool gotoTester, TestCaseControl tcc);
        public delegate void RemoveTestCaseEventHandler(TestCaseControl tcc);
        public event CopyTestToExtractorEventHandler CopyToExtractor;
        public event CopyTestToTesterEventHandler CopyToTester;
        public event RemoveTestCaseEventHandler RemoveTestCase;

        public ExtractionTestCase TestCase;

        public TestCaseControl()
        {
            InitializeComponent();
        }

        public TestCaseControl(ExtractionTestCase testCase)
        {
            InitializeComponent();
            SetTestCase(testCase);
        }

        private void SetTestCase(ExtractionTestCase testCase)
        {
            TestCase = testCase;
            UpdateTestCaseTitle();
            lbTestResult.BackColor = SystemColors.Control;
        }

        private void UpdateTestCaseTitle()
        {
            groupBox1.Text = $"{TestCase.speciesName} (Lv {TestCase.totalLevel}, {(TestCase.bred ? "B" : (TestCase.postTamed ? "T" : "W"))}), {TestCase.testName}";
        }

        private void bt2Ex_Click(object sender, EventArgs e)
        {
            CopyToExtractor?.Invoke(TestCase.speciesBlueprintPath, TestCase.levelsWild[Stats.Torpidity] + 1 + TestCase.levelsDom.Sum(), TestCase.statValues, TestCase.postTamed, TestCase.bred, TestCase.imprintingBonus, true, this);
        }

        private void bt2Te_Click(object sender, EventArgs e)
        {
            CopyToTester?.Invoke(TestCase.speciesBlueprintPath, TestCase.levelsWild, TestCase.levelsDom, TestCase.levelsMut, TestCase.postTamed, TestCase.bred, TestCase.tamingEff, TestCase.imprintingBonus, true, this);
        }

        private void btRunTest_Click(object sender, EventArgs e)
        {
            RunTest();
        }

        public void RunTest()
        {
            ClearTestResult();
            CopyToExtractor?.Invoke(TestCase.speciesBlueprintPath, TestCase.levelsWild[Stats.Torpidity] + 1 + TestCase.levelsDom.Sum(), TestCase.statValues, TestCase.postTamed, TestCase.bred, TestCase.imprintingBonus, false, this);
        }

        public void SetTestResult(bool success, int time, int additionalResults = 0, string info = null)
        {
            if (success)
            {
                lbTestResult.Text = "Check" + (string.IsNullOrEmpty(info) ? string.Empty : " | " + info);
                lbTestResult.BackColor = Color.LightGreen;
            }
            else
            {
                lbTestResult.Text = info;
                lbTestResult.BackColor = Color.LightSalmon;
            }

            lbTime.Text = $"{time} ms";
            lbTime.SetBackColorAndAccordingForeColor(Utils.GetColorFromPercent(100 - time));

            if (additionalResults > 0)
            {
                lbAdditionalResults.Text = $"additional Results: {additionalResults}";
                lbAdditionalResults.SetBackColorAndAccordingForeColor(Utils.GetColorFromPercent(60 - additionalResults / 4));
            }
            else
            {
                lbAdditionalResults.Text = null;
                lbAdditionalResults.BackColor = Color.Transparent;
            }
        }

        public void ClearTestResult()
        {
            lbTestResult.Text = "untested";
            lbTime.Text = null;
            lbTestResult.BackColor = Color.Transparent;
        }

        private void lbTestResult_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right
                && Utils.ShowTextInput("Test case info", out string name, "Name of the test case", TestCase.testName))
            {
                TestCase.testName = name;
                UpdateTestCaseTitle();
            }
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete this test case?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                RemoveTestCase?.Invoke(this);
        }
    }
}
