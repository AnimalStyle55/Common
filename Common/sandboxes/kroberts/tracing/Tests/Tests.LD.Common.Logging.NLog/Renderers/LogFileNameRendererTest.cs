using LD.Common.Logging.NLog.Renderers;
using NUnit.Framework;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.LD.Common.Logging.NLog.Renderers
{
    [TestFixture]
    class LogFileNameRendererTest
    {
        [Test]
        public void Test_LogFileNameAll()
        {
            var lf = new LogFileNameRenderer()
            {
                Component = "test-component",
                Environment = "test",
                IncludeMachine = true,
                Type = "error"
            };

            Assert.AreEqual($"loandepot.error.test-component.test.{Environment.MachineName.ToLowerInvariant()}", lf.Render(new LogEventInfo()));
        }

        [Test]
        public void Test_LogFileNameMinimal()
        {
            var lf = new LogFileNameRenderer()
            {
                Component = "test-component",
                IncludeMachine = false
            };

            Assert.AreEqual($"loandepot.test-component", lf.Render(new LogEventInfo()));
        }

        [Test]
        public void Test_LogFileNameMixture()
        {
            var lf = new LogFileNameRenderer()
            {
                Component = "test-component",
                IncludeMachine = false,
                Type = "type"
            };

            Assert.AreEqual($"loandepot.type.test-component", lf.Render(new LogEventInfo()));
        }
    }
}
