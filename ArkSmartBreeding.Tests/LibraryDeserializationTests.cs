using ARKBreedingStats;
using ARKBreedingStats.Library;
using ARKBreedingStats.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Integration tests that load the library.asb test asset and verify deserialization.
    /// </summary>
    [TestClass]
    public class LibraryDeserializationTests
    {
        private static CreatureCollection _collection;

        [ClassInitialize]
        public static void LoadLibrary(TestContext context)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "assets", "library.asb");
            Assert.IsTrue(File.Exists(path), $"Test asset not found at: {path}");

            var json = File.ReadAllText(path);
            _collection = JsonConvert.DeserializeObject<CreatureCollection>(json);
            Assert.IsNotNull(_collection, "Deserialized CreatureCollection should not be null");
        }

        [TestMethod]
        public void FormatVersion_IsCurrentVersion()
        {
            Assert.AreEqual(CreatureCollection.CurrentLibraryFormatVersion, _collection.FormatVersion);
        }

        [TestMethod]
        public void Creatures_IsNotEmpty()
        {
            Assert.IsNotNull(_collection.creatures);
            Assert.IsNotEmpty(_collection.creatures, "Library should contain creatures");
        }

        [TestMethod]
        public void Game_IsASA()
        {
            Assert.AreEqual("ASA", _collection.Game);
        }

        [TestMethod]
        public void MaxDomLevel_IsDefault88()
        {
            Assert.AreEqual(CreatureCollection.MaxDomLevelDefault, _collection.maxDomLevel);
        }

        [TestMethod]
        public void MaxWildLevel_Is150()
        {
            Assert.AreEqual(150, _collection.maxWildLevel);
        }

        [TestMethod]
        public void WildLevelStep_Is5()
        {
            Assert.AreEqual(5, _collection.wildLevelStep);
        }

        [TestMethod]
        public void ServerMultipliers_AreNotNull()
        {
            Assert.IsNotNull(_collection.serverMultipliers);
        }

        [TestMethod]
        public void ServerMultipliers_StatMultipliers_Have12Stats()
        {
            Assert.IsNotNull(_collection.serverMultipliers.statMultipliers);
            Assert.HasCount(Stats.StatsCount, _collection.serverMultipliers.statMultipliers);
        }

        [TestMethod]
        public void Creatures_HaveSpeciesBlueprint()
        {
            foreach (var c in _collection.creatures)
            {
                Assert.IsFalse(string.IsNullOrEmpty(c.speciesBlueprint),
                    $"Creature '{c.name}' should have a speciesBlueprint");
            }
        }

        [TestMethod]
        public void Creatures_HaveLevelsWild_With12Stats()
        {
            foreach (var c in _collection.creatures.Where(c => c.levelsWild != null))
            {
                Assert.HasCount(Stats.StatsCount, c.levelsWild,
                    $"Creature '{c.name}' levelsWild should have {Stats.StatsCount} entries");
            }
        }

        [TestMethod]
        public void Creatures_WithColors_Have6Regions()
        {
            foreach (var c in _collection.creatures.Where(c => c.colors != null))
            {
                Assert.HasCount(Ark.ColorRegionCount, c.colors,
                    $"Creature '{c.name}' colors should have {Ark.ColorRegionCount} regions");
            }
        }

        [TestMethod]
        public void Creatures_ContainRex()
        {
            bool hasRex = _collection.creatures.Any(c =>
                c.speciesBlueprint != null && c.speciesBlueprint.Contains("Rex_Character_BP"));
            Assert.IsTrue(hasRex, "Library should contain at least one Rex");
        }

        [TestMethod]
        public void Creatures_HaveValidSex()
        {
            foreach (var c in _collection.creatures)
            {
                Assert.IsTrue(Enum.IsDefined(typeof(Sex), c.sex),
                    $"Creature '{c.name}' has invalid sex value: {c.sex}");
            }
        }

        [TestMethod]
        public void BredCreatures_HaveTamingEffOne()
        {
            // Bred creatures have TE = 1.0 (perfect taming effectiveness)
            foreach (var c in _collection.creatures.Where(c => c.isBred && c.tamingEff > 0))
            {
                Assert.AreEqual(1.0, c.tamingEff, 0.0001,
                    $"Bred creature '{c.name}' should have TE = 1.0");
            }
        }

        [TestMethod]
        public void Creatures_WithArkId_HaveNonZeroId()
        {
            foreach (var c in _collection.creatures.Where(c => c.ArkIdImported))
            {
                Assert.AreNotEqual(0L, c.ArkId,
                    $"Imported creature '{c.name}' should have non-zero ArkId");
            }
        }

        [TestMethod]
        public void Players_ListIsNotNull()
        {
            Assert.IsNotNull(_collection.players);
        }

        [TestMethod]
        public void Tribes_ListIsNotNull()
        {
            Assert.IsNotNull(_collection.tribes);
        }

        [TestMethod]
        public void ModIDs_Contains_ASA()
        {
            Assert.IsNotNull(_collection.modIDs);
            Assert.Contains("ASA", _collection.modIDs);
        }

        [TestMethod]
        public void DeletedCreatureGuids_IsNotNull()
        {
            Assert.IsNotNull(_collection.DeletedCreatureGuids);
        }

        [TestMethod]
        public void CreatureCollection_RoundTrips_ThroughJson()
        {
            var json = JsonConvert.SerializeObject(_collection);
            var roundTripped = JsonConvert.DeserializeObject<CreatureCollection>(json);

            Assert.IsNotNull(roundTripped);
            Assert.HasCount(_collection.creatures.Count, roundTripped.creatures);
            Assert.AreEqual(_collection.FormatVersion, roundTripped.FormatVersion);
            Assert.AreEqual(_collection.Game, roundTripped.Game);
        }
    }
}
