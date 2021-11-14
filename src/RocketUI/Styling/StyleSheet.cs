using System;
using System.Collections.Generic;
using System.Linq;
using Portable.Xaml.Markup;

namespace RocketUI
{
    [ContentProperty(nameof(Styles))]
    public class StyleSheet
    {
        public ResourceDictionary Resources { get; set; }
        public List<Style> Styles { get; set; }

        public StyleSheet()
        {
            Resources = new ResourceDictionary();
            Styles = new List<Style>();
        }

        public Style[] ResolveStyles(Type targetType, string[] classNames)
        {
            if (Styles == null) return Array.Empty<Style>();

            return Styles
                .Where(x => x.TargetType == null || x.TargetType.IsAssignableFrom(targetType))
                .Where(x => x.Name == null || classNames == null || classNames.Contains(x.Name))
                .ToArray();
        }
    }
}