using System;

namespace GZipTest.Interfaces
{
    public interface IExecution : ICancelling, IDisposable
    {
        void Execute();
    }
}
