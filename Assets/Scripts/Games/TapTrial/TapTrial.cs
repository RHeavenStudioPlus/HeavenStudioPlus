using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class AgbTapLoader
    {
        public static Minigame AddGame(EventCaller eventCaller) {
            return new Minigame("tapTrial", "Tap Trial \n<color=#eb5454>[WIP]</color>", "93ffb3", false, false, new List<GameAction>()
            {
                new GameAction("bop",                   delegate { TapTrial.instance.Bop(eventCaller.currentEntity.toggle); }, .5f, false, new List<Param>()
                {
                    new Param("toggle", true, "Bop", "Whether both will bop to the beat or not")
                }),
                new GameAction("tap",                   delegate { TapTrial.instance.Tap(eventCaller.currentEntity.beat); }, 2.0f, false),
                new GameAction("double tap",            delegate { TapTrial.instance.DoubleTap(eventCaller.currentEntity.beat); }, 2.0f, false),
                new GameAction("triple tap",            delegate { TapTrial.instance.TripleTap(eventCaller.currentEntity.beat); }, 4.0f, false),
                new GameAction("jump tap",              delegate { TapTrial.instance.JumpTap(eventCaller.currentEntity.beat); }, 2.0f, false),
                new GameAction("final jump tap",        delegate { TapTrial.instance.FinalJumpTap(eventCaller.currentEntity.beat); }, 2.0f, false),
            });
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_TapTrial;

    public class TapTrial : Minigame
    {
        [Header("References")]
        public TapTrialPlayer player;
        public GameObject tap;
        [SerializeField] List<Animator> monkeys;
        bool goBop;
        float lastReportedBeat = 0f;

        public static TapTrial instance { get; set; }

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (goBop)
            {         
                if (Conductor.instance.ReportBeat(ref lastReportedBeat))
                {
                    monkeys[0].Play("Bop", 0, 0);
                    monkeys[1].Play("Bop", 0, 0);
                }
                else if (Conductor.instance.songPositionInBeats < lastReportedBeat)
                {
                    lastReportedBeat = Mathf.Round(Conductor.instance.songPositionInBeats);
                }
            }
            
        }

        public void Bop(bool isBopping)
        {
            if (isBopping)
            {
                goBop = true;
            }
            else
            {
                goBop = false;
            }
        }

        public void Tap(float beat)
        {
            Jukebox.PlayOneShotGame("tapTrial/ook");
            player.anim.Play("TapPrepare", 0, 0);

            //Monkey Tap Prepare Anim
            BeatAction.New(gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { monkeys[0].Play("TapPrepare"); }),
                new BeatAction.Action(beat, delegate { monkeys[1].Play("TapPrepare"); }),
                new BeatAction.Action(beat + 1f, delegate { monkeys[0].Play("Tap"); }),
                new BeatAction.Action(beat + 1f, delegate { monkeys[1].Play("Tap"); }),
            });
            //CreateTap(beat);
            ScheduleInput(beat, 1f, InputType.STANDARD_DOWN, OnTap, OnTapMiss, OnEmpty);
        }

        public void OnTap(PlayerActionEvent caller, float beat)
        {
            Jukebox.PlayOneShotGame("tapTrial/tap");
            player.anim.Play("Tap", 0, 0);
        }

        public void OnDoubleTap(PlayerActionEvent caller, float beat)
        {
            Jukebox.PlayOneShotGame("tapTrial/tap");
            player.anim.Play("DoubleTap", 0, 0);
        }

        public void OnTapMiss(PlayerActionEvent caller)
        {

        }

        public void OnEmpty(PlayerActionEvent caller)
        {
            //empty
        }

        public void DoubleTap(float beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("tapTrial/ookook",   beat),
                new MultiSound.Sound("tapTrial/ookook",   beat + 0.5f)
            });

            player.anim.Play("DoubleTapPrepare", 0, 0);

            BeatAction.New(gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { monkeys[0].Play("DoubleTapPrepare", 0, 0); }),
                new BeatAction.Action(beat + .5f, delegate { monkeys[0].Play("DoubleTapPrepare_2", 0, 0); }),
                new BeatAction.Action(beat + 1f, delegate { monkeys[0].Play("DoubleTap", 0, 0); }),
                new BeatAction.Action(beat + 1.5f, delegate { monkeys[0].Play("DoubleTap", 0, 0); }),
            });
            
            BeatAction.New(gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { monkeys[1].Play("DoubleTapPrepare", 0, 0); }),
                new BeatAction.Action(beat + .5f, delegate { monkeys[1].Play("DoubleTapPrepare_2", 0, 0); }),
                new BeatAction.Action(beat + 1f, delegate { monkeys[1].Play("DoubleTap", 0, 0); }),
                new BeatAction.Action(beat + 1.5f, delegate { monkeys[1].Play("DoubleTap", 0, 0); }),
            });

            ScheduleInput(beat, 1f, InputType.STANDARD_DOWN, OnDoubleTap, OnTapMiss, OnEmpty);
            ScheduleInput(beat, 1.5f, InputType.STANDARD_DOWN, OnDoubleTap, OnTapMiss, OnEmpty);
        }

        public void TripleTap(float beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("tapTrial/ooki1",   beat),
                new MultiSound.Sound("tapTrial/ooki2",   beat + 0.5f)
            });

            //player.anim.Play("PosePrepare", 0, 0);
            player.tripleOffset = 0;

            BeatAction.New(gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { player.anim.Play("PosePrepare_1", 0, 0);}),
                new BeatAction.Action(beat + .5f, delegate { player.anim.Play("PosePrepare_2", 0, 0);}),
            });

            BeatAction.New(gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { monkeys[0].Play("PostPrepare_1", 0, 0); }),
                new BeatAction.Action(beat + .5f, delegate { monkeys[0].Play("PostPrepare_2", 0, 0); }),
                new BeatAction.Action(beat + 2f, delegate { monkeys[0].Play("PostTap", 0, 0); }),
                new BeatAction.Action(beat + 2.5f, delegate { monkeys[0].Play("PostTap", 0, 0);}),
                new BeatAction.Action(beat + 3f, delegate { monkeys[0].Play("PostTap", 0, 0);}),
            });

            BeatAction.New(gameObject, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { monkeys[1].Play("PostPrepare_1", 0, 0); }),
                new BeatAction.Action(beat + .5f, delegate { monkeys[1].Play("PostPrepare_2", 0, 0); }),
                new BeatAction.Action(beat + 2f, delegate { monkeys[1].Play("PostTap", 0, 0); }),
                new BeatAction.Action(beat + 2.5f, delegate { monkeys[1].Play("PostTap", 0, 0);}),
                new BeatAction.Action(beat + 3f, delegate { monkeys[1].Play("PostTap", 0, 0);}),
            });



            ScheduleInput(beat, 2f, InputType.STANDARD_DOWN, OnTap, OnTapMiss, OnEmpty);
            ScheduleInput(beat, 2.5f, InputType.STANDARD_DOWN, OnDoubleTap, OnTapMiss, OnEmpty);
            ScheduleInput(beat, 3f, InputType.STANDARD_DOWN, OnTap, OnTapMiss, OnEmpty);
        }

        public void JumpTap(float beat)
        {

        }

        public void FinalJumpTap(float beat)
        {

        }

        public void CreateTap(float beat, int type = 0)
        {
            GameObject _tap = Instantiate(tap);
            _tap.transform.parent = tap.transform.parent;
            _tap.SetActive(true);
            Tap t = _tap.GetComponent<Tap>();
            t.startBeat = beat;
            t.type = type;
        }
    }
}