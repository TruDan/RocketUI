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

        private List<IInputListener> InputListeners { get; } = new List<IInputListener>();

        public List<InputActionBinding> Bindings { get; } = new List<InputActionBinding>();

        public EventHandler<InputListenerAdded> InputListenerAdded;

        private bool _isVr = false;

        public PlayerInputManager(PlayerIndex playerIndex, InputType inputType = InputType.GamePad)
        {
            PlayerIndex = playerIndex;
            InputType = inputType;
        }

        public void AddListener(IInputListener listener)
        {
            InputListeners.Add(listener);

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
            foreach (var inputListener in InputListeners.OrderBy(x => x.Order).OfType<ICursorInputListener>())
            {
                var cursorRay = inputListener.GetCursorRay();
                return cursorRay;
            }

            return new Ray();
        }
    }
}