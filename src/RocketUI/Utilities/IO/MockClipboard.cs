using System;

namespace RocketUI.Utilities.IO
{
	public class MockClipboard : IClipboardImplementation
	{
		public void SetText(string value)
		{
            
		}

		public string GetText()
		{
			return String.Empty;
		}
	}
}