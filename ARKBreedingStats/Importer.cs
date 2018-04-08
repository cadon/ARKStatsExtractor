using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    class Importer
    {
        private const int UNCLAIMED_TEAM_ID = 2000000000;
        private const string RAFT_SPECIES = "Raft";

        private List<ImportedCreature> loadedCreatures;
        private Dictionary<string, string> nameReplacing;

        public Importer(string classesJson)
        {
            ClassesJson = classesJson;
            BasePath = Path.GetDirectoryName(ClassesJson);

            nameReplacing = new Dictionary<string, string>(){
                { "Paraceratherium", "Paracer" },
                { "Ichthyosaurus", "Ichthy" },
                { "Bigfoot_Character_BP_Aberrant_C", "Aberrant Gigantopithecus" }, // TODO more general fix?
                { "Dire Bear", "Direbear" }
            };
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
                try
                {
                    return (T)(ser.ReadObject(file));
                }
                catch (Exception e)
                {
                    MessageBox.Show("File Couldn't be opened or read.\nErrormessage:\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return default(T);
            }
        }

        public void LoadAllSpecies()
        {
            loadedCreatures = new List<ImportedCreature>();
            foreach (var cls in Mappings.Values)
            {
                var creatureFile = Path.Combine(BasePath, cls + ".json");
                LoadCreaturesFrom(creatureFile);
            }
        }

        public List<Creature> ConvertLoadedCreatures(int? levelStep)
        {
            return loadedCreatures
                .Where(lc => !String.Equals(lc.Type, RAFT_SPECIES))  // no rafts, thanks... dunno ID for motorboat yet
                .Where(lc => lc.Team != UNCLAIMED_TEAM_ID)  // this avoids unclaimed births which may or may not be dead
                .Select(lc => ConvertCreature(lc, levelStep))  // produce a creature suitable for the rest of the app
                .ToList();
        }

        private Creature ConvertCreature(ImportedCreature lc, int? levelStep)
        {
            var owner = String.IsNullOrWhiteSpace(lc.Imprinter) ? lc.Tamer : lc.Imprinter;
            int[] wildLevels = new int[] { -1, -1, -1, -1, -1, -1, -1, -1 }; // -1 is unknown
            int[] tamedLevels = new int[8];
            if (lc.WildLevels != null) wildLevels = ConvertLevels(lc.WildLevels, lc.BaseLevel - 1);
            if (lc.TamedLevels != null) tamedLevels = ConvertLevels(lc.TamedLevels);

            string convertedSpeciesName = ConvertSpecies(lc.Type);

            // fix for wrong TE (bug in ark-tools) TODO. got it fixed in ark-tools?
            // if it got fixed in ark-tools, use
            // double te = lc.TamingEffectiveness;
            double te = 1 / (2 - lc.TamingEffectiveness);

            var creature = new Creature(convertedSpeciesName, lc.Name, owner, lc.Tribe,
                lc.Female ? Sex.Female : Sex.Male,
                wildLevels, tamedLevels,
                te, !string.IsNullOrWhiteSpace(lc.Imprinter), lc.ImprintingQuality, levelStep);

            creature.imprinterName = lc.Imprinter;
            creature.guid = Utils.ConvertIdToGuid(lc.Id);
            creature.domesticatedAt = DateTime.Now; // TODO: convert ingame-time to realtime?
            creature.addedToLibrary = DateTime.Now;
            creature.mutationsMaternal = lc.MutationsFemaleLine;
            creature.mutationsPaternal = lc.MutationsMaleLine;

            // If it's a baby and still growing, work out growingUntil
            if (lc.Baby || (!lc.Baby && !String.IsNullOrWhiteSpace(lc.Imprinter)))
            {
                int i = Values.V.speciesNames.IndexOf(convertedSpeciesName);
                double maturationTime = Values.V.species[i].breeding.maturationTimeAdjusted;
                if (lc.TamedTime < maturationTime)
                    creature.growingUntil = DateTime.Now + TimeSpan.FromSeconds(maturationTime - lc.TamedTime);
            }

            // Ancestor linking is done later after entire collection is formed - here we just set the guids
            if (lc.FemaleAncestors != null && lc.FemaleAncestors.Count > 0)
            {
                var femaleAncestor = lc.FemaleAncestors.Last();
                creature.motherGuid = Utils.ConvertIdToGuid(femaleAncestor.FemaleId);
                creature.motherName = femaleAncestor.FemaleName;
                creature.isBred = true;
            }
            if (lc.MaleAncestors != null && lc.MaleAncestors.Count > 0)
            {
                var maleAncestor = lc.MaleAncestors.Last();
                creature.fatherGuid = Utils.ConvertIdToGuid(maleAncestor.MaleId);
                creature.fatherName = maleAncestor.MaleName;
                creature.isBred = true;
            }

            if (lc.Colors != null)
                creature.colors = ConvertColors(lc.Colors);

            if (lc.Dead) creature.status = CreatureStatus.Dead; // dead is always dead
            if (!lc.Dead && creature.status == CreatureStatus.Dead) creature.status = CreatureStatus.Unavailable; // if found alive when marked dead, mark as unavailable

            creature.recalculateAncestorGenerations();
            creature.recalculateCreatureValues(levelStep);

            return creature;
        }

        private string ConvertSpecies(string species)
        {
            // some creatures are called differently ingame than in the extracted save-files
            if (nameReplacing.ContainsKey(species))
                species = nameReplacing[species];

            // Use fuzzy matching to convert between the two slightly different naming schemes
            // This doesn't handle spaces well, so we simply remove them and then it works perfectly
            var scores = Values.V.speciesNames.Select(n => new { Score = DiceCoefficient.diceCoefficient(n.Replace(" ", ""), species.Replace(" ", "")), Name = n });
            //return scores.OrderByDescending(o => o.Score).First().Name;
            var ttt = scores.OrderByDescending(o => o.Score).First().Name;
            return ttt;
        }

        private static int[] ConvertColors(Colors colors)
        {
            return new int[] { colorModulo(colors.Color0), colorModulo(colors.Color1), colorModulo(colors.Color2), colorModulo(colors.Color3), colorModulo(colors.Color4), colorModulo(colors.Color5) };
        }

        private static int colorModulo(int color)
        {
            // color ids ingame can be stored as higher numbers, it appears the colors just repeat
            return ((color - 1) % 56) + 1;
        }

        private static int[] ConvertLevels(Levels levels, int addTorpor = 0)
        {
            return new int[] { levels.Health, levels.Stamina, levels.Oxygen, levels.Food, levels.Weight, levels.Melee, levels.Speed, levels.Torpor + addTorpor };
        }

        private void LoadCreaturesFrom(string creatureFile)
        {
            var species = Deserialise<List<ImportedCreature>>(creatureFile);
            loadedCreatures.AddRange(species);
        }

        [DataContract]
        class ClassNamePair
        {
            [DataMember(Name = "cls")] public string Cls { get; set; }
            [DataMember(Name = "name")] public string Name { get; set; }
        }

        [DataContract]
        class ImportedCreature
        {
            [DataMember(Name = "tamed")] public bool Tamed { get; set; }
            [DataMember(Name = "type")] public string Type { get; set; }
            [DataMember(Name = "name")] public string Name { get; set; }
            [DataMember(Name = "female")] public bool Female { get; set; }
            [DataMember(Name = "dead")] public bool Dead { get; set; }
            [DataMember(Name = "baby")] public bool Baby { get; set; }
            [DataMember(Name = "mutationsMaleLine")] public int MutationsMaleLine { get; set; }
            [DataMember(Name = "mutationsFemaleLine")] public int MutationsFemaleLine { get; set; }
            [DataMember(Name = "wildLevels")] public Levels WildLevels { get; set; }
            [DataMember(Name = "tamedLevels")] public Levels TamedLevels { get; set; }
            [DataMember(Name = "tamingEffectivness")] public double TamingEffectiveness { get; set; }
            [DataMember(Name = "imprintingQuality")] public double ImprintingQuality { get; set; }
            [DataMember(Name = "tamer")] public string Tamer { get; set; }
            [DataMember(Name = "team")] public int Team { get; set; }
            [DataMember(Name = "tamedTime")] public double TamedTime { get; set; }
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
