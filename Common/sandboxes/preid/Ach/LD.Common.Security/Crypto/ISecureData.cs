using System;
using System.Collections.Generic;
using System.Linq;

namespace LD.Common.Security.Crypto
{
    /// <summary>
    /// Securly Encrypt and Decrypt Data
    /// </summary>
    public interface ISecureData
    {
        /// <summary>
        /// Encrypt bytes to bytes
        /// </summary>
        byte[] Encrypt(byte[] data);

        /// <summary>
        /// Encrypt string to bytes
        /// </summary>
        byte[] Encrypt(string data);

        /// <summary>
        /// Encrypt bytes to Url-Safe Base64 Encoded string
        /// </summary>
        string EncryptToString(byte[] data);

        /// <summary>
        /// Encrypt string to Url-Safe Base64 Encoded string
        /// </summary>
        string EncryptToString(string data);

        /// <summary>
        /// Decrypt bytes to bytes
        /// </summary>
        byte[] Decrypt(byte[] encrypted);

        /// <summary>
        /// Decrypt Url-Safe Base64 Encoded string to bytes
        /// </summary>
        byte[] Decrypt(string encrypted);

        /// <summary>
        /// Decrypt bytes to string
        /// </summary>
        string DecryptToString(byte[] encrypted);

        /// <summary>
        /// Decrypt Url-Safe Base64 Encoded string to string
        /// </summary>
        string DecryptToString(string encrypted);

        /// <summary>
        /// Securly Hash a password into a one-way hashed value
        /// </summary>
        /// <param name="password">password from user</param>
        string HashPassword(string password);

        /// <summary>
        /// Verify a password against the hash
        /// </summary>
        /// <param name="password">password from user</param>
        /// <param name="hash">hash from storage</param>
        /// <returns>true if password matches, false otherwise</returns>
        bool VerifyPasswordHash(string password, string hash);
    }
}