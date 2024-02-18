using System;
using System.Collections.Generic;

namespace ARKBreedingStats.miscClasses
{
    /// <summary>
    /// Provides specific help for extraction-issues
    /// </summary>
    public static class IssueNotes
    {
        /// <summary>
        /// Returns a list of possible reasons that could cause the issues.
        /// </summary>
        /// <param name="issues"></param>
        /// <returns></returns>
        public static string getHelpTexts(Issue issues)
        {
            List<string> notes = new List<string>();
            int n = 1;
            int i = (int)issues;
            while (i >= n)
            {
                if ((i & n) != 0)
                    notes.Add($"{(notes.Count + 1)}. {GetHelpText((Issue)n)}");
                n <<= 1;
            }
            return string.Join("\n\n", notes.ToArray());
        }

        private static string GetHelpText(Issue issue)
        {
            switch (issue)
            {
                case Issue.Typo: return Loc.S("issueCauseTypo");
                case Issue.WildTamedBred: return Loc.S("issueCauseWildTamedBred");
                case Issue.TamingEffectivenessRange: return Loc.S("issueCauseTamingEffectivenessRange");
                case Issue.LockedDom: return Loc.S("issueCauseLockedDomLevel");
                case Issue.ImprintingLocked: return Loc.S("issueCauseImprintingLocked");
                case Issue.ImprintingNotUpdated: return Loc.S("issueCauseImprintingNotUpdated");
                case Issue.ImprintingNotPossible: return Loc.S("issueCauseImprintingNotPossible");
                case Issue.SinglePlayer: return Loc.S("issueCauseSingleplayer");
                case Issue.WildLevelSteps: return Loc.S("issueCauseWildLevelSteps");
                case Issue.MaxWildLevel: return Loc.S("issueCauseMaxWildLevel");
                case Issue.SpeedLevelingSetting: return Loc.S("issueCauseSpeedLevelingSetting");
                case Issue.StatMultipliers: return Loc.S("issueCauseStatMultipliers");
                case Issue.ModValues: return Loc.S("issueCauseModValues");
                case Issue.ArkStatIssue: return Loc.S("issueCauseArkStatIssue");
                case Issue.CreatureLevel: return Loc.S("issueCauseCreatureLevel");
                case Issue.OutdatedInGameValues: return Loc.S("issueCauseOutdatedIngameValues");
                case Issue.ImpossibleTe: return Loc.S("issueCauseImpossibleTe");
            }
            return string.Empty;
        }

        // order the enums according to their desired position in the issue-help, i.e. critical and common issues first
        [Flags]
        public enum Issue
        {
            None = 0,
            ImprintingNotPossible = 1 << 0,
            Typo = 1 << 1,
            CreatureLevel = 1 << 2,
            WildTamedBred = 1 << 3,
            LockedDom = 1 << 4,
            SinglePlayer = 1 << 5,
            MaxWildLevel = 1 << 6,
            SpeedLevelingSetting = 1 << 7,
            StatMultipliers = 1 << 8,
            ImprintingNotUpdated = 1 << 9,
            TamingEffectivenessRange = 1 << 10,
            ImprintingLocked = 1 << 11,
            ModValues = 1 << 12,
            WildLevelSteps = 1 << 13,
            ArkStatIssue = 1 << 14,
            OutdatedInGameValues = 1 << 15,
            ImpossibleTe = 1 << 16
        }
    }
}
