using LD.Common.Utils;
using NUnit.Framework;
using System;

namespace Tests.LD.Common.Utils
{
    [TestFixture]
    public class TimeUtilTest
    {
        [Test]
        public void Test_YearDiff()
        {
            Assert.AreEqual(1.0d, (double)TimeUtil.YearDiff(new DateTime(2014, 05, 05), new DateTime(2013, 05, 05)), 0.01d);

            Assert.AreEqual(3.5d, (double)TimeUtil.YearDiff(new DateTime(2014, 08, 20), new DateTime(2011, 02, 22)), 0.01d);
        }

        [Test]
        public void Test_MostRecent()
        {
            DateTime dt1 = new DateTime(2014, 11, 20);
            DateTime dt2 = new DateTime(2014, 11, 21);

            Assert.AreEqual(dt2, TimeUtil.MostRecent(dt1, dt2));

            Assert.AreEqual(dt2, TimeUtil.MostRecent(dt2, dt1));

            Assert.AreEqual(dt1, TimeUtil.MostRecent(dt1, dt1));
        }

        [Test]
        public void Test_BeginningOfDay()
        {
            DateTime dt1 = DateTime.UtcNow;
            DateTime beg = TimeUtil.BeginningOfDay(dt1);
            Assert.AreEqual(dt1.Day, beg.Day);
            Assert.AreEqual(dt1.Month, beg.Month);
            Assert.AreEqual(dt1.Year, beg.Year);
            Assert.AreEqual(0, beg.Hour);
            Assert.AreEqual(0, beg.Minute);
            Assert.AreEqual(0, beg.Second);
            Assert.AreEqual(0, beg.Millisecond);
        }

        [Test]
        public void Test_EndOfDay()
        {
            DateTime dt1 = DateTime.UtcNow;
            DateTime end = TimeUtil.EndOfDay(dt1);
            Assert.AreEqual(dt1.Day, end.Day);
            Assert.AreEqual(dt1.Month, end.Month);
            Assert.AreEqual(dt1.Year, end.Year);
            Assert.AreEqual(23, end.Hour);
            Assert.AreEqual(59, end.Minute);
            Assert.AreEqual(59, end.Second);
            Assert.AreEqual(999, end.Millisecond);
        }

        [Test]
        public void Test_BeginningOfDay_AtBeginningOfDay()
        {
            DateTime dt1 = DateTime.UtcNow;
            DateTime dt2 = new DateTime(dt1.Year, dt1.Month, dt1.Day, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime beg = TimeUtil.BeginningOfDay(dt2);
            Assert.AreEqual(dt2.Day, beg.Day);
            Assert.AreEqual(dt2.Month, beg.Month);
            Assert.AreEqual(dt2.Year, beg.Year);
            Assert.AreEqual(0, beg.Hour);
            Assert.AreEqual(0, beg.Minute);
            Assert.AreEqual(0, beg.Second);
            Assert.AreEqual(0, beg.Millisecond);
        }

        [Test]
        public void Test_EndOfDay_AtEndOfDay()
        {
            DateTime dt1 = DateTime.UtcNow;
            DateTime dt2 = new DateTime(dt1.Year, dt1.Month, dt1.Day, 23, 59, 59, 999, DateTimeKind.Utc);
            DateTime end = TimeUtil.EndOfDay(dt2);

            Assert.AreEqual(dt2.Day, end.Day);
            Assert.AreEqual(dt2.Month, end.Month);
            Assert.AreEqual(dt2.Year, end.Year);
            Assert.AreEqual(23, end.Hour);
            Assert.AreEqual(59, end.Minute);
            Assert.AreEqual(59, end.Second);
            Assert.AreEqual(999, end.Millisecond);
        }

        [Test]
        public void Test_DaysOfYear()
        {
            Assert.AreEqual(365, TimeUtil.DaysInYear(2015));
            Assert.AreEqual(366, TimeUtil.DaysInYear(2016));
            Assert.AreEqual(366, TimeUtil.DaysInYear(2000));
            Assert.AreEqual(365, TimeUtil.DaysInYear(1900));
        }

        [Test]
        public void Test_ConvertToUtcFromLoanDepot()
        {
            DateTime t = new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified);
            DateTime c = TimeUtil.ToUtc(t, TimeUtil.Zones.LoanDepot);

            Assert.AreEqual(new DateTime(2015, 6, 8, 17, 34, 12, 0, DateTimeKind.Utc), c);

            // DST
            t = new DateTime(2015, 3, 8, 3, 0, 0, 0, DateTimeKind.Unspecified);
            c = TimeUtil.ToUtc(t, TimeUtil.Zones.LoanDepot);

            Assert.AreEqual(new DateTime(2015, 3, 8, 10, 0, 0, 0, DateTimeKind.Utc), c);

            DateTime? n = null;
            Assert.IsNull(TimeUtil.ToUtc(n, TimeUtil.Zones.LoanDepot));
            n = new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified);
            Assert.AreEqual(new DateTime(2015, 6, 8, 17, 34, 12, 0, DateTimeKind.Utc), TimeUtil.ToUtc(n, TimeUtil.Zones.LoanDepot));
        }

