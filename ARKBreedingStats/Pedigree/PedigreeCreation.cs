using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.uiControls;

namespace ARKBreedingStats.Pedigree
{
    /// <summary>
    /// Creates pedigree views.
    /// </summary>
    public static class PedigreeCreation
    {
        private const int YOffsetLineCompact = 30;
        internal const int YMarginCreatureCompact = 5;
        internal const int Margin = 10;
        private const int MinXPosCreature = 400;

        #region Compact View

        internal static int CreateCompactView(Creature creature, List<int[]>[] lines, List<Control> pedigreeControls, int displayedGenerations, int autoScrollPosX, int autoScrollPosY, int highlightInheritanceStatIndex)
        {
            // each extra generation adds one control width
            var xOffsetStart = 4 * Margin + (displayedGenerations < 2 ? 0 : (PedigreeCreatureCompact.ControlWidth + 3) * (1 << (displayedGenerations - 2)));
            var xOffsetParents = xOffsetStart / 2;
            if (xOffsetStart < MinXPosCreature) xOffsetStart = MinXPosCreature;
            var yOffsetGenerations = Math.Max(displayedGenerations - 1, 2); // don't shift few generations to the top
            var yOffsetStart = 6 * Margin + (PedigreeCreatureCompact.ControlHeight + PedigreeCreation.YMarginCreatureCompact) * yOffsetGenerations;
            var leftMargin = 2 * Margin;
            var xLowest = PedigreeCreation.CreateOffspringParentsCompact(creature, xOffsetStart, yOffsetStart, autoScrollPosX, autoScrollPosY,
                false, displayedGenerations, xOffsetParents, lines, pedigreeControls, int.MaxValue, true, highlightInheritanceStatIndex);
            var moveToLeft = xLowest - leftMargin;
            var maxMoveToLeft = Math.Max(0, xOffsetStart - MinXPosCreature);
            if (moveToLeft > maxMoveToLeft) moveToLeft = maxMoveToLeft;
            if (moveToLeft > 0)
                CompactViewLeftAlign(pedigreeControls, lines, moveToLeft);

            return yOffsetStart;
        }

        private static int CreateOffspringParentsCompact(Creature creature, int x, int y, int autoScrollPosX, int autoScrollPosY, bool onlyDrawParents, int generations, int xOffsetParent,
                    List<int[]>[] lines, List<Control> pcs, int xLowest, bool highlightCreature, int highlightStatIndex, int highlightMotherLine = 0, int highlightFatherLine = 0)
        {
            var (motherInheritance, fatherInheritance) =
                CreateParentsChildCompact(creature, x, y, autoScrollPosX, autoScrollPosY, xOffsetParent,
                    lines, pcs, onlyDrawParents, highlightCreature, highlightStatIndex, highlightMotherLine, highlightFatherLine);

            var newXLowest = creature.Mother != null ? x - xOffsetParent : x;
            if (newXLowest < xLowest) xLowest = newXLowest;

            if (--generations < 2) return xLowest;
            var yParents = y - PedigreeCreatureCompact.ControlHeight - YMarginCreatureCompact;
            if (creature.Mother != null)
                xLowest = CreateOffspringParentsCompact(creature.Mother, x - xOffsetParent, yParents, autoScrollPosX, autoScrollPosY,
                    true, generations, xOffsetParent / 2, lines, pcs, xLowest, false, highlightStatIndex, motherInheritance.maternalInheritance, motherInheritance.paternalInheritance);
            if (creature.Father != null)
                CreateOffspringParentsCompact(creature.Father, x + xOffsetParent, yParents, autoScrollPosX, autoScrollPosY,
                    true, generations, xOffsetParent / 2, lines, pcs, xLowest, false, highlightStatIndex, fatherInheritance.maternalInheritance, fatherInheritance.paternalInheritance);

            return xLowest;
        }

