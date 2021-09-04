using System.Drawing;
using RocketUI.Utilities.Helpers;

namespace RocketUI
{
    public class TextureElement : RocketElement
    {
        public TextureSlice2D Texture { get; set; }
        public TextureRepeatMode RepeatMode { get; set; } = TextureRepeatMode.Stretch;
        public TextureElement()
        {

        }

        protected override void OnDraw(GuiSpriteBatch graphics)
        {
            base.OnDraw(graphics);

            if (Texture != null)
            {
                graphics.FillRectangle(new Rectangle(RenderPosition.ToPoint(), Size), Texture, RepeatMode);
            }
        }
    }
}