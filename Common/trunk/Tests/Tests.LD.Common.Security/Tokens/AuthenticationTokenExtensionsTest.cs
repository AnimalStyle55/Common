using LD.Common.Security.Tokens;
using LD.Common.Utils;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.LD.Common.Security.Tokens
{
    [TestFixture]
    class AuthenticationTokenExtensionsTest
    {
        private byte[] _key = HexUtil.ToBytes("12345678901234567890123456789012");
        private Guid _guid = new Guid("e651d1e2-58af-4aba-bb3e-794c0d699146");
        private TimeSpan _exp = TimeSpan.FromHours(1);

        public enum TestEnum
        {
            EnumName1,
            EnumName2,
            EnumName3
        }

        private class TestObject
        {
            public string StringField { get; set; }
            public int IntField { get; set; }
            public bool BoolField { get; set; }
            public List<string> ListOfStrings { get; set; }
            public List<TestObject> ListOfObjs { get; set; }
        }

        [Test]
        public void Test_GetDataField_Scalars()
        {
            var token = new AuthenticationToken(_key, _guid, _exp,
                Tuple.Create<string, object>("stringparam", "value1"),
                Tuple.Create<string, object>("intparam", 5573),
                Tuple.Create<string, object>("longparam", 287472657252525L),
                Tuple.Create<string, object>("boolparamT", true),
                Tuple.Create<string, object>("boolparamF", false),
                Tuple.Create<string, object>("doubleparam", 1.7462));

            Assert.AreEqual("value1", token.GetDataField<string>("stringparam"));
            Assert.AreEqual(5573, token.GetDataField<int>("intparam"));
            Assert.AreEqual(287472657252525L, token.GetDataField<long>("longparam"));
            Assert.AreEqual(true, token.GetDataField<bool>("boolparamT"));
            Assert.AreEqual(false, token.GetDataField<bool>("boolparamF"));
            Assert.AreEqual(1.7462, token.GetDataField<double>("doubleparam"));

            token = new AuthenticationToken(_key, token.Token);

            Assert.AreEqual("value1", token.GetDataField<string>("stringparam"));
            Assert.AreEqual(5573, token.GetDataField<int>("intparam"));
            Assert.AreEqual(287472657252525L, token.GetDataField<long>("longparam"));
            Assert.AreEqual(true, token.GetDataField<bool>("boolparamT"));
            Assert.AreEqual(false, token.GetDataField<bool>("boolparamF"));
            Assert.AreEqual(1.7462, token.GetDataField<double>("doubleparam"));
        }

        [Test]
        public void Test_GetDataField_Defaults()
        {
            var token = new AuthenticationToken(_key, _guid, _exp,
                Tuple.Create<string, object>("stringparam", "value1"),
                Tuple.Create<string, object>("intparam", 5573));

            Assert.AreEqual("value1", token.GetDataField<string>("stringparam"));
            Assert.AreEqual(5573, token.GetDataField<int>("intparam"));

            Assert.AreEqual("default", token.GetDataField<string>("notthere", "default"));
            Assert.AreEqual(55, token.GetDataField<int>("notthere", 55));
            Assert.IsNull(token.GetDataField<List<string>>("notthere"));

            token = new AuthenticationToken(_key, token.Token);

            Assert.AreEqual("value1", token.GetDataField<string>("stringparam"));
            Assert.AreEqual(5573, token.GetDataField<int>("intparam"));

            Assert.AreEqual("default", token.GetDataField<string>("notthere", "default"));
            Assert.AreEqual(55, token.GetDataField<int>("notthere", 55));
            Assert.IsNull(token.GetDataField<List<string>>("notthere"));
        }

        [Test]
        public void Test_GetDataField_ThereButNull()
        {
            var token = new AuthenticationToken(_key, _guid, _exp,
                Tuple.Create<string, object>("stringparam", (string)null),
                Tuple.Create<string, object>("intparam", (int?)null),
                Tuple.Create<string, object>("longparam", (long?)null),
                Tuple.Create<string, object>("boolparamT", (bool?)null),
                Tuple.Create<string, object>("doubleparam", (double?)null),
                Tuple.Create<string, object>("objparam", (TestObject)null),
                Tuple.Create<string, object>("listparam", (List<string>)null));

            Assert.IsNull(token.GetDataField<string>("stringparam"));
            Assert.IsNull(token.GetDataField<int?>("intparam"));
            Assert.IsNull(token.GetDataField<long?>("longparam"));
            Assert.IsNull(token.GetDataField<bool?>("boolparamT"));
            Assert.IsNull(token.GetDataField<bool?>("boolparamF"));
            Assert.IsNull(token.GetDataField<double?>("doubleparam"));
            Assert.IsNull(token.GetDataField<TestObject>("objparam"));
            Assert.IsNull(token.GetDataField<List<string>>("listparam"));

            token = new AuthenticationToken(_key, token.Token);

            Assert.IsNull(token.GetDataField<string>("stringparam"));
            Assert.IsNull(token.GetDataField<int?>("intparam"));
            Assert.IsNull(token.GetDataField<long?>("longparam"));
            Assert.IsNull(token.GetDataField<bool?>("boolparamT"));
            Assert.IsNull(token.GetDataField<bool?>("boolparamF"));
            Assert.IsNull(token.GetDataField<double?>("doubleparam"));
            Assert.IsNull(token.GetDataField<TestObject>("objparam"));
            Assert.IsNull(token.GetDataField<List<string>>("listparam"));
        }

        [Test]
        public void Test_TryGetDataField_Defaults()
        {
            var token = new AuthenticationToken(_key, _guid, _exp,
                Tuple.Create<string, object>("stringparam", "value1"),
                Tuple.Create<string, object>("intparam", 5573));

            string s;
            Assert.IsTrue(token.TryGetDataField<string>("stringparam", out s));
            Assert.AreEqual("value1", s);
            int i;
            Assert.IsTrue(token.TryGetDataField<int>("intparam", out i));
            Assert.AreEqual(5573, i);

            Assert.IsFalse(token.TryGetDataField<string>("notthere", out s));
            Assert.IsFalse(token.TryGetDataField<int>("notthere", out i));
            List<string> list;
            Assert.IsFalse(token.TryGetDataField<List<string>>("notthere", out list));

            token = new AuthenticationToken(_key, token.Token);

            string s2;
            Assert.IsTrue(token.TryGetDataField<string>("stringparam", out s2));
            Assert.AreEqual("value1", s2);
            int i2;
            Assert.IsTrue(token.TryGetDataField<int>("intparam", out i2));
            Assert.AreEqual(5573, i2);

            Assert.IsFalse(token.TryGetDataField<string>("notthere", out s));
            Assert.IsFalse(token.TryGetDataField<int>("notthere", out i));
            List<string> list2;
            Assert.IsFalse(token.TryGetDataField<List<string>>("notthere", out list2));
        }

        [Test]
        public void Test_GetDataField_Objects()
        {
            var obj = new TestObject()
            {
                StringField = "str1",
                IntField = 48,
                BoolField = true,
                ListOfStrings = new List<string>() { "a", "b", "c" },
                ListOfObjs = new List<TestObject>() { new TestObject() { IntField = 5 }, new TestObject() { StringField = "hello" } }
            };

            var token = new AuthenticationToken(_key, _guid, _exp,
                Tuple.Create<string, object>("obj", obj),
                Tuple.Create<string, object>("enum", TestEnum.EnumName2),
                Tuple.Create<string, object>("enumlist", new List<TestEnum>() { TestEnum.EnumName2, TestEnum.EnumName3 }),
                Tuple.Create<string, object>("intlist", new List<int>() { 1, 5, 10 }),
                Tuple.Create<string, object>("stringlist", new List<string>() { "10", "4", "2" }),
                Tuple.Create<string, object>("set", new HashSet<string>() { "d", "e", "f" }),
                Tuple.Create<string, object>("listofobj", obj.ListOfObjs));

            Assert.AreEqual(TestEnum.EnumName2, token.GetDataField<TestEnum>("enum"));
            TestObject to = token.GetDataField<TestObject>("obj");
            Assert.AreEqual("str1", to.StringField);
            Assert.AreEqual(48, to.IntField);
            Assert.AreEqual(true, to.BoolField);
            CollectionAssert.AreEqual(new List<string>() { "a", "b", "c" }, to.ListOfStrings);
            Assert.AreEqual(2, to.ListOfObjs.Count);
            Assert.AreEqual(5, to.ListOfObjs[0].IntField);
            Assert.AreEqual("hello", to.ListOfObjs[1].StringField);

            CollectionAssert.AreEqual(new List<TestEnum>() { TestEnum.EnumName2, TestEnum.EnumName3 }, token.GetDataField<List<TestEnum>>("enumlist"));
            CollectionAssert.AreEqual(new List<int>() { 1, 5, 10 }, token.GetDataField<List<int>>("intlist"));
            CollectionAssert.AreEqual(new List<string>() { "10", "4", "2" }, token.GetDataField<List<string>>("stringlist"));
            CollectionAssert.AreEquivalent(new HashSet<string>() { "d", "e", "f" }, token.GetDataField<HashSet<string>>("set"));
            var list = token.GetDataField<List<TestObject>>("listofobj");
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(5, list[0].IntField);
            Assert.AreEqual("hello", list[1].StringField);

            token = new AuthenticationToken(_key, token.Token);

            Assert.AreEqual(TestEnum.EnumName2, token.GetDataField<TestEnum>("enum"));
            to = token.GetDataField<TestObject>("obj");
            Assert.AreEqual("str1", to.StringField);
            Assert.AreEqual(48, to.IntField);
            Assert.AreEqual(true, to.BoolField);
            CollectionAssert.AreEqual(new List<string>() { "a", "b", "c" }, to.ListOfStrings);
            Assert.AreEqual(2, to.ListOfObjs.Count);
            Assert.AreEqual(5, to.ListOfObjs[0].IntField);
            Assert.AreEqual("hello", to.ListOfObjs[1].StringField);

            CollectionAssert.AreEqual(new List<TestEnum>() { TestEnum.EnumName2, TestEnum.EnumName3 }, token.GetDataField<List<TestEnum>>("enumlist"));
            CollectionAssert.AreEqual(new List<int>() { 1, 5, 10 }, token.GetDataField<List<int>>("intlist"));
            CollectionAssert.AreEqual(new List<string>() { "10", "4", "2" }, token.GetDataField<List<string>>("stringlist"));
            CollectionAssert.AreEquivalent(new HashSet<string>() { "d", "e", "f" }, token.GetDataField<HashSet<string>>("set"));
            list = token.GetDataField<List<TestObject>>("listofobj");
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(5, list[0].IntField);
            Assert.AreEqual("hello", list[1].StringField);
        }

        [Test]
        public void Test_GetDataField_CannotConvert()
        {
            var obj = new TestObject()
            {
                StringField = "str1",
                IntField = 48,
                BoolField = true
            };
            
            var token = new AuthenticationToken(_key, _guid, _exp,
                Tuple.Create<string, object>("str", "value1"),
                Tuple.Create<string, object>("obj", obj)
                );

            string str;
            TestObject o;
            Assert.IsFalse(token.TryGetDataField<TestObject>("str", out o));
            Assert.IsFalse(token.TryGetDataField<string>("obj", out str));
        }
    }
}
