using System;
using GZipTest.Interfaces;

namespace GZipTest.AbstractClasses
{
    public abstract class ArchivatorAbstract : IDisposable, ICancelling
    {
        protected IExecution _archivatorWork { get; set; }


        /// <summary>
        /// Archive data
        /// </summary>
        public virtual void Archive()
        {
            this._archivatorWork.Execute();
        }

        /// <summary>
        /// Extract archived data
        /// </summary>
        public virtual void Extract()
        {
            this._archivatorWork.Execute();
        }

        public void Dispose()
        {
            _archivatorWork?.Dispose();
        }

        /// <summary>
        /// Cancel operation
        /// </summary>
        public void Cancel()
        {
            _archivatorWork.Cancel();
        }
    }
}
