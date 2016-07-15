using CuttingEdge.Conditions;
using LD.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace LD.Common.Security.Crypto
{
    /// <summary>
    /// Utility class containing cryptography primitives for use by other projects
    /// </summary>
    internal static class Primitives
    {
        public const int Sha256SizeBytes = 32;
        public const int AesCbcIvSizeBytes = 16;
        public const int AesCbcBlockSizeBytes = 16;
        public const int Sha1SizeBytes = 20;

        /// <summary>
        /// Generate secure random bytes suitable for cryptography
        /// </summary>
        /// <param name="nBytes"></param>
        /// <returns>array of bytes</returns>
        public static byte[] GetSecureRandomBytes(int nBytes)
        {
            Condition.Requires(nBytes, nameof(nBytes)).IsGreaterOrEqual(0);

            if (nBytes == 0)
                return new byte[0];

            var rng = GetSecureRNG();
            byte[] val = new byte[nBytes];
            rng.GetBytes(val);
            return val;
        }

        /// <summary>
        /// Get a secure random number generator
        /// </summary>
        /// <returns></returns>
        public static RandomNumberGenerator GetSecureRNG()
        {
            return new RNGCryptoServiceProvider();
        }

        /// <summary>
        /// Securely hash some plain text using PBKDF2/HMAC-SHA1
        /// </summary>
        /// <remarks>http://en.wikipedia.org/wiki/PBKDF2</remarks>
        /// <param name="password">the secret value, used to derive the key</param>
        /// <param name="salt">the message for the HMAC calculation, should be at least 128-bits</param>
        /// <param name="iterations">how 'slow' you want the hash.  More is more secure.  100K for Passwords (as of 2016)</param>
        /// <returns></returns>
        public static byte[] HmacPBKDF2(string password, byte[] salt, int iterations)
        {
            Condition.Requires(password, "password").IsNotNull();
            Condition.Requires(salt, "salt").IsNotNull();
            Condition.Requires(iterations, "iterations").IsGreaterOrEqual(1);

            using (Rfc2898DeriveBytes hasher = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return hasher.GetBytes(Sha1SizeBytes);
            }
        }

        /// <summary>
        /// Securely stretch a key using PBKDF2/HMAC
        /// </summary>
        /// <remarks>
        /// http://en.wikipedia.org/wiki/PBKDF2
        /// To properly use this to do key stretching when you need more than 16 bytes is
        /// keyPrime = HmacPBKDF2(key, salt, 16)
        /// keys = HmacPBKDF2(keyPrime, salt, 32)
        /// key1 = keys[0..15]
        /// key2 = keys[16..31]
        /// </remarks>
        /// <param name="key">the secret value, used to derive the key</param>
        /// <param name="salt">the message for the HMAC calculation, should be at least 128-bits</param>
        /// <param name="iterations">how 'slow' you want the hash.  More is more secure, but slower.</param>
        /// <param name="outputBytes">how many bytes of output you need</param>
        /// <returns></returns>
        public static byte[] HmacPBKDF2(byte[] key, byte[] salt, int outputBytes, int iterations)
        {
            Condition.Requires(key, "password").IsNotNull();
            Condition.Requires(salt, "salt").IsNotNull();
            Condition.Requires(iterations, "iterations").IsGreaterOrEqual(1);

            using (Rfc2898DeriveBytes hasher = new Rfc2898DeriveBytes(key, salt, iterations))
            {
                return hasher.GetBytes(outputBytes);
            }
        }

        /// <summary>
        /// Encrypt some plaintext using AES-128/192/256-CBC
        /// </summary>
        /// <param name="key">a 128/192/256 bit key</param>
        /// <param name="plaintext">byte array</param>
        /// <param name="iv">a 128-bit initialization vector</param>
        /// <returns>an encrypted ciphertext</returns>
        public static byte[] EncryptAES_CBC(byte[] key, byte[] plaintext, byte[] iv)
        {
            Condition.Requires(plaintext, "plaintext").IsNotNull().IsLongerThan(0);
            Condition.Requires(key, "key").IsNotNull().Evaluate(key.Length == 16 || key.Length == 24 || key.Length == 32, "128/192/256 bits required");
            Condition.Requires(iv, "iv").IsNotNull().HasLength(AesCbcIvSizeBytes, "128-bits required");

            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.KeySize = key.Length * 8;
                aesAlg.BlockSize = AesCbcBlockSizeBytes * 8;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(key, iv);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plaintext, 0, plaintext.Length);
                    }

                    return msEncrypt.ToArray();
                }
            }
        }

        /// <summary>
        /// Decrypt a ciphertext using AES-128/192/256-CBC
        /// </summary>
        /// <param name="key">a 128/192/256 bit key</param>
        /// <param name="cipherText">byte array</param>
        /// <param name="iv">a 128-bit initialization vector</param>
        /// <returns>the decrypted plaintext</returns>
        public static byte[] DecryptAES_CBC(byte[] key, byte[] cipherText, byte[] iv)
        {
            Condition.Requires(cipherText, "ciphertext").IsNotNull().IsLongerThan(0);

            return DecryptAES_CBC(key, cipherText, 0, cipherText.Length, iv);
        }

        /// <summary>
        /// Decrypt a ciphertext using AES-128/192/256-CBC from a buffer
        /// </summary>
        /// <param name="key">a 128/192/256 bit key</param>
        /// <param name="buffer">byte array</param>
        /// <param name="cipherOffset">where the cipher is in buffer</param>
        /// <param name="cipherLength">how long the cipher is</param>
        /// <param name="iv">a 128-bit initialization vector</param>
        /// <returns>the decrypted plaintext</returns>
        public static byte[] DecryptAES_CBC(byte[] key, byte[] buffer, int cipherOffset, int cipherLength, byte[] iv)
        {
            Condition.Requires(buffer, "buffer").IsNotNull().IsLongerOrEqual(cipherLength);
            Condition.Requires(key, "key").IsNotNull().Evaluate(key.Length == 16 || key.Length == 24 || key.Length == 32, "128/192/256 bits required");
            Condition.Requires(iv, "iv").IsNotNull().HasLength(AesCbcIvSizeBytes, "128-bits required");

            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.KeySize = key.Length * 8;
                aesAlg.BlockSize = AesCbcBlockSizeBytes * 8;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(key, iv);

                using (MemoryStream msDecrypt = new MemoryStream(buffer, cipherOffset, cipherLength))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (MemoryStream output = new MemoryStream())
                    {
                        byte[] decryptBuffer = new byte[cipherLength > 256 ? 256 : cipherLength];
                        int bytes;
                        while ((bytes = csDecrypt.Read(decryptBuffer, 0, decryptBuffer.Length)) > 0)
                            output.Write(decryptBuffer, 0, bytes);

                        return output.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Compute an HMAC-SHA256 hash for a ciphertext and a key
        /// </summary>
        /// <param name="cipherText">the encrypted message (for CBC, should include version,IV,cipherText)</param>
        /// <param name="key">key used to compute HMAC</param>
        /// <remarks>
        /// When using HMAC for message authentication, be sure to
        /// a) encrypt then HMAC
        /// b) include all data in the HMAC (version, IV, cipherText)
        /// </remarks>
        /// <returns>the hash (32 bytes)</returns>
        public static byte[] HmacSHA256(byte[] cipherText, byte[] key)
        {
            Condition.Requires(key, "key").IsNotNull().IsLongerThan(0);
            Condition.Requires(cipherText, "cipherText").IsNotNull().IsLongerThan(0);

            using (HMACSHA256 hasher = new HMACSHA256(key))
            {
                return hasher.ComputeHash(cipherText);
            }
        }

        /// <summary>
        /// Confirm that a given hmac is valid for the ciphertext
        /// </summary>
        /// <param name="hmac">the hmac sent in the message</param>
        /// <param name="cipherText">the encrypted message</param>
        /// <param name="key">key used to compute HMAC</param>
        /// <returns>true if valid, false if not</returns>
        public static bool VerifyHmacSHA256(byte[] hmac, byte[] cipherText, byte[] key)
        {
            return SecureEquals(HmacSHA256(cipherText, key), hmac);
        }

        /// <summary>
        /// Confirm that a given hmac is valid for the ciphertext
        /// </summary>
        /// <param name="buffer">buffer containing cipher and hmac</param>
        /// <param name="hmacOffset">where the hmac is in the buffer</param>
        /// <param name="cipherOffset">where the cipher is in the buffer</param>
        /// <param name="cipherLength">how long the cipher is</param>
        /// <param name="key">key used to compute HMAC</param>
        /// <returns>true if valid, false if not</returns>
        public static bool VerifyHmacSHA256(byte[] buffer, int hmacOffset, int cipherOffset, int cipherLength, byte[] key)
        {
            Condition.Requires(key, "key").IsNotNull().IsLongerThan(0);
            Condition.Requires(buffer, "buffer").IsNotNull().IsLongerOrEqual(cipherLength + Sha256SizeBytes);

            using (HMACSHA256 hasher = new HMACSHA256(key))
            {
                byte[] computedHmac = hasher.ComputeHash(buffer, cipherOffset, cipherLength);

                return SecureEquals(computedHmac, 0, computedHmac.Length, buffer, hmacOffset, Sha256SizeBytes);
            }
        }

        /// <summary>
        /// Do a length constant comparison of two byte arrays
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <remarks>read about length constant comparison here
        /// https://crackstation.net/hashing-security.htm
        /// Essentially, the fact that if the hash doesn't match, normal '==' will return faster
        /// This makes every comparison take the same time regardless of equality
        /// </remarks>
        /// <returns></returns>
        public static bool SecureEquals(byte[] a, byte[] b)
        {
            return SecureEquals(a, 0, a.Length, b, 0, b.Length);
        }

        /// <summary>
        /// Do a length constant comparison of two byte arrays
        /// </summary>
        /// <seealso cref="SecureEquals(byte[], byte[])"/>
        /// <param name="a"></param>
        /// <param name="a_offset"></param>
        /// <param name="a_length"></param>
        /// <param name="b"></param>
        /// <param name="b_offset"></param>
        /// <param name="b_length"></param>
        /// <returns></returns>
        public static bool SecureEquals(byte[] a, int a_offset, int a_length, byte[] b, int b_offset, int b_length)
        {
            Condition.Requires(a, "a").IsNotNull();
            Condition.Requires(b, "b").IsNotNull();
            Condition.Requires(a_offset, "a_offset").IsGreaterOrEqual(0).IsLessOrEqual(a.Length - a_length);
            Condition.Requires(a_length, "a_length").IsGreaterOrEqual(0);
            Condition.Requires(b_offset, "b_offset").IsGreaterOrEqual(0).IsLessOrEqual(b.Length - b_length);
            Condition.Requires(b_length, "b_length").IsGreaterOrEqual(0);

            uint diff = (uint)a_length ^ (uint)b_length;
            for (int i = 0; i < a_length && i < b_length; i++)
                diff |= (uint)(a[i + a_offset] ^ b[i + b_offset]);
            return diff == 0;
        }
    }
}