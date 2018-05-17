using NModbus.Message;

namespace NModbus
{
    /// <summary>
    /// A Modbus slave message handler.
    /// </summary>
    public interface IModbusFunctionService
    {
        /// <summary>
        /// The function code that this handles
        /// </summary>
        byte FunctionCode { get; }

        /// <summary>
        /// Creates a message that wrapps the request frame.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        IModbusMessage CreateRequest(byte[] frame);

        /// <summary>
        /// Handle a slave request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="dataStore"></param>
        /// <returns></returns>
        IModbusMessage HandleSlaveRequest(IModbusMessage request, ISlaveDataStore dataStore);

        /// <summary>
        /// Gets the number of bytes to read for a request
        /// </summary>
        /// <param name="frameStart"></param>
        /// <returns></returns>
        int GetRtuRequestBytesToRead(byte[] frameStart);

        /// <summary>
        /// Gets the number of bytes to read for a response
        /// </summary>
        /// <param name="frameStart"></param>
        /// <returns></returns>
        int GetRtuResponseBytesToRead(byte[] frameStart);
    }
}