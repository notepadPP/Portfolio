using System;

namespace Framework.Common
{

    public static partial class Extension
    {
        public static string ToFirstUpper(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            return char.ToUpper(s[0]) + s.Substring(1).ToLower();
        }
        public static string ToSignCommaString(this long value) => string.Format("{0:+#,##0;-#,##0;0}", value);
        public static string ToSignCommaString(this int value) => string.Format("{0:+#,##0;-#,##0;0}", value);
        public static string ToSignCommaString(this float value) => string.Format("{0:+#,##0;-#,##0;0}", value);
        public static string ToCommaString(this float value) => string.Format("{0:#,##0}", value);
        public static string ToCommaString(this int value) => string.Format("{0:#,##0}", value);
        public static string ToCommaString(this long value) => string.Format("{0:#,##0}", value);
        public static string ToSizeString(this long value)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (value > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(value, max), order);

                max /= scale;
            }
            return "0 Bytes";
        }

    }
}