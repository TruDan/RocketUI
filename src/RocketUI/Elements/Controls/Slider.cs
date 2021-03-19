using System;
using Microsoft.Xna.Framework;
using RocketUI.Input;
using RocketUI.Utilities.Helpers;

namespace RocketUI
{
    public class Slider : RocketControl, IValuedControl<double>
    {
		public static readonly ValueFormatter<double> DefaultDisplayFormat = "{0:0.#}";
        
        public event EventHandler<double> ValueChanged; 

        public double MinValue { get; set; } = 0.0d;
        public double MaxValue { get; set; } = 100.0d;
        private double _value = 0.0d;

        public double Value
        {
            get => _value;
            set
            {
                var proposedValue = value;
                if (StepInterval != 0d)
                {
                    proposedValue = MathHelpers.RoundToNearestInterval(proposedValue, StepInterval);
                }

                if (proposedValue != _value)
                {
                    _value = proposedValue;
                    ValueChanged?.Invoke(this, _value);
                }
            }
        }

        public double StepInterval { get; set; } = 1.0d;

        public GuiTexture2D ThumbBackground
        {
            get
            {
                if (_thumbBackground == null)
                    _thumbBackground = new GuiTexture2D();
                return _thumbBackground;
            }
            set => _thumbBackground = value;
        }

        public GuiTexture2D ThumbHighlightBackground
        {
            get
            {
                if (_thumbHighlightBackground == null)
                    _thumbHighlightBackground = new GuiTexture2D();
                return _thumbHighlightBackground;
            }
            set => _thumbHighlightBackground = value;
        }

        public GuiTexture2D ThumbDisabledBackground
        {
            get
            {
                if (_thumbDisabledBackground == null)
                    _thumbDisabledBackground = new GuiTexture2D();
                return _thumbDisabledBackground;
            }
            set => _thumbDisabledBackground = value;
        }

        public int ThumbWidth
        {
            get => _thumbWidth;
            set { _thumbWidth = value; }
        }

        public TextElement Label { get; private set; }
        public ValueFormatter<double> DisplayFormat { get; set; } = DefaultDisplayFormat;

        private double _thumbOffsetX;
        private int _thumbWidth = 10;

        [Obsolete("Use DisplayFormat property instead.", true)]
        public Func<double, string> ValueFormatter { get; set; } = null;
        
        private Color _foregroundColor = Color.White;
        private Color _originalForegroundColor = Color.White;
        
        public Color ForegroundColor
        {
            get
            {
                return _foregroundColor;
            }
            set
            {
                _originalForegroundColor = value;
                FixForegroundColor(value);
            }
        }

        void FixForegroundColor(Color color)
        {
            _foregroundColor = color;
            var c = color;

            if (!Enabled)
            {
                c = c.Darken(0.5f);
            }
            
            _foregroundColor = c;
        }

        public Slider()
        {
            Background = GuiTextures.ButtonDisabled;
            ThumbBackground = GuiTextures.ButtonDefault;
            ThumbHighlightBackground = GuiTextures.ButtonHover;
            ThumbDisabledBackground = GuiTextures.ButtonDisabled;
            
            Background.RepeatMode = TextureRepeatMode.Stretch;
            ThumbBackground.RepeatMode = TextureRepeatMode.NoScaleCenterSlice;
            ThumbHighlightBackground.RepeatMode = TextureRepeatMode.NoScaleCenterSlice;
            ThumbDisabledBackground.RepeatMode = TextureRepeatMode.NoScaleCenterSlice;

            MinWidth = 20;
            MinHeight = 20;

            MaxHeight = 22;
           // MaxWidth  = 200;
            //Padding = new Thickness(5, 5);
            Margin = new Thickness(2);
            Height = 20;

            // Background.RepeatMode = TextureRepeatMode.NoScaleCenterSlice;

            AddChild(Label = new AutoUpdatingTextElement(() => DisplayFormat?.FormatValue(Value) ?? string.Empty)
            {
                Margin      =  Thickness.Zero,
                Anchor      = Alignment.MiddleCenter,
                TextColor   = _foregroundColor,
                FontStyle   = FontStyle.DropShadow,
                //Enabled = false,
              //  CanFocus = false
            });
        }

