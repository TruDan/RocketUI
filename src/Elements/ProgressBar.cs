﻿using System;
using Microsoft.Xna.Framework;

namespace RocketUI
{
    public class ProgressBar : RocketElement
    {

        public float MinValue { get; set; } = 0;
        public float Value    { get; set; } = 0;
        public float MaxValue { get; set; } = 100;

        public float Percent => Math.Max(0, Math.Min(1, Value / (float)Math.Abs(MaxValue - MinValue)));

        private int _spriteSheetSegmentWidth = 3;
        public ITexture2D Highlight { get; set; }

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

            _spriteSheetSegmentWidth = (int)Math.Round(b.Width / 4f);
	        var bgBounds = new Rectangle(b.X, b.Y, _spriteSheetSegmentWidth * 3, b.Height);

            Background = new NinePatchTexture2D(texture.Texture.Slice(bgBounds), _spriteSheetSegmentWidth);

	        Highlight = texture.Texture.Slice(new Rectangle(b.X + _spriteSheetSegmentWidth * 3, b.Y, _spriteSheetSegmentWidth, b.Height));
        }

	    protected override void OnUpdate(GameTime gameTime)
	    {
		    base.OnUpdate(gameTime);

		}

	    protected override void OnDraw(GuiSpriteBatch graphics, GameTime gameTime)
        {
            var bounds = RenderBounds;

            var fillWidth = bounds.Width - 2 * _spriteSheetSegmentWidth;

            base.OnDraw(graphics, gameTime);

            bounds = new Rectangle(bounds.X + _spriteSheetSegmentWidth, bounds.Y, Math.Max(1, (int)(fillWidth * Percent)), bounds.Height);
            graphics.FillRectangle(bounds, Highlight);

	       //	args.SpriteBatch.DrawString(FontRenderer, Text, RenderBounds.Center.ToVector2() - (TextSize / 2f), Color.Black, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);
        }
    }
}
