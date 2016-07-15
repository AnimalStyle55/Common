using LD.Common.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.LD.Common.Utils
{
    [TestFixture]
    internal class SensitiveDataUtilTest
    {
        [Test]
        public void Test_MaskAllJsonValues()
        {
            string s = "{ \"borrower\": \"buddy\", \"cosigner\": \"buddy's buddy\", \"primary_phone\": \"123-435-6678\", \"work_phone\": \"1234356678\" }";

            Assert.AreEqual("{ \"borrower\": \"<REDACTED>\", \"cosigner\": \"<REDACTED>\", \"primary_phone\": \"<REDACTED>\", \"work_phone\": \"<REDACTED>\" }", SensitiveDataUtil.MaskJsonValues(s));
        }

        [Test]
        public void Test_MaskJsonValuesWithKeyFormat()
        {
            string s = "{ \"borrower\": \"buddy\", \"cosigner\": \"buddy's buddy\", \"primary_phone\": \"123-435-6678\", \"work_phone\": \"1234356678\" }";

            Assert.AreEqual("{ \"borrower\": \"buddy\", \"cosigner\": \"buddy's buddy\", \"primary_phone\": \"<REDACTED>\", \"work_phone\": \"1234356678\" }", SensitiveDataUtil.MaskJsonValues(s, "primary_phone"));
            Assert.AreEqual("{ \"borrower\": \"<REDACTED>\", \"cosigner\": \"buddy's buddy\", \"primary_phone\": \"123-435-6678\", \"work_phone\": \"1234356678\" }", SensitiveDataUtil.MaskJsonValues(s, "borrower"));
            Assert.AreEqual("{ \"borrower\": \"<REDACTED>\", \"cosigner\": \"<REDACTED>\", \"primary_phone\": \"123-435-6678\", \"work_phone\": \"1234356678\" }", SensitiveDataUtil.MaskJsonValues(s, "[a-zA-Z]*er"));
        }

        [Test]
        public void Test_MaskJsonValuesWithValueFormat()
        {
            string s = "{ \"borrower\": \"buddy\", \"phone\": \"123-435-6678\", \"phone\": \"1234356678\" }";

            Assert.AreEqual("{ \"borrower\": \"buddy\", \"phone\": \"<REDACTED>\", \"phone\": \"1234356678\" }", SensitiveDataUtil.MaskJsonValues(s, "phone", "\\d{3}[\\-]\\d{3}[\\-]\\d{4}"));
        }

        [Test]
        public void Test_MaskAllXmlValues()
        {
            string s =
                "<info>" +
                "   <borrower>buddy</borrower>" +
                "   <cosigner>buddy's buddy</cosigner>" +
                "   <primary_phone>123-435-6678</primary_phone>" +
                "   <work_phone>1234356678</work_phone>" +
                "</info>";

            Assert.AreEqual(
                "<info>" +
                "   <borrower><REDACTED></borrower>" +
                "   <cosigner><REDACTED></cosigner>" +
                "   <primary_phone><REDACTED></primary_phone>" +
                "   <work_phone><REDACTED></work_phone>" +
                "</info>", SensitiveDataUtil.MaskXmlValues(s));
        }

        [Test]
        public void Test_MaskXmlValuesWithKeyFormat()
        {
            string s =
                "<info>" +
                "   <borrower>buddy</borrower>" +
                "   <cosigner>buddy's buddy</cosigner>" +
                "   <primary_phone>123-435-6678</primary_phone>" +
                "   <work_phone>1234356678</work_phone>" +
                "</info>";

            Assert.AreEqual(
                "<info>" +
                "   <borrower>buddy</borrower>" +
                "   <cosigner>buddy's buddy</cosigner>" +
                "   <primary_phone><REDACTED></primary_phone>" +
                "   <work_phone>1234356678</work_phone>" +
                "</info>", SensitiveDataUtil.MaskXmlValues(s, "primary_phone"));
            Assert.AreEqual(
                "<info>" +
                "   <borrower><REDACTED></borrower>" +
                "   <cosigner>buddy's buddy</cosigner>" +
                "   <primary_phone>123-435-6678</primary_phone>" +
                "   <work_phone>1234356678</work_phone>" +
                "</info>", SensitiveDataUtil.MaskXmlValues(s, "borrower"));
            Assert.AreEqual(
                "<info>" +
                "   <borrower><REDACTED></borrower>" +
                "   <cosigner><REDACTED></cosigner>" +
                "   <primary_phone>123-435-6678</primary_phone>" +
                "   <work_phone>1234356678</work_phone>" +
                "</info>", SensitiveDataUtil.MaskXmlValues(s, "[a-zA-Z]*er"));
        }

        [Test]
        public void Test_MaskXmlValuesWithValueFormat()
        {
            string s =
                "<info>" +
                "   <borrower>buddy</borrower>" +
                "   <phone>123-435-6678</phone>" +
                "   <phone>1234356678</phone>" +
                "</info>";

            Assert.AreEqual(
                "<info>" +
                "   <borrower>buddy</borrower>" +
                "   <phone><REDACTED></phone>" +
                "   <phone>1234356678</phone>" +
                "</info>", SensitiveDataUtil.MaskXmlValues(s, "phone", "\\d{3}[\\-]\\d{3}[\\-]\\d{4}"));
        }
    }
}