using System;
using Xunit;

namespace GZipTest.Tests.FileWriter
{
    public class CreationTest
    {
        [Fact]
        public void Test()
        {
            // Arrange
            string filePath = AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0] + "/Data/CreationTest.txt";

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Act
            using (Services.FileWriter writer = new Services.FileWriter(filePath))
            {
                // Assert
                Assert.IsType<Services.FileWriter>(writer);
            }

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
