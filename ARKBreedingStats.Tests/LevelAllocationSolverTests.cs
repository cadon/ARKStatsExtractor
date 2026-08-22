using ARKBreedingStats.leveling;
using ARKBreedingStats.species;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace ARKBreedingStats.Tests
{
    [TestClass]
    public class LevelAllocationSolverTests
    {
        [TestMethod]
        public void ServerLevelCapIncrease_UsesTemporaryVariantNameHeuristic()
        {
            Assert.AreEqual(0, new Species { name = "Rex" }.ServerLevelCapIncrease);
            Assert.AreEqual(50, new Species { name = "X-Rex" }.ServerLevelCapIncrease);
            Assert.AreEqual(50, new Species { name = "R-Rex" }.ServerLevelCapIncrease);
        }

        [TestMethod]
        public void Solve_MaximizeSpeed_RespectsServerSpeedLevelingSetting()
        {
            var disabled = CreateRexRequest(20);
            disabled.Species.stats[Stats.SpeedMultiplier] = CreateStat(1, 0.01, 0.01, 0.01, 0, 0);
            disabled.Species.ApplyCanLevelOptions(false, false);
            disabled.StatTargets[Stats.SpeedMultiplier].Mode = LevelTargetMode.Maximize;

            var disabledResult = LevelAllocationSolver.Solve(disabled);

            Assert.IsTrue(disabledResult.Feasible, disabledResult.Message);
            Assert.AreEqual(0, disabledResult.MutationLevels[Stats.SpeedMultiplier]);
            Assert.AreEqual(0, disabledResult.DomesticLevels[Stats.SpeedMultiplier]);

            var enabled = CreateRexRequest(20);
            enabled.Species.stats[Stats.SpeedMultiplier] = CreateStat(1, 0.01, 0.01, 0.01, 0, 0);
            enabled.StatTargets[Stats.SpeedMultiplier].Mode = LevelTargetMode.Maximize;

            var enabledResult = LevelAllocationSolver.Solve(enabled);

            Assert.IsTrue(enabledResult.Feasible, enabledResult.Message);
            Assert.IsTrue(enabledResult.MutationLevels[Stats.SpeedMultiplier] > 0
                || enabledResult.DomesticLevels[Stats.SpeedMultiplier] > 0);
        }

        [TestMethod]
        public void Solve_RexHealthTargetAndMaxMelee_UsesOptimalAllocation()
        {
            var request = CreateRexRequest(450);
            request.WildLevels[Stats.Health] = 84;
            request.WildLevels[Stats.MeleeDamageMultiplier] = 255;
            request.StatTargets[Stats.Health] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Target,
                TargetValue = 50000
            };
            request.StatTargets[Stats.MeleeDamageMultiplier] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Maximize
            };

            var result = LevelAllocationSolver.Solve(request);

            Assert.IsTrue(result.Feasible, result.Message);
            Assert.AreEqual(84, result.WildLevels[Stats.Health]);
            Assert.AreEqual(21, result.DomesticLevels[Stats.Health]);
            Assert.AreEqual(255, result.WildLevels[Stats.MeleeDamageMultiplier]);
            Assert.AreEqual(22, result.MutationLevels[Stats.MeleeDamageMultiplier]);
            Assert.AreEqual(67, result.DomesticLevels[Stats.MeleeDamageMultiplier]);
            Assert.AreEqual(450, result.TotalLevel);
            Assert.AreEqual(50140.6, result.Values[Stats.Health], 0.1);
            Assert.AreEqual(45.002, result.Values[Stats.MeleeDamageMultiplier], 0.001);
        }

        [TestMethod]
        public void Solve_RexWeightTarget_ReservesLevelsBeforeMaximizingMelee()
        {
            var request = CreateRexRequest(450);
            request.WildLevels[Stats.Health] = 84;
            request.WildLevels[Stats.Weight] = 22;
            request.WildLevels[Stats.MeleeDamageMultiplier] = 255;
            request.StatTargets[Stats.Health] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Target,
                TargetValue = 50000
            };
            request.StatTargets[Stats.Weight] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Target,
                TargetValue = 1000
            };
            request.StatTargets[Stats.MeleeDamageMultiplier] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Maximize
            };

            var result = LevelAllocationSolver.Solve(request);

            Assert.IsTrue(result.Feasible, result.Message);
            Assert.AreEqual(22, result.WildLevels[Stats.Weight]);
            Assert.AreEqual(4, result.DomesticLevels[Stats.Weight]);
            Assert.AreEqual(1002.2, result.Values[Stats.Weight], 0.1);
            Assert.AreEqual(450, result.TotalLevel);
        }

        [TestMethod]
        public void Solve_ImprintingReducesLevelsNeededForTarget()
        {
            var imprinted = CreateRexRequest(450);
            imprinted.StatTargets[Stats.Health] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Target,
                TargetValue = 50000
            };
            var unimprinted = CreateRexRequest(450);
            unimprinted.ImprintingBonus = 0;
            unimprinted.StatTargets[Stats.Health] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Target,
                TargetValue = 50000
            };

            var imprintedResult = LevelAllocationSolver.Solve(imprinted);
            var unimprintedResult = LevelAllocationSolver.Solve(unimprinted);

            Assert.IsTrue(imprintedResult.Feasible);
            Assert.IsTrue(unimprintedResult.Feasible);
            Assert.IsTrue(imprintedResult.TotalLevel < unimprintedResult.TotalLevel);
        }

        [TestMethod]
        public void Solve_UnreachableTarget_ReturnsInfeasibleResult()
        {
            var request = CreateRexRequest(100);
            request.StatTargets[Stats.Health] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Target,
                TargetValue = 1000000
            };

            var result = LevelAllocationSolver.Solve(request);

            Assert.IsFalse(result.Feasible);
            Assert.IsNotNull(result.Message);
        }

        [TestMethod]
        public void Solve_StrongerMutationIncrease_AllocatesMutationLevelsFirst()
        {
            var request = CreateRexRequest(11);
            request.MaxDomesticLevels = 0;
            request.Species.stats[Stats.Health].IncPerMutatedLevel = 0.4;
            request.StatTargets[Stats.Health] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Maximize
            };

            var result = LevelAllocationSolver.Solve(request);

            Assert.IsTrue(result.Feasible, result.Message);
            Assert.AreEqual(0, result.WildLevels[Stats.Health]);
            Assert.AreEqual(10, result.MutationLevels[Stats.Health]);
        }

        [TestMethod]
        public void Solve_TargetThatNeedsOneMutationLevel_RoundsUpToTwo()
        {
            var request = CreateRexRequest(450);
            request.MaxDomesticLevels = 0;
            request.StatTargets[Stats.Health] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Target,
                TargetValue = 1400
            };

            var result = LevelAllocationSolver.Solve(request);

            Assert.IsTrue(result.Feasible, result.Message);
            Assert.AreEqual(2, result.MutationLevels[Stats.Health]);
        }

        [TestMethod]
        public void Solve_OddMutationCap_RoundsDownToReachableEvenLevel()
        {
            var request = CreateRexRequest(500);
            request.MaxDomesticLevels = 0;
            request.MaxMutationLevelPerStat = 255;
            request.StatTargets[Stats.MeleeDamageMultiplier] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Maximize
            };

            var result = LevelAllocationSolver.Solve(request);

            Assert.IsTrue(result.Feasible, result.Message);
            Assert.AreEqual(254, result.MutationLevels[Stats.MeleeDamageMultiplier]);
        }

        [TestMethod]
        public void Solve_FixedWildLevels_PreservesPlayerInput()
        {
            var request = CreateRexRequest(450);
            request.WildLevels[Stats.MeleeDamageMultiplier] = 37;
            request.StatTargets[Stats.MeleeDamageMultiplier] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Maximize
            };

            var result = LevelAllocationSolver.Solve(request);

            Assert.IsTrue(result.Feasible, result.Message);
            Assert.AreEqual(37, result.WildLevels[Stats.MeleeDamageMultiplier]);
        }

        [TestMethod]
        public void Solve_MultipleMaximizeStats_HonorsPriority()
        {
            var request = CreateRexRequest(11);
            request.MaxDomesticLevels = 0;
            request.StatTargets[Stats.Health] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Maximize,
                Priority = 1
            };
            request.StatTargets[Stats.MeleeDamageMultiplier] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Maximize,
                Priority = 2
            };

            var result = LevelAllocationSolver.Solve(request);

            Assert.IsTrue(result.Feasible, result.Message);
            Assert.AreEqual(10, result.MutationLevels[Stats.Health]);
            Assert.AreEqual(0, result.WildLevels[Stats.MeleeDamageMultiplier]);
        }

        [TestMethod]
        public void Solve_ZeroTarget_DoesNotAllocateToStat()
        {
            var request = CreateRexRequest(50);
            request.WildLevels[Stats.Health] = 12;
            request.StatTargets[Stats.Health] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Target,
                TargetValue = 0
            };
            request.StatTargets[Stats.MeleeDamageMultiplier] = new LevelStatTarget
            {
                Mode = LevelTargetMode.Maximize
            };

            var result = LevelAllocationSolver.Solve(request);

            Assert.IsTrue(result.Feasible, result.Message);
            Assert.AreEqual(12, result.WildLevels[Stats.Health]);
            Assert.AreEqual(0, result.MutationLevels[Stats.Health]);
            Assert.AreEqual(0, result.DomesticLevels[Stats.Health]);
        }

        [TestMethod]
        public void Solve_FixedWildLevels_CountTowardTotalLevel()
        {
            var request = CreateRexRequest(450);
            request.WildLevels[Stats.Health] = 12;

            var result = LevelAllocationSolver.Solve(request);

            Assert.IsTrue(result.Feasible, result.Message);
            Assert.AreEqual(13, result.TotalLevel);
        }

        [TestMethod]
        public void Solve_CancelledSearch_ThrowsOperationCancelled()
        {
            var request = CreateRexRequest(450);
            request.StatTargets[Stats.Health].Mode = LevelTargetMode.Maximize;
            request.StatTargets[Stats.Weight].Mode = LevelTargetMode.Maximize;
            request.StatTargets[Stats.MeleeDamageMultiplier].Mode = LevelTargetMode.Maximize;
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            Assert.ThrowsException<OperationCanceledException>(
                () => LevelAllocationSolver.Solve(request, cancellation.Token));
        }

        private static LevelSolverRequest CreateRexRequest(int levelCap)
        {
            var stats = new SpeciesStat[Stats.StatsCount];
            stats[Stats.Health] = CreateStat(1100, 0.2, 0.2, 0.054, 0.07, 0);
            stats[Stats.Weight] = CreateStat(500, 0.02, 0.02, 0.04, 0, 0);
            stats[Stats.MeleeDamageMultiplier] = CreateStat(1, 0.05, 0.05, 0.017, 0.07, 0.176);
            var species = new Species
            {
                name = "Rex",
                stats = stats,
                TamedBaseHealthMultiplier = 1,
                StatImprintMultipliers = new[] { 0.2, 0, 0.2, 0, 0.2, 0.2, 0, 0.2, 0.2, 0.2, 0, 0 }
            };

            return new LevelSolverRequest
            {
                Species = species,
                LevelCap = levelCap,
                ImprintingBonus = 1
            };
        }

        private static SpeciesStat CreateStat(double baseValue, double wildIncrease, double mutationIncrease,
            double domesticIncrease, double additiveBonus, double multiplicativeBonus)
            => new SpeciesStat
            {
                BaseValue = baseValue,
                IncPerWildLevel = wildIncrease,
                IncPerMutatedLevel = mutationIncrease,
                IncPerTamedLevel = domesticIncrease,
                AddWhenTamed = additiveBonus,
                MultAffinity = multiplicativeBonus,
                IncreaseStatAsPercentage = true,
                ValueCap = double.MaxValue
            };
    }
}