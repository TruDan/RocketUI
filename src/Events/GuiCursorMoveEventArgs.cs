using Microsoft.Xna.Framework;
using RocketUI.Input;

namespace RocketUI.Events
{
    public class GuiCursorMoveEventArgs : GuiCursorEventArgs
    {
		public Point PreviousCursorPosition { get; }

		public bool IsCursorDown { get; }

		internal GuiCursorMoveEventArgs(Point cursorPosition, Point previousCursorPosition, bool isCursorDown, MouseButton button) : base(cursorPosition, button)
		{
			PreviousCursorPosition = previousCursorPosition;
			IsCursorDown = isCursorDown;
		}
    }
}
