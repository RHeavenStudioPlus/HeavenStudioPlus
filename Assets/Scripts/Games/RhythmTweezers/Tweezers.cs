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
        private RhythmTweezers game;

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
                Jukebox.PlayOneShotGame($"rhythmTweezers/shortPluck{Random.Range(1, 21)}");
                Destroy(hair.gameObject);

                game.hairsLeft--;

                if (game.hairsLeft <= 0)
                    vegetableAnim.Play("HopFinal", 0, 0);
                else
                    vegetableAnim.Play("Hop", 0, 0);
            }
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
                    new MultiSound.Sound($"rhythmTweezers/longPull{Random.Range(1, 5)}", beat),
                    new MultiSound.Sound("rhythmTweezers/longPullEnd", beat + 0.5f),
                });

                Destroy(hair.gameObject);
            }
        }
    }
}