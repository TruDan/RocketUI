using System;

namespace RocketUI.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DebuggerVisibleAttribute : Attribute
    {

        public bool Visible { get; set; } = true;

        public DebuggerVisibleAttribute()
        {

        }


    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DebuggerIgnoreAttribute : DebuggerVisibleAttribute
    {
        public DebuggerIgnoreAttribute() : base()
        {
            Visible = false;
        }
    }
}
