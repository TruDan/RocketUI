using System;

namespace RocketUI.Input
{
    public delegate bool InputActionPredicate();

    public enum InputBindingTrigger
    {
        /// <summary>
        ///     Continuously trigger as long as the input is being held down
        /// </summary>
        Continuous,
        
        /// <summary>
        ///     Trigger once when the input is pressed, will only trigger again once the input has been released and then pressed again.
        /// </summary>
        Discrete,
        
        /// <summary>
        ///     Triggered once when the input is pressed and released again
        /// </summary>
        Tap
    }

    public class InputActionBinding
    {
        public static readonly InputActionPredicate AlwaysTrue = () => true;

        public InputBindingTrigger  Trigger      { get; }
        public InputCommand         InputCommand { get; }
        public InputActionPredicate Predicate    { get; private set; }
        public Action               Action       { get; private set; }

        public InputActionBinding(InputCommand command, Action action) : this(command, InputBindingTrigger.Discrete,AlwaysTrue, action)
        {
        }

        public InputActionBinding(InputCommand command, InputActionPredicate predicate, Action action) : this(command, InputBindingTrigger.Discrete, predicate, action)
        {
        }
        
        public InputActionBinding(InputCommand command, InputBindingTrigger trigger, Action action) : this(command,
            trigger, AlwaysTrue, action)
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

        ~InputActionBinding()
        {
            //Stop's reference leaks
            Predicate = null;
            Action = null;
        }
    }
}