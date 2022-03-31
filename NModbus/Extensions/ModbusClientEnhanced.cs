namespace NModbus.Extensions
{
  using System;

  [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ModbusClientEnhanced instead.")]
  public class ModbusMasterEnhanced : ModbusClientEnhanced
  {
      public ModbusMasterEnhanced(IModbusClient client, uint wordSize = 32, Func<byte[], byte[]> endian = null, bool wordSwapped = false) : base(client, wordSize, endian, wordSwapped) { }
  }

  /// <summary>
  /// Utility Class to support Modbus 32/64bit devices. 
  /// </summary>
    public class ModbusClientEnhanced
  {
    private readonly IModbusClient client;
    private readonly uint wordSize;
    private readonly Func<byte[], byte[]> endian;
    private readonly bool wordSwapped;


    /// <summary>
    /// Constructor with values to be used by all methods. 
    /// Default is 32bit, LittleEndian, with wordswapping.
    /// </summary>
    /// <param name="client">The Modbus client</param>
    /// <param name="wordSize">Wordsize used by device. 16/32/64 are valid.</param>
    /// <param name="endian">The endian encoding of the device.</param>
    /// <param name="wordSwapped">Should the ushort words mirrored then flattened to bytes.</param>
    public ModbusClientEnhanced(IModbusClient client, uint wordSize=32, Func<byte[], byte[]> endian = null, bool wordSwapped = false)
    {
      this.client = client;
      this.wordSize = wordSize;
      this.endian = endian ?? Functions.Endian.LittleEndian;
      this.wordSwapped = wordSwapped;
    }

    /// <summary>
    /// Reads registers and converts the result into a char array.
    /// </summary>
    /// <param name="serverAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of chars to read.</param>
    /// <returns></returns>
    public char[] ReadCharHoldingRegisters(byte serverAddress, ushort startAddress, ushort numberOfPoints)
      => Functions.RegisterFunctions.ByteValueArraysToChars(
        Functions.RegisterFunctions.ReadRegisters(serverAddress, startAddress, numberOfPoints, this.client, this.wordSize, this.endian, this.wordSwapped));

    /// <summary>
    /// Reads registers and converts the result into a ushort array.
    /// </summary>
    /// <param name="serverAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of ushorts to read.</param>
    /// <returns></returns>
    public ushort[] ReadUshortHoldingRegisters(byte serverAddress, ushort startAddress, ushort numberOfPoints)
        => Functions.RegisterFunctions.ByteValueArraysToUShorts(
          Functions.RegisterFunctions.ReadRegisters(serverAddress, startAddress, numberOfPoints, this.client, this.wordSize, this.endian, this.wordSwapped));

    /// <summary>
    /// Reads registers and converts the result into a short array.
    /// </summary>
    /// <param name="serverAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of shots to read.</param>
    /// <returns></returns>
    public short[] ReadShortHoldingRegisters(byte serverAddress, ushort startAddress, ushort numberOfPoints)
      => Functions.RegisterFunctions.ByteValueArraysToShorts(
        Functions.RegisterFunctions.ReadRegisters(serverAddress, startAddress, numberOfPoints, this.client, this.wordSize, this.endian, this.wordSwapped));

    /// <summary>
    /// Reads registers and converts the result into a uint array.
    /// </summary>
    /// <param name="serverAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of uints to read.</param>
    /// <returns></returns>
    public uint[] ReadUintHoldingRegisters(byte serverAddress, ushort startAddress, ushort numberOfPoints)
      => Functions.RegisterFunctions.ByteValueArraysToUInts(
        Functions.RegisterFunctions.ReadRegisters(serverAddress, startAddress, numberOfPoints, this.client, this.wordSize, this.endian, this.wordSwapped));

    /// <summary>
    /// Reads registers and converts the result into a int array.
    /// </summary>
    /// <param name="serverAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of ints to read.</param>
    /// <returns></returns>
    public int[] ReadIntHoldingRegisters(byte serverAddress, ushort startAddress, ushort numberOfPoints)
      => Functions.RegisterFunctions.ByteValueArraysToInts(
        Functions.RegisterFunctions.ReadRegisters(serverAddress, startAddress, numberOfPoints, this.client, this.wordSize, this.endian, this.wordSwapped));

    /// <summary>
    /// Reads registers and converts the result into a float array.
    /// </summary>
    /// <param name="serverAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of floats to read.</param>
    /// <returns></returns>
    public float[] ReadFloatHoldingRegisters(byte serverAddress, ushort startAddress, ushort numberOfPoints)
      => Functions.RegisterFunctions.ByteValueArraysToFloats(
           Functions.RegisterFunctions.ReadRegisters(serverAddress, startAddress, numberOfPoints, this.client, this.wordSize, this.endian, this.wordSwapped));

    /// <summary>
    ///     Write a char array to registers.
    /// </summary>
    /// <param name="serverAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to start writting values at.</param>
    /// <param name="data">Chars to write to device.</param>
    public void WriteCharHoldingRegisters(byte serverAddress, ushort startAddress, char[] data)
      => Functions.RegisterFunctions.WriteRegistersFunc(
        serverAddress,
        startAddress,
        Functions.RegisterFunctions.CharsToByteValueArrays(data, this.wordSize),
        this.client,
        this.wordSize,
        this.endian, this.wordSwapped);

    /// <summary>
    ///     Write a ushort array to registers.
    /// </summary>
    /// <param name="serverAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to start writting values at.</param>
    /// <param name="data">Ushorts to write to device.</param>
    public void WriteUshortHoldingRegisters(byte serverAddress, ushort startAddress, ushort[] data)
      => Functions.RegisterFunctions.WriteRegistersFunc(
        serverAddress,
        startAddress,
        Functions.RegisterFunctions.UShortsToByteValueArrays(data, this.wordSize),
        this.client,
        this.wordSize,
        this.endian, this.wordSwapped);

    /// <summary>
    ///     Write a short array to registers.
    /// </summary>
    /// <param name="serverAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to start writting values at.</param>
    /// <param name="data">Shorts to write to device.</param>
    public void WriteShortHoldingRegisters(byte serverAddress, ushort startAddress, short[] data)
      => Functions.RegisterFunctions.WriteRegistersFunc(
        serverAddress,
        startAddress,
        Functions.RegisterFunctions.ShortsToByteValueArrays(data, this.wordSize),
        this.client,
        this.wordSize,
        this.endian, this.wordSwapped);

    /// <summary>
    ///     Write a int array to registers.
    /// </summary>
    /// <param name="serverAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to start writting values at.</param>
    /// <param name="data">Ints to write to device.</param>
    public void WriteIntHoldingRegisters(byte serverAddress, ushort startAddress, int[] data)
      => Functions.RegisterFunctions.WriteRegistersFunc(
        serverAddress,
        startAddress,
        Functions.RegisterFunctions.IntToByteValueArrays(data, this.wordSize),
        this.client,
        this.wordSize,
        this.endian, this.wordSwapped);

    /// <summary>
    ///     Write a uint array to registers.
    /// </summary>
    /// <param name="serverAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to start writting values at.</param>
    /// <param name="data">Uints to write to device.</param>
    public void WriteUIntHoldingRegisters(byte serverAddress, ushort startAddress, uint[] data)
      => Functions.RegisterFunctions.WriteRegistersFunc(
        serverAddress,
        startAddress,
        Functions.RegisterFunctions.UIntToByteValueArrays(data, this.wordSize),
        this.client,
        this.wordSize,
        this.endian, this.wordSwapped);

    /// <summary>
    ///     Write a float array to registers.
    /// </summary>
    /// <param name="serverAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to start writting values at.</param>
    /// <param name="data">Floats to write to device.</param>
    public void WriteFloatHoldingRegisters(byte serverAddress, ushort startAddress, float[] data)
      => Functions.RegisterFunctions.WriteRegistersFunc(
        serverAddress,
        startAddress,
        Functions.RegisterFunctions.FloatToByteValueArrays(data, this.wordSize),
        this.client,
        this.wordSize,
        this.endian, this.wordSwapped);
  }
}
