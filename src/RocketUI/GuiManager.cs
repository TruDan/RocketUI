using System;
using System.Collections.Generic;
using System.Linq;
using RocketUI.Input;
using RocketUI.Utilities.Helpers;
using SharpVR;

namespace RocketUI
{
    public class GuiDrawScreenEventArgs : EventArgs
    {
        public Screen Screen { get; }

        internal GuiDrawScreenEventArgs(Screen screen)
        {
            Screen = screen;
        }
    }

    public class GuiManager
    {
        public GuiDebugHelper DebugHelper { get; }

        public event EventHandler<GuiDrawScreenEventArgs> DrawScreen;

        public GuiScaledResolution ScaledResolution { get; }
        public GuiFocusHelper      FocusManager     { get; }

        public IGuiRenderer GuiRenderer { get; }

        public InputManager   InputManager   { get; }
        public GuiSpriteBatch GuiSpriteBatch { get; private set; }

        public List<Screen> Screens { get; } = new List<Screen>();

        public DialogBase ActiveDialog
        {
            get => _activeDialog;
            private set
            {
                var oldValue = _activeDialog;

                if (oldValue != null)
                {
                    Game.IsMouseVisible = value != null;
                    Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);

                    RemoveScreen(oldValue);
                    oldValue.OnClose();
                }

                _activeDialog = value;

                if (value == null)
                    return;

                Game.IsMouseVisible = true;
                Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
                AddScreen(value);
                value?.OnShow();
            }
        }

        private IServiceProvider ServiceProvider { get; }

        public GuiManager(IServiceProvider serviceProvider,
            InputManager                   inputManager,
            IGuiRenderer                   guiRenderer
        )
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
            DebugHelper = new GuiDebugHelper(game, this);
        }

        public void InvokeDrawScreen(Screen screen)
        {
            DrawScreen?.Invoke(this, new GuiDrawScreenEventArgs(screen));
        }

        private void ScaledResolutionOnScaleChanged(object sender, UiScaleEventArgs args)
        {
            Init();
        }

        public void SetSize(int width, int height)
        {
            ScaledResolution.ViewportSize = new Size(width, height);
            GuiSpriteBatch.UpdateProjection();

            foreach (var screen in Screens.ToArray())
            {
                if (!screen.SizeToWindow)
                {
                    screen.InvalidateLayout();
                    continue;
                }

                screen.UpdateSize(ScaledResolution.ScaledWidth, ScaledResolution.ScaledHeight);
            }
        }

        public void Initialize()
        {
            Init();
        }

        public void Init()
        {
            GuiRenderer.Init(ServiceProvider);
            ApplyFont(GuiRenderer.Font);

            GuiSpriteBatch?.Dispose();
            GuiSpriteBatch = new GuiSpriteBatch(GuiRenderer, Game.GraphicsDevice, SpriteBatch);

            SetSize(ScaledResolution.ViewportSize.Width, ScaledResolution.ViewportSize.Height);
        }

        private bool       _doInit = true;
        private DialogBase _activeDialog;

        public void ApplyFont(IFont font)
        {
            GuiRenderer.Font = font;
            GuiSpriteBatch.Font = font;

            _doInit = true;
        }

        public void ShowDialog(DialogBase dialog)
        {
            ActiveDialog = dialog;
        }

        public void HideDialog(DialogBase dialog)
        {
            if (ActiveDialog == dialog)
                ActiveDialog = null;
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
            screen.AutoSizeMode = AutoSizeMode.None;
            screen.Anchor = Alignment.Fixed;

            if (screen.SizeToWindow)
                screen.UpdateSize(ScaledResolution.ScaledWidth, ScaledResolution.ScaledHeight);
            else
                screen.InvalidateLayout();

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

        public void Update()
        {
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

            FocusManager.Update();

            foreach (var screen in screens)
            {
                if (screen == null || screen.IsSelfUpdating)
                    continue;

                screen.Update();
            }

            // DebugHelper.Update(gameTime);
        }

        public void Draw()
        {
            foreach (var screen in Screens.ToArray())
            {
                if (screen == null || screen.IsSelfDrawing)
                    continue;

                try
                {
                    GuiSpriteBatch.Begin(screen.IsAutomaticallyScaled);

                    screen.Draw(GuiSpriteBatch);
                    InvokeDrawScreen(screen);
                    //  DebugHelper.DrawScreen(screen);
                }
                finally
                {
                    GuiSpriteBatch.End();
                }
            }
        }
    }
}