using LD.Common.Security.Tokens;
using LD.Common.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.LD.Common.Security.Tokens
{
    [TestFixture]
    public class AuthenticationTokenTest
    {
        private byte[] _key = HexUtil.ToBytes("12345678901234567890123456789012");

        [Test]
        public void Test_Create()
        {
            var guid = new Guid("e651d1e2-58af-4aba-bb3e-794c0d699146");
            var token = new AuthenticationToken(_key, guid, TimeSpan.FromHours(1));

            Assert.AreEqual(guid, token.SubjectGuid);
            Assert.Less(DateTime.UtcNow - token.Issued, TimeSpan.FromSeconds(2));
            Assert.Less(token.Expires - DateTime.UtcNow - TimeSpan.FromHours(1), TimeSpan.FromSeconds(2));
            Assert.AreEqual(DateTimeKind.Utc, token.Expires.Kind);
            Assert.AreEqual(DateTimeKind.Utc, token.Issued.Kind);
            Assert.IsFalse(token.IsExpired);
            Assert.AreNotEqual(guid.ToString(), token.Token);
            Assert.IsNull(token.Data);
        }

        [Test]
        public void Test_Decrypt()
        {
            var guid = new Guid("e651d1e2-58af-4aba-bb3e-794c0d699146");
            var origToken = new AuthenticationToken(_key, guid, TimeSpan.FromHours(1));

            var token = new AuthenticationToken(_key, origToken.Token);

            Assert.AreEqual(guid, token.SubjectGuid);
            Assert.Less(DateTime.UtcNow - token.Issued, TimeSpan.FromSeconds(2));
            Assert.Less(token.Expires - DateTime.UtcNow - TimeSpan.FromHours(1), TimeSpan.FromSeconds(2));
            Assert.IsFalse(token.IsExpired);
        }

        [Test]
        public void Test_Invalid_Decrypt()
        {
            var guid = new Guid("e651d1e2-58af-4aba-bb3e-794c0d699146");
            var token = new AuthenticationToken(_key, guid, TimeSpan.FromHours(1));

            var tokenContents = new StringBuilder(token.Token);
            tokenContents[12] = tokenContents[12] == 'a' ? 'b' : 'a';

            Assert.Throws<InvalidTokenException>(() => new AuthenticationToken(_key, tokenContents.ToString()));
        }

        [Test]
        public void Test_Wrong_Key()
        {
            var guid = new Guid("e651d1e2-58af-4aba-bb3e-794c0d699146");
            var token = new AuthenticationToken(_key, guid, TimeSpan.FromHours(1));

            var key2 = HexUtil.ToBytes("55555555555555555555555555555555");

            Assert.Throws<InvalidTokenException>(() => new AuthenticationToken(key2, token.Token));
        }

        [Test]
        public void Test_Expired()
        {
            var guid = new Guid("e651d1e2-58af-4aba-bb3e-794c0d699146");
            var origToken = new AuthenticationToken(_key, guid, TimeSpan.FromMilliseconds(1));

            System.Threading.Thread.Sleep(20);

            var token = new AuthenticationToken(_key, origToken.Token);

            Assert.IsTrue(token.IsExpired);
        }

        [Test]
        public void Test_Data()
        {
            var guid = new Guid("e651d1e2-58af-4aba-bb3e-794c0d699146");
            var data = new Dictionary<string, object>()
            {
                { "test", "value" },
                { "test2", "value2" }
            };
            var token = new AuthenticationToken(_key, guid, TimeSpan.FromHours(1), data);

            Assert.AreEqual(guid, token.SubjectGuid);
            Assert.Less(DateTime.UtcNow - token.Issued, TimeSpan.FromSeconds(2));
            Assert.Less(token.Expires - DateTime.UtcNow - TimeSpan.FromHours(1), TimeSpan.FromSeconds(2));
            Assert.IsFalse(token.IsExpired);
            Assert.AreNotEqual(guid.ToString(), token.Token);
            Assert.IsNotNull(token.Data);
            CollectionAssert.AreEquivalent(data, token.Data);

            token = new AuthenticationToken(_key, token.Token);

            Assert.AreEqual(guid, token.SubjectGuid);
            CollectionAssert.AreEquivalent(data, token.Data);
        }

        [Test]
        public void Test_Data_Tuples()
        {
            var guid = new Guid("e651d1e2-58af-4aba-bb3e-794c0d699146");
            var token = new AuthenticationToken(_key, guid, TimeSpan.FromHours(1), Tuple.Create("test", (object)"value"), Tuple.Create("test2", (object)"value2"));

            Assert.AreEqual(guid, token.SubjectGuid);
            Assert.Less(DateTime.UtcNow - token.Issued, TimeSpan.FromSeconds(2));
            Assert.Less(token.Expires - DateTime.UtcNow - TimeSpan.FromHours(1), TimeSpan.FromSeconds(2));
            Assert.IsFalse(token.IsExpired);
            Assert.AreNotEqual(guid.ToString(), token.Token);
            Assert.IsNotNull(token.Data);
            Assert.AreEqual(2, token.Data.Count);
            Assert.AreEqual("value", token.Data["test"]);
            Assert.AreEqual("value2", token.Data["test2"]);

            token = new AuthenticationToken(_key, token.Token);

            Assert.AreEqual(guid, token.SubjectGuid);
            Assert.AreEqual(2, token.Data.Count);
            Assert.AreEqual("value", token.Data["test"]);
            Assert.AreEqual("value2", token.Data["test2"]);
        }
    }
}