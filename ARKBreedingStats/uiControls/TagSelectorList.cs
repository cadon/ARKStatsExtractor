using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class TagSelectorList : UserControl
    {
        private List<TagSelector> tagSelectors;
        private List<string> tagStrings;
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
                TagSelector ts;
                int i = 0;
                foreach (string t in value)
                {
                    ts = new TagSelector();
                    ts.TagName = t;
                    ts.Location = new Point(3, 3 + i * 29);
                    ts.Width = Width - 6;
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
            if (i >= 0)
                return tagSelectors[i].Status;
            return TagSelector.tagStatus.indifferent;
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
