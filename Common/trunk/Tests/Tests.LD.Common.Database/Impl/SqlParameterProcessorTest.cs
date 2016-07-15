using LD.Common.Database;
using LD.Common.Database.Impl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Tests.LD.Common.Database.Impl
{
    [TestFixture]
    public class SqlParameterProcessorTest
    {
        [Test]
        public void Test_null()
        {
            string s = null;
            var p = new SqlParameter("@Null", s);
            SqlParameterProcessor.ProcessParam(p);
            Assert.IsNull(p.Value);

            p = new SqlParameter("@Null", DBNull.Value);
            SqlParameterProcessor.ProcessParam(p);
            Assert.AreEqual(DBNull.Value, p.Value);
        }

        [Test]
        public void Test_DateTime()
        {
            var dt = new DateTime(2015, 02, 15, 14, 05, 01, DateTimeKind.Local);
            var p = new SqlParameter("@DateTime", dt);
            SqlParameterProcessor.ProcessParam(p);

            Assert.AreEqual(dt.ToUniversalTime(), (DateTime)p.Value);
        }

        private enum TestEnum
        {
            Value1,
            Value2
        }

        [Test]
        public void Test_Enum()
        {
            var p = new SqlParameter("@Enum1", TestEnum.Value1);
            SqlParameterProcessor.ProcessParam(p);

            Assert.AreEqual("Value1", p.Value);

            p = new SqlParameter("@Enum2", TestEnum.Value2.ToString());
            SqlParameterProcessor.ProcessParam(p);

            Assert.AreEqual("Value2", p.Value);
        }

        [Test]
        public void Test_Trim()
        {
            var p = new SqlParameter("@String1", "Test");
            SqlParameterProcessor.ProcessParam(p);
            Assert.AreEqual("Test", p.Value);

            p = new SqlParameter("@String1", "Test   ");
            SqlParameterProcessor.ProcessParam(p);
            Assert.AreEqual("Test", p.Value);

            p = new SqlParameter("@String1", "   Test");
            SqlParameterProcessor.ProcessParam(p);
            Assert.AreEqual("Test", p.Value);

            p = new SqlParameter("@String1", " Test ");
            SqlParameterProcessor.ProcessParam(p);
            Assert.AreEqual("Test", p.Value);

            p = new SqlParameter("@String1", "\tTest \t");
            SqlParameterProcessor.ProcessParam(p);
            Assert.AreEqual("Test", p.Value);
        }

        [Test]
        public void Test_NoTrim()
        {
            var p = new SqlParameter(SqlConstants.DoNotTrimHeader + "@String1", "Test");
            SqlParameterProcessor.ProcessParam(p);
            Assert.AreEqual("@String1", p.ParameterName);
            Assert.AreEqual("Test", p.Value);

            p = new SqlParameter(SqlConstants.DoNotTrimHeader + "@String1", "  Test  ");
            SqlParameterProcessor.ProcessParam(p);
            Assert.AreEqual("@String1", p.ParameterName);
            Assert.AreEqual("  Test  ", p.Value);

            p = new SqlParameter(SqlConstants.DoNotTrimHeader + "@String1", null);
            SqlParameterProcessor.ProcessParam(p);
            Assert.AreEqual("@String1", p.ParameterName);
            Assert.IsNull(p.Value);
        }

        [Test]
        public void Test_ForceNull()
        {
            var p = new SqlParameter(SqlConstants.NullableHeader + "@String1", "Test");
            SqlParameterProcessor.ProcessParam(p);
            Assert.AreEqual("@String1", p.ParameterName);
            Assert.AreEqual("Test", p.Value);

            p = new SqlParameter(SqlConstants.NullableHeader + "@String1", null);
            SqlParameterProcessor.ProcessParam(p);
            Assert.AreEqual("@String1", p.ParameterName);
            Assert.AreEqual(DBNull.Value, p.Value);
        }
    }
}