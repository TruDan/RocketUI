using System.ComponentModel;
using RocketUI.Utilities.Converters;

namespace RocketUI
{
    [TypeConverter(typeof(EnumTypeConverter<TextureRepeatMode>))]
    public enum TextureRepeatMode
    {
        NoRepeat,
        Tile,
        ScaleToFit,
        Stretch,
        NoScaleCenterSlice
    }
}
