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

            string result = enc.EncryptToBase64(ssn);
            byte[] result2 = enc.Encrypt(ssn);
            string result3 = enc.EncryptToHex(ssn);

            string roundTrip = enc.DecryptBase64ToString(result);
            string roundTrip2 = enc.DecryptToString(result2);
            string roundTrip3 = enc.DecryptHexToString(result3);

            Assert.AreEqual(ssn, roundTrip);
            Assert.AreEqual(ssn, roundTrip2);
            Assert.AreEqual(ssn, roundTrip3);
            Assert.AreNotEqual(ssn, result);

            // encrypt again, different result
            string result2d = enc.EncryptToBase64(ssn);
            Assert.AreNotEqual(result, result2d);

            string result3d = enc.EncryptToHex(ssn);
            Assert.AreNotEqual(result, result3d);

            string roundTrip2_2 = enc.DecryptToString(result2);
            Assert.AreEqual(roundTrip, roundTrip2_2);
            Assert.AreEqual(roundTrip3, roundTrip2_2);
        }

        [Test]
        public void Test_Encrypt_Bytes()
        {
            byte[] data = Primitives.GetSecureRandomBytes(100000);

            SecureData enc = new SecureData("12345678901234567890123456789012");

            byte[] result = enc.Encrypt(data);
            string result2 = enc.EncryptToBase64(data);
            string result3 = enc.EncryptToHex(data);

            byte[] roundTrip = enc.Decrypt(result);
            byte[] roundTrip2 = enc.DecryptBase64(result2);
            byte[] roundTrip3 = enc.DecryptHex(result3);

            CollectionAssert.AreEqual(data, roundTrip);
            CollectionAssert.AreEqual(data, roundTrip2);
            CollectionAssert.AreEqual(data, roundTrip3);

            // encrypt again, different result
            byte[] result_again = enc.Encrypt(data);
            CollectionAssert.AreNotEqual(result, result_again);

            byte[] roundTrip2_2 = enc.DecryptBase64(result2);
            CollectionAssert.AreEqual(roundTrip, roundTrip2_2);

            byte[] roundTrip3_2 = enc.DecryptHex(result3);
            CollectionAssert.AreEqual(roundTrip, roundTrip3_2);
        }

        [Test]
        public void Test_Null_Inputs()
        {
            SecureData enc = new SecureData("12345678901234567890123456789012");

            Assert.IsNull(enc.Encrypt((string)null));
            Assert.IsNull(enc.EncryptToBase64((string)null));
            Assert.IsNull(enc.EncryptToHex((string)null));
            Assert.IsNull(enc.Encrypt((byte[])null));
            Assert.IsNull(enc.EncryptToBase64((byte[])null));
            Assert.IsNull(enc.EncryptToHex((byte[])null));

            Assert.IsNull(enc.DecryptBase64(null));
            Assert.IsNull(enc.DecryptHex(null));
            Assert.IsNull(enc.DecryptBase64ToString(null));
            Assert.IsNull(enc.DecryptHexToString(null));
            Assert.IsNull(enc.Decrypt(null));
            Assert.IsNull(enc.DecryptToString(null));
        }

        [Test]
        public void Test_Encrypt_NullKey()
        {
            Assert.Throws<ArgumentNullException>(() => new SecureData(null));
        }

        [Test]
        public void Test_Encrypt_InvalidKey()
        {
            Assert.Throws<ArgumentException>(() => new SecureData("1234"));
        }
    }
}