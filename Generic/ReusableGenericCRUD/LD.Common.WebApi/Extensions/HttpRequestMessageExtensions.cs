using Common.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Common.WebApi.Extensions
{
    /// <summary>
    /// Extensions for HttpRequestMessage
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        private static readonly ILog log = CommonLogManager.GetLogger();

        private const string HttpContext = "MS_HttpContext";

        private const string RemoteEndpointMessage =
            "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

        private const string OwinContext = "MS_OwinContext";

        /// <summary>
        /// Get the Client IP Address (best effort) from a request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            // Web-hosting. Needs reference to System.Web.dll
            if (request.Properties.ContainsKey(HttpContext))
            {
                var ctx = request.Properties[HttpContext] as HttpContextWrapper;
                if (ctx != null)
                {
                    try
                    {
                        // look for forwarded header which is set by load balancers when proxying a request
                        var forwarded = ctx.Request.ServerVariables.Get("HTTP_X_FORWARDED_FOR");
                        if (!string.IsNullOrWhiteSpace(forwarded))
                        {
                            log.TraceFormat("IP: got forwarded header: value {0}", forwarded);
                            var ip = forwarded.Split(',').FirstOrDefault()?.Trim();

                            if (!string.IsNullOrEmpty(ip))
                            {
                                log.TraceFormat("IP: returning forwarded ip: value {0}", ip);
                                return ip;
                            }
                        }

                        log.TraceFormat("IP: returning non forwarded ip: value {0}", ctx.Request.UserHostAddress);
                        return ctx.Request.UserHostAddress;
                    }
                    catch (Exception e)
                    {
                        log.Error("failed", e);
                        return "unknown";
                    }
                }
            }

            // Self-hosting. Needs reference to System.ServiceModel.dll.
            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }

            // Self-hosting using Owin. Needs reference to Microsoft.Owin.dll.
            if (request.Properties.ContainsKey(OwinContext))
            {
                dynamic owinContext = request.Properties[OwinContext];
                if (owinContext != null)
                {
                    return owinContext.Request.RemoteIpAddress;
                }
            }

            return "unknown";
        }
    }
}