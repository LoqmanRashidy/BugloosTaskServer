using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer
{
    public static class GuardExtensions
    {
        /// <summary>
        /// Checks if the argument is null.
        /// </summary>
        public static void CheckArgumentIsNull(this object o, string name)
        {
            if (o == null)
                throw new ArgumentNullException(name);
        }

    }

    public static class Globals
    {
        public static dynamic User = null;
        public static dynamic Teacher = null;
        public static dynamic Student = null;
    }
    public static class Extensions
    {
        public static byte ToByte(this object @this)
        {
            return Convert.ToByte(@this);
        }

        public static int ToInt(this object @this)
        {
            return Convert.ToInt32(@this);
        }

        public static long ToLong(this object @this)
        {
            return Convert.ToInt64(@this);
        }

        public static double ToDouble(this object @this)
        {
            return Convert.ToDouble(@this);
        }

        public static decimal ToDecimal(this object @this)
        {
            return Convert.ToDecimal(@this);
        }

        public static bool ToBool(this object @this)
        {
            return Convert.ToBoolean(@this);
        }
    }
}
