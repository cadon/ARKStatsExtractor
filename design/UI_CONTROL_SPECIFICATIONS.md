# UI Control Specifications

This document outlines the specifications for the main UI controls in ARK Smart Breeding. These specifications will guide the creation of automated UI tests and the separation of domain logic from UI concerns.

## Overview

The application has significant domain logic mixed into UI controls. The following specs document current behavior that should be preserved while refactoring.

---

## 1. TamingControl

**Purpose**: Calculate and display taming information for creatures

**Location**: `ARKBreedingStats/TamingControl.cs`

### Responsibilities (Current - Mixed UI & Domain)
- Display species taming data
- Calculate taming effectiveness based on food type
- Calculate torpor/knockout timing
- Generate wake-up and starving timers
- Calculate weapon damage for different tranquilizers
- Update taming food consumption rates

### Domain Logic Identified
- **Taming calculation algorithms** (lines 100-250+)
  - Food consumption calculations
  - Torpor depletion calculations
  - Effectiveness calculations based on food type
- **Bone damage multiplier calculations**
- **Food depletion rate calculations**: `_foodDepletion = td.foodConsumptionBase * td.foodConsumptionMult * _serverMultipliers...`

### UI Responsibilities
- Display calculated values
- Handle user input (level, food type)
- Generate timer creation events
- Display list of taming foods

### Test Scenarios
1. **Setting species updates display**
   - Given: Control is initialized
   - When: `SetSpecies()` is called with valid species
   - Then: Species name, wiki link, and taming data are displayed
   
2. **Level change triggers recalculation**
   - Given: Species is set and taming data exists
   - When: User changes level via nudLevel
   - Then: All taming values are recalculated
   
3. **Food selection updates effectiveness**
   - Given: Multiple food options are available
   - When: User selects different food type
   - Then: Taming time and effectiveness values update

4. **Creates wake-up timer correctly**
   - Given: Taming data is calculated
   - When: User clicks "Add Wake-Up Timer"
   - Then: CreateTimer event fires with correct time

5. **Handles species with no taming data**
   - Given: Control is initialized
   - When: SetSpecies called with untameable species
   - Then: Display shows "No taming data available"

---

## 2. CreatureInfoInput

**Purpose**: Input and edit creature information (name, owner, parents, colors, etc.)

**Location**: `ARKBreedingStats/CreatureInfoInput.cs`

### Responsibilities (Current - Mixed UI & Domain)
- Display and edit creature metadata (name, owner, tribe, server)
- Parent selection and validation
- Color region selection
- Naming pattern generation
- Maturation tracking
- Mutation counter management
- Trait selection

### Domain Logic Identified
- **Naming pattern generation** (lines 130+)
- **Duplicate name detection**
- **Parent validation logic**
- **Maturation calculations** (cooldown, growing time)
- **Color validation against existing creatures**

### UI Responsibilities
- Display creature information fields
- Handle parent combobox population
- Display region color chooser
- Show naming pattern buttons
- Display maturation progress

### Test Scenarios
1. **Setting creature data populates fields**
   - Given: Control is initialized
   - When: `SetCreatureData()` called with creature
   - Then: All fields populated correctly

2. **Name uniqueness checking**
   - Given: Creature list with existing names
   - When: User enters duplicate name
   - Then: Warning indicator appears

3. **Parent selection updates correctly**
   - Given: Parent list is populated
   - When: User selects mother and father
   - Then: Parent inheritance displays correctly

4. **Naming pattern generates unique names**
   - Given: Naming pattern is configured
   - When: User clicks naming pattern button
   - Then: Unique name is generated following pattern

5. **Color selection updates visualization**
   - Given: Species colors are loaded
   - When: User selects region colors
   - Then: Creature image updates with colors

6. **Maturation timer updates**
   - Given: Creature has growing time
   - When: Time passes or user adjusts
   - Then: Maturation percentage updates

---

## 3. StatIO

**Purpose**: Display and edit individual stat levels (Health, Stamina, etc.)

**Location**: `ARKBreedingStats/uiControls/StatIO.cs`

### Responsibilities (Current - Mixed UI & Domain)
- Display stat input value
- Track wild levels
- Track domesticated levels
- Track mutated levels
- Calculate breeding value
- Show status indicators (unique, top stats, etc.)
- Handle stat fixing (locking at zero)

### Domain Logic Identified
- **Breeding value calculations**
- **Wild/Dom/Mutated level tracking logic**
- **Status determination** (unique, new top stats)
- **Level cap validations**
- **Stat value to level calculations**

### UI Responsibilities
- Display numeric inputs for stat value
- Display level indicators
- Show visual status (colors, bars)
- Handle user input

