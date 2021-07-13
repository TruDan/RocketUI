using Microsoft.Xna.Framework;

namespace RocketUI
{
    public class Image : RocketElement
    {
        private GuiTexture2D _value;
        public bool ResizeToImageSize { get; set; }

        public GuiTexture2D Value
        {
            get => _value;
            set
            {
                _value = value;

                if (GuiRenderer != null)
                {
                    UpdateImage(GuiRenderer);
                }
            }
        }
        
        public Image(GuiTextures texture, TextureRepeatMode mode = TextureRepeatMode.Stretch)
        {
            _value = texture;
            _value.RepeatMode = mode;
            ResizeToImageSize = true;
        }

        public Image(NinePatchTexture2D background, TextureRepeatMode mode = TextureRepeatMode.Stretch)
        {
            _value = background;
            _value.RepeatMode = mode;
            ResizeToImageSize = true;
        }

        public Image(string filepath, TextureRepeatMode mode = TextureRepeatMode.Stretch)
        {
            _value = (GuiTexture2D) filepath;
            _value.RepeatMode = mode;
            ResizeToImageSize = true;
        }

        public Image()
        {
            
        }

        private void UpdateImage(IGuiRenderer renderer)
        {
            var val = _value;
            
            val.TryResolveTexture(renderer);

            if (ResizeToImageSize)
            {
                Width = val.ClipBounds.Width;
                Height = val.ClipBounds.Height;
            }
        }

        protected override void OnInit(IGuiRenderer renderer)
        {
            base.OnInit(renderer);

            UpdateImage(renderer);
        }

        protected override void GetPreferredSize(out Size size, out Size minSize, out Size maxSize)
        {
            base.GetPreferredSize(out size, out minSize, out maxSize);
            if (ResizeToImageSize)
            {
                var val = _value;
                if (val.HasValue)
                {
                    size = new Size(val.Width, val.Height);
                    size = Size.Clamp(size, minSize, maxSize);
                }
            }
        }

        /// <inheritdoc />
        protected override void OnDraw(GuiSpriteBatch graphics, GameTime gameTime)
        {
            base.OnDraw(graphics, gameTime);

            var position = RenderPosition;
            var value = _value;

            if (value.HasValue)
            {
                graphics.SpriteBatch.Draw(
                    value, position + RotationOrigin, value.Color.GetValueOrDefault(Color.White), Rotation, RotationOrigin,
                    value.Scale.GetValueOrDefault(Vector2.One));
            }
        }
    }
}
