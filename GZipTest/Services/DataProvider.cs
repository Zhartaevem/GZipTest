using System.IO;
using System.Threading;
using GZipTest.Interfaces;
using GZipTest.Models;

namespace GZipTest.Services
{
    /// <summary>
    ///  Provide data from file
    /// </summary>
    public class DataProvider : IDataProvider
    {
        /// <summary>
        /// FileStream from initial file
        /// </summary>
        private FileStream _reader { get; set; }

        /// <summary>
        /// Max size of part of the data into which all data is divided
        /// </summary>
        private int _workBlockSize { get; set; }

        /// <summary>
        /// Inner size of data buffer which decrease in reading data process
        /// </summary>
        private int _bufferSize { get; set; }

        /// <summary>
        /// Max size read data
        /// </summary>
        private int _maxBufferSize { get; set; }

        /// <summary>
        /// Count of bytes which need to provide
        /// </summary>
        private long _notReadSpace { get; set; }

        /// <summary>
        /// Real size of part of the data which will be read in work block 
        /// </summary>
        private long _readData { get; set; }

        private readonly object _lockObject = new object();

        /// <summary>
        /// Provide data from file
        /// </summary>
        /// <param name="fileName">The file name will read data from there</param>
        /// <param name="workBlockSize">Max size of work block data</param>
        /// <param name="maxBufferSize">Max size read data</param>
        public DataProvider(string fileName, int workBlockSize, int maxBufferSize)
        {
            FileInfo file = new FileInfo(fileName);

            this._reader = file.OpenRead();

            this._notReadSpace = file.Length;

            this._workBlockSize = workBlockSize;

            this._maxBufferSize = maxBufferSize;

            this.ResetCounterProperties();
        }


        /// <summary>
        /// Get data from source file
        /// </summary>
        /// <param name="initialBuffer">The Dictionary will read data from there. Where key is index read data and DataPart <see cref="T:GZipTest.Models.DataPart" /></param>
        /// <returns>>Sum of bytes filled buffer dictionary</returns>
        public long GetData(DataPart[] initialBuffer)
        {
            int dataIndex = 0;

            this.ResetCounterProperties();

            while (_bufferSize > 0 && _notReadSpace > 0)
            {
                int realBufferSize = _workBlockSize;

                if (_notReadSpace < _workBlockSize)
                {
                    realBufferSize = (int)_notReadSpace;
                }

                DataPart buffer;

                lock (_lockObject)
                {
                    if (_reader is null)
                    {
                        break;
                    }

                    buffer = new DataPart(new byte[realBufferSize], _reader.Position, _reader.Position + realBufferSize);

                    _reader?.Read(buffer.Data, 0, realBufferSize);
                }

                //buffer.EndPosition = _reader.Position - 1;

                initialBuffer[dataIndex] = buffer;

                _notReadSpace -= realBufferSize;
                _bufferSize -= realBufferSize;
                _readData += realBufferSize;

                dataIndex++;
            }

            if (dataIndex != initialBuffer.Length)
            {
                for (int i = dataIndex; i < initialBuffer.Length; i++)
                {
                    initialBuffer[i] = new DataPart(new byte[0], 0, 0);
                }
            }


            return _readData;
        }


        /// <summary>
        /// Reset properties to initial values
        /// </summary>
        private void ResetCounterProperties()
        {
            this._bufferSize = _maxBufferSize;

            this._readData = 0;
        }


        public void Dispose()
        {
            lock (_lockObject)
            {
                _reader?.Close();
                _reader = null;
            }
        }
    }
}
