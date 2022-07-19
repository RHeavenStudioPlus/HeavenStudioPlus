using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static JSL;

namespace HeavenStudio
{
    public class PlayerInput
    {
        ///////////////////////////////
        ////TEMPORARY JSL FUNCTIONS////
        ///////////////////////////////
        static string[] joyShockNames =
        {
            "Unknown",
            "Joy-Con (L)",
            "Joy-Con (R)",
            "Pro Controller",
            "DualShock 4",
            "DualSense"
        };

        static int numDevicesFound = 0;
        static int numDevicesConnected = 0;
        static int[] deviceHandles;

        static Dictionary<int, JOY_SHOCK_STATE> joyBtStateCurrent;
        static Dictionary<int, JOY_SHOCK_STATE> joyBtStateLast;

        static Dictionary<int, IMU_STATE> joyImuStateCurrent;
        static Dictionary<int, IMU_STATE> joyImuStateLast;

        public static int InitJoyShocks()
        {
            //flush old joyshocks
            Debug.Log("Flushing possible JoyShocks...");
            JslDisconnectAndDisposeAll();

            numDevicesFound = 0;
            numDevicesConnected = 0;

            numDevicesFound = JslConnectDevices();
            if (numDevicesFound > 0)
            {
                deviceHandles = new int[numDevicesFound];
                numDevicesConnected = JslGetConnectedDeviceHandles(deviceHandles, numDevicesFound);
                joyBtStateCurrent = new Dictionary<int, JOY_SHOCK_STATE>();
                joyBtStateLast = new Dictionary<int, JOY_SHOCK_STATE>();
                if (numDevicesConnected < numDevicesFound)
                {
                    Debug.Log("Found " + numDevicesFound + " JoyShocks, but only " + numDevicesConnected + " are connected.");
                }
                else
                {
                    Debug.Log("Found " + numDevicesFound + " JoyShocks.");
                    Debug.Log("Connected " + numDevicesConnected + " JoyShocks.");
                }

                foreach (int i in deviceHandles)
                {
                    Debug.Log("Setting up JoyShock: " + joyShockNames[JslGetControllerType(i)] + " ( Player " + i + ", type " + JslGetControllerType(i) + " )");
                }
                return numDevicesConnected;
            }
            else
            {
                Debug.Log("No JoyShocks found.");
                return 0;
            }
        }

        public static string GetJoyShockName(int playerNum)
        {
            return joyShockNames[JslGetControllerType(deviceHandles[playerNum])];
        }

        public static void UpdateJoyShocks()
        {
            if (deviceHandles == null || numDevicesConnected == 0) return;
            foreach (var id in deviceHandles)
            {
                if (joyBtStateCurrent.ContainsKey(id))
                {
                    joyBtStateLast[id] = joyBtStateCurrent[id];
                }
                else
                {
                    joyBtStateLast[id] = new JOY_SHOCK_STATE();
                }
                joyBtStateCurrent[id] = JslGetSimpleState(id);
            }
        }

