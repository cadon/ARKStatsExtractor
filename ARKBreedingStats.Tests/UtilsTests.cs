using System.Drawing;
using ARKBreedingStats.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ARKBreedingStats.Tests
{
    /// <summary>
    /// Smoke tests for the Utils class
    /// </summary>
    [TestClass]
    public class UtilsTests
    {
        [TestMethod]
        public void GetColorFromPercent_AtZeroPercent_ReturnsRed()
        {
            // Arrange & Act
            var color = Utils.GetColorFromPercent(0);

            // Assert
            Assert.AreEqual(255, color.R, "Red component should be 255 at 0%");
            Assert.AreEqual(0, color.G, "Green component should be 0 at 0%");
            Assert.AreEqual(0, color.B, "Blue component should be 0 at 0%");
        }

        [TestMethod]
        public void GetColorFromPercent_AtFiftyPercent_ReturnsYellow()
        {
            // Arrange & Act
            var color = Utils.GetColorFromPercent(50);

            // Assert
            // g = (int)(50 * 5.1) = 254 due to floating-point truncation
            Assert.AreEqual(255, color.R, "Red component should be 255 at 50%");
            Assert.AreEqual(254, color.G, "Green component should be 254 at 50% (float truncation of 50*5.1)");
            Assert.AreEqual(0, color.B, "Blue component should be 0 at 50%");
        }

        [TestMethod]
        public void GetColorFromPercent_AtOneHundredPercent_ReturnsGreen()
        {
            // Arrange & Act
            var color = Utils.GetColorFromPercent(100);

            // Assert
            // g = (int)(100 * 5.1) = 509 due to floating-point truncation; r = 511 - 509 = 2
            Assert.AreEqual(2, color.R, "Red component should be 2 at 100% (float truncation of 100*5.1)");
            Assert.AreEqual(255, color.G, "Green component should be 255 at 100% (capped)");
            Assert.AreEqual(0, color.B, "Blue component should be 0 at 100%");
        }

        [TestMethod]
        public void GetColorFromPercent_WithLightDelta_AdjustsBrightness()
        {
            // Arrange & Act
            var normalColor = Utils.GetColorFromPercent(50);
            var brighterColor = Utils.GetColorFromPercent(50, 0.5);
            var darkerColor = Utils.GetColorFromPercent(50, -0.5);

            // Assert
            Assert.IsTrue(brighterColor.R >= normalColor.R, "Brighter color should have higher or equal red component");
            Assert.IsTrue(brighterColor.G >= normalColor.G, "Brighter color should have higher or equal green component");
            Assert.IsTrue(darkerColor.R <= normalColor.R, "Darker color should have lower or equal red component");
            Assert.IsTrue(darkerColor.G <= normalColor.G, "Darker color should have lower or equal green component");
        }

        [TestMethod]
        public void AdjustColorLight_WithZeroDelta_ReturnsOriginalColor()
        {
            // Arrange
            var color = Color.FromArgb(128, 128, 128);

            // Act
            var adjustedColor = Utils.AdjustColorLight(color, 0);

            // Assert
            Assert.AreEqual(color, adjustedColor, "Color should remain unchanged with zero delta");
        }

        [TestMethod]
        public void AdjustColorLight_WithPositiveDelta_MakesColorBrighter()
        {
            // Arrange
            var color = Color.FromArgb(100, 100, 100);

            // Act
            var brighterColor = Utils.AdjustColorLight(color, 0.5);

            // Assert
            Assert.IsTrue(brighterColor.R > color.R, "Red component should be brighter");
            Assert.IsTrue(brighterColor.G > color.G, "Green component should be brighter");
            Assert.IsTrue(brighterColor.B > color.B, "Blue component should be brighter");
        }

        [TestMethod]
        public void AdjustColorLight_WithNegativeDelta_MakesColorDarker()
        {
            // Arrange
            var color = Color.FromArgb(200, 200, 200);

            // Act
            var darkerColor = Utils.AdjustColorLight(color, -0.5);

            // Assert
            Assert.IsTrue(darkerColor.R < color.R, "Red component should be darker");
            Assert.IsTrue(darkerColor.G < color.G, "Green component should be darker");
            Assert.IsTrue(darkerColor.B < color.B, "Blue component should be darker");
        }

        [TestMethod]
        public void GetARKml_CreatesProperlyFormattedString()
        {
            // Arrange
            string text = "TestText";
            int r = 255, g = 128, b = 0;

            // Act
            var result = Utils.GetARKml(text, r, g, b);

            // Assert
            Assert.IsTrue(result.Contains("TestText"), "Result should contain the input text");
            Assert.IsTrue(result.Contains("<RichColor"), "Result should contain RichColor tag");
            Assert.IsTrue(result.Contains("Color="), "Result should contain Color attribute");
        }

        [TestMethod]
        public void GetARKmlFromPercent_CreatesValidString()
        {
            // Arrange & Act
            var result = Utils.GetARKmlFromPercent("Test", 75);

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.IsTrue(result.Contains("Test"), "Result should contain the input text");
            Assert.IsTrue(result.Length > 4, "Result should be longer than just the input text");
        }

        // ── SexSymbol ──────────────────────────────────────────────────────────

        [TestMethod]
        public void SexSymbol_Male_ReturnsMaleSymbol()
        {
            Assert.AreEqual("♂", Utils.SexSymbol(Sex.Male));
        }

        [TestMethod]
        public void SexSymbol_Female_ReturnsFemaleSymbol()
        {
            Assert.AreEqual("♀", Utils.SexSymbol(Sex.Female));
        }

        [TestMethod]
        public void SexSymbol_Unknown_ReturnsQuestionMark()
        {
            Assert.AreEqual("?", Utils.SexSymbol(Sex.Unknown));
        }

        // ── NextSex ────────────────────────────────────────────────────────────

        [TestMethod]
        public void NextSex_Female_ReturnsMale()
        {
            Assert.AreEqual(Sex.Male, Utils.NextSex(Sex.Female));
        }

        [TestMethod]
        public void NextSex_MaleIncludingUnknown_ReturnsUnknown()
        {
            Assert.AreEqual(Sex.Unknown, Utils.NextSex(Sex.Male, includingUnknown: true));
        }

        [TestMethod]
        public void NextSex_MaleExcludingUnknown_ReturnsFemale()
        {
            Assert.AreEqual(Sex.Female, Utils.NextSex(Sex.Male, includingUnknown: false));
        }

        [TestMethod]
        public void NextSex_Unknown_ReturnsFemale()
        {
            Assert.AreEqual(Sex.Female, Utils.NextSex(Sex.Unknown));
        }

        // ── StatusSymbol ───────────────────────────────────────────────────────

        [TestMethod]
        public void StatusSymbol_Dead_ReturnsCross()
        {
            Assert.AreEqual("†", Utils.StatusSymbol(CreatureStatus.Dead));
        }

        [TestMethod]
        public void StatusSymbol_Unavailable_ReturnsBallotX()
        {
            Assert.AreEqual("✗", Utils.StatusSymbol(CreatureStatus.Unavailable));
        }

        [TestMethod]
        public void StatusSymbol_Obelisk_ReturnsHouseSymbol()
        {
            Assert.AreEqual("⌂", Utils.StatusSymbol(CreatureStatus.Obelisk));
        }

        [TestMethod]
        public void StatusSymbol_Available_ReturnsDefaultCheckmark()
        {
            // Available falls through to the default branch
            Assert.AreEqual("✓", Utils.StatusSymbol(CreatureStatus.Available));
        }

        // ── NextStatus ─────────────────────────────────────────────────────────

        [TestMethod]
        public void NextStatus_Available_ReturnsUnavailable()
        {
            Assert.AreEqual(CreatureStatus.Unavailable, Utils.NextStatus(CreatureStatus.Available));
        }

        [TestMethod]
        public void NextStatus_Unavailable_ReturnsDead()
        {
            Assert.AreEqual(CreatureStatus.Dead, Utils.NextStatus(CreatureStatus.Unavailable));
        }

        [TestMethod]
        public void NextStatus_Dead_ReturnsObelisk()
        {
            Assert.AreEqual(CreatureStatus.Obelisk, Utils.NextStatus(CreatureStatus.Dead));
        }

        [TestMethod]
        public void NextStatus_Obelisk_ReturnsCryopod()
        {
            Assert.AreEqual(CreatureStatus.Cryopod, Utils.NextStatus(CreatureStatus.Obelisk));
        }

        [TestMethod]
        public void NextStatus_Cryopod_WrapsToAvailable()
        {
            Assert.AreEqual(CreatureStatus.Available, Utils.NextStatus(CreatureStatus.Cryopod));
        }

        // ── ForeColor ──────────────────────────────────────────────────────────

        [TestMethod]
        public void ForeColor_DarkBackground_ReturnsWhite()
        {
            var foreColor = Utils.ForeColor(System.Drawing.Color.Black);
            Assert.AreEqual(System.Drawing.Color.White, foreColor);
        }

        [TestMethod]
        public void ForeColor_LightBackground_ReturnsBlack()
        {
            var foreColor = Utils.ForeColor(System.Drawing.Color.White);
            Assert.AreEqual(System.Drawing.Color.Black, foreColor);
        }

        // ── Duration ──────────────────────────────────────────────────────────

        [TestMethod]
        public void Duration_ZeroTimeSpan_ReturnsAllZeroes()
        {
            var result = Utils.Duration(System.TimeSpan.Zero);
            Assert.AreEqual("00:00:00:00", result);
        }

        [TestMethod]
        public void Duration_OneDayTwoHoursThreeMinutesFourSeconds_FormatsCorrectly()
        {
            var ts = new System.TimeSpan(days: 1, hours: 2, minutes: 3, seconds: 4);
            var result = Utils.Duration(ts);
            Assert.AreEqual("01:02:03:04", result);
        }

        [TestMethod]
        public void Duration_FromSeconds_MatchesTimeSpanOverload()
        {
            const int seconds = 3661; // 1 h 1 min 1 sec
            var fromInt = Utils.Duration(seconds);
            var fromTimeSpan = Utils.Duration(new System.TimeSpan(0, 0, seconds));
            Assert.AreEqual(fromTimeSpan, fromInt);
        }

        // ── ColorFromHsv ──────────────────────────────────────────────────────

        [TestMethod]
        public void ColorFromHsv_ZeroSaturation_ReturnsGrayShade()
        {
            // With saturation 0, R == G == B (grey)
            var color = Utils.ColorFromHsv(0, saturation: 0, value: 0.5);
            Assert.AreEqual(color.R, color.G, "Grey should have equal R and G");
            Assert.AreEqual(color.G, color.B, "Grey should have equal G and B");
        }

        [TestMethod]
        public void ColorFromHsv_Hue0_FullSaturation_ReturnsRed()
        {
            var color = Utils.ColorFromHsv(0, saturation: 1, value: 1);
            Assert.AreEqual(255, color.R, "Hue 0 should be red (R=255)");
            Assert.AreEqual(0, color.B, "Hue 0 red should have B=0");
        }
    }
}
