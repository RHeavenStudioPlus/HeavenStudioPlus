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

        public GameObject RotHolder;
        private Vector3 lastRot;

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
                    if (normalizedBeat > 4)
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

            lastRot = RotHolder.transform.eulerAngles;
        }

        public void Hit()
        {
            KarateJoe.instance.HitEffectF(new Vector3(0.9f, 2.0549f));

            Jukebox.PlayOneShotGame("karateman/bombKick");
            hitBeat = Conductor.instance.songPositionInBeats;
            kicked = true;
            RotHolder.transform.eulerAngles = lastRot;
        }
    }
}