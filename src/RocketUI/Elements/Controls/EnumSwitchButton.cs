using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RocketUI.Input;

namespace RocketUI
{
    public class EnumSwitchButton<TEnum> : Button, IValuedControl<TEnum> where TEnum : Enum
    {
        public static readonly ValueFormatter<TEnum> DefaultDisplayFormat = "{0}";
        
        public event EventHandler<TEnum> ValueChanged;
        
        private TEnum[] Values { get; set; }
        private int CurrentIndex { get; set; } = 0;
        public ValueFormatter<TEnum> DisplayFormat { get; set; } = DefaultDisplayFormat;

        public TEnum Value
        {
            get { return Values[CurrentIndex]; }
            set
            {
                for (int i = 0; i < Values.Length; i++)
                {
                    if (Values[i].Equals(value))
                    {
                        CurrentIndex = i;
                        Text = DisplayFormat.FormatValue(value) ?? string.Empty;
            
                        ValueChanged?.Invoke(this, value);
                        return;
                    }
                }
                
                
            }
        }

        public EnumSwitchButton()
        {
            var values = Enum.GetValues(typeof(TEnum));

            List<TEnum> v = new List<TEnum>();
            foreach (var value in values)
            {
                v.Add((TEnum) value);
            }

            Values = v.ToArray();

            Text = DisplayFormat?.FormatValue(Values[CurrentIndex]) ?? string.Empty;
        }

        protected override void OnCursorPressed(Point cursorPosition, MouseButton button)
        {
            CurrentIndex++;
            if (CurrentIndex >= Values.Length)
            {
                CurrentIndex = 0;
            }

            var value = Values[CurrentIndex];
            Text = DisplayFormat.FormatValue(value) ?? string.Empty;
            
            ValueChanged?.Invoke(this, value);
        }
    }
}