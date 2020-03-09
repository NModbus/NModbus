using System;
using System.IO;
using NModbus.Data;
using NModbus.Message;
using Xunit;

namespace NModbus.UnitTests.Message
{
    public class ReadFileRecordResquestFixture
    {
        [Fact]
        public void Create()
        {
            var request = new WriteFileRecordRequest(17, new FileRecordCollection(1, 2, new byte[] { 4, 5 } ));
            Assert.Equal(ModbusFunctionCodes.WriteFileRecord, request.FunctionCode);
            Assert.Equal(17, request.SlaveAddress);
            Assert.Equal(1, request.Data.FileNumber);
            Assert.Equal(2, request.Data.StartingAddress);
            Assert.Equal(new byte[] { 4, 5 }, request.Data.DataBytes);
        }

        [Fact]
        public void Validate_ThrowsOnFileNumberMismatch()
        {
            var request = new WriteFileRecordRequest(17, new FileRecordCollection(1, 2, new byte[] { 4, 5 }));
            var response = new WriteFileRecordResponse(17, new FileRecordCollection(2, 2, new byte[] { 4, 5 }));
            Assert.Throws<IOException>(() => request.ValidateResponse(response));
        }

        [Fact]
        public void Validate_ThrowsOnStartingAddressMismatch()
        {
            var request = new WriteFileRecordRequest(17, new FileRecordCollection(1, 2, new byte[] { 4, 5 }));
            var response = new WriteFileRecordResponse(17, new FileRecordCollection(1, 4, new byte[] { 4, 5 }));
            Assert.Throws<IOException>(() => request.ValidateResponse(response));
        }

        [Fact]
        public void Initialize()
        {
            var response = new WriteFileRecordRequest();
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
            var request = new WriteFileRecordRequest(17, new FileRecordCollection(1, 2, new byte[] { 4, 5 }));

            Assert.Equal("Write 2 bytes for file 1 starting at address 2.", request.ToString());
        }
    }
}