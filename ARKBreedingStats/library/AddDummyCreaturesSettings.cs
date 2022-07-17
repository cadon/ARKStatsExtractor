using System;
using System.Windows.Forms;

namespace ARKBreedingStats.library
{
    public partial class AddDummyCreaturesSettings : Form
    {
        public AddDummyCreaturesSettings()
        {
            InitializeComponent();

            var settings = DummyCreatures.LastSettings != null
                ? DummyCreatures.LastSettings
                : new DummyCreatureCreationSettings();
            NudAmount.ValueSave = settings.CreatureCount;
            if (settings.OnlySelectedSpecies)
                RbOnlySelectedSpecies.Checked = true;
            else RbMultipleRandomSpecies.Checked = true;
            NudSpeciesAmount.ValueSave = settings.SpeciesCount;
            NudBreedForGenerations.ValueSave = settings.Generations;
            NudUsePairsPerGeneration.ValueSave = settings.PairsPerGeneration;
            NudProbabilityInheritingHigherStat.ValueSaveDouble = settings.ProbabilityHigherStat * 100;
            NudMutationChance.ValueSaveDouble = settings.RandomMutationChance * 100;
            nudMaxWildLevel.ValueSave = settings.MaxWildLevel;
            CbSetOwner.Checked = settings.SetOwner;
            CbSetTribe.Checked = settings.SetTribe;
            CbSetServer.Checked = settings.SetServer;
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtOk_Click(object sender, EventArgs e)
        {
            if (NudAmount.Value > 0)
            {
                DialogResult = DialogResult.OK;
                Settings = new DummyCreatureCreationSettings
                {
                    CreatureCount = (int)NudAmount.Value,
                    OnlySelectedSpecies = RbOnlySelectedSpecies.Checked,
                    SpeciesCount = (int)NudSpeciesAmount.Value,
                    Generations = (int)NudBreedForGenerations.Value,
                    PairsPerGeneration = (int)NudUsePairsPerGeneration.Value,
                    ProbabilityHigherStat = (double)NudProbabilityInheritingHigherStat.Value / 100,
                    RandomMutationChance = (double)NudMutationChance.Value / 100,
                    MaxWildLevel = (int)nudMaxWildLevel.Value,
                    SetOwner = CbSetOwner.Checked,
                    SetTribe = CbSetTribe.Checked,
                    SetServer = CbSetServer.Checked
                };
            }
            Close();
        }

        public DummyCreatureCreationSettings Settings;
    }
}
