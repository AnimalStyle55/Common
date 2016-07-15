using LD.Common.Logging.NLog.Renderers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.LD.Common.Logging.NLog.Renderers
{
    [TestFixture]
    internal class TruncateLeftRendererTest
    {
        private class Child : TruncateLeftRenderer
        {
            public string CallTransform(string text)
            {
                return base.Transform(text);
            }
        }

        [Test]
        public void Test_Longer()
        {
            var tw = new Child();
            tw.Length = 20;

            Assert.AreEqual("abcdefghij!@#$%^&*()", tw.CallTransform("1234567890abcdefghij!@#$%^&*()"));
        }

        [Test]
        public void Test_Exact()
        {
            var tw = new Child();
            tw.Length = 20;

            Assert.AreEqual("abcdefghij!@#$%^&*()", tw.CallTransform("abcdefghij!@#$%^&*()"));
        }

        [Test]
        public void Test_Shorter()
        {
            var tw = new Child();
            tw.Length = 20;

            Assert.AreEqual("   abcdefghij!@#$%^&", tw.CallTransform("abcdefghij!@#$%^&"));
        }
    }
}