using Microsoft.Xna.Framework.Graphics;

namespace RocketUI.Design
{
    public class BackgroundScreen : Screen
    {
        private static ITexture2D _backgroundTexture;
        private static ITexture2D BackgroundTexture
        {
            get
            {
                if (_backgroundTexture == null)
                {
                    var texture = GpuResourceManager.CreateTexture2D(16, 16, false, SurfaceFormat.Color);

                    var a = 0xFF252525;
                    var b = 0xFF606060;
                    texture.SetData(new uint[]
                    {
                        a, a, a, a, a, a, a, a, b, b, b, b, b, b, b, b,
                        a, a, a, a, a, a, a, a, b, b, b, b, b, b, b, b,
                        a, a, a, a, a, a, a, a, b, b, b, b, b, b, b, b,
                        a, a, a, a, a, a, a, a, b, b, b, b, b, b, b, b,
                        a, a, a, a, a, a, a, a, b, b, b, b, b, b, b, b,
                        a, a, a, a, a, a, a, a, b, b, b, b, b, b, b, b,
                        a, a, a, a, a, a, a, a, b, b, b, b, b, b, b, b,
                        a, a, a, a, a, a, a, a, b, b, b, b, b, b, b, b,
                        b, b, b, b, b, b, b, b, a, a, a, a, a, a, a, a,
                        b, b, b, b, b, b, b, b, a, a, a, a, a, a, a, a,
                        b, b, b, b, b, b, b, b, a, a, a, a, a, a, a, a,
                        b, b, b, b, b, b, b, b, a, a, a, a, a, a, a, a,
                        b, b, b, b, b, b, b, b, a, a, a, a, a, a, a, a,
                        b, b, b, b, b, b, b, b, a, a, a, a, a, a, a, a,
                        b, b, b, b, b, b, b, b, a, a, a, a, a, a, a, a,
                        b, b, b, b, b, b, b, b, a, a, a, a, a, a, a, a,
                    });
                    
                    _backgroundTexture = new TextureSlice2D(texture, texture.Bounds);
                }
                
                return _backgroundTexture;
            }
        }

        public BackgroundScreen()
        {
            Anchor = Alignment.Fill;
        }

        protected override void OnInit(IGuiRenderer renderer)
        {
            Background.Texture = BackgroundTexture;
            Background.RepeatMode = TextureRepeatMode.Tile;
            
            base.OnInit(renderer);
        }
    }
}