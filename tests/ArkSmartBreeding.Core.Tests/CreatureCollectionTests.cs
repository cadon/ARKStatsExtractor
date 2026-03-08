using ARKBreedingStats;
using ARKBreedingStats.Library;
using ARKBreedingStats.Models;
using ARKBreedingStats.Mods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for CreatureCollection methods.
    /// </summary>
    [TestClass]
    public class CreatureCollectionTests
    {
        [TestMethod]
        public void NewCollection_HasSensibleDefaults()
        {
            // Arrange & Act
            var cc = new CreatureCollection();

            // Assert
            Assert.AreEqual(CreatureCollection.CurrentLibraryFormatVersion, cc.FormatVersion);
            Assert.AreEqual(CreatureCollection.MaxDomLevelDefault, cc.maxDomLevel);
            Assert.AreEqual(Ark.MaxWildLevelDefault, cc.maxWildLevel);
            Assert.AreEqual(Ark.WildLevelStepDefault, cc.wildLevelStep);
            Assert.IsNotNull(cc.creatures);
            Assert.IsEmpty(cc.creatures);
        }

        [TestMethod]
        public void DeleteCreature_RemovesFromList()
        {
            // Arrange
            var cc = new CreatureCollection();
            var c = new Creature
            {
                guid = Guid.NewGuid(),
                name = "TestRex",
                Species = new Species { name = "TestRex", blueprintPath = "TestBP" }
            };
            cc.creatures.Add(c);

            // Act
            cc.DeleteCreature(c);

            // Assert
            Assert.IsEmpty(cc.creatures);
        }

        [TestMethod]
        public void DeleteCreature_AddsToDeletedGuids()
        {
            // Arrange
            var cc = new CreatureCollection();
            cc.DeletedCreatureGuids = new List<Guid>();
            var c = new Creature
            {
                guid = Guid.NewGuid(),
                Species = new Species { name = "TestRex", blueprintPath = "TestBP" }
            };
            cc.creatures.Add(c);

            // Act
            cc.DeleteCreature(c);

            // Assert
            Assert.Contains(c.guid, cc.DeletedCreatureGuids);
        }

        [TestMethod]
        public void ArkIdAlreadyExist_NewId_ReturnsFalse()
        {
            // Arrange
            var cc = new CreatureCollection();
            var c = new Creature { guid = Guid.NewGuid(), ArkId = 12345L };
            cc.creatures.Add(c);

            // Act
            bool exists = cc.ArkIdAlreadyExist(99999L, c, out var found);

            // Assert
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void ArkIdAlreadyExist_DuplicateId_ReturnsTrue()
        {
            // Arrange
            var cc = new CreatureCollection();
            var c1 = new Creature { guid = Guid.NewGuid(), ArkId = 12345L };
            var c2 = new Creature { guid = Guid.NewGuid(), ArkId = 12345L };
            cc.creatures.Add(c1);

            // Act
            bool exists = cc.ArkIdAlreadyExist(12345L, c2, out var found);

            // Assert
            Assert.IsTrue(exists);
            Assert.AreSame(c1, found);
        }

        [TestMethod]
        public void CreatureById_Guid_FindsCreature()
        {
            // Arrange
            var cc = new CreatureCollection();
            var guid = Guid.NewGuid();
            var c = new Creature { guid = guid, ArkId = 0 };
            cc.creatures.Add(c);

            // Act
            bool found = cc.CreatureById(guid, 0, out var result);

            // Assert
            Assert.IsTrue(found);
            Assert.AreSame(c, result);
        }

        [TestMethod]
        public void CreatureById_NotFound_ReturnsFalse()
        {
            // Arrange
            var cc = new CreatureCollection();

            // Act
            bool found = cc.CreatureById(Guid.NewGuid(), 0, out _);

            // Assert
            Assert.IsFalse(found);
        }

        [TestMethod]
        public void GetWildLevelStep_NotConsidered_ReturnsNull()
        {
            // Arrange
            var cc = new CreatureCollection { considerWildLevelSteps = false };

            // Act & Assert
            Assert.IsNull(cc.getWildLevelStep());
        }

        [TestMethod]
        public void GetWildLevelStep_Considered_ReturnsStep()
        {
            // Arrange
            var cc = new CreatureCollection { considerWildLevelSteps = true, wildLevelStep = 5 };

            // Act & Assert
            Assert.AreEqual(5, cc.getWildLevelStep());
        }

        [TestMethod]
        public void MergeCreatureList_AddsNewCreatures()
        {
            // Arrange
            var cc = new CreatureCollection();
            var newCreature = new Creature { guid = Guid.NewGuid(), speciesBlueprint = "TestBP" };

            // Act
            cc.MergeCreatureList([newCreature]);

            // Assert
            Assert.HasCount(1, cc.creatures);
        }

        [TestMethod]
        public void CalculateModListHash_SameOrder_SameHash()
        {
            // Arrange
            string[] mods = ["mod1", "mod2", "mod3"];

            // Act
            int hash1 = CreatureCollection.CalculateModListHash(mods);
            int hash2 = CreatureCollection.CalculateModListHash(mods);

            // Assert
            Assert.AreEqual(hash1, hash2);
        }

        [TestMethod]
        public void CalculateModListHash_DifferentOrder_DifferentHash()
        {
            // Act
            int hash1 = CreatureCollection.CalculateModListHash(["mod1", "mod2"]);
            int hash2 = CreatureCollection.CalculateModListHash(["mod2", "mod1"]);

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void GetTotalCreatureCount_ViaMerge_ExcludesPlaceholders()
        {
            // Arrange
            CreatureCollection cc = new();
            Creature c1 = new() { guid = Guid.NewGuid(), speciesBlueprint = "BP1", flags = CreatureFlags.None };
            Creature c2 = new() { guid = Guid.NewGuid(), speciesBlueprint = "BP1", flags = CreatureFlags.Placeholder };
            cc.MergeCreatureList([c1, c2]);

            // Act
            int count = cc.GetTotalCreatureCount();

            // Assert
            Assert.AreEqual(1, count, "Placeholder creatures should not be counted");
        }
    }
}
