using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LD.Common.Security.Crypto
{
    internal static class DataEncryption
    {
        private static readonly ILog log = LoanDepotLogManager.GetLogger();

        /// <summary>
        /// Version tag as the first byte in an encrypted token
        /// </summary>
        private const byte VER_HMACSHA256_AESCBC = 0x01;

        /// <summary>
        /// Encrypt bytes into a url-safe token string, suitable for sending out into the world
        /// </summary>
        /// <param name="masterKey">A 128-bit master key for encryption and HMAC</param>
        /// <param name="plainText">the content</param>
        /// <returns>the token as a string</returns>
        public static string EncryptToUrlSafeToken(byte[] masterKey, byte[] plainText)
        {
            return HttpServerUtility.UrlTokenEncode(Encrypt(masterKey, plainText)); // url safe modified base64
        }

        /// <summary>
        /// Encrypt bytes, suitable for sending out into the world
        /// </summary>
        /// <param name="masterKey">A 128-bit master key for encryption and HMAC</param>
        /// <param name="plainText">the content</param>
        /// <returns>the token as a string</returns>
        public static byte[] Encrypt(byte[] masterKey, byte[] plainText)
        {
            byte[] iv = Primitives.GetSecureRandomBytes(Primitives.AesCbcIvSizeBytes);

            // derive encryption and hmac keys from the master key, using iv as salt
            // key1 is encryption, key2 is hmac
            var keys = deriveKeys(masterKey, iv);

            using (var buffer = new MemoryStream(192))
            {
                buffer.WriteByte(VER_HMACSHA256_AESCBC);

                buffer.Write(iv, 0, iv.Length);

                byte[] cipher = Primitives.EncryptAES_CBC(keys.Item1, plainText, iv);
                buffer.Write(cipher, 0, cipher.Length);

                byte[] hmac = Primitives.HmacSHA256(buffer.ToArray(), keys.Item2);
                buffer.Write(hmac, 0, hmac.Length);

                return buffer.ToArray();
            }
        }

        /// <summary>
        /// Decrypt bytes that were previously encrypted
        /// </summary>
        /// <param name="masterKey">A 128-bit master key for HMAC and decryption</param>
        /// <param name="data">the token</param>
        /// <returns>the decrypted bytes, or null if token was not valid</returns>
        public static byte[] Decrypt(byte[] masterKey, byte[] data)
        {
            if (data == null || data.Length < (1 + Primitives.AesCbcIvSizeBytes + Primitives.AesCbcBlockSizeBytes + Primitives.Sha256SizeBytes))
            {
                log.Error("invalid token, data not long enough to be valid, possible tampering attempt");
                return null;
            }

            if (data[0] != VER_HMACSHA256_AESCBC)
            {
                log.ErrorFormat("invalid token version: {0}", data[0]);
                return null;
            }

            byte[] iv = new byte[Primitives.AesCbcIvSizeBytes];
            Array.Copy(data, 1, iv, 0, iv.Length);

            // derive encryption and hmac keys from the master key, using iv as salt
            // key1 is encryption, key2 is hmac
            var keys = deriveKeys(masterKey, iv);

            int hmacOffset = data.Length - Primitives.Sha256SizeBytes;
            int cipherLength = hmacOffset;
            if (!Primitives.VerifyHmacSHA256(data, hmacOffset, 0, cipherLength, keys.Item2))
            {
                log.Warn("token failed hmac validation!");
                return null;
            }

            return Primitives.DecryptAES_CBC(keys.Item1, data, 1 + iv.Length, cipherLength - 1 - iv.Length, iv);
        }

        /// <summary>
        /// Decrypt a url-safe token that was previously encrypted
        /// </summary>
        /// <param name="masterKey">A 128-bit master key for HMAC and decryption</param>
        /// <param name="token">the token</param>
        /// <returns>the decrypted bytes, or null if token was not valid</returns>
        public static byte[] DecryptUrlSafeToken(byte[] masterKey, string token)
        {
            try
            {
                return Decrypt(masterKey, HttpServerUtility.UrlTokenDecode(token)); // url safe modified base64
            }
            catch (FormatException e)
            {
                log.DebugFormat("token is not valid url-base64: {0}", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Derive 2 keys from a master key with a given salt
        /// </summary>
        /// <remarks>
        /// The purpose of this is to use 2 different keys, 1 for encryption and 1 for HMAC
        /// 2 keys can be safely derived from a single key using PBKDF2.
        /// see Crypto.HmacPBKDF2() for why we generate a key prime and then regenerate a double sized key
        /// </remarks>
        /// <param name="masterKey"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        private static Tuple<byte[], byte[]> deriveKeys(byte[] masterKey, byte[] salt)
        {
            byte[] keyPrime = Primitives.HmacPBKDF2(masterKey, salt, Primitives.AesCbcBlockSizeBytes, 200);
            byte[] keys = Primitives.HmacPBKDF2(keyPrime, salt, Primitives.AesCbcBlockSizeBytes * 2, 200);
            byte[] key1 = new byte[Primitives.AesCbcBlockSizeBytes];
            byte[] key2 = new byte[Primitives.AesCbcBlockSizeBytes];
            Array.Copy(keys, 0, key1, 0, Primitives.AesCbcBlockSizeBytes);
            Array.Copy(keys, 16, key2, 0, Primitives.AesCbcBlockSizeBytes);

            return Tuple.Create(key1, key2);
        }
    }
}