//using System;
//using System.Diagnostics;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using System.Threading.Tasks;
//using NModbus.Data;
//using NModbus.IO;
//using NModbus.Message;

//namespace NModbus.Device
//{
//    /// <summary>
//    ///     Modbus slave device.
//    /// </summary>
//    public class ModbusSlave : ModbusDevice
//    {
//        private readonly byte _unitId;
//        private readonly DataStore _dataStore;

//        internal ModbusSlave(byte unitId, ModbusTransport transport)
//            : base(transport)
//        {
//            if (unitId == 0) throw new ArgumentOutOfRangeException(nameof(unitId));

//            _dataStore = DataStoreFactory.CreateDefaultDataStore();
//            _unitId = unitId;
//        }

//        /// <summary>
//        ///     Raised when a Modbus slave receives a request, before processing request function.
//        /// </summary>
//        /// <exception cref="InvalidModbusRequestException">The Modbus request was invalid, and an error response the specified exception should be sent.</exception>
//        public event EventHandler<ModbusSlaveRequestEventArgs> ModbusSlaveRequestReceived;

//        /// <summary>
//        ///     Raised when a Modbus slave receives a write request, after processing the write portion of the function.
//        /// </summary>
//        /// <remarks>For Read/Write Multiple registers (function code 23), this method is raised after writing and before reading.</remarks>
//        public event EventHandler<ModbusSlaveRequestEventArgs> WriteComplete;

//        /// <summary>
//        ///     Gets or sets the data store.
//        /// </summary>
//        public DataStore DataStore
//        {
//            get {  return _dataStore; }
//        }

//        /// <summary>
//        ///     Gets or sets the unit ID.
//        /// </summary>
//        public byte UnitId
//        {
//            get { return _unitId; }
//        }

//        /// <summary>
//        ///     Start slave listening for requests.
//        /// </summary>
//        public virtual Task ListenAsync()
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        ///    Create a slave to be added to a network.
//        /// </summary>
//        /// <param name="unitId"></param>
//        /// <returns></returns>
//        public static ModbusSlave Create(byte unitId)
//        {
//            return new ModbusSlave(unitId, null);
//        }

//        internal static ReadCoilsInputsResponse ReadDiscretes(
//            ReadCoilsInputsRequest request,
//            DataStore dataStore,
//            ModbusDataCollection<bool> dataSource)
//        {
//            var data = DataStore.ReadData<DiscreteCollection, bool>(
//                dataStore,
//                dataSource,
//                request.StartAddress,
//                request.NumberOfPoints,
//                dataStore.SyncRoot);

//            return new ReadCoilsInputsResponse(
//                request.FunctionCode,
//                request.SlaveAddress,
//                data.ByteCount,
//                data);
//        }

//        internal static ReadHoldingInputRegistersResponse ReadRegisters(
//            ReadHoldingInputRegistersRequest request,
//            DataStore dataStore,
//            ModbusDataCollection<ushort> dataSource)
//        {
//            var data = DataStore.ReadData<RegisterCollection, ushort>(
//                dataStore,
//                dataSource,
//                request.StartAddress,
//                request.NumberOfPoints,
//                dataStore.SyncRoot);

//            return new ReadHoldingInputRegistersResponse(
//                request.FunctionCode,
//                request.SlaveAddress,
//                data);
//        }

//        internal static WriteSingleCoilRequestResponse WriteSingleCoil(
//            WriteSingleCoilRequestResponse request,
//            DataStore dataStore,
//            ModbusDataCollection<bool> dataSource)
//        {
//            DataStore.WriteData(
//                dataStore,
//                new DiscreteCollection(request.Data[0] == Modbus.CoilOn),
//                dataSource,
//                request.StartAddress,
//                dataStore.SyncRoot);

//            return request;
//        }

//        internal static WriteMultipleCoilsResponse WriteMultipleCoils(
//            WriteMultipleCoilsRequest request,
//            DataStore dataStore,
//            ModbusDataCollection<bool> dataSource)
//        {
//            DataStore.WriteData(
//                dataStore,
//                request.Data.Take(request.NumberOfPoints),
//                dataSource,
//                request.StartAddress,
//                dataStore.SyncRoot);

//            return new WriteMultipleCoilsResponse(
//                request.SlaveAddress,
//                request.StartAddress,
//                request.NumberOfPoints);
//        }

//        internal static WriteSingleRegisterRequestResponse WriteSingleRegister(
//            WriteSingleRegisterRequestResponse request,
//            DataStore dataStore,
//            ModbusDataCollection<ushort> dataSource)
//        {
//            DataStore.WriteData(
//                dataStore,
//                request.Data,
//                dataSource,
//                request.StartAddress,
//                dataStore.SyncRoot);

