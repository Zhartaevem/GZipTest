using System;
using System.IO;
using GZipTest.Models;
using Xunit;

namespace GZipTest.Tests.Archivator
{
    public class CompressionTest
    {
        [Fact]
        public void Test()
        {
            // Arrange
            string initialDirectory = AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0] + "/Data/sample-2mb-text-file.txt";

            string destinationDirectory = AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0] + "/Data/CompressionTest";

            if (File.Exists(destinationDirectory + ".gz"))
            {
                File.Delete(destinationDirectory + ".gz");
            }

            FileInfo sampleFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0] + "/Data/sample-2mb-text-file.gz");

            using (Services.Archivator archivator = new Services.Archivator(initialDirectory, destinationDirectory, ArchiveActionModel.Compress))
            {
                // Act
                archivator.Archive();

                // Assert
                FileInfo destinationFile = new FileInfo(destinationDirectory + ".gz");

                Assert.Matches(sampleFile.Length.ToString(), destinationFile.Length.ToString());
            }


            if (File.Exists(destinationDirectory + ".gz"))
            {
                File.Delete(destinationDirectory + ".gz");
            }
        }
    }
}
