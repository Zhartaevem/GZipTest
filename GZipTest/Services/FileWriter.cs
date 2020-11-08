using System.IO;
using GZipTest.Interfaces;
using GZipTest.Models;

namespace GZipTest.Services
{
    /// <summary>
    /// Data writer to file
    /// </summary>
    public class FileWriter : IDataWriter
    {
        /// <summary>
        /// Destination filestream opened for writing
        /// </summary>
        private FileStream _fileStream { get; set; }

        private readonly object _lockObject = new object();

        /// <summary>
        /// Data writer to file
        /// </summary>
        /// <param name="destinationFileName">Destination path</param>
        public FileWriter(string destinationFileName)
        {
            this._fileStream = File.Exists(destinationFileName) ? File.OpenWrite(destinationFileName) : File.Create(destinationFileName);
        }


        /// <summary>
        /// Write data in the specified order 
        /// </summary>
        /// <param name="data">Dictionary where key is index number and DataPart <see cref="T:GZipTest.Models.DataPart" /></param>
        public void Write(DataPart[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    return;
                }

                lock (_lockObject)
                {
                    if (_fileStream is null)
                    {
                        break;
                    }

                    _fileStream?.Write(data[i].Data);
                }
            }
        }


        public void Dispose()
        {
            lock (_lockObject)
            {
                this._fileStream?.Close();
                _fileStream = null;
            }
        }
    }
}
