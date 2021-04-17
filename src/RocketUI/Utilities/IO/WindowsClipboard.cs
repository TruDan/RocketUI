using System;
using System.Runtime.InteropServices;

namespace RocketUI.Utilities.IO
{
	public class WindowsClipboard : IClipboardImplementation
	{
		[DllImport("user32.dll")]
		internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

		[DllImport("user32.dll")]
		internal static extern bool CloseClipboard();

		[DllImport("user32.dll")]
		internal static extern bool SetClipboardData(uint uFormat, IntPtr data);
        
		[DllImport("user32.dll")]
		static extern IntPtr GetClipboardData(uint uFormat);

		[DllImport("kernel32.dll")]
		static extern IntPtr GlobalLock(IntPtr hMem);
        
		[DllImport("kernel32.dll")]
		static extern bool GlobalUnlock(IntPtr hMem);
        
		[DllImport("kernel32.dll")]
		private static extern UIntPtr GlobalSize(IntPtr hMem);
        
		[DllImport("user32.dll")]
		static extern bool IsClipboardFormatAvailable(uint format);
        
		const uint CF_UNICODETEXT = 13;
        
		public void SetText(string value)
		{
			OpenClipboard(IntPtr.Zero);

			SetClipboardData(CF_UNICODETEXT, Marshal.StringToHGlobalUni(value));
            
			CloseClipboard();
		}

		public string GetText()
		{
			if (!IsClipboardFormatAvailable(CF_UNICODETEXT))
				return null;
            
			if (!OpenClipboard(IntPtr.Zero))
				return null;

			string data = null;
			var hGlobal = GetClipboardData(CF_UNICODETEXT);
			if (hGlobal != IntPtr.Zero)
			{
				var lpwcstr = GlobalLock(hGlobal);
				if (lpwcstr != IntPtr.Zero)
				{
					data = Marshal.PtrToStringUni(lpwcstr);
					GlobalUnlock(lpwcstr);
				}
			}
			CloseClipboard();

			return data;
		}
	}
}