using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace RocketUI.Utilities.Converters
{
    public class EnumTypeConverter<TEnum> : TypeConverter where TEnum : struct
    {
        /// <summary>
        /// The character to split up the string which will be converted
        /// </summary>
        static readonly char[] FlagsSplitter = new char[] { ',' };


        /// <summary>
        /// Determines if this converter can convert from the specified <paramref name="sourceType"/>
        /// </summary>
        /// <param name="context">Conversion context</param>
        /// <param name="sourceType">Type to convert from</param>
        /// <returns>True if this converter can convert from the specified type, false otherwise</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the specified value to a <see cref="Point"/>
        /// </summary>
        /// <param name="context">Conversion context</param>
        /// <param name="culture">Culture to perform the conversion</param>
        /// <param name="value">Value to convert</param>
        /// <returns>A new instance of a <see cref="Point"/> converted from the specified <paramref name="value"/></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string text = value as string;
            if (text != null)
            {
                string[] parts = text.Split(FlagsSplitter, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                    return default(TEnum);

                TEnum val;
                
                try
                {
                    if (Enum.TryParse(text, true, out val))
                    {
                        return val;
                    }
                }
                catch
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        "Cannot parse value '{0}' as '{1}'. Should be in the form of 'Flag1,Flag2' or 'Flag1'",
                        text, typeof(TEnum).GetTypeInfo().FullName));
                }
                
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}