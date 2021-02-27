using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RocketUI.Utilities.Extensions;

namespace RocketUI
{
    public class GuiSpriteBatch : IDisposable
    {
        private static readonly RasterizerState RasterizerState = GetDefaultRasterizerState();

        private static RasterizerState GetDefaultRasterizerState()
        {
            var rast = CopyRasterizerState(RasterizerState.CullNone);
            rast.ScissorTestEnable = true;
            return rast;
        }

        public IFont Font { get; set; }

        //public GraphicsContext Graphics { get; }
        public SpriteBatch         SpriteBatch      { get; private set; }
        public GuiScaledResolution ScaledResolution { get; }

        public GraphicsContext Context { get; }

        private readonly GraphicsDevice _graphicsDevice;
        private readonly IGuiRenderer _renderer;
        private Texture2D _colorTexture;
        public Matrix RenderMatrix = Matrix.Identity;

        private bool _beginSpriteBatchAfterContext;
        private bool _hasBegun;

        public GuiSpriteBatch(IGuiRenderer renderer, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _renderer = renderer;
            _graphicsDevice = graphicsDevice;
            SpriteBatch = spriteBatch;

            Effect = new BasicEffect(graphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true
            };
            
            var cameraPosition = new Vector3(0, 0, 13);
            cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateScale(1));

            //var view        = Matrix.CreateLookAt(cameraPosition, new Vector3(0, 0, 0), Vector3.Up);
            //var projection  = Matrix.CreatePerspectiveFieldOfView(1, _graphicsDevice.Viewport.AspectRatio, 1.0f, 1000.0f);

            
            //Effect.World       = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            //Effect.View        = view;
            //Effect.Projection = projection;
            
            Effect.World = Matrix.Identity;
            Effect.View = Matrix.Identity;
            Effect.Projection = Matrix.Identity;
            
//            Context = GraphicsContext.CreateContext(_graphicsDevice, BlendState.NonPremultiplied, DepthStencilState.None, RasterizerState, SamplerState.PointClamp);
            Context = GraphicsContext.CreateContext(_graphicsDevice, BlendState.AlphaBlend, DepthStencilState.None, RasterizerState.CullNone, SamplerState.AnisotropicWrap);

            Font = _renderer.Font;
            ScaledResolution = _renderer.ScaledResolution;
        }

        public Vector2 Project(Vector2 point)
        {
            return Vector2.Transform(point, ScaledResolution.TransformMatrix);
        }

        public Vector2 Unproject(Vector2 screen)
        {
            return Vector2.Transform(screen, ScaledResolution.InverseTransformMatrix);
        }


        public Rectangle Project(Rectangle rectangle)
        {
            var loc1 = Vector2.Transform(rectangle.Location.ToVector2(), ScaledResolution.TransformMatrix);
            var loc2 = Vector2.Transform((rectangle.Location + rectangle.Size).ToVector2(),
                ScaledResolution.TransformMatrix);

            var loc1P = new Point((int) Math.Floor(loc1.X), (int) Math.Floor(loc1.Y));
            var loc2P = new Point((int) Math.Ceiling(loc2.X), (int) Math.Ceiling(loc2.Y));
            var size = loc2P - loc1P;

            return new Rectangle(loc1P, size);
        }

        public Rectangle Unproject(Rectangle screen)
        {
            var loc1 = Vector2.Transform(screen.Location.ToVector2(), ScaledResolution.InverseTransformMatrix)
                .ToPoint();
            var loc2 = Vector2.Transform((screen.Location + screen.Size).ToVector2(),
                ScaledResolution.InverseTransformMatrix).ToPoint();
            return new Rectangle(loc1, loc2 - loc1);
        }

        public void Begin(bool withScale = true)
        {
            if (_hasBegun) return;

            SpriteBatch.Begin(SpriteSortMode.Deferred, Context.BlendState, Context.SamplerState, Context.DepthStencilState, Context.RasterizerState, Effect, withScale ? ScaledResolution.TransformMatrix * RenderMatrix : ScaledResolution.TransformMatrix);

            _hasBegun = true;
        }

        public void End()
        {
            if (!_hasBegun) return;
            SpriteBatch.End();

            _hasBegun = false;
        }


        #region Sub-Contexts

        public GraphicsContext BranchContext(BlendState blendState = null, DepthStencilState depthStencilState = null,
            RasterizerState rasterizerState = null, SamplerState samplerState = null)
        {
            _beginSpriteBatchAfterContext = _hasBegun;
            End();

            var context = GraphicsContext.CreateContext(_graphicsDevice, blendState, depthStencilState, rasterizerState, samplerState);
            context.Disposed += OnGraphicsContextDisposed;

            return context;
        }
        public IDisposable BeginWorld(Matrix world)
        {
            var previousWorld = Effect.World;

            Effect.World = world;
            
            return new ContextDisposable(() =>
            {
                Effect.World = previousWorld;
            });
        }

