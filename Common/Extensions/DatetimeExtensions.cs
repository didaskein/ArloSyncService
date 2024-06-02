using System;

namespace ArloSyncService.Common.Extensions
{

    public static class DateTimeExtensions
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixTimeMilliseconds(long unixTime)
        {
            return epoch.AddMilliseconds(unixTime);
        }


        public static DateTime FromUnixTimeSeconds(long unixTime)
        {
            return epoch.AddSeconds(unixTime);
        }


        /// <summary>
        /// Convert a date time object to Unix time representation.
        /// </summary>
        /// <param name="datetime">The datetime object to convert to Unix time stamp.</param>
        /// <returns>Returns a numerical representation (Unix time) of the DateTime object.</returns>
        public static long ConvertToUnixTimeMilliseconds(this DateTime datetime)
        {
            return (long)(datetime - epoch).TotalMilliseconds;
        }

        public static long ConvertToUnixTimeSeconds(this DateTime datetime)
        {
            return (long)(datetime - epoch).TotalSeconds;
        }

    }
}
