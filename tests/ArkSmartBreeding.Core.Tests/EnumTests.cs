using ARKBreedingStats.Library;
using ARKBreedingStats.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for Sex enum, CreatureStatus enum, and CreatureFlags enum.
    /// </summary>
    [TestClass]
    public class EnumTests
    {
        [TestMethod]
        public void CreatureFlags_AreFlagValues()
        {
            // Verify it's a proper flags enum
            var combined = CreatureFlags.Mutated | CreatureFlags.Neutered;
            Assert.IsTrue(combined.HasFlag(CreatureFlags.Mutated));
            Assert.IsTrue(combined.HasFlag(CreatureFlags.Neutered));
            Assert.IsFalse(combined.HasFlag(CreatureFlags.Female));
        }

        [TestMethod]
        public void CreatureFlags_StatusMask_ExcludesStatusBits()
        {
            // StatusMask should only include non-status flags
            Assert.IsTrue(CreatureFlags.StatusMask.HasFlag(CreatureFlags.Mutated));
            Assert.IsTrue(CreatureFlags.StatusMask.HasFlag(CreatureFlags.Neutered));
            Assert.IsFalse(CreatureFlags.StatusMask.HasFlag(CreatureFlags.Available));
        }
    }
}