//            return request;
//        }

//        internal static WriteMultipleRegistersResponse WriteMultipleRegisters(
//            WriteMultipleRegistersRequest request,
//            DataStore dataStore,
//            ModbusDataCollection<ushort> dataSource)
//        {
//            DataStore.WriteData(
//                dataStore,
//                request.Data,
//                dataSource,
//                request.StartAddress,
//                dataStore.SyncRoot);

//            return new WriteMultipleRegistersResponse(
//                request.SlaveAddress,
//                request.StartAddress,
//                request.NumberOfPoints);
//        }

//        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Cast is not unneccessary.")]
//        internal IModbusMessage ApplyRequest(IModbusMessage request)
//        {
//            IModbusMessage response;

//            try
//            {
//                Debug.WriteLine(request.ToString());
//                var eventArgs = new ModbusSlaveRequestEventArgs(request);
//                ModbusSlaveRequestReceived?.Invoke(this, eventArgs);

//                switch (request.FunctionCode)
//                {
//                    case ModbusFunctionCodes.ReadCoils:
//                        response = ReadDiscretes(
//                            (ReadCoilsInputsRequest)request,
//                            DataStore,
//                            DataStore.CoilDiscretes);
//                        break;
//                    case ModbusFunctionCodes.ReadInputs:
//                        response = ReadDiscretes(
//                            (ReadCoilsInputsRequest)request,
//                            DataStore,
//                            DataStore.InputDiscretes);
//                        break;
//                    case ModbusFunctionCodes.ReadHoldingRegisters:
//                        response = ReadRegisters(
//                            (ReadHoldingInputRegistersRequest)request,
//                            DataStore,
//                            DataStore.HoldingRegisters);
//                        break;
//                    case ModbusFunctionCodes.ReadInputRegisters:
//                        response = ReadRegisters(
//                            (ReadHoldingInputRegistersRequest)request,
//                            DataStore,
//                            DataStore.InputRegisters);
//                        break;
//                    case ModbusFunctionCodes.Diagnostics:
//                        response = request;
//                        break;
//                    case ModbusFunctionCodes.WriteSingleCoil:
//                        response = WriteSingleCoil(
//                            (WriteSingleCoilRequestResponse)request,
//                            DataStore,
//                            DataStore.CoilDiscretes);
//                        WriteComplete?.Invoke(this, eventArgs);
//                        break;
//                    case ModbusFunctionCodes.WriteSingleRegister:
//                        response = WriteSingleRegister(
//                            (WriteSingleRegisterRequestResponse)request,
//                            DataStore,
//                            DataStore.HoldingRegisters);
//                        WriteComplete?.Invoke(this, eventArgs);
//                        break;
//                    case ModbusFunctionCodes.WriteMultipleCoils:
//                        response = WriteMultipleCoils(
//                            (WriteMultipleCoilsRequest)request,
//                            DataStore,
//                            DataStore.CoilDiscretes);
//                        WriteComplete?.Invoke(this, eventArgs);
//                        break;
//                    case ModbusFunctionCodes.WriteMultipleRegisters:
//                        response = WriteMultipleRegisters(
//                            (WriteMultipleRegistersRequest)request,
//                            DataStore,
//                            DataStore.HoldingRegisters);
//                        WriteComplete?.Invoke(this, eventArgs);
//                        break;
//                    case ModbusFunctionCodes.ReadWriteMultipleRegisters:
//                        ReadWriteMultipleRegistersRequest readWriteRequest = (ReadWriteMultipleRegistersRequest)request;
//                        WriteMultipleRegisters(
//                            readWriteRequest.WriteRequest,
//                            DataStore,
//                            DataStore.HoldingRegisters);
//                        WriteComplete?.Invoke(this, eventArgs);
//                        response = ReadRegisters(
//                            readWriteRequest.ReadRequest,
//                            DataStore,
//                            DataStore.HoldingRegisters);
//                        break;
//                    default:
//                        string msg = $"Unsupported function code {request.FunctionCode}.";
//                        Debug.WriteLine(msg);
//                        throw new InvalidModbusRequestException(Modbus.IllegalFunction);
//                }
//            }
//            catch (InvalidModbusRequestException ex)
//            {
//                // Catches the exception for an illegal function or a custom exception from the ModbusSlaveRequestReceived event.
//                response = new SlaveExceptionResponse(
//                    request.SlaveAddress,
//                    (byte)(Modbus.ExceptionOffset + request.FunctionCode),
//                    ex.ExceptionCode);
//            }

//            return response;
//        }
//    }
//}
