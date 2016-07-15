using LD.Common.WebApi.Configuration;
using NUnit.Framework;
using System;

namespace Tests.LD.Common.WebApi
{
    [TestFixture]
    public class CorsUtilTest
    {
        [Test]
        [TestCase("http://foo.bar", "http://foo.bar")]
        [TestCase("http://foo.bar/", "http://foo.bar")] 
        [TestCase("http://foo.bar/,http://foo.bar", "http://foo.bar")]
        [TestCase("http://foo.bar/,http://foo.bar/", "http://foo.bar")]
        [TestCase("http://foo.bar,http://foo.bar/", "http://foo.bar")]
        [TestCase("http://foo.bar , http://foo.bar/", "http://foo.bar")]
        [TestCase("http://foo.bar/ , http://foo.bar ", "http://foo.bar")]
        [TestCase("http://foo.bar/ , http://foo.bar/ ", "http://foo.bar")]
        [TestCase("http://foo.bar/ , http://bar.foo/ ", "http://bar.foo,http://foo.bar")]
        [TestCase("https://foo.bar/ , https://bar.foo/ ", "https://bar.foo,https://foo.bar")]
        [TestCase("https://foo.bar:8443/ , https://bar.foo:8443 ", "https://bar.foo:8443,https://foo.bar:8443")]
        public void Test_ParseOrigins(string input, string output)
        {
            Assert.AreEqual(output, CorsUtil.ParseOrigins(input));
        }
    }
}
