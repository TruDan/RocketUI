using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Portable.Xaml.Markup;
using RocketUI.Attributes;
using RocketUI.Utilities.Extensions;

namespace RocketUI
{
    public class TextClickedEvent : EventArgs
    {
        public Uri ClickedText;
    }

    [ContentProperty(nameof(Text))]
    public class TextElement : RocketElement //GuiControl
    {
        public static readonly Color DefaultTextBackgroundColor = new Color(Color.Black, 0.6f);

        private string   _text;
        private float    _textOpacity = 1f;
        private Vector2  _scale       = Vector2.One;
        private Vector2? _rotationOrigin;
        private IFont    _font;

        [DebuggerVisible]
        public TextAlignment TextAlignment
        {
            get => _textAlignment;
            set
            {
                _textAlignment = value;
                OnTextUpdated();
            }
        }

        [DebuggerVisible]
        public override Vector2 RotationOrigin
        {
            get { return _rotationOrigin.HasValue ? _rotationOrigin.Value : new Vector2(-0.5f, -0.5f); }
            set { _rotationOrigin = value; }
        }

        private string _translationKey;

        [DebuggerVisible]
        public string TranslationKey
        {
            get => _translationKey;
            set
            {
                _translationKey = value;
                OnTranslationKeyUpdated();
            }
        }

        [DebuggerVisible]
        public string Text
        {
            get => _text;
            set
            {
                _text = value ?? string.Empty;
                OnTextUpdated();
            }
        }

        [DebuggerVisible]
        public Color TextColor { get; set; } = Color.White;

        [DebuggerVisible]
        public float TextOpacity
        {
            get => _textOpacity;
            set => _textOpacity = value;
        }

        [DebuggerVisible]
        public float Scale
        {
            get => _scale.X;
            set
            {
                _scale = new Vector2(value);
                OnTextUpdated();
            }
        }

        private FontStyle _fontStyle;

        [DebuggerVisible]
        public FontStyle FontStyle
        {
            get => _fontStyle;
            set => _fontStyle = value;
        }

        public bool HasShadow { get; set; } = true;

        [DebuggerVisible]
        public IFont Font
        {
            get => _font;
            set
            {
                _font = value;
                OnTextUpdated();
            }
        }

        private bool _fixedSize = false;

        [DebuggerVisible]
        public bool HasFixedSize
        {
            get { return _fixedSize; }
            set { _fixedSize = value; }
        }

        private string _renderText = String.Empty;

        public EventHandler<TextClickedEvent> OnLinkClicked;

        //public void AddClickable()

        /*public class ClickableElement
        {
            public Rectangle Area { get; set; } 
            public Action<GuiTextElement, string> ClickAction { get; set; }
            public string Text { get; set; }
        }*/

        //private List<ClickableElement> ClickableElements = new List<ClickableElement>();

        private bool          _wrap = false;
        private TextAlignment _textAlignment;

        public bool Wrap
        {
            get => _wrap;
            set
            {
                _wrap = value;
                OnTextUpdated();
            }
        }

        public TextElement(string text, bool hasBackground = false) : this(hasBackground)
        {
            Text = text;
        }

        private bool HasBackground { get; }

        public TextElement(bool hasBackground = false)
        {
            HasBackground = hasBackground;
            if (hasBackground)
            {
                BackgroundOverlay = DefaultTextBackgroundColor;
            }
            else
            {
                Background = null;
                BackgroundOverlay = null;
            }

            Margin = new Thickness(2);
        }

        public TextElement()
        {
        }

        protected override void OnInit(IGuiRenderer renderer)
        {
            base.OnInit(renderer);

            Font = renderer.Font;

            OnTranslationKeyUpdated();
        }