        public IDisposable BeginProjection(Matrix projection)
        {
            var previousProjection = Effect.Projection;

            Effect.Projection = projection;
            
            return new ContextDisposable(() =>
            {
                Effect.Projection = previousProjection;
            });
        }
        
        public IDisposable BeginViewProjection(Matrix view, Matrix projection)
        {
            var previousView       = Effect.View;
            var previousProjection = Effect.Projection;

            Effect.View = view;
            Effect.Projection = projection;
            
            return new ContextDisposable(() =>
            {
                Effect.View = previousView;
                Effect.Projection = previousProjection;
            });
        }
        
        public IDisposable BeginTransform(Matrix transformMatrix, bool mergeTransform = true)
        {
            var previousRenderMatrix = RenderMatrix;
            if (mergeTransform)
                RenderMatrix = RenderMatrix * transformMatrix;
            else
                RenderMatrix = transformMatrix;

            return new ContextDisposable(() => { RenderMatrix = previousRenderMatrix; });
        }

        public IDisposable BeginClipBounds(Rectangle scissorRectangle, bool mergeBounds = false)
        {
            return new ContextDisposable(() => { });
            //if (scissorRectangle == Rectangle.Empty) return new ContextDisposable(() => {});

            var currentScissorRectangle = Context.ScissorRectangle;

            var rect = Project(scissorRectangle);
            if (mergeBounds)
                rect = Rectangle.Intersect(currentScissorRectangle, rect);

            if (_hasBegun)
            {
                End();
                Begin();
            }

            Context.ScissorRectangle = rect;

            return new ContextDisposable(() =>
            {
                if (_hasBegun)
                {
                    End();
                    Begin();
                }

                Context.ScissorRectangle = currentScissorRectangle;
            });
        }

        private void OnGraphicsContextDisposed(object sender, EventArgs e)
        {
            if (_beginSpriteBatchAfterContext)
            {
                Begin();
            }
        }

        private static RasterizerState CopyRasterizerState(RasterizerState rasterizerState)
        {
            return new RasterizerState()
            {
                CullMode = rasterizerState.CullMode,
                DepthBias = rasterizerState.DepthBias,
                DepthClipEnable = rasterizerState.DepthClipEnable,
                FillMode = rasterizerState.FillMode,
                MultiSampleAntiAlias = rasterizerState.MultiSampleAntiAlias,
                Name = rasterizerState.Name,
                ScissorTestEnable = rasterizerState.ScissorTestEnable,
                SlopeScaleDepthBias = rasterizerState.SlopeScaleDepthBias,
                Tag = rasterizerState.Tag
            };
        }

        #endregion

        #region Drawing

        public void DrawLine(Vector2 from, Vector2 to, Color color, int thickness = 1)
        {
            var length = Vector2.Distance(from, to);
            var angle = (float) Math.Atan2(to.Y - from.Y, to.X - from.X);

            DrawLine(from, length, angle, color, thickness);
        }

        public void DrawLine(Vector2 from, float length, float angle, Color color, int thickness = 1)
        {
            SpriteBatch.Draw(ColorTexture, from, null, color, angle, new Vector2(0f, 0.5f),
                new Vector2(length, thickness), SpriteEffects.None, 0f);
        }

        public void DrawRectangle(Rectangle rectangle, Color color, int thickness)
        {
            DrawRectangle(rectangle, color, new Thickness(thickness));
        }

        public void DrawRectangle(Rectangle rectangle, Color color, Thickness thickness, bool borderOutside = false)
        {
            var borderRectangle = rectangle;

            if (borderOutside)
            {
                borderRectangle += thickness;
            }

            if (thickness.Top > 0)
            {
                DrawLine(borderRectangle.TopLeft(), borderRectangle.TopRight(), color, thickness.Top);
            }

            if (thickness.Right > 0)
            {
                DrawLine(borderRectangle.TopRight(), borderRectangle.BottomRight(), color, thickness.Right);
            }

            if (thickness.Bottom > 0)
            {
                DrawLine(borderRectangle.BottomLeft(), borderRectangle.BottomRight(), color, thickness.Bottom);
            }

            if (thickness.Left > 0)
            {
                DrawLine(borderRectangle.TopLeft(), borderRectangle.BottomLeft(), color, thickness.Left);
            }
        }

        public void FillRectangle(Rectangle rectangle, Color color)
        {
            SpriteBatch.Draw(ColorTexture, rectangle, color);
        }

