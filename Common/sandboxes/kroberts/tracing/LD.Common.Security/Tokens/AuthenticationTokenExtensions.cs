using Common.Logging;
using LD.Common.Extensions;
using LD.Common.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD.Common.Security.Tokens
{
    /// <summary>
    /// Extensions for AuthenticationTokens
    /// </summary>
    public static class AuthenticationTokenExtensions
    {
        private static readonly ILog log = LoanDepotLogManager.GetLogger();

        private static readonly JsonSerializer _serializer;

        static AuthenticationTokenExtensions()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            JsonUtil.ApplyDefaultSettings(settings);
            _serializer = JsonSerializer.Create(settings);
        }

        /// <summary>
        /// Get a field from the Data Dictionary
        /// </summary>
        /// <typeparam name="T">type that you want</typeparam>
        /// <param name="token">the token</param>
        /// <param name="keyName">key name in the dictionary</param>
        /// <param name="ifNotFound">(optional) value if key not found</param>
        /// <returns></returns>
        public static T GetDataField<T>(this AuthenticationToken token, string keyName, T ifNotFound = default(T))
        {
            T output;
            if (!token.TryGetDataField(keyName, out output))
                return ifNotFound;

            return output;
        }

        /// <summary>
        /// Try to get a field from the Data Dictionary
        /// </summary>
        /// <typeparam name="T">type that you want</typeparam>
        /// <param name="token">the token</param>
        /// <param name="keyName">key name in the dictionary</param>
        /// <param name="value">(out) if found, dictionary value is converted and written</param>
        /// <returns>true if found and converted successfully, false otherwise</returns>
        public static bool TryGetDataField<T>(this AuthenticationToken token, string keyName, out T value)
        {
            // going to treat a null value equivalent to a key not found
            object o;
            if (!token.Data.TryGetValue(keyName, out o) || o == null)
            {
                value = default(T);
                return false;
            }

            try
            {
                if (o is T)
                {
                    // direct access to token object after construction, generally no conversion required
                    value = (T)o;
                }
                else if (o is string && typeof(T).IsEnum)
                {
                    value = (T)Enum.Parse(typeof(T), o as string, true);
                }
                else if (o is IConvertible)
                {
                    // after decryption, types are whatever json.net decides
                    // if convertable (scalar types), just do conversion
                    value = (T)Convert.ChangeType(o, typeof(T));
                }
                else if (o is JObject)
                {
                    // after decryption, objects will be JObject, and need converting to object
                    value = ((JObject)o).ToObject<T>(_serializer);
                }
                else if (o is JArray)
                {
                    // after decryption, lists will be JArray, and need converting to object
                    value = ((JArray)o).ToObject<T>(_serializer);
                }
                else
                {
                    throw new InvalidCastException("object is not T, not IConvertible, and not JObject");
                }
            }
            catch (Exception e)
            {
                log.Debug($"failed converting object in token: cannot convert {o.GetType().Name} to {typeof(T).Name}: {e.GetType().Name}: {e.Message}");
                value = default(T);
                return false;
            }

            return true;
        }

    }
}
