using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ARKBreedingStats.Settings;

/// <summary>
/// User-configurable settings that affect how species data flows through the domain layer.
/// Implements INotifyPropertyChanged to allow consumers (e.g. SpeciesLibrary) to react when
/// settings change and re-initialize affected data.
/// </summary>
public class DomainSettings : INotifyPropertyChanged
{
    private string[] _ignoreVariantsInName = Array.Empty<string>();
    private bool _alwaysShowAllColorRegions;
    private bool _hideInvisibleColorRegions;
    private Dictionary<string, int>? _wildLevelExceptions;

    /// <summary>
    /// Variant tag strings whose presence in a species name is suppressed in the descriptive display name.
    /// Loaded from the user-editable hideVariantsInSpeciesName.txt file.
    /// </summary>
    public string[] IgnoreVariantsInName
    {
        get => _ignoreVariantsInName;
        set => SetField(ref _ignoreVariantsInName, value);
    }

    /// <summary>
    /// If true, all 6 color regions are shown for every species regardless of its configuration.
    /// </summary>
    public bool AlwaysShowAllColorRegions
    {
        get => _alwaysShowAllColorRegions;
        set => SetField(ref _alwaysShowAllColorRegions, value);
    }

    /// <summary>
    /// If true, color regions marked as invisible in the species definition are hidden.
    /// </summary>
    public bool HideInvisibleColorRegions
    {
        get => _hideInvisibleColorRegions;
        set => SetField(ref _hideInvisibleColorRegions, value);
    }

    /// <summary>
    /// Per-species bit flags of stats that can have wild levels despite the current species definition not including them.
    /// Loaded from canHaveWildLevelExceptions.json — accounts for historically changed stat definitions.
    /// </summary>
    public Dictionary<string, int>? WildLevelExceptions
    {
        get => _wildLevelExceptions;
        set => SetField(ref _wildLevelExceptions, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
