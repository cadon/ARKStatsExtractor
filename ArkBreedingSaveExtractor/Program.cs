using System;
using System.IO;
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
            Console.WriteLine("ASB Server file Extractor");
            var opts = CliParser.StrictParse<Options>(args);

            try
            {
                new Program(opts).Run();
            }
            catch (ApplicationException ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
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
                catch
                {
                    throw new ApplicationException("Unable to load the supplied library file.");
                }
            }

            // Import from the savegame
            try
            {
                Msg("Reading save file...");
                Task.WaitAll(saveExtractor.Run(cc));
                Msg("Complete.");
            }
            catch
            {
                throw new ApplicationException("Unable to load the ark file.");
            }

            // Save the library
            try
            {
                Msg("Saving updated library.");
                libHandler.SaveLibrary(cc, opts.LibraryFile);
            }
            catch
            {
                throw new ApplicationException("Unable to save the library file.");
            }

            Msg("Update complete.");
        }

        private void Msg(string msg)
        {
            if (!opts.Quiet) Console.WriteLine(msg);
        }
    }
}
