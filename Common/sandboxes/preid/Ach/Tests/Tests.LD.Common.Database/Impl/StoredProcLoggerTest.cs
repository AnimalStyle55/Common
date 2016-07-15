using LD.Common.Database.Impl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Tests.LD.Common.Database.Impl
{
    [TestFixture]
    public class StoredProcLoggerTest
    {
        [Test]
        public void Test_ExecToString()
        {
            int? nullInt = null;
            Assert.AreEqual("EXECUTE usp_DoThis @Param1 = 5, @Param2 = 'abc', @Param3 = NULL, @Param4 = NULL",
                StoredProcLogger.ExecToString("usp_DoThis", new[] {new SqlParameter("@Param1", 5),
                                                            new SqlParameter("@Param2", "abc"),
                                                            new SqlParameter("@Param3", nullInt),
                                                            new SqlParameter("@Param4", null)}));
        }

        [Test]
        public void Test_ExecToString_Guid()
        {
            Assert.AreEqual("EXECUTE usp_DoThis @Param1 = 'f75da73e-fb3f-4867-b27c-183e241a0e1e'",
                StoredProcLogger.ExecToString("usp_DoThis", new[] { new SqlParameter("@Param1", new Guid("f75da73e-fb3f-4867-b27c-183e241a0e1e")) }));
        }

        [Test]
        public void Test_RawExecToString()
        {
            int? nullInt = null;
            Assert.AreEqual("UPDATE table SET P1=@Param1, P2=@Param2, P3=@Param3, P4=@Param4 ==> VALUES (@Param1 = 5, @Param2 = 'abc', @Param3 = NULL, @Param4 = NULL)",
                StoredProcLogger.RawExecToString("UPDATE table SET P1=@Param1, P2=@Param2, P3=@Param3, P4=@Param4",
                    new[] {
                        new SqlParameter("@Param1", 5),
                        new SqlParameter("@Param2", "abc"),
                        new SqlParameter("@Param3", nullInt),
                        new SqlParameter("@Param4", null)}));
        }

        [Test]
        public void Test_RawExecToString_NoValue()
        {
            Assert.AreEqual("SELECT * FROM TABLE",
                StoredProcLogger.RawExecToString("SELECT * FROM TABLE", Enumerable.Empty<SqlParameter>()));
        }
    }
}