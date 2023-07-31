using System;
using System.IO;
using System.Text;
using ARKBreedingStats.Library;
using ARKBreedingStats.values;
using Newtonsoft.Json;

namespace ARKBreedingStats.importExportGun
{
    /// <summary>
    /// Imports creature files created with the export gun (mod).
    /// </summary>
    internal static class ImportExportGun
    {
        /// <summary>
        /// Import file created with the export gun (mod).
        /// </summary>
        public static Creature ImportCreature(string filePath, string serverUuid, out string error)
        {
            error = null;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return null;

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        br.ReadBytes(4);
                        if (Encoding.UTF8.GetString(br.ReadBytes(19))
                            != "DinoExportGunSave_C")
                            return null;

                        if (!SearchBytes(br, Encoding.ASCII.GetBytes("StrProperty")))
                            return null;

                        br.ReadBytes(9); // skipping to json string length
                        var jsonLength = br.ReadInt32();
                        if (jsonLength <= 0) return null;
                        var jsonText = Encoding.UTF8.GetString(br.ReadBytes(jsonLength));
                        return ConvertExportGunToCreature(JsonConvert.DeserializeObject<ExportGunFile>(jsonText), serverUuid, out error);
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return null;
        }

        /// <summary>
        /// Looks for a specific byte pattern sequence, the stream position is set after that pattern.
        /// </summary>
        /// <param name="br"></param>
        /// <param name="bytesToFind"></param>
        /// <returns>True if pattern found.</returns>
        private static bool SearchBytes(BinaryReader br, byte[] bytesToFind)
        {
            if (bytesToFind == null || bytesToFind.Length == 0) return false;
            var pi = 0; // index of pattern currently comparing, indices before already found
            var l = br.BaseStream.Length;
            while (br.BaseStream.Position < l)
            {
                if (br.ReadByte() == bytesToFind[pi])
                {
                    pi++;
                    if (pi == bytesToFind.Length)
                        return true;
                    continue;
                }

                pi = 0;
            }
            return false;
        }

        private static Creature ConvertExportGunToCreature(ExportGunFile ec, string serverUuid, out string error)
        {
            error = null;
            if (ec == null) return null;

            if (serverUuid != ec.ServerUUID)
            {
                error = "Server UUID doesn't match, the stat values might differ. It's recommend to export the server multipliers.";
            }

            var species = Values.V.SpeciesByBlueprint(ec.BlueprintPath, true);
            if (species == null)
            {
                error = $"blueprintpath {ec.BlueprintPath} couldn't be found, maybe you need to load a mod values file.";
                return null;
            }

            var wildLevels = new int[Stats.StatsCount];
            var domLevels = new int[Stats.StatsCount];
            var si = 0;
            foreach (var s in ec.Stats)
            {
                wildLevels[si] = s.Wild;
                domLevels[si] = s.Tamed;
                si++;
            }

            var arkId = Utils.ConvertArkIdsToLongArkId(ec.DinoID1, ec.DinoID2);

            var c = new Creature(species, ec.DinoName, !string.IsNullOrEmpty(ec.OwningPlayerName) ? ec.OwningPlayerName : !string.IsNullOrEmpty(ec.ImprinterName) ? ec.ImprinterName : ec.TamerString,
                ec.TribeName, species.noGender ? Sex.Unknown : ec.IsFemale ? Sex.Female : Sex.Male, wildLevels, domLevels,
                ec.TameEffectiveness, !string.IsNullOrEmpty(ec.ImprinterName), ec.DinoImprintingQuality,
                CreatureCollection.CurrentCreatureCollection?.wildLevelStep)
            {
                ArkId = arkId,
                guid = Utils.ConvertArkIdToGuid(arkId),
                ArkIdImported = true,
                colors = ec.ColorSetIndices,
                Maturation = ec.BabyAge,
                mutationsMaternal = ec.RandomMutationsFemale,
                mutationsPaternal = ec.RandomMutationsMale
            };

            c.RecalculateCreatureValues(CreatureCollection.CurrentCreatureCollection?.wildLevelStep);
            if (ec.NextAllowedMatingTimeDuration > 0)
                c.cooldownUntil = DateTime.Now.AddSeconds(ec.NextAllowedMatingTimeDuration);
            if (ec.MutagenApplied)
                c.flags |= CreatureFlags.MutagenApplied;
            if (ec.Neutered)
                c.flags |= CreatureFlags.Neutered;
            if (ec.Ancestry != null)
            {
                if (ec.Ancestry.femaleDinoId1 != 0 || ec.Ancestry.femaleDinoId2 != 0)
                    c.motherGuid =
                        Utils.ConvertArkIdToGuid(Utils.ConvertArkIdsToLongArkId(ec.Ancestry.femaleDinoId1,
                            ec.Ancestry.femaleDinoId2));
                if (ec.Ancestry.maleDinoId1 != 0 || ec.Ancestry.maleDinoId2 != 0)
                    c.fatherGuid =
                        Utils.ConvertArkIdToGuid(Utils.ConvertArkIdsToLongArkId(ec.Ancestry.maleDinoId1,
                            ec.Ancestry.maleDinoId2));
            }

            return c;
        }
    }
}
