using HeavenStudio.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Common;
using HeavenStudio.InputSystem;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class AgbSpaceDanceLoader
    {
        public static Minigame AddGame(EventCaller eventCaller) {
            return new Minigame("spaceDance", "Space Dance", "0014d6", false, false, new List<GameAction>()
            {
                new GameAction("bop", "Bop")
                {
                    function = delegate { var e = eventCaller.currentEntity; SpaceDance.instance.EpicBop(e.beat, e.length, e["auto"], e["bop"], e["grampsAuto"], e["gramps"]); },
                    parameters = new List<Param>()
                    {
                        new Param("bop", true, "Dancers Bop", "Toggle if the dancers should bop for the duration of this event."),
                        new Param("auto", false, "Dancers Bop (Auto)", "Toggle if the dancers should automatically bop until another Bop event is reached."),
                        new Param("gramps", false, "Gramps Bop", "Toggle if Space Gramps should bop for the duration of this event."),
                        new Param("grampsAuto", false, "Gramps Bop (Auto)", "Toggle if Space Gramps should automatically bop until another Bop event is reached.")
                    },
                    resizable = true,
                    defaultLength = 4f
                },
                new GameAction("turn right", "Turn Right")
                {
                    function = delegate { var e = eventCaller.currentEntity; SpaceDance.instance.DoTurnRight(e.beat, e["gramps"]); },
                    preFunction = delegate { var e = eventCaller.currentEntity; SpaceDance.TurnRightSfx(e.beat, e["whoSpeaks"]); },
                    defaultLength = 2.0f,
                    parameters = new List<Param>()
                    {
                        new Param("whoSpeaks", SpaceDance.WhoSpeaks.Dancers, "Speaker", "Choose who will say the voice line."),
                        new Param("gramps", false, "Space Gramps Animations", "Toggle if Space Gramps will turn right with the dancers.")
                    }
                },
                new GameAction("sit down", "Sit Down")
                {
                    function = delegate { var e = eventCaller.currentEntity; SpaceDance.instance.DoSitDown(e.beat, e["gramps"]); },
                    preFunction = delegate { var e = eventCaller.currentEntity; SpaceDance.SitDownSfx(e.beat, e["whoSpeaks"]); },
                    defaultLength = 2.0f,
                    parameters = new List<Param>()
                    {
                        new Param("whoSpeaks", SpaceDance.WhoSpeaks.Dancers, "Speaker", "Choose who will say the voice line."),
                        new Param("gramps", false, "Space Gramps Animations", "Toggle if Space Gramps will sit down with the dancers.")
                    }
                },
                new GameAction("punch", "Punch")
                {
                    function = delegate { var e = eventCaller.currentEntity; SpaceDance.instance.DoPunch(e.beat, e["gramps"]); },
                    preFunction = delegate { var e = eventCaller.currentEntity; SpaceDance.PunchSfx(e.beat, e["whoSpeaks"]); },
                    defaultLength = 2.0f,
                    parameters = new List<Param>()
                    {
                        new Param("whoSpeaks", SpaceDance.WhoSpeaks.Dancers, "Speaker", "Choose who will say the voice line."),
                        new Param("gramps", false, "Space Gramps Animations", "Toggle if Space Gramps will punch with the dancers.")
                    }
                },
                new GameAction("shootingStar", "Shooting Star")
                {
                    function = delegate { var e = eventCaller.currentEntity; SpaceDance.instance.UpdateShootingStar(e.beat, e.length, (EasingFunction.Ease)e["ease"]); },
                    defaultLength = 2f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("ease", EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.")
                    }
                },
                new GameAction("changeBG", "Change Background Color")
                {
                    function = delegate {var e = eventCaller.currentEntity; SpaceDance.instance.BackgroundColor(e.beat, e.length, e["start"], e["end"], e["ease"]); },
                    defaultLength = 1f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("start", SpaceDance.defaultBGColor, "Start Color", "Set the color at the start of the event."),
                        new Param("end", SpaceDance.defaultBGColor, "End Color", "Set the color at the end of the event."),
                        new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.")
                    }
                },
                new GameAction("grampsAnims", "Space Gramps Animations")
                {
                    function = delegate {var e = eventCaller.currentEntity; SpaceDance.instance.GrampsAnimations(e.beat, e["type"], e["toggle"]); },
                    defaultLength = 0.5f,
                    parameters = new List<Param>()
                    {
                        new Param("toggle", true, "Loop", "Toggle if the animation should loop."),
                        new Param("type", SpaceDance.GrampsAnimationType.Talk, "Animation", "Set the animation for Space Gramps to perform.")
                    }
                },
                new GameAction("scroll", "Scrolling Background")
                {
                    function = delegate { var e = eventCaller.currentEntity; SpaceDance.instance.UpdateScrollSpeed(e["x"], e["y"]); },
                    defaultLength = 1f,
                    parameters = new List<Param>() {
                        new Param("x", new EntityTypes.Float(-10f, 10f, 0), "Horizontal Speed", "Set how fast the background will scroll horizontally."),
                        new Param("y", new EntityTypes.Float(-10f, 10f, 0), "Vertical Speed", "Set how fast the background will scroll vertically."),
                    }
                },
            },
            new List<string>() {"agb", "normal"},
            "agbspacedance", "jp",
            new List<string>() {"jp"},
            chronologicalSortKey: 17
            );
        }
    }
}