        protected override void OnEnabledChanged()
        {
            base.OnEnabledChanged();

            FixForegroundColor(_originalForegroundColor);
        }

        protected override void OnInit(IGuiRenderer renderer)
        {
            base.OnInit(renderer);
            
            FixForegroundColor(_originalForegroundColor);
            
            ThumbBackground.TryResolveTexture(renderer);
            ThumbHighlightBackground.TryResolveTexture(renderer);
            ThumbDisabledBackground.TryResolveTexture(renderer);
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            var val = MathHelper.Clamp((float)Value, (float)MinValue, (float)MaxValue);
            val = MathHelpers.RoundToNearestInterval(val, (float)StepInterval);


            var diff = MathHelpers.RoundToNearestInterval(Math.Abs(MinValue - MaxValue), StepInterval);
            
            _thumbOffsetX = ((RenderBounds.Width - ThumbWidth) / (double) diff) * (val - MinValue);
        }

        protected override void OnDraw(GuiSpriteBatch graphics, GameTime gameTime)
        {
            base.OnDraw(graphics, gameTime);

            var bounds = new Rectangle((int) (RenderPosition.X + _thumbOffsetX), (int) RenderPosition.Y, ThumbWidth,
                                       RenderSize.Height);
            
            graphics.FillRectangle(bounds, Enabled ? (Highlighted ? ThumbHighlightBackground : ThumbBackground) : ThumbDisabledBackground);
        }

        private void SetValueFromCursor(Point relativePosition)
        {
            var halfThumb = _thumbWidth / 2f;

            float percentageClicked = 0f;
            if (relativePosition.X <= halfThumb)
                percentageClicked = 0f;
            else if (relativePosition.X >= (RenderBounds.Width - halfThumb))
                percentageClicked = 1f;
            else 
                percentageClicked = (relativePosition.X - halfThumb) / (float)(RenderBounds.Width - _thumbWidth);

            var diff = Math.Abs(MinValue - MaxValue);
            Value = MinValue + (diff * percentageClicked);
        }

        protected override void OnHighlightActivate()
        {
            base.OnHighlightActivate();

            Label.TextColor = Color.Yellow;
        }

        protected override void OnHighlightDeactivate()
        {
            base.OnHighlightDeactivate();

            Label.TextColor = Color.White;
        }
        
        protected override void OnCursorPressed(Point cursorPosition, MouseButton button)
        {
            SetValueFromCursor(cursorPosition);
        }

        protected override void OnCursorDown(Point cursorPosition)
        {
            if (Focused)
                _cursorDown = true;
            
            base.OnCursorDown(cursorPosition);
        }

        protected override void OnCursorUp(Point cursorPosition)
        {
            _cursorDown = false;
            base.OnCursorUp(cursorPosition);
        }

        private bool         _cursorDown = false;
        private GuiTexture2D _thumbBackground;
        private GuiTexture2D _thumbDisabledBackground;
        private GuiTexture2D _thumbHighlightBackground;

        protected override void OnCursorLeave(Point cursorPosition)
        {
            _cursorDown = false;
            base.OnCursorLeave(cursorPosition);
        }

        protected override void OnCursorEnter(Point cursorPosition)
        {
            base.OnCursorEnter(cursorPosition);
        }

        protected override void OnCursorMove(Point relativeNewPosition, Point relativeOldPosition, bool isCursorDown)
        {
            if (!isCursorDown && _cursorDown) _cursorDown = false;
            
            if (isCursorDown && _cursorDown)
            {
                SetValueFromCursor(relativeNewPosition);
            }
        }
    }
}
