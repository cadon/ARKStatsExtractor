using ARKBreedingStats.BreedingPlanning;
using ARKBreedingStats.Library;
using ARKBreedingStats.Pedigree;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class CurrentBreeds : UserControl
    {
        private Species _currentSpecies;
        public event Form1.CollectionChangedEventHandler Changed;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CurrentBreedingPair[] CurrentBreedingPairs
        {
            set
            {
                _currentBreedsBySpeciesBp.Clear();
                if (value?.Any() != true) return;
                foreach (var p in value)
                {
                    if (p.Mother == null || p.Father == null) continue;
                    var bp = p.Mother.speciesBlueprint;
                    if (_currentBreedsBySpeciesBp.TryGetValue(bp, out var list))
                        list.Add(p);
                    else _currentBreedsBySpeciesBp.Add(bp, new List<CurrentBreedingPair> { p });
                }
            }
            get
            {
                if (!_currentBreedsBySpeciesBp.Any()) return null;
                return _currentBreedsBySpeciesBp.SelectMany(kv => kv.Value).ToArray();
            }
        }

        /// <summary>
        /// Current breeding pairs, the key is the blueprint path of the mother's species.
        /// </summary>
        private readonly Dictionary<string, List<CurrentBreedingPair>> _currentBreedsBySpeciesBp =
            new Dictionary<string, List<CurrentBreedingPair>>();

        public CurrentBreeds()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Updates controls of breeding pairs.
        /// </summary>
        /// <param name="species">Species to set. If null and if forceUpdate is true the currently displayed species is updated.</param>
        /// <param name="forceUpdate">Updates controls even if the species is already displayed</param>
        public void DisplaySpeciesCurrentBreedingPairs(Species species, bool forceUpdate = false)
        {
            if (species != null)
            {
                if (_currentSpecies == species && !forceUpdate)
                    return;
                _currentSpecies = species;
                LbTitle.Text = "Current breeding pairs of " + species.name;
            }
            else if (!forceUpdate) return;
            var pairsToDisplay = GetCurrentBreedingPairs(_currentSpecies, out var interSpeciesMating);
            var controls = CreateControlsOfBreedingPairs(pairsToDisplay, _currentSpecies, interSpeciesMating);

            FlpBreedingPairs.SuspendDrawingAndLayout();
            FlpBreedingPairs.Controls.Clear();
            if (controls != null)
                FlpBreedingPairs.Controls.AddRange(controls);
            FlpBreedingPairs.ResumeDrawingAndLayout();
        }

        private List<CurrentBreedingPair> GetCurrentBreedingPairs(Species species, out bool interSpeciesMating)
        {
            interSpeciesMating = false;
            if (species == null) return null;
            if (!_currentBreedsBySpeciesBp.TryGetValue(species.blueprintPath, out var pairsToDisplay))
                pairsToDisplay = new List<CurrentBreedingPair>();
            if (species.matesWith?.Any() == true)
            {
                foreach (var bp in species.matesWith)
                {
                    if (_currentBreedsBySpeciesBp.TryGetValue(bp, out var additionalPairsToDisplay)
                        && additionalPairsToDisplay.Any())
                    {
                        pairsToDisplay.AddRange(additionalPairsToDisplay);
                        interSpeciesMating = true;
                    }
                }
            }

            return pairsToDisplay.OrderBy(p => p.StartedBreedingAt).ToList();
        }

        private Control[] CreateControlsOfBreedingPairs(List<CurrentBreedingPair> pairsToDisplay, Species species, bool displaySpecies)
        {
            if (pairsToDisplay == null) return null;
            var enabledColorRegions = species.EnabledColorRegions;
            var controls = new List<Control>();
            foreach (var pair in pairsToDisplay)
            {
                var leftParentControl =
                    new PedigreeCreature(pair.Mother, enabledColorRegions, displaySpecies: displaySpecies, cursorHand: false);
                var rightParentControl =
                    new PedigreeCreature(pair.Father, enabledColorRegions, displaySpecies: displaySpecies, cursorHand: false);
                var delButton = new Button { Text = "×", BackColor = Color.LightSalmon, Width = 23, Height = PedigreeCreation.PedigreeElementHeight - 5, Tag = pair, Anchor = AnchorStyles.Bottom };
                FlpBreedingPairs.SetFlowBreak(delButton, true);
                delButton.Click += (s, e) => RemovePair(((Button)s).Tag as CurrentBreedingPair);
                controls.Add(leftParentControl);
                controls.Add(rightParentControl);
                controls.Add(delButton);
            }

            return controls.ToArray();
        }

        public void AddPair(Creature mother, Creature father)
        {
            var pair = new CurrentBreedingPair(mother, father);
            var bp = mother.speciesBlueprint;
            if (_currentBreedsBySpeciesBp.TryGetValue(bp, out var list))
            {
                if (list.Contains(pair)) return;
                list.Add(pair);
            }
            else _currentBreedsBySpeciesBp.Add(bp, new List<CurrentBreedingPair> { pair });

            if (_currentSpecies == mother.Species)
                DisplaySpeciesCurrentBreedingPairs(mother.Species, true);

            Changed?.Invoke();
        }

        public void RemovePair(CurrentBreedingPair pair)
        {
            if (pair == null) return;

            var motherSpeciesBlueprint = pair.Mother?.speciesBlueprint;
            if (motherSpeciesBlueprint == null) return;

            if (_currentBreedsBySpeciesBp.TryGetValue(motherSpeciesBlueprint, out var pairs))
                if (pairs.Remove(pair) && pairs.Count == 0)
                    _currentBreedsBySpeciesBp.Remove(motherSpeciesBlueprint);

            if (_currentSpecies == pair.Mother?.Species)
                DisplaySpeciesCurrentBreedingPairs(pair.Mother?.Species, true);

            Changed?.Invoke();
        }
    }
}
