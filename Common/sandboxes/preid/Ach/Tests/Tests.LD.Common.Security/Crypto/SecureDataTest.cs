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
    public class SecureDataTest
    {
        [Test]
        public void Test_Encrypt_String()
        {
            string ssn = "123-45-6789";

            SecureData enc = new SecureData("12121234343456565678787890909009");

            string result = enc.EncryptToString(ssn);
            byte[] result2 = enc.Encrypt(ssn);

            string roundTrip = enc.DecryptToString(result);
            string roundTrip2 = enc.DecryptToString(result2);

            Assert.AreEqual(ssn, roundTrip);
            Assert.AreEqual(ssn, roundTrip2);
            Assert.AreNotEqual(ssn, result);

            // encrypt again, different result
            string result3 = enc.EncryptToString(ssn);
            Assert.AreNotEqual(result, result3);

            string roundTrip3 = enc.DecryptToString(result2);
            Assert.AreEqual(roundTrip, roundTrip3);
        }

        [Test]
        public void Test_Encrypt_Bytes()
        {
            byte[] data = Primitives.GetSecureRandomBytes(100000);

            SecureData enc = new SecureData("12345678901234567890123456789012");

            byte[] result = enc.Encrypt(data);
            string result2 = enc.EncryptToString(data);

            byte[] roundTrip = enc.Decrypt(result);
            byte[] roundTrip2 = enc.Decrypt(result2);

            CollectionAssert.AreEqual(data, roundTrip);
            CollectionAssert.AreEqual(data, roundTrip2);

            // encrypt again, different result
            byte[] result3 = enc.Encrypt(data);
            CollectionAssert.AreNotEqual(result, result3);

            byte[] roundTrip3 = enc.Decrypt(result2);
            CollectionAssert.AreEqual(roundTrip, roundTrip3);
        }

        [Test]
        public void Test_Null_Inputs()
        {
            SecureData enc = new SecureData("12345678901234567890123456789012");

            Assert.IsNull(enc.Encrypt((string)null));
            Assert.IsNull(enc.EncryptToString((string)null));
            Assert.IsNull(enc.Encrypt((byte[])null));
            Assert.IsNull(enc.EncryptToString((byte[])null));

            Assert.IsNull(enc.Decrypt((string)null));
            Assert.IsNull(enc.DecryptToString((string)null));
            Assert.IsNull(enc.Decrypt((byte[])null));
            Assert.IsNull(enc.DecryptToString((byte[])null));
        }

        [Test]
        public void Test_Encrypt_NullKey()
        {
            Assert.Throws<ArgumentNullException>(() => new SecureData(null));
        }

        [Test]
        public void Test_Encrypt_InvalidKey_SSN()
        {
            Assert.Throws<ArgumentException>(() => new SecureData("1234"));
        }

        [Test]
        public void Test_HashPassword()
        {
            SecureData sd = new SecureData("12345678901234567890123456789012", 0xff);

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
        public void Test_HashPassword_InvalidIterations()
        {
            SecureData sd = new SecureData("12345678901234567890123456789012", 0xff);

            string password = "Password01";
            string hash = sd.HashPassword(password);

            var sb = new StringBuilder(hash);
            sb[1] = '2';

            Assert.IsFalse(sd.VerifyPasswordHash(password, sb.ToString()));
        }

        [Test]
        public void Test_HashPassword_InvalidData()
        {
            SecureData sd = new SecureData("12345678901234567890123456789012", 0xff);

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
            SecureData[] sds = new[]
            {
                new SecureData(key, 1),
                new SecureData(key, 2)
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
    }
}