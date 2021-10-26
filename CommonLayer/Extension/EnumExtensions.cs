using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer
{

    public class EnumClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
    }
    public static class EnumExtensions
    {
        public static string ToIntString(this Enum Enumer)
        {
            return Convert.ToInt32((object)Enumer).ToString();
        }

        public static int ToInt32(this Enum Enumer)
        {
            return Convert.ToInt32((object)Enumer);
        }

        public static short ToInt16(this Enum Enumer)
        {
            return Convert.ToInt16((object)Enumer);
        }

        public static byte ToByte(this Enum Enumer)
        {
            return Convert.ToByte((object)Enumer);
        }

        public static bool IsSet(this Enum input, Enum matchTo)
        {
            return (Convert.ToUInt32((object)input) & Convert.ToUInt32((object)matchTo)) > 0U;
        }


        public static IEnumerable<EnumClass> EnumToList(this Type type)
        {
            List<string> list1 = new List<string>();
            List<EnumClass> list2 = new List<EnumClass>();
            foreach (string name in Enum.GetNames(type))
            {
                foreach (DescriptionAttribute descriptionAttribute in type.GetField(name).GetCustomAttributes(typeof(DescriptionAttribute), true))
                {
                    byte num1 = (byte)Enum.Parse(type, name);
                    List<EnumClass> list3 = list2;
                    EnumClass enumClass = new EnumClass();
                    byte num2 = num1;
                    enumClass.Id = num2;
                    string description = descriptionAttribute.Description;
                    enumClass.Title = description;
                    enumClass.Name = name;
                    list3.Add(enumClass);
                }
            }
            return (IEnumerable<EnumClass>)list2;
        }

        public static T ToEnum<T>(this string value) where T : struct
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        public static T Parse<T>(string value) where T : struct
        {
            return Parse<T>(value, false);
        }

        public static T Parse<T>(string value, bool ignoreCase) where T : struct
        {
            //if (!typeof(T).IsEnum)
            //{
            //    throw new ArgumentException("T must be an enum type.");
            //}

            var result = (T)Enum.Parse(typeof(T), value, ignoreCase);
            return result;
        }

        public static T ToEnum<T>(this string value, bool ignoreCase) where T : struct
        {
            return Parse<T>(value, ignoreCase);
        }

        /// <summary>
        /// Converts Enumeration type into a dictionary of names and values
        /// </summary>
        /// <param name="t">Enum type</param>
        public static IDictionary<string, int> EnumToDictionary(this Type t)
        {
            if (t == null) throw new NullReferenceException();
            if (!t.IsEnum) throw new InvalidCastException("object is not an Enumeration");

            string[] names = Enum.GetNames(t);
            Array values = Enum.GetValues(t);

            return (from i in Enumerable.Range(0, names.Length)
                    select new { Key = names[i], Value = (int)values.GetValue(i) })
                        .ToDictionary(k => k.Key, k => k.Value);
        }


        public static string GetEnumDescription<T>(string value)
        {
            Type type = typeof(T);
            var name = Enum.GetNames(type).Where(f => f.Equals(value, StringComparison.CurrentCultureIgnoreCase)).Select(d => d).FirstOrDefault();
            if (name == null) return string.Empty;
            var customAttribute = type.GetField(name).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return customAttribute.Length > 0 ? ((DescriptionAttribute)customAttribute[0]).Description : name;
        }
        public static string GetEnumDescription(object value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi != null)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
            }
            else return "";
        }
        public static string GetDescription(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return null;
        }


        #region DateEnum

        public enum PersianDayOfWeek
        {
            Saturday = 0,
            Sunday = 1,
            Monday = 2,
            Tuesday = 3,
            Wednesday = 4,
            Thursday = 5,
            Friday = 6,
        }

        #endregion DateEnume

        #region UserEnum

        public enum UserTypeEnum
        {
            Admin = 1,
            Teacher = 2,
            Student = 3,
        }

        #endregion UserEnum

        #region PersonEnum


        public enum EnumGenderType : byte
        {
            [Description("مرد")]
            Man = 1,
            [Description("زن")]
            Voman = 2
        }
        public enum EnumGrade : byte
        {
            [Description("دکترا")]
            PhD = 1,
            [Description("فوق لیسانس")]
            Master = 2,
            [Description("لیسانس")]
            Bachelor = 3,
            [Description("فوق دیپلم")]
            Associate = 4,
            [Description("دیپلم")]
            Diploma = 5
        }
        #endregion PersonEnum



    }
}