using System;
using ARKBreedingStats.Library;
using ARKBreedingStats.uiControls;

namespace ARKBreedingStats.Pedigree
{
    public interface IPedigreeCreature
    {
        event PedigreeCreature.CreatureChangedEventHandler CreatureClicked;

        /// <summary>
        /// Edit the creature. Boolean parameter determines if the creature is virtual.
        /// </summary>
        event Action<Creature, bool> CreatureEdit;

        /// <summary>
        /// Display the best breeding partners for the given creature.
        /// </summary>
        event Action<Creature> BestBreedingPartners;

        /// <summary>
        /// Display the creature in the pedigree.
        /// </summary>
        event Action<Creature> DisplayInPedigree;

        /// <summary>
        /// Recalculate the breeding plan, e.g. if the cooldown was reset.
        /// </summary>
        event Action RecalculateBreedingPlan;
    }
}
