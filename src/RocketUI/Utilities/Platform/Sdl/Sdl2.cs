using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace RocketUI.Serialization.Platform.Sdl
{
	public static class Sdl
	{
		public static IntPtr NativeLibrary = GetNativeLibrary();

		private static IntPtr GetNativeLibrary()
		{
            if (CurrentPlatform.OS == OS.Windows)
                return FuncLoader.LoadLibraryExt("SDL2.dll");
            else if (CurrentPlatform.OS == OS.Linux)
                return FuncLoader.LoadLibraryExt("libSDL2-2.0.so.0");
            else if (CurrentPlatform.OS == OS.MacOSX)
                return FuncLoader.LoadLibraryExt("libSDL2-2.0.0.dylib");
            else
                return FuncLoader.LoadLibraryExt("sdl2");
		}
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate bool SDL_HAS_TEST();

		internal static SDL_HAS_TEST SDL_HasClipboardText =
			FuncLoader.LoadFunction<SDL_HAS_TEST>(Sdl.NativeLibrary, "SDL_HasClipboardText");
		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr SDL_CLIPBOARD_TEXT();

		private static SDL_CLIPBOARD_TEXT INTERNAL_SDL_GetClipboardText =
			FuncLoader.LoadFunction<SDL_CLIPBOARD_TEXT>(Sdl.NativeLibrary, "SDL_GetClipboardText");
		
		public static string SDL_GetClipboardText()
		{
			return UTF8_ToManaged(INTERNAL_SDL_GetClipboardText(), true);
		}
		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private unsafe delegate int SDL_SET_TEXT(
			byte* text
		);
		
		private static SDL_SET_TEXT INTERNAL_SDL_SetClipboardText = FuncLoader.LoadFunction<SDL_SET_TEXT>(Sdl.NativeLibrary, "SDL_SetClipboardText");
		
		public static unsafe int SDL_SetClipboardText(
			string text
		) {
			byte* utf8Text = Utf8Encode(text);
			int result = INTERNAL_SDL_SetClipboardText(
				utf8Text
			);
			Marshal.FreeHGlobal((IntPtr) utf8Text);
			return result;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void D_SDL_FREE(IntPtr ptr);
		
		private static D_SDL_FREE SDL_free = FuncLoader.LoadFunction<D_SDL_FREE>(Sdl.NativeLibrary, "SDL_SetClipboardText");

		public static unsafe string UTF8_ToManaged(IntPtr s, bool freePtr = false)
		{
			if (s == IntPtr.Zero)
			{
				return null;
			}

			/* We get to do strlen ourselves! */
			byte* ptr = (byte*) s;
			while (*ptr != 0)
			{
				ptr++;
			}

			/* TODO: This #ifdef is only here because the equivalent
			 * .NET 2.0 constructor appears to be less efficient?
			 * Here's the pretty version, maybe steal this instead:
			 *
			string result = new string(
				(sbyte*) s, // Also, why sbyte???
				0,
				(int) (ptr - (byte*) s),
				System.Text.Encoding.UTF8
			);
			 * See the CoreCLR source for more info.
			 * -flibit
			 */

			/* Modern C# lets you just send the byte*, nice! */
			string result = System.Text.Encoding.UTF8.GetString(
				(byte*) s,
				(int) (ptr - (byte*) s)
			);

			/* Some SDL functions will malloc, we have to free! */
			if (freePtr)
			{
				SDL_free(s);
			}
			return result;
		}
		
		internal static unsafe byte* Utf8Encode(string str, byte* buffer, int bufferSize)
		{
			fixed (char* strPtr = str)
			{
				Encoding.UTF8.GetBytes(strPtr, str.Length + 1, buffer, bufferSize);
			}
			return buffer;
		}
		
		internal static int Utf8Size(string str)
		{
			return (str.Length * 4) + 1;
		}
		
		internal static unsafe byte* Utf8Encode(string str)
		{
			int bufferSize = Utf8Size(str);
			byte* buffer = (byte*) Marshal.AllocHGlobal(bufferSize);
			fixed (char* strPtr = str)
			{
				Encoding.UTF8.GetBytes(strPtr, str.Length + 1, buffer, bufferSize);
			}
			return buffer;
		}
	}
	
	internal class FuncLoader
    {
        private class Windows
        {
            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr LoadLibraryW(string lpszLib);
        }

        private class Linux
        {
            [DllImport("libdl.so.2")]
            public static extern IntPtr dlopen(string path, int flags);

            [DllImport("libdl.so.2")]
            public static extern IntPtr dlsym(IntPtr handle, string symbol);
        }

        private class OSX
        {
            [DllImport("/usr/lib/libSystem.dylib")]
            public static extern IntPtr dlopen(string path, int flags);

            [DllImport("/usr/lib/libSystem.dylib")]
            public static extern IntPtr dlsym(IntPtr handle, string symbol);
        }
        
        private const int RTLD_LAZY = 0x0001;

        public static IntPtr LoadLibraryExt(string libname)
        {
            var ret = IntPtr.Zero;
            var assemblyLocation = Path.GetDirectoryName(typeof(FuncLoader).Assembly.Location) ?? "./";

            // Try .NET Framework / mono locations
            if (CurrentPlatform.OS == OS.MacOSX)
            {
                ret = LoadLibrary(Path.Combine(assemblyLocation, libname));

                // Look in Frameworks for .app bundles
                if (ret == IntPtr.Zero)
                    ret = LoadLibrary(Path.Combine(assemblyLocation, "..", "Frameworks", libname));
            }
            else
            {
                if (Environment.Is64BitProcess)
                    ret = LoadLibrary(Path.Combine(assemblyLocation, "x64", libname));
                else
                    ret = LoadLibrary(Path.Combine(assemblyLocation, "x86", libname));
            }

            // Try .NET Core development locations
            if (ret == IntPtr.Zero)
                ret = LoadLibrary(Path.Combine(assemblyLocation, "runtimes", CurrentPlatform.Rid, "native", libname));

            // Try current folder (.NET Core will copy it there after publish)
            if (ret == IntPtr.Zero)
                ret = LoadLibrary(Path.Combine(assemblyLocation, libname));

            // Try alternate way of checking current folder
            // assemblyLocation is null if we are inside macOS app bundle
            if (ret == IntPtr.Zero)
                ret = LoadLibrary(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, libname));

            // Try loading system library
            if (ret == IntPtr.Zero)
                ret = LoadLibrary(libname);

            // Welp, all failed, PANIC!!!
            if (ret == IntPtr.Zero)
                throw new Exception("Failed to load library: " + libname);

            return ret;
        }

        public static IntPtr LoadLibrary(string libname)
        {
            if (CurrentPlatform.OS == OS.Windows)
                return Windows.LoadLibraryW(libname);

            if (CurrentPlatform.OS == OS.MacOSX)
                return OSX.dlopen(libname, RTLD_LAZY);

            return Linux.dlopen(libname, RTLD_LAZY);
        }

        public static T LoadFunction<T>(IntPtr library, string function, bool throwIfNotFound = false)
        {
            var ret = IntPtr.Zero;

            if (CurrentPlatform.OS == OS.Windows)
                ret = Windows.GetProcAddress(library, function);
            else if (CurrentPlatform.OS == OS.MacOSX)
                ret = OSX.dlsym(library, function);
            else
                ret = Linux.dlsym(library, function);

            if (ret == IntPtr.Zero)
            {
                if (throwIfNotFound)
                    throw new EntryPointNotFoundException(function);

                return default(T);
            }

#if NETSTANDARD
            return Marshal.GetDelegateForFunctionPointer<T>(ret);
#else
            return (T)(object)Marshal.GetDelegateForFunctionPointer(ret, typeof(T));
#endif
        }
    }
    
     internal enum OS
    {
        Windows,
        Linux,
        MacOSX,
        Unknown
    }

    internal static class CurrentPlatform
    {
        private static bool _init = false;
        private static OS _os;

        [DllImport("libc")]
        static extern int uname(IntPtr buf);

        private static void Init()
        {
            if (_init)
                return;

            var pid = Environment.OSVersion.Platform;

            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    _os = OS.Windows;
                    break;
                case PlatformID.MacOSX:
                    _os = OS.MacOSX;
                    break;
                case PlatformID.Unix:
                    _os = OS.MacOSX;

                    var buf = IntPtr.Zero;
                    
                    try
                    {
                        buf = Marshal.AllocHGlobal(8192);

                        if (uname(buf) == 0 && Marshal.PtrToStringAnsi(buf) == "Linux")
                            _os = OS.Linux;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if (buf != IntPtr.Zero)
                            Marshal.FreeHGlobal(buf);
                    }

                    break;
                default:
                    _os = OS.Unknown;
                    break;
            }

            _init = true;
        }

        public static OS OS
        {
            get
            {
                Init();
                return _os;
            }
        }

        public static string Rid
        {
            get
            {
                if (CurrentPlatform.OS == OS.Windows && Environment.Is64BitProcess)
                    return "win-x64";
                else if (CurrentPlatform.OS == OS.Windows && !Environment.Is64BitProcess)
                    return "win-x86";
                else if (CurrentPlatform.OS == OS.Linux)
                    return "linux-x64";
                else if (CurrentPlatform.OS == OS.MacOSX)
                    return "osx";
                else
                    return "unknown";
            }
        }
    }
}