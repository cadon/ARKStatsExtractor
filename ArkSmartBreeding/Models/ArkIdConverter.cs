using System;

namespace ARKBreedingStats.Models;

/// <summary>
/// Pure conversion helpers for ARK creature ID formats.
/// </summary>
public static class ArkIdConverter
{
    /// <summary>
    /// Converts an imported ARK id (id1 &lt;&lt; 32 | id2) to a Guid.
    /// This may only be used if the ArkId is unique (i.e. imported, not user input).
    /// </summary>
    public static Guid ConvertArkIdToGuid(long arkId)
    {
        byte[] bytes = new byte[16];
        BitConverter.GetBytes(arkId).CopyTo(bytes, 0);
        return new Guid(bytes);
    }

    /// <summary>
    /// Converts a Guid back to an imported ARK id.
    /// This may only be used if the Guid was created from an imported ARK id.
    /// </summary>
    public static long ConvertCreatureGuidToArkId(Guid guid)
    {
        return BitConverter.ToInt64(guid.ToByteArray(), 0);
    }

    /// <summary>
    /// Returns the ARK id as shown in-game from the unique imported representation.
    /// The result is not always unique.
    /// </summary>
    public static string ConvertImportedArkIdToIngameVisualization(long importedArkId)
        => $"{(int)(importedArkId >> 32)}{(int)importedArkId}";

    /// <summary>
    /// Converts the two 32-bit ARK id parts into one 64-bit ARK id.
    /// </summary>
    public static long ConvertArkIdsToLongArkId(int id1, int id2) => ((long)id1 << 32) | (id2 & 0xFFFFFFFFL);

    /// <summary>
    /// Converts an int64 ARK id to the two int32 ids used in the game.
    /// </summary>
    public static (int, int) ConvertArkId64ToArkIds32(long id) => ((int)(id >> 32), (int)id);

    /// <summary>
    /// Returns true if the ArkId matches the Guid (i.e. the Guid was created from an imported ARK id).
    /// </summary>
    public static bool IsArkIdImported(long arkId, Guid guid)
        => arkId != 0 && guid == ConvertArkIdToGuid(arkId);
}
