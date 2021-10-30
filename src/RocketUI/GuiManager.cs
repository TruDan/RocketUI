using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketUI.Input;
using RocketUI.Utilities.Helpers;
using SharpVR;
using SimpleInjector;

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
        public GuiDebugHelper DebugHelper { get; }

        public event EventHandler<GuiDrawScreenEventArgs> DrawScreen;

        public GuiScaledResolution ScaledResolution { get; }
        public GuiFocusHelper      FocusManager     { get; }

        public IGuiRenderer GuiRenderer { get; }

        public InputManager InputManager { get; }
        internal SpriteBatch  SpriteBatch  { get; private set; }

        public GuiSpriteBatch GuiSpriteBatch { get; private set; }

        private ObservableCollection<Screen> _screens = new ObservableCollection<Screen>();

        public IReadOnlyCollection<Screen> Screens => _screens;
        public DialogBase ActiveDialog
        {
            get => _activeDialog;
            private set
            {
                var oldValue = _activeDialog;
                
                if (oldValue != null)
                    CloseDialogBase(oldValue);

                _activeDialog = value;

                if (value == null)
                    return;
                
                Game.IsMouseVisible = true;
                Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
                AddScreen(value);
                value?.OnShow();
            }
        }

        private void CloseDialogBase(DialogBase dialog)
        {
            if (dialog != null)
            {
                Game.IsMouseVisible = dialog != null;
                Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
                    
                RemoveScreen(dialog);
                dialog.OnClose();
            }
        }

        private IServiceProvider ServiceProvider { get; }
        public GuiManager(Game game, IGuiRenderer guiRenderer, IServiceProvider serviceProvider
        ) : base(game)
        {
            ServiceProvider = serviceProvider;
            ScaledResolution = new GuiScaledResolution(game)
            {
                GuiScale = 9999
            };
            ScaledResolution.ScaleChanged += ScaledResolutionOnScaleChanged;

            GuiRenderer = guiRenderer;
            guiRenderer.ScaledResolution = ScaledResolution;
            SpriteBatch = new SpriteBatch(Game.GraphicsDevice);

            GuiSpriteBatch = new GuiSpriteBatch(guiRenderer, Game.GraphicsDevice, SpriteBatch);
            //  DebugHelper = new GuiDebugHelper(this);
            DebugHelper = new GuiDebugHelper(game, this);
            
            _screens.CollectionChanged += ScreensOnCollectionChanged;

            InputManager = serviceProvider.GetService<InputManager>();
            FocusManager = new GuiFocusHelper(this, InputManager, game.GraphicsDevice);
        }

        private void ContainerOnResolveUnregisteredType(object sender, UnregisteredTypeEventArgs e)
        {
            var gameService = Game.Services.GetService(e.UnregisteredServiceType);
            if (gameService != null)
            {
                e.Register(() => Game.Services.GetService(e.UnregisteredServiceType));
                return;
            }
        }

        private void InitScreen(Screen screen)
        {
            screen.GuiManager = this;
            screen.AutoSizeMode = AutoSizeMode.None;
            screen.Anchor = Alignment.Fixed;

            if (screen.SizeToWindow)
                screen.UpdateSize(ScaledResolution.ScaledWidth, ScaledResolution.ScaledHeight);
            else
                screen.InvalidateLayout();

            screen.Init(GuiRenderer);
        }
        
        private void ScreensOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Screen screen in e.NewItems)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            InitScreen(screen);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            screen.GuiManager = null;
                            break;
                    }
                }
            }
        }

        public void InvokeDrawScreen(Screen screen, GameTime gameTime)
        {
            DrawScreen?.Invoke(this, new GuiDrawScreenEventArgs(screen, gameTime));
        }

        private void ScaledResolutionOnScaleChanged(object sender, UiScaleEventArgs args)
        {
            Init();
        }

        public void SetSize(int width, int height)
        {
            ScaledResolution.ViewportSize = new Size(width, height);
            GuiSpriteBatch.UpdateProjection();

            foreach (var screen in _screens.ToArray())
            {
                if (!screen.SizeToWindow)
                {
                    screen.InvalidateLayout();
                    continue;
                }

                screen.UpdateSize(ScaledResolution.ScaledWidth, ScaledResolution.ScaledHeight);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            Init();
        }

        public void Init()
        {
            SpriteBatch = new SpriteBatch(Game.GraphicsDevice);
            GuiRenderer.Init(Game.GraphicsDevice, ServiceProvider);
            ApplyFont(GuiRenderer.Font);
            
            GuiSpriteBatch?.Dispose();
            GuiSpriteBatch = new GuiSpriteBatch(GuiRenderer, Game.GraphicsDevice, SpriteBatch);
                  
            SetSize(ScaledResolution.ViewportSize.Width, ScaledResolution.ViewportSize.Height);
        }

        private bool _doInit = true;
        private DialogBase _activeDialog;

        public void ApplyFont(IFont font)
        {
            GuiRenderer.Font = font;
            GuiSpriteBatch.Font = font;

            _doInit = true;
        }

        private T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        private object CreateInstance(Type type)
        {
            return ServiceProvider.GetService(type);
        }

        public T CreateScreen<T>() where T : Screen
        {
            var instance = CreateInstance<T>();
            AddScreen(instance);
            return instance;
        }

        public T CreateDialog<T>() where T : DialogBase
        {
            var dialog = CreateInstance<T>();
            ActiveDialog = dialog;
            
            return dialog;
        }
        
        public void ShowDialog(DialogBase dialog)
        {
            ActiveDialog = dialog;
        }

        public void HideDialog(DialogBase dialog)
        {
            if (ActiveDialog == dialog)
            {
                ActiveDialog = null;
            }
        }

        public void HideDialog<TGuiDialog>() where TGuiDialog : DialogBase
        {
            if (ActiveDialog is TGuiDialog)
            {
                ActiveDialog = null;
                return;
            }
            
            foreach (var screen in _screens.ToArray())
            {
                if (screen is TGuiDialog dialog)
                {
                    CloseDialogBase(dialog);
                    
                    //if (ActiveDialog == dialog)
                    //    ActiveDialog = _screens.ToArray().LastOrDefault(e => e is TGuiDialog) as DialogBase;
                }
            }
        }

        public void AddScreen(Screen screen)
        {
            if (_screens.Contains(screen))
                return;
            
            _screens.Add(screen);
        }

        public void RemoveScreen(Screen screen)
        {
            if (!_screens.Contains(screen))
                return;
            
            _screens.Remove(screen);
        }

        public bool HasScreen(Screen screen)
        {
            return _screens.Contains(screen);
        }

        public IEnumerable<Screen> GetActiveScreens()
        {
            var dialog = _activeDialog;

            if (dialog != null)
            {
                yield return dialog;
                
                if (dialog.AlwaysInFront)
                    yield break;
            }

            foreach(var s in _screens.ToArray())
            yield return s;
        }

        public bool IsScreenActive(Screen screen)
        {
            return GetActiveScreens().Any(x => x == screen);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            ScaledResolution.Update();

            var screens = _screens.ToArray();

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
                if (screen == null || screen.IsSelfUpdating)
                    continue;

                screen.Update(gameTime);
            }

            // DebugHelper.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;

            foreach (var screen in _screens.ToArray())
            {
                if (screen == null || screen.IsSelfDrawing)
                    continue;
                
                try
                {
                    GuiSpriteBatch.Begin(screen.IsAutomaticallyScaled);

                    screen.Draw(GuiSpriteBatch, gameTime);

                    DrawScreen?.Invoke(this, new GuiDrawScreenEventArgs(screen, gameTime));
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