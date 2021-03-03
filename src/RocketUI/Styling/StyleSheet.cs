using System.Collections.Generic;
using Portable.Xaml.Markup;

namespace RocketUI
{
    [ContentProperty(nameof(Styles))]
    public class StyleSheet
    {
        public List<Style> Styles { get; set; }
    }
}