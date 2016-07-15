using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Net.Http;
using System.Web;
using LD.Common.WebApi.Extensions;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;
using System.Web.Hosting;

namespace Tests.LD.Common.WebApi.Extensions
{
    [TestFixture]
    class HttpRequestMessageExtensionsTest
    {
        private const string HttpContext = "MS_HttpContext";

        private const string RemoteEndpointMessage =
            "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

        private const string OwinContext = "MS_OwinContext";

        [Test]
        public void Test_NoContext()
        {
            var req = new HttpRequestMessage();
            Assert.AreEqual("unknown", req.GetClientIpAddress());
        }

        [Test]
        public void Test_IIS_Hosting()
        {
            var req = new HttpRequestMessage();
            var httpWorker = new MyWorkerRequest("4.5.2.50");
            var httpReq = new HttpRequest("file", "http://server", "query");

            var field = httpReq.GetType()
                               .GetField("_wr", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(httpReq, httpWorker);

            var httpResp = new HttpResponse(new StringWriter());
            req.Properties[HttpContext] = new HttpContextWrapper(new HttpContext(httpReq, httpResp));

            Assert.AreEqual("4.5.2.50", req.GetClientIpAddress());
        }

        class ThrowWorker : MyWorkerRequest
        {
            public ThrowWorker() : base("4.5.2.50") { }

            public override string GetRemoteAddress()
            {
                throw new Exception("!");
            }
        };

        [Test]
        public void Test_IIS_Hosting_Failure()
        {
            var req = new HttpRequestMessage();
            var httpWorker = new ThrowWorker();
            var httpReq = new HttpRequest("file", "http://server", "query");

            var field = httpReq.GetType()
                               .GetField("_wr", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(httpReq, httpWorker);

            var httpResp = new HttpResponse(new StringWriter());
            req.Properties[HttpContext] = new HttpContextWrapper(new HttpContext(httpReq, httpResp));

            Assert.AreEqual("unknown", req.GetClientIpAddress());
        }

        [Test]
        [TestCase("9.8.7.60", "9.8.7.60", "4.5.2.50")]
        [TestCase("9.8.7.60,22.33.22.33", "9.8.7.60", "4.5.2.50")]
        [TestCase(null, "9.8.7.60", "9.8.7.60")]
        [TestCase("", "9.8.7.60", "9.8.7.60")]
        public void Test_IIS_Hosting_Forwarded(string forwardFor, string remoteIP, string remoteAddr)
        {
            var req = new HttpRequestMessage();
            var httpWorker = new MyWorkerRequest(remoteAddr);
            var httpReq = new HttpRequest("file", "http://server", "query");

            var field = httpReq.GetType()
                               .GetField("_wr", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(httpReq, httpWorker);

            httpReq.AddServerVariable("HTTP_X_FORWARDED_FOR", forwardFor);

            var httpResp = new HttpResponse(new StringWriter());
            req.Properties[HttpContext] = new HttpContextWrapper(new HttpContext(httpReq, httpResp));

            Assert.AreEqual(remoteIP, req.GetClientIpAddress());
        }

        [Test]
        public void Test_Self_Hosting()
        {
            var req = new HttpRequestMessage();
            req.Properties[RemoteEndpointMessage] = new SelfOwinData { Address = "4.5.2.50" };
            var httpResp = new HttpResponse(new StringWriter());

            Assert.AreEqual("4.5.2.50", req.GetClientIpAddress());
        }

        [Test]
        public void Test_Self_Hosting_Null()
        {
            var req = new HttpRequestMessage();
            req.Properties[RemoteEndpointMessage] = null;
            var httpResp = new HttpResponse(new StringWriter());

            Assert.AreEqual("unknown", req.GetClientIpAddress());
        }

        [Test]
        public void Test_Owin_Hosting()
        {
            var req = new HttpRequestMessage();
            req.Properties[OwinContext] = new SelfOwinData { Request = new SelfOwinData.RequestData() { RemoteIpAddress = "4.5.2.50" } };
            var httpResp = new HttpResponse(new StringWriter());

            Assert.AreEqual("4.5.2.50", req.GetClientIpAddress());
        }

        [Test]
        public void Test_Owin_Hosting_Null()
        {
            var req = new HttpRequestMessage();
            req.Properties[OwinContext] = null;
            var httpResp = new HttpResponse(new StringWriter());

            Assert.AreEqual("unknown", req.GetClientIpAddress());
        }
    }

    // this must be public for Test_Self_Hosting and Test_Owin_Hosting to work
    public class SelfOwinData
    {
        public string Address { get; set; }
        public class RequestData
        {
            public string RemoteIpAddress { get; set; }
        }
        public RequestData Request { get; set; }
    }

    class MyWorkerRequest : HttpWorkerRequest
    {
        private readonly string _remoteAddr;

        public MyWorkerRequest(string remoteAddr)
        {
            _remoteAddr = remoteAddr;
        }

        public override void EndOfRequest()
        {
        }

        public override void FlushResponse(bool finalFlush)
        {
        }

        public override string GetHttpVerbName()
        {
            return "GET";
        }

        public override string GetHttpVersion()
        {
            return "1.1";
        }

        public override string GetLocalAddress()
        {
            return "127.0.0.1";
        }

        public override int GetLocalPort()
        {
            return 29474;
        }

        public override string GetQueryString()
        {
            return "";
        }

        public override string GetRawUrl()
        {
            return "/";
        }

        public override string GetRemoteAddress()
        {
            return _remoteAddr;
        }

        public override int GetRemotePort()
        {
            return 80;
        }

        public override string GetUriPath()
        {
            return "path";
        }

        public override void SendKnownResponseHeader(int index, string value)
        {
        }

        public override void SendResponseFromFile(IntPtr handle, long offset, long length)
        {
        }

        public override void SendResponseFromFile(string filename, long offset, long length)
        {
        }

        public override void SendResponseFromMemory(byte[] data, int length)
        {
        }

        public override void SendStatus(int statusCode, string statusDescription)
        {
        }

        public override void SendUnknownResponseHeader(string name, string value)
        {
        }
    }

    /// <summary>
    /// Extension methods for the HttpRequest class. (might break if HttpRequest internals are modified)
    /// </summary>
    static class HttpRequestExtensions
    {
        /// <summary>
        /// Adds the name/value pair to the ServerVariables for the HttpRequest.
        /// </summary>
        /// <param name="request">The request to append the variables to.</param>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        public static void AddServerVariable(this HttpRequest request, string name, string value)
        {
            if (request == null) return;

            AddServerVariables(request, new Dictionary<string, string>() { { name, value } });
        }

        /// <summary>
        /// Adds the name/value pairs to the ServerVariables for the HttpRequest.
        /// </summary>
        /// <param name="request">The request to append the variables to.</param>
        /// <param name="collection">The collection of name/value pairs to add.</param>
        public static void AddServerVariables(this HttpRequest request, NameValueCollection collection)
        {
            if (request == null) return;
            if (collection == null) return;

            AddServerVariables(request, collection.AllKeys
                                                  .ToDictionary(k => k, k => collection[k]));
        }

        /// <summary>
        /// Adds the name/value pairs to the ServerVariables for the HttpRequest.
        /// </summary>
        /// <param name="request">The request to append the variables to.</param>
        /// <param name="dictionary">The dictionary containing the pairs to add.</param>
        public static void AddServerVariables(this HttpRequest request, IDictionary<string, string> dictionary)
        {
            if (request == null) return;
            if (dictionary == null) return;

            var field = request.GetType()
                               .GetField("_serverVariables", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                var type = field.FieldType;

                var serverVariables = field.GetValue(request);
                if (serverVariables == null)
                {
                    var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                                                          new[] { typeof(HttpWorkerRequest), typeof(HttpRequest) }, null);
                    serverVariables = constructor.Invoke(new[] { null, request });
                    field.SetValue(request, serverVariables);
                }
                var addStatic = type.GetMethod("AddStatic", BindingFlags.Instance | BindingFlags.NonPublic);

                ((NameValueCollection)serverVariables).MakeWriteable();
                foreach (var item in dictionary)
                {
                    addStatic.Invoke(serverVariables, new[] { item.Key, item.Value });
                }
                ((NameValueCollection)serverVariables).MakeReadOnly();
            }
        }
    }

    /// <summary>
    /// Extension methods for the NameValueCollection class. (might break if NameValueCollection internals are modified)
    /// </summary>
    static class NameValueCollectionExtensions
    {
        /// <summary>
        /// Retreives the IsReadOnly property from the NameValueCollection
        /// </summary>
        /// <param name="collection">The collection to retrieve the propertyInfo from.</param>
        /// <param name="bindingFlags">The optional BindingFlags to use. If not specified defautls to Instance|NonPublic.</param>
        /// <returns>The PropertyInfo for the IsReadOnly property.</returns>
        private static PropertyInfo GetIsReadOnlyProperty(this NameValueCollection collection, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic)
        {
            if (collection == null) return (null);
            return (collection.GetType().GetProperty("IsReadOnly", bindingFlags));
        }

        /// <summary>
        /// Sets the IsReadOnly property to the specified value.
        /// </summary>
        /// <param name="collection">The collection to modify.</param>
        /// <param name="isReadOnly">The value to set.</param>
        private static void SetIsReadOnly(this NameValueCollection collection, bool isReadOnly)
        {
            if (collection == null) return;

            var property = GetIsReadOnlyProperty(collection);
            if (property != null)
            {
                property.SetValue(collection, isReadOnly, null);
            }
        }

        /// <summary>
        /// Makes the specified collection writable via reflection.
        /// </summary>
        /// <param name="collection">The collection to make writable.</param>
        public static void MakeWriteable(this NameValueCollection collection)
        {
            SetIsReadOnly(collection, false);
        }

        /// <summary>
        /// Makes the specified collection readonly via reflection.
        /// </summary>
        /// <param name="collection">The collection to make readonly.</param>
        public static void MakeReadOnly(this NameValueCollection collection)
        {
            SetIsReadOnly(collection, true);
        }
    }
}
