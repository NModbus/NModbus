using System;
using NModbus.Data;
using NModbus.Message;
using Xunit;

namespace NModbus.UnitTests.Message
{
    public class ReadFileRecordResponseFixture
    {
        [Fact]
        public void Create()
        {
            var response = new WriteFileRecordResponse(17);
            Assert.Equal(ModbusFunctionCodes.WriteFileRecord, response.FunctionCode);
            Assert.Equal(17, response.SlaveAddress);
        }

        [Fact]
        public void CreateWithData()
        {
            var response = new WriteFileRecordResponse(17, new FileRecordCollection(1, 2, new byte[] { 4, 5 }));
            Assert.Equal(ModbusFunctionCodes.WriteFileRecord, response.FunctionCode);
            Assert.Equal(17, response.SlaveAddress);
            Assert.Equal(1, response.Data.FileNumber);
            Assert.Equal(2, response.Data.StartingAddress);
            Assert.Equal(new byte[] { 4, 5 }, response.Data.DataBytes);
        }

        [Fact]
        public void Initialize()
        {
            var response = new WriteFileRecordResponse();
            response.Initialize(new byte[] {
                17, ModbusFunctionCodes.WriteFileRecord, 9, 6, 0, 1, 0, 2, 0, 1, 4, 5
            });

            Assert.Equal(ModbusFunctionCodes.WriteFileRecord, response.FunctionCode);
            Assert.Equal(17, response.SlaveAddress);
            Assert.Equal(1, response.Data.FileNumber);
            Assert.Equal(2, response.Data.StartingAddress);
            Assert.Equal(new byte[] { 4, 5 }, response.Data.DataBytes);
        }

        [Fact]
        public void ToString_Test()
        {
            var response = new WriteFileRecordResponse(17, new FileRecordCollection(1, 2, new byte[] { 4, 5 }));

            Assert.Equal("Wrote 2 bytes for file 1 starting at address 2.", response.ToString());
        }
    }
}