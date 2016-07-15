using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LD.Common.Utils
{
    /// <summary>
    /// Utilities for performing date and time calculations
    /// </summary>
    public static class TimeUtil
    {
        private static readonly ILog log = LoanDepotLogManager.GetLogger();

        /// <summary>
        /// Supported time zones for 3rd party input/output and for display
        /// </summary>
        public enum Zones
        {
            /// <summary>The LoanDepot time zone is currently Pacific, use when you want a TZ that represents LD corporate</summary>
            LoanDepot,
            /// <summary>EST</summary>
            Eastern,
            /// <summary>CST</summary>
            Central,
            /// <summary>PST</summary>
            Pacific
        }

        private static readonly Dictionary<Zones, TimeZoneInfo> _tzDict = new Dictionary<Zones, TimeZoneInfo>();

        static TimeUtil()
        {
            _tzDict[Zones.Pacific] = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            // "Loan Depot Time" is defined as "America/Los_Angeles", or Pacific Time
            // It is used for interfaces to other services that don't use UTC
            _tzDict[Zones.LoanDepot] = _tzDict[Zones.Pacific];

            _tzDict[Zones.Central] = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            _tzDict[Zones.Eastern] = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }

        /// <summary>
        /// Calculate the difference between two dates in decimal years
        /// </summary>
        /// <param name="newer"></param>
        /// <param name="older"></param>
        /// <returns></returns>
        public static decimal YearDiff(DateTime newer, DateTime older)
        {
            return (decimal)((newer - older).TotalDays / 365.25);
        }

        /// <summary>
        /// Determine which of two dates is the newer date
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static DateTime MostRecent(DateTime date1, DateTime date2)
        {
            return date1 > date2 ? date1 : date2;
        }

        /// <summary>
        /// Round a date time to the nearest second (i.e. remove the milliseconds)
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static DateTime RoundToSecond(DateTime t)
        {
            return new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, t.Kind);
        }

        /// <summary>
        /// Return a datetime at the beginning of the day
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static DateTime BeginningOfDay(DateTime t)
        {
            return t.Date; // Returns: A new object with the same date as this instance, and the time value set to 12:00:00 midnight (00:00:00).
        }

        /// <summary>
        /// Return a datetime at the end of the day
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static DateTime EndOfDay(DateTime t)
        {
            var beginOfTomorrow = BeginningOfDay(t.AddDays(1));

            return beginOfTomorrow.AddSeconds(-1).AddMilliseconds(999);
        }

        /// <summary>
        /// Returns the number of days in the given year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int DaysInYear(int year)
        {
            return (new DateTime(year + 1, 1, 1) - new DateTime(year, 1, 1)).Days;
        }

        /// <summary>
        /// Time the duration of an action and log it
        /// </summary>
        /// <param name="action">code to time</param>
        /// <param name="description">(optional) description of action</param>
        /// <param name="theLog">(optional) log variable to use</param>
        public static void LogExecTime(Action action, string description = "Execution", ILog theLog = null)
        {
            using (new TimeLogger(description, theLog))
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Returns the UTC offset of the Loan Depot time zone in minutes
        /// </summary>
        public static int LoanDepotUtcOffsetMinutes
        {
            get { return (int)_tzDict[Zones.LoanDepot].BaseUtcOffset.TotalMinutes; }
        }

        /// <summary>
        /// Determine if a time in Loan Depot time zone is currently in Daylight Savings Time
        /// </summary>
        /// <param name="time">null to use the current time</param>
        /// <returns>true if time is in DST</returns>
        public static bool IsLoanDepotTimeInDST(DateTime? time = null)
        {
            return _tzDict[Zones.LoanDepot].IsDaylightSavingTime(time.HasValue ? time.Value : DateTime.UtcNow.ToLoanDepotTime());
        }

        /// <summary>
        /// Converts a DateTime in UTC to the LoanDepot time zone
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime ToLoanDepotTime(this DateTime time)
        {
            return FromUtc(time, Zones.LoanDepot);
        }

        /// <summary>
        /// Converts a DateTime? in UTC to the LoanDepot time zone
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime? ToLoanDepotTime(this DateTime? time)
        {
            return time == null ? (DateTime?)null : time.Value.ToLoanDepotTime();
        }

        /// <summary>
        /// Returns the current time in the LoanDepot time zone
        /// </summary>
        public static DateTime LoanDepotTime
        {
            get { return DateTime.UtcNow.ToLoanDepotTime(); }
        }

        /// <summary>
        /// Converts a DateTime from a time zone to UTC
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static DateTime ToUtc(DateTime date, Zones timeZone)
        {
            return TimeZoneInfo.ConvertTimeToUtc(date, _tzDict[timeZone]);
        }

        /// <summary>
        /// Converts a DateTime? from Pacific time zone to UTC
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static DateTime? ToUtc(DateTime? date, Zones timeZone)
        {
            return (date == null) ? (DateTime?)null : ToUtc(date.Value, timeZone);
        }

        /// <summary>
        /// Converts a DateTime from UTC to a time zone
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static DateTime FromUtc(DateTime date, Zones timeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(date, _tzDict[timeZone]);
        }

        /// <summary>
        /// Converts a DateTime from UTC to a time zone
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static DateTime? FromUtc(DateTime? date, Zones timeZone)
        {
            return (date == null) ? (DateTime?)null : FromUtc(date.Value, timeZone);
        }

        /// <summary>
        /// Handy class to log the duration of code inside a using block
        /// </summary>
        /// <example>
        /// using(new TimeLogger("some code", log))
        /// {
        ///     some code
        /// }
        /// will log:  "some code Took: 00.00.00.274472"
        /// </example>
        public sealed class TimeLogger : IDisposable
        {
            private readonly string _desc;
            private readonly ILog _theLog;
            private readonly Stopwatch _sw;

            /// <summary>
            /// Duration of the stop watch (as of time of call)
            /// </summary>
            public TimeSpan Duration
            {
                get
                {
                    return _sw.Elapsed;
                }
            }

            /// <summary>
            /// Construct a TimeLogger
            /// </summary>
            /// <param name="description">logged before and after scope block</param>
            /// <param name="theLog">(optional) log object to use</param>
            public TimeLogger(string description = "Execution", ILog theLog = null)
            {
                _theLog = theLog ?? TimeUtil.log;
                _desc = description;
                _sw = Stopwatch.StartNew();
                _theLog.DebugFormat("Starting {0}", description);
            }

            /// <summary>
            /// Stops the timing and logs the total duration
            /// </summary>
            public void Dispose()
            {
                _sw.Stop();
                _theLog.DebugFormat("{0} Took: {1}", _desc, _sw.Elapsed);
            }
        }
    }
}