using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.ForkLifter
{
    public class Pea : PlayerActionObject
    {
        private Animator anim;

        public float startBeat;

        public int type;

        private void Start()
        {
            anim = GetComponent<Animator>();
            Jukebox.PlayOneShotGame("forkLifter/zoom");
            GetComponentInChildren<SpriteRenderer>().sprite = ForkLifter.instance.peaSprites[type];

            for (int i = 0; i < transform.GetChild(0).childCount; i++)
            {
                transform.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>().sprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            }

            PlayerActionInit(this.gameObject, startBeat, ForkLifter.instance.EligibleHits);

            isEligible = true;
        }

        private void Update()
        {
            float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(startBeat, 2.5f);
            anim.Play("Flicked_Object", -1, normalizedBeatAnim);
            anim.speed = 0;

            float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(startBeat, 2f);

            StateCheck(normalizedBeat);

            if (normalizedBeat > 1.35f)
            {
                Jukebox.PlayOneShot("audience/disappointed");
                Destroy(this.gameObject);
            }
        }
    }
}