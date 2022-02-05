using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.DJSchool
{
    public class DJSchool : Minigame
    {
        [Header("Components")]
        [SerializeField] private Student student;

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

            student.swipeBeat = beat;
            student.ResetState();
        }
    }
}