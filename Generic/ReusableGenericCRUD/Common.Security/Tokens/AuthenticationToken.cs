using Common.Logging;
using Common.Security.Crypto;
using Common.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Security.Tokens
{
    /// <summary>
    /// Represents an authentication token that is securely encrypted,
    /// and contains a subject, start/end times, and arbitrary data fields
    ///
    /// Loosely based on JWT
    /// </summary>
    public class AuthenticationToken
    {
        private static readonly ILog log = CommonLogManager.GetLogger();

        /// <summary>Encrypted token</summary>
        public string Token { get; private set; }

        /// <summary>When the token expires</summary>
        public DateTime Expires { get; private set; }

        /// <summary>When the token was issued</summary>
        public DateTime Issued { get; private set; }

        /// <summary>guid key for the token</summary>
        public Guid SubjectGuid { get; private set; }

        /// <summary>custom data values, may be null, must be serializable to JSON</summary>
        public IReadOnlyDictionary<string, object> Data { get; private set; }

        /// <summary>
        /// Determine if the token is expired
        /// </summary>
        public bool IsExpired
        {
            get
            {
                return Expires < DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Construct a new token
        /// </summary>
        /// <param name="masterKey">key used to derive other keys</param>
        /// <param name="subjectGuid">the guid for the subject (user or app)</param>
        /// <param name="lifetime">how long the token is valid</param>
        /// <param name="dataItems">optional custom data values, must be serializable to JSON</param>
        public AuthenticationToken(byte[] masterKey, Guid subjectGuid, TimeSpan lifetime, params Tuple<string, object>[] dataItems)
            : this(masterKey, subjectGuid, lifetime, dataItems.ToDictionary(k => k.Item1, k => k.Item2))
        {
        }

        /// <summary>
        /// Construct a new token
        /// </summary>
        /// <param name="masterKey">key used to derive other keys</param>
        /// <param name="subjectGuid">the guid for the subject (user or app)</param>
        /// <param name="lifetime">how long the token is valid</param>
        /// <param name="data">optional custom data values, must be serializable to JSON</param>
        public AuthenticationToken(byte[] masterKey, Guid subjectGuid, TimeSpan lifetime, Dictionary<string, object> data = null)
        {
            // get issued and expires, but remove sub-second values
            DateTimeOffset issued = DateTimeOffset.UtcNow;
            DateTimeOffset expires = issued.Add(lifetime);

            var token = new TokenContents()
            {
                sub = Convert.ToBase64String(subjectGuid.ToByteArray()),
                exp = expires.ToUnixTimeSeconds(),
                iat = issued.ToUnixTimeSeconds(),
                data = data
            };

            string tokenContents = JsonUtil.SerializeObject(token);

            log.DebugFormat("the token contents: {0}", tokenContents);

            Token = DataEncryption.EncryptToUrlSafeToken(masterKey, Encoding.UTF8.GetBytes(tokenContents));
            Expires = expires.UtcDateTime;
            Issued = issued.UtcDateTime;
            SubjectGuid = subjectGuid;
            Data = data;
        }

        /// <summary>
        /// Construct a token from an existing encrypted token
        /// </summary>
        /// <param name="masterKey">key used to derive other keys</param>
        /// <param name="encryptedToken">token from user</param>
        /// <exception cref="InvalidTokenException">if token has been tampered with</exception>
        public AuthenticationToken(byte[] masterKey, string encryptedToken)
        {
            byte[] decryptedToken = DataEncryption.DecryptUrlSafeToken(masterKey, encryptedToken);

            if (decryptedToken == null)
            {
                throw new InvalidTokenException("token could not be decrypted");
            }

            var tokenContents = Encoding.UTF8.GetString(decryptedToken);
            log.DebugFormat("the token contents: {0}", tokenContents);

            var authToken = JsonUtil.DeserializeObject<TokenContents>(tokenContents);

            Issued = DateTimeOffset.FromUnixTimeSeconds(authToken.iat).UtcDateTime;
            Expires = DateTimeOffset.FromUnixTimeSeconds(authToken.exp).UtcDateTime;
            var tokenAge = DateTime.UtcNow - Expires;

            log.DebugFormat("token is valid, age is {0} (>0 == expired)", tokenAge);

            SubjectGuid = new Guid(Convert.FromBase64String(authToken.sub));
            Data = authToken.data;
        }

        /// <summary>
        /// The internal values within a token, modeled after a JSON Web Token
        /// </summary>
        private class TokenContents
        {
            /// <summary>
            /// The subject, for us: the user guid, base64 encoded
            /// </summary>
            public string sub { get; set; }

            /// <summary>
            /// Expiration (seconds since unix epoch)
            /// </summary>
            public long exp { get; set; }

            /// <summary>
            /// Issued At (seconds since unix epoch)
            /// </summary>
            public long iat { get; set; }

            /// <summary>
            /// custom data values, may be null
            /// </summary>
            public Dictionary<string, object> data { get; set; }
        }
    }
}