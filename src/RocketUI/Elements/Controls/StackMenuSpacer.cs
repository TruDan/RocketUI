#if STRIDE
using Stride.Core.Mathematics;
#else
using Microsoft.Xna.Framework;
#endif

namespace RocketUI
{
	public class StackMenuSpacer : RocketElement
	{
		public StackMenuSpacer()
		{
			Margin = new Thickness(5, 5, 5, 5);
		}

		protected override void OnDraw(GuiSpriteBatch graphics, GameTime gameTime)
		{
			//	base.OnDraw(graphics, gameTime);
			graphics.DrawLine(new Vector2(RenderBounds.Left, RenderBounds.Height / 2f), RenderBounds.Width, 0f, Color.DarkGray, 1);
		}
	}
}
