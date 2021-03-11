using System.Collections.Generic;
using Portable.Xaml.Markup;

namespace RocketUI
{
    [ContentProperty(nameof(Styles))]
    public class StyleSheet
    {
        public ResourceDictionary Resources { get; set; }
        public List<Style>   Styles    { get; set; }
    }
}