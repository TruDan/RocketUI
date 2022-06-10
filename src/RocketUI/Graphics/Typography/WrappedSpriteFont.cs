using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketUI
{
    public class WrappedSpriteFont : IFont
    {
        public IReadOnlyCollection<char> Characters => _spriteFont.Characters;

        private readonly SpriteFont _spriteFont;

        public WrappedSpriteFont(SpriteFont spriteFont)
        {
            _spriteFont = spriteFont;
        }

        public Vector2 MeasureString(string text) => _spriteFont.MeasureString(text);

        public void DrawString(SpriteBatch sb, string text, Vector2 position,
            Color                          color,
            FontStyle                      style      = FontStyle.None, Vector2? scale = null,
            float                          opacity    = 1f,
            float                          rotation   = 0f, Vector2? origin = null,
            SpriteEffects                  effects    = SpriteEffects.None,
            float                          layerDepth = 0f)
        {
            sb.DrawString(_spriteFont, text, position, color * opacity, rotation, origin ?? Vector2.Zero,
                scale ?? Vector2.One, effects, layerDepth);
        }

        public static implicit operator WrappedSpriteFont(SpriteFont spriteFont)
        {
            return new WrappedSpriteFont(spriteFont);
        }

        public static implicit operator SpriteFont(WrappedSpriteFont font)
        {
            return font._spriteFont;
        }

        /// <inheritdoc />
        public void Dispose() { }
    }
}