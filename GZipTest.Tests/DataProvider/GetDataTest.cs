using System;
using GZipTest.Models;
using Xunit;

namespace GZipTest.Tests.DataProvider
{
    public class GetDataTest
    {
        [Fact]
        public void Test()
        {
            // Arrange
            int workBlockSize = 1;

            int processorCount = Environment.ProcessorCount;

            string dataDirectory = AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0] + "/Data/sample-2mb-text-file.txt";

            DataPart[] initialBuffer = new DataPart[processorCount];

            // Act
            using (Services.DataProvider dataProvider = new Services.DataProvider(dataDirectory, workBlockSize, processorCount * workBlockSize))
            {

                long readBytesCount = dataProvider.GetData(initialBuffer);

                // Assert
                Assert.Equal(processorCount * workBlockSize, readBytesCount);
            }
        }
    }
}
