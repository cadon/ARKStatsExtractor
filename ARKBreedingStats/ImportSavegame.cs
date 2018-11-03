using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SavegameToolkit;
using SavegameToolkit.Arrays;
using SavegameToolkit.Structs;
using SavegameToolkit.Types;
using SavegameToolkitAdditions;

namespace ARKBreedingStats {

    public class ImportSavegame
    {
        private readonly Dictionary<string, string> nameReplacing;

        private static readonly int[] asbStatsToSavegameIndex = { 0, 1, 3, 4, 7, 8, 9, 2 };

        private readonly ArkData arkData;

        private readonly float gameTime;

        public ImportSavegame(float gameTime)
        {
            this.gameTime = gameTime;

            // manual species-name fixing
            nameReplacing = new Dictionary<string, string> {
                    { "Paraceratherium", "Paracer" },
                    { "Ichthyosaurus", "Ichthy" },
                    { "Dire Bear", "Direbear" }
            };

            // add Blueprints of species (ark-tools doesn't convert e.g. the aberrant species)
            Regex r = new Regex(@"\/([^\/.]+)\.");
            foreach (Species s in Values.V.species)
            {
                Match m = r.Match(s.blueprintPath);
                if (!m.Success)
                    continue;
                string bpPart = m.Groups[1].Value + "_C";
                if (!nameReplacing.ContainsKey(bpPart))
                {
                    nameReplacing.Add(bpPart, s.name);
                }
            }

            arkData = ArkDataReader.ReadFromFile(@"json\ark_data.json");
        }

