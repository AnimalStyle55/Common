using LD.Common.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tests.LD.Common.Serialization
{
    [TestFixture]
    public class XmlUtilUtilTest
    {
        public enum TestEnum
        {
            Value1,
            Value2
        }

        public class BasicObject
        {
            public int x { get; set; }
            public string y { get; set; }
            public decimal z { get; set; }
            public TestEnum v { get; set; }
        }

        [Test]
        public void Test_Deserialize()
        {
            var b = XmlUtil.Deserialize<BasicObject>(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "\r\n<BasicObject xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                "\r\n  <x>55</x>" +
                "\r\n  <y>abc</y>" +
                "\r\n  <z>1.25</z>" +
                "\r\n  <v>Value2</v>" +
                "\r\n</BasicObject>");

            Assert.AreEqual(55, b.x);
            Assert.AreEqual("abc", b.y);
            Assert.AreEqual(1.25m, b.z);
            Assert.AreEqual(TestEnum.Value2, b.v);
        }

        [Test]
        public void Test_DeserializeFromFile()
        {
            var b = XmlUtil.DeserializeFromFile<BasicObject>("data/BasicObject.xml");

            Assert.AreEqual(55, b.x);
            Assert.AreEqual("abc", b.y);
            Assert.AreEqual(1.25m, b.z);
            Assert.AreEqual(TestEnum.Value2, b.v);
        }

        [Test]
        public void Test_DeserializeFromStream()
        {
            var b = XmlUtil.DeserializeFromStream<BasicObject>(new StreamReader("data/BasicObject.xml").BaseStream);

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

            var s = XmlUtil.Serialize(b, true);

            Assert.AreEqual(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "\r\n<BasicObject>" +
                "\r\n  <x>55</x>" +
                "\r\n  <y>abc</y>" +
                "\r\n  <z>1.25</z>" +
                "\r\n  <v>Value2</v>" +
                "\r\n</BasicObject>", s);

            s = XmlUtil.Serialize(b, false);

            // the serializer sometimes puts the namespaces out of order
            string opt1 =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "\r\n<BasicObject xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                "\r\n  <x>55</x>" +
                "\r\n  <y>abc</y>" +
                "\r\n  <z>1.25</z>" +
                "\r\n  <v>Value2</v>" +
                "\r\n</BasicObject>";

            string opt2 =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "\r\n<BasicObject xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                "\r\n  <x>55</x>" +
                "\r\n  <y>abc</y>" +
                "\r\n  <z>1.25</z>" +
                "\r\n  <v>Value2</v>" +
                "\r\n</BasicObject>";

            Assert.IsTrue(opt1 == s || opt2 == s);
        }
    }
}