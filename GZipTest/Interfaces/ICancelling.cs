using System;
using System.Collections.Generic;
using System.Text;

namespace GZipTest.Interfaces
{
    /// <summary>
    /// Interface for providing cancel operation
    /// </summary>
    public interface ICancelling
    {
        /// <summary>
        /// Cancel operation
        /// </summary>
        void Cancel();
    }
}
