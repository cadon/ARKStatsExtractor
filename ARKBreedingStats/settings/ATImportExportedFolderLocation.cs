using System;
using System.IO;
using System.Linq;
using Acornima;

namespace ARKBreedingStats.settings
{

    public class ATImportExportedFolderLocation
    {

        public string ConvenientName { get; set; }
        public string OwnerSuffix { get; set; }
        public string FolderPath { get; set; }
        public string[] SetDefaultForLibraryFiles { get; set; }

        public ATImportExportedFolderLocation() { }

        public ATImportExportedFolderLocation(string convenientName, string ownerSuffix, string folderPath, string setDefaultForLibraries = null)
        {
            ConvenientName = convenientName;
            OwnerSuffix = ownerSuffix;
            FolderPath = folderPath;
            if (!string.IsNullOrEmpty(setDefaultForLibraries))
            {
                var arr = setDefaultForLibraries.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim())
                     .Where(p => !string.IsNullOrEmpty(p)).ToArray();
                SetDefaultForLibraryFiles = arr.Any() ? arr : null;

            }
        }

        public static ATImportExportedFolderLocation CreateFromString(string path)
        {
            string[] pathParts = path.Split('|');

            switch (pathParts.Length)
            {
                case 1:
                    return new ATImportExportedFolderLocation(path, string.Empty, path);
                case 3:
                    return new ATImportExportedFolderLocation(pathParts[0], pathParts[1], pathParts[2]);
                case 4:
                    return new ATImportExportedFolderLocation(pathParts[0], pathParts[1], pathParts[2], pathParts[3]);
                default: return null;
            }
        }

        /// <summary>
        /// String representation of this export folder, used to save it.
        /// </summary>
        public override string ToString() => $"{ConvenientName}|{OwnerSuffix}|{FolderPath}|{(SetDefaultForLibraryFiles == null ? string.Empty : string.Join(";", SetDefaultForLibraryFiles))}";

        public override bool Equals(object obj) => obj is ATImportExportedFolderLocation ef && ef.ToString() == ToString();
        public override int GetHashCode() => ToString().GetHashCode();

        public static bool operator ==(ATImportExportedFolderLocation a, ATImportExportedFolderLocation b)
        {
            if (a is null)
                return b is null;

            return ReferenceEquals(a, b) || a.Equals(b);
        }

        public static bool operator !=(ATImportExportedFolderLocation a, ATImportExportedFolderLocation b) => !(a == b);

        /// <summary>
        /// Returns true if this export folder should be set as default for a library file path.
        /// Usually checked when a library file is loaded.
        /// </summary>
        public bool IsDefaultForLibraryFile(string libraryFilePath)
        {
            if (string.IsNullOrEmpty(libraryFilePath) || SetDefaultForLibraryFiles?.Any() != true) return false;
            var fileName = Path.GetFileName(libraryFilePath);
            return SetDefaultForLibraryFiles.Any(p => p == libraryFilePath || p == fileName);
        }
    }
}
