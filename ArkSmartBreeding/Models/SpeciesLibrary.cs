using ARKBreedingStats.Settings;
using System.Collections.Generic;
using System.ComponentModel;

namespace ARKBreedingStats.Models;

/// <summary>
/// Manages all species instances and handles recalculation when server multipliers or domain settings change.
/// </summary>
public class SpeciesLibrary
{
    private readonly ServerMultipliers _multipliers;
    private readonly DomainSettings _settings;
    // TODO: Add species collection when Species class is migrated to Core
    // private readonly List<Species> _species = new();

    /// <summary>
    /// Creates a new SpeciesLibrary with the given server multipliers and domain settings.
    /// Registers listeners to react when multipliers or settings change.
    /// </summary>
    /// <param name="multipliers">The server multipliers used for species stat calculations.</param>
    /// <param name="settings">The domain settings affecting how species data is displayed and initialized.</param>
    public SpeciesLibrary(ServerMultipliers multipliers, DomainSettings settings)
    {
        _multipliers = multipliers;
        _multipliers.PropertyChanged += OnMultipliersChanged;

        _settings = settings;
        _settings.PropertyChanged += OnSettingsChanged;
    }

    /// <summary>
    /// Called when any server multiplier changes.
    /// Invalidates cached calculations for all species.
    /// </summary>
    private void OnMultipliersChanged(object? sender, PropertyChangedEventArgs e)
    {
        // TODO: Enumerate all species and invalidate their calculated stats
        // foreach (var species in _species)
        //     species.InvalidateCalculatedStats();
    }

    /// <summary>
    /// Called when any domain setting changes.
    /// Re-initializes display names and color regions for all species.
    /// </summary>
    private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        // TODO: When Species is in Core, re-initialize the affected data:
        // if (e.PropertyName == nameof(DomainSettings.IgnoreVariantsInName))
        //     foreach (var species in _species) species.InitializeNames(_settings.IgnoreVariantsInName);
        // else if (e.PropertyName is nameof(DomainSettings.AlwaysShowAllColorRegions)
        //                          or nameof(DomainSettings.HideInvisibleColorRegions))
        //     foreach (var species in _species) species.InitializeColorRegions(_settings);
    }

    /// <summary>
    /// Gets the server multipliers used by this library.
    /// </summary>
    public ServerMultipliers Multipliers => _multipliers;

    /// <summary>
    /// Gets the domain settings used by this library.
    /// </summary>
    public DomainSettings Settings => _settings;

    // TODO: Add methods to add/remove/lookup species when Species class is in Core
    // public void AddSpecies(Species species) { ... }
    // public Species? GetSpeciesByBlueprintPath(string blueprintPath) { ... }
    // public IReadOnlyList<Species> AllSpecies => _species.AsReadOnly();
}
