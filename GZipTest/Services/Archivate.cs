using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using GZipTest.Interfaces;
using GZipTest.Models;
using Microsoft.Win32.SafeHandles;

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

        private ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        private EventWaitHandle[] autoResetEvents;


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

                foreach (var autoResetEvent in this.autoResetEvents)
                {
                    autoResetEvent?.Dispose();
                }

                this.manualResetEvent?.Dispose();
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

            //use thread safe dictionary as buffer for reading data and archiving data
            DataPart[] initialBuffer = new DataPart[processorCount];

            this._writer = new FileWriter(_destinationFileName);

            Thread[] threads = new Thread[processorCount];

            autoResetEvents = new EventWaitHandle[processorCount];

            //read data from file while it has unread data
            while (_provider.GetData(initialBuffer) > 0)
            {
                if (!_threadsAreRun)
                {
                    CreateAndRunThreads(threads, initialBuffer);
                }

                //permit to work
                manualResetEvent.Set();

                //Wait all archiving tasks
                WaitHandle.WaitAll(autoResetEvents);

                //Write all archived data to file
                _writer.Write(initialBuffer);

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

                autoResetEvents[currentIndex] = new AutoResetEvent(false);

                threads[currentIndex] = new Thread(() =>
                    {
                        int currentIndex1 = currentIndex;

                        while (initialBuffer[currentIndex1].Data.Length > 0)
                        {
                            //reuse collection for saving archived data
                            initialBuffer[currentIndex1].Data = Compress(initialBuffer[currentIndex1]);

                            //end of work signaling to autoResetEvents
                            autoResetEvents[currentIndex1].Set();

                            //end of work signaling to manualResetEvent
                            manualResetEvent.Reset();

                            //wait then manualResetEvent permit to work
                            manualResetEvent.WaitOne();
                        }
                    })
                    { IsBackground = true };

                threads[currentIndex].Start();
            }

            autoResetEvents = autoResetEvents.Where(are => are != null).ToArray();

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
