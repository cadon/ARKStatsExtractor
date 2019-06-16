using ARKBreedingStats.species;
using SavegameToolkit;
using SavegameToolkit.Arrays;
using SavegameToolkit.Structs;
using SavegameToolkit.Types;
using SavegameToolkitAdditions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ARKBreedingStats
{

    public class ImportSavegame
    {
        private readonly Dictionary<string, string> nameReplacing;

        private static int statsCount = 12;

        private readonly ArkData arkData;

        private readonly float gameTime;

        private ImportSavegame(float gameTime)
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

            arkData = ArkDataReader.ReadFromFile(FileService.GetJsonPath(FileService.ArkDataJson));
        }

        public static async Task ImportCollectionFromSavegame(CreatureCollection creatureCollection, string filename, string serverName)
        {
            string[] rafts = { "Raft_BP_C", "MotorRaft_BP_C", "Barge_BP_C" };
            (GameObjectContainer gameObjectContainer, float gameTime) = await readSavegameFile(filename);

            IEnumerable<GameObject> tamedCreatureObjects = gameObjectContainer
                    .Where(o => o.IsCreature() && o.IsTamed() && !o.IsUnclaimedBaby() && !rafts.Contains(o.ClassString));

            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.ImportTribeNameFilter))
            {
                string[] filters = Properties.Settings.Default.ImportTribeNameFilter.Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToArray();

                tamedCreatureObjects = tamedCreatureObjects.Where(o =>
                {
                    string tribeName = o.GetPropertyValue<string>("TribeName", defaultValue: string.Empty);
                    return filters.Any(filter => tribeName.Contains(filter));
                });
            }

            ImportSavegame importSavegame = new ImportSavegame(gameTime);
            int? wildLevelStep = creatureCollection.getWildLevelStep();
            List<Creature> creatures = tamedCreatureObjects.Select(o => importSavegame.convertGameObject(o, wildLevelStep)).ToList();

            importCollection(creatureCollection, creatures, serverName);
        }

        private static async Task<(GameObjectContainer, float)> readSavegameFile(string fileName)
        {
            return await Task.Run(() =>
            {
                if (new FileInfo(fileName).Length > int.MaxValue)
                {
                    throw new Exception("Input file is too large.");
                }

                Stream stream = new MemoryStream(File.ReadAllBytes(fileName));

                ArkSavegame arkSavegame = new ArkSavegame();

                using (ArkArchive archive = new ArkArchive(stream))
                {
                    arkSavegame.ReadBinary(archive, ReadingOptions.Create()
                            .WithDataFiles(false)
                            .WithEmbeddedData(false)
                            .WithDataFilesObjectMap(false)
                            .WithObjectFilter(o => !o.IsItem && (o.Parent != null || o.Components.Any()))
                            .WithBuildComponentTree(true));
                }

                if (!arkSavegame.HibernationEntries.Any())
                {
                    return (arkSavegame, arkSavegame.GameTime);
                }

                List<GameObject> combinedObjects = arkSavegame.Objects;

                foreach (HibernationEntry entry in arkSavegame.HibernationEntries)
                {
                    ObjectCollector collector = new ObjectCollector(entry, 1);
                    combinedObjects.AddRange(collector.Remap(combinedObjects.Count));
                }

                return (new GameObjectContainer(combinedObjects), arkSavegame.GameTime);
            });
        }

        private static void importCollection(CreatureCollection creatureCollection, List<Creature> newCreatures, string serverName)
        {
            if (Properties.Settings.Default.importChangeCreatureStatus)
            {
                // mark creatures that are no longer present as unavailable
                var removedCreatures = creatureCollection.creatures.Where(c => c.status == CreatureStatus.Available).Except(newCreatures);
                foreach (var c in removedCreatures)
                    c.status = CreatureStatus.Unavailable;

                // mark creatures that re-appear as available (due to server transfer / obelisk / etc)
                var readdedCreatures = creatureCollection.creatures.Where(c => c.status == CreatureStatus.Unavailable || c.status == CreatureStatus.Obelisk).Intersect(newCreatures);
                foreach (var c in readdedCreatures)
                    c.status = CreatureStatus.Available;
            }

            newCreatures.ForEach(creature =>
            {
                creature.server = serverName;
            });
            creatureCollection.mergeCreatureList(newCreatures, true);
        }

        private Creature convertGameObject(GameObject creatureObject, int? levelStep)
        {
            GameObject statusObject = creatureObject.CharacterStatusComponent();

            string imprinterName = creatureObject.GetPropertyValue<string>("ImprinterName");
            string owner = string.IsNullOrWhiteSpace(imprinterName) ? creatureObject.GetPropertyValue<string>("TamerString") : imprinterName;

            int[] wildLevels = Enumerable.Repeat(-1, statsCount).ToArray(); // -1 is unknown
            int[] tamedLevels = new int[statsCount];

            for (int i = 0; i < statsCount; i++)
            {
                wildLevels[i] = statusObject.GetPropertyValue<ArkByteValue>("NumberOfLevelUpPointsApplied", i)?.ByteValue ?? 0;
            }
            wildLevels[(int)StatNames.Torpidity] = statusObject.GetPropertyValue<int>("BaseCharacterLevel", defaultValue: 1) - 1; // torpor

            for (int i = 0; i < statsCount; i++)
            {
                tamedLevels[i] = statusObject.GetPropertyValue<ArkByteValue>("NumberOfLevelUpPointsAppliedTamed", i)?.ByteValue ?? 0;
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
                ArkId = creatureObject.GetDinoId(),
                ArkIdImported = true,
                domesticatedAt = DateTime.Now, // TODO: possible to convert ingame-time to realtime?
                addedToLibrary = DateTime.Now,
                mutationsMaternal = creatureObject.GetPropertyValue<int>("RandomMutationsFemale"),
                mutationsPaternal = creatureObject.GetPropertyValue<int>("RandomMutationsMale")
            };

            // If it's a baby and still growing, work out growingUntil
            if (creatureObject.GetPropertyValue<bool>("bIsBaby") || !creatureObject.GetPropertyValue<bool>("bIsBaby") && !string.IsNullOrWhiteSpace(imprinterName))
            {
                int i = Values.V.speciesNames.IndexOf(convertedSpeciesName);
                double maturationTime = Values.V.species[i].breeding?.maturationTimeAdjusted ?? 0;
                float tamedTime = gameTime - (float)creatureObject.GetPropertyValue<double>("TamedAtTime");
                if (tamedTime < maturationTime - 120) // there seems to be a slight offset of one of these saved values, so don't display a creature as being in cooldown if it is about to leave it in the next 2 minutes
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
            var scores = Values.V.speciesNames.Select(n => new
            {
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
