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

            Console.ReadKey();

            try
            {
                args = new string[3];
                args[0] = @"compress";
                args[1] = @"F:\Copressing\IMG_20180907_214752.jpg";
                args[2] = @"F:\Copressing\IMG_20180907_214752";

                //args[0] = @"decompress";
                //args[1] = @"F:\Copressing\IMG_20180907_214752.gz";
                //args[2] = @"F:\Copressing\Clever.bak";

                //args[0] = @"compress";
                //args[1] = @"F:\Copressing\cleverPark_backup_2020_09_08_233003_9708773.bak";
                //args[2] = @"F:\Copressing\IMG_20180907_214752";

                //args[0] = @"compress";
                //args[1] = @"F:\Copressing\IMG_20180907_214752.jpg";
                //args[2] = @"F:\Copressing\photo";

                //args[0] = @"compress";
                //args[1] = @"G:\Movies\LOTR\The.Lord.of.the.Rings-The Fellowship.of.the.Ring.2001.Blu-ray.Extended.1080p.x264-NiP.Rus.Eng.mkv";
                //args[2] = @"F:\Copressing\Lotr";

                //args[0] = @"compress";
                //args[1] = @"G:\Movies\Breaking Bad (2008-2013) BDRip 1080p [KvK]\Breaking Bad - Season 5\5x01 - Live Free or Die.mkv";
                //args[2] = @"F:\Copressing\Break";

                //args[0] = @"compress";
                //args[1] = "F:\\TestProjects\\GZipTest\\GZipTest.Tests\\/Data/sample-2mb-text-file.txt";
                //args[2] = "F:\\TestProjects\\GZipTest\\GZipTest.Tests\\/Data/sample-2mb-text-file";

                //args[0] = @"decompress";
                //args[1] = @"F:\Copressing\Break.gz";
                //args[2] = @"F:\Copressing\Breaking Bad.mkv";


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

                var stopwatcj = new Stopwatch();

                stopwatcj.Start();

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

                //НУжно убрать
                stopwatcj.Stop();

                Console.WriteLine($"Time {stopwatcj.ElapsedMilliseconds} millisecond.");

                Console.ReadKey();

                return 0;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error is occured!\n Method: {0}\n Error description {1}", ex.TargetSite, ex.Message);

                //НУжно убрать
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

                Console.ReadKey();
            }
        }
    }
}
