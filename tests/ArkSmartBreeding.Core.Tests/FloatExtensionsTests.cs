using System;
using ARKBreedingStats;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for the FloatExtensions.FloatPrecision extension method (ULP calculation).
    /// </summary>
    [TestClass]
    public class FloatExtensionsTests
    {
        [TestMethod]
        public void FloatPrecision_OfOne_ReturnsSmallPositiveValue()
        {
            // Act
            float ulp = 1.0f.FloatPrecision();

            // Assert
            Assert.IsGreaterThan(0, ulp, "ULP of 1.0 should be positive");
            // ULP of 1.0f is 2^-23 ≈ 1.1920929E-07
            Assert.AreEqual(1.1920929E-07f, ulp, 1E-12f);
        }

        [TestMethod]
        public void FloatPrecision_OfZero_ReturnsSmallValue()
        {
            // Act
            float ulp = 0.0f.FloatPrecision();

            // Assert
            // For 0.0f the bit pattern is 0, incrementing gives the smallest positive denorm ~1.4E-45
            // but 0.0f is neither > 0 nor < 0, so the sign branch returns a negative ULP.
            // The function returns v - x where v is next representable, and x = 0
            Assert.AreNotEqual(0.0f, ulp, "ULP of 0 should not be zero");
        }

        [TestMethod]
        public void FloatPrecision_OfNaN_ReturnsNaN()
        {
            // Act
            float ulp = float.NaN.FloatPrecision();

            // Assert
            Assert.IsTrue(float.IsNaN(ulp), "NaN input should return NaN");
        }

        [TestMethod]
        public void FloatPrecision_LargerValues_HaveLargerUlp()
        {
            // Act
            float ulpSmall = 1.0f.FloatPrecision();
            float ulpLarge = 1000.0f.FloatPrecision();

            // Assert
            Assert.IsGreaterThan(ulpSmall, ulpLarge, "Larger values should have larger ULP");
        }

        [TestMethod]
        public void FloatPrecision_NegativeValue_ReturnsNonZeroUlp()
        {
            // Act
            // For negative values the bit pattern increment moves the magnitude further from zero
            // so the ULP (v - x) will be negative (more negative minus less negative)
            float ulp = (-1.0f).FloatPrecision();

            // Assert
            Assert.AreNotEqual(0.0f, ulp, "ULP of -1 should not be zero");
        }
    }
}
