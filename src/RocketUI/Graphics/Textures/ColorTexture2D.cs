using System;
using System.Drawing;

namespace RocketUI
{
    public class ColorTexture2D : ITexture2D, IDisposable
    {
        private static readonly Size DefaultSize = new Size(1, 1);
        
        public Texture2D Texture    { get; }
        public Rectangle ClipBounds { get; }
    
        public int Width  { get; }
        public int Height { get; }

        public ColorTexture2D(GraphicsDevice graphics, RgbaColor color) : this(graphics, color, DefaultSize)
        {
        }

        public ColorTexture2D(GraphicsDevice graphics, RgbaColor color, Size size)
        {
            Width = size.Width;
            Height = size.Height;
            ClipBounds = new Rectangle(0, 0, Width, Height);
            Texture = CreateTexture(graphics, ClipBounds, color);
        }


        private Texture2D CreateTexture(GraphicsDevice graphics, Rectangle bounds, RgbaColor color)
        {
            var texture = GpuResourceManager.CreateTexture2D( bounds.Width, bounds.Height, false, SurfaceFormat.RgbaColor);
            var data = new RgbaColor[bounds.Width * bounds.Height];

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = color;
            }
            texture.SetData(data);
            return texture;
        }
        
        
        public static implicit operator Texture2D(ColorTexture2D texture)
        {
            return texture.Texture;
        }

        public void Dispose()
        {
            Texture?.Dispose();
        }
    }
}
