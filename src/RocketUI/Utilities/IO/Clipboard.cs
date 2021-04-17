using System;
using System.Runtime.InteropServices;
using MonoGame.Framework.Utilities;

namespace RocketUI.Utilities.IO
{
    public static class Clipboard
    {
        private static IClipboardImplementation Implementation { get; set; } = null;

        static Clipboard()
        {
            if (PlatformInfo.MonoGamePlatform == MonoGamePlatform.DesktopGL)
            {
                Implementation = new SdlClipboard();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Implementation = new WindowsClipboard();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (XClipClipboard.IsXClipAvailable())
                {
                    Implementation = new XClipClipboard();
                }
               /* else
                {
                    var desktopEnvironment = Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP");
                    if (desktopEnvironment != null)
                    {
                        if (desktopEnvironment.ToLower().Contains("kde"))
                        {
                            Implementation = new LinuxKdeClipboard();
                        }
                    }
                }*/
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Implementation = new MacOSClipboard();
            }

            if (Implementation == null)
            {
                Implementation = new MockClipboard();
            }
        }

        public static bool IsClipboardAvailable()
        {
            return Implementation != null && !(Implementation is MockClipboard);
        }
        
        public static void SetText(string text)
        {
            Implementation.SetText(text);
        }

        public static string GetText()
        {
            return Implementation.GetText();
        }
    }

    public interface IClipboardImplementation
    {
        void SetText(string value);
        string GetText();
    }
}