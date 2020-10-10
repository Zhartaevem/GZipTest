using System;
using System.IO;
using System.IO.Compression;
using GZipTest.Validation;

namespace GZipTest
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelEventHandler);

            try
            {
                Console.ReadKey();

                args = new string[3];
                //args[0] = @"compress";
                //args[1] = @"F:\Copressing\IMG_20180907_214752.jpg";
                //args[2] = @"F:\Copressing\IMG_20180907_214752";

                args[0] = @"decompress";
                args[1] = @"F:\Copressing\IMG_20180907_214752.gz";
                args[2] = @"F:\Copressing\IMGaga.jpg";

                InputValidator validator = new InputValidator();

                if (!validator.Validate(args))
                {
                    foreach (var validatorError in validator.Errors)
                    {
                        Console.WriteLine(validatorError);
                    }

                    return 1;
                }

                switch (args[0].ToLower())
                {
                    case "compress":
                        Compress(args[1], args[2]);
                        break;
                    case "decompress":
                        Decompress(args[1], args[2]);
                        break;
                }

                return 0;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error is occured!\n Method: {0}\n Error description {1}", ex.TargetSite, ex.Message);
                return 1;
            }

        }

        protected static void CancelEventHandler(object sender, ConsoleCancelEventArgs args)
        {
            if (args.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                Console.WriteLine("\nCancelling... Please wait");
                args.Cancel = true;
                //код который корректно завершает работу компрессора
            }
        }

        protected static void Compress(string fileName, string destinationFileName)
        {
            FileInfo file = new FileInfo(fileName);

            using (FileStream originalFileStream = file.OpenRead())
            {
                if ((File.GetAttributes(file.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & file.Extension != ".gz")
                {
                    using (FileStream compressedFileStream = File.Create(destinationFileName + ".gz"))
                    {
                        using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                        {
                            originalFileStream.CopyTo(compressionStream);
                        }
                    }
                    FileInfo info = new FileInfo(destinationFileName + ".gz");
                    Console.WriteLine($"Compressed {file.Name} from {file.Length.ToString()} to {info.Length.ToString()} bytes.");
                }
            }
        }

        protected static void Decompress(string fileName, string destinationFileName)
        {
            FileInfo file = new FileInfo(fileName);

            using (FileStream originalFileStream = file.OpenRead())
            {
                using (FileStream decompressedFileStream = File.Create(destinationFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        Console.WriteLine($"Decompressed: {fileName}");
                    }
                }
            }
        }
    }
}
