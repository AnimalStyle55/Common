using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LD.Common.Extensions;

namespace Tests.LD.Common.Extensions
{
    public enum TestEnum
    {
        NoDescValue,

        [System.ComponentModel.Description("This is the description")]
        DescValue
    }

    [TestFixture]
    class EnumExtensionTest
    {
        public enum SimpleEnum1
        {
            EnumVal1 = 0,
            EnumVal2 = 1,
            EnumVal3 = 2,
            EnumVal4 = 3,
            EnumValDefault = 4,
            EnumOnlyIn1 = 5
        }

        public enum SimpleEnum2
        {
            EnumVal1 = 5,
            EnumVal2 = 4,
            EnumVal3 = 3,
            EnumVal4 = 2,
            EnumValDefault = 1,
            EnumOnlyIn2 = 0
        }

        [Test]
        public void Test_GetDesc()
        {
            Assert.AreEqual("NoDescValue", TestEnum.NoDescValue.GetDescription());
            Assert.AreEqual("This is the description", TestEnum.DescValue.GetDescription());
        }

        [Test]
        public void Test_ToEnum()
        {
            SimpleEnum1 e1 = SimpleEnum1.EnumVal1;
            Assert.AreEqual(SimpleEnum2.EnumVal1, e1.ToEnum<SimpleEnum2>());

            e1 = SimpleEnum1.EnumVal4;
            Assert.AreEqual(SimpleEnum2.EnumVal4, e1.ToEnum<SimpleEnum2>());

            SimpleEnum2 e2 = SimpleEnum2.EnumVal2;
            Assert.AreEqual(SimpleEnum1.EnumVal2, e2.ToEnum<SimpleEnum1>());

            e2 = SimpleEnum2.EnumVal3;
            Assert.AreEqual(SimpleEnum1.EnumVal3, e2.ToEnum<SimpleEnum1>());

            Assert.Throws<ArgumentException>(() => SimpleEnum1.EnumOnlyIn1.ToEnum<SimpleEnum2>());
            Assert.Throws<ArgumentException>(() => SimpleEnum2.EnumOnlyIn2.ToEnum<SimpleEnum1>());
        }

        [Test]
        public void Test_ToEnumOrNull()
        {
            SimpleEnum1? e1 = null;
            Assert.IsNull(e1.ToEnumOrNull<SimpleEnum2>());

            e1 = SimpleEnum1.EnumVal4;
            Assert.AreEqual(SimpleEnum2.EnumVal4, e1.ToEnumOrNull<SimpleEnum2>());

            SimpleEnum2? e2 = null;
            Assert.IsNull(e2.ToEnumOrNull<SimpleEnum1>());

            e2 = SimpleEnum2.EnumVal3;
            Assert.AreEqual(SimpleEnum1.EnumVal3, e2.ToEnumOrNull<SimpleEnum1>());

            Assert.IsNull(SimpleEnum1.EnumOnlyIn1.ToEnumOrNull<SimpleEnum2>());
            Assert.IsNull(SimpleEnum2.EnumOnlyIn2.ToEnumOrNull<SimpleEnum1>());
        }

        [Test]
        public void Test_ToEnumOrDefault()
        {
            SimpleEnum1 e1 = SimpleEnum1.EnumVal1;
            Assert.AreEqual(SimpleEnum2.EnumVal1, e1.ToEnumOrDefault<SimpleEnum2>(SimpleEnum2.EnumVal4));

            e1 = SimpleEnum1.EnumVal4;
            Assert.AreEqual(SimpleEnum2.EnumVal4, e1.ToEnumOrDefault<SimpleEnum2>(SimpleEnum2.EnumVal3));

            SimpleEnum2 e2 = SimpleEnum2.EnumVal2;
            Assert.AreEqual(SimpleEnum1.EnumVal2, e2.ToEnumOrDefault<SimpleEnum1>(SimpleEnum1.EnumVal1));

            e2 = SimpleEnum2.EnumVal3;
            Assert.AreEqual(SimpleEnum1.EnumVal3, e2.ToEnumOrDefault<SimpleEnum1>(SimpleEnum1.EnumVal4));

            Assert.AreEqual(SimpleEnum2.EnumOnlyIn2, SimpleEnum1.EnumOnlyIn1.ToEnumOrDefault<SimpleEnum2>(SimpleEnum2.EnumOnlyIn2));
            Assert.AreEqual(SimpleEnum1.EnumOnlyIn1, SimpleEnum2.EnumOnlyIn2.ToEnumOrDefault<SimpleEnum1>(SimpleEnum1.EnumOnlyIn1));

            SimpleEnum1? e1q = SimpleEnum1.EnumVal1;
            Assert.AreEqual(SimpleEnum2.EnumVal1, e1q.ToEnumOrDefault<SimpleEnum2>(SimpleEnum2.EnumVal4));

            e1q = null;
            Assert.AreEqual(SimpleEnum2.EnumVal4, e1q.ToEnumOrDefault<SimpleEnum2>(SimpleEnum2.EnumVal4));

            e1q = SimpleEnum1.EnumVal4;
            Assert.AreEqual(SimpleEnum2.EnumVal4, e1q.ToEnumOrDefault<SimpleEnum2>(SimpleEnum2.EnumVal3));

            SimpleEnum2? e2q = SimpleEnum2.EnumVal2;
            Assert.AreEqual(SimpleEnum1.EnumVal2, e2q.ToEnumOrDefault<SimpleEnum1>(SimpleEnum1.EnumVal1));

            e2q = null;
            Assert.AreEqual(SimpleEnum1.EnumVal1, e2q.ToEnumOrDefault<SimpleEnum1>(SimpleEnum1.EnumVal1));

            e2q = SimpleEnum2.EnumVal3;
            Assert.AreEqual(SimpleEnum1.EnumVal3, e2q.ToEnumOrDefault<SimpleEnum1>(SimpleEnum1.EnumVal4));
        }
    }
}
