using System.ComponentModel;
using RocketUI.Utilities.Converters;

namespace RocketUI
{
    [TypeConverter(typeof(EnumTypeConverter<AutoSizeMode>))]
    public enum AutoSizeMode
    {
        None,
        GrowAndShrink,
        GrowOnly,
    }
}
