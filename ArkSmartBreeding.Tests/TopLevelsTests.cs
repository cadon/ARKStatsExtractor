using ARKBreedingStats.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for the TopLevels class.
    /// </summary>
    [TestClass]
    public class TopLevelsTests
    {
        [TestMethod]
        public void DefaultConstructor_HighestArraysAreZero()
        {
            var tl = new TopLevels();
            for (int i = 0; i < Stats.StatsCount; i++)
            {
                Assert.AreEqual(0, tl.WildLevelsHighest[i]);
                Assert.AreEqual(0, tl.MutationLevelsHighest[i]);
            }
        }

        [TestMethod]
        public void DefaultConstructor_LowestArraysAreMaxValue()
        {
            var tl = new TopLevels();
            for (int i = 0; i < Stats.StatsCount; i++)
            {
                Assert.AreEqual(int.MaxValue, tl.WildLevelsLowest[i]);
                Assert.AreEqual(int.MaxValue, tl.MutationLevelsLowest[i]);
            }
        }

        [TestMethod]
        public void AllZerosConstructor_AllArraysAreZero()
        {
            var tl = new TopLevels(allZeros: true);
            for (int i = 0; i < Stats.StatsCount; i++)
            {
                Assert.AreEqual(0, tl.WildLevelsHighest[i]);
                Assert.AreEqual(0, tl.WildLevelsLowest[i]);
                Assert.AreEqual(0, tl.MutationLevelsHighest[i]);
                Assert.AreEqual(0, tl.MutationLevelsLowest[i]);
            }
        }

        [TestMethod]
        public void AllZerosFalse_SameAsDefault()
        {
            var tl = new TopLevels(allZeros: false);
            Assert.AreEqual(0, tl.WildLevelsHighest[0]);
            Assert.AreEqual(int.MaxValue, tl.WildLevelsLowest[0]);
        }

        [TestMethod]
        public void MinLevelForTopCreature_DefaultIsNegativeOne()
        {
            var tl = new TopLevels();
            Assert.AreEqual(-1, tl.MinLevelForTopCreature);
        }

        [TestMethod]
        public void ArrayLengths_AreStatsCount()
        {
            var tl = new TopLevels();
            Assert.HasCount(Stats.StatsCount, tl.WildLevelsHighest);
            Assert.HasCount(Stats.StatsCount, tl.WildLevelsLowest);
            Assert.HasCount(Stats.StatsCount, tl.MutationLevelsHighest);
            Assert.HasCount(Stats.StatsCount, tl.MutationLevelsLowest);
        }
    }
}
