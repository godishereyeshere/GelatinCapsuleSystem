using System.Globalization;

namespace GelatinCapsule.Application.Common.Helpers;

public static class PersianDateHelper
{
    private static readonly PersianCalendar _pc = new PersianCalendar();

    public static string ToPersianDateString(DateTime dt)
    {
        return $"{_pc.GetYear(dt):0000}{_pc.GetMonth(dt):00}{_pc.GetDayOfMonth(dt):00}";
    }

    public static string GetYear(DateTime dt)
    {
        return _pc.GetYear(dt).ToString();
    }

    public static string GetDayOfWeekName(DateTime dt)
    {
        return dt.DayOfWeek switch
        {
            DayOfWeek.Saturday => "شنبه",
            DayOfWeek.Sunday => "یکشنبه",
            DayOfWeek.Monday => "دوشنبه",
            DayOfWeek.Tuesday => "سه‌شنبه",
            DayOfWeek.Wednesday => "چهارشنبه",
            DayOfWeek.Thursday => "پنج‌شنبه",
            DayOfWeek.Friday => "جمعه",
            _ => ""
        };
    }

    public static string FormatFarsiDate(string farsidate)
    {
        if (string.IsNullOrEmpty(farsidate) || farsidate.Length != 8)
            return farsidate;

        return $"{farsidate.Substring(0, 4)}/{farsidate.Substring(4, 2)}/{farsidate.Substring(6, 2)}";
    }
}