        /// <summary>
        /// Creates the controls that display a creature and its parents.
        /// </summary>
        /// <returns>True if stat inheritance line is continued (maternal, paternal).</returns>
        private static ((int maternalInheritance, int paternalInheritance) motherInheritance,
            (int maternalInheritance, int paternalInheritance) fatherInheritance)
            CreateParentsChildCompact(Creature creature, int x, int y, int autoScrollX, int autoScrollY, int xOffsetParents,
                List<int[]>[] lines, List<Control> pcs, bool onlyDrawParents, bool highlightCreature, int highlightStatIndex, int highlightMotherLine = 0, int highlightFatherLine = 0)
        {
            if (creature == null) return ((0, 0), (0, 0));

            // scroll offset for control-locations (not for lines)
            var xLine = x;
            var yLine = y;
            x += autoScrollX;
            y += autoScrollY;

            if (!onlyDrawParents)
            {
                // creature
                var c = new PedigreeCreatureCompact(creature, highlightCreature, highlightStatIndex)
                {
                    Location = new Point(x, y)
                };
                pcs.Add(c);
                if (highlightStatIndex != -1)
                    (highlightMotherLine, highlightFatherLine) = c.PossibleStatInheritance(highlightStatIndex);
            }

            if (creature.Mother == null && creature.Father == null) return ((0, 0), (0, 0));

            var statInheritanceMother = (0, 0);
            var statInheritanceFather = (0, 0);

            var yParents = y - PedigreeCreatureCompact.ControlHeight - YMarginCreatureCompact;
            // mother
            if (creature.Mother != null)
            {
                var c = new PedigreeCreatureCompact(creature.Mother, highlightStatIndex: highlightMotherLine != 0 ? highlightStatIndex : -1)
                {
                    Location = new Point(x - xOffsetParents, yParents)
                };
                pcs.Add(c);
                if (highlightMotherLine != 0 && highlightStatIndex != -1)
                    statInheritanceMother = c.PossibleStatInheritance(highlightStatIndex);
            }
            // father
            if (creature.Father != null)
            {
                var c = new PedigreeCreatureCompact(creature.Father, highlightStatIndex: highlightFatherLine != 0 ? highlightStatIndex : -1)
                {
                    Location = new Point(x + xOffsetParents, yParents)
                };
                pcs.Add(c);
                if (highlightFatherLine != 0 && highlightStatIndex != -1)
                    statInheritanceFather = c.PossibleStatInheritance(highlightStatIndex);
            }

            // lines
            //  M──┬──F
            //     O
            var yLineHorizontal = yLine - YOffsetLineCompact;
            var xCenterOffspring = xLine + PedigreeCreatureCompact.ControlWidth / 2;
            lines[2].Add(new[] { xLine - xOffsetParents + PedigreeCreatureCompact.ControlWidth, yLineHorizontal, xCenterOffspring, yLineHorizontal, highlightMotherLine });
            lines[2].Add(new[] { xLine + xOffsetParents, yLineHorizontal, xCenterOffspring, yLineHorizontal, highlightFatherLine });
            var combinedStyle = highlightMotherLine == 2 || highlightFatherLine == 2 ? 2
                : highlightMotherLine == 1 ? 1
                : highlightFatherLine;
            lines[1].Add(new[] { xCenterOffspring, yLineHorizontal, xCenterOffspring, yLine, combinedStyle });

            return (statInheritanceMother, statInheritanceFather);
        }

        /// <summary>
        /// Some pedigrees don't use all the controls at the left, so move the existing controls to the left.
        /// </summary>
        private static void CompactViewLeftAlign(List<Control> controls, List<int[]>[] lines, int moveToLeft)
        {
            foreach (var c in controls) c.Left -= moveToLeft;
            foreach (var ls in lines)
                foreach (var l in ls)
                {
                    l[0] -= moveToLeft;
                    l[2] -= moveToLeft;
                }
        }

        #endregion

        #region Detailed View

        internal const int LeftBorder = 40;
        internal static void CreateDetailedView(Creature creature, List<int[]>[] lines, List<Control> pedigreeControls,
            int autoScrollPosX, int autoScrollPosY, bool[] enabledColorRegions)
        {
            const int pedigreeElementWidth = 325;
            const int yCenterOfCreatureParent = 79;

            // draw creature
            CreateParentsChild(creature, lines, pedigreeControls, LeftBorder + pedigreeElementWidth + PedigreeCreation.Margin, 60, autoScrollPosX, autoScrollPosY, enabledColorRegions, true, true);

            // create ancestors
            if (creature.Mother != null
                && CreateParentsChild(creature.Mother, lines, pedigreeControls, LeftBorder, 20, autoScrollPosX, autoScrollPosY, enabledColorRegions))
            {
                lines[1].Add(new[]
                {
                    LeftBorder + pedigreeElementWidth, yCenterOfCreatureParent,
                    LeftBorder + pedigreeElementWidth + PedigreeCreation.Margin, yCenterOfCreatureParent, 0
                });
            }
            if (creature.Father != null
                && CreateParentsChild(creature.Father, lines, pedigreeControls, LeftBorder + 2 * (pedigreeElementWidth + PedigreeCreation.Margin), 20, autoScrollPosX, autoScrollPosY, enabledColorRegions))
            {
                lines[1].Add(new[]
                {
                    LeftBorder + 2 * pedigreeElementWidth + 2 * PedigreeCreation.Margin, yCenterOfCreatureParent,
                    LeftBorder + 2 * pedigreeElementWidth + PedigreeCreation.Margin, yCenterOfCreatureParent + 80, 0
                });
            }
        }

