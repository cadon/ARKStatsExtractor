using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ARKBreedingStats.testCases
{
    public partial class ExtractionTestControl : UserControl
    {
        private ExtractionTestCases cases = new ExtractionTestCases();
        private readonly List<TestCaseControl> extractionTestControls = new List<TestCaseControl>();
        public event TestCaseControl.CopyTestToExtractorEventHandler CopyToExtractor;
        public event TestCaseControl.CopyTestToTesterEventHandler CopyToTester;

        public ExtractionTestControl()
        {
            InitializeComponent();
        }

        public void loadExtractionTestCases(string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                XmlSerializer reader = new XmlSerializer(typeof(ExtractionTestCases));

                if (!System.IO.File.Exists(fileName))
                {
                    MessageBox.Show("Save file with name \"" + fileName + "\" does not exist!", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                System.IO.FileStream file = System.IO.File.OpenRead(fileName);

                try
                {
                    cases = (ExtractionTestCases)reader.Deserialize(file);
                    Properties.Settings.Default.LastSaveFileTestCases = fileName;
                }
                catch (Exception e)
                {
                    MessageBox.Show("File Couldn't be opened, we thought you should know.\nErrormessage:\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    file.Close();
                }
                file.Close();

                // convert from 8 to 12 stats, reorder // TODO remove
                var newToOldIndices = new int[] { 0, 1, 7, 2, 3, -1, -1, 4, 5, 6, -1, -1 };
                foreach (var c in cases.testCases)
                {
                    if (c.multipliers.Length == 8)
                    {
                        var newMultipliers = new double[Values.STATS_COUNT][];
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            newMultipliers[s] = new double[4];
                            if (newToOldIndices[s] >= 0)
                            {
                                for (int si = 0; si < 4; si++)
                                    newMultipliers[s][si] = c.multipliers[newToOldIndices[s]][si];
                            }
                            else
                            {
                                for (int si = 0; si < 4; si++)
                                    newMultipliers[s][si] = 1;
                            }
                        }
                        c.multipliers = newMultipliers;
                    }
                    // fix statlevel-indices
                    if (c.levelsWild.Length == 8)
                    {
                        var newLevels = new int[Values.STATS_COUNT];
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            if (newToOldIndices[s] >= 0)
                                newLevels[s] = c.levelsWild[newToOldIndices[s]];
                        }
                        c.levelsWild = newLevels;
                    }
                    if (c.levelsDom.Length == 8)
                    {
                        var newLevels = new int[Values.STATS_COUNT];
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            if (newToOldIndices[s] >= 0)
                                newLevels[s] = c.levelsDom[newToOldIndices[s]];
                        }
                        c.levelsDom = newLevels;
                    }
                    if (c.statValues.Length == 8)
                    {
                        var newValues = new double[Values.STATS_COUNT];
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            if (newToOldIndices[s] >= 0)
                                newValues[s] = c.statValues[newToOldIndices[s]];
                        }
                        c.statValues = newValues;
                    }

                    // convert species-identifier
                    if (string.IsNullOrEmpty(c.speciesName)
                        || string.IsNullOrEmpty(c.speciesBP))
                    {
                        if (Values.V.TryGetSpeciesByName(c.species, out Species species))
                        {
                            c.Species = species;
                        }
                    }
                }

                showTestCases();
                updateFileLabel();
            }
        }

        private void saveExtractionTestCasesToFile(string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                XmlSerializer writer = new XmlSerializer(typeof(ExtractionTestCases));
                try
                {
                    System.IO.FileStream file = System.IO.File.Create(fileName);
                    writer.Serialize(file, cases);
                    file.Close();
                    Properties.Settings.Default.LastSaveFileTestCases = fileName;
                    updateFileLabel();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error during serialization.\nErrormessage:\n\n" + e.Message, "Serialization-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                saveExtractionTestCasesAs();
            }
        }

        private void saveExtractionTestCasesAs()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "ASB Extraction Testcases (*.xml)|*.xml";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.LastSaveFileTestCases = dlg.FileName;
                saveTestFile();
            }
        }

        private void showTestCases()
        {
            SuspendLayout();
            ClearAll();

            foreach (ExtractionTestCase c in cases.testCases)
            {
                TestCaseControl tcc = new TestCaseControl(c);
                tcc.CopyToExtractor += CopyToExtractor;
                tcc.CopyToTester += CopyToTester;
                tcc.RemoveTestCase += Tcc_RemoveTestCase;
                extractionTestControls.Add(tcc);
                flowLayoutPanelTestCases.Controls.Add(tcc);
                flowLayoutPanelTestCases.SetFlowBreak(tcc, true);
            }
            ResumeLayout();
        }

        private void Tcc_RemoveTestCase(TestCaseControl tcc)
        {
            cases.testCases.Remove(tcc.testCase);
            tcc.Dispose();
            extractionTestControls.Remove(tcc);
            showTestCases();
        }

        private void ClearAll(bool clearCases = false)
        {
            foreach (var e in extractionTestControls)
                e.Dispose();
            extractionTestControls.Clear();
            if (cases == null)
                cases = new ExtractionTestCases();
            if (clearCases)
                cases.testCases.Clear();
        }

        public void addTestCase(ExtractionTestCase etc)
        {
            cases.testCases.Insert(0, etc);
            showTestCases();
        }

        private void newTestfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearAll(true);
            Properties.Settings.Default.LastSaveFileTestCases = "";
            showTestCases();
            updateFileLabel();
        }

        private void loadTestfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "ASB Extraction Testcases (*.xml)|*.xml";
            dlg.InitialDirectory = Application.StartupPath;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                loadExtractionTestCases(dlg.FileName);
            }
        }

        private void btSaveTestFile_Click(object sender, EventArgs e)
        {
            saveTestFile();
        }

        private void saveTestfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveTestFile();
        }

        private void saveTestFile()
        {
            saveExtractionTestCasesToFile(Properties.Settings.Default.LastSaveFileTestCases);
        }

        private void saveTestfileAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveExtractionTestCasesAs();
        }

        private void updateFileLabel()
        {
            lbTestFile.Text = Properties.Settings.Default.LastSaveFileTestCases;
        }

        private void btRunAllTests_Click(object sender, EventArgs e)
        {
            foreach (var t in extractionTestControls)
                t.ClearTestResult();
            Invalidate();
            foreach (var t in extractionTestControls)
                t.runTest();
        }
    }
}
