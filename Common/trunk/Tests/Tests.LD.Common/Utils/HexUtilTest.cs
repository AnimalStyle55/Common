using LD.Common.Utils;
using NUnit.Framework;
using System;

namespace Tests.LD.Common.Utils
{
    [TestFixture]
    public class HexUtilTest
    {
        [Test]
        public void Test_ToHex()
        {
            Assert.AreEqual("12ab1c4f", HexUtil.ToHex(new byte[] { 0x12, 0xab, 0x1c, 0x4f }));

            Assert.AreEqual("12ab1c", HexUtil.ToHex(new byte[] { 0x12, 0xab, 0x1c }));
        }

        [Test]
        public void Test_ToHex_ZeroBytes()
        {
            Assert.AreEqual("", HexUtil.ToHex(new byte[0]));
        }

        [Test]
        public void Test_ToHex_Null()
        {
            Assert.Throws<ArgumentNullException>(() => HexUtil.ToHex(null));
        }

        [Test]
        public void Test_HexToBytes()
        {
            CollectionAssert.AreEqual(new byte[] { 0x12, 0xab, 0x1c, 0x4f }, HexUtil.ToBytes("12ab1c4f"));
            CollectionAssert.AreEqual(new byte[] { 0x12, 0xab, 0x1c, 0x4f }, HexUtil.ToBytes("12AB1C4f"));

            CollectionAssert.AreEqual(new byte[] { 0x12, 0xab, 0x1c }, HexUtil.ToBytes("12AB1C"));
        }

        [Test]
        public void Test_HexToBytes_OddLength()
        {
            Assert.Throws<ArgumentException>(() => HexUtil.ToBytes("12abc"));
        }

        [Test]
        public void Test_HexToBytes_BadChar()
        {
            Assert.Throws<IndexOutOfRangeException>(() => HexUtil.ToBytes("12abcP"));
        }

        [Test]
        public void Test_HexToBytes_Null()
        {
            Assert.Throws<ArgumentNullException>(() => HexUtil.ToBytes(null));
        }
    }
}