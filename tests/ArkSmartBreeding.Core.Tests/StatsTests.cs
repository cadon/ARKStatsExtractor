using ARKBreedingStats.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for the Stats static class (stat indices, display order, precision).
    /// </summary>
    [TestClass]
    public class StatsTests
    {
        [TestMethod]
        public void DisplayOrder_Has12Elements()
        {
            Assert.HasCount(Stats.StatsCount, Stats.DisplayOrder);
        }

        [TestMethod]
        public void DisplayOrder_ContainsAllStatIndices()
        {
            for (int i = 0; i < Stats.StatsCount; i++)
            {
                bool found = false;
                foreach (var s in Stats.DisplayOrder)
                {
                    if (s == i) { found = true; break; }
                }
                Assert.IsTrue(found, $"Stat index {i} should be in DisplayOrder");
            }
        }

        [TestMethod]
        public void UsuallyVisibleStats_Has12Elements()
        {
            Assert.HasCount(Stats.StatsCount, Stats.UsuallyVisibleStats);
        }

        [TestMethod]
        public void IsPercentage_MeleeDamage_ReturnsTrue()
        {
            Assert.IsTrue(Stats.IsPercentage(Stats.MeleeDamageMultiplier));
        }

        [TestMethod]
        public void IsPercentage_SpeedMultiplier_ReturnsTrue()
        {
            Assert.IsTrue(Stats.IsPercentage(Stats.SpeedMultiplier));
        }

        [TestMethod]
        public void IsPercentage_Health_ReturnsFalse()
        {
            Assert.IsFalse(Stats.IsPercentage(Stats.Health));
        }

        [TestMethod]
        public void IsPercentage_Weight_ReturnsFalse()
        {
            Assert.IsFalse(Stats.IsPercentage(Stats.Weight));
        }

        [TestMethod]
        public void Precision_PercentageStat_Returns3()
        {
            Assert.AreEqual(3, Stats.Precision(Stats.MeleeDamageMultiplier));
            Assert.AreEqual(3, Stats.Precision(Stats.SpeedMultiplier));
        }

        [TestMethod]
        public void Precision_NonPercentageStat_Returns1()
        {
            Assert.AreEqual(1, Stats.Precision(Stats.Health));
            Assert.AreEqual(1, Stats.Precision(Stats.Weight));
            Assert.AreEqual(1, Stats.Precision(Stats.Stamina));
        }
    }
}
