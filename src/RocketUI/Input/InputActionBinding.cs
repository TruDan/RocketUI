using System;

namespace RocketUI.Input
{
    public delegate bool InputActionPredicate();

    public enum InputBindingTrigger
    {
        Continuous,
        Discrete,
    }

    public class InputActionBinding
    {
        public static readonly InputActionPredicate AlwaysTrue = () => true;

        public InputBindingTrigger  Trigger      { get; }
        public InputCommand         InputCommand { get; }
        public InputActionPredicate Predicate    { get; }
        public Action               Action       { get; }

        public InputActionBinding(InputCommand command, Action action) : this(command, InputBindingTrigger.Discrete,AlwaysTrue, action)
        {
        }

        public InputActionBinding(InputCommand command, InputActionPredicate predicate, Action action) : this(command, InputBindingTrigger.Discrete, predicate, action)
        {
        }
        
        public InputActionBinding(InputCommand command, InputBindingTrigger trigger, Action action) : this(command,
            InputBindingTrigger.Discrete, AlwaysTrue, action)
        {
        }
        public InputActionBinding(InputCommand command, InputBindingTrigger trigger, InputActionPredicate predicate,
            Action                             action)
        {
            Trigger = trigger;
            InputCommand = command;
            Predicate = predicate;
            Action = action;
        }
    }
}