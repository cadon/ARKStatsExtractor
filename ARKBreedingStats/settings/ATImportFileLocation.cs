namespace ARKBreedingStats.settings {

    public class ATImportFileLocation {

        public string ConvenientName { get; set; }
        public string ServerName { get; set; }
        public string FileLocation { get; set; }

        public ATImportFileLocation() { }

        public ATImportFileLocation(string convenientName, string serverName, string fileLocation) {
            ConvenientName = convenientName;
            ServerName = serverName;
            FileLocation = fileLocation;
        }

        public static ATImportFileLocation CreateFromString(string path) {
            string[] pathParts = path.Split('|');

            return pathParts.Length == 3 ? 
                    new ATImportFileLocation(pathParts[0], pathParts[1], pathParts[2]) : 
                    new ATImportFileLocation(path, string.Empty, path);
        }

    }

}
