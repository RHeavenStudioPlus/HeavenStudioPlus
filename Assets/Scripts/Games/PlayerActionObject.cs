using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Games
{
    public class PlayerActionObject : MonoBehaviour
    {
        public bool inList = false;
        public int lastState;
        private Minigame.Eligible e = new Minigame.Eligible();

        public void PlayerActionInit(GameObject g)
        {
            e.gameObject = g;
        }

        // could possibly add support for custom early, perfect, and end times if needed.
        public void StateCheck(float normalizedBeat, List<Minigame.Eligible> eligibleHitsList)
        {
            if (normalizedBeat > Minigame.EarlyTime() && normalizedBeat < Minigame.PerfectTime() && lastState == 0)
            {
                MakeEligible(true, false, false, eligibleHitsList);
                lastState++;
            }
            // Perfect State
            else if (normalizedBeat > Minigame.PerfectTime() && normalizedBeat < Minigame.LateTime() && lastState == 1)
            {
                MakeEligible(false, true, false, eligibleHitsList);
                lastState++;
            }
            // Late State
            else if (normalizedBeat > Minigame.LateTime() && normalizedBeat < Minigame.EndTime() && lastState == 2)
            {
                MakeEligible(false, false, true, eligibleHitsList);
                lastState++;
            }
            else if (normalizedBeat < Minigame.EarlyTime() || normalizedBeat > Minigame.EndTime())
            {
                MakeInEligible(eligibleHitsList);
            }
        }

        public void MakeEligible(bool early, bool perfect, bool late, List<Minigame.Eligible> eligibleHitsList)
        {
            if (!inList)
            {
                e.early = early;
                e.perfect = perfect;
                e.late = late;

                eligibleHitsList.Add(e);
                inList = true;
            }
            else
            {
                Minigame.Eligible es = eligibleHitsList[eligibleHitsList.IndexOf(e)];
                es.early = early;
                es.perfect = perfect;
                es.late = late;
            }
        }

        public void MakeInEligible(List<Minigame.Eligible> eligibleHitsList)
        {
            if (!inList) return;

            eligibleHitsList.Remove(e);
            inList = false;
        }

        public void RemoveObject(int currentHitInList, List<Minigame.Eligible> EligibleHits)
        {
            if (currentHitInList < EligibleHits.Count)
            {
                EligibleHits.Remove(EligibleHits[currentHitInList]);
                currentHitInList++;
            }
        }
    }
}