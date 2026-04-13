using NModbus.Message;

namespace NModbus.Device.MessageHandlers
{
    /// <summary>
    ///     Service handler for Modbus function code 0x2B (Read Device Identification).
    /// </summary>
    public class ReadDeviceIdentificationService : IModbusFunctionService
    {
        /// <summary>Gets the function code (0x2B / 43).</summary>
        public byte FunctionCode => ModbusFunctionCodes.ReadDeviceIdentification;

        /// <summary>Creates a request message from the raw frame.</summary>
        public IModbusMessage CreateRequest(byte[] frame)
        {
            var request = new ReadDeviceIdRequest();
            request.Initialize(frame);
            return request;
        }

        /// <summary>
        ///     Gets the number of remaining bytes to read for a request frame.
        ///     Device ID request is fixed-length: 5 bytes total (SlaveAddr + FC + MEI + Category + ObjectId).
        /// </summary>
        public int GetRtuRequestBytesToRead(byte[] frameStart)
        {
            return 5 - frameStart.Length;
        }

        /// <summary>
        ///     Gets the number of remaining bytes to read for a response frame.
        ///     Returns -1 because Device ID responses are variable-length and must be
        ///     handled by the transport layer with custom frame reading.
        /// </summary>
        public int GetRtuResponseBytesToRead(byte[] frameStart)
        {
            return -1;
        }

        /// <summary>
        ///     Slave-side request handling is not implemented.
        ///     This service is for master (client) use only.
        /// </summary>
        public IModbusMessage HandleSlaveRequest(IModbusMessage request, ISlaveDataStore dataStore)
        {
            throw new System.NotImplementedException(
                "Device ID slave-side handling is not implemented.");
        }
    }
}
