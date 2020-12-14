using System.ComponentModel;

namespace ARKBreedingStats.ocr.PatternMatching
{
    public class TrainingSettings
    {
        [DefaultValue(true)]
        public bool IsTrainingEnabled = true;
        public bool SkipName;
        public bool SkipTribe;
        public bool SkipOwner;
    }
}