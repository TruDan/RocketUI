using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RocketUI.Input.Listeners;
using SharpVR;

namespace RocketUI.Input
{
    public class InputListenerAdded
    {
        public IInputListener InputListener { get; }

        public InputListenerAdded(IInputListener inputListener)
        {
            InputListener = inputListener;
        }
    }

    public class PlayerInputManager
    {
        public PlayerIndex PlayerIndex { get; }
        public InputType   InputType   { get; private set; }

        public ICursorInputListener CursorInputListener
        {
            get => _cursorInputListener;
        }

        private List<IInputListener> InputListeners { get; } = new List<IInputListener>();

        public List<InputActionBinding> Bindings { get; } = new List<InputActionBinding>();

        public EventHandler<InputListenerAdded> InputListenerAdded;

        private bool                 _isVr = false;
        private ICursorInputListener _cursorInputListener;

        public PlayerInputManager(PlayerIndex playerIndex, InputType inputType = InputType.GamePad)
        {
            PlayerIndex = playerIndex;
            InputType = inputType;
        }

        public void AddListener(IInputListener listener)
        {
            InputListeners.Add(listener);
            if (listener is ICursorInputListener cursorInputListener)
            {
                _cursorInputListener = cursorInputListener;
            }

            InputListenerAdded?.Invoke(this, new InputListenerAdded(listener));
        }

        public bool TryGetListener<TType>(out TType value) where TType : IInputListener
        {
            value = default;

            var first = InputListeners.FirstOrDefault(x => typeof(TType) == x.GetType());
            if (first != default)
            {
                value = (TType) first;
                return true;
            }

            return false;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var inputListener in InputListeners.ToArray())
            {
                inputListener.Update(gameTime);
            }
        }

        public bool IsUp(params InputCommand[] commands)
        {
            return InputListeners.Any(l => commands.Any(l.IsUp));
        }
        public bool IsDown(params InputCommand[] commands)
        {
            return InputListeners.Any(l => commands.Any(l.IsDown));
        }

        public bool IsBeginPress(params InputCommand[] commands)
        {
            return InputListeners.Any(l => commands.Any(l.IsBeginPress));
        }

        public bool IsPressed(params InputCommand[] commands)
        {
            return InputListeners.Any(l => commands.Any(l.IsPressed));
        }

        public Ray GetCursorRay()
        {
            return _cursorInputListener.GetCursorRay();
        }
    }
}