using RocketUI.Serialization.Platform.Sdl;

namespace RocketUI.Utilities.IO
{
	public class SdlClipboard : IClipboardImplementation
	{
		/// <inheritdoc />
		public void SetText(string value)
		{
			Sdl.SDL_SetClipboardText(value);
		}

		/// <inheritdoc />
		public string GetText()
		{
			if (Sdl.SDL_HasClipboardText())
				return Sdl.SDL_GetClipboardText();

			return string.Empty;
		}
	}
}