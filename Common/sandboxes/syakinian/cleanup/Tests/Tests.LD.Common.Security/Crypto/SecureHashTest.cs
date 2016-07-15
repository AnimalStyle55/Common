using LD.Common.Security.Crypto;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Tests.LD.Common.Security.Crypto
{
    [TestFixture]
    public class SecureHashTest
    {
        [Test]
        public void Test_HashPassword()
        {
            SecureHash sd = new SecureHash(0xff, "12345678901234567890123456789012", 1000);

            string password = "Password01";

            string hash = sd.HashPassword(password);
            string hash2 = sd.HashPassword(password);

            Assert.AreNotEqual(hash2, hash);

            Assert.IsTrue(sd.VerifyPasswordHash(password, hash));
            Assert.IsTrue(sd.VerifyPasswordHash(password, hash2));

            string hash3 = sd.HashPassword(password + "2");
            Assert.IsFalse(sd.VerifyPasswordHash(password, hash3));
        }

        [Test]
        public void Test_PredictableHash()
        {
            SecureHash sd = new SecureHash(0xff, "12345678901234567890123456789012", 1000);

            string h1 = sd.PredictableHash("123-45-6789");
            string h2 = sd.PredictableHash("123-45-6789");

            Assert.AreEqual(h1, h2);
        }

        [Test]
        public void Test_HashPassword_InvalidIterations()
        {
            SecureHash sd = new SecureHash(0xff, "12345678901234567890123456789012", 1000);

            string password = "Password01";
            string hash = sd.HashPassword(password);

            var sb = new StringBuilder(hash);
            sb[1] = '2';

            Assert.IsFalse(sd.VerifyPasswordHash(password, sb.ToString()));
        }

        [Test]
        public void Test_HashPassword_InvalidData()
        {
            SecureHash sd = new SecureHash(0xff, "12345678901234567890123456789012", 1000);

            string password = "Password01";

            Assert.IsFalse(sd.VerifyPasswordHash(password, null));
            Assert.IsFalse(sd.VerifyPasswordHash(password, ""));
            Assert.IsFalse(sd.VerifyPasswordHash(password, "01"));
            Assert.IsFalse(sd.VerifyPasswordHash(password, "0102030405"));
            Assert.IsFalse(sd.VerifyPasswordHash(password, "010102030405060708091011121314151617181920"));
            Assert.IsFalse(sd.VerifyPasswordHash(password, "01010203040506070809101112131415161718192021222324252627282930313233343536373839"));
            Assert.IsFalse(sd.VerifyPasswordHash(password, "010102030405060708091011121314151617181920212223242526272829303132333435363738394041"));
        }

        [Test]
        public void Test_HashPassword_RealIterations()
        {
            string key = "12345678901234567890123456789012";
            SecureHash[] sds = new[]
            {
                new SecureHash(1, key, 100000),
                new SecureHash(2, key, 100000)
            };

            string password = "Password01";

            foreach (var sd in sds)
            {
                Stopwatch sw = Stopwatch.StartNew();
                string hash = sd.HashPassword(password);
                sw.Stop();
                Console.WriteLine(sw.Elapsed);

                foreach (var sdv in sds)
                    Assert.IsTrue(sdv.VerifyPasswordHash(password, hash));
            }
        }

        [Test]
        public void Test_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => new SecureHash(1, null, 1));
            Assert.Throws<ArgumentException>(() => new SecureHash(200, "12345678901234567890123456789012", 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new SecureHash(1, "12345678901234567890123456789012", 0));
            Assert.Throws<ArgumentException>(() => new SecureHash(1, "1234", 1));
        }
    }
}