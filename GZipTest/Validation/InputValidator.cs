using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GZipTest.Validation
{
    public sealed class InputValidator
    {
        public IList<string> Errors { get; set; }

        public InputValidator()
        {
            this.Errors = new List<string>();
        }

        public bool Validate(string[] inputArgs)
        {
            if (inputArgs.Length != 3)
            {
                this.Errors.Add("Arguments pattern error. Use the following pattern:\n compress(decompress) [Source file] [Destination file].");
                return false;
            }

            if (inputArgs[0].ToUpper() != "COMPRESS" && inputArgs[0].ToUpper() != "DECOMPRESS")
            {
                this.Errors.Add("First argument error. Use \"compress\" or \"decompress\" for first argument.");
            }

            if (!File.Exists(inputArgs[1]))
            {
                this.Errors.Add("Source file error. Source file not found.");
            }

            if (inputArgs[1] == inputArgs[2])
            {
                this.Errors.Add("Files error. Source and destination files must be different.");
            }

            FileInfo sourceFile = new FileInfo(inputArgs[1]);

            if (sourceFile.Extension == ".gz" && inputArgs[0] == "compress")
            {
                this.Errors.Add("Source file error. Source file has already been compressed.");
            }

            if (sourceFile.Extension != ".gz" && inputArgs[0] == "decompress")
            {
                this.Errors.Add("Source file error. Source file must has .gz extension.");
            }

            FileInfo destinationFile = new FileInfo(inputArgs[2]);

            if (destinationFile.Exists)
            {
                this.Errors.Add("Destination file error. Destination file exist.");
            }

            return !Errors.Any();
        }
    }
}
