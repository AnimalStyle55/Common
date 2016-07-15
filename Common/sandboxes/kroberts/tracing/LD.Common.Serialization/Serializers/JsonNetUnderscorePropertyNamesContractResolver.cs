using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LD.Common.Serialization.Serializers
{
    /// <summary>
    /// Use in Json Deserialization if the json multi-word keys are delimited by underscores
    ///
    /// "api_key" => string ApiKey {get;set;}
    ///
    /// Usage:
    ///
    ///     return JsonUtil.DeserializeObject(
    ///         stringValue,
    ///         (s) =>
    ///         {
    ///             s.ContractResolver = new JsonNetUnderscorePropertyNamesContractResolver();
    ///         });
    ///
    /// </summary>
    public class JsonNetUnderscorePropertyNamesContractResolver : DefaultContractResolver
    {
        private readonly Regex _regex = new Regex("(?<=[^A-Z])(?<letter>[A-Z])");

        /// <summary>
        /// Called by Json.Net to resolve the property name
        /// </summary>
        protected override string ResolvePropertyName(string propertyName)
        {
            return _regex.Replace(propertyName, "_${letter}").ToLowerInvariant();
        }
    }
}