        //TODO: refactor to allow controller selection (and for split controllers, multiple controllers)
        static bool GetJoyBtDown(int bt)
        {
            if (deviceHandles == null || numDevicesConnected <= 0) // <= player number in the future
            {
                return false;
            }
            bt = 1 << bt;
            int p1Id = deviceHandles[0];
            try
            {
                int curBt = joyBtStateCurrent[p1Id].buttons;
                int oldBt = joyBtStateLast[p1Id].buttons;
                return ((curBt & bt) == bt) && ((oldBt & bt) != bt);
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        static bool GetJoyBt(int bt)
        {
            if (deviceHandles == null || numDevicesConnected <= 0) // <= player number in the future
            {
                return false;
            }
            bt = 1 << bt;
            int p1Id = deviceHandles[0];
            try
            {
                int curBt = joyBtStateCurrent[p1Id].buttons;
                return (curBt & bt) == bt;
            }
            catch (System.Exception)
            {
                return false;
            }
            
        }

        static bool GetJoyBtUp(int bt)
        {
            if (deviceHandles == null || numDevicesConnected <= 0) // <= player number in the future
            {
                return false;
            }
            bt = 1 << bt;
            int p1Id = deviceHandles[0];
            try
            {
                int curBt = joyBtStateCurrent[p1Id].buttons;
                int oldBt = joyBtStateLast[p1Id].buttons;
                return ((curBt & bt) != bt) && ((oldBt & bt) == bt);
            }
            catch (System.Exception)
            {
                return false;
            }
            
        }

        ////END TEMPORARY JSL FUNCTIONS
        ///////////////////////////////

        //Clockwise
        public const int UP = 0;
        public const int RIGHT = 1;
        public const int DOWN = 2;
        public const int LEFT = 3;

        // The autoplay isn't activated AND
        // The song is actually playing AND
        // The GameManager allows you to Input
        public static bool playerHasControl()
        {
            return !GameManager.instance.autoplay && Conductor.instance.isPlaying && GameManager.instance.canInput;
        }

        /*--------------------*/
        /* MAIN INPUT METHODS */
        /*--------------------*/

        // BUTTONS
        //TODO: refactor for controller and custom binds, currently uses temporary button checks

        public static bool Pressed(bool includeDPad = false)
        {
            bool keyDown = Input.GetKeyDown(KeyCode.Z) || GetJoyBtDown(ButtonMaskE) || (includeDPad && GetAnyDirectionDown());
            return keyDown && !GameManager.instance.autoplay && Conductor.instance.isPlaying && GameManager.instance.canInput ;
        }

        public static bool PressedUp(bool includeDPad = false)
        {
            bool keyUp = Input.GetKeyUp(KeyCode.Z) || GetJoyBtUp(ButtonMaskE) || (includeDPad && GetAnyDirectionUp());
            return keyUp && !GameManager.instance.autoplay && Conductor.instance.isPlaying && GameManager.instance.canInput;
        }

        public static bool Pressing(bool includeDPad = false)
        {
            bool pressing = Input.GetKey(KeyCode.Z) || GetJoyBt(ButtonMaskE) || (includeDPad && GetAnyDirection());
            return pressing && !GameManager.instance.autoplay && Conductor.instance.isPlaying && GameManager.instance.canInput;
        }


        public static bool AltPressed()
        {
            bool down = Input.GetKeyDown(KeyCode.X) || GetJoyBtDown(ButtonMaskS);
            return down && playerHasControl();
        }

        public static bool AltPressedUp()
        {
            bool up = Input.GetKeyUp(KeyCode.X) || GetJoyBtUp(ButtonMaskS);
            return up && playerHasControl();
        }

        public static bool AltPressing()
        {
            bool pressing = Input.GetKey(KeyCode.X) || GetJoyBt(ButtonMaskS);
            return pressing && playerHasControl();
        }

        //Directions

        public static bool GetAnyDirectionDown()
        {
            return (Input.GetKeyDown(KeyCode.UpArrow)
                    || Input.GetKeyDown(KeyCode.DownArrow)
                    || Input.GetKeyDown(KeyCode.LeftArrow)
                    || Input.GetKeyDown(KeyCode.RightArrow)
                    
                    || GetJoyBtDown(ButtonMaskUp)
                    || GetJoyBtDown(ButtonMaskDown)
                    || GetJoyBtDown(ButtonMaskLeft)
                    || GetJoyBtDown(ButtonMaskRight)
                    ) && playerHasControl();

        }

        public static bool GetAnyDirectionUp()
        {
            return (Input.GetKeyUp(KeyCode.UpArrow)
                    || Input.GetKeyUp(KeyCode.DownArrow)
                    || Input.GetKeyUp(KeyCode.LeftArrow)
                    || Input.GetKeyUp(KeyCode.RightArrow)
                    
                    || GetJoyBtUp(ButtonMaskUp)
                    || GetJoyBtUp(ButtonMaskDown)
                    || GetJoyBtUp(ButtonMaskLeft)
                    || GetJoyBtUp(ButtonMaskRight)
                    ) && playerHasControl();

        }

        public static bool GetAnyDirection()
        {
            return (Input.GetKey(KeyCode.UpArrow)
                    || Input.GetKey(KeyCode.DownArrow)
                    || Input.GetKey(KeyCode.LeftArrow)
                    || Input.GetKey(KeyCode.RightArrow)
                    
                    || GetJoyBt(ButtonMaskUp)
                    || GetJoyBt(ButtonMaskDown)
                    || GetJoyBt(ButtonMaskLeft)
                    || GetJoyBt(ButtonMaskRight)
                    ) && playerHasControl();

        }

        public static bool GetSpecificDirectionDown(int direction)
        {
            KeyCode targetCode = getKeyCode(direction);
            if (targetCode == KeyCode.None) return false;

            int targetMask = getButtonMask(direction);
            if (targetMask == 0) return false;
            return (Input.GetKeyDown(targetCode) || GetJoyBtDown(targetMask)) && playerHasControl();
        }

        public static bool GetSpecificDirectionUp(int direction)
        {
            KeyCode targetCode = getKeyCode(direction);
            if (targetCode == KeyCode.None) return false;

            int targetMask = getButtonMask(direction);
            if (targetMask == 0) return false;

            return (Input.GetKeyUp(targetCode) || GetJoyBtUp(targetMask)) && playerHasControl();
        }


        private static KeyCode getKeyCode(int direction)
        {
            KeyCode targetKeyCode;

            switch (direction)
            {
                case PlayerInput.UP: targetKeyCode = KeyCode.UpArrow; break;
                case PlayerInput.DOWN: targetKeyCode = KeyCode.DownArrow; break;
                case PlayerInput.LEFT: targetKeyCode = KeyCode.LeftArrow; break;
                case PlayerInput.RIGHT: targetKeyCode = KeyCode.RightArrow; break;
                default: targetKeyCode = KeyCode.None; break;
            }

            return targetKeyCode;
        }

        private static int getButtonMask(int direction)
        {
            int targetKeyCode;

            switch (direction)
            {
                case PlayerInput.UP: targetKeyCode = ButtonMaskUp; break;
                case PlayerInput.DOWN: targetKeyCode = ButtonMaskDown; break;
                case PlayerInput.LEFT: targetKeyCode = ButtonMaskLeft; break;
                case PlayerInput.RIGHT: targetKeyCode = ButtonMaskRight; break;
                default: targetKeyCode = -1; break;
            }

            return targetKeyCode;
        }
        
    }
}