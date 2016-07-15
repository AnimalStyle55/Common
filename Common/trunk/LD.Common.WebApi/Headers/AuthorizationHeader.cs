using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LD.Common.WebApi.Headers
{
    /// <summary>
    /// Parses, Constructs, Manages the value of an AuthorizationHeader
    /// </summary>
    public class AuthorizationHeader
    {
        /// <summary>
        /// The name of the authorization scheme
        /// </summary>
        public string Scheme { get; private set; }

        /// <summary>
        /// Name-value pairs for all parameter strings 
        /// </summary>
        public IDictionary<string, string> Parameters { get; private set; }

        /// <summary>
        /// Construct an authorization header from parts
        /// </summary>
        /// <param name="schemeName">the name of the authorization scheme, no spaces</param>
        /// <param name="parameters">a dictionary of string pairs, the keys cannot have spaces, the values can be null</param>
        public AuthorizationHeader(string schemeName, IDictionary<string, string> parameters)
        {
            Condition.Requires(schemeName, "schemeName").IsNotNullOrEmpty().DoesNotContain(' ');
            Condition.Requires(parameters, "parameters").IsNotNull().Evaluate(parameters.Keys.All(k => !string.IsNullOrEmpty(k) && !k.Contains(' ')));

            Scheme = schemeName;
            Parameters = parameters;
        }

        /// <summary>
        /// Parse an authorization header
        /// </summary>
        /// <param name="headerValue">the value of the header (does not include "Authorization: ")</param>
        public AuthorizationHeader(string headerValue)
        {
            Condition.Requires(headerValue, "headerValue").IsNotNullOrEmpty();

            int firstSpace = headerValue.IndexOf(' ');
            Scheme = headerValue.Substring(0, firstSpace);
            Parameters = new Dictionary<string, string>();

            init(headerValue.Substring(firstSpace), Parameters);
        }

        /// <summary>
        /// Parse an authorization header from scheme and parameter strings
        /// </summary>
        /// <param name="scheme">the scheme name</param>
        /// <param name="parametersValue">all the parameters combined</param>
        public AuthorizationHeader(string scheme, string parametersValue)
        {
            Condition.Requires(scheme, "scheme").IsNotNullOrEmpty().Evaluate(!scheme.Contains(' '));
            Condition.Requires(parametersValue, "parametersValue").IsNotNullOrEmpty();

            Scheme = scheme;
            Parameters = new Dictionary<string, string>();

            init(parametersValue, Parameters);
        }

        private static void init(string parametersValue, IDictionary<string, string> parameters)
        {
            string[] parts = parametersValue.Split(',');

            for (int i = 0; i < parts.Length; i++)
            {
                var match = Regex.Match(parts[i], @"^[\s]*([\w]+)=[""]?([^""]*)[""]?[\s]*$");

                if (!match.Success)
                    throw new ArgumentException("invalid param: " + parts[i]);

                parameters[match.Groups[1].Value] = match.Groups[2].Value;
            }
        }

        /// <summary>
        /// Attempt to get a parameter in the header value by name
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns>the value, or null if not present</returns>
        public string TryGetParameter(string paramName)
        {
            string ret;
            if (Parameters.TryGetValue(paramName, out ret))
                return ret;
            return null;
        }

        /// <summary>
        /// Construct the header string for this authorization header
        /// </summary>
        /// <returns></returns>
        public string GetHeaderString()
        {
            // credentials = auth-scheme #auth-param
            // auth-param = token "=" ( token / quoted-string )
            // comma separated

            StringBuilder sb = new StringBuilder();
            sb.Append(Scheme).Append(' ');

            sb.Append(GetParameterString());

            return sb.ToString();
        }

        /// <summary>
        /// Construct the part of the header string containing the parameters (i.e. not the scheme)
        /// </summary>
        /// <returns></returns>
        public string GetParameterString()
        {
            return string.Join(", ", Parameters.Select(kv => string.Format("{0}=\"{1}\"", kv.Key, kv.Value)));
        }
    }
}