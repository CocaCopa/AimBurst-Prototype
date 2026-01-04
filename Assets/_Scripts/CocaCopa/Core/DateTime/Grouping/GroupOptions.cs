using System;
using System.Globalization;

namespace CocaCopa.Core.Dates.Group {
    public sealed class GroupOptions {
        /// <summary>
        /// Controls which period the items will be grouped by.
        /// <br/>Default: <see cref="GroupByPeriod.Day"/>.
        /// </summary>
        public GroupByPeriod Period { get; set; } = GroupByPeriod.Day;

        /// <summary>
        /// Calendar used for week calculations.
        /// <br/>Default: <see cref="CultureInfo.CurrentCulture"/>'s calendar.
        /// </summary>
        public Calendar Calendar { get; set; } = CultureInfo.CurrentCulture.Calendar;

        /// <summary>
        /// Rule for determining the week of the year.
        /// <br/>Default: <see cref="CalendarWeekRule.FirstFourDayWeek"/>.
        /// </summary>
        public CalendarWeekRule WeekRule { get; set; } = CalendarWeekRule.FirstFourDayWeek;

        /// <summary>
        /// First day of the week used when calculating week numbers.
        /// <br/>Default: <see cref="DayOfWeek.Monday"/>.
        /// </summary>
        public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Monday;

        /// <summary>
        /// Custom format string for the resulting key.
        /// Supported tokens:
        /// <list type="bullet">
        ///   <item><description><c>{year}</c></description></item>
        ///   <item><description><c>{month}</c></description></item>
        ///   <item><description><c>{week}</c></description></item>
        ///   <item><description><c>{day}</c></description></item>
        /// </list>
        /// <br/>Default: empty string (uses the built-in fallback format based on <see cref="Period"/>).
        /// </summary>
        public string Format { get; set; } = string.Empty;
    }
}
