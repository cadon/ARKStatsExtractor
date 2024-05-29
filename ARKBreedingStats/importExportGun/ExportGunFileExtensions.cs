namespace ARKBreedingStats.importExportGun
{
    internal static class ExportGunFileExtensions
    {
        public static bool IsWild(this ExportGunCreatureFile ec)
        // wild creatures have a TE of 100 %, so don't use that here
            => string.IsNullOrEmpty(ec.DinoName)
                         && string.IsNullOrEmpty(ec.TribeName)
                         && string.IsNullOrEmpty(ec.TamerString)
                         && string.IsNullOrEmpty(ec.OwningPlayerName)
                         && string.IsNullOrEmpty(ec.ImprinterName)
                         && ec.OwningPlayerID == 0
                ;

        public static bool IsBred(this ExportGunCreatureFile ec)
            => !string.IsNullOrEmpty(ec.ImprinterName) || (ec.DinoImprintingQuality > 0 && ec.TameEffectiveness > 0.9999);


        public static string Owner(this ExportGunCreatureFile ec)
            => !string.IsNullOrEmpty(ec.OwningPlayerName) ? ec.OwningPlayerName
                : !string.IsNullOrEmpty(ec.ImprinterName) ? ec.ImprinterName
                : ec.TamerString;

        /// <summary>
        /// Returns the stat value of this creature considering the offset for percentage based stats.
        /// If a stat is percentage based, Ark and this object internally store it as offset from 100 %.
        /// ASB expects the absolute value.
        /// </summary>
        public static float GetStatValue(this ExportGunCreatureFile ec, int statIndex)
            => ec == null ? 0 : ec.Stats[statIndex].Value + (Stats.IsPercentage(statIndex) ? 1 : 0);
    }
}
