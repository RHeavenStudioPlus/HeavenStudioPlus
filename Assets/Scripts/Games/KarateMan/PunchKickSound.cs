using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.KarateMan
{
    public class PunchKickSound : MonoBehaviour
    {
        public float startBeat;
        private int index;

        private void Update()
        {
            float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(startBeat, 1);

            if (normalizedBeat >= 1 && index < 1)
            {
                Jukebox.PlayOneShotGame("karateman/punchKick1");
                index++;
            }
            else if (normalizedBeat >= 1.5f && index < 2)
            {
                Jukebox.PlayOneShotGame("karateman/punchKick2");
                index++;
            }
            else if (normalizedBeat >= 1.75f && index < 3)
            {
                Jukebox.PlayOneShotGame("karateman/punchKick3");
                index++;
            }
            else if (normalizedBeat >= 2.25f && index < 4)
            {
                Jukebox.PlayOneShotGame("karateman/punchKick4");
                index++;
            }
        }
    }
}