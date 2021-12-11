using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using RocketUI.Attributes;

namespace RocketUI
{
    public partial class RocketElement
    {        
        private          ObservableCollection<string> _classNames;
        private          IStyle[]                     _styles;
        private          ElementStyle                 _style;

        public ElementStyle Style
        {
            get
            {
                if (_style == null)
                {
                    _style = new ElementStyle(this);
                }

                return _style;
            }
        }

        public ObservableCollection<string> ClassNames 
        {
            get
            {
                if (_classNames == null)
                {
                    _classNames = new ObservableCollection<string>();
                    _classNames.CollectionChanged += ClassNamesOnCollectionChanged;
                }

                return _classNames;
            }
        }

        [DebuggerVisible] public bool                         IsStyleDirty { get; protected set; } = true;
        
        private void ClassNamesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateStyle();
        }

        public void AddClass(params string[] classNames)
        {
            foreach (var className in classNames)
            {
                ClassNames.Add(className);
            }
        }
        public void RemoveClass(params string[] classNames)
        {
            foreach (var className in classNames)
            {
                if(ClassNames.Contains(className))
                    ClassNames.Remove(className);
            }
        }

        public bool HasClass(string className) => HasAnyClass(new[] {className});
        public bool HasAnyClass(params string[] classNames)
        {
            return classNames.Any(className => ClassNames.Contains(className));
        }    
        public void ToggleClass(bool value, params string[] classNames)
        {
            if (value)
            {
                AddClass(classNames);
            }
            else
            {
                RemoveClass(classNames);
            }
        }
        
        public void InvalidateStyle(bool invalidateChildren = true)
        {
            InvalidateStyle(this, invalidateChildren);
        }
        
        public void InvalidateStyle(IGuiElement sender, bool invalidateChildren = true)
        {
            IsStyleDirty = true;

            if (invalidateChildren)
            {
                ForEachChild(c =>
                {
                    if (c != sender)
                    {
                        c.InvalidateStyle(this, true);
                    }
                });
            }
        }

        private void UpdateStyle()
        {
            if(!IsStyleDirty) return;
            
            _styles = GuiRenderer.ResolveStyles(this.GetType(), ClassNames.ToArray()) ?? new IStyle[0];
            Style.Inherited = _styles;
            ApplyStyles();
        }

        public void ApplyStyles()
        {            
            Style.ApplyStyle(this);
        }

    }
    
    public class ElementStyle : IStyle
    {
        private Style       _overridesStyle = new Style();
        public  IGuiElement Owner { get; }

        public string Name       { get; set; }

        public Type     TargetType { get; set; }
        public IStyle[] Inherited  { get; set; }

        private Dictionary<string, Setter> _overrides = new Dictionary<string, Setter>();
        
        public object this[string property]
        {
            get
            {
                if (_overrides.TryGetValue(property, out var setter))
                {
                    return setter.Value;
                }

                return null;
            }
            set
            {
                if (_overrides.TryGetValue(property, out var oldSetter))
                {
                    _overridesStyle.Setters.Remove(oldSetter);
                }
                
                var setter = new Setter(property, value);
                _overridesStyle.Setters.Add(setter);
                _overrides[property] = setter;
                
                Owner.InvalidateStyle();
            }
        }

        public ElementStyle(IGuiElement owner)
        {
            Owner = owner;
        }
        
        public void ApplyStyle(IGuiElement element)
        {
            if (Inherited != null)
            {
                foreach (var style in Inherited)
                {
                    style.ApplyStyle(element);
                }
            }
            
            _overridesStyle.ApplyStyle(element);
        }
    }
}