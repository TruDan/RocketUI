using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLog;
using RocketUI.Design.Screens;
using RocketUI.Input;
using RocketUI.Serialization.Xaml;
using RocketUI.Utilities.Helpers;

namespace RocketUI.Design
{
    public class RocketDesignerGame : Game
    {
        private static readonly Logger         Log = LogManager.GetCurrentClassLogger();

        private static RocketDesignerGame _instance;
        public static  RocketDesignerGame Instance => _instance;
        
        public                  IGuiRenderer   GuiRenderer    { get; private set; }
        public                  GuiManager     GuiManager     { get; private set; }
        public                  InputManager   InputManager   { get; private set; }

        public GraphicsDeviceManager GraphicsDeviceManager => _graphics;

        private DesignerScreen        _designerScreen;
        private GraphicsDeviceManager _graphics;

        public RocketDesignerGame(IGuiRenderer renderer)
        {
            _instance = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(renderer.GetType().Assembly.Location)), "Content");

            GuiRenderer = renderer;
            _designerScreen = new DesignerScreen();
        }

        private Screen _previewScreen;

        public void PreviewScreen(string screenXamlPath)
        {
            if (_exceptionScreen != null)
            {
                _designerScreen.SetScreen(null);
                //GuiManager.RemoveScreen(_exceptionScreen);
                _exceptionScreen = null;
            }

            try
            {
                if (_previewScreen != null)
                {
                    _designerScreen.SetScreen(null);
//                    GuiManager.RemoveScreen(_previewScreen);
                    _previewScreen = null;
                }

                if (string.IsNullOrEmpty(screenXamlPath)) return;

                Log.Info("Previewing screen {0}", screenXamlPath);
                var screen = new Screen();
                RocketXamlLoader.LoadFromFile(screen, screenXamlPath);
                _previewScreen = screen;
                
                _designerScreen.SetScreen(_previewScreen);
                //GuiManager?.AddScreen(_previewScreen);
            }
            catch (Exception ex)
            {
                RenderException(ex);
            }
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

            InputManager = new InputManager(this, Services);
            Components.Add(InputManager);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            GpuResourceManager.Init(GraphicsDevice);
            base.LoadContent();

            GuiManager = new GuiManager(this, GuiRenderer, Services);
            GuiManager.DrawOrder = 10;
            GuiManager.Visible = true;
            GuiManager.Enabled = true;
            Components.Add(GuiManager);
            GuiManager.Init();
            

            GuiManager.AddScreen(new BackgroundScreen()
            {
                Width = 500,
                Height = 500,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Anchor = Alignment.Fixed
            });

            if (_designerScreen != null)
                GuiManager.AddScreen(_designerScreen);

            _graphics.GraphicsDevice.Viewport = new Viewport(Window.ClientBounds);
            GraphicsDevice.PresentationParameters.BackBufferHeight = Window.ClientBounds.Height;
            GraphicsDevice.PresentationParameters.BackBufferWidth = Window.ClientBounds.Width;

            GraphicsDeviceManager.PreferredBackBufferWidth = Window.ClientBounds.Width;
            GraphicsDeviceManager.PreferredBackBufferHeight = Window.ClientBounds.Height;
            GraphicsDeviceManager.ApplyChanges();
            
            GuiManager.SetSize(Window.ClientBounds.Width, Window.ClientBounds.Height);
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

        private ExceptionScreen _exceptionScreen;
        
        public void RenderException(Exception exception)
        {
            if (_exceptionScreen != null)
            {
                _designerScreen.SetScreen(null);
                //GuiManager.RemoveScreen(_exceptionScreen);
                _exceptionScreen = null;
            }

            _exceptionScreen = new ExceptionScreen(exception);
            
            // if(_previewScreen != null)
            //     GuiManager.RemoveScreen(_previewScreen);
            
            _designerScreen.SetScreen(_exceptionScreen);
            //GuiManager.Screens.Add(_exceptionScreen);
        }
    }

    public class ExceptionScreen : Screen
    {
        private readonly StackContainer _container;
        public ExceptionScreen(Exception exception)
        {
            AddChild(_container = new StackContainer()
            {
                Anchor = Alignment.MiddleCenter,
                Margin = new Thickness(10),
                BackgroundOverlay = Color.Black * 0.5f,
                Orientation = Orientation.Vertical
            });
            
            _container.AddChild(new TextElement("Exception")
            {
                Scale = 2.0f,
                TextColor = Color.OrangeRed
            });

            _container.AddChild(new TextElement(exception.Message)
            {
                TextColor = Color.OrangeRed
            });
            
            if (exception.StackTrace != null)
            {
                StackContainer stacktrace;
                _container.AddChild(stacktrace = new StackContainer()
                {
                    Orientation = Orientation.Vertical
                });
                foreach (var s in exception.StackTrace.Split('\n'))
                {
                    stacktrace.AddChild(new TextElement(s)
                    {
                        TextColor = Color.OrangeRed,
                        Scale = 0.75f
                    });
                }
            }
        }

        public ExceptionScreen()
        {
            
        }
    }
}