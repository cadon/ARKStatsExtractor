using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.library
{
    /// <summary>
    /// Creates console commands to spawn creatures.
    /// </summary>
    public static class ArkConsoleCommands
    {
        public static string CheatPrefix => Properties.Settings.Default.AdminConsoleCommandWithCheat
            ? "cheat "
            : string.Empty;

        public static Form1.SetMessageLabelTextEventHandler SetMessageLabelText { get; set; }

        /// <summary>
        /// First value is distance in view direction, second is offset above-below, third is left-right.
        /// </summary>
        private const string SpawnDistance = "500 0 0";

        /// <summary>
        /// Copies a spawn command for a wild creature to the clipboard.
        /// </summary>
        public static void WildSpawnToClipboard(Species species, int level = 35)
        {
            if (species?.blueprintPath == null) return;
            level = Math.Max(1, level);
            var command = $"SpawnDino \"Blueprint'{species.blueprintPath}'\" {SpawnDistance} {level}";
            if (TryCopyCommandToClipboard(command))
                SetMessageLabelText($"Copied to clipboard: {command}", MessageBoxIcon.Information);
        }

        /// <summary>
        /// Creates a spawn command that works in the vanilla game, but that command can cause the game to crash if the result of this method is changed.
        /// The stat values and colors are only correct after cryoing the creature.
        /// </summary>
        public static void UnstableSpawnCommandToClipboard(Creature cr, string game = null)
        {
            if (cr == null) return;

            // see https://ark.wiki.gg/wiki/Console_commands#SpawnExactDino for this command in ARK. It's unstable and can crash the game if the format or data is not correct.
            var xp = 0; // TODO
            long arkIdInGame = cr.ArkIdImported ? cr.ArkId : 0;

            var spawnCommand =
                $"SpawnExactDino \"Blueprint'{cr.speciesBlueprint}'\" \"\" 1 {cr.LevelHatched} {cr.levelsDom.Sum()} "
                + $"\"{GetLevelStringForExactSpawningCommand(cr.levelsWild)}\" \"{GetLevelStringForExactSpawningCommand(cr.levelsDom)}\" \"{cr.name}\" "
                + $"0 {(cr.flags.HasFlag(CreatureFlags.Neutered) ? "1" : "0")} \"\" \"\" \"{cr.imprinterName}\" 0 {cr.imprintingBonus} "
                + $"\"{(cr.colors == null ? string.Empty : string.Join(",", cr.colors))}\" {arkIdInGame} {xp} 0 1000 20";

            if (TryCopyCommandToClipboard(spawnCommand))
            {
                var notIncluded = "The command doesn't include the XP, " + game == Ark.Ase
                    ? "and the imprinterName, thus the imprinting is probably not set."
                    : ", the imprinterName (imprinting is probably not set) and the mutation levels (you can use the mutation level command for adding them).";
                SetMessageLabelText(
                    $"The SpawnExactDino admin console command for the creature {cr.name} ({cr.SpeciesName}) was copied to the clipboard. " +
                    notIncluded
                    + "WARNING: this console command is unstable and can crash your game. Use with caution! The colors and stats will only be correct after putting the creature in a cryopod.",
                    MessageBoxIcon.Warning);
            }
        }

        private static string GetLevelStringForExactSpawningCommand(int[] levels)
        {
            // 8 stats are expected
            // stat order for this command is health, stamina, oxygen, food, weight, melee damage, movement speed, crafting skill
            var statValues = new[]
                {
                    Stats.Health, Stats.Stamina, Stats.Oxygen, Stats.Food, Stats.Weight, Stats.MeleeDamageMultiplier, Stats.SpeedMultiplier, Stats.CraftingSpeedMultiplier
                }
                .Select(si => Math.Min(255, Math.Max(0, levels?[si] ?? 0))).ToArray();

            return string.Join(",", statValues);
        }

        private static string GetLevelStringForExactSpawningCommandDS2(int[] wildLvl, int[] domLvl)
        {
            wildLvl = wildLvl ?? Enumerable.Repeat(0, Stats.StatsCount).ToArray();
            domLvl = domLvl ?? Enumerable.Repeat(0, Stats.StatsCount).ToArray();
            var statIndices = new[]
            {
                Stats.Health, Stats.Stamina, Stats.Oxygen, Stats.Food,
                Stats.Water, Stats.Weight, Stats.MeleeDamageMultiplier,
                Stats.SpeedMultiplier, Stats.CraftingSpeedMultiplier
            };
            return string.Join(string.Empty, statIndices.Select(si => $"{wildLvl[si]}/{domLvl[si]} "));
        }

        /// <summary>
        /// Creates a spawn command that needs the mod DinoStorageV2.
        /// </summary>
        public static void DinoStorageV2CommandToClipboard(Creature cr)
        {
            var xyzPosition = "0 50 50";
            var spawnCommand = $"admincheat scriptcommand spawndino_ds {cr.speciesBlueprint} 0 {xyzPosition} {(cr.isDomesticated ? "1" : "0")} {(cr.sex == Sex.Female ? "1" : cr.sex == Sex.Unknown ? "?" : "0")} "
                               + $"{cr.Maturation} {cr.imprintingBonus} {(cr.flags.HasFlag(CreatureFlags.Neutered) ? "1" : "0")} 0 0 "
                               + GetLevelStringForExactSpawningCommandDS2(cr.levelsWild, cr.levelsDom)
                               + $"{(cr.colors == null ? "0 0 0 0 0 0" : string.Join(" ", cr.colors))}";

            if (TryCopyCommandToClipboard(spawnCommand))
                SetMessageLabelText($"The SpawnExactDino admin console command for the creature {cr.name} ({cr.SpeciesName}) was copied to the clipboard. The command needs the mod DinoStorage V2 installed on the server to work. It doesn't include the mutation levels",
                    MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Creates a console command that will add the mutation levels of the creature.
        /// </summary>
        public static void MutationLevelCommandToClipboard(Creature cr)
        {
            if (cr == null) return;
            var commands = new List<string>();
            if (cr.levelsMutated?.Any(l => l > 0) == true)
                commands.AddRange(cr.levelsMutated.Select((l, i) => (l, i)).Where(li => li.l > 0).Select(li => $"addmutations {li.i} {li.l}"));

            if (TryCopyCommandsToClipboard(commands))
                SetMessageLabelText($"The admin console command for adding the mutation levels to the creature {cr.name} ({cr.SpeciesName}) was copied to the clipboard.",
                    MessageBoxIcon.Information);
        }

        public static void AdminCommandToSetColors(byte[] colors, Species species = null)
        {
            if (colors == null) return;

            var colorCommands = new List<string>(Ark.ColorRegionCount);
            for (int ci = 0; ci < colors.Length; ci++)
            {
                if (species?.EnabledColorRegions[ci] != false)
                    colorCommands.Add($"setTargetDinoColor {ci} {colors[ci]}");
            }

            TryCopyCommandsToClipboard(colorCommands);
        }

        private static bool TryCopyCommandToClipboard(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                ClipboardHandler.Clear();
                return false;
            }

            if (!ClipboardHandler.SetText(CheatPrefix + command, out var error))
            {
                SetMessageLabelText?.Invoke($"Error while trying to copy command to clipboard. You can try again. Error: {error}", MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private static bool TryCopyCommandsToClipboard(List<string> commands)
        {
            if (commands?.Any() != true)
            {
                ClipboardHandler.Clear();
                return false;
            }
            return TryCopyCommandToClipboard(string.Join(" | " + CheatPrefix, commands));
        }
    }
}
