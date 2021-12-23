using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.ForkLifter
{
    public class Pea : MonoBehaviour
    {
        [Header("Latency")]
        public float earlyTime;
        public float perfectTime;
        public float lateTime;
        public float endTime;

        private Animator anim;

        public float startBeat;

        private bool inList = false;

        public int type;

        private ForkLifterPlayer.Eligible e = new ForkLifterPlayer.Eligible();

        public int estate, pstate, lstate, endstate;

        private void Start()
        {
            anim = GetComponent<Animator>();
            Jukebox.PlayOneShotGame("forkLifter/zoom");
            GetComponentInChildren<SpriteRenderer>().sprite = ForkLifter.instance.peaSprites[type];

            e = new ForkLifterPlayer.Eligible();
            e.pea = this;

            for (int i = 0; i < transform.GetChild(0).childCount; i++)
            {
                transform.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>().sprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            }
        }

        private void Update()
        {
            float normalizedBeat = (Conductor.instance.GetLoopPositionFromBeat(startBeat, 2.5f));
            anim.Play("Flicked_Object", -1, normalizedBeat);
            anim.speed = 0;

            // Early State
            if (normalizedBeat > earlyTime && normalizedBeat < perfectTime && estate <= 1)
            {
                estate++;
                MakeEligible(true, false, false);
            }
            // Perfect State
            else if (normalizedBeat > perfectTime && normalizedBeat < lateTime && pstate <= 1)
            {
                pstate++;
                MakeEligible(false, true, false);
            }
            // Late State
            else if (normalizedBeat > lateTime && normalizedBeat < endTime && lstate <= 1)
            {
                lstate++;
                MakeEligible(false, false, true);
            }
            else if (normalizedBeat < earlyTime || normalizedBeat > endTime)
            {
                MakeInEligible();
            }

            if (normalizedBeat > endTime && endstate <= 1)
            {
                endstate++;
                Jukebox.PlayOneShotGame("audience/disappointed");
            }

            if (normalizedBeat > 1.35f)
            {
                MakeInEligible();
                Destroy(this.gameObject);
            }
        }

        public void MakeEligible(bool early, bool perfect, bool late)
        {
            // print($"{early}, {perfect}, {late}");

            if (!inList)
            {
                e.early = early;
                e.perfect = perfect;
                e.late = late;

                ForkLifterPlayer.instance.EligibleHits.Add(e);
                inList = true;
            }
            else
            {
                ForkLifterPlayer.Eligible es = ForkLifterPlayer.instance.EligibleHits[ForkLifterPlayer.instance.EligibleHits.IndexOf(e)];
                es.early = early;
                es.perfect = perfect;
                es.late = late;
            }
        }

        public void MakeInEligible()
        {
            if (!inList) return;

            ForkLifterPlayer.instance.EligibleHits.Remove(e);
            inList = false;
        }
    }
}