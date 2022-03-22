using System;
using JetBrains.Annotations;

namespace RocketUI.Debugger.Exceptions
{
    public class RocketDebuggerException : Exception
    {
        public RocketDebuggerException()
        {
        }

        public RocketDebuggerException([CanBeNull] string message) : base(message)
        {
        }

        public RocketDebuggerException([CanBeNull] string message, [CanBeNull] Exception innerException) : base(message, innerException)
        {
        }
    }
}