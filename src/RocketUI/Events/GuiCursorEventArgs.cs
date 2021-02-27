using System;
using Microsoft.Xna.Framework;
using RocketUI.Input;

namespace RocketUI.Events
{
    public class GuiCursorEventArgs : EventArgs
    {
		public Point CursorPosition { get; }
		public MouseButton Button { get; }

		internal GuiCursorEventArgs(Point cursorPosition, MouseButton button)
		{
			CursorPosition = cursorPosition;
			Button = button;
		}
    }
}
