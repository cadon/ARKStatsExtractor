using System;

using ARKBreedingStats.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for the ArkIdConverter static helper methods.
    /// </summary>
    [TestClass]
    public class ArkIdConverterTests
    {
        [TestMethod]
        public void ConvertArkIdToGuid_AndBack_RoundTrips()
        {
            // Arrange
            long arkId = 380416179841773683L;

            // Act
            var guid = ArkIdConverter.ConvertArkIdToGuid(arkId);
            long result = ArkIdConverter.ConvertCreatureGuidToArkId(guid);

            // Assert
            Assert.AreEqual(arkId, result);
        }

        [TestMethod]
        public void ConvertArkIdToGuid_ZeroArkId_ProducesEmptyGuid()
        {
            // Act
            var guid = ArkIdConverter.ConvertArkIdToGuid(0L);

            // Assert
            Assert.AreEqual(Guid.Empty, guid);
        }

        [TestMethod]
        public void ConvertArkIdsToLongArkId_CombinesTwoInts()
        {
            // Arrange
            int id1 = 12345;
            int id2 = 67890;

            // Act
            long combined = ArkIdConverter.ConvertArkIdsToLongArkId(id1, id2);

            // Assert
            Assert.AreEqual(((long)id1 << 32) | (id2 & 0xFFFFFFFFL), combined);
        }

        [TestMethod]
        public void ConvertArkId64ToArkIds32_SplitsLong()
        {
            // Arrange
            int id1 = 12345;
            int id2 = 67890;
            long combined = ArkIdConverter.ConvertArkIdsToLongArkId(id1, id2);

            // Act
            var (high, low) = ArkIdConverter.ConvertArkId64ToArkIds32(combined);

            // Assert
            Assert.AreEqual(id1, high);
            Assert.AreEqual(id2, low);
        }

        [TestMethod]
        public void ConvertArkIdsToLongArkId_NegativeId2_MasksCorrectly()
        {
            // Arrange
            int id1 = 1;
            int id2 = -1; // 0xFFFFFFFF

            // Act
            long combined = ArkIdConverter.ConvertArkIdsToLongArkId(id1, id2);
            var (high, low) = ArkIdConverter.ConvertArkId64ToArkIds32(combined);

            // Assert
            Assert.AreEqual(id1, high);
            Assert.AreEqual(id2, low);
        }

        [TestMethod]
        public void ConvertImportedArkIdToIngameVisualization_FormatsCorrectly()
        {
            // Arrange
            int id1 = 88;
            int id2 = 654321;
            long arkId = ArkIdConverter.ConvertArkIdsToLongArkId(id1, id2);

            // Act
            string vis = ArkIdConverter.ConvertImportedArkIdToIngameVisualization(arkId);

            // Assert
            Assert.AreEqual($"{id1}{id2}", vis);
        }

        [TestMethod]
        public void IsArkIdImported_MatchingGuid_ReturnsTrue()
        {
            // Arrange
            long arkId = 123456789L;
            var guid = ArkIdConverter.ConvertArkIdToGuid(arkId);

            // Act & Assert
            Assert.IsTrue(ArkIdConverter.IsArkIdImported(arkId, guid));
        }

        [TestMethod]
        public void IsArkIdImported_ZeroArkId_ReturnsFalse()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act & Assert
            Assert.IsFalse(ArkIdConverter.IsArkIdImported(0L, guid));
        }

        [TestMethod]
        public void IsArkIdImported_MismatchedGuid_ReturnsFalse()
        {
            // Arrange
            long arkId = 123456789L;
            var wrongGuid = Guid.NewGuid();

            // Act & Assert
            Assert.IsFalse(ArkIdConverter.IsArkIdImported(arkId, wrongGuid));
        }
    }
}
