using System.Collections.Concurrent;
using Microsoft.Xna.Framework.Graphics;

namespace RocketUI
{
    public static class GpuResourceManager
    {
        private static ConcurrentBag<Texture2D> _managedTextures;

        private static GraphicsDevice _graphics;        
        
        static GpuResourceManager()
        {
            _managedTextures = new ConcurrentBag<Texture2D>();
        }


        public static void Init(GraphicsDevice graphics)
        {
            _graphics = graphics;
        }

        public static Texture2D CreateTexture2D(int width, int height, bool mipmap = false, SurfaceFormat format = SurfaceFormat.Color)
        {
            var texture = new Texture2D(_graphics, width, height, mipmap, format);
            _managedTextures.Add(texture);
            return texture;
        }

        public static void Dispose()
        {
            if (_managedTextures != null)
            {
                while (_managedTextures.TryTake(out var texture))
                {
                    texture.Dispose();
                }
            }
        }
    }
}