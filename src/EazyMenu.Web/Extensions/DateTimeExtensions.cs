using System.Globalization;

namespace EazyMenu.Web.Extensions
{
    /// <summary>
    /// متدهای کمکی برای تبدیل تاریخ میلادی به شمسی
    /// </summary>
    public static class DateTimeExtensions
    {
        private static readonly PersianCalendar PersianCalendar = new PersianCalendar();

        /// <summary>
        /// تبدیل DateTime به تاریخ شمسی (فقط تاریخ)
        /// </summary>
        public static string ToPersianDate(this DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
                return "-";

            var year = PersianCalendar.GetYear(dateTime);
            var month = PersianCalendar.GetMonth(dateTime);
            var day = PersianCalendar.GetDayOfMonth(dateTime);

            return $"{year:0000}/{month:00}/{day:00}";
        }

        /// <summary>
        /// تبدیل DateTime? به تاریخ شمسی (فقط تاریخ)
        /// </summary>
        public static string ToPersianDate(this DateTime? dateTime)
        {
            return dateTime?.ToPersianDate() ?? "-";
        }

        /// <summary>
        /// تبدیل DateTime به تاریخ و زمان شمسی
        /// </summary>
        public static string ToPersianDateTime(this DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
                return "-";

            var year = PersianCalendar.GetYear(dateTime);
            var month = PersianCalendar.GetMonth(dateTime);
            var day = PersianCalendar.GetDayOfMonth(dateTime);

            return $"{year:0000}/{month:00}/{day:00} - {dateTime:HH:mm}";
        }

        /// <summary>
        /// تبدیل DateTime? به تاریخ و زمان شمسی
        /// </summary>
        public static string ToPersianDateTime(this DateTime? dateTime)
        {
            return dateTime?.ToPersianDateTime() ?? "-";
        }

        /// <summary>
        /// تبدیل DateTime به تاریخ شمسی با نام روز هفته
        /// </summary>
        public static string ToPersianDateWithDayName(this DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
                return "-";

            var dayOfWeek = dateTime.DayOfWeek;
            var persianDayName = dayOfWeek switch
            {
                DayOfWeek.Saturday => "شنبه",
                DayOfWeek.Sunday => "یکشنبه",
                DayOfWeek.Monday => "دوشنبه",
                DayOfWeek.Tuesday => "سه‌شنبه",
                DayOfWeek.Wednesday => "چهارشنبه",
                DayOfWeek.Thursday => "پنجشنبه",
                DayOfWeek.Friday => "جمعه",
                _ => ""
            };

            return $"{persianDayName} {dateTime.ToPersianDate()}";
        }

        /// <summary>
        /// دریافت نام فارسی ماه
        /// </summary>
        public static string GetPersianMonthName(this DateTime dateTime)
        {
            var month = PersianCalendar.GetMonth(dateTime);
            return month switch
            {
                1 => "فروردین",
                2 => "اردیبهشت",
                3 => "خرداد",
                4 => "تیر",
                5 => "مرداد",
                6 => "شهریور",
                7 => "مهر",
                8 => "آبان",
                9 => "آذر",
                10 => "دی",
                11 => "بهمن",
                12 => "اسفند",
                _ => ""
            };
        }

        /// <summary>
        /// تبدیل به فرمت نسبی (چند دقیقه پیش، چند ساعت پیش، ...)
        /// </summary>
        public static string ToRelativeTime(this DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalSeconds < 60)
                return "چند لحظه پیش";

            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} دقیقه پیش";

            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} ساعت پیش";

            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} روز پیش";

            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} هفته پیش";

            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} ماه پیش";

            return dateTime.ToPersianDate();
        }

        /// <summary>
        /// تبدیل DateTime? به فرمت نسبی
        /// </summary>
        public static string ToRelativeTime(this DateTime? dateTime)
        {
            return dateTime?.ToRelativeTime() ?? "-";
        }
    }
}
