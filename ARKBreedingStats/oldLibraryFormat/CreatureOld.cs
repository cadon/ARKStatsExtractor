using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ARKBreedingStats.oldLibraryFormat
{
    [Serializable]
    [XmlType(TypeName = "Creature")]
    public class CreatureOld : IEquatable<CreatureOld>
    {
        public string speciesBlueprint;
        [XmlIgnore]
        private Species _species;
        public string species; // speciesName
        public string name;
        public Sex sex;
        public CreatureStatus status;
        public CreatureFlags flags;
        public int[] levelsWild;
        public int[] levelsDom;
        public double tamingEff;
        public double imprintingBonus;
        public string owner;
        public string imprinterName;
        public string tribe;
        public string server;
        public string note; // user defined note about that creature
        public Guid guid; // the id used in ASB for parent-linking. The user cannot change it
        public long ArkId; // the creature's id in ARK. This id is shown to the user ingame, but it's not always unique. (It's build from two int, which are concatenated as strings).
        public bool ArkIdImported; // if true it's assumed the ArkId is correct (ingame visualization can be wrong). This field should only be true if the ArkId was imported.
        public bool isBred;
        public Guid fatherGuid;
        public Guid motherGuid;
        public int generation; // number of generations from the oldest wild creature
        public int[] colors = { 0, 0, 0, 0, 0, 0 }; // id of colors
        public DateTime growingUntil;
        [XmlIgnore]
        public TimeSpan growingLeft;
        public bool growingPaused;
        public DateTime cooldownUntil;
        public DateTime domesticatedAt;
        public DateTime addedToLibrary;
        public bool neutered = false;
        public int mutationsMaternal;
        public int mutationsPaternal;
        public List<string> tags = new List<string>();
        public bool IsPlaceholder; // if a creature has unknown parents, they are placeholders until they are imported. placeholders are not shown in the library

        public bool Equals(CreatureOld other)
        {
            return other.guid == guid;
        }

        [XmlIgnore]
        public Species Species
        {
            set
            {
                _species = value;
                speciesBlueprint = value?.blueprintPath ?? string.Empty;
            }
            get { return _species; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return obj is CreatureOld creatureObj && Equals(creatureObj);
        }

        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }

        public override string ToString()
        {
            return name + " (" + _species.name + ")";
        }

        // XmlSerializer does not support TimeSpan, so use this property for serialization instead.
        [System.ComponentModel.Browsable(false)]
        [XmlElement(DataType = "duration", ElementName = "growingLeft")]
        public string growingLeftString
        {
            get
            {
                return System.Xml.XmlConvert.ToString(growingLeft);
            }
            set
            {
                growingLeft = string.IsNullOrEmpty(value) ?
                    TimeSpan.Zero : System.Xml.XmlConvert.ToTimeSpan(value);
            }
        }
    }
}