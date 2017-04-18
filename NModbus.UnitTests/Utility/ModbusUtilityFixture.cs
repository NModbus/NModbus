using System;
using Modbus.Message;
using Modbus.Utility;
using Xunit;

namespace Modbus.UnitTests.Utility
{
    public class ModbusUtilityFixture
    {
        [Fact]
        public void GetAsciiBytesFromEmpty()
        {
            Assert.Equal(new byte[] { }, ModbusUtility.GetAsciiBytes(new byte[] { }));
            Assert.Equal(new byte[] { }, ModbusUtility.GetAsciiBytes(new ushort[] { }));
        }

        [Fact]
        public void GetAsciiBytesFromBytes()
        {
            byte[] buf = { 2, 5 };
            byte[] expectedResult = { 48, 50, 48, 53 };
            byte[] result = ModbusUtility.GetAsciiBytes(buf);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void GetAsciiBytesFromUshorts()
        {
            ushort[] buf = { 300, 400 };
            byte[] expectedResult = { 48, 49, 50, 67, 48, 49, 57, 48 };
            byte[] result = ModbusUtility.GetAsciiBytes(buf);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void HexToBytes()
        {
            Assert.Equal(new byte[] { 255 }, ModbusUtility.HexToBytes("FF"));
        }

        [Fact]
        public void HexToBytes2()
        {
            Assert.Equal(new byte[] { 204, 255 }, ModbusUtility.HexToBytes("CCFF"));
        }

        [Fact]
        public void HexToBytesEmpty()
        {
            Assert.Equal(new byte[] { }, ModbusUtility.HexToBytes(string.Empty));
        }

        [Fact]
        public void HexToBytesNull()
        {
            Assert.Throws<ArgumentNullException>(() => ModbusUtility.HexToBytes(null));
        }

        [Fact]
        public void HexToBytesOdd()
        {
            Assert.Throws<FormatException>(() => ModbusUtility.HexToBytes("CCF"));
        }

        [Fact]
        public void CalculateCrc()
        {
            byte[] result = ModbusUtility.CalculateCrc(new byte[] { 1, 1 });
            Assert.Equal(new byte[] { 193, 224 }, result);
        }

        [Fact]
        public void CalculateCrc2()
        {
            byte[] result = ModbusUtility.CalculateCrc(new byte[] { 2, 1, 5, 0 });
            Assert.Equal(new byte[] { 83, 12 }, result);
        }

        [Fact]
        public void CalculateCrcEmpty()
        {
            Assert.Equal(new byte[] { 255, 255 }, ModbusUtility.CalculateCrc(new byte[] { }));
        }

        [Fact]
        public void CalculateCrcNull()
        {
            Assert.Throws<ArgumentNullException>(() => ModbusUtility.CalculateCrc(null));
        }

        [Fact]
        public void CalculateLrc()
        {
            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 10);
            Assert.Equal(243, ModbusUtility.CalculateLrc(new byte[] { 1, 1, 0, 1, 0, 10 }));
        }

        [Fact]
        public void CalculateLrc2()
        {
            // : 02 01 0000 0001 FC
            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 2, 0, 1);
            Assert.Equal(252, ModbusUtility.CalculateLrc(new byte[] { 2, 1, 0, 0, 0, 1 }));
        }

        [Fact]
        public void CalculateLrcNull()
        {
            Assert.Throws<ArgumentNullException>(() => ModbusUtility.CalculateLrc(null));
        }

        [Fact]
        public void CalculateLrcEmpty()
        {
            Assert.Equal(0, ModbusUtility.CalculateLrc(new byte[] { }));
        }

        [Fact]
        public void NetworkBytesToHostUInt16()
        {
            Assert.Equal(new ushort[] { 1, 2 }, ModbusUtility.NetworkBytesToHostUInt16(new byte[] { 0, 1, 0, 2 }));
        }

        [Fact]
        public void NetworkBytesToHostUInt16Null()
        {
            Assert.Throws<ArgumentNullException>(() => ModbusUtility.NetworkBytesToHostUInt16(null));
        }

        [Fact]
        public void NetworkBytesToHostUInt16OddNumberOfBytes()
        {
            Assert.Throws<FormatException>(() => ModbusUtility.NetworkBytesToHostUInt16(new byte[] { 1 }));
        }

        [Fact]
        public void NetworkBytesToHostUInt16EmptyBytes()
        {
            Assert.Equal(new ushort[] { }, ModbusUtility.NetworkBytesToHostUInt16(new byte[] { }));
        }

        [Fact]
        public void GetDouble()
        {
            Assert.Equal(0.0, ModbusUtility.GetDouble(0, 0, 0, 0));
            Assert.Equal(1.0, ModbusUtility.GetDouble(16368, 0, 0, 0));
            Assert.Equal(Math.PI, ModbusUtility.GetDouble(16393, 8699, 21572, 11544));
            Assert.Equal(500.625, ModbusUtility.GetDouble(16511, 18944, 0, 0));
        }

        [Fact]
        public void GetSingle()
        {
            Assert.Equal(0F, ModbusUtility.GetSingle(0, 0));
            Assert.Equal(1F, ModbusUtility.GetSingle(16256, 0));
            Assert.Equal(9999999F, ModbusUtility.GetSingle(19224, 38527));
            Assert.Equal(500.625F, ModbusUtility.GetSingle(17402, 20480));
        }

        [Fact]
        public void GetUInt32()
        {
            Assert.Equal((uint)0, ModbusUtility.GetUInt32(0, 0));
            Assert.Equal((uint)1, ModbusUtility.GetUInt32(0, 1));
            Assert.Equal((uint)45, ModbusUtility.GetUInt32(0, 45));
            Assert.Equal((uint)65536, ModbusUtility.GetUInt32(1, 0));
        }
    }
}