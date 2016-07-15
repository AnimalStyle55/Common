using LD.Common.WebApi.Headers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests.LD.CLP.Common.Security
{
    [TestFixture]
    internal class AuthorizationHeaderTest
    {
        [Test]
        public void Test_SimpleHeader()
        {
            var dict = new Dictionary<string, string>()
            {
                {"param1", "abc"},
                {"param2", "12345"},
            };
            var h = new AuthorizationHeader("LD-SCHEME", dict);

            string value = h.GetHeaderString();
            if (value != "LD-SCHEME param1=\"abc\", param2=\"12345\"" &&
                value != "LD-SCHEME param2=\"12345\", param1=\"abc\"")
            {
                Assert.Fail("{0} did not contain correct values", value);
            }

            Assert.IsNull(h.TryGetParameter("doesntexist"));
            Assert.AreEqual("abc", h.TryGetParameter("param1"));

            string param = h.GetParameterString();
            if (param != "param1=\"abc\", param2=\"12345\"" &&
                param != "param2=\"12345\", param1=\"abc\"")
            {
                Assert.Fail("{0} did not contain correct param values", param);
            }
        }

        [Test]
        public void Test_NullOrBlankValue()
        {
            var dict = new Dictionary<string, string>()
            {
                {"param1", ""},
                {"param2", null},
            };
            var h = new AuthorizationHeader("LD-SCHEME", dict);

            string value = h.GetHeaderString();
            if (value != "LD-SCHEME param1=\"\", param2=\"\"" &&
                value != "LD-SCHEME param2=\"\", param1=\"\"")
            {
                Assert.Fail("{0} did not contain correct values", value);
            }

            string param = h.GetParameterString();
            if (param != "param1=\"\", param2=\"\"" &&
                param != "param2=\"\", param1=\"\"")
            {
                Assert.Fail("{0} did not contain correct param values", param);
            }
        }

        [Test]
        public void Test_ParseSimpleHeader()
        {
            var h = new AuthorizationHeader("LD-SCHEME param1=\"abc\", param2=\"12345\", param3=noquotes, param4=\"space s\", param5=\"\"");

            Assert.AreEqual("LD-SCHEME", h.Scheme);
            Assert.AreEqual(5, h.Parameters.Count);
            Assert.AreEqual("abc", h.Parameters["param1"]);
            Assert.AreEqual("12345", h.Parameters["param2"]);
            Assert.AreEqual("noquotes", h.Parameters["param3"]);
            Assert.AreEqual("space s", h.Parameters["param4"]);
            Assert.AreEqual("", h.Parameters["param5"]);
        }

        [Test]
        public void Test_ParseSimpleHeader_Split()
        {
            var h = new AuthorizationHeader("LD-SCHEME", "param1=\"abc\", param2=\"12345\", param3=noquotes, param4=\"space s\"");

            Assert.AreEqual("LD-SCHEME", h.Scheme);
            Assert.AreEqual(4, h.Parameters.Count);
            Assert.AreEqual("abc", h.Parameters["param1"]);
            Assert.AreEqual("12345", h.Parameters["param2"]);
            Assert.AreEqual("noquotes", h.Parameters["param3"]);
            Assert.AreEqual("space s", h.Parameters["param4"]);
        }

        [Test]
        public void Test_InvalidScheme_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new AuthorizationHeader(null, new Dictionary<string, string>()));
        }

        [Test]
        public void Test_InvalidScheme_Empty()
        {
            Assert.Throws<ArgumentException>(() => new AuthorizationHeader(string.Empty, new Dictionary<string, string>()));
        }

        [Test]
        public void Test_InvalidScheme_Spaces()
        {
            Assert.Throws<ArgumentException>(() => new AuthorizationHeader("a b", new Dictionary<string, string>()));
        }

        [Test]
        public void Test_InvalidParam_Empty()
        {
            var dict = new Dictionary<string, string>() { { string.Empty, "value" } };
            Assert.Throws<ArgumentException>(() => new AuthorizationHeader("SCHEME", dict));
        }

        [Test]
        public void Test_InvalidParam_Spaces()
        {
            var dict = new Dictionary<string, string>() { { "a b", "value" } };
            Assert.Throws<ArgumentException>(() => new AuthorizationHeader("SCHEME", dict));
        }

        [Test]
        public void Test_Parse_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new AuthorizationHeader(null));
        }

        [Test]
        public void Test_Parse_Empty()
        {
            Assert.Throws<ArgumentException>(() => new AuthorizationHeader(string.Empty));
        }

        [Test]
        public void Test_Parse_BadScheme()
        {
            Assert.Throws<ArgumentException>(() => new AuthorizationHeader("SC HEME p=1"));
        }

        [Test]
        public void Test_Parse_BadParam()
        {
            Assert.Throws<ArgumentException>(() => new AuthorizationHeader("SCHEME p 1=ok, d=what?"));
        }
    }
}