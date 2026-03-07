using ARKBreedingStats;
using ARKBreedingStats.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for the Ark static constants and helper methods.
    /// </summary>
    [TestClass]
    public class ArkConstantsTests
    {
        [TestMethod]
        public void BreedingProbabilities_SumToOne()
        {
            double sum = Ark.ProbabilityInheritHigherLevel + Ark.ProbabilityInheritLowerLevel;
            Assert.AreEqual(1.0, sum, 0.0001, "Higher + Lower level probabilities should sum to 1");
        }

        [TestMethod]
        public void ProbabilityOfOneMutation_IsCorrect()
        {
            double formulaResult = 1 - System.Math.Pow(1 - Ark.ProbabilityOfMutation, Ark.MutationRolls);
            Assert.AreEqual(Ark.ProbabilityOfOneMutation, formulaResult, 0.0001);
        }

        [TestMethod]
        public void StatIndicesAffectedByMutagen_ContainsFourStats()
        {
            Assert.HasCount(4, Ark.StatIndicesAffectedByMutagen);
            Assert.Contains(Stats.Health, Ark.StatIndicesAffectedByMutagen);
            Assert.Contains(Stats.Stamina, Ark.StatIndicesAffectedByMutagen);
            Assert.Contains(Stats.Weight, Ark.StatIndicesAffectedByMutagen);
            Assert.Contains(Stats.MeleeDamageMultiplier, Ark.StatIndicesAffectedByMutagen);
        }

        [TestMethod]
        public void SetUndefinedColorId_Ase_Sets227()
        {
            Ark.SetUndefinedColorId(false);
            Assert.AreEqual(Ark.UndefinedColorIdAse, Ark.UndefinedColorId);
        }

        [TestMethod]
        public void SetUndefinedColorId_Asa_Sets255()
        {
            Ark.SetUndefinedColorId(true);
            Assert.AreEqual(Ark.UndefinedColorIdAsa, Ark.UndefinedColorId);
            // Reset
            Ark.SetUndefinedColorId(false);
        }

        [TestMethod]
        public void ProbabilityOfOneMutationWithOffset_ZeroOffset_MatchesConstant()
        {
            double result = Ark.ProbabilityOfOneMutationWithOffset(Ark.ProbabilityOfMutation, 0);
            Assert.AreEqual(Ark.ProbabilityOfOneMutation, result, 0.0001);
        }

        [TestMethod]
        public void ProbabilityOfOneMutationWithOffset_LargerOffset_IncreasedProbability()
        {
            double baseline = Ark.ProbabilityOfOneMutationWithOffset(Ark.ProbabilityOfMutation, 0);
            double boosted = Ark.ProbabilityOfOneMutationWithOffset(Ark.ProbabilityOfMutation, 0.01);
            Assert.IsGreaterThan(baseline, boosted, "Adding offset should increase mutation probability");
        }

        [TestMethod]
        public void ImprintingPerBondedTamingRank_IsLinear()
        {
            Assert.AreEqual(0.0, Ark.ImprintingPerBondedTamingRank(0), 0.0001);
            Assert.AreEqual(0.1, Ark.ImprintingPerBondedTamingRank(1), 0.0001);
            Assert.AreEqual(0.5, Ark.ImprintingPerBondedTamingRank(5), 0.0001);
        }
    }
}
