using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARKBreedingStats.importExportGun;
using ARKBreedingStats.species;
using ARKBreedingStats.values;

namespace ARKBreedingStats.multiplierTesting
{
    /// <summary>
    /// Determines stats of a species from a combination of given levels and stat values.
    /// </summary>
    internal static class SpeciesStatsExtractor
    {
        public static bool ExtractStatValues(IList<ExportGunCreatureFile> creatureFiles, ServerMultipliers serverMultipliers, out Species species, out string errorText)
        {
            if (!CheckInput(creatureFiles, out creatureFiles, serverMultipliers, out species, out errorText))
                return false;

            if (!ExtractValues(creatureFiles, serverMultipliers, species, out errorText))
                return false;

            return true;
        }

        private static bool CheckInput(IList<ExportGunCreatureFile> creatureFiles, out IList<ExportGunCreatureFile> cleanedCreatureFiles, ServerMultipliers serverMultipliers,
            out Species species, out string errorText)
        {
            species = null;
            cleanedCreatureFiles = null;
            if (serverMultipliers?.statMultipliers == null)
            {
                errorText = "server multipliers contain no stat multipliers.";
                return false;
            }
            if (creatureFiles?.Any() != true)
            {
                errorText = "no creature files provided.";
                return false;
            }
            errorText = null;

            // only one species per time, only use species of first file
            var blueprintPath = creatureFiles.First().BlueprintPath;
            species = new Species
            {
                blueprintPath = blueprintPath,
                fullStatsRaw = new double[Stats.StatsCount][]
            };
            species.Initialize();
            cleanedCreatureFiles = creatureFiles.Where(cf => cf.BlueprintPath == blueprintPath).ToArray();
            return true;
        }

        private static bool ExtractValues(IList<ExportGunCreatureFile> creatureFiles, ServerMultipliers serverMultipliers, Species species, out string errorText)
        {
            const int roundToDigits = 3;
            var wildCreatures = creatureFiles.Where(c => c.IsWild()).ToArray();
            var domCreatures = creatureFiles.Where(c => !c.IsWild()).ToArray();

            // for the determination of Ta and Tm two creatures with different TE are needed
            var creaturesOrderedByTeWithoutDomLevels = domCreatures
                .Where(ec => ec.Stats.All(s => s.Tamed == 0 && s.Mutated == 0))
                .OrderBy(ec => ec.TameEffectiveness).ToArray();
            var crHighTe = creaturesOrderedByTeWithoutDomLevels.Last();
            var crLowTe = creaturesOrderedByTeWithoutDomLevels.First();

            errorText = null;
            var errorSb = new StringBuilder();
            var taTmSolver = new TaTmSolver();

            ServerMultipliers singlePlayerMultipliers = null;
            if (serverMultipliers.SinglePlayerSettings)
            {
                singlePlayerMultipliers =
                   Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.Singleplayer);
                if (singlePlayerMultipliers == null)
                {
                    errorText = "Singleplayer server multiplier preset not available.";
                    return false;
                }
            }

