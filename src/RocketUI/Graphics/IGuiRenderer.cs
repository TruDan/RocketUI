using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using RocketUI.Audio;

namespace RocketUI
{
    public interface IGuiRenderer
    {
        GuiScaledResolution ScaledResolution { get; set; }
        void Init(IServiceProvider serviceProvider);

        IFont Font { get; set; }

        ISoundEffect GetSoundEffect(GuiSoundEffects soundEffects);
        TextureSlice2D GetTexture(GuiTextures                     guiTexture);
        TextureSlice2D GetTexture(string                          texturePath);
        Texture2D GetTexture2D(GuiTextures                        guiTexture);

        string GetTranslation(string key);

        Vector2 Project(Vector2 point);
        Vector2 Unproject(Vector2 screen);

        IElementRenderer<TElement> ResolveRenderer<TElement>(TElement element) where TElement : IGuiElement;
        void Draw(IGuiElement element);
        
        IStyle[] ResolveStyles(Type elementType, string[] classNames);
        
        void OnBeginDraw() {}
        void OnEndDraw() {}
    }
}
