﻿using System;
using System.Drawing;
using System.Numerics;
using RocketUI.Serialization;

namespace RocketUI
{
    public class GuiRenderArgs : GraphicsContext
    {
        public IGuiRenderer Renderer { get; }

        public GraphicsDevice Graphics { get; }
        public GraphicsContext GraphicsContext { get; }

        public SpriteBatch SpriteBatch { get; }

        public GameTime GameTime { get; }

        public GuiScaledResolution ScaledResolution { get; }

        //public GuiElementRenderContext ActiveContext { get; private set; }

        public GuiRenderArgs(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GuiScaledResolution scaledResolution, IGuiRenderer renderer, GameTime gameTime) : base(graphicsDevice)
        {
            Renderer = renderer;
            Graphics = graphicsDevice;
            SpriteBatch = spriteBatch;
            GameTime = gameTime;
            ScaledResolution = scaledResolution;
        }

        public void BeginSpriteBatch()
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, ScaledResolution.TransformMatrix);
        }

        public void EndSpriteBatch()
        {
            SpriteBatch.End();
        }
       
        public void DrawRectangle(Rectangle bounds, RgbaColor color, int thickness = 1)
        {
            DrawRectangle(bounds, color, thickness, thickness, thickness, thickness);
        }

        public void DrawRectangle(Rectangle bounds, RgbaColor color, Thickness thickness)
        {
            DrawRectangle(bounds, color, thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
        }

        public void DrawRectangle(Rectangle bounds, RgbaColor color, int thicknessVertical, int thicknessHorizontal)
        {
            DrawRectangle(bounds, color, thicknessHorizontal, thicknessVertical, thicknessHorizontal, thicknessVertical);
        }

        public void DrawRectangle(Rectangle bounds, RgbaColor color, int thicknessLeft, int thicknessTop, int thicknessRight, int thicknessBottom)
        {
            var texture = GpuResourceManager.CreateTexture2D( 1, 1, false, SurfaceFormat.RgbaColor);
            texture.SetData(new RgbaColor[] {color});

            // MinY
            if (thicknessTop > 0)
            {
                SpriteBatch.Draw(texture, new Rectangle(bounds.X, bounds.Y, bounds.Width, thicknessTop), color);   
            }

            // MaxX
            if (thicknessRight > 0)
            {
                SpriteBatch.Draw(texture,
                                 new Rectangle(bounds.X + bounds.Width - thicknessRight, bounds.Y, thicknessRight, bounds.Height),
                                 color);
            }

            // MaxY
            if (thicknessBottom > 0)
            {
                SpriteBatch.Draw(texture,
                                 new Rectangle(bounds.X, bounds.Y + bounds.Height - thicknessBottom, bounds.Width, thicknessBottom),
                                 color);
            }

            // MinX
            if (thicknessLeft > 0)
            {
                SpriteBatch.Draw(texture, new Rectangle(bounds.X, bounds.Y, thicknessLeft, bounds.Height), color);
            }
        }

        public void FillRectangle(Rectangle bounds, RgbaColor color)
        {
            var texture = GpuResourceManager.CreateTexture2D( 1, 1, false, SurfaceFormat.RgbaColor);
            texture.SetData(new RgbaColor[] {color});

            SpriteBatch.Draw(texture, bounds, Colors.White);
        }
        
        public void DrawLine(int startX, int startY, int endX, int endY, RgbaColor color, int thickness = 1)
        {
            // TODO: support angles
            DrawRectangle(new Rectangle(startX, startY, endX-startX, endY-startY), color, thickness);
        }

        public void DrawLine(Vector2 startPosition, Vector2 endPosition, RgbaColor color)
        {
            //var diff = (endPosition - startPosition);
            //var len = Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);

            //diff.Normalize();
            
            //SpriteBatch.
        }

        public void Draw(TextureSlice2D texture, Rectangle bounds, TextureRepeatMode repeatMode = TextureRepeatMode.Stretch, Vector2? scale = null)
        {
            if (texture is NinePatchTexture2D ninePatch)
            {
                DrawNinePatch(bounds, ninePatch, repeatMode, scale);
            }
            else
            {
                FillRectangle(bounds, texture, repeatMode, scale);
            }
        }
        
        public void DrawNinePatch(Rectangle bounds, NinePatchTexture2D ninePatchTexture, TextureRepeatMode repeatMode, Vector2? scale = null)
        {
            if(scale == null) scale = Vector2.One;
            
            if (ninePatchTexture.Padding == Thickness.Zero)
            {
                FillRectangle(bounds, ninePatchTexture.Texture, repeatMode);
                return;
            }
			
            var sourceRegions = ninePatchTexture.SourceRegions;
            var destRegions   = ninePatchTexture.ProjectRegions(bounds);

            for (var i = 0; i < sourceRegions.Length; i++)
            {
                var srcPatch = sourceRegions[i];
                var dstPatch = destRegions[i];

                if(dstPatch.Width > 0 && dstPatch.Height > 0)
                    SpriteBatch.Draw(ninePatchTexture, sourceRectangle: srcPatch, destinationRectangle: dstPatch);
            }
        }

        public void FillRectangle(Rectangle bounds, TextureSlice2D texture, TextureRepeatMode repeatMode, Vector2? scale = null)
        {
            if(scale == null) scale = Vector2.One;

            if (repeatMode == TextureRepeatMode.NoRepeat)
            {
                SpriteBatch.Draw(texture, bounds);
            }
            else if (repeatMode == TextureRepeatMode.Stretch)
            {
                SpriteBatch.Draw(texture, bounds);
            }
            else if (repeatMode == TextureRepeatMode.ScaleToFit)
            {
            }
            else if (repeatMode == TextureRepeatMode.Tile)
            {
                Vector2 size = texture.ClipBounds.Size.ToVector2() * scale.Value;

                var repeatX = Math.Ceiling((float) bounds.Width  / size.X);
                var repeatY = Math.Ceiling((float) bounds.Height / size.Y);

                for (int i = 0; i < repeatX; i++)
                {
                    for (int j = 0; j < repeatY; j++)
                    {
                        var p = bounds.Location.ToVector2() + new Vector2(i * size.X, j * size.Y);
                        SpriteBatch.Draw(texture, p, scale);
                    }
                }
            }
            else if (repeatMode == TextureRepeatMode.NoScaleCenterSlice)
            {
                var halfWidth = bounds.Width / 2f;
                var halfHeight = bounds.Height / 2f;
                int xOffset = bounds.X + (int)Math.Max(0, (bounds.Width - texture.Width) / 2f);
                int yOffset = bounds.Y + (int)Math.Max(0, (bounds.Height - texture.Height) / 2f);

                int dstLeftWidth = (int) Math.Floor(halfWidth);
                int dstRightWidth = (int) Math.Ceiling(halfWidth);
                int dstLeftHeight = (int) Math.Floor(halfHeight);
                int dstRightHeight = (int) Math.Ceiling(halfHeight);

                var srcHalfWidth = Math.Min(texture.Width / 2f, halfWidth);
                var srcHalfHeight = Math.Min(texture.Height / 2f, halfHeight);

                var srcX = texture.ClipBounds.X;
                var srcY = texture.ClipBounds.Y;

                int srcLeftWidth   = (int) Math.Floor(srcHalfWidth);
                int srcRightWidth  = (int) Math.Ceiling(srcHalfWidth);
                int srcLeftHeight  = (int) Math.Floor(srcHalfHeight);
                int srcRightHeight = (int) Math.Ceiling(srcHalfHeight);

                // MinY MinX
                SpriteBatch.Draw(texture, new Rectangle(xOffset               , yOffset, dstLeftWidth, dstLeftHeight), new Rectangle(srcX, srcY, srcLeftWidth, srcLeftHeight), Colors.White);
                
                // MinY MaxX
                SpriteBatch.Draw(texture, new Rectangle(xOffset + dstLeftWidth, yOffset, dstRightWidth, dstRightHeight), new Rectangle(srcX + texture.Width - srcRightWidth, srcY, srcRightWidth, srcRightHeight), Colors.White);


                // MaxY MinX
                SpriteBatch.Draw(texture, new Rectangle(xOffset               , yOffset + dstLeftHeight , dstLeftWidth, dstLeftHeight), new Rectangle(srcX, srcY + texture.Height - srcRightHeight, srcLeftWidth, srcLeftHeight), Colors.White);
                
                // MaxY MaxX
                SpriteBatch.Draw(texture, new Rectangle(xOffset + dstLeftWidth, yOffset + dstRightHeight, dstRightWidth, dstRightHeight), new Rectangle(srcX + texture.Width - srcRightWidth, srcY + texture.Height - srcRightHeight, srcRightWidth, srcRightHeight), Colors.White);
            }
        }
        
        #region IFont Proxy
        
        public void DrawString(IFont     font,                        string    text,
                               Vector2   position,                    RgbaColor color, float scale = 1f,
                               FontStyle style      = FontStyle.None, float     rotation = 0f,
                               Vector2?  origin     = null,
                               float     opacity    = 1f, SpriteEffects effects = SpriteEffects.None,
                               float     layerDepth = 0f)
        {
            font.DrawString(SpriteBatch, text, position, color, style, scale: new Vector2(scale), rotation: rotation, origin: origin ?? Vector2.Zero, opacity: opacity, effects: effects, layerDepth: layerDepth);
        }

        public void DrawString(IFont font, string text, Vector2 position, RgbaColor color, Vector2? scale = null, FontStyle style = FontStyle.None, float rotation = 0f, Vector2? origin = null, float opacity = 1f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            font.DrawString(SpriteBatch, text, position, color, style, scale: scale ?? Vector2.One, rotation: rotation, origin: origin ?? Vector2.Zero, opacity: opacity, effects: effects, layerDepth: layerDepth);
        }

        #endregion
    }
    
}
