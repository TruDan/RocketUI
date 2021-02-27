using System;
using Valve.VR;

namespace RocketUI.Input.Listeners
{
    [Flags]
    public enum VRButtons : ulong
    {
        LeftSystem          = Left | EVRButtonId.System,
        LeftApplicationMenu = Left | EVRButtonId.ApplicationMenu,
        LeftGrip            = Left | EVRButtonId.Grip,
        LeftDPadLeft        = Left | EVRButtonId.DPadLeft,
        LeftDPadUp          = Left | EVRButtonId.DPadUp,
        LeftDPadRight       = Left | EVRButtonId.DPadRight,
        LeftDPadDown        = Left | EVRButtonId.DPadDown,
        LeftA               = Left | EVRButtonId.A,
        LeftProximitySensor = Left | EVRButtonId.ProximitySensor,
        LeftAxis0           = Left | EVRButtonId.Axis0,
        LeftAxis1           = Left | EVRButtonId.Axis1,
        LeftAxis2           = Left | EVRButtonId.Axis2,
        LeftAxis3           = Left | EVRButtonId.Axis3,
        LeftAxis4           = Left | EVRButtonId.Axis4,
        LeftSteamVRTouchpad = Left | EVRButtonId.SteamVRTouchpad,
        LeftSteamVRTrigger  = Left | EVRButtonId.SteamVRTrigger,
        LeftDashboardBack   = Left | EVRButtonId.DashboardBack,
        LeftMax             = Left | EVRButtonId.Max,

        RightSystem          = Right | EVRButtonId.System,
        RightApplicationMenu = Right | EVRButtonId.ApplicationMenu,
        RightGrip            = Right | EVRButtonId.Grip,
        RightDPadLeft        = Right | EVRButtonId.DPadLeft,
        RightDPadUp          = Right | EVRButtonId.DPadUp,
        RightDPadRight       = Right | EVRButtonId.DPadRight,
        RightDPadDown        = Right | EVRButtonId.DPadDown,
        RightA               = Right | EVRButtonId.A,
        RightProximitySensor = Right | EVRButtonId.ProximitySensor,
        RightAxis0           = Right | EVRButtonId.Axis0,
        RightAxis1           = Right | EVRButtonId.Axis1,
        RightAxis2           = Right | EVRButtonId.Axis2,
        RightAxis3           = Right | EVRButtonId.Axis3,
        RightAxis4           = Right | EVRButtonId.Axis4,
        RightSteamVRTouchpad = Right | EVRButtonId.SteamVRTouchpad,
        RightSteamVRTrigger  = Right | EVRButtonId.SteamVRTrigger,
        RightDashboardBack   = Right | EVRButtonId.DashboardBack,
        RightMax             = Right | EVRButtonId.Max,

        Left       = 0b00000000,
        Right      = 0b10000000
    }
}