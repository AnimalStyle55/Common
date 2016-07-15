using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LD.Common.Extensions.Configuration
{
    /// <summary>
    /// Extensions for strings specifically for loading from app.config/web.config files
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Converts a string to the value of its corresponding value from <see cref="ConfigurationManager.AppSettings" />
        /// </summary>
        /// <param name="setting">The name of the setting to retrieve</param>
        /// <param name="defaultValue">default if key does not exist</param>
        /// <returns>The value of the setting as a string</returns>
        public static string GetAppSetting(this string setting, string defaultValue = null)
        {
            return ConfigurationManager.AppSettings[setting] ?? defaultValue;
        }

        /// <summary>
        ///     Converts a string to the value of its corresponding value from <see cref="ConfigurationManager.AppSettings" />
        /// </summary>
        /// <param name="setting">The name of the setting to retrieve</param>
        /// <param name="defaultValue">default if key does not exist</param>
        /// <typeparam name="T">The type to convert the value of the setting to</typeparam>
        /// <returns>The value of the setting converted to <typeparamref name="T" /></returns>
        public static T GetAppSetting<T>(this string setting, T defaultValue = default(T))
        {
            var value = GetAppSetting(setting, null);

            if (value == null)
                return defaultValue;

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}