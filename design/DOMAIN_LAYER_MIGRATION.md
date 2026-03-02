# Domain Layer Migration Progress

## Overview
Separating domain layer (ARKBreedingStats.Core) from UI layer to enable:
- Pure domain logic without UI dependencies
- Automated testing of domain models
- Clear architectural boundaries

## Architecture Design

### Three-Tier Pattern
1. **Core Layer** (ARKBreedingStats.Core - .NET 10.0)
   - Static domain data (Species, SpeciesStat, TamingData, BreedingData)
   - Pure value objects (Sex, Stats constants)
   - No UI, no Settings, nullable enabled

2. **Application Layer** (ARKBreedingStats - .NET 10.0-windows)
   - Calculated/reactive models (SpeciesCalculated, ServerMultipliers)
   - Business logic combining Core + runtime state
   - Event-based invalidation with Lazy<T>

3. **UI Layer** (WinForms)
   - Presentation logic only
   - Data binding to Application models

### Key Pattern: Static vs Calculated
**Problem:** Species has both static data (base stats) and calculated data (affected by server multipliers).

**Solution:**
- `Species` (Core) = static JSON data (base stats, breeding, taming)
- `ServerMultipliers` (App) = runtime server settings
- `SpeciesCalculated` (App) = Species + ServerMultipliers → lazy calculated stats
- `SpeciesLibrary` (App) = manages all SpeciesCalculated instances, handles multiplier change events

### Library Pattern: Collection Management
**Problem:** Need to efficiently update many entities when global settings change.

**Solution:**
- **SpeciesLibrary** = holds references to all species in the library
  - Enumerates all species when ServerMultipliers change
  - Invalidates cached calculated stats on all species
  - Triggers recalculation lazily on next access
  
- **CreatureLibrary** = holds references to all creatures in the library
  - Enumerates creatures to update stats when species data changes
  - Updates timers, breeding calculations, etc.
  - Manages creature lifecycle and persistence

## Migration Progress

### ✅ Completed (17 classes, ~920 lines)

**Support Classes:**
- [x] DiceCoefficient (32 lines, 5 tests) - string similarity
- [x] MinMaxDouble/MinMaxInt (135 lines, 14 tests) - range types
- [x] StatResult (20 lines, 6 tests) - extraction results
- [x] Note (17 lines) - note model
- [x] Player (13 lines) - player model

**Domain Constants:**
- [x] Stats (90 lines) - stat index constants, ~50 files updated
- [x] Sex (13 lines) - creature sex enum, 13 files updated
- [x] GameConstants (17 lines) - game edition identifiers (Ase/Asa)

**Species Data Models:**
- [x] SpeciesStat (26 lines) - stat multipliers
- [x] TamingFood (31 lines) - food taming values
- [x] TamingData (82 lines) - taming mechanics
- [x] BreedingData (46 lines) - breeding times/temps
- [x] Mod (109 lines) - mod metadata, 6 files updated

**Files Updated:** ~60+ files with `using ARKBreedingStats.Core;`

**Test Status:** 90 passed, 20 skipped, 0 failed ✅

**Color Classes (3 classes, ~115 lines):**
- [x] ColorPattern (14 lines) - simple data class
- [x] ArkColor (74 lines) - color definition (name, RGB, linear values)
  - Removed `Loc.S()` dependency, replaced with plain "No Color" string
- [x] ColorRegion (33 lines) - species color region data  
  - Removed `Loc.S()` dependency, replaced with plain "Unknown" string
  - Moved `Initialize(ArkColors)` method to app layer extension class
  - Created `ColorRegionExtensions.cs` in app layer for initialization logic

**Files Updated:** 8 files with `using ARKBreedingStats.Core;` (ARKColors.cs, CreatureColors.cs, RegionColorChooser.cs, ColorPickerControl.cs, CreatureAnalysis.cs, ColorRegionExtensions.cs created, and Species.cs, ValuesFile.cs already had it)

**Note:** ArkColors.cs remains in app layer as a collection/management class

**Server Architecture (2 classes, ~305 lines):**
- [x] ServerMultipliers (280 lines) - migrated from App layer to Core
  - Added INotifyPropertyChanged implementation with property change notifications
  - All 16+ multiplier properties now notify observers when changed
  - Maintains JSON serialization for save/load
  - Includes stat multipliers arrays, breeding multipliers, speed leveling settings
