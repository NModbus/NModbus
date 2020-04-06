using NModbus.Data;
using System;
using Xunit;

namespace NModbus.UnitTests.Data
{
    public class FileRecordCollectionFixture
    {
        [Fact]
        public void Constructor_ThrowsOddByteCount()
        {
            Assert.Throws<FormatException>(() => new FileRecordCollection(1, 2, new byte[] { 1, 2, 3 }));
        }

        [Fact]
        public void ByteCount()
        {
            var col = new FileRecordCollection(1, 2, new byte[] { 1, 2, 3, 4 });
            Assert.Equal(11, col.ByteCount);
        }

        [Fact]
        public void FileNumber()
        {
            var col = new FileRecordCollection(1, 2, new byte[] { 1, 2, 3, 4 });
            Assert.Equal(1, col.FileNumber);
        }

        [Fact]
        public void StartingAdress()
        {
            var col = new FileRecordCollection(1, 2, new byte[] { 1, 2, 3, 4 });
            Assert.Equal(2, col.StartingAddress);
        }

        [Fact]
        public void NetworkBytes()
        {
            var col = new FileRecordCollection(1, 3, new byte[] { 1, 2, 3,4  });
            Assert.Equal(new byte[] { 6, 0, 1, 0, 3, 0, 2, 1, 2, 3, 4 }, col.NetworkBytes);
        }
    }
}
