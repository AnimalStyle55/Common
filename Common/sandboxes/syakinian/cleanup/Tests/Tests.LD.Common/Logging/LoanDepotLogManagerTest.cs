using Common.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.LD.Common.Logging
{
    [TestFixture]
    public class LoanDepotLogManagerTest
    {
        private static readonly ILog log = LoanDepotLogManager.GetLogger();

        [Test]
        public void TestGetLogger()
        {
            Assert.IsNotNull(log);
        }

        [Test]
        public void TestGetLogger_NonStatic()
        {
            Assert.Throws<ArgumentException>(() => LoanDepotLogManager.GetLogger());
        }
    }
}