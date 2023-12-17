using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ARKBreedingStats.importExportGun
{
    /// <summary>
    /// Structure of the export file created by the export gun mod.
    /// </summary>
    [JsonObject]
    internal class ExportGunCreatureFile
    {
        public int Version { get; set; } = 1;
        public string DinoName { get; set; }
        public string SpeciesName { get; set; }
        public string TribeName { get; set; }
        public string TamerString { get; set; }
        public string OwningPlayerName { get; set; }
        public string ImprinterName { get; set; }
        public int OwningPlayerID { get; set; }
        public string DinoID1 { get; set; }
        public string DinoID2 { get; set; }
        [JsonIgnore]
        public int DinoId1Int { get => int.TryParse(DinoID1, out var id) ? id : 0; set => DinoID1 = value.ToString(); }
        [JsonIgnore]
        public int DinoId2Int { get => int.TryParse(DinoID2, out var id) ? id : 0; set => DinoID2 = value.ToString(); }
        public Ancestry Ancestry { get; set; }
        public string BlueprintPath { get; set; }
        public Stat[] Stats { get; set; }

        [JsonIgnore]
        public byte[] ColorIds;
        [JsonProperty]
        private int[] ColorSetIndices
        {
            set => ColorIds = value?.Select(i => (byte)i).ToArray();
            get => ColorIds?.Select(i => (int)i).ToArray();
        }

        public Dictionary<byte, float[]> ColorSetValues { get; set; }
        public bool IsFemale { get; set; }
        public float NextAllowedMatingTimeDuration { get; set; }
        public float BabyAge { get; set; }
        public bool MutagenApplied { get; set; }
        public bool Neutered { get; set; }
        public int RandomMutationsMale { get; set; }
        public int RandomMutationsFemale { get; set; }
        /// <summary>
        /// Hash of the server multipliers, used to make sure the stat multipliers are from this server when importing via the export gun mod.
        /// </summary>
        public string ServerMultipliersHash { get; set; }
        public float TameEffectiveness { get; set; }
        public int BaseCharacterLevel { get; set; }
        public float DinoImprintingQuality { get; set; }
    }

    [JsonObject]
    internal class Ancestry
    {
        public string MaleName { get; set; }
        public string MaleDinoId1 { get; set; }
        public string MaleDinoId2 { get; set; }
        public string FemaleName { get; set; }
        public string FemaleDinoId1 { get; set; }
        public string FemaleDinoId2 { get; set; }
        [JsonIgnore]
        public int MaleDinoId1Int { get => int.TryParse(MaleDinoId1, out var id) ? id : 0; set => MaleDinoId1 = value.ToString(); }
        [JsonIgnore]
        public int MaleDinoId2Int { get => int.TryParse(MaleDinoId2, out var id) ? id : 0; set => MaleDinoId2 = value.ToString(); }
        [JsonIgnore]
        public int FemaleDinoId1Int { get => int.TryParse(FemaleDinoId1, out var id) ? id : 0; set => FemaleDinoId1 = value.ToString(); }
        [JsonIgnore]
        public int FemaleDinoId2Int { get => int.TryParse(FemaleDinoId2, out var id) ? id : 0; set => FemaleDinoId2 = value.ToString(); }
    }

    [JsonObject]
    internal class Stat
    {
        public int Wild { get; set; }
        public int Tamed { get; set; }
        public int Mutated { get; set; }
        public float Value { get; set; }
    }
}
