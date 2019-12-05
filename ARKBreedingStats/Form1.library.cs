using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.values;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class Form1
    {
        /// <summary>
        /// Add a new creature to the library based from the data of the extractor or tester
        /// </summary>
        /// <param name="fromExtractor">if true, take data from extractor-infoinput, else from tester</param>
        /// <param name="motherArkId">only pass if from import. Used for creating placeholder parents</param>
        /// <param name="fatherArkId">only pass if from import. Used for creating placeholder parents</param>
        /// <param name="goToLibraryTab">go to library tab after the creature is added</param>
        private void AddCreatureToCollection(bool fromExtractor = true, long motherArkId = 0, long fatherArkId = 0, bool goToLibraryTab = true)
        {
            CreatureInfoInput input;
            bool bred;
            double te, imprinting;
            Species species = speciesSelector1.SelectedSpecies;
            if (fromExtractor)
            {
                input = creatureInfoInputExtractor;
                bred = rbBredExtractor.Checked;
                te = extractor.UniqueTE();
                imprinting = extractor.ImprintingBonus;
            }
            else
            {
                input = creatureInfoInputTester;
                bred = rbBredTester.Checked;
                te = (double)NumericUpDownTestingTE.Value / 100;
                imprinting = (double)numericUpDownImprintingBonusTester.Value / 100;
            }

            var levelStep = creatureCollection.getWildLevelStep();
            Creature creature = new Creature(species, input.CreatureName, input.CreatureOwner, input.CreatureTribe, input.CreatureSex, GetCurrentWildLevels(fromExtractor), GetCurrentDomLevels(fromExtractor), te, bred, imprinting, levelStep: levelStep)
            {
                // set parents
                Mother = input.mother,
                Father = input.father,

                // cooldown-, growing-time
                cooldownUntil = input.Cooldown,
                growingUntil = input.Grown,

                flags = input.creatureFlags,
                note = input.CreatureNote,
                server = input.CreatureServer,

                domesticatedAt = input.domesticatedAt,
                addedToLibrary = DateTime.Now,
                mutationsMaternal = input.MutationCounterMother,
                mutationsPaternal = input.MutationCounterFather,
                status = input.CreatureStatus,
                colors = input.RegionColors
            };

            // Ids: ArkId and Guid
            creature.guid = input.CreatureGuid != Guid.Empty ? input.CreatureGuid : Guid.NewGuid();

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
            creature.RecalculateAncestorGenerations();
            creature.RecalculateNewMutations();

            if (creatureCollection.DeletedCreatureGuids != null
                && creatureCollection.DeletedCreatureGuids.Contains(creature.guid))
                creatureCollection.DeletedCreatureGuids.RemoveAll(guid => guid == creature.guid);

            creatureCollection.MergeCreatureList(new List<Creature> { creature }, update: true);

            // if new creature is parent of existing creatures, update link
            var motherOf = creatureCollection.creatures.Where(c => c.motherGuid == creature.guid).ToList();
            foreach (Creature c in motherOf)
                c.Mother = creature;
            var fatherOf = creatureCollection.creatures.Where(c => c.fatherGuid == creature.guid).ToList();
            foreach (Creature c in fatherOf)
                c.Father = creature;

            // if the new creature is the ancestor of any other creatures, update the generation count of all creatures
            if (motherOf.Any() || fatherOf.Any())
            {
                var creaturesOfSpecies = creatureCollection.creatures.Where(c => c.Species == c.Species).ToList();
                foreach (var cr in creaturesOfSpecies) cr.generation = -1;
                foreach (var cr in creaturesOfSpecies) cr.RecalculateAncestorGenerations();
            }

            // link new creature to its parents if they're available, or creature placeholders
            if (creature.Mother == null || creature.Father == null)
                UpdateParents(new List<Creature> { creature });

            UpdateCreatureListings(species);
            // show only the added creatures' species
            if (goToLibraryTab)
            {
                listBoxSpeciesLib.SelectedIndex = listBoxSpeciesLib.Items.IndexOf(creature.Species);
                tabControlMain.SelectedTab = tabPageLibrary;
            }

            creatureInfoInputExtractor.parentListValid = false;
            creatureInfoInputTester.parentListValid = false;

            // set status of exportedCreatureControl if available
            exportedCreatureControl?.setStatus(importExported.ExportedCreatureControl.ImportStatus.JustImported, DateTime.Now);

            SetCollectionChanged(true, species);
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
                            $"{(listViewLibrary.SelectedItems.Count > 1 ? " and " + (listViewLibrary.SelectedItems.Count - 1) + " other creatures" : "")}?",
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
                            creatureCollection.DeleteCreature((Creature)i.Tag);
                        }
                        creatureCollection.RemoveUnlinkedPlaceholders();
                        UpdateCreatureListings(onlyOneSpecies ? species : null);
                        SetCollectionChanged(true, onlyOneSpecies ? species : null);
                    }
                }
            }
            else if (tabControlMain.SelectedTab == tabPagePlayerTribes)
            {
                tribesControl1.removeSelected();
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

            if (creature.ArkId != 0 && creatureCollection.ArkIdAlreadyExist(creature.ArkId, creature, out Creature guidCreature))
            {
                // if the creature is a placeholder replace the placeholder with the real creature
                if (guidCreature.flags.HasFlag(CreatureFlags.Placeholder) && creature.sex == guidCreature.sex && creature.Species == guidCreature.Species)
                {
                    // remove placeholder-creature from collection (is replaced by new creature)
                    creatureCollection.creatures.Remove(guidCreature);
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
                levelsWild[s] = fromExtractor ? statIOs[s].LevelWild : testingIOs[s].LevelWild;
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
                levelsDom[s] = fromExtractor ? statIOs[s].LevelDom : testingIOs[s].LevelDom;
            }
            return levelsDom;
        }

        /// <summary>
        /// Call after the creatureCollection-object was created anew (e.g. after loading a file)
        /// </summary>
        private void InitializeCollection()
        {
            // set pointer to current collection
            pedigree1.creatures = creatureCollection.creatures;
            breedingPlan1.creatureCollection = creatureCollection;
            tribesControl1.Tribes = creatureCollection.tribes;
            tribesControl1.Players = creatureCollection.players;
            timerList1.CreatureCollection = creatureCollection;
            notesControl1.NoteList = creatureCollection.noteList;
            raisingControl1.creatureCollection = creatureCollection;
            statsMultiplierTesting1.CreatureCollection = creatureCollection;

            UpdateParents(creatureCollection.creatures);
            UpdateIncubationParents(creatureCollection);

            CreateCreatureTagList();

            if (creatureCollection.modIDs == null) creatureCollection.modIDs = new List<string>();

            pedigree1.Clear();
            breedingPlan1.Clear();

            // assign species objects to creatures
            foreach (var cr in creatureCollection.creatures)
            {
                cr.Species = Values.V.SpeciesByBlueprint(cr.speciesBlueprint);
            }
            foreach (var cv in creatureCollection.creaturesValues)
            {
                cv.Species = Values.V.SpeciesByBlueprint(cv.speciesBlueprint);
            }

            UpdateTempCreatureDropDown();
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

            List<Creature> filteredCreatures = (creatureCollection.useFiltersInTopStatCalculation ? ApplyLibraryFilterSettings(creatures) : Enumerable.Empty<Creature>()).ToList();
            foreach (Species species in Values.V.species)
            {
                toolStripProgressBar1.Value++;
                List<int> usedStatIndices = new List<int>(Values.STATS_COUNT);
                List<int> usedAndConsideredStatIndices = new List<int>(Values.STATS_COUNT);
                int[] bestStat = new int[Values.STATS_COUNT];
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    bestStat[s] = -1;
                    if (species.UsesStat(s))
                    {
                        usedStatIndices.Add(s);
                        if (considerStatHighlight[s])
                            usedAndConsideredStatIndices.Add(s);
                    }
                }
                List<Creature>[] bestCreatures = new List<Creature>[Values.STATS_COUNT];
                int usedStatsCount = usedStatIndices.Count;
                int usedAndConsideredStatsCount = usedAndConsideredStatIndices.Count;

                bool noCreaturesInThisSpecies = true;
                foreach (Creature c in creatures)
                {
                    if (c.Species != species)
                        continue;

                    noCreaturesInThisSpecies = false;
                    // reset topBreeding stats for this creature
                    c.topBreedingStats = new bool[Values.STATS_COUNT];
                    c.topBreedingCreature = false;

                    if (creatureCollection.useFiltersInTopStatCalculation)
                    {
                        //if not in the filtered collection (using library filter settings), continue
                        if (!filteredCreatures.Contains(c))
                            continue;
                    }
                    else
                    {
                        // only consider creature if it's available for breeding
                        if (!(c.status == CreatureStatus.Available
                            || c.status == CreatureStatus.Cryopod
                            || c.status == CreatureStatus.Obelisk
                            ))
                            continue;
                    }

                    for (int s = 0; s < usedStatsCount; s++)
                    {
                        int si = usedStatIndices[s];
                        if (c.levelsWild[si] <= 0)
                            continue;
                        if (c.levelsWild[si] == bestStat[si])
                        {
                            bestCreatures[si].Add(c);
                        }
                        else if (c.levelsWild[si] > bestStat[si])
                        {
                            bestCreatures[si] = new List<Creature>
                                    { c };
                            bestStat[si] = c.levelsWild[si];
                        }
                    }
                }
                if (noCreaturesInThisSpecies)
                {
                    continue;
                }

                if (!topLevels.ContainsKey(species))
                {
                    topLevels.Add(species, bestStat);
                }
                else
                {
                    topLevels[species] = bestStat;
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
                        if (c.Species != species)
                            continue;
                        int sumCreatureLevels = 0;
                        for (int s = 0; s < usedAndConsideredStatsCount; s++)
                        {
                            int si = usedAndConsideredStatIndices[s];
                            sumCreatureLevels += c.levelsWild[si] > 0 ? c.levelsWild[si] : 0;
                        }
                        c.topness = (short)(100 * sumCreatureLevels / sumTopLevels);
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
                    c.SetTopStatCount(considerStatHighlight);
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
                    foreach (Creature p in creatureCollection.creatures)
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

            creatureCollection.creatures.AddRange(placeholderAncestors);
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
                    levelStep: creatureCollection.getWildLevelStep())
            {
                guid = creatureGuid,
                status = CreatureStatus.Unavailable,
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

        private void ShowCreaturesInListView(List<Creature> creatures)
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
        }

        /// <summary>
        /// Call this function to update the displayed values of a creature. Usually called after a creature was edited.
        /// </summary>
        /// <param name="cr">Creature that was changed</param>
        /// <param name="creatureStatusChanged"></param>
        private void UpdateCreatureValues(Creature cr, bool creatureStatusChanged)
        {
            // if row is selected, save and reselect later
            List<Creature> selectedCreatures = new List<Creature>();
            foreach (ListViewItem i in listViewLibrary.SelectedItems)
                selectedCreatures.Add((Creature)i.Tag);

            // data of the selected creature changed, update listview
            cr.RecalculateCreatureValues(creatureCollection.getWildLevelStep());
            // if creaturestatus (available/dead) changed, recalculate topstats (dead creatures are not considered there)
            if (creatureStatusChanged)
            {
                CalculateTopStats(creatureCollection.creatures.Where(c => c.Species == cr.Species).ToList());
                FilterLib();
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
            CreateOwnerList();
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

        private ListViewItem CreateCreatureLVItem(Creature cr, ListViewGroup g)
        {
            double colorFactor = 100d / creatureCollection.maxChartLevel;
            DateTime? cldGr = cr.cooldownUntil.HasValue && cr.growingUntil.HasValue ?
                (cr.cooldownUntil.Value > cr.growingUntil.Value ? cr.cooldownUntil.Value : cr.growingUntil.Value)
                : cr.cooldownUntil ?? (cr.growingUntil ?? default(DateTime?));
            bool cld = cr.cooldownUntil > cr.growingUntil;

            string[] subItems = new[]
                    {
                            cr.name + (cr.status!=CreatureStatus.Available ? $" ({Utils.statusSymbol(cr.status)})" : ""),
                            cr.owner + (string.IsNullOrEmpty(cr.tribe) ? "" : $" ({cr.tribe})"),
                            cr.note,
                            cr.server,
                            Utils.sexSymbol(cr.sex),
                            cr.domesticatedAt?.ToString("yyyy'-'MM'-'dd HH':'mm':'ss") ?? "?",
                            cr.topness.ToString(),
                            cr.topStatsCount.ToString(),
                            cr.generation.ToString(),
                            cr.levelFound.ToString(),
                            cr.Mutations.ToString(),
                            DateTime.Now.CompareTo(cldGr) < 0 ? cldGr.ToString() : "-"
                    }
                    .Concat(cr.levelsWild.Select(x => x.ToString()).ToArray())
                    .ToArray();

            if (Properties.Settings.Default.showColorsInLibrary)
                subItems = subItems.Concat(cr.colors.Select(cl => cl.ToString()).ToArray()).ToArray();

            ListViewItem lvi = new ListViewItem(subItems, g);
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
                    lvi.SubItems[s + 12].BackColor = Utils.getColorFromPercent((int)(cr.levelsWild[s] * (s == (int)StatNames.Torpidity ? colorFactor / 7 : colorFactor)), // TODO set factor to number of other stats (flyers have 6, Gacha has 8?)
                            considerStatHighlight[s] ? cr.topBreedingStats[s] ? 0.2 : 0.7 : 0.93);
            }
            lvi.SubItems[4].BackColor = cr.flags.HasFlag(CreatureFlags.Neutered) ? Color.FromArgb(220, 220, 220) :
                    cr.sex == Sex.Female ? Color.FromArgb(255, 230, 255) :
                    cr.sex == Sex.Male ? Color.FromArgb(220, 235, 255) : SystemColors.Window;

            if (cr.status == CreatureStatus.Dead)
            {
                lvi.SubItems[0].ForeColor = SystemColors.GrayText;
                lvi.BackColor = Color.FromArgb(255, 250, 240);
            }
            else if (cr.status == CreatureStatus.Unavailable)
            {
                lvi.SubItems[0].ForeColor = SystemColors.GrayText;
            }
            else if (cr.status == CreatureStatus.Obelisk)
            {
                lvi.SubItems[0].ForeColor = Color.DarkBlue;
            }
            else if (creatureCollection.maxServerLevel > 0
                    && cr.levelsWild[(int)StatNames.Torpidity] + 1 + creatureCollection.maxDomLevel > creatureCollection.maxServerLevel)
            {
                lvi.SubItems[0].ForeColor = Color.OrangeRed; // this creature may pass the max server level and could be deleted by the game
            }

            lvi.UseItemStyleForSubItems = false;

            // color for top-stats-nr
            if (cr.topStatsCount > 0)
            {
                if (cr.topBreedingCreature)
                {
                    if (cr.topStatsCount == considerStatHighlight.Count(ts => ts))
                        lvi.BackColor = Color.Gold;
                    else
                        lvi.BackColor = Color.LightGreen;
                }
                lvi.SubItems[7].BackColor = Utils.getColorFromPercent(cr.topStatsCount * 8 + 44, 0.7);
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
            lvi.SubItems[6].BackColor = Utils.getColorFromPercent(cr.topness * 2 - 100, 0.8); // topness is in percent. gradient from 50-100

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
                        lvi.SubItems[24 + cl].BackColor = CreatureColors.creatureColor(cr.colors[cl]);
                        lvi.SubItems[24 + cl].ForeColor = Utils.ForeColor(lvi.SubItems[24 + cl].BackColor);
                    }
                    else
                    {
                        lvi.SubItems[24 + cl].ForeColor = Color.White;
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
            ListViewColumnSorter.doSort((ListView)sender, e.Column);
        }

        // onlibrarychange
        private async void listViewLibrary_SelectedIndexChanged(object sender, EventArgs e)
        {
            cancelTokenLibrarySelection?.Cancel();
            using (cancelTokenLibrarySelection = new CancellationTokenSource())
            {
                try
                {
                    reactOnSelectionChange = false;
                    await Task.Delay(20, cancelTokenLibrarySelection.Token); // recalculate breedingplan at most a certain interval
                    reactOnSelectionChange = true;
                    LibrarySelectedIndexChanged();
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
            cancelTokenLibrarySelection = null;
        }

        /// <summary>
        /// Updates infos about the selected creatures like tags, levels and stat-level distribution.
        /// </summary>
        private void LibrarySelectedIndexChanged()
        {
            if (!reactOnSelectionChange)
                return;

            int cnt = listViewLibrary.SelectedItems.Count;
            if (cnt > 0)
            {
                if (cnt == 1)
                {
                    Creature c = (Creature)listViewLibrary.SelectedItems[0].Tag;
                    creatureBoxListView.SetCreature(c);
                    if (tabControlLibFilter.SelectedTab == tabPageLibRadarChart)
                        radarChartLibrary.setLevels(c.levelsWild);
                    pedigreeNeedsUpdate = true;
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
                            ? $"level: {selCrs[0].Level}" + (selCrs[0].ArkIdImported ? $"; Ark-Id (ingame): {Utils.ConvertImportedArkIdToIngameVisualization(selCrs[0].ArkId)}" : "")
                            : $"level-range: {selCrs.Min(cr => cr.Level)} - {selCrs.Max(cr => cr.Level)}"
                        ) + "\n" +
                        $"Tags: {string.Join(", ", tagList)}");
            }
            else
            {
                SetMessageLabelText();
                creatureBoxListView.Clear();
            }
        }

        /// <summary>
        /// Filter the displayed creatures for the library listview
        /// </summary>
        /// <param name="param"></param>
        /// <param name="show"></param>
        private void SetLibraryFilter(string param, bool show)
        {
            if (libraryViews.ContainsKey(param) && libraryViews[param] != show)
            {
                libraryViews[param] = show;

                switch (param)
                {
                    case "Dead":
                        creatureCollection.showFlags = show ? (creatureCollection.showFlags | CreatureFlags.Dead) : (creatureCollection.showFlags & ~CreatureFlags.Dead);
                        checkBoxShowDead.Checked = show;
                        deadCreaturesToolStripMenuItem.Checked = show;
                        break;
                    case "Unavailable":
                        creatureCollection.showFlags = show ? (creatureCollection.showFlags | CreatureFlags.Unavailable) : (creatureCollection.showFlags & ~CreatureFlags.Unavailable);
                        checkBoxShowUnavailableCreatures.Checked = show;
                        unavailableCreaturesToolStripMenuItem.Checked = show;
                        break;
                    case "Neutered":
                        creatureCollection.showFlags = show ? (creatureCollection.showFlags | CreatureFlags.Neutered) : (creatureCollection.showFlags & ~CreatureFlags.Neutered);
                        checkBoxShowNeuteredCreatures.Checked = show;
                        neuteredCreaturesToolStripMenuItem.Checked = show;
                        break;
                    case "Obelisk":
                        creatureCollection.showFlags = show ? (creatureCollection.showFlags | CreatureFlags.Obelisk) : (creatureCollection.showFlags & ~CreatureFlags.Obelisk);
                        checkBoxShowObeliskCreatures.Checked = show;
                        obeliskCreaturesToolStripMenuItem.Checked = show;
                        break;
                    case "Cryopod":
                        creatureCollection.showFlags = show ? (creatureCollection.showFlags | CreatureFlags.Cryopod) : (creatureCollection.showFlags & ~CreatureFlags.Cryopod);
                        checkBoxShowCryopodCreatures.Checked = show;
                        cryopodCreaturesToolStripMenuItem.Checked = show;
                        break;
                    case "Mutated":
                        creatureCollection.showFlags = show ? (creatureCollection.showFlags | CreatureFlags.Mutated) : (creatureCollection.showFlags & ~CreatureFlags.Mutated);
                        checkBoxShowMutatedCreatures.Checked = show;
                        mutatedCreaturesToolStripMenuItem.Checked = show;
                        break;
                    case "Females":
                        checkBoxShowMutatedCreatures.Checked = show;
                        mutatedCreaturesToolStripMenuItem.Checked = show;
                        break;
                    case "Males":
                        checkBoxShowMutatedCreatures.Checked = show;
                        mutatedCreaturesToolStripMenuItem.Checked = show;
                        break;
                }

                RecalculateTopStatsIfNeeded();
                FilterLib();
            }
        }

        /// <summary>
        /// Call this list to set the listview for the library to the current filters
        /// </summary>
        private void FilterLib()
        {
            if (!filterListAllowed)
                return;

            // save selected creatures to re-select them after the filtering
            List<Creature> selectedCreatures = new List<Creature>();
            foreach (ListViewItem i in listViewLibrary.SelectedItems)
                selectedCreatures.Add((Creature)i.Tag);

            var filteredList = from creature in creatureCollection.creatures
                               where !creature.flags.HasFlag(CreatureFlags.Placeholder)
                               select creature;

            // if only one species should be shown adjust statnames if the selected species is a glow-species
            bool chargeStatsHeaders = false;
            if (listBoxSpeciesLib.SelectedIndex > 0
                && listBoxSpeciesLib.SelectedItem.GetType() == typeof(Species))
            {
                Species selectedSpecies = listBoxSpeciesLib.SelectedItem as Species;
                filteredList = filteredList.Where(c => c.Species == selectedSpecies);
                if (selectedSpecies.IsGlowSpecies)
                    chargeStatsHeaders = true;
            }
            for (int s = 0; s < Values.STATS_COUNT; s++)
                listViewLibrary.Columns[12 + s].Text = Utils.statName(s, true, chargeStatsHeaders);

            filteredList = ApplyLibraryFilterSettings(filteredList);

            // display new results
            ShowCreaturesInListView(filteredList.OrderBy(c => c.name).ToList());

            // update creaturebox
            creatureBoxListView.UpdateLabel();

            // select previous selecteded creatures again
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
        /// Apply library filter settings to a creature collection
        /// </summary>
        private IEnumerable<Creature> ApplyLibraryFilterSettings(IEnumerable<Creature> creatures)
        {
            if (creatures == null)
                return Enumerable.Empty<Creature>();

            // if only certain owner's creatures should be shown
            bool hideWOOwner = creatureCollection.hiddenOwners.Contains("n/a");
            creatures = creatures.Where(c => !creatureCollection.hiddenOwners.Contains(c.owner) && (!hideWOOwner || c.owner != ""));

            // server filter
            bool hideWOServer = creatureCollection.hiddenServers.Contains("n/a");
            creatures = creatures.Where(c => !creatureCollection.hiddenServers.Contains(c.server) && (!hideWOServer || c.server != ""));

            // tags filter
            bool dontShowWOTags = creatureCollection.dontShowTags.Contains("n/a");
            creatures = creatures.Where(c => !dontShowWOTags && c.tags.Count == 0 || c.tags.Except(creatureCollection.dontShowTags).Any());

            // show also dead creatures?
            if (!libraryViews["Dead"])
                creatures = creatures.Where(c => c.status != CreatureStatus.Dead);

            // show also unavailable creatures?
            if (!libraryViews["Unavailable"])
                creatures = creatures.Where(c => c.status != CreatureStatus.Unavailable);

            // show also in obelisks uploaded creatures?
            if (!libraryViews["Obelisk"])
                creatures = creatures.Where(c => c.status != CreatureStatus.Obelisk);

            // show also creatures in cryopods?
            if (!libraryViews["Cryopod"])
                creatures = creatures.Where(c => c.status != CreatureStatus.Cryopod);

            // show also neutered creatures?
            if (!libraryViews["Neutered"])
                creatures = creatures.Where(c => !c.flags.HasFlag(CreatureFlags.Neutered));

            // show also creatures with mutations?
            if (!libraryViews["Mutated"])
                creatures = creatures.Where(c => c.Mutations <= 0);

            // show also different sexes?
            if (!libraryViews["Females"])
                creatures = creatures.Where(c => c.sex != Sex.Female);
            if (!libraryViews["Males"])
                creatures = creatures.Where(c => c.sex != Sex.Male);

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
                    editCreatureInTester((Creature)listViewLibrary.Items[listViewLibrary.SelectedIndices[0]].Tag);
            }
            else if (e.KeyCode == Keys.F3)
            {
                if (listViewLibrary.SelectedIndices.Count > 0)
                    ShowMultiSetter();
            }
            else if (e.KeyCode == Keys.A && e.Control)
            {
                // select all list-entries
                reactOnSelectionChange = false;
                listViewLibrary.BeginUpdate();
                foreach (ListViewItem i in listViewLibrary.Items)
                    i.Selected = true;
                listViewLibrary.EndUpdate();
                reactOnSelectionChange = true;
                listViewLibrary_SelectedIndexChanged(null, null);
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
                    string output = "Species\tName\tSex\tOwner\t";

                    var suffixe = new List<string> { "w", "d", "b", "v" }; // wild, dom, bred-values, dom-values
                    foreach (var suffix in suffixe)
                    {
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            output += Utils.statName(Values.statsDisplayOrder[s], true) + suffix + "\t";
                        }
                    }
                    output += "mother\tfather\tMut\tNotes\tColor0\tColor1\tColor2\tColor3\tColor4\tColor5";

                    foreach (ListViewItem l in listViewLibrary.SelectedItems)
                    {
                        Creature c = (Creature)l.Tag;
                        output += "\n" + c.Species.name + "\t" + c.name + "\t" + c.sex + "\t" + c.owner;
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            output += "\t" + c.levelsWild[Values.statsDisplayOrder[s]];
                        }
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            output += "\t" + c.levelsDom[Values.statsDisplayOrder[s]];
                        }
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            output += $"\t{c.valuesBreeding[Values.statsDisplayOrder[s]] * (Utils.precision(Values.statsDisplayOrder[s]) == 3 ? 100 : 1)}{(Utils.precision(Values.statsDisplayOrder[s]) == 3 ? "%" : "")}";
                        }
                        for (int s = 0; s < Values.STATS_COUNT; s++)
                        {
                            output += $"\t{c.valuesDom[Values.statsDisplayOrder[s]] * (Utils.precision(Values.statsDisplayOrder[s]) == 3 ? 100 : 1)}{(Utils.precision(Values.statsDisplayOrder[s]) == 3 ? "%" : "")}";
                        }
                        output += $"\t{(c.Mother != null ? c.Mother.name : "")}\t{(c.Father != null ? c.Father.name : "")}\t{c.Mutations}\t{(c.note != null ? c.note.Replace("\r", "").Replace("\n", " ") : "")}";
                        for (int cl = 0; cl < 6; cl++)
                        {
                            output += "\t" + c.colors[cl];
                        }
                    }
                    Clipboard.SetText(output);
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
        private void ExportAsTextToClipboard(Creature c, bool breeding = true, bool ARKml = true)
        {
            if (c != null)
            {
                double colorFactor = 100d / creatureCollection.maxChartLevel;
                bool wild = c.tamingEff == -2;
                string modifierText = "";
                if (!breeding)
                {
                    if (wild)
                        modifierText = ", wild";
                    else if (c.tamingEff < 1)
                        modifierText = ", TE: " + Math.Round(100 * c.tamingEff, 1) + "%";
                    else if (c.imprintingBonus > 0)
                        modifierText = ", Impr: " + Math.Round(100 * c.imprintingBonus, 2) + "%";
                }

                string output = (string.IsNullOrEmpty(c.name) ? "noName" : c.name) + " (" + (ARKml ? Utils.getARKml(c.Species.name, 50, 172, 255) : c.Species.name)
                        + ", Lvl " + (breeding ? c.LevelHatched : c.Level) + modifierText + (c.sex != Sex.Unknown ? ", " + c.sex : "") + "): ";
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    int si = Values.statsDisplayOrder[s];
                    if (c.levelsWild[si] >= 0 && c.valuesBreeding[si] > 0) // ignore unknown levels (e.g. oxygen, speed)
                        output += Utils.statName(si, true) + ": " + (breeding ? c.valuesBreeding[si] : c.valuesDom[si]) * (Utils.precision(si) == 3 ? 100 : 1) + (Utils.precision(si) == 3 ? "%" : "") +
                                " (" + (ARKml ? Utils.getARKmlFromPercent(c.levelsWild[si].ToString(), (int)(c.levelsWild[si] * (si == (int)StatNames.Torpidity ? colorFactor / 7 : colorFactor))) : c.levelsWild[si].ToString()) +
                                (ARKml ? breeding || si == (int)StatNames.Torpidity ? "" : ", " + Utils.getARKmlFromPercent(c.levelsDom[si].ToString(), (int)(c.levelsDom[si] * colorFactor)) : breeding || si == (int)StatNames.Torpidity ? "" : ", " + c.levelsDom[si]) + "); ";
                }
                Clipboard.SetText(output.Substring(0, output.Length - 1));
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
                creatureCollection.tags,
                Values.V.species,
                creatureCollection.ownerList,
                creatureCollection.tribes.Select(t => t.TribeName).ToArray(),
                creatureCollection.serverList))
            {
                if (ms.ShowDialog() == DialogResult.OK)
                {
                    if (ms.ParentsChanged)
                        UpdateParents(selectedCreatures);
                    if (ms.TagsChanged)
                        CreateCreatureTagList();
                    if (ms.SpeciesChanged)
                        UpdateSpeciesLists(creatureCollection.creatures);
                    CreateOwnerList();
                    SetCollectionChanged(true, !multipleSpecies ? sp : null);
                    RecalculateTopStatsIfNeeded();
                    FilterLib();
                }
            }
        }
    }
}
