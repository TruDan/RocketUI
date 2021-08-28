using System;
using System.ComponentModel;
using System.Globalization;

namespace RocketUI.Utilities.Converters
{


    public class SizeConverter : TypeConverter
    {
        /// <summary>
        /// The character to split up the string which will be converted
        /// </summary>
        static readonly char DimensionSplitter = ',';

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
        /// Converts the specified value to a <see cref="Size"/>
        /// </summary>
        /// <param name="context">Conversion context</param>
        /// <param name="culture">Culture to perform the conversion</param>
        /// <param name="value">Value to convert</param>
        /// <returns>A new instance of a <see cref="Size"/> converted from the specified <paramref name="value"/></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string text = value as string;
            if (text != null)
            {
                string[] parts = text.Split(DimensionSplitter, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot parse value '{0}' as Size. Should be in the form of 'width, height'", text));

                try
                {
                    return new Size(
                                    int.Parse(parts[0]),
                                    int.Parse(parts[1])
                                   );
                }
                catch
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot parse value '{0}' as Size. Should be in the form of 'width, height'", text));
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType.IsAssignableFrom(typeof(string))) return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type                                                destinationType)
        {
            if (!destinationType.IsAssignableFrom(typeof(string)) || !(value is Size size))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            return size.Width.ToString() + DimensionSplitter +
                   size.Height.ToString();
        }
    }
}