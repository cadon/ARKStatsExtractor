using ARKBreedingStats.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for MinMaxDouble and MinMaxInt value types.
    /// </summary>
    [TestClass]
    public class MinMaxDoubleTests
    {
        [TestMethod]
        public void Constructor_MinMax_SetsFields()
        {
            // Act
            var r = new MinMaxDouble(2.0, 8.0);

            // Assert
            Assert.AreEqual(2.0, r.Min);
            Assert.AreEqual(8.0, r.Max);
        }

        [TestMethod]
        public void Constructor_SingleValue_SetsBothEqual()
        {
            // Act
            var r = new MinMaxDouble(5.0);

            // Assert
            Assert.AreEqual(5.0, r.Min);
            Assert.AreEqual(5.0, r.Max);
        }

        [TestMethod]
        public void CopyConstructor_CopiesValues()
        {
            // Arrange
            var original = new MinMaxDouble(1.0, 9.0);

            // Act
            var copy = new MinMaxDouble(original);

            // Assert
            Assert.AreEqual(original.Min, copy.Min);
            Assert.AreEqual(original.Max, copy.Max);
        }

        [TestMethod]
        public void Mean_ReturnsAverage()
        {
            // Arrange
            var r = new MinMaxDouble(2.0, 8.0);

            // Act & Assert
            Assert.AreEqual(5.0, r.Mean, 0.0001);
        }

        [TestMethod]
        public void MinMaxSetter_SetsBothValues()
        {
            // Arrange
            var r = new MinMaxDouble(1.0, 9.0);

            // Act
            r.MinMax = 5.0;

            // Assert
            Assert.AreEqual(5.0, r.Min);
            Assert.AreEqual(5.0, r.Max);
        }

        [TestMethod]
        public void ValidRange_WhenMinLessThanMax_ReturnsTrue()
        {
            // Act & Assert
            Assert.IsTrue(new MinMaxDouble(1.0, 5.0).ValidRange);
        }

        [TestMethod]
        public void ValidRange_WhenMinEqualsMax_ReturnsTrue()
        {
            // Act & Assert
            Assert.IsTrue(new MinMaxDouble(3.0).ValidRange);
        }

        [TestMethod]
        public void ValidRange_WhenMinGreaterThanMax_ReturnsFalse()
        {
            // Act & Assert
            Assert.IsFalse(new MinMaxDouble(5.0, 1.0).ValidRange);
        }

        [TestMethod]
        public void Includes_Range_ContainedRange_ReturnsTrue()
        {
            // Arrange
            var outer = new MinMaxDouble(1.0, 10.0);
            var inner = new MinMaxDouble(3.0, 7.0);

            // Act & Assert
            Assert.IsTrue(outer.Includes(inner));
        }

        [TestMethod]
        public void Includes_Range_NotContained_ReturnsFalse()
        {
            // Arrange
            var outer = new MinMaxDouble(3.0, 7.0);
            var wider = new MinMaxDouble(1.0, 10.0);

            // Act & Assert
            Assert.IsFalse(outer.Includes(wider));
        }

        [TestMethod]
        public void Includes_Value_InRange_ReturnsTrue()
        {
            // Arrange
            var r = new MinMaxDouble(1.0, 10.0);

            // Act & Assert
            Assert.IsTrue(r.Includes(5.0));
        }

        [TestMethod]
        public void Includes_Value_OutOfRange_ReturnsFalse()
        {
            // Arrange
            var r = new MinMaxDouble(1.0, 10.0);

            // Act & Assert
            Assert.IsFalse(r.Includes(11.0));
        }

        [TestMethod]
        public void Overlaps_OverlappingRanges_ReturnsTrue()
        {
            // Arrange
            var a = new MinMaxDouble(1.0, 5.0);
            var b = new MinMaxDouble(4.0, 8.0);

            // Act & Assert
            Assert.IsTrue(a.Overlaps(b));
            Assert.IsTrue(MinMaxDouble.Overlaps(a, b));
        }

        [TestMethod]
        public void Overlaps_NonOverlappingRanges_ReturnsFalse()
        {
            // Arrange
            var a = new MinMaxDouble(1.0, 3.0);
            var b = new MinMaxDouble(5.0, 8.0);

            // Act & Assert
            Assert.IsFalse(a.Overlaps(b));
        }

        [TestMethod]
        public void SetToIntersectionWith_Overlapping_NarrowsRange()
        {
            // Arrange
            var a = new MinMaxDouble(1.0, 8.0);
            var b = new MinMaxDouble(3.0, 6.0);

            // Act
            bool result = a.SetToIntersectionWith(b);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(3.0, a.Min, 0.0001);
            Assert.AreEqual(6.0, a.Max, 0.0001);
        }

        [TestMethod]
        public void SetToIntersectionWith_NonOverlapping_ReturnsFalse()
        {
            // Arrange
            var a = new MinMaxDouble(1.0, 3.0);
            var b = new MinMaxDouble(5.0, 8.0);

            // Act
            bool result = a.SetToIntersectionWith(b);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(1.0, a.Min);
            Assert.AreEqual(3.0, a.Max);
        }

        [TestMethod]
        public void Clone_CreatesIndependentCopy()
        {
            // Arrange
            var original = new MinMaxDouble(2.0, 8.0);

            // Act
            var clone = original.Clone();
            clone.Min = 99.0;

            // Assert
            Assert.AreEqual(2.0, original.Min, "Original should not be affected");
        }

        [TestMethod]
        public void OperatorAdd_AddsToMinAndMax()
        {
            // Act
            var r = new MinMaxDouble(2.0, 8.0) + 3.0;

            // Assert
            Assert.AreEqual(5.0, r.Min, 0.0001);
            Assert.AreEqual(11.0, r.Max, 0.0001);
        }

        [TestMethod]
        public void OperatorSubtract_SubtractsFromMinAndMax()
        {
            // Act
            var r = new MinMaxDouble(5.0, 10.0) - 2.0;

            // Assert
            Assert.AreEqual(3.0, r.Min, 0.0001);
            Assert.AreEqual(8.0, r.Max, 0.0001);
        }

        [TestMethod]
        public void OperatorMultiply_MultipliesMinAndMax()
        {
            // Act
            var r = new MinMaxDouble(2.0, 5.0) * 3.0;

            // Assert
            Assert.AreEqual(6.0, r.Min, 0.0001);
            Assert.AreEqual(15.0, r.Max, 0.0001);
        }

        [TestMethod]
        public void OperatorDivide_DividesMinAndMax()
        {
            // Act
            var r = new MinMaxDouble(6.0, 12.0) / 3.0;

            // Assert
            Assert.AreEqual(2.0, r.Min, 0.0001);
            Assert.AreEqual(4.0, r.Max, 0.0001);
        }

        [TestMethod]
        public void ToString_ContainsMinMeanMax()
        {
            // Arrange
            var r = new MinMaxDouble(2.0, 8.0);

            // Act
            var s = r.ToString();

            // Assert
            Assert.Contains("2", s, "Should contain Min");
            Assert.Contains("5", s, "Should contain Mean");
            Assert.Contains("8", s, "Should contain Max");
        }
    }

    [TestClass]
    public class MinMaxIntTests
    {
        [TestMethod]
        public void Constructor_IntMinMax_SetsFields()
        {
            // Act
            var r = new MinMaxInt(3, 7);

            // Assert
            Assert.AreEqual(3, r.Min);
            Assert.AreEqual(7, r.Max);
        }

        [TestMethod]
        public void Constructor_DoubleMinMax_CeilsMinFloorsMax()
        {
            // Act
            var r = new MinMaxInt(2.3, 7.8);

            // Assert
            Assert.AreEqual(3, r.Min, "Min should be Ceiling(2.3) = 3");
            Assert.AreEqual(7, r.Max, "Max should be Floor(7.8) = 7");
        }

        [TestMethod]
        public void ValidRange_WhenMinLessThanMax_ReturnsTrue()
        {
            // Act & Assert
            Assert.IsTrue(new MinMaxInt(1, 5).ValidRange);
        }

        [TestMethod]
        public void ValidRange_WhenMinGreaterThanMax_ReturnsFalse()
        {
            // Act & Assert
            // e.g. Ceiling(7.9) = 8, Floor(7.1) = 7 → invalid
            Assert.IsFalse(new MinMaxInt(7.9, 7.1).ValidRange);
        }

        [TestMethod]
        public void Includes_ValueInRange_ReturnsTrue()
        {
            // Arrange
            var r = new MinMaxInt(1, 10);

            // Act & Assert
            Assert.IsTrue(r.Includes(5));
        }

        [TestMethod]
        public void Includes_ValueOutOfRange_ReturnsFalse()
        {
            // Arrange
            var r = new MinMaxInt(1, 10);

            // Act & Assert
            Assert.IsFalse(r.Includes(11));
        }

        [TestMethod]
        public void Mean_ReturnsAverage()
        {
            // Arrange
            var r = new MinMaxInt(2, 8);

            // Act & Assert
            Assert.AreEqual(5.0, r.Mean, 0.0001);
        }

        [TestMethod]
        public void MinMaxSetter_SetsBothValues()
        {
            // Arrange
            var r = new MinMaxInt(1, 9);

            // Act
            r.MinMax = 5;

            // Assert
            Assert.AreEqual(5, r.Min);
            Assert.AreEqual(5, r.Max);
        }
    }
}
