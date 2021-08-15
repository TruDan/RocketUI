using System;

namespace RocketUI.Debugger.Exceptions
{
    public class ElementNotFoundException : RocketDebuggerException
    {
        public ElementNotFoundException()
        {
        }

        public ElementNotFoundException(Guid? elementId) : base($"Element not found with ID '{elementId}'")
        {
        }
    }
}