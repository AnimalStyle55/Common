using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LD.Common.Serialization
{
    /// <summary>
    /// Utility methods for serialization and deserialization using .NET Xml Serializer
    /// </summary>
    public class XmlUtil
    {
        private static readonly XmlWriterSettings _settings = new XmlWriterSettings();

        /// <summary>
        /// Configures the supplied serializer settings
        /// </summary>
        static XmlUtil()
        {
            ApplyDefaultSettings(_settings);
        }

        /// <summary>
        /// Apply our default settings to the settings object
        /// </summary>
        /// <param name="s"></param>
        public static void ApplyDefaultSettings(XmlWriterSettings s)
        {
            s.Encoding = new UTF8Encoding(false);
            s.Indent = true;
        }

        /// <summary>
        /// Serializes an object to xml string
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="omitNamespaces">does not include standard namespaces in the generated xml</param>
        /// <returns>xml string</returns>
        public static string Serialize(object obj, bool omitNamespaces = false)
        {
            using (var stream = new MemoryStream())
            {
                Serialize(obj.GetType(), obj, stream, omitNamespaces);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// Serializes an object to a stream
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        /// <param name="omitNamespaces">does not include standard namespaces in the generated xml</param>
        public static void Serialize(Type type, object obj, Stream stream, bool omitNamespaces = false)
        {
            var serializer = new XmlSerializer(type);

            using (var writer = XmlWriter.Create(stream, _settings))
            {
                if (omitNamespaces)
                {
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);
                    serializer.Serialize(writer, obj, namespaces);
                }
                else
                {
                    serializer.Serialize(writer, obj);
                }
            }
        }

        /// <summary>
        /// Deserializes xml string to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns>object of type T</returns>
        public static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new StringReader(xml))
            {
                return (T)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Deserializes contents of an xml file to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns>object of type T</returns>
        public static T DeserializeFromFile<T>(string fileName)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new StreamReader(fileName))
            {
                return (T)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Deserializes contents of an xml stream to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns>object of type T</returns>
        public static T DeserializeFromStream<T>(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
        }

        /// <summary>
        /// Validate an xml file against a set of schemas
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="schemaPaths"></param>
        /// <returns>a list of errors, will be empty if successful</returns>
        public static List<string> Validate(string xml, params string[] schemaPaths)
        {
            var errors = new List<string>();

            Action<object, ValidationEventArgs> handler = (sender, args) =>
             {
                 errors.Add(string.Format("{0}: {1}", args.Severity, args.Message));
             };

            var xsc = new XmlSchemaSet();
            foreach (string schemaPath in schemaPaths)
                xsc.Add(null, schemaPath);

            var settings = new XmlReaderSettings()
            {
                ValidationType = ValidationType.Schema
            };

            settings.Schemas.Add(xsc);
            settings.ValidationEventHandler += new ValidationEventHandler(handler);

            var vr = XmlReader.Create(new StringReader(xml), settings);

            while (vr.Read()) ;

            vr.Close();

            return errors;
        }
    }
}