using ARKBreedingStats.miscClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ARKBreedingStats.Tests
{
    /// <summary>
    /// Smoke tests for the StatResult class.
    /// </summary>
    [TestClass]
    public class StatResultTests
    {
        [TestMethod]
        public void StatResult_Constructor_SetsLevelsCorrectly()
        {
            // Arrange & Act
            var result = new StatResult(levelWild: 10, levelDom: 5, levelMut: 1);

            // Assert
            Assert.AreEqual(10, result.LevelWild);
            Assert.AreEqual(5, result.LevelDom);
            Assert.AreEqual(1, result.LevelMut);
        }

        [TestMethod]
        public void StatResult_Constructor_DefaultsToNoMutation()
        {
            // Act
            var result = new StatResult(15, 3);

            // Assert
            Assert.AreEqual(0, result.LevelMut, "Default mutation level should be 0");
        }

        [TestMethod]
        public void StatResult_Constructor_DefaultsCurrentlyNotValidToFalse()
        {
            // Act
            var result = new StatResult(5, 2);

            // Assert
            Assert.IsFalse(result.CurrentlyNotValid, "CurrentlyNotValid should default to false");
        }

        [TestMethod]
        public void StatResult_WithExplicitTe_StoresTe()
        {
            // Arrange
            var te = new MinMaxDouble(0.7, 1.0);

            // Act
            var result = new StatResult(8, 4, te);

            // Assert
            Assert.AreEqual(0.7, result.Te.Min, 0.0001);
            Assert.AreEqual(1.0, result.Te.Max, 0.0001);
        }

        [TestMethod]
        public void StatResult_WithNoTe_StoresNegativeOneMeanTe()
        {
            // When no TE is supplied the convention is MinMaxDouble(-1).
            var result = new StatResult(1, 0);

            Assert.AreEqual(-1.0, result.Te.Mean, 0.0001,
                "Default TE should be -1 (indicating no taming effectiveness)");
        }

        [TestMethod]
        public void StatResult_ToString_ContainsAllComponents()
        {
            // Arrange
            var result = new StatResult(levelWild: 3, levelDom: 7, levelMut: 2);

            // Act
            var s = result.ToString();

            // Assert
            Assert.IsTrue(s.Contains("3"), "ToString should contain wild level");
            Assert.IsTrue(s.Contains("7"), "ToString should contain dom level");
            Assert.IsTrue(s.Contains("2"), "ToString should contain mut level");
        }
    }

    /// <summary>
    /// Smoke tests for the MinMaxDouble struct.
    /// </summary>
    [TestClass]
    public class MinMaxDoubleTests
    {
        [TestMethod]
        public void MinMaxDouble_Constructor_SetsMinAndMax()
        {
            var range = new MinMaxDouble(2.0, 8.0);

            Assert.AreEqual(2.0, range.Min);
            Assert.AreEqual(8.0, range.Max);
        }

        [TestMethod]
        public void MinMaxDouble_SingleValueConstructor_SetsBothToSameValue()
        {
            var range = new MinMaxDouble(5.0);

            Assert.AreEqual(5.0, range.Min);
            Assert.AreEqual(5.0, range.Max);
        }

        [TestMethod]
        public void MinMaxDouble_Mean_IsAverage()
        {
            var range = new MinMaxDouble(2.0, 8.0);

            Assert.AreEqual(5.0, range.Mean, 0.0001);
        }

        [TestMethod]
        public void MinMaxDouble_ValidRange_TrueWhenMinLessThanMax()
        {
            Assert.IsTrue(new MinMaxDouble(1.0, 2.0).ValidRange);
        }

        [TestMethod]
        public void MinMaxDouble_ValidRange_TrueWhenEqual()
        {
            Assert.IsTrue(new MinMaxDouble(3.0).ValidRange);
        }

        [TestMethod]
        public void MinMaxDouble_ValidRange_FalseWhenMinGreaterThanMax()
        {
            var inverted = new MinMaxDouble(10.0, 1.0);
            Assert.IsFalse(inverted.ValidRange);
        }

        [TestMethod]
        public void MinMaxDouble_Includes_ReturnsTrueForValueInsideRange()
        {
            var range = new MinMaxDouble(0.0, 10.0);

            Assert.IsTrue(range.Includes(5.0));
            Assert.IsTrue(range.Includes(0.0), "Min boundary should be included");
            Assert.IsTrue(range.Includes(10.0), "Max boundary should be included");
        }

        [TestMethod]
        public void MinMaxDouble_Includes_ReturnsFalseForValueOutsideRange()
        {
            var range = new MinMaxDouble(2.0, 8.0);

            Assert.IsFalse(range.Includes(-1.0));
            Assert.IsFalse(range.Includes(9.0));
        }

        [TestMethod]
        public void MinMaxDouble_Overlaps_ReturnsTrueForOverlappingRanges()
        {
            var a = new MinMaxDouble(0.0, 5.0);
            var b = new MinMaxDouble(3.0, 8.0);

            Assert.IsTrue(a.Overlaps(b));
            Assert.IsTrue(b.Overlaps(a));
        }

        [TestMethod]
        public void MinMaxDouble_Overlaps_ReturnsFalseForNonOverlappingRanges()
        {
            var a = new MinMaxDouble(0.0, 2.0);
            var b = new MinMaxDouble(5.0, 8.0);

            Assert.IsFalse(a.Overlaps(b));
        }

        [TestMethod]
        public void MinMaxDouble_SetToIntersectionWith_ModifiesRangeCorrectly()
        {
            var a = new MinMaxDouble(0.0, 6.0);
            var b = new MinMaxDouble(4.0, 10.0);

            bool overlaps = a.SetToIntersectionWith(b);

            Assert.IsTrue(overlaps);
            Assert.AreEqual(4.0, a.Min, 0.0001);
            Assert.AreEqual(6.0, a.Max, 0.0001);
        }

        [TestMethod]
        public void MinMaxDouble_SetToIntersectionWith_ReturnsFalseAndDoesNotModifyWhenNoOverlap()
        {
            var a = new MinMaxDouble(0.0, 2.0);
            var b = new MinMaxDouble(5.0, 8.0);

            bool overlaps = a.SetToIntersectionWith(b);

            Assert.IsFalse(overlaps);
            // original values should be unchanged
            Assert.AreEqual(0.0, a.Min, 0.0001);
            Assert.AreEqual(2.0, a.Max, 0.0001);
        }

        [TestMethod]
        public void MinMaxDouble_AddOperator_ShiftsRangeByOffset()
        {
            var range = new MinMaxDouble(2.0, 8.0);
            var shifted = range + 3.0;

            Assert.AreEqual(5.0, shifted.Min, 0.0001);
            Assert.AreEqual(11.0, shifted.Max, 0.0001);
        }

        [TestMethod]
        public void MinMaxDouble_MultiplyOperator_ScalesRange()
        {
            var range = new MinMaxDouble(2.0, 4.0);
            var scaled = range * 0.5;

            Assert.AreEqual(1.0, scaled.Min, 0.0001);
            Assert.AreEqual(2.0, scaled.Max, 0.0001);
        }
    }
}
