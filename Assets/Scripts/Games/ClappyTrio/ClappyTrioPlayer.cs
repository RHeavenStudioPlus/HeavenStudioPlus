using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.ClappyTrio
{
    public class ClappyTrioPlayer : PlayerActionObject
    {
        private float lastClapBeat;
        private float lastClapLength;
        [SerializeField] private bool clapVacant;

        private int lastIndex;

        private bool hit;

        public bool clapStarted = false;
        public bool canHit;

        private GameObject clapEffect;
        new int aceTimes = 0;

        private void Start()
        {
            clapEffect = transform.GetChild(4).GetChild(3).gameObject;
        }

        public override void OnAce()
        {
            if (aceTimes == 0)
            {
                Clap(true);
                aceTimes++;
            }
        }

        private void Update()
        {
            if (clapVacant == true)
            {
                float normalizedBeat = (Conductor.instance.GetPositionFromBeat(lastClapBeat, lastClapLength));

                StateCheck(normalizedBeat);

                if (normalizedBeat > Minigame.LateTime())
                {
                    clapVacant = false;
                    lastIndex = 0;
                    lastClapLength = 0;
                    lastClapBeat = 0;
                }
            }

            if (PlayerInput.Pressed())
            {
                Clap(false);
            }

        }

        public void SetClapAvailability(float startBeat, float length)
        {
            aceTimes = 0;
            lastClapBeat = startBeat;
            clapVacant = true;
            lastClapLength = length;
        }

        private void Clap(bool overrideCanHit)
        {
            bool canHit = state.early != true && state.late != true && state.perfect == true && hit == false;

            if (canHit || overrideCanHit)
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

            ClappyTrio.instance.SetFace(ClappyTrio.instance.Lion.Count - 1, 4);
            this.GetComponent<Animator>().Play("Clap", 0, 0);
        }
    }
}