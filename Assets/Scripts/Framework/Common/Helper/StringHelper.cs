using UnityEngine;

namespace Framework.Common.Helper
{
    public static partial class StringHelper
    {
        public static string ToSignCommaString(this long self) => string.Format("{0:+#,##0;-#,##0;0}", self);
        public static string ToSignCommaString(this int self) => string.Format("{0:+#,##0;-#,##0;0}", self);
        public static string ToSignCommaString(this float self) => string.Format("{0:+#,##0;-#,##0;0}", self);
        public static string ToCommaString(this float self) => string.Format("{0:#,##0}", self);
        public static string ToCommaString(this int self) => string.Format("{0:#,##0}", self);
        public static string ToCommaString(this long self) => string.Format("{0:#,##0}", self);
        public static string ToRichColorString(this string str, Color color) => $"<color=#{color.ToARGB()}>{str}</color>";
    }
}