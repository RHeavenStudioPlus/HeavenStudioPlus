using RhythmHeavenMania.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Games
{
    // none yet
    //using Scripts_FanClub;

    public class FanClub : Minigame
    {
        // userdata here
        [Header("Animators")]

        [Header("Objects")]
        public GameObject Arisa;

        // end userdata
        private Animator idolAnimator;
        public GameEvent bop = new GameEvent();
        public static FanClub instance;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            idolAnimator = Arisa.GetComponent<Animator>();
        }

        private void Update()
        {
            if (Conductor.instance.ReportBeat(ref bop.lastReportedBeat, bop.startBeat % 1))
            {
                if (Conductor.instance.songPositionInBeats >= bop.startBeat && Conductor.instance.songPositionInBeats < bop.startBeat + bop.length)
                {
                    idolAnimator.Play("IdolBeat", 0, 0);
                }
            }
        }

        public void Bop(float beat, float length)
        {
            bop.length = length;
            bop.startBeat = beat;
        }

        public void CallHai(float beat)
        {
            BeatAction.New(Arisa, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { Arisa.GetComponent<Animator>().Play("IdolPeace", 0, 0); }),
                new BeatAction.Action(beat + 1f, delegate { Arisa.GetComponent<Animator>().Play("IdolPeace", 0, 0); }),
                new BeatAction.Action(beat + 2f, delegate { Arisa.GetComponent<Animator>().Play("IdolPeace", 0, 0); }),
                new BeatAction.Action(beat + 3f, delegate { Arisa.GetComponent<Animator>().Play("IdolPeace", 0, 0); }),

                new BeatAction.Action(beat + 4f, delegate { Arisa.GetComponent<Animator>().Play("IdolCrap", 0, 0); }),
                new BeatAction.Action(beat + 5f, delegate { Arisa.GetComponent<Animator>().Play("IdolCrap", 0, 0); }),
                new BeatAction.Action(beat + 6f, delegate { Arisa.GetComponent<Animator>().Play("IdolCrap", 0, 0); }),
                new BeatAction.Action(beat + 7f, delegate { Arisa.GetComponent<Animator>().Play("IdolCrap", 0, 0); }),
            });
        }
    }
}