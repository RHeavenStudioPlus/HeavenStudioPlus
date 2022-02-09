using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Games.RhythmTweezers
{
    public class Hair : PlayerActionObject
    {
        public float createBeat;

        private void Update()
        {
            float stateBeat = Conductor.instance.GetPositionFromBeat(createBeat, 4f);
            StateCheck(stateBeat);

            if (PlayerInput.Pressed() && RhythmTweezers.instance.Tweezers.GetComponent<Tweezers>().hitOnFrame == 0)
            {
                if (state.perfect)
                {
                    Ace();
                }
            }
        }

        public void Ace()
        {
            Tweezers tweezers = RhythmTweezers.instance.Tweezers.GetComponent<Tweezers>();
            tweezers.Pluck(true, this);

            tweezers.hitOnFrame++;
        }
    }
}