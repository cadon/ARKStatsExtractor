using ARKBreedingStats.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for the Kibble class.
    /// </summary>
    [TestClass]
    public class KibbleTests
    {
        [TestMethod]
        public void RecipeAsText_EmptyKibble_ReturnsEmpty()
        {
            var kibble = new Kibble();
            Assert.AreEqual("", kibble.RecipeAsText());
        }

        [TestMethod]
        public void RecipeAsText_SingleIngredient_FormatsCorrectly()
        {
            var kibble = new Kibble { { "Mejoberry", 10 } };
            var text = kibble.RecipeAsText();
            Assert.Contains("10", text, "Should contain quantity");
            Assert.Contains("Mejoberry", text, "Should contain ingredient name");
            Assert.Contains("×", text, "Should contain multiply sign");
        }

        [TestMethod]
        public void RecipeAsText_MultipleIngredients_ContainsAll()
        {
            var kibble = new Kibble
            {
                { "Mejoberry", 10 },
                { "Raw Meat", 3 },
                { "Fiber", 25 }
            };
            var text = kibble.RecipeAsText();
            Assert.Contains("Mejoberry", text);
            Assert.Contains("Raw Meat", text);
            Assert.Contains("Fiber", text);
        }

        [TestMethod]
        public void Kibble_IsDictionary()
        {
            var kibble = new Kibble { { "Berry", 5 } };
            Assert.AreEqual(5, kibble["Berry"]);
            Assert.HasCount(1, kibble);
        }
    }
}
