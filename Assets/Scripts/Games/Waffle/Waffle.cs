using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;

using Jukebox;

using Debug = UnityEngine.Debug;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class PcoWaffleLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("fallingWaffle", "Falling Waffle", "ffffff", false, false, new List<GameAction>()
            {
                new GameAction("splat", "Flop")
                {
                    function = delegate { var e = eventCaller.currentEntity; Waffle.instance.Flop(e.beat); },
                    defaultLength = 2f,
                },
                new GameAction("fall", "Force Fall")
                {
                    function = delegate { var e = eventCaller.currentEntity; Waffle.instance.forceFall(e.beat); },
                    defaultLength = 0.5f,
                },
                new GameAction("unfall", "Reset")
                {
                    function = delegate { var e = eventCaller.currentEntity; Waffle.instance.forceStand(e.beat); },
                    defaultLength = 0.5f,
                }
                //new GameAction("tick", "Start Ticking")
                //{
                    //preFunction = delegate { Waffle.PreStartClock(eventCaller.currentEntity.beat, eventCaller.currentEntity.length); },
                    //defaultLength = 0.5f,
                    //resizable = false,
                //}
            }
            //new List<string>() { "pco", "normal" },
            //"pcowaffle", "en",
            //new List<string>() {  },
            //chronologicalSortKey: 999
            );
        }
    }
}

namespace HeavenStudio.Games
{
    public class Waffle : Minigame
    {
        [Header("Animators")]
        [SerializeField] public Animator waffleAnim;
        [SerializeField] public Animator squareAnim;
        public bool HasFallen = false;

        //public double beat;
        //public float length;
        //double lastTick = double.MaxValue;

        //public struct QueuedTick
        //{
            //public double beat;
            //public float length;
        //}
        //static List<QueuedTick> queuedTicks = new List<QueuedTick>();

        public static Waffle instance;

        public void Awake()
        {
            instance = this;
        }

        public void Update()
        {
            //if (queuedTicks.Count > 0)
            //{
                //foreach (var input in queuedTicks)
                //{
                    //StartClock(input.beat, input.length);
                //}
                //queuedTicks.Clear();
            //}

            //if (lastTick + 1 <= conductor.songPositionInBeatsAsDouble)
            //{
                //lastTick++;
                //SoundByte.PlayOneShotGame("fallingWaffle/tick");
            //}
        }

        public void Flop(double beat)
        {
            if(HasFallen)
            {
                //Debug.LogWarning("L");
            }
            else
            {
                HasFallen = true;
                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat + 0.5, delegate { waffleAnim.DoScaledAnimationAsync("fall", 0.5f); }),
                    new BeatAction.Action(beat + 1, delegate { SoundByte.PlayOneShotGame("fallingWaffle/waffleSplat"); }),
                });
                ScheduleInput(beat, 1f, InputAction_BasicPress, waffleHit, waffleMiss, waffleNothing);
            }
        }
        public void waffleHit(PlayerActionEvent caller, float state)
        {
            HasFallen = true;
            if (state >= 1f || state <= -1f)
            {
                SoundByte.PlayOneShotGame("fallingWaffle/tink");
            }
        }
        public void waffleMiss(PlayerActionEvent caller)
        {
            HasFallen = true;
            SoundByte.PlayOneShotGame("fallingWaffle/miss");
        }
        public void waffleNothing(PlayerActionEvent caller) { }

        public void forceFall(double beat)
        {
            HasFallen = true;
            waffleAnim.DoScaledAnimationAsync("IdleFlop", 0.5f);
        }

        public void forceStand(double beat)
        {
            HasFallen = false;
            squareAnim.DoScaledAnimationAsync("Fade", 0.5f);
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 1, delegate { waffleAnim.DoScaledAnimationAsync("Idle", 0.5f); }),
            });
        }

        //public static void PreStartClock(double beat, float length)
        //{
            //if (GameManager.instance.currentGame == "fallingWaffle")
            //{
                //instance.StartClock(beat, length);
            //}
            //else
            //{
                //queuedTicks.Add(new QueuedTick { beat = beat, length = length });
            //}
        //}
        //public void StartClock(double beat, float length)
        //{
            //lastTick = beat - 1;
        //}
    }
}