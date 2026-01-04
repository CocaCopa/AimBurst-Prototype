using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CocaCopa.Core.Dates.Group {
    public static class DateGrouping {
        /// <summary>
        /// Groups the elements in the sequence based on a DateTime extracted from each item,
        /// using the period and calendar rules defined in the provided <see cref="GroupOptions"/>.
        /// </summary>
        /// <typeparam name="T">The element type of the source sequence.</typeparam>
        /// <param name="source">The sequence of items to group.</param>
        /// <param name="dateSelector">A function that extracts a <see cref="DateTime"/> from each element.</param>
        /// <param name="options">Grouping configuration, including period and calendar rules.</param>
        /// <returns>
        /// A dictionary with the following structure:
        /// <list type="bullet">
        ///   <item>
        ///     <term>Key</term>
        ///     <description>The generated grouping string.</description>
        ///   </item>
        ///   <item>
        ///     <term>Value</term>
        ///     <description>The list of items that fall within the corresponding period.</description>
        ///   </item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="source"/>, <paramref name="dateSelector"/>, or <paramref name="options"/> is null.
        /// </exception>
        public static IDictionary<string, List<T>> GroupBy<T>(this IEnumerable<T> source, Func<T, DateTime> dateSelector, GroupOptions options) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (dateSelector == null) throw new ArgumentNullException(nameof(dateSelector));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var calendar = options.Calendar ?? CultureInfo.CurrentCulture.Calendar;
            string format = options.Format;

            return source.GroupBy(item => {
                DateTime dt = dateSelector(item);
                return GetPeriodKey(dt, options.Period, calendar, options.WeekRule, options.FirstDayOfWeek, format);
            }).ToDictionary(g => g.Key, g => g.ToList());
        }

        /// <summary>
        /// Generates a period-based grouping key from the specified <see cref="DateTime"/>,
        /// using the provided calendar configuration to determine year, month, week, or day boundaries.
        /// </summary>
        /// <param name="dt">The date value used to compute the key.</param>
        /// <param name="period">The period granularity (Year, Month, Week, or Day).</param>
        /// <param name="calendar">The calendar used for week calculations.</param>
        /// <param name="weekRule">The rule that defines how weeks are numbered.</param>
        /// <param name="firstDayOfWeek">The first day of the week used for week calculations.</param>
        /// <param name="format">Optional custom format string for the output key.</param>
        /// <returns>A formatted string representing the selected period (e.g., "2025", "2025-03", "2025-W12").</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="period"/> is not a valid <see cref="GroupByPeriod"/> value.
        /// </exception>
        private static string GetPeriodKey(DateTime dt, GroupByPeriod period, Calendar calendar, CalendarWeekRule weekRule, DayOfWeek firstDayOfWeek, string format) {
            int year = dt.Year;
            int month = dt.Month;
            int day = dt.Day;
            int week = calendar.GetWeekOfYear(dt, weekRule, firstDayOfWeek);

            // If a custom format is provided, use it.
            if (!string.IsNullOrWhiteSpace(format)) {
                return format
                    .Replace("{year}", year.ToString("D4", CultureInfo.InvariantCulture))
                    .Replace("{month}", month.ToString("D2", CultureInfo.InvariantCulture))
                    .Replace("{day}", day.ToString("D2", CultureInfo.InvariantCulture))
                    .Replace("{week}", week.ToString("D2", CultureInfo.InvariantCulture));
            }

            // Default formats (fallback when Format is null/empty)
            return period switch {
                GroupByPeriod.Year => year.ToString(CultureInfo.InvariantCulture),
                GroupByPeriod.Month => $"{year:D4}-{month:D2}",
                GroupByPeriod.Week => $"{year:D4}-W{week:D2}",
                GroupByPeriod.Day => $"{year:D4}-{month:D2}-{day:D2}",
                _ => throw new ArgumentOutOfRangeException(nameof(period), period, null),
            };
        }
    }
}
