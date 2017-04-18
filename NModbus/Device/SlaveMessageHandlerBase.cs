using System;
using NModbus.Data;
using NModbus.Interfaces;
using NModbus.Message;

namespace NModbus.Device
{
    /// <summary>
    /// Base class for 
    /// </summary>
    /// <typeparam name="TRequest">The type of request to handle.</typeparam>
    /// <typeparam name="TDataStore">The type of data store to operate on.</typeparam>
    public abstract class SlaveMessageHandlerBase<TRequest, TDataStore> : ISlaveMessageHandler
        where TRequest : class
        where TDataStore : class, ISlaveDataStore
    {
        private readonly byte _functionCode;
        private readonly TDataStore _dataStore;

        protected SlaveMessageHandlerBase(byte functionCode, TDataStore dataStore)
        {
            _functionCode = functionCode;
            _dataStore = dataStore;
        }

        public byte FunctionCode
        {
            get { return _functionCode; }
        }

        public IModbusMessage Handle(IModbusMessage request, ISlaveDataStore dataStore)
        {
            //Attempt to cast the message
            TRequest typedRequest = request as TRequest;
            TDataStore typedDataStore = dataStore as TDataStore;

            if (typedRequest == null)
                throw new InvalidOperationException($"Unable to cast request of type '{request.GetType().Name}' to type '{typeof(TRequest).Name}");

            if (typedDataStore == null)
                throw new InvalidOperationException($"Unable to cast data store of type '{dataStore.GetType().Name}' to type '{typeof(TDataStore).Name}");

            //Do the implementation specific logic
            return Handle(typedRequest, typedDataStore);
        }

        protected abstract IModbusMessage Handle(TRequest request, TDataStore dataStore);
    }
}