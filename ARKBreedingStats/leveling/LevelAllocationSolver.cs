using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ARKBreedingStats.leveling
{
    public enum LevelTargetMode
    {
        Target,
        Maximize
    }

    public sealed class LevelStatTarget
    {
        public LevelTargetMode Mode { get; set; }
        public double TargetValue { get; set; }
        public int Priority { get; set; }
    }

    public sealed class LevelSolverRequest
    {
        public Species Species { get; set; }
        public int LevelCap { get; set; } = 450;
        public int MaxDomesticLevels { get; set; } = 88;
        public int MaxMutationLevelPerStat { get; set; } = 255;
        public double ImprintingBonus { get; set; } = 1;
        public int[] WildLevels { get; set; } = new int[Stats.StatsCount];
        public LevelStatTarget[] StatTargets { get; set; } = Enumerable.Range(0, Stats.StatsCount)
            .Select(_ => new LevelStatTarget()).ToArray();
    }

    public sealed class LevelSolverResult
    {
        public bool Feasible { get; internal set; }
        public string Message { get; internal set; }
        public int[] WildLevels { get; internal set; }
        public int[] MutationLevels { get; internal set; }
        public int[] DomesticLevels { get; internal set; }
        public double[] Values { get; internal set; }
        public int TotalLevel { get; internal set; }
        public int TotalDomesticLevels { get; internal set; }
    }

    public static class LevelAllocationSolver
    {
        private sealed class Candidate
        {
            public int[] WildLevels;
            public int[] MutationLevels;
            public int[] DomesticLevels;
            public double[] Values;
            public int TotalLevel;
            public int TotalDomesticLevels;
            public int TotalMutationLevels;
            public double[] Objectives;
        }

        public static LevelSolverResult Solve(LevelSolverRequest request, CancellationToken cancellationToken = default)
        {
            var validationError = Validate(request);
            if (validationError != null)
                return new LevelSolverResult { Message = validationError };

            var activeStats = Enumerable.Range(0, Stats.StatsCount)
                .Where(s => s != Stats.Torpidity
                            && request.Species.stats[s] != null
                            && request.Species.DisplaysStat(s)
                            && (request.StatTargets[s].Mode == LevelTargetMode.Maximize
                                || request.StatTargets[s].TargetValue > 0))
                .ToArray();
            var maximizeStats = activeStats
                .Where(s => request.StatTargets[s].Mode == LevelTargetMode.Maximize)
                .OrderBy(s => request.StatTargets[s].Priority)
                .ThenBy(s => s)
                .ToArray();

            Candidate best = null;
            var domesticLevels = new int[Stats.StatsCount];
            SearchDomesticAllocations(request, activeStats, maximizeStats, domesticLevels, 0, 0, ref best,
                cancellationToken);

            if (best == null)
                return new LevelSolverResult { Message = "No allocation satisfies all targets within the level limits." };

            return new LevelSolverResult
            {
                Feasible = true,
                Message = "Optimal allocation found.",
                WildLevels = best.WildLevels,
                MutationLevels = best.MutationLevels,
                DomesticLevels = best.DomesticLevels,
                Values = best.Values,
                TotalLevel = best.TotalLevel,
                TotalDomesticLevels = best.TotalDomesticLevels
            };
        }

        private static void SearchDomesticAllocations(LevelSolverRequest request, int[] activeStats, int[] maximizeStats,
            int[] domesticLevels, int activeIndex, int domesticUsed, ref Candidate best,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (activeIndex == activeStats.Length)
            {
                var candidate = BuildCandidate(request, maximizeStats, domesticLevels);
                if (candidate != null && IsBetter(candidate, best))
                    best = candidate;
                return;
            }

            var statIndex = activeStats[activeIndex];
            var maxForStat = Math.Min(request.MaxDomesticLevels - domesticUsed, request.LevelCap - 1 - domesticUsed);
            for (var level = 0; level <= maxForStat; level++)
            {
                domesticLevels[statIndex] = level;
                SearchDomesticAllocations(request, activeStats, maximizeStats, domesticLevels,
                    activeIndex + 1, domesticUsed + level, ref best, cancellationToken);
            }
            domesticLevels[statIndex] = 0;
        }

        private static Candidate BuildCandidate(LevelSolverRequest request, int[] maximizeStats, int[] domesticLevels)
        {
            var wildLevels = request.WildLevels.ToArray();
            var mutationLevels = new int[Stats.StatsCount];
            var values = new double[Stats.StatsCount];
            var domesticUsed = domesticLevels.Sum();
            var pointsUsed = domesticUsed + wildLevels.Sum();

            for (var statIndex = 0; statIndex < Stats.StatsCount; statIndex++)
            {
                if (request.StatTargets[statIndex].Mode != LevelTargetMode.Target
                    || request.StatTargets[statIndex].TargetValue == 0)
                    continue;

                var mutationLevel = FindMinimumMutationLevel(request, statIndex, domesticLevels[statIndex]);
                if (mutationLevel < 0)
                    return null;

                mutationLevels[statIndex] = mutationLevel;
                pointsUsed += mutationLevel;
            }

            if (pointsUsed + 1 > request.LevelCap)
                return null;

            var remainingLevels = request.LevelCap - 1 - pointsUsed;
            foreach (var statIndex in maximizeStats)
            {
                var allocatedLevels = RoundDownToMutationStep(
                    Math.Min(GetMaxMutationLevels(request, statIndex), remainingLevels));
                if (allocatedLevels == 0)
                    continue;

                var valueAtZero = CalculateValue(request, statIndex, 0, domesticLevels[statIndex]);
                var maximumValue = CalculateValue(request, statIndex, allocatedLevels, domesticLevels[statIndex]);
                if (maximumValue <= valueAtZero)
                    continue;

                allocatedLevels = FindFirstMutationLevelForValue(request, statIndex, domesticLevels[statIndex], maximumValue,
                    allocatedLevels);
                mutationLevels[statIndex] = allocatedLevels;
                remainingLevels -= allocatedLevels;
                pointsUsed += allocatedLevels;
            }

            for (var statIndex = 0; statIndex < Stats.StatsCount; statIndex++)
            {
                values[statIndex] = StatValueCalculation.CalculateValue(request.Species, statIndex,
                    wildLevels[statIndex], mutationLevels[statIndex], domesticLevels[statIndex], true, 1,
                    request.ImprintingBonus, false);
            }

            return new Candidate
            {
                WildLevels = wildLevels,
                MutationLevels = mutationLevels,
                DomesticLevels = domesticLevels.ToArray(),
                Values = values,
                TotalLevel = pointsUsed + 1,
                TotalDomesticLevels = domesticUsed,
                TotalMutationLevels = mutationLevels.Sum(),
                Objectives = maximizeStats.Select(s => values[s]).ToArray()
            };
        }

        private static int FindMinimumMutationLevel(LevelSolverRequest request, int statIndex, int domesticLevels)
        {
            var target = request.StatTargets[statIndex].TargetValue;
            var maxMutationLevels = GetMaxMutationLevels(request, statIndex);
            if (CalculateValue(request, statIndex, maxMutationLevels, domesticLevels) < target)
                return -1;

            return FindFirstMutationLevelForValue(request, statIndex, domesticLevels, target, maxMutationLevels);
        }

        private static int FindFirstMutationLevelForValue(LevelSolverRequest request, int statIndex, int domesticLevels,
            double targetValue, int maximum)
        {
            var low = 0;
            var high = maximum / Ark.LevelsAddedPerMutation;
            while (low < high)
            {
                var middle = low + (high - low) / 2;
                if (CalculateValue(request, statIndex, middle * Ark.LevelsAddedPerMutation, domesticLevels) >= targetValue)
                    high = middle;
                else
                    low = middle + 1;
            }
            return low * Ark.LevelsAddedPerMutation;
        }

        private static double CalculateValue(LevelSolverRequest request, int statIndex, int mutationLevels,
            int domesticLevels)
            => StatValueCalculation.CalculateValue(request.Species, statIndex, request.WildLevels[statIndex],
                mutationLevels, domesticLevels, true, 1, request.ImprintingBonus, false);

        private static int GetMaxMutationLevels(LevelSolverRequest request, int statIndex)
            => request.Species.DisplaysStat(statIndex)
                ? RoundDownToMutationStep(request.MaxMutationLevelPerStat)
                : 0;

        private static int RoundDownToMutationStep(int levels)
            => levels - levels % Ark.LevelsAddedPerMutation;

        private static bool IsBetter(Candidate candidate, Candidate currentBest)
        {
            if (currentBest == null) return true;
            for (var index = 0; index < candidate.Objectives.Length; index++)
            {
                var comparison = candidate.Objectives[index].CompareTo(currentBest.Objectives[index]);
                if (comparison != 0) return comparison > 0;
            }
            if (candidate.TotalMutationLevels != currentBest.TotalMutationLevels)
                return candidate.TotalMutationLevels < currentBest.TotalMutationLevels;
            return candidate.TotalLevel < currentBest.TotalLevel;
        }

        private static string Validate(LevelSolverRequest request)
        {
            if (request?.Species?.stats == null || request.Species.stats.Length != Stats.StatsCount)
                return "A species with initialized stat values is required.";
            if (request.StatTargets == null || request.StatTargets.Length != Stats.StatsCount
                                            || request.StatTargets.Any(t => t == null))
                return $"Exactly {Stats.StatsCount} stat targets are required.";
            if (request.WildLevels == null || request.WildLevels.Length != Stats.StatsCount)
                return $"Exactly {Stats.StatsCount} wild levels are required.";
            if (request.LevelCap < 1 || request.MaxDomesticLevels < 0
                                     || request.MaxMutationLevelPerStat < 0
                                     || request.WildLevels.Any(level => level < 0))
                return "Level limits cannot be negative.";
            if (request.ImprintingBonus < 0)
                return "Imprinting cannot be negative.";
            if (request.StatTargets.Any(t => t.Mode == LevelTargetMode.Target && t.TargetValue < 0))
                return "Minimum stat targets cannot be negative.";
            return null;
        }
    }
}