        protected override void OnDraw(GuiSpriteBatch graphics, GameTime gameTime)
        {
            var text = _renderText;

            if (!string.IsNullOrWhiteSpace(text))
            {
                //base.OnDraw(graphics, gameTime);

                /*var size = Font.MeasureString(text, Scale);
                while (size.X > RenderBounds.Width && text.Length >= 1)
                {
                    text = text.Substring(0, text.Length - 1);
                    size = Font.MeasureString(text, Scale);
                }*/

                // graphics.FillRectangle(new Rectangle(RenderPosition.ToPoint(), Size.ToPoint()), BackgroundOverlay);
                //if (HasBackground && BackgroundOverlay.HasValue && BackgroundOverlay.Color.HasValue)
                // {
                //   graphics.SpriteBatch.FillRectangle(new Rectangle(RenderPosition.ToPoint(), Size /*GetSize(text, _scale).ToPoint()*/), BackgroundOverlay.Color.Value);
                // }
                var renderPosition = RenderPosition;

                foreach (var line in text.Split('\n'))
                {
                    var size     = Font.MeasureString(line, _scale);
                    var position = renderPosition;
                    if ((TextAlignment & TextAlignment.Center) != 0)
                    {
                        position.X = RenderBounds.Center.X - (size.X / 2f);
                    }
                    else if ((TextAlignment & TextAlignment.Right) != 0)
                    {
                        position.X = RenderBounds.Right - size.X;
                    }

                    if (HasBackground && BackgroundOverlay.HasValue && BackgroundOverlay.Color.HasValue)
                    {
                        var p = position.ToPoint();
                        var s = size.ToPoint();

                        graphics.SpriteBatch.FillRectangle(
                            new Rectangle(p.X - 2, p.Y - 2, s.X + 4, s.Y + 2),
                            BackgroundOverlay.Color.Value);
                    }

                    Font.DrawString(
                        graphics.SpriteBatch, line, position, TextColor, FontStyle, _scale, TextOpacity, Rotation,
                        RotationOrigin);

                    renderPosition.Y += size.Y;
                }
            }
        }


        private Vector2 GetSize(string text, Vector2 scale)
        {
            var size = Font?.MeasureString(text, scale) ?? Vector2.Zero;
            if (FontStyle.HasFlag(FontStyle.Bold))
            {
                size.X += text.Length * scale.X;
            }

            return size;
        }

        private void OnTranslationKeyUpdated()
        {
            if (!string.IsNullOrEmpty(TranslationKey))
            {
                Text = GuiRenderer?.GetTranslation(TranslationKey);
            }
        }

        protected override void GetPreferredSize(out Size size, out Size minSize, out Size maxSize)
        {
            base.GetPreferredSize(out size, out minSize, out maxSize);
            var scale = new Vector2(Scale, Scale);

            string text = _renderText;

            var textSize = GetSize(text, scale);

            size = new Size((int) Math.Ceiling(textSize.X), (int) Math.Ceiling(textSize.Y));
            minSize = size;
            maxSize = size;
        }

        //private static Regex LinkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        protected virtual void OnTextUpdated()
        {
            string text = _text;
            //if (Font != null && !string.IsNullOrWhiteSpace(text))
            if (string.IsNullOrWhiteSpace(text))
            {
                _renderText = string.Empty;
                Width = 0;
                Height = 0;

                if (!HasFixedSize)
                {
                    // InvalidateLayout();
                }
                else
                {
                }
            }
            else
            {
                //var scale = new Vector2(Scale, Scale);

                if (_wrap)
                {
                    StringBuilder sb = new StringBuilder();
                    var scale = new Vector2(Scale, Scale);
                    var textSize = GetSize(text, scale);

                    var maxWidth = (MaxWidth - (Margin.Horizontal + Padding.Horizontal));
                    if (textSize.X > maxWidth)
                    {
                        var lineWidth = textSize.X;
                        var splitText = new LinkedList<string>(text.Split(' '));
                        do
                        {
                            var str = splitText.First.Value + " ";
                            var stringWidth = GetSize(str, scale);

                            if (lineWidth + stringWidth.X < maxWidth)
                            {
                                sb.Append(str);
                                lineWidth += stringWidth.X;
                            }
                            else
                            {
                                sb.AppendLine();
                                sb.Append(str);

                                lineWidth = stringWidth.X;
                            }
                            
                            splitText.RemoveFirst();
                        } while (splitText.Count > 0);

                        text = sb.ToString();
                    }
                }

                _renderText = text;

                GetPreferredSize(out var size, out var minSize, out var maxSize);
                Width = Math.Max(Math.Min(size.Width, maxSize.Width), minSize.Width); // size.Width;
                Height = Math.Max(Math.Min(size.Height, maxSize.Height), minSize.Height); // size.Height;
            }
        }

        //protected override void OnCursorPressed(Point cursorPosition, MouseButton button)
        //	{
        //	base.OnCursorPressed(cursorPosition, button);
        /*	foreach (var c in ClickableElements.ToArray())
            {
                if (c.Area.Contains(cursorPosition))
                {
                    OnLinkClicked?.Invoke(this, new GuiTextClickedEvent() { ClickedText = new Uri(c.Text) });
                    //c.ClickAction?.Invoke(this, c.Text);
                }
            }*/
        //}
    }
}