using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Util;

using static JSL;

namespace HeavenStudio.InputSystem
{
    public class InputJoyshock : InputController
    {
        static string[] joyShockNames =
        {
            "Unknown",
            "Joy-Con (L)",
            "Joy-Con (R)",
            "Pro Controller",
            "DualShock 4",
            "DualSense"
        };

        int[] mappings = new int[]
        {
            ButtonMaskUp,
            ButtonMaskDown,
            ButtonMaskLeft,
            ButtonMaskRight,
            ButtonMaskS,
            ButtonMaskE,
            ButtonMaskW,
            ButtonMaskN,
            ButtonMaskL,
            ButtonMaskR,
            ButtonMaskPlus,
        };

        int joyshockHandle;
        int type;
        int splitType;
        string joyshockName;

        InputDirection hatDirectionCurrent;
        InputDirection hatDirectionLast;

        //buttons, sticks, triggers
        JOY_SHOCK_STATE joyBtStateCurrent, joyBtStateLast;
        //gyro and accelerometer
        IMU_STATE joyImuStateCurrent, joyImuStateLast;
        //touchpad
        TOUCH_STATE joyTouchStateCurrent, joyTouchStateLast;

        InputJoyshock otherHalf;

        public InputJoyshock(int handle)
        {
            joyshockHandle = handle;
        }

        public override void InitializeController()
        {
            joyBtStateCurrent = new JOY_SHOCK_STATE();
            joyBtStateLast = new JOY_SHOCK_STATE();
            joyImuStateCurrent = new IMU_STATE();
            joyImuStateLast = new IMU_STATE();
            joyTouchStateCurrent = new TOUCH_STATE();
            joyTouchStateLast = new TOUCH_STATE();

            //FUTURE: remappable controls

            type = JslGetControllerType(joyshockHandle);
            joyshockName = joyShockNames[type];

            splitType = JslGetControllerSplitType(joyshockHandle);
        }

        public override void UpdateState()
        {
            joyBtStateLast = joyBtStateCurrent;
            joyBtStateCurrent = JslGetSimpleState(joyshockHandle);
        }

        public override string GetDeviceName()
        {
            if (otherHalf != null)
                return "Joy-Con Pair";
            return joyshockName;
        }

        public override InputFeatures GetFeatures()
        {
            InputFeatures features = InputFeatures.Style_Pad | InputFeatures.Style_Baton;
            switch (type)
            {
                case TypeJoyConLeft:
                    features |= InputFeatures.Readable_ShellColour | InputFeatures.Readable_ButtonColour | InputFeatures.Writable_PlayerLED | InputFeatures.Extra_SplitControllerLeft | InputFeatures.Extra_HDRumble;
                    break;
                case TypeJoyConRight:
                    features |= InputFeatures.Readable_ShellColour | InputFeatures.Readable_ButtonColour | InputFeatures.Writable_PlayerLED | InputFeatures.Extra_SplitControllerRight | InputFeatures.Extra_HDRumble;
                    break;
                case TypeProController:
                    features |= InputFeatures.Readable_ShellColour | InputFeatures.Readable_ButtonColour | InputFeatures.Readable_LeftGripColour | InputFeatures.Readable_RightGripColour | InputFeatures.Writable_PlayerLED | InputFeatures.Extra_HDRumble;
                    break;
                case TypeDualShock4:
                    features |= InputFeatures.Readable_AnalogueTriggers | InputFeatures.Readable_Pointer | InputFeatures.Writable_LightBar;
                    break;
                case TypeDualSense:
                    features |= InputFeatures.Readable_AnalogueTriggers | InputFeatures.Readable_Pointer | InputFeatures.Writable_PlayerLED | InputFeatures.Writable_LightBar;
                    break;
            }
            features |= InputFeatures.Readable_MotionSensor | InputFeatures.Extra_Rumble | InputFeatures.Style_Pad | InputFeatures.Style_Baton | InputFeatures.Style_Touch;
            return features;
        }

        public override int GetLastButtonDown()
        {
            return BitwiseUtils.FirstSetBit(joyBtStateCurrent.buttons & joyBtStateLast.buttons);
        }

        public override KeyCode GetLastKeyDown()
        {
            return KeyCode.None;
        }

        public override bool GetButton(int button)
        {
            return BitwiseUtils.WantCurrent(joyBtStateCurrent.buttons, mappings[button]);
        }

        public override bool GetButtonDown(int button)
        {
            return BitwiseUtils.WantCurrentAndNotLast(joyBtStateCurrent.buttons, joyBtStateLast.buttons, mappings[button]);
        }

