using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Portable.Xaml.Markup;

namespace RocketUI
{
    public interface IStyle
    {
        string Name       { get; set; }
        Type   TargetType { get; set; }
        void ApplyStyle(IGuiElement element);
    }
    
    [ContentProperty(nameof(Setters))]
    public class Style : IStyle
    {
        public string Name { get; set; }
        
        public Style InheritFrom { get; set; }
        
        public Type  TargetType  { get; set; }

        public ObservableCollection<Setter> Setters
        {
            get => _setters;
            set
            {
                _setters = value;
                if (_setters != null)
                {
                    _setters.CollectionChanged += SettersOnCollectionChanged;
                }
            }
        }

        public Style()
        {
            Setters = new ObservableCollection<Setter>();
        }

        public void ApplyStyle(IGuiElement element)
        {
            if(element == null) return;

            if (TargetType != null && !(TargetType.IsAssignableFrom(element.GetType())))
                return;
            
            var styles = CompileSetters();
            foreach ((var prop, var setProp) in styles)
            {
                setProp(element);
            }
        }
        
        private Dictionary<string, SetStyleProperty> _computedSetters = new Dictionary<string, SetStyleProperty>();
        private ObservableCollection<Setter>         _setters;
        private bool                                 _dirty;
        private void SettersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _dirty = true;
        }

        private Dictionary<string, SetStyleProperty> CompileSetters()
        {
            if (_dirty)
            {
                var result = new Dictionary<string, SetStyleProperty>();

                if (InheritFrom != null)
                {
                    var inheritedSetters = InheritFrom.CompileSetters();
                    foreach (var kvp in inheritedSetters)
                    {
                        result.Add(kvp.Key, kvp.Value);
                    }
                }
                
                foreach (var setter in Setters.ToArray())
                {
                    result[setter.Property] = CreateSetterDelegate(setter.Property, setter.Value);
                }

                _computedSetters = result;
                _dirty = false;
            }

            return _computedSetters;
        }

        private static SetStyleProperty CreateSetterDelegate(string propertyName, object newValue)
        {
            return (element) =>
            {
                try
                {
                    var type = element.GetType();
                    var prop = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                    if (prop != null)
                    {
                        if (prop.CanWrite)
                        {
                            if (newValue != null && prop.PropertyType.IsInstanceOfType(newValue))
                            {
                                prop.SetValue(element, newValue);
                            }
                            else
                            {
                                var converter = TypeDescriptor.GetConverter(prop.PropertyType);
                                var convertedValue = newValue is string str 
                                    ? converter.ConvertFromInvariantString(str) 
                                    : converter.ConvertFrom(newValue);
                                prop.SetValue(element, convertedValue);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception while trying to set style property value: {ex.Message}");
                    Console.WriteLine(ex);
                }
            };
        }
    }
    
    public delegate void SetStyleProperty(IGuiElement guiElement);
}