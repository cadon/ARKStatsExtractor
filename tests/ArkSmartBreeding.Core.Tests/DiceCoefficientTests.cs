using ARKBreedingStats.OCR;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for the DiceCoefficient string similarity algorithm.
    /// </summary>
    [TestClass]
    public class DiceCoefficientTests
    {
        [TestMethod]
        public void IdenticalStrings_ReturnsOne()
        {
            Assert.AreEqual(1.0, DiceCoefficient.diceCoefficient("raptor", "raptor"), 0.0001);
        }

        [TestMethod]
        public void CompletelyDifferent_ReturnsZero()
        {
            Assert.AreEqual(0.0, DiceCoefficient.diceCoefficient("aaaa", "zzzz"), 0.0001);
        }

        [TestMethod]
        public void PartialMatch_ReturnsBetweenZeroAndOne()
        {
            var result = DiceCoefficient.diceCoefficient("night", "nacht");
            Assert.IsGreaterThan(0.0, result, $"Expected > 0 but got {result}");
            Assert.IsLessThan(1.0, result, $"Expected < 1 but got {result}");
        }

        [TestMethod]
        public void IsSymmetric()
        {
            var ab = DiceCoefficient.diceCoefficient("Triceratops", "Triceraptor");
            var ba = DiceCoefficient.diceCoefficient("Triceraptor", "Triceratops");
            Assert.AreEqual(ab, ba, 0.0001);
        }

        [TestMethod]
        public void SingleCharStrings_SameChar_ReturnsOne()
        {
            Assert.AreEqual(1.0, DiceCoefficient.diceCoefficient("x", "x"), 0.0001);
        }

        [TestMethod]
        public void SingleCharStrings_DifferentChars_ReturnsZero()
        {
            Assert.AreEqual(0.0, DiceCoefficient.diceCoefficient("x", "y"), 0.0001);
        }

        [TestMethod]
        public void SimilarCreatureNames_HighCoefficient()
        {
            var result = DiceCoefficient.diceCoefficient("Tyrannosaurus", "Tyrannosauros");
            Assert.IsGreaterThan(0.8, result, $"Very similar names should be > 0.8 but was {result}");
        }

        [TestMethod]
        public void EmptyAndNonEmpty_ReturnsZero()
        {
            var result = DiceCoefficient.diceCoefficient("", "hello");
            Assert.AreEqual(0.0, result, 0.0001);
        }
    }
}
