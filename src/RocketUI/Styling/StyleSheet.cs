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
                .Where(x =>
                {
                    if (classNames == null || x.Name == null)
                        return true;

                    if (classNames.Contains(x.Name))
                        return true;

                    var split = x.Name.Split("&&");

                    for (int i = 0; i < split.Length; i++)
                    {
                        var className = split[i].Trim();

                        if (className.StartsWith('!'))
                        {
                            className = className.Substring(1);

                            if (classNames.Contains(className))
                                return false;
                        }
                        else
                        {
                            if (!classNames.Contains(className))
                                return false;
                        }
                    }

                    return true;
                })
                .ToArray();
        }
    }
}