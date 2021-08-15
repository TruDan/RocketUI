using System;

namespace RocketUI.Debugger.Exceptions
{
    public class PropertyNotFoundException : RocketDebuggerException
    {
        public PropertyNotFoundException()
        {
        }

        public PropertyNotFoundException(Guid? elementId, string propertyName) : base($"Property '{propertyName}' not found on element with ID '{elementId}'")
        {
        }
    }
}