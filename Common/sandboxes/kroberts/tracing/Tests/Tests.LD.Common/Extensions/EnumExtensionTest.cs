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
        [Test]
        public void Test_GetDesc()
        {
            Assert.AreEqual("NoDescValue", TestEnum.NoDescValue.GetDescription());
            Assert.AreEqual("This is the description", TestEnum.DescValue.GetDescription());
        }
    }
}
