using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ARKBreedingStats.utils;

namespace ARKBreedingStats
{
    public class SpeechRecognition
    {
        public Action<string, int> SpeechCreatureRecognized;
        public Action<Commands> SpeechCommandRecognized;
        private readonly SpeechRecognitionEngine _recognizer;
        private readonly Label _indicator;
        private bool _listening;
        private int _maxLevel;
        private int _levelStep;
        private int _aliasesCount;
        public bool Initialized { get; set; }

        public SpeechRecognition(int maxLevel, int levelStep, List<string> aliases, Label indicator)
        {
            Initialized = false;
            if (!aliases.Any()) return;
            _indicator = indicator;
            _recognizer = new SpeechRecognitionEngine();
            SetMaxLevelAndSpecies(maxLevel, levelStep, aliases);
            _recognizer.SpeechRecognized += Sre_SpeechRecognized;
            try
            {
                _recognizer.SetInputToDefaultAudioDevice();
                Initialized = true;
            }
            catch
            {
                MessageBoxes.ShowMessageBox("Couldn't set Audio-Input to default-audio device. The speech recognition will not work until a restart.\nTry to change the default-audio-input (e.g. plug-in a microphone).",
                    "Microphone Error");
            }
            _recognizer.SpeechRecognitionRejected += Recognizer_SpeechRecognitionRejected;
        }

        private void Recognizer_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Blink(Color.Orange);
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Blink(Color.Green);
            //if (e.Result.Grammar == recognizer.Grammars[0])
            //{
            // taming info
            Regex rg = new Regex(@"(\w+)(?: level)? (\d+)");
            Match m = rg.Match(e.Result.Text);
            if (m.Success)
            {
                string species = m.Groups[1].Value;
                if (int.TryParse(m.Groups[2].Value, out int level))
                    SpeechCreatureRecognized?.Invoke(species, level);
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

        private void Blink(Color c)
        {
            Utils.BlinkAsync(_indicator, c);
        }

        private Grammar CreateTamingGrammar(int maxLevel, int levelSteps, List<string> aliases, CultureInfo culture)
        {
            Choices speciesChoice = new Choices(aliases.ToArray());
            GrammarBuilder tamingElement = new GrammarBuilder(speciesChoice)
            {
                Culture = culture
            };

            if (levelSteps < 1) levelSteps = 1;
            int levelCount = (int)Math.Ceiling((double)maxLevel / levelSteps);
            Choices levelsChoice = new Choices(Enumerable.Range(1, levelCount).Select(i => (i * levelSteps).ToString()).ToArray());
            GrammarBuilder levelElement = new GrammarBuilder(levelsChoice);

            tamingElement.Append("level", 0, 1);
            tamingElement.Append(levelElement);

            // Create choices for the combinations
            Grammar grammar = new Grammar(tamingElement);
            return grammar;
        }

        public bool Listen
        {
            set
            {
                _listening = value;
                if (_listening)
                {
                    try
                    {
                        _recognizer.RecognizeAsync(RecognizeMode.Multiple);
                        _indicator.ForeColor = Color.Red;
                    }
                    catch
                    {
                        MessageBoxes.ShowMessageBox("Couldn't set Audio-Input to default-audio device. The speech recognition will not work until a restart.\nTry to change the default-audio-input (e.g. plug-in a microphone).",
                            "Microphone Error");
                        _listening = false;
                        _indicator.ForeColor = SystemColors.GrayText;
                    }
                }
                else
                {
                    _recognizer.RecognizeAsyncStop();
                    _indicator.ForeColor = SystemColors.GrayText;
                }
            }
        }

        /// <summary>
        /// Initializes the speech recognition with possible commands.
        /// </summary>
        /// <param name="maxLevel"></param>
        /// <param name="levelStep"></param>
        /// <param name="aliases"></param>
        public void SetMaxLevelAndSpecies(int maxLevel, int levelStep, List<string> aliases)
        {
            if (maxLevel == _maxLevel && levelStep == _levelStep && _aliasesCount == aliases.Count) return;
            _maxLevel = maxLevel;
            _levelStep = levelStep;
            _aliasesCount = aliases.Count;
            _recognizer.UnloadAllGrammars();
            _recognizer.LoadGrammar(CreateTamingGrammar(maxLevel, levelStep, aliases, _recognizer.RecognizerInfo.Culture));
            //recognizer.LoadGrammar(CreateCommandsGrammar()); // remove for now, it's too easy to say something that is recognized as "extract" and disturbes the play-flow
        }

        //private Grammar CreateCommandsGrammar()
        //{
        //    // currently not used, appears to execute falsely too often.
        //    Choices commands = new Choices("extract");
        //    GrammarBuilder commandsElement = new GrammarBuilder(commands);
        //
        //    Grammar grammar = new Grammar(commandsElement);
        //    return grammar;
        //}

        public void ToggleListening()
        {
            Listen = !_listening;
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
                _recognizer.Dispose();
            }
        }
    }
}
