using System;
using GZipTest.Models;

namespace GZipTest.Interfaces
{
    /// <summary>
    /// Provide data from different source
    /// </summary>
    public interface IDataProvider : IDisposable
    {
        /// <summary>
        /// Fill the dictionary with parts of data from the source
        /// </summary>
        /// <param name="initialBuffer">Original data dictionary where key is an index number and DataPart <see cref="T:GZipTest.Models.DataPart" />"</param>
        /// <returns>Sum of bytes filled buffer dictionary</returns>
        long GetData(DataPart[]  initialBuffer);
    }
}
