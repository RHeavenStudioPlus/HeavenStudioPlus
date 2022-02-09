using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.RhythmTweezers
{
    public class Tweezers : MonoBehaviour
    {
        public int hitOnFrame;
        private Animator anim;
        private Animator vegetableAnim;

        private void Start()
        {
            anim = GetComponent<Animator>();
            vegetableAnim = RhythmTweezers.instance.VegetableAnimator;
        }

        private void LateUpdate()
        {
            if (PlayerInput.Pressed())
            {
                hitOnFrame = 0;
            }
        }

        public void Pluck(bool ace, Hair hair)
        {
            anim.Play("Tweezers_Pluck", 0, 0);

            if (hitOnFrame > 0) return;
            // tweezer pluck anim here

            if (ace)
            {
                var game = RhythmTweezers.instance;

                Jukebox.PlayOneShotGame($"rhythmTweezers/shortPluck{Random.Range(1, 21)}");
                Destroy(hair.gameObject);

                game.hairsLeft--;

                if (game.hairsLeft <= 0)
                    vegetableAnim.Play("HopFinal", 0, 0);
                else
                    vegetableAnim.Play("Hop", 0, 0);
            }
        }
    }
}