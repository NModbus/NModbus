using System;

namespace NModbus.Device
{
    /// <summary>
    /// Base class for 
    /// </summary>
    /// <typeparam name="TRequest">The type of request to handle.</typeparam>
    internal abstract class ModbusFunctionServiceBase<TRequest> : IModbusFunctionService
        where TRequest : class
    {
        private readonly byte _functionCode;

        protected ModbusFunctionServiceBase(byte functionCode)
        {
            _functionCode = functionCode;
        }

        public byte FunctionCode => _functionCode;

        public abstract IModbusMessage CreateRequest(byte[] frame);

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use HandleServerRequest instead.")]
        public IModbusMessage HandleSlaveRequest(IModbusMessage request, ISlaveDataStore dataStore)
        {
            return HandleServerRequest(request, dataStore as IServerDataStore);
        }

        public IModbusMessage HandleServerRequest(IModbusMessage request, IServerDataStore dataStore)
        {
            //Attempt to cast the message
            TRequest typedRequest = request as TRequest;

            if (typedRequest == null)
                throw new InvalidOperationException($"Unable to cast request of type '{request.GetType().Name}' to type '{typeof(TRequest).Name}");

            //Do the implementation specific logic
            return Handle(typedRequest, dataStore);
        }

        public abstract int GetRtuRequestBytesToRead(byte[] frameStart);

        public abstract int GetRtuResponseBytesToRead(byte[] frameStart);

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use Handle with IServerDataStore parameter instead.")]
        protected abstract IModbusMessage Handle(TRequest request, ISlaveDataStore dataStore);
        protected abstract IModbusMessage Handle(TRequest request, IServerDataStore dataStore);

        protected static T CreateModbusMessage<T>(byte[] frame)
            where T : IModbusMessage, new()
        {
            IModbusMessage message = new T();
            message.Initialize(frame);

            return (T)message;
        }
    }
}