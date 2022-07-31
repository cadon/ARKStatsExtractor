namespace ARKBreedingStats.settings
{

    public class ATImportExportedFolderLocation
    {

        public string ConvenientName { get; set; }
        public string OwnerSuffix { get; set; }
        public string FolderPath { get; set; }

        public ATImportExportedFolderLocation() { }

        public ATImportExportedFolderLocation(string convenientName, string ownerSuffix, string folderPath)
        {
            ConvenientName = convenientName;
            OwnerSuffix = ownerSuffix;
            FolderPath = folderPath;
        }

        public static ATImportExportedFolderLocation CreateFromString(string path)
        {
            string[] pathParts = path.Split('|');

            return pathParts.Length == 3 ?
                    new ATImportExportedFolderLocation(pathParts[0], pathParts[1], pathParts[2]) :
                    new ATImportExportedFolderLocation(path, string.Empty, path);
        }

        /// <summary>
        /// String representation of this export folder, used to save it.
        /// </summary>
        public override string ToString() => $"{ConvenientName}|{OwnerSuffix}|{FolderPath}";
    }
}
