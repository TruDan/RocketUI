﻿using System;
using Microsoft.Xna.Framework;
using SharpVR;

namespace RocketUI.Input.Listeners
{
    public class VRControllerInputListener : InputListenerBase<VRControllerPairState, VRButtons>, ICursorInputListener
    {
        public Hand       ActiveHand       { get; private set; }
        public Quaternion ControllerOffset { get; set; }

        private VrContext _vrContext;

        public VRControllerInputListener(PlayerIndex playerIndex) : base(playerIndex)
        {
            RegisterMap(InputCommand.MoveForwards, VRButtons.LeftDPadUp);
            RegisterMap(InputCommand.MoveBackwards, VRButtons.LeftDPadDown);
            RegisterMap(InputCommand.MoveLeft, VRButtons.LeftDPadLeft);
            RegisterMap(InputCommand.MoveRight, VRButtons.LeftDPadRight);

            RegisterMap(InputCommand.LookUp, VRButtons.RightDPadUp);
            RegisterMap(InputCommand.LookDown, VRButtons.RightDPadDown);
            RegisterMap(InputCommand.LookLeft, VRButtons.RightDPadLeft);
            RegisterMap(InputCommand.LookRight, VRButtons.RightDPadRight);
            
            RegisterMap(InputCommand.A, VRButtons.LeftA);
            RegisterMap(InputCommand.A, VRButtons.RightA);
            RegisterMap(InputCommand.LeftClick, VRButtons.LeftSteamVRTrigger);
            RegisterMap(InputCommand.LeftClick, VRButtons.RightSteamVRTrigger);
            RegisterMap(InputCommand.Navigate, VRButtons.LeftSteamVRTrigger);
            RegisterMap(InputCommand.Navigate, VRButtons.RightSteamVRTrigger);
            
            _vrContext = VrContext.Get();
            ActiveHand = Hand.Right;
            ControllerOffset = Quaternion.CreateFromYawPitchRoll(0f, -((float)Math.PI/2f), 0f);
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (CurrentState.Left.ButtonPressed > 0 && CurrentState.Right.ButtonPressed == 0)
            {
                ActiveHand = Hand.Left;
            }
            else if (CurrentState.Left.ButtonPressed == 0 && CurrentState.Right.ButtonPressed > 0)
            {
                ActiveHand = Hand.Right;
            }
        }

        protected override VRControllerPairState GetCurrentState()
        {
            var leftHand  = _vrContext.LeftController.State;
            var rightHand = _vrContext.RightController.State;

            return new VRControllerPairState(VRControllerState.FromSharpVR(leftHand),
                VRControllerState.FromSharpVR(rightHand));
        }

        protected override bool IsButtonDown(VRControllerPairState state, VRButtons buttons)
        {
            if (buttons.HasFlag(VRButtons.Right))
            {
                var vrButtons = (ulong) (1ul << (int) (buttons & ~VRButtons.Right));
                return (state.Right.ButtonPressed & vrButtons) != 0;
            }
            else
            {
                var vrButtons = (ulong) (1ul << (int) buttons);
                return (state.Left.ButtonPressed & vrButtons) != 0;
            }
        }

        protected override bool IsButtonUp(VRControllerPairState state, VRButtons buttons)
        {
            if (buttons.HasFlag(VRButtons.Right))
            {
                var vrButtons = (ulong) (1ul << (int) (buttons & ~VRButtons.Right));
                return (state.Right.ButtonPressed & vrButtons) == 0;
            }
            else
            {
                var vrButtons = (ulong) (1ul << (int) buttons);
                return (state.Left.ButtonPressed & vrButtons) == 0;
            }
        }

        public Ray GetCursorRay()
        {
            if (ActiveHand == Hand.Left)
            {
                return _vrContext.LeftController.GetPointer();
            }
            else if (ActiveHand == Hand.Right)
            {
                return _vrContext.RightController.GetPointer();
            }
            
            return new Ray(Vector3.Zero, Vector3.Forward);
        }
    }
}