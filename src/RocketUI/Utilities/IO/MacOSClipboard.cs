using System;
using System.Runtime.InteropServices;

namespace RocketUI.Utilities.IO
{
	public class MacOSClipboard : IClipboardImplementation
	{
		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		static extern IntPtr objc_getClass(string className);

		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, string arg1);

		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		static extern IntPtr sel_registerName(string selectorName);
        
		IntPtr nsString = objc_getClass("NSString");
		IntPtr nsPasteboard = objc_getClass("NSPasteboard");
		IntPtr nsStringPboardType;
		IntPtr utfTextType;
		IntPtr generalPasteboard;
		IntPtr initWithUtf8Register = sel_registerName("initWithUTF8String:");
		IntPtr allocRegister = sel_registerName("alloc");
		IntPtr setStringRegister = sel_registerName("setString:forType:");
		IntPtr stringForTypeRegister = sel_registerName("stringForType:");
		IntPtr utf8Register = sel_registerName("UTF8String");
		IntPtr generalPasteboardRegister = sel_registerName("generalPasteboard");
		IntPtr clearContentsRegister = sel_registerName("clearContents");

		public MacOSClipboard()
		{
			utfTextType = objc_msgSend(objc_msgSend(nsString, allocRegister), initWithUtf8Register, "public.utf8-plain-text");
			nsStringPboardType = objc_msgSend(objc_msgSend(nsString, allocRegister), initWithUtf8Register, "NSStringPboardType");

			generalPasteboard = objc_msgSend(nsPasteboard, generalPasteboardRegister);
		}
        
		public void SetText(string value)
		{
			IntPtr str = default;
			try
			{
				str = objc_msgSend(objc_msgSend(nsString, allocRegister), initWithUtf8Register, value);
				objc_msgSend(generalPasteboard, clearContentsRegister);
				objc_msgSend(generalPasteboard, setStringRegister, str, utfTextType);
			}
			finally
			{
				if (str != default)
				{
					objc_msgSend(str, sel_registerName("release"));
				}
			}
		}

		public string GetText()
		{
			var ptr = objc_msgSend(generalPasteboard, stringForTypeRegister, nsStringPboardType);
			var charArray = objc_msgSend(ptr, utf8Register);
			return Marshal.PtrToStringAnsi(charArray);
		}
	}
}