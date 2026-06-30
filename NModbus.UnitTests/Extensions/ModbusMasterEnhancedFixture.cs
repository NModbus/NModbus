using System;
using Moq;
using NModbus.Extensions;
using NModbus.Extensions.Functions;
using Xunit;

namespace NModbus.UnitTests.Extensions
{
    /// <summary>
    /// Regression coverage for issue #38: <see cref="ModbusMasterEnhanced.ReadIntHoldingRegisters"/>
    /// (and the other Read*HoldingRegisters helpers) crashed with a confusing
    /// "Destination array is not long enough" exception when the configured
    /// <c>wordSize</c> was smaller than the requested numeric type.
    /// </summary>
    public class ModbusMasterEnhancedFixture
    {
        private static Mock<IModbusMaster> BuildMaster(ushort[] response)
        {
            var master = new Mock<IModbusMaster>(MockBehavior.Strict);
            master
                .Setup(m => m.ReadHoldingRegisters(It.IsAny<byte>(), It.IsAny<ushort>(), It.IsAny<ushort>()))
                .Returns(response);
            return master;
        }

        [Fact]
        public void ReadIntHoldingRegisters_WordSize16_ThrowsClearArgumentException()
        {
            // Arrange: wordSize=16 means a "value" is a single 16-bit register,
            // which is too small to hold a 32-bit int. The library should reject
            // the call with an actionable error rather than crashing inside
            // BitConverter with "Destination array is not long enough" (issue #38).
            var master = BuildMaster(new ushort[] { 0x1234 });
            var enhanced = new ModbusMasterEnhanced(master.Object, wordSize: 16);

            // Act + Assert
            var ex = Assert.Throws<ArgumentException>(() => enhanced.ReadIntHoldingRegisters(1, 10, 1));
            Assert.Contains("wordSize", ex.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("32", ex.Message);
        }

        [Fact]
        public void ReadUintHoldingRegisters_WordSize16_ThrowsClearArgumentException()
        {
            var master = BuildMaster(new ushort[] { 0x1234 });
            var enhanced = new ModbusMasterEnhanced(master.Object, wordSize: 16);

            var ex = Assert.Throws<ArgumentException>(() => enhanced.ReadUintHoldingRegisters(1, 10, 1));
            Assert.Contains("wordSize", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ReadFloatHoldingRegisters_WordSize16_ThrowsClearArgumentException()
        {
            var master = BuildMaster(new ushort[] { 0x1234 });
            var enhanced = new ModbusMasterEnhanced(master.Object, wordSize: 16);

            var ex = Assert.Throws<ArgumentException>(() => enhanced.ReadFloatHoldingRegisters(1, 10, 1));
            Assert.Contains("wordSize", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ReadShortHoldingRegisters_WordSize16_StillWorks()
        {
            // 16-bit values fit in a 16-bit word, so this should succeed and not regress.
            var master = BuildMaster(new ushort[] { 0x1234 });
            var enhanced = new ModbusMasterEnhanced(master.Object, wordSize: 16);

            short[] result = enhanced.ReadShortHoldingRegisters(1, 10, 1);
            Assert.Single(result);
            Assert.Equal(unchecked((short)0x1234), result[0]);
        }

        [Fact]
        public void ReadIntHoldingRegisters_WordSize32_StillWorks()
        {
            // Sanity check that the wordSize=32 path is unchanged by the validation guard.
            // We don't pin the exact byte order here, that's already covered by the
            // existing extension tests and depends on host endianness.
            var master = BuildMaster(new ushort[] { 0x1234, 0x5678 });
            var enhanced = new ModbusMasterEnhanced(master.Object, wordSize: 32);

            int[] result = enhanced.ReadIntHoldingRegisters(1, 10, 1);
            Assert.Single(result);
        }

        [Fact]
        public void ByteValueArraysToInts_FrontPaddingWithSmallArray_ThrowsClearArgumentException()
        {
            // Direct coverage on the low-level helper: passing 2-byte arrays
            // would previously throw ArgumentOutOfRangeException from BitConverter
            // with "index" name and a confusing "Destination array is not long enough"
            // chain. We now throw ArgumentException up front with a clearer message.
            byte[][] tooSmall = { new byte[] { 0x12, 0x34 } };

            var ex = Assert.Throws<ArgumentException>(() => RegisterFunctions.ByteValueArraysToInts(tooSmall));
            Assert.Contains("4 bytes", ex.Message);
        }
    }
}
