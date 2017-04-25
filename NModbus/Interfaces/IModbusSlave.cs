namespace NModbus
{
    /// <summary>
    /// A modbus slave.
    /// </summary>
    public interface IModbusSlave
    {
        /// <summary>
        /// Gets the unit id of this slave.
        /// </summary>
        byte UnitId { get; }

        /// <summary>
        /// Gets the data store for this slave.
        /// </summary>
        ISlaveDataStore DataStore { get; }

        /// <summary>
        /// Applies the request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IModbusMessage ApplyRequest(IModbusMessage request);
    }
}