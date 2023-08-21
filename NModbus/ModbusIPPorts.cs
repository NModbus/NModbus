namespace NModbus
{
    public static class ModbusIPPorts
    {
        /// <summary>
        /// 502: mbap/TCP
        /// </summary>
        public const int Insecure = 502;

        /// <summary>
        /// 802: mbap/TLS/TCP
        /// </summary>
        public const int Secure = 802;
    }
}
