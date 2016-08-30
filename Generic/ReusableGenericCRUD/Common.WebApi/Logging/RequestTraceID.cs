using Common.Utils;
using System;
using System.Security.Cryptography;
using System.Threading;

namespace Common.WebApi.Logging
{
    /// <summary>
    /// Generates and stores a unique trace id for a request
    /// To be used to trace requests from top level services all the way down to lower level services
    /// </summary>
    public class RequestTraceID
    {
        /// <summary>
        /// Shared instance for all requests on a machine
        /// </summary>
        public static RequestTraceID Instance { get; } = new RequestTraceID();

        /// <summary>
        /// Header name for sending the trace id
        /// </summary>
        internal const string RequestTraceIDHeader = "LD-ReqTraceID";

        private readonly AsyncLocal<string> _traceId = new AsyncLocal<string>();
        private readonly Random _random;

        internal RequestTraceID()
        {
            // use machine generated entropy to generate a random seed
            // probably paranoid, but this prevents machines booted at the same time from producing same string of random service ids

            byte[] seedBytes = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(seedBytes);
            int seed = BitConverter.ToInt32(seedBytes, 0);
            _random = new Random(seed);
        }

        /// <summary>
        /// Create a new request trace id
        /// </summary>
        /// <returns></returns>
        public string Create()
        {
            byte[] buffer = new byte[6];
            _random.NextBytes(buffer);
            Set(HexUtil.ToHex(buffer));

            return _traceId.Value;
        }

        /// <summary>
        /// Get the current request id
        /// </summary>
        /// <returns></returns>
        public string Get()
        {
            return _traceId.Value;
        }

        /// <summary>
        /// Set the current request trace id
        /// </summary>
        /// <param name="id">new id</param>
        public void Set(string id)
        {
            _traceId.Value = id;
        }

        /*
        // https://en.wikipedia.org/wiki/Universally_unique_identifier#Random_UUID_probability_of_duplicates
         def p(n, x):
             return 1 - math.exp(-1 * n*n / (2 * math.pow(2, x)))

         (25 servers * 25 req/s * 1 minute) = 37500 ids
         p(37500, 48) ~ 2.5 / 1 million
         */
    }
}
