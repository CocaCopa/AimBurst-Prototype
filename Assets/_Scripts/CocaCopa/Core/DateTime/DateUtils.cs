using System;

namespace CocaCopa.Core.Dates {
    public static class DateUtils {
        /// <summary>
        /// Returns a DateTime from ticks and sets its Kind to Local without converting the value.
        /// </summary>
        public static DateTime FromLocalTicks(this long ticks) => new DateTime(ticks, DateTimeKind.Local);

        /// <summary>
        /// Returns a DateTime from ticks and sets its Kind to Utc without converting the value.
        /// </summary>
        public static DateTime FromUtcTicks(this long ticks) => new DateTime(ticks, DateTimeKind.Utc);

        /// <summary>
        /// Formats the date portion of this <see cref="DateTime"/> using the pattern "dd{sep}MM{sep}yyyy".
        /// </summary>
        /// <param name="source">The source <see cref="DateTime"/>.</param>
        /// <param name="separator">The character used to separate day, month, and year.</param>
        /// <returns>A formatted date string.</returns>
        public static string FormatDate(this DateTime source, char separator = '/') {
            int day = source.Day;
            int month = source.Month;
            int year = source.Year;
            return $"{day:00}{separator}{month:00}{separator}{year}";
        }

        /// <summary>
        /// Formats the time portion of this <see cref="DateTime"/> using the pattern "HH{sep}mm{sep}ss".
        /// </summary>
        /// <param name="source">The source <see cref="DateTime"/>.</param>
        /// <param name="separator">The character used to separate hours, minutes, and seconds.</param>
        /// <returns>A formatted time string.</returns>
        public static string FormatTime(this DateTime source, char separator = ':') {
            int h = source.Hour;
            int m = source.Minute;
            int s = source.Second;
            return $"{h:00}{separator}{m:00}{separator}{s:00}";
        }
    }
}
