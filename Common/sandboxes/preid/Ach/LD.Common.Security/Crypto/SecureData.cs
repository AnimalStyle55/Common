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
        /// The Iteration version for the Password Iterations
        /// </summary>
        public const int CurrentIterationVersion = 2;

        private const int PasswordSaltBytes = 20;
        private const int PasswordHashLength = 2 + PasswordSaltBytes * 2 + Primitives.Sha1SizeBytes * 2;
        private readonly byte _iterationVersion;
        private readonly static Dictionary<byte, int> _iterationsVerMap;

        static SecureData()
        {
            // the actual iterations are mapped to a version here.
            // the version gets encoded into the password hash as the first 1 byte
            // this allows us to keep the iterations secret, but also be able to change them later
            _iterationsVerMap = new Dictionary<byte, int>()
            {
                {0xff, 500 },   // for unit tests
                {   1, 50000 },
                {   2, 100000 } // current 2016 recommendation is at least 100K
            };
        }

        /// <summary>
        /// Securly handle data, use default iterations for passwords
        /// </summary>
        /// <param name="key">encryption key, 16 hex bytes</param>
        public SecureData(string key) : this(key, CurrentIterationVersion)
        {
        }

        /// <summary>
        /// Securly handle data
        /// </summary>
        /// <param name="key">encryption key, 16 hex bytes</param>
        /// <param name="iterationVersion">version id for iteration, defaults to current</param>
        public SecureData(string key, byte iterationVersion)
        {
            Condition.Requires(key, nameof(key)).IsNotNull().Evaluate(key.Length == 32);

            _key = HexUtil.ToBytes(key);
            _iterationVersion = iterationVersion;
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
        public string EncryptToString(byte[] data)
        {
            return (data != null) ? DataEncryption.EncryptToUrlSafeToken(_key, data) : null;
        }

        /// <see cref="ISecureData"/>
        public string EncryptToString(string data)
        {
            return (data != null) ? DataEncryption.EncryptToUrlSafeToken(_key, Encoding.UTF8.GetBytes(data)) : null;
        }

        /// <see cref="ISecureData"/>
        public byte[] Decrypt(byte[] encrypted)
        {
            return (encrypted != null) ? DataEncryption.Decrypt(_key, encrypted) : null;
        }

        /// <see cref="ISecureData"/>
        public byte[] Decrypt(string encrypted)
        {
            return (encrypted != null) ? DataEncryption.DecryptUrlSafeToken(_key, encrypted) : null;
        }

        /// <see cref="ISecureData"/>
        public string DecryptToString(byte[] encrypted)
        {
            return (encrypted != null) ? Encoding.UTF8.GetString(DataEncryption.Decrypt(_key, encrypted)) : null;
        }

        /// <see cref="ISecureData"/>
        public string DecryptToString(string encrypted)
        {
            return (encrypted != null) ? Encoding.UTF8.GetString(DataEncryption.DecryptUrlSafeToken(_key, encrypted)) : null;
        }

        /// <see cref="ISecureData"/>
        public string HashPassword(string password)
        {
            var salt = Primitives.GetSecureRandomBytes(PasswordSaltBytes);

            using (var buffer = new MemoryStream(PasswordHashLength / 2))
            {
                buffer.WriteByte(_iterationVersion);
                buffer.Write(salt, 0, salt.Length);

                var hash = Primitives.HmacPBKDF2(password, salt, _iterationsVerMap[_iterationVersion]);
                buffer.Write(hash, 0, hash.Length);

                return HexUtil.ToHex(buffer.ToArray());
            }
        }

        /// <see cref="ISecureData"/>
        public bool VerifyPasswordHash(string password, string hashedPassword)
        {
            if (hashedPassword == null || hashedPassword.Length != PasswordHashLength)
                return false;

            var iterVer = HexUtil.ToBytes(hashedPassword.Substring(0, 2))[0];
            var salt = HexUtil.ToBytes(hashedPassword.Substring(2, PasswordSaltBytes * 2));
            var hash = hashedPassword.Substring(2 + PasswordSaltBytes * 2);

            int iters;
            if (!_iterationsVerMap.TryGetValue(iterVer, out iters))
                return false;

            var testHash = Primitives.HmacPBKDF2(password, salt, iters);

            return Primitives.SecureEquals(HexUtil.ToBytes(hash), testHash);
        }
    }
}