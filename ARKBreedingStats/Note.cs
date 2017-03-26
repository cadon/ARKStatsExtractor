using System;

namespace ARKBreedingStats
{
    [Serializable()]
    public class Note
    {
        public string Title;
        public string Text;

        public Note() { }

        public Note(string title)
        {
            Title = title;
        }
    }
}
