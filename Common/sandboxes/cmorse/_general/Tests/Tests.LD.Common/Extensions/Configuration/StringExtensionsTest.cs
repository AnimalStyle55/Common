using LD.Common.Extensions.Configuration;
using NUnit.Framework;
using System;

namespace Tests.LD.Common.Extensions.Configuration
{
    [TestFixture]
    public class StringExtensionsTest
    {
        [Test]
        public void Test_GetSetting()
        {
            Assert.AreEqual("TestValue", "String_Test_1".GetAppSetting());
            Assert.AreEqual("", "String_Test_2".GetAppSetting());

            Assert.IsNull("String_Test_NotFound".GetAppSetting());
            Assert.AreEqual("hello", "String_Test_NotFound".GetAppSetting("hello"));
        }

        [Test]
        public void Test_GetSettingType()
        {
            Assert.AreEqual("TestValue", "String_Test_1".GetAppSetting<string>());
            Assert.AreEqual("", "String_Test_2".GetAppSetting<string>());
            Assert.IsNull("String_Test_NotFound".GetAppSetting<string>());

            Assert.AreEqual(50, "Int_Test_1".GetAppSetting<int>());
            Assert.AreEqual(-1078, "Int_Test_2".GetAppSetting<int>());
            Assert.AreEqual(777, "Int_Test_NotFound".GetAppSetting<int>(777));
            Assert.AreEqual(5008278748728L, "Long_Test_1".GetAppSetting<long>());
            Assert.AreEqual(-528635648728L, "Long_Test_2".GetAppSetting<long>());
            Assert.AreEqual(1003004005006, "Long_Test_NotFound".GetAppSetting<long>(1003004005006L));
            Assert.IsTrue("Bool_Test_1".GetAppSetting<bool>());
            Assert.IsFalse("Bool_Test_2".GetAppSetting<bool>());
            Assert.IsTrue("Bool_Test_3".GetAppSetting<bool>());
            Assert.IsFalse("Bool_Test_4".GetAppSetting<bool>());
            Assert.IsFalse("Bool_Test_NotFound".GetAppSetting<bool>());
            Assert.IsTrue("Bool_Test_NotFound".GetAppSetting<bool>(true));
            Assert.AreEqual(1.836, "Double_Test_1".GetAppSetting<double>());
            Assert.AreEqual(-2.9376, "Double_Test_2".GetAppSetting<double>());
            Assert.AreEqual(1.002003, "Double_Test_NotFound".GetAppSetting<double>(1.002003));
        }
    }
}
