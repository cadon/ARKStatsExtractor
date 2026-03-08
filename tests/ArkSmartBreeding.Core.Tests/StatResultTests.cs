using ARKBreedingStats.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for the StatResult class.
    /// </summary>
    [TestClass]
    public class StatResultTests
    {
        [TestMethod]
        public void Constructor_SetsLevels()
        {
            var r = new StatResult(levelWild: 10, levelDom: 5, levelMut: 2);
            Assert.AreEqual(10, r.LevelWild);
            Assert.AreEqual(5, r.LevelDom);
            Assert.AreEqual(2, r.LevelMut);
        }

        [TestMethod]
        public void Constructor_DefaultMutationIsZero()
        {
            var r = new StatResult(15, 3);
            Assert.AreEqual(0, r.LevelMut);
        }

        [TestMethod]
        public void Constructor_DefaultCurrentlyNotValid_IsFalse()
        {
            var r = new StatResult(5, 2);
            Assert.IsFalse(r.CurrentlyNotValid);
        }

        [TestMethod]
        public void Constructor_WithTe_StoresTe()
        {
            var te = new MinMaxDouble(0.7, 1.0);
            var r = new StatResult(8, 4, te);
            Assert.AreEqual(0.7, r.Te.Min, 0.0001);
            Assert.AreEqual(1.0, r.Te.Max, 0.0001);
        }

        [TestMethod]
        public void Constructor_WithoutTe_DefaultTeIsNegativeOne()
        {
            var r = new StatResult(1, 0);
            Assert.AreEqual(-1.0, r.Te.Mean, 0.0001);
        }
    }
}
