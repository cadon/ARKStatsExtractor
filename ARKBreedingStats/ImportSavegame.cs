using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using AsaSavegameToolkit.Porcelain;
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
using Creature = ARKBreedingStats.Library.Creature;
using SavegameCreature = AsaSavegameToolkit.Porcelain.Creature;
using GameObjectExtensions = SavegameToolkitAdditions.GameObjectExtensions;
using AsaSavegameToolkit.Plumbing.Utilities;
#if DEBUG
using AsaSavegameToolkit.Plumbing.Records;
#else
using AsaSavegameToolkit.Plumbing.Properties;
#endif
using Microsoft.Extensions.Logging.Abstractions;

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
            var ignoreClasses = Values.V.IgnoreSpeciesClassesOnImport;
            var importUnclaimedBabies = Properties.Settings.Default.SaveFileImportUnclaimedBabies;
            var saveImportCryo = Properties.Settings.Default.SaveImportCryo;

            var save = AsaSaveGame.ReadFrom(filename, logger: NullLogger.Instance);
#if DEBUG
            var gameTime = save.GameTime;
#else
            var gameTime = 0d; // wait for https://github.com/flintthatchwood/AsaSavegameToolkit/pull/3 to be merged and published
#endif

            var tamedCreatureObjects = save.TamedCreatures.Values.Where(o => (importUnclaimedBabies || !o.Record.IsUnclaimedBaby()) && !ignoreClasses.Contains(o.ClassName.Split('.').Last()));
            if (saveImportCryo)
            {
                tamedCreatureObjects = tamedCreatureObjects.Concat(save.CryopoddedCreatures.Values.Where(o => !ignoreClasses.Contains(o.ClassName.Split('.').Last())));
            }

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
                        string tribeName = o.TribeName ?? string.Empty;
                        return filters.Any(filter => tribeName.Contains(filter));
                    });
                }
            }

            ImportSavegame importSavegame = new ImportSavegame(Convert.ToSingle(gameTime));
            int? wildLevelStep = creatureCollection.getWildLevelStep();
            return tamedCreatureObjects.Select(o => importSavegame.ConvertCreature(o, wildLevelStep)).Where(c => c != null).ToArray();
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

        private Creature ConvertCreature(SavegameCreature creatureObject, int? levelStep)
        {
            if (!Values.V.TryGetSpeciesByBlueprint(creatureObject.ClassName, out Species species))
            {
                // species is unknown, creature cannot be imported.
                // use name-field to temporarily save the unknown classString to display in a messageBox
                return new Creature { name = creatureObject.ClassName };
            }

            string imprinterName = creatureObject.ImprinterName;
            string owner = string.IsNullOrWhiteSpace(imprinterName) ? creatureObject.TamerString : imprinterName;

            int[] wildLevels = Enumerable.Repeat(-1, Stats.StatsCount).ToArray(); // -1 is unknown
            int[] tamedLevels = new int[Stats.StatsCount];
            int[] mutatedLevels = new int[Stats.StatsCount];

            for (int i = 0; i < Stats.StatsCount; i++)
            {
                wildLevels[i] = creatureObject.WildLevels[i] ?? 0;
            }
            wildLevels[Stats.Torpidity] = (creatureObject.BaseLevel ?? 1) - 1; // torpor

            for (int i = 0; i < Stats.StatsCount; i++)
            {
                tamedLevels[i] = creatureObject.TamedLevelsApplied[i] ?? 0;
                mutatedLevels[i] = creatureObject.MutationLevelsApplied[i] ?? 0;
            }

            if (!creatureObject.Record.Properties.TryGet("TamedIneffectivenessModifier", out float ti))
            {
                ti = creatureObject.Record.Properties.Get<float>("TameIneffectivenessModifier");
            }
            double te = 1f / (1 + ti);

            Guid creatureGuid = creatureObject.Id;
            long arkId = Utils.ConvertCreatureGuidToArkId(creatureGuid);
            Creature creature = new Creature(species,
                    creatureObject.TamedName, owner, creatureObject.TribeName,
                    creatureObject.IsFemale ? Sex.Female : Sex.Male,
                    wildLevels, tamedLevels, mutatedLevels, te,
                    !string.IsNullOrWhiteSpace(creatureObject.ImprinterName),
                    creatureObject.ImprintQuality,
                    levelStep
            )
            {
                imprinterName = creatureObject.ImprinterName,
                guid = creatureGuid,
                ArkId = arkId,
                ArkIdImported = true,
                ArkIdInGame = Utils.ConvertImportedArkIdToIngameVisualization(arkId),
                domesticatedAt = DateTime.Now,
                addedToLibrary = DateTime.Now,
                mutationsMaternal = creatureObject.MutationsFemale,
                mutationsPaternal = creatureObject.MutationsMale,
                flags = (creatureObject.Record.Properties.Get<bool>("bNeutered") ? CreatureFlags.Neutered : CreatureFlags.None)
                      | (creatureObject.Record.Properties.Get<bool>("MutagenApplied") ? CreatureFlags.MutagenApplied : CreatureFlags.None)
            };

            // If it's a baby and still growing, work out growingUntil
            float babyAge = creatureObject.BabyAge ?? 1;
            if (babyAge < 1)
            {
                double maturationDuration = species.breeding?.maturationTimeAdjusted ?? 0;
                float bornSecondsAgo = (float)maturationDuration * babyAge;
                if (bornSecondsAgo < maturationDuration - 120) // there seems to be a slight offset of one of these saved values, so don't display a creature as being in cooldown if it is about to leave it in the next 2 minutes
                    creature.growingUntil = DateTime.Now.Add(TimeSpan.FromSeconds(maturationDuration - bornSecondsAgo));
            }
            else
            {
                double nextMatingPossible = creatureObject.Record.Properties.Get<double>("NextAllowedMatingTime");
                if (_gameTime < nextMatingPossible)
                {
                    creature.cooldownUntil = DateTime.Now.Add(TimeSpan.FromSeconds(nextMatingPossible - _gameTime));
                }
            }

            // Ancestor linking is done later after entire collection is formed - here we just set the guids
#if DEBUG
            IEnumerable<DinoAncestorsEntryRecord> femaleAncestors = creatureObject.Record.Properties.Get<object[]>("DinoAncestors")?.OfType<DinoAncestorsEntryRecord>();
            DinoAncestorsEntryRecord femaleAncestor = femaleAncestors?.LastOrDefault();
            if (femaleAncestor != null)
            {
                creature.motherGuid = Utils.ConvertArkIdToGuid(Utils.ConvertArkIdsToLongArkId(
                        (int)femaleAncestor.FemaleId1,
                        (int)femaleAncestor.FemaleId2));
                creature.motherName = femaleAncestor.FemaleName;
                creature.isBred = true;
            }
            IEnumerable<DinoAncestorsEntryRecord> maleAncestors = creatureObject.Record.Properties.Get<object[]>("DinoAncestors")?.OfType<DinoAncestorsEntryRecord>();
            DinoAncestorsEntryRecord maleAncestor = maleAncestors?.LastOrDefault();
            if (maleAncestor != null)
            {
                creature.fatherGuid = Utils.ConvertArkIdToGuid(Utils.ConvertArkIdsToLongArkId(
                        (int)maleAncestor.MaleId1,
                        (int)maleAncestor.MaleId2));
                creature.fatherName = maleAncestor.MaleName;
                creature.isBred = true;
            }
#else
            IEnumerable<List<Property>> femaleAncestors = creatureObject.Record.Properties.Get<object[]>("DinoAncestors")?.OfType<List<Property>>(); // for now, this has to use the raw properties until https://github.com/flintthatchwood/AsaSavegameToolkit/pull/4 is merged and published.
            List<Property> femaleAncestor = femaleAncestors?.LastOrDefault();
            if (femaleAncestor != null)
            {
                creature.motherGuid = Utils.ConvertArkIdToGuid(Utils.ConvertArkIdsToLongArkId(
                        (int)femaleAncestor.Get<uint>("FemaleDinoID1"),
                        (int)femaleAncestor.Get<uint>("FemaleDinoID2")));
                creature.motherName = femaleAncestor.Get<string>("FemaleName");
                creature.isBred = true;
            }
            IEnumerable<List<Property>> maleAncestors = creatureObject.Record.Properties.Get<object[]>("DinoAncestorsMale")?.OfType<List<Property>>();
            List<Property> maleAncestor = maleAncestors?.LastOrDefault();
            if (maleAncestor != null)
            {
                creature.fatherGuid = Utils.ConvertArkIdToGuid(GameObjectExtensions.CreateDinoId(
                        (int)maleAncestor.Get<uint>("MaleDinoID1"),
                        (int)maleAncestor.Get<uint>("MaleDinoID2")));
                creature.fatherName = maleAncestor.Get<string>("MaleName");
                creature.isBred = true;
            }
#endif

            creature.colors = new byte[Ark.ColorRegionCount];
            for (int i = 0; i < 6; i++)
            {
                creature.colors[i] = creatureObject.ColorRegions[i] ?? 0;
            }

            bool isDead = creatureObject.Record.Properties.Get<bool>("bIsDead");
            if (isDead)
            {
                creature.Status = CreatureStatus.Dead; // dead is always dead
            }

            if (creatureObject.IsInCryo)
                creature.Status = CreatureStatus.Cryopod;

            creature.RecalculateCreatureValues(levelStep);

            return creature;
        }
    }
}
