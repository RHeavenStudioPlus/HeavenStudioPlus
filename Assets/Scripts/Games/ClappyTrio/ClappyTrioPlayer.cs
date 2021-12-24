using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.ClappyTrio
{
    public class ClappyTrioPlayer : MonoBehaviour
    {
        public bool early;
        public bool perfect;
        public bool late;

        private float lastClapBeat;
        private bool clapVacant;

        private int lastIndex;

        private float perfectTime = 0.25f, lateTime = 0.43f;

        private bool hit;

        public bool clapStarted = false;
        public bool canHit;

        private GameObject clapEffect;

        private void Start()
        {
            clapEffect = transform.GetChild(4).GetChild(3).gameObject;
        }

        private void Update()
        {
            if (PlayerInput.Pressed())
            {
                Clap();
            }

            if (clapVacant == true)
            {
                float songPosBeat = Conductor.instance.songPositionInBeats;

                if (songPosBeat > lastClapBeat && songPosBeat < lastClapBeat + perfectTime && lastIndex == 0)
                {
                    SetEligibility(true, false, false);
                    lastIndex++;
                }
                else if (songPosBeat > lastClapBeat + perfectTime && songPosBeat < lastClapBeat + lateTime && lastIndex == 1)
                {
                    SetEligibility(false, true, false);
                    // Clap();
                    lastIndex++;
                }
                else if (songPosBeat > lastClapBeat + lateTime && lastIndex == 2)
                {
                    SetEligibility(false, false, true);
                    clapVacant = false;
                    lastIndex = 0;
                    hit = false;
                }
            }
        }

        public void SetClapAvailability(float startBeat)
        {
            lastClapBeat = startBeat;
            clapVacant = true;
        }

        private void SetEligibility(bool early, bool perfect, bool late)
        {
            this.early = false;
            this.perfect = false;
            this.late = false;

            if (early)
                this.early = true;
            else if (perfect)
                this.perfect = true;
            else if (late)
                this.late = true;
        }

        private void Clap()
        {
            bool canHit = early != true && late != true && perfect == true && hit == false;

            if (canHit)
            {
                clapEffect.SetActive(true);
                Jukebox.PlayOneShotGame("clappyTrio/rightClap");

                if (this.canHit)
                    ClappyTrio.instance.playerHitLast = true;
            }
            else
            {
                clapEffect.SetActive(false);
                Jukebox.PlayOneShot("miss");
                ClappyTrio.instance.playerHitLast = false;

                if (clapStarted)
                    this.canHit = false;
            }

            ClappyTrio.instance.SetFace(2, 4);
            this.GetComponent<Animator>().Play("Clap", 0, 0);
        }
    }
}