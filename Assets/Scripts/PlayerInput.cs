using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania
{
    public class PlayerInput
    {
        public static bool Pressed()
        {
            return Input.GetKeyDown(KeyCode.Z) && !GameManager.instance.autoplay && Conductor.instance.isPlaying;
        }

        public static bool PressedUp()
        {
            return Input.GetKeyUp(KeyCode.Z) && !GameManager.instance.autoplay && Conductor.instance.isPlaying;
        }

        public static bool Pressing()
        {
            return Input.GetKey(KeyCode.Z) && !GameManager.instance.autoplay && Conductor.instance.isPlaying;
        }


        public static bool AltPressed()
        {
            return Input.GetKeyDown(KeyCode.X) && !GameManager.instance.autoplay && Conductor.instance.isPlaying;
        }

        public static bool AltPressedUp()
        {
            return Input.GetKeyUp(KeyCode.X) && !GameManager.instance.autoplay && Conductor.instance.isPlaying;
        }

        public static bool AltPressing()
        {
            return Input.GetKey(KeyCode.X) && !GameManager.instance.autoplay && Conductor.instance.isPlaying;
        }
    }
}