        public void FillRectangle(Rectangle rectangle, GuiTexture2D texture)
        {
            if (texture.Color.HasValue)
            {
                FillRectangle(rectangle, texture.Color.Value);
            }

            if (texture.Texture == null && texture.TextureResource.HasValue)
            {
                texture.TryResolveTexture(_renderer);
            }

            if (texture.Texture != null)
            {
                FillRectangle(rectangle, texture.Texture, texture.RepeatMode, texture.Scale, texture.Mask);
            }
        }

        public void FillRectangle(Rectangle rectangle, ITexture2D texture,
            TextureRepeatMode repeatMode = TextureRepeatMode.Stretch)
        {
            FillRectangle(rectangle, texture, repeatMode, null, Color.White);
        }

        public void FillRectangle(Rectangle rectangle, ITexture2D texture, TextureRepeatMode repeatMode, Vector2? scale,
            Color? colorMask)
        {
            if (texture?.Texture == null) return;

            var mask = colorMask.HasValue ? colorMask.Value : Color.White;

            if (repeatMode == TextureRepeatMode.NoScaleCenterSlice)
            {
                DrawTextureCenterSliced(rectangle, texture, mask);
            }
            else if (repeatMode == TextureRepeatMode.Tile)
            {
                DrawTextureTiled(rectangle, texture, mask);
            }
            else if (repeatMode == TextureRepeatMode.ScaleToFit)
            {
                DrawTextureScaledToFit(rectangle, texture, mask);
            }
            else if (texture is NinePatchTexture2D ninePatchTexture)
            {
                DrawTextureNinePatch(rectangle, ninePatchTexture, mask);
            }
            else if (scale.HasValue)
            {
                SpriteBatch.Draw(texture.Texture, rectangle.Location.ToVector2(), texture.ClipBounds, mask, 0f,
                    Vector2.Zero, scale.Value, SpriteEffects.None, 0f);
            }
            else
            {
                SpriteBatch.Draw(texture, rectangle, mask);
            }
        }

        #endregion

        #region Drawing - Text

        public void DrawString(Vector2 position, string text, Color color, FontStyle style, float scale = 1f,
            float rotation = 0f, Vector2? rotationOrigin = null, float opacity = 1f)
        {
            Font?.DrawString(SpriteBatch, text, position, color, style, new Vector2(scale), opacity, rotation,
                rotationOrigin);
        }

        public void DrawString(Vector2 position, string text, Color color, FontStyle style, Vector2? scale = null,
            float rotation = 0f, Vector2? rotationOrigin = null, float opacity = 1f)
        {
            Font?.DrawString(SpriteBatch, text, position, color, style, scale, opacity, rotation, rotationOrigin);
        }

        public void DrawString(Vector2 position, string text, IFont font, Color color, FontStyle style,
            float scale = 1f,
            float rotation = 0f, Vector2? rotationOrigin = null, float opacity = 1f)
        {
            font?.DrawString(SpriteBatch, text, position, color, style, new Vector2(scale), opacity, rotation,
                rotationOrigin);
        }

        public void DrawString(Vector2 position, string text, IFont font, Color color, FontStyle style,
            Vector2? scale = null,
            float rotation = 0f, Vector2? rotationOrigin = null, float opacity = 1f)
        {
            font?.DrawString(SpriteBatch, text, position, color, style, scale, opacity, rotation, rotationOrigin);
        }

        #endregion

        #region TextureRepeatMode Helpers

        private void DrawTextureNinePatch(Rectangle rectangle, NinePatchTexture2D ninePatchTexture, Color mask)
        {
            if (ninePatchTexture.Padding == Thickness.Zero)
            {
                SpriteBatch.Draw(ninePatchTexture, rectangle);
                return;
            }

            var sourceRegions = ninePatchTexture.SourceRegions;
            var destRegions = ninePatchTexture.ProjectRegions(rectangle);

            for (var i = 0; i < sourceRegions.Length; i++)
            {
                var srcPatch = sourceRegions[i];
                var dstPatch = destRegions[i];

                if (dstPatch.Width > 0 && dstPatch.Height > 0)
                    SpriteBatch.Draw(ninePatchTexture, dstPatch, srcPatch, mask);
            }
        }

        private void DrawTextureTiled(Rectangle rectangle, ITexture2D texture, Color mask)
        {
            var repeatX = Math.Ceiling((float) rectangle.Width / texture.Width);
            var repeatY = Math.Ceiling((float) rectangle.Height / texture.Height);

            for (int i = 0; i < repeatX; i++)
            for (int j = 0; j < repeatY; j++)
            {
                SpriteBatch.Draw(texture, new Vector2(i * texture.Width, j * texture.Height), mask);
            }
        }

