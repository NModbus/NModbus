using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NModbus.Data
{
    /// <summary>
    /// Helper class for easier access to device ID values
    /// </summary>
    public static class DeviceIdentificationHelper
    {
        // Standard object IDs
        public const byte VendorNameId = 0x00;
        public const byte ProductCodeId = 0x01;
        public const byte MajorMinorRevisionId = 0x02;
        public const byte VendorUrlId = 0x03;
        public const byte ProductNameId = 0x04;
        public const byte ModelNameId = 0x05;
        public const byte UserApplicationNameId = 0x06;

        /// <summary>
        /// Gets the vendor name from device identification objects.
        /// </summary>
        public static string GetVendorName(Dictionary<byte, string> objects)
        {
            return objects.ContainsKey(VendorNameId) ? objects[VendorNameId] : null;
        }

        /// <summary>
        /// Gets the product code from device identification objects.
        /// </summary>
        public static string GetProductCode(Dictionary<byte, string> objects)
        {
            return objects.ContainsKey(ProductCodeId) ? objects[ProductCodeId] : null;
        }

        /// <summary>
        /// Gets the version from device identification objects.
        /// </summary>
        public static string GetVersion(Dictionary<byte, string> objects)
        {
            return objects.ContainsKey(MajorMinorRevisionId) ? objects[MajorMinorRevisionId] : null;
        }
    }
}
