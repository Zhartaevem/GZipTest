using System;
using System.Collections.Generic;
using System.Text;

namespace GZipTest.Interfaces
{

    public interface IExecution : ICancelling, IDisposable
    {
        void Execute();
    }
}
