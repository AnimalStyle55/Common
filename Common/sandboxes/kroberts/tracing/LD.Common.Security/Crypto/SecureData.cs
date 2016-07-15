using CuttingEdge.Conditions;
using LD.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LD.Common.Security.Crypto
{
    /// <summary>
    /// Implementation of ISecureData
    /// </summary>
    public class SecureData : ISecureData
    {
        private readonly byte[] _key;

        /// <summary>
        /// Securly encrypt data
        /// </summary>
        /// <param name="key">encryption key, 16 hex bytes</param>
        public SecureData(string key)
        {
            Condition.Requires(key, nameof(key)).IsNotNull().Evaluate(key.Length == 32);

            _key = HexUtil.ToBytes(key);
        }

        /// <see cref="ISecureData"/>
        public byte[] Encrypt(byte[] data)
        {
            return (data != null) ? DataEncryption.Encrypt(_key, data) : null;
        }

        /// <see cref="ISecureData"/>
        public byte[] Encrypt(string data)
        {
            return (data != null) ? DataEncryption.Encrypt(_key, Encoding.UTF8.GetBytes(data)) : null;
        }

        /// <see cref="ISecureData"/>
        public string EncryptToBase64(byte[] data)
        {
            return (data != null) ? DataEncryption.EncryptToUrlSafeToken(_key, data) : null;
        }

        /// <see cref="ISecureData"/>
        public string EncryptToBase64(string data)
        {
            return (data != null) ? DataEncryption.EncryptToUrlSafeToken(_key, Encoding.UTF8.GetBytes(data)) : null;
        }

        /// <see cref="ISecureData"/>
        public string EncryptToHex(byte[] data)
        {
            return (data != null) ? HexUtil.ToHex(DataEncryption.Encrypt(_key, data)) : null;
        }

        /// <see cref="ISecureData"/>
        public string EncryptToHex(string data)
        {
            return (data != null) ? HexUtil.ToHex(DataEncryption.Encrypt(_key, Encoding.UTF8.GetBytes(data))) : null;
        }

        /// <see cref="ISecureData"/>
        public byte[] Decrypt(byte[] encrypted)
        {
            return (encrypted != null) ? DataEncryption.Decrypt(_key, encrypted) : null;
        }

        /// <see cref="ISecureData"/>
        public byte[] DecryptBase64(string encrypted)
        {
            return (encrypted != null) ? DataEncryption.DecryptUrlSafeToken(_key, encrypted) : null;
        }

        /// <see cref="ISecureData"/>
        public string DecryptToString(byte[] encrypted)
        {
            return (encrypted != null) ? Encoding.UTF8.GetString(DataEncryption.Decrypt(_key, encrypted)) : null;
        }

        /// <see cref="ISecureData"/>
        public string DecryptBase64ToString(string encrypted)
        {
            return (encrypted != null) ? Encoding.UTF8.GetString(DataEncryption.DecryptUrlSafeToken(_key, encrypted)) : null;
        }

        /// <see cref="ISecureData"/>
        public byte[] DecryptHex(string encrypted)
        {
            return (encrypted != null) ? DataEncryption.Decrypt(_key, HexUtil.ToBytes(encrypted)) : null;
        }

        /// <see cref="ISecureData"/>
        public string DecryptHexToString(string encrypted)
        {
            return (encrypted != null) ? Encoding.UTF8.GetString(DataEncryption.Decrypt(_key, HexUtil.ToBytes(encrypted))) : null;
        }
    }
}