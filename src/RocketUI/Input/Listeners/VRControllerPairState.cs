namespace RocketUI.Input.Listeners
{
    public struct VRControllerPairState
    {
        public VRControllerState Left  { get; }
        public VRControllerState Right { get; }

        public VRControllerPairState(VRControllerState left, VRControllerState right)
        {
            Left = left;
            Right = right;
        }
    }
}