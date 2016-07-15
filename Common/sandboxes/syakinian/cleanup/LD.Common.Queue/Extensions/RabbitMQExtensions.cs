using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace LD.Common.Queue.Extensions
{
    /// <summary>
    /// Utility methods for interacting with headers of rabbitmq messages
    /// </summary>
    public static class RabbitMQExtensions
    {
        /// <summary>
        /// Set a header in message properties
        /// </summary>
        /// <typeparam name="T">type of the header, recommended string or primitive type</typeparam>
        /// <param name="properties">BasicProperties object from rabbit</param>
        /// <param name="headerName">name of the header, recommended to prefix with "ld-"</param>
        /// <param name="headerValue">value of type T</param>
        public static void SetHeader<T>(this IBasicProperties properties, string headerName, T headerValue)
        {
            if (properties == null)
                return;

            if (properties.Headers == null)
                properties.Headers = new Dictionary<string, object>();

            properties.Headers[headerName] = headerValue;
        }

        /// <summary>
        /// Get header value from message properties
        /// </summary>
        /// <typeparam name="T">type of the header, see SetHeader</typeparam>
        /// <param name="properties">BasicProperties object from rabbit</param>
        /// <param name="headerName">name of the header, recommended to prefix with "ld-"</param>
        /// <returns>header value or default(T)</returns>
        public static T GetHeader<T>(this IBasicProperties properties, string headerName)
        {
            return properties.GetHeader<T>(headerName, default(T));
        }

        /// <summary>
        /// Get header value from message properties
        /// </summary>
        /// <typeparam name="T">type of the header, see SetHeader</typeparam>
        /// <param name="properties">BasicProperties object from rabbit</param>
        /// <param name="headerName">name of the header, recommended to prefix with "ld-"</param>
        /// <param name="defaultVal">(optional) default value if header isn't found</param>
        /// <returns>header value or defaultVal</returns>
        public static T GetHeader<T>(this IBasicProperties properties, string headerName, T defaultVal)
        {
            T value;
            if (!TryGetHeader(properties, headerName, out value))
                return defaultVal;
            return value;
        }

        /// <summary>
        /// Get header value from message properties
        /// </summary>
        /// <typeparam name="T">type of the header, see SetHeader</typeparam>
        /// <param name="properties">BasicProperties object from rabbit</param>
        /// <param name="headerName">name of the header, recommended to prefix with "ld-"</param>
        /// <param name="value">(out) receives the value</param>
        /// <returns>true if header was found, value will be set, false otherwise and value is default(T)</returns>
        public static bool TryGetHeader<T>(this IBasicProperties properties, string headerName, out T value)
        {
            Func<object, T> converter = null;

            // strings get encoded as byte[], other primitives are deserialized directly
            if (typeof(T) == typeof(string))
                converter = o => (T)(object)Encoding.UTF8.GetString((byte[])o);

            return TryGetHeader(properties, headerName, out value, converter);
        }

        /// <summary>
        /// Get header value from message properties
        /// </summary>
        /// <typeparam name="T">type of the header, see SetHeader</typeparam>
        /// <param name="properties">BasicProperties object from rabbit</param>
        /// <param name="headerName">name of the header, recommended to prefix with "ld-"</param>
        /// <param name="value">(out) receives the value</param>
        /// <param name="converter">a function for doing a custom conversion of the header value, null is identity function</param>
        /// <returns>true if header was found, value will be set, false otherwise and value is default(T)</returns>
        public static bool TryGetHeader<T>(this IBasicProperties properties, string headerName, out T value, Func<object, T> converter)
        {
            object o;
            if (properties == null || properties.Headers == null || !properties.Headers.TryGetValue(headerName, out o))
            {
                value = default(T);
                return false;
            }

            value = (converter == null) ? (T)o : converter.Invoke(o);
            return true;
        }
    }
}