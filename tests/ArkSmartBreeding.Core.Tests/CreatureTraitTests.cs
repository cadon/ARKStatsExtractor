using ARKBreedingStats.Library;
using ARKBreedingStats.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for the CreatureTrait class and its static parse/format methods.
    /// </summary>
    [TestClass]
    public class CreatureTraitTests
    {
        [TestInitialize]
        public void Setup()
        {
            // Set up trait definitions for tests
            var defs = new Dictionary<string, TraitDefinition>
            {
                ["Athletic"] = new TraitDefinition
                {
                    Id = "Athletic",
                    Name = "Athletic",
                    Description = "Test trait",
                    InheritHigherProbability = [0.05, 0.10, 0.15],
                    MutationProbability = [0.01, 0.02, 0.03]
                },
                ["Bulky"] = new TraitDefinition
                {
                    Id = "Bulky",
                    Name = "Bulky",
                    Description = "Another test trait",
                    InheritHigherProbability = [0.02],
                    MutationProbability = [0.005]
                }
            };
            TraitDefinition.SetTraitDefinitions(defs);
        }

        [TestMethod]
        public void Constructor_WithTraitDefinition_SetsProperties()
        {
            var def = TraitDefinition.GetTraitDefinition("Athletic");
            var trait = new CreatureTrait(def, tier: 1);

            Assert.AreEqual("Athletic", trait.Id);
            Assert.AreEqual(1, trait.Tier);
            Assert.AreEqual(0.10, trait.InheritHigherProbability, 0.0001);
            Assert.AreEqual(0.02, trait.MutationProbability, 0.0001);
        }

        [TestMethod]
        public void Constructor_WithTraitId_ResolvesDefinition()
        {
            var trait = new CreatureTrait("Bulky", tier: 0);
            Assert.IsNotNull(trait.TraitDefinition);
            Assert.AreEqual("Bulky", trait.TraitDefinition.Name);
            Assert.AreEqual(0.02, trait.InheritHigherProbability, 0.0001);
        }

        [TestMethod]
        public void Constructor_WithUnknownId_HasNullDefinition()
        {
            var trait = new CreatureTrait("NonExistent", tier: 0);
            Assert.IsNull(trait.TraitDefinition);
            Assert.AreEqual(0.0, trait.InheritHigherProbability);
            Assert.AreEqual(0.0, trait.MutationProbability);
        }

        [TestMethod]
        public void ToDefinitionString_FormatsCorrectly()
        {
            var trait = new CreatureTrait("Athletic", tier: 2);
            Assert.AreEqual("Athletic[2]", trait.ToDefinitionString());
        }

        [TestMethod]
        public void TryParse_WithTier_ParsesCorrectly()
        {
            var trait = CreatureTrait.TryParse("Athletic[1]");
            Assert.IsNotNull(trait);
            Assert.AreEqual("Athletic", trait.Id);
            Assert.AreEqual(1, trait.Tier);
        }

        [TestMethod]
        public void TryParse_WithoutTier_DefaultsToZero()
        {
            var trait = CreatureTrait.TryParse("Bulky");
            Assert.IsNotNull(trait);
            Assert.AreEqual("Bulky", trait.Id);
            Assert.AreEqual(0, trait.Tier);
        }

        [TestMethod]
        public void TryParse_NullOrEmpty_ReturnsNull()
        {
            Assert.IsNull(CreatureTrait.TryParse(null));
            Assert.IsNull(CreatureTrait.TryParse(""));
        }

        [TestMethod]
        public void TryParse_RoundTrips_WithToDefinitionString()
        {
            var original = new CreatureTrait("Athletic", tier: 2);
            var defString = original.ToDefinitionString();
            var parsed = CreatureTrait.TryParse(defString);
            Assert.AreEqual(original.Id, parsed.Id);
            Assert.AreEqual(original.Tier, parsed.Tier);
        }

        [TestMethod]
        public void ToString_ContainsNameAndTier()
        {
            var trait = new CreatureTrait("Athletic", tier: 0);
            var str = trait.ToString();
            Assert.Contains("Athletic", str, "Should contain trait name");
            Assert.Contains("T1", str, "Should show tier 1 (0-based internally, 1-based display)");
        }

        [TestMethod]
        public void StringList_ConcatenatesTraits()
        {
            var traits = new[]
            {
                new CreatureTrait("Athletic", tier: 0),
                new CreatureTrait("Bulky", tier: 0)
            };
            var result = CreatureTrait.StringList(traits);
            Assert.Contains("Athletic", result);
            Assert.Contains("Bulky", result);
            Assert.Contains(", ", result, "Should use comma separator by default");
        }

        [TestMethod]
        public void StringList_Null_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, CreatureTrait.StringList(null));
        }

        [TestMethod]
        public void GetTraitDefinition_KnownId_ReturnsDefinition()
        {
            var def = TraitDefinition.GetTraitDefinition("Athletic");
            Assert.IsNotNull(def);
            Assert.AreEqual("Athletic", def.Name);
        }

        [TestMethod]
        public void GetTraitDefinition_UnknownId_ReturnsNull()
        {
            Assert.IsNull(TraitDefinition.GetTraitDefinition("UnknownTrait"));
        }

        [TestMethod]
        public void GetTraitDefinitions_ReturnsAllRegistered()
        {
            var defs = TraitDefinition.GetTraitDefinitions();
            Assert.IsNotNull(defs);
            Assert.HasCount(2, defs);
        }
    }
}
