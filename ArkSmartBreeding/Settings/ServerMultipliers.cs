using ARKBreedingStats.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ARKBreedingStats.Settings;

/// <summary>
/// Contains the multipliers of a server for stats, taming and breeding and levels.
/// Implements INotifyPropertyChanged to notify observers when multipliers change.
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public class ServerMultipliers : INotifyPropertyChanged
{
    /// <summary>
    /// statMultipliers[statIndex][m], m: 0: IndexTamingAdd, 1: IndexTamingMult, 2: IndexLevelDom, 3: IndexLevelWild
    /// </summary>
    [JsonProperty]
    public double[][]? statMultipliers { get; set; }

    private double _tamingSpeedMultiplier = 1;
    private double _wildDinoTorporDrainMultiplier = 1;
    private double _dinoCharacterFoodDrainMultiplier = 1;
    private double _tamedDinoCharacterFoodDrainMultiplier = 1;
    private double _wildDinoCharacterFoodDrainMultiplier = 1;
    private double _matingSpeedMultiplier = 1;
    private double _matingIntervalMultiplier = 1;
    private double _eggHatchSpeedMultiplier = 1;
    private double _babyMatureSpeedMultiplier = 1;
    private double _babyFoodConsumptionSpeedMultiplier = 1;
    private double _babyCuddleIntervalMultiplier = 1;
    private double _babyImprintingStatScaleMultiplier = 1;
    private double _babyImprintAmountMultiplier = 1;
    private bool _allowSpeedLeveling;
    private bool _allowFlyerSpeedLeveling;
    private bool _singlePlayerSettings;
    private bool _atlasSettings;

    [JsonProperty]
    public double TamingSpeedMultiplier
    {
        get => _tamingSpeedMultiplier;
        set => SetField(ref _tamingSpeedMultiplier, value);
    }

    [JsonProperty]
    public double WildDinoTorporDrainMultiplier
    {
        get => _wildDinoTorporDrainMultiplier;
        set => SetField(ref _wildDinoTorporDrainMultiplier, value);
    }

    [JsonProperty]
    public double DinoCharacterFoodDrainMultiplier
    {
        get => _dinoCharacterFoodDrainMultiplier;
        set => SetField(ref _dinoCharacterFoodDrainMultiplier, value);
    }

    [JsonProperty]
    public double TamedDinoCharacterFoodDrainMultiplier
    {
        get => _tamedDinoCharacterFoodDrainMultiplier;
        set => SetField(ref _tamedDinoCharacterFoodDrainMultiplier, value);
    }

    [JsonProperty]
    public double WildDinoCharacterFoodDrainMultiplier
    {
        get => _wildDinoCharacterFoodDrainMultiplier;
        set => SetField(ref _wildDinoCharacterFoodDrainMultiplier, value);
    }

    [JsonProperty]
    public double MatingSpeedMultiplier
    {
        get => _matingSpeedMultiplier;
        set => SetField(ref _matingSpeedMultiplier, value);
    }

    [JsonProperty]
    public double MatingIntervalMultiplier
    {
        get => _matingIntervalMultiplier;
        set => SetField(ref _matingIntervalMultiplier, value);
    }

    [JsonProperty]
    public double EggHatchSpeedMultiplier
    {
        get => _eggHatchSpeedMultiplier;
        set => SetField(ref _eggHatchSpeedMultiplier, value);
    }

    [JsonProperty]
    public double BabyMatureSpeedMultiplier
    {
        get => _babyMatureSpeedMultiplier;
        set => SetField(ref _babyMatureSpeedMultiplier, value);
    }

    [JsonProperty]
    public double BabyFoodConsumptionSpeedMultiplier
    {
        get => _babyFoodConsumptionSpeedMultiplier;
        set => SetField(ref _babyFoodConsumptionSpeedMultiplier, value);
    }

    [JsonProperty]
    public double BabyCuddleIntervalMultiplier
    {
        get => _babyCuddleIntervalMultiplier;
        set => SetField(ref _babyCuddleIntervalMultiplier, value);
    }

    [JsonProperty]
    public double BabyImprintingStatScaleMultiplier
    {
        get => _babyImprintingStatScaleMultiplier;
        set => SetField(ref _babyImprintingStatScaleMultiplier, value);
    }

    [JsonProperty]
    public double BabyImprintAmountMultiplier
    {
        get => _babyImprintAmountMultiplier;
        set => SetField(ref _babyImprintAmountMultiplier, value);
    }

    /// <summary>
    /// Setting introduced in ASA, for ASE it's always true.
    /// </summary>
    [JsonProperty]
    public bool AllowSpeedLeveling
    {
        get => _allowSpeedLeveling;
        set => SetField(ref _allowSpeedLeveling, value);
    }

    [JsonProperty]
    public bool AllowFlyerSpeedLeveling
    {
        get => _allowFlyerSpeedLeveling;
        set => SetField(ref _allowFlyerSpeedLeveling, value);
    }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool SinglePlayerSettings
    {
        get => _singlePlayerSettings;
        set => SetField(ref _singlePlayerSettings, value);
    }

    /// <summary>
    /// If true, apply extra multipliers for the game ATLAS.
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool AtlasSettings
    {
        get => _atlasSettings;
        set => SetField(ref _atlasSettings, value);
    }

    /// <summary>
    /// Fix any null values
    /// </summary>
    [OnDeserialized]
    private void DefineNullValues(StreamingContext _)
    {
        if (statMultipliers == null)
        {
            return;
        }

        int l = statMultipliers.Length;
        for (int s = 0; s < l; s++)
        {
            if (statMultipliers[s] == null)
            {
                statMultipliers[s] = new double[] { 1, 1, 1, 1 };
            }
        }
    }

    public ServerMultipliers() { }

    public ServerMultipliers(bool withStatMultipliersObject)
    {
        if (!withStatMultipliersObject)
        {
            return;
        }

        statMultipliers = new double[Stats.StatsCount][];
        for (int s = 0; s < Stats.StatsCount; s++)
        {
            statMultipliers[s] = new double[4];
        }
    }

    /// <summary>
    /// Returns a copy of the server multipliers
    /// </summary>
    /// <returns></returns>
    public ServerMultipliers Copy(bool withStatMultipliers)
    {
        var sm = new ServerMultipliers
        {
            TamingSpeedMultiplier = TamingSpeedMultiplier,
            WildDinoTorporDrainMultiplier = WildDinoTorporDrainMultiplier,
            DinoCharacterFoodDrainMultiplier = DinoCharacterFoodDrainMultiplier,
            WildDinoCharacterFoodDrainMultiplier = WildDinoCharacterFoodDrainMultiplier,
            TamedDinoCharacterFoodDrainMultiplier = TamedDinoCharacterFoodDrainMultiplier,
            MatingIntervalMultiplier = MatingIntervalMultiplier,
            EggHatchSpeedMultiplier = EggHatchSpeedMultiplier,
            MatingSpeedMultiplier = MatingSpeedMultiplier,
            BabyMatureSpeedMultiplier = BabyMatureSpeedMultiplier,
            BabyFoodConsumptionSpeedMultiplier = BabyFoodConsumptionSpeedMultiplier,
            BabyCuddleIntervalMultiplier = BabyCuddleIntervalMultiplier,
            BabyImprintingStatScaleMultiplier = BabyImprintingStatScaleMultiplier,
            BabyImprintAmountMultiplier = BabyImprintAmountMultiplier,
            AllowFlyerSpeedLeveling = AllowFlyerSpeedLeveling,
            AllowSpeedLeveling = AllowSpeedLeveling,
            SinglePlayerSettings = SinglePlayerSettings,
            AtlasSettings = AtlasSettings
        };

        if (withStatMultipliers && statMultipliers != null)
        {
            sm.statMultipliers = new double[Stats.StatsCount][];
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                sm.statMultipliers[s] = new double[4];
                for (int si = 0; si < 4; si++)
                {
                    sm.statMultipliers[s][si] = statMultipliers[s][si];
                }
            }
        }

        return sm;
    }

    /// <summary>
    /// Checks if critical values are zero and then sets them to one directly before they are used.
    /// This cannot be done directly after deserialization because these values can be multiplied later and can become zero.
    /// </summary>
    public void FixZeroValues()
    {
        if (TamingSpeedMultiplier == 0)
        {
            TamingSpeedMultiplier = 1;
        }

        if (WildDinoTorporDrainMultiplier == 0)
        {
            WildDinoTorporDrainMultiplier = 1;
        }

        if (MatingIntervalMultiplier == 0)
        {
            MatingIntervalMultiplier = 1;
        }

        if (EggHatchSpeedMultiplier == 0)
        {
            EggHatchSpeedMultiplier = 1;
        }

        if (MatingSpeedMultiplier == 0)
        {
            MatingSpeedMultiplier = 1;
        }

        if (BabyMatureSpeedMultiplier == 0)
        {
            BabyMatureSpeedMultiplier = 1;
        }

        if (BabyCuddleIntervalMultiplier == 0)
        {
            BabyCuddleIntervalMultiplier = 1;
        }

        if (BabyImprintAmountMultiplier == 0)
        {
            BabyImprintAmountMultiplier = 1;
        }
    }

    /// <summary>
    /// Index of additive taming multiplier in stat multipliers.
    /// </summary>
    public const int IndexTamingAdd = 0;
    /// <summary>
    /// Index of multiplicative taming multiplier in stat multipliers.
    /// </summary>
    public const int IndexTamingMult = 1;
    /// <summary>
    /// Index of domesticated level multiplier in stat multipliers.
    /// </summary>
    public const int IndexLevelDom = 2;
    /// <summary>
    /// Index of wild level multiplier in stat multipliers.
    /// </summary>
    public const int IndexLevelWild = 3;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
