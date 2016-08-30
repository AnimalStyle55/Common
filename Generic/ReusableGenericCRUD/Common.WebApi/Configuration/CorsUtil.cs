using System;
using System.Linq;

namespace Common.WebApi.Configuration
{
    /// <summary>
    /// Utilities for enabling CORS in web api projects
    /// </summary>
    public static class CorsUtil
    {
        /// <summary>
        /// Eliminates trailing slashes from a comma separated list of URLs
        /// </summary>
        /// <remarks>
        /// NOTE: This will distinct and sort as well
        /// </remarks>
        /// <param name="allowedOrigins">A comma separated list of URLs</param>
        /// <returns>a string suitable for passing to EnableCorsAttribute()</returns>
        public static string ParseOrigins(string allowedOrigins)
        {
            return string.Join(",",
                allowedOrigins
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim().TrimEnd('/'))
                    .Distinct()
                    .OrderBy(k => k)
                );
        }
    }
}
