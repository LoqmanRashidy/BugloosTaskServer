
using CommonLayer.Extension;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Helper
{
    public static partial class PersianTools
    {
        public static PersianDateTime DateTime(DateTime dateTime)
        {
            return new PersianDateTime(dateTime);
        }


        public static PersianDateTime ToPersianDateTime(this DateTime DT)
        {
            return new PersianDateTime(DT);
        }

        public static PersianDateTime ToPersianDateTime(string text)
        {
            return new PersianDateTime(Convert.ToDateTime(text));
        }

        public static string PersianDateTime(DateTime? dateTime)
        {
            if (dateTime.HasValue)
                return new PersianDateTime(dateTime).ToString();
            else return string.Empty;
        }

        public static DateTime ToDate(this string persianDate)
        {
            int year = Convert.ToInt32(persianDate.Substring(0, 4));
            int month = Convert.ToInt32(persianDate.Substring(5, 2));
            int day = Convert.ToInt32(persianDate.Substring(8, 2));
            DateTime georgianDateTime = new DateTime(year, month, day, new System.Globalization.PersianCalendar());
            return georgianDateTime;
        }

        public static DateTime? ToDateTime(this string persianDateTime, DateTime defaultDate)
        {
            if (!string.IsNullOrWhiteSpace(persianDateTime))
            {
                persianDateTime = persianDateTime.Trim().Replace("   ", " ");
                int year = 0, month = 0, day = 0, hour = 0, minute = 0;
                if (persianDateTime.Length>11)
                {
                     year = Convert.ToInt32(persianDateTime.Substring(0, 4));
                     month = Convert.ToInt32(persianDateTime.Substring(5, 2));
                     day = Convert.ToInt32(persianDateTime.Substring(8, 2));
                     hour = Convert.ToInt32(persianDateTime.Substring(11, 2));
                     minute = Convert.ToInt32(persianDateTime.Substring(14, 2));
                }
                else
                {
                     year = Convert.ToInt32(persianDateTime.Substring(0, 4));
                     month = Convert.ToInt32(persianDateTime.Substring(5, 2));
                     day = Convert.ToInt32(persianDateTime.Substring(8, 2));
                }
              
                DateTime georgianDateTime = new DateTime(year, month, day, hour, minute, 0, new System.Globalization.PersianCalendar());
                return georgianDateTime;
            }
            return defaultDate;
        }

        public static DateTime LocalPersianDateTime(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {

                var info = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
                DateTimeOffset localServerTime = dateTime.Value;
                DateTimeOffset localTime = TimeZoneInfo.ConvertTime(localServerTime, info);
                //string userDateTimeString =
                //    localTime.ToString
                //    ("dd MMM yyyy - HH:mm:ss (zzz)");
                return localTime.DateTime;
            }

            else return new DateTime();
        }

        //public static string PersianDateTime(DateTime? dateTime)
        //{
        //    if (dateTime.HasValue)
        //    {
        //        PersianCalendar pc = new PersianCalendar();
        //        var date = pc.ToDateTime(pc.GetYear(dateTime.Value),
        //           pc.GetMonth(dateTime.Value),
        //             pc.GetDayOfMonth(dateTime.Value),
        //               pc.GetHour(dateTime.Value),
        //                  pc.GetMinute(dateTime.Value),
        //                   pc.GetSecond(dateTime.Value), 0);
        //        return date.ToString();
        //    }

        //    else return string.Empty;
        //}
    }

    public static class ConvertToPersian
    {
        public static API.PersianDaysOfWeek ToPersianDaysOfWeek(this DayOfWeek days)
        {
            switch (days)
            {
                case DayOfWeek.Friday:
                    return API.PersianDaysOfWeek.جمعه;
                case DayOfWeek.Monday:
                    return API.PersianDaysOfWeek.دوشنبه;
                case DayOfWeek.Saturday:
                    return API.PersianDaysOfWeek.شنبه;
                case DayOfWeek.Sunday:
                    return API.PersianDaysOfWeek.یکشنبه;
                case DayOfWeek.Thursday:
                    return API.PersianDaysOfWeek.پنجشنبه;
                case DayOfWeek.Tuesday:
                    return API.PersianDaysOfWeek.سهشنبه;
                case DayOfWeek.Wednesday:
                    return API.PersianDaysOfWeek.چهارشنبه;
                default:
                    return API.PersianDaysOfWeek.None;
            }
        }
    }
}


namespace API
{

    public enum PersianDaysOfWeek : int
    {
        None = 0,
        شنبه = 1 << 0,
        یکشنبه = 1 << 1,
        دوشنبه = 1 << 2,
        سهشنبه = 1 << 3,
        چهارشنبه = 1 << 4,
        پنجشنبه = 1 << 5,
        جمعه = 1 << 6,
    }

