using System;
using System.IO;
using GZipTest.Models;
using Xunit;

namespace GZipTest.Tests.FileWriter
{
    public class WriteTest
    {
        [Fact]
        public void Test()
        {
            // Arrange
            string filePath = AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0] + "/Data/WriteTest.txt";

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            DataPart[] writtenData = new DataPart[1]
            {
                new DataPart(new byte[12] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}, 0, 11)
            };

            // Act
            using (Services.FileWriter writer = new Services.FileWriter(filePath))
            {
                writer.Write(writtenData);
            }

            FileInfo file = new FileInfo(filePath);

            // Assert
            Assert.Equal(file.Length, writtenData[0].Data.Length);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
