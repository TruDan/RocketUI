using Microsoft.Xna.Framework;

namespace RocketUI.Input.Listeners
{
    public interface ICursorInputListener : IInputListener
    {
        Ray GetCursorRay();
    }
}
