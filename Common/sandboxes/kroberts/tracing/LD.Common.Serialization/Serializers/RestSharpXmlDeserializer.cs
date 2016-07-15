using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD.Common.Serialization.Serializers
{
    /// <summary>
    /// A Deserializer that allows RestSharp to deserialize Xml responses using .NET XmlSerializer
    ///
    /// Usage:
    ///
    ///     var restClient = new RestClient();
    ///     restClient.AddHandler(ContentTypes.TextXml, new RestSharpXmlDeserializer());
    ///
    /// </summary>
    public class RestSharpXmlDeserializer : RestSharp.Deserializers.IDeserializer
    {
        /// <summary>Required by RestSharp</summary>
        public string DateFormat { get; set; }
        /// <summary>Required by RestSharp</summary>
        public string Namespace { get; set; }
        /// <summary>Required by RestSharp</summary>
        public string RootElement { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RestSharpXmlDeserializer()
        {
        }

        /// <summary>
        /// Deserialize a rest response to an object via XmlSerializer
        /// </summary>
        public T Deserialize<T>(IRestResponse response)
        {
            return XmlUtil.Deserialize<T>(response.Content);
        }
    }
}