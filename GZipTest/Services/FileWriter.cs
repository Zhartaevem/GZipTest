using System.Collections.Generic;
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

        private readonly object _writingLockObject = new object();

        private int _currentWritingIndex { get; set; }

        private Dictionary<int, bool> _writingIndexes;

        /// <summary>
        /// Data writer to file
        /// </summary>
        /// <param name="destinationFileName">Destination path</param>
        public FileWriter(string destinationFileName, int indexesCount)
        {
            this._fileStream = File.Exists(destinationFileName) ? File.OpenWrite(destinationFileName) : File.Create(destinationFileName);

            this._writingIndexes = new Dictionary<int, bool>();

            for (int i = 0; i < indexesCount; i++)
            {
                this._writingIndexes.Add(i, false);
            }
        }


        /// <summary>
        /// Write data in the specified order 
        /// </summary>
        /// <param name="data">Dictionary where key is index number and DataPart <see cref="T:GZipTest.Models.DataPart" /></param>
        /// <param name="index">index of data which can be write <see cref="T:GZipTest.Models.DataPart" /></param>
        public void Write(DataPart[] data, int index)
        {
            lock (_writingLockObject)
            {
                if (_currentWritingIndex == data.Length)
                {
                    _currentWritingIndex = 0;
                }

                this._writingIndexes[index] = true;

                //if (index != _currentWritingIndex)
                //{
                //    return;
                //}

                for (int i = _currentWritingIndex; i < data.Length; i++)
                {
                    if (data[i] == null || !this._writingIndexes[i])
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

                    this._writingIndexes[i] = false;

                    this._currentWritingIndex++;
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
