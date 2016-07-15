using LD.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD.Common.Serialization.Serializers
{
    /// <summary>
    /// A Serializer that allows RestSharp to serialize Xml requests using .NET XmlSerializer
    ///
    /// Usage:
    ///
    ///    var request = new RestRequest(resource, method);
    ///
    ///    request.RequestFormat = DataFormat.Xml
    ///    request.JsonSerializer = new RestSharpXmlSerializer();
    ///
    /// </summary>
    public class RestSharpXmlSerializer : RestSharp.Serializers.ISerializer
    {
        private readonly bool _omitNamespaces;

        /// <summary>Required by RestSharp</summary>
        public string ContentType { get; set; }
        /// <summary>Required by RestSharp</summary>
        public string DateFormat { get; set; }
        /// <summary>Required by RestSharp</summary>
        public string Namespace { get; set; }
        /// <summary>Required by RestSharp</summary>
        public string RootElement { get; set; }

        /// <summary>
        /// Constructore
        /// </summary>
        /// <param name="omitNamespaces">true to not output xml namespaces</param>
        public RestSharpXmlSerializer(bool omitNamespaces = false)
        {
            _omitNamespaces = omitNamespaces;
            ContentType = ContentTypes.ApplicationJson;
        }

        /// <summary>
        /// Serialize an object to xml for sending
        /// </summary>
        public string Serialize(object obj)
        {
            return XmlUtil.Serialize(obj, _omitNamespaces);
        }
    }
}