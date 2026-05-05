using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using FluentFTP.Helpers;
using Newtonsoft.Json;
using Python.Runtime;
using SavegameToolkit;
using SavegameToolkit.Arrays;
using SavegameToolkit.Structs;
using SavegameToolkit.Types;
using SavegameToolkitAdditions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{

    public class ImportSavegame
    {
        private readonly float _gameTime;

        private ImportSavegame(float gameTime)
        {
            _gameTime = gameTime;
        }

        public static async Task ImportCollectionFromSavegame(CreatureCollection creatureCollection, string filename, string serverName)
        {
            byte[] first16Bytes = new byte[16];
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                fs.ReadExactly(first16Bytes, 0, 16);
            }
            string checkString = System.Text.ASCIIEncoding.ASCII.GetString(first16Bytes);
            bool isAsaSavegame = checkString.Contains("SQLite format");
            var creatures = await (isAsaSavegame ? ImportCollectionFromAsaSavegame(creatureCollection, filename, serverName) : ImportCollectionFromAseSavegame(creatureCollection, filename, serverName));

            ArkName.ClearCache();

            // if there are creatures with unknown species, check if the according mod-file is available
            var unknownSpeciesCreatures = creatures.Where(c => c.Species == null).ToArray();

            if (!unknownSpeciesCreatures.Any()
                || Properties.Settings.Default.IgnoreUnknownBlueprintsOnSaveImport
                || MessageBox.Show("The species of " + unknownSpeciesCreatures.Length + " creature" + (unknownSpeciesCreatures.Length != 1 ? "s" : "") + " is not recognized, probably because they are from a mod that is not loaded.\n"
                                  + "The unrecognized species-classes are as follows, all the according creatures cannot be imported:\n\n" + string.Join("\n", unknownSpeciesCreatures.Select(c => c.name).Distinct().ToArray())
                                  + "\n\nTo import the unrecognized creatures, you first need mod values-files, see Settings - Mod value manager… if the mod value is available\n\n"
                                  + "Do you want to import the recognized creatures? If you click no, nothing is imported.",
                                  "Unrecognized species while importing savegame", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                                 ) == DialogResult.Yes
               )
            {
                ImportCollection(creatureCollection, creatures.Where(c => c.Species != null).ToList(), serverName);
            }
        }

        private static async Task<Creature[]> ImportCollectionFromAsaSavegame(CreatureCollection creatureCollection, string filename, string serverName)
        {
            var importUnclaimedBabies = Properties.Settings.Default.SaveFileImportUnclaimedBabies;
            var saveImportCryo = Properties.Settings.Default.SaveImportCryo;

            // TODO: properly set up python engine & make sure arkparse is installed
            if (!PythonEngine.IsInitialized) PythonEngine.Initialize();
            Creature[] creatures;
            using (Py.GIL())
            {
                dynamic pathlib = Py.Import("pathlib");
                dynamic asa_save = Py.Import("arkparse.saves.asa_save");
                dynamic path = pathlib.Path(filename);
                dynamic save = asa_save.AsaSave(path);
                float gameTime = save.save_context.game_time;
                dynamic dino_api = Py.Import("arkparse.api.dino_api");
                dynamic DinoApi = dino_api.DinoApi(save);
                PyDict creatureObjects = DinoApi.get_all_tamed();

                IEnumerable<dynamic> tamedCreatures = creatureObjects.Values().Where(o => (importUnclaimedBabies || !IsUnclaimedBaby(o)) && (saveImportCryo || !IsInCryo(o)));

                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.ImportTribeNameFilter))
                {
                    string[] filters = Properties.Settings.Default.ImportTribeNameFilter.Split(',')
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .ToArray();

                    if (filters.Any())
                    {
                        tamedCreatures = tamedCreatures.Where(c =>
                        {
                            return filters.Any(filter => c.owner.tribe.Contains(filter));
                        });
                    }
                }

                ImportSavegame importSavegame = new ImportSavegame(gameTime);
                int? wildLevelStep = creatureCollection.getWildLevelStep();
                creatures = tamedCreatures.Select(o => (Creature)importSavegame.ConvertTamedDino(o, wildLevelStep)).Where(c => c != null).ToArray();
            }

            return creatures;
        }

        private static async Task<Creature[]> ImportCollectionFromAseSavegame(CreatureCollection creatureCollection, string filename, string serverName)
        {
            (GameObjectContainer gameObjectContainer, float gameTime) = await Task.Run(() => ReadSavegameFile(filename));
            var ignoreClasses = Values.V.IgnoreSpeciesClassesOnImport;
            var importUnclaimedBabies = Properties.Settings.Default.SaveFileImportUnclaimedBabies;

            IEnumerable<GameObject> tamedCreatureObjects = gameObjectContainer
                    .Where(o => o.IsCreature()
                    && o.IsTamed()
                    && (importUnclaimedBabies || (o.IsInCryo && Properties.Settings.Default.SaveImportCryo) || !o.IsUnclaimedBaby())
                    && !ignoreClasses.Contains(o.ClassString));

            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.ImportTribeNameFilter))
            {
                string[] filters = Properties.Settings.Default.ImportTribeNameFilter.Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToArray();

                if (filters.Any())
                {
                    tamedCreatureObjects = tamedCreatureObjects.Where(o =>
                    {
                        string tribeName = o.GetPropertyValue<string>("TribeName", defaultValue: string.Empty);
                        return filters.Any(filter => tribeName.Contains(filter));
                    });
                }
            }

            ImportSavegame importSavegame = new ImportSavegame(gameTime);
            int? wildLevelStep = creatureCollection.getWildLevelStep();
            return tamedCreatureObjects.Select(o => importSavegame.ConvertGameObject(o, wildLevelStep)).Where(c => c != null).ToArray();
        }

        private static (GameObjectContainer, float) ReadSavegameFile(string fileName)
        {
            ArkSavegame arkSavegame = new ArkSavegame();

            bool PredicateCreatures(GameObject o) => !o.IsItem && (o.Parent != null || o.Components.Any());
            bool PredicateCreaturesAndCryopods(GameObject o) => (!o.IsItem && (o.Parent != null || o.Components.Any())) || o.ClassString.Contains("Cryopod") || o.ClassString.Contains("SoulTrap_") || o.ClassString.Contains("Vivarium_");

            var largeFile = new FileInfo(fileName).Length > int.MaxValue;
            using (var stream = largeFile ? (Stream)new FileStream(fileName, FileMode.Open) : new MemoryStream(File.ReadAllBytes(fileName)))
            using (ArkArchive archive = new ArkArchive(stream))
            {
                arkSavegame.ReadBinary(archive, ReadingOptions.Create()
                    .WithDataFiles(false)
                    .WithEmbeddedData(false)
                    .WithDataFilesObjectMap(false)
                    .WithObjectFilter(Properties.Settings.Default.SaveImportCryo
                        ? new Predicate<GameObject>(PredicateCreaturesAndCryopods)
                        : new Predicate<GameObject>(PredicateCreatures))
                    .WithCryopodCreatures(Properties.Settings.Default.SaveImportCryo)
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
        }

        private static void ImportCollection(CreatureCollection creatureCollection, List<Creature> newCreatures, string serverName)
        {
            if (creatureCollection.changeCreatureStatusOnSavegameImport)
            {
                // mark creatures that are no longer present as unavailable
                var removedCreatures = creatureCollection.creatures.Where(c =>
                        (c.Status == CreatureStatus.Available || c.Status == CreatureStatus.Cryopod) && c.server == serverName
                        ).Except(newCreatures);
                foreach (var c in removedCreatures)
                    c.Status = CreatureStatus.Unavailable;
            }

            newCreatures.ForEach(creature =>
            {
                creature.server = serverName;
            });

            creatureCollection.MergeCreatureList(newCreatures, true);
        }

        private Creature ConvertGameObject(GameObject creatureObject, int? levelStep)
        {
            if (!Values.V.TryGetSpeciesByClassName(creatureObject.ClassString, out Species species))
            {
                // species is unknown, creature cannot be imported.
                // use name-field to temporarily save the unknown classString to display in a messageBox
                return new Creature { name = creatureObject.ClassString };
            }

            GameObject statusObject = creatureObject.CharacterStatusComponent();

            // error while deserializing that creature
            if (statusObject == null)
                return null;

            string imprinterName = creatureObject.GetPropertyValue<string>("ImprinterName");
            string owner = string.IsNullOrWhiteSpace(imprinterName) ? creatureObject.GetPropertyValue<string>("TamerString") : imprinterName;

            int[] wildLevels = Enumerable.Repeat(-1, Stats.StatsCount).ToArray(); // -1 is unknown
            int[] tamedLevels = new int[Stats.StatsCount];
            int[] mutatedLevels = new int[Stats.StatsCount];

            for (int i = 0; i < Stats.StatsCount; i++)
            {
                wildLevels[i] = statusObject.GetPropertyValue<ArkByteValue>("NumberOfLevelUpPointsApplied", i)?.ByteValue ?? 0;
            }
            wildLevels[Stats.Torpidity] = statusObject.GetPropertyValue<int>("BaseCharacterLevel", defaultValue: 1) - 1; // torpor

            for (int i = 0; i < Stats.StatsCount; i++)
            {
                tamedLevels[i] = statusObject.GetPropertyValue<ArkByteValue>("NumberOfLevelUpPointsAppliedTamed", i)?.ByteValue ?? 0;
                //mutatedLevels[i] = statusObject.GetPropertyValue<ArkByteValue>("NumberOfLevelUpPointsAppliedMutated", i)?.ByteValue ?? 0; // TODO
            }

            float ti = statusObject.GetPropertyValue<float>("TamedIneffectivenessModifier", defaultValue: float.NaN);
            double te = 1f / (1 + (!float.IsNaN(ti) ? ti : creatureObject.GetPropertyValue<float>("TameIneffectivenessModifier")));

            var arkId = creatureObject.GetDinoId();
            Creature creature = new Creature(species,
                    creatureObject.GetPropertyValue<string>("TamedName"), owner, creatureObject.GetPropertyValue<string>("TribeName"),
                    creatureObject.IsFemale() ? Sex.Female : Sex.Male,
                    wildLevels, tamedLevels, mutatedLevels, te,
                    !string.IsNullOrWhiteSpace(creatureObject.GetPropertyValue<string>("ImprinterName")),
                    statusObject.GetPropertyValue<float>("DinoImprintingQuality"),
                    levelStep
            )
            {
                imprinterName = creatureObject.GetPropertyValue<string>("ImprinterName"),
                guid = Utils.ConvertArkIdToGuid(arkId),
                ArkId = arkId,
                ArkIdImported = true,
                ArkIdInGame = Utils.ConvertImportedArkIdToIngameVisualization(arkId),
                domesticatedAt = DateTime.Now, // TODO: possible to convert ingame-time to realtime?
                addedToLibrary = DateTime.Now,
                mutationsMaternal = creatureObject.GetPropertyValue<int>("RandomMutationsFemale"),
                mutationsPaternal = creatureObject.GetPropertyValue<int>("RandomMutationsMale"),
                flags = (creatureObject.GetPropertyValue<bool>("bNeutered") ? CreatureFlags.Neutered : CreatureFlags.None)
                      | (creatureObject.GetPropertyValue<bool>("MutagenApplied") ? CreatureFlags.MutagenApplied : CreatureFlags.None)
            };

            // If it's a baby and still growing, work out growingUntil
            float babyAge = creatureObject.GetPropertyValue<float>("BabyAge", defaultValue: 1);
            if (babyAge < 1)
            {
                double maturationDuration = species.breeding?.maturationTimeAdjusted ?? 0;
                float bornSecondsAgo = (float)maturationDuration * babyAge;
                if (bornSecondsAgo < maturationDuration - 120) // there seems to be a slight offset of one of these saved values, so don't display a creature as being in cooldown if it is about to leave it in the next 2 minutes
                    creature.growingUntil = DateTime.Now.Add(TimeSpan.FromSeconds(maturationDuration - bornSecondsAgo));
            }
            else
            {
                double nextMatingPossible = creatureObject.GetPropertyValue<double>("NextAllowedMatingTime");
                if (_gameTime < nextMatingPossible)
                {
                    creature.cooldownUntil = DateTime.Now.Add(TimeSpan.FromSeconds(nextMatingPossible - _gameTime));
                }
            }

            // Ancestor linking is done later after entire collection is formed - here we just set the guids
            ArkArrayStruct femaleAncestors = creatureObject.GetPropertyValue<IArkArray, ArkArrayStruct>("DinoAncestors");
            StructPropertyList femaleAncestor = (StructPropertyList)femaleAncestors?.LastOrDefault();
            if (femaleAncestor != null)
            {
                creature.motherGuid = Utils.ConvertArkIdToGuid(Utils.ConvertArkIdsToLongArkId(
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

            creature.colors = new byte[Ark.ColorRegionCount];
            for (int i = 0; i < 6; i++)
            {
                creature.colors[i] = creatureObject.GetPropertyValue<ArkByteValue>("ColorSetIndices", i)?.ByteValue ?? 0;
            }

            bool isDead = creatureObject.GetPropertyValue<bool>("bIsDead");
            if (isDead)
            {
                creature.Status = CreatureStatus.Dead; // dead is always dead
            }

            if (creatureObject.IsInCryo)
                creature.Status = CreatureStatus.Cryopod;

            creature.RecalculateCreatureValues(levelStep);

            return creature;
        }

        private Creature ConvertTamedDino(dynamic dino, int? levelStep)
        {
            if (!Values.V.TryGetSpeciesByBlueprint(dino.@object.blueprint.As<string>(), out Species species))
            {
                // species is unknown, creature cannot be imported.
                // use name-field to temporarily save the unknown blueprint name to display in a messageBox
                return new Creature { name = dino.@object.blueprint.As<string>().Split('.').Last() };
            }

            string imprinterName = dino.owner.imprinter;
            string owner = string.IsNullOrWhiteSpace(imprinterName) ? dino.owner.tamer_string : imprinterName;

            int[] wildLevels = ExtractStatLevels(dino.stats.base_stat_points);
            wildLevels[Stats.Torpidity] = dino.stats.base_level.As<int>() - 1;
            int[] tamedLevels = ExtractStatLevels(dino.stats.added_stat_points);
            int[] mutatedLevels = ExtractStatLevels(dino.stats.mutated_stat_points);

            float ti = dino.@object.get_property_value("TamedIneffectivenessModifier", 0).As<float>();
            double te = 1f / (1 + (ti == 0 ? dino.@object.get_property_value("TameIneffectivenessModifier", 0).As<float>() : ti));

            var arkId = Utils.ConvertArkIdsToLongArkId(dino.id_.id1.As<int>(), dino.id_.id2.As<int>());

            string tamedName = dino.tamed_name;
            string tribeName = dino.owner.tribe;
            bool isFemale = dino.is_female.As<bool>();
            float imprintingQuality = dino.stats.@object.get_property_value("DinoImprintingQuality", 0).As<float>();
            int mutationsFemale = dino.@object.get_property_value("RandomMutationsFemale", 0).As<int>();
            int mutationsMale = dino.@object.get_property_value("RandomMutationsMale", 0).As<int>();
            bool neutered = dino.@object.get_property_value("bNeutered", false).As<bool>();
            bool mutagenApplied = dino.@object.get_property_value("MutagenApplied", false).As<bool>();

            Creature creature = new Creature(species,
                    tamedName, owner, tribeName,
                    isFemale ? Sex.Female : Sex.Male,
                    wildLevels, tamedLevels, mutatedLevels, te,
                    !string.IsNullOrWhiteSpace(imprinterName),
                    imprintingQuality,
                    levelStep
            )
            {
                imprinterName = imprinterName,
                guid = Utils.ConvertArkIdToGuid(arkId),
                ArkId = arkId,
                ArkIdImported = true,
                ArkIdInGame = Utils.ConvertImportedArkIdToIngameVisualization(arkId),
                domesticatedAt = DateTime.Now, // TODO: possible to convert ingame-time to realtime?
                addedToLibrary = DateTime.Now,
                mutationsMaternal = mutationsFemale,
                mutationsPaternal = mutationsMale,
                flags = (neutered ? CreatureFlags.Neutered : CreatureFlags.None)
                      | (mutagenApplied ? CreatureFlags.MutagenApplied : CreatureFlags.None)
            };

            float babyAge = dino.@object.get_property_value("BabyAge", 1).As<float>();
            if (babyAge < 1)
            {
                double maturationDuration = species.breeding?.maturationTimeAdjusted ?? 0;
                float bornSecondsAgo = (float)maturationDuration * babyAge;
                if (bornSecondsAgo < maturationDuration - 120) // there seems to be a slight offset of one of these saved values, so don't display a creature as being in cooldown if it is about to leave it in the next 2 minutes
                    creature.growingUntil = DateTime.Now.Add(TimeSpan.FromSeconds(maturationDuration - bornSecondsAgo));
            }
            else
            {
                double nextMatingPossible = dino.@object.get_property_value("NextAllowedMatingTime", 0).As<double>();
                if (_gameTime < nextMatingPossible)
                {
                    creature.cooldownUntil = DateTime.Now.Add(TimeSpan.FromSeconds(nextMatingPossible - _gameTime));
                }
            }

            dynamic ancestorsFemale = dino.@object.get_property_value("DinoAncestors");
            if (ancestorsFemale != null)
            {
                dynamic femaleParent = ancestorsFemale[-1].female;
                creature.motherGuid = Utils.ConvertArkIdToGuid(Utils.ConvertArkIdsToLongArkId(
                    femaleParent.id_.id1.As<int>(),
                    femaleParent.id_.id2.As<int>()
                    ));
                creature.motherName = femaleParent.name;
                creature.isBred = true;
            }
            dynamic ancestorsMale = dino.@object.get_property_value("DinoAncestorsMale");
            if (ancestorsMale != null)
            {
                dynamic maleParent = ancestorsMale[-1].male;
                creature.fatherGuid = Utils.ConvertArkIdToGuid(Utils.ConvertArkIdsToLongArkId(
                    maleParent.id_.id1.As<int>(),
                    maleParent.id_.id2.As<int>()
                    ));
                creature.fatherName = maleParent.name;
                creature.isBred = true;
            }

            creature.colors = new byte[Ark.ColorRegionCount];
            dynamic colorSetIndices = dino.get_color_set_indices();
            for (int i = 0; i < Ark.ColorRegionCount; i++)
            {
                creature.colors[i] = colorSetIndices[i].As<byte>();
            }

            bool isDead = dino.is_dead.As<bool>();
            if (isDead)
            {
                creature.Status = CreatureStatus.Dead;
            }
            bool isInCryo = dino.is_cryopodded.As<bool>();
            if (isInCryo)
            {
                creature.Status = CreatureStatus.Cryopod;
            }

            creature.RecalculateCreatureValues(levelStep);

            return creature;
        }

        private static bool IsUnclaimedBaby(dynamic dino)
        {
            TeamType teamType = TeamTypes.ForTeam(dino.@object.get_property_value("TargetingTeam", 0).As<int>());
            return teamType == TeamType.Breeding;
        }
        private static bool IsInCryo(dynamic dino)
        {
            return dino.is_cryopodded.As<bool>();
        }

        private static int[] ExtractStatLevels(dynamic statPoints)
        {
            int[] stats = new int[Stats.StatsCount];
            stats[Stats.Health] = statPoints.health.As<int>();
            stats[Stats.Stamina] = statPoints.stamina.As<int>();
            stats[Stats.Torpidity] = statPoints.torpidity.As<int>();
            stats[Stats.Oxygen] = statPoints.oxygen.As<int>();
            stats[Stats.Food] = statPoints.food.As<int>();
            stats[Stats.Water] = statPoints.water.As<int>();
            stats[Stats.Temperature] = statPoints.temperature.As<int>();
            stats[Stats.Weight] = statPoints.weight.As<int>();
            stats[Stats.MeleeDamageMultiplier] = statPoints.melee_damage.As<int>();
            stats[Stats.SpeedMultiplier] = statPoints.movement_speed.As<int>();
            stats[Stats.TemperatureFortitude] = statPoints.fortitude.As<int>();
            stats[Stats.CraftingSpeedMultiplier] = statPoints.crafting_speed.As<int>();
            return stats;
        }
    }
}
