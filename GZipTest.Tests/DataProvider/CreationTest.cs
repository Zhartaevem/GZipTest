using System;
using Xunit;

namespace GZipTest.Tests.DataProvider
{
    public class CreationTest
    {
        [Fact]
        public void Test()
        {
            // Arrange
            int workBlockSize = 1000000;

            int processorCount = Environment.ProcessorCount;

            string dataDirectory = AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0] + "/Data/sample-2mb-text-file.txt";

            // Act
            using (Services.DataProvider dataProvider = new Services.DataProvider(dataDirectory, workBlockSize, processorCount * workBlockSize))
            {
                // Assert
                Assert.IsType<Services.DataProvider>(dataProvider);
            }
        }
    }
}
