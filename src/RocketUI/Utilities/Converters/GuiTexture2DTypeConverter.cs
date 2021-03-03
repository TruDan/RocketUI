using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace RocketUI.Utilities.Converters
{
    public class GuiTexture2DTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string text = value as string;
            if (text != null)
            {
                if (text.StartsWith('#'))
                {
                    // Parse as color
                    return ConvertFromColorString(text);
                }
                else if (text.Contains(':'))
                {

                    return new GuiTexture2D()
                    {
                        TextureResource = GuiTextures.Parse(text)
                    };
                }
                else
                {
                    return new GuiTexture2D(text);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }


        private GuiTexture2D ConvertFromColorString(string input)
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
                    throw new FormatException("Color is not in an acceptable format (#RGB, #ARGB, #RRGGBB, #AARRGGBB)");
            }

            var a = int.Parse(aStr, NumberStyles.HexNumber);
            var r = int.Parse(rStr, NumberStyles.HexNumber);
            var g = int.Parse(gStr, NumberStyles.HexNumber);
            var b = int.Parse(bStr, NumberStyles.HexNumber);

            return new GuiTexture2D()
            {
                Color = new Color(r, g, b, a)
            };
        }
    }
}