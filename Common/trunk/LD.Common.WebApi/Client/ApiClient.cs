using Common.Logging;
using LD.Common.Constants;
using LD.Common.Serialization;
using LD.Common.Serialization.Serializers;
using LD.Common.WebApi.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LD.Common.WebApi.Client
{
    /// <summary>
    /// Base class for ApiClients using RestSharp
    /// </summary>
    public class ApiClient
    {
        private static readonly ILog log = LoanDepotLogManager.GetLogger();

        /// <summary>
        /// Internal RestSharp client
        /// </summary>
        protected readonly RestClient _restClient;

        /// <summary>
        /// Construct from url and timeout
        /// </summary>
        /// <param name="serverUrl">protocal and host:port only</param>
        /// <param name="timeout">(optional) timeout, if not specified will use restsharp default</param>
        public ApiClient(string serverUrl, TimeSpan? timeout = null)
        {
            _restClient = new RestClient();
            _restClient.AddHandler(ContentTypes.ApplicationJson, new RestSharpJsonNetDeserializer());

            _restClient.BaseUrl = new Uri(serverUrl);
            if (timeout.HasValue)
            {
                _restClient.Timeout = (int)timeout.Value.TotalMilliseconds;
                _restClient.ReadWriteTimeout = (int)timeout.Value.TotalMilliseconds;
            }
        }

        /// <summary>
        /// Construct a JSON request
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected RestRequest MakeRequest(string resource, Method method = Method.GET)
        {
            var request = new RestRequest(resource, method);

            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = new RestSharpJsonNetSerializer();

            return request;
        }

        /// <summary>
        /// Execute a Request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="expectedStatusCodes"></param>
        protected IRestResponse Execute(IRestRequest request, params HttpStatusCode[] expectedStatusCodes)
        {
            // add in request trace header for next service level down
            request.AddHeader(RequestTraceID.RequestTraceIDHeader, RequestTraceID.Instance.Get());

            logRequest(request);

            var response = _restClient.Execute(request);

            logResponse(response);
            checkForErrors(response, expectedStatusCodes);

            return response;
        }

        /// <summary>
        /// Execute a Request with an expected object return type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="expectedStatusCodes"></param>
        /// <returns></returns>
        protected T Execute<T>(IRestRequest request, params HttpStatusCode[] expectedStatusCodes) where T : new()
        {
            return JsonUtil.DeserializeObject<T>(Execute(request, expectedStatusCodes).Content);
        }

        private void logRequest(IRestRequest request)
        {
            var parameters = string.Join(",",
                request.Parameters
                .Where(p => p.Type == ParameterType.UrlSegment || p.Type == ParameterType.QueryString)
                .Select(p => $"{p.Name}={p.Value}"));

            var rId = RequestTraceID.Instance.Get();

            log.Debug($"[{rId}] Server Request: {request.Method} {request.Resource}, {parameters}");
        }

        private void logResponse(IRestResponse response)
        {
            var statusCode = response.StatusCode;

            var rId = RequestTraceID.Instance.Get();

            log.Debug($"[{rId}] Server Response: {(int)statusCode}/{statusCode} {response.ContentType} {response.Content}");
        }

        private void checkForErrors(IRestResponse response, HttpStatusCode[] expectedStatusCodes = null)
        {
            var statusCode = response.StatusCode;

            // Check expected status codes, if specified
            if (expectedStatusCodes != null
                && expectedStatusCodes.Length > 0)
            {
                if (!expectedStatusCodes.Contains(statusCode))
                {
                    throw new RestException(
                        statusCode,
                        tryExtractErrorMessage(response),
                        response.Content);
                }

                return;
            }

            // Check general status codes
            if ((int)statusCode < 200
                || (int)statusCode >= 300)
            {
                throw new RestException(
                    statusCode,
                    tryExtractErrorMessage(response),
                    response.Content);
            }

            // Check error message
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        private string tryExtractErrorMessage(IRestResponse response)
        {
            try
            {
                var error = JsonUtil.DeserializeObject<ErrorResponse>(response.Content);
                return error.Message;
            }
            catch
            {
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    return response.ErrorMessage;
                }
            }

            return null;
        }

        /// <summary>
        /// Response object returned if error detected
        /// </summary>
        public class ErrorResponse
        {
            /// <summary>
            /// response data
            /// </summary>
            public string Message { get; set; }
        }

        /// <summary>
        /// Exception if rest call returned an HTTP error code
        /// </summary>
        public class RestException : Exception
        {
            /// <summary>
            /// HTTP Status Code returned from call
            /// </summary>
            public HttpStatusCode StatusCode { get; set; }

            /// <summary>
            /// Content of the response
            /// </summary>
            public string ResponseContent { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="statusCode"></param>
            /// <param name="message"></param>
            /// <param name="responseContent"></param>
            public RestException(HttpStatusCode statusCode, string message, string responseContent)
                : base(message ?? "Status code = " + statusCode)
            {
                StatusCode = statusCode;
                ResponseContent = responseContent;
            }
        }
    }
}