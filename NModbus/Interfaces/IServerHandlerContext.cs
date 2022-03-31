using System;

namespace NModbus
{
    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use IServerHandlerContext instead.")]
    public interface ISlaveHandlerContext : IServerHandlerContext {}
    public interface IServerHandlerContext
    {
        IModbusFunctionService GetHandler(byte functionCode);
    }
}