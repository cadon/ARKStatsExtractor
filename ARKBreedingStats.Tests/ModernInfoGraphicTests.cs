using System.Drawing;
using System.IO;
using System.Linq;
using ARKBreedingStats.InfoGraphic;
using ARKBreedingStats.InfoGraphic.Modern;
using ARKBreedingStats.Library;
using ARKBreedingStats.values;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ARKBreedingStats.Tests
{
    /// <summary>
    /// Smoke tests for the modern info graphic renderer.
    /// These render real creatures, so they also cover the layout math and the vector stat icons.
    /// </summary>
    [TestClass]
    public class ModernInfoGraphicTests
    {
        private static CreatureCollection _cc;
        private static Creature _creature;

        [ClassInitialize]
        public static void LoadGameValues(TestContext context)
        {
            // the app downloads the mods manifest before loading values, an empty one is enough here
            Values.V.SetModsManifest(null);
            Values.V.LoadValues(true, out _, out _);
            if (!(Values.V.Species?.Count > 0)) return;

            _cc = new CreatureCollection
            {
                Game = Ark.Asa,
                serverMultipliers = Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.Official)
            };
            Values.V.ApplyMultipliers(_cc);

            _creature = DummyCreatures.CreateCreatures(1)?.FirstOrDefault();
            _creature?.RecalculateCreatureValues(_cc.wildLevelStep);
        }

        /// <summary>
        /// Skips a test when the values files are not available in the test output.
        /// </summary>
        private static void RequireGameValues()
        {
            if (_creature == null)
                Assert.Inconclusive("game values could not be loaded, skipping render test");
        }

        [TestMethod]
        public void RenderAsync_NullCreature_ReturnsNull()
        {
            var bmp = ModernInfoGraphic.RenderAsync(null, null, new ModernInfoGraphicSettings())
                .GetAwaiter().GetResult();

            Assert.IsNull(bmp, "a creature without species cannot be rendered");
        }

        [TestMethod]
        public void RenderAsync_DefaultSettings_ReturnsBitmapOfRequestedWidth()
        {
            RequireGameValues();

            var settings = new ModernInfoGraphicSettings();
            using var bmp = ModernInfoGraphic.RenderAsync(_creature, _cc, settings).GetAwaiter().GetResult();

            Assert.IsNotNull(bmp, "renderer returned no image");
            Assert.AreEqual(settings.Width, bmp.Width, "the card must have the configured width");

            // A chat client renders an image 1:1 only while it fits the preview box, roughly 350 px
            // across, and downscales it otherwise. A downscaled render is what makes the text look
            // soft, so the default card has to stay inside that box.
            Assert.IsTrue(bmp.Width <= 400,
                $"the default card must fit an inline preview unscaled, was {bmp.Width} px wide");
            Assert.IsTrue(bmp.Width > bmp.Height, "the card is landscape");
        }

        [TestMethod]
        public void RenderAsync_CustomWidth_ScalesTheCard()
        {
            RequireGameValues();

            using var small = ModernInfoGraphic
                .RenderAsync(_creature, _cc, new ModernInfoGraphicSettings { Width = 400 })
                .GetAwaiter().GetResult();
            using var large = ModernInfoGraphic
                .RenderAsync(_creature, _cc, new ModernInfoGraphicSettings { Width = 1200 })
                .GetAwaiter().GetResult();

            Assert.AreEqual(400, small.Width);
            Assert.AreEqual(1200, large.Width);
            // the aspect ratio only depends on the number of stat rows, not on the width
            Assert.AreEqual(small.Height / (double)small.Width, large.Height / (double)large.Width, 0.02,
                "the aspect ratio must not change with the width");
        }

        [TestMethod]
        public void RenderAsync_AllValueDisplayModes_Render()
        {
            RequireGameValues();

            foreach (var mode in new[]
                     {
                         ModernInfoGraphicSettings.StatValueDisplays.Current,
                         ModernInfoGraphicSettings.StatValueDisplays.Breeding,
                         ModernInfoGraphicSettings.StatValueDisplays.Both
                     })
            {
                using var bmp = ModernInfoGraphic
                    .RenderAsync(_creature, _cc, new ModernInfoGraphicSettings { ValueDisplay = mode })
                    .GetAwaiter().GetResult();
                Assert.IsNotNull(bmp, $"value display mode {mode} did not render");
            }
        }

        [TestMethod]
        public void RenderAsync_AllRegionNameModes_Render()
        {
            RequireGameValues();

            foreach (var mode in new[]
                     {
                         ModernInfoGraphicSettings.ColorRegionNameDisplays.Off,
                         ModernInfoGraphicSettings.ColorRegionNameDisplays.WithoutArtwork,
                         ModernInfoGraphicSettings.ColorRegionNameDisplays.Always
                     })
            {
                using var bmp = ModernInfoGraphic
                    .RenderAsync(_creature, _cc, new ModernInfoGraphicSettings { RegionNames = mode })
                    .GetAwaiter().GetResult();
                Assert.IsNotNull(bmp, $"region name mode {mode} did not render");
            }
        }

        [TestMethod]
        public void RenderAsync_NamedRegions_NeverShrinkTheCard()
        {
            RequireGameValues();

            using var compact = ModernInfoGraphic.RenderAsync(_creature, _cc, new ModernInfoGraphicSettings
            { RegionNames = ModernInfoGraphicSettings.ColorRegionNameDisplays.Off }).GetAwaiter().GetResult();
            using var named = ModernInfoGraphic.RenderAsync(_creature, _cc, new ModernInfoGraphicSettings
            { RegionNames = ModernInfoGraphicSettings.ColorRegionNameDisplays.Always }).GetAwaiter().GetResult();

            // Names reflow the chip strip into a grid. A species with few regions still fits on one
            // row, so the card only grows once the regions outnumber the grid columns, but it must
            // never come out shorter than the compact strip.
            Assert.IsTrue(named.Height >= compact.Height,
                $"named regions must not shrink the card, got {named.Height} vs {compact.Height}");
            Assert.AreEqual(compact.Width, named.Width, "the width must not change");
        }

        [TestMethod]
        public void RenderAsync_NamedRegionsWithColorsHidden_AddsNoHeight()
        {
            RequireGameValues();

            // the region name setting must not reserve space when the colors are switched off
            using var withNames = ModernInfoGraphic.RenderAsync(_creature, _cc, new ModernInfoGraphicSettings
            {
                ShowColors = false,
                RegionNames = ModernInfoGraphicSettings.ColorRegionNameDisplays.Always
            }).GetAwaiter().GetResult();
            using var withoutNames = ModernInfoGraphic.RenderAsync(_creature, _cc, new ModernInfoGraphicSettings
            {
                ShowColors = false,
                RegionNames = ModernInfoGraphicSettings.ColorRegionNameDisplays.Off
            }).GetAwaiter().GetResult();

            Assert.AreEqual(withoutNames.Height, withNames.Height);
        }

        [TestMethod]
        public void RenderAsync_BothThemes_Render()
        {
            RequireGameValues();

            foreach (var theme in new[]
                     {
                         ModernInfoGraphicSettings.ModernThemes.Dark,
                         ModernInfoGraphicSettings.ModernThemes.Light
                     })
            {
                using var bmp = ModernInfoGraphic
                    .RenderAsync(_creature, _cc, new ModernInfoGraphicSettings { Theme = theme })
                    .GetAwaiter().GetResult();
                Assert.IsNotNull(bmp, $"theme {theme} did not render");
            }
        }

        [TestMethod]
        public void RenderAsync_WithoutCreatureCollection_StillRenders()
        {
            RequireGameValues();

            // the pedigree and the extractor can call this before a collection exists
            using var bmp = ModernInfoGraphic.RenderAsync(_creature, null, new ModernInfoGraphicSettings())
                .GetAwaiter().GetResult();

            Assert.IsNotNull(bmp, "the graphic should also be rendered without explicit creature collection (values may be wrong then)");
        }

        [TestMethod]
        public void RenderAsync_AllOptionalPartsHidden_StillRenders()
        {
            RequireGameValues();

            using var bmp = ModernInfoGraphic.RenderAsync(_creature, _cc, new ModernInfoGraphicSettings
            {
                ShowColors = false,
                ShowMutations = false,
                ShowSpecies = false,
                ShowStatValues = false,
                TransparentBackground = true
            }).GetAwaiter().GetResult();

            Assert.IsNotNull(bmp, "hiding every optional part must not break the layout");
        }

        [TestMethod]
        public void InfoGraphicAsync_DispatchesToTheSelectedStyle()
        {
            RequireGameValues();

            var originalStyle = Properties.Settings.Default.InfoGraphicStyle;
            try
            {
                Properties.Settings.Default.InfoGraphicStyle = (int)InfoGraphic.InfoGraphicStyles.Classic;
                using var classic = _creature.InfoGraphicAsync(_cc).GetAwaiter().GetResult();

                Properties.Settings.Default.InfoGraphicStyle = (int)InfoGraphic.InfoGraphicStyles.Modern;
                using var modern = _creature.InfoGraphicAsync(_cc).GetAwaiter().GetResult();

                Assert.IsNotNull(classic, "the classic renderer must still work");
                Assert.IsNotNull(modern, "the modern renderer must be reachable through the router");
                // both are landscape, they are told apart by which size setting drives them
                Assert.AreEqual(Properties.Settings.Default.InfoGraphicHeight, classic.Height,
                    "the classic graphic is sized by InfoGraphicHeight");
                Assert.AreEqual(Properties.Settings.Default.InfoGraphicModernWidth, modern.Width,
                    "the modern graphic is sized by InfoGraphicModernWidth");
            }
            finally
            {
                Properties.Settings.Default.InfoGraphicStyle = originalStyle;
            }
        }

        [TestMethod]
        public void InfoGraphicStyle_DefaultsToClassic()
        {
            var defaultValue = Properties.Settings.Default.Properties["InfoGraphicStyle"].DefaultValue?.ToString();

            Assert.AreEqual(((int)InfoGraphic.InfoGraphicStyles.Classic).ToString(), defaultValue,
                "the modern style is opt in, a fresh profile must keep the classic graphic");
        }

        [TestMethod]
        public void RenderAsync_CreatureWithMissingStatArrays_StillRenders()
        {
            RequireGameValues();

            // Creature stat arrays use DefaultValueHandling.Ignore, so a creature saved without
            // domesticate levels, mutated levels or recalculated values comes back from the library
            // with those arrays null. Stitching a whole library hits such creatures.
            var sparse = new Creature(_creature.Species, "sparse", sex: Sex.Female)
            {
                levelsWild = _creature.levelsWild,
                levelsDom = null,
                levelsMutated = null,
                valuesBreeding = null,
                valuesCurrent = null,
                colors = _creature.colors
            };

            using var bmp = ModernInfoGraphic.RenderAsync(sparse, _cc, new ModernInfoGraphicSettings())
                .GetAwaiter().GetResult();

            Assert.IsNotNull(bmp, "a creature with missing stat arrays must still render");
        }

        [TestMethod]
        public void RenderAsync_CreatureWithShortArrays_StillRenders()
        {
            RequireGameValues();

            // an array shorter than the stat count must be read defensively, not indexed blindly
            var truncated = new Creature(_creature.Species, "short", sex: Sex.Male)
            {
                levelsWild = new int[Stats.StatsCount],
                levelsDom = new int[2],
                levelsMutated = new int[1],
                valuesBreeding = new double[3],
                valuesCurrent = new double[0],
                colors = new byte[2]
            };

            using var bmp = ModernInfoGraphic.RenderAsync(truncated, _cc, new ModernInfoGraphicSettings
            { RegionNames = ModernInfoGraphicSettings.ColorRegionNameDisplays.Always })
                .GetAwaiter().GetResult();

            Assert.IsNotNull(bmp, "short stat arrays must not throw");
        }

        [TestMethod]
        public void CreatureLevel_WithoutDomesticateLevels_DoesNotThrow()
        {
            RequireGameValues();

            var creature = new Creature(_creature.Species, "no dom levels", sex: Sex.Unknown)
            {
                levelsWild = new int[Stats.StatsCount],
                levelsDom = null
            };
            creature.levelsWild[Stats.Torpidity] = 41;

            Assert.AreEqual(42, creature.Level, "level is the torpidity level plus one when there are no dom levels");
        }

        [TestMethod]
        public void ClassicInfoGraphic_CreatureWithoutWildLevels_ReturnsNullInsteadOfThrowing()
        {
            RequireGameValues();

            // levelsWild is never filled in by InitializeArrays and stays null for a creature that
            // was never extracted. The classic layout reads it unconditionally.
            var bmp = NotExtracted().InfoGraphicAsync(_cc, CreatureInfoGraphic.ClassicSettingsFromUserSettings())
                .GetAwaiter().GetResult();

            Assert.IsNull(bmp, "the classic graphic should decline rather than throw");
        }

        [TestMethod]
        public void EveryDisplayToggle_ChangesTheOutput()
        {
            RequireGameValues();

            // A creature exercising every toggle: it has a name, a generation, mutated levels,
            // domesticate levels and colors, so flipping any option has something to act on.
            var creature = new Creature(_creature.Species, "Toggle Test", sex: Sex.Female)
            {
                levelsWild = new int[Stats.StatsCount],
                levelsDom = new int[Stats.StatsCount],
                levelsMutated = new int[Stats.StatsCount],
                colors = _creature.colors,
                generation = 3,
                isBred = true
            };
            creature.levelsWild[Stats.Health] = 22;
            creature.levelsWild[Stats.Torpidity] = 40;
            creature.levelsDom[Stats.Health] = 5;
            creature.levelsMutated[Stats.Health] = 4;
            creature.RecalculateCreatureValues(_cc.wildLevelStep);

            var baseline = RenderBytes(creature, new ModernInfoGraphicSettings());

            var variants = new (string Option, ModernInfoGraphicSettings Settings)[]
            {
                ("ShowCreatureName", new ModernInfoGraphicSettings { ShowCreatureName = false }),
                ("ShowSpecies", new ModernInfoGraphicSettings { ShowSpecies = false }),
                ("ShowGeneration", new ModernInfoGraphicSettings { ShowGeneration = false }),
                ("ShowMaxWildLevel", new ModernInfoGraphicSettings { ShowMaxWildLevel = true }),
                ("SumWildAndMutatedLevels", new ModernInfoGraphicSettings { SumWildAndMutatedLevels = true }),
                ("ShowStatValues", new ModernInfoGraphicSettings { ShowStatValues = false }),
                ("ShowMutations", new ModernInfoGraphicSettings { ShowMutations = false }),
                ("ShowColors", new ModernInfoGraphicSettings { ShowColors = false }),
                ("BarsByLevelQuality", new ModernInfoGraphicSettings { BarsByLevelQuality = false }),
                ("TransparentBackground", new ModernInfoGraphicSettings { TransparentBackground = true }),
                ("AccentColor", new ModernInfoGraphicSettings { AccentColor = Color.Orange }),
                ("AccentFromCreature", new ModernInfoGraphicSettings { AccentFromCreature = true }),
                ("BackgroundColor", new ModernInfoGraphicSettings { BackgroundColor = Color.FromArgb(0x3A, 0x10, 0x4A) }),
                ("ArtworkHalo", new ModernInfoGraphicSettings { ArtworkHalo = false })
            };

            foreach (var variant in variants)
                CollectionAssert.AreNotEqual(baseline, RenderBytes(creature, variant.Settings),
                    $"{variant.Option} made no difference to the rendered card, it is not wired up");
        }

        private static byte[] RenderBytes(Creature creature, ModernInfoGraphicSettings settings)
        {
            using var bmp = ModernInfoGraphic.RenderAsync(creature, _cc, settings).GetAwaiter().GetResult();
            Assert.IsNotNull(bmp);
            using var stream = new MemoryStream();
            bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }

        [TestMethod]
        public void SpeciesSuffixes_CanBeExcluded()
        {
            RequireGameValues();

            var species = _creature.Species;
            var originalVariant = species.VariantInfo;
            try
            {
                // a modded species carries bracketed variant and mod suffixes, e.g.
                // "Veilwyn (Companion, LostColony) (ASA)", which are noise in a single mod library
                species.VariantInfo = " (Companion, LostColony)";
                var creature = RenderableCreature("Veilwyn");

                var withSuffixes = RenderBytes(creature,
                    new ModernInfoGraphicSettings { ShowSpeciesSuffixes = true });
                var withoutSuffixes = RenderBytes(creature,
                    new ModernInfoGraphicSettings { ShowSpeciesSuffixes = false });

                CollectionAssert.AreNotEqual(withSuffixes, withoutSuffixes,
                    "excluding the bracketed variant and mod should change the card");
            }
            finally
            {
                species.VariantInfo = originalVariant;
            }
        }

        [TestMethod]
        public void LongSpeciesName_IsShortenedInsteadOfReflowingTheCard()
        {
            RequireGameValues();

            var species = _creature.Species;
            var originalVariant = species.VariantInfo;
            try
            {
                // unnamed, so the species name is the title
                var creature = RenderableCreature(null);

                species.VariantInfo = string.Empty;
                var plainHeight = RenderHeight(creature);

                species.VariantInfo = " (An Extremely Long Modded Variant Description That Goes On)";
                var longHeight = RenderHeight(creature);

                // the title gives up its variant suffix, which moves to the detail line, so the
                // header stays two lines and the card keeps its size
                Assert.AreEqual(plainHeight, longHeight,
                    "a long species name should be shortened for the title, not reflow the whole card");
            }
            finally
            {
                species.VariantInfo = originalVariant;
            }
        }

        [TestMethod]
        public void LongestPossibleCreatureName_StillRenders()
        {
            RequireGameValues();

            // the game trims names to this length, so this is the worst case that can reach us.
            // W is about the widest glyph there is.
            var creature = RenderableCreature(new string('W', Ark.MaxCreatureNameLength));
            var settings = new ModernInfoGraphicSettings();

            using var bmp = ModernInfoGraphic.RenderAsync(creature, _cc, settings).GetAwaiter().GetResult();

            Assert.IsNotNull(bmp);
            Assert.AreEqual(settings.Width, bmp.Width, "a long name must not widen the card");
        }

        private static Creature RenderableCreature(string name)
        {
            var creature = new Creature(_creature.Species, name, sex: Sex.Female)
            {
                levelsWild = new int[Stats.StatsCount],
                levelsDom = new int[Stats.StatsCount],
                levelsMutated = new int[Stats.StatsCount],
                colors = _creature.colors
            };
            creature.levelsWild[Stats.Health] = 22;
            creature.levelsWild[Stats.Torpidity] = 40;
            creature.RecalculateCreatureValues(_cc.wildLevelStep);
            return creature;
        }

        private static int RenderHeight(Creature creature)
        {
            using var bmp = ModernInfoGraphic.RenderAsync(creature, _cc, new ModernInfoGraphicSettings())
                .GetAwaiter().GetResult();
            Assert.IsNotNull(bmp);
            return bmp.Height;
        }

        [TestMethod]
        public void StatRowText_ListsLevelsInGameOrder()
        {
            RequireGameValues();

            var creature = RenderableCreature("Order");
            creature.levelsWild[Stats.Health] = 56;
            creature.levelsMutated[Stats.Health] = 254;
            creature.levelsDom[Stats.Health] = 7;

            var levelsOnly = new ModernInfoGraphicSettings { ShowStatValues = false };

            // wild, mutated, domesticated. This is the order the game shows and the order behind
            // the classic graphic's W | M | D columns.
            Assert.AreEqual("(56 | 254 | 7)",
                ModernInfoGraphic.StatRowText(creature, Stats.Health, levelsOnly, showMutatedLevels: true));

            // without a mutated column it is wild then domesticated
            Assert.AreEqual("(56 | 7)",
                ModernInfoGraphic.StatRowText(creature, Stats.Health, levelsOnly, showMutatedLevels: false));

            // summed, the mutated levels fold into the wild ones and lose their column
            var summed = new ModernInfoGraphicSettings { ShowStatValues = false, SumWildAndMutatedLevels = true };
            Assert.AreEqual("(310 | 7)",
                ModernInfoGraphic.StatRowText(creature, Stats.Health, summed, showMutatedLevels: false));
        }

        [TestMethod]
        public void ContrastingText_PicksTheMoreReadableInk()
        {
            Assert.AreEqual(Color.White, DrawingPrimitives.ContrastingText(Color.Black));
            Assert.AreEqual(Color.Black, DrawingPrimitives.ContrastingText(Color.White));
            // the crossover on a grey ramp sits at about 117
            Assert.AreEqual(Color.White, DrawingPrimitives.ContrastingText(Color.FromArgb(110, 110, 110)));
            Assert.AreEqual(Color.Black, DrawingPrimitives.ContrastingText(Color.FromArgb(125, 125, 125)));

            // Every ARK color must reach the WCAG AA ratio for normal text. Utils.ForeColor only
            // manages 3.14:1 on magenta, because its weights and its lack of gamma correction
            // misjudge saturated colors, and those are exactly what creature regions use.
            for (byte colorId = 1; colorId < 255; colorId++)
            {
                var color = species.CreatureColors.CreatureColor(colorId);
                if (color.A == 0) continue;

                var ink = DrawingPrimitives.ContrastingText(color);
                Assert.IsTrue(ContrastRatio(color, ink) >= 4.5,
                    $"color id {colorId} ({color}) only reaches {ContrastRatio(color, ink):F2}:1");
            }

            double ContrastRatio(Color a, Color b)
            {
                var la = RelativeLuminance(a);
                var lb = RelativeLuminance(b);
                return (System.Math.Max(la, lb) + 0.05) / (System.Math.Min(la, lb) + 0.05);
            }

            double RelativeLuminance(Color c)
            {
                double Channel(int v)
                {
                    var n = v / 255.0;
                    return n <= 0.03928 ? n / 12.92 : System.Math.Pow((n + 0.055) / 1.055, 2.4);
                }
                return 0.2126 * Channel(c.R) + 0.7152 * Channel(c.G) + 0.0722 * Channel(c.B);
            }
        }

        [TestMethod]
        public void NonContiguousColorRegions_RenderInBothChipLayouts()
        {
            RequireGameValues();

            // Species commonly enable a scattered set of regions, e.g. 0, 4 and 5. The chips carry
            // the region index precisely because the third chip is then not region two, and the
            // index has to come from the region list rather than from the loop counter.
            var species = _creature.Species;
            var originalRegions = species.EnabledColorRegions;
            try
            {
                species.EnabledColorRegions = new[] { true, false, false, false, true, true };
                var creature = RenderableCreature("Scattered");
                creature.colors = new byte[] { 10, 0, 0, 0, 20, 30 };

                foreach (var mode in new[]
                         {
                             ModernInfoGraphicSettings.ColorRegionNameDisplays.Off,
                             ModernInfoGraphicSettings.ColorRegionNameDisplays.Always
                         })
                {
                    var settings = new ModernInfoGraphicSettings { RegionNames = mode };
                    using var bmp = ModernInfoGraphic.RenderAsync(creature, _cc, settings)
                        .GetAwaiter().GetResult();

                    Assert.IsNotNull(bmp, $"region name mode {mode} did not render");
                    Assert.AreEqual(settings.Width, bmp.Width);
                }
            }
            finally
            {
                species.EnabledColorRegions = originalRegions;
            }
        }

        [TestMethod]
        public void CustomBackground_KeepsTheTextReadable()
        {
            RequireGameValues();

            // A light background on the dark theme would leave white text on white unless the text
            // colors follow the background. Sample the card's own background pixel and the darkest
            // text pixel and check they are far enough apart.
            foreach (var background in new[]
                     {
                         Color.FromArgb(0xF2, 0xE8, 0xD0), // light
                         Color.FromArgb(0x3A, 0x10, 0x4A), // dark
                         Color.FromArgb(0x80, 0x80, 0x80)  // mid grey
                     })
            {
                using var bmp = ModernInfoGraphic.RenderAsync(_creature, _cc,
                        new ModernInfoGraphicSettings { BackgroundColor = background })
                    .GetAwaiter().GetResult();

                // a point inside the card that is background, just under the top border
                var cardBackground = bmp.GetPixel(bmp.Width / 2, 6);
                var expectedText = DrawingPrimitives.ContrastingText(background);

                Assert.AreNotEqual(Brightness(cardBackground) > 127, Brightness(expectedText) > 127,
                    $"background {background} should have produced contrasting text");
                Assert.IsTrue(System.Math.Abs(Brightness(cardBackground) - Brightness(background)) < 40,
                    $"the card background should be the picked color, got {cardBackground} for {background}");
            }

            double Brightness(Color c) => c.R * .3 + c.G * .59 + c.B * .11;
        }

        [TestMethod]
        public void BothStyles_DeclineTheSameCreature()
        {
            RequireGameValues();

            // A creature without wild levels should not create an infographic.
            var notExtracted = NotExtracted();

            var classic = notExtracted
                .InfoGraphicAsync(_cc, CreatureInfoGraphic.ClassicSettingsFromUserSettings())
                .GetAwaiter().GetResult();
            var modern = ModernInfoGraphic.RenderAsync(notExtracted, _cc, new ModernInfoGraphicSettings())
                .GetAwaiter().GetResult();

            Assert.IsNull(classic, "the classic style should decline it");
            Assert.IsNull(modern, "the modern style should decline it too");
        }

        [TestMethod]
        public void Stitching_SkipsCreaturesThatCannotBeDrawn()
        {
            RequireGameValues();

            var creatures = new[] { _creature, NotExtracted() };
            var originalStyle = Properties.Settings.Default.InfoGraphicStyle;

            foreach (var style in new[] { InfoGraphic.InfoGraphicStyles.Classic, InfoGraphic.InfoGraphicStyles.Modern })
            {
                var outputPath = Path.Combine(Path.GetTempPath(), $"asb_stitch_test_{System.Guid.NewGuid():N}.png");
                try
                {
                    Properties.Settings.Default.InfoGraphicStyle = (int)style;

                    var placed = Stitching.CreateStitchedImages(creatures, _cc, outputPath, 2000,
                        System.Drawing.Color.Black).GetAwaiter().GetResult();

                    Assert.AreEqual(1, placed, $"{style}: the creature without stat levels should be skipped");
                    Assert.IsTrue(File.Exists(outputPath), $"{style}: the sheet should still have been written");
                }
                finally
                {
                    Properties.Settings.Default.InfoGraphicStyle = originalStyle;
                    if (File.Exists(outputPath)) File.Delete(outputPath);
                }
            }
        }

        /// <summary>
        /// A creature as it comes back from the library when it was never extracted: no wild levels
        /// and no calculated values, because those arrays are not serialized when unset.
        /// </summary>
        private static Creature NotExtracted() =>
            new Creature(_creature.Species, "not extracted", sex: Sex.Female)
            {
                levelsWild = null,
                valuesBreeding = null
            };

        [TestMethod]
        public void SexSymbols_EverySex_HasAVectorSymbol()
        {
            using var bmp = new Bitmap(32, 32);
            using var g = Graphics.FromImage(bmp);

            // the font glyphs are too thin at badge size, every sex must have a drawn symbol so
            // they all carry the same weight
            foreach (Sex sex in System.Enum.GetValues(typeof(Sex)))
                Assert.IsTrue(SexSymbols.Draw(g, sex, new RectangleF(0, 0, 32, 32), Color.White),
                    $"{sex} falls back to the text glyph");
        }

        [TestMethod]
        public void StatIcons_EveryStatIndex_DrawsWithoutError()
        {
            using var bmp = new Bitmap(32, 32);
            using var g = Graphics.FromImage(bmp);

            // also covers an index outside the range, which must fall back rather than throw
            for (var si = -1; si <= Stats.StatsCount; si++)
                StatIcons.Draw(g, si, new RectangleF(0, 0, 32, 32), Color.White);
        }

        [TestMethod]
        public void StatIcons_EveryStatIndex_PaintsARecognisableShape()
        {
            // Guards against path edits that silently degenerate: a self intersecting outline
            // filled with the wrong fill mode comes out either empty or as a solid blob.
            const int size = 40;
            for (var si = 0; si < Stats.StatsCount; si++)
            {
                using var bmp = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    StatIcons.Draw(g, si, new RectangleF(0, 0, size, size), Color.White);
                }

                var painted = 0;
                for (var x = 0; x < size; x++)
                    for (var y = 0; y < size; y++)
                        if (bmp.GetPixel(x, y).A > 128)
                            painted++;

                var coverage = painted / (double)(size * size);
                Assert.IsTrue(coverage > 0.04, $"stat {si} icon painted almost nothing ({coverage:P1})");
                Assert.IsTrue(coverage < 0.80, $"stat {si} icon filled almost the whole box ({coverage:P1})");
            }
        }
    }
}
