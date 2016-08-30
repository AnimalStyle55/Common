using CuttingEdge.Conditions;
using Common.Utils;
using System.Collections.Generic;
using System.IO;

namespace Common.Security.Crypto
{
    /// <summary>
    /// Implementation of ISecureHash
    /// </summary>
    public class SecureHash : ISecureHash
    {
        /// <summary>
        /// The Iteration version for the Password Iterations
        /// </summary>
        public const byte CurrentIterationVersion = 2;

        /// <summary>
        /// The default iterations for predictable hashes
        /// </summary>
        public const int CurrentPredictableIterations = 100000;

        private readonly byte[] _predictableKey;
        private readonly int _predictableIterations;

        private const int PasswordSaltBytes = 20;
        private const int PasswordHashLength = 2 + PasswordSaltBytes * 2 + Primitives.Sha1SizeBytes * 2;
        private readonly byte _iterationVersion;
        private readonly static Dictionary<byte, int> _iterationsVerMap;

        static SecureHash()
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
        /// Securly encrypt data
        /// </summary>
        /// <param name="passwordIterVer">version indicating the of hashing rounds for passwords</param>
        /// <param name="predictableHashKey">key for predictable hashes, 16 hex bytes</param>
        /// <param name="predictableIterations"># of iteration rounds for predictable hashes</param>
        public SecureHash(byte passwordIterVer, string predictableHashKey, int predictableIterations)
        {
            Condition.Requires(predictableHashKey, nameof(predictableHashKey)).IsNotNull().Evaluate(predictableHashKey.Length == 32);
            Condition.Requires(predictableIterations, nameof(predictableIterations)).IsGreaterThan(0);
            Condition.Requires(passwordIterVer, nameof(passwordIterVer)).Evaluate(_iterationsVerMap.ContainsKey(passwordIterVer));

            _predictableKey = HexUtil.ToBytes(predictableHashKey);
            _predictableIterations = predictableIterations;

            _iterationVersion = passwordIterVer;
        }

        /// <summary>
        /// Securly encrypt data
        /// </summary>
        /// <param name="predictableHashKey">key for predictable hashes, 16 hex bytes</param>
        public SecureHash(string predictableHashKey) 
            : this(CurrentIterationVersion, predictableHashKey, CurrentPredictableIterations)
        {
        }

        /// <see cref="ISecureHash"/>
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

        /// <see cref="ISecureHash"/>
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

        /// <see cref="ISecureHash"/>
        public string PredictableHash(string value)
        {
            return HexUtil.ToHex(Primitives.HmacPBKDF2(value, _predictableKey, _predictableIterations));
        }
    }
}
