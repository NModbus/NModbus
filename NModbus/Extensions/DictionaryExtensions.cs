using System.Collections.Generic;

namespace NModbus.Extensions
{
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// Gets the specified value in the dictionary. If not found, returns default for TValue.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;

            if (dictionary.TryGetValue(key, out value))
                return value;

            return default(TValue);
        }
    }
}