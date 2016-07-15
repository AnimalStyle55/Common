using LD.Common.Mapping;
using NUnit.Framework;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.LD.Common.Mapping
{
    [TestFixture]
    public class PropertyNameInjectionTest
    {
        public class Foo
        {
            public enum FooEnum
            {
                Unknown,
                Value1,
                Value2,
                Value3,
                Value4
            }

            public string String { get; set; }

            public FooEnum Enum1 { get; set; }
            public Bar.BarEnum Enum2 { get; set; }
            public FooEnum Enum3 { get; set; }
            public FooEnum Enum4 { get; set; }
            public FooEnum? Enum5 { get; set; }
            public FooEnum? Enum6 { get; set; }

            public FooEnum EnumE2S { get; set; }
            public FooEnum? EnumEq2S { get; set; }
            public FooEnum? EnumEqnull2S { get; set; }
            public string EnumS2E { get; set; }
            public string EnumSq2E { get; set; }
            public string EnumSq2Enull { get; set; }

            public int Int1 { get; set; }
            public int Int2 { get; set; }
            public int? Int3 { get; set; }
            public int? Int4 { get; set; }

            public Foo Child { get; set; }

            public Foo[] Array { get; set; }
            public List<Foo> List { get; set; }
            public Dictionary<string, string> ScalarDictionary { get; set; }
            public Dictionary<Foo, string> ObjectDictionary { get; set; }

            public string[] StringArray { get; set; }
            public List<string> StringList { get; set; }
            public List<int> IntList { get; set; }
            public int[] IntArray { get; set; }
            public List<int?> NullableIntList { get; set; }
            public int?[] NullableIntArray { get; set; }
        }

        public class Bar
        {
            public enum BarEnum
            {
                Unknown,
                Value1,
                Value2,
                Value3
            }

            public string String { get; set; }

            public BarEnum Enum1 { get; set; }
            public BarEnum Enum2 { get; set; }
            public BarEnum Enum3 { get; set; }
            public BarEnum? Enum4 { get; set; }
            public BarEnum Enum5 { get; set; }
            public BarEnum Enum6 { get; set; }

            public string EnumE2S { get; set; }
            public string EnumEq2S { get; set; }
            public string EnumEqnull2S { get; set; }
            public BarEnum EnumS2E { get; set; }
            public BarEnum? EnumSq2E { get; set; }
            public BarEnum? EnumSq2Enull { get; set; }

            public int Int1 { get; set; }
            public int? Int2 { get; set; }
            public int Int3 { get; set; }
            public int? Int4 { get; set; }

            public Bar Child { get; set; }

            public Bar[] Array { get; set; }
            public List<Bar> List { get; set; }
            public Dictionary<string, string> ScalarDictionary { get; set; }
            public Dictionary<Bar, string> ObjectDictionary { get; set; }

            public string[] StringArray { get; set; }
            public List<string> StringList { get; set; }
            public List<int> IntList { get; set; }
            public int[] IntArray { get; set; }
            public List<int?> NullableIntList { get; set; }
            public int?[] NullableIntArray { get; set; }
        }

        [Test]
        public void Test_Inject()
        {
            var foo = new Foo()
            {
                String = "test",
                Enum1 = Foo.FooEnum.Value1,
                Enum2 = Bar.BarEnum.Value2,
                Enum3 = Foo.FooEnum.Value4,
                Enum4 = Foo.FooEnum.Value4,
                Enum5 = null,
                Enum6 = Foo.FooEnum.Value1,

                EnumE2S = Foo.FooEnum.Value1,
                EnumEq2S = Foo.FooEnum.Value2,
                EnumEqnull2S = null,
                EnumS2E = "Value3",
                EnumSq2E = "Value2",
                EnumSq2Enull = null,

                Int1 = 1,
                Int2 = 2,
                Int3 = null,
                Int4 = null,
                Child = new Foo()
                {
                    String = "test",
                    Enum1 = Foo.FooEnum.Value1,
                    Enum2 = Bar.BarEnum.Value2,
                    Enum3 = Foo.FooEnum.Value4,
                    Enum4 = Foo.FooEnum.Value4,
                    Enum5 = null,
                    Enum6 = Foo.FooEnum.Value1,
                    Int1 = 1,
                    Int2 = 2,
                    Int3 = null,
                    Int4 = null,
                    IntList = new List<int>() { 5, 6 }
                },
                Array = new Foo[] { new Foo { Int1 = 5 }, new Foo { Int1 = 8, Enum1 = Foo.FooEnum.Value1 } },
                List = new List<Foo>() { new Foo { Int1 = 99 }, new Foo { Int1 = 999, Enum3 = Foo.FooEnum.Value3 } },
                ScalarDictionary = new Dictionary<string, string>() { { "1", "12" }, { "2", "22" } },
                ObjectDictionary = new Dictionary<Foo, string>() { { new Foo { Int1 = 5 }, "a" } },

                StringList = new List<string>() { "A", "B", "C" },
                StringArray = new string[] { "D", "E", "F" },
                IntList = new List<int>() { 1, 2, 3 },
                IntArray = new int[] { 4, 5, 6 },
                NullableIntList = new List<int?>() { 1, null, 3 },
                NullableIntArray = new int?[] { 4, null, 6 },
            };

            Bar bar = new Bar();
            bar.InjectFrom<PropertyNameInjection>(foo);

            Assert.AreEqual(Bar.BarEnum.Value1, bar.Enum1);
            Assert.AreEqual(Bar.BarEnum.Value2, bar.Enum2);
            Assert.AreEqual(Bar.BarEnum.Unknown, bar.Enum3);
            Assert.IsNull(bar.Enum4);
            Assert.AreEqual(Bar.BarEnum.Unknown, bar.Enum5);
            Assert.AreEqual(Bar.BarEnum.Value1, bar.Enum6);

            Assert.AreEqual("Value1", bar.EnumE2S);
            Assert.AreEqual("Value2", bar.EnumEq2S);
            Assert.IsNull(bar.EnumEqnull2S);
            Assert.AreEqual(Bar.BarEnum.Value3, bar.EnumS2E);
            Assert.AreEqual(Bar.BarEnum.Value2, bar.EnumSq2E);
            Assert.IsNull(bar.EnumSq2Enull);

            Assert.AreEqual(1, bar.Int1);
            Assert.AreEqual(2, bar.Int2);
            Assert.AreEqual(0, bar.Int3);
            Assert.AreEqual(null, bar.Int4);

            Assert.AreEqual("test", bar.Child.String);

            Assert.AreEqual(Bar.BarEnum.Value1, bar.Child.Enum1);
            Assert.AreEqual(Bar.BarEnum.Value2, bar.Child.Enum2);
            Assert.AreEqual(Bar.BarEnum.Unknown, bar.Child.Enum3);
            Assert.IsNull(bar.Enum4);
            Assert.AreEqual(Bar.BarEnum.Unknown, bar.Child.Enum5);
            Assert.AreEqual(Bar.BarEnum.Value1, bar.Child.Enum6);

            Assert.AreEqual(1, bar.Child.Int1);
            Assert.AreEqual(2, bar.Child.Int2);
            Assert.AreEqual(0, bar.Child.Int3);
            Assert.AreEqual(null, bar.Child.Int4);

            Assert.AreEqual(2, bar.Array.Length);
            Assert.AreEqual(5, bar.Array[0].Int1);
            Assert.AreEqual(8, bar.Array[1].Int1);
            Assert.AreEqual(Bar.BarEnum.Value1, bar.Array[1].Enum1);

            Assert.AreEqual(2, bar.List.Count);
            Assert.AreEqual(99, bar.List[0].Int1);
            Assert.AreEqual(999, bar.List[1].Int1);
            Assert.AreEqual(Bar.BarEnum.Value3, bar.List[1].Enum3);

            Assert.AreEqual(2, bar.ScalarDictionary.Keys.Count);
            Assert.IsTrue(bar.ScalarDictionary.ContainsKey("2"));
            // dictionary is only reference copy at this point
            Assert.AreSame(foo.ScalarDictionary, bar.ScalarDictionary);

            Assert.IsNull(bar.ObjectDictionary);

            CollectionAssert.AreEqual(foo.StringList, bar.StringList);
            Assert.AreNotSame(foo.StringList, bar.StringList);
            CollectionAssert.AreEqual(foo.IntList, bar.IntList);
            Assert.AreNotSame(foo.IntList, bar.IntList);
            CollectionAssert.AreEqual(foo.NullableIntList, bar.NullableIntList);
            Assert.AreNotSame(foo.NullableIntList, bar.NullableIntList);

            CollectionAssert.AreEqual(foo.Child.IntList, bar.Child.IntList);
            Assert.AreNotSame(foo.StringList, bar.IntList);

            CollectionAssert.AreEqual(foo.StringArray, bar.StringArray);
            Assert.AreNotSame(foo.StringArray, bar.StringArray);
            CollectionAssert.AreEqual(foo.IntArray, bar.IntArray);
            Assert.AreNotSame(foo.IntArray, bar.IntArray);
            CollectionAssert.AreEqual(foo.NullableIntArray, bar.NullableIntArray);
            Assert.AreNotSame(foo.NullableIntArray, bar.NullableIntArray);
        }
    }
}
