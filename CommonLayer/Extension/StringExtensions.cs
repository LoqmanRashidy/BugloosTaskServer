using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonLayer.Extension
{
    public static class StringExtensions
    {

        /// <summary>
        /// If string is number return true
        /// </summary>
        /// <param name="s"></param>
        /// <returns>bool</returns>
        public static bool IsItNumber(this string s)
        {
            var isnumber = new Regex("[^0-9]");
            return !isnumber.IsMatch(s);
        }

        /// <summary>
        /// Returns the last few characters of the string with a length
        /// specified by the given parameter. If the string's length is less than the 
        /// given length the complete string is returned. If length is zero or 
        /// less an empty string is returned
        /// </summary>
        /// <param name="s">the string to process</param>
        /// <param name="length">Number of characters to return</param>
        /// <returns></returns>
        public static string Right(this string s, int length)
        {
            length = Math.Max(length, 0);
            return s.Length > length ? s.Substring(s.Length - length, length) : s;
        }

        /// <summary>
        /// Returns the first few characters of the string with a length
        /// specified by the given parameter. If the string's length is less than the 
        /// given length the complete string is returned. If length is zero or 
        /// less an empty string is returned
        /// </summary>
        /// <param name="s">the string to process</param>
        /// <param name="length">Number of characters to return</param>
        /// <returns></returns>
        public static string Left(this string s, int length)
        {
            length = Math.Max(length, 0);
            return s.Length > length ? s.Substring(0, length) : s;
        }

        /// <summary>
        /// returns default value if string is null or empty or white spaces string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <param name="considerWhiteSpaceIsEmpty"></param>
        /// <returns></returns>
        public static string DefaultIfEmpty(this string str, string defaultValue, bool considerWhiteSpaceIsEmpty = false)
        {
            return (considerWhiteSpaceIsEmpty ? string.IsNullOrWhiteSpace(str) : string.IsNullOrEmpty(str))
                       ? defaultValue
                       : str;
        }

        public static string NullIfEmpty(this string str, bool considerWhiteSpaceIsEmpty = false)
        {
            return (considerWhiteSpaceIsEmpty ? string.IsNullOrWhiteSpace(str) : string.IsNullOrEmpty(str))
                       ? null
                       : str;
        }

        /// <summary>
        /// It returns true if string is null or empty or just a white space otherwise it returns false.
        /// </summary>
        /// <param name="input">Input String</param>
        /// <returns>bool</returns>
        public static bool IsEmpty(this string input)
        {
            return string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input);
        }

        /// <summary>
        /// Reverse a string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Reverse(this string s)
        {
            char[] c = s.ToCharArray();
            Array.Reverse(c);
            return new string(c);
        }

        /// <summary>
        /// Repeat the given char the specified number of times.
        /// </summary>
        /// <param name="input">The char to repeat.</param>
        /// <param name="count">The number of times to repeat the string.</param>
        /// <returns>The repeated char string.</returns>
        public static string Repeat(this char input, int count)
        {
            return new string(input, count);
        }

        /// <summary>
        /// Capitalize each word
        /// Exp: 
        ///     david jons => David Jons
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToCapitalWord(this string input)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
        }

        public static string Summery(this string txt, int Maxlen)
        {
            string[] words = txt.Split(' ');
            string ret = "";

            for (int i = 1; i <= words.Length; i++)
            {
                ret = string.Join(" ", words, 0, i);
                if (ret.Length > Maxlen) break;
            }
            if (txt.Length > ret.Length) ret += "...";
            return ret;
        }

        public static int? ToInt32(this string txt)
        {
            int value;
            if (int.TryParse(txt, out value)) return (int?)value;
            else return null;
        }

        public static float? ToFloat(this string txt)
        {
            float value;
            if (float.TryParse(txt, out value)) return (float?)value;
            else return null;
        }

        public static double? ToDouble(this string txt)
        {
            double value;
            if (double.TryParse(txt, out value)) return (double?)value;
            else return null;
        }

        public static long? ToLong(this string txt)
        {
            long value;
            if (long.TryParse(txt, out value)) return (long?)value;
            else return null;
        }

        //public static string BuildString(this string Message, object obj)
        //{
        //    try
        //    {
        //        int startindex = 0, lastindex = 0;
        //        string variable = "", ret = "", var;
        //        while (startindex != -1 && lastindex != -1)
        //        {
        //            startindex = Message.IndexOf("[", startindex + (startindex == 0 ? 0 : 1));
        //            lastindex = Message.IndexOf("]", startindex + (startindex == 0 ? 0 : 1));
        //            if (startindex != -1 && lastindex != -1)
        //            {
        //                var = Message.Substring(startindex, lastindex - startindex + 1);
        //                if (var.Length > 2) variable = var.Substring(1, var.Length - 2);

        //                ret = (obj.ReflectValue(variable) ?? "").ToString();
        //                Message = Message.Replace(var, ret);
        //            }
        //        }
        //        return Message;
        //    }
        //    catch
        //    {
        //        return "-1";
        //    }
        //}

        public static string ToEnglishNumber(this string input)
        {
            string[] strArray1 = new string[10];
            int index1 = 0;
            string str1 = "۰";
            strArray1[index1] = str1;
            int index2 = 1;
            string str2 = "۱";
            strArray1[index2] = str2;
            int index3 = 2;
            string str3 = "۲";
            strArray1[index3] = str3;
            int index4 = 3;
            string str4 = "۳";
            strArray1[index4] = str4;
            int index5 = 4;
            string str5 = "۴";
            strArray1[index5] = str5;
            int index6 = 5;
            string str6 = "۵";
            strArray1[index6] = str6;
            int index7 = 6;
            string str7 = "۶";
            strArray1[index7] = str7;
            int index8 = 7;
            string str8 = "۷";
            strArray1[index8] = str8;
            int index9 = 8;
            string str9 = "۸";
            strArray1[index9] = str9;
            int index10 = 9;
            string str10 = "۹";
            strArray1[index10] = str10;
            string[] strArray2 = strArray1;
            if(Enumerable.Any<string>((IEnumerable<string>)strArray2, new Func<string, bool>(input.Contains)))
            {
                for(int index = 0; index < strArray2.Length; ++index)
                    input = input.Replace(strArray2[index], index.ToString());
            }
            return input;
        }

    }
}