        /// <summary>
        /// Creates the controls that display a creature and its parents.
        /// </summary>
        /// <returns></returns>
        private static bool CreateParentsChild(Creature creature, List<int[]>[] lines, List<Control> pedigreeControls, int x, int y, int autoScrollPosX, int autoScrollPosY, bool[] enabledColorRegions, bool drawWithNoParents = false, bool highlightCreature = false)
        {
            if (creature == null || (!drawWithNoParents && creature.Mother == null && creature.Father == null))
                return false;

            // scroll offset for control-locations (not for lines)
            var xLine = x;
            var yLine = y;
            x += autoScrollPosX;
            y += autoScrollPosY;
            // creature
            pedigreeControls.Add(new PedigreeCreature(creature, enabledColorRegions)
            {
                Location = new Point(x, y + 40),
                Highlight = highlightCreature
            });

            // mother
            if (creature.Mother != null)
            {
                pedigreeControls.Add(new PedigreeCreature(creature.Mother, enabledColorRegions)
                {
                    Location = new Point(x, y)
                });
            }
            // father
            if (creature.Father != null)
            {
                pedigreeControls.Add(new PedigreeCreature(creature.Father, enabledColorRegions)
                {
                    Location = new Point(x, y + 80)
                });
            }

            CreateGeneInheritanceLines(creature, creature.Mother, creature.Father, lines, xLine, yLine);
            return true;
        }

        internal static void CreateGeneInheritanceLines(Creature offspring, Creature mother, Creature father, List<int[]>[] lines, int x, int y)
        {
            if (offspring.levelsWild == null || offspring.valuesDom == null) return;

            for (int s = 0; s < PedigreeCreature.DisplayedStatsCount; s++)
            {
                int si = PedigreeCreature.DisplayedStats[s];
                if (offspring.valuesDom[si] <= 0) continue; // don't display arrows for non used stats
                int better = 0; // if father < mother: 1, if mother < father: -1
                if (mother?.levelsWild != null && father?.levelsWild != null)
                {
                    if (mother.levelsWild[si] < father.levelsWild[si])
                        better = -1;
                    else if (mother.levelsWild[si] > father.levelsWild[si])
                        better = 1;
                }

                // offspring can have stats that are up to 2 levels higher due to mutations. currently there are no decreasing levels due to mutations
                if (mother?.levelsWild != null && offspring.levelsWild[si] >= 0 &&
                    (offspring.levelsWild[si] == mother.levelsWild[si] ||
                     offspring.levelsWild[si] == mother.levelsWild[si] + 2))
                {
                    lines[0].Add(new[]
                    {
                           PedigreeCreature.XOffsetFirstStat + x + PedigreeCreature.HorizontalStatDistance * s, y + 33,
                           PedigreeCreature.XOffsetFirstStat + x + PedigreeCreature.HorizontalStatDistance * s, y + 42, (better == -1 ? 1 : 2),
                            (offspring.levelsWild[si] > mother.levelsWild[si] ? 1 : 0)
                        });
                }

                if (father?.levelsWild != null && offspring.levelsWild[si] >= 0 &&
                    (offspring.levelsWild[si] == father.levelsWild[si] ||
                     offspring.levelsWild[si] == father.levelsWild[si] + 2))
                {
                    lines[0].Add(new[]
                    {
                           PedigreeCreature.XOffsetFirstStat + x +PedigreeCreature.HorizontalStatDistance * s, y + 83,
                           PedigreeCreature.XOffsetFirstStat + x +PedigreeCreature.HorizontalStatDistance * s, y + 74, (better == 1 ? 1 : 2),
                            (offspring.levelsWild[si] > father.levelsWild[si] ? 1 : 0)
                        });
                }
            }
        }

        #endregion
    }
}
