using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;

namespace RocketUI.Input.Listeners
{
    public abstract class InputListenerBase<TState, TButtons> : IInputListener
    {
        public PlayerIndex PlayerIndex { get; }

        public int Order { get; set; } = 0;

        public IReadOnlyDictionary<InputCommand, List<TButtons>> ButtonMap => new ReadOnlyDictionary<InputCommand, List<TButtons>>(_buttonMap);

        private readonly IDictionary<InputCommand, List<TButtons>> _buttonMap = new Dictionary<InputCommand, List<TButtons>>();

        protected TState PreviousState, CurrentState;

        protected abstract TState GetCurrentState();

        protected abstract bool IsButtonDown(TState state, TButtons buttons);
        protected abstract bool IsButtonUp(TState state, TButtons buttons);

        protected InputListenerBase(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public void Update(GameTime gameTime)
        {
            PreviousState = CurrentState;
            CurrentState = GetCurrentState();

            OnUpdate(gameTime);
        }

        protected virtual void OnUpdate(GameTime gameTime)
        {

        }

        public void RegisterMap(InputCommand command, TButtons buttons)
        {
            if (_buttonMap.ContainsKey(command))
            {
                if (!_buttonMap[command].Contains(buttons))
                {
                    _buttonMap[command].Add(buttons);
                }
            }
            else
            {
                _buttonMap.Add(command, new List<TButtons>(){buttons});
            }
        }

        public void RemoveMap(InputCommand command)
        {
            if (_buttonMap.ContainsKey(command))
                _buttonMap.Remove(command);
        }

        public void ClearMap()
        {
            _buttonMap.Clear();
        }

        public bool IsAnyDown(params TButtons[] buttons)
        {
            return buttons.Any(button => IsButtonDown(CurrentState, button));
        }
        
        public bool IsAnyUp(params TButtons[] buttons)
        {
            return buttons.Any(button => IsButtonUp(CurrentState, button));
        }
        
        public bool IsAnyBeginPress(params TButtons[] buttons)
        {
            return buttons.Any(button => IsButtonDown(CurrentState, button) && IsButtonUp(PreviousState, button));
        }
        
        public bool IsAnyPressed(params TButtons[] buttons)
        {
            return buttons.Any(button => IsButtonUp(CurrentState, button) && IsButtonDown(PreviousState, button));
        }
        
        public bool IsDown(InputCommand command)
        {
            return (TryGetButtons(command, out var buttons) && buttons.Any(button => IsButtonDown(CurrentState, button)));
        }

        public bool IsUp(InputCommand command)
        {
            return (TryGetButtons(command, out var buttons) && buttons.Any(button => IsButtonUp(CurrentState, button)));
        }
        
        public bool IsBeginPress(InputCommand command)
        {
            return (TryGetButtons(command, out var buttons) && buttons.Any(button => IsButtonDown(CurrentState, button) && IsButtonUp(PreviousState, button)));
        }

        public bool IsPressed(InputCommand command)
        {
            return (TryGetButtons(command, out var buttons) && buttons.Any(button => IsButtonUp(CurrentState, button) && IsButtonDown(PreviousState, button)));
        }

        private bool TryGetButtons(InputCommand command, out TButtons[] buttons)
        {
            if (_buttonMap.TryGetValue(command, out var btns))
            {
                buttons = btns.ToArray();
                return true;
            }

            buttons = new TButtons[0];
            return false;
        }
    }
}
