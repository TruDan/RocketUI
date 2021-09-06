using System;
using Microsoft.Xna.Framework;

namespace RocketUI.Platform.MonoGame.DesktopGL.Renderers
{
    public class MonoGameSpriteBatchRenderer : IElementRenderer<RocketElement>,
        IElementRenderer<ProgressBar>
    {
        private RgbaColor color;

        private GuiSpriteBatch _graphics;

        private IDisposable BeginDraw(RocketElement element)
        {
            IDisposable clipDispose = null;
            if (!element.IsVisible)
                return clipDispose;
            if (element.RenderBounds.Size == Point.Zero)
                return clipDispose;


            if (element.ClipToBounds)
                clipDispose = _graphics.BeginClipBounds(element.RenderBounds, true);


            return clipDispose;
        }

        public void _DrawInternal<TElement>(TElement element, Action<TElement> chain = null)
            where TElement : RocketElement
        {
            using (BeginDraw(element))
            {
                _graphics.FillRectangle(element.RenderBounds, element.Background);
                _graphics.FillRectangle(element.RenderBounds, element.BackgroundOverlay);
                chain?.Invoke(element);
                element.ForEachChild(c => c?.Draw());
            }
        }

        public void Draw(RocketElement element) => _DrawInternal(element);
        public void Draw(ProgressBar element) => _DrawInternal(element, e =>
        {
            var bounds = e.RenderBounds;
            var fillWidth = bounds.Width - 2 * e.SpriteSheetSegmentWidth;
            
            bounds = new Rectangle(bounds.X + e.SpriteSheetSegmentWidth, bounds.Y, Math.Max(1, (int)(fillWidth * e.Percent)), bounds.Height);
            _graphics.FillRectangle(bounds, e.Highlight);
        });
    }
}