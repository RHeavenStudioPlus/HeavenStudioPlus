using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.CropStomp
{
    public class Farmer : PlayerActionObject
    {
        public float nextStompBeat;

        private CropStomp game;

        private void Start()
        {
            game = CropStomp.instance;
        }

        private void Update()
        {
            if (!game.isMarching)
                return;
            
            float normalizedBeat = Conductor.instance.GetPositionFromMargin(nextStompBeat, 1f);

            StateCheck(normalizedBeat);

            if (normalizedBeat > Minigame.LateTime())
            {
                nextStompBeat += 2f;
                ResetState();
                return;
            }

            if (PlayerInput.Pressed())
            {
                if (state.perfect)
                {
                    game.Stomp();
                    nextStompBeat += 2f;
                    ResetState();
                }
                else if (state.notPerfect())
                {
                    nextStompBeat += 2f;
                    ResetState();
                }
            }
        }
    }
}
