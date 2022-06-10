using ARKBreedingStats.Library;
using ARKBreedingStats.values;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ARKBreedingStats.duplicates
{
    class MergingDuplicates
    {
        private readonly List<Creature> creatureDuplicates1 = new List<Creature>();
        private readonly List<Creature> creatureDuplicates2 = new List<Creature>();
        public ProgressBar progressBar;

        public void CheckForDuplicates(List<Creature> creatureList)
        {
            creatureDuplicates1.Clear();
            creatureDuplicates2.Clear();

            int cnt = creatureList.Count;

            progressBar.Value = 0;
            progressBar.Maximum = cnt;
            progressBar.Visible = true;

            for (int i = 0; i < cnt; i++)
            {
                for (int j = i + 1; j < cnt; j++)
                {
                    if (isPossibleDuplicate(creatureList[i], creatureList[j]))
                    {
                        creatureDuplicates1.Add(creatureList[i]);
                        creatureDuplicates2.Add(creatureList[j]);
                    }
                }
                progressBar.Value++;
            }
            progressBar.Visible = false;

            if (creatureDuplicates1.Count == 0)
            {
                MessageBox.Show("No possible duplicates found", "No Duplicates", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // TODO. Handle duplicates. Suggest merging of possible duplicates
            if (MessageBox.Show(creatureDuplicates1.Count.ToString() + " possible duplicates found. Show them?\n" +
                    "This function is currently under development and does currently not more than showing a messagebox for each possible duplicate.",
                "Duplicates found", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int duplCount = creatureDuplicates1.Count;
                for (int i = 0; i < duplCount; i++)
                {
                    if (MessageBox.Show("Possible duplicate found (all wild levels are equal, the creatures also could be related).\n" + creatureDuplicates1[i].Species.name + "\n\""
                        + creatureDuplicates1[i].name + "\" and \""
                        + creatureDuplicates1[i].name + "\"",
                        "Possible duplicate found", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                        break;
                }
            }
        }

        /// <summary>
        /// Checks if creatures are possible duplicates. Checked are the species, bred-status, wild-levels and if one is not a parent of the other
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns>True if possible duplicate</returns>
        private static bool isPossibleDuplicate(Creature c1, Creature c2)
        {
            if (c1.Species != c2.Species
                || c1.isBred != c2.isBred
                )
                return false;

            // check if one creature is a parent of the other
            if (isAscendant(c1, c2)) return false;
            if (isAscendant(c2, c1)) return false;

            // check wild levels
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (c1.levelsWild[s] != c2.levelsWild[s])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if c1 is an ascendant of c2 (two generations are checked).
        /// </summary>
        /// <param name="possibleAscendant"></param>
        /// <param name="possibleDescendant"></param>
        /// <returns>true if possibleAscendant is ascendant in the two previous generations</returns>
        private static bool isAscendant(Creature possibleAscendant, Creature possibleDescendant)
        {
            if (possibleAscendant.sex == Sex.Female)
            {
                if (possibleDescendant.Mother != null &&
                    (possibleDescendant.Mother == possibleAscendant
                    || (possibleDescendant.Mother.Mother != null && possibleDescendant.Mother.Mother == possibleAscendant)
                    || (possibleDescendant.Father != null && possibleDescendant.Father.Mother != null && possibleDescendant.Father.Mother == possibleAscendant))
                    )
                    return true;
            }
            else if (possibleAscendant.sex == Sex.Male)
            {
                if (possibleDescendant.Father != null &&
                    (possibleDescendant.Father == possibleAscendant
                    || (possibleDescendant.Mother.Father != null && possibleDescendant.Mother.Father == possibleAscendant)
                    || (possibleDescendant.Father != null && possibleDescendant.Father.Father != null && possibleDescendant.Father.Father == possibleAscendant))
                    )
                    return true;
            }
            return false;
        }
    }
}
