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

        public float targetBeat = 0.25f;
        public int stepTimes = 0;
        private bool stepped = false;

        public GameEvent blip = new GameEvent();

        private void Update()
        {
            float normalizedBeat = Conductor.instance.GetPositionFromMargin(targetBeat, 0.5f);
            StateCheck(normalizedBeat);

            if(game.canGo && normalizedBeat > Minigame.LateTime())
            {
                //Fall();
                targetBeat += 100f;
                return;
            }

            if (PlayerInput.Pressed())
            {
                if (state.perfect)
                {
                    Step();
                }
                else if(state.notPerfect())
                {
                    Fall();
                }
                else
                {
                    Step();
                }
            }
        }

        public override void OnAce()
        {
            if (!game.canGo) return;
            
            Step();
        }

        public void Idle()
        {
            stepTimes = 0;
            transform.localScale = new Vector3(1, 1);
            animator.Play("Idle", 0, 0);
        }

        public void Step()
        {
            stepTimes++;

            animator.Play("Step", 0, 0);
            Jukebox.PlayOneShotGame("mrUpbeat/step");

            if (stepTimes % 2 == 1)
            {
                shadows[0].SetActive(false);
                shadows[1].SetActive(true);
                transform.localScale = new Vector3(-1, 1);
            } else
            {
                shadows[0].SetActive(true);
                shadows[1].SetActive(false);
                transform.localScale = new Vector3(1, 1);
            }
        }

        public void Fall()
        {
            animator.Play("Fall", 0, 0);
            Jukebox.PlayOneShot("miss");
        }

        public void Blip()
        {
            Jukebox.PlayOneShotGame("mrUpbeat/blip");
            blipAnimator.Play("Blip", 0, 0);
        }
       

    }
}