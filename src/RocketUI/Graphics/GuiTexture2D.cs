using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using RocketUI.Audio;
using RocketUI.Utilities.Converters;

namespace RocketUI
{
    public class GuiSound
    {
        public GuiSoundEffects? GuiSoundEffect { get; set; }
        public ISoundEffect     SoundEffect { get; set; }

        public bool HasValue => GuiSoundEffect.HasValue && GuiSoundEffect.Value != GuiSoundEffects.None;

        public GuiSound() { }
        public GuiSound(GuiSoundEffects guiSoundEffects) : this()
        {
            GuiSoundEffect = guiSoundEffects;
        }

        public bool TryResolve(IGuiRenderer renderer)
        {
            if (GuiSoundEffect.HasValue)
            {
                if(GuiSoundEffect.Value != GuiSoundEffects.None) 
                    SoundEffect = renderer.GetSoundEffect(GuiSoundEffect.Value);
                return SoundEffect != null;
            }

            return true;
        }

        public void Play()
        {
            SoundEffect?.Play();
        }

        public static implicit operator GuiSound(GuiSoundEffects guiSoundEffects)
        {
            return new GuiSound(guiSoundEffects);
        }
    }
    
    [TypeConverter(typeof(GuiTexture2DTypeConverter))]
    public class GuiTexture2D : ITexture2D
    {
        public Color?            Color           { get; set; }
        public GuiTextures?      TextureResource { get; set; }
        public string            TexturePath     { get; set; }
        public ITexture2D        Texture         { get; set; }
        public TextureRepeatMode RepeatMode      { get; set; }
        public Color?            Mask            { get; set; }
        public Vector2?          Scale           { get; set; }

        public bool HasValue => Texture != null || Color.HasValue || TextureResource.HasValue || !string.IsNullOrEmpty(TexturePath);

        public GuiTexture2D() { }
        public GuiTexture2D(ITexture2D texture) : this()
        {
            Texture = texture;
		}
		public GuiTexture2D(ITexture2D texture, TextureRepeatMode repeatMode = TextureRepeatMode.Stretch, Vector2? scale = null) : this()
		{
			Texture = texture;
			RepeatMode = repeatMode;
			Scale = scale;
		}

		public GuiTexture2D(GuiTextures guiTexture, TextureRepeatMode repeatMode = TextureRepeatMode.Stretch, Vector2? scale = null) : this()
        {
            TextureResource = guiTexture;
            RepeatMode = repeatMode;
            Scale = scale;
        }
        public GuiTexture2D(string texturePath, TextureRepeatMode repeatMode = TextureRepeatMode.Stretch, Vector2? scale = null) : this()
        {
            TexturePath = texturePath;
            RepeatMode = repeatMode;
            Scale = scale;
        }

        public bool TryResolveTexture(IGuiRenderer renderer)
        {
            if (TextureResource.HasValue)
            {
                Texture = renderer.GetTexture(TextureResource.Value);
                return Texture != null;
            }

            if (!string.IsNullOrEmpty(TexturePath))
            {
                Texture = renderer.GetTexture(TexturePath);
                return Texture != null;
            }   
            return true;
        }

        public static implicit operator GuiTexture2D(TextureSlice2D texture)
        {
            return new GuiTexture2D(texture);
        }

        public static implicit operator GuiTexture2D(NinePatchTexture2D texture)
        {
            return new GuiTexture2D(texture);
        }

        public static implicit operator GuiTexture2D(Color color)
        {
            return new GuiTexture2D { Color = color };
        }
        
        public static explicit operator GuiTexture2D(string texturePath)
        {
            return new GuiTexture2D { TexturePath = texturePath };
        }

        public static implicit operator GuiTexture2D(GuiTextures textureResource)
        {
            return new GuiTexture2D { TextureResource = textureResource };
        }

        Texture2D ITexture2D.Texture => Texture.Texture;
        public Rectangle ClipBounds => Texture?.ClipBounds ?? Rectangle.Empty;
        public int Width => Texture?.Width ?? 0;
        public int Height => Texture?.Height ?? 0;
    }
}
