using LD.Common.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.LD.Common.Utils
{
    [TestFixture]
    internal class SensitiveDataExtensionsTest
    {
        [Test]
        public void Test_MaskAllJsonValues()
        {
            string s = "{ \"borrower\": \"buddy\", \"cosigner\": \"buddy's buddy\", \"primary_phone\": \"123-435-6678\", \"work_phone\": \"1234356678\" }";

            Assert.AreEqual("{ \"borrower\": \"_REDACTED_\", \"cosigner\": \"_REDACTED_\", \"primary_phone\": \"_REDACTED_\", \"work_phone\": \"_REDACTED_\" }", s.MaskJsonValues());
        }

        [Test]
        public void Test_MaskJsonValuesWithKeyFormat()
        {
            string s = "{ \"borrower\": \"buddy\", \"cosigner\": \"buddy's buddy\", \"primary_phone\": \"123-435-6678\", \"work_phone\": \"1234356678\" }";

            Assert.AreEqual("{ \"borrower\": \"buddy\", \"cosigner\": \"buddy's buddy\", \"primary_phone\": \"_REDACTED_\", \"work_phone\": \"1234356678\" }", s.MaskJsonValues("primary_phone"));
            Assert.AreEqual("{ \"borrower\": \"_REDACTED_\", \"cosigner\": \"buddy's buddy\", \"primary_phone\": \"123-435-6678\", \"work_phone\": \"1234356678\" }", s.MaskJsonValues("borrower"));
            Assert.AreEqual("{ \"borrower\": \"_REDACTED_\", \"cosigner\": \"_REDACTED_\", \"primary_phone\": \"123-435-6678\", \"work_phone\": \"1234356678\" }", s.MaskJsonValues("[a-zA-Z]*er"));
        }

        [Test]
        public void Test_MaskJsonValuesWithValueFormat()
        {
            string s = "{ \"borrower\": \"buddy\", \"phone\": \"123-435-6678\", \"phone\": \"1234356678\" }";

            Assert.AreEqual("{ \"borrower\": \"buddy\", \"phone\": \"_REDACTED_\", \"phone\": \"1234356678\" }", s.MaskJsonValues("phone", "\\d{3}[\\-]\\d{3}[\\-]\\d{4}"));
        }

        [Test]
        public void Test_MaskAllQueryStringValues()
        {
            string s = "www.loandepot.com?borrower=buddy&cosigner=buddy%27s%20buddy&primary_phone=123-435-6678&work_phone=1234356678";

            Assert.AreEqual("www.loandepot.com?borrower=_REDACTED_&cosigner=_REDACTED_&primary_phone=_REDACTED_&work_phone=_REDACTED_", s.MaskQueryStringValues());
        }

        [Test]
        public void Test_MaskQueryStringValuesWithKeyFormat()
        {
            string s = "www.loandepot.com?borrower=buddy&cosigner=buddy%27s%20buddy&primary_phone=123-435-6678&work_phone=1234356678";

            Assert.AreEqual("www.loandepot.com?borrower=buddy&cosigner=buddy%27s%20buddy&primary_phone=_REDACTED_&work_phone=1234356678", s.MaskQueryStringValues("primary_phone"));
            Assert.AreEqual("www.loandepot.com?borrower=_REDACTED_&cosigner=buddy%27s%20buddy&primary_phone=123-435-6678&work_phone=1234356678", s.MaskQueryStringValues("borrower"));
            Assert.AreEqual("www.loandepot.com?borrower=_REDACTED_&cosigner=_REDACTED_&primary_phone=123-435-6678&work_phone=1234356678", s.MaskQueryStringValues("[a-zA-Z]*er"));
        }

        [Test]
        public void Test_MaskQueryStringValuesWithValueFormat()
        {
            string s = "www.loandepot.com?borrower=buddy&phone=123-435-6678&phone=1234356678";

            Assert.AreEqual("www.loandepot.com?borrower=buddy&phone=_REDACTED_&phone=1234356678", s.MaskQueryStringValues("phone", "\\d{3}[\\-]\\d{3}[\\-]\\d{4}"));
        }

        [Test]
        public void Test_MaskJsonSsn()
        {
            string s = "{ \"borrower\": \"buddy\", \"ssn\": \"123-45-6678\" }";

            Assert.AreEqual("{ \"borrower\": \"buddy\", \"ssn\": \"_REDACTED_\" }", s.MaskSsn());
        }

        [Test]
        public void Test_MaskXmlSsn()
        {
            string s =
                "<info>" +
                "   <ssn>111-22-3344</ssn>" +
                "   <phone>1234356678</phone>" +
                "</info>";

            Assert.AreEqual(
                "<info>" +
                "   <ssn>_REDACTED_</ssn>" +
                "   <phone>1234356678</phone>" +
                "</info>", s.MaskSsn());
        }

        [Test]
        public void Test_MaskQueryStringSsn()
        {
            string s = "www.loandepot.com?borrower=buddy&ssn=123-45-6678";

            Assert.AreEqual("www.loandepot.com?borrower=buddy&ssn=_REDACTED_", s.MaskSsn());
        }

        [Test]
        public void Test_MaskSsnCase()
        {
            string s = "{ \"borrower\": \"buddy\", \"Ssn\":\"123456789\" }";

            Assert.AreEqual("{ \"borrower\": \"buddy\", \"Ssn\":\"_REDACTED_\" }", s.MaskSsn());
        }

        [Test]
        public void Test_MaskJsonPassword()
        {
            string s = "{ \"borrower\": \"buddy\", \"password\": \"abc123\" }";

            Assert.AreEqual("{ \"borrower\": \"buddy\", \"password\": \"_REDACTED_\" }", s.MaskPassword());
        }

        [Test]
        public void Test_MaskXmlPassword()
        {
            string s =
                "<info>" +
                "   <password>LoanDepot123</password>" +
                "   <phone>1234356678</phone>" +
                "</info>";

            Assert.AreEqual(
                "<info>" +
                "   <password>_REDACTED_</password>" +
                "   <phone>1234356678</phone>" +
                "</info>", s.MaskPassword());
        }

        [Test]
        public void Test_MaskQueryStringPassword()
        {
            string s = "www.loandepot.com?borrower=buddy&password=LoanDepot123";

            Assert.AreEqual("www.loandepot.com?borrower=buddy&password=_REDACTED_", s.MaskPassword());
        }

        [Test]
        public void Test_MaskPrefixedPassword()
        {
            string s = "{ \"borrower\": \"buddy\", \"newPassword\": \"abc123\" }";

            Assert.AreEqual("{ \"borrower\": \"buddy\", \"newPassword\": \"_REDACTED_\" }", s.MaskPassword());
        }
    }
}