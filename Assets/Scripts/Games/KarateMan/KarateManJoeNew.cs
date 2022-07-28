using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio;
using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_KarateMan
{
    public class KarateManJoeNew : MonoBehaviour
    {
        public Animator anim;
        public GameEvent bop = new GameEvent();

        float lastPunchTime = Single.MinValue;
        int inComboId = -1;
        public void SetComboId(int id) { inComboId = id; }

        private void Awake()
        {

        }

        private void Update()
        {
            var cond = Conductor.instance;
            if (cond.ReportBeat(ref bop.lastReportedBeat, bop.startBeat % 1, false) && cond.songPositionInBeats > bop.startBeat)
            {
                anim.Play("Beat", -1, 0);
            }

            if (PlayerInput.Pressed(true))
            {
                if (!KarateManNew.instance.IsExpectingInputNow())
                {
                    Punch(1);
                    Jukebox.PlayOneShotGame("karateman/swingNoHit", forcePlay: true);
                }
            }
            else if (PlayerInput.AltPressedUp())
            {
                if (!KarateManNew.instance.IsExpectingInputNow())
                {
                    if (inComboId != -1 && !KarateManNew.instance.IsExpectingInputNow())
                    {
                        //let go too early, make joe spin later
                        inComboId = -1;
                    }
                }
            }
        }

        public void Punch(int forceHand = 0)
        {
            var cond = Conductor.instance;
            switch (forceHand)
            {
                case 0:
                    if (cond.songPositionInBeats - lastPunchTime < 0.25f + (Minigame.LateTime() - 1f))
                    {
                        lastPunchTime = Single.MinValue;
                        anim.Play("Straight", -1, 0);
                    }
                    else
                    {
                        lastPunchTime = cond.songPositionInBeats;
                        anim.Play("Jab", -1, 0);
                    }
                    break;
                case 1:
                    anim.Play("Jab", -1, 0);
                    break;
                case 2:
                    anim.Play("Straight", -1, 0);
                    break;
            }
            bop.startBeat = cond.songPositionInBeats + 0.5f;
        }
    }
}