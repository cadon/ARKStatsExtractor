# Species Class Analysis for Core Migration

## Overview
Species class (602 lines) is the largest migration target. Contains both static JSON data and calculated runtime properties.

## Static Domain Data (belongs in Core)

### JSON Properties (deserialized from values files):
```csharp
[JsonProperty] string name
[JsonProperty] string nameFemale
[JsonProperty] string nameMale
[JsonProperty] string[] variants
[JsonProperty] string blueprintPath
[JsonProperty] double[][] fullStatsRaw  // Raw stat values [stat][base, incPerWild, incPerDom, addBonus, multBonus]
[JsonProperty("altBaseStats")] Dictionary<int, double> altBaseStatsRaw  // Troodonism alternate base values
[JsonProperty] float[] mutationMult
[JsonProperty("displayedStats")] int DisplayedStats  // bit flags for stats shown in game
[JsonProperty] private int skipWildLevelStats  // bit flags for stats that can't get wild levels
[JsonProperty] bool? isFlyer
[JsonProperty] string[] matesWith  // blueprint paths of compatible mates
[JsonProperty] float? TamedBaseHealthMultiplier
[JsonProperty] private double[] statImprintMult  // default imprinting multipliers
[JsonProperty] ColorRegion[] colors  // Already in Core
[JsonProperty] double[] regionIntensities
[JsonProperty] TamingData taming  // Already in Core
[JsonProperty] BreedingData breeding  // Already in Core
[JsonProperty] bool? noGender
[JsonProperty] Dictionary<string, double> boneDamageAdjusters
[JsonProperty] List<string> immobilizedBy
[JsonProperty] Dictionary<string, string> statNames  // custom stat names for this species
[JsonProperty("statCaps")] private Dictionary<int, double> _statCaps
[JsonProperty("statLevelUpsAdditive")] private Dictionary<int, bool> _statLevelUpsAdditive
public ColorPattern patterns  // Already in Core
```

### Derived Static Data (computed once from JSON):
```csharp
private Mod _mod  // Already in Core
public bool IsFlyer => isFlyer == true
public bool NoGender => noGender == true
```

## Calculated/Runtime Properties (stay in App or move to SpeciesCalculated)

### Calculated from fullStatsRaw + Server Multipliers:
```csharp
public SpeciesStat[] stats  // Calculated with multipliers applied
public SpeciesStat[] altStats  // Alternative stats if applicable
private int usedStats  // bit flags, calculated from fullStatsRaw
private int _skipWildLevelStatsWithServerSettings  // skipWildLevelStats + server exceptions
```

### Runtime State/Overrides:
```csharp
private double[] statImprintMultOverride  // Custom overrides set by user
public double[] StatImprintMultipliers  // Effective multipliers (default or override + server settings)
public double[] StatImprintMultipliersRaw  // For custom species
public bool[] EnabledColorRegions  // Calculated from colors + UI settings
```

### Derived Names (calculated from name + variants + mod):
```csharp
public string SortName
public string DescriptiveName
public string VariantInfo
public string DescriptiveNameAndMod
```

### Computed Flags:
```csharp
public bool IsDomesticable  // Calculated from taming + breeding data
```

## Methods Analysis

### Core-Compatible (pure functions):
- `ToString()` - uses DescriptiveNameAndMod or name
- `GetHashCode()` - uses blueprintPath
- `Equals()`, operators - use blueprintPath
- `Name(Sex)` - returns name variant by sex
- Constants (StatsRawIndexBase, etc.)

### App Layer Initialization (requires server settings):
```csharp
void Initialize()  // Main init, applies multipliers, needs server settings
void InitializeNames()  // Name derivation, needs Mod info
void InitializeColors(ArkColors, settings...)  // Needs app-layer ArkColors
void InitializeColorRegions(settings...)  // Already refactored
void ApplyCanLevelOptions(canLevelSpeed, canFlyerLevelSpeed)  // Server settings
void SetCustomImprintingMultipliers(overrides)  // Runtime modification
```

### App Layer Methods:
```csharp
byte[] RandomSpeciesColors(Random)  // Uses EnabledColorRegions (calculated)
void LoadOverrides(Species)  // Mod loading logic
bool UsesStat(int)  // Uses usedStats (calculated)
bool DisplaysStat(int)  // Uses DisplayedStats (could be Core)
bool CanLevelUpWildOrHaveMutations(int)  // Uses _skipWildLevelStatsWithServerSettings (calculated)
```

## Migration Strategy

### Option A: Full Split (Complex)
1. Create `Species` in Core with all JSON properties
2. Create `SpeciesCalculated` in App with calculated properties
3. Refactor all usage to use SpeciesCalculated

**Pros:** Clean separation, true domain layer
**Cons:** Massive refactoring, 100+ files affected

### Option B: Incremental (Recommended)
1. Keep Species in App layer for now
2. Move supporting data classes to Core first (already done: SpeciesStat, TamingData, BreedingData, ColorRegion, etc.)
3. Refactor Species.Initialize() to separate concerns
4. Create ServerMultipliers class to hold server settings
5. Eventually extract static data when needed

**Pros:** Incremental, testable at each step
**Cons:** Species stays in App layer temporarily

### Option C: Hybrid
1. Create a lightweight `SpeciesData` in Core with only JSON properties
2. Keep current `Species` class in App, have it compose SpeciesData
3. Gradually refactor to use SpeciesData directly

## Dependencies to Remove for Core Migration

### Current UI/App Dependencies in Species:
1. ✅ **DONE:** `Properties.Settings.Default` removed from InitializeColorRegions
2. `FileService` - used in `_getIgnoreVariantInName()` for loading hideVariants file
3. `ArkColors` collection class (app layer) - used in InitializeColors
4. Server settings (wild level exceptions) - used in Initialize
5. `Random` class (acceptable in Core, but EnabledColorRegions is calculated)

## Next Steps

1. Create `ServerMultipliers` class (App layer) to encapsulate:
   - Wild level exceptions settings
   - Can level speed settings  
   - Imprinting multipliers global settings
   - Other server-specific multipliers

2. Refactor `Species.Initialize()` to accept ServerMultipliers parameter

3. Consider if Species should move to Core or stay in App:
   - If Core: Extract pure data model
   - If App: Keep as-is but reduce dependencies

4. Implement SpeciesCalculated pattern if needed for lazy recalculation

## Size Estimate
- **Core extractable:** ~300 lines of JSON properties + basic methods
- **App layer:** ~300 lines of initialization + calculated properties
- **Supporting classes needed:** ServerMultipliers (~50-100 lines)