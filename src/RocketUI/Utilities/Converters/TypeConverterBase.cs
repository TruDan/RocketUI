using System;
using System.ComponentModel;
using System.Globalization;

namespace RocketUI.Utilities.Converters
{
    public abstract class TypeConverterBase<T> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (
                sourceType.IsAssignableFrom(typeof(string))
                || sourceType.IsAssignableFrom(typeof(T))
            ) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType.IsAssignableFrom(typeof(T)) ||
                destinationType.IsAssignableFrom(typeof(string))) return true;

            return base.CanConvertTo(context, destinationType);
        }

        protected abstract T FromString(CultureInfo cultureInfo, string value);

        protected virtual string ToString(CultureInfo cultureInfo, T value)
        {
            return value?.ToString();
        }
        
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                return FromString(culture, stringValue);
            }
            
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType.IsAssignableFrom(typeof(string)))
            {
                if (value is T t)
                {
                    return ToString(culture, t);
                }
            }
            
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}