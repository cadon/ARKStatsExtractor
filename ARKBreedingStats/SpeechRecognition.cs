using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    class SpeechRecognition
    {
        public delegate void SpeechRecognizedEventHandler(string species, int level);
        public SpeechRecognizedEventHandler speechRecognized;
        public delegate void SpeechCommandRecognizedEventHandler(Commands command);
        public SpeechCommandRecognizedEventHandler speechCommandRecognized;
        private SpeechRecognitionEngine recognizer;
        public Label indicator;
        private bool listening;
        public bool updateNeeded;
        private int MaxLevel, LevelStep;

        public SpeechRecognition(int maxLevel, int levelStep, List<string> aliases, Label indicator)
        {
            if (Values.V.speciesNames.Count > 0)
            {
                this.indicator = indicator;
                recognizer = new SpeechRecognitionEngine();
                updateNeeded = true;
                setMaxLevelAndSpecies(maxLevel, levelStep, aliases);
                recognizer.SpeechRecognized += sre_SpeechRecognized;
                try
                {
                    recognizer.SetInputToDefaultAudioDevice();
                }
                catch
                {
                    MessageBox.Show("Couldn't set Audio-Input to default-audio device. The speech recognition will not work until a restart.\nTry to change the default-audio-input (e.g. plug-in a microphone).",
               "Microphone Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                recognizer.SpeechRecognitionRejected += Recognizer_SpeechRecognitionRejected;
            }
        }

        private void Recognizer_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            blink(Color.Orange);
        }

        private async void blink(Color c)
        {
            Color original = indicator.ForeColor;
            indicator.ForeColor = c;
            await Task.Delay(500);
            indicator.ForeColor = original;

        }

        private void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            blink(Color.Green);
            //if (e.Result.Grammar == recognizer.Grammars[0])
            //{
            // taming info
            Regex rg = new Regex(@"(\w+)(?: level)? (\d+)");
            Match m = rg.Match(e.Result.Text);
            if (m.Success)
            {
                string species = m.Groups[1].Value;
                int level;
                if (int.TryParse(m.Groups[2].Value, out level))
                    speechRecognized?.Invoke(species, level);
            }
            /*}
            else
            {
                // commands
                switch (e.Result.Text)
                {
                    case "extract": speechCommandRecognized?.Invoke(Commands.Extract); break;
                }
            }*/
        }

        private Grammar CreateTamingGrammar(int maxLevel, int levelSteps, List<string> aliases)
        {
            Choices speciesChoice = new Choices(aliases.ToArray());
            GrammarBuilder tamingElement = new GrammarBuilder(speciesChoice);

            if (levelSteps < 1) levelSteps = 1;
            int levelCount = (int)Math.Ceiling((double)maxLevel / levelSteps);
            Choices levelsChoice = new Choices(Enumerable.Range(1, levelCount).Select(i => (i * levelSteps).ToString()).ToArray());
            GrammarBuilder levelElement = new GrammarBuilder(levelsChoice);

            tamingElement.Append("level", 0, 1);
            tamingElement.Append(levelElement);

            // Create choices for the combinations
            Grammar grammar = new Grammar(tamingElement);
            updateNeeded = false;
            return grammar;
        }

        public bool Listen
        {
            set
            {
                listening = value;
                if (listening)
                {
                    try
                    {
                        recognizer.RecognizeAsync(RecognizeMode.Multiple);
                        indicator.ForeColor = Color.Red;
                    }
                    catch
                    {
                        MessageBox.Show("Couldn't set Audio-Input to default-audio device. The speech recognition will not work until a restart.\nTry to change the default-audio-input (e.g. plug-in a microphone).",
                   "Microphone Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        listening = false;
                        indicator.ForeColor = SystemColors.GrayText;
                    }
                }
                else
                {
                    recognizer.RecognizeAsyncStop();
                    indicator.ForeColor = SystemColors.GrayText;
                }
            }
        }

        public void setMaxLevelAndSpecies(int maxLevel, int levelStep, List<string> aliases)
        {
            if (updateNeeded || maxLevel != MaxLevel || levelStep != LevelStep)
            {
                MaxLevel = maxLevel;
                LevelStep = levelStep;
                recognizer.UnloadAllGrammars();
                recognizer.LoadGrammar(CreateTamingGrammar(maxLevel, levelStep, aliases));
                //recognizer.LoadGrammar(createCommandsGrammar()); // remove for now, it's too easy to say something that is recognized as "extract" and disturbes the play-flow
                updateNeeded = false;
            }
        }

        private Grammar createCommandsGrammar()
        {
            Choices commands = new Choices(new string[] { "extract" });
            GrammarBuilder commandsElement = new GrammarBuilder(commands);

            Grammar grammar = new Grammar(commandsElement);
            return grammar;
        }

        public void toggleListening()
        {
            Listen = !listening;
        }

        public enum Commands
        {
            TamingInfo,
            Extract
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                recognizer.Dispose();
            }
        }
    }
}
