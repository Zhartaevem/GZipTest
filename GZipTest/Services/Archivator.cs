using System;
using System.Linq;
using GZipTest.AbstractClasses;
using GZipTest.Models;

namespace GZipTest.Services
{
    public class Archivator : ArchivatorAbstract
    {
        public Archivator(string fileName, string destinationFileName, ArchiveActionModel mode)
        {
            if (mode == ArchiveActionModel.Compress)
            {
                this._archivatorWork = new Archivate(fileName, destinationFileName);
            }
            else
            {
                this._archivatorWork = new Extract(fileName, destinationFileName);
            }
        }


        /// <summary>
        /// Archive data from file
        /// </summary>
        public override void Archive()
        {
            try
            {
                _archivatorWork.Execute();
            }
            catch (System.ObjectDisposedException e)
            {
            }
            catch (AggregateException e)
            {
                if (e.InnerExceptions != null && e.InnerExceptions.Any(i => typeof(System.Threading.Tasks.TaskCanceledException) != i.GetType()))
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }


        /// <summary>
        /// Extract data from file
        /// </summary>
        public override void Extract()
        {
            try
            {
                this._archivatorWork.Execute();
            }
            catch (System.ObjectDisposedException e)
            {
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
