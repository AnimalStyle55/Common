using LD.Common.Security.Crypto;
using LD.Common.Utils;
using NUnit.Framework;
using System;
using System.Text;

namespace Tests.LD.Common.Security.Crypto
{
    [TestFixture]
    public class PrimitivesTest
    {
        [Test]
        public void Test_GetSecureRandomBytes()
        {
            byte[] bytes = Primitives.GetSecureRandomBytes(6);
            Assert.AreEqual(6, bytes.Length);

            byte[] bytes2 = Primitives.GetSecureRandomBytes(6);
            CollectionAssert.AreNotEqual(bytes, bytes2);

            byte[] empty = Primitives.GetSecureRandomBytes(0);
            CollectionAssert.AreEqual(new byte[0], empty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_GetSecureRandomBytes_Negative()
        {
            Primitives.GetSecureRandomBytes(-5);
        }

        [Test]
        public void Test_HmacPBKDF2()
        {
            var salt = HexUtil.ToBytes("f8fe61afe0d709961d3057cdc9fcd647");
            Assert.AreEqual("969a21f6ae211b729b33ebd7999e1809e9b0b925", HexUtil.ToHex(Primitives.HmacPBKDF2("123-45-6789", salt, 20000)));
            Assert.AreEqual("4ddcc7f70fd1887a4af6fb93540c7bf791fb13fe", HexUtil.ToHex(Primitives.HmacPBKDF2("984-48-2735", salt, 20000)));
            Assert.AreEqual("b58b8b17602dc024e559f4560a02d126b00e7bbf", HexUtil.ToHex(Primitives.HmacPBKDF2("984-48-2736", salt, 20000)));
        }

        [Test]
        public void Test_HmacPBKDF2_KeyStretching()
        {
            byte[] salt = HexUtil.ToBytes("f8fe61afe0d709961d3057cdc9fcd647");
            Assert.AreEqual("f72bb3c551b15e9f219ee6b12984fcd0", HexUtil.ToHex(Primitives.HmacPBKDF2(HexUtil.ToBytes("8e81f697bc8dd0caab83399de5d5424e"), salt, 16, 200)));
            Assert.AreEqual("ad273ccae2a4dc03d6538d7ba9593c2b", HexUtil.ToHex(Primitives.HmacPBKDF2(HexUtil.ToBytes("c9ccb1a49ff16fc930691c00d0d04222"), salt, 16, 200)));
            Assert.AreEqual("df80c54c12e420dacb569e259e9cc95f", HexUtil.ToHex(Primitives.HmacPBKDF2(HexUtil.ToBytes("c9ccb1a49ff16fc930691c00d0d04223"), salt, 16, 200)));

            // note how these 32-byte hashes have the same 1st block at the above 16-byte hashes
            // the correct way to key-stretch is to gen a 16-byte from your key, and then gen a 32-byte from that key
            Assert.AreEqual("f72bb3c551b15e9f219ee6b12984fcd0b17bd4ac4720aebbaaaaf2a05aa31c88", HexUtil.ToHex(Primitives.HmacPBKDF2(HexUtil.ToBytes("8e81f697bc8dd0caab83399de5d5424e"), salt, 32, 200)));
            Assert.AreEqual("ad273ccae2a4dc03d6538d7ba9593c2be32c3a461bf62054ebb4f577c991bc5a", HexUtil.ToHex(Primitives.HmacPBKDF2(HexUtil.ToBytes("c9ccb1a49ff16fc930691c00d0d04222"), salt, 32, 200)));
            Assert.AreEqual("df80c54c12e420dacb569e259e9cc95f61f7245c6357b8e515e76bd4aab84f9a", HexUtil.ToHex(Primitives.HmacPBKDF2(HexUtil.ToBytes("c9ccb1a49ff16fc930691c00d0d04223"), salt, 32, 200)));

            byte[] keyPrime = Primitives.HmacPBKDF2(HexUtil.ToBytes("8e81f697bc8dd0caab83399de5d5424e"), salt, 16, 200);
            string keys = HexUtil.ToHex(Primitives.HmacPBKDF2(keyPrime, salt, 32, 200));
            Assert.AreEqual("4ad0c7f1e0fb9fe844da6793937de84d", keys.Substring(0, 32));
            Assert.AreEqual("c734b07053681bfefaeef9f5ea55b1ff", keys.Substring(32, 32));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_HmacPBKDF2_Null()
        {
            Primitives.HmacPBKDF2(null, HexUtil.ToBytes("1234"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_HmacPBKDF2_Null2()
        {
            Primitives.HmacPBKDF2("plain", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_HmacPBKDF2_IterToSmall()
        {
            Primitives.HmacPBKDF2("plain", HexUtil.ToBytes("1234"), 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_HmacPBKDF2_IterToSmall2()
        {
            Primitives.HmacPBKDF2("plain", HexUtil.ToBytes("1234"), -5);
        }

        [Test]
        public void Test_EncryptAES()
        {
            byte[] key = HexUtil.ToBytes("9b9111948a32b817dee2dd6a513b7064");
            byte[] iv = HexUtil.ToBytes("0ee7f5ecffc33809732c63d764ea82ec");

            byte[] ct = Primitives.EncryptAES_CBC(key, HexUtil.ToBytes("11223344"), iv);
            Assert.AreEqual("fbe0a340bbc33f2bbda63fe94a3d71b2", HexUtil.ToHex(ct));

            byte[] pt = Primitives.DecryptAES_CBC(key, ct, iv);
            Assert.AreEqual("11223344", HexUtil.ToHex(pt));
        }

        [Test]
        public void Test_EncryptAES_Big()
        {
            byte[] key = HexUtil.ToBytes("9b9111948a32b817dee2dd6a513b7064");
            byte[] iv = HexUtil.ToBytes("0ee7f5ecffc33809732c63d764ea82ec");

            StringBuilder sb = new StringBuilder();
            sb.Insert(0, "11223344", 128);
            string plain = sb.ToString();
            byte[] ct = Primitives.EncryptAES_CBC(key, HexUtil.ToBytes(plain), iv);
            Assert.AreEqual("1101333bdcc4cf35221e4f1d8bf41f0aa8e08853ee62bc492ea927281ee9f55a741103797bb9cd0224359f5067e0de14f3149b67f52dccd6da9b3639180be826aee37600857458c4caee0ccc219ad43cc5c15cee25ca9a902bf093a8d076ab03728c3d2526070da0bfeebd142ac46626c52ddc15084ad4855e6eea5be6bdcb262a77067ad3e05d067fa4ae02db3510a79bf5197d276879ab226480d2f78e68d5a05c8b32ef92011c3f5b431fa2fb80157d0d29d7835006da6900a065e03f33257c363fc31c11a9bb6ee463f71b7d02f538f16120f8d6773eca606877624e36bd4b961170274cebc1a216813af5810000275761901a2b552b1df28d2a13ea09d9dd87227f6691a35957fb7e32bdebc75232dcdeaabbb050acefa838310eb55229c29615667577264f7b40bca6a45292ac09e82b09eb2ccf435745e9841c2e42a33a5ea6e5305241eb4453532e741ac3b679e2114df5a26c89d87cbff26787ecc6da69e4a410a1702df7fadb5cba4ed01f5e04b109339a287760cbd14ab06fc1bf5d23239e8d9eeacc173b2a5da8e4e99f5b25d7d364004b85da37d3a344c22e9bbc17fb9feb570c6f63030dc5b45edd0f90598c2088db761712ef84cefd7aee13f0d0d4ce106e20236696b668b2273f3e5966341de4f18dd6aa0d68db5b7009bfcd369843f4f8d8b2c9b61838decd4f0bcc5cbaa2b8b05aed7b311323fb70003395ed32f3bf9fb80563a39b323b851a7e", HexUtil.ToHex(ct));

            byte[] pt = Primitives.DecryptAES_CBC(key, ct, iv);
            Assert.AreEqual(plain, HexUtil.ToHex(pt));

            byte[] buffer = new byte[ct.Length + 10];
            Array.Copy(ct, 0, buffer, 5, ct.Length);
            byte[] pt2 = Primitives.DecryptAES_CBC(key, buffer, 5, ct.Length, iv);
            Assert.AreEqual(plain, HexUtil.ToHex(pt2));
        }

        [Test]
        public void Test_EncryptAES_2Blocks()
        {
            byte[] key = HexUtil.ToBytes("9b9111948a32b817dee2dd6a513b7064");
            byte[] iv = HexUtil.ToBytes("0ee7f5ecffc33809732c63d764ea82ec");

            string plain = "1122334455667788990011223344556677889900112233";
            byte[] ct = Primitives.EncryptAES_CBC(key, HexUtil.ToBytes(plain), iv);
            Assert.AreEqual("7cce0142c9a4e4f6f0be4bf22511a2a7ab07cefe44ea98e135b59dae558072c6", HexUtil.ToHex(ct));

            byte[] pt = Primitives.DecryptAES_CBC(key, ct, iv);
            Assert.AreEqual("1122334455667788990011223344556677889900112233", HexUtil.ToHex(pt));

            byte[] buffer = new byte[ct.Length + 10];
            Array.Copy(ct, 0, buffer, 5, ct.Length);
            byte[] pt2 = Primitives.DecryptAES_CBC(key, buffer, 5, ct.Length, iv);
            Assert.AreEqual(plain, HexUtil.ToHex(pt2));

            // 2nd block change only changes 2nd block
            byte[] ct2 = Primitives.EncryptAES_CBC(key, HexUtil.ToBytes("1122334455667788990011223344556677889900112234"), iv);
            Assert.AreEqual("7cce0142c9a4e4f6f0be4bf22511a2a762382df5a7134b23e8a92487afe67b75", HexUtil.ToHex(ct2));
            Assert.AreNotEqual(HexUtil.ToHex(ct), HexUtil.ToHex(ct2));
        }

        [Test]
        public void Test_EncryptAES_IVChangesOutput()
        {
            byte[] key = HexUtil.ToBytes("9b9111948a32b817dee2dd6a513b7064");
            byte[] iv1 = HexUtil.ToBytes("0ee7f5ecffc33809732c63d764ea82ec");
            byte[] iv2 = HexUtil.ToBytes("0ee7f5ecffc33809732c63d764ea82e1");

            byte[] ct1 = Primitives.EncryptAES_CBC(key, HexUtil.ToBytes("11223344"), iv1);
            byte[] ct2 = Primitives.EncryptAES_CBC(key, HexUtil.ToBytes("11223344"), iv2);
            Assert.AreEqual("fbe0a340bbc33f2bbda63fe94a3d71b2", HexUtil.ToHex(ct1));
            Assert.AreEqual("f3aa851801d09fb182b11324d6e87f6a", HexUtil.ToHex(ct2));

            byte[] pt1 = Primitives.DecryptAES_CBC(key, ct1, iv1);
            Assert.AreEqual("11223344", HexUtil.ToHex(pt1));

            byte[] buffer1 = new byte[ct1.Length + 10];
            Array.Copy(ct1, 0, buffer1, 5, ct1.Length);
            byte[] pt1_buf = Primitives.DecryptAES_CBC(key, buffer1, 5, ct1.Length, iv1);
            Assert.AreEqual("11223344", HexUtil.ToHex(pt1_buf));

            byte[] pt2 = Primitives.DecryptAES_CBC(key, ct2, iv2);
            Assert.AreEqual("11223344", HexUtil.ToHex(pt2));

            byte[] buffer2 = new byte[ct2.Length + 10];
            Array.Copy(ct2, 0, buffer2, 5, ct2.Length);
            byte[] pt2_buf = Primitives.DecryptAES_CBC(key, buffer2, 5, ct2.Length, iv2);
            Assert.AreEqual("11223344", HexUtil.ToHex(pt2_buf));
        }

        [Test]
        public void Test_EncryptAES_PTChangesOutput()
        {
            byte[] key = HexUtil.ToBytes("9b9111948a32b817dee2dd6a513b7064");
            byte[] iv = HexUtil.ToBytes("0ee7f5ecffc33809732c63d764ea82ec");

            byte[] ct1 = Primitives.EncryptAES_CBC(key, HexUtil.ToBytes("11223344"), iv);
            byte[] ct2 = Primitives.EncryptAES_CBC(key, HexUtil.ToBytes("11223345"), iv);
            Assert.AreEqual("fbe0a340bbc33f2bbda63fe94a3d71b2", HexUtil.ToHex(ct1));
            Assert.AreEqual("1b220f7f6b09d35543da0e76bed8dfed", HexUtil.ToHex(ct2));

            byte[] pt1 = Primitives.DecryptAES_CBC(key, ct1, iv);
            Assert.AreEqual("11223344", HexUtil.ToHex(pt1));

            byte[] buffer1 = new byte[ct1.Length + 10];
            Array.Copy(ct1, 0, buffer1, 5, ct1.Length);
            byte[] pt1_buf = Primitives.DecryptAES_CBC(key, buffer1, 5, ct1.Length, iv);
            Assert.AreEqual("11223344", HexUtil.ToHex(pt1_buf));

            byte[] pt2 = Primitives.DecryptAES_CBC(key, ct2, iv);
            Assert.AreEqual("11223345", HexUtil.ToHex(pt2));

            byte[] buffer2 = new byte[ct2.Length + 10];
            Array.Copy(ct2, 0, buffer2, 5, ct2.Length);
            byte[] pt2_buf = Primitives.DecryptAES_CBC(key, buffer2, 5, ct2.Length, iv);
            Assert.AreEqual("11223345", HexUtil.ToHex(pt2_buf));
        }

        [Test]
        public void Test_EncryptAES_256bitKey()
        {
            byte[] key = HexUtil.ToBytes("9b9111948a32b817dee2dd6a513b7064c19b1aa3df43395600b762fea8b2b29b");
            byte[] iv = HexUtil.ToBytes("0ee7f5ecffc33809732c63d764ea82ec");

            byte[] ct1 = Primitives.EncryptAES_CBC(key, HexUtil.ToBytes("11223344"), iv);
            Assert.AreEqual("e375fcb6ac2ed41ead39d5044baea79d", HexUtil.ToHex(ct1));

            byte[] pt1 = Primitives.DecryptAES_CBC(key, ct1, iv);
            Assert.AreEqual("11223344", HexUtil.ToHex(pt1));

            byte[] buffer = new byte[ct1.Length + 10];
            Array.Copy(ct1, 0, buffer, 5, ct1.Length);
            byte[] pt2 = Primitives.DecryptAES_CBC(key, buffer, 5, ct1.Length, iv);
            Assert.AreEqual("11223344", HexUtil.ToHex(pt2));
        }

        private void test_HmacSHA256(string expected, string data, string key)
        {
            Assert.AreEqual(expected, HexUtil.ToHex(Primitives.HmacSHA256(HexUtil.ToBytes(data), HexUtil.ToBytes(key))));
        }

        [Test]
        public void Test_HmacSHA256()
        {
            test_HmacSHA256("b0344c61d8db38535ca8afceaf0bf12b881dc200c9833da726e9376c2e32cff7", "4869205468657265", "0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b");
            test_HmacSHA256("b436e3e86cb3800b3864aeecc8d06c126f005e7645803461717a8e4b2de3a905", "616d6f756e743d3130302663757272656e63793d455552", "57617b5d2349434b34734345635073433835777e2d244c31715535255a366773755a4d70532a5879793238235f707c4f7865753f3f446e633a21575643303f66");
        }

        private void test_VerifyHmacSHA256(string hmac, string data, string key)
        {
            byte[] hmacB = HexUtil.ToBytes(hmac);
            byte[] dataB = HexUtil.ToBytes(data);
            Assert.IsTrue(Primitives.VerifyHmacSHA256(hmacB, dataB, HexUtil.ToBytes(key)));

            byte[] buffer = new byte[hmacB.Length + dataB.Length + 10];
            Array.Copy(hmacB, 0, buffer, 5, hmacB.Length);
            Array.Copy(dataB, 0, buffer, 10 + hmacB.Length, dataB.Length);
            Assert.IsTrue(Primitives.VerifyHmacSHA256(buffer, 5, 10 + hmacB.Length, dataB.Length, HexUtil.ToBytes(key)));

            // invalid hmac
            hmacB[3] = 0x74;
            Assert.IsFalse(Primitives.VerifyHmacSHA256(hmacB, dataB, HexUtil.ToBytes(key)));

            buffer = new byte[hmacB.Length + dataB.Length + 10];
            Array.Copy(hmacB, 0, buffer, 5, hmacB.Length);
            Array.Copy(dataB, 0, buffer, 10 + hmacB.Length, dataB.Length);
            Assert.IsFalse(Primitives.VerifyHmacSHA256(buffer, 5, 10 + hmacB.Length, dataB.Length, HexUtil.ToBytes(key)));

            // invalid data
            hmacB = HexUtil.ToBytes(hmac);
            dataB[dataB.Length - 1] = 0x86;
            Assert.IsFalse(Primitives.VerifyHmacSHA256(hmacB, dataB, HexUtil.ToBytes(key)));

            buffer = new byte[hmacB.Length + dataB.Length + 10];
            Array.Copy(hmacB, 0, buffer, 5, hmacB.Length);
            Array.Copy(dataB, 0, buffer, 10 + hmacB.Length, dataB.Length);
            Assert.IsFalse(Primitives.VerifyHmacSHA256(buffer, 5, 10 + hmacB.Length, dataB.Length, HexUtil.ToBytes(key)));

            // invalid data and hmac
            hmacB[3] = 0x74;
            Assert.IsFalse(Primitives.VerifyHmacSHA256(hmacB, dataB, HexUtil.ToBytes(key)));

            buffer = new byte[hmacB.Length + dataB.Length + 10];
            Array.Copy(hmacB, 0, buffer, 5, hmacB.Length);
            Array.Copy(dataB, 0, buffer, 10 + hmacB.Length, dataB.Length);
            Assert.IsFalse(Primitives.VerifyHmacSHA256(buffer, 5, 10 + hmacB.Length, dataB.Length, HexUtil.ToBytes(key)));
        }

        [Test]
        public void Test_VerifyHmacSHA256()
        {
            test_VerifyHmacSHA256("b0344c61d8db38535ca8afceaf0bf12b881dc200c9833da726e9376c2e32cff7", "4869205468657265", "0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b");
            test_VerifyHmacSHA256("b436e3e86cb3800b3864aeecc8d06c126f005e7645803461717a8e4b2de3a905", "616d6f756e743d3130302663757272656e63793d455552", "57617b5d2349434b34734345635073433835777e2d244c31715535255a366773755a4d70532a5879793238235f707c4f7865753f3f446e633a21575643303f66");
        }

        [Test]
        public void Test_SecureEquals()
        {
            Assert.IsTrue(Primitives.SecureEquals(HexUtil.ToBytes("11"), HexUtil.ToBytes("11")));
            Assert.IsTrue(Primitives.SecureEquals(HexUtil.ToBytes("112233445566"), HexUtil.ToBytes("112233445566")));

            Assert.IsTrue(Primitives.SecureEquals(HexUtil.ToBytes("aaaa11aaaa"), 2, 1, HexUtil.ToBytes("bb11ccddee"), 1, 1));
            Assert.IsTrue(Primitives.SecureEquals(HexUtil.ToBytes("aabbccdd112233445566aabb"), 4, 6, HexUtil.ToBytes("aabbccddeeff112233445566"), 6, 6));

            Assert.IsFalse(Primitives.SecureEquals(HexUtil.ToBytes("11"), HexUtil.ToBytes("33")));
            Assert.IsFalse(Primitives.SecureEquals(HexUtil.ToBytes("112233445566"), HexUtil.ToBytes("2764592f2861")));

            Assert.IsFalse(Primitives.SecureEquals(HexUtil.ToBytes("aaaa11aaaa"), 2, 1, HexUtil.ToBytes("bb33ccddee"), 1, 1));
            Assert.IsFalse(Primitives.SecureEquals(HexUtil.ToBytes("aabbccdd112233445566aabb"), 4, 6, HexUtil.ToBytes("aabbccddeeff2764592f2861"), 6, 6));

            Assert.IsFalse(Primitives.SecureEquals(HexUtil.ToBytes("11"), HexUtil.ToBytes("0d5dd463cb69c57a19799b95af5c3b4e")));
            Assert.IsFalse(Primitives.SecureEquals(HexUtil.ToBytes("112233445566"), HexUtil.ToBytes("edea5ca8fd7985935897ce6e2868a7302764592f2861")));

            Assert.IsFalse(Primitives.SecureEquals(HexUtil.ToBytes("aaaa11aaaa"), 2, 1, HexUtil.ToBytes("bb0d5dd463cb69c57a19799b95af5c3b4eccddee"), 1, 16));
            Assert.IsFalse(Primitives.SecureEquals(HexUtil.ToBytes("aabbccdd112233445566aabb"), 4, 6, HexUtil.ToBytes("aabbccddeeffedea5ca8fd7985935897ce6e2868a7302764592f2861"), 6, 16));
        }
    }
}