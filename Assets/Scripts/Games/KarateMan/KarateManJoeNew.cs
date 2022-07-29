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
        float lastComboMissTime = Single.MinValue;
        public bool inCombo = false;
        int inComboId = -1;
        int shouldComboId = -1;
        public void SetComboId(int id) { inComboId = id; }
        public void SetShouldComboId(int id) { shouldComboId = id; }
        public int GetComboId() { return inComboId; }
        public int GetShouldComboId() { return shouldComboId; }

        private void Awake()
        {

        }

        private void Update()
        {
            var cond = Conductor.instance;
            if (cond.ReportBeat(ref bop.lastReportedBeat, bop.startBeat % 1, false) && cond.songPositionInBeats > bop.startBeat && !inCombo)
            {
                anim.Play("Beat", -1, 0);
            }

            if (inCombo && shouldComboId == -2)
            {
                float missProg = cond.GetPositionFromBeat(lastComboMissTime, 3.25f);
                if (missProg >= 0f && missProg < 1f)
                {
                    anim.DoScaledAnimation("LowKickMiss", lastComboMissTime, 3.25f);
                }
                else if (missProg >= 1f)
                {
                    anim.speed = 1f;
                    bop.startBeat = lastComboMissTime + 3.25f;
                    lastComboMissTime = Single.MinValue;
                    inCombo = false;
                    inComboId = -1;
                    shouldComboId = -1;
                    Debug.Log("Getup");
                }
            }

            if (PlayerInput.Pressed(true) && !inCombo)
            {
                if (!KarateManNew.instance.IsExpectingInputNow())
                {
                    Punch(1);
                    Jukebox.PlayOneShotGame("karateman/swingNoHit", forcePlay: true);
                }
            }
            else if (PlayerInput.AltPressed() && !inCombo)
            {
                if (!KarateManNew.instance.IsExpectingInputNow())
                {
                    //start a forced-fail combo sequence
                    float beat = cond.songPositionInBeats;
                    BeatAction.New(gameObject, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(beat, delegate { Punch(1); inCombo = true; inComboId = -1; shouldComboId = -1;}),
                        new BeatAction.Action(beat + 0.25f, delegate { Punch(2); }),
                        new BeatAction.Action(beat + 0.5f, delegate { ComboSequence(0); }),
                        new BeatAction.Action(beat + 0.75f, delegate { shouldComboId = -2; ComboMiss(beat + 0.75f); }),
                    });

                    MultiSound.Play(new MultiSound.Sound[] 
                    {
                        new MultiSound.Sound("karateman/swingNoHit", beat), 
                        new MultiSound.Sound("karateman/swingNoHit_Alt", beat + 0.25f), 
                        new MultiSound.Sound("karateman/swingNoHit_Alt", beat + 0.5f), 
                        new MultiSound.Sound("karateman/comboMiss", beat + 0.75f),  
                    }, forcePlay: true);
                }
            }
            else if (PlayerInput.AltPressedUp())
            {
                if (!KarateManNew.instance.IsExpectingInputNow())
                {
                    if (inComboId != -1 && !KarateManNew.instance.IsExpectingInputNow())
                    {
                        inComboId = -1;
                    }
                }
            }
        }

        public bool Punch(int forceHand = 0)
        {
            var cond = Conductor.instance;
            bool straight = false;
            switch (forceHand)
            {
                case 0:
                    if (cond.songPositionInBeats - lastPunchTime < 0.25f + (Minigame.LateTime() - 1f))
                    {
                        lastPunchTime = Single.MinValue;
                        anim.Play("Straight", -1, 0);
                        straight = true;
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
                    straight = true;
                    break;
            }
            bop.startBeat = cond.songPositionInBeats + 0.5f;
            return straight;    //returns what hand was used to punch the object
        }

        public void ComboSequence(int seq)
        {
            var cond = Conductor.instance;
            switch (seq)
            {
                case 0:
                    anim.Play("LowJab", -1, 0);
                    break;
                case 1:
                    anim.Play("LowKick", -1, 0);
                    break;
                case 2:
                    anim.Play("BackHand", -1, 0);
                    break;
                default:
                    break;
            }
            bop.startBeat = cond.songPositionInBeats + 1f;
        }

        public void ComboMiss(float beat)
        {
            var cond = Conductor.instance;
            lastComboMissTime = beat;
            bop.startBeat = beat + 3.25f;
        }
    }
}