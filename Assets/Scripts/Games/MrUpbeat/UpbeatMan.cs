using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Starpelly;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.MrUpbeat
{
    public class UpbeatMan : PlayerActionObject
    {
        [Header("References")]
        public MrUpbeat game;
        public Animator animator;
        public Animator blipAnimator;
        public GameObject[] shadows;

        public int stepTimes = 0;

        public GameEvent blip = new GameEvent();

        private void Update()
        {
            float normalizedBeat = Conductor.instance.GetPositionFromBeat(game.nextBeat, 0.5f);
            StateCheck(normalizedBeat);
            CheckIfFall(normalizedBeat);

            if (PlayerInput.Pressed(true))
            {
                if (state.perfect)
                {
                    Step();
                } else if (state.notPerfect())
                {
                    Fall();
                }
            }
        }

        public void ProgressBeat()
        {
            game.nextBeat += 1f;
            Blip();
        }

        public override void OnAce()
        {
            Step();
        }

        public void Step()
        {
            if (!game.canGo) return;

            stepTimes++;

            Jukebox.PlayOneShotGame("mrUpbeat/step");

            if (stepTimes % 2 == 1)
                transform.localScale = new Vector3(-1, 1);
            else
                transform.localScale = new Vector3(1, 1);

            ProgressBeat();
        }

        public void Fall()
        {
            if (!game.canGo) return;

            Jukebox.PlayOneShot("miss");
        }

        private void CheckIfFall(float normalizedBeat)
        {
            if (normalizedBeat > Minigame.LateTime())
            {
                Fall();
                ProgressBeat();
            }
        }

        public void Blip()
        {
            Jukebox.PlayOneShotGame("mrUpbeat/blip");
            blipAnimator.Play("Blip", 0, 0);
        }
       

    }
}