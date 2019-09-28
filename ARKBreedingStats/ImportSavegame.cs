﻿using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
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
        private readonly float gameTime;

        private ImportSavegame(float gameTime)
        {
            this.gameTime = gameTime;
        }

        public static async Task ImportCollectionFromSavegame(CreatureCollection creatureCollection, string filename, string serverName)
        {
            if (Values.V.ignoreSpeciesClassesOnImport == null)
            {
                Values.V.LoadIgnoreSpeciesClassesFile();
            }

            (GameObjectContainer gameObjectContainer, float gameTime) = await readSavegameFile(filename);

            IEnumerable<GameObject> tamedCreatureObjects = gameObjectContainer
                    .Where(o => o.IsCreature() && o.IsTamed() && !o.IsUnclaimedBaby() && !Values.V.ignoreSpeciesClassesOnImport.Contains(o.ClassString));

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

            // if there are creatures with unknown species, check if the according mod-file is available
            var unknownSpeciesCreatures = creatures.Where(c => c.Species == null).ToList();

            if (!unknownSpeciesCreatures.Any()
               || MessageBox.Show("The species of " + unknownSpeciesCreatures.Count.ToString() + " creature" + (unknownSpeciesCreatures.Count != 1 ? "s" : "") + " is not recognized, probably because they are from a mod that is not loaded.\n"
                                  + "The unrecognized species-classes are as follows, all the according creatures cannot be imported: " + string.Join(", ", unknownSpeciesCreatures.Select(c => c.name).Distinct().ToArray())
                                  + "\n(To import the unrecognized creatures, you first need additional values-files.)\n\n"
                                  + "Do you want to import the recognized creatures? If you click no, nothing is imported.",
                                  "Unrecognized species while importing savegame", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                                 ) == DialogResult.Yes
               )
            {
                importCollection(creatureCollection, creatures.Where(c => c.Species != null).ToList(), serverName);
            }
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
            if (creatureCollection.changeCreatureStatusOnSavegameImport)
            {
                // mark creatures that are no longer present as unavailable
                var removedCreatures = creatureCollection.creatures.Where(c => c.status == CreatureStatus.Available && c.server == serverName).Except(newCreatures);
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
            if (!Values.V.TryGetSpeciesByClassName(creatureObject.ClassString, out Species species))
            {
                // species is unknown, creature cannot be imported.
                // use name-field to temporarily save the unknown classString to display in a messagebox
                return new Creature { name = creatureObject.ClassString };
            }

            GameObject statusObject = creatureObject.CharacterStatusComponent();

            string imprinterName = creatureObject.GetPropertyValue<string>("ImprinterName");
            string owner = string.IsNullOrWhiteSpace(imprinterName) ? creatureObject.GetPropertyValue<string>("TamerString") : imprinterName;

            int[] wildLevels = Enumerable.Repeat(-1, Values.STATS_COUNT).ToArray(); // -1 is unknown
            int[] tamedLevels = new int[Values.STATS_COUNT];

            for (int i = 0; i < Values.STATS_COUNT; i++)
            {
                wildLevels[i] = statusObject.GetPropertyValue<ArkByteValue>("NumberOfLevelUpPointsApplied", i)?.ByteValue ?? 0;
            }
            wildLevels[(int)StatNames.Torpidity] = statusObject.GetPropertyValue<int>("BaseCharacterLevel", defaultValue: 1) - 1; // torpor

            for (int i = 0; i < Values.STATS_COUNT; i++)
            {
                tamedLevels[i] = statusObject.GetPropertyValue<ArkByteValue>("NumberOfLevelUpPointsAppliedTamed", i)?.ByteValue ?? 0;
            }

            float ti = statusObject.GetPropertyValue<float>("TamedIneffectivenessModifier", defaultValue: float.NaN);
            double te = 1f / (1 + (!float.IsNaN(ti) ? ti : creatureObject.GetPropertyValue<float>("TameIneffectivenessModifier")));

            Creature creature = new Creature(species,
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
                double maturationTime = species.breeding?.maturationTimeAdjusted ?? 0;
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

            creature.RecalculateCreatureValues(levelStep);

            return creature;
        }

        private static int colorModulo(int color)
        {
            // color ids ingame can be stored as higher numbers, it appears the colors just repeat
            return (color - 1) % 56 + 1;
        }
    }

}
