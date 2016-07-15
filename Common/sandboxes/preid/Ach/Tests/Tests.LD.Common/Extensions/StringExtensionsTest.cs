using LD.Common.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.LD.Common.Extensions
{
    [TestFixture]
    internal class StringExtensionsTest
    {
        private enum SimpleEnum
        {
            EnumVal1,
            EnumVal2,
            EnumVal3,
            EnumVal4,
            EnumValDefault
        }

        [Flags]
        private enum FlagsEnum
        {
            EnumValNone = 0,
            EnumVal1 = 1,
            EnumVal2 = 2,
            EnumVal3 = 4,
            EnumVal4 = 8,
            EnumVal5 = 16,
        }

        [Test]
        public void Test_ToEnum()
        {
            Assert.AreEqual(SimpleEnum.EnumVal1, "EnumVal1".ToEnum<SimpleEnum>());
            Assert.AreEqual(SimpleEnum.EnumVal2, "EnumVal2".ToEnum<SimpleEnum>());
            Assert.AreEqual(SimpleEnum.EnumVal3, "EnumVal3".ToEnum<SimpleEnum>());
            Assert.AreEqual(SimpleEnum.EnumVal4, "EnumVal4".ToEnum<SimpleEnum>());

            Assert.AreEqual(FlagsEnum.EnumVal2 | FlagsEnum.EnumVal4, "EnumVal2,EnumVal4".ToEnum<FlagsEnum>());
            Assert.AreEqual(FlagsEnum.EnumVal1 | FlagsEnum.EnumVal3, "EnumVal1, EnumVal3".ToEnum<FlagsEnum>());
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_ToEnum_Invalid()
        {
            Assert.AreEqual(SimpleEnum.EnumVal1, "NotAValidValue".ToEnum<SimpleEnum>());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_ToEnum_Null()
        {
            Assert.AreEqual(SimpleEnum.EnumVal1, ((string)null).ToEnum<SimpleEnum>());
        }

        [Test]
        public void Test_ToEnumOrDefault()
        {
            Assert.AreEqual(SimpleEnum.EnumVal1, "EnumVal1".ToEnumOrDefault<SimpleEnum>(SimpleEnum.EnumValDefault));
            Assert.AreEqual(SimpleEnum.EnumVal2, "EnumVal2".ToEnumOrDefault<SimpleEnum>(SimpleEnum.EnumValDefault));
            Assert.AreEqual(SimpleEnum.EnumVal3, "EnumVal3".ToEnumOrDefault<SimpleEnum>(SimpleEnum.EnumValDefault));
            Assert.AreEqual(SimpleEnum.EnumVal4, "EnumVal4".ToEnumOrDefault<SimpleEnum>(SimpleEnum.EnumValDefault));

            Assert.AreEqual(SimpleEnum.EnumValDefault, "".ToEnumOrDefault<SimpleEnum>(SimpleEnum.EnumValDefault));
            Assert.AreEqual(SimpleEnum.EnumValDefault, ((string)null).ToEnumOrDefault<SimpleEnum>(SimpleEnum.EnumValDefault));

            Assert.AreEqual(FlagsEnum.EnumVal2 | FlagsEnum.EnumVal4, "EnumVal2,EnumVal4".ToEnumOrDefault<FlagsEnum>(FlagsEnum.EnumValNone));
            Assert.AreEqual(FlagsEnum.EnumVal1 | FlagsEnum.EnumVal3, "EnumVal1, EnumVal3".ToEnumOrDefault<FlagsEnum>(FlagsEnum.EnumValNone));

            Assert.AreEqual(FlagsEnum.EnumValNone, "".ToEnumOrDefault<FlagsEnum>(FlagsEnum.EnumValNone));
            Assert.AreEqual(FlagsEnum.EnumValNone, ((string)null).ToEnumOrDefault<FlagsEnum>(FlagsEnum.EnumValNone));
        }

        [Test]
        public void Test_ToEnumOrNull()
        {
            Assert.AreEqual(SimpleEnum.EnumVal1, "EnumVal1".ToEnumOrNull<SimpleEnum>());
            Assert.AreEqual(SimpleEnum.EnumVal2, "EnumVal2".ToEnumOrNull<SimpleEnum>());
            Assert.AreEqual(SimpleEnum.EnumVal3, "EnumVal3".ToEnumOrNull<SimpleEnum>());
            Assert.AreEqual(SimpleEnum.EnumVal4, "EnumVal4".ToEnumOrNull<SimpleEnum>());

            Assert.IsNull("".ToEnumOrNull<SimpleEnum>());
            Assert.IsNull(((string)null).ToEnumOrNull<SimpleEnum>());

            Assert.AreEqual(FlagsEnum.EnumVal2 | FlagsEnum.EnumVal4, "EnumVal2,EnumVal4".ToEnumOrNull<FlagsEnum>());
            Assert.AreEqual(FlagsEnum.EnumVal1 | FlagsEnum.EnumVal3, "EnumVal1, EnumVal3".ToEnumOrNull<FlagsEnum>());

            Assert.IsNull("".ToEnumOrNull<FlagsEnum>());
            Assert.IsNull(((string)null).ToEnumOrNull<FlagsEnum>());
        }
    }
}