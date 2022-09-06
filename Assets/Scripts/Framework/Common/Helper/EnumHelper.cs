using System;
using System.Runtime.InteropServices;
using System.Text;
namespace Framework.Common.Helper
{
    public static class EnumHelper
    {
        [StructLayout(LayoutKind.Explicit)]
        struct EnumValue<T, E> where T : unmanaged where E : unmanaged, Enum
        {
            [FieldOffset(0)]
            public T iValue;
            [FieldOffset(0)]
            public E eValue;
        }
        // # Enum
        #region ToValue
        public static T ToValue<T, E>(this E value) where T : unmanaged where E : unmanaged, Enum
        {
            var enumValue = new EnumValue<T, E>() { eValue = value };
            return enumValue.iValue;
        }
        public static byte ToByte<E>(this E value) where E : unmanaged, Enum
        {
            var enumValue = new EnumValue<byte, E>() { eValue = value };
            return enumValue.iValue;
        }
        public static sbyte ToSByte<E>(this E value) where E : unmanaged, Enum
        {
            var enumValue = new EnumValue<sbyte, E>() { eValue = value };
            return enumValue.iValue;
        }
        public static short ToShort<E>(this E value) where E : unmanaged, Enum
        {
            var enumValue = new EnumValue<short, E>() { eValue = value };
            return enumValue.iValue;
        }
        public static ushort ToUShort<E>(this E value) where E : unmanaged, Enum
        {
            var enumValue = new EnumValue<ushort, E>() { eValue = value };
            return enumValue.iValue;
        }
        public static int ToInt<E>(this E value) where E : unmanaged, Enum
        {
            var enumValue = new EnumValue<int, E>() { eValue = value };
            return enumValue.iValue;
        }
        public static uint ToUInt<E>(this E value) where E : unmanaged, Enum
        {
            var enumValue = new EnumValue<uint, E>() { eValue = value };
            return enumValue.iValue;
        }
        public static long ToLong<E>(this E value) where E : unmanaged, Enum
        {
            var enumValue = new EnumValue<long, E>() { eValue = value };
            return enumValue.iValue;
        }
        public static ulong ToULong<E>(this E value) where E : unmanaged, Enum
        {
            var enumValue = new EnumValue<ulong, E>() { eValue = value };
            return enumValue.iValue;
        }
        #endregion// ToValue
        #region ToEnum
        public static E ToEnum<T, E>(this T value) where T : unmanaged where E : unmanaged, Enum
        {
            var enumValue = default(EnumValue<T, E>);
            enumValue.iValue = value;
            return enumValue.eValue;
            
        }
        public static E ToEnum<E>(this byte value) where E : unmanaged, Enum
        {
            var enumValue = default(EnumValue<byte, E>);
            enumValue.iValue = value;
            return enumValue.eValue;
        }
        public static E ToEnum<E>(this sbyte value) where E : unmanaged, Enum
        {
            var enumValue = default(EnumValue<sbyte, E>);
            enumValue.iValue = value;
            return enumValue.eValue;
        }
        public static E ToEnum<E>(this short value) where E : unmanaged, Enum
        {
            var enumValue = default(EnumValue<short, E>);
            enumValue.iValue = value;
            return enumValue.eValue;
        }
        public static E ToEnum<E>(this ushort value) where E : unmanaged, Enum
        {
            var enumValue = default(EnumValue<ushort, E>);
            enumValue.iValue = value;
            return enumValue.eValue;
        }
        public static E ToEnum<E>(this int value) where E : unmanaged, Enum
        {
            var enumValue = default(EnumValue<int, E>);
            enumValue.iValue = value;
            return enumValue.eValue;
        }
        public static E ToEnum<E>(this uint value) where E : unmanaged, Enum
        {
            var enumValue = default(EnumValue<uint, E>);
            enumValue.iValue = value;
            return enumValue.eValue;
        }
        public static E ToEnum<E>(this long value) where E : unmanaged, Enum
        {
            var enumValue = default(EnumValue<long, E>);
            enumValue.iValue = value;
            return enumValue.eValue;
        }
        public static E ToEnum<E>(this ulong value) where E : unmanaged, Enum
        {
            var enumValue = default(EnumValue<ulong, E>);
            enumValue.iValue = value;
            return enumValue.eValue;
        }
        #endregion// ToEnum
    }
}