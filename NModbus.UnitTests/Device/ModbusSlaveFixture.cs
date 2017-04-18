#if SERIAL
using System.IO.Ports;
#endif
using System.Linq;
using Modbus.Data;
using Modbus.Device;
using Modbus.Message;
using Modbus.UnitTests.Message;
using Modbus.Unme.Common;
using Xunit;

namespace Modbus.UnitTests.Device
{
    public class ModbusSlaveFixture
    {
        private readonly DataStore _testDataStore;

        public ModbusSlaveFixture()
        {
            _testDataStore = DataStoreFactory.CreateTestDataStore();
        }

        [Fact]
        public void ReadDiscretesCoils()
        {
            ReadCoilsInputsResponse expectedResponse = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 2,
                new DiscreteCollection(false, true, false, true, false, true, false, true, false));
            ReadCoilsInputsResponse response =
                ModbusSlave.ReadDiscretes(new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 9), _testDataStore,
                    _testDataStore.CoilDiscretes);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
            Assert.Equal(expectedResponse.ByteCount, response.ByteCount);
        }

        [Fact]
        public void ReadDiscretesInputs()
        {
            ReadCoilsInputsResponse expectedResponse = new ReadCoilsInputsResponse(Modbus.ReadInputs, 1, 2,
                new DiscreteCollection(true, false, true, false, true, false, true, false, true));
            ReadCoilsInputsResponse response =
                ModbusSlave.ReadDiscretes(new ReadCoilsInputsRequest(Modbus.ReadInputs, 1, 1, 9), _testDataStore,
                    _testDataStore.InputDiscretes);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
            Assert.Equal(expectedResponse.ByteCount, response.ByteCount);
        }

        [Fact]
        public void ReadRegistersHoldingRegisters()
        {
            ReadHoldingInputRegistersResponse expectedResponse =
                new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1,
                    new RegisterCollection(1, 2, 3, 4, 5, 6));
            ReadHoldingInputRegistersResponse response =
                ModbusSlave.ReadRegisters(new ReadHoldingInputRegistersRequest(Modbus.ReadHoldingRegisters, 1, 0, 6),
                    _testDataStore, _testDataStore.HoldingRegisters);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
            Assert.Equal(expectedResponse.ByteCount, response.ByteCount);
        }

        [Fact]
        public void ReadRegistersInputRegisters()
        {
            ReadHoldingInputRegistersResponse expectedResponse =
                new ReadHoldingInputRegistersResponse(Modbus.ReadInputRegisters, 1,
                    new RegisterCollection(10, 20, 30, 40, 50, 60));
            ReadHoldingInputRegistersResponse response =
                ModbusSlave.ReadRegisters(new ReadHoldingInputRegistersRequest(Modbus.ReadInputRegisters, 1, 0, 6),
                    _testDataStore, _testDataStore.InputRegisters);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
            Assert.Equal(expectedResponse.ByteCount, response.ByteCount);
        }

        [Fact]
        public void WriteSingleCoil()
        {
            ushort addressToWrite = 35;
            bool valueToWrite = !_testDataStore.CoilDiscretes[addressToWrite + 1];
            WriteSingleCoilRequestResponse expectedResponse = new WriteSingleCoilRequestResponse(1, addressToWrite,
                valueToWrite);
            WriteSingleCoilRequestResponse response =
                ModbusSlave.WriteSingleCoil(new WriteSingleCoilRequestResponse(1, addressToWrite, valueToWrite),
                    _testDataStore, _testDataStore.CoilDiscretes);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
            Assert.Equal(valueToWrite, _testDataStore.CoilDiscretes[addressToWrite + 1]);
        }

        [Fact]
        public void WriteMultipleCoils()
        {
            ushort startAddress = 35;
            ushort numberOfPoints = 10;
            bool val = !_testDataStore.CoilDiscretes[startAddress + 1];
            WriteMultipleCoilsResponse expectedResponse = new WriteMultipleCoilsResponse(1, startAddress, numberOfPoints);
            WriteMultipleCoilsResponse response =
                ModbusSlave.WriteMultipleCoils(
                    new WriteMultipleCoilsRequest(1, startAddress,
                        new DiscreteCollection(val, val, val, val, val, val, val, val, val, val)), _testDataStore,
                    _testDataStore.CoilDiscretes);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
            Assert.Equal(new bool[] { val, val, val, val, val, val, val, val, val, val },
                _testDataStore.CoilDiscretes.Slice(startAddress + 1, numberOfPoints).ToArray());
        }

        [Fact]
        public void WriteSingleRegister()
        {
            ushort startAddress = 35;
            ushort value = 45;
            Assert.NotEqual(value, _testDataStore.HoldingRegisters[startAddress - 1]);
            WriteSingleRegisterRequestResponse expectedResponse = new WriteSingleRegisterRequestResponse(1, startAddress,
                value);
            WriteSingleRegisterRequestResponse response =
                ModbusSlave.WriteSingleRegister(new WriteSingleRegisterRequestResponse(1, startAddress, value),
                    _testDataStore, _testDataStore.HoldingRegisters);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
        }

        [Fact]
        public void WriteMultipleRegisters()
        {
            ushort startAddress = 35;
            ushort[] valuesToWrite = new ushort[] { 1, 2, 3, 4, 5 };
            Assert.NotEqual(valuesToWrite,
                _testDataStore.HoldingRegisters.Slice(startAddress - 1, valuesToWrite.Length).ToArray());
            WriteMultipleRegistersResponse expectedResponse = new WriteMultipleRegistersResponse(1, startAddress,
                (ushort)valuesToWrite.Length);
            WriteMultipleRegistersResponse response =
                ModbusSlave.WriteMultipleRegisters(
                    new WriteMultipleRegistersRequest(1, startAddress, new RegisterCollection(valuesToWrite)),
                    _testDataStore, _testDataStore.HoldingRegisters);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
        }

#if SERIAL
        [Fact]
        public void ApplyRequest_VerifyModbusRequestReceivedEventIsFired()
        {
            bool eventFired = false;
            ModbusSlave slave = ModbusSerialSlave.CreateAscii(1, new SerialPort());
            WriteSingleRegisterRequestResponse request = new WriteSingleRegisterRequestResponse(1, 1, 1);
            slave.ModbusSlaveRequestReceived += (obj, args) =>
            {
                eventFired = true;
                Assert.Equal(request, args.Message);
            };

            slave.ApplyRequest(request);
            Assert.True(eventFired);
        }
#endif

        [Fact]
        public void WriteMultipCoils_MakeSureWeDoNotWriteRemainder()
        {
            // 0, false initialized data store
            var dataStore = DataStoreFactory.CreateDefaultDataStore();

            var request = new WriteMultipleCoilsRequest(1, 0,
                new DiscreteCollection(Enumerable.Repeat(true, 8).ToArray())) { NumberOfPoints = 2 };
            ModbusSlave.WriteMultipleCoils(request, dataStore, dataStore.CoilDiscretes);

            Assert.Equal(dataStore.CoilDiscretes.Slice(1, 8).ToArray(),
                new[] { true, true, false, false, false, false, false, false });
        }
    }
}