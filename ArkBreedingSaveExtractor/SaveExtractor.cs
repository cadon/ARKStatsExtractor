using System.Threading.Tasks;

using ARKBreedingStats;


namespace ArkBreedingSaveExtractor
{
    class SaveExtractor
    {
        public string ArkFile { get; set; }
        public string ServerName { get; set; }
        public string TribeFilter { get; set; }
        public bool UpdateStatus { get; set; } = true;

        public async Task Run(CreatureCollection cc)
        {
            Values.V.loadValues();

            await ImportSavegame.ImportCollectionFromSavegame(cc, ArkFile, ServerName, TribeFilter, UpdateStatus);
        }
    }
}
