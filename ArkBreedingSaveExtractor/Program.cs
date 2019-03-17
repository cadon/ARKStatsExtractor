using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using clipr;

using ARKBreedingStats;

namespace ArkBreedingSaveExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Command-line args
            var opts = CliParser.StrictParse<Options>(args);

            try
            {
                new Program(opts).Run();
            }
            catch (ApplicationException ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
                if (opts.ShowExceptions)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine(ex.ToString());
                    Console.Error.WriteLine();
                }
                Console.Error.WriteLine("Aborted.");
                Environment.Exit(1);
            }
        }

        private readonly Options opts;

        private Program(Options opts)
        {
            this.opts = opts;
        }

        private void Run()
        {
            Msg("ASB Server file Extractor");

            // Some safety checks
            if (opts.ArkFile.ToLower().EndsWith(".xml"))
                throw new ApplicationException("Refusing to read from an Ark save file with a .xml extension!");

            if (opts.LibraryFile.ToLower().EndsWith(".ark"))
                throw new ApplicationException("Refusing to write to a library file with a .ark extension!");

            // Initialise sub-components
            var libHandler = new LibraryHandler();
            var saveExtractor = new SaveExtractor()
            {
                ArkFile = opts.ArkFile,
                ServerName = opts.ServerName,
                TribeFilter = opts.TribeFilter,
                UpdateStatus = !opts.DontUpdateStatus,
            };

            // Load core values.json
            try
            {
                Msg("Loading values.json.");
                Values.V.loadValues(passExceptions:true);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to load values.json", ex);
            }

            // Load mod support values
            if (opts.ModList.Count > 0)
            {
                foreach (var mod in opts.ModList)
                {
                    Msg($"Loading values for {mod}.");
                    var modFile = FileService.GetJsonPath(mod + ".json");
                    try
                    {
                        Values.V.loadAdditionalValues(modFile, false, passExceptions: true);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException($"Unable to load values for {mod}", ex);
                    }
                }
            }

            // Load or create the library
            CreatureCollection cc;
            if (!File.Exists(opts.LibraryFile))
            {
                if (!opts.CreateLibrary)
                    throw new ApplicationException("A library file must be specified or --create must be included.");

                Msg("Creating new library.");
                cc = LibraryHandler.Empty;
            }
            else
            {
                try
                {
                    Msg("Loading library.");
                    cc = libHandler.LoadLibrary(opts.LibraryFile);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to load the supplied library file.", ex);
                }
            }

            // Import from the savegame
            try
            {
                Msg("Reading save file...");
                Task.WaitAll(saveExtractor.Run(cc));
                Msg("Complete.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to load the ark file: "+ex.Message, ex);
            }

            // Save the library
            try
            {
                Msg("Saving updated library.");
                libHandler.SaveLibrary(cc, opts.LibraryFile);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to save the library file.", ex);
            }

            Msg("Update complete.");
        }

        private void Msg(string msg)
        {
            if (!opts.Quiet) Console.WriteLine(msg);
        }
    }
}
