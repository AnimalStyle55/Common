using LD.Common.Constants;

namespace LD.Common.Serializion.Serializers
{
    /// <summary>
    /// A Serializer that allows RestSharp to serialize a request body as plain text
    /// Note: Defaults to ContentTypes.TextPlain, change .ContentType to use something else
    ///
    /// Usage:
    ///
    ///    var request = new RestRequest(resource, method);
    ///
    ///    request.JsonSerializer = new RestSharpStringSerializer();
    ///
    /// </summary>
    public class RestSharpStringSerializer : RestSharp.Serializers.ISerializer
    {
        /// <summary>Required by RestSharp</summary>
        public string ContentType { get; set; }
        /// <summary>Required by RestSharp</summary>
        public string DateFormat { get; set; }
        /// <summary>Required by RestSharp</summary>
        public string Namespace { get; set; }
        /// <summary>Required by RestSharp</summary>
        public string RootElement { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RestSharpStringSerializer()
        {
            ContentType = ContentTypes.TextPlain;
        }

        /// <summary>
        /// Serialize a string to a string (no translation)
        /// </summary>
        public string Serialize(object obj)
        {
            return (obj as string) ?? string.Empty;
        }
    }
}