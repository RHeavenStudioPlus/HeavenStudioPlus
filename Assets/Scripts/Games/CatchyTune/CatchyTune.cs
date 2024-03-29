using DG.Tweening;
using HeavenStudio.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.InputSystem;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class CtrCatchLoader
    {
        // minigame menu items
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("catchyTune", "Catchy Tune", "f2f2f2", "ff376c", "f2f2f2", false, false, new List<GameAction>()
            {
                new GameAction("bop", "Bop")
                {
                    function = delegate {var e = eventCaller.currentEntity; CatchyTune.instance.Bop(e.beat, e.length, e["bop"], e["bopAuto"]); },
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("bop", CatchyTune.WhoBops.Both, "Bop", "Set the character(s) to bop for the duration of this event."),
                        new Param("bopAuto", CatchyTune.WhoBops.None, "Bop (Auto)", "Set the character(s) to automatically bop until another Bop event is reached."),
                    },
                },

                new GameAction("orange", "Orange")
                {
                    defaultLength = 4f,
                    parameters = new List<Param>()
                    {
                        new Param("side", CatchyTune.Side.Left, "Side", "Choose the side the orange falls down."),
                        new Param("smile", false, "Smile", "Toggle if Plalin and Alalin should smile after catching.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "endSmile" })
                        }),
                        new Param("endSmile", new EntityTypes.Float(2, 100, 2), "Smile Length", "Choose how long the smile should last after the catch.")
                    },
                    preFunction = delegate {var e = eventCaller.currentEntity; CatchyTune.PreDropFruit(e.beat, e["side"], e["smile"], false, e["endSmile"]); },
                },

                new GameAction("pineapple", "Pineapple")
                {
                    defaultLength = 8f,
                    parameters = new List<Param>()
                    {
                        new Param("side", CatchyTune.Side.Left, "Side", "Choose the side the pineapple falls down."),
                        new Param("smile", false, "Smile", "Toggle if Plalin and Alalin should smile after catching.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "endSmile" })
                        }),
                        new Param("endSmile", new EntityTypes.Float(2, 100, 2), "Smile Length", "Choose how long the smile should last after the catch.")
                    },
                    preFunction = delegate {var e = eventCaller.currentEntity; CatchyTune.PreDropFruit(e.beat, e["side"], e["smile"], true, e["endSmile"]); },
                }
            },
            new List<string>() { "ctr", "normal" },
            "ctrcatchy",
            "en",
            new List<string>() { },
            chronologicalSortKey: 4
            );
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_CatchyTune;
    public class CatchyTune : Minigame
    {

        public enum Side
        {
            Left,
            Right,
            Both
        }

        public enum WhoBops
        {
            Alalin = 1,
            Plalin = 2,
            Both = 0,
            None = 3
        }

        public enum Background
        {
            Short,
            Long
        }


        [Header("Animators")]
        public Animator plalinAnim; // Left d-pad
        public Animator alalinAnim; // right A button

        [Header("References")]
        public GameObject orangeBase;
        public GameObject pineappleBase;
        public Transform fruitHolder;
        public GameObject heartMessage;
        public GameObject bg2;

        // when to stop playing the catch animation
        private double stopCatchLeft = 0;
        private double stopCatchRight = 0;

        private double startSmile = 0;
        private double stopSmile = 0;

        public static CatchyTune instance;
        static List<QueuedFruit> queuedFruits = new List<QueuedFruit>();
        struct QueuedFruit
        {
            public double beat;
            public int side;
            public bool smile;
            public bool isPineapple;
            public float endSmile;
        }

        const int IALeft = 0;
        const int IARight = 1;
        protected static bool IA_PadLeft(out double dt)
        {
            return PlayerInput.GetPadDown(InputController.ActionsPad.Up, out dt)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Down, out dt)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Left, out dt)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Right, out dt);
        }
        protected static bool IA_BatonLeft(out double dt)
        {
            return PlayerInput.GetBatonDown(InputController.ActionsBaton.West, out dt);
        }
        protected static bool IA_TouchLeft(out double dt)
        {
            bool want = PlayerInput.GetTouchDown(InputController.ActionsTouch.Left, out dt);
            bool simul = false;
            if (!want)
            {
                simul = PlayerInput.GetTouchDown(InputController.ActionsTouch.Right, out dt)
                            && instance.IsExpectingInputNow(InputAction_Left)
                            && instance.IsExpectingInputNow(InputAction_Right);
            }
            return want || simul;
        }

        protected static bool IA_PadRight(out double dt)
        {
            return PlayerInput.GetPadDown(InputController.ActionsPad.East, out dt);
        }
        protected static bool IA_BatonRight(out double dt)
        {
            return PlayerInput.GetBatonDown(InputController.ActionsBaton.East, out dt);
        }
        protected static bool IA_TouchRight(out double dt)
        {
            bool want = PlayerInput.GetTouchDown(InputController.ActionsTouch.Right, out dt);
            bool simul = false;
            if (!want)
            {
                simul = PlayerInput.GetTouchDown(InputController.ActionsTouch.Left, out dt)
                            && instance.IsExpectingInputNow(InputAction_Left)
                            && instance.IsExpectingInputNow(InputAction_Right);
            }
            return want || simul;
        }

        public static PlayerInput.InputAction InputAction_Left =
            new("CtrStepLeft", new int[] { IALeft, IALeft, IALeft },
            IA_PadLeft, IA_TouchLeft, IA_BatonLeft);

        public static PlayerInput.InputAction InputAction_Right =
            new("CtrStepRight", new int[] { IARight, IARight, IARight },
            IA_PadRight, IA_TouchRight, IA_BatonRight);

        private void Awake()
        {
            instance = this;
            SetupBopRegion("catchyTune", "bop", "bopAuto", false);
        }

        const float orangeoffset = 0.5f;
        const float pineappleoffset = 0.5f;

        private void Update()
        {
            Conductor cond = Conductor.instance;

            if (cond.isPlaying && !cond.isPaused)
            {
                if (queuedFruits.Count > 0)
                {
                    foreach (var fruit in queuedFruits)
                    {
                        DropFruit(fruit.beat, fruit.side, fruit.smile, fruit.isPineapple, fruit.endSmile);
                    }
                    queuedFruits.Clear();
                }

                // print(stopCatchLeft + " " + stopCatchRight);
                // print("current beat: " + conductor.songPositionInBeatsAsDouble);
                if (stopCatchLeft > 0 && stopCatchLeft <= cond.songPositionInBeatsAsDouble)
                {
                    plalinAnim.DoScaledAnimationAsync("idle", 0.5f);
                    stopCatchLeft = 0;
                }

                if (stopCatchRight > 0 && stopCatchRight <= cond.songPositionInBeatsAsDouble)
                {
                    alalinAnim.DoScaledAnimationAsync("idle", 0.5f);
                    stopCatchRight = 0;
                }

                if (startSmile > 0 && startSmile <= cond.songPositionInBeatsAsDouble)
                {
                    //print("smile start");
                    plalinAnim.Play("smile", 1, 0);
                    alalinAnim.Play("smile", 1, 0);
                    startSmile = 0;
                    heartMessage.SetActive(true);
                }

                if (stopSmile > 0 && stopSmile <= cond.songPositionInBeatsAsDouble)
                {
                    //print("smile stop");
                    plalinAnim.Play("stopsmile", 1, 0);
                    alalinAnim.Play("stopsmile", 1, 0);
                    stopSmile = 0;
                    heartMessage.SetActive(false);
                }

                if (PlayerInput.GetIsAction(InputAction_Left) && !IsExpectingInputNow(InputAction_Left.inputLockCategory))
                {
                    catchWhiff(false);
                }
                if (PlayerInput.GetIsAction(InputAction_Right) && !IsExpectingInputNow(InputAction_Right.inputLockCategory))
                {
                    catchWhiff(true);
                }
            }
        }

        public override void OnBeatPulse(double beat)
        {
            int whoBopsAuto = BeatIsInBopRegionInt(beat);
            bool bopLeft = whoBopsAuto == (int)WhoBops.Plalin || whoBopsAuto == (int)WhoBops.Both;
            bool bopRight = whoBopsAuto == (int)WhoBops.Alalin || whoBopsAuto == (int)WhoBops.Both;
            if (bopLeft && stopCatchLeft == 0)
            {
                plalinAnim.DoScaledAnimationAsync("bop", 0.5f);
            }

            if (bopRight && stopCatchRight == 0)
            {
                alalinAnim.DoScaledAnimationAsync("bop", 0.5f);
            }
        }

        public void DropFruit(double beat, int side, bool smile, bool isPineapple, float endSmile)
        {
            var objectToSpawn = isPineapple ? pineappleBase : orangeBase;

            if (side == (int)Side.Left || side == (int)Side.Both)
            {
                DropFruitSingle(beat, false, smile, objectToSpawn, endSmile);
            }

            if (side == (int)Side.Right || side == (int)Side.Both)
            {
                DropFruitSingle(beat, true, smile, objectToSpawn, endSmile);
            }
        }

        //minenice: experiment to test preFunction
        public static void PreDropFruit(double beat, int side, bool smile, bool isPineapple, float endSmile)
        {
            double spawnBeat = beat - 1;
            beat = beat - (isPineapple ? 2f : 1f);
            if (GameManager.instance.currentGame == "catchyTune")
            {
                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(spawnBeat, delegate { if (instance != null) instance.DropFruit(beat, side, smile, isPineapple, endSmile); }),
                });
            }
            else
            {
                queuedFruits.Add(new QueuedFruit()
                {
                    beat = beat,
                    side = side,
                    smile = smile,
                    isPineapple = isPineapple,
                    endSmile = endSmile
                });
            }

            if (side == (int)Side.Left || side == (int)Side.Both)
            {
                Fruit.PlaySound(beat, false, isPineapple);
            }
            if (side == (int)Side.Right || side == (int)Side.Both)
            {
                Fruit.PlaySound(beat, true, isPineapple);
            }
        }

        public void DropFruitSingle(double beat, bool side, bool smile, GameObject objectToSpawn, float endSmile)
        {

            var newFruit = GameObject.Instantiate(objectToSpawn, fruitHolder);
            var fruitComp = newFruit.GetComponent<Fruit>();
            fruitComp.startBeat = beat;
            fruitComp.side = side;
            fruitComp.smile = smile;
            fruitComp.endSmile = endSmile;
            newFruit.SetActive(true);
        }

        public void Bop(double beat, float length, int whoBops, int whoBopsAuto)
        {
            for (int i = 0; i < length; i++)
            {
                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat + i, delegate
                    {
                        BopSingle(whoBops);
                    })
                });
            }
        }

        void BopSingle(int whoBops)
        {
            switch (whoBops)
            {
                case (int)WhoBops.Plalin:
                    if (stopCatchLeft == 0)
                    {
                        plalinAnim.DoScaledAnimationAsync("bop", 0.5f);
                    }
                    break;
                case (int)WhoBops.Alalin:
                    if (stopCatchRight == 0)
                    {
                        alalinAnim.DoScaledAnimationAsync("bop", 0.5f);
                    }
                    break;
                case (int)WhoBops.Both:
                    if (stopCatchRight == 0)
                    {
                        alalinAnim.DoScaledAnimationAsync("bop", 0.5f);
                    }
                    if (stopCatchLeft == 0)
                    {
                        plalinAnim.DoScaledAnimationAsync("bop", 0.5f);
                    }
                    break;
                default:
                    break;
            }
        }

        public void catchSuccess(bool side, bool isPineapple, bool smile, double beat, float endSmile)
        {
            string anim = isPineapple ? "catchPineapple" : "catchOrange";

            if (side)
            {
                alalinAnim.DoScaledAnimationAsync(anim, 0.5f);
                stopCatchRight = beat + 0.9f;
            }
            else
            {
                plalinAnim.DoScaledAnimationAsync(anim, 0.5f);
                stopCatchLeft = beat + 0.9f;
            }

            if (smile)
            {
                startSmile = beat + 1f;
                stopSmile = beat + endSmile;
            }

        }

        public void catchMiss(bool side, bool isPineapple)
        {
            // not the right sound at all but need an accurate rip
            SoundByte.PlayOneShotGame("catchyTune/fruitThrough");

            double beat = Conductor.instance.songPositionInBeatsAsDouble;

            string fruitType = isPineapple ? "Pineapple" : "Orange";

            if (side)
            {
                alalinAnim.DoScaledAnimationAsync("miss" + fruitType, 0.5f);
                stopCatchRight = beat + 0.7f;
            }
            else
            {
                plalinAnim.DoScaledAnimationAsync("miss" + fruitType, 0.5f);
                stopCatchLeft = beat + 0.7f;
            }
        }

        public void catchWhiff(bool side)
        {
            SoundByte.PlayOneShotGame("catchyTune/whiff");
            whiffAnim(side);
        }

        public void catchBarely(bool side)
        {
            if (side)
            {
                SoundByte.PlayOneShotGame("catchyTune/barely right");
            }
            else
            {
                SoundByte.PlayOneShotGame("catchyTune/barely left");
            }

            whiffAnim(side);
        }

        public void whiffAnim(bool side)
        {
            double beat = Conductor.instance.songPositionInBeatsAsDouble;

            if (side)
            {
                alalinAnim.DoScaledAnimationAsync("whiff", 0.5f);
                stopCatchRight = beat + 0.5f;
            }
            else
            {
                plalinAnim.DoScaledAnimationAsync("whiff", 0.5f);
                stopCatchLeft = beat + 0.5f;
            }
        }
    }
}
