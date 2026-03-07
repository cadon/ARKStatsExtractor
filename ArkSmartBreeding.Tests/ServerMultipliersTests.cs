using ARKBreedingStats.Settings;
using ARKBreedingStats.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for the ServerMultipliers class.
    /// </summary>
    [TestClass]
    public class ServerMultipliersTests
    {
        [TestMethod]
        public void DefaultConstructor_AllMultipliersAreOne()
        {
            var sm = new ServerMultipliers();
            Assert.AreEqual(1.0, sm.TamingSpeedMultiplier);
            Assert.AreEqual(1.0, sm.WildDinoTorporDrainMultiplier);
            Assert.AreEqual(1.0, sm.DinoCharacterFoodDrainMultiplier);
            Assert.AreEqual(1.0, sm.TamedDinoCharacterFoodDrainMultiplier);
            Assert.AreEqual(1.0, sm.WildDinoCharacterFoodDrainMultiplier);
            Assert.AreEqual(1.0, sm.MatingSpeedMultiplier);
            Assert.AreEqual(1.0, sm.MatingIntervalMultiplier);
            Assert.AreEqual(1.0, sm.EggHatchSpeedMultiplier);
            Assert.AreEqual(1.0, sm.BabyMatureSpeedMultiplier);
            Assert.AreEqual(1.0, sm.BabyFoodConsumptionSpeedMultiplier);
            Assert.AreEqual(1.0, sm.BabyCuddleIntervalMultiplier);
            Assert.AreEqual(1.0, sm.BabyImprintingStatScaleMultiplier);
            Assert.AreEqual(1.0, sm.BabyImprintAmountMultiplier);
        }

        [TestMethod]
        public void DefaultConstructor_BoolsAreFalse()
        {
            var sm = new ServerMultipliers();
            Assert.IsFalse(sm.AllowSpeedLeveling);
            Assert.IsFalse(sm.AllowFlyerSpeedLeveling);
            Assert.IsFalse(sm.SinglePlayerSettings);
            Assert.IsFalse(sm.AtlasSettings);
        }

        [TestMethod]
        public void DefaultConstructor_StatMultipliersIsNull()
        {
            var sm = new ServerMultipliers();
            Assert.IsNull(sm.statMultipliers);
        }

        [TestMethod]
        public void ConstructorWithStatMultipliers_CreatesArrays()
        {
            var sm = new ServerMultipliers(withStatMultipliersObject: true);
            Assert.IsNotNull(sm.statMultipliers);
            Assert.HasCount(Stats.StatsCount, sm.statMultipliers);
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                Assert.IsNotNull(sm.statMultipliers[s]);
                Assert.HasCount(4, sm.statMultipliers[s]);
            }
        }

        [TestMethod]
        public void Copy_WithoutStatMultipliers_CopiesScalars()
        {
            var original = new ServerMultipliers
            {
                TamingSpeedMultiplier = 3.0,
                BabyMatureSpeedMultiplier = 5.0,
                AllowSpeedLeveling = true
            };
            var copy = original.Copy(withStatMultipliers: false);
            Assert.AreEqual(3.0, copy.TamingSpeedMultiplier);
            Assert.AreEqual(5.0, copy.BabyMatureSpeedMultiplier);
            Assert.IsTrue(copy.AllowSpeedLeveling);
            Assert.IsNull(copy.statMultipliers);
        }

        [TestMethod]
        public void Copy_WithStatMultipliers_CopiesArrayValues()
        {
            var original = new ServerMultipliers(withStatMultipliersObject: true);
            original.statMultipliers[0][0] = 2.5;
            original.statMultipliers[7][3] = 3.0;

            var copy = original.Copy(withStatMultipliers: true);
            Assert.AreEqual(2.5, copy.statMultipliers[0][0]);
            Assert.AreEqual(3.0, copy.statMultipliers[7][3]);

            // Ensure it's a deep copy
            copy.statMultipliers[0][0] = 99.0;
            Assert.AreEqual(2.5, original.statMultipliers[0][0], "Original should not be affected");
        }

        [TestMethod]
        public void FixZeroValues_SetsZerosToOne()
        {
            var sm = new ServerMultipliers
            {
                TamingSpeedMultiplier = 0,
                WildDinoTorporDrainMultiplier = 0,
                MatingIntervalMultiplier = 0,
                EggHatchSpeedMultiplier = 0,
                MatingSpeedMultiplier = 0,
                BabyMatureSpeedMultiplier = 0,
                BabyCuddleIntervalMultiplier = 0,
                BabyImprintAmountMultiplier = 0
            };

            sm.FixZeroValues();

            Assert.AreEqual(1.0, sm.TamingSpeedMultiplier);
            Assert.AreEqual(1.0, sm.WildDinoTorporDrainMultiplier);
            Assert.AreEqual(1.0, sm.MatingIntervalMultiplier);
            Assert.AreEqual(1.0, sm.EggHatchSpeedMultiplier);
            Assert.AreEqual(1.0, sm.MatingSpeedMultiplier);
            Assert.AreEqual(1.0, sm.BabyMatureSpeedMultiplier);
            Assert.AreEqual(1.0, sm.BabyCuddleIntervalMultiplier);
            Assert.AreEqual(1.0, sm.BabyImprintAmountMultiplier);
        }

        [TestMethod]
        public void FixZeroValues_LeavesNonZeroAlone()
        {
            var sm = new ServerMultipliers { TamingSpeedMultiplier = 2.5 };
            sm.FixZeroValues();
            Assert.AreEqual(2.5, sm.TamingSpeedMultiplier);
        }

        [TestMethod]
        public void PropertyChanged_FiresOnChange()
        {
            var sm = new ServerMultipliers();
            string changedProp = null;
            sm.PropertyChanged += (sender, e) => changedProp = e.PropertyName;

            sm.TamingSpeedMultiplier = 5.0;
            Assert.AreEqual(nameof(ServerMultipliers.TamingSpeedMultiplier), changedProp);
        }

        [TestMethod]
        public void PropertyChanged_DoesNotFireOnSameValue()
        {
            var sm = new ServerMultipliers();
            bool fired = false;
            sm.PropertyChanged += (sender, e) => fired = true;

            sm.TamingSpeedMultiplier = 1.0; // same as default
            Assert.IsFalse(fired, "PropertyChanged should not fire when value is unchanged");
        }
    }
}
