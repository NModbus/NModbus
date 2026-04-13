namespace NModbus.Data
{
    /// <summary>
    ///     Specifies the category of device identification objects to read
    ///     (Modbus Application Protocol V1.1b3, section 6.21).
    /// </summary>
    public enum DeviceIdCategory : byte
    {
        /// <summary>Basic identification: Vendor Name, Product Code, Major Minor Revision.</summary>
        Basic = 0x01,

        /// <summary>Regular identification: basic objects plus Vendor URL, Product Name, Model Name, User Application Name.</summary>
        Regular = 0x02,

        /// <summary>Extended identification: all standard objects plus vendor-specific objects (0x80+).</summary>
        Extended = 0x03,

        /// <summary>Read a single specific object by ID.</summary>
        Individual = 0x04
    }
}
