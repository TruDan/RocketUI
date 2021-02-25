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

        public GuiTexture2D Background;
        public GuiTexture2D BackgroundOverlay;


        protected virtual void OnDraw(GuiSpriteBatch graphics, GameTime gameTime)
        {
            graphics.FillRectangle(RenderBounds, Background);
            
            graphics.FillRectangle(RenderBounds, BackgroundOverlay);
        }
    }
}
