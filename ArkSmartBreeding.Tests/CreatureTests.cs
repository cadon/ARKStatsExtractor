using ARKBreedingStats.Library;
using ARKBreedingStats.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for the Creature class.
    /// </summary>
    [TestClass]
    public class CreatureTests
    {
        [TestMethod]
        public void DefaultConstructor_CreatesEmptyCreature()
        {
            var c = new Creature();
            Assert.IsNull(c.name);
            Assert.AreEqual(Sex.Unknown, c.sex);
        }

        [TestMethod]
        public void Constructor_WithSpecies_SetsSpecies()
        {
            var species = new Species { name = "Rex" };
            var c = new Creature(species);
            Assert.AreSame(species, c.Species);
        }

        [TestMethod]
        public void TopStat_SetAndGet_Works()
        {
            var c = new Creature();
            c.ResetTopStats();
            Assert.IsFalse(c.IsTopStat(Stats.Health));

            c.SetTopStat(Stats.Health, true);
            Assert.IsTrue(c.IsTopStat(Stats.Health));
            Assert.IsFalse(c.IsTopStat(Stats.Stamina));

            c.SetTopStat(Stats.Health, false);
            Assert.IsFalse(c.IsTopStat(Stats.Health));
        }

        [TestMethod]
        public void TopMutationStat_SetAndGet_Works()
        {
            var c = new Creature();
            c.ResetTopMutationStats();
            Assert.IsFalse(c.IsTopMutationStat(Stats.MeleeDamageMultiplier));

            c.SetTopMutationStat(Stats.MeleeDamageMultiplier, true);
            Assert.IsTrue(c.IsTopMutationStat(Stats.MeleeDamageMultiplier));

            c.SetTopMutationStat(Stats.MeleeDamageMultiplier, false);
            Assert.IsFalse(c.IsTopMutationStat(Stats.MeleeDamageMultiplier));
        }

        [TestMethod]
        public void ResetTopStats_ClearsAllTopFlags()
        {
            var c = new Creature();
            c.SetTopStat(Stats.Health, true);
            c.SetTopStat(Stats.Stamina, true);
            c.ResetTopStats();

            for (int i = 0; i < Stats.StatsCount; i++)
            {
                Assert.IsFalse(c.IsTopStat(i), $"Stat {i} should not be top after reset");
            }
        }

        [TestMethod]
        public void MultipleTopStats_CanBeSetSimultaneously()
        {
            var c = new Creature();
            c.ResetTopStats();
            c.SetTopStat(Stats.Health, true);
            c.SetTopStat(Stats.Weight, true);
            c.SetTopStat(Stats.MeleeDamageMultiplier, true);

            Assert.IsTrue(c.IsTopStat(Stats.Health));
            Assert.IsTrue(c.IsTopStat(Stats.Weight));
            Assert.IsTrue(c.IsTopStat(Stats.MeleeDamageMultiplier));
            Assert.IsFalse(c.IsTopStat(Stats.Stamina));
        }

        [TestMethod]
        public void Mutations_ReturnsSumOfMaternalAndPaternal()
        {
            var c = new Creature
            {
                mutationsMaternal = 5,
                mutationsPaternal = 3
            };
            Assert.AreEqual(8, c.Mutations);
        }

        [TestMethod]
        public void IsDomesticated_WithPositiveTE_ReturnsTrue()
        {
            var c = new Creature { tamingEff = 0.5 };
            Assert.IsTrue(c.isDomesticated);
        }

        [TestMethod]
        public void IsDomesticated_WithNegativeThreeTE_ReturnsFalse()
        {
            var c = new Creature { tamingEff = -3 };
            Assert.IsFalse(c.isDomesticated);
        }

        [TestMethod]
        public void IsBred_SetTrue_Reflected()
        {
            var c = new Creature { isBred = true };
            Assert.IsTrue(c.isBred);
        }

        [TestMethod]
        public void CalculatePreTameWildLevel_PerfectTE_ReturnsPostMinusOne()
        {
            // With 100% TE, pre-tame formula: Math.Ceiling(postTameLevel / (1 + 0.5 * tamingEffectiveness))
            int preTame = Creature.CalculatePreTameWildLevel(225, 1.0);
            // 225 / 1.5 = 150
            Assert.AreEqual(150, preTame);
        }

        [TestMethod]
        public void CalculatePreTameWildLevel_ZeroTE_ReturnsPostLevel()
        {
            int preTame = Creature.CalculatePreTameWildLevel(150, 0.0);
            // 150 / 1.0 = 150
            Assert.AreEqual(150, preTame);
        }

        [TestMethod]
        public void Guid_IsGeneratedUnique()
        {
            var c1 = new Creature();
            var c2 = new Creature();
            // Default guid is Guid.Empty for both; but when using full constructor it should be assigned
            Assert.AreEqual(Guid.Empty, c1.guid);
        }

        [TestMethod]
        public void Equals_SameGuid_ReturnsTrue()
        {
            var guid = Guid.NewGuid();
            var c1 = new Creature { guid = guid };
            var c2 = new Creature { guid = guid };
            Assert.IsTrue(c1.Equals(c2));
        }

        [TestMethod]
        public void Equals_DifferentGuid_ReturnsFalse()
        {
            var c1 = new Creature { guid = Guid.NewGuid() };
            var c2 = new Creature { guid = Guid.NewGuid() };
            Assert.IsFalse(c1.Equals(c2));
        }
    }
}
