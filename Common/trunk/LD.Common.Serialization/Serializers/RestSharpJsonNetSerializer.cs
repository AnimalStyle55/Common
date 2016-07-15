using LD.Common.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD.Common.Serialization.Serializers
{
    /// <summary>
    /// A Serializer that allows RestSharp to serialize json requests using Json.Net
    ///
    /// Usage:
    ///
    ///    var request = new RestRequest(resource, method);
    ///
    ///    request.RequestFormat = DataFormat.Json;
    ///    request.JsonSerializer = new RestSharpJsonNetSerializer();
    ///
    /// </summary>
    public class RestSharpJsonNetSerializer : RestSharp.Serializers.ISerializer
    {
        /// <summary>Required by RestSharp</summary>
        public string ContentType { get; set; }
        /// <summary>Required by RestSharp</summary>
        public string DateFormat { get; set; }
        /// <summary>Required by RestSharp</summary>
        public string Namespace { get; set; }
        /// <summary>Required by RestSharp</summary>
        public string RootElement { get; set; }

        private Action<JsonSerializerSettings> JsonSettings { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RestSharpJsonNetSerializer(Action<JsonSerializerSettings> jsonSettings = null)
        {
            ContentType = ContentTypes.ApplicationJson;
            JsonSettings = jsonSettings;
        }

        /// <summary>
        /// Serialize an object into json format for sending
        /// </summary>
        public string Serialize(object obj)
        {
            return JsonUtil.SerializeObject(obj, JsonSettings);
        }
    }
}