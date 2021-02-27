using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Portable.Xaml.Markup;

namespace RocketUI.Serialization.Xaml
{
    class RocketNameScope : INameScope
    {
        public object Instance { get; set; }

        readonly Dictionary<string, object> _registeredNames = new Dictionary<string, object>();

        public object FindName(string name)
        {
            if (_registeredNames.TryGetValue(name, out var result))
                return result;
            return null;
        }

        public void RegisterName(string name, object scopedElement)
        {
            if (scopedElement != null)
                _registeredNames.Add(name, scopedElement);

            if (Instance == null)
                return;

            var instanceType = Instance.GetType();
            var obj          = scopedElement as IGuiElement;
            if (obj != null && !string.IsNullOrEmpty(name))
            {
                var property = instanceType.GetRuntimeProperties()
                    .FirstOrDefault(r => r.Name == name && r.SetMethod != null &&
                                         !r.SetMethod.IsStatic);
                if (property != null)
                    property.SetValue(Instance, obj, null);
                else
                {
                    var field = instanceType.GetTypeInfo().GetDeclaredField(name);
                    if (field != null && !field.IsStatic)
                        field.SetValue(Instance, obj);
                }
            }
        }

        public void UnregisterName(string name)
        {
        }
    }
}