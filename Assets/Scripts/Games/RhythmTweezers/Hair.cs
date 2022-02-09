using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Games.RhythmTweezers
{
    public class Hair : PlayerActionObject
    {
        public float createBeat;
        private Tweezers tweezers;

        private void Awake()
        {
            tweezers = RhythmTweezers.instance.Tweezers;
        }

        private void Update()
        {
            float stateBeat = Conductor.instance.GetPositionFromBeat(createBeat, 4f);
            StateCheck(stateBeat);

            if (PlayerInput.Pressed() && tweezers.hitOnFrame == 0)
            {
                if (state.perfect)
                {
                    Ace();
                }
            }
        }

        public void Ace()
        {
            tweezers.Pluck(true, this);

            tweezers.hitOnFrame++;
        }
    }
}