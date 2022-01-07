using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania
{
    public class PlayerInput
    {
        public static bool Pressed()
        {
            return Input.GetKeyDown(KeyCode.Z);
        }

        public static bool PressedUp()
        {
            return Input.GetKeyUp(KeyCode.Z);
        }

        public static bool Pressing()
        {
            return Input.GetKey(KeyCode.Z);
        }
    }
}