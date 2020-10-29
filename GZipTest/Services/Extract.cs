using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using GZipTest.Interfaces;
using Microsoft.Win32.SafeHandles;

namespace GZipTest.Services
{
    /// <summary>
    /// Get, archive and write data to destination
    /// </summary>
    public class Extract : IExecution
    {
        /// <summary>
        /// Path to destination file
        /// </summary>
        private string _destinationFileName { get; set; }

        /// <summary>
        /// Path to initial file
        /// </summary>
        private string _initialFileName { get; set; }

        /// <summary>
        /// File stream from initial file
        /// </summary>
        private FileStream _originalFileStream { get; set; }

        /// <summary>
        /// File stream from destination file
        /// </summary>
        private FileStream _destinationFileStream { get; set; }

        /// <summary>
        /// File stream with decompressed data
        /// </summary>
        private GZipStream _decompressionStream { get; set; }

        private bool _disposed = false;

        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);


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
                handle.Dispose();

                this._decompressionStream?.Dispose();

                this._destinationFileStream?.Dispose();

                this._originalFileStream?.Dispose();
            }

            _disposed = true;
        }


        public Extract(string fileName, string destinationFileName)
        {
            this._destinationFileName = destinationFileName;

            this._initialFileName = fileName;
        }


        public void Cancel()
        {
            this.Dispose();

            if (File.Exists(_destinationFileName))
            {
                File.Delete(_destinationFileName);
            }
        }


        public void Execute()
        {
            FileInfo file = new FileInfo(_initialFileName);

            this._originalFileStream = file.OpenRead();

            this._destinationFileStream = File.Create(_destinationFileName);

            this._decompressionStream = new GZipStream(_originalFileStream, CompressionMode.Decompress);

            this._decompressionStream.CopyTo(_destinationFileStream);
        }
    }
}
