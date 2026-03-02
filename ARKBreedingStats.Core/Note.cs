namespace ARKBreedingStats.Core
{
    /// <summary>
    /// A note with a title and text content.
    /// </summary>
    public class Note
    {
        public string? Title;
        public string? Text;

        public Note() { }

        public Note(string title)
        {
            Title = title;
        }
    }
}
