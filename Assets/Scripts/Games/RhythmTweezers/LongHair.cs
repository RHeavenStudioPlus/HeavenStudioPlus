using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Games.RhythmTweezers
{
    public class LongHair : PlayerActionObject
    {
        public float createBeat;
        private RhythmTweezers game;
        private Tweezers tweezers;

        private void Awake()
        {
            game = RhythmTweezers.instance;
            tweezers = game.Tweezers;
        }

        private void Update()
        {
            float stateBeat = Conductor.instance.GetPositionFromBeat(createBeat + game.tweezerBeatOffset, game.beatInterval);
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
            tweezers.LongPluck(true, this);

            tweezers.hitOnFrame++;
        }
    }
}