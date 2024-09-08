using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ARKBreedingStats.Library;
using ARKBreedingStats.utils;
using Jint;
using static ARKBreedingStats.Library.CreatureCollection;

namespace ARKBreedingStats.NamePatterns
{
    internal static class JavaScriptNamePattern
    {

        public static Regex JavaScriptShebang = new Regex(@"^\#!javascript\s*?\n", RegexOptions.IgnoreCase);
        private static readonly string FlattenScript = BuildModelFlattenScript();

        public static string ResolveJavaScript(string pattern, Creature creature, TokenModel tokenModel, Dictionary<string, string> customReplacings, ColorExisting[] colorsExisting, string[] creatureNames, bool displayError, Action<string> consoleLog)
        {
            var stopwatch = Stopwatch.StartNew();
            var log = consoleLog ?? (s => { });
            string numberedUniqueName;
            string lastNumberedUniqueName = null;

            var n = 1;

            do
            {
                if (n > 1)
                {
                    log($">> Name not unique. Repeating with model. n = {n}");
                }

                try
                {
                    using (var engine = new Engine(options => options.TimeoutInterval(TimeSpan.FromSeconds(4))))
                    {
                        engine.SetValue("model", tokenModel);
                        engine.Execute(FlattenScript);

                        engine.SetValue("n", n);
                        engine.SetValue("log", log);
                        engine.SetValue<Func<string, string, string>>("customReplace", (key, defaultValue) => NamePatternFunctions.CustomReplace(key, defaultValue, customReplacings));
                        engine.SetValue<Func<int, string, string>>("listName", NameList.GetName);

                        numberedUniqueName = engine.Evaluate(pattern).ToString();

                        // check if numberedUniqueName actually is different, else break the potentially infinite loop. E.g. it is not different if {n} is an unreached if branch or was altered with other functions
                        if (numberedUniqueName == lastNumberedUniqueName)
                            break;

                        lastNumberedUniqueName = numberedUniqueName;
                        n++;
                    }
                }
                catch (Exception ex)
                {
                    if (displayError)
                    {
                        MessageBoxes.ShowMessageBox($"The naming script generated an exception\n\nSpecific error:\n{ex.Message}", "Naming script error");
                        return null;
                    }

                    log($">> ERROR: {ex.Message}");
                    return ex.Message;
                }
            } while (creatureNames?.Contains(numberedUniqueName, StringComparer.OrdinalIgnoreCase) == true);

            log($">> Name resolved in {stopwatch.Elapsed.TotalMilliseconds} ms");

            return numberedUniqueName;
        }

        private static string BuildModelFlattenScript()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Object.assign(globalThis, model)");

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                var abbreviation = NamePattern.StatAbbreviationFromIndex[s];
                builder.AppendLine($"{abbreviation}_m = {abbreviation}.level_m");
                builder.AppendLine($"{abbreviation}_vb = {abbreviation}.level_vb");
                builder.AppendLine($"istop{abbreviation} = {abbreviation}.istop");
                builder.AppendLine($"isnewtop{abbreviation} = {abbreviation}.isnewtop");
                builder.AppendLine($"islowest{abbreviation} = {abbreviation}.islowest");
                builder.AppendLine($"isnewlowest{abbreviation} = {abbreviation}.isnewlowest");
                builder.AppendLine($"istop{abbreviation}_m = {abbreviation}.istop_m");
                builder.AppendLine($"isnewtop{abbreviation}_m = {abbreviation}.isnewtop_m");
                builder.AppendLine($"islowest{abbreviation}_m = {abbreviation}.islowest_m");
                builder.AppendLine($"isnewlowest{abbreviation}_m = {abbreviation}.isnewlowest_m");
                builder.AppendLine($"{abbreviation} = {abbreviation}.level");
            }

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                builder.AppendLine($"highest{s + 1}l = highest_l[{s}]");
                builder.AppendLine($"highest{s + 1}s = highest_s[{s}]");
                builder.AppendLine($"highest{s + 1}l_m = highest_l_m[{s}]");
                builder.AppendLine($"highest{s + 1}s_m = highest_s_m[{s}]");
            }

            builder.AppendLine("delete model");
            builder.AppendLine("delete highest_l");
            builder.AppendLine("delete highest_s");
            builder.AppendLine("delete highest_s_m");

            var script = builder.ToString();
            return script;
        }
    }
}