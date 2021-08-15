using System;

namespace RocketUI.Debugger.Exceptions
{
    public class PropertyNotEditableException : RocketDebuggerException
    {
        public PropertyNotEditableException()
        {
        }

        public PropertyNotEditableException(Guid? elementId, string propertyName) : base($"Property '{propertyName}' is not editable on element with ID '{elementId}'")
        {
        }
    }
}