- [x] SpeciesLibrary (25 lines) - collection manager in Core
  - Subscribes to ServerMultipliers.PropertyChanged events
  - Infrastructure ready for species invalidation when multipliers change
  - Will enumerate all species to trigger recalculation (to be implemented with Species migration)

**Files Updated:** 1 file (ServerMultipliersPresets.cs) updated to use Core.ServerMultipliers

### 🔜 Species Migration Preparation

**Preparation Complete:**
1. [x] Refactor Species.InitializeColorRegions() 
   - ✅ Removed direct `Properties.Settings.Default` access (UI dependency)
   - ✅ Added parameters: `alwaysShowAllColorRegions` and `hideInvisibleColorRegions`
   - ✅ Updated `InitializeColors()` to accept and pass through these parameters
   - ✅ Updated all 3 call sites (Species.cs, Values.cs, Form1.cs) to pass settings explicitly
   - Methods now ready for Core migration (no UI dependencies)
2. [x] Complete Species class analysis (see [SPECIES_ANALYSIS.md](SPECIES_ANALYSIS.md))
   - ✅ Identified static domain data (~300 lines of JSON properties)
   - ✅ Identified calculated/runtime properties (~300 lines)
   - ✅ Documented remaining dependencies to remove
   - ✅ Defined migration strategy: Incremental approach with ServerMultipliers first
3. [x] Create ServerMultipliers class (Core layer) - **COMPLETED**
   - ✅ Migrated comprehensive ServerMultipliers from App to Core (~280 lines)
   - ✅ Added INotifyPropertyChanged with property notifications on all multipliers
   - ✅ Maintains JSON serialization and all existing functionality
   - ✅ Created SpeciesLibrary in Core to manage species and listen for multiplier changes

**Next Steps:**
4. [ ] Refactor Species.Initialize() to accept ServerMultipliers parameter
5. [ ] Decide: Move Species to Core or keep in App with reduced dependencies
6. [ ] If needed: Create SpeciesCalculated wrapper pattern

### 🎯 Final Architecture Implementation

1. [ ] Create ServerMultipliers class (App layer)
   - Breeding multipliers, stat multipliers, etc.
   - INotifyPropertyChanged or event-based
   - Fire change events when any multiplier is modified

2. [ ] Create SpeciesCalculated class (App layer)
   - Combines Species + ServerMultipliers
   - Lazy<T> calculated properties (stats with multipliers applied)
   - Invalidate on multiplier changes
   - Recalculate lazily on next access

3. [ ] Create SpeciesLibrary manager (App layer)
   - Manages all SpeciesCalculated instances (or Species with calculated stats)
   - Subscribes to ServerMultipliers change events
   - Enumerates and invalidates all species when multipliers change
   - Provides lookup by blueprint path, name, etc.

4. [ ] Consider CreatureLibrary pattern
   - Holds references to all creatures in the user's library
   - Enumerates creatures to update stats when species data changes
   - Updates timers, breeding calculations, aging, etc.
   - Manages creature persistence and lifecycle

5. [ ] Update UI to use SpeciesLibrary/CreatureLibrary instead of direct species access

## Key Migration Patterns

### Pattern 1: Simple Extract
Classes with zero dependencies → direct copy to Core + delete old + add imports

### Pattern 2: Constant Extraction
When Core classes need simple constants → extract to GameConstants:
```csharp
// Core/GameConstants.cs
public const string Ase = "ASE";
public const string Asa = "ASA";

// App/Ark.cs
public const string Ase = GameConstants.Ase; // reference Core
```

### Pattern 3: Build Cache Issues
If build fails despite correct imports → `dotnet clean` before rebuild

### Pattern 4: Extension Methods for App-Layer Logic
When Core data classes need app-layer behavior → use extension methods:
```csharp
// Core/ColorRegion.cs (data only)
public class ColorRegion {
    public List<string>? colors;
    public List<ArkColor>? naturalColors;
}

// App/ColorRegionExtensions.cs (behavior)
internal static class ColorRegionExtensions {
    internal static void Initialize(this ColorRegion colorRegion, ArkColors arkColors) {
        // app-layer logic using app-layer types
    }
}
```
This keeps Core pure while allowing app layer to add functionality.

