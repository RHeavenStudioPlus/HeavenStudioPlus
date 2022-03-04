using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RhythmHeavenMania.Games.WizardsWaltz
{
    public class Plant : PlayerActionObject
    {
        public Animator animator;
        public float createBeat;

        private WizardsWaltz game;
        private bool hit = false;
        private bool passed = false;

        private void Awake()
        {
            game = WizardsWaltz.instance;
        }

        private void Start()
        {
            animator.Play("Appear", 0, 0);
        }

        private void Update()
        {
            if (!passed && Conductor.instance.songPositionInBeats > createBeat + game.beatInterval)
            {
                StartCoroutine(FadeOut());
                passed = true;
            }

            if (hit) return;

            float stateBeat = Conductor.instance.GetPositionFromMargin(createBeat + game.beatInterval, 1f);
            StateCheck(stateBeat);

            if (PlayerInput.Pressed(true))
            {
                if (state.perfect)
                {
                    Ace();
                } else if (state.notPerfect())
                {
                    Miss();
                }
            }

        }

        public void Bloom()
        {
            animator.Play("Hit", 0, 0);
        }

        public void Eat()
        {
            animator.Play("Eat", 0, 0);
        }

        public void EatLoop()
        {
            animator.Play("EatLoop", 0, 0);
        }

        public void Ace()
        {
            game.wizard.Magic(this, true);
            hit = true;
        }

        public void Miss()
        {
            game.wizard.Magic(this, false);
            hit = true;
        }

        public override void OnAce()
        {
            Ace();
        }

        public IEnumerator FadeOut()
        {
            yield return new WaitForSeconds(Conductor.instance.secPerBeat * game.beatInterval / 2f);
            Destroy(gameObject);
        }
    }
}