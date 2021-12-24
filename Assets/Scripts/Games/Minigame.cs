using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Games
{
    public class Minigame : MonoBehaviour
    {
        public static float earlyTime = 0.38f, perfectTime = 0.41f, lateTime = 0.535f, endTime = 1f;

        public int firstEnable = 0;

        public virtual void OnGameSwitch()
        {

        }
    }
}