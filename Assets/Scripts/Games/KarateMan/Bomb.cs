using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.KarateMan
{
    public class Bomb : PlayerActionObject
    {
        private Animator anim;

        private float startBeat;
        private float hitBeat;

        public bool kicked;
        private bool eligible;

        private void Start()
        {
            anim = GetComponent<Animator>();

            startBeat = Conductor.instance.songPositionInBeats;
            eligible = true;
        }

        private void Update()
        {
            if (!kicked)
            {
                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(startBeat, 2.75f);

                anim.Play("BombOut", 0, normalizedBeatAnim);
                anim.speed = 0;

                float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(startBeat, 0.75f);

                StateCheckNoList(normalizedBeat);

                if (normalizedBeat > 1.5f)
                {
                    eligible = false;
                    // explode animation
                    Destroy(this.gameObject);
                }


                if (PlayerInput.PressedUp() && eligible)
                {
                    eligible = false;
                    if (state.perfect)
                    {
                        Hit();
                    }
                    else
                    {
                        Jukebox.PlayOneShot("miss");
                        // some miss animation here or somethin
                    }
                }
            }
            else
            {
                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(hitBeat, 3f);
                anim.Play("BombHit", 0, normalizedBeatAnim);
                anim.speed = 0;

                if (normalizedBeatAnim > 1)
                {
                    Destroy(this.gameObject);
                }
            }
        }

        public void Hit()
        {
            Jukebox.PlayOneShotGame("karateman/bombKick");
            hitBeat = Conductor.instance.songPositionInBeats;
            kicked = true;
        }
    }
}