using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RocketUI.Input;

namespace RocketUI
{
    public class Button : RocketControl, IGuiButton
    {
        public TextAlignment TextAlignment
        {
            get => TextElement.TextAlignment;
            set => TextElement.TextAlignment = value;
        }
        
        public string Text
        {
            get => TextElement.Text;
            set => TextElement.Text = value;
        }

        public string TranslationKey
        {
            get => TextElement.TranslationKey;
            set => TextElement.TranslationKey = value;
        }

        protected TextElement TextElement { get; }
        public    Action         Action      { get; set; }

        public virtual Color? DisabledColor  { get; set; } = Color.DarkGray;
        public virtual Color  DefaultColor   { get; set; } = Color.White;
        public virtual Color? HighlightColor { get; set; } = Color.Yellow;
        public virtual Color? FocusColor     { get; set; } = Color.White;

        public GuiSound HighlightSound;
        public GuiSound ClickSound;
        
        public Button(Action action = null) : this(string.Empty, action)
        {
        }

        public Button(string text, Action action = null, bool isTranslationKey = false)
        {
            Background = GuiTextures.ButtonDefault;
            DisabledBackground = GuiTextures.ButtonDisabled;
            HighlightedBackground = GuiTextures.ButtonHover;
            FocusedBackground = GuiTextures.ButtonFocused;

            Background.RepeatMode = TextureRepeatMode.NoScaleCenterSlice;
            DisabledBackground.RepeatMode = TextureRepeatMode.NoScaleCenterSlice;
            HighlightedBackground.RepeatMode = TextureRepeatMode.NoScaleCenterSlice;
            FocusedBackground.RepeatMode = TextureRepeatMode.NoScaleCenterSlice;

            ClickSound = GuiSoundEffects.ButtonClick;
            HighlightSound = GuiSoundEffects.ButtonHighlight;

            Action = action;
            MinHeight = 20;
            MinWidth = 20;

            //MaxHeight = 22;
            //MaxWidth = 200;
            Padding = new Thickness(5, 5);
            Margin = new Thickness(2);
            
            

            AddChild(TextElement = new TextElement()
            {
                Margin = new Thickness(5),
                Anchor = Alignment.MiddleCenter,
                Text = text,
                TextColor = Color.White,
                TextOpacity = 0.875f,
                FontStyle = FontStyle.DropShadow,
                //Enabled = false,
                //CanFocus = false
            });

            if (isTranslationKey)
            {
                TextElement.TranslationKey = text;
            }
        }

        public Button() : this(null)
        {
            
        }
        
        protected override void OnInit(IGuiRenderer renderer)
        {
            base.OnInit(renderer);
            ClickSound.TryResolve(renderer);
            HighlightSound.TryResolve(renderer);
        }

        protected override void OnHighlightActivate()
        {
            base.OnHighlightActivate();
            if(Enabled && HighlightColor.HasValue)
                TextElement.TextColor = HighlightColor.Value;
            HighlightSound.Play();
        }

        protected override void OnHighlightDeactivate()
        {
            base.OnHighlightDeactivate();
            if(Enabled)
                TextElement.TextColor = DefaultColor;
        }

        protected override void OnCursorMove(Point cursorPosition, Point previousCursorPosition, bool isCursorDown)
        {
            base.OnCursorMove(cursorPosition, previousCursorPosition, isCursorDown);
        }

        protected override void OnFocusActivate()
        {
            base.OnFocusActivate();
            if (Enabled && FocusColor.HasValue)
                TextElement.TextColor = FocusColor.Value;
            ClickSound.Play();
            Action?.Invoke();
        }

        protected override void OnFocusDeactivate()
        {
            base.OnFocusDeactivate();
            if (Enabled)
            {
                if (Highlighted && HighlightColor.HasValue)
                    TextElement.TextColor = HighlightColor.Value;
                else
                    TextElement.TextColor = DefaultColor;
            }
        }

        protected override void OnCursorPressed(Point cursorPosition, MouseButton button)
        {
            //Focus();
            //Action?.Invoke();
        }

        protected override void OnDraw(GuiSpriteBatch graphics, GameTime gameTime)
        {
            base.OnDraw(graphics, gameTime);
        }

        protected override void OnEnabledChanged()
        {
            if (!Enabled)
            {
                if(DisabledColor.HasValue)
                    TextElement.TextColor = DisabledColor.Value;
                // TextElement.TextOpacity = 0.3f;
            }
            else
            {
                TextElement.TextColor = DefaultColor;
                //TextElement.TextOpacity = 1f;
            }
        }

        protected override bool OnKeyInput(char character, Keys key)
        {
            if (key == Keys.Enter)
            {
                Action?.Invoke();
                return true;
            }

            return base.OnKeyInput(character, key);
        }

    }
}