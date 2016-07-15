using LD.Common.TextFormatting;
using NUnit.Framework;
using System;

namespace Tests.ACH
{
    [TestFixture]   
    public class TrimAndPadTests
    {
        [Test]
        public void TrimAndPadLeftTest()
        {
            string a = null;
            Assert.AreEqual(a.TrimAndPadLeft(5), "     ");
            Assert.AreEqual("".TrimAndPadLeft(5), "     ");
            Assert.AreEqual("".TrimAndPadLeft(5, '*'), "*****");
            Assert.AreEqual("a".TrimAndPadLeft(5), "    a");
            Assert.AreEqual("a".TrimAndPadLeft(5, '*'), "****a");
            Assert.AreEqual("abcdef".TrimAndPadLeft(5), "bcdef");
            Assert.AreEqual("123-45-6789".TrimAndPadLeft(4), "6789");
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TrimAndPadLeftExceptionTest()
        {
            "a".TrimAndPadLeft(-1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CheckAndPadLeftOutOfRangeTest()
        {
            100000.CheckAndPadLeft(5);
            Assert.IsTrue(false);  // Guarantees failure unless we get the exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CheckAndPadLeftNegativeTest()
        {
            (-10000).CheckAndPadLeft(6);
            Assert.IsTrue(false); // Guarantees failure unless we get the exception
        }

        [Test]
        public void CheckAndPadLeftTest()
        {
            Assert.AreEqual(10000.CheckAndPadLeft(5), "10000");
            Assert.AreEqual(100000.CheckAndPadLeft(6), "100000");
            Assert.AreEqual(1.CheckAndPadLeft(6), "000001");
            Assert.AreEqual(0.CheckAndPadLeft(10), "0000000000");
            Assert.AreEqual(((int)(150000.00m * 100.00m)).CheckAndPadLeft(12), "000015000000");
        }

        [Test]
        public void TrimAndPadRightTest()
        {
            string a = null;
            Assert.AreEqual(a.TrimAndPadRight(5), "     ");
            Assert.AreEqual("".TrimAndPadRight(5), "     ");
            Assert.AreEqual("".TrimAndPadRight(5, '*'), "*****");
            Assert.AreEqual("a".TrimAndPadRight(5), "a    ");
            Assert.AreEqual("a".TrimAndPadRight(5, '*'), "a****");
            Assert.AreEqual("abcdef".TrimAndPadRight(5), "abcde");
        }
    }
}