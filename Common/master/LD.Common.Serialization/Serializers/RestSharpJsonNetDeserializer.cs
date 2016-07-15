using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD.Common.Serialization.Serializers
{
    /// <summary>
    /// A Deserializer that allows RestSharp to deserialize json responses using Json.Net
    ///
    /// Usage:
    ///
    ///     var restClient = new RestClient();
    ///     restClient.AddHandler(ContentTypes.ApplicationJson, new RestSharpJsonNetDeserializer());
    ///
    /// </summary>
    public class RestSharpJsonNetDeserializer : RestSharp.Deserializers.IDeserializer
    {
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
        public RestSharpJsonNetDeserializer(Action<JsonSerializerSettings> jsonSettings = null)
        {
            JsonSettings = jsonSettings;
        }

        /// <summary>
        /// Deserialize a rest response into an object via Json.net
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public T Deserialize<T>(IRestResponse response)
        {
            return JsonUtil.DeserializeObject<T>(response.Content, JsonSettings);
        }
    }
}