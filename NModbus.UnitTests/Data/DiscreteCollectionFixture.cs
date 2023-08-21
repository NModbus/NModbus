using System;
using System.Linq;
using NModbus.Data;
using Xunit;

namespace NModbus.UnitTests.Data
{
    public class DiscreteCollectionFixture
    {
        [Fact]
        public void ByteCount()
        {
            DiscreteCollection col = new DiscreteCollection(true, true, false, false, false, false, false, false, false);
            Assert.Equal(2, col.ByteCount);
        }

        [Fact]
        public void ByteCountEven()
        {
            DiscreteCollection col = new DiscreteCollection(true, true, false, false, false, false, false, false);
            Assert.Equal(1, col.ByteCount);
        }

        [Fact]
        public void NetworkBytes()
        {
            DiscreteCollection col = new DiscreteCollection(true, true);
            Assert.Equal(new byte[] { 3 }, col.NetworkBytes);
        }

        [Fact]
        public void CreateNewDiscreteCollectionInitialize()
        {
            DiscreteCollection col = new DiscreteCollection(true, true, true);
            Assert.Equal(3, col.Count);
            Assert.DoesNotContain(false, col);
        }

        [Fact]
        public void CreateNewDiscreteCollectionFromBoolParams()
        {
            DiscreteCollection col = new DiscreteCollection(true, false, true);
            Assert.Equal(3, col.Count);
        }

        [Fact]
        public void CreateNewDiscreteCollectionFromBytesParams()
        {
            DiscreteCollection col = new DiscreteCollection(1, 2, 3);
            Assert.Equal(24, col.Count);
            var expected = new bool[]
            {
                true, false, false, false, false, false, false, false,
                false, true, false, false, false, false, false, false,
                true, true, false, false, false, false, false, false,
            };

            Assert.Equal(expected, col);
        }

        [Fact]
        public void CreateNewDiscreteCollectionFromBytesParams_ZeroLengthArray()
        {
            DiscreteCollection col = new DiscreteCollection(new byte[0]);
            Assert.Empty(col);
        }

        [Fact]
        public void CreateNewDiscreteCollectionFromBytesParams_NullArray()
        {
            Assert.Throws<ArgumentNullException>(() => new DiscreteCollection((byte[])null));
        }

        [Fact]
        public void CreateNewDiscreteCollectionFromBytesParamsOrder()
        {
            DiscreteCollection col = new DiscreteCollection(194);
            Assert.Equal(new bool[] { false, true, false, false, false, false, true, true }, col.ToArray());
        }

        [Fact]
        public void CreateNewDiscreteCollectionFromBytesParamsOrder2()
        {
            DiscreteCollection col = new DiscreteCollection(157, 7);
            Assert.Equal(
                new bool[]
                { true, false, true, true, true, false, false, true, true, true, true, false, false, false, false, false },
                col.ToArray());
        }

        [Fact]
        public void Resize()
        {
            DiscreteCollection col = new DiscreteCollection(byte.MaxValue, byte.MaxValue);
            Assert.Equal(16, col.Count);
            col.RemoveAt(3);
            Assert.Equal(15, col.Count);
        }

        [Fact]
        public void BytesPersistence()
        {
            DiscreteCollection col = new DiscreteCollection(byte.MaxValue, byte.MaxValue);
            Assert.Equal(16, col.Count);
            byte[] originalBytes = col.NetworkBytes;
            col.RemoveAt(3);
            Assert.Equal(15, col.Count);
            Assert.NotEqual(originalBytes, col.NetworkBytes);
        }

        [Fact]
        public void AddCoil()
        {
            DiscreteCollection col = new DiscreteCollection();
            Assert.Empty(col);
            col.Add(true);
            Assert.Single(col);
        }
    }
}