using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace RocketUI
{
    public interface IGuiRenderer
    {
        GuiScaledResolution ScaledResolution { get; set; }
        void Init(GraphicsDevice graphics, IServiceProvider serviceProvider);

        IFont Font { get; set; }

        SoundEffectInstance GetSoundEffect(GuiSoundEffects soundEffects);
        TextureSlice2D GetTexture(GuiTextures                     guiTexture);
        TextureSlice2D GetTexture(string                          texturePath);
        Texture2D GetTexture2D(GuiTextures                        guiTexture);

        string GetTranslation(string key);

        Vector2 Project(Vector2 point);
        Vector2 Unproject(Vector2 screen);
    }
}
