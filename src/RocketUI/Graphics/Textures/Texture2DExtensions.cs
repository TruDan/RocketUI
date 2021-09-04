﻿using System.Drawing;

namespace RocketUI
{
    public static class Texture2DExtensions
    {
        public static TextureSlice2D Slice(this Texture2D texture, Rectangle bounds)
        {
            return new TextureSlice2D(texture, bounds);
        }

        public static TextureSlice2D Slice(this Texture2D texture, int x, int y, int width, int height)
        {
            return Slice(texture, new Rectangle(x, y, width, height));

        }

    }
}
