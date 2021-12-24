using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania
{
    public class PlayerInput
    {
        public static bool Pressed()
        {
            return (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
        }

        public static bool Pressing()
        {
            return (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0));
        }
    }
}