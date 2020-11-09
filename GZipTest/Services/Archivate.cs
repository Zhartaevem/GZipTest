using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using GZipTest.Interfaces;
using GZipTest.Models;

namespace GZipTest.Services
{
    /// <summary>
    /// Get, archive and write data to destination
    /// </summary>
    public class Archivate : IExecution
    {

        /// <summary>
        /// Data provider for archiving
        /// </summary>
        private IDataProvider _provider { get; set; }

        /// <summary>
        /// Destination file data writer
        /// </summary>
        private IDataWriter _writer { get; set; }

        /// <summary>
        /// Path to destination file
        /// </summary>
        private string _destinationFileName { get; set; }

        /// <summary>
        /// Path to initial file
        /// </summary>
        private string _initialFileName { get; set; }

        private bool _disposed = false;

        private bool _threadsAreRun = false;

        private ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        private EventWaitHandle[] _autoResetEvents;

        private Thread[] _threads;

        private readonly object _lockObject = new object();

        /// <summary>
        /// Constructor for <see cref="T:GZipTest.Services.Archivate" />
        /// </summary>
        /// <param name="fileName">initial file path</param>
        /// <param name="destinationFileName">destination file path</param>
        public Archivate(string fileName, string destinationFileName)
        {
            this._destinationFileName = destinationFileName.EndsWith(".gz") ? destinationFileName : destinationFileName + ".gz";

            this._initialFileName = fileName;
        }


        /// <summary>
        /// Cancel all works in each threads
        /// </summary>
        public void Cancel()
        {
            this.Dispose();

            if (File.Exists(_destinationFileName))
            {
                File.Delete(_destinationFileName);
            }
        }


        public void Dispose()
        {
            Dispose(true);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                this._provider?.Dispose();

                this._writer?.Dispose();

                lock (_lockObject)
                {
                    foreach (var autoResetEvent in this._autoResetEvents)
                    {
                        autoResetEvent?.Dispose();
                    }
                }

                lock (_lockObject) this._manualResetEvent?.Dispose();

            }

            _disposed = true;
        }


        /// <summary>
        /// Start execution of archiving
        /// </summary>
        public void Execute()
        {
            int workBlockSize = 1000000;

            int processorCount = Environment.ProcessorCount;

            this._provider = new DataProvider(_initialFileName, workBlockSize, workBlockSize * processorCount);

            DataPart[] initialBuffer = new DataPart[processorCount];

            this._writer = new FileWriter(_destinationFileName, processorCount);

            this._threads = new Thread[processorCount];

            _autoResetEvents = new EventWaitHandle[processorCount];

            //read data from file while it has unread data
            while (_provider != null && _provider.GetData(initialBuffer) > 0)
            {
                if (!_threadsAreRun)
                {
                    CreateAndRunThreads(_threads, initialBuffer);
                }

                //permit to work
                lock (_lockObject) _manualResetEvent?.Set();

                //Wait all archiving tasks
                WaitHandle.WaitAll(_autoResetEvents);
            }
        }


        private void CreateAndRunThreads(Thread[] threads, DataPart[] initialBuffer)
        {
            for (int i = 0; i < initialBuffer.Length; i++)
            {
                if (initialBuffer[i] is null)
                {
                    break;
                }

                int currentIndex = i;

                _autoResetEvents[currentIndex] = new AutoResetEvent(false);

                threads[currentIndex] = new Thread(() =>
                    {
                        int innerIndex = currentIndex;

                        //while (initialBuffer[innerIndex].Data.Length > 0)
                        while (initialBuffer[innerIndex] != null)
                        {
                            //reuse collection for saving archived data
                            initialBuffer[innerIndex].Data = Compress(initialBuffer[innerIndex]);

                            //Write all archived data to file
                            _writer.Write(initialBuffer, innerIndex);

                            //end of work signaling to autoResetEvents
                            _autoResetEvents[innerIndex]?.Set();

                            //end of work signaling to manualResetEvent
                            lock (_lockObject)
                            {
                                if (_manualResetEvent.SafeWaitHandle.IsClosed)
                                {
                                    break;
                                }

                                _manualResetEvent?.Reset();
                            }

                            //wait then manualResetEvent permit to work
                            _manualResetEvent?.WaitOne();
                        }

                        //end of work signaling to autoResetEvents
                        _autoResetEvents[innerIndex]?.Set();
                    })
                    { IsBackground = true };

                threads[currentIndex].Start();
            }

            _autoResetEvents = _autoResetEvents.Where(are => are != null).ToArray();

            _threadsAreRun = true;
        }


        private byte[] Compress(DataPart dataPart)
        {
            if (dataPart == null)
            {
                return new byte[0];
            }

            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream compressionStream = new GZipStream(stream, CompressionMode.Compress))
                {
                    compressionStream.Write(dataPart.Data);
                }

                return stream.ToArray();
            }
        }
    }
}
