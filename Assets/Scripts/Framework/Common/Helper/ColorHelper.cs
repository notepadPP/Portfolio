using UnityEngine;

namespace Framework.Common.Helper
{
    public static partial class ExtColor
    {
        public static int ColorToInt(this Color color) => (((int)color.r) << 24) | (((int)color.g) << 16) | (((int)color.b) << 8) | (((int)color.a));
        public static string ColorToHex(this Color color) => ColorUtility.ToHtmlStringRGBA(color);
        public static int HexToInt(this string hex, Color fallback) => HexToColor(hex, fallback).ColorToInt();
        public static Color IntToColor32(this int value) => new Color32((byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value);
        public static Color IntToColor24(this int value) => new Color32((byte)(value >> 16), (byte)(value >> 8), (byte)value, byte.MaxValue);
        public static Color HexToColor(this string hex) => HexToColor(hex, Color.white);
        public static Color HexToColor(this string hex, Color fallback) => ColorUtility.TryParseHtmlString(hex, out var color) ? color : fallback;
        public static string ToARGB(this Color color) => ColorUtility.ToHtmlStringRGB(color);
    }
}
