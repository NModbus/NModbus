namespace NModbus.Extensions.Functions
{
  using System;

  /// <summary>
  /// Class containing functions to covert endian from network device to host this code is running on.
  /// </summary>
  public class Endian
  {
    /// <summary>
    /// Converts BigEndian source bytes to Endian format of system.
    /// Source BE: 0x0A,0x0B,0x0C,0x0D. 
    /// Target BE: 0x0A,0x0B,0x0C,0x0D.
    /// Target LE: 0x0D,0x0C,0x0B,0x0A.
    /// </summary>
    /// <param name="sourceBytes">Byte array from device</param>
    /// <returns>Bytes in Endian format for system</returns>
    public static byte[] BigEndian(byte[] sourceBytes)
    {
      if (BitConverter.IsLittleEndian)
      {
        Array.Reverse(sourceBytes);
      }
      return sourceBytes;
    }

    /// <summary>
    /// Converts LittleEndian source bytes to Endian format of system. 
    /// Source LE: 0x0D,0x0C,0x0B,0x0A. 
    /// Target BE: 0x0A,0x0B,0x0C,0x0D.
    /// Target LE: 0x0D,0x0C,0x0B,0x0A.
    /// </summary>
    /// <param name="sourceBytes">Byte array from device</param>
    /// <returns>Bytes in Endian format for system</returns>
    public static byte[] LittleEndian(byte[] sourceBytes)
    {
      if (!BitConverter.IsLittleEndian)
      {
        Array.Reverse(sourceBytes);
      }
      return sourceBytes;
    }

  }
}
