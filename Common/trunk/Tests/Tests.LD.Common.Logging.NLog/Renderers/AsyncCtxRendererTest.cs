using LD.Common.Logging.NLog.Renderers;
using NUnit.Framework;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;

namespace Tests.LD.Common.Logging.NLog.Renderers
{
    [TestFixture]
    class AsyncCtxRendererTest
    {
        [Test]
        public void Test_AsyncContext()
        {
            var lf = new AsyncCtxRenderer();

            LoanDepotLogManager.SetContextId(0x1a3b5);

            Assert.AreEqual("1A3B5", lf.Render(new LogEventInfo()));
        }

        [Test]
        public void Test_AsyncContextOverflow()
        {
            var lf = new AsyncCtxRenderer();

            LoanDepotLogManager.SetContextId(0xabcde21c3d5);

            Assert.AreEqual("1C3D5", lf.Render(new LogEventInfo()));
        }
    }
}
