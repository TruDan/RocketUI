namespace RocketUI
{
    public class Image : RocketElement
    {
        public bool ResizeToImageSize { get; set; }
        
        public Image(GuiTextures texture, TextureRepeatMode mode = TextureRepeatMode.Stretch)
        {
            Background = texture;
            Background.RepeatMode = mode;
            ResizeToImageSize = true;
        }

        public Image(NinePatchTexture2D background, TextureRepeatMode mode = TextureRepeatMode.Stretch)
        {
            Background = background;
            Background.RepeatMode = mode;
            ResizeToImageSize = true;
        }

        public Image(string filepath, TextureRepeatMode mode = TextureRepeatMode.Stretch)
        {
            Background = (GuiTexture2D) filepath;
            Background.RepeatMode = mode;
            ResizeToImageSize = true;
        }

        protected override void OnInit(IGuiRenderer renderer)
        {
            base.OnInit(renderer);

            if (ResizeToImageSize)
            {
                Width = Background.ClipBounds.Width;
                Height = Background.ClipBounds.Height;
            }
        }

        protected override void GetPreferredSize(out Size size, out Size minSize, out Size maxSize)
        {
            base.GetPreferredSize(out size, out minSize, out maxSize);
            if (ResizeToImageSize)
            {
                if (Background.HasValue)
                {
                    size = new Size(Background.Width, Background.Height);
                    size = Size.Clamp(size, minSize, maxSize);
                }
            }
        }
    }
}
