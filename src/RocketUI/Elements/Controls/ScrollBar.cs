using System;
using Microsoft.Xna.Framework;
using RocketUI.Input;
using RocketUI.Utilities.Helpers;

namespace RocketUI
{
    public class ScrollOffsetValueChangedEventArgs : EventArgs
    {
        public int ScrollOffsetValue { get; }

        internal ScrollOffsetValueChangedEventArgs(int scrollOffsetValue)
        {
            ScrollOffsetValue = scrollOffsetValue;
        }
    }

    public class ScrollBar : RocketControl
    {
        public event EventHandler<ScrollOffsetValueChangedEventArgs> ScrollOffsetValueChanged;

        public Orientation Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                OnOrientationChanged();
            }
        }

        private Button _track;

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

        private Orientation  _orientation     = Orientation.Vertical;
        private int          _maxScrollOffset = 0;
        private int          _scrollOffsetValue;
        private int          _scrollBarSize = 5;
        private GuiTexture2D _thumbBackground;
        private GuiTexture2D _thumbHighlightBackground;

        public int ScrollBarSize
        {
            get => _scrollBarSize;
            set
            {
                if (value == _scrollBarSize) return;

                var prevValue = _maxScrollOffset;
                _scrollBarSize = value;
            }
        }

        public int ScrollButtonStep { get; set; } = 5;

        public int ScrollOffsetValue
        {
            get => _scrollOffsetValue;
            set
            {
                if (value == _scrollOffsetValue) return;

                var prevValue = _scrollOffsetValue;
                _scrollOffsetValue = MathHelpers.Clamp(value, 0, _maxScrollOffset);
                ScrollOffsetValueChanged?.Invoke(this, new ScrollOffsetValueChangedEventArgs(_scrollOffsetValue));

                OnScrollOffsetValueChanged(prevValue, _scrollOffsetValue);

                /*   Log.Info(
                       $"ScrollOffsetValue.Change {{ScrollOffsetValue=({prevValue} => {_scrollOffsetValue}), ScrollButtonStep={ScrollButtonStep}, MaxScrollOffset={MaxScrollOffset}}}");
                   Debug.WriteLine(
                       $"ScrollOffsetValue.Change {{ScrollOffsetValue=({prevValue} => {_scrollOffsetValue}), ScrollButtonStep={ScrollButtonStep}, MaxScrollOffset={MaxScrollOffset}}}");*/
            }
        }

        public int MaxScrollOffset
        {
            get => _maxScrollOffset;
            set
            {
                if (value == _maxScrollOffset) return;

                var val = _scrollOffsetValue;
                var pct = val / (float) _maxScrollOffset;

                _maxScrollOffset = value;
                ScrollOffsetValue = (int) (_maxScrollOffset * pct);
            }
        }

        public ScrollBar()
        {
            Background.Color = new Color(Color.Black, 0.2f);
            ThumbBackground = GuiTextures.ButtonDefault;
            ThumbHighlightBackground = GuiTextures.ButtonHover;

            Background.RepeatMode = TextureRepeatMode.NoScaleCenterSlice;
            ThumbBackground.RepeatMode = TextureRepeatMode.NoScaleCenterSlice;
            ThumbHighlightBackground.RepeatMode = TextureRepeatMode.NoScaleCenterSlice;

            MinWidth = 10;
            MinHeight = 10;

            Padding = Thickness.Zero;
            Margin = Thickness.Zero;

            AddChild(_track = new Button()
            {
                MinWidth = 10,
                MinHeight = 10,
                Margin = new Thickness(0, 0, 0, 0),

                AutoSizeMode = AutoSizeMode.None,
                Anchor = Alignment.Fill,

                CanFocus = false,
                CanHighlight = false,

                HighlightOutlineColor = Color.Yellow,
                HighlightOutlineThickness = new Thickness(2),

                Background = Color.MediumVioletRed,
                HighlightedBackground = ThumbHighlightBackground,
                FocusedBackground = GuiTextures.ScrollBarTrackFocused,
                DisabledBackground = GuiTextures.ScrollBarTrackDisabled
            });

            _track.Background.RepeatMode = TextureRepeatMode.NoScaleCenterSlice;
            _track.HighlightedBackground.RepeatMode = TextureRepeatMode.NoScaleCenterSlice;
            _track.FocusedBackground.RepeatMode = TextureRepeatMode.NoScaleCenterSlice;
            _track.DisabledBackground.RepeatMode = TextureRepeatMode.NoScaleCenterSlice;

            Orientation = Orientation.Vertical;
        }

        protected override void OnCursorMove(Point cursorPosition, Point previousCursorPosition, bool isCursorDown)
        {
            base.OnCursorMove(cursorPosition, previousCursorPosition, isCursorDown);

            if (isCursorDown)
                SetValueFromCursor(cursorPosition);
        }

        protected override void OnCursorPressed(Point cursorPosition, MouseButton button)
        {
            base.OnCursorPressed(cursorPosition, button);

            SetValueFromCursor(cursorPosition);
        }


        private void SetValueFromCursor(Point relativePosition)
        {
            var percentageClicked = Orientation == Orientation.Vertical
                ? (relativePosition.Y / (float) Size.Height)
                : (relativePosition.X / (float) Size.Width);

            ScrollOffsetValue = (int) Math.Round(MaxScrollOffset * percentageClicked);
        }

        protected override void OnInit(IGuiRenderer renderer)
        {
            base.OnInit(renderer);

            ThumbBackground.TryResolveTexture(renderer);
            ThumbHighlightBackground.TryResolveTexture(renderer);
        }

        private void OnScrollOffsetValueChanged(int prevValue, int scrollOffsetValue)
        {
            UpdateTrack();
        }

        private void OnOrientationChanged()
        {
            if (Orientation == Orientation.Vertical)
            {
                _track.Anchor = Alignment.FillX | Alignment.MinY;
            }
            else
            {
                _track.Anchor = Alignment.FillY | Alignment.MinX;
            }

            UpdateTrack();
        }

        private void UpdateTrack()
        {
            var containerSize = Orientation == Orientation.Vertical ? Size.Height : Size.Width;
            var trackSize     = containerSize;
            var maxOffset     = MaxScrollOffset;

            var contentSize = containerSize + maxOffset;

            if (containerSize == 0 || contentSize == 0) return;

            var visibleSizeAsPercentage = (containerSize / (double) contentSize);
            //var visibleSizeAsPercentage = 1d;
            if (visibleSizeAsPercentage >= 1.0d)
            {
                if (Orientation == Orientation.Vertical)
                {
                    _track.Height = trackSize;
                    _track.Margin = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    _track.Width = trackSize;
                    _track.Margin = new Thickness(0, 0, 0, 0);
                }

                _track.Enabled = false;
            }
            else
            {
                var size              = (int) Math.Floor(visibleSizeAsPercentage * trackSize);
                var positionOffsetPct = (_scrollOffsetValue / (double) maxOffset);
                var positionOffset    = (int) ((containerSize - size) * positionOffsetPct);

                positionOffset = MathHelpers.Clamp(positionOffset, 0, containerSize - size);

                if (Orientation == Orientation.Vertical)
                {
                    _track.Height = size;
                    _track.Width = Size.Width;
                    _track.Margin = new Thickness(0, positionOffset, 0, 0);
                }
                else
                {
                    _track.Width = size;
                    _track.Height = Size.Height;
                    _track.Margin = new Thickness(positionOffset, 0, 0, 0);
                }

                _track.Enabled = true;
            }
        }

        public void Increase() => Increase(ScrollButtonStep);

        public void Increase(int step)
        {
            ScrollOffsetValue += Math.Abs(step);
        }

        public void Decrease() => Decrease(ScrollButtonStep);

        public void Decrease(int step)
        {
            ScrollOffsetValue -= Math.Abs(step);
        }
    }
}