        [Test]
        public void Test_ConvertToUtcFromPacific()
        {
            DateTime t = new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified);
            DateTime c = TimeUtil.ToUtc(t, TimeUtil.Zones.Pacific);

            Assert.AreEqual(new DateTime(2015, 6, 8, 17, 34, 12, 0, DateTimeKind.Utc), c);

            // DST
            t = new DateTime(2015, 3, 8, 3, 0, 0, 0, DateTimeKind.Unspecified);
            c = TimeUtil.ToUtc(t, TimeUtil.Zones.Pacific);

            Assert.AreEqual(new DateTime(2015, 3, 8, 10, 0, 0, 0, DateTimeKind.Utc), c);

            DateTime? n = null;
            Assert.IsNull(TimeUtil.ToUtc(n, TimeUtil.Zones.Pacific));
            n = new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified);
            Assert.AreEqual(new DateTime(2015, 6, 8, 17, 34, 12, 0, DateTimeKind.Utc), TimeUtil.ToUtc(n, TimeUtil.Zones.Pacific));
        }

        [Test]
        public void Test_ConvertToUtcFromCentral()
        {
            DateTime t = new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified);
            DateTime c = TimeUtil.ToUtc(t, TimeUtil.Zones.Central);

            Assert.AreEqual(new DateTime(2015, 6, 8, 15, 34, 12, 0, DateTimeKind.Utc), c);

            // DST
            t = new DateTime(2015, 3, 8, 3, 0, 0, 0, DateTimeKind.Unspecified);
            c = TimeUtil.ToUtc(t, TimeUtil.Zones.Central);

            Assert.AreEqual(new DateTime(2015, 3, 8, 8, 0, 0, 0, DateTimeKind.Utc), c);

            DateTime? n = null;
            Assert.IsNull(TimeUtil.ToUtc(n, TimeUtil.Zones.Central));
            n = new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified);
            Assert.AreEqual(new DateTime(2015, 6, 8, 15, 34, 12, 0, DateTimeKind.Utc), TimeUtil.ToUtc(n, TimeUtil.Zones.Central));
        }

        [Test]
        public void Test_ConvertToUtcFromEastern()
        {
            DateTime t = new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified);
            DateTime c = TimeUtil.ToUtc(t, TimeUtil.Zones.Eastern);

            Assert.AreEqual(new DateTime(2015, 6, 8, 14, 34, 12, 0, DateTimeKind.Utc), c);

            // DST
            t = new DateTime(2015, 3, 8, 3, 0, 0, 0, DateTimeKind.Unspecified);
            c = TimeUtil.ToUtc(t, TimeUtil.Zones.Eastern);

            Assert.AreEqual(new DateTime(2015, 3, 8, 7, 0, 0, 0, DateTimeKind.Utc), c);

            DateTime? n = null;
            Assert.IsNull(TimeUtil.ToUtc(n, TimeUtil.Zones.Eastern));
            n = new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified);
            Assert.AreEqual(new DateTime(2015, 6, 8, 14, 34, 12, 0, DateTimeKind.Utc), TimeUtil.ToUtc(n, TimeUtil.Zones.Eastern));
        }

        [Test]
        public void Test_ConvertLoanDepot()
        {
            DateTime t = new DateTime(2015, 6, 8, 17, 34, 12, 0, DateTimeKind.Utc);
            DateTime c = TimeUtil.FromUtc(t, TimeUtil.Zones.LoanDepot);

            Assert.AreEqual(new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified), c);

            // DST
            t = new DateTime(2015, 3, 8, 10, 0, 0, 0, DateTimeKind.Utc);
            c = TimeUtil.FromUtc(t, TimeUtil.Zones.LoanDepot);

            Assert.AreEqual(new DateTime(2015, 3, 8, 3, 0, 0, 0, DateTimeKind.Unspecified), c);

            DateTime? n = null;
            Assert.IsNull(TimeUtil.FromUtc(n, TimeUtil.Zones.LoanDepot));
            n = new DateTime(2015, 6, 8, 17, 34, 12, 0, DateTimeKind.Utc);
            Assert.AreEqual(new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified), TimeUtil.FromUtc(n, TimeUtil.Zones.LoanDepot));
        }

        [Test]
        public void Test_ConvertPacific()
        {
            DateTime t = new DateTime(2015, 6, 8, 17, 34, 12, 0, DateTimeKind.Utc);
            DateTime c = TimeUtil.FromUtc(t, TimeUtil.Zones.Pacific);

            Assert.AreEqual(new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified), c);

            // DST
            t = new DateTime(2015, 3, 8, 10, 0, 0, 0, DateTimeKind.Utc);
            c = TimeUtil.FromUtc(t, TimeUtil.Zones.Pacific);

            Assert.AreEqual(new DateTime(2015, 3, 8, 3, 0, 0, 0, DateTimeKind.Unspecified), c);

            DateTime? n = null;
            Assert.IsNull(TimeUtil.FromUtc(n, TimeUtil.Zones.Pacific));
            n = new DateTime(2015, 6, 8, 17, 34, 12, 0, DateTimeKind.Utc);
            Assert.AreEqual(new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified), TimeUtil.FromUtc(n, TimeUtil.Zones.Pacific));
        }

        [Test]
        public void Test_ConvertCentral()
        {
            DateTime t = new DateTime(2015, 6, 8, 15, 34, 12, 0, DateTimeKind.Utc);
            DateTime c = TimeUtil.FromUtc(t, TimeUtil.Zones.Central);

            Assert.AreEqual(new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified), c);

            // DST
            t = new DateTime(2015, 3, 8, 8, 0, 0, 0, DateTimeKind.Utc);
            c = TimeUtil.FromUtc(t, TimeUtil.Zones.Central);

            Assert.AreEqual(new DateTime(2015, 3, 8, 3, 0, 0, 0, DateTimeKind.Unspecified), c);

            DateTime? n = null;
            Assert.IsNull(TimeUtil.FromUtc(n, TimeUtil.Zones.Central));
            n = new DateTime(2015, 6, 8, 15, 34, 12, 0, DateTimeKind.Utc);
            Assert.AreEqual(new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified), TimeUtil.FromUtc(n, TimeUtil.Zones.Central));
        }

        [Test]
        public void Test_ConvertToEasternFromUtc()
        {
            DateTime t = new DateTime(2015, 6, 8, 14, 34, 12, 0, DateTimeKind.Utc);
            DateTime c = TimeUtil.FromUtc(t, TimeUtil.Zones.Eastern);

            Assert.AreEqual(new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified), c);

            // DST
            t = new DateTime(2015, 3, 8, 7, 0, 0, 0, DateTimeKind.Utc);
            c = TimeUtil.FromUtc(t, TimeUtil.Zones.Eastern);

            Assert.AreEqual(new DateTime(2015, 3, 8, 3, 0, 0, 0, DateTimeKind.Unspecified), c);

            DateTime? n = null;
            Assert.IsNull(TimeUtil.FromUtc(n, TimeUtil.Zones.Eastern));
            n = new DateTime(2015, 6, 8, 14, 34, 12, 0, DateTimeKind.Utc);
            Assert.AreEqual(new DateTime(2015, 6, 8, 10, 34, 12, 0, DateTimeKind.Unspecified), TimeUtil.FromUtc(n, TimeUtil.Zones.Eastern));
        }

        [Test]
        public void Test_LoanDepotUtcOffsetMinutes()
        {
            Assert.AreEqual(-1 * 8 * 60, TimeUtil.LoanDepotUtcOffsetMinutes);
        }

        [Test]
        public void Test_IsLoanDepotTimeInDST()
        {
            Assert.IsFalse(TimeUtil.IsLoanDepotTimeInDST(new DateTime(2016, 1, 10)));
            Assert.IsTrue(TimeUtil.IsLoanDepotTimeInDST(new DateTime(2016, 5, 10)));
            Assert.IsFalse(TimeUtil.IsLoanDepotTimeInDST(new DateTime(2016, 11, 10)));
        }
    }
}