using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using ValidationPilotServices.Infrastructure.Enums;

namespace ValidationPilotServices.Infrastructure.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// This extension returns DescriptionAttribute Value
        /// for Enum item of the enumerator selected.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetItemDescription(this Enum item)
        {
            FieldInfo fi = item.GetType().GetField(item.ToString());
            DescriptionAttribute[] fields = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (fields?.Length > 0)
                return fields[0].Description;
            else
                return string.Empty;
        }

        public static EnumPresence IsNullableFieldFromFieldProfile(this string fieldPresence)
        {
            if (string.IsNullOrEmpty(fieldPresence))
            {
                return EnumPresence.NON_MANDATORY;
            }
            else if (fieldPresence.ToUpper().Equals("MANDATORY"))
            {
                return EnumPresence.MANDATORY;
            }
            else if (fieldPresence.ToUpper().Equals("MANDATORYIF"))
            {
                return EnumPresence.MANDATORY_IF;
            }

            return EnumPresence.MANDATORY;
        }

        public static int GetBytesCount(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return 0;
            }

            Encoding utf8 = Encoding.UTF8;
            char[] array = source.ToCharArray();
            return utf8.GetByteCount(array);
        }
    }
}
