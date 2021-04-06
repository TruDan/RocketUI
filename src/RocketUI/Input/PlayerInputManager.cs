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
        private object _listenersLock = new object();

        private List<InputActionBinding> Bindings { get; } = new List<InputActionBinding>();
        private object _bindingsLock = new object();

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
                value = (TType) first;
                return true;
            }

            return false;
        }

        public void Update(GameTime gameTime)
        {
            IInputListener[] listeners;

            lock (_listenersLock)
            {
                listeners = InputListeners.ToArray();
            }

            foreach (var inputListener in listeners)
            {
                inputListener.Update(gameTime);
            }

            CheckTriggeredBindings();
        }

        private void CheckTriggeredBindings()
        {
            InputActionBinding[] bindings;

            lock (_bindingsLock)
            {
                bindings = Bindings.ToArray();
            }

            foreach (var binding in bindings)
            {
                if ((binding.Trigger == InputBindingTrigger.Continuous && IsDown(binding.InputCommand))
                    || (binding.Trigger == InputBindingTrigger.Discrete && IsBeginPress(binding.InputCommand)))
                {
                    if (binding.Predicate())
                    {
                        // triggered
                        HandleBindingTriggered(binding);
                    }
                }
            }
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
            lock (_bindingsLock)
            {
                var binding = new InputActionBinding(command, trigger, predicate, action);
                Bindings.Add(binding);

                return binding;
            }
        }
        public void UnregisterListener(InputActionBinding binding)
        {
            lock (_bindingsLock)
            {
                Bindings.Remove(binding);
            }
        }

        public bool IsUp(params InputCommand[] commands)
        {
            lock (_bindingsLock)
            {
                return InputListeners.Any(l => commands.Any(l.IsUp));
            }
        }

        public bool IsDown(params InputCommand[] commands)
        {
            lock (_bindingsLock)
            {
                return InputListeners.Any(l => commands.Any(l.IsDown));
            }
        }

        public bool IsBeginPress(params InputCommand[] commands)
        {
            lock (_bindingsLock)
            {
                return InputListeners.Any(l => commands.Any(l.IsBeginPress));
            }
        }

        public bool IsPressed(params InputCommand[] commands)
        {
            lock (_bindingsLock)
            {
                return InputListeners.Any(l => commands.Any(l.IsPressed));
            }
        }

        public Ray GetCursorRay()
        {
            IInputListener[] inputListeners;

            lock (_listenersLock)
            {
                inputListeners = InputListeners.ToArray();
            }
            
            foreach (var inputListener in inputListeners.OrderBy(x => x.Order).OfType<ICursorInputListener>())
            {
                var cursorRay = inputListener.GetCursorRay();
                return cursorRay;
            }

            return new Ray();
            
        }
    }
}