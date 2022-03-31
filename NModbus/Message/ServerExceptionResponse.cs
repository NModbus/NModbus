using System;
using System.Collections.Generic;
using System.Globalization;

namespace NModbus.Message
{
    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ServerExceptionResponse instead.")]
    internal class SlaveExceptionResponse : ServerExceptionResponse
    {

        // ReSharper disable once RedundantOverriddenMember : satisfying unit test
        public override string ToString() { return base.ToString(); }
    }

    internal class ServerExceptionResponse : AbstractModbusMessage, IModbusMessage
    {
        private static readonly Dictionary<byte, string> _exceptionMessages = CreateExceptionMessages();

        public ServerExceptionResponse()
        {
        }

        public ServerExceptionResponse(byte serverAddress, byte functionCode, byte exceptionCode)
            : base(serverAddress, functionCode)
        {
            ServerExceptionCode = exceptionCode;
        }

        public override int MinimumFrameSize => 3;

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ServerExceptionCode instead.")]
        public byte SlaveExceptionCode
        {
            get => ServerExceptionCode;
            set => ServerExceptionCode = value;
        }
        public byte ServerExceptionCode
        {
            get => MessageImpl.ExceptionCode.Value;
            set => MessageImpl.ExceptionCode = value;
        }

        /// <summary>
        ///     Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            string msg = _exceptionMessages.ContainsKey(ServerExceptionCode)
                ? _exceptionMessages[ServerExceptionCode]
                : Resources.Unknown;

            return string.Format(
                CultureInfo.InvariantCulture,
                Resources.ServerExceptionResponseFormat,
                Environment.NewLine,
                FunctionCode,
                ServerExceptionCode,
                msg);
        }

        internal static Dictionary<byte, string> CreateExceptionMessages()
        {
            return new Dictionary<byte, string>(9)
            {
                {1, Resources.IllegalFunction},
                {2, Resources.IllegalDataAddress},
                {3, Resources.IllegalDataValue},
                {4, Resources.ServerDeviceFailure},
                {5, Resources.Acknowlege},
                {6, Resources.ServerDeviceBusy},
                {8, Resources.MemoryParityError},
                {10, Resources.GatewayPathUnavailable},
                {11, Resources.GatewayTargetDeviceFailedToRespond}
            };
        }

        protected override void InitializeUnique(byte[] frame)
        {
            if (FunctionCode <= Modbus.ExceptionOffset)
            {
                throw new FormatException(Resources.ServerExceptionResponseInvalidFunctionCode);
            }

            ServerExceptionCode = frame[2];
        }
    }
}