        public Creature ConvertGameObject(GameObject creatureObject, int? levelStep)
        {
            GameObject statusObject = creatureObject.CharacterStatusComponent();

            string imprinterName = creatureObject.GetPropertyValue<string>("ImprinterName");
            string owner = string.IsNullOrWhiteSpace(imprinterName) ? creatureObject.GetPropertyValue<string>("TamerString") : imprinterName;

            int[] wildLevels = Enumerable.Repeat(-1, 8).ToArray(); // -1 is unknown
            int[] tamedLevels = new int[8];

            for (int i = 0; i < 8; i++)
            {
                ArkByteValue value = statusObject.GetPropertyValue<ArkByteValue>("NumberOfLevelUpPointsApplied", asbStatsToSavegameIndex[i]);
                if (value != null)
                    wildLevels[i] = value.ByteValue;
            }
            wildLevels[7] = statusObject.GetPropertyValue<int>("BaseCharacterLevel") - 1; // torpor

            for (int i = 0; i < 8; i++)
            {
                ArkByteValue value = statusObject.GetPropertyValue<ArkByteValue>("NumberOfLevelUpPointsAppliedTamed", asbStatsToSavegameIndex[i]);
                if (value != null)
                    tamedLevels[i] = value.ByteValue;
            }

            string convertedSpeciesName = convertSpecies(creatureObject.GetNameForCreature(arkData) ?? creatureObject.ClassString);

            float ti = statusObject.GetPropertyValue<float>("TamedIneffectivenessModifier", defaultValue: float.NaN);
            double te = 1f / (1 + (!float.IsNaN(ti) ? ti : creatureObject.GetPropertyValue<float>("TameIneffectivenessModifier")));

            Creature creature = new Creature(convertedSpeciesName,
                    creatureObject.GetPropertyValue<string>("TamedName"), owner, creatureObject.GetPropertyValue<string>("TribeName"),
                    creatureObject.IsFemale() ? Sex.Female : Sex.Male,
                    wildLevels, tamedLevels, te,
                    !string.IsNullOrWhiteSpace(creatureObject.GetPropertyValue<string>("ImprinterName")),
                    statusObject.GetPropertyValue<float>("DinoImprintingQuality"),
                    levelStep
            )
            {
                imprinterName = creatureObject.GetPropertyValue<string>("ImprinterName"),
                guid = Utils.ConvertArkIdToGuid(creatureObject.GetDinoId()),
                domesticatedAt = DateTime.Now, // TODO: convert ingame-time to realtime?
                addedToLibrary = DateTime.Now,
                mutationsMaternal = creatureObject.GetPropertyValue<int>("RandomMutationsFemale"),
                mutationsPaternal = creatureObject.GetPropertyValue<int>("RandomMutationsMale")
            };

            // If it's a baby and still growing, work out growingUntil
            if (creatureObject.GetPropertyValue<bool>("bIsBaby") || !creatureObject.GetPropertyValue<bool>("bIsBaby") && !string.IsNullOrWhiteSpace(imprinterName))
            {
                int i = Values.V.speciesNames.IndexOf(convertedSpeciesName);
                double maturationTime = Values.V.species[i].breeding.maturationTimeAdjusted;
                float tamedTime = gameTime - (float)creatureObject.GetPropertyValue<double>("TamedAtTime");
                if (tamedTime < maturationTime)
                    creature.growingUntil = DateTime.Now + TimeSpan.FromSeconds(maturationTime - tamedTime);
            }

            // Ancestor linking is done later after entire collection is formed - here we just set the guids
            ArkArrayStruct femaleAncestors = creatureObject.GetPropertyValue<IArkArray, ArkArrayStruct>("DinoAncestors");
            StructPropertyList femaleAncestor = (StructPropertyList)femaleAncestors?.LastOrDefault();
            if (femaleAncestor != null)
            {
                creature.motherGuid = Utils.ConvertArkIdToGuid(GameObjectExtensions.CreateDinoId(
                        femaleAncestor.GetPropertyValue<int>("FemaleDinoID1"),
                        femaleAncestor.GetPropertyValue<int>("FemaleDinoID2")));
                creature.motherName = femaleAncestor.GetPropertyValue<string>("FemaleName");
                creature.isBred = true;
            }
            ArkArrayStruct maleAncestors = creatureObject.GetPropertyValue<IArkArray, ArkArrayStruct>("DinoAncestorsMale");
            StructPropertyList maleAncestor = (StructPropertyList)maleAncestors?.LastOrDefault();
            if (maleAncestor != null)
            {
                creature.fatherGuid = Utils.ConvertArkIdToGuid(GameObjectExtensions.CreateDinoId(
                        maleAncestor.GetPropertyValue<int>("MaleDinoID1"),
                        maleAncestor.GetPropertyValue<int>("MaleDinoID2")));
                creature.fatherName = maleAncestor.GetPropertyValue<string>("MaleName");
                creature.isBred = true;
            }

            creature.colors = new int[6];
            for (int i = 0; i < 6; i++)
            {
                creature.colors[i] = colorModulo(creatureObject.GetPropertyValue<ArkByteValue>("ColorSetIndices", i)?.ByteValue ?? 0);
            }

            bool isDead = creatureObject.GetPropertyValue<bool>("bIsDead");
            if (isDead)
            {
                creature.status = CreatureStatus.Dead; // dead is always dead
            }

            if (!isDead && creature.status == CreatureStatus.Dead)
            {
                creature.status = CreatureStatus.Unavailable; // if found alive when marked dead, mark as unavailable
            }

            creature.recalculateAncestorGenerations();
            creature.recalculateCreatureValues(levelStep);

            return creature;
        }

        private string convertSpecies(string species)
        {
            // some creatures are called differently ingame than in the extracted save-files
            if (nameReplacing.ContainsKey(species))
            {
                return nameReplacing[species];
            }

            // Use fuzzy matching to convert between the two slightly different naming schemes
            // This doesn't handle spaces well, so we simply remove them and then it works perfectly
            var scores = Values.V.speciesNames.Select(n => new {
                Score = DiceCoefficient.diceCoefficient(n.Replace(" ", string.Empty), species.Replace(" ", string.Empty)),
                Name = n
            });
            return scores.OrderByDescending(o => o.Score).First().Name;
        }

        private static int colorModulo(int color)
        {
            // color ids ingame can be stored as higher numbers, it appears the colors just repeat
            return (color - 1) % 56 + 1;
        }
    }

}
