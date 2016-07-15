using System;
using System.Net;

namespace LD.Common.WebApi.Client
{
    /// <summary>
    /// Extends <see cref="WebClient"/> to allow control over the handling of redirect responses.
    /// 
    /// Example usage:
    ///   
    /// <code>
    ///     using (var client = new LoanDepotWebClient(false))
    ///     {
    ///         var result = client.DownloadString("https://goo.gl/SeILxU");
    /// 
    ///         if(client.StatusCode == HttpStatusCode.MovedPermanently)
    ///             string location = client.Location;  // https://en.wikipedia.org/wiki/URL_shortening
    ///     }
    /// </code>
    ///                                              
    /// </summary>
    public class LoanDepotWebClient : WebClient
    {
        private WebRequest _request;

        /// <summary>
        /// Creates a new isntance of <see cref="LoanDepotWebClient"/> with <see cref="AutoRedirect"/> = true.
        /// </summary>
        public LoanDepotWebClient() : this( true )
        {
        }

        /// <summary>
        /// Creates a new isntance of <see cref="LoanDepotWebClient"/>.
        /// </summary>
        /// <param name="autoRedirect">
        /// Specifies weather or not to follow redirects:
        ///     True - follow redirects (also the default behavior of <see cref="System.Net.WebClient"/>)
        ///     False - do not follow redirects and simply return the response verbatim.
        /// </param>
        public LoanDepotWebClient(bool autoRedirect)
        {
            AutoRedirect = autoRedirect;
        }

        /// <summary>
        /// Get or Set weather or not to follow redirects:
        ///     True - follow redirects (also the default behavior of <see cref="System.Net.WebClient"/>)
        ///     False - do not follow redirects and simply return the response verbatim.
        /// </summary>
        public bool AutoRedirect { get; set; }

        /// <summary>
        /// Service Call Timeout in Milliseconds, defaults to 100 seconds
        /// </summary>
        public int Timeout { get; set; } = (int)TimeSpan.FromSeconds(100).TotalMilliseconds;

        /// <summary>
        /// Gets the value of the Location header from the response.  When the response is a redirect,
        /// this will typically contain the target URL of the redirect.
        /// </summary>
        public string Location => GetHeaderValue("Location");

        /// <summary>
        /// Get the <see cref="HttpStatusCode"/> of the response.  For example, if the response is 302 redirect it
        /// will be <see cref="HttpStatusCode.Found"/>.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get
            {
                var httpWebResponse = GetWebResponse(_request) as HttpWebResponse;

                if (httpWebResponse != null)
                    return httpWebResponse.StatusCode;

                throw new Exception("invalid response type");
            }
        }

        private string GetHeaderValue(string headerName)
        {
            return (GetWebResponse(_request) as HttpWebResponse)?.Headers[headerName];
        }

        /// <summary>
        /// Overriding the GetWebRequest to set the auto-redirect flag
        /// </summary>
        protected override WebRequest GetWebRequest(Uri address)
        {
            _request = base.GetWebRequest(address);
            _request.Timeout = Timeout;

            var request = _request as HttpWebRequest;
            if (request != null)
                request.AllowAutoRedirect = AutoRedirect;

            return _request;
        }
    }
}