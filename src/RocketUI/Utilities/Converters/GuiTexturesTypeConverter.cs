using System.Globalization;

namespace RocketUI.Utilities.Converters
{
    public class GuiTexturesTypeConverter : TypeConverterBase<GuiTextures>
    {
        protected override GuiTextures FromString(CultureInfo cultureInfo, string value)
        {
            return GuiTextures.Parse(value);
        }
    }
}