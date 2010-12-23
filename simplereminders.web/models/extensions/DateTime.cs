using System;

namespace simplereminders.web.models.extensions
{
    // TODO : Update to use future friend date time phrases
    public static class DateTimeExtensions
    {
        public const string TODAY_FORMAT = "HH:mm:ss";
        
        public static string YESTERDAY_FORMAT = String.Format("\"{0}\" {1}", "", "HH:mm:ss");
        
        public const string WEEK_FORMAT = "dddd HH:mm:ss";
        public const string STANDARD_FORMAT = "d MMM yyyy HH:mm:ss";

        public static string ToFriendlyDateTime(this DateTime dateTime)
        {
            return ToFriendlyDateTime(dateTime, DateTime.UtcNow);
        }

        public static string ToFriendlyDateTime(this DateTime dateTime, DateTime current)
        {
            string outputString = dateTime.ToString(STANDARD_FORMAT);

            // if dateTime.Day is current day, use TODAY_FORMAT
            if ((current.Year == dateTime.Year) && (current.Month == dateTime.Month) && (current.Day == dateTime.Day))
            {
                outputString = dateTime.ToString(TODAY_FORMAT);
            }
                // if dateTime.Day is yesterday, use YESTERDAY_FORMAT
            else if ((current.AddDays(-1).Year == dateTime.Year) && (current.AddDays(-1).Month == dateTime.Month) &&
                     (current.AddDays(-1).Day == dateTime.Day))
            {
                outputString = dateTime.ToString(YESTERDAY_FORMAT);
            }
                // if dateTime is within last seven days but not the same day as previous week use WEEK_FORMAT
            else if ((current - dateTime) <= TimeSpan.FromDays(7) && current.DayOfWeek != dateTime.DayOfWeek)
            {
                outputString = dateTime.ToString(WEEK_FORMAT);
            }

            // Otherwise use STANDARD_FORMAT
            return outputString;
        }

        public static string ToFriendlyDateTime(this DateTime? dateTime)
        {
            string friendlyDateTime = string.Empty;

            if (dateTime.HasValue)
            {
                friendlyDateTime = dateTime.Value.ToFriendlyDateTime();
            }

            return friendlyDateTime;
        }

        public static bool Between(this DateTime dateTime, DateTime start, DateTime finish)
        {
            return (dateTime >= start && dateTime <= finish);
        }

        public static DateTime UtcToLocal(this DateTime utc, int utcOffset)
        {
            if (utc != DateTime.MinValue && utc != DateTime.MaxValue)
            {
                try
                {
                    return utc.AddMinutes(utcOffset);
                }
                catch
                {
                }
            }

            return utc;
        }

        public static DateTime LocalToUtc(this DateTime local, int utcOffset)
        {
            if (local != DateTime.MinValue && local != DateTime.MaxValue)
            {
                try
                {
                    return local.AddMinutes(0 - utcOffset);
                }
                catch
                {
                }
            }

            return local;
        }

        public static bool HasExcelMinDate(this DateTime dateTime)
        {
            bool result = false;

            if (dateTime.Year == 1899 && dateTime.Month == 12 && dateTime.Day == 30)
            {
                result = true;
            }

            return result;
        }

        public static bool HasMinTime(this DateTime dateTime)
        {
            bool result = false;

            if (dateTime.Hour == 0 && dateTime.Minute == 0 && dateTime.Second == 0)
            {
                result = true;
            }

            return result;
        }
    }
}