        public override bool GetButtonUp(int button)
        {
            return BitwiseUtils.WantNotCurrentAndLast(joyBtStateCurrent.buttons, joyBtStateLast.buttons, mappings[button]);
        }

        public override float GetAxis(InputAxis axis)
        {
            switch (axis)
            {
                case InputAxis.AxisLTrigger:
                    return joyBtStateCurrent.lTrigger;
                case InputAxis.AxisRTrigger:
                    return joyBtStateCurrent.rTrigger;
                case InputAxis.AxisLStickX:
                    return joyBtStateCurrent.stickLX;
                case InputAxis.AxisLStickY:
                    return joyBtStateCurrent.stickLY;
                case InputAxis.AxisRStickX:
                    return joyBtStateCurrent.stickRX;
                case InputAxis.AxisRStickY:
                    return joyBtStateCurrent.stickRY;
                case InputAxis.TouchpadX:   //isn't updated for now, so always returns 0f
                    //return joyTouchStateCurrent.t0X;
                case InputAxis.TouchpadY:
                    //return joyTouchStateCurrent.t0Y;
                default:
                    return 0f;
            }
        }

        public override bool GetHatDirection(InputDirection direction)
        {
            //todo: check analogue stick hat direction too
            switch (direction)
            {
                case InputDirection.Up:
                    return BitwiseUtils.WantCurrent(joyBtStateCurrent.buttons, ButtonMaskUp);
                case InputDirection.Down:
                    return BitwiseUtils.WantCurrent(joyBtStateCurrent.buttons, ButtonMaskDown);
                case InputDirection.Left:
                    return BitwiseUtils.WantCurrent(joyBtStateCurrent.buttons, ButtonMaskLeft);
                case InputDirection.Right:
                    return BitwiseUtils.WantCurrent(joyBtStateCurrent.buttons, ButtonMaskRight);
                default:
                    return false;
            }
        }

        public override bool GetHatDirectionDown(InputDirection direction)
        {
            //todo: check analogue stick hat direction too
            switch (direction)
            {
                case InputDirection.Up:
                    return BitwiseUtils.WantCurrentAndNotLast(joyBtStateCurrent.buttons, joyBtStateLast.buttons, ButtonMaskUp);
                case InputDirection.Down:
                    return BitwiseUtils.WantCurrentAndNotLast(joyBtStateCurrent.buttons, joyBtStateLast.buttons, ButtonMaskDown);
                case InputDirection.Left:
                    return BitwiseUtils.WantCurrentAndNotLast(joyBtStateCurrent.buttons, joyBtStateLast.buttons, ButtonMaskLeft);
                case InputDirection.Right:
                    return BitwiseUtils.WantCurrentAndNotLast(joyBtStateCurrent.buttons, joyBtStateLast.buttons, ButtonMaskRight);
                default:
                    return false;
            }
        }

        public override bool GetHatDirectionUp(InputDirection direction)
        {
            //todo: check analogue stick hat direction too
            switch (direction)
            {
                case InputDirection.Up:
                    return BitwiseUtils.WantNotCurrentAndLast(joyBtStateCurrent.buttons, joyBtStateLast.buttons, ButtonMaskUp);
                case InputDirection.Down:
                    return BitwiseUtils.WantNotCurrentAndLast(joyBtStateCurrent.buttons, joyBtStateLast.buttons, ButtonMaskDown);
                case InputDirection.Left:
                    return BitwiseUtils.WantNotCurrentAndLast(joyBtStateCurrent.buttons, joyBtStateLast.buttons, ButtonMaskLeft);
                case InputDirection.Right:
                    return BitwiseUtils.WantNotCurrentAndLast(joyBtStateCurrent.buttons, joyBtStateLast.buttons, ButtonMaskRight);
                default:
                    return false;
            }
        }

        public override void SetPlayer(int playerNum)
        {
            if (playerNum == -1)
            {
                this.playerNum = null;
                JslSetPlayerNumber(joyshockHandle, 0);
                return;
            }
            if (type == TypeDualSense)
            {
                if (playerNum <= 4)
                {
                    playerNum = DualSensePlayerMask[playerNum];
                }
            }
            JslSetPlayerNumber(joyshockHandle, playerNum);
            this.playerNum = playerNum;
        }

        public override int? GetPlayer()
        {
            return this.playerNum;
        }

        public int GetHandle()
        {
            return joyshockHandle;
        }

        public void DisconnectJoyshock()
        {
            if (otherHalf != null)
            {
                otherHalf = null;
            }
            JslSetRumble(joyshockHandle, 0, 0);
            JslSetLightColour(joyshockHandle, 0);
            JslSetPlayerNumber(joyshockHandle, 0);
        }
    }
}