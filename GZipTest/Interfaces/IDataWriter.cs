using System;
using GZipTest.Models;

namespace GZipTest.Interfaces
{
    /// <summary>
    /// Provide interface for writing data
    /// </summary>
    public interface IDataWriter : IDisposable
    {
        /// <summary>
        /// Write data from Dictionary <see cref="T:GZipTest.Models.DataPart" />
        /// </summary>
        /// <param name="data"></param>
        void Write(DataPart[] data);
    }
}
