using System.ComponentModel;
using Microsoft.Xna.Framework;
using RocketUI.Utilities.Converters;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        TypeDescriptor.AddAttributes(typeof(Color), new TypeConverterAttribute(typeof(ColorTypeConverter)));
    }
}