using CuttingEdge.Conditions;
using System;
using System.Globalization;

namespace Common.Utils
{
    /// <summary>
    /// Utility class for converting bytes to hex strings and vice versa
    /// </summary>
    public static class HexUtil
    {
        private static readonly int[] _hexValue = new int[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

        /// <summary>
        /// Convert an array of bytes to a hex string (lower case).
        /// e.g.  [0x12, 0xab, 0x4f] becomes "12ab4f"
        /// </summary>
        /// <param name="bytes">0 or more bytes</param>
        /// <returns>lower case hex string</returns>
        public static string ToHex(byte[] bytes)
        {
            Condition.Requires(bytes, nameof(bytes)).IsNotNull();

            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
        }

        /// <summary>
        /// Convert a hex string into an array of bytes
        /// </summary>
        /// <param name="hex">a string containing hex digits [0-9A-Fa-f]</param>
        /// <returns></returns>
        public static byte[] ToBytes(string hex)
        {
            Condition.Requires(hex, nameof(hex)).IsNotNull();
            Condition.Requires(hex, nameof(hex)).Evaluate(hex.Length % 2 == 0, "length of hex must be even");

            byte[] bytes = new byte[hex.Length / 2];

            for (int x = 0, i = 0; i < hex.Length; i += 2, x += 1)
            {
                bytes[x] = (byte)(_hexValue[char.ToUpper(hex[i + 0], CultureInfo.InvariantCulture) - '0'] << 4 |
                                  _hexValue[char.ToUpper(hex[i + 1], CultureInfo.InvariantCulture) - '0']);
            }

            return bytes;
        }
    }
}