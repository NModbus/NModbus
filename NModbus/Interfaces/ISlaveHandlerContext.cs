namespace NModbus
{
    public interface ISlaveHandlerContext
    {
        IModbusFunctionService GetHandler(byte functionCode);
    }
}