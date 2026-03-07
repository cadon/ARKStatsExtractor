using ARKBreedingStats.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARKBreedingStats.Library;

/// <summary>
/// This class is used to store creature-values of creatures that couldn't be extracted, to store their values temporarily until the issue is solved
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public class CreatureValues
{
    /// <summary>
    /// Used to identify the species
    /// </summary>
    [JsonProperty]
    public string speciesBlueprint { get; set; }
    private Species _species;
    [JsonProperty]
    public Guid guid { get; set; }
    /// <summary>
    /// Real Ark Id, not the one displayed ingame. Can only be set by importing a creature.
    /// </summary>
    [JsonProperty]
    public long ARKID { get; set; }
    /// <summary>
    /// Ark Id like it is shown in game. Is not unique, because it's built by two 32 bit integers concatenated as strings.
    /// </summary>
    [JsonProperty]
    public string ArkIdInGame { get; set; }
    [JsonProperty]
    public string name { get; set; }
    [JsonProperty]
    public Sex sex { get; set; }
    [JsonProperty]
    public double[] statValues { get; set; } = new double[Stats.StatsCount];
    [JsonProperty]
    public int[] levelsWild { get; set; } = new int[Stats.StatsCount];
    [JsonProperty]
    public int[] levelsMut { get; set; } = new int[Stats.StatsCount];
    [JsonProperty]
    public int[] levelsDom { get; set; } = new int[Stats.StatsCount];
    [JsonProperty]
    public int level { get; set; }
    [JsonProperty]
    public double tamingEffMin { get; set; }
    [JsonProperty]
    public double tamingEffMax { get; set; }
    [JsonProperty]
    public double imprintingBonus { get; set; }
    [JsonProperty]
    public bool isTamed { get; set; }
    [JsonProperty]
    public bool isBred { get; set; }
    [JsonProperty]
    public string owner { get; set; }
    [JsonProperty]
    public string imprinterName { get; set; }
    [JsonProperty]
    public string tribe { get; set; }
    [JsonProperty]
    public string server { get; set; }
    [JsonProperty]
    public string note { get; set; }
    [JsonProperty]
    public long fatherArkId { get; set; } // used when importing creatures, parents are indicated by this id
    [JsonProperty]
    public long motherArkId { get; set; }
    [JsonProperty]
    public Guid motherGuid { get; set; }
    [JsonProperty]
    public Guid fatherGuid { get; set; }
    private Creature mother;
    private Creature father;
    [JsonProperty]
    public DateTime? growingUntil { get; set; }
    [JsonProperty]
    public DateTime? cooldownUntil { get; set; }
    [JsonProperty]
    public DateTime? domesticatedAt { get; set; }
    [JsonProperty]
    public CreatureFlags flags { get; set; }
    [JsonProperty]
    public int mutationCounter { get; set; }
    [JsonProperty]
    public int mutationCounterMother { get; set; }
    [JsonProperty]
    public int mutationCounterFather { get; set; }
    [JsonIgnore]
    public byte[] colorIDs { get; set; } = new byte[Ark.ColorRegionCount];
    [JsonProperty("colorIDs", DefaultValueHandling = DefaultValueHandling.Ignore)]
    private int[] colorIDsSerialization
    {
        set => colorIDs = value?.Select(i => (byte)i).ToArray() ?? [];
        get => colorIDs?.Select(i => (int)i).ToArray() ?? [];
    }
    /// <summary>
    /// Some color ids cannot be determined uniquely because of equal color values.
    /// If this property is set it contains the other possible color ids.
    /// </summary>
    [JsonIgnore]
    public byte[] ColorIdsAlsoPossible { get; set; }
    [JsonProperty("altCol", DefaultValueHandling = DefaultValueHandling.Ignore)]
    private int[] ColorIdsAlsoPossibleSerialization
    {
        set => ColorIdsAlsoPossible = value?.Select(i => (byte)i).ToArray() ?? [];
        get => ColorIdsAlsoPossible?.Select(i => (int)i).ToArray() ?? [];
    }

    [JsonProperty("traits", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<CreatureTrait> Traits { get; set; }

    public CreatureValues() { }

    public CreatureValues(Species species, string name, string owner, string tribe, Sex sex,
            double[] statValues, int level, double tamingEffMin, double tamingEffMax, bool isTamed, bool isBred, double imprintingBonus, CreatureFlags flags,
            Creature mother, Creature father)
    {
        Species = species;
        this.name = name;
        this.owner = owner;
        this.tribe = tribe;
        this.sex = sex;
        this.statValues = statValues;
        this.level = level;
        this.tamingEffMin = tamingEffMin;
        this.tamingEffMax = tamingEffMax;
        this.isTamed = isTamed;
        this.isBred = isBred;
        this.imprintingBonus = imprintingBonus;
        this.flags = flags;
        Mother = mother;
        Father = father;
    }

    public Creature Mother
    {
        get => mother;
        set
        {
            mother = value;
            motherArkId = mother?.ArkId ?? 0;
            motherGuid = mother?.guid ?? Guid.Empty;
        }
    }

    public Creature Father
    {
        get => father;
        set
        {
            father = value;
            fatherArkId = father?.ArkId ?? 0;
            fatherGuid = father?.guid ?? Guid.Empty;
        }
    }

    public Species Species
    {
        set
        {
            _species = value;
            if (value != null)
            {
                speciesBlueprint = value.blueprintPath;
            }
        }
        get => _species;
    }
}
