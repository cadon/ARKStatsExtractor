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
        private const int MinXPosCreature = 440;

        #region Compact View

        internal static int CreateCompactView(Creature creature, List<int[]>[] lines, List<Control> pedigreeControls, int displayedGenerations, int autoScrollPosX, int autoScrollPosY, int highlightInheritanceStatIndex, bool hView)
        {
            const int pedigreeControlHorizontalMargin = 3;
            const int distanceInHView = 5;

            // y
            var controlHeightWithMargin = hView ? PedigreeCreatureCompact.ControlWidth + distanceInHView : PedigreeCreatureCompact.ControlHeight + YMarginCreatureCompact;
            var yOffsetOriginCreature = 6 * Margin;
            int yOffsetPedigreeBottom;
            // x
            var xOffsetStart = 4 * Margin;
            if (hView)
            {
                var pedigreeWidth = controlHeightWithMargin * ((1 << (displayedGenerations / 2 + 1)) - 1);
                xOffsetStart += pedigreeWidth / 2;
                var pedigreeHeightInCreatures = displayedGenerations < 5 ? 3 : (1 << ((displayedGenerations - 1) / 2 + 1)) - 1;
                yOffsetPedigreeBottom = yOffsetOriginCreature + pedigreeHeightInCreatures * controlHeightWithMargin;
                yOffsetOriginCreature += controlHeightWithMargin * (pedigreeHeightInCreatures / 2);
            }
            else
            {
                xOffsetStart += displayedGenerations < 2 ? 0 : (PedigreeCreatureCompact.ControlWidth + pedigreeControlHorizontalMargin) * (1 << (displayedGenerations - 2));
                yOffsetOriginCreature += controlHeightWithMargin * Math.Max(2, displayedGenerations - 1); // don't shift few generations to the top
                yOffsetPedigreeBottom = yOffsetOriginCreature + controlHeightWithMargin;
            }

            var xOffsetParents = xOffsetStart / 2;
            if (xOffsetStart < MinXPosCreature) xOffsetStart = MinXPosCreature;
            var xLowest = CreateOffspringParentsCompact(creature, xOffsetStart, yOffsetOriginCreature, autoScrollPosX, autoScrollPosY,
                false, displayedGenerations, xOffsetParents, lines, pedigreeControls, int.MaxValue, true, highlightInheritanceStatIndex, hView);
            var moveToLeft = xLowest - 2 * Margin;
            var maxMoveToLeft = Math.Max(0, xOffsetStart - MinXPosCreature);
            if (moveToLeft > maxMoveToLeft) moveToLeft = maxMoveToLeft;
            if (moveToLeft > 0)
                CompactViewLeftAlign(pedigreeControls, lines, moveToLeft);

            return yOffsetPedigreeBottom + 2 * Margin;
        }

        private static int CreateOffspringParentsCompact(Creature creature, int x, int y, int autoScrollPosX, int autoScrollPosY, bool onlyDrawParents, int generations, int xOffsetParent,
                    List<int[]>[] lines, List<Control> pcs, int xLowest, bool highlightCreature, int highlightStatIndex, bool hView, int hViewRotation = 0, int highlightMotherLine = 0, int highlightFatherLine = 0)
        {
            var (motherInheritance, fatherInheritance) =
                CreateParentsChildCompact(creature, x, y, autoScrollPosX, autoScrollPosY, xOffsetParent,
                    lines, pcs, onlyDrawParents, highlightCreature, highlightStatIndex, hView, hViewRotation, out Point locationMother, out Point locationFather, highlightMotherLine, highlightFatherLine);

            if (creature.Mother != null && xLowest > locationMother.X) xLowest = locationMother.X;
            if (creature.Father != null && xLowest > locationFather.X) xLowest = locationFather.X;

            if (--generations < 2) return xLowest;
            bool halfXOffset = !hView || (generations % 2) == 1;
            if (creature.Mother != null)
            {
                var xLowestNew = CreateOffspringParentsCompact(creature.Mother, locationMother.X, locationMother.Y, autoScrollPosX, autoScrollPosY,
                    true, generations, halfXOffset ? xOffsetParent / 2 : xOffsetParent, lines, pcs, xLowest, false, highlightStatIndex, hView, (hViewRotation + 3) % 4, motherInheritance.maternalInheritance, motherInheritance.paternalInheritance);
                if (xLowest > xLowestNew) xLowest = xLowestNew;
            }
            if (creature.Father != null)
            {
                var xLowestNew = CreateOffspringParentsCompact(creature.Father, locationFather.X, locationFather.Y, autoScrollPosX, autoScrollPosY,
                    true, generations, halfXOffset ? xOffsetParent / 2 : xOffsetParent, lines, pcs, xLowest, false, highlightStatIndex, hView, (hViewRotation + 1) % 4, fatherInheritance.maternalInheritance, fatherInheritance.paternalInheritance);
                if (xLowest > xLowestNew) xLowest = xLowestNew;
            }

            return xLowest;
        }

        /// <summary>
        /// Creates the controls that display a creature and its parents.
        /// </summary>
        /// <returns>True if stat inheritance line is continued (maternal, paternal).</returns>
        private static ((int maternalInheritance, int paternalInheritance) motherInheritance,
            (int maternalInheritance, int paternalInheritance) fatherInheritance)
            CreateParentsChildCompact(Creature creature, int x, int y, int autoScrollX, int autoScrollY, int xOffsetParents,
                List<int[]>[] lines, List<Control> pcs, bool onlyDrawParents, bool highlightCreature, int highlightStatIndex, bool hView, int hViewRotation, out Point locationMother, out Point locationFather, int highlightMotherLine = 0, int highlightFatherLine = 0)
        {
            locationMother = Point.Empty;
            locationFather = Point.Empty;
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
                locationMother = hView ? RotateOffset(x, y, -xOffsetParents, 0, hViewRotation) : new Point(x - xOffsetParents, yParents);
                var c = new PedigreeCreatureCompact(creature.Mother, highlightStatIndex: highlightMotherLine != 0 ? highlightStatIndex : -1)
                {
                    Location = locationMother
                };
                locationMother.Offset(-autoScrollX, -autoScrollY);
                pcs.Add(c);
                if (highlightMotherLine != 0 && highlightStatIndex != -1)
                    statInheritanceMother = c.PossibleStatInheritance(highlightStatIndex);
            }
            // father
            if (creature.Father != null)
            {
                locationFather = hView ? RotateOffset(x, y, xOffsetParents, 0, hViewRotation) : new Point(x + xOffsetParents, yParents);
                var c = new PedigreeCreatureCompact(creature.Father, highlightStatIndex: highlightFatherLine != 0 ? highlightStatIndex : -1)
                {
                    Location = locationFather
                };
                locationFather.Offset(-autoScrollX, -autoScrollY);
                pcs.Add(c);
                if (highlightFatherLine != 0 && highlightStatIndex != -1)
                    statInheritanceFather = c.PossibleStatInheritance(highlightStatIndex);
            }

            // lines
            if (hView)
            {
                //  M──O──F

                // keep normal lines black to make them more visible in this mode
                if (highlightMotherLine == 0) highlightMotherLine = 1;
                if (highlightFatherLine == 0) highlightFatherLine = 1;

                var halfControlWidth = PedigreeCreatureCompact.ControlWidth / 2 - 1;
                var xCenter = xLine + halfControlWidth;
                var yCenter = yLine + PedigreeCreatureCompact.ControlHeight / 2;

                var start = RotateOffset(xCenter, yCenter, -xOffsetParents + halfControlWidth, 0, hViewRotation);
                var end = RotateOffset(xCenter, yCenter, -halfControlWidth, 0, hViewRotation);
                lines[1].Add(new[] { start.X, start.Y, end.X, end.Y, highlightMotherLine });

                start = RotateOffset(xCenter, yCenter, xOffsetParents - halfControlWidth, 0, hViewRotation);
                end = RotateOffset(xCenter, yCenter, halfControlWidth, 0, hViewRotation);
                lines[1].Add(new[] { start.X, start.Y, end.X, end.Y, highlightFatherLine });
            }
            else
            {
                //  M──┬──F
                //     O
                var yLineHorizontal = yLine - YOffsetLineCompact;
                var xCenterOffspring = xLine + PedigreeCreatureCompact.ControlWidth / 2;
                lines[2].Add(new[]
                {
                    xLine - xOffsetParents + PedigreeCreatureCompact.ControlWidth, yLineHorizontal, xCenterOffspring,
                    yLineHorizontal, highlightMotherLine
                });
                lines[2].Add(new[]
                    {xLine + xOffsetParents, yLineHorizontal, xCenterOffspring, yLineHorizontal, highlightFatherLine });
                lines[1].Add(new[] { xCenterOffspring, yLineHorizontal, xCenterOffspring, yLine, Math.Max(highlightMotherLine, highlightFatherLine) });
            }

            return (statInheritanceMother, statInheritanceFather);
        }

        /// <summary>
        /// Returns rotated offset coordinates.
        /// </summary>
        /// <param name="xOrigin"></param>
        /// <param name="yOrigin"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="rotation">0: no, 1: 90 deg clockwise, 2: 180 deg, 3: 270 deg clockwise.</param>
        private static Point RotateOffset(int xOrigin, int yOrigin, int xOffset, int yOffset, int rotation = 0)
        {
            switch (rotation)
            {
                case 0: return new Point(xOrigin + xOffset, yOrigin + yOffset);
                case 1: return new Point(xOrigin - yOffset, yOrigin + xOffset);
                case 2: return new Point(xOrigin - xOffset, yOrigin - yOffset);
                case 3: return new Point(xOrigin + yOffset, yOrigin - xOffset);
            }

            throw new ArgumentException();
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
            CreateParentsChild(creature, lines, pedigreeControls, LeftBorder + pedigreeElementWidth + Margin, 60, autoScrollPosX, autoScrollPosY, enabledColorRegions, true, true);

            // create ancestors
            if (creature.Mother != null
                && CreateParentsChild(creature.Mother, lines, pedigreeControls, LeftBorder, 20, autoScrollPosX, autoScrollPosY, enabledColorRegions))
            {
                lines[1].Add(new[]
                {
                    LeftBorder + pedigreeElementWidth, yCenterOfCreatureParent,
                    LeftBorder + pedigreeElementWidth + Margin, yCenterOfCreatureParent, 0
                });
            }
            if (creature.Father != null
                && CreateParentsChild(creature.Father, lines, pedigreeControls, LeftBorder + 2 * (pedigreeElementWidth + Margin), 20, autoScrollPosX, autoScrollPosY, enabledColorRegions))
            {
                lines[1].Add(new[]
                {
                    LeftBorder + 2 * pedigreeElementWidth + 2 * Margin, yCenterOfCreatureParent,
                    LeftBorder + 2 * pedigreeElementWidth + Margin, yCenterOfCreatureParent + 80, 0
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
