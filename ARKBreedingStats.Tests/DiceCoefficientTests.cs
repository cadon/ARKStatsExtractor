using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ARKBreedingStats.Tests
{
    /// <summary>
    /// Smoke tests for the DiceCoefficient class (Sørensen–Dice similarity).
    /// </summary>
    [TestClass]
    public class DiceCoefficientTests
    {
        [TestMethod]
        public void DiceCoefficient_IdenticalStrings_ReturnsOne()
        {
            // Arrange
            const string word = "raptor";

            // Act
            var result = DiceCoefficient.diceCoefficient(word, word);

            // Assert
            Assert.AreEqual(1.0, result, 0.0001, "Identical strings should have a coefficient of 1");
        }

        [TestMethod]
        public void DiceCoefficient_CompletelyDifferentStrings_ReturnsZero()
        {
            // Arrange – no shared bi-grams once the sentinel chars are included
            const string a = "aaaa";
            const string b = "zzzz";

            // Act
            var result = DiceCoefficient.diceCoefficient(a, b);

            // Assert
            Assert.AreEqual(0.0, result, 0.0001, "Completely different strings should have a coefficient of 0");
        }

        [TestMethod]
        public void DiceCoefficient_PartialMatch_ReturnsBetweenZeroAndOne()
        {
            // Act
            var result = DiceCoefficient.diceCoefficient("night", "nacht");

            // Assert
            Assert.IsTrue(result > 0.0 && result < 1.0,
                $"Partially similar strings should return a value in (0, 1) but got {result}");
        }

        [TestMethod]
        public void DiceCoefficient_IsSymmetric()
        {
            // Arrange
            const string a = "Triceratops";
            const string b = "Triceraptor";

            // Act
            var ab = DiceCoefficient.diceCoefficient(a, b);
            var ba = DiceCoefficient.diceCoefficient(b, a);

            // Assert
            Assert.AreEqual(ab, ba, 0.0001, "diceCoefficient(a,b) should equal diceCoefficient(b,a)");
        }

        [TestMethod]
        public void DiceCoefficient_SingleCharacterStrings_ReturnsExpectedResult()
        {
            // Even single chars produce sentinels, so the coefficient is well-defined.
            var same = DiceCoefficient.diceCoefficient("x", "x");
            var diff = DiceCoefficient.diceCoefficient("x", "y");

            Assert.AreEqual(1.0, same, 0.0001, "Same single-char strings should have coefficient 1");
            Assert.AreEqual(0.0, diff, 0.0001, "Completely different single-char strings should have coefficient 0");
        }
    }
}
