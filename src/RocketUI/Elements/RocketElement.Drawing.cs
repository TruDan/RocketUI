using Microsoft.Xna.Framework;
using RocketUI.Attributes;

namespace RocketUI
{
    public partial class RocketElement
    {
        private float _rotation;
        [DebuggerVisible] public float Rotation
        {
            get => _rotation;
            set => _rotation = MathHelper.ToRadians(value);
        }

        [DebuggerVisible] public virtual Vector2 RotationOrigin { get; set; } = Vector2.Zero;

        [DebuggerVisible] public bool ClipToBounds { get; set; } = false;

        public GuiTexture2D Background
        {
            get
            {
                if (_background == null)
                    _background = new GuiTexture2D();
                return _background;
            }
            set => _background = value;
        }

        public GuiTexture2D BackgroundOverlay
        {
            get
            {
                if (_backgroundOverlay == null)
                    _backgroundOverlay = new GuiTexture2D();
                return _backgroundOverlay;
            }
            set => _backgroundOverlay = value;
        }

        private GuiTexture2D _background;
        private GuiTexture2D _backgroundOverlay;

        public int ZIndex { get; set; }

        protected virtual void OnDraw(GuiSpriteBatch graphics, GameTime gameTime)
        {
            graphics.FillRectangle(RenderBounds, Background);
            
            graphics.FillRectangle(RenderBounds, BackgroundOverlay);
        }
    }
}
