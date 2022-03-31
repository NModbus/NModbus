using System;

namespace NModbus
{
    /// <summary>
    /// A Modbus server message handler.
    /// </summary>
    public interface IModbusFunctionService
    {
        /// <summary>
        /// The function code that this handles
        /// </summary>
        byte FunctionCode { get; }

        /// <summary>
        /// Creates a message that wraps the request frame.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        IModbusMessage CreateRequest(byte[] frame);

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use HandleServerRequest instead.")]
        IModbusMessage HandleSlaveRequest(IModbusMessage request, ISlaveDataStore dataStore);

        /// <summary>
        /// Handle a server request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="dataStore"></param>
        /// <returns></returns>
        IModbusMessage HandleServerRequest(IModbusMessage request, IServerDataStore dataStore);

        /// <summary>
        /// Gets the number of bytes to read for a request
        /// </summary>
        /// <param name="frameStart"></param>
        /// <returns></returns>
        int GetRtuRequestBytesToRead(byte[] frameStart);

        /// <summary>
        /// Gets the number of bytes to read for a response.
        /// </summary>
        /// <param name="frameStart"></param>
        /// <returns></returns>
        int GetRtuResponseBytesToRead(byte[] frameStart);
    }
}