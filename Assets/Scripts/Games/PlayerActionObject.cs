using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Games
{
    public class PlayerActionObject : MonoBehaviour
    {
        public bool inList = false;
        public int lastState;
        public Minigame.Eligible state = new Minigame.Eligible();
        public bool isEligible;

        public void PlayerActionInit(GameObject g)
        {
            state.gameObject = g;
        }

        // could possibly add support for custom early, perfect, and end times if needed.
        public void StateCheck(float normalizedBeat, List<Minigame.Eligible> eligibleHitsList)
        {
            if (!isEligible) return;
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
                state.early = early;
                state.perfect = perfect;
                state.late = late;

                eligibleHitsList.Add(state);
                inList = true;
            }
            else
            {
                Minigame.Eligible es = eligibleHitsList[eligibleHitsList.IndexOf(state)];
                es.early = early;
                es.perfect = perfect;
                es.late = late;
            }
        }

        public void MakeInEligible(List<Minigame.Eligible> eligibleHitsList)
        {
            if (!inList) return;

            eligibleHitsList.Remove(state);
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
        
        // No list
        public void StateCheckNoList(float normalizedBeat)
        {
            if (normalizedBeat > Minigame.EarlyTime() && normalizedBeat < Minigame.PerfectTime() && lastState == 0)
            {
                ModifyState(true, false, false);
                lastState++;
            }
            // Perfect State
            else if (normalizedBeat > Minigame.PerfectTime() && normalizedBeat < Minigame.LateTime() && lastState == 1)
            {
                ModifyState(false, true, false);
                lastState++;
            }
            // Late State
            else if (normalizedBeat > Minigame.LateTime() && normalizedBeat < Minigame.EndTime() && lastState == 2)
            {
                ModifyState(false, false, true);
                lastState++;
            }
            else if (normalizedBeat < Minigame.EarlyTime() || normalizedBeat > Minigame.EndTime())
            {
                // ineligible
            }
        }

        private void ModifyState(bool early, bool perfect, bool late)
        {
            state.early = early;
            state.perfect = perfect;
            state.late = late;
        }
    }
}