namespace HeavenStudio.Games
{
    // using Scripts_SpaceDance;
    public class SpaceDance : Minigame
    {
        public static Color defaultBGColor = new(0f, 0.161f, 0.839f);
        public enum WhoSpeaks
        {
            Dancers = 0,
            Gramps = 1,
            Both = 2
        }
        public enum GrampsAnimationType
        {
            Stand = 0,
            Talk = 1,
            Sniff = 2
        }
        [SerializeField] SpriteRenderer bg;
        [SerializeField] Animator shootingStarAnim;
        public Animator DancerP;
        public Animator Dancer1;
        public Animator Dancer2;
        public Animator Dancer3;
        public Animator Gramps;
        public Animator Hit;
        public GameObject Player;
        bool canBop = true;
        bool grampsCanBop = true;
        public bool spaceGrampsShouldBop = false;
        float shootingStarLength;
        double shootingStarStartBeat;
        EasingFunction.Ease lastEase;
        bool isShootingStar;
        bool grampsLoopingAnim;
        bool grampsSniffing;

        [SerializeField] CanvasScroll scroll;
        float xScrollMultiplier = 0;
        float yScrollMultiplier = 0;
        [SerializeField] private float xBaseSpeed = 1;
        [SerializeField] private float yBaseSpeed = 1;

        public GameEvent bop = new GameEvent();

        public static SpaceDance instance;

        const int IA_TurnPress = IAMAXCAT;
        const int IA_DownPress = IAMAXCAT + 1;
        const int IA_PunchPress = IAMAXCAT + 2;

        protected static bool IA_PadTurnPress(out double dt)
        {
            return PlayerInput.GetPadDown(InputController.ActionsPad.Right, out dt);
        }
        protected static bool IA_BatonTurnPress(out double dt)
        {
            return PlayerInput.GetBatonDown(InputController.ActionsBaton.East, out dt)
                && !instance.IsExpectingInputNow(InputAction_Punch);
        }
        protected static bool IA_TouchTurnPress(out double dt)
        {
            return PlayerInput.GetTouchDown(InputController.ActionsTouch.Tap, out dt)
                && !(instance.IsExpectingInputNow(InputAction_Down) || instance.IsExpectingInputNow(InputAction_Punch));
        }

        protected static bool IA_PadDownPress(out double dt)
        {
            return PlayerInput.GetPadDown(InputController.ActionsPad.Down, out dt);
        }
        protected static bool IA_BatonDownPress(out double dt)
        {
            return PlayerInput.GetBatonDown(InputController.ActionsBaton.South, out dt)
                && !instance.IsExpectingInputNow(InputAction_Punch);
        }
        protected static bool IA_TouchDownPress(out double dt)
        {
            return PlayerInput.GetTouchDown(InputController.ActionsTouch.Tap, out dt)
                && instance.IsExpectingInputNow(InputAction_Down);
        }

