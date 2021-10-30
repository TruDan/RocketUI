using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;

namespace RocketUI.Utilities.Converters
{
    public class ColorTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (
                sourceType.IsAssignableFrom(typeof(string))
                || sourceType.IsAssignableFrom(typeof(Color))
            ) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType.IsAssignableFrom(typeof(Color)) ||
                destinationType.IsAssignableFrom(typeof(string))) return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string input)
            {
                if (input.StartsWith('#'))
                {
                    input = input.Substring(1); // remove leading '#'

                    string aStr, rStr, gStr, bStr;

                    switch (input.Length)
                    {
                        case 8: // ARGB
                            aStr = input.Substring(0, 2);
                            rStr = input.Substring(2, 2);
                            gStr = input.Substring(4, 2);
                            bStr = input.Substring(6, 2);
                            break;
                        case 4:
                            aStr = new string(input[0], 2);
                            rStr = new string(input[1], 2);
                            gStr = new string(input[2], 2);
                            bStr = new string(input[3], 2);
                            break;

                        case 6:
                            aStr = "FF";
                            rStr = input.Substring(0, 2);
                            gStr = input.Substring(2, 2);
                            bStr = input.Substring(4, 2);
                            break;

                        case 3:
                            aStr = "FF";
                            rStr = new string(input[0], 2);
                            gStr = new string(input[1], 2);
                            bStr = new string(input[2], 2);
                            break;

                        default:
                            throw new FormatException(
                                "Color is not in an acceptable format (#RGB, #ARGB, #RRGGBB, #AARRGGBB)");
                    }

                    var a = int.Parse(aStr, NumberStyles.HexNumber);
                    var r = int.Parse(rStr, NumberStyles.HexNumber);
                    var g = int.Parse(gStr, NumberStyles.HexNumber);
                    var b = int.Parse(bStr, NumberStyles.HexNumber);
                    return new Color(r, g, b, a);
                }
                else
                {
                    throw new FormatException("Color is not in an acceptable format (#RGB, #ARGB, #RRGGBB, #AARRGGBB)");
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType.IsAssignableFrom(typeof(string)))
            {
                if (value is Color color)
                {
                    if (color.A == byte.MaxValue)
                        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                    return $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
                }
            }
            
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}