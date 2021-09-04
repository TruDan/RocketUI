using System;

namespace RocketUI.Input.Listeners
{
    public class KeyboardInputListener : InputListenerBase<KeyboardState, Keys>
    {
        public static EventHandler<KeyboardInputListener> InstanceCreated;
        
        public KeyboardInputListener() : this(1)
        {
		}

        public KeyboardInputListener(int playerIndex) : base(playerIndex)
        {
            InstanceCreated?.Invoke(this, this);
        }

        protected override KeyboardState GetCurrentState()
        {
            return Keyboard.GetState();
        }

        protected override bool IsButtonDown(KeyboardState state, Keys buttons)
        {
            return state.IsKeyDown(buttons);
        }

        protected override bool IsButtonUp(KeyboardState state, Keys buttons)
        {
            return state.IsKeyUp(buttons);
        }
    }
}
