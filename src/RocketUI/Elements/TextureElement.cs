#if STRIDE
using Stride.Core.Mathematics;
#else
using Microsoft.Xna.Framework;
#endif

namespace RocketUI
{
    public class TextureElement : RocketElement
    {
        public TextureSlice2D Texture { get; set; }
        public TextureRepeatMode RepeatMode { get; set; } = TextureRepeatMode.Stretch;
        public TextureElement()
        {

        }

        protected override void OnDraw(GuiSpriteBatch graphics, GameTime gameTime)
        {
            base.OnDraw(graphics, gameTime);

            if (Texture != null)
            {
                graphics.FillRectangle(new Rectangle(RenderPosition.ToPoint(), Size), Texture, RepeatMode);
            }
        }
    }
}