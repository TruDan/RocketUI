using System;
using Microsoft.Xna.Framework;
using RocketUI.Input;

namespace RocketUI
{
    public class ToggleButton : Button, IValuedControl<bool>
    {
        public static readonly ValueFormatter<bool> DefaultDisplayFormat = "{0}";

        public event EventHandler<bool> ValueChanged;
        private bool                    _value;

        public GuiTexture2D CheckedBackground
        {
            get
            {
                if (_checkedBackground == null)
                    _checkedBackground = new GuiTexture2D();
                return _checkedBackground;
            }
            set => _checkedBackground = value;
        }

        public override Color DefaultColor
        {
            get
            {
                if (Checked && CheckedColor.HasValue) 
                    return CheckedColor.Value;
                return _defaultColor;
            }
            set => _defaultColor = value;
        }

        public virtual  Color? CheckedColor { get; set; } = Color.Yellow;

        public virtual Color     CheckedOutlineColor     { get; set; } = new Color(Color.White, 0.75f);
        public virtual Thickness CheckedOutlineThickness { get; set; } = Thickness.Zero;

        public bool Checked
        {
            get => Value;
            set => Value = value;
        }

        public bool Value
        {
            get => _value;
            set
            {
                if (value != _value)
                {
                    _value = value;
                    ValueChanged?.Invoke(this, _value);
                    OnCheckedChanged();
                }

                Text = DisplayFormat?.FormatValue(value) ?? string.Empty;
            }
        }

        private ValueFormatter<bool> _formatter = DefaultDisplayFormat;
        private Color                _defaultColor;
        private GuiTexture2D         _checkedBackground;

        public ValueFormatter<bool> DisplayFormat
        {
            get { return _formatter; }
            set
            {
                _formatter = value;
                Text = _formatter?.FormatValue(_value) ?? string.Empty;
            }
        }

        public ToggleButton() : base()
        {
        }

        public ToggleButton(string text) : base(text)
        {
            DisplayFormat = new ValueFormatter<bool>(b => text);
        }
        
        public ToggleButton(string text, Action<bool> onValueChanged) : this(text)
        {
            ValueChanged += (sender, value) => onValueChanged(value);
        }

        protected virtual void OnCheckedChanged()
        {
            if (Checked)
            {
                if (CheckedColor.HasValue)
                    TextElement.TextColor = CheckedColor.Value;
            }
            else
                TextElement.TextColor = DefaultColor;
        }

        protected override void OnHighlightDeactivate()
        {
            base.OnHighlightDeactivate();

            if (Checked)
            {
                OnCheckedChanged();
            }
        }

        protected override void OnFocusDeactivate()
        {
            base.OnFocusDeactivate();

            if (Checked)
            {
                OnCheckedChanged();
            }
        }

        protected override void OnCursorPressed(Point cursorPosition, MouseButton button)
        {
            Value = !_value;
        }

        protected override void OnInit(IGuiRenderer renderer)
        {
            base.OnInit(renderer);
            CheckedBackground.TryResolveTexture(renderer);
        }

        protected override void OnDraw(GuiSpriteBatch graphics, GameTime gameTime)
        {
            base.OnDraw(graphics, gameTime);

            if (Enabled)
            {
                if (Value)
                {
                    graphics.FillRectangle(RenderBounds, CheckedBackground);

                    if (CheckedOutlineThickness != Thickness.Zero)
                    {
                        graphics.DrawRectangle(RenderBounds, FocusOutlineColor, FocusOutlineThickness, true);
                    }
                }
            }
        }
    }
}