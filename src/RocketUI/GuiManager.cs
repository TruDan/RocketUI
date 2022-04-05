using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketUI.Input;
using RocketUI.Utilities.Extensions;
using RocketUI.Utilities.Helpers;

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
                        case NotifyCollectionChangedAction.Replace:
                        case NotifyCollectionChangedAction.Add:
                            InitScreen(screen);
                            break;
                    }
                }
            }
            else if (e.OldItems != null)
            {
                foreach (Screen screen in e.OldItems)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Replace:
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
            //SetSize(ScaledResolution.ScaledWidth, ScaledResolution.ScaledHeight);
        }

        public void SetSize(int width, int height)
        {
           // ScaledResolution.ViewportSize = new Size(width, height);
            GuiSpriteBatch.UpdateProjection();

            foreach (var screen in _screens.ToArray())
            {
                if (screen.SizeToWindow)
                {
                    screen.UpdateSize(width, height);
                    continue;
                }
                
                screen.InvalidateLayout();
            }
        }

        public void Reinitialize()
        {
            foreach (var screen in _screens.ToArray())
            {
                screen.Init(GuiRenderer, true);
                screen.InvalidateLayout();
            }
        }
        
        public override void Initialize()
        {
            base.Initialize();
            ScaledResolution.Update();
            Mouse.WindowHandle = Game.Window.Handle;
            
            //Game.Window.TextInput -= WindowOnTextInput;
            Game.Window.TextInput += WindowOnTextInput;
            Game.Window.KeyDown += WindowOnKeyDown;
            
            Init();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Init();
            Reinitialize();
        }

        private void WindowOnKeyDown(object sender, InputKeyEventArgs e)
        {
            if (!e.Key.TryConvertKeyboardInput(out var c))
            {
                FocusManager.OnTextInput(this, new TextInputEventArgs('\0', e.Key));
            }
        }

        private void WindowOnTextInput(object sender, TextInputEventArgs e)
        {
            
            if (char.IsLetterOrDigit(e.Character) || char.IsPunctuation(e.Character) || char.IsSymbol(e.Character) || char.IsWhiteSpace(e.Character))
            {
                if (e.Key == Keys.Tab)
                    return;
                FocusManager.OnTextInput(this, e);
            }
        }

        public void Init()
        {
            var oldSpriteBatch = SpriteBatch;
            SpriteBatch = new SpriteBatch(Game.GraphicsDevice);
            GuiRenderer.Init(Game.GraphicsDevice, ServiceProvider);
            ApplyFont(GuiRenderer.Font);
            
            GuiSpriteBatch?.Dispose();
            GuiSpriteBatch = new GuiSpriteBatch(GuiRenderer, Game.GraphicsDevice, SpriteBatch);
                  
            SetSize(ScaledResolution.ScaledWidth, ScaledResolution.ScaledHeight);
            oldSpriteBatch?.Dispose();
        }

        private bool _doInit = true;
        private DialogBase _activeDialog;

        public void ApplyFont(IFont font)
        {
            GuiRenderer.Font = font;
            GuiSpriteBatch.Font = font;

            _doInit = true;
        }

        private T CreateInstance<T>() where T : class
        {
            return CreateInstance(typeof(T)) as T;
        }

        private T Construct<T>() where T : class
        {
            return CreateInstance(typeof(T)) as T;
        }
        
        private object CreateInstance(Type type)
        {
            var serviceProvider = ServiceProvider;

            object state = null; 

            foreach (var constructor in (type.GetConstructors()))
            {
                bool canConstruct = true;
                object[] passedParameters = new object[0];
                var objparams = constructor.GetParameters();

                passedParameters = new object[objparams.Length];

                for (var index = 0; index < objparams.Length; index++)
                {
                    var param = objparams[index];
                    var p = serviceProvider.GetService(param.ParameterType);

                    if (p != null)
                    {
                        passedParameters[index] = p;
                    }
                    else
                    {
                        canConstruct = false;

                        break;
                    }
                }

                if (canConstruct)
                {
                    state = constructor.Invoke(passedParameters);

                    break;
                }
            }

            return state;
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
            dialog.GuiManager = this;
            ShowDialog(dialog);
            
            return dialog;
        }
        
        public void ShowDialog(DialogBase dialog)
        {
            var activeDialog = _activeDialog;
            if (activeDialog != null)
                HideDialog(activeDialog);
            
            _activeDialog = dialog;

            if (dialog != null)
            {
                dialog.GuiManager = this;
                if (!Game.IsMouseVisible)
                {
                    Game.IsMouseVisible = true;
                    Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
                }

                AddScreen(dialog);
                dialog?.OnShow();
            }
        }

        public void HideDialog(DialogBase dialog)
        {
            if (ActiveDialog == dialog)
            {
                _activeDialog = null;
            }
            
            EnsureDialogClosed(dialog);
        }

        public void HideDialog<TGuiDialog>() where TGuiDialog : DialogBase
        {
            if (ActiveDialog is TGuiDialog activeDialog)
            {
                HideDialog(activeDialog);
                return;
            }
            
            foreach (var screen in _screens.ToArray())
            {
                if (screen is TGuiDialog dialog)
                {
                    EnsureDialogClosed(dialog);
                    
                    //if (ActiveDialog == dialog)
                    //    ActiveDialog = _screens.ToArray().LastOrDefault(e => e is TGuiDialog) as DialogBase;
                }
            }
        }
        
        private void EnsureDialogClosed(DialogBase dialog)
        {
            if (dialog == null) return;
            
            //   Game.IsMouseVisible = false;
            //Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
                    
            RemoveScreen(dialog);
            dialog.OnClose();
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

            foreach (var s in _screens.ToArray())
            {
                if (s == dialog)
                    continue;
                
                yield return s;
            }
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

            var screens = _screens.OrderBy(x => x.ZIndex).ToArray();

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

            DebugHelper.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;

            foreach (var screen in _screens.OrderBy(x => x.ZIndex).ToArray())
            {
                if (screen == null || screen.IsSelfDrawing)
                    continue;
                
                try
                {
                    GuiSpriteBatch.Begin(screen.IsAutomaticallyScaled);

                    screen.Draw(GuiSpriteBatch, gameTime);

                    DrawScreen?.Invoke(this, new GuiDrawScreenEventArgs(screen, gameTime));
                    DebugHelper.DrawScreen(screen);
                }
                finally
                {
                    GuiSpriteBatch.End();
                }
            }
        }
    }
}