using System;
using NUnit.Framework;
using LD.Common.WebApi.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.LD.Common.WebApi.Logging
{
    [TestFixture]
    public class RequestTraceIDTest
    {
        private async Task<string> getNew()
        {
            await Task.Yield();

            return RequestTraceID.Instance.Create();
        }

        [Test]
        public void Test_Instance()
        {
            var s1 = getNew().Result;
            var s2 = getNew().Result;

            Assert.AreNotEqual(s1, s2);
        }

        [Test]
        public void Test_Lots()
        {
            var set = new HashSet<string>();

            for (int i = 0; i < 50000; i++)
            {
                var s = getNew().Result;
                Assert.IsFalse(set.Contains(s), $"found dupe: i={i}, val={s}");
                set.Add(s);
            }
        }

        [Test]
        public void Test_2Machines()
        {
            var r1 = new RequestTraceID();
            var r2 = new RequestTraceID();

            Assert.AreNotEqual(r1.Create(), r2.Create());
            Assert.AreNotEqual(r1.Get(), r2.Get());

            Assert.AreEqual(r1.Get(), r1.Get());
            Assert.AreEqual(r2.Get(), r2.Get());
        }

        [Test]
        public void Test_Set()
        {
            var r1 = new RequestTraceID();

            r1.Set("abcd1234ab");
            Assert.AreEqual("abcd1234ab", r1.Get());

            r1.Set("12345678EF");
            Assert.AreEqual("12345678EF", r1.Get());
        }
    }
}
