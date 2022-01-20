using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.KarateMan
{
    public class ComboSound : MonoBehaviour
    {
        public float startBeat;
        private int index;

        private void Update()
        {
            float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(startBeat, 1);

            if (normalizedBeat >= 1 && index < 1)
            {
                Jukebox.PlayOneShotGame("karateman/punchy1");
                index++;
            }
            else if (normalizedBeat >= 1.25f && index < 2)
            {
                Jukebox.PlayOneShotGame("karateman/punchy2");
                index++;
            }
            else if (normalizedBeat >= 1.5f && index < 3)
            {
                Jukebox.PlayOneShotGame("karateman/punchy3");
                index++;
            }
            else if (normalizedBeat >= 1.75f && index < 4)
            {
                Jukebox.PlayOneShotGame("karateman/punchy4");
                index++;
            }
            else if (normalizedBeat >= 2f && index < 5)
            {
                Jukebox.PlayOneShotGame("karateman/ko");
                index++;
            }
            else if (normalizedBeat >= 2.5f && index < 6)
            {
                Jukebox.PlayOneShotGame("karateman/pow");
                index++;
            }
            else if (normalizedBeat >= 3)
            {
                Destroy(this.gameObject);
            }
        }
    }
}