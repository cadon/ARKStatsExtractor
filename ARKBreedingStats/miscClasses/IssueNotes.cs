using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats.miscClasses
{
    /// <summary>
    /// Provides specific help for extraction-issues
    /// </summary>
    public static class IssueNotes
    {


        public static string getHelpTexts(Issue issues)
        {
            List<string> notes = new List<string>();
            int n = 1;
            int i = (int)issues;
            while (i >= n)
            {
                if ((i & n) != 0)
                    notes.Add((notes.Count + 1).ToString() + ". " + getHelpText((Issue)n));
                n <<= 1;
            }
            return String.Join("\n\n", notes.ToArray());
        }

        public static string getHelpText(Issue issue)
        {
            switch (issue)
            {
                case Issue.Typo: return "Double check if all stat-values are entered correctly.";
                case Issue.WildTamedBred: return "Make sure the correct state (wild, tamed, bred) for the creature is chosen.";
                case Issue.TamingEffectivenessRange: return "Taming-Effectiveness-Range too narrow, increase the upper and / or decrase the lower bound.";
                case Issue.LockedDom: return "Uncheck all Lock-to-Zero-buttons in the stats (all lock-symbols should be green and opened).";
                case Issue.ImprintingLocked: return "You locked the imprinting-value by checking \"Exactly, don't adjust\" and the tool won't adjust it, this may cause the extraction to fail.";
                case Issue.ImprintingNotUpdated: return "Sometimes the game doesn't update the stat-value after an imprinting. Try to leave and re-enter the render-distance or (wait for a) restart the server and try again.";
                case Issue.ImprintingNotPossible:
                    return "The imprinting-percentage given is not possible with the current multipliers and may cause wrong values during the extraction-process.\n"
+ "Make sure the BabyCuddleIntervallMultiplier and the BabyMatureSpeedMultiplier are set correctly.\n"
+ "They may have to be set to the value when the creature hatched/was born, even if they were changed.";
                case Issue.Singleplayer: return "If you have enabled the Singleplayer-Settings in the game, make sure the checkbox is enabled in the settings in this program as well";
                case Issue.WildLevelSteps: return "Adjust or disable the \"Consider Wild-level - steps\" in the settings.";
                case Issue.MaxWildLevel: return "The maximal wild level is set too low, go to the settings and adjust it";
                case Issue.StatMultipliers: return "The multipliers in the Settings (File - Settings) are not set to the multipliers of the server. Ask your admin for the correct multipliers and adjust them in the Settings.";
                case Issue.OutdatedIngameValues: return "The stats of the creature were changed recently and the game displays the old values. Level up a stat, that should trigger a recalculation of the values.";
                case Issue.ASBUpdate: return "The stat-values in this tool are wrong or the game does show wrong stats. You can send me a screenshot that contains the stats of the creature ingame and the extractor with the typed in values along with the stat-multipliers in the settings via reddit or github.";
                case Issue.CreatureLevel: return "Check if the total level of the creature is entered correctly.";
            }
            return "";
        }


        // order the enums according to their desired position in the issue-help, i.e. critical and common issues first
        [Flags]
        public enum Issue
        {
            None = 0,
            ImprintingNotPossible = 1,
            Typo = 2,
            CreatureLevel = 4,
            WildTamedBred = 8,
            LockedDom = 16,
            Singleplayer = 32,
            MaxWildLevel = 64,
            StatMultipliers = 128,
            ImprintingNotUpdated = 256,
            TamingEffectivenessRange = 512,
            ImprintingLocked = 1024,
            WildLevelSteps = 2048,
            OutdatedIngameValues = 4096,
            ASBUpdate = 8192
        }
    }
}