        private void DrawTextureScaledToFit(Rectangle rectangle, ITexture2D texture, Color mask)
        {
            var widthRatio = rectangle.Width / (float) texture.Width;
            var heightRatio = rectangle.Height / (float) texture.Height;

            var resultRatio = Math.Min(heightRatio, widthRatio);
            var scaledSize = new Vector2(texture.Width, texture.Height) * resultRatio;

            var xOffset = (rectangle.Width - scaledSize.X) / 2f;
            var yOffset = (rectangle.Height - scaledSize.Y) / 2f;

            var dstBounds = new Rectangle((int) xOffset, (int) yOffset, (int) scaledSize.X, (int) scaledSize.Y);
            SpriteBatch.Draw(texture, dstBounds, mask);
            // SpriteBatch.Draw(texture, new Vector2(xOffset, yOffset), mask, 0f, Vector2.Zero, new Vector2(resultRatio));
        }

        private void DrawTextureCenterSliced(Rectangle rectangle, ITexture2D texture, Color mask)
        {
            var halfWidth = rectangle.Width / 2f;
            var halfHeight = rectangle.Height / 2f;
            int xOffset = rectangle.X + (int) Math.Max(0, (rectangle.Width - texture.Width) / 2f);
            int yOffset = rectangle.Y + (int) Math.Max(0, (rectangle.Height - texture.Height) / 2f);

            int dstLeftWidth = (int) Math.Floor(halfWidth);
            int dstRightWidth = (int) Math.Ceiling(halfWidth);
            int dstLeftHeight = (int) Math.Floor(halfHeight);
            int dstRightHeight = (int) Math.Ceiling(halfHeight);

            var srcHalfWidth = Math.Min(texture.Width / 2f, halfWidth);
            var srcHalfHeight = Math.Min(texture.Height / 2f, halfHeight);

            var srcX = texture.ClipBounds.X;
            var srcY = texture.ClipBounds.Y;

            int srcLeftWidth = (int) Math.Floor(srcHalfWidth);
            int srcRightWidth = (int) Math.Ceiling(srcHalfWidth);
            int srcLeftHeight = (int) Math.Floor(srcHalfHeight);
            int srcRightHeight = (int) Math.Ceiling(srcHalfHeight);

            // MinY MinX
            SpriteBatch.Draw(texture.Texture, new Rectangle(xOffset, yOffset, dstLeftWidth, dstLeftHeight),
                new Rectangle(srcX, srcY, srcLeftWidth, srcLeftHeight), mask);

            // MinY MaxX
            SpriteBatch.Draw(texture.Texture,
                new Rectangle(xOffset + dstLeftWidth, yOffset, dstRightWidth, dstRightHeight),
                new Rectangle(srcX + texture.Width - srcRightWidth, srcY, srcRightWidth, srcRightHeight), mask);


            // MaxY MinX
            SpriteBatch.Draw(texture.Texture,
                new Rectangle(xOffset, yOffset + dstLeftHeight, dstLeftWidth, dstLeftHeight),
                new Rectangle(srcX, srcY + texture.Height - srcRightHeight, srcLeftWidth, srcLeftHeight), mask);

            // MaxY MaxX
            SpriteBatch.Draw(texture.Texture,
                new Rectangle(xOffset + dstLeftWidth, yOffset + dstRightHeight, dstRightWidth, dstRightHeight),
                new Rectangle(srcX + texture.Width - srcRightWidth, srcY + texture.Height - srcRightHeight,
                    srcRightWidth, srcRightHeight), mask);
        }

        #endregion

        public BasicEffect Effect = null;

        private Texture2D ColorTexture
        {
            get
            {
                if (_colorTexture == null)
                {
                    _colorTexture =
                        GpuResourceManager.CreateTexture2D(1, 1, false, SurfaceFormat.Color);
                    _colorTexture.SetData(new[] {Color.White});
                }

                return _colorTexture;
            }
        }

        #region Disposing

        public void Dispose()
        {
            _colorTexture?.Dispose();
            _colorTexture = null;
            
            SpriteBatch?.Dispose();
            SpriteBatch = null;
        }

        #endregion

        public void UpdateProjection()
        {
            Effect.View = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up);
            Effect.Projection = Matrix.CreateOrthographicOffCenter(0, _renderer.ScaledResolution.ViewportSize.Width, _renderer.ScaledResolution.ViewportSize.Height, 0, 0.1f, 1000.0f);
        }
    }

    internal class ContextDisposable : IDisposable
    {
        private readonly Action _onDisposeAction;

        public ContextDisposable(Action onDisposeAction)
        {
            _onDisposeAction = onDisposeAction;
        }

        public void Dispose()
        {
            _onDisposeAction?.Invoke();
        }
    }
}