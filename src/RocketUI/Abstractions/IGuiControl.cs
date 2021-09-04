using System.Numerics;
using RocketUI.Attributes;
using RocketUI.Input;

namespace RocketUI
{
    public interface IGuiControl : IGuiElement
    {
        [DebuggerVisible] bool Enabled { get; }
        [DebuggerVisible] bool CanFocus { get; }
        [DebuggerVisible] bool Focused { get; }
        [DebuggerVisible] bool CanHighlight { get; }
        [DebuggerVisible] bool Highlighted { get; }

        [DebuggerVisible] char AccessKey { get; set; }
        [DebuggerVisible] int TabIndex { get; set; }

        bool Focus();

        void InvokeHighlightActivate();
        void InvokeHighlightDeactivate();

        void InvokeFocusActivate();
        void InvokeFocusDeactivate();
        
        bool InvokeKeyInput(char character);

        void InvokeCursorDown(Vector2    cursorPosition);
        void InvokeCursorUp(Vector2      cursorPosition);
        void InvokeCursorPressed(Vector2 cursorPosition, MouseButton button);
        void InvokeCursorMove(Vector2    cursorPosition, Vector2     previousCursorPosition, bool isCursorDown);

        void InvokeCursorEnter(Vector2 cursorPosition);
        void InvokeCursorLeave(Vector2 cursorPosition);
    }
}
