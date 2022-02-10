using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.RhythmTweezers
{
    public class Tweezers : MonoBehaviour
    {
        public int hitOnFrame;
        [NonSerialized] public Animator anim;
        private Animator vegetableAnim;
        private RhythmTweezers game;
        private bool plucking;

        private void Start()
        {
            anim = GetComponent<Animator>();
            vegetableAnim = RhythmTweezers.instance.VegetableAnimator;

            game = RhythmTweezers.instance;
        }

        private void LateUpdate()
        {
            if (PlayerInput.Pressed())
            {
                if (!plucking) // Did you do a successful pluck earlier in the frame?
                {
                    anim.Play("Tweezers_Pluck", 0, 0);
                }
            }

            plucking = false;
        }

        public void Pluck(bool ace, Hair hair)
        {
            if (ace)
            {
                Jukebox.PlayOneShotGame($"rhythmTweezers/shortPluck{UnityEngine.Random.Range(1, 21)}");

                hair.hairSprite.SetActive(false);
                hair.stubbleSprite.SetActive(true);

                game.hairsLeft--;

                if (game.hairsLeft <= 0)
                    vegetableAnim.Play("HopFinal", 0, 0);
                else
                    vegetableAnim.Play("Hop", 0, 0);

                anim.Play("Tweezers_Pluck_Success", 0, 0);
            }
            else
            {
                Jukebox.PlayOneShotGame($"rhythmTweezers/shortPluck{UnityEngine.Random.Range(1, 21)}");
                Jukebox.PlayOneShot("miss");

                hair.hairSprite.SetActive(false);
                hair.missedSprite.SetActive(true);

                vegetableAnim.Play("Blink", 0, 0);

                anim.Play("Tweezers_Pluck_Fail", 0, 0);
            }

            plucking = true; // Prevents standard pluck from playing in LateUpdate().
        }

        public void LongPluck(bool ace, LongHair hair)
        {
            anim.Play("Tweezers_Pluck", 0, 0);

            if (hitOnFrame > 0) return;

            if (ace)
            {
                float beat = Conductor.instance.songPositionInBeats;
                MultiSound.Play(new MultiSound.Sound[]
                {
                    new MultiSound.Sound($"rhythmTweezers/longPull{UnityEngine.Random.Range(1, 5)}", beat),
                    new MultiSound.Sound("rhythmTweezers/longPullEnd", beat + 0.5f),
                });

                Destroy(hair.gameObject);
            }
        }
    }
}