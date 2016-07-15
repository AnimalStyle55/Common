using LD.Common.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.LD.Common.Extensions
{
    [TestFixture]
    internal class NullableExtensionsTest
    {
        [Test]
        public void Test_ToValueOrDefault()
        {
            int? i = null;
            var actual = i.ToValueOrDefault();
            Assert.AreEqual(0, actual);

            actual = i.ToValueOrDefault(5);
            Assert.AreEqual(5, actual);

            i = 3;
            actual = i.ToValueOrDefault();
            Assert.AreEqual(3, actual);


            bool? b = null;
            var actualB = b.ToValueOrDefault();
            Assert.AreEqual(false, actualB);

            actualB = b.ToValueOrDefault(true);
            Assert.AreEqual(true, actualB);

            b = true;
            actualB = b.ToValueOrDefault();
            Assert.AreEqual(true, actualB);
        }

        [Test]
        public void Test_ToStringOrNull()
        {
            int? i = null;
            var actual = i.ToStringOrNull();
            Assert.IsNull(actual);

            i = 5;
            actual = i.ToStringOrNull();
            Assert.AreEqual("5", actual);

            bool? b = null;
            var actualB = b.ToStringOrNull();
            Assert.IsNull(actualB);

            b = true;
            actualB = b.ToStringOrNull();
            Assert.AreEqual("True", actualB);
        }
    }
}
