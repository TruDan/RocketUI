using Portable.Xaml.Markup;

namespace RocketUI
{
    [ContentProperty(nameof(Value))]
    public class Setter
    {
        public string Property { get; set; }
        
        public object Value { get; set; }
    }
}