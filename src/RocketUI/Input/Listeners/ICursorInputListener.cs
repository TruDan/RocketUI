#if STRIDE
using Stride.Core.Mathematics;
#else
using Microsoft.Xna.Framework;
#endif

namespace RocketUI.Input.Listeners
{
    public interface ICursorInputListener : IInputListener
    {
        Ray GetCursorRay();
    }
}
