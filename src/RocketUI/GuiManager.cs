using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketUI.Input;
using SharpVR;

namespace RocketUI
{
    public class GuiDrawScreenEventArgs : EventArgs
    {
        public Screen Screen { get; }

        public GameTime GameTime { get; }

        internal GuiDrawScreenEventArgs(Screen screen, GameTime gameTime)
        {
            Screen = screen;
            GameTime = gameTime;
        }
    }

    public class GuiManager : DrawableGameComponent
    {
        // public GuiDebugHelper DebugHelper { get; }

        public event EventHandler<GuiDrawScreenEventArgs> DrawScreen;

        public GuiScaledResolution ScaledResolution { get; }
        public GuiFocusHelper FocusManager { get; }

        public IGuiRenderer GuiRenderer { get; }

        internal InputManager InputManager { get; }
        internal SpriteBatch SpriteBatch { get; private set; }

        public GuiSpriteBatch GuiSpriteBatch { get; private set; }

        public List<Screen> Screens { get; } = new List<Screen>();

        public DialogBase ActiveDialog { get; private set; }

        private IServiceProvider ServiceProvider { get; }

        public GuiManager(Game game,
            IServiceProvider serviceProvider,
            InputManager inputManager,
            IGuiRenderer guiRenderer
        ) : base(game)
        {
            ServiceProvider = serviceProvider;
            InputManager = inputManager;
            ScaledResolution = new GuiScaledResolution(game)
            {
                GuiScale = 9999
            };
            ScaledResolution.ScaleChanged += ScaledResolutionOnScaleChanged;
            
            FocusManager = new GuiFocusHelper(this, InputManager, game.GraphicsDevice);

            GuiRenderer = guiRenderer;
            guiRenderer.ScaledResolution = ScaledResolution;
            SpriteBatch = new SpriteBatch(Game.GraphicsDevice);

            GuiSpriteBatch = new GuiSpriteBatch(guiRenderer, Game.GraphicsDevice, SpriteBatch);
            //  DebugHelper = new GuiDebugHelper(this);

        }

        public void InvokeDrawScreen(Screen screen, GameTime gameTime)
        {
            DrawScreen?.Invoke(this, new GuiDrawScreenEventArgs(screen, gameTime));
        }
        
        private void ScaledResolutionOnScaleChanged(object sender, UiScaleEventArgs args)
        {
            Init();
            SetSize(args.ScaledWidth, args.ScaledHeight);
        }

        public void SetSize(int width, int height)
        {
            GuiRenderer.ScaledResolution.ViewportSize = new Size(width, height);
            GuiSpriteBatch.UpdateProjection();
            
            foreach (var screen in Screens.ToArray())
            {
                screen.UpdateSize(width, height);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            Init();
        }

        public void Init()
        {
         //   SpriteBatch = new SpriteBatch(graphicsDevice);
            GuiRenderer.Init(Game.GraphicsDevice, ServiceProvider);
            ApplyFont(GuiRenderer.Font);

      //      GuiSpriteBatch?.Dispose();
      //      GuiSpriteBatch = new GuiSpriteBatch(GuiRenderer, graphicsDevice, SpriteBatch);
        }

        private bool _doInit = true;

        public void ApplyFont(IFont font)
        {
            GuiRenderer.Font = font;
            GuiSpriteBatch.Font = font;

            _doInit = true;
        }

        public void ShowDialog(DialogBase dialog)
        {
            ActiveDialog?.OnClose();

            if (ActiveDialog != null) RemoveScreen(ActiveDialog);
            ActiveDialog = dialog;
            AddScreen(ActiveDialog);

            Game.IsMouseVisible = true;
        }

        public void HideDialog(DialogBase dialog)
        {
            if (ActiveDialog == dialog)
            {
                dialog?.OnClose();

                Game.IsMouseVisible = false;
                Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);

                RemoveScreen(ActiveDialog);

                ActiveDialog = null;
            }
        }

        public void HideDialog<TGuiDialog>() where TGuiDialog : DialogBase
        {
            foreach (var screen in Screens.ToArray())
            {
                if (screen is TGuiDialog dialog)
                {
                    dialog?.OnClose();
                    Screens.Remove(dialog);
                    if (ActiveDialog == dialog)
                        ActiveDialog = Screens.ToArray().LastOrDefault(e => e is TGuiDialog) as DialogBase;
                }
            }
        }

        public void AddScreen(Screen screen)
        {
            screen.GuiManager = this;
            
            if(!(screen is IGuiScreen3D))
                screen.UpdateSize(ScaledResolution.ScaledWidth, ScaledResolution.ScaledHeight);
            
            screen.Init(GuiRenderer);
            Screens.Add(screen);
        }

        public void RemoveScreen(Screen screen)
        {
            Screens.Remove(screen);
            screen.GuiManager = null;
        }

        public bool HasScreen(Screen screen)
        {
            return Screens.Contains(screen);
        }

        public override void Update(GameTime gameTime)
        {
            if(!Enabled)
                return;
            
            ScaledResolution.Update();
            
            var screens = Screens.ToArray();

            if (_doInit)
            {
                _doInit = false;

                foreach (var screen in screens)
                {
                    screen?.Init(GuiRenderer, true);
                }
            }

            FocusManager.Update(gameTime);

            foreach (var screen in screens)
            {
                if (screen == null || screen is IGuiManaged)
                    continue;

                screen.Update(gameTime);
            }

            // DebugHelper.Update(gameTime);
        }

        private BasicEffect _basicEffect;

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;
            
            IDisposable maybeADisposable = null;

            try
            {
                GuiSpriteBatch.Begin();

                ForEachScreen(screen =>
                {
                    screen.Draw(GuiSpriteBatch, gameTime);

                    DrawScreen?.Invoke(this, new GuiDrawScreenEventArgs(screen, gameTime));
                    //  DebugHelper.DrawScreen(screen);
                });
            }
            finally
            {
                GuiSpriteBatch.End();
            }
        }


        private void ForEachScreen(Action<Screen> action)
        {
            foreach (var screen in Screens.ToArray())
            {
                if (screen == null || screen is IGuiManaged)
                    continue;
                
                action.Invoke(screen);
            }
        }
    }
}