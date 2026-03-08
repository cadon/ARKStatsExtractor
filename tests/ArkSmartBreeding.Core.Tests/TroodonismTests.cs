using ARKBreedingStats.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for the Troodonism static class.
    /// </summary>
    [TestClass]
    public class TroodonismTests
    {
        private SpeciesStat MakeNormalStat() => new SpeciesStat
        {
            BaseValue = 100,
            IncPerWildLevel = 10,
            IncPerMutatedLevel = 10,
            AddWhenTamed = 5,
            MultAffinity = 1,
            IncPerTamedLevel = 2
        };

        private SpeciesStat MakeAltStat() => new SpeciesStat
        {
            BaseValue = 200,
            IncPerWildLevel = 20,
            IncPerMutatedLevel = 20,
            AddWhenTamed = 50,
            MultAffinity = 2,
            IncPerTamedLevel = 4
        };

        [TestMethod]
        public void SelectStats_None_ReturnsNormalValues()
        {
            var normal = MakeNormalStat();
            var alt = MakeAltStat();
            var result = Troodonism.SelectStats(normal, alt, Troodonism.AffectedStats.None);
            Assert.AreEqual(normal.BaseValue, result.BaseValue, "Base should come from normal");
            Assert.AreEqual(normal.IncPerWildLevel, result.IncPerWildLevel, "IncPerWildLevel should come from normal");
        }

        [TestMethod]
        public void SelectStats_Base_UsesAltBaseOnly()
        {
            var normal = MakeNormalStat();
            var alt = MakeAltStat();
            var result = Troodonism.SelectStats(normal, alt, Troodonism.AffectedStats.Base);
            Assert.AreEqual(alt.BaseValue, result.BaseValue, "Base should come from alt");
            Assert.AreEqual(normal.IncPerWildLevel, result.IncPerWildLevel, "IncPerWildLevel should come from normal");
            Assert.AreEqual(normal.AddWhenTamed, result.AddWhenTamed, "AddWhenTamed should come from normal");
        }

        [TestMethod]
        public void SelectStats_IncreaseWild_UsesAltWildOnly()
        {
            var normal = MakeNormalStat();
            var alt = MakeAltStat();
            var result = Troodonism.SelectStats(normal, alt, Troodonism.AffectedStats.IncreaseWild);
            Assert.AreEqual(normal.BaseValue, result.BaseValue, "Base should come from normal");
            Assert.AreEqual(alt.IncPerWildLevel, result.IncPerWildLevel, "IncPerWildLevel should come from alt");
            Assert.AreEqual(alt.IncPerMutatedLevel, result.IncPerMutatedLevel, "IncPerMutatedLevel should come from alt");
        }

        [TestMethod]
        public void SelectStats_UncryoCombination_UsesAltBaseAndWild()
        {
            var normal = MakeNormalStat();
            var alt = MakeAltStat();
            var result = Troodonism.SelectStats(normal, alt, Troodonism.AffectedStats.UncryoCombination);
            Assert.AreEqual(alt.BaseValue, result.BaseValue, "Base should come from alt");
            Assert.AreEqual(alt.IncPerWildLevel, result.IncPerWildLevel, "IncPerWildLevel should come from alt");
            // Non-affected fields still from normal
            Assert.AreEqual(normal.AddWhenTamed, result.AddWhenTamed);
            Assert.AreEqual(normal.MultAffinity, result.MultAffinity);
            Assert.AreEqual(normal.IncPerTamedLevel, result.IncPerTamedLevel);
        }

        [TestMethod]
        public void SelectStats_NullAltStats_ReturnsNormal()
        {
            var normal = MakeNormalStat();
            var result = Troodonism.SelectStats(normal, null, Troodonism.AffectedStats.Base);
            Assert.AreEqual(normal.BaseValue, result.BaseValue);
        }

        [TestMethod]
        public void SelectStats_Array_ReturnsArrayOfCorrectLength()
        {
            var normals = new SpeciesStat[Stats.StatsCount];
            var alts = new SpeciesStat[Stats.StatsCount];
            for (int i = 0; i < Stats.StatsCount; i++)
            {
                normals[i] = MakeNormalStat();
                alts[i] = MakeAltStat();
            }

            var result = Troodonism.SelectStats(normals, alts, Troodonism.AffectedStats.Base);
            Assert.HasCount(Stats.StatsCount, result);
            Assert.AreEqual(alts[0].BaseValue, result[0].BaseValue);
            Assert.AreEqual(normals[0].IncPerWildLevel, result[0].IncPerWildLevel);
        }

        [TestMethod]
        public void SelectStats_Array_NullAlt_ReturnsNormals()
        {
            var normals = new SpeciesStat[Stats.StatsCount];
            for (int i = 0; i < Stats.StatsCount; i++)
            {
                normals[i] = MakeNormalStat();
            }

            var result = Troodonism.SelectStats(normals, null, Troodonism.AffectedStats.Base);
            Assert.AreSame(normals, result);
        }
    }
}
