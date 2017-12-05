using System;
using System.Linq;
using NModbus.Utility;

namespace NModbus.Extensions
{
    public static class CrcExtensions
    {
        /// <summary>
        /// Determines whether the crc stored in the message matches the calculated crc.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool DoesCrcMatch(this byte[] message)
        {
            var messageFrame = message.Take(message.Length - 2).ToArray();

            //Calculate the CRC with the given set of bytes
            var calculatedCrc = BitConverter.ToUInt16(ModbusUtility.CalculateCrc(messageFrame), 0);

            //Get the crc that is stored in the message
            var messageCrc = message.GetCRC();

            //Determine if they match
            return calculatedCrc == messageCrc;
        }

        /// <summary>
        /// Gets the CRC of the message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ushort GetCRC(this byte[] message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (message.Length < 4)
                throw new ArgumentException("message must be at least four bytes long");

            return BitConverter.ToUInt16(message, message.Length - 2);
        }
    }
}