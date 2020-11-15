using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.settings
{
    public partial class customSoundChooser : UserControl
    {
        private string soundFile;
        private string title;
        private System.Media.SoundPlayer soundplayer;

        public customSoundChooser()
        {
            InitializeComponent();
        }

        private void buttonFileChooser_Click(object sender, EventArgs e)
        {
            using (var fileSelect = new OpenFileDialog
            {
                Filter = "Wav Files (wav)|*.wav"
            })
            {
                if (fileSelect.ShowDialog() == DialogResult.OK)
                    SoundFile = fileSelect.FileName;
                else if (MessageBox.Show("Set to default sound?", "To default?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    soundFile = "";
                    soundplayer = null;
                    Title = title;
                }
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (soundplayer == null) System.Media.SystemSounds.Hand.Play();
            else
            {
                try { soundplayer.Play(); }
                catch
                {
                    MessageBoxes.ShowMessageBox("Unsupported sound-format. Only PCM-WAV is supported.", $"Unsupported sound-format");
                }
            }
        }

        public string SoundFile
        {
            get => soundFile;
            set
            {
                if (value == null)
                {
                    soundFile = "";
                    soundplayer = null;
                }
                else
                {
                    List<string> exts = new List<string> { ".WAV" };//, ".MID", ".MIDI", ".WMA", ".MP3", ".OGG" };
                    if (File.Exists(value) && exts.IndexOf(Path.GetExtension(value).ToUpperInvariant()) >= 0)
                    {
                        soundFile = value;
                        soundplayer = new System.Media.SoundPlayer(soundFile);
                        Title = title;
                    }
                }
            }
        }

        public string Title
        {
            set
            {
                title = value;
                labelName.Text = title + (soundFile == "" ? "" : "(" + Path.GetFileName(soundFile) + ")");
            }
        }
    }
}