        protected static bool IA_BatonPunchPress(out double dt)
        {
            return PlayerInput.GetBatonDown(InputController.ActionsBaton.Face, out dt)
                && instance.IsExpectingInputNow(InputAction_Punch);
        }
        protected static bool IA_TouchPunchPress(out double dt)
        {
            return PlayerInput.GetTouchDown(InputController.ActionsTouch.Tap, out dt)
                && instance.IsExpectingInputNow(InputAction_Punch);
        }

        public static PlayerInput.InputAction InputAction_Turn =
            new("AgbSpaceDanceTurn", new int[] { IA_TurnPress, IA_TurnPress, IA_TurnPress },
            IA_PadTurnPress, IA_TouchTurnPress, IA_BatonTurnPress);
        public static PlayerInput.InputAction InputAction_Down =
            new("AgbSpaceDanceDown", new int[] { IA_DownPress, IA_DownPress, IA_DownPress },
            IA_PadDownPress, IA_TouchDownPress, IA_BatonDownPress);
        public static PlayerInput.InputAction InputAction_Punch =
            new("AgbSpaceDancePunch", new int[] { IA_PunchPress, IA_PunchPress, IA_PunchPress },
            IA_PadBasicPress, IA_TouchPunchPress, IA_BatonPunchPress);

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
            SetupBopRegion("spaceDance", "bop", "auto");
        }

        public override void OnBeatPulse(double beat)
        {
            if (BeatIsInBopRegion(beat))
            {
                Bop();
            }
            if (spaceGrampsShouldBop)
            {
                GrampsBop();
            }
        }

        // Update is called once per frame
        void Update()
        {
            var cond = Conductor.instance;
            bg.color = bgColorEase.GetColor();
            if (cond.isPlaying && !cond.isPaused)
            {
                scroll.NormalizedX -= xBaseSpeed * xScrollMultiplier * Time.deltaTime;
                scroll.NormalizedY -= yBaseSpeed * yScrollMultiplier * Time.deltaTime;
                if (isShootingStar)
                {
                    float normalizedBeat = cond.GetPositionFromBeat(shootingStarStartBeat, shootingStarLength);
                    if (normalizedBeat >= 0)
                    {
                        if (normalizedBeat > 1)
                        {
                            isShootingStar = false;
                        }
                        else
                        {
                            EasingFunction.Function func = EasingFunction.GetEasingFunction(lastEase);
                            float newAnimPos = func(0f, 1f, normalizedBeat);
                            shootingStarAnim.DoNormalizedAnimation("ShootingStar", newAnimPos);
                        }
                    }
                }
                if (!DancerP.IsPlayingAnimationNames("PunchDo", "TurnRightDo", "SitDownDo"))
                {
                    if (PlayerInput.GetIsAction(InputAction_Punch) && !IsExpectingInputNow(InputAction_Punch))
                    {
                        SoundByte.PlayOneShotGame("spaceDance/inputBad");
                        DancerP.DoScaledAnimationAsync("PunchDo", 0.5f);
                        Gramps.Play("GrampsOhFuck", 0, 0);
                    }
                    if (PlayerInput.GetIsAction(InputAction_Down) && !IsExpectingInputNow(InputAction_Down))
                    {
                        DancerP.DoScaledAnimationAsync("SitDownDo", 0.5f);
                        SoundByte.PlayOneShotGame("spaceDance/inputBad");
                        Gramps.Play("GrampsOhFuck", 0, 0);
                    }
                    if (PlayerInput.GetIsAction(InputAction_Turn) && !IsExpectingInputNow(InputAction_Turn))
                    {
                        DancerP.DoScaledAnimationAsync("TurnRightDo", 0.5f);
                        SoundByte.PlayOneShotGame("spaceDance/inputBad");
                        Gramps.Play("GrampsOhFuck", 0, 0);
                    }
                }
            }
        }

        public void UpdateScrollSpeed(float scrollSpeedX, float scrollSpeedY)
        {
            xScrollMultiplier = scrollSpeedX;
            yScrollMultiplier = scrollSpeedY;
        }

