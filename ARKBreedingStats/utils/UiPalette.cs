using System;
using System.ComponentModel;
using System.Drawing;

namespace ARKBreedingStats.utils
{
    /// <summary>How a palette color is applied in the UI.</summary>
    internal enum ColorRole { Background, Foreground, Border, Line }

    /// <summary>Which UI area uses this color.</summary>
    internal enum ColorContext
    {
        Generic,
        Extractor,
        LibraryGrid,
        LibraryFilter,
        Pedigree,
        BreedingPlan,
        TimerList,
        StatIO,
        CreatureInfo,
    }

    /// <summary>
    /// Marks a <see cref="UiPalette"/> property with rendering hints so the
    /// preview panel can show a contextual mockup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class PreviewHintAttribute : Attribute
    {
        public ColorRole Role { get; }
        public ColorContext Context { get; }

        public PreviewHintAttribute(ColorRole role, ColorContext context = ColorContext.Generic)
        {
            Role = role;
            Context = context;
        }
    }

    /// <summary>
    /// Defines all semantic color slots used by the application UI.
    /// Each combination of AppTheme (Light/Dark) and AsbColorMode (accessibility)
    /// has its own palette instance with hand-picked colors.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    internal record UiPalette
    {
        // --- UI Semantic Status ---

        [Category("Status"), DisplayName("Success")]
        [Description("Positive outcome, unique match, confirmed good (green family).")]
        [PreviewHint(ColorRole.Background, ColorContext.Extractor)]
        public Color Success { get; set; }

        [Category("Status"), DisplayName("Success Text")]
        [Description("Foreground color for success text (green family).")]
        [PreviewHint(ColorRole.Foreground, ColorContext.Extractor)]
        public Color SuccessText { get; set; }

        [Category("Status"), DisplayName("Warning")]
        [Description("Ambiguous or attention-needed state (yellow/salmon family).")]
        [PreviewHint(ColorRole.Background, ColorContext.Extractor)]
        public Color Warning { get; set; }

        [Category("Status"), DisplayName("Warning Text")]
        [Description("Foreground color for warning text (yellow family).")]
        [PreviewHint(ColorRole.Foreground, ColorContext.Extractor)]
        public Color WarningText { get; set; }

        [Category("Status"), DisplayName("Error")]
        [Description("Failed validation, critical issue (red/coral family).")]
        [PreviewHint(ColorRole.Background, ColorContext.Extractor)]
        public Color Error { get; set; }

        [Category("Status"), DisplayName("Info")]
        [Description("Informational highlight (blue family).")]
        [PreviewHint(ColorRole.Background, ColorContext.Extractor)]
        public Color Info { get; set; }

        [Category("Status"), DisplayName("Caution")]
        [Description("Needs attention but not wrong (gold/orange family).")]
        [PreviewHint(ColorRole.Background, ColorContext.Extractor)]
        public Color Caution { get; set; }

        [Category("Status"), DisplayName("Neutral")]
        [Description("Default/cleared state background.")]
        [PreviewHint(ColorRole.Background, ColorContext.Generic)]
        public Color Neutral { get; set; }

        [Category("Status"), DisplayName("Non-Unique")]
        [Description("Non-unique extraction result (yellow family, distinct from Caution).")]
        [PreviewHint(ColorRole.Background, ColorContext.Extractor)]
        public Color NonUnique { get; set; }

        [Category("Status"), DisplayName("Error Text")]
        [Description("Foreground color for error text (red family).")]
        [PreviewHint(ColorRole.Foreground, ColorContext.Extractor)]
        public Color ErrorText { get; set; }

        // --- Creature Sex ---

        [Category("Creature Sex"), DisplayName("Male")]
        [PreviewHint(ColorRole.Background, ColorContext.LibraryGrid)]
        public Color SexMale { get; set; }

        [Category("Creature Sex"), DisplayName("Female")]
        [PreviewHint(ColorRole.Background, ColorContext.LibraryGrid)]
        public Color SexFemale { get; set; }

        [Category("Creature Sex"), DisplayName("Neutered")]
        [PreviewHint(ColorRole.Background, ColorContext.LibraryGrid)]
        public Color SexNeutered { get; set; }

        [Category("Creature Sex"), DisplayName("Male Text")]
        [Description("Foreground text color for male-related labels.")]
        [PreviewHint(ColorRole.Foreground, ColorContext.CreatureInfo)]
        public Color SexMaleText { get; set; }

        [Category("Creature Sex"), DisplayName("Female Text")]
        [Description("Foreground text color for female-related labels.")]
        [PreviewHint(ColorRole.Foreground, ColorContext.CreatureInfo)]
        public Color SexFemaleText { get; set; }

        // --- Creature Status ---

        [Category("Creature Status"), DisplayName("Dead Creature")]
        [Description("Background for dead creatures.")]
        [PreviewHint(ColorRole.Background, ColorContext.LibraryGrid)]
        public Color DeadCreature { get; set; }

        [Category("Creature Status"), DisplayName("Obelisk Text")]
        [Description("Text color for obelisk creatures.")]
        [PreviewHint(ColorRole.Foreground, ColorContext.LibraryGrid)]
        public Color ObeliskText { get; set; }

        [Category("Creature Status"), DisplayName("Over Level Warning")]
        [Description("Text color for creatures that may exceed the server level cap.")]
        [PreviewHint(ColorRole.Foreground, ColorContext.LibraryGrid)]
        public Color OverLevelWarning { get; set; }

        [Category("Creature Status"), DisplayName("Top Breeding (All)")]
        [Description("Background for top breeding creatures with all considered top stats.")]
        [PreviewHint(ColorRole.Background, ColorContext.LibraryGrid)]
        public Color TopBreedingAll { get; set; }

        [Category("Creature Status"), DisplayName("Top Breeding (Some)")]
        [Description("Background for top breeding creatures with some top stats.")]
        [PreviewHint(ColorRole.Background, ColorContext.LibraryGrid)]
        public Color TopBreedingSome { get; set; }

        [Category("Creature Status"), DisplayName("Creature Existing")]
        [Description("Background indicating a creature already exists in the library.")]
        [PreviewHint(ColorRole.Background, ColorContext.LibraryGrid)]
        public Color CreatureExisting { get; set; }

        // --- Mutations ---

        [Category("Mutations"), DisplayName("Mutation")]
        [Description("Color representing a mutation counter unequal zero.")]
        [PreviewHint(ColorRole.Background, ColorContext.StatIO)]
        public Color Mutation { get; set; }

        [Category("Mutations"), DisplayName("Mutation Over Limit")]
        [Description("Color representing a mutation counter over the limit where more mutations can be produced.")]
        [PreviewHint(ColorRole.Background, ColorContext.StatIO)]
        public Color MutationOverLimit { get; set; }

        [Category("Mutations"), DisplayName("Mutation Marker")]
        [Description("Color representing a mutation for a inheritage line or marker.")]
        [PreviewHint(ColorRole.Foreground, ColorContext.Pedigree)]
        public Color MutationMarker { get; set; }

        [Category("Mutations"), DisplayName("Mutation Marker (Possible)")]
        [Description("Color representing a possible mutation for a inheritage line or marker.")]
        [PreviewHint(ColorRole.Foreground, ColorContext.Pedigree)]
        public Color MutationMarkerPossible { get; set; }

        // --- Special Levels ---

        [Category("Special Levels"), DisplayName("Level 254")]
        [Description("Level 254 — highest that allows dom leveling.")]
        [PreviewHint(ColorRole.Background, ColorContext.StatIO)]
        public Color Level254 { get; set; }

        [Category("Special Levels"), DisplayName("Level 255")]
        [Description("Level 255 — highest that can be saved.")]
        [PreviewHint(ColorRole.Background, ColorContext.StatIO)]
        public Color Level255 { get; set; }

        // --- Countdown Timers ---

        [Category("Countdown Timers"), DisplayName("Growing Imminent")]
        [PreviewHint(ColorRole.Background, ColorContext.TimerList)]
        public Color GrowingImminent { get; set; }

        [Category("Countdown Timers"), DisplayName("Growing Soon")]
        [PreviewHint(ColorRole.Background, ColorContext.TimerList)]
        public Color GrowingSoon { get; set; }

        [Category("Countdown Timers"), DisplayName("Growing Later")]
        [PreviewHint(ColorRole.Background, ColorContext.TimerList)]
        public Color GrowingLater { get; set; }

        [Category("Countdown Timers"), DisplayName("Cooldown Imminent")]
        [PreviewHint(ColorRole.Background, ColorContext.TimerList)]
        public Color CooldownImminent { get; set; }

        [Category("Countdown Timers"), DisplayName("Cooldown Soon")]
        [PreviewHint(ColorRole.Background, ColorContext.TimerList)]
        public Color CooldownSoon { get; set; }

        [Category("Countdown Timers"), DisplayName("Cooldown Later")]
        [PreviewHint(ColorRole.Background, ColorContext.TimerList)]
        public Color CooldownLater { get; set; }

        // --- Parent Similarity ---

        [Category("Parent Similarity"), DisplayName("Good Match")]
        [PreviewHint(ColorRole.Background, ColorContext.BreedingPlan)]
        public Color SimilarityGood { get; set; }

        [Category("Parent Similarity"), DisplayName("Ok Match")]
        [PreviewHint(ColorRole.Background, ColorContext.BreedingPlan)]
        public Color SimilarityOk { get; set; }

        [Category("Parent Similarity"), DisplayName("Poor Match")]
        [PreviewHint(ColorRole.Background, ColorContext.BreedingPlan)]
        public Color SimilarityPoor { get; set; }

        // --- Actions ---

        [Category("Actions"), DisplayName("Add")]
        [PreviewHint(ColorRole.Background, ColorContext.Generic)]
        public Color ActionAdd { get; set; }

        [Category("Actions"), DisplayName("Remove")]
        [PreviewHint(ColorRole.Background, ColorContext.Generic)]
        public Color ActionRemove { get; set; }

        [Category("Actions"), DisplayName("Locked Input")]
        [PreviewHint(ColorRole.Background, ColorContext.StatIO)]
        public Color LockedInput { get; set; }

        // --- Library Filter ---

        [Category("Library Filter"), DisplayName("Filter Active")]
        [PreviewHint(ColorRole.Background, ColorContext.LibraryFilter)]
        public Color FilterActive { get; set; }

        [Category("Library Filter"), DisplayName("Filter Empty")]
        [PreviewHint(ColorRole.Background, ColorContext.LibraryFilter)]
        public Color FilterEmpty { get; set; }

        [Category("Library Filter"), DisplayName("Filter Button")]
        [PreviewHint(ColorRole.Background, ColorContext.LibraryFilter)]
        public Color FilterButton { get; set; }

        // --- Color Indicators ---

        [Category("Color Indicators"), DisplayName("New Color in Region")]
        [PreviewHint(ColorRole.Border, ColorContext.Generic)]
        public Color NewColorInRegion { get; set; }

        [Category("Color Indicators"), DisplayName("New Color in Species")]
        [PreviewHint(ColorRole.Border, ColorContext.Generic)]
        public Color NewColorInSpecies { get; set; }

        // --- Controls ---

        [Category("Controls"), DisplayName("Border around controls")]
        [PreviewHint(ColorRole.Line, ColorContext.Generic)]
        public Color ControlBorder { get; set; }

        [Category("Controls"), DisplayName("Divider Line")]
        [PreviewHint(ColorRole.Line, ColorContext.Generic)]
        public Color DividerLine { get; set; }

        // --- Pedigree ---

        [Category("Pedigree"), DisplayName("Inheritance Line Better")]
        [Description("Pen color for ancestry lines of better stat inheritance.")]
        [PreviewHint(ColorRole.Line, ColorContext.Pedigree)]
        public Color InheritanceLineBetter { get; set; }

        [Category("Pedigree"), DisplayName("Inheritance Line Worse")]
        [Description("Pen color for ancestry lines of worse stat inheritance.")]
        [PreviewHint(ColorRole.Line, ColorContext.Pedigree)]
        public Color InheritanceLineWorse { get; set; }

        [Category("Pedigree"), DisplayName("Selected Creature")]
        [Description("Highlight color for the selected creature in the pedigree.")]
        [PreviewHint(ColorRole.Background, ColorContext.Pedigree)]
        public Color PedigreeSelected { get; set; }
    }
}