    public enum PersianMonthsOfyear : int
    {
        None = 0,
        فروردین = 1 << 0,
        اردیبهشت = 1 << 1,
        خرداد = 1 << 2,
        تیر = 1 << 3,
        مرداد = 1 << 4,
        شهریور = 1 << 5,
        مهر = 1 << 6,
        آبان = 1 << 7,
        آذر = 1 << 8,
        دی = 1 << 9,
        بهمن = 1 << 10,
        اسفند = 1 << 11,
    }

    public static class PersianExtensions
    {
        /// <summary>
        /// To the persian date.
        /// </summary>
        /// <param name="EnDate">The en date.</param>
        /// <param name="Format">The format.
        /// Full Format "{0}/{1}/{2} {3}:{4}:{5}.{6}"
        ///             "Year/Month/Day Hour:Minute:Second.Milliseconds"
        /// </param>
        /// <returns></returns>
        public static string ToPersianDateTimeString(this DateTime? EnDate, string Format = "{0}/{1}/{2} {3}:{4}:{5}")
        {
            try
            {
                if (EnDate.HasValue)
                {
                    PersianCalendar PC = new PersianCalendar();
                    return string.Format(Format,
                                         PC.GetYear(EnDate.Value).ToString("0000"),
                                         PC.GetMonth(EnDate.Value).ToString("00"),
                                         PC.GetDayOfMonth(EnDate.Value).ToString("00"),
                                         PC.GetHour(EnDate.Value).ToString("00"),
                                         PC.GetMinute(EnDate.Value).ToString("00"),
                                         PC.GetSecond(EnDate.Value).ToString("00"),
                                         PC.GetMilliseconds(EnDate.Value).ToString("0000")
                                        );
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        public static string ToPersianDateStringFull(this DateTime EnDate)
        {
            try
            {
                PersianCalendar PC = new PersianCalendar();
                string dayOfWeek = "";
                string monthOfYear = "";

                switch (PC.GetDayOfWeek(EnDate))
                {
                    case DayOfWeek.Saturday:
                        dayOfWeek = "شنبه";
                        break;
                    case DayOfWeek.Sunday:
                        dayOfWeek = "یکشنبه";
                        break;
                    case DayOfWeek.Monday:
                        dayOfWeek = "دوشنبه";
                        break;
                    case DayOfWeek.Tuesday:
                        dayOfWeek = "سه شنبه";
                        break;
                    case DayOfWeek.Wednesday:
                        dayOfWeek = "چهارشنبه";
                        break;
                    case DayOfWeek.Thursday:
                        dayOfWeek = "پنجشنبه";
                        break;
                    case DayOfWeek.Friday:
                        dayOfWeek = "جمعه";
                        break;
                    default:
                        dayOfWeek = "";
                        break;
                }

                switch (PC.GetMonth(EnDate))
                {
                    case 1:
                        monthOfYear = "فروردین";
                        break;
                    case 2:
                        monthOfYear = "اردیبهشت";
                        break;
                    case 3:
                        monthOfYear = "خرداد";
                        break;
                    case 4:
                        monthOfYear = "تیر";
                        break;
                    case 5:
                        monthOfYear = "مرداد";
                        break;
                    case 6:
                        monthOfYear = "شهریور";
                        break;
                    case 7:
                        monthOfYear = "مهر";
                        break;
                    case 8:
                        monthOfYear = "آبان";
                        break;
                    case 9:
                        monthOfYear = "آذر";
                        break;
                    case 10:
                        monthOfYear = "دی";
                        break;
                    case 11:
                        monthOfYear = "بهمن";
                        break;
                    case 12:
                        monthOfYear = "اسفند";
                        break;
                    default:
                        break;
                }

                return string.Format("{0} {1} {2} {3}", dayOfWeek, PC.GetDayOfMonth(EnDate).ToString("00"), monthOfYear, PC.GetYear(EnDate).ToString("0000"));
            }
            catch
            {
                return "";
            }
        }

        public static string ToPersianMonthString(this DateTime EnDate)
        {
            try
            {
                PersianCalendar PC = new PersianCalendar();
                string monthOfYear = "";

                switch (PC.GetMonth(EnDate))
                {
                    case 1:
                        monthOfYear = "فروردین";
                        break;
                    case 2:
                        monthOfYear = "اردیبهشت";
                        break;
                    case 3:
                        monthOfYear = "خرداد";
                        break;
                    case 4:
                        monthOfYear = "تیر";
                        break;
                    case 5:
                        monthOfYear = "مرداد";
                        break;
                    case 6:
                        monthOfYear = "شهریور";
                        break;
                    case 7:
                        monthOfYear = "مهر";
                        break;
                    case 8:
                        monthOfYear = "آبان";
                        break;
                    case 9:
                        monthOfYear = "آذر";
                        break;
                    case 10:
                        monthOfYear = "دی";
                        break;
                    case 11:
                        monthOfYear = "بهمن";
                        break;
                    case 12:
                        monthOfYear = "اسفند";
                        break;
                    default:
                        break;
                }

                return string.Format("{0} {1}", monthOfYear, PC.GetYear(EnDate).ToString("0000"));
            }
            catch
            {
                return "";
            }
        }

        public static string ToPersianDayOfWeekString(this DateTime EnDate)
        {
            try
            {
                PersianCalendar PC = new PersianCalendar();
                string dayOfWeek = "";

                switch (PC.GetDayOfWeek(EnDate))
                {
                    case DayOfWeek.Saturday:
                        dayOfWeek = "شنبه";
                        break;
                    case DayOfWeek.Sunday:
                        dayOfWeek = "یکشنبه";
                        break;
                    case DayOfWeek.Monday:
                        dayOfWeek = "دوشنبه";
                        break;
                    case DayOfWeek.Tuesday:
                        dayOfWeek = "سه شنبه";
                        break;
                    case DayOfWeek.Wednesday:
                        dayOfWeek = "چهارشنبه";
                        break;
                    case DayOfWeek.Thursday:
                        dayOfWeek = "پنجشنبه";
                        break;
                    case DayOfWeek.Friday:
                        dayOfWeek = "جمعه";
                        break;
                    default:
                        dayOfWeek = "";
                        break;
                }

                return string.Format("{0}", dayOfWeek);
            }
            catch
            {
                return "";
            }
        }

        public static string ToPersianDayOfMonthString(this DateTime EnDate)
        {
            PersianCalendar PC = new PersianCalendar();
            return string.Format("{0}", PC.GetDayOfMonth(EnDate).ToString("00"));
        }

        /// <summary>
        /// Converts the en date.
        /// </summary>
        /// <param name="PersianDate">The persian date.</param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string PersianDate /*1398/01/23 23:15*/)
        {
            try
            {
                if (PersianDate.IsEmpty()) return null;
                PersianCalendar pc = new PersianCalendar();

                bool NightDay = PersianDate.Contains("ب.ظ") | PersianDate.Contains("PM");
                PersianDate = PersianDate.Replace("ق.ظ", "").Replace("ب.ظ", "").
                                          Replace("AM", "").Replace("PM", "")
                                         .Replace("۰", "0")
                                         .Replace("۱", "1")
                                         .Replace("۲", "2")
                                         .Replace("۳", "3")
                                         .Replace("۴", "4")
                                         .Replace("۵", "5")
                                         .Replace("۶", "6")
                                         .Replace("۷", "7")
                                         .Replace("۸", "8")
                                         .Replace("۹", "9").Trim();
                int[] Date = PersianDate.Split('/', ':', ' ').Select(x => Convert.ToInt32(string.Format(new CultureInfo("en-GB"), "{0:C}", x.Trim(' ')))).ToArray();

                return pc.ToDateTime(Date.GetInt(0),// Year
                                     Date.GetInt(1),// Mounth
                                     Date.GetInt(2),// Day
                                     Date.GetInt(3) + (NightDay ? 12 : 0), // Hour
                                     Date.GetInt(4),// Minute
                                     Date.GetInt(5),// Seconde
                                     0);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the int.
        /// </summary>
        /// <param name="Date">The date.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static int GetInt(this int[] Date, int index)
        {
            return index < Date.Count() ? Date[index] : 0;
        }

        public static string FixPersianChars(this string Value)
        {
            return Value.Replace('ي', 'ی').Replace("ك", "ک");
        }
        /// <summary>
        /// Validate IR National Code
        /// </summary>
        /// <param name="nationalcode">National Code</param>
        /// <returns></returns>
        public static bool IsValidNationalCode(this string nationalcode)
        {
            int last;
            return nationalcode.IsValidNationalCode(out last);
        }
        /// <summary>
        /// Validate IR National Code
        /// </summary>
        /// <param name="nationalcode">National Code</param>
        /// <param name="lastNumber">Last Number Of National Code</param>
        /// <returns></returns>
        public static bool IsValidNationalCode(this string nationalcode, out int lastNumber)
        {
            lastNumber = -1;
            if (!nationalcode.IsItNumber()) return false;
            var invalid = new[]
                                    {
                                        "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555",
                                        "6666666666", "7777777777", "8888888888", "9999999999"
                                    };
            if (invalid.Contains(nationalcode)) return false;
            var array = nationalcode.ToCharArray();
            if (array.Length != 10) return false;
            var j = 10;
            var sum = 0;
            for (var i = 0; i < array.Length - 1; i++)
            {
                sum += Int32.Parse(array[i].ToString(CultureInfo.InvariantCulture)) * j;
                j--;
            }

            var diff = sum % 11;

            if (diff < 2)
            {
                lastNumber = diff;
                return diff == Int32.Parse(array[9].ToString(CultureInfo.InvariantCulture));
            }
            var temp = Math.Abs(diff - 11);
            lastNumber = temp;
            return temp == Int32.Parse(array[9].ToString(CultureInfo.InvariantCulture));
        }

    }

}
