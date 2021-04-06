using System;

namespace RocketUI.Events
{
	public class GuiFocusChangedEventArgs : EventArgs
	{
		public IGuiControl FocusedElement { get; }
		public GuiFocusChangedEventArgs(IGuiControl focusedElement)
		{
			FocusedElement = focusedElement;
		}
	}
}