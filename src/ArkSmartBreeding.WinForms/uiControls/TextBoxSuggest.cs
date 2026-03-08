using System;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public class TextBoxSuggest : TextBox
    {
        public TextBoxSuggest()
        {
            Enter += TextBoxSuggest_Enter;
        }

        private void TextBoxSuggest_Enter(object sender, EventArgs e)
        {
            BeginInvoke((Action)SelectAll);
        }
    }
}
