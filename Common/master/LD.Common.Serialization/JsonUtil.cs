using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD.Common.Serialization
{
    /// <summary>
    /// Utility methods for serialization and deserialization using Json.Net
    /// </summary>
    public static class JsonUtil
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings();

        /// <summary>
        /// Configures the supplied serializer settings
        /// </summary>
        static JsonUtil()
        {
            ApplyDefaultSettings(_settings);
        }

        /// <summary>
        /// Apply our default settings to the settings object
        /// </summary>
        /// <param name="s"></param>
        public static void ApplyDefaultSettings(JsonSerializerSettings s)
        {
            s.Converters.Add(new StringEnumConverter());
            s.FloatParseHandling = FloatParseHandling.Decimal;
            s.NullValueHandling = NullValueHandling.Ignore;
        }

        /// <summary>
        /// Serializes an object to json string using standard serialization settings
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="modSettings">customize the json settings with this action, null for no customization</param>
        /// <returns>json string</returns>
        public static string SerializeObject(object obj, Action<JsonSerializerSettings> modSettings = null)
        {
            return JsonConvert.SerializeObject(obj, customizeSettings(modSettings));
        }

        /// <summary>
        /// Deserializes json string to object using standard serialization settings
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="modSettings">customize the json settings with this action, null for no customization</param>
        /// <returns>object of type T</returns>
        public static T DeserializeObject<T>(string s, Action<JsonSerializerSettings> modSettings = null)
        {
            return JsonConvert.DeserializeObject<T>(s, customizeSettings(modSettings));
        }

        /// <summary>
        /// Uses json serialize/deserialize to create a deep clone of an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">object to clone</param>
        /// <returns>cloned object of type T</returns>
        public static T DeepClone<T>(T obj)
        {
            return DeserializeObject<T>(SerializeObject(obj));
        }

        private static JsonSerializerSettings customizeSettings(Action<JsonSerializerSettings> modSettings)
        {
            JsonSerializerSettings settings = _settings;
            if (modSettings != null)
            {
                // make a new copy if customization is desired
                settings = new JsonSerializerSettings();
                ApplyDefaultSettings(settings);
                modSettings.Invoke(settings);
            }
            return settings;
        }
    }
}