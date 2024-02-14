using Newtonsoft.Json;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Tribitgroup.Framework.Shared.Extensions
{
    public static class BasicTypesExtensions
    {
        public static Guid GetSequentialGuid() => DateTime.UtcNow.ToGuid();

        public static bool IsArrayOrList(this Type type)
        {
            return type.IsArray || (type.IsGenericType && type.GetInterfaces().Contains(typeof(IEnumerable)));
        }

        public static bool IsArrayOrList<T>(this T instance)
        {
            var type= typeof(T);
            return type.IsArray || (type.IsGenericType && type.GetInterfaces().Contains(typeof(IEnumerable)));
        }

        public static bool IsBasicType(this Type type)
        {
            return
                type == typeof(int) ||
                type == typeof(double) ||
                type == typeof(char) ||
                type == typeof(bool) ||
                type == typeof(string) ||
                type == typeof(byte) ||
                type == typeof(short) ||
                type == typeof(long) ||
                type == typeof(float) ||
                type == typeof(decimal) ||
                type == typeof(Guid) ||
                type == typeof(DateTime) ||
                type == typeof(sbyte) ||
                type == typeof(ushort) ||
                type == typeof(uint);
        }

        public static object? ChangeTo<T>(this T instance, Type toType, object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return default;
            }

            if (toType.IsEnum)
            {
                return Enum.Parse(toType, value.ToString() ?? "");
            }

            if (toType == typeof(Guid))
            {
                return new Guid(value.ToString() ?? "");
            }

            if (toType == typeof(DateTime))
            {
                if (DateTime.TryParse(value.ToString(), out DateTime dateTime))
                {
                    return dateTime;
                }
                return default;
            }

            try
            {
                return Convert.ChangeType(value, toType);
            }
            catch
            {
                return default(T);
            }
        }

        public static T? To<T>(this object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return default;
            }

            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), value.ToString() ?? "");
            }

            if (typeof(T) == typeof(Guid))
            {
                return (T)(object)new Guid(value.ToString() ?? "");
            }

            if (typeof(T) == typeof(DateTime))
            {
                if (DateTime.TryParse(value.ToString(), out DateTime dateTime))
                {
                    return (T)(object)dateTime;
                }
                return default;
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }

        public static Type? GetGenericTypeFromList(this Type type, string propName)
            => type.GetProperty(propName)?.PropertyType.GenericTypeArguments[0];

        public static Guid ToGuid(this long value)
        {
            byte[] guidData = new byte[16];
            Array.Copy(BitConverter.GetBytes(value), guidData, 8);
            return new Guid(guidData);
        }

        public static long ToLong(this Guid guid)
        {
            if (BitConverter.ToInt64(guid.ToByteArray(), 8) != 0)
                throw new OverflowException("Value was either too large or too small for an Int64.");
            return BitConverter.ToInt64(guid.ToByteArray(), 0);
        }

        public static Guid ToGuid(this DateTime date) => date.Ticks.ToGuid();

        public static string ToJson(this object value) => JsonConvert.SerializeObject(value);
        public static T? FromJson<T>(this string value) => JsonConvert.DeserializeObject<T>(value);

        public static T? NewId<T>(T id)
        {
            if (typeof(T) != typeof(Guid) && id != null)
                return id;
            if (typeof(T) == typeof(Guid) && id?.ToString() != Guid.Empty.ToString())
                return id;
            if (typeof(T) == typeof(Guid))
                return (T)Convert.ChangeType(GetSequentialGuid(), typeof(T));
            return default;
        }

        public static string Description(this Enum @enum)
        {
            FieldInfo fi = @enum.GetType().GetField(@enum.ToString()) ?? throw new NullReferenceException();

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return @enum.ToString();
        }
    }
}