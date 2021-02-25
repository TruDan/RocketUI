using Microsoft.Xna.Framework;

namespace RocketUI
{
    public class Crosshair : RocketElement
    {
        public Crosshair()
        {
            AutoSizeMode = AutoSizeMode.None;
            Anchor = Alignment.TopLeft;
            Width = 15;
            Height = 15;
        }

        protected override void OnInit(IGuiRenderer renderer)
        {
            Background = GuiTextures.Crosshair;
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (RootScreen.GuiManager != null)
            {
                var pos = RootScreen.GuiManager.FocusManager.CursorPosition.ToPoint();
                Margin = new Thickness(pos.X, pos.Y, 0, 0);
            }
            base.OnUpdate(gameTime);
        }
    }
}
