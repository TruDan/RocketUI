using Microsoft.Xna.Framework;
using Valve.VR;

namespace RocketUI.Input.Listeners
{
    public struct VRControllerState
    {
        public static readonly VRControllerState Default;

        public ulong ButtonPressed;
        public ulong ButtonTouched;

        public Vector2[] Axis;

        public static VRControllerState FromSharpVR(Valve.VR.VRControllerState state)
        {
            return new VRControllerState()
            {
                Axis = new[]
                {
                    ToXna(state.rAxis0),
                    ToXna(state.rAxis1),
                    ToXna(state.rAxis2),
                    ToXna(state.rAxis3),
                    ToXna(state.rAxis4),
                },
                ButtonPressed = state.ulButtonPressed,
                ButtonTouched = state.ulButtonTouched,
            };
        }

        private static Vector2 ToXna(VRControllerAxis_t axis)
        {
            return new Vector2(axis.x, axis.y);
        }
    }
}