using System;
using System.IO;
using GZipTest.Models;
using Xunit;

namespace GZipTest.Tests.Archivator
{
    public class ExtractionTest
    {
        [Fact]
        public void Test()
        {
            // Arrange
            string initialDirectory = AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0] + "/Data/sample-2mb-text-file.gz";

            string destinationDirectory = AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0] + "/Data/DecompressionTest.txt";

            if (File.Exists(destinationDirectory))
            {
                File.Delete(destinationDirectory);
            }

            FileInfo sampleFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0] + "/Data/sample-2mb-text-file.txt");

            using (Services.Archivator archivator = new Services.Archivator(initialDirectory, destinationDirectory, ArchiveActionModel.Decompress))
            {
                // Act
                archivator.Extract();

                // Assert
                FileInfo destinationFile = new FileInfo(destinationDirectory);

                Assert.Matches(sampleFile.Length.ToString(), destinationFile.Length.ToString());
            }

            if (File.Exists(destinationDirectory))
            {
                File.Delete(destinationDirectory);
            }
        }
    }
}
