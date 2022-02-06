using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.DJSchool
{
    public class DJSchool : Minigame
    {
        [Header("Components")]
        [SerializeField] private Student student;
        [SerializeField] private GameObject djYellow;

        [Header("Properties")]
        public GameEvent bop = new GameEvent();

        public static DJSchool instance { get; private set; }

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (Conductor.instance.ReportBeat(ref bop.lastReportedBeat, bop.startBeat % 1))
            {
                if (Conductor.instance.songPositionInBeats >= bop.startBeat && Conductor.instance.songPositionInBeats < bop.startBeat + bop.length)
                {
                    if (student.anim.IsAnimationNotPlaying())
                    {
                        if (student.isHolding)
                        {
                            student.anim.Play("HoldBop", 0, 0);
                        }
                        else
                        {
                            student.anim.Play("IdleBop", 0, 0);
                        }
                    }
                }
            }
        }

        public void Bop(float beat, float length)
        {
            bop.startBeat = beat;
            bop.length = length;
        }

        public void BreakCmon(float beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("djSchool/breakCmon1",   beat),
                new MultiSound.Sound("djSchool/breakCmon2", beat + 1f),
                new MultiSound.Sound("djSchool/ooh", beat + 2f),
            });

            BeatAction.New(djYellow, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { djYellow.GetComponent<Animator>().Play("BreakCmon", 0, 0); }),
                new BeatAction.Action(beat + 1f, delegate { djYellow.GetComponent<Animator>().Play("BreakCmon", 0, 0); }),
                new BeatAction.Action(beat + 2f, delegate { djYellow.GetComponent<Animator>().Play("Hold", 0, 0); }),
            });
            
            student.holdBeat = beat;
            student.ResetState();
        }

        public void ScratchoHey(float beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("djSchool/scratchoHey1",   beat),
                new MultiSound.Sound("djSchool/scratchoHey2", beat + 1f),
                new MultiSound.Sound("djSchool/hey", beat + 2f),
            });

            BeatAction.New(djYellow, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { djYellow.GetComponent<Animator>().Play("Scratcho", 0, 0); }),
                new BeatAction.Action(beat + 1f, delegate { djYellow.GetComponent<Animator>().Play("Scratcho2", 0, 0); }),
                new BeatAction.Action(beat + 2.05f, delegate { djYellow.GetComponent<Animator>().Play("Hey", 0, 0); }),
            });

            student.swipeBeat = beat;
            student.ResetState();
        }
    }
}