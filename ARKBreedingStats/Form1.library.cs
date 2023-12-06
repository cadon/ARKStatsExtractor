using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.utils;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ARKBreedingStats.library;
using ARKBreedingStats.settings;

namespace ARKBreedingStats
{
    public partial class Form1
    {
        /// <summary>
        /// Creatures filtered according to the library-filter.
        /// Used so the live filter doesn't need to do the base filtering every time.
        /// </summary>
        private Creature[] _creaturesPreFiltered;

        private Species[] _speciesInLibraryOrdered;

        /// <summary>
        /// Add a new creature to the library based from the data of the extractor or tester
        /// </summary>
        /// <param name="fromExtractor">if true, take data from extractor-infoInput, else from tester</param>
        /// <param name="motherArkId">only pass if from import. Used for creating placeholder parents</param>
        /// <param name="fatherArkId">only pass if from import. Used for creating placeholder parents</param>
        /// <param name="goToLibraryTab">go to library tab after the creature is added</param>
        private Creature AddCreatureToCollection(bool fromExtractor = true, long motherArkId = 0, long fatherArkId = 0, bool goToLibraryTab = true)
        {
            var levelStep = _creatureCollection.getWildLevelStep();
            var species = speciesSelector1.SelectedSpecies;
            var creature = GetCreatureFromInput(fromExtractor, species, levelStep, motherArkId, fatherArkId);

            // if creature is placeholder: add it
            // if creature's ArkId is already in library, suggest updating of the creature
            //if (!IsArkIdUniqueOrOnlyPlaceHolder(creature))
            //{
            //    // if creature is already in library, suggest updating or dismissing

            //    //ShowDuplicateMergerAndCheckForDuplicates()

            //    return;
            //}

            creature.RecalculateCreatureValues(levelStep);
            creature.RecalculateNewMutations();

            if (_creatureCollection.DeletedCreatureGuids != null
                && _creatureCollection.DeletedCreatureGuids.Contains(creature.guid))
                _creatureCollection.DeletedCreatureGuids.RemoveAll(guid => guid == creature.guid);

            _creatureCollection.MergeCreatureList(new[] { creature });

            // set status of exportedCreatureControl if available
            _exportedCreatureControl?.setStatus(importExported.ExportedCreatureControl.ImportStatus.JustImported, DateTime.Now);

            // if creature already exists by guid, use the already existing creature object for the parent assignments
            creature = _creatureCollection.creatures.FirstOrDefault(c => c.guid == creature.guid) ?? creature;

            // if new creature is parent of existing creatures, update link
            var motherOf = _creatureCollection.creatures.Where(c => c.motherGuid == creature.guid).ToArray();
            foreach (Creature c in motherOf)
            {
                c.Mother = creature;
                c.RecalculateNewMutations();
            }
            var fatherOf = _creatureCollection.creatures.Where(c => c.fatherGuid == creature.guid).ToArray();
            foreach (Creature c in fatherOf)
            {
                c.Father = creature;
                c.RecalculateNewMutations();
            }

            // link new creature to its parents if they're available, or creature placeholders
            if (creature.Mother == null || creature.Father == null)
                UpdateParents(new List<Creature> { creature });

            // if the new creature is the ancestor of any other creatures, update the generation count of all creatures
            if (motherOf.Any() || fatherOf.Any())
            {
                var creaturesOfSpecies = _creatureCollection.creatures.Where(c => c.Species == creature.Species).ToArray();
                foreach (var cr in creaturesOfSpecies) cr.generation = -1;
                foreach (var cr in creaturesOfSpecies) cr.RecalculateAncestorGenerations();
            }
            else
            {
                creature.RecalculateAncestorGenerations();
            }

            if (Properties.Settings.Default.PauseGrowingTimerAfterAddingBaby)
                creature.StartStopMatureTimer(false);

            _filterListAllowed = false;
            UpdateCreatureListings(species, false);

            // show only the added creatures' species
            listBoxSpeciesLib.SelectedItem = creature.Species;
            _filterListAllowed = true;
            _libraryNeedsUpdate = true;

            if (goToLibraryTab)
            {
                tabControlMain.SelectedTab = tabPageLibrary;
                SelectCreatureInLibrary(creature);
            }

            creatureInfoInputExtractor.parentListValid = false;
            creatureInfoInputTester.parentListValid = false;

            SetCollectionChanged(true, species);
            return creature;
        }

        /// <summary>
        /// Deletes the creatures selected in the library after a confirmation.
        /// </summary>
        private void DeleteSelectedCreatures()
        {
            if (tabControlMain.SelectedTab == tabPageLibrary)
            {
                if (listViewLibrary.SelectedIndices.Count > 0)
                {
                    if (MessageBox.Show("Do you really want to delete the entry and all data for " +
                            $"\"{_creaturesDisplayed[listViewLibrary.SelectedIndices[0]].name}\"" +
                            $"{(listViewLibrary.SelectedIndices.Count > 1 ? " and " + (listViewLibrary.SelectedIndices.Count - 1) + " other creatures" : null)}?",
                            "Delete Creature?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        bool onlyOneSpecies = true;
                        Species species = _creaturesDisplayed[listViewLibrary.SelectedIndices[0]].Species;
                        foreach (int i in listViewLibrary.SelectedIndices)
                        {
                            var cr = _creaturesDisplayed[i];
                            if (onlyOneSpecies)
                            {
                                if (species != cr.Species)
                                    onlyOneSpecies = false;
                            }
                            _creatureCollection.DeleteCreature(cr);
                        }
                        _creatureCollection.RemoveUnlinkedPlaceholders();
                        UpdateCreatureListings(onlyOneSpecies ? species : null);
                        SetCollectionChanged(true, onlyOneSpecies ? species : null);
                    }
                }
            }
            else if (tabControlMain.SelectedTab == tabPagePlayerTribes)
            {
                tribesControl1.RemoveSelected();
            }
        }

        /// <summary>
        /// Checks if the ArkId of the given creature is already in the collection. If a placeholder has this id, the placeholder is removed and the placeholder.Guid is set to the creature.
        /// </summary>
        /// <param name="creature">Creature whose ArkId will be checked</param>
        /// <returns>True if the ArkId is unique (or only a placeholder had it). False if there is a conflict.</returns>
        private bool IsArkIdUniqueOrOnlyPlaceHolder(Creature creature)
        {
            bool arkIdIsUnique = true;

            if (creature.ArkId != 0 && _creatureCollection.ArkIdAlreadyExist(creature.ArkId, creature, out Creature guidCreature))
            {
                // if the creature is a placeholder replace the placeholder with the real creature
                if (guidCreature.flags.HasFlag(CreatureFlags.Placeholder) && creature.sex == guidCreature.sex && creature.Species == guidCreature.Species)
                {
                    // remove placeholder-creature from collection (is replaced by new creature)
                    _creatureCollection.creatures.Remove(guidCreature);
                }
                else
                {
                    // creature is not a placeholder, warn about id-conflict and don't add creature.
                    // TODO offer merging of the two creatures if they are similar (e.g. same species). merge automatically if only the dom-levels are different?
                    MessageBox.Show("The entered ARK-ID is already existing in this library " +
                            $"({guidCreature.Species.name} (lvl {guidCreature.Level}): {guidCreature.name}).\n" +
                            "You have to choose a different ARK-ID or delete the other creature first.",
                            "ARK-ID already existing",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    arkIdIsUnique = false;
                }
            }

            return arkIdIsUnique;
        }

        /// <summary>
        /// Returns the wild levels from the extractor or tester in an array.
        /// </summary>
        private int[] GetCurrentWildLevels(bool fromExtractor = true)
        {
            int[] levelsWild = new int[Stats.StatsCount];
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                levelsWild[s] = fromExtractor ? _statIOs[s].LevelWild : _testingIOs[s].LevelWild;
            }
            return levelsWild;
        }

        /// <summary>
        /// Returns the mutated levels from the extractor or tester in an array.
        /// </summary>
        private int[] GetCurrentMutLevels(bool fromExtractor = true)
        {
            int[] levelsMut = new int[Stats.StatsCount];
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                levelsMut[s] = fromExtractor ? _statIOs[s].LevelMut : _testingIOs[s].LevelMut;
            }
            return levelsMut;
        }

        /// <summary>
        /// Returns the domesticated levels from the extractor or tester in an array.
        /// </summary>
        private int[] GetCurrentDomLevels(bool fromExtractor = true)
        {
            int[] levelsDom = new int[Stats.StatsCount];
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                levelsDom[s] = fromExtractor ? _statIOs[s].LevelDom : _testingIOs[s].LevelDom;
            }
            return levelsDom;
        }

        /// <summary>
        /// Call after the creatureCollection-object was created anew (e.g. after loading a file)
        /// </summary>
        /// <param name="keepCurrentSelection">True if synchronized library file is loaded.</param>
        private bool InitializeCollection(bool keepCurrentSelection = false)
        {
            // set pointer to current collection
            CreatureCollection.CurrentCreatureCollection = _creatureCollection;
            pedigree1.SetCreatures(_creatureCollection.creatures);
            breedingPlan1.CreatureCollection = _creatureCollection;
            tribesControl1.Tribes = _creatureCollection.tribes;
            tribesControl1.Players = _creatureCollection.players;
            timerList1.CreatureCollection = _creatureCollection;
            notesControl1.NoteList = _creatureCollection.noteList;
            raisingControl1.CreatureCollection = _creatureCollection;
            statsMultiplierTesting1.CreatureCollection = _creatureCollection;

            var duplicatesWereRemoved = UpdateParents(_creatureCollection.creatures);
            UpdateIncubationParents(_creatureCollection);

            CreateCreatureTagList();

            if (_creatureCollection.modIDs == null) _creatureCollection.modIDs = new List<string>();

            if (keepCurrentSelection)
            {
                pedigree1.RecreateAfterLoading(tabControlMain.SelectedTab == tabPagePedigree);
                breedingPlan1.RecreateAfterLoading(tabControlMain.SelectedTab == tabPageBreedingPlan);
            }
            else
            {
                pedigree1.Clear();
                breedingPlan1.Clear();
            }

            ApplySpeciesObjectsToCollection(_creatureCollection);

            UpdateTempCreatureDropDown();

            return duplicatesWereRemoved;
        }

        /// <summary>
        /// Applies the species object to the creatures and creatureValues of the collection.
        /// </summary>
        /// <param name="cc"></param>
        private static void ApplySpeciesObjectsToCollection(CreatureCollection cc)
        {
            foreach (var cr in cc.creatures)
            {
                cr.Species = Values.V.SpeciesByBlueprint(cr.speciesBlueprint);
            }
            foreach (var cv in cc.creaturesValues)
            {
                cv.Species = Values.V.SpeciesByBlueprint(cv.speciesBlueprint);
            }
        }

        /// <summary>
        /// Calculates the top-stats in each species, sets the top-stat-flags in the creatures
        /// </summary>
        /// <param name="creatures">creatures to consider</param>
        private void CalculateTopStats(List<Creature> creatures)
        {
            var filteredCreaturesHash = Properties.Settings.Default.useFiltersInTopStatCalculation ? new HashSet<Creature>(ApplyLibraryFilterSettings(creatures)) : null;

            var speciesCreaturesGroups = creatures.GroupBy(c => c.Species);

            foreach (var g in speciesCreaturesGroups)
            {
                var species = g.Key;
                if (species == null)
                    continue;
                var speciesCreatures = g.ToArray();

                List<int> usedStatIndices = new List<int>(Stats.StatsCount);
                List<int> usedAndConsideredStatIndices = new List<int>(Stats.StatsCount);
                int[] bestStat = new int[Stats.StatsCount];
                int[] lowestStat = new int[Stats.StatsCount];
                var statWeights = breedingPlan1.StatWeighting.GetWeightingForSpecies(species);
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    bestStat[s] = -1;
                    lowestStat[s] = -1;
                    if (species.UsesStat(s))
                    {
                        usedStatIndices.Add(s);
                        if (_considerStatHighlight[s])
                            usedAndConsideredStatIndices.Add(s);
                    }
                }
                List<Creature>[] bestCreatures = new List<Creature>[Stats.StatsCount];
                int usedStatsCount = usedStatIndices.Count;
                int usedAndConsideredStatsCount = usedAndConsideredStatIndices.Count;

                foreach (var c in speciesCreatures)
                {
                    if (c.flags.HasFlag(CreatureFlags.Placeholder))
                        continue;

                    // reset topBreeding stats for this creature
                    c.topBreedingStats = new bool[Stats.StatsCount];
                    c.topBreedingCreature = false;

                    if (
                        //if not in the filtered collection (using library filter settings), continue
                        (filteredCreaturesHash != null && !filteredCreaturesHash.Contains(c))
                        // only consider creature if it's available for breeding
                        || !(c.Status == CreatureStatus.Available
                            || c.Status == CreatureStatus.Cryopod
                            || c.Status == CreatureStatus.Obelisk
                            )
                        )
                    {
                        continue;
                    }

                    for (int s = 0; s < usedStatsCount; s++)
                    {
                        int si = usedStatIndices[s];
                        if (c.levelsWild[si] != -1 && (lowestStat[si] == -1 || c.levelsWild[si] < lowestStat[si]))
                        {
                            lowestStat[si] = c.levelsWild[si];
                        }

                        if (c.levelsWild[si] <= 0) continue;

                        if (c.levelsWild[si] == bestStat[si])
                        {
                            bestCreatures[si].Add(c);
                        }
                        else if (c.levelsWild[si] > bestStat[si])
                        {
                            // check if highest stats are only counted if odd or even
                            if ((statWeights.Item2?[s] ?? 0) == 0 // even/odd doesn't matter
                                || (statWeights.Item2[s] == 1 && c.levelsWild[si] % 2 == 1)
                                || (statWeights.Item2[s] == 2 && c.levelsWild[si] % 2 == 0)
                               )
                            {
                                bestCreatures[si] = new List<Creature> { c };
                                bestStat[si] = c.levelsWild[si];
                            }
                        }
                    }
                }

                _topLevels[species] = bestStat;
                _lowestLevels[species] = lowestStat;

                // bestStat and bestCreatures now contain the best stats and creatures for each stat.

                // set topness of each creature (== mean wildLevels/mean top wildLevels in permille)
                int sumTopLevels = 0;
                for (int s = 0; s < usedAndConsideredStatsCount; s++)
                {
                    int si = usedAndConsideredStatIndices[s];
                    if (bestStat[si] > 0)
                        sumTopLevels += bestStat[si];
                }
                if (sumTopLevels > 0)
                {
                    foreach (var c in speciesCreatures)
                    {
                        if (c.levelsWild == null || c.flags.HasFlag(CreatureFlags.Placeholder)) continue;
                        int sumCreatureLevels = 0;
                        for (int s = 0; s < usedAndConsideredStatsCount; s++)
                        {
                            int si = usedAndConsideredStatIndices[s];
                            sumCreatureLevels += c.levelsWild[si] > 0 ? c.levelsWild[si] : 0;
                        }
                        c.topness = (short)(1000 * sumCreatureLevels / sumTopLevels);
                    }
                }

                // if any male is in more than 1 category, remove any male from the topBreedingCreatures that is not top in at least 2 categories himself
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (bestCreatures[s] == null || bestCreatures[s].Count == 0)
                    {
                        continue; // no creature has levelups in this stat or the stat is not used for this species
                    }

                    var crCount = bestCreatures[s].Count;
                    if (crCount == 1)
                    {
                        bestCreatures[s][0].topBreedingCreature = true;
                        continue;
                    }

                    for (int c = 0; c < crCount; c++)
                    {
                        bestCreatures[s][c].topBreedingCreature = true;
                        if (bestCreatures[s][c].sex != Sex.Male)
                            continue;

                        Creature currentCreature = bestCreatures[s][c];
                        // check how many best stat the male has
                        int maxval = 0;
                        for (int cs = 0; cs < Stats.StatsCount; cs++)
                        {
                            if (currentCreature.levelsWild[cs] == bestStat[cs])
                                maxval++;
                        }

                        if (maxval > 1)
                        {
                            // check now if the other males have only 1.
                            for (int oc = 0; oc < crCount; oc++)
                            {
                                if (bestCreatures[s][oc].sex != Sex.Male)
                                    continue;

                                if (oc == c)
                                    continue;

                                Creature otherMale = bestCreatures[s][oc];

                                int othermaxval = 0;
                                for (int ocs = 0; ocs < Stats.StatsCount; ocs++)
                                {
                                    if (otherMale.levelsWild[ocs] == bestStat[ocs])
                                        othermaxval++;
                                }
                                if (othermaxval == 1)
                                    bestCreatures[s][oc].topBreedingCreature = false;
                            }
                        }
                    }
                }