### Test Scenarios
1. **Input value updates correctly**
   - Given: StatIO is initialized
   - When: User enters stat value
   - Then: Input accepted and InputValueChanged event fires

2. **Wild level changes trigger events**
   - Given: Control has valid stat
   - When: User changes wild level
   - Then: LevelChanged event fires

3. **Status indicators display correctly**
   - Given: Stat has unique/top values
   - When: Status is set
   - Then: Visual indicators show correct status

4. **Percentage stats display correctly**
   - Given: Stat is percentage type (e.g., Speed)
   - When: Value is set
   - Then: Displays with % symbol

5. **Fixed dom zero locks level**
   - Given: Stat is at base value
   - When: User checks "Fix Dom Zero"
   - Then: Dom level locked at 0

6. **Bar visualization scales correctly**
   - Given: Control has max level set
   - When: Level changes
   - Then: Bar length proportional to level

---

## 4. CreatureBox

**Purpose**: Display creature summary with edit capabilities

**Location**: `ARKBreedingStats/CreatureBox.cs`

### Responsibilities (Current - Mixed UI & Domain)
- Display creature name, stats, colors
- Show sex and status
- Parent selection in edit mode
- Note editing
- Trigger creature selection

### Domain Logic Identified
- **Parent similarity calculations** (parentListSimilarity)
- **Color validation logic**
- **Status determination**

### UI Responsibilities
- Display creature information
- Toggle edit panel visibility
- Handle button clicks
- Show tooltips

### Test Scenarios
1. **Setting creature updates display**
   - Given: Control is initialized
   - When: `SetCreature()` called
   - Then: All creature data displays correctly

2. **Edit button toggles panel**
   - Given: Creature is set
   - When: User clicks edit button
   - Then: Edit panel becomes visible

3. **Saving changes fires event**
   - Given: Edit panel is open with changes
   - When: User clicks save
   - Then: Changed event fires with updated data

4. **Parent population shows valid options**
   - Given: Creature is bred
   - When: Edit panel opens
   - Then: Parent lists show same-species creatures

5. **Status button cycles statuses**
   - Given: Edit panel is open
   - When: User clicks status button
   - Then: Status cycles through valid options

---

## 5. SpeciesSelector

**Purpose**: Select species from available list

**Location**: `ARKBreedingStats/SpeciesSelector.cs`

### Test Scenarios
1. **Filter updates visible species**
2. **Recent species list updates**
3. **Variant filtering works correctly**
4. **Selection confirms with correct species**

---

## 6. MultiSetter

**Purpose**: Bulk edit multiple creatures

**Location**: `ARKBreedingStats/uiControls/MultiSetter.cs`

### Test Scenarios
1. **Setting owner applies to all selected**
2. **Tag selection applies correctly**
3. **Color changes apply to all**
4. **Cancel reverts changes**

---

## Domain Logic to Extract

### High Priority for Extraction
1. **Taming calculations** → TamingCalculator service
2. **Breeding value calculations** → BreedingCalculator service
3. **Naming pattern generation** → NamingService
4. **Stat level calculations** → StatCalculator service
5. **Parent matching/similarity** → ParentMatchingService

### Medium Priority
1. **Color validation logic** → ColorValidationService
2. **Maturation tracking** → MaturationService
3. **Torpor calculations** → TorporCalculator

### Architecture Goal
```
UI Layer (Controls)
    ↓ (uses)
Services Layer (Domain Logic)
    ↓ (uses)
Domain Models (Species, Creature, etc.)
```

---

## Testing Strategy

### Test Framework
- **MSTest** (already in use)
- **UI Automation**: Need to add Windows Forms testing support

### Test Types
1. **Unit Tests**: Test domain logic in isolation (after extraction)
2. **Integration Tests**: Test UI controls with services
3. **UI Tests**: Test user interactions end-to-end

### Test Organization
```
ARKBreedingStats.Tests/
├── Unit/
│   ├── Services/
│   │   ├── TamingCalculatorTests.cs
│   │   ├── BreedingCalculatorTests.cs
│   │   └── NamingServiceTests.cs
│   └── Models/
├── Integration/
│   └── Controls/
│       ├── TamingControlTests.cs
│       ├── CreatureInfoInputTests.cs
│       └── StatIOTests.cs
└── UI/
    └── (E2E tests if needed)
```

---

## Next Steps

1. ✅ Document specifications
2. ⏳ Set up UI testing infrastructure
3. ⏳ Write initial tests for existing behavior
4. ⏳ Extract domain logic to services
5. ⏳ Refactor controls to use services
6. ⏳ Verify tests still pass
