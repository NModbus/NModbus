namespace NModbus.Extensions
{
  using System;

  /// <summary>
  /// Utility Class to support Modbus 32/64bit devices. 
  /// </summary>
  public class ModbusMasterEnhanced
  {
    private readonly IModbusMaster master;
    private readonly uint wordSize;
    private readonly Func<byte[], byte[]> endian;
    private readonly bool wordSwapped;


    /// <summary>
    /// Constructor with values to be used by all methods. 
    /// Default is 32bit, LittleEndian, with wordswapping.
    /// </summary>
    /// <param name="master">The Modbus master</param>
    /// <param name="wordSize">Wordsize used by device. 16/32/64 are valid.</param>
    /// <param name="endian">The endian encoding of the device.</param>
    /// <param name="wordSwapped">Should the ushort words mirrored then flattened to bytes.</param>
    public ModbusMasterEnhanced(IModbusMaster master, uint wordSize=32, Func<byte[], byte[]> endian = null, bool wordSwapped = false)
    {
      this.master = master;
      this.wordSize = wordSize;
      this.endian = endian ?? Functions.Endian.LittleEndian;
      this.wordSwapped = wordSwapped;
    }

    public char[] ReadCharHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
      => Functions.RegisterFunctions.ByteValueArraysToChars(
        Functions.RegisterFunctions.ReadRegisters(slaveAddress, startAddress, numberOfPoints, this.master, this.wordSize, this.endian, this.wordSwapped));

    public ushort[] ReadUshortHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        => Functions.RegisterFunctions.ByteValueArraysToUShorts(
          Functions.RegisterFunctions.ReadRegisters(slaveAddress, startAddress, numberOfPoints, this.master, this.wordSize, this.endian, this.wordSwapped));

    public short[] ReadShortHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
      => Functions.RegisterFunctions.ByteValueArraysToShorts(
        Functions.RegisterFunctions.ReadRegisters(slaveAddress, startAddress, numberOfPoints, this.master, this.wordSize, this.endian, this.wordSwapped));

    public uint[] ReadUintHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
      => Functions.RegisterFunctions.ByteValueArraysToUInts(
        Functions.RegisterFunctions.ReadRegisters(slaveAddress, startAddress, numberOfPoints, this.master, this.wordSize, this.endian, this.wordSwapped));

    public int[] ReadIntHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
      => Functions.RegisterFunctions.ByteValueArraysToInts(
        Functions.RegisterFunctions.ReadRegisters(slaveAddress, startAddress, numberOfPoints, this.master, this.wordSize, this.endian, this.wordSwapped));

    public float[] ReadFloatHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
      => Functions.RegisterFunctions.ByteValueArraysToFloats(
           Functions.RegisterFunctions.ReadRegisters(slaveAddress, startAddress, numberOfPoints, this.master, this.wordSize, this.endian, this.wordSwapped));

    public void WriteCharHoldingRegisters(byte slaveAddress, ushort startAddress, char[] data)
      => Functions.RegisterFunctions.WriteRegistersFunc(
        slaveAddress,
        startAddress,
        Functions.RegisterFunctions.CharsToByteValueArrays(data, this.wordSize),
        this.master,
        this.wordSize,
        this.endian, this.wordSwapped);

    public void WriteUshortHoldingRegisters(byte slaveAddress, ushort startAddress, ushort[] data)
      => Functions.RegisterFunctions.WriteRegistersFunc(
        slaveAddress,
        startAddress,
        Functions.RegisterFunctions.UShortsToByteValueArrays(data, this.wordSize),
        this.master,
        this.wordSize,
        this.endian, this.wordSwapped);

    public void WriteShortHoldingRegisters(byte slaveAddress, ushort startAddress, short[] data)
      => Functions.RegisterFunctions.WriteRegistersFunc(
        slaveAddress,
        startAddress,
        Functions.RegisterFunctions.ShortsToByteValueArrays(data, this.wordSize),
        this.master,
        this.wordSize,
        this.endian, this.wordSwapped);

    public void WriteIntHoldingRegisters(byte slaveAddress, ushort startAddress, int[] data)
      => Functions.RegisterFunctions.WriteRegistersFunc(
        slaveAddress,
        startAddress,
        Functions.RegisterFunctions.IntToByteValueArrays(data, this.wordSize),
        this.master,
        this.wordSize,
        this.endian, this.wordSwapped);

    public void WriteUIntHoldingRegisters(byte slaveAddress, ushort startAddress, uint[] data)
      => Functions.RegisterFunctions.WriteRegistersFunc(
        slaveAddress,
        startAddress,
        Functions.RegisterFunctions.UIntToByteValueArrays(data, this.wordSize),
        this.master,
        this.wordSize,
        this.endian, this.wordSwapped);

    public void WriteFloatHoldingRegisters(byte slaveAddress, ushort startAddress, float[] data)
      => Functions.RegisterFunctions.WriteRegistersFunc(
        slaveAddress,
        startAddress,
        Functions.RegisterFunctions.FloatToByteValueArrays(data, this.wordSize),
        this.master,
        this.wordSize,
        this.endian, this.wordSwapped);
  }
}
