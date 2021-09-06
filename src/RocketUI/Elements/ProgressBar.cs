using System;
using System.Drawing;

namespace RocketUI
{
    public class ProgressBar : RocketElement
    {

        public float MinValue { get; set; } = 0;
        public float Value    { get; set; } = 0;
        public float MaxValue { get; set; } = 100;

        public float Percent => Math.Max(0, Math.Min(1, Value / (float)Math.Abs(MaxValue - MinValue)));

        public int        SpriteSheetSegmentWidth { get; private set; } = 3;
        public ITexture2D Highlight                { get; set; }

		public ProgressBar()
		{
			Height = 15;
			Width = 100;
			MinHeight = 15;
			MinWidth = 100;

		}

        protected override void OnInit(IGuiRenderer renderer)
        {
	        base.OnInit(renderer);
	        
			var texture = renderer.GetTexture(GuiTextures.ProgressBar);
            var b = texture.ClipBounds;

            SpriteSheetSegmentWidth = (int)Math.Round(b.Width / 4f);
	        var bgBounds = new Rectangle(b.X, b.Y, SpriteSheetSegmentWidth * 3, b.Height);

            Background = new NinePatchTexture2D(texture.Texture.Slice(bgBounds), SpriteSheetSegmentWidth);

	        Highlight = texture.Texture.Slice(new Rectangle(b.X + SpriteSheetSegmentWidth * 3, b.Y, SpriteSheetSegmentWidth, b.Height));
        }

	    protected override void OnUpdate()
	    {
		    base.OnUpdate();

		}
    }
}
