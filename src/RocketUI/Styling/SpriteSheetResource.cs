using System;
using System.Collections.Generic;
using Portable.Xaml.Markup;

namespace RocketUI
{
    public class SpriteSheetResource : Dictionary<string, SpriteSheetItem>
    {
        public Uri Source { get; set; }

    }
    public class SpriteSheetItem : TextureResource
    {
        public int X      { get; set; }
        public int Y      { get; set; }
        public int Width  { get; set; }
        public int Height { get; set; }
    }
            
    public class NinePatchTextureSpriteSheetItem : SpriteSheetItem
    {
        public Thickness NinePatchThickness { get; set; }
    }
}