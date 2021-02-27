using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLog;
using RocketUI.Input;
using RocketUI.Serialization.Xaml;
using RocketUI.Utilities.Helpers;

namespace RocketUI.Design
{
    public class RocketDesignerGame : Game
    {
        private static readonly Logger         Log = LogManager.GetCurrentClassLogger();
        public                  IGuiRenderer   GuiRenderer    { get; private set; }
        public                  GuiManager     GuiManager     { get; private set; }
        public                  InputManager   InputManager   { get; private set; }
        public                  GuiDebugHelper GuiDebugHelper { get; private set; }

        public GraphicsDeviceManager GraphicsDeviceManager => _graphics;

        private GraphicsDeviceManager _graphics;

        public RocketDesignerGame(IGuiRenderer renderer)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(renderer.GetType().Assembly.Location)), "Content");

            GuiRenderer = renderer;
        }

        private Screen _previewScreen;

        public void PreviewScreen(string screenXamlPath)
        {
            if (_previewScreen != null)
            {
                GuiManager.RemoveScreen(_previewScreen);
                _previewScreen = null;
            }
            if(string.IsNullOrEmpty(screenXamlPath)) return;
            
            Log.Info("Previewing screen {0}", screenXamlPath);
            var screen = new Screen();
            RocketXamlLoader.LoadFromFile(screen, screenXamlPath);
            _previewScreen = screen;
            GuiManager?.AddScreen(_previewScreen);
        }

        protected override void Initialize()
        {
            Services.AddService(typeof(Game), this);

            Window.ClientSizeChanged += (sender, args) =>
            {
                GraphicsDevice.PresentationParameters.BackBufferHeight = Window.ClientBounds.Height;
                GraphicsDevice.PresentationParameters.BackBufferWidth = Window.ClientBounds.Width;
                GuiManager.SetSize(Window.ClientBounds.Width, Window.ClientBounds.Height);
            };

            GraphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
            GraphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            Window.AllowUserResizing = true;
            IsFixedTimeStep = true;
            IsMouseVisible = true;

            GraphicsDeviceManager.ApplyChanges();

            InputManager = new InputManager(this);
            Components.Add(InputManager);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            GpuResourceManager.Init(GraphicsDevice);
            base.LoadContent();

            GuiManager = new GuiManager(this, Services, InputManager, GuiRenderer);
            GuiManager.DrawOrder = 10;
            GuiManager.Visible = true;
            GuiManager.Enabled = true;
            Components.Add(GuiManager);
            GuiManager.Init();
            
            GuiDebugHelper = new GuiDebugHelper(this, GuiManager);
            //GuiDebugHelper.Enabled = true;

            GuiManager.AddScreen(new BackgroundScreen()
            {
                Width = 500,
                Height = 500,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Anchor = Alignment.Fixed
            });

            if (_previewScreen != null)
                GuiManager.AddScreen(_previewScreen);

            _graphics.GraphicsDevice.Viewport = new Viewport(Window.ClientBounds);
            GraphicsDevice.PresentationParameters.BackBufferHeight = Window.ClientBounds.Height;
            GraphicsDevice.PresentationParameters.BackBufferWidth = Window.ClientBounds.Width;
            GuiManager.SetSize(Window.ClientBounds.Width, Window.ClientBounds.Height);
            
            GuiManager.Screens.ForEach(screen => screen.UpdateLayout());
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            GpuResourceManager.Dispose();
        }
    }
}