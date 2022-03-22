using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketUI
{
    public interface IFont : IDisposable
    {
        IReadOnlyCollection<char> Characters { get; }
        //IFontGlyph GetGlyphOrDefault(char character);

        Vector2 MeasureString(string text, float scale) => MeasureString(text) * scale;
        Vector2 MeasureString(string text, Vector2 scale) => MeasureString(text) * scale;

        Vector2 MeasureString(string text);
        //void MeasureString(string text, out Vector2 size);

        void DrawString(SpriteBatch sb,
            string text,
            Vector2 position,
            Color color,
            FontStyle style = FontStyle.None, Vector2? scale = null,
            float opacity = 1f,
            float rotation = 0f, Vector2? origin = null,
            SpriteEffects effects = SpriteEffects.None,
            float layerDepth = 0f);
    }

    public interface IFontGlyph
    {
        char Character { get; }
        ITexture2D Texture { get; }
        float Width { get; }
        float Height { get; }
        float Scale { get; }
    }
}