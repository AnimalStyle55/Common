using Common.Logging;
using Common.Constants;
using Common.Utils;
using Common.WebApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Common.WebApi.Logging
{
    /// <summary>
    /// Handler that runs at start of request flow to log the incoming request and outgoing response
    /// </summary>
    public class LoggingMessageHandler : DelegatingHandler
    {
        private static readonly ILog log = CommonLogManager.GetLogger();

        // start the request ids off at a random int32, so it doesn't reset to 0 on app reset
        private static long _requestId = new Random().Next();

        /// <summary>
        /// Set the context ID (logged as CTX-ABCDE)
        /// You only need to call this directly if spawning a thread outside the request flow
        /// For example, if using HostingEnvironment.QueueBackgroundWorkItem() or Task.Run()
        /// </summary>
        public static void SetContextId()
        {
            // do a thread safe increment on the request id and use that for async context
            // only the bottom 20 bits are used, so it will reset every ~1M requests
            CommonLogManager.SetContextId(Interlocked.Increment(ref _requestId));
        }

        private readonly string[] _loggablePaths;

        /// <summary>
        /// Default constructor, will only log requests to /api/
        /// </summary>
        public LoggingMessageHandler()
        {
            _loggablePaths = new string[] { "/api/" };
        }

        /// <summary>
        /// Constructor with variable loggable paths
        /// </summary>
        /// <param name="loggablePaths">array of custom loggable paths.  Will only log if uri starts with one of these</param>
        public LoggingMessageHandler(params string[] loggablePaths)
        {
            _loggablePaths = (string[])loggablePaths.Clone();
        }

        /// <summary>
        /// Override SendAsync
        /// </summary>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            SetContextId();

            if (!isLoggable(request.RequestUri.AbsolutePath))
                return await base.SendAsync(request, cancellationToken);

            string requestContent = null;
            bool isFormPost = isForm(request);

            if (!isFormPost && request.Content != null)
                requestContent = await request.Content.ReadAsStringAsync();

            HttpResponseMessage response = null;
            string responseContent = null;
            string contentType = null;
            bool shouldLog = false;

            SetTraceId(request);

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

        private void SetTraceId(HttpRequestMessage request)
        {
            IEnumerable<string> vals;
            if (!request.Headers.TryGetValues(RequestTraceID.RequestTraceIDHeader, out vals) ||
                !vals.Any() ||
                string.IsNullOrWhiteSpace(vals.FirstOrDefault()))
            {
                var rId = RequestTraceID.Instance.Create();
                log.Debug($"No trace id: creating = {rId}");
            }
            else
            {
                RequestTraceID.Instance.Set(vals.FirstOrDefault());
            }
        }

        private bool isLoggable(string path)
        {
            return _loggablePaths.Any(l => path.StartsWith(l));
        }

        private void LogRequest(HttpRequestMessage r, string requestContent, bool isFormPost)
        {
            var rId = RequestTraceID.Instance.Get();

            if (isFormPost)
                log.Debug($"<== [{rId}] Received form request from {r.GetClientIpAddress()}: Method: {r.Method}, Uri: {r.RequestUri}, Content-Length: {r.Content.Headers.ContentLength}");
            else if (requestContent == null)
                log.Debug($"<== [{rId}] Received empty request from {r.GetClientIpAddress()}: Method: {r.Method}, Uri: {r.RequestUri}, Content-Length: {r.Content.Headers.ContentLength}");
            else
                log.Debug($"<== [{rId}] Received request from {r.GetClientIpAddress()}: Method: {r.Method}, Uri: {r.RequestUri}, Content: {requestContent}");
        }

        private void LogResponse(HttpResponseMessage r, string responseContent, bool shouldLog, string contentType)
        {
            var content = responseContent ?? string.Empty;
            var rId = RequestTraceID.Instance.Get();

            if (shouldLog)
            {
                log.Debug($"==> [{rId}] Sending response: Status Code: {(int)r.StatusCode}/{r.StatusCode}, Type: {contentType}, Content: {content}");
            }
            else
            {
                long contentLength = r.Content?.Headers?.ContentLength ?? 0;

                log.Debug($"==> [{rId}] Sending response: Status Code: {(int)r.StatusCode}/{r.StatusCode}, Type: {contentType}, Content-Length: {contentLength}");
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