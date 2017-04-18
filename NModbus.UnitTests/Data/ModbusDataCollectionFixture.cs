using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Modbus.Data;
using Xunit;

namespace Modbus.UnitTests.Data
{
    public abstract class ModbusDataCollectionFixture<TData>
    {
        [Fact]
        public void DefaultContstructor()
        {
            var col = new ModbusDataCollection<TData>();
            Assert.NotEmpty(col);
            Assert.Equal(1, col.Count);

            col.Add(default(TData));
            Assert.Equal(2, col.Count);
        }

        [Fact]
        public void ContstructorWithParams()
        {
            TData[] source = GetArray();
            var col = new ModbusDataCollection<TData>(source);
            Assert.Equal(source.Length + 1, col.Count);
            Assert.NotEmpty(col);

            col.Add(default(TData));
            Assert.Equal(source.Length + 2, col.Count);
        }

        [Fact]
        public void ContstructorWithIList()
        {
            List<TData> source = GetList();
            int expectedCount = source.Count;

            var col = new ModbusDataCollection<TData>(source);

            Assert.Equal(expectedCount + 1, source.Count);
            Assert.Equal(expectedCount + 1, col.Count);

            source.Insert(0, default(TData));
            Assert.Equal(source, col);
        }

        [Fact]
        public void ContstructorWithIList_FromReadOnlyList()
        {
            List<TData> source = GetList();
            var readOnly = new ReadOnlyCollection<TData>(source);
            int expectedCount = source.Count;

            var col = new ModbusDataCollection<TData>(readOnly);

            Assert.Equal(expectedCount, source.Count);
            Assert.Equal(expectedCount + 1, col.Count);

            source.Insert(0, default(TData));
            Assert.Equal(source, col);
        }

        [Fact]
        public void SetZeroElementUsingItem()
        {
            var source = GetArray();
            var col = new ModbusDataCollection<TData>(source);
            Assert.Throws<ArgumentOutOfRangeException>(() => col[0] = source[3]);
        }

        [Fact]
        public void ZeroElementUsingItem_Negative()
        {
            var source = GetArray();
            var col = new ModbusDataCollection<TData>(source);

            Assert.Throws<ArgumentOutOfRangeException>(() => col[0] = source[3]);
            Assert.Throws<ArgumentOutOfRangeException>(() => col.Insert(0, source[3]));
            Assert.Throws<ArgumentOutOfRangeException>(() => col.RemoveAt(0));

            // Remove forst zero/false
            Assert.Throws<ArgumentOutOfRangeException>(() => col.Remove(default(TData)));
        }

        [Fact]
        public void Clear()
        {
            var col = new ModbusDataCollection<TData>(GetArray());
            col.Clear();

            Assert.Equal(1, col.Count);
        }

        [Fact]
        public void Remove()
        {
            List<TData> source = GetList();
            var col = new ModbusDataCollection<TData>(source);
            int expectedCount = source.Count - 1;

            Assert.True(col.Remove(source[3]));

            Assert.Equal(expectedCount, col.Count);
            Assert.Equal(expectedCount, source.Count);
            Assert.Equal(source, col);
        }

        protected abstract TData[] GetArray();

        protected abstract TData GetNonExistentElement();

        protected List<TData> GetList()
        {
            return new List<TData>(GetArray());
        }
    }
}
