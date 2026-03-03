using ARKBreedingStats.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for the SpeciesStat class.
    /// </summary>
    [TestClass]
    public class SpeciesStatTests
    {
        [TestMethod]
        public void DefaultValues_AreCorrect()
        {
            var stat = new SpeciesStat();
            Assert.AreEqual(0, stat.BaseValue);
            Assert.AreEqual(0, stat.IncPerWildLevel);
            Assert.AreEqual(0, stat.IncPerTamedLevel);
            Assert.AreEqual(0, stat.AddWhenTamed);
            Assert.AreEqual(0, stat.MultAffinity);
            Assert.IsTrue(stat.IncreaseStatAsPercentage, "Default should be percentage-based stat increase");
        }

        [TestMethod]
        public void ApplyCap_ValueBelowCap_ReturnsSameValue()
        {
            var stat = new SpeciesStat { ValueCap = 100 };
            Assert.AreEqual(50.0, stat.ApplyCap(50.0), 0.0001);
        }

        [TestMethod]
        public void ApplyCap_ValueAboveCap_ReturnsCap()
        {
            var stat = new SpeciesStat { ValueCap = 100 };
            Assert.AreEqual(100.0, stat.ApplyCap(200.0), 0.0001);
        }

        [TestMethod]
        public void ApplyCap_ValueEqualsCap_ReturnsCap()
        {
            var stat = new SpeciesStat { ValueCap = 100 };
            Assert.AreEqual(100.0, stat.ApplyCap(100.0), 0.0001);
        }

        [TestMethod]
        public void ApplyCap_ZeroCap_ReturnsZero()
        {
            // ValueCap defaults to 0; Min(anyPositive, 0) = 0
            var stat = new SpeciesStat();
            Assert.AreEqual(0.0, stat.ApplyCap(50.0), 0.0001);
        }
    }
}
