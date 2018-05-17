using System;
using NModbus.Logging;

namespace NModbus.IO
{
    internal class EmptyTransport : ModbusTransport
    {
        public EmptyTransport(IModbusFactory modbusFactory) 
            : base(modbusFactory, NullModbusLogger.Instance)
        {
        }

        public override byte[] ReadRequest()
        {
            throw new NotImplementedException();
        }

        public override IModbusMessage ReadResponse<T>()
        {
            throw new NotImplementedException();
        }

        public override byte[] BuildMessageFrame(IModbusMessage message)
        {
            throw new NotImplementedException();
        }

        public override void Write(IModbusMessage message)
        {
            throw new NotImplementedException();
        }

        internal override void OnValidateResponse(IModbusMessage request, IModbusMessage response)
        {
            throw new NotImplementedException();
        }

        

        
    }
}
