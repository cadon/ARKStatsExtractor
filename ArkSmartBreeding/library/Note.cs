namespace ARKBreedingStats.Library
{
    /// <summary>
    /// A note with a title and text content.
    /// </summary>
    public class Note
    {
        public string? Title { get; set; }
        public string? Text { get; set; }

        public Note() { }

        public Note(string title)
        {
            Title = title;
        }
    }
}
