using clipr;


namespace ArkBreedingSaveExtractor
{
    [ApplicationInfo(Name = "ASB Server file Extractor",
        Description = "Updates an ASB library file with creatures from a saved Ark.")]
    public class Options
    {
        [NamedArgument('q', "quiet", Action = ParseAction.StoreTrue,
            Description = "Don't show stats and results.")]
        public bool Quiet { get; set; } = false;

        [PositionalArgument(0, MetaVar = "ARKFILE",
            Description = "The .ark file to read from.")]
        public string ArkFile { get; set; }

        [PositionalArgument(1, MetaVar = "SERVERNAME",
            Description = "Name to use for the server field in creatures.")]
        public string ServerName { get; set; }

        [PositionalArgument(2, MetaVar = "LIBRARY",
            Description = "The ASB library .xml file to update.")]
        public string LibraryFile { get; set; }

        [NamedArgument('c', "create", Action = ParseAction.StoreTrue,
            Description = "Create the library if it doesn't already exist.")]
        public bool CreateLibrary { get; set; } = false;

        [NamedArgument('t', "tribe", Action = ParseAction.Store,
            Description = "Filter and only read creatures from the named tribe.")]
        public string TribeFilter { get; set; }

        [NamedArgument('n', "no-status", Action = ParseAction.StoreTrue,
            Description = "Do not update the status of creatures (use if importing from multiple arks).")]
        public bool DontUpdateStatus { get; set; } = false;
    }
}
