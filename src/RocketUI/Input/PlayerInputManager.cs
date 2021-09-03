using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RocketUI.Input.Listeners;
using SharpVR;

namespace RocketUI.Input
{
    public class InputListenerEventArgs
    {
        public IInputListener InputListener { get; }

        public InputListenerEventArgs(IInputListener inputListener)
        {
            InputListener = inputListener;
        }
    }

    public class InputBindingEventArgs : EventArgs
    {
        public PlayerInputManager PlayerInputManager { get; }

        public InputActionBinding Binding      { get; }
        public InputCommand       InputCommand => Binding.InputCommand;
        public PlayerIndex        PlayerIndex  => PlayerInputManager.PlayerIndex;

        public InputBindingEventArgs(PlayerInputManager playerInputManager, InputActionBinding binding)
        {
            PlayerInputManager = playerInputManager;
            Binding = binding;
        }
    }

    public class PlayerInputManager
    {
        public event EventHandler<InputListenerEventArgs> InputListenerAdded;
        public event EventHandler<InputBindingEventArgs>  InputCommandTriggered;

        public PlayerIndex PlayerIndex { get; }
        public InputType   InputType   { get; private set; }

        private List<IInputListener> InputListeners { get; } = new List<IInputListener>();
        private IInputListener[]     _inputListeners      = Array.Empty<IInputListener>();
        private bool                 _inputListenersDirty = false;
        private object               _listenersLock       = new object();

        private List<InputActionBinding> Bindings { get; } = new List<InputActionBinding>();
        private InputActionBinding[]     _bindings = Array.Empty<InputActionBinding>();
        private bool                     _bindingsDirty = false;
        private object                   _bindingsLock  = new object();

        private bool _isVr = false;

        public PlayerInputManager(PlayerIndex playerIndex, InputType inputType = InputType.GamePad)
        {
            PlayerIndex = playerIndex;
            InputType = inputType;
        }

        public void AddListener(IInputListener listener)
        {
            lock (_listenersLock)
            {
                InputListeners.Add(listener);
                _inputListenersDirty = true;
            }

            InputListenerAdded?.Invoke(this, new InputListenerEventArgs(listener));
        }

        public bool TryGetListener<TType>(out TType value) where TType : IInputListener
        {
            value = default;
            IInputListener? first;

            lock (_listenersLock)
            {
                first = InputListeners.FirstOrDefault(x => typeof(TType) == x.GetType());
            }

            if (first != default)
            {
                value = (TType)first;
                return true;
            }

            return false;
        }

        public void Update(GameTime gameTime)
        {
            if (_inputListenersDirty)
            {
                lock (_listenersLock)
                {
                    _inputListeners = InputListeners.OrderBy(x => x.Order).ToArray();
                    _inputListenersDirty = false;
                }
            }

            foreach (var inputListener in _inputListeners)
            {
                inputListener.Update(gameTime);
            }

            CheckTriggeredBindings();
        }

        private void CheckTriggeredBindings()
        {
            if (_bindingsDirty)
            {
                lock (_bindingsLock)
                {
                    _bindings = Bindings.ToArray();
                    _bindingsDirty = false;
                }
            }

            foreach (var binding in _bindings)
            {
                CheckBinding(binding);
            }
        }

        internal bool CheckBinding(InputActionBinding binding)
        {
            if (binding.Trigger == InputBindingTrigger.Continuous && !IsDown(binding.InputCommand))
                return false;

            if (binding.Trigger == InputBindingTrigger.Discrete && !IsBeginPress(binding.InputCommand))
                return false;

            if (binding.Trigger == InputBindingTrigger.Tap && !IsPressed(binding.InputCommand))
                return false;

            if (binding.Predicate())
            {
                // triggered
                HandleBindingTriggered(binding);
                return true;
            }

            return false;
        }

        private void HandleBindingTriggered(InputActionBinding binding)
        {
            binding.Action?.Invoke();
            InputCommandTriggered?.Invoke(this, new InputBindingEventArgs(this, binding));
        }

        public InputActionBinding RegisterListener(InputCommand command, InputBindingTrigger trigger,
            InputActionPredicate                                predicate,
            Action                                              action)
        {
            var binding = new InputActionBinding(command, trigger, predicate, action);

            lock (_bindingsLock)
            {
                Bindings.Add(binding);
                _bindingsDirty = true;
            }

            return binding;
        }

        public InputActionBinding RegisterListener(InputCommand command, InputBindingTrigger trigger, Action action)
        {
            return RegisterListener(command, trigger, InputActionBinding.AlwaysTrue, action);
        }

        public void UnregisterListener(InputActionBinding binding)
        {
            lock (_bindingsLock)
            {
                Bindings.Remove(binding);
                _bindingsDirty = true;
            }
        }

        public bool IsUp(params InputCommand[] commands)
        {
            return _inputListeners.Any(l => commands.Any(l.IsUp));
        }

        public bool IsDown(params InputCommand[] commands)
        {
            return _inputListeners.Any(l => commands.Any(l.IsDown));
        }

        public bool IsBeginPress(params InputCommand[] commands)
        {
            return _inputListeners.Any(l => commands.Any(l.IsBeginPress));
        }

        public bool IsPressed(params InputCommand[] commands)
        {
            return _inputListeners.Any(l => commands.Any(l.IsPressed));
        }

        public Ray GetCursorRay()
        {
            foreach (var inputListener in _inputListeners.OfType<ICursorInputListener>())
            {
                var cursorRay = inputListener.GetCursorRay();
                return cursorRay;
            }

            return new Ray();
        }
    }
}