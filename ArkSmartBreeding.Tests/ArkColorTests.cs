using ARKBreedingStats.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for the ArkColor class.
    /// </summary>
    [TestClass]
    public class ArkColorTests
    {
        [TestMethod]
        public void DefaultConstructor_SetsDefaultValues()
        {
            var color = new ArkColor();
            Assert.AreEqual(0, color.Id);
            Assert.AreEqual("No Color", color.Name);
            Assert.AreEqual(System.Drawing.Color.LightGray, color.Color);
            Assert.IsFalse(color.IsDye);
        }

        [TestMethod]
        public void ParameterizedConstructor_SetsValuesFromLinearRgba()
        {
            // Use white linear RGBA [1.0, 1.0, 1.0, 1.0]
            var color = new ArkColor("White", [1.0, 1.0, 1.0, 1.0], false);
            Assert.AreEqual("White", color.Name);
            Assert.IsFalse(color.IsDye);
            // Linear 1.0 → sRGB ≈ 255.999 * pow(1.0, 1/2.2) = ~255
            Assert.AreEqual(255, color.Color.R, $"Red should be 255 but was {color.Color.R}");
            Assert.AreEqual(255, color.Color.G, $"Green should be 255 but was {color.Color.G}");
            Assert.AreEqual(255, color.Color.B, $"Blue should be 255 but was {color.Color.B}");
        }

        [TestMethod]
        public void ParameterizedConstructor_DarkColor_HasLowRgb()
        {
            // Very dim linear color
            var color = new ArkColor("Dark", [0.01, 0.01, 0.01, 1.0], false);
            Assert.IsLessThan(50, color.Color.R, $"Expected dark R but got {color.Color.R}");
            Assert.IsLessThan(50, color.Color.G, $"Expected dark G but got {color.Color.G}");
        }

        [TestMethod]
        public void ParameterizedConstructor_IsDye_IsTrue()
        {
            var color = new ArkColor("Pink Dye", [0.8, 0.2, 0.5, 1.0], true);
            Assert.IsTrue(color.IsDye);
        }

        [TestMethod]
        public void ToString_ContainsName()
        {
            var color = new ArkColor("Teal", [0.0, 0.5, 0.5, 1.0], false);
            Assert.Contains("Teal", color.ToString(), $"ToString should contain name but was: {color}");
        }
    }
}
