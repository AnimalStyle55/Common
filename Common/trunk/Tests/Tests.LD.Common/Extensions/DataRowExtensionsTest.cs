using LD.Common.Extensions;
using NUnit.Framework;
using System.Data;

namespace Tests.LD.Common.Extensions
{
    [TestFixture]
    internal class DataRowExtensionsTest
    {
        public DataTable DataTable
        {
            get
            {
                var dt = new DataTable();
                dt.Columns.Add(new DataColumn("int", typeof(int)));
                dt.Columns.Add(new DataColumn("string", typeof(string)));
                dt.Columns.Add(new DataColumn("bool", typeof(bool)));
                dt.Columns.Add(new DataColumn("decimal", typeof(decimal)));
                dt.Columns.Add(new DataColumn("null", typeof(string)));

                var row = dt.NewRow();
                row["int"] = 1;
                row["string"] = "str";
                row["bool"] = true;
                row["decimal"] = 5m;
                row["null"] = null;
                dt.Rows.Add(row);

                return dt;
            }
        }

        [Test]
        public void Test_DataRowExtensions_AsString()
        {
            var dt = DataTable;
            var row = dt.Rows[0];

            // Get row values
            Assert.AreEqual("1", row.AsString("int"));
            Assert.AreEqual("str", row.AsString("string"));
            Assert.AreEqual("True", row.AsString("bool"));
            Assert.AreEqual("5", row.AsString("decimal"));

            // Nonexistent column
            Assert.AreEqual(string.Empty, row.AsString("asdf"));
            Assert.AreEqual("default", row.AsString("asdf", "default"));

            // Null value
            Assert.AreEqual(string.Empty, row.AsString("null"));
            Assert.AreEqual("default", row.AsString("null", "default"));
        }

        [Test]
        public void Test_DataRowExtensions_AsBool()
        {
            var dt = DataTable;
            var row = dt.Rows[0];

            // Get row values
            Assert.AreEqual(false, row.AsBool("int"));
            Assert.AreEqual(false, row.AsBool("string"));
            Assert.AreEqual(true, row.AsBool("bool"));
            Assert.AreEqual(false, row.AsBool("decimal"));

            Assert.AreEqual(true, row.AsBool("decimal", true));

            // Nonexistent column
            Assert.AreEqual(false, row.AsBool("asdf"));
            Assert.AreEqual(true, row.AsBool("asdf", true));

            // Null value
            Assert.AreEqual(false, row.AsBool("null"));
            Assert.AreEqual(true, row.AsBool("null", true));


            // Get row values
            Assert.AreEqual(null, row.AsBoolNull("int"));
            Assert.AreEqual(null, row.AsBoolNull("string"));
            Assert.AreEqual(true, row.AsBoolNull("bool"));
            Assert.AreEqual(null, row.AsBoolNull("decimal"));

            // Nonexistent column
            Assert.AreEqual(null, row.AsBoolNull("asdf"));

            // Null value
            Assert.AreEqual(null, row.AsBoolNull("null"));
        }

        [Test]
        public void Test_DataRowExtensions_AsDecimal()
        {
            var dt = DataTable;
            var row = dt.Rows[0];

            // Get row values
            Assert.AreEqual(1m, row.AsDecimal("int"));
            Assert.AreEqual(0m, row.AsDecimal("string"));
            Assert.AreEqual(0m, row.AsDecimal("bool"));
            Assert.AreEqual(5m, row.AsDecimal("decimal"));

            Assert.AreEqual(5m, row.AsDecimal("decimal", 2m));
            Assert.AreEqual(2m, row.AsDecimal("bool", 2m));
         
            // Nonexistent column
            Assert.AreEqual(0m, row.AsDecimal("asdf"));
            Assert.AreEqual(2m, row.AsDecimal("asdf", 2m));

            // Null value
            Assert.AreEqual(0m, row.AsDecimal("null"));
            Assert.AreEqual(1m, row.AsDecimal("null", 1m));


            // Get row values
            Assert.AreEqual(1m, row.AsDecimalNull("int"));
            Assert.AreEqual(null, row.AsDecimalNull("string"));
            Assert.AreEqual(null, row.AsDecimalNull("bool"));
            Assert.AreEqual(5m, row.AsDecimalNull("decimal"));

            // Nonexistent column
            Assert.AreEqual(null, row.AsDecimalNull("asdf"));

            // Null value
            Assert.AreEqual(null, row.AsDecimalNull("null"));
        }

        [Test]
        public void Test_DataRowExtensions_AsInt()
        {
            var dt = DataTable;
            var row = dt.Rows[0];

            // Get row values
            Assert.AreEqual(1, row.AsInt("int"));
            Assert.AreEqual(0, row.AsInt("string"));
            Assert.AreEqual(0, row.AsInt("bool"));
            Assert.AreEqual(5, row.AsInt("decimal"));

            Assert.AreEqual(1, row.AsInt("int", 2));
            Assert.AreEqual(2, row.AsInt("bool", 2));

            // Nonexistent column
            Assert.AreEqual(0, row.AsInt("asdf"));
            Assert.AreEqual(2, row.AsInt("asdf", 2));

            // Null value
            Assert.AreEqual(0, row.AsInt("null"));
            Assert.AreEqual(1, row.AsInt("null", 1));


            // Get row values
            Assert.AreEqual(1, row.AsIntNull("int"));
            Assert.AreEqual(null, row.AsIntNull("string"));
            Assert.AreEqual(null, row.AsIntNull("bool"));
            Assert.AreEqual(5, row.AsIntNull("decimal"));

            // Nonexistent column
            Assert.AreEqual(null, row.AsIntNull("asdf"));

            // Null value
            Assert.AreEqual(null, row.AsIntNull("null"));


            // Get row values
            Assert.AreEqual(1, row.AsIntNullZero("int"));
            Assert.AreEqual(null, row.AsIntNullZero("string"));
            Assert.AreEqual(null, row.AsIntNullZero("bool"));
            Assert.AreEqual(5, row.AsIntNullZero("decimal"));

            // Nonexistent column
            Assert.AreEqual(null, row.AsIntNullZero("asdf"));

            // Null value
            Assert.AreEqual(null, row.AsIntNullZero("null"));

            row["int"] = 0;
            Assert.AreEqual(null, row.AsIntNullZero("int"));
        }
    }
}
