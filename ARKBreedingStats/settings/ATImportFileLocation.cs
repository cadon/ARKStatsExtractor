namespace ARKBreedingStats.settings
{

    public class ATImportFileLocation
    {
        public string ConvenientName { get; }
        public string ServerName { get; }
        public string FileLocation { get; }
        public bool ImportWithQuickImport { get; set; }

        public ATImportFileLocation() { }

        public ATImportFileLocation(string convenientName, string serverName, string fileLocation, bool quickImport = false)
        {
            ConvenientName = convenientName;
            ServerName = serverName;
            FileLocation = fileLocation;
            ImportWithQuickImport = quickImport;
        }

        public static ATImportFileLocation CreateFromString(string path)
        {
            string[] pathParts = path.Split('|');

            return pathParts.Length == 3 ?
                    new ATImportFileLocation(pathParts[0], pathParts[1], pathParts[2])
                    : pathParts.Length == 4 ?
                    new ATImportFileLocation(pathParts[0], pathParts[1], pathParts[2], pathParts[3] == "True")
                    : new ATImportFileLocation(path, string.Empty, path);
        }

        public override string ToString() => $"{ConvenientName}|{ServerName}|{FileLocation}|{ImportWithQuickImport}";
    }

}
