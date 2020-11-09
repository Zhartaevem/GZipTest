using System;
using System.Collections.Generic;
using System.Diagnostics;
using GZipTest.Interfaces;
using GZipTest.Models;
using GZipTest.Services;
using GZipTest.Validation;

namespace GZipTest
{
    class Program
    {
        private static IList<ICancelling> CancellingObjects { get; set; } = new List<ICancelling>();


        static int Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelEventHandler);

            try
            {

#if DEBUG
                Console.ReadKey();

                args = new string[3];

                args[0] = @"compress";
                args[1] = @"F:\Copressing\expansion2-speech-ruRU.MPQ";
                args[2] = @"F:\Copressing\Break";

                //args[0] = @"compress";
                //args[1] = @"F:\WOW\WoWCircle 4.3.4\Data\world.MPQ";
                //args[2] = @"F:\Copressing\Break";

                //args[0] = @"compress";
                //args[1] = @"G:\Movies\Breaking Bad (2008-2013) BDRip 1080p [KvK]\Breaking Bad - Season 5\5x01 - Live Free or Die.mkv";
                //args[2] = @"F:\Copressing\Break";

                //args[0] = @"decompress";
                //args[1] = @"F:\Copressing\Break.gz";
                //args[2] = @"F:\Copressing\Breaking Bad.mkv";
#endif

                InputValidator validator = new InputValidator();

                if (!validator.Validate(args))
                {
                    foreach (var validatorError in validator.Errors)
                    {
                        Console.WriteLine(validatorError);
                    }

                    Console.ReadKey();

                    return 1;
                }

                Console.WriteLine("Start process");

#if DEBUG

                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();

#endif
                switch (args[0].ToLower())
                {
                    case "compress":
                        using (Archivator archivator = new Archivator(args[1], args[2], ArchiveActionModel.Compress))
                        {
                            CancellingObjects.Add((ICancelling)archivator);
                            archivator.Archive();
                        }
                        break;
                    case "decompress":
                        using (Archivator archivator = new Archivator(args[1], args[2], ArchiveActionModel.Decompress))
                        {
                            CancellingObjects.Add((ICancelling)archivator);
                            archivator.Extract();
                        }
                        break;
                }

#if DEBUG
                stopwatch.Stop();

                Console.WriteLine($"Time {stopwatch.ElapsedMilliseconds} millisecond.");

                Console.ReadKey();
#endif

                Console.WriteLine("Finish process");

                return 0;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error is occured!\n Method: {0}\n Error description {1}", ex.TargetSite, ex.Message);

                Console.ReadKey();

                return 1;
            }
        }


        protected static void CancelEventHandler(object sender, ConsoleCancelEventArgs args)
        {
            if (args.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                Console.WriteLine("\nCancelling... Please wait");

                foreach (var cancellingObject in CancellingObjects)
                {
                    try
                    {
                        cancellingObject.Cancel();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error is occured!\n Method: {0}\n Error description {1}", e.TargetSite, e.Message);
                    }
                }

                args.Cancel = true;

                Console.WriteLine("\nCanceled");
            }
        }
    }
}
