using System.Drawing;
using System.Numerics;
using RocketUI.Serialization;

namespace RocketUI
{
    public static class SpriteBatchExtensions
    {
        

        //public static void Draw(this SpriteBatch spriteBatch, TextureSlice2D textureSlice, Rectangle bounds)
        //{
        //    Draw(spriteBatch, textureSlice, bounds, Colors.White);
        //}

        //public static void Draw(this SpriteBatch spriteBatch, TextureSlice2D textureSlice, Rectangle bounds, RgbaColor color)
        //{
        //    Draw(spriteBatch, textureSlice.Texture, textureSlice.RenderBounds, bounds, color);
        //}
        
        //public static void Draw(this SpriteBatch spriteBatch, TextureSlice2D textureSlice, Rectangle sourceRectangle, Rectangle destinationRectangle)
        //{
        //    Draw(spriteBatch, textureSlice, sourceRectangle, destinationRectangle, Colors.White);
        //}

        //public static void Draw(this SpriteBatch spriteBatch, TextureSlice2D textureSlice, Rectangle sourceRectangle, Rectangle destinationRectangle, RgbaColor color)
        //{
        //    spriteBatch.Draw(textureSlice.Texture, sourceRectangle, destinationRectangle, color);
        //}

        //public static void Draw(this SpriteBatch spriteBatch, TextureSlice2D textureSlice, Rectangle bounds, RgbaColor color, float rotation, Vector2 origin, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        //{
        //    spriteBatch.Draw(textureSlice.Texture, bounds, textureSlice.RenderBounds, color, rotation, origin, effects, layerDepth);
        //}

        #region Helpers

        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Rectangle destinationRectangle) =>
            Draw(spriteBatch, textureSlice, destinationRectangle, Colors.White);
        
        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Rectangle destinationRectangle, Rectangle? sourceRectangle) =>
            Draw(spriteBatch, textureSlice, destinationRectangle, sourceRectangle, Colors.White);
        
        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position) =>
            Draw(spriteBatch, textureSlice, position, Colors.White);
        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position, Vector2? scale) =>
            Draw(spriteBatch, textureSlice, position, null, Colors.White, 0f, Vector2.Zero, scale ?? Vector2.One);
        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position, Rectangle? sourceRectangle) =>
            Draw(spriteBatch, textureSlice, position, sourceRectangle, Colors.White);
        #endregion

        #region Based on original
            
        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Rectangle destinationRectangle, RgbaColor color)
        {
            spriteBatch.Draw(textureSlice.Texture, destinationRectangle, textureSlice.ClipBounds, color);
        }

        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Rectangle destinationRectangle, Rectangle? sourceRectangle, RgbaColor color)
        {
            spriteBatch.Draw(textureSlice.Texture, destinationRectangle, sourceRectangle, color);
        }

        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Rectangle destinationRectangle, Rectangle? sourceRectangle, RgbaColor color, float rotation, Vector2 origin, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            spriteBatch.Draw(textureSlice.Texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
        }
        
        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position, RgbaColor color)
        {
            spriteBatch.Draw(textureSlice.Texture, position, textureSlice.ClipBounds, color);
        }
        
        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position, RgbaColor color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            spriteBatch.Draw(textureSlice.Texture, position, textureSlice.ClipBounds, color, rotation, origin, scale, effects, layerDepth);
        }

        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position, Rectangle? sourceRectangle, RgbaColor color)
        {
            spriteBatch.Draw(textureSlice.Texture, position, sourceRectangle ?? textureSlice.ClipBounds, color);
        }

        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position, Rectangle? sourceRectangle, RgbaColor color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            spriteBatch.Draw(textureSlice.Texture, position, sourceRectangle ?? textureSlice.ClipBounds, color, rotation, origin, scale, effects, layerDepth);
        }

        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position, Rectangle? sourceRectangle, RgbaColor color, float rotation, Vector2 origin, float scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            spriteBatch.Draw(textureSlice.Texture, position, sourceRectangle ?? textureSlice.ClipBounds, color, rotation, origin, scale, effects, layerDepth);
        }

        #endregion
    }
}