            for (var s = 0; s < Stats.StatsCount; s++)
            {
                var svStats = serverMultipliers.statMultipliers[s];
                if (singlePlayerMultipliers?.statMultipliers[s] != null)
                {
                    svStats = svStats.ToArray(); // use copy to not alter original server values
                    for (int i = 0; i < svStats.Length; i++)
                    {
                        svStats[i] *= singlePlayerMultipliers.statMultipliers[s][i];
                    }
                }

                // base stat value
                var wildCreatureWithZeroWildLevels =
                    wildCreatures.FirstOrDefault(ec => ec.Stats[s].Wild == 0 && ec.Stats[s].Mutated == 0);
                if (wildCreatureWithZeroWildLevels == null)
                {
                    errorSb.AppendLine(
                        $"no wild creature with 0 levels in stat [{s}] ({Utils.StatName(s)}) provided. This stat cannot be calculated further");
                    continue;
                }

                species.fullStatsRaw[s] = new double[5];
                var spStats = species.fullStatsRaw[s];
                var baseValue = wildCreatureWithZeroWildLevels.GetStatValue(s);
                if (baseValue == 0) continue;

                spStats[Species.StatsRawIndexBase] = baseValue;

                // inc per wild
                var wildCreatureWithNonZeroWildLevels =
                    wildCreatures.FirstOrDefault(ec => ec.Stats[s].Wild > 0 && ec.Stats[s].Mutated == 0);
                if (wildCreatureWithNonZeroWildLevels == null)
                {
                    errorSb.AppendLine(
                        $"no wild creature with >0 levels in stat [{s}] ({Utils.StatName(s)}), iw cannot be determined and other values of this stat will be skipped.");
                    continue;
                }

                var incPerWild = Math.Round(
                    (wildCreatureWithNonZeroWildLevels.GetStatValue(s) / spStats[Species.StatsRawIndexBase] - 1)
                    / (wildCreatureWithNonZeroWildLevels.Stats[s].Wild * svStats[ServerMultipliers.IndexLevelWild])
                    , roundToDigits);
                spStats[Species.StatsRawIndexIncPerWildLevel] = incPerWild;

                // TBHM
                var tbhm = 1;
                if (s == Stats.Health)
                {
                    species.TamedBaseHealthMultiplier = 1;
                    // todo
                }

                // ta, tm
                taTmSolver.SetFirstEquation(crHighTe.GetStatValue(s), baseValue,
                    crHighTe.Stats[s].Wild, incPerWild, svStats[ServerMultipliers.IndexLevelWild],
                    tbhm, crHighTe.DinoImprintingQuality, species.StatImprintMultipliers[s],
                    serverMultipliers.BabyImprintingStatScaleMultiplier,
                    crHighTe.TameEffectiveness, crHighTe.Stats[s].Tamed, 0, 0);

                errorText = taTmSolver.CalculateTaTm(crLowTe.GetStatValue(s), baseValue,
                    crLowTe.Stats[s].Wild, incPerWild,
                    svStats[ServerMultipliers.IndexLevelWild],
                    tbhm, crLowTe.DinoImprintingQuality, species.StatImprintMultipliers[s],
                    serverMultipliers.BabyImprintingStatScaleMultiplier,
                    crLowTe.TameEffectiveness, crLowTe.Stats[s].Tamed, 0, 0, out var taTaM, out var tmTmM);
                if (!string.IsNullOrEmpty(errorText))
                {
                    errorSb.AppendLine($"Error when calculating ta tm for stat {s}: " + errorText);
                }

                if (taTaM != 0 && svStats[ServerMultipliers.IndexTamingAdd] != 0)
                    spStats[Species.StatsRawIndexAdditiveBonus] =
                        Math.Round(taTaM / svStats[ServerMultipliers.IndexTamingAdd], roundToDigits);
                if (tmTmM != 0 && svStats[ServerMultipliers.IndexTamingMult] != 0)
                    spStats[Species.StatsRawIndexMultiplicativeBonus] =
                        Math.Round(tmTmM / svStats[ServerMultipliers.IndexTamingMult], roundToDigits);

                // dom level
                var creatureWithNonZeroDomLevels =
                    domCreatures.FirstOrDefault(ec => ec.Stats[s].Tamed > 0 && ec.Stats[s].Mutated == 0);
                if (creatureWithNonZeroDomLevels == null)
                {
                    errorSb.AppendLine(
                        $"no creature with >0 domestic levels in stat [{s}] ({Utils.StatName(s)}), id cannot be calculated.");
                }
                else
                {
                    var crStats = creatureWithNonZeroDomLevels.Stats[s];
                    spStats[Species.StatsRawIndexIncPerDomLevel] = Math.Round(
                        (creatureWithNonZeroDomLevels.GetStatValue(s) /
                            ((spStats[Species.StatsRawIndexBase] * (1 + (crStats.Wild + crStats.Mutated) * incPerWild *
                            svStats[ServerMultipliers.IndexLevelWild]) * tbhm * (1 +
                                  creatureWithNonZeroDomLevels.DinoImprintingQuality *
                                  species.StatImprintMultipliers[s] *
                                  serverMultipliers.BabyImprintingStatScaleMultiplier)
                              + spStats[Species.StatsRawIndexAdditiveBonus] * svStats[ServerMultipliers.IndexTamingAdd])
                             * (1 + creatureWithNonZeroDomLevels.TameEffectiveness *
                                 spStats[Species.StatsRawIndexMultiplicativeBonus] *
                                 svStats[ServerMultipliers.IndexTamingMult])) - 1)
                        / (crStats.Tamed * svStats[ServerMultipliers.IndexLevelDom])
                        , roundToDigits);
                }
            }

            species.Initialize(); // initialize second time to set used stats

            errorText = errorSb.ToString();
            return true;
        }
    }
}
