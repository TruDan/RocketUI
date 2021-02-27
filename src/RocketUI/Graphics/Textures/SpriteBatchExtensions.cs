﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketUI
{
    public static class SpriteBatchExtensions
    {
        

        //public static void Draw(this SpriteBatch spriteBatch, TextureSlice2D textureSlice, Rectangle bounds)
        //{
        //    Draw(spriteBatch, textureSlice, bounds, Color.White);
        //}

        //public static void Draw(this SpriteBatch spriteBatch, TextureSlice2D textureSlice, Rectangle bounds, Color color)
        //{
        //    Draw(spriteBatch, textureSlice.Texture, textureSlice.RenderBounds, bounds, color);
        //}
        
        //public static void Draw(this SpriteBatch spriteBatch, TextureSlice2D textureSlice, Rectangle sourceRectangle, Rectangle destinationRectangle)
        //{
        //    Draw(spriteBatch, textureSlice, sourceRectangle, destinationRectangle, Color.White);
        //}

        //public static void Draw(this SpriteBatch spriteBatch, TextureSlice2D textureSlice, Rectangle sourceRectangle, Rectangle destinationRectangle, Color color)
        //{
        //    spriteBatch.Draw(textureSlice.Texture, sourceRectangle, destinationRectangle, color);
        //}

        //public static void Draw(this SpriteBatch spriteBatch, TextureSlice2D textureSlice, Rectangle bounds, Color color, float rotation, Vector2 origin, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        //{
        //    spriteBatch.Draw(textureSlice.Texture, bounds, textureSlice.RenderBounds, color, rotation, origin, effects, layerDepth);
        //}

        #region Helpers

        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Rectangle destinationRectangle) =>
            Draw(spriteBatch, textureSlice, destinationRectangle, Color.White);
        
        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Rectangle destinationRectangle, Rectangle? sourceRectangle) =>
            Draw(spriteBatch, textureSlice, destinationRectangle, sourceRectangle, Color.White);
        
        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position) =>
            Draw(spriteBatch, textureSlice, position, Color.White);
        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position, Vector2? scale) =>
            Draw(spriteBatch, textureSlice, position, null, Color.White, 0f, Vector2.Zero, scale ?? Vector2.One);
        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position, Rectangle? sourceRectangle) =>
            Draw(spriteBatch, textureSlice, position, sourceRectangle, Color.White);
        #endregion

        #region Based on original
            
        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Rectangle destinationRectangle, Color color)
        {
            spriteBatch.Draw(textureSlice.Texture, destinationRectangle, textureSlice.ClipBounds, color);
        }

        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            spriteBatch.Draw(textureSlice.Texture, destinationRectangle, sourceRectangle, color);
        }

        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            spriteBatch.Draw(textureSlice.Texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
        }
        
        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position, Color color)
        {
            spriteBatch.Draw(textureSlice.Texture, position, textureSlice.ClipBounds, color);
        }
        
        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            spriteBatch.Draw(textureSlice.Texture, position, textureSlice.ClipBounds, color, rotation, origin, scale, effects, layerDepth);
        }

        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            spriteBatch.Draw(textureSlice.Texture, position, sourceRectangle ?? textureSlice.ClipBounds, color);
        }

        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            spriteBatch.Draw(textureSlice.Texture, position, sourceRectangle ?? textureSlice.ClipBounds, color, rotation, origin, scale, effects, layerDepth);
        }

        public static void Draw(this SpriteBatch spriteBatch, ITexture2D textureSlice, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            spriteBatch.Draw(textureSlice.Texture, position, sourceRectangle ?? textureSlice.ClipBounds, color, rotation, origin, scale, effects, layerDepth);
        }

        #endregion
    }
}
