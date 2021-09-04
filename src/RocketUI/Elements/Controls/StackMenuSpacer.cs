﻿using System.Numerics;
using RocketUI.Serialization;

namespace RocketUI
{
	public class StackMenuSpacer : RocketElement
	{
		public StackMenuSpacer()
		{
			Margin = new Thickness(5, 5, 5, 5);
		}

		protected override void OnDraw(GuiSpriteBatch graphics)
		{
			//	base.OnDraw(graphics, gameTime);
			graphics.DrawLine(new Vector2(RenderBounds.Left, RenderBounds.Height / 2f), RenderBounds.Width, 0f, Colors.DarkGray, 1);
		}
	}
}