### Pattern 5: Remove Localization from Core
Replace `Loc.S("key")` with plain English strings:
```csharp
// Before (App layer):
Name = Loc.S("noColor");

// After (Core layer):
Name = "No Color";
```
Localization is a presentation concern, not domain logic.

### Pattern 6: Parameter Injection for UI Dependencies
Replace direct access to settings/UI state with method parameters:
```csharp
// Before (UI-dependent):
public void InitializeColorRegions() {
    EnabledColorRegions = !Properties.Settings.Default.AlwaysShowAllColorRegions
        ? colors.Select(n => !n.invisible || !Properties.Settings.Default.HideInvisibleColorRegions).ToArray()
        : new[] { true, true, true, true, true, true };
}

// After (Core-compatible):
public void InitializeColorRegions(bool alwaysShowAllColorRegions = false, bool hideInvisibleColorRegions = false) {
    EnabledColorRegions = !alwaysShowAllColorRegions
        ? colors.Select(n => !n.invisible || !hideInvisibleColorRegions).ToArray()
        : new[] { true, true, true, true, true, true };
}

// Call sites explicitly pass settings:
species.InitializeColorRegions(Properties.Settings.Default.AlwaysShowAllColorRegions, 
                                Properties.Settings.Default.HideInvisibleColorRegions);
```
This makes dependencies explicit and allows Core classes to remain testable and UI-independent.

### Pattern 7: Library/Collection Management for Bulk Updates
Use collection classes to manage and update multiple entities when global state changes:
```csharp
// SpeciesLibrary - manages all species instances
public class SpeciesLibrary {
    private readonly List<SpeciesCalculated> _species;
    private readonly ServerMultipliers _multipliers;
    
    public SpeciesLibrary(ServerMultipliers multipliers) {
        _multipliers = multipliers;
        _multipliers.Changed += OnMultipliersChanged;
    }
    
    private void OnMultipliersChanged() {
        // Enumerate all species and invalidate cached calculations
        foreach (var sp in _species) {
            sp.InvalidateCalculatedStats();
        }
    }
}

// CreatureLibrary - manages all creature instances
public class CreatureLibrary {
    private readonly List<Creature> _creatures;
    
    public void UpdateAllStats() {
        // Enumerate all creatures to recalculate stats, timers, etc.
        foreach (var creature in _creatures) {
            creature.RecalculateStats();
            creature.UpdateTimers();
        }
    }
}
```
This centralizes bulk updates rather than having UI code loop through collections.

## Commands Reference

```powershell
# Build and check for errors
dotnet build 2>&1 | Select-String "(Build succeeded|error CS)"

# Clean build (when cached issues)
dotnet clean; dotnet build

# Run tests
dotnet test --no-build --verbosity quiet

# Delete old file after migration
Remove-Item 'path/to/old/File.cs'
```

## Notes

- **Nullable Safety:** All Core classes enforce `<Nullable>enable</Nullable>`
- **No Regressions:** All 90 tests pass throughout migrations
- **Incremental:** One class at a time, verify build + tests after each
- **Import Pattern:** `using ARKBreedingStats.Core;` alphabetically first

## Next Session Resume Point

**Current State:** 
- Color classes migrated to Core (ArkColor, ColorRegion, ColorPattern)
- Species.InitializeColorRegions() refactored to remove UI dependencies
- Species class fully analyzed (see [SPECIES_ANALYSIS.md](SPECIES_ANALYSIS.md))
- **ServerMultipliers migrated to Core with INotifyPropertyChanged** ✅
- **SpeciesLibrary created in Core** ✅
- Build passing, tests green (90/90)

**Next Action:** 
Refactor Species.Initialize() and ApplyCanLevelOptions() to accept ServerMultipliers parameter instead of accessing settings directly. This will:
- Remove `CanHaveWildLevelExceptions.GetWildLevelExceptions(name)` dependency
- Pass speed leveling settings explicitly (AllowSpeedLeveling, AllowFlyerSpeedLeveling)
- Make Species.Initialize() testable with different server configurations

**Estimated Remaining:** ~2-3 major tasks:
1. Species.Initialize() refactoring (~5-10 files to update)
2. Decide on Species location (Core vs App, likely stays in App for now)
3. Wire up SpeciesLibrary to invalidate species on multiplier changes
