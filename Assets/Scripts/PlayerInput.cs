using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeavenStudio
{
    public class PlayerInput
    {

        public static bool Pressed(bool includeDPad = false)
        {
            bool keyDown = Input.GetKeyDown(KeyCode.Z) || (includeDPad && GetAnyDirectionDown());
            return keyDown && !GameManager.instance.autoplay && Conductor.instance.isPlaying && GameManager.instance.canInput ;
        }

        public static bool PressedUp(bool includeDPad = false)
        {
            bool keyUp = Input.GetKeyUp(KeyCode.Z) || (includeDPad && GetAnyDirectionUp());
            return keyUp && !GameManager.instance.autoplay && Conductor.instance.isPlaying && GameManager.instance.canInput;
        }

        public static bool Pressing(bool includeDPad = false)
        {
            bool pressing = Input.GetKey(KeyCode.Z) || (includeDPad && GetAnyDirection());
            return pressing && !GameManager.instance.autoplay && Conductor.instance.isPlaying && GameManager.instance.canInput;
        }


        public static bool AltPressed()
        {
            return Input.GetKeyDown(KeyCode.X) && playerHasControl();
        }

        public static bool AltPressedUp()
        {
            return Input.GetKeyUp(KeyCode.X) && playerHasControl();
        }

        public static bool AltPressing()
        {
            return Input.GetKey(KeyCode.X) && playerHasControl();
        }

        public static bool GetAnyDirectionDown()
        {
            return (Input.GetKeyDown(KeyCode.UpArrow)
                    || Input.GetKeyDown(KeyCode.DownArrow)
                    || Input.GetKeyDown(KeyCode.LeftArrow)
                    || Input.GetKeyDown(KeyCode.RightArrow)) && playerHasControl();

        }

        public static bool GetAnyDirectionUp()
        {
            return (Input.GetKeyUp(KeyCode.UpArrow)
                    || Input.GetKeyUp(KeyCode.DownArrow)
                    || Input.GetKeyUp(KeyCode.LeftArrow)
                    || Input.GetKeyUp(KeyCode.RightArrow)) && playerHasControl();

        }

        public static bool GetAnyDirection()
        {
            return (Input.GetKey(KeyCode.UpArrow)
                    || Input.GetKey(KeyCode.DownArrow)
                    || Input.GetKey(KeyCode.LeftArrow)
                    || Input.GetKey(KeyCode.RightArrow)) && playerHasControl();

        }


        // The autoplay isn't activated AND
        // The song is actually playing AND
        // The GameManager allows you to Input
        public static bool playerHasControl()
        {
            return !GameManager.instance.autoplay && Conductor.instance.isPlaying && GameManager.instance.canInput;
        }
    }
}