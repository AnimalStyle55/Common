using LD.Common.Logging.NLog.Renderers;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.LD.Common.Logging.NLog.Renderers
{
    [TestFixture]
    internal class FilteredMessageRendererTest
    {
        private class Renderer : FilteredMessageRenderer
        {
            public string RunIt(LogEventInfo e)
            {
                StringBuilder sb = new StringBuilder();
                base.Append(sb, e);
                return sb.ToString();
            }
        }

        [Test]
        public void Test_NoFilter()
        {
            LogEventInfo lei = LogEventInfo.Create(LogLevel.Debug, "logger",
                "{ \"borrower\": \"buddy\", \"phone\": \"123-435-6678\" }");

            Assert.AreEqual("{ \"borrower\": \"buddy\", \"phone\": \"123-435-6678\" }", new Renderer().RunIt(lei));
        }

        [Test]
        public void Test_HasSsn()
        {
            LogEventInfo lei = LogEventInfo.Create(LogLevel.Debug, "logger",
                "{ \"borrower\": \"buddy\", \"ssn\": \"123-45-6678\" }");

            Assert.AreEqual("{ \"borrower\": \"buddy\", \"ssn\": \"_REDACTED_\" }", new Renderer().RunIt(lei));
        }

        [Test]
        public void Test_HasPassword()
        {
            LogEventInfo lei = LogEventInfo.Create(LogLevel.Debug, "logger",
                "{ \"borrower\": \"buddy\", \"password\": \"abc123\" }");

            Assert.AreEqual("{ \"borrower\": \"buddy\", \"password\": \"_REDACTED_\" }", new Renderer().RunIt(lei));
        }

        [Test]
        public void Test_HasPrefixedPassword()
        {
            LogEventInfo lei = LogEventInfo.Create(LogLevel.Debug, "logger",
                "{ \"borrower\": \"buddy\", \"newPassword\": \"abc123\" }");

            Assert.AreEqual("{ \"borrower\": \"buddy\", \"newPassword\": \"_REDACTED_\" }", new Renderer().RunIt(lei));
        }

        [Test]
        public void Test_HasSsnCase()
        {
            LogEventInfo lei = LogEventInfo.Create(LogLevel.Debug, "logger",
                "{ \"borrower\": \"buddy\", \"Ssn\":\"123456789\" }");

            Assert.AreEqual("{ \"borrower\": \"buddy\", \"Ssn\":\"_REDACTED_\" }", new Renderer().RunIt(lei));
        }

        [Test]
        public void Test_BigString()
        {
            string bigStr = "123" + string.Join("", Enumerable.Repeat('A', 100000));

            LogEventInfo lei = LogEventInfo.Create(LogLevel.Debug, "logger", bigStr);

            Assert.AreEqual(bigStr.Substring(0, 25000) + "<truncated>", new Renderer().RunIt(lei));
        }
    }
}