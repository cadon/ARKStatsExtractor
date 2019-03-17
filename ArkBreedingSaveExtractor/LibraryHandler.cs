using System.IO;
using System.Xml.Serialization;

using ARKBreedingStats;


namespace ArkBreedingSaveExtractor
{
    class LibraryHandler
    {
        public static CreatureCollection Empty => new CreatureCollection();

        public CreatureCollection LoadLibrary(string fileName)
        {
            CreatureCollection creatureCollection;

            var reader = new XmlSerializer(typeof(CreatureCollection));

            using (var file = File.OpenRead(fileName))
                creatureCollection = (CreatureCollection)reader.Deserialize(file);

            return creatureCollection;
        }

        public void SaveLibrary(CreatureCollection cc, string fileName)
        {
            var writer = new XmlSerializer(typeof(CreatureCollection));
            using (var file = File.Create(fileName))
                writer.Serialize(file, cc);
        }
    }
}
