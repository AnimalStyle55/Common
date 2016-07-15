using LD.Common.Serialization;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.LD.Common.Serialization
{
    [TestFixture]
    internal class JsonUtilUtilTest
    {
        private enum TestEnum
        {
            Value1,
            Value2
        }

        private class BasicObject
        {
            public int x { get; set; }
            public string y { get; set; }
            public decimal z { get; set; }
            public TestEnum v { get; set; }
        }

        private class CamelCaseObject
        {
            public int IntField { get; set; }
            public string StringField { get; set; }
            public decimal DecimalField { get; set; }
            public DateTime? DateField { get; set; }
        }

        [Test]
        public void Test_Deserialize()
        {
            var b = JsonUtil.DeserializeObject<BasicObject>("{\"x\": 55, \"y\": \"abc\", \"z\": 1.25, \"v\": \"Value2\"}");

            Assert.AreEqual(55, b.x);
            Assert.AreEqual("abc", b.y);
            Assert.AreEqual(1.25m, b.z);
            Assert.AreEqual(TestEnum.Value2, b.v);
        }

        [Test]
        public void Test_Serialize()
        {
            var b = new BasicObject()
            {
                x = 55,
                y = "abc",
                z = 1.25m,
                v = TestEnum.Value2
            };

            var s = JsonUtil.SerializeObject(b);

            Assert.AreEqual("{\"x\":55,\"y\":\"abc\",\"z\":1.25,\"v\":\"Value2\"}", s);
        }

        [Test]
        public void Test_Customize()
        {
            var b = JsonUtil.DeserializeObject<CamelCaseObject>("{\"intField\": 55, \"stringField\": \"abc\", \"decimalField\": 1.25, \"dateField\": \"10-04-2014\"}",
                (settings) => settings.DateFormatString = "MM-DD-YYYY");

            Assert.AreEqual(55, b.IntField);
            Assert.AreEqual("abc", b.StringField);
            Assert.AreEqual(1.25m, b.DecimalField);
            Assert.AreEqual(new DateTime(2014, 10, 4), b.DateField);

            b = new CamelCaseObject()
            {
                IntField = 55,
                StringField = "abc",
                DecimalField = 1.25m,
                DateField = null
            };

            var s = JsonUtil.SerializeObject(b);
            Assert.AreEqual("{\"IntField\":55,\"StringField\":\"abc\",\"DecimalField\":1.25}", s);

            s = JsonUtil.SerializeObject(b, (settings) => settings.ContractResolver = new CamelCasePropertyNamesContractResolver());
            Assert.AreEqual("{\"intField\":55,\"stringField\":\"abc\",\"decimalField\":1.25}", s);
        }
    }
}