using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Games.KarateMan
{
    public class Bomb : MonoBehaviour
    {
        private Animator anim;

        private float startBeat;
        private float hitBeat;

        public bool hit;

        private void Start()
        {
            anim = GetComponent<Animator>();

            startBeat = Conductor.instance.songPositionInBeats;
        }

        private void Update()
        {
            if (!hit)
            {
                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(startBeat, 1.25f);

                anim.Play("BombOut", 0, normalizedBeatAnim);
                anim.speed = 0;
            }
            else
            {
                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(hitBeat, 1.5f);
                anim.Play("BombHit", 0, 0);
                anim.speed = 0;

            }
        }

        public void Hit()
        {

        }
    }
}