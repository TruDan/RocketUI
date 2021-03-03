using System;
using System.Collections.Generic;
using Portable.Xaml.Markup;

namespace RocketUI
{
    [ContentProperty(nameof(Setters))]
    public class Style
    {
        public string Name { get; set; }
        
        public string InheritFrom { get; set; }
        
        public Type TargetType { get; set; }
        
        public List<Setter> Setters { get; set; }
    }
}