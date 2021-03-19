using Microsoft.Xna.Framework;
using RocketUI.Utilities.Helpers;

namespace RocketUI.Design.Screens
{
    public class DesignerScreen : Screen
    {
        private readonly StackContainer _toolbar;
        private readonly Container      _screenContainer;

        private GuiDebugHelper _debugHelper => RocketDesignerGame.Instance.GuiManager.DebugHelper;

        public DesignerScreen()
        {
            Children.Add(_toolbar = new StackContainer()
            {
                Orientation = Orientation.Horizontal,
                Anchor = Alignment.TopFill,
                ChildAnchor = Alignment.FillLeft,
                MinHeight = 20,
                Height = 30,
                Background = Color.DarkGray
            });

            Children.Add(_screenContainer = new Container()
            {
                Margin = new Thickness(5,35, 5, 5),
                Anchor = Alignment.Fill
            });

            _toolbar.AddChild(new ToggleButton("Toggle Enabled", (value) => _debugHelper.Enabled = value));
            _toolbar.AddChild(new ToggleButton("Toggle Boundary Boxes", (value) => _debugHelper.BoundingBoxesEnabled = value));
            _toolbar.AddChild(new ToggleButton("Toggle Boundary Boxes Hover", (value) => _debugHelper.BoundingBoxesHoverEnabled = value));
        }

        public void SetScreen(Screen screen)
        {
            _screenContainer.Children.Clear();
            if (screen != null)
            {
                screen.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                screen.Anchor = Alignment.Fill;
                _screenContainer.Children.Add(screen);
            }
        }
    }
}