        public void GrampsAnimations(double beat, int type, bool looping)
        {
            switch (type)
            {
                case (int)GrampsAnimationType.Stand:
                    Gramps.Play("GrampsStand", 0, 0);
                    grampsLoopingAnim = false;
                    grampsSniffing = false;
                    break;
                case (int)GrampsAnimationType.Talk:
                    if (looping)
                    {
                        grampsLoopingAnim = true;
                        grampsSniffing = false;
                        GrampsTalkLoop(beat);
                    }
                    else
                    {
                        grampsLoopingAnim = false;
                        grampsSniffing = false;
                        Gramps.DoScaledAnimationAsync("GrampsTalk", 0.5f);
                    }
                    break;
                case (int)GrampsAnimationType.Sniff:
                    if (looping)
                    {
                        grampsLoopingAnim = true;
                        grampsSniffing = true;
                        GrampsSniffLoop(beat);
                    }
                    else
                    {
                        grampsLoopingAnim = false;
                        grampsSniffing = false;
                        Gramps.DoScaledAnimationAsync("GrampsSniff", 0.5f);
                    }
                    break;
            }
        }

        void GrampsSniffLoop(double beat)
        {
            if (!grampsLoopingAnim || !grampsSniffing) return;
            spaceGrampsShouldBop = false;
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate
                {
                    if (grampsSniffing && grampsLoopingAnim)
                    {
                        Gramps.DoScaledAnimationAsync("GrampsSniff", 0.5f);
                    }
                }),
                new BeatAction.Action(beat + 3, delegate
                {
                    if (grampsSniffing && grampsLoopingAnim)
                    {
                        Gramps.DoScaledAnimationAsync("GrampsSniff", 0.5f);
                    }
                }),
                new BeatAction.Action(beat + 3.5f, delegate
                {
                    if (grampsSniffing && grampsLoopingAnim)
                    {
                        Gramps.DoScaledAnimationAsync("GrampsSniff", 0.5f);
                    }
                }),
                new BeatAction.Action(beat + 5.5f, delegate
                {
                    GrampsSniffLoop(beat + 5.5f);
                }),
            });
        }

        void GrampsTalkLoop(double beat)
        {
            if (!grampsLoopingAnim || grampsSniffing) return;
            spaceGrampsShouldBop = false;
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 0.66666f , delegate
                {
                    if (!grampsSniffing && grampsLoopingAnim)
                    {
                        Gramps.DoScaledAnimationAsync("GrampsTalk", 0.5f);
                    }
                }),
                new BeatAction.Action(beat + 1.33333f, delegate
                {
                    if (!grampsSniffing && grampsLoopingAnim)
                    {
                        Gramps.DoScaledAnimationAsync("GrampsTalk", 0.5f);
                    }
                }),
                new BeatAction.Action(beat + 2f, delegate
                {
                    if (!grampsSniffing && grampsLoopingAnim)
                    {
                        Gramps.DoScaledAnimationAsync("GrampsTalk", 0.5f);
                    }
                }),
                new BeatAction.Action(beat + 3f, delegate
                {
                    if (!grampsSniffing && grampsLoopingAnim)
                    {
                        Gramps.DoScaledAnimationAsync("GrampsTalk", 0.5f);
                    }
                }),
                new BeatAction.Action(beat + 3.5f, delegate
                {
                    if (!grampsSniffing && grampsLoopingAnim)
                    {
                        Gramps.DoScaledAnimationAsync("GrampsTalk", 0.5f);
                    }
                }),
                new BeatAction.Action(beat + 4f, delegate
                {
                    GrampsTalkLoop(beat + 4f);
                }),
            });
        }

        public void UpdateShootingStar(double beat, float length, EasingFunction.Ease ease)
        {
            lastEase = ease;
            shootingStarLength = length;
            shootingStarStartBeat = beat;
            isShootingStar = true;
        }

        public static void TurnRightSfx(double beat, int whoSpeaks)
        {
            List<MultiSound.Sound> soundsToPlay = new List<MultiSound.Sound>()
            {
                new MultiSound.Sound("spaceDance/voicelessTurn", beat),
            };

            switch (whoSpeaks)
            {
                case (int)WhoSpeaks.Dancers:
                    soundsToPlay.AddRange(new List<MultiSound.Sound>()
                    {
                        new MultiSound.Sound("spaceDance/dancerTurn", beat),
                        new MultiSound.Sound("spaceDance/dancerRight", beat + 1.0f, 1, 1, false, 0.012f),
                    });
                    break;
                case (int)WhoSpeaks.Gramps:
                    soundsToPlay.AddRange(new List<MultiSound.Sound>()
                    {
                        new MultiSound.Sound("spaceDance/otherTurn", beat),
                        new MultiSound.Sound("spaceDance/otherRight", beat + 1.0f, 1, 1, false, 0.005f),
                    });
                    break;
                case (int)WhoSpeaks.Both:
                    soundsToPlay.AddRange(new List<MultiSound.Sound>()
                    {
                        new MultiSound.Sound("spaceDance/dancerTurn", beat),
                        new MultiSound.Sound("spaceDance/dancerRight", beat + 1.0f, 1, 1, false, 0.012f),
                        new MultiSound.Sound("spaceDance/otherTurn", beat),
                        new MultiSound.Sound("spaceDance/otherRight", beat + 1.0f, 1, 1, false, 0.005f),
                    });
                    break;
            }

            MultiSound.Play(soundsToPlay.ToArray(), true, true);
        }

        public void DoTurnRight(double beat, bool grampsTurns)
        {
            canBop = false;
            if (grampsTurns) grampsCanBop = false;
            ScheduleInput(beat, 1f, InputAction_Turn, JustRight, RightMiss, null);

            BeatAction.New(instance, new List<BeatAction.Action>() 
            {
                new BeatAction.Action(beat,     delegate 
                {
                    DancerP.DoScaledAnimationAsync("TurnRightStart", 0.5f);
                    Dancer1.DoScaledAnimationAsync("TurnRightStart", 0.5f);
                    Dancer2.DoScaledAnimationAsync("TurnRightStart", 0.5f);
                    Dancer3.DoScaledAnimationAsync("TurnRightStart", 0.5f);
                    if (grampsTurns) Gramps.DoScaledAnimationAsync("GrampsTurnRightStart", 0.5f);
                }),
                new BeatAction.Action(beat + 1f,     delegate 
                {
                    Dancer1.DoScaledAnimationAsync("TurnRightDo", 0.5f);
                    Dancer2.DoScaledAnimationAsync("TurnRightDo", 0.5f);
                    Dancer3.DoScaledAnimationAsync("TurnRightDo", 0.5f);
                    if (grampsTurns) Gramps.DoScaledAnimationAsync("GrampsTurnRightDo", 0.5f);
                }),
                new BeatAction.Action(beat + 1.5f,     delegate { canBop = true; grampsCanBop = true; }),
            });

        }

        public static void SitDownSfx(double beat, int whoSpeaks)
        {
            List<MultiSound.Sound> soundsToPlay = new List<MultiSound.Sound>()
            {
                new MultiSound.Sound("spaceDance/voicelessSit", beat),
            };

            switch (whoSpeaks)
            {
                case (int)WhoSpeaks.Dancers:
                    soundsToPlay.AddRange(new List<MultiSound.Sound>()
                    {
                        new MultiSound.Sound("spaceDance/dancerLets", beat, 1, 1, false, 0.055f),
                        new MultiSound.Sound("spaceDance/dancerSit", beat + 0.5f, 1, 1, false, 0.05f),
                        new MultiSound.Sound("spaceDance/dancerDown", beat + 1f, 1, 1, false, 0.004f),
                    });
                    break;
                case (int)WhoSpeaks.Gramps:
                    soundsToPlay.AddRange(new List<MultiSound.Sound>()
                    {
                        new MultiSound.Sound("spaceDance/otherLets", beat, 1, 1, false, 0.02f),
                        new MultiSound.Sound("spaceDance/otherSit", beat + 0.5f, 1, 1, false, 0.064f),
                        new MultiSound.Sound("spaceDance/otherDown", beat + 1f, 1, 1, false, 0.01f),
                    });
                    break;
                case (int)WhoSpeaks.Both:
                    soundsToPlay.AddRange(new List<MultiSound.Sound>()
                    {
                        new MultiSound.Sound("spaceDance/dancerLets", beat, 1, 1, false, 0.055f),
                        new MultiSound.Sound("spaceDance/dancerSit", beat + 0.5f, 1, 1, false, 0.05f),
                        new MultiSound.Sound("spaceDance/dancerDown", beat + 1f, 1, 1, false, 0.004f),
                        new MultiSound.Sound("spaceDance/otherLets", beat, 1, 1, false, 0.02f),
                        new MultiSound.Sound("spaceDance/otherSit", beat + 0.5f, 1, 1, false, 0.064f),
                        new MultiSound.Sound("spaceDance/otherDown", beat + 1f, 1, 1, false, 0.01f),
                    });
                    break;
            }

            MultiSound.Play(soundsToPlay.ToArray(), true, true);
        }

        public void DoSitDown(double beat, bool grampsSits)
        {
            canBop = false;
            if (grampsSits) grampsCanBop = false;
            ScheduleInput(beat, 1f, InputAction_Down, JustSit, SitMiss, null);

            BeatAction.New(instance, new List<BeatAction.Action>() 
            {
                new BeatAction.Action(beat,     delegate 
                {
                    DancerP.DoScaledAnimationAsync("SitDownStart", 0.5f);
                    Dancer1.DoScaledAnimationAsync("SitDownStart", 0.5f);
                    Dancer2.DoScaledAnimationAsync("SitDownStart", 0.5f);
                    Dancer3.DoScaledAnimationAsync("SitDownStart", 0.5f);
                    if (grampsSits) Gramps.DoScaledAnimationAsync("GrampsSitDownStart", 0.5f);
                }),
                new BeatAction.Action(beat + 1f,     delegate 
                {
                    Dancer1.DoScaledAnimationAsync("SitDownDo", 0.5f);
                    Dancer2.DoScaledAnimationAsync("SitDownDo", 0.5f);
                    Dancer3.DoScaledAnimationAsync("SitDownDo", 0.5f);
                    if (grampsSits) Gramps.DoScaledAnimationAsync("GrampsSitDownDo", 0.5f);
                }),
                new BeatAction.Action(beat + 1.5f,     delegate { canBop = true; grampsCanBop = true; }),
            });

        }

        public static void PunchSfx(double beat, int whoSpeaks)
        {
            List<MultiSound.Sound> soundsToPlay = new List<MultiSound.Sound>()
            {
                new MultiSound.Sound("spaceDance/voicelessPunch", beat),
                new MultiSound.Sound("spaceDance/voicelessPunch", beat + 0.5f),
                new MultiSound.Sound("spaceDance/voicelessPunch", beat + 1f),
            };

            switch (whoSpeaks)
            {
                case (int)WhoSpeaks.Dancers:
                    soundsToPlay.AddRange(new List<MultiSound.Sound>()
                    {
                        new MultiSound.Sound("spaceDance/dancerPa", beat),
                        new MultiSound.Sound("spaceDance/dancerPa", beat + 0.5f),
                        new MultiSound.Sound("spaceDance/dancerPa", beat + 1f),
                        new MultiSound.Sound("spaceDance/dancerPunch", beat + 1.5f),
                    });
                    break;
                case (int)WhoSpeaks.Gramps:
                    soundsToPlay.AddRange(new List<MultiSound.Sound>()
                    {
                        new MultiSound.Sound("spaceDance/otherPa", beat),
                        new MultiSound.Sound("spaceDance/otherPa", beat + 0.5f),
                        new MultiSound.Sound("spaceDance/otherPa", beat + 1f),
                        new MultiSound.Sound("spaceDance/otherPunch", beat + 1.5f),
                    });
                    break;
                case (int)WhoSpeaks.Both:
                    soundsToPlay.AddRange(new List<MultiSound.Sound>()
                    {
                        new MultiSound.Sound("spaceDance/dancerPa", beat),
                        new MultiSound.Sound("spaceDance/dancerPa", beat + 0.5f),
                        new MultiSound.Sound("spaceDance/dancerPa", beat + 1f),
                        new MultiSound.Sound("spaceDance/dancerPunch", beat + 1.5f),
                        new MultiSound.Sound("spaceDance/otherPa", beat),
                        new MultiSound.Sound("spaceDance/otherPa", beat + 0.5f),
                        new MultiSound.Sound("spaceDance/otherPa", beat + 1f),
                        new MultiSound.Sound("spaceDance/otherPunch", beat + 1.5f),
                    });
                    break;
            }

            MultiSound.Play(soundsToPlay.ToArray(), true, true);
        }

        public void DoPunch(double beat, bool grampsPunches)
        {
            canBop = false;
            if (grampsPunches) grampsCanBop = false;
            ScheduleInput(beat, 1.5f, InputAction_Punch, JustPunch, PunchMiss, null);

            BeatAction.New(instance, new List<BeatAction.Action>() 
                {
                new BeatAction.Action(beat, delegate 
                {
                    DancerP.DoScaledAnimationAsync("PunchStartInner", 0.5f);
                    Dancer1.DoScaledAnimationAsync("PunchStartInner", 0.5f);
                    Dancer2.DoScaledAnimationAsync("PunchStartInner", 0.5f);
                    Dancer3.DoScaledAnimationAsync("PunchStartInner", 0.5f);
                    if (grampsPunches) Gramps.DoScaledAnimationAsync("GrampsPunchStartOdd", 0.5f);
                }),
                new BeatAction.Action(beat + 0.5f,  delegate 
                {
                    DancerP.DoScaledAnimationAsync("PunchStartOuter", 0.5f);
                    Dancer1.DoScaledAnimationAsync("PunchStartOuter", 0.5f);
                    Dancer2.DoScaledAnimationAsync("PunchStartOuter", 0.5f);
                    Dancer3.DoScaledAnimationAsync("PunchStartOuter", 0.5f);
                    if (grampsPunches) Gramps.DoScaledAnimationAsync("GrampsPunchStartEven", 0.5f);
                }),
                new BeatAction.Action(beat + 1f, delegate 
                {
                    DancerP.DoScaledAnimationAsync("PunchStartInner", 0.5f);
                    Dancer1.DoScaledAnimationAsync("PunchStartInner", 0.5f);
                    Dancer2.DoScaledAnimationAsync("PunchStartInner", 0.5f);
                    Dancer3.DoScaledAnimationAsync("PunchStartInner", 0.5f);
                    if (grampsPunches) Gramps.DoScaledAnimationAsync("GrampsPunchStartOdd", 0.5f);
                }),
                new BeatAction.Action(beat + 1.5f, delegate 
                {
                    Dancer1.DoScaledAnimationAsync("PunchDo", 0.5f);
                    Dancer2.DoScaledAnimationAsync("PunchDo", 0.5f);
                    Dancer3.DoScaledAnimationAsync("PunchDo", 0.5f);
                    if (grampsPunches) Gramps.DoScaledAnimationAsync("GrampsPunchDo", 0.5f);
                }),
                new BeatAction.Action(beat + 2.5, delegate
                {
                    canBop = true; grampsCanBop = true;
                })
                });

        }

        public void EpicBop(double beat, float length, bool autoDancers, bool dancers, bool autoGramps, bool gramps)
        {
            spaceGrampsShouldBop = autoGramps;
            if (dancers || gramps)
            {
                List<BeatAction.Action> bops = new List<BeatAction.Action>();
                for (int i = 0; i < length; i++)
                {
                    if (dancers)
                    {
                        bops.Add(new BeatAction.Action(beat + i, delegate { Bop(); }));
                    }
                    if (gramps)
                    {
                        bops.Add(new BeatAction.Action(beat + i, delegate { GrampsBop(); }));
                    }
                }
                BeatAction.New(instance, bops);
            }
        }

        public void Bop()
        {
            if (!canBop) return;
            DancerP.DoScaledAnimationAsync("Bop", 0.5f);
            Dancer1.DoScaledAnimationAsync("Bop", 0.5f);
            Dancer2.DoScaledAnimationAsync("Bop", 0.5f);
            Dancer3.DoScaledAnimationAsync("Bop", 0.5f);
        }

        public void GrampsBop()
        {
            if (!grampsCanBop) return;
            Gramps.DoScaledAnimationAsync("GrampsBop", 0.5f);
        }

        private ColorEase bgColorEase = new(defaultBGColor);

        public void BackgroundColor(double beat, float length, Color startColor, Color endColor, int ease)
        {
            bgColorEase = new(beat, length, startColor, endColor, ease);
        }

        //call this in OnPlay(double beat) and OnGameSwitch(double beat)
        private void PersistColor(double beat)
        {
            var allEventsBeforeBeat = EventCaller.GetAllInGameManagerList("spaceDance", new string[] { "changeBG" }).FindAll(x => x.beat < beat);
            if (allEventsBeforeBeat.Count > 0)
            {
                allEventsBeforeBeat.Sort((x, y) => x.beat.CompareTo(y.beat)); //just in case
                var lastEvent = allEventsBeforeBeat[^1];
                BackgroundColor(lastEvent.beat, lastEvent.length, lastEvent["start"], lastEvent["end"], lastEvent["ease"]);
            }
        }

        public override void OnPlay(double beat)
        {
            PersistColor(beat);
        }

        public override void OnGameSwitch(double beat)
        {
            PersistColor(beat);
        }

        public void JustRight(PlayerActionEvent caller, float state)
        {
            if (state >= 1f || state <= -1f)
            {
                SoundByte.PlayOneShotGame("spaceDance/inputBad");
                DancerP.DoScaledAnimationAsync("TurnRightDo", 0.5f);
                Gramps.DoScaledAnimationAsync("GrampsOhFuck", 0.5f);
                return;
            }
            RightSuccess();
        }

        public void RightSuccess()
        {
            SoundByte.PlayOneShotGame("spaceDance/inputGood");
            DancerP.DoScaledAnimationAsync("TurnRightDo", 0.5f);
        }

        public void RightMiss(PlayerActionEvent caller)
        {
            SoundByte.PlayOneShotGame("spaceDance/inputBad2");
            DancerP.DoScaledAnimationAsync("Ouch", 0.5f);
            Hit.Play("HitTurn", -1, 0);
            Gramps.DoScaledAnimationAsync("GrampsMiss", 0.5f);
        }

        public void JustSit(PlayerActionEvent caller, float state)
        {
            if (state >= 1f || state <= -1f)
            {
                SoundByte.PlayOneShotGame("spaceDance/inputBad");
                DancerP.DoScaledAnimationAsync("SitDownDo", 0.5f);
                Gramps.DoScaledAnimationAsync("GrampsOhFuck", 0.5f);
                return;
            }
            SitSuccess();
        }

        public void SitSuccess()
        {
            SoundByte.PlayOneShotGame("spaceDance/inputGood");
            DancerP.DoScaledAnimationAsync("SitDownDo", 0.5f);
        }

        public void SitMiss(PlayerActionEvent caller)
        {
            SoundByte.PlayOneShotGame("spaceDance/inputBad2");
            DancerP.DoScaledAnimationAsync("Ouch", 0.5f);
            Hit.Play("HitSit", -1, 0);
            Gramps.DoScaledAnimationAsync("GrampsMiss", 0.5f);
        }

        public void JustPunch(PlayerActionEvent caller, float state)
        {
            if (state >= 1f || state <= -1f)
            {
                SoundByte.PlayOneShotGame("spaceDance/inputBad");
                DancerP.DoScaledAnimationAsync("PunchDo", 0.5f);
                Gramps.DoScaledAnimationAsync("GrampsOhFuck", 0.5f);
                return;
            }
            PunchSuccess();
        }

        public void PunchSuccess()
        {
            SoundByte.PlayOneShotGame("spaceDance/inputGood");
            DancerP.DoScaledAnimationAsync("PunchDo", 0.5f);
        }

        public void PunchMiss(PlayerActionEvent caller)
        {
            SoundByte.PlayOneShotGame("spaceDance/inputBad2");
            DancerP.DoScaledAnimationAsync("Ouch", 0.5f);
            Hit.Play("HitPunch", -1, 0);
            Gramps.DoScaledAnimationAsync("GrampsMiss", 0.5f);
        }
    }
}