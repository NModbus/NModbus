﻿using System;
using System.Threading.Tasks;

namespace NModbus
{
    /// <summary>
    ///     Modbus slave device.
    /// </summary>
    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use IModbusClient instead.")]
    public interface IModbusMaster : IModbusClient {}

    /// <summary>
    ///     Modbus client device.
    /// </summary>
    public interface IModbusClient : IDisposable
    {
        /// <summary>
        ///     Transport used by this client.
        /// </summary>
        IModbusTransport Transport { get; }

        /// <summary>
        ///    Reads from 1 to 2000 contiguous coils status.
        /// </summary>
        /// <param name="serverAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of coils to read.</param>
        /// <returns>Coils status.</returns>
        bool[] ReadCoils(byte serverAddress, ushort startAddress, ushort numberOfPoints);

        /// <summary>
        ///    Asynchronously reads from 1 to 2000 contiguous coils status.
        /// </summary>
        /// <param name="serverAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of coils to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        Task<bool[]> ReadCoilsAsync(byte serverAddress, ushort startAddress, ushort numberOfPoints);

        /// <summary>
        ///    Reads from 1 to 2000 contiguous discrete input status.
        /// </summary>
        /// <param name="serverAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of discrete inputs to read.</param>
        /// <returns>Discrete inputs status.</returns>
        bool[] ReadInputs(byte serverAddress, ushort startAddress, ushort numberOfPoints);

        /// <summary>
        ///    Asynchronously reads from 1 to 2000 contiguous discrete input status.
        /// </summary>
        /// <param name="serverAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of discrete inputs to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        Task<bool[]> ReadInputsAsync(byte serverAddress, ushort startAddress, ushort numberOfPoints);

        /// <summary>
        ///    Reads contiguous block of holding registers.
        /// </summary>
        /// <param name="serverAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>Holding registers status.</returns>
        ushort[] ReadHoldingRegisters(byte serverAddress, ushort startAddress, ushort numberOfPoints);

        /// <summary>
        ///    Asynchronously reads contiguous block of holding registers.
        /// </summary>
        /// <param name="serverAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        Task<ushort[]> ReadHoldingRegistersAsync(byte serverAddress, ushort startAddress, ushort numberOfPoints);

        /// <summary>
        ///    Reads contiguous block of input registers.
        /// </summary>
        /// <param name="serverAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>Input registers status.</returns>
        ushort[] ReadInputRegisters(byte serverAddress, ushort startAddress, ushort numberOfPoints);

        /// <summary>
        ///    Asynchronously reads contiguous block of input registers.
        /// </summary>
        /// <param name="serverAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        Task<ushort[]> ReadInputRegistersAsync(byte serverAddress, ushort startAddress, ushort numberOfPoints);

        /// <summary>
        ///    Writes a single coil value.
        /// </summary>
        /// <param name="serverAddress">Address of the device to write to.</param>
        /// <param name="coilAddress">Address to write value to.</param>
        /// <param name="value">Value to write.</param>
        void WriteSingleCoil(byte serverAddress, ushort coilAddress, bool value);

        /// <summary>
        ///    Asynchronously writes a single coil value.
        /// </summary>
        /// <param name="serverAddress">Address of the device to write to.</param>
        /// <param name="coilAddress">Address to write value to.</param>
        /// <param name="value">Value to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteSingleCoilAsync(byte serverAddress, ushort coilAddress, bool value);

        /// <summary>
        ///    Writes a single holding register.
        /// </summary>
        /// <param name="serverAddress">Address of the device to write to.</param>
        /// <param name="registerAddress">Address to write.</param>
        /// <param name="value">Value to write.</param>
        void WriteSingleRegister(byte serverAddress, ushort registerAddress, ushort value);

        /// <summary>
        ///    Asynchronously writes a single holding register.
        /// </summary>
        /// <param name="serverAddress">Address of the device to write to.</param>
        /// <param name="registerAddress">Address to write.</param>
        /// <param name="value">Value to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteSingleRegisterAsync(byte serverAddress, ushort registerAddress, ushort value);

        /// <summary>
        ///    Writes a block of 1 to 123 contiguous registers.
        /// </summary>
        /// <param name="serverAddress">Address of the device to write to.</param>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        void WriteMultipleRegisters(byte serverAddress, ushort startAddress, ushort[] data);

        /// <summary>
        ///    Asynchronously writes a block of 1 to 123 contiguous registers.
        /// </summary>
        /// <param name="serverAddress">Address of the device to write to.</param>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteMultipleRegistersAsync(byte serverAddress, ushort startAddress, ushort[] data);

        /// <summary>
        ///    Writes a sequence of coils.
        /// </summary>
        /// <param name="serverAddress">Address of the device to write to.</param>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        void WriteMultipleCoils(byte serverAddress, ushort startAddress, bool[] data);

        /// <summary>
        ///    Asynchronously writes a sequence of coils.
        /// </summary>
        /// <param name="serverAddress">Address of the device to write to.</param>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteMultipleCoilsAsync(byte serverAddress, ushort startAddress, bool[] data);

        /// <summary>
        ///    Performs a combination of one read operation and one write operation in a single Modbus transaction.
        ///    The write operation is performed before the read.
        /// </summary>
        /// <param name="serverAddress">Address of device to read values from.</param>
        /// <param name="startReadAddress">Address to begin reading (Holding registers are addressed starting at 0).</param>
        /// <param name="numberOfPointsToRead">Number of registers to read.</param>
        /// <param name="startWriteAddress">Address to begin writing (Holding registers are addressed starting at 0).</param>
        /// <param name="writeData">Register values to write.</param>
        ushort[] ReadWriteMultipleRegisters(
            byte serverAddress,
            ushort startReadAddress,
            ushort numberOfPointsToRead,
            ushort startWriteAddress,
            ushort[] writeData);

        /// <summary>
        ///    Asynchronously performs a combination of one read operation and one write operation in a single Modbus transaction.
        ///    The write operation is performed before the read.
        /// </summary>
        /// <param name="serverAddress">Address of device to read values from.</param>
        /// <param name="startReadAddress">Address to begin reading (Holding registers are addressed starting at 0).</param>
        /// <param name="numberOfPointsToRead">Number of registers to read.</param>
        /// <param name="startWriteAddress">Address to begin writing (Holding registers are addressed starting at 0).</param>
        /// <param name="writeData">Register values to write.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<ushort[]> ReadWriteMultipleRegistersAsync(
            byte serverAddress,
            ushort startReadAddress,
            ushort numberOfPointsToRead,
            ushort startWriteAddress,
            ushort[] writeData);

        /// <summary>
        /// Write a file record to the device.
        /// </summary>
        /// <param name="serverAdress">Address of device to write values to</param>
        /// <param name="fileNumber">The Extended Memory file number</param>
        /// <param name="startingAddress">The starting register address within the file</param>
        /// <param name="data">The data to be written</param>
        void WriteFileRecord(byte serverAdress, ushort fileNumber, ushort startingAddress, byte[] data);

        /// <summary>
        ///    Executes the custom message.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request.</param>
        TResponse ExecuteCustomMessage<TResponse>(IModbusMessage request)
            where TResponse : IModbusMessage, new();
    }
}
