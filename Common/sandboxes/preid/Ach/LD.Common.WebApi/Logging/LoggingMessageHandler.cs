using Common.Logging;
using LD.Common.Constants;
using LD.Common.Utils;
using LD.Common.WebApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LD.Common.WebApi.Logging
{
    /// <summary>
    /// Handler that runs at start of request flow to log the incoming request and outgoing response
    /// </summary>
    public class LoggingMessageHandler : DelegatingHandler
    {
        private static readonly ILog log = LoanDepotLogManager.GetLogger();

        // start the request ids off at a random int32, so it doesn't reset to 0 on app reset
        private static long _requestId = new Random().Next();

        /// <summary>
        /// Override SendAsync
        /// </summary>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // do a thread safe increment on the request id and use that for async context
            // only the bottom 20 bits are used, so it will reset every ~1M requests
            LoanDepotLogManager.SetContextId(Interlocked.Increment(ref _requestId));

            string requestContent = null;
            bool isFormPost = isForm(request);

            if (!isFormPost && request.Content != null)
                requestContent = await request.Content.ReadAsStringAsync();

            HttpResponseMessage response = null;
            string responseContent = null;
            string contentType = null;
            bool shouldLog = false;

            LogRequest(request, requestContent, isFormPost);
            using (new TimeUtil.TimeLogger("Processing request", log))
            {
                response = await base.SendAsync(request, cancellationToken);
                contentType = getContentType(response);
                shouldLog = ShouldLogContent(contentType);

                if (shouldLog && response.Content != null)
                    responseContent = await response.Content.ReadAsStringAsync();
            }
            LogResponse(response, responseContent, shouldLog, contentType);

            return response;
        }

        private void LogRequest(HttpRequestMessage request, string requestContent, bool isFormPost)
        {
            if (isFormPost)
                log.DebugFormat("<== Received form request from {0}: Method: {1}, Uri: {2}, Content-Length: {3}",
                    request.GetClientIpAddress(), request.Method, request.RequestUri, request.Content.Headers.ContentLength);
            else if (requestContent == null)
                log.DebugFormat("<== Received empty request from {0}: Method: {1}, Uri: {2}, Content-Length: {3}",
                    request.GetClientIpAddress(), request.Method, request.RequestUri, request.Content.Headers.ContentLength);
            else
                log.DebugFormat("<== Received request from {0}: Method: {1}, Uri: {2}, Content: {3}",
                    request.GetClientIpAddress(), request.Method, request.RequestUri, requestContent);
        }

        private void LogResponse(HttpResponseMessage response, string responseContent, bool shouldLog, string contentType)
        {
            responseContent = responseContent ?? string.Empty;

            if (shouldLog)
            {
                log.DebugFormat("==> Sending response: Status Code: {0}/{1}, Type: {2}, Content: {3}",
                    (int)response.StatusCode, response.StatusCode, contentType, responseContent);
            }
            else
            {
                long? contentLength = response.Content == null ? 0 : response.Content.Headers.ContentLength;

                log.DebugFormat("==> Sending response: Status Code: {0}/{1}, Type: {2}, Content-Length: {3}",
                    (int)response.StatusCode, response.StatusCode, contentType, contentLength);
            }
        }

        private bool isForm(HttpRequestMessage req)
        {
            return req.Content != null && req.Content.Headers != null && req.Content.Headers.ContentType != null &&
                ContentTypes.MultipartFormData == req.Content.Headers.ContentType.MediaType;
        }

        private string getContentType(HttpResponseMessage resp)
        {
            if (resp.Content != null && resp.Content.Headers != null && resp.Content.Headers.ContentType != null)
                return resp.Content.Headers.ContentType.MediaType;

            return null;
        }

        private static List<string> _loggableContentTypes = new List<string>()
        {
            ContentTypes.ApplicationJavascript,
            ContentTypes.ApplicationEcmascript,
            ContentTypes.ApplicationJson
        };

        /// <summary>
        /// Utility function which determines if content is suitable for logging
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static bool ShouldLogContent(string contentType)
        {
            if (contentType == null)
                return false;

            // log any text/ or anything that is xml
            if (contentType.StartsWith("text/") || contentType.Contains("xml"))
                return true;

            // check if content type starts with any of the loggable types
            // starts with is used because some content types come back like "application/json; charset=utf-8"
            foreach (var loggable in _loggableContentTypes)
            {
                if (contentType.StartsWith(loggable))
                    return true;
            }

            return false;
        }
    }
}