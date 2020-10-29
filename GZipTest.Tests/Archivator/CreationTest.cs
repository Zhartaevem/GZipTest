using System;
using GZipTest.Models;
using Xunit;

namespace GZipTest.Tests.Archivator
{
    public class CreationTest
    {
        [Fact]
        public void TestCompressMode()
        {
            // Arrange
            string initialDirectory = AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0] + "/Data/sample-2mb-text-file.txt";

            string destinationDirectory = AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0] + "/Data/sample-2mb-text-file";

            // Act
            using (Services.Archivator archivator = new Services.Archivator(initialDirectory, destinationDirectory, ArchiveActionModel.Compress))
            {
                // Assert
                Assert.IsType<Services.Archivator>(archivator);
            }
        }

        [Fact]
        public void TestDecompressMode()
        {
            // Arrange
            string initialDirectory = AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0] + "/Data/sample-2mb-text-file";

            string destinationDirectory = AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0] + "/Data/sampleExtract.txt";

            // Act
            using (Services.Archivator archivator = new Services.Archivator(initialDirectory, destinationDirectory, ArchiveActionModel.Decompress))
            {
                // Assert
                Assert.IsType<Services.Archivator>(archivator);
            }
        }
    }
}
