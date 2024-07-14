using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;

using Jint;

using static ARKBreedingStats.Library.CreatureCollection;

namespace ARKBreedingStats.NamePatterns
{
    internal static class JavaScriptNamePattern
    {

        public static Regex JavaScriptShebang = new Regex(@"^\#!javascript\s*?\n", RegexOptions.IgnoreCase);

        public static string ResolveJavaScript(string pattern, Creature creature, TokenModel tokenModel, Dictionary<string, string> customReplacings, ColorExisting[] colorsExisting, string[] creatureNames, bool displayError, Action<string> consoleLog)
        {
            using (var engine = new Engine(options =>
            {
                options.TimeoutInterval(TimeSpan.FromSeconds(4));
            }))
            {
                var log = consoleLog ?? ((s) => { });

                try
                {
                    engine.SetValue("c", tokenModel);
                    engine.SetValue("log", log);
                    engine.SetValue<Func<string, string, string>>("customReplace", (key, defaultValue) => NamePatternFunctions.CustomReplace(key, defaultValue, customReplacings));
                    engine.SetValue<Func<int, string, string>>("listName", NameList.GetName);
                    engine.Execute(pattern);

                    string numberedUniqueName;
                    string lastNumberedUniqueName = null;

                    var n = 1;
                    do
                    {
                        if (n > 1)
                        {
                            log($">> Name not unique. Repeating with model.n = {n}");
                        }

                        engine.Execute($"c.n = {n}");

                        numberedUniqueName = engine.Evaluate("nameCreature()").ToString();

                        // check if numberedUniqueName actually is different, else break the potentially infinite loop. E.g. it is not different if {n} is an unreached if branch or was altered with other functions
                        if (numberedUniqueName == lastNumberedUniqueName) break;

                        lastNumberedUniqueName = numberedUniqueName;
                        n++;
                    } while (creatureNames?.Contains(numberedUniqueName, StringComparer.OrdinalIgnoreCase) == true);

                    return numberedUniqueName;
                }
                catch (Exception ex)
                {
                    if (displayError)
                    {
                        MessageBoxes.ShowMessageBox($"The naming script generated an exception\n\nSpecific error:\n{ex.Message}", $"Naming script error");
                        return null;
                    }

                    log($">> ERROR: {ex.Message}");
                    return ex.Message;
                }
            }
        }
    }
}