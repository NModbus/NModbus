using System;
using System.Collections.Generic;

namespace Modbus.UnitTests.Message
{
    public static class MessageUtility
    {
        /// <summary>
        ///     Creates a collection initialized to a default value.
        /// </summary>
        public static T CreateDefaultCollection<T, V>(V defaultValue, int size)
            where T : ICollection<V>, new()
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException("Collection size cannot be less than 0.");
            }

            T col = new T();

            for (int i = 0; i < size; i++)
            {
                col.Add(defaultValue);
            }

            return col;
        }
    }
}