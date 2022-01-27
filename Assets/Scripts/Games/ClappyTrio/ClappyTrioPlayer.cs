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
        int aceTimes = 0;

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
            if (PlayerInput.Pressed())
            {
                Clap(false);
            }

            if (clapVacant == true)
            {
                float normalizedBeat = (Conductor.instance.GetLoopPositionFromBeat(lastClapBeat, lastClapLength));

                /*if (normalizedBeat > Minigame.EarlyTime() && normalizedBeat < Minigame.PerfectTime() && lastIndex == 0)
                {
                    SetEligibility(true, false, false);
                    lastIndex++;
                }
                else if (normalizedBeat > Minigame.PerfectTime() && normalizedBeat < Minigame.LateTime() && lastIndex == 1)
                {
                    SetEligibility(false, true, false);
                    // Clap();
                    lastIndex++;
                }
                else if (normalizedBeat > Minigame.LateTime() && lastIndex == 2)
                {
                    SetEligibility(false, false, true);
                    clapVacant = false;
                    lastIndex = 0;
                    lastClapLength = 0;
                    lastClapBeat = 0;
                    hit = false;
                }*/

                StateCheck(normalizedBeat);

                if (normalizedBeat > Minigame.LateTime())
                {
                    clapVacant = false;
                    lastIndex = 0;
                    lastClapLength = 0;
                    lastClapBeat = 0;
                }
            }
        }

        /*public void ClearLog()
        {
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }*/

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