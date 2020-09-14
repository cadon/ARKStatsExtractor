using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.utils;

namespace ARKBreedingStats
{
    public partial class Form1
    {
        /// <summary>
        /// Creatures filtered according to the library-filter.
        /// Used so the live filter doesn't need to do the base filtering everytime.
        /// </summary>
        private Creature[] _creaturesPreFiltered;

        /// <summary>
        /// Add a new creature to the library based from the data of the extractor or tester
        /// </summary>
        /// <param name="fromExtractor">if true, take data from extractor-infoInput, else from tester</param>
        /// <param name="motherArkId">only pass if from import. Used for creating placeholder parents</param>
        /// <param name="fatherArkId">only pass if from import. Used for creating placeholder parents</param>
        /// <param name="goToLibraryTab">go to library tab after the creature is added</param>
        private Creature AddCreatureToCollection(bool fromExtractor = true, long motherArkId = 0, long fatherArkId = 0, bool goToLibraryTab = true)
        {
            CreatureInfoInput input;
            bool bred;
            double te, imprinting;
            Species species = speciesSelector1.SelectedSpecies;
            if (fromExtractor)
            {
                input = creatureInfoInputExtractor;
                bred = rbBredExtractor.Checked;
                te = _extractor.UniqueTE();
                imprinting = _extractor.ImprintingBonus;
            }
            else
            {
                input = creatureInfoInputTester;
                bred = rbBredTester.Checked;
                te = (double)NumericUpDownTestingTE.Value / 100;
                imprinting = (double)numericUpDownImprintingBonusTester.Value / 100;
            }

            var levelStep = _creatureCollection.getWildLevelStep();
            Creature creature = new Creature(species, input.CreatureName, input.CreatureOwner, input.CreatureTribe, input.CreatureSex, GetCurrentWildLevels(fromExtractor), GetCurrentDomLevels(fromExtractor), te, bred, imprinting, levelStep: levelStep)
            {
                // set parents
                Mother = input.Mother,
                Father = input.Father,

                // cooldown-, growing-time
                cooldownUntil = input.CooldownUntil,
                growingUntil = input.GrowingUntil,

                flags = input.CreatureFlags,
                note = input.CreatureNote,
                server = input.CreatureServer,

                domesticatedAt = input.DomesticatedAt.HasValue && input.DomesticatedAt.Value.Year > 2014 ? input.DomesticatedAt.Value : default(DateTime?),
                addedToLibrary = DateTime.Now,
                mutationsMaternal = input.MutationCounterMother,
                mutationsPaternal = input.MutationCounterFather,
                Status = input.CreatureStatus,
                colors = input.RegionColors
            };

            // Ids: ArkId and Guid
            creature.guid = fromExtractor && input.CreatureGuid != Guid.Empty ? input.CreatureGuid : Guid.NewGuid();

            creature.ArkId = input.ArkId;
            creature.ArkIdImported = Utils.IsArkIdImported(creature.ArkId, creature.guid);

            // parent guids
            if (motherArkId != 0)
                creature.motherGuid = Utils.ConvertArkIdToGuid(motherArkId);
            else if (input.MotherArkId != 0)
                creature.motherGuid = Utils.ConvertArkIdToGuid(input.MotherArkId);
            if (fatherArkId != 0)
                creature.fatherGuid = Utils.ConvertArkIdToGuid(fatherArkId);
            else if (input.FatherArkId != 0)
                creature.fatherGuid = Utils.ConvertArkIdToGuid(input.FatherArkId);

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

            _creatureCollection.MergeCreatureList(new List<Creature> { creature });

            // set status of exportedCreatureControl if available
            _exportedCreatureControl?.setStatus(importExported.ExportedCreatureControl.ImportStatus.JustImported, DateTime.Now);

            // if creature already exists by guid, use the already existing creature object for the parent assignments
            creature = _creatureCollection.creatures.SingleOrDefault(c => c.guid == creature.guid) ?? creature;

            // if new creature is parent of existing creatures, update link
            var motherOf = _creatureCollection.creatures.Where(c => c.motherGuid == creature.guid).ToList();
            foreach (Creature c in motherOf)
                c.Mother = creature;
            var fatherOf = _creatureCollection.creatures.Where(c => c.fatherGuid == creature.guid).ToList();
            foreach (Creature c in fatherOf)
                c.Father = creature;

            // if the new creature is the ancestor of any other creatures, update the generation count of all creatures
            if (motherOf.Any() || fatherOf.Any())
            {
                var creaturesOfSpecies = _creatureCollection.creatures.Where(c => c.Species == c.Species).ToList();
                foreach (var cr in creaturesOfSpecies) cr.generation = -1;
                foreach (var cr in creaturesOfSpecies) cr.RecalculateAncestorGenerations();
            }
            else
            {
                creature.RecalculateAncestorGenerations();
            }

            // link new creature to its parents if they're available, or creature placeholders
            if (creature.Mother == null || creature.Father == null)
                UpdateParents(new List<Creature> { creature });

            _filterListAllowed = false;
            UpdateCreatureListings(species, false);

            // show only the added creatures' species
            listBoxSpeciesLib.SelectedItem = creature.Species;
            _filterListAllowed = true;
            _libraryNeedsUpdate = true;

            if (goToLibraryTab)
            {
                tabControlMain.SelectedTab = tabPageLibrary;
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
                if (listViewLibrary.SelectedItems.Count > 0)
                {
                    if (MessageBox.Show("Do you really want to delete the entry and all data for " +
                            $"\"{((Creature)listViewLibrary.SelectedItems[0].Tag).name}\"" +
                            $"{(listViewLibrary.SelectedItems.Count > 1 ? " and " + (listViewLibrary.SelectedItems.Count - 1) + " other creatures" : string.Empty)}?",
                            "Delete Creature?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        bool onlyOneSpecies = true;
                        Species species = ((Creature)listViewLibrary.SelectedItems[0].Tag).Species;
                        foreach (ListViewItem i in listViewLibrary.SelectedItems)
                        {
                            if (onlyOneSpecies)
                            {
                                if (species != ((Creature)i.Tag).Species)
                                    onlyOneSpecies = false;
                            }
                            _creatureCollection.DeleteCreature((Creature)i.Tag);
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
        /// <param name="fromExtractor"></param>
        /// <returns></returns>
        private int[] GetCurrentWildLevels(bool fromExtractor = true)
        {
            int[] levelsWild = new int[Values.STATS_COUNT];
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                levelsWild[s] = fromExtractor ? _statIOs[s].LevelWild : _testingIOs[s].LevelWild;
            }
            return levelsWild;
        }

        /// <summary>
        /// Returns the domesticated levels from the extractor or tester in an array.
        /// </summary>
        /// <param name="fromExtractor"></param>
        /// <returns></returns>
        private int[] GetCurrentDomLevels(bool fromExtractor = true)
        {
            int[] levelsDom = new int[Values.STATS_COUNT];
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                levelsDom[s] = fromExtractor ? _statIOs[s].LevelDom : _testingIOs[s].LevelDom;
            }
            return levelsDom;
        }

        /// <summary>
        /// Call after the creatureCollection-object was created anew (e.g. after loading a file)
        /// </summary>
        /// <param name="keepCurrentSelection">True if synchronized library file is loaded.</param>
        private void InitializeCollection(bool keepCurrentSelection = false)
        {
            // set pointer to current collection
            pedigree1.SetCreatures(_creatureCollection.creatures);
            breedingPlan1.CreatureCollection = _creatureCollection;
            tribesControl1.Tribes = _creatureCollection.tribes;
            tribesControl1.Players = _creatureCollection.players;
            timerList1.CreatureCollection = _creatureCollection;
            notesControl1.NoteList = _creatureCollection.noteList;
            raisingControl1.creatureCollection = _creatureCollection;
            statsMultiplierTesting1.CreatureCollection = _creatureCollection;

            UpdateParents(_creatureCollection.creatures);
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
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = Values.V.speciesNames.Count;
            toolStripProgressBar1.Visible = true;

            var filteredCreatures = Properties.Settings.Default.useFiltersInTopStatCalculation ? ApplyLibraryFilterSettings(creatures).ToArray() : null;
            foreach (Species species in Values.V.species)
            {
                toolStripProgressBar1.Value++;
                List<int> usedStatIndices = new List<int>(Values.STATS_COUNT);
                List<int> usedAndConsideredStatIndices = new List<int>(Values.STATS_COUNT);
                int[] bestStat = new int[Values.STATS_COUNT];
                int[] lowestStat = new int[Values.STATS_COUNT];
                for (int s = 0; s < Values.STATS_COUNT; s++)
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
                List<Creature>[] bestCreatures = new List<Creature>[Values.STATS_COUNT];
                int usedStatsCount = usedStatIndices.Count;
                int usedAndConsideredStatsCount = usedAndConsideredStatIndices.Count;

                bool noCreaturesInThisSpecies = true;
                foreach (Creature c in creatures)
                {
                    if (c.Species != species
                        || c.flags.HasFlag(CreatureFlags.Placeholder))
                        continue;

                    noCreaturesInThisSpecies = false;
                    // reset topBreeding stats for this creature
                    c.topBreedingStats = new bool[Values.STATS_COUNT];
                    c.topBreedingCreature = false;

                    if (
                        //if not in the filtered collection (using library filter settings), continue
                        (filteredCreatures != null && !filteredCreatures.Contains(c))
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
                            bestCreatures[si] = new List<Creature> { c };
                            bestStat[si] = c.levelsWild[si];
                        }
                    }
                }
                if (noCreaturesInThisSpecies)
                {
                    continue;
                }

                if (!_topLevels.ContainsKey(species))
                {
                    _topLevels.Add(species, bestStat);
                }
                else
                {
                    _topLevels[species] = bestStat;
                }

                if (!_lowestLevels.ContainsKey(species))
                {
                    _lowestLevels.Add(species, lowestStat);
                }
                else
                {
                    _lowestLevels[species] = lowestStat;
                }

                // beststat and bestcreatures now contain the best stats and creatures for each stat.

                // set topness of each creature (== mean wildlevels/mean top wildlevels in permille)
                int sumTopLevels = 0;
                for (int s = 0; s < usedAndConsideredStatsCount; s++)
                {
                    int si = usedAndConsideredStatIndices[s];
                    if (bestStat[si] > 0)
                        sumTopLevels += bestStat[si];
                }
                if (sumTopLevels > 0)
                {
                    foreach (Creature c in creatures)
                    {
                        if (c.Species != species
                            || c.flags.HasFlag(CreatureFlags.Placeholder))
                            continue;
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
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (bestCreatures[s] == null || bestCreatures[s].Count == 0)
                    {
                        continue; // no creature has levelups in this stat or the stat is not used for this species
                    }

                    if (bestCreatures[s].Count == 1)
                    {
                        bestCreatures[s][0].topBreedingCreature = true;
                        continue;
                    }

                    for (int c = 0; c < bestCreatures[s].Count; c++)
                    {
                        bestCreatures[s][c].topBreedingCreature = true;
                        if (bestCreatures[s][c].sex != Sex.Male)
                            continue;

                        Creature currentCreature = bestCreatures[s][c];
                        // check how many best stat the male has
                        int maxval = 0;
                        for (int cs = 0; cs < Values.STATS_COUNT; cs++)
                        {
                            if (currentCreature.levelsWild[cs] == bestStat[cs])
                                maxval++;
                        }

                        if (maxval > 1)
                        {
                            // check now if the other males have only 1.
                            for (int oc = 0; oc < bestCreatures[s].Count; oc++)
                            {
                                if (bestCreatures[s][oc].sex != Sex.Male)
                                    continue;

                                if (oc == c)
                                    continue;

                                Creature otherMale = bestCreatures[s][oc];

                                int othermaxval = 0;
                                for (int ocs = 0; ocs < Values.STATS_COUNT; ocs++)
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
                if (noCreaturesInThisSpecies)
                {
                    continue;
                }

                // now we have a list of all candidates for breeding. Iterate on stats.
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (bestCreatures[s] != null)
                    {
                        for (int c = 0; c < bestCreatures[s].Count; c++)
                        {
                            // flag topstats in creatures
                            bestCreatures[s][c].topBreedingStats[s] = true;
                        }
                    }
                }
                foreach (Creature c in creatures)
                    c.SetTopStatCount(_considerStatHighlight);
            }
            toolStripProgressBar1.Visible = false;
        }

        /// <summary>
        /// Sets the parents according to the guids. Call after a file is loaded.
        /// </summary>
        /// <param name="creatures"></param>
        private void UpdateParents(IEnumerable<Creature> creatures)
        {
            List<Creature> placeholderAncestors = new List<Creature>();

            foreach (Creature c in creatures)
            {
                if (c.motherGuid != Guid.Empty || c.fatherGuid != Guid.Empty)
                {
                    Creature mother = null;
                    Creature father = null;
                    foreach (Creature p in _creatureCollection.creatures)
                    {
                        if (c.motherGuid != Guid.Empty && c.motherGuid == p.guid)
                        {
                            mother = p;
                            if (father != null || c.fatherGuid == Guid.Empty)
                                break;
                        }
                        else if (c.fatherGuid != Guid.Empty && c.fatherGuid == p.guid)
                        {
                            father = p;
                            if (mother != null || c.motherGuid == Guid.Empty)
                                break;
                        }
                    }

                    if (mother == null)
                        mother = EnsurePlaceholderCreature(placeholderAncestors, c, c.motherArkId, c.motherGuid, c.motherName, Sex.Female);
                    if (father == null)
                        father = EnsurePlaceholderCreature(placeholderAncestors, c, c.fatherArkId, c.fatherGuid, c.fatherName, Sex.Male);

                    c.Mother = mother;
                    c.Father = father;
                }
            }

            _creatureCollection.creatures.AddRange(placeholderAncestors);
        }

        /// <summary>
        /// Ensures the given placeholder ancestor exists in the list of placeholders.
        /// Does nothing when the details are not well specified.
        /// </summary>
        /// <param name="placeholders">List of placeholders to amend</param>
        /// <param name="tmpl">Descendant creature to use as a template</param>
        /// <param name="arkId">ArkId of creature to create. Only pass this if it's from an import</param>
        /// <param name="guid">GUID of creature to create</param>
        /// <param name="name">Name of the creature to create</param>
        /// <param name="sex">Sex of the creature to create</param>
        /// <returns></returns>
        private Creature EnsurePlaceholderCreature(List<Creature> placeholders, Creature tmpl, long arkId, Guid guid, string name, Sex sex)
        {
            if (guid == Guid.Empty && arkId == 0)
                return null;
            var existing = placeholders.SingleOrDefault(ph => ph.guid == guid);
            if (existing != null)
                return existing;

            if (string.IsNullOrEmpty(name))
                name = (sex == Sex.Female ? "Mother" : "Father") + " of " + tmpl.name;

            Guid creatureGuid = arkId != 0 ? Utils.ConvertArkIdToGuid(arkId) : guid;
            var creature = new Creature(tmpl.Species, name, tmpl.owner, tmpl.tribe, sex, new[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
                    levelStep: _creatureCollection.getWildLevelStep())
            {
                guid = creatureGuid,
                Status = CreatureStatus.Unavailable,
                flags = CreatureFlags.Placeholder,
                ArkId = arkId,
                ArkIdImported = Utils.IsArkIdImported(arkId, creatureGuid)
            };

            placeholders.Add(creature);

            return creature;
        }

        /// <summary>
        /// Sets the parents of the incubation-timers according to the guids. Call after a file is loaded.
        /// </summary>
        /// <param name="cc"></param>
        private void UpdateIncubationParents(CreatureCollection cc)
        {
            foreach (Creature c in cc.creatures)
            {
                if (c.guid != Guid.Empty)
                {
                    foreach (IncubationTimerEntry it in cc.incubationListEntries)
                    {
                        if (c.guid == it.motherGuid)
                            it.mother = c;
                        else if (c.guid == it.fatherGuid)
                            it.father = c;
                    }
                }
            }
        }

        private void ShowCreaturesInListView(IEnumerable<Creature> creatures)
        {
            listViewLibrary.BeginUpdate();

            // clear ListView
            listViewLibrary.Items.Clear();
            listViewLibrary.Groups.Clear();

            // add groups for each species (so they are sorted alphabetically)
            foreach (Species s in Values.V.species)
            {
                listViewLibrary.Groups.Add(new ListViewGroup(s.name));
            }

            List<ListViewItem> items = new List<ListViewItem>();
            foreach (Creature cr in creatures)
            {
                // if species is unknown, don't display the creature
                if (cr.Species == null)
                    continue;

                // check if group of species exists
                ListViewGroup g = null;
                foreach (ListViewGroup lvg in listViewLibrary.Groups)
                {
                    if (lvg.Header == cr.Species.DescriptiveNameAndMod)
                    {
                        g = lvg;
                        break;
                    }
                }
                if (g == null)
                {
                    g = new ListViewGroup(cr.Species.DescriptiveNameAndMod);
                    listViewLibrary.Groups.Add(g);
                }
                items.Add(CreateCreatureLVItem(cr, g));
            }
            listViewLibrary.Items.AddRange(items.ToArray());
            listViewLibrary.EndUpdate();

            // highlight filter input if something is entered and no results are available
            ToolStripTextBoxLibraryFilter.BackColor = string.IsNullOrEmpty(ToolStripTextBoxLibraryFilter.Text) || items.Any()
                ? SystemColors.Window : Color.LightSalmon;
        }

        /// <summary>
        /// Call this function to update the displayed values of a creature. Usually called after a creature was edited.
        /// </summary>
        /// <param name="cr">Creature that was changed</param>
        /// <param name="creatureStatusChanged"></param>
        private void UpdateDisplayedCreatureValues(Creature cr, bool creatureStatusChanged, bool ownerServerChanged)
        {
            // if row is selected, save and reselect later
            List<Creature> selectedCreatures = new List<Creature>();
            foreach (ListViewItem i in listViewLibrary.SelectedItems)
                selectedCreatures.Add((Creature)i.Tag);

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
                // int listViewLibrary replace old row with new one
                int ci = -1;
                for (int i = 0; i < listViewLibrary.Items.Count; i++)
                {
                    if ((Creature)listViewLibrary.Items[i].Tag == cr)
                    {
                        ci = i;
                        break;
                    }
                }
                if (ci >= 0)
                    listViewLibrary.Items[ci] = CreateCreatureLVItem(cr, listViewLibrary.Items[ci].Group);
            }

            // recreate ownerlist
            if (ownerServerChanged)
                UpdateOwnerServerTagLists();
            SetCollectionChanged(true, cr.Species);

            // select previous selecteded again
            int selectedCount = selectedCreatures.Count;
            if (selectedCount > 0)
            {
                for (int i = 0; i < listViewLibrary.Items.Count; i++)
                {
                    if (selectedCreatures.Contains((Creature)listViewLibrary.Items[i].Tag))
                    {
                        listViewLibrary.Items[i].Focused = true;
                        listViewLibrary.Items[i].Selected = true;
                        if (--selectedCount == 0)
                        {
                            listViewLibrary.EnsureVisible(i);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the dateTime when the countdown of a creature is ready. Either the maturingTime, the matingCooldownTime or null if no countdown is set.
        /// </summary>
        /// <returns></returns>
        private DateTime? DisplayedCreatureCountdown(DateTime? matingCooldownUntil, DateTime? growingUntil)
        {
            var countdown = matingCooldownUntil.HasValue && growingUntil.HasValue
                    ? (matingCooldownUntil.Value > growingUntil.Value ? matingCooldownUntil.Value : growingUntil.Value)
                    : matingCooldownUntil ?? growingUntil;
            if (countdown == null) return null;

            return DateTime.Now.CompareTo(countdown) < 0 ? countdown : null;
        }

        private ListViewItem CreateCreatureLVItem(Creature cr, ListViewGroup g)
        {
            double colorFactor = 100d / _creatureCollection.maxChartLevel;
            DateTime? cldGr = cr.cooldownUntil.HasValue && cr.growingUntil.HasValue ?
                (cr.cooldownUntil.Value > cr.growingUntil.Value ? cr.cooldownUntil.Value : cr.growingUntil.Value)
                : cr.cooldownUntil ?? (cr.growingUntil);

            string[] subItems = new[]
                    {
                            cr.name,
                            cr.owner,
                            cr.note,
                            cr.server,
                            Utils.SexSymbol(cr.sex),
                            cr.domesticatedAt?.ToString("yyyy'-'MM'-'dd HH':'mm':'ss") ?? "?",
                            (cr.topness / 10).ToString(),
                            cr.topStatsCount.ToString(),
                            cr.generation.ToString(),
                            cr.levelFound.ToString(),
                            cr.Mutations.ToString(),
                            DisplayedCreatureCountdown(cr.cooldownUntil,cr.growingUntil)?.ToString() ?? "-"
                    }
                    .Concat(cr.levelsWild.Select(x => x.ToString()).ToArray())
                    .ToArray();

            if (Properties.Settings.Default.showColorsInLibrary)
                subItems = subItems.Concat(cr.colors.Select(cl => cl.ToString()).ToArray()).ToArray();
            else
                subItems = subItems.Concat(new string[6]).ToArray();

            // add the species and status and tribe
            subItems = subItems.Concat(new[] {
                cr.Species.DescriptiveNameAndMod,
                cr.Status.ToString(),
                cr.tribe,
                Utils.StatusSymbol(cr.Status, string.Empty)
            }).ToArray();

            // check if we display group for species or not.
            ListViewItem lvi = Properties.Settings.Default.LibraryGroupBySpecies ? new ListViewItem(subItems, g) : new ListViewItem(subItems);

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (cr.valuesDom[s] == 0)
                {
                    // not used
                    lvi.SubItems[s + 12].ForeColor = Color.White;
                    lvi.SubItems[s + 12].BackColor = Color.White;
                }
                else if (cr.levelsWild[s] < 0)
                {
                    // unknown level 
                    lvi.SubItems[s + 12].ForeColor = Color.WhiteSmoke;
                    lvi.SubItems[s + 12].BackColor = Color.White;
                }
                else
                    lvi.SubItems[s + 12].BackColor = Utils.GetColorFromPercent((int)(cr.levelsWild[s] * (s == (int)StatNames.Torpidity ? colorFactor / 7 : colorFactor)), // TODO set factor to number of other stats (flyers have 6, Gacha has 8?)
                            _considerStatHighlight[s] ? cr.topBreedingStats[s] ? 0.2 : 0.7 : 0.93);
            }
            lvi.SubItems[4].BackColor = cr.flags.HasFlag(CreatureFlags.Neutered) ? Color.FromArgb(220, 220, 220) :
                    cr.sex == Sex.Female ? Color.FromArgb(255, 230, 255) :
                    cr.sex == Sex.Male ? Color.FromArgb(220, 235, 255) : SystemColors.Window;

            if (cr.Status == CreatureStatus.Dead)
            {
                lvi.SubItems[0].ForeColor = SystemColors.GrayText;
                lvi.BackColor = Color.FromArgb(255, 250, 240);
            }
            else if (cr.Status == CreatureStatus.Unavailable)
            {
                lvi.SubItems[0].ForeColor = SystemColors.GrayText;
            }
            else if (cr.Status == CreatureStatus.Obelisk)
            {
                lvi.SubItems[0].ForeColor = Color.DarkBlue;
            }
            else if (_creatureCollection.maxServerLevel > 0
                    && cr.levelsWild[(int)StatNames.Torpidity] + 1 + _creatureCollection.maxDomLevel > _creatureCollection.maxServerLevel)
            {
                lvi.SubItems[0].ForeColor = Color.OrangeRed; // this creature may pass the max server level and could be deleted by the game
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
                lvi.SubItems[7].BackColor = Utils.GetColorFromPercent(cr.topStatsCount * 8 + 44, 0.7);
            }
            else
            {
                lvi.SubItems[7].ForeColor = Color.LightGray;
            }

            // color for timestamp added
            if (cr.domesticatedAt == null || cr.domesticatedAt.Value.Year < 2015)
            {
                lvi.SubItems[5].Text = "n/a";
                lvi.SubItems[5].ForeColor = Color.LightGray;
            }

            // color for topness
            lvi.SubItems[6].BackColor = Utils.GetColorFromPercent(cr.topness / 5 - 100, 0.8); // topness is in permille. gradient from 50-100

            // color for generation
            if (cr.generation == 0)
                lvi.SubItems[8].ForeColor = Color.LightGray;

            // color of WildLevelColumn
            if (cr.levelFound == 0)
                lvi.SubItems[9].ForeColor = Color.LightGray;

            // color for mutation
            if (cr.Mutations > 0)
            {
                if (cr.Mutations > 19)
                    lvi.SubItems[10].BackColor = Utils.MutationColorOverLimit;
                else
                    lvi.SubItems[10].BackColor = Utils.MutationColor;
            }
            else
                lvi.SubItems[10].ForeColor = Color.LightGray;

            // color for cooldown
            CooldownColors(cr, out Color forecolor, out Color backcolor);
            lvi.SubItems[11].ForeColor = forecolor;
            lvi.SubItems[11].BackColor = backcolor;

            if (Properties.Settings.Default.showColorsInLibrary)
            {
                // color for colors
                for (int cl = 0; cl < 6; cl++)
                {
                    if (cr.colors[cl] != 0)
                    {
                        lvi.SubItems[24 + cl].BackColor = CreatureColors.CreatureColor(cr.colors[cl]);
                        lvi.SubItems[24 + cl].ForeColor = Utils.ForeColor(lvi.SubItems[24 + cl].BackColor);
                    }
                    else
                    {
                        lvi.SubItems[24 + cl].ForeColor = cr.Species.EnabledColorRegions[cl] ? Color.LightGray : Color.White;
                    }
                }
            }

            lvi.Tag = cr;
            return lvi;
        }

        /// <summary>
        /// Sets the cooldown colors depending if the cooldown is maturing or post-mating.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        private void CooldownColors(Creature c, out Color forecolor, out Color backcolor)
        {
            DateTime? cldGr = c.cooldownUntil.HasValue && c.growingUntil.HasValue ?
                (c.cooldownUntil.Value > c.growingUntil.Value ? c.cooldownUntil.Value : c.growingUntil.Value)
                : c.cooldownUntil ?? (c.growingUntil ?? default(DateTime?));

            forecolor = SystemColors.ControlText;
            backcolor = SystemColors.Window;

            double minCld = cldGr?.Subtract(DateTime.Now).TotalMinutes ?? 0;
            if (minCld <= 0)
            {
                forecolor = Color.LightGray;
                return;
            }

            if ((c.cooldownUntil.HasValue && c.growingUntil.HasValue && c.cooldownUntil > c.growingUntil)
                || !c.growingUntil.HasValue)
            {
                // mating-cooldown
                if (minCld < 1)
                    backcolor = Color.FromArgb(235, 255, 109); // green-yellow
                else if (minCld < 10)
                    backcolor = Color.FromArgb(255, 250, 109); // yellow
                else
                    backcolor = Color.FromArgb(255, 179, 109); // yellow-orange
            }
            else
            {
                // growing
                if (minCld < 1)
                    backcolor = Color.FromArgb(168, 187, 255); // light blue
                else if (minCld < 10)
                    backcolor = Color.FromArgb(197, 168, 255); // light blue/pink
                else
                    backcolor = Color.FromArgb(236, 168, 255); // light pink
            }
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter.DoSort((ListView)sender, e.Column);
        }

        private Debouncer libraryIndexChangedDebouncer = new Debouncer();

        // onlibrarychange
        private void listViewLibrary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_reactOnCreatureSelectionChange)
                libraryIndexChangedDebouncer.Debounce(100, LibrarySelectedIndexChanged, Dispatcher.CurrentDispatcher);
        }

        /// <summary>
        /// Updates infos about the selected creatures like tags, levels and stat-level distribution.
        /// </summary>
        private void LibrarySelectedIndexChanged()
        {
            int cnt = listViewLibrary.SelectedItems.Count;
            if (cnt == 0)
            {
                SetMessageLabelText();
                creatureBoxListView.Clear();
                return;
            }

            if (cnt == 1)
            {
                Creature c = (Creature)listViewLibrary.SelectedItems[0].Tag;
                creatureBoxListView.SetCreature(c);
                if (tabControlLibFilter.SelectedTab == tabPageLibRadarChart)
                    radarChartLibrary.SetLevels(c.levelsWild);
                pedigree1.PedigreeNeedsUpdate = true;
            }

            // display infos about the selected creatures
            List<Creature> selCrs = new List<Creature>();
            for (int i = 0; i < cnt; i++)
                selCrs.Add((Creature)listViewLibrary.SelectedItems[i].Tag);

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
                    $"{selCrs.Count(cr => cr.sex == Sex.Male)} males\n" +
                    (cnt == 1
                        ? $"level: {selCrs[0].Level}" + (selCrs[0].ArkIdImported ? $"; Ark-Id (ingame): {Utils.ConvertImportedArkIdToIngameVisualization(selCrs[0].ArkId)}" : string.Empty)
                        : $"level-range: {selCrs.Min(cr => cr.Level)} - {selCrs.Max(cr => cr.Level)}"
                    ) + "\n" +
                    $"Tags: {string.Join(", ", tagList)}");
        }

        /// <summary>
        /// Display the creatures with the current filter.
        /// Recalculate all filters.
        /// </summary>
        private void FilterLibRecalculate()
        {
            _creaturesPreFiltered = null;
            FilterLib();
        }

        /// <summary>
        /// Display the creatures with the current filter.
        /// Use the pre filtered list (if available) and only apply the live filter.
        /// </summary>
        private void FilterLib()
        {
            if (!_filterListAllowed)
                return;

            // save selected creatures to re-select them after the filtering
            List<Creature> selectedCreatures = new List<Creature>();
            foreach (ListViewItem i in listViewLibrary.SelectedItems)
                selectedCreatures.Add((Creature)i.Tag);

            IEnumerable<Creature> filteredList;

            if (_creaturesPreFiltered == null)
            {
                filteredList = from creature in _creatureCollection.creatures
                               where !creature.flags.HasFlag(CreatureFlags.Placeholder)
                               select creature;

                // if only one species should be shown adjust headers if the selected species has custom statNames
                Dictionary<string, string> customStatNames = null;
                if (listBoxSpeciesLib.SelectedItem is Species selectedSpecies)
                {
                    filteredList = filteredList.Where(c => c.Species == selectedSpecies);
                    customStatNames = selectedSpecies.statNames;
                }

                for (int s = 0; s < Values.STATS_COUNT; s++)
                    listViewLibrary.Columns[12 + s].Text = Utils.StatName(s, true, customStatNames);

                _creaturesPreFiltered = ApplyLibraryFilterSettings(filteredList).ToArray();
            }

            filteredList = _creaturesPreFiltered;
            // apply live filter
            var filterString = ToolStripTextBoxLibraryFilter.Text.Trim();
            if (!string.IsNullOrEmpty(filterString))
            {
                // filter parameter are separated by commas and all parameter must be found on an item to have it included
                var filterStrings = filterString.Split(',').Select(f => f.Trim())
                    .Where(f => !string.IsNullOrEmpty(f)).ToArray();

                filteredList = filteredList.Where(c => filterStrings.All(f =>
                    c.name.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) != -1
                    || (c.Species?.name.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) ?? -1) != -1
                    || (c.owner?.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) ?? -1) != -1
                    || (c.tribe?.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) ?? -1) != -1
                    || (c.note?.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) ?? -1) != -1
                    || (c.server?.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) ?? -1) != -1
                    || (c.tags?.Any(t => string.Equals(t, f, StringComparison.InvariantCultureIgnoreCase)) ?? false)
                ));
            }

            // display new results
            ShowCreaturesInListView(filteredList);

            // update creatureBox
            creatureBoxListView.UpdateLabel();

            // select previous selected creatures again
            int selectedCount = selectedCreatures.Count;
            if (selectedCount > 0)
            {
                for (int i = 0; i < listViewLibrary.Items.Count; i++)
                {
                    if (selectedCreatures.Contains((Creature)listViewLibrary.Items[i].Tag))
                    {
                        listViewLibrary.Items[i].Selected = true;
                        if (--selectedCount == 0)
                        {
                            listViewLibrary.Items[i].Focused = true;
                            listViewLibrary.EnsureVisible(i);
                            break;
                        }
                    }
                }
            }
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

        private void listViewLibrary_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedCreatures();
            }
            else if (e.KeyCode == Keys.F2)
            {
                if (listViewLibrary.SelectedIndices.Count > 0)
                    EditCreatureInTester((Creature)listViewLibrary.Items[listViewLibrary.SelectedIndices[0]].Tag);
            }
            else if (e.KeyCode == Keys.F3)
            {
                if (listViewLibrary.SelectedIndices.Count > 0)
                    ShowMultiSetter();
            }
            else if (e.KeyCode == Keys.F5)
            {
                if (listViewLibrary.SelectedIndices.Count > 0)
                    AdminCommandToSetColors();
            }
            else if (e.KeyCode == Keys.A && e.Control)
            {
                // select all list-entries
                _reactOnCreatureSelectionChange = false;
                listViewLibrary.BeginUpdate();
                foreach (ListViewItem i in listViewLibrary.Items)
                    i.Selected = true;
                listViewLibrary.EndUpdate();
                _reactOnCreatureSelectionChange = true;
                listViewLibrary_SelectedIndexChanged(null, null);
            }
            else if (e.KeyCode == Keys.G && e.Control)
            {
                GenerateCreatureNames();
            }
            else if (e.KeyCode == Keys.B && e.Control)
            {
                CopySelectedCreatureName();
            }
        }

        /// <summary>
        /// Copies the data of the selected creatures to the clipboard for use in a spreadsheet.
        /// </summary>
        private void ExportForSpreadsheet()
        {
            if (tabControlMain.SelectedTab == tabPageLibrary)
            {
                if (listViewLibrary.SelectedItems.Count > 0)
                {
                    // header
                    var output = new StringBuilder("Species\tName\tSex\tOwner\t");

                    var suffixe = new List<string> { "w", "d", "b", "v" }; // wild, dom, bred-values, dom-values
                    foreach (var suffix in suffixe)
                    {
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            output.Append(Utils.StatName(Values.statsDisplayOrder[s], true) + suffix + "\t");
                        }
                    }
                    output.Append("mother\tfather\tMut\tNotes\tColor0\tColor1\tColor2\tColor3\tColor4\tColor5");

                    foreach (ListViewItem l in listViewLibrary.SelectedItems)
                    {
                        Creature c = (Creature)l.Tag;
                        output.Append("\n" + c.Species.name + "\t" + c.name + "\t" + c.sex + "\t" + c.owner);
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            output.Append("\t" + c.levelsWild[Values.statsDisplayOrder[s]]);
                        }
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            output.Append("\t" + c.levelsDom[Values.statsDisplayOrder[s]]);
                        }
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            output.Append($"\t{c.valuesBreeding[Values.statsDisplayOrder[s]] * (Utils.Precision(Values.statsDisplayOrder[s]) == 3 ? 100 : 1)}{(Utils.Precision(Values.statsDisplayOrder[s]) == 3 ? "%" : string.Empty)}");
                        }
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            output.Append($"\t{c.valuesDom[Values.statsDisplayOrder[s]] * (Utils.Precision(Values.statsDisplayOrder[s]) == 3 ? 100 : 1)}{(Utils.Precision(Values.statsDisplayOrder[s]) == 3 ? "%" : string.Empty)}");
                        }
                        output.Append($"\t{(c.Mother?.name ?? string.Empty)}\t{(c.Father?.name ?? string.Empty)}\t{c.Mutations}\t{(string.IsNullOrEmpty(c.note) ? string.Empty : c.note.Replace("\r", string.Empty).Replace("\n", " "))}");
                        for (int cl = 0; cl < 6; cl++)
                        {
                            output.Append("\t" + c.colors[cl]);
                        }
                    }
                    Clipboard.SetText(output.ToString());
                }
                else
                    MessageBox.Show("No creatures in the library selected to copy to the clipboard", "No Creatures Selected",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (tabControlMain.SelectedTab == tabPageExtractor)
                CopyExtractionToClipboard();
        }

        /// <summary>
        /// Export the data of a creature to the clipboard in plain text.
        /// </summary>
        /// <param name="c">Creature to export</param>
        /// <param name="breeding">Stat values that are inherited</param>
        /// <param name="ARKml">True if ARKml markup for coloring should be used. That feature was disabled in the ARK-chat.</param>
        private void ExportAsTextToClipboard(Creature c, bool breeding = true, bool ARKml = false)
        {
            if (c != null)
            {
                double colorFactor = 100d / _creatureCollection.maxChartLevel;
                bool wild = c.tamingEff == -2;
                string modifierText = string.Empty;
                if (!breeding)
                {
                    if (wild)
                        modifierText = ", wild";
                    else if (c.tamingEff < 1)
                        modifierText = ", TE: " + Math.Round(100 * c.tamingEff, 1) + "%";
                    else if (c.imprintingBonus > 0)
                        modifierText = ", Impr: " + Math.Round(100 * c.imprintingBonus, 2) + "%";
                }

                var output = new StringBuilder((string.IsNullOrEmpty(c.name) ? "noName" : c.name) + " (" + (ARKml ? Utils.GetARKml(c.Species.name, 50, 172, 255) : c.Species.name)
                        + ", Lvl " + (breeding ? c.LevelHatched : c.Level) + modifierText + (c.sex != Sex.Unknown ? ", " + c.sex : string.Empty) + "): ");
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    int si = Values.statsDisplayOrder[s];
                    if (c.levelsWild[si] >= 0 && c.valuesBreeding[si] > 0) // ignore unknown levels (e.g. oxygen, speed for some species)
                        output.Append(Utils.StatName(si, true) + ": " + (breeding ? c.valuesBreeding[si] : c.valuesDom[si]) * (Utils.Precision(si) == 3 ? 100 : 1) + (Utils.Precision(si) == 3 ? "%" : string.Empty) +
                                " (" + (ARKml ? Utils.GetARKmlFromPercent(c.levelsWild[si].ToString(), (int)(c.levelsWild[si] * (si == (int)StatNames.Torpidity ? colorFactor / 7 : colorFactor))) : c.levelsWild[si].ToString()) +
                                (ARKml ? breeding || si == (int)StatNames.Torpidity ? string.Empty : ", " + Utils.GetARKmlFromPercent(c.levelsDom[si].ToString(), (int)(c.levelsDom[si] * colorFactor)) : breeding || si == (int)StatNames.Torpidity ? string.Empty : ", " + c.levelsDom[si]) + "); ");
                }
                var outputString = output.ToString();
                Clipboard.SetText(outputString.Substring(0, outputString.Length - 1));
            }
        }

        /// <summary>
        /// Display a window to edit multiple creatures at once. Also used to set tags.
        /// </summary>
        private void ShowMultiSetter()
        {
            // shows a dialog to set multiple settings to all selected creatures
            if (listViewLibrary.SelectedIndices.Count <= 0)
                return;
            Creature c = new Creature();
            List<Creature> selectedCreatures = new List<Creature>();

            // check if multiple species are selected
            bool multipleSpecies = false;
            Species sp = ((Creature)listViewLibrary.SelectedItems[0].Tag).Species;
            c.Species = sp;
            foreach (ListViewItem i in listViewLibrary.SelectedItems)
            {
                selectedCreatures.Add((Creature)i.Tag);
                if (!multipleSpecies && ((Creature)i.Tag).speciesBlueprint != sp.blueprintPath)
                {
                    multipleSpecies = true;
                }
            }
            List<Creature>[] parents = null;
            if (!multipleSpecies)
                parents = FindPossibleParents(c);

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
                        UpdateSpeciesLists(_creatureCollection.creatures);
                    UpdateOwnerServerTagLists();
                    SetCollectionChanged(true, !multipleSpecies ? sp : null);
                    RecalculateTopStatsIfNeeded();
                    FilterLibRecalculate();
                }
            }
        }

        private Debouncer filterLibraryDebouncer = new Debouncer();

        private void ToolStripTextBoxLibraryFilter_TextChanged(object sender, EventArgs e)
        {
            filterLibraryDebouncer.Debounce(ToolStripTextBoxLibraryFilter.Text == string.Empty ? 0 : 300, FilterLib, Dispatcher.CurrentDispatcher);
        }

        private void ToolStripButtonLibraryFilterClear_Click(object sender, EventArgs e)
        {
            ToolStripTextBoxLibraryFilter.Clear();
            ToolStripTextBoxLibraryFilter.Focus();
        }
    }
}
