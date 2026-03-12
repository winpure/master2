namespace WinPure.Configuration.Helper;

internal static class DateHelper
{
    internal static DateTime TrimSeconds(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, 0);
    }

    /// <summary>
    /// Converts a DateTime to local time, safely assuming it is UTC if Kind is Unspecified.
    /// </summary>
    internal static DateTime ToLocalTimeSafe(this DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Local)
            return dateTime;

        if (dateTime.Kind == DateTimeKind.Unspecified)
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

        return dateTime.ToLocalTime();
    }
}