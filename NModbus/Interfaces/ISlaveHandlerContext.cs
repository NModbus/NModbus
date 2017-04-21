namespace NModbus.Interfaces
{
    public interface ISlaveHandlerContext
    {
        IModbusFunctionService GetHandler(byte functionCode);
    }
}