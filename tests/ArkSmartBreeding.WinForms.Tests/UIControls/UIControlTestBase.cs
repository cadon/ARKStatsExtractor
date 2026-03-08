using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.Models;
using ARKBreedingStats.species;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ARKBreedingStats.Tests.UIControls
{
    /// <summary>
    /// Base class for testing WinForms UI controls.
    /// Handles STA thread requirements and common setup/teardown.
    /// </summary>
    [TestClass]
    public abstract class UIControlTestBase
    {
        /// <summary>
        /// Form to host controls during testing
        /// </summary>
        protected Form TestForm { get; private set; }

        /// <summary>
        /// Loaded test creature collection from library.asb
        /// </summary>
        protected static CreatureCollection TestCreatureCollection { get; private set; }

        /// <summary>
        /// Initialize test data once for all tests
        /// </summary>
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            // Load test library with sample creatures
            var libraryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "library.asb");
            if (!File.Exists(libraryPath))
            {
                // Library file not found - tests can still run but will use mock data
                return;
            }

            if (FileService.LoadJsonFile(libraryPath, out CreatureCollection collection, out string libraryError))
            {
                TestCreatureCollection = collection;

                // Set as current collection so UI controls can access it
                CreatureCollection.CurrentCreatureCollection = collection;

                // Load mods manifest before loading values (mirrors production startup order)
                var modsManifest = mods.ModsManifest.TryLoadModManifestFile(forceDownload: false).Result;
                modsManifest?.Initialize();
                values.Values.V.SetModsManifest(modsManifest);

                // Initialize Values for species data
                var valuesLoaded = values.Values.V.LoadValues(forceReload: false, out string valuesError, out string valuesErrorTitle);
                if (valuesLoaded != null && valuesLoaded.Species != null && valuesLoaded.Species.Count > 0)
                {
                    // Link creatures to species from Values
                    foreach (var creature in collection.creatures)
                    {
                        if (creature.Species == null && !string.IsNullOrEmpty(creature.speciesBlueprint))
                        {
                            creature.Species = values.Values.V.SpeciesByBlueprint(creature.speciesBlueprint);
                        }
                    }
                }
                else
                {
                    throw new Exception($"Failed to load Values: {valuesErrorTitle} - {valuesError}");
                }
            }
        }

        [TestInitialize]
        public void BaseSetup()
        {
            // Note: When using STATestMethodAttribute, tests will run on STA thread automatically
            // The attribute handles thread apartment state, so no check needed here

            // Create a test form to host controls
            TestForm = new Form
            {
                Width = 800,
                Height = 600,
                ShowInTaskbar = false,
                // Form is not shown during tests to keep them fast
                // Can be shown for debugging: TestForm.Show();
            };

            // Call derived class setup
            OnSetup();
        }

        [TestCleanup]
        public void BaseTeardown()
        {
            OnTeardown();

            // Clean up form
            if (TestForm != null)
            {
                TestForm.Dispose();
                TestForm = null;
            }
        }

        /// <summary>
        /// Override in derived classes for additional setup
        /// </summary>
        protected virtual void OnSetup() { }

        /// <summary>
        /// Override in derived classes for additional teardown
        /// </summary>
        protected virtual void OnTeardown() { }

        /// <summary>
        /// Add a control to the test form and ensure it's properly initialized
        /// </summary>
        protected T AddControlToForm<T>(T control) where T : Control
        {
            TestForm.Controls.Add(control);
            control.CreateControl(); // Force handle creation
            Application.DoEvents(); // Process pending events
            return control;
        }

        /// <summary>
        /// Simulate a button click
        /// </summary>
        protected void ClickButton(Button button)
        {
            button.PerformClick();
            Application.DoEvents();
        }

        /// <summary>
        /// Set a numeric up/down value
        /// </summary>
        protected void SetNumericUpDown(NumericUpDown nud, decimal value)
        {
            nud.Value = value;
            Application.DoEvents();
        }

        /// <summary>
        /// Set text box value and trigger change events
        /// </summary>
        protected void SetTextBox(TextBox textBox, string text)
        {
            textBox.Text = text;
            Application.DoEvents();
        }

        /// <summary>
        /// Select an item in a combo box
        /// </summary>
        protected void SelectComboBoxItem(ComboBox comboBox, int index)
        {
            if (index < 0 || index >= comboBox.Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"Index {index} is out of range. ComboBox has {comboBox.Items.Count} items.");
            }

            comboBox.SelectedIndex = index;
            Application.DoEvents();
        }

        /// <summary>
        /// Check or uncheck a checkbox
        /// </summary>
        protected void SetCheckBox(CheckBox checkBox, bool isChecked)
        {
            checkBox.Checked = isChecked;
            Application.DoEvents();
        }

        /// <summary>
        /// Wait for async operations or debouncers to complete
        /// </summary>
        protected void WaitForAsync(int milliseconds = 100)
        {
            Thread.Sleep(milliseconds);
            Application.DoEvents();
        }

        /// <summary>
        /// Assert that a control is visible
        /// </summary>
        protected void AssertVisible(Control control, string message = null)
        {
            Assert.IsTrue(control.Visible, message ?? $"{control.Name} should be visible");
        }

        /// <summary>
        /// Assert that a control is not visible
        /// </summary>
        protected void AssertNotVisible(Control control, string message = null)
        {
            Assert.IsFalse(control.Visible, message ?? $"{control.Name} should not be visible");
        }

        /// <summary>
        /// Assert that a control is enabled
        /// </summary>
        protected void AssertEnabled(Control control, string message = null)
        {
            Assert.IsTrue(control.Enabled, message ?? $"{control.Name} should be enabled");
        }

        /// <summary>
        /// Assert that a control is disabled
        /// </summary>
        protected void AssertDisabled(Control control, string message = null)
        {
            Assert.IsFalse(control.Enabled, message ?? $"{control.Name} should be disabled");
        }

        /// <summary>
        /// Get a test creature by species name (e.g., "Rex", "Argentavis")
        /// </summary>
        protected Creature GetTestCreature(string speciesNamePart)
        {
            if (TestCreatureCollection == null)
            {
                Assert.Inconclusive("Test library not loaded. Make sure Library.asb exists in assets folder.");
            }

            var creature = TestCreatureCollection.creatures
                .FirstOrDefault(c => c.Species?.name?.Contains(speciesNamePart, StringComparison.OrdinalIgnoreCase) == true);

            if (creature == null)
            {
                Assert.Inconclusive($"No creature with species containing '{speciesNamePart}' found in test library.");
            }

            return creature;
        }

        /// <summary>
        /// Get multiple test creatures by species name
        /// </summary>
        protected System.Collections.Generic.List<Creature> GetTestCreatures(string speciesNamePart)
        {
            if (TestCreatureCollection == null)
            {
                Assert.Inconclusive("Test library not loaded. Make sure Library.asb exists in assets folder.");
            }

            var creatures = TestCreatureCollection.creatures
                .Where(c => c.Species?.name?.Contains(speciesNamePart, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            if (!creatures.Any())
            {
                Assert.Inconclusive($"No creatures with species containing '{speciesNamePart}' found in test library.");
            }

            return creatures;
        }

        /// <summary>
        /// Get a test species by name (e.g., "Rex", "Argentavis")
        /// </summary>
        protected Species GetTestSpecies(string speciesNamePart)
        {
            var creature = GetTestCreature(speciesNamePart);
            return creature?.Species;
        }
    }
}