                // now we have a list of all candidates for breeding. Iterate on stats.
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (bestCreatures[s] != null)
                    {
                        for (int c = 0; c < bestCreatures[s].Count; c++)
                        {
                            // flag topStats in creatures
                            bestCreatures[s][c].topBreedingStats[s] = true;
                        }
                    }
                }
            }

            bool considerWastedStatsForTopCreatures = Properties.Settings.Default.ConsiderWastedStatsForTopCreatures;
            foreach (Creature c in creatures)
                c.SetTopStatCount(_considerStatHighlight, considerWastedStatsForTopCreatures);

            var selectedSpecies = speciesSelector1.SelectedSpecies;
            if (selectedSpecies != null)
                hatching1.SetSpecies(selectedSpecies, _topLevels.TryGetValue(selectedSpecies, out var tl) ? tl : null, _lowestLevels.TryGetValue(selectedSpecies, out var ll) ? ll : null);
        }

        /// <summary>
        /// Sets the parents according to the guids. Call after a file is loaded. Returns true if duplicates were removed.
        /// </summary>
        private bool UpdateParents(IEnumerable<Creature> creatures)
        {
            Dictionary<Guid, Creature> creatureGuids;

            bool duplicatesWereRemoved = false;

            try
            {
                creatureGuids = _creatureCollection.creatures.ToDictionary(c => c.guid);
            }
            catch (ArgumentException)
            {
                // assuming there are somehow multiple creatures with the same guid
                // if it's only placeholders, remove the duplicates
                var guidGroups = _creatureCollection.creatures.GroupBy(c => c.guid);
                var uniqueList = new List<Creature>();

                foreach (var g in guidGroups)
                {
                    var count = g.Count();
                    var firstCreature = g.First();
                    if (count == 1)
                    {
                        uniqueList.Add(firstCreature);
                        continue;
                    }
                    // if only one creature is not a placeholder, use that
                    var nonPlaceholders = g.Where(c => !c.flags.HasFlag(CreatureFlags.Placeholder)).ToArray();
                    count = nonPlaceholders.Length;
                    if (count == 1)
                    {
                        uniqueList.Add(nonPlaceholders.First());
                        continue;
                    }


                    if (count == 0)
                    {
                        // just take the first placeholder
                        uniqueList.Add(firstCreature);
                        continue;
                    }

                    // there are more than 1 non-placeholder with the same guid. Check if the objects represent the same.
                    bool sameCreature = true;
                    for (int i = 1; i < count; i++)
                    {
                        var duplicateCreature = nonPlaceholders[i];
                        if (firstCreature.name.Trim() != duplicateCreature.name.Trim()
                            || !AreIntArraysEqual(firstCreature.levelsWild, duplicateCreature.levelsWild)
                            || !AreByteArraysEqual(firstCreature.colors, duplicateCreature.colors)
                            )
                        {
                            sameCreature = false;
                            break;
                        }
                    }

                    bool AreIntArraysEqual(int[] firstArray, int[] secondArray)
                    {
                        if (firstArray == null && secondArray == null) return true;
                        if (firstArray == null || secondArray == null) return false;
                        var firstCount = firstArray.Length;
                        var secondCount = secondArray.Length;
                        if (firstCount != secondCount) return false;

                        for (int i = 0; i < firstCount; i++)
                        {
                            if (firstArray[i] != secondArray[i])
                                return false;
                        }

                        return true;
                    }

                    bool AreByteArraysEqual(byte[] firstArray, byte[] secondArray)
                    {
                        if (firstArray == null && secondArray == null) return true;
                        if (firstArray == null || secondArray == null) return false;
                        var firstCount = firstArray.Length;
                        var secondCount = secondArray.Length;
                        if (firstCount != secondCount) return false;

                        for (int i = 0; i < firstCount; i++)
                        {
                            if (firstArray[i] != secondArray[i])
                                return false;
                        }

                        return true;
                    }

                    if (sameCreature)
                    {
                        uniqueList.Add(firstCreature);
                        continue;
                    }

                    // duplicate creatures differ
                    var text = new StringBuilder();
                    text.AppendLine($"There is an issue with some creatures of this library.\nEach creature must have a unique id (guid),\nbut all the following creatures share the same guid {firstCreature.guid}");
                    text.AppendLine();
                    for (int i = 0; i < count; i++)
                    {
                        var c = nonPlaceholders[i];
                        var species = Values.V.SpeciesByBlueprint(c.speciesBlueprint)?.DescriptiveNameAndMod ?? c.speciesBlueprint;
                        text.AppendLine($"{(i + 1)}: {species} - {c.name}");
                    }

                    text.AppendLine();
                    text.AppendLine("If you click on Yes, the first listed creature will be kept, all the other creatures will be removed. A backup file of the following library file will be created:");
                    text.AppendLine(_currentFileName);
                    text.AppendLine("If you click on No, the application will quit.");
                    text.AppendLine("Remove duplicates?");

                    if (MessageBox.Show(text.ToString(), $"Duplicate creatures - {Utils.ApplicationNameVersion}",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        uniqueList.Add(firstCreature);
                        continue;
                    }

                    Environment.Exit(0);
                }

                _creatureCollection.creatures = uniqueList;

                creatureGuids = _creatureCollection.creatures.ToDictionary(c => c.guid);
                // create backup file of file before duplicates were removed
                if (!string.IsNullOrEmpty(_currentFileName)
                    && File.Exists(_currentFileName))
                {
                    File.Copy(_currentFileName, Path.Combine(Path.GetDirectoryName(_currentFileName), $"{Path.GetFileNameWithoutExtension(_currentFileName)}_BackupBeforeRemovingDuplicates_{DateTime.Now:yyyy-MM-dd_HH-mm-ss-ffff}.asb"));
                }

                duplicatesWereRemoved = true;
            }

            var placeholderAncestors = new Dictionary<Guid, Creature>();

            foreach (Creature c in creatures)
            {
                if (c.motherGuid == Guid.Empty && c.fatherGuid == Guid.Empty) continue;

                Creature mother = null;
                if (c.motherGuid != Guid.Empty
                    && !creatureGuids.TryGetValue(c.motherGuid, out mother))
                    mother = EnsurePlaceholderCreature(placeholderAncestors, c, c.motherGuid, c.motherName, Sex.Female);

                Creature father = null;
                if (c.fatherGuid != Guid.Empty
                    && !creatureGuids.TryGetValue(c.fatherGuid, out father))
                    father = EnsurePlaceholderCreature(placeholderAncestors, c, c.fatherGuid, c.fatherName, Sex.Male);

                c.Mother = mother;
                c.Father = father;
            }

            _creatureCollection.creatures.AddRange(placeholderAncestors.Values);

            return duplicatesWereRemoved;
        }

        /// <summary>
        /// Ensures the given placeholder ancestor exists in the list of placeholders.
        /// Does nothing when the details are not well specified.
        /// </summary>
        /// <param name="placeholders">List of placeholders to amend</param>
        /// <param name="tmpl">Descendant creature to use as a template</param>
        /// <param name="guid">GUID of creature to create</param>
        /// <param name="name">Name of the creature to create</param>
        /// <param name="sex">Sex of the creature to create</param>
        /// <returns></returns>
        private Creature EnsurePlaceholderCreature(Dictionary<Guid, Creature> placeholders, Creature tmpl, Guid guid, string name, Sex sex)
        {
            if (guid == Guid.Empty)
                return null;
            if (placeholders.TryGetValue(guid, out var existingCreature))
                return existingCreature;

            if (string.IsNullOrEmpty(name))
                name = (sex == Sex.Female ? "Mother" : "Father") + " of " + tmpl.name;

            var creature = new Creature(tmpl.Species, name, tmpl.owner, tmpl.tribe, sex, levelStep: _creatureCollection.getWildLevelStep())
            {
                guid = guid,
                Status = CreatureStatus.Unavailable,
                flags = CreatureFlags.Placeholder
            };

            placeholders.Add(creature.guid, creature);

            return creature;
        }

        /// <summary>
        /// Sets the parents of the incubation-timers according to the guids. Call after a file is loaded.
        /// </summary>
        /// <param name="cc"></param>
        private void UpdateIncubationParents(CreatureCollection cc)
        {
            if (!cc.incubationListEntries.Any()) return;

            var dict = cc.creatures.ToDictionary(c => c.guid);

            foreach (IncubationTimerEntry it in cc.incubationListEntries)
            {
                if (it.motherGuid != Guid.Empty && dict.TryGetValue(it.motherGuid, out var m))
                    it.Mother = m;
                if (it.fatherGuid != Guid.Empty && dict.TryGetValue(it.fatherGuid, out var f))
                    it.Father = f;
            }
        }

        private void ShowCreaturesInListView(IEnumerable<Creature> creatures)
        {
            listViewLibrary.BeginUpdate();
            var sorted = _creatureListSorter.DoSort(creatures, orderBySpecies: Properties.Settings.Default.LibraryGroupBySpecies ? _speciesInLibraryOrdered : null);
            _creaturesDisplayed = Properties.Settings.Default.LibraryGroupBySpecies ? InsertDividers(sorted) : sorted;
            listViewLibrary.VirtualListSize = _creaturesDisplayed.Length;
            _libraryListViewItemCache = null;
            listViewLibrary.EndUpdate();

            // highlight filter input if something is entered and no results are available
            if (string.IsNullOrEmpty(ToolStripTextBoxLibraryFilter.Text))
            {
                ToolStripTextBoxLibraryFilter.BackColor = SystemColors.Window;
                ToolStripButtonLibraryFilterClear.BackColor = SystemColors.Control;
            }
            else
            {
                // if no items are shown, shade red, if something is shown and potentially some are sorted out, shade yellow
                ToolStripTextBoxLibraryFilter.BackColor = _creaturesDisplayed.Any() ? Color.LightGoldenrodYellow : Color.LightSalmon;
                ToolStripButtonLibraryFilterClear.BackColor = Color.Orange;
            }
        }

        private Creature[] InsertDividers(IList<Creature> creatures)
        {
            if (!creatures.Any())
            {
                return Array.Empty<Creature>();
            }
            List<Creature> result = new List<Creature>();
            Species lastSpecies = null;
            foreach (Creature c in creatures)
            {
                if (lastSpecies == null || c.Species != lastSpecies)
                {
                    result.Add(new Creature(c.Species)
                    {
                        flags = CreatureFlags.Placeholder | CreatureFlags.Divider,
                        Status = CreatureStatus.Unavailable
                    });
                }
                result.Add(c);
                lastSpecies = c.Species;
            }
            return result.ToArray();
        }

        #region ListViewLibrary virtual

        private Creature[] _creaturesDisplayed;
        private ListViewItem[] _libraryListViewItemCache; //array to cache items for the virtual list
        private int _libraryItemCacheFirstIndex; //stores the index of the first item in the cache

        private void ListViewLibrary_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            // check to see if the requested item is currently in the cache
            if (_libraryListViewItemCache != null && e.ItemIndex >= _libraryItemCacheFirstIndex && e.ItemIndex < _libraryItemCacheFirstIndex + _libraryListViewItemCache.Length)
            {
                // get the ListViewItem from the cache instead of making a new one.
                e.Item = _libraryListViewItemCache[e.ItemIndex - _libraryItemCacheFirstIndex];
            }
            else if (_creaturesDisplayed?.Length > e.ItemIndex)
            {
                // create item not available in the cache
                e.Item = CreateCreatureLvItem(_creaturesDisplayed[e.ItemIndex], Properties.Settings.Default.DisplayLibraryCreatureIndex);
            }
        }

        private void ListViewLibrary_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            if (_libraryListViewItemCache != null && e.StartIndex >= _libraryItemCacheFirstIndex && e.EndIndex <= _libraryItemCacheFirstIndex + _libraryListViewItemCache.Length)
            {
                // cache already contains needed items, so do nothing.
                return;
            }

            // rebuild the cache.
            const int cacheMoreRows = 60;
            var indexStart = Math.Max(0, e.StartIndex - cacheMoreRows);
            var indexEnd = Math.Min(_creaturesDisplayed.Length - 1, e.EndIndex + cacheMoreRows);
            _libraryItemCacheFirstIndex = indexStart;
            var length = indexEnd - indexStart + 1;
            _libraryListViewItemCache = new ListViewItem[length];

            var displayIndex = Properties.Settings.Default.DisplayLibraryCreatureIndex;
            //Fill the cache with the appropriate ListViewItems.
            for (int i = 0; i < length; i++)
            {
                _libraryListViewItemCache[i] = CreateCreatureLvItem(_creaturesDisplayed[i + _libraryItemCacheFirstIndex], displayIndex);
            }
        }

        private void ListViewLibrary_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;

            if (!(e.Item.Tag is Creature creature))
            {
                return;
            }

            if (creature.flags.HasFlag(CreatureFlags.Divider))
            {
                e.DrawDefault = false;
                var rect = e.Bounds;
                var count = 0;
                if (creature.Species.blueprintPath != null)
                    _creatureCollection.GetCreatureCountBySpecies()
                        .TryGetValue(creature.Species.blueprintPath, out count);
                var displayedText = creature.Species.DescriptiveNameAndMod + " (" + count + ")";
                float middle = (rect.Top + rect.Bottom) / 2f;
                e.Graphics.FillRectangle(Brushes.Blue, rect.Left, middle, rect.Width - 3, 1);
                SizeF strSize = e.Graphics.MeasureString(displayedText, e.Item.Font);
                e.Graphics.FillRectangle(new SolidBrush(e.Item.BackColor), rect.Left, rect.Top, strSize.Width + 15, rect.Height);
                e.Graphics.DrawString(displayedText, e.Item.Font, Brushes.Black, rect.Left + 10, rect.Top + ((rect.Height - strSize.Height) / 2f));
            }
        }

        private void ListViewLibrary_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            var isDivider = e.Item.Tag is Creature creature && creature.flags.HasFlag(CreatureFlags.Divider);
            e.DrawDefault = !isDivider;
        }

        #endregion

        /// <summary>
        /// Call this function to update the displayed values of a creature. Usually called after a creature was edited.
        /// </summary>
        /// <param name="cr">Creature that was changed</param>
        /// <param name="creatureStatusChanged"></param>
        private void UpdateDisplayedCreatureValues(Creature cr, bool creatureStatusChanged, bool ownerServerChanged)
        {
            // if row is selected, save and reselect later
            var selectedCreatures = new HashSet<Creature>();
            foreach (int i in listViewLibrary.SelectedIndices)
                selectedCreatures.Add(_creaturesDisplayed[i]);

            // data of the selected creature changed, update listview
            cr.RecalculateCreatureValues(_creatureCollection.getWildLevelStep());
            // if creatureStatus (available/dead) changed, recalculate topStats (dead creatures are not considered there)
            if (creatureStatusChanged)
            {
                CalculateTopStats(_creatureCollection.creatures.Where(c => c.Species == cr.Species).ToList());
                FilterLibRecalculate();
                UpdateStatusBar();
            }
            else
            {
                UpdateCreatureListViewItem(cr);
            }

            // recreate ownerList
            if (ownerServerChanged)
                UpdateOwnerServerTagLists();
            SetCollectionChanged(true, cr.Species);

            SelectCreaturesInLibrary(selectedCreatures);
        }

        /// <summary>
        /// Selects the passed creatures in the library and sets _reactOnCreatureSelectionChange on true again.
        /// </summary>
        /// <param name="selectedCreatures"></param>
        private void SelectCreaturesInLibrary(HashSet<Creature> selectedCreatures, bool selectFirstIfNothingIsSelected = false)
        {
            var selectedCount = selectedCreatures?.Count ?? 0;
            if (selectedCount == 0)
            {
                listViewLibrary.SelectedIndices.Clear();
                if (selectFirstIfNothingIsSelected && _creaturesDisplayed.Length != 0)
                {
                    _reactOnCreatureSelectionChange = true;
                    listViewLibrary.SelectedIndices.Add(0);
                    listViewLibrary.EnsureVisible(0);
                }
                else
                {
                    creatureBoxListView.Clear();
                }
                return;
            }

            _reactOnCreatureSelectionChange = false;

            listViewLibrary.SelectedIndices.Clear();

            var creatureSelected = false;
            // for loop is faster than foreach loop for small selected item amount, which is usually the case
            for (int i = 0; i < _creaturesDisplayed.Length; i++)
            {
                if (selectedCreatures.Contains(_creaturesDisplayed[i]))
                {
                    creatureSelected = true;
                    if (--selectedCount == 0)
                    {
                        _reactOnCreatureSelectionChange = true;
                        listViewLibrary.SelectedIndices.Add(i);
                        listViewLibrary.EnsureVisible(i);
                        break;
                    }
                    listViewLibrary.SelectedIndices.Add(i);
                }
            }

            if (!creatureSelected)
            {
                if (selectFirstIfNothingIsSelected && _creaturesDisplayed.Length != 0)
                {
                    _reactOnCreatureSelectionChange = true;
                    listViewLibrary.SelectedIndices.Add(0);
                    listViewLibrary.EnsureVisible(0);
                }
                else
                {
                    creatureBoxListView.Clear();
                }
            }

            _reactOnCreatureSelectionChange = true; // make sure it reacts again even if the previously creature is not visible anymore
        }

        /// <summary>
        /// Selects a creature in the library
        /// </summary>
        /// <param name="creature"></param>
        private void SelectCreatureInLibrary(Creature creature)
        {
            if (creature == null) return;

            var index = Array.IndexOf(_creaturesDisplayed, creature);
            if (index == -1) return;

            _reactOnCreatureSelectionChange = false;
            listViewLibrary.SelectedIndices.Clear();
            _reactOnCreatureSelectionChange = true;
            listViewLibrary.SelectedIndices.Add(index);
            listViewLibrary.EnsureVisible(index);
        }

        private void UpdateCreatureListViewItem(Creature creature)
        {
            // int listViewLibrary replace old row with new one
            var index = Array.IndexOf(_creaturesDisplayed, creature);
            if (index == -1) return; // not in cache currently
            var cacheIndex = index - _libraryItemCacheFirstIndex;
            if (cacheIndex >= 0 && cacheIndex < _libraryListViewItemCache.Length)
            {
                _libraryListViewItemCache[cacheIndex] = CreateCreatureLvItem(creature, Properties.Settings.Default.DisplayLibraryCreatureIndex);
            }
        }

        private const int ColumnIndexName = 0;
        private const int ColumnIndexSex = 4;
        private const int ColumnIndexAdded = 5;
        private const int ColumnIndexTopness = 6;
        private const int ColumnIndexTopStats = 7;
        private const int ColumnIndexGeneration = 8;
        private const int ColumnIndexWildLevel = 9;
        private const int ColumnIndexMutations = 10;
        private const int ColumnIndexCountdown = 11;
        private const int ColumnIndexFirstStat = 12;
        private const int ColumnIndexFirstColor = 36;
        private const int ColumnIndexPostColor = 42;
        private const int ColumnIndexMutagenApplied = 46;

        private ListViewItem CreateCreatureLvItem(Creature cr, bool displayIndex = false)
        {
            if (cr.flags.HasFlag(CreatureFlags.Divider))
            {
                return new ListViewItem(Enumerable.Repeat(string.Empty, listViewLibrary.Columns.Count).ToArray())
                {
                    Tag = cr
                };
            }

            double colorFactor = 100d / _creatureCollection.maxChartLevel;

            string[] subItems = new[] {
                        (displayIndex ? cr.ListIndex + " - " : string.Empty) +
                        cr.name,
                        cr.owner,
                        cr.note,
                        cr.server,
                        Utils.SexSymbol(cr.sex),
                        cr.domesticatedAt?.ToString("yyyy'-'MM'-'dd HH':'mm':'ss") ?? string.Empty,
                        (cr.topness / 10).ToString(),
                        cr.topStatsCount.ToString(),
                        cr.generation.ToString(),
                        cr.levelFound.ToString(),
                        cr.Mutations.ToString(),
                        DisplayedCreatureCountdown(cr, out var cooldownForeColor, out var cooldownBackColor)
                    }
                    .Concat(cr.levelsWild.Select(l => l.ToString()))
                    .Concat((cr.levelsMutated ?? new int[Stats.StatsCount]).Select(l => l.ToString()))
                    .Concat(Properties.Settings.Default.showColorsInLibrary
                        ? cr.colors.Select(cl => cl.ToString())
                        : new string[Ark.ColorRegionCount]
                        )
                    .Concat(new[] {
                        cr.Species.DescriptiveNameAndMod,
                        cr.Status.ToString(),
                        cr.tribe,
                        Utils.StatusSymbol(cr.Status, string.Empty),
                        (cr.flags & CreatureFlags.MutagenApplied) != 0 ? "M" : string.Empty
                    })
                    .ToArray();

            // check if groups for species are displayed
            ListViewItem lvi = new ListViewItem(subItems) { Tag = cr };

            // apply colors to the subItems

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (cr.valuesDom[s] == 0)
                {
                    // not used
                    lvi.SubItems[ColumnIndexFirstStat + s].ForeColor = Color.White;
                    lvi.SubItems[ColumnIndexFirstStat + s].BackColor = Color.White;
                }
                else if (cr.levelsWild[s] < 0)
                {
                    // unknown level 
                    lvi.SubItems[ColumnIndexFirstStat + s].ForeColor = Color.WhiteSmoke;
                    lvi.SubItems[ColumnIndexFirstStat + s].BackColor = Color.White;
                }
                else
                    lvi.SubItems[ColumnIndexFirstStat + s].BackColor = Utils.GetColorFromPercent((int)(cr.levelsWild[s] * (s == Stats.Torpidity ? colorFactor / 7 : colorFactor)), // TODO set factor to number of other stats (flyers have 6, Gacha has 8?)
                            _considerStatHighlight[s] ? cr.topBreedingStats[s] ? 0.2 : 0.7 : 0.93);

                // mutated levels
                if (cr.levelsMutated == null || cr.valuesDom[s] == 0)
                {
                    // not used
                    lvi.SubItems[ColumnIndexFirstStat + Stats.StatsCount + s].ForeColor = Color.White;
                    lvi.SubItems[ColumnIndexFirstStat + Stats.StatsCount + s].BackColor = Color.White;
                }
                else
                    lvi.SubItems[ColumnIndexFirstStat + Stats.StatsCount + s].BackColor = Utils.GetColorFromPercent((int)(cr.levelsMutated[s] * (s == Stats.Torpidity ? colorFactor / 7 : colorFactor)),
                            _considerStatHighlight[s] ? cr.topBreedingStats[s] ? 0.2 : 0.7 : 0.93);
            }
            lvi.SubItems[ColumnIndexSex].BackColor = cr.flags.HasFlag(CreatureFlags.Neutered) ? Color.FromArgb(220, 220, 220) :
                    cr.sex == Sex.Female ? Color.FromArgb(255, 230, 255) :
                    cr.sex == Sex.Male ? Color.FromArgb(220, 235, 255) : SystemColors.Window;

            switch (cr.Status)
            {
                case CreatureStatus.Dead:
                    lvi.SubItems[ColumnIndexName].ForeColor = SystemColors.GrayText;
                    lvi.BackColor = Color.FromArgb(255, 250, 240);
                    break;
                case CreatureStatus.Unavailable:
                    lvi.SubItems[ColumnIndexName].ForeColor = SystemColors.GrayText;
                    break;
                case CreatureStatus.Obelisk:
                    lvi.SubItems[ColumnIndexName].ForeColor = Color.DarkBlue;
                    break;
                default:
                    {
                        if (_creatureCollection.maxServerLevel > 0
                            && cr.levelsWild[Stats.Torpidity] + 1 + _creatureCollection.maxDomLevel > _creatureCollection.maxServerLevel + (cr.Species.name.StartsWith("X-") || cr.Species.name.StartsWith("R-") ? 50 : 0))
                        {
                            lvi.SubItems[ColumnIndexName].ForeColor = Color.OrangeRed; // this creature may pass the max server level and could be deleted by the game
                        }
                        break;
                    }
            }

            lvi.UseItemStyleForSubItems = false;

            // color for top-stats-nr
            if (cr.topStatsCount > 0)
            {
                if (Properties.Settings.Default.LibraryHighlightTopCreatures && cr.topBreedingCreature)
                {
                    if (cr.onlyTopConsideredStats)
                        lvi.BackColor = Color.Gold;
                    else
                        lvi.BackColor = Color.LightGreen;
                }
                lvi.SubItems[ColumnIndexTopStats].BackColor = Utils.GetColorFromPercent(cr.topStatsCount * 8 + 44, 0.7);
            }
            else
            {
                lvi.SubItems[ColumnIndexTopStats].ForeColor = Color.LightGray;
            }

            // color for timestamp domesticated
            if (cr.domesticatedAt == null || cr.domesticatedAt.Value.Year < 2015)
            {
                lvi.SubItems[ColumnIndexAdded].Text = "n/a";
                lvi.SubItems[ColumnIndexAdded].ForeColor = Color.LightGray;
            }

            // color for topness
            lvi.SubItems[ColumnIndexTopness].BackColor = Utils.GetColorFromPercent(cr.topness / 5 - 100, 0.8); // topness is in permille. gradient from 50-100

            // color for generation
            if (cr.generation == 0)
                lvi.SubItems[ColumnIndexGeneration].ForeColor = Color.LightGray;

            // color of WildLevelColumn
            if (cr.levelFound == 0)
                lvi.SubItems[ColumnIndexWildLevel].ForeColor = Color.LightGray;

            // color for mutation
            if (cr.Mutations > 0)
            {
                if (cr.Mutations < Ark.MutationPossibleWithLessThan)
                    lvi.SubItems[ColumnIndexMutations].BackColor = Utils.MutationColor;
                else
                    lvi.SubItems[ColumnIndexMutations].BackColor = Utils.MutationColorOverLimit;
            }
            else
                lvi.SubItems[ColumnIndexMutations].ForeColor = Color.LightGray;

            // color for cooldown
            lvi.SubItems[ColumnIndexCountdown].ForeColor = cooldownForeColor;
            lvi.SubItems[ColumnIndexCountdown].BackColor = cooldownBackColor;

            if (Properties.Settings.Default.showColorsInLibrary)
            {
                // color for colors
                for (int cl = 0; cl < Ark.ColorRegionCount; cl++)
                {
                    if (cr.colors[cl] != 0)
                    {
                        lvi.SubItems[ColumnIndexFirstColor + cl].BackColor = CreatureColors.CreatureColor(cr.colors[cl]);
                        lvi.SubItems[ColumnIndexFirstColor + cl].ForeColor = Utils.ForeColor(lvi.SubItems[ColumnIndexFirstColor + cl].BackColor);
                    }
                    else
                    {
                        lvi.SubItems[ColumnIndexFirstColor + cl].ForeColor = cr.Species.EnabledColorRegions[cl] ? Color.LightGray : Color.White;
                    }
                }
            }

            return lvi;
        }

        /// <summary>
        /// Returns the dateTime when the countdown of a creature is ready. Either the maturingTime, the matingCooldownTime or null if no countdown is set.
        /// </summary>
        /// <returns></returns>
        private string DisplayedCreatureCountdown(Creature cr, out Color foreColor, out Color backColor)
        {
            foreColor = SystemColors.ControlText;
            backColor = SystemColors.Window;
            DateTime dt;
            var isGrowing = true;
            var useGrowingLeft = false;
            var now = DateTime.Now;
            if (cr.cooldownUntil.HasValue && cr.cooldownUntil.Value > now)
            {
                isGrowing = false;
                dt = cr.cooldownUntil.Value;
            }
            else if (!cr.growingUntil.HasValue || cr.growingUntil.Value <= now)
            {
                foreColor = Color.LightGray;
                return "-";
            }
            else if (!cr.growingPaused)
            {
                dt = cr.growingUntil.Value;
            }
            else
            {
                useGrowingLeft = true;
                dt = new DateTime();
            }

            if (!useGrowingLeft && now > dt)
            {
                foreColor = Color.LightGray;
                return "-";
            }

            double minCld;
            if (useGrowingLeft)
                minCld = cr.growingLeft.TotalMinutes;
            else
                minCld = dt.Subtract(now).TotalMinutes;

            if (isGrowing)
            {
                // growing
                if (minCld < 1)
                    backColor = Color.FromArgb(168, 187, 255); // light blue
                else if (minCld < 10)
                    backColor = Color.FromArgb(197, 168, 255); // light blue/pink
                else
                    backColor = Color.FromArgb(236, 168, 255); // light pink
            }
            else
            {
                // mating-cooldown
                if (minCld < 1)
                    backColor = Color.FromArgb(235, 255, 109); // green-yellow
                else if (minCld < 10)
                    backColor = Color.FromArgb(255, 250, 109); // yellow
                else
                    backColor = Color.FromArgb(255, 179, 109); // yellow-orange
            }

            return useGrowingLeft ? Utils.Duration(cr.growingLeft) : dt.ToString();
        }

        private readonly CreatureListSorter _creatureListSorter = new CreatureListSorter();

        private void libraryListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SortLibrary(e.Column);
        }

        /// <summary>
        /// /// Sort the library by given column index. If the columnIndex is -1, use last sorting.
        /// </summary>
        private void SortLibrary(int columnIndex = -1)
        {
            listViewLibrary.BeginUpdate();

            var selectedCreatures = new HashSet<Creature>();
            foreach (int i in listViewLibrary.SelectedIndices)
                selectedCreatures.Add(_creaturesDisplayed[i]);

            var sorted = _creatureListSorter.DoSort(_creaturesDisplayed.Where(c => !c.flags.HasFlag(CreatureFlags.Divider)), columnIndex, Properties.Settings.Default.LibraryGroupBySpecies ? _speciesInLibraryOrdered : null);
            _creaturesDisplayed = Properties.Settings.Default.LibraryGroupBySpecies ? InsertDividers(sorted) : sorted;
            _libraryListViewItemCache = null;
            listViewLibrary.EndUpdate();
            SelectCreaturesInLibrary(selectedCreatures);
        }

        private readonly Debouncer _libraryIndexChangedDebouncer = new Debouncer();

        // onLibraryChange
        private void listViewLibrary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_reactOnCreatureSelectionChange)
                _libraryIndexChangedDebouncer.Debounce(100, LibrarySelectedIndexChanged, Dispatcher.CurrentDispatcher);
        }

        /// <summary>
        /// Updates infos about the selected creatures like tags, levels and stat-level distribution.
        /// </summary>
        private void LibrarySelectedIndexChanged()
        {
            // remove dividers from selection
            foreach (int i in listViewLibrary.SelectedIndices)
            {
                if (_creaturesDisplayed[i].flags.HasFlag(CreatureFlags.Divider))
                    listViewLibrary.SelectedIndices.Remove(i);
            }

            int cnt = listViewLibrary.SelectedIndices.Count;
            if (cnt == 0)
            {
                SetMessageLabelText();
                creatureBoxListView.Clear();
                return;
            }

            if (cnt == 1)
            {
                Creature c = _creaturesDisplayed[listViewLibrary.SelectedIndices[0]];
                creatureBoxListView.SetCreature(c);
                if (tabControlLibFilter.SelectedTab == tabPageLibRadarChart)
                    radarChartLibrary.SetLevels(c.levelsWild);
                pedigree1.PedigreeNeedsUpdate = true;
            }

            // display infos about the selected creatures
            var selCrs = new List<Creature>(cnt);

            foreach (int i in listViewLibrary.SelectedIndices)
                selCrs.Add(_creaturesDisplayed[i]);

            List<string> tagList = new List<string>();
            foreach (Creature cr in selCrs)
            {
                foreach (string t in cr.tags)
                    if (!tagList.Contains(t))
                        tagList.Add(t);
            }
            tagList.Sort();

            SetMessageLabelText($"{cnt} creatures selected, " +
                    $"{selCrs.Count(cr => cr.sex == Sex.Female)} females, " +
                    $"{selCrs.Count(cr => cr.sex == Sex.Male)} males\r\n" +
                    (cnt == 1
                        ? $"level: {selCrs[0].Level}; Ark-Id (ingame): " + (selCrs[0].ArkIdImported ? Utils.ConvertImportedArkIdToIngameVisualization(selCrs[0].ArkId) : selCrs[0].ArkId.ToString())
                        : $"level-range: {selCrs.Min(cr => cr.Level)} - {selCrs.Max(cr => cr.Level)}"
                    ) + "\r\n" +
                    $"Tags: {string.Join(", ", tagList)}");
        }

        /// <summary>
        /// Display the creatures with the current filter.
        /// Recalculate all filters.
        /// </summary>
        private void FilterLibRecalculate(bool selectFirstIfNothingIsSelected = false)
        {
            _creaturesPreFiltered = null;
            FilterLib(selectFirstIfNothingIsSelected);
        }

        /// <summary>
        /// Display the creatures with the current filter.
        /// Use the pre filtered list (if available) and only apply the live filter.
        /// </summary>
        private void FilterLib(bool selectFirstIfNothingIsSelected = false)
        {
            if (!_filterListAllowed)
                return;

            // save selected creatures to re-select them after the filtering
            var selectedCreatures = new HashSet<Creature>();
            foreach (int i in listViewLibrary.SelectedIndices)
                selectedCreatures.Add(_creaturesDisplayed[i]);

            IEnumerable<Creature> filteredList;

            if (_creaturesPreFiltered == null)
            {
                filteredList = from creature in _creatureCollection.creatures
                               where creature.Species != null && !creature.flags.HasFlag(CreatureFlags.Placeholder)
                               select creature;

                // if only one species should be shown adjust headers if the selected species has custom statNames
                Dictionary<string, string> customStatNames = null;
                if (listBoxSpeciesLib.SelectedItem is Species selectedSpecies)
                {
                    filteredList = filteredList.Where(c => c.Species == selectedSpecies);
                    customStatNames = selectedSpecies.statNames;
                }

                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    listViewLibrary.Columns[ColumnIndexFirstStat + s].Text = Utils.StatName(s, true, customStatNames);
                    listViewLibrary.Columns[ColumnIndexFirstStat + Stats.StatsCount + s].Text = Utils.StatName(s, true, customStatNames) + "M";
                }

                _creaturesPreFiltered = ApplyLibraryFilterSettings(filteredList).ToArray();
            }

            filteredList = _creaturesPreFiltered;
            // apply live filter
            var filterString = ToolStripTextBoxLibraryFilter.Text.Trim();
            if (!string.IsNullOrEmpty(filterString))
            {
                // filter parameter are separated by commas and all parameter must be found on an item to have it included
                var filterStrings = filterString.Split(',').Select(f => f.Trim())
                    .Where(f => !string.IsNullOrEmpty(f)).ToList();

                // extract stat level filter
                var statGreaterThan = new Dictionary<int, int>();
                var statLessThan = new Dictionary<int, int>();
                var statEqualTo = new Dictionary<int, int>();
                var statFilterRegex = new Regex(@"(\w{2}) ?(<|>|==) ?(\d+)");

                // color filter
                var colorFilterOr = new Dictionary<int[], int[]>(); // includes creatures that have in one of the regions one of the colors
                var colorFilterRegexOr = new Regex(@"c([0-5 ]+): ?([\d ]+)");

                // mutation filter
                var mutationFilterEqualTo = -1;
                var mutationFilterGreaterThan = -1;
                var mutationFilterLessThan = -1;

                var removeFilterIndex = new List<int>(); // remove all filter entries that are added to specific filter properties
                // start at the end, so the removed filter indices are also removed from the end
                for (var i = filterStrings.Count - 1; i >= 0; i--)
                {
                    var f = filterStrings[i];

                    // color region filter
                    var m = colorFilterRegexOr.Match(f);
                    if (m.Success)
                    {
                        var colorIds = m.Groups[2].Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(int.Parse).Distinct().ToArray();
                        if (!colorIds.Any()) continue;

                        var colorRegions = m.Groups[1].Value.Where(r => r != ' ').Select(r => int.Parse(r.ToString())).ToArray();

                        colorFilterOr[colorRegions] = colorIds;
                        removeFilterIndex.Add(i);
                        continue;
                    }

                    // stat filter
                    m = statFilterRegex.Match(f);
                    if (!m.Success) continue;
                    if (!Utils.StatAbbreviationToIndex.TryGetValue(m.Groups[1].Value, out var statIndex))
                    {
                        // mutations
                        if (m.Groups[1].Value == "mu")
                        {
                            switch (m.Groups[2].Value)
                            {
                                case ">":
                                    mutationFilterGreaterThan = int.Parse(m.Groups[3].Value);
                                    break;
                                case "<":
                                    mutationFilterLessThan = int.Parse(m.Groups[3].Value);
                                    break;
                                case "==":
                                    mutationFilterEqualTo = int.Parse(m.Groups[3].Value);
                                    break;
                            }
                            removeFilterIndex.Add(i);
                        }
                        continue;
                    }

                    switch (m.Groups[2].Value)
                    {
                        case ">":
                            statGreaterThan[statIndex] = int.Parse(m.Groups[3].Value);
                            break;
                        case "<":
                            statLessThan[statIndex] = int.Parse(m.Groups[3].Value);
                            break;
                        case "==":
                            statEqualTo[statIndex] = int.Parse(m.Groups[3].Value);
                            break;
                    }
                    removeFilterIndex.Add(i);
                }

                if (!statGreaterThan.Any()) statGreaterThan = null;
                if (!statLessThan.Any()) statLessThan = null;
                if (!statEqualTo.Any()) statEqualTo = null;
                if (!colorFilterOr.Any()) colorFilterOr = null;
                foreach (var i in removeFilterIndex)
                    filterStrings.RemoveAt(i);

                filteredList = filteredList.Where(c => filterStrings.All(f =>
                    c.name.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) != -1
                    || (c.Species?.name.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) ?? -1) != -1
                    || (c.owner?.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) ?? -1) != -1
                    || (c.tribe?.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) ?? -1) != -1
                    || (c.note?.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) ?? -1) != -1
                    || (c.ArkIdInGame?.StartsWith(f) ?? false)
                    || (c.server?.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) ?? -1) != -1
                    || (c.tags?.Any(t => string.Equals(t, f, StringComparison.InvariantCultureIgnoreCase)) ?? false)
                )
                && (statGreaterThan?.All(si => c.levelsWild[si.Key] > si.Value) ?? true)
                && (statLessThan?.All(si => c.levelsWild[si.Key] < si.Value) ?? true)
                && (statEqualTo?.All(si => c.levelsWild[si.Key] == si.Value) ?? true)
                && (colorFilterOr?.All(colorRegions => colorRegions.Key.Any(colorRegion => colorRegions.Value.Contains(c.colors[colorRegion]))) ?? true)
                && (mutationFilterGreaterThan == -1 || mutationFilterGreaterThan < c.Mutations)
                && (mutationFilterLessThan == -1 || mutationFilterLessThan > c.Mutations)
                && (mutationFilterEqualTo == -1 || mutationFilterEqualTo == c.Mutations)
                );
            }

            // display new results
            ShowCreaturesInListView(filteredList);

            // select previous selected creatures again
            SelectCreaturesInLibrary(selectedCreatures, selectFirstIfNothingIsSelected);
        }

        /// <summary>
        /// Apply library filter settings to a creature collection
        /// </summary>
        private IEnumerable<Creature> ApplyLibraryFilterSettings(IEnumerable<Creature> creatures)
        {
            if (creatures == null)
                return Enumerable.Empty<Creature>();

            if (Properties.Settings.Default.FilterHideOwners?.Any() ?? false)
                creatures = creatures.Where(c => !Properties.Settings.Default.FilterHideOwners.Contains(c.owner ?? string.Empty));

            if (Properties.Settings.Default.FilterHideTribes?.Any() ?? false)
                creatures = creatures.Where(c => !Properties.Settings.Default.FilterHideTribes.Contains(c.tribe ?? string.Empty));

            if (Properties.Settings.Default.FilterHideServers?.Any() ?? false)
                creatures = creatures.Where(c => !Properties.Settings.Default.FilterHideServers.Contains(c.server ?? string.Empty));

            if (Properties.Settings.Default.FilterOnlyIfColorId != 0)
                creatures = creatures.Where(c => c.colors.Contains(Properties.Settings.Default.FilterOnlyIfColorId));

            if (Properties.Settings.Default.FilterHideAdults)
                creatures = creatures.Where(c => c.Maturation < 1);
            if (Properties.Settings.Default.FilterHideNonAdults)
                creatures = creatures.Where(c => c.Maturation >= 1);
            if (Properties.Settings.Default.FilterHideCooldowns)
                creatures = creatures.Where(c => c.cooldownUntil == null || c.cooldownUntil < DateTime.Now);
            if (Properties.Settings.Default.FilterHideNonCooldowns)
                creatures = creatures.Where(c => c.cooldownUntil != null && c.cooldownUntil > DateTime.Now);

            // tags filter
            if (Properties.Settings.Default.FilterHideTags?.Any() ?? false)
            {
                bool hideCreaturesWOTags = Properties.Settings.Default.FilterHideTags.Contains(string.Empty);
                creatures = creatures.Where(c =>
                    !hideCreaturesWOTags && c.tags.Count == 0 ||
                    c.tags.Except(Properties.Settings.Default.FilterHideTags).Any());
            }

            // hide creatures with the set hide flags
            if (Properties.Settings.Default.FilterFlagsExclude != 0)
            {
                creatures = creatures.Where(c => ((int)c.flags & Properties.Settings.Default.FilterFlagsExclude) == 0);
            }
            if (Properties.Settings.Default.FilterFlagsAllNeeded != 0)
            {
                creatures = creatures.Where(c => ((int)c.flags & Properties.Settings.Default.FilterFlagsAllNeeded) == Properties.Settings.Default.FilterFlagsAllNeeded);
            }
            if (Properties.Settings.Default.FilterFlagsOneNeeded != 0)
            {
                int flagsOneNeeded = Properties.Settings.Default.FilterFlagsOneNeeded |
                                     Properties.Settings.Default.FilterFlagsAllNeeded;
                creatures = creatures.Where(c => ((int)c.flags & flagsOneNeeded) != 0);
            }

            return creatures;
        }

        private void listBoxSpeciesLib_Click(object sender, EventArgs e)
        {
            if (!(ModifierKeys == Keys.Control && listBoxSpeciesLib.SelectedItem is Species species)) return;

            Values.V.ToggleSpeciesFavorite(species);
            UpdateSpeciesLists(_creatureCollection.creatures);
        }

        private void listViewLibrary_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    DeleteSelectedCreatures();
                    break;
                case Keys.F2:
                    if (listViewLibrary.SelectedIndices.Count > 0)
                        EditCreatureInTester(_creaturesDisplayed[listViewLibrary.SelectedIndices[0]]);
                    break;
                case Keys.F3:
                    if (listViewLibrary.SelectedIndices.Count > 0)
                        ShowMultiSetter();
                    break;
                case Keys.F5:
                    if (listViewLibrary.SelectedIndices.Count > 0)
                        AdminCommandToSetColors();
                    break;
                case Keys.A when e.Control:
                    // select all list-entries
                    _reactOnCreatureSelectionChange = false;
                    listViewLibrary.BeginUpdate();
                    listViewLibrary.SelectAllItems();
                    listViewLibrary.EndUpdate();
                    _reactOnCreatureSelectionChange = true;
                    listViewLibrary_SelectedIndexChanged(null, null);
                    break;
                case Keys.B when e.Control:
                    CopySelectedCreatureName();
                    break;
                default: return;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Copies the data of the selected creatures to the clipboard for use in a spreadsheet.
        /// </summary>
        private void ExportForSpreadsheet()
        {
            if (tabControlMain.SelectedTab == tabPageLibrary)
            {
                if (Properties.Settings.Default.CreatureTableExportFields?.Any() == false)
                {
                    if (MessageBox.Show("No fields for the table export selected.\nDo you want to go to the options to edit the export fields?", "No Export Fields set",
                         MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        OpenSettingsDialog(Settings.SettingsTabPages.General);
                    return;
                }
                if (listViewLibrary.SelectedIndices.Count > 0)
                {
                    var exportCount = ExportImportCreatures.ExportTable(listViewLibrary.SelectedIndices.Cast<int>().Select(i => _creaturesDisplayed[i]).ToArray());
                    if (exportCount != 0)
                        SetMessageLabelText($"{exportCount} creatures were exported to the clipboard for pasting in a spreadsheet.", MessageBoxIcon.Information);

                    return;
                }
                MessageBox.Show("No creatures in the library selected to copy to the clipboard", "No Creatures Selected",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (tabControlMain.SelectedTab == tabPageExtractor)
                CopyExtractionToClipboard();
        }

        private void editSpreadsheetExportFieldsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSettingsDialog(Settings.SettingsTabPages.General);
        }

        /// <summary>
        /// Display a window to edit multiple creatures at once. Also used to set tags.
        /// </summary>
        private void ShowMultiSetter()
        {
            // shows a dialog to set multiple settings to all selected creatures
            if (listViewLibrary.SelectedIndices.Count <= 0)
                return;
            List<Creature> selectedCreatures = new List<Creature>();

            // check if multiple species are selected
            bool multipleSpecies = false;
            Species sp = _creaturesDisplayed[listViewLibrary.SelectedIndices[0]].Species;
            foreach (int i in listViewLibrary.SelectedIndices)
            {
                var cr = _creaturesDisplayed[i];
                selectedCreatures.Add(cr);
                if (!multipleSpecies && cr.speciesBlueprint != sp.blueprintPath)
                {
                    multipleSpecies = true;
                }
            }
            List<Creature>[] parents = null;
            if (!multipleSpecies)
                parents = FindPossibleParents(new Creature(sp));

            using (MultiSetter ms = new MultiSetter(selectedCreatures,
                parents,
                _creatureCollection.tags,
                Values.V.species,
                _creatureCollection.ownerList,
                _creatureCollection.tribes.Select(t => t.TribeName).ToArray(),
                _creatureCollection.serverList))
            {
                if (ms.ShowDialog() == DialogResult.OK)
                {
                    if (ms.ParentsChanged)
                        UpdateParents(selectedCreatures);
                    if (ms.TagsChanged)
                        CreateCreatureTagList();
                    if (ms.SpeciesChanged)
                    {
                        UpdateSpeciesLists(_creatureCollection.creatures);
                        foreach (var c in selectedCreatures)
                            c.RecalculateCreatureValues(_creatureCollection.wildLevelStep);
                    }
                    UpdateOwnerServerTagLists();
                    SetCollectionChanged(true, !multipleSpecies ? sp : null);
                    RecalculateTopStatsIfNeeded();
                    FilterLibRecalculate();
                }
            }
        }

        private readonly Debouncer _filterLibraryDebouncer = new Debouncer();

        private void ToolStripTextBoxLibraryFilter_TextChanged(object sender, EventArgs e)
        {
            _filterLibraryDebouncer.Debounce(ToolStripTextBoxLibraryFilter.Text == string.Empty ? 0 : 500, FilterLib, Dispatcher.CurrentDispatcher, false);
        }

        private void ToolStripButtonLibraryFilterClear_Click(object sender, EventArgs e)
        {
            if (_libraryFilterTemplates != null && !_libraryFilterTemplates.IsDisposed)
                _libraryFilterTemplates.ControlVisibility = false;
            ToolStripTextBoxLibraryFilter.Clear();
            ToolStripTextBoxLibraryFilter.Focus();
        }

        /// <summary>
        /// User can select a folder where infoGraphics for all selected creatures are saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveInfographicsToFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count == 0) return;

            var initialFolder = Properties.Settings.Default.InfoGraphicExportFolder;
            if (string.IsNullOrEmpty(initialFolder) || !Directory.Exists(initialFolder))
                initialFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string folderPath = null;
            using (var fs = new FolderBrowserDialog
            {
                SelectedPath = initialFolder
            })
            {
                if (fs.ShowDialog() == DialogResult.OK)
                    folderPath = fs.SelectedPath;
            }

            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath)) return;

            Properties.Settings.Default.InfoGraphicExportFolder = folderPath;

            // test if files can be written to the folder
            var testFileName = "testFile.txt";
            try
            {
                var testFilePath = Path.Combine(folderPath, testFileName);
                File.WriteAllText(testFilePath, string.Empty);
                FileService.TryDeleteFile(testFilePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, $"The selected folder\n{folderPath}\nis protected, the files cannot be saved there. Select a different folder.");
                return;
            }

            int imagesCreated = 0;
            string firstImageFilePath = null;

            var invalidCharacters = Path.GetInvalidFileNameChars();

            foreach (int i in listViewLibrary.SelectedIndices)
            {
                var c = _creaturesDisplayed[i];

                var fileName = $"{c.Species.name}_{(string.IsNullOrEmpty(c.name) ? c.guid.ToString() : c.name)}";
                foreach (var invalidChar in invalidCharacters)
                    fileName = fileName.Replace(invalidChar, '_');

                var filePath = Path.Combine(folderPath, $"ARK_info_{fileName}.png");

                if (File.Exists(filePath))
                {
                    switch (MessageBox.Show($"The file\n{filePath}\nalready exists.\nOverwrite the file?", "File exists already", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                    {
                        case DialogResult.No: continue;
                        case DialogResult.Yes: break;
                        default: return;
                    }
                }
                c.InfoGraphic(_creatureCollection).Save(filePath);
                if (firstImageFilePath == null) firstImageFilePath = filePath;

                imagesCreated++;
            }

            if (imagesCreated == 0) return;

            var pluralS = (imagesCreated != 1 ? "s" : string.Empty);
            SetMessageLabelText($"Infographic{pluralS} for {imagesCreated} creature{pluralS} created at\r\n{(imagesCreated == 1 ? firstImageFilePath : folderPath)}", MessageBoxIcon.Information, firstImageFilePath);
        }

        #region Library ContextMenu

        private void toolStripMenuItemEdit_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count > 0)
                EditCreatureInTester(_creaturesDisplayed[listViewLibrary.SelectedIndices[0]]);
        }

        private void toolStripMenuItemRemove_Click(object sender, EventArgs e)
        {
            DeleteSelectedCreatures();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetStatusOfSelectedCreatures(CreatureStatus.Available);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SetStatusOfSelectedCreatures(CreatureStatus.Unavailable);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            SetStatusOfSelectedCreatures(CreatureStatus.Dead);
        }

        private void obeliskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStatusOfSelectedCreatures(CreatureStatus.Obelisk);
        }

        private void cryopodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStatusOfSelectedCreatures(CreatureStatus.Cryopod);
        }

        private void currentValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count > 0)
                SetCreatureValuesToExtractor(_creaturesDisplayed[listViewLibrary.SelectedIndices[0]]);
        }

        private void wildValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count > 0)
                SetCreatureValuesToExtractor(_creaturesDisplayed[listViewLibrary.SelectedIndices[0]],
                    true);
        }

        private void SetMatureBreedingStateOfSelectedCreatures(bool setMature = false, bool clearMatingCooldown = false,
            bool justMated = false)
        {
            listViewLibrary.BeginUpdate();
            foreach (int i in listViewLibrary.SelectedIndices)
            {
                var c = _creaturesDisplayed[i];
                if (setMature && c.growingUntil > DateTime.Now)
                    c.growingUntil = null;

                if (clearMatingCooldown && c.cooldownUntil > DateTime.Now)
                    c.cooldownUntil = null;

                if (justMated)
                    c.cooldownUntil = DateTime.Now.AddSeconds(c.Species.breeding?.matingCooldownMinAdjusted ?? 0);

                UpdateCreatureListViewItem(c);
            }

            breedingPlan1.BreedingPlanNeedsUpdate = true;
            listViewLibrary.EndUpdate();
        }

        private void setToMatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMatureBreedingStateOfSelectedCreatures(setMature: true);
        }

        private void clearMatingCooldownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMatureBreedingStateOfSelectedCreatures(clearMatingCooldown: true);
        }

        private void justMatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMatureBreedingStateOfSelectedCreatures(justMated: true);
        }

        private void applyMutagenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // a tamed creature receives 5 level in hp, st, we, dm (i.e. a total of 20 levels)
            // a bred creature receives 1 level in hp, st, we, dm (i.e. a total of 4 levels)

            bool libraryChanged = false;
            var affectedSpeciesBlueprints = new List<string>();

            var statCountAffectedByMutagen = Ark.StatIndicesAffectedByMutagen.Length;

            foreach (int i in listViewLibrary.SelectedIndices)
            {
                var c = _creaturesDisplayed[i];

                if (!c.isDomesticated
                    || c.flags.HasFlag(CreatureFlags.MutagenApplied)) continue;

                var levelIncrease = c.isBred ? Ark.MutagenLevelUpsBred : Ark.MutagenLevelUpsNonBred;

                foreach (var si in Ark.StatIndicesAffectedByMutagen)
                    c.levelsWild[si] += levelIncrease;
                c.levelsWild[Stats.Torpidity] += statCountAffectedByMutagen * levelIncrease;

                c.flags |= CreatureFlags.MutagenApplied;

                libraryChanged = true;
                if (!affectedSpeciesBlueprints.Contains(c.speciesBlueprint))
                    affectedSpeciesBlueprints.Add(c.speciesBlueprint);
            }

            if (!libraryChanged) return;

            // update list / recalculate topStats
            CalculateTopStats(_creatureCollection.creatures
                .Where(c => affectedSpeciesBlueprints.Contains(c.speciesBlueprint)).ToList());
            FilterLibRecalculate();
            UpdateStatusBar();
            SetCollectionChanged(true,
                affectedSpeciesBlueprints.Count == 1 ? Values.V.SpeciesByBlueprint(affectedSpeciesBlueprints.First()) : null);
        }

        private void adminCommandToSetColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdminCommandToSetColors();
        }

        private void AdminCommandToSetColors()
        {
            if (listViewLibrary.SelectedIndices.Count == 0) return;

            var cr = _creaturesDisplayed[listViewLibrary.SelectedIndices[0]];
            byte[] cl = cr.colors;
            if (cl == null) return;
            var colorCommands = new List<string>(Ark.ColorRegionCount);
            for (int ci = 0; ci < Ark.ColorRegionCount; ci++)
            {
                if (cr.Species.EnabledColorRegions[ci])
                    colorCommands.Add($"setTargetDinoColor {ci} {cl[ci]}");
            }

            if (colorCommands.Any())
            {
                var cheatPrefix = Properties.Settings.Default.AdminConsoleCommandWithCheat
                    ? "cheat "
                    : string.Empty;
                Clipboard.SetText(cheatPrefix + string.Join(" | " + cheatPrefix, colorCommands));
            }
        }

        private void adminCommandToSpawnExactDinoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count > 0)
                CreateExactSpawnCommand(_creaturesDisplayed[listViewLibrary.SelectedIndices[0]]);
        }

        private void adminCommandToSpawnExactDinoDS2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewLibrary.SelectedIndices.Count > 0)
                CreateExactSpawnDS2Command(_creaturesDisplayed[listViewLibrary.SelectedIndices[0]]);
        }

        private void exactSpawnCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Creature cr = null;
            if (tabControlMain.SelectedTab == tabPageExtractor)
                cr = CreateCreatureFromExtractorOrTester(creatureInfoInputExtractor);
            else if (tabControlMain.SelectedTab == tabPageStatTesting)
                cr = CreateCreatureFromExtractorOrTester(creatureInfoInputTester);
            if (cr == null) return;
            CreateExactSpawnCommand(cr);
        }

        private void exactSpawnCommandDS2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Creature cr = null;
            if (tabControlMain.SelectedTab == tabPageExtractor)
                cr = CreateCreatureFromExtractorOrTester(creatureInfoInputExtractor);
            else if (tabControlMain.SelectedTab == tabPageStatTesting)
                cr = CreateCreatureFromExtractorOrTester(creatureInfoInputTester);
            if (cr == null) return;
            CreateExactSpawnDS2Command(cr);
        }

        private void CreateExactSpawnCommand(Creature cr)
        {
            CreatureSpawnCommand.InstableCommandToClipboard(cr);
            SetMessageLabelText($"The SpawnExactDino admin console command for the creature {cr.name} ({cr.Species?.name}) was copied to the clipboard. The command doesn't include the XP and the imprinterName, thus the imprinting is probably not set."
                                + "WARNING: this console command is unstable and can crash your game. Use with caution! The colors and stats will only be correct after putting the creature in a cryopod.", MessageBoxIcon.Warning);
        }

        private void CreateExactSpawnDS2Command(Creature cr)
        {
            CreatureSpawnCommand.DinoStorageV2CommandToClipboard(cr);
            SetMessageLabelText($"The SpawnExactDino admin console command for the creature {cr.name} ({cr.Species?.name}) was copied to the clipboard. The command needs the mod DinoStorage V2 installed on the server to work."
                                , MessageBoxIcon.Warning);
        }

        #endregion

        #region LibraryFilterPresets

        private LibraryFilterTemplates _libraryFilterTemplates;

        private void ToolStripButtonSaveFilterPresetClick(object sender, EventArgs e)
        {
            var text = ToolStripTextBoxLibraryFilter.Text.Trim();
            if (string.IsNullOrEmpty(text)) return;

            var presets = Properties.Settings.Default.LibraryFilterPresets;
            if (presets != null && presets.Contains(text)) return;

            int oldLength = presets?.Length ?? 0;
            var newPresets = new string[oldLength + 1];
            if (presets != null)
                Array.Copy(presets, newPresets, oldLength);
            newPresets[oldLength] = text;

            Properties.Settings.Default.LibraryFilterPresets = newPresets;
            _libraryFilterTemplates?.AddPreset(text);
        }

        private void ToolStripTextBoxLibraryFilter_Click(object sender, EventArgs e)
        {
            ToggleLibraryFilterPresets();
            ToolStripTextBoxLibraryFilter.Focus();
        }

        private void ToggleLibraryFilterPresets()
        {
            if (_libraryFilterTemplates == null || _libraryFilterTemplates.IsDisposed)
            {
                if (Properties.Settings.Default.LibraryFilterPresets == null)
                    return;

                _libraryFilterTemplates = new LibraryFilterTemplates
                {
                    Presets = Properties.Settings.Default.LibraryFilterPresets
                };
                _libraryFilterTemplates.StringSelected += _libraryFilterTemplates_StringSelected;
                _libraryFilterTemplates.Location = new Point(Location.X + ToolStripTextBoxLibraryFilter.Bounds.X, Location.Y + ToolStripTextBoxLibraryFilter.Bounds.Bottom + 60);
                _libraryFilterTemplates.Show(this);
                return;
            }

            _libraryFilterTemplates.ControlVisibility = !_libraryFilterTemplates.Visible;
        }

        private void _libraryFilterTemplates_StringSelected(string filterPreset)
        {
            ToolStripTextBoxLibraryFilter.Text = filterPreset;
            _libraryFilterTemplates.ControlVisibility = false;
        }

        #endregion

        private void importFromTabSeparatedFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = null;
            using (var ofd = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                CheckFileExists = true
            })
            {
                if (ofd.ShowDialog(this) == DialogResult.OK)
                    filePath = ofd.FileName;
            }

            if (string.IsNullOrEmpty(filePath)) return;

            if (!ExportImportCreatures.ImportCreaturesFromTsvFile(filePath, out var creatures, out var result))
            {
                MessageBoxes.ShowMessageBox(result, "Error while importing from tsv file");
                return;
            }

            _creatureCollection.MergeCreatureList(creatures);

            // update UI
            UpdateCreatureListings();
            SetCollectionChanged(true);

            if (_creatureCollection.creatures.Any())
                tabControlMain.SelectedTab = tabPageLibrary;

            // reapply last sorting
            SortLibrary();

            MessageBoxes.ShowMessageBox(result, "Creatures imported from tsv file", MessageBoxIcon.Information);
        }

        #region library list view columns

        private void resetColumnOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewLibrary.BeginUpdate();
            var colIndices = new[] { 1, 2, 4, 5, 6, 36, 31, 32, 33, 34, 35, 37, 7, 9, 29, 11, 13, 15, 17, 19, 21, 23, 25, 27, 8, 10, 30, 12, 14, 16, 18, 20, 22, 24, 26, 28, 40, 41, 42, 43, 44, 45, 46, 38, 3, 0, 39 };

            // indices have to be set increasingly, or they will "push" other values up
            var colIndicesOrdered = colIndices.Select((i, c) => (columnIndex: c, displayIndex: i))
                .OrderBy(c => c.displayIndex).ToArray();
            for (int c = 0; c < colIndicesOrdered.Length && c < listViewLibrary.Columns.Count; c++)
                listViewLibrary.Columns[colIndicesOrdered[c].columnIndex].DisplayIndex = colIndicesOrdered[c].displayIndex;

            listViewLibrary.EndUpdate();
        }

        private void toolStripMenuItemResetLibraryColumnWidths_Click(object sender, EventArgs e)
        {
            ResetColumnWidthListViewLibrary(false);
        }

        private void resetColumnWidthNoMutationLevelColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetColumnWidthListViewLibrary(true);
        }

        private void restoreMutationLevelsASAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LibraryColumnsMutationsWidth(false);
        }

        private void collapseMutationsLevelsASEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LibraryColumnsMutationsWidth(true);
        }

        private void ResetColumnWidthListViewLibrary(bool mutationColumnWidthsZero)
        {
            listViewLibrary.BeginUpdate();
            var statWidths = Stats.UsuallyVisibleStats.Select(w => w ? 30 : 0).ToArray();
            for (int ci = 0; ci < listViewLibrary.Columns.Count; ci++)
                listViewLibrary.Columns[ci].Width = ci == ColumnIndexMutagenApplied ? 30
                    : ci < ColumnIndexFirstStat || ci >= ColumnIndexPostColor ? 60
                    : ci >= ColumnIndexFirstStat + Stats.StatsCount + Stats.StatsCount ? 30 // color
                    : ci < ColumnIndexFirstStat + Stats.StatsCount ? statWidths[ci - ColumnIndexFirstStat] // wild levels
                    : (int)(statWidths[ci - ColumnIndexFirstStat - Stats.StatsCount] * 1.24); // mutated needs space for one more letter

            if (mutationColumnWidthsZero)
                LibraryColumnsMutationsWidth(true);

            listViewLibrary.EndUpdate();
        }

        /// <summary>
        /// Set width of mutation level columns to zero or restore.
        /// </summary>
        private void LibraryColumnsMutationsWidth(bool collapse)
        {
            listViewLibrary.BeginUpdate();
            var statWidths = Stats.UsuallyVisibleStats.Select(w => !collapse && w ? 38 : 0).ToArray();
            for (int c = 0; c < Stats.StatsCount; c++)
            {
                listViewLibrary.Columns[c + ColumnIndexFirstStat + Stats.StatsCount].Width = statWidths[c];
            }
            listViewLibrary.EndUpdate();
        }

        #endregion
    }
}
