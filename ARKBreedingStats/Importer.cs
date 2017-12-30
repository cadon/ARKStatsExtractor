using DuoVia.FuzzyStrings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    class Importer
    {
        private List<LoadedCreature> loadedCreatures;

        public Importer(string classesJson)
        {
            ClassesJson = classesJson;
            BasePath = Path.GetDirectoryName(ClassesJson);
        }

        public string ClassesJson { get; }
        public string BasePath { get; }

        public Dictionary<string, string> Mappings { get; private set; }

        public void ParseClasses()
        {
            var list = Deserialise<List<ClassNamePair>>(ClassesJson);
            Mappings = list.ToDictionary(pair => pair.Cls.Contains("BP_Ocean_C") ? pair.Name + "Ocean" : pair.Name, pair => pair.Cls);
        }

        private T Deserialise<T>(string fileName)
        {
            using (var file = File.OpenRead(fileName))
            {
                var ser = new DataContractJsonSerializer(typeof(T));
                return (T)(ser.ReadObject(file));
            }
        }

        public void LoadAllSpecies()
        {
            loadedCreatures = new List<LoadedCreature>();
            foreach (var cls in Mappings.Values)
            {
                var creatureFile = Path.Combine(BasePath, cls + ".json");
                LoadCreaturesFrom(creatureFile);
            }
        }

        public List<Creature> ConvertLoadedCreatures()
        {
            return loadedCreatures.Select(lc => ConvertCreatures(lc)).ToList();
        }

        private Creature ConvertCreatures(LoadedCreature lc)
        {
            var owner = String.IsNullOrWhiteSpace(lc.Imprinter) ? lc.Tamer : lc.Imprinter;
            var wildLevels = ConvertLevels(lc.WildLevels, lc.BaseLevel-1);
            var tamedLevels = ConvertLevels(lc.TamedLevels);

            var creature = new Creature(ConvertSpecies(lc.Type), lc.Name, owner, lc.Tribe,
                lc.Female ? Sex.Female : Sex.Male,
                wildLevels, tamedLevels,
                lc.TamingEffectiveness, !String.IsNullOrWhiteSpace(lc.Imprinter), lc.ImprintingQuality);

            creature.guid = ConvertIdToGuid(lc.Id);

            if (lc.FemaleAncestors != null && lc.FemaleAncestors.Count > 0)
                creature.motherGuid = ConvertIdToGuid(lc.FemaleAncestors.Last().FemaleId);
            if (lc.MaleAncestors != null && lc.MaleAncestors.Count > 0)
                creature.fatherGuid = ConvertIdToGuid(lc.MaleAncestors.Last().MaleId);

            creature.colors = ConvertColors(lc.Colors);

            // mother and father linking is done later after entire collection is formed

            creature.recalculateAncestorGenerations();
            creature.recalculateCreatureValues();

            return creature;
        }

        private string ConvertSpecies(string species)
        {
            // Use fuzzy matching to convert between the two slightly different naming schemes
            var scores = Values.V.speciesNames.Select(n => new { Score = n.Replace(" ", "").FuzzyMatch(species.Replace(" ", "")), Name = n });
            return scores.OrderByDescending(o => o.Score).First().Name;
        }

        private int[] ConvertColors(Colors colors)
        {
            return new int[] { colors.Color0, colors.Color1, colors.Color2, colors.Color3, colors.Color4, colors.Color5 };
        }

        private Guid ConvertIdToGuid(long id)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(id).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        private int[] ConvertLevels(Levels levels, int addTorpor=0)
        {
            return new int[] { levels.Health, levels.Stamina, levels.Oxygen, levels.Food, levels.Weight, levels.Melee, levels.Speed, levels.Torpor + addTorpor };
        }

        private void LoadCreaturesFrom(string creatureFile)
        {
            var species = Deserialise<List<LoadedCreature>>(creatureFile);
            loadedCreatures.AddRange(species);
        }

        [DataContract]
        class ClassNamePair
        {
            [DataMember(Name = "cls")] public string Cls { get; set; }
            [DataMember(Name = "name")] public string Name { get; set; }
        }

        [DataContract]
        class LoadedCreature
        {
            [DataMember(Name = "tamed")] public bool Tamed { get; set; }
            [DataMember(Name = "type")] public string Type { get; set; }
            [DataMember(Name = "name")] public string Name { get; set; }
            [DataMember(Name = "female")] public bool Female { get; set; }
            [DataMember(Name = "wildLevels")] public Levels WildLevels { get; set; }
            [DataMember(Name = "tamedLevels")] public Levels TamedLevels { get; set; }
            [DataMember(Name = "tamingEffectivness")] public double TamingEffectiveness { get; set; }
            [DataMember(Name = "imprintingQuality")] public double ImprintingQuality { get; set; }
            [DataMember(Name = "tamer")] public string Tamer { get; set; }
            [DataMember(Name = "imprinter")] public string Imprinter { get; set; }
            [DataMember(Name = "tribe")] public string Tribe { get; set; }
            [DataMember(Name = "tamedOnServerName")] public string TamedOnServerName { get; set; }
            [DataMember(Name = "baseLevel")] public int BaseLevel { get; set; }
            [DataMember(Name = "id")] public long Id { get; set; }
            [DataMember(Name = "femaleAncestors")] public List<Ancestor> FemaleAncestors { get; set; }
            [DataMember(Name = "maleAncestors")] public List<Ancestor> MaleAncestors { get; set; }
            [DataMember(Name = "colorSetIndices")] public Colors Colors { get; set; }
        }

        [DataContract]
        class Levels
        {
            [DataMember(Name = "health")] public int Health { get; set; }
            [DataMember(Name = "stamina")] public int Stamina { get; set; }
            [DataMember(Name = "torpor")] public int Torpor { get; set; }
            [DataMember(Name = "oxygen")] public int Oxygen { get; set; }
            [DataMember(Name = "food")] public int Food { get; set; }
            [DataMember(Name = "water")] public int Water { get; set; }
            [DataMember(Name = "temperature")] public int Temperature { get; set; }
            [DataMember(Name = "weight")] public int Weight { get; set; }
            [DataMember(Name = "melee")] public int Melee { get; set; }
            [DataMember(Name = "speed")] public int Speed { get; set; }
            [DataMember(Name = "fortitude")] public int Fortitude { get; set; }
            [DataMember(Name = "crafting")] public int Crafting { get; set; }
        }

        [DataContract]
        class Ancestor
        {
            [DataMember(Name = "maleName")] public string MaleName { get; set; }
            [DataMember(Name = "maleId")] public long MaleId { get; set; }
            [DataMember(Name = "femaleName")] public string FemaleName { get; set; }
            [DataMember(Name = "femaleId")] public long FemaleId { get; set; }
        }

        [DataContract]
        class Colors
        {
            [DataMember(Name = "0")] public int Color0 { get; set; }
            [DataMember(Name = "1")] public int Color1 { get; set; }
            [DataMember(Name = "2")] public int Color2 { get; set; }
            [DataMember(Name = "3")] public int Color3 { get; set; }
            [DataMember(Name = "4")] public int Color4 { get; set; }
            [DataMember(Name = "5")] public int Color5 { get; set; }
        }
    }
}
