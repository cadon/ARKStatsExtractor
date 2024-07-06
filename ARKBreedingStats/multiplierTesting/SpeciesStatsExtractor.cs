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
        public static bool ExtractStatValues(IList<ExportGunCreatureFile> creatureFiles, ServerMultipliers serverMultipliers, out Species species, out string resultText, out bool isError)
        {
            if (!CheckInput(creatureFiles, out creatureFiles, serverMultipliers, out species, out resultText))
            {
                isError = true;
                return false;
            }

            return ExtractValues(creatureFiles, serverMultipliers, species, out resultText, out isError);
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

        private static bool ExtractValues(IList<ExportGunCreatureFile> creatureFiles, ServerMultipliers serverMultipliers, Species species, out string resultText, out bool isError)
        {
            const int roundToDigits = 3;

            resultText = null;
            isError = false;
            var errorSb = new StringBuilder();
            var taTmSolver = new TaTmSolver();

            var wildCreatures = creatureFiles.Where(c => c.IsWild()).ToArray();
            var domCreatures = creatureFiles.Where(c => !c.IsWild()).ToArray();

            // for the determination of Ta and Tm two creatures with different TE are needed
            var creaturesOrderedByTeWithoutDomLevels = domCreatures
                .Where(ec => ec.DinoImprintingQuality == 0 && ec.Stats.All(s => s.Tamed == 0 && s.Mutated == 0))
                .OrderBy(ec => ec.TameEffectiveness).ToArray();
            var crHighTe = creaturesOrderedByTeWithoutDomLevels.Last();
            var crLowTe = creaturesOrderedByTeWithoutDomLevels.First();

            var creaturesWithImprinting = domCreatures
                .Where(c => c.DinoImprintingQuality > 0.01)
                    .OrderByDescending(c => c.DinoImprintingQuality).ToArray();

            if (!creaturesWithImprinting.Any())
            {
                errorSb.AppendLine("No creature with imprinting given. Species specific stat imprinting multipliers cannot be determined and default values are assumed.");
            }
            else if (creaturesWithImprinting.First().DinoImprintingQuality < 0.1)
            {
                errorSb.AppendLine($"Creatures with imprinting have low imprinting of {creaturesWithImprinting.First().DinoImprintingQuality:p1}. Stat imprinting multipliers may be unprecise.");
            }

            ServerMultipliers singlePlayerMultipliers = null;
            if (serverMultipliers.SinglePlayerSettings)
            {
                singlePlayerMultipliers =
                   Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.Singleplayer);
                if (singlePlayerMultipliers == null)
                {
                    resultText = "Singleplayer server multiplier preset not available.";
                    isError = true;
                    return false;
                }
            }

            var speciesStatImprintingMultipliers = Species.StatImprintMultipliersDefaultAse.ToArray();

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
                    isError = true;
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
                    isError = true;
                    continue;
                }

                var incPerWild = Math.Round(
                    (wildCreatureWithNonZeroWildLevels.GetStatValue(s) / spStats[Species.StatsRawIndexBase] - 1)
                    / (wildCreatureWithNonZeroWildLevels.Stats[s].Wild * svStats[ServerMultipliers.IndexLevelWild])
                    , roundToDigits);
                spStats[Species.StatsRawIndexIncPerWildLevel] = incPerWild;

                var tbhm = 1d;
                if (s == Stats.Health)
                {
                    // assuming TBHM is only for HP and Tm == 0 for HP
                    // to determine TBHM two creatures with a difference between
                    // baseValue * (1 + lw * iw * iwm) * (1 + ib * ibs * ibm)
                    // and without dom levels is needed. Take the creatures with the min and max.
                    var creaturesOrderedByWildHpLevelsWithoutDomLevels = creaturesOrderedByTeWithoutDomLevels
                        .OrderBy(c =>
                                baseValue * (1 + c.Stats[Stats.Health].Wild * incPerWild * svStats[ServerMultipliers.IndexLevelWild])
                                * (1 + c.DinoImprintingQuality * species.StatImprintMultipliers[s] * serverMultipliers.BabyImprintingStatScaleMultiplier)
                            ).ToArray();
                    var lowLevelHpCreature = creaturesOrderedByWildHpLevelsWithoutDomLevels.First();
                    var highLevelHpCreature = creaturesOrderedByWildHpLevelsWithoutDomLevels.Last();

                    taTmSolver.SetFirstEquation(lowLevelHpCreature.GetStatValue(s), baseValue,
                        lowLevelHpCreature.Stats[s].Wild, incPerWild, svStats[ServerMultipliers.IndexLevelWild],
                        1, lowLevelHpCreature.DinoImprintingQuality, species.StatImprintMultipliers[s],
                        serverMultipliers.BabyImprintingStatScaleMultiplier,
                        lowLevelHpCreature.TameEffectiveness, lowLevelHpCreature.Stats[s].Tamed, 0, 0);

                    resultText = taTmSolver.CalculateTaTbhm(highLevelHpCreature.GetStatValue(s), baseValue,
                        highLevelHpCreature.Stats[s].Wild, incPerWild,
                        svStats[ServerMultipliers.IndexLevelWild],
                         highLevelHpCreature.DinoImprintingQuality, species.StatImprintMultipliers[s],
                        serverMultipliers.BabyImprintingStatScaleMultiplier,
                        highLevelHpCreature.TameEffectiveness, highLevelHpCreature.Stats[s].Tamed, 0, 0, out var taTaM, out tbhm);

                    if (!string.IsNullOrEmpty(resultText))
                    {
                        errorSb.AppendLine($"Error when calculating ta and tbhm for stat {s}: " + resultText);
                        isError = true;
                    }

                    if (taTaM != 0 && svStats[ServerMultipliers.IndexTamingAdd] != 0)
                        spStats[Species.StatsRawIndexAdditiveBonus] =
                            Math.Round(taTaM / svStats[ServerMultipliers.IndexTamingAdd], roundToDigits);
                    if (tbhm != 0)
                        species.TamedBaseHealthMultiplier = (float)tbhm;
                }
                else
                {
                    // ta, tm
                    taTmSolver.SetFirstEquation(crHighTe.GetStatValue(s), baseValue,
                        crHighTe.Stats[s].Wild, incPerWild, svStats[ServerMultipliers.IndexLevelWild],
                        1, crHighTe.DinoImprintingQuality, species.StatImprintMultipliers[s],
                        serverMultipliers.BabyImprintingStatScaleMultiplier,
                        crHighTe.TameEffectiveness, crHighTe.Stats[s].Tamed, 0, 0);

                    resultText = taTmSolver.CalculateTaTm(crLowTe.GetStatValue(s), baseValue,
                        crLowTe.Stats[s].Wild, incPerWild,
                        svStats[ServerMultipliers.IndexLevelWild],
                        1, crLowTe.DinoImprintingQuality, species.StatImprintMultipliers[s],
                        serverMultipliers.BabyImprintingStatScaleMultiplier,
                        crLowTe.TameEffectiveness, crLowTe.Stats[s].Tamed, 0, 0, out var taTaM, out var tmTmM);

                    if (!string.IsNullOrEmpty(resultText))
                    {
                        errorSb.AppendLine($"Error when calculating ta tm for stat {s}: " + resultText);
                        isError = true;
                    }

                    if (taTaM != 0 && svStats[ServerMultipliers.IndexTamingAdd] != 0)
                        spStats[Species.StatsRawIndexAdditiveBonus] =
                            Math.Round(taTaM / svStats[ServerMultipliers.IndexTamingAdd], roundToDigits);
                    if (tmTmM != 0 && svStats[ServerMultipliers.IndexTamingMult] != 0)
                        spStats[Species.StatsRawIndexMultiplicativeBonus] =
                            Math.Round(tmTmM / svStats[ServerMultipliers.IndexTamingMult], roundToDigits);
                }

                // dom level
                var creatureWithNonZeroDomLevels =
                    domCreatures.FirstOrDefault(ec => ec.Stats[s].Tamed > 0 && ec.Stats[s].Mutated == 0 && ec.DinoImprintingQuality == 0);
                if (creatureWithNonZeroDomLevels == null)
                {
                    // some levels cannot be levelled, so it's not necessarily an error
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

                // imprinting multiplier
                if (creaturesWithImprinting.Any())
                {
                    // if dom levels are not known, only use creature with no dom levels
                    var creatureWithImprinting = creaturesWithImprinting
                        .FirstOrDefault(c => creatureWithNonZeroDomLevels != null || c.Stats[s].Tamed == 0);
                    if (creatureWithImprinting != null)
                    {
                        var crStats = creatureWithImprinting.Stats[s];
                        speciesStatImprintingMultipliers[s] = Math.Round(
                            ((creatureWithNonZeroDomLevels.GetStatValue(s) /
                              ((1 + creatureWithImprinting.TameEffectiveness *
                                  spStats[Species.StatsRawIndexMultiplicativeBonus] *
                                  svStats[ServerMultipliers.IndexTamingMult]) * (1 + crStats.Tamed * spStats[Species.StatsRawIndexIncPerDomLevel] * svStats[ServerMultipliers.IndexLevelDom]))
                              - spStats[Species.StatsRawIndexAdditiveBonus] * svStats[ServerMultipliers.IndexTamingAdd]) / (spStats[Species.StatsRawIndexBase] * (1 + (crStats.Wild + crStats.Mutated) * incPerWild *
                                svStats[ServerMultipliers.IndexLevelWild]) * tbhm) - 1) / (creatureWithImprinting.DinoImprintingQuality *
                                serverMultipliers.BabyImprintingStatScaleMultiplier)
                            , roundToDigits);
                    }
                    else
                    {
                        errorSb.AppendLine(
                            $"For stat [{s}] ({Utils.StatName(s)}) species stat imprinting could not be determined (incPerDomLevel could be determined before).");
                    }
                }
            }

            // if statImprinting is default, no need to save it
            var defaultStatImprintingMultipliers = Species.StatImprintMultipliersDefaultAse;
            for (var si = 0; si < Stats.StatsCount; si++)
            {
                if (speciesStatImprintingMultipliers[si] != defaultStatImprintingMultipliers[si])
                {
                    species.StatImprintMultipliersRaw = speciesStatImprintingMultipliers;
                    break;
                }
            }

            species.Initialize(); // initialize second time to set used stats

            resultText = errorSb.ToString();
            return true;
        }
    }
}
