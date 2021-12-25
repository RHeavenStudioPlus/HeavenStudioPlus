using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.ForkLifter
{
    public class Pea : MonoBehaviour
    {

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
            float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(startBeat, 2.5f);
            anim.Play("Flicked_Object", -1, normalizedBeatAnim);
            anim.speed = 0;

            float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(startBeat, 2f);

            print(normalizedBeat + " " + Minigame.PerfectTime());

            // Early State
            if (normalizedBeat > Minigame.EarlyTime() && normalizedBeat < Minigame.PerfectTime() && estate <= 1)
            {
                MakeEligible(true, false, false);
                estate++;
            }
            // Perfect State
            else if (normalizedBeat > Minigame.PerfectTime() && normalizedBeat < Minigame.LateTime() && pstate <= 1)
            {
                MakeEligible(false, true, false);
                pstate++;
            }
            // Late State
            else if (normalizedBeat > Minigame.LateTime() && normalizedBeat < Minigame.EndTime() && lstate <= 1)
            {
                MakeEligible(false, false, true);
                lstate++;
            }
            else if (normalizedBeat < Minigame.EarlyTime() || normalizedBeat > Minigame.EndTime())
            {
                MakeInEligible();
            }

            if (normalizedBeat > Minigame.EndTime() && endstate <= 1)
            {
                endstate++;
                Jukebox.PlayOneShot("audience/disappointed");
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