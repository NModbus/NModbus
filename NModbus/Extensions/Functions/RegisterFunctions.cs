namespace NModbus.Extensions.Functions
{
  using System;
  using System.IO;
  using System.Linq;

  /// <summary>
  ///   This class provides some functions that can be used to read/write values of a set word size.
  /// </summary>
  public class RegisterFunctions
  {
    public static byte[][] ReadRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints, IModbusMaster master, uint wordSize, Func<byte[], byte[]> endianConverter, bool wordSwap = false)
    {
      var registerMultiplier = RegisterFunctions.GetRegisterMultiplier(wordSize);
      var registersToRead = (ushort)(numberOfPoints * registerMultiplier);
      var values = master.ReadHoldingRegisters(slaveAddress, startAddress, registersToRead);
      if (wordSwap) Array.Reverse(values);
      return RegisterFunctions.ConvertRegistersToValues(values, registerMultiplier).Select(endianConverter).ToArray();
    }

    public static void WriteRegistersFunc(byte slaveAddress, ushort startAddress, byte[][] data, IModbusMaster master, uint wordSize, Func<byte[], byte[]> endianConverter, bool wordSwap = false)
    {
      var wordByteArraySize = RegisterFunctions.GetRegisterMultiplier(wordSize) * 2;
      if (data.Any(e => e.Length != wordByteArraySize))
      {
        throw new ArgumentException("All data values must be of the correct word length.");
      }
      var dataCorrectEndian = data.Select(endianConverter).ToArray();
      var registerValues = RegisterFunctions.ConvertValuesToRegisters(dataCorrectEndian);
      if (wordSwap) Array.Reverse(registerValues);
      master.WriteMultipleRegisters(slaveAddress, startAddress, registerValues);
    }



    public static char[] ByteValueArraysToChars(byte[][] data, bool frontPadding = true, bool singleCharPerRegister = true)
    {
      if (singleCharPerRegister)
      {
        return frontPadding
          ? data.Select(e => BitConverter.ToChar(e, e.Length - 2)).ToArray()
          : data.Select(e => BitConverter.ToChar(e, 0)).ToArray();
      }
      var flatData = data.SelectMany(e => e).ToArray();
      var count = flatData.Length / 2;
      var chars = new char[count];
      for (var index = 0; index < count; index++)
      {
        chars[index] = BitConverter.ToChar(flatData, index);
      }
      return chars;
    }

    public static short[] ByteValueArraysToShorts(byte[][] data, bool frontPadding = true)
    {
      return frontPadding
        ? data.Select(e => BitConverter.ToInt16(e, e.Length - 2)).ToArray()
        : data.Select(e => BitConverter.ToInt16(e, 0)).ToArray();
    }

    public static ushort[] ByteValueArraysToUShorts(byte[][] data, bool frontPadding = true)
    {
      return frontPadding
        ? data.Select(e => BitConverter.ToUInt16(e, e.Length - 2)).ToArray()
        : data.Select(e => BitConverter.ToUInt16(e, 0)).ToArray();
    }

    public static int[] ByteValueArraysToInts(byte[][] data, bool frontPadding = true)
    {
      return frontPadding
        ? data.Select(e => BitConverter.ToInt32(e, e.Length - 4)).ToArray()
        : data.Select(e => BitConverter.ToInt32(e, 0)).ToArray();
    }

    public static uint[] ByteValueArraysToUInts(byte[][] data, bool frontPadding = true)
    {
      return frontPadding
        ? data.Select(e => BitConverter.ToUInt32(e, e.Length - 4)).ToArray()
        : data.Select(e => BitConverter.ToUInt32(e, 0)).ToArray();
    }

    public static float[] ByteValueArraysToFloats(byte[][] data, bool frontPadding = true)
    {
      return frontPadding
        ? data.Select(e => BitConverter.ToSingle(e, e.Length - 4)).ToArray()
        : data.Select(e => BitConverter.ToSingle(e, 0)).ToArray();
    }


    public static byte[][] CharsToByteValueArrays(char[] data, uint wordSize, bool frontPadding = true, bool singleCharPerRegister = true)
    {
      var bytesPerWord = RegisterFunctions.GetRegisterMultiplier(wordSize) * 2;
      if (!singleCharPerRegister)
      {
        var remainder = data.Length % bytesPerWord;
        var registerBytes = remainder > 0
          ? data.Length + (bytesPerWord - remainder)
          : data.Length;
        var byteArray = new byte[registerBytes];
        for (var index = 0; index < byteArray.Length; index++)
        {
          byteArray[index] = index < data.Length
            ? Convert.ToByte(data[index])
            : Convert.ToByte('\0'); //Unicode Null Charector
        }
        var byteValueArrays = new byte[byteArray.Length / bytesPerWord][];
        for (var index = 0; index < byteValueArrays.Length; index++)
        {
          var offset = index * bytesPerWord;
          byteValueArrays[index] = new ArraySegment<byte>(byteArray, offset, bytesPerWord).ToArray();
        }
        return byteValueArrays;
      }
      return (frontPadding)
        ? data.Select(e =>
        {
          var bytes = new byte[bytesPerWord];
          bytes[bytes.Length - 1] = Convert.ToByte(e);
          return bytes;
        }).ToArray()
        : data.Select(e =>
        {
          var bytes = new byte[bytesPerWord];
          bytes[0] = Convert.ToByte(e);
          return bytes;
        }).ToArray();
    }

    public static byte[][] ShortsToByteValueArrays(short[] data, uint wordSize, bool frontPadding = true)
      => data.Select(e => RegisterFunctions.PadBytesToWordSize(
        wordSize, BitConverter.GetBytes(e), frontPadding)).ToArray();

    public static byte[][] UShortsToByteValueArrays(ushort[] data, uint wordSize, bool frontPadding = true)
      => data.Select(e => RegisterFunctions.PadBytesToWordSize(
        wordSize, BitConverter.GetBytes(e), frontPadding)).ToArray();

    public static byte[][] IntToByteValueArrays(int[] data, uint wordSize, bool frontPadding = true)
      => data.Select(e => RegisterFunctions.PadBytesToWordSize(
        wordSize, BitConverter.GetBytes(e), frontPadding)).ToArray();

    public static byte[][] UIntToByteValueArrays(uint[] data, uint wordSize, bool frontPadding = true)
      => data.Select(e => RegisterFunctions.PadBytesToWordSize(
        wordSize, BitConverter.GetBytes(e), frontPadding)).ToArray();

    public static byte[][] FloatToByteValueArrays(float[] data, uint wordSize, bool frontPadding = true)
      => data.Select(e => RegisterFunctions.PadBytesToWordSize(
        wordSize, BitConverter.GetBytes(e), frontPadding)).ToArray();


    private static byte[] PadBytesToWordSize(uint wordSize, byte[] source, bool frontPadding)
    {
      var targetLength = RegisterFunctions.GetRegisterMultiplier(wordSize) * 2;
      var target = new byte[targetLength];
      if (source.Length > target.Length)
      {
        throw new ArgumentException("Source bytes can not greater than target");
      }
      var offset = frontPadding
        ? target.Length - source.Length
        : 0;
      Array.Copy(
        source, 0, target, offset, source.Length);
      return target;
    }

    private static ushort[] ConvertValuesToRegisters(byte[][] data)
    {
      var flatData = data.SelectMany(e => e).ToArray();
      var count = flatData.Count() / 2;
      var registers = new ushort[count];
      for (var index = 0; index < count; index++)
      {
        registers[index] = BitConverter.ToUInt16(flatData, (index * 2));
      }
      return registers;
    }

    private static byte[][] ConvertRegistersToValues(ushort[] registers, int registerMultiplier) //TODO::Convert to function pass in everything it needs
    {
      if ((registers.Length % registerMultiplier) != 0)
      {
        throw new InvalidDataException("registers.Length is not a multiple of RegisterMultiplier");
      }
      var count = registers.Length / registerMultiplier;
      var values = new byte[count][];
      for (var index = 0; index < count; index++)
      {
        var offset = index * registerMultiplier;
        var segment = new ArraySegment<ushort>(registers, offset, registerMultiplier);
        var bytes = segment.SelectMany(BitConverter.GetBytes).ToArray();
        values[index] = bytes;
      }
      return values;
    }

    private static int GetRegisterMultiplier(uint wordSize)
    {
      switch (wordSize)
      {
        case (16):
          return 1;
        case (32):
          return 2;
        case (64):
          return 4;
        default: throw new ArgumentException("Word size mus be 16/32/64");
      }
    }
  }
}
