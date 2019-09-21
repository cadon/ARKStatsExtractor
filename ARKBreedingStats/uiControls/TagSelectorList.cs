using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class TagSelectorList : UserControl
    {
        private readonly List<TagSelector> tagSelectors;
        private readonly List<string> tagStrings;
        public event TagSelector.TagStatusChangedEventHandler OnTagChanged;

        public TagSelectorList()
        {
            InitializeComponent();
            tagSelectors = new List<TagSelector>();
            tagStrings = new List<string>();
        }

        public List<string> tags
        {
            set
            {
                foreach (Control c in Controls)
                    c.Dispose();
                Controls.Clear();
                tagSelectors.Clear();
                int i = 0;
                foreach (string t in value)
                {
                    TagSelector ts = new TagSelector
                    {
                        TagName = t,
                        Location = new Point(3, 3 + i * 29),
                        Width = Width - 6
                    };
                    ts.OnTagChanged += OnTagChanged;
                    Controls.Add(ts);
                    tagSelectors.Add(ts);
                    tagStrings.Add(t);
                    i++;
                }
            }
        }

        public TagSelector.tagStatus TagStatus(string tag)
        {
            int i = tagStrings.IndexOf(tag);
            return i >= 0 ? tagSelectors[i].Status : TagSelector.tagStatus.indifferent;
        }

        public void setTagStatus(string tag, TagSelector.tagStatus status)
        {
            int i = tagStrings.IndexOf(tag);
            if (i >= 0)
                tagSelectors[i].Status = status;
        }

        public List<string> excludingTags
        {
            get
            {
                List<string> l = new List<string>();
                foreach (TagSelector ts in tagSelectors)
                    if (ts.Status == TagSelector.tagStatus.exclude)
                        l.Add(ts.TagName);
                return l;
            }
        }

        public List<string> includingTags
        {
            get
            {
                List<string> l = new List<string>();
                foreach (TagSelector ts in tagSelectors)
                    if (ts.Status == TagSelector.tagStatus.include)
                        l.Add(ts.TagName);
                return l;
            }
        }
    }
}
