using System;

namespace ArkSmartBreeding.Enums
{
    [Flags]
    public enum CreatureFlags
    {
        None = 0,
        Available = 1,
        Dead = 2,
        Unavailable = 4,
        Obelisk = 8,
        Cryopod = 16,
        // Deleted = 32, // not used anymore
        Mutated = 64,
        Neutered = 128,
        /// <summary>
        /// If a creature has unknown parents, they are placeholders until they are imported. placeholders are not shown in the library
        /// </summary>
        Placeholder = 256,
        Female = 512,
        Male = 1024,
        MutagenApplied = 2048,
        /// <summary>
        /// Indicates a dummy creature used as a species separator in the library listView.
        /// </summary>
        Divider = 4096,
        /// <summary>
        /// If applied to the flags with &, the status is removed.
        /// </summary>
        StatusMask = Mutated | Neutered | Placeholder | Female | Male | MutagenApplied | Divider
    }
}