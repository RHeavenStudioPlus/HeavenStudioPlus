using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Util;
using Jukebox;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class NtrFillbotsLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            var botParams = new List<Param>()
            {
                new Param("practice", false, "Count-In"),
                new Param("alt", false, "Alternate OK"),
                new Param("type", Scripts_Fillbots.EndAnim.Both, "Success Reaction", "Set the reaction of the Robot."),
                new Param("stop", false, "Stop Conveyor", "Toggle if the conveyor should be stopped when finished."),
                new Param("color", false, "Custom Color", "Toggle if the robot color should be changed.", new List<Param.CollapseParam>()
                {
                    new Param.CollapseParam((x, _) => (bool)x, new string[] { "colorFuel", "colorLampOff", "colorLampOn" }),
                }),
                new Param("colorFuel", new Color(1f, 0.385f, 0.385f), "Fuel Color", "Set the color of the fuel."),
                new Param("colorLampOff", new Color(0.635f, 0.635f, 0.185f), "Off Lamp Color", "Set the color of the off lamp."),
                new Param("colorLampOn", new Color(1f, 1f, 0.42f), "On Lamp Color", "Set the color of the on lamp."),
            };

            var customBotParams = new List<Param>(botParams);
            customBotParams.Insert(0,
                new Param("size", Scripts_Fillbots.BotSize.Medium, "Size", "Set the size of the Robot.")
            );
            
            return new Minigame("fillbots", "Fillbots", "FFFFFF", false, false, new List<GameAction>()
            {
                new("bop", "Bop")
                {
                    function = delegate
                    {
                        var e = eventCaller.currentEntity;
                        Fillbots.instance.ToggleBop(e.beat, e.length, e["toggle"], e["auto"]);
                    },
                    resizable = true,
                    parameters = new()
                    {
                        new("toggle", true, "Bop"),
                        new("auto", false, "Bop (Auto)")
                    }
                },
                new GameAction("medium", "Medium Bot")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        Fillbots.PreSpawnFillbot(e.beat, 3, (int)Scripts_Fillbots.BotSize.Medium, e["colorFuel"], e["colorLampOff"], e["colorLampOn"], e["type"], e["alt"], e["stop"], e["color"]);
                        if (e["practice"]) Fillbots.FillErUp(e.beat + 3);
                    },
                    defaultLength = 8f,
                    parameters = botParams,
                },
                new GameAction("large", "Large Bot")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        Fillbots.PreSpawnFillbot(e.beat, 7, (int)Scripts_Fillbots.BotSize.Large, e["colorFuel"], e["colorLampOff"], e["colorLampOn"], e["type"], e["alt"], e["stop"], e["color"]);
                        if (e["practice"]) Fillbots.FillErUp(e.beat + 3);
                    },
                    defaultLength = 12f,
                    parameters = botParams,
                },
                new GameAction("small", "Small Bot")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        Fillbots.PreSpawnFillbot(e.beat, 1, (int)Scripts_Fillbots.BotSize.Small, e["colorFuel"], e["colorLampOff"], e["colorLampOn"], e["type"], e["alt"], e["stop"], e["color"]);
                        if (e["practice"]) Fillbots.FillErUp(e.beat + 3);
                    },
                    defaultLength = 6f,
                    parameters = botParams,
                },
                new GameAction("custom", "Custom Bot")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        Fillbots.PreSpawnFillbot(e.beat, e.length-5, e["size"], e["colorFuel"], e["colorLampOff"], e["colorLampOn"], e["type"], e["alt"], e["stop"], e["color"]);
                        if (e["practice"]) Fillbots.FillErUp(e.beat + 3);
                    },
                    defaultLength = 6f,
                    resizable = true,
                    parameters = customBotParams,
                },
                new GameAction("blackout", "Blackout")
                {
                    function = delegate { Fillbots.instance.Blackout();},
                    defaultLength = 0.5f,
                },
                new GameAction("background appearance", "Background Appearance")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        Fillbots.instance.BackgroundColorSet(e.beat, e.length, e["colorBGStart"], e["colorBGEnd"],
                            e["colorMetersStart"], e["colorMeter1Start"], e["colorMeter2Start"], e["colorMeter3Start"], e["colorMeter4Start"], e["colorMeter5Start"], e["colorMeter6Start"],
                            e["colorMetersEnd"], e["colorMeter1End"], e["colorMeter2End"], e["colorMeter3End"], e["colorMeter4End"], e["colorMeter5End"], e["colorMeter6End"],
                            e["separate"], e["ease"]);
                    },
                    defaultLength = 0.5f,
                    resizable = true, 
                    parameters = new List<Param>()
                    {
                        new Param("colorBGStart", Color.white, "Start BG Color", "Set the color at the start of the event."),
                        new Param("colorBGEnd", Color.white, "End BG Color", "Set the color at the end of the event."),
                        new Param("ease", Util.EasingFunction.Ease.Instant, "Ease", "Set the easing of the action."),
                        new Param("separate", false, "Separate Meter Color", "Toggle if the robot color should be changed.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => !(bool)x, new string[] { "colorMetersStart", "colorMetersEnd" }),
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "colorMeter1Start", "colorMeter2Start", "colorMeter3Start", "colorMeter4Start", "colorMeter5Start", "colorMeter6Start",
                                                                                      "colorMeter1End", "colorMeter2End", "colorMeter3End", "colorMeter4End", "colorMeter5End", "colorMeter6End" }),
                        }),
                        new Param("colorMetersStart", new Color(1f, 0.88f, 0.88f), "Start Meter Color", "Set the color at the start of the event."),
                        new Param("colorMetersEnd", new Color(1f, 0.88f, 0.88f), "End Meter Color", "Set the color at the end of the event."),
                        new Param("colorMeter1Start", new Color(1f, 0.88f, 0.88f), "Start 1st Meter Color", "Set the color at the start of the event."),
                        new Param("colorMeter1End", new Color(1f, 0.88f, 0.88f), "End 1st Meter Color", "Set the color at the end of the event."),
                        new Param("colorMeter2Start", new Color(1f, 0.88f, 0.88f), "Start 2nd Meter Color", "Set the color at the start of the event."),
                        new Param("colorMeter2End", new Color(1f, 0.88f, 0.88f), "End 2nd Meter Color", "Set the color at the end of the event."),
                        new Param("colorMeter3Start", new Color(1f, 0.88f, 0.88f), "Start 3rd Meter Color", "Set the color at the start of the event."),
                        new Param("colorMeter3End", new Color(1f, 0.88f, 0.88f), "End 3rd Meter Color", "Set the color at the end of the event."),
                        new Param("colorMeter4Start", new Color(1f, 0.88f, 0.88f), "Start 4th Meter Color", "Set the color at the start of the event."),
                        new Param("colorMeter4End", new Color(1f, 0.88f, 0.88f), "End 4th Meter Color", "Set the color at the end of the event."),
                        new Param("colorMeter5Start", new Color(1f, 0.88f, 0.88f), "Start 5th Meter Color", "Set the color at the start of the event."),
                        new Param("colorMeter5End", new Color(1f, 0.88f, 0.88f), "End 5th Meter Color", "Set the color at the end of the event."),
                        new Param("colorMeter6Start", new Color(1f, 0.88f, 0.88f), "Start 6th Meter Color", "Set the color at the start of the event."),
                        new Param("colorMeter6End", new Color(1f, 0.88f, 0.88f), "End 6th Meter Color", "Set the color at the end of the event."),
                    }
                },
                new GameAction("object appearance", "Object Appearance")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        Fillbots.instance.ObjectColorSet(e["colorFuel"], e["colorLampOff"], e["colorLampOn"], e["colorImpact"], e["colorFiller"], e["colorConveyer"]);
                    },
                    defaultLength = 0.5f,
                    parameters = new List<Param>()
                    {
                        new Param("colorFuel", new Color(1f, 0.385f, 0.385f), "Fuel Color", "Set the color of the fuel."),
                        new Param("colorLampOff", new Color(0.635f, 0.635f, 0.185f), "Off Lamp Color", "Set the color of the off lamp."),
                        new Param("colorLampOn", new Color(1f, 1f, 0.42f), "On Lamp Color", "Set the color of the on lamp."),
                        new Param("colorImpact", new Color(1f, 0.59f, 0.01f), "Impact Color", "Set the color of the impact."),
                        new Param("colorFiller", Color.white, "Filler Color", "Set the color of the filler."),
                        new Param("colorConveyer", Color.white, "Conveyor Color", "Set the color of the conveyor."),
                    }
                },
            },
            new List<string>() {"ntr", "normal"},
            "ntrfillbots", "en",
            new List<string>() {},
            chronologicalSortKey: 3
            );
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_Fillbots;
    using System;

    public class Fillbots : Minigame
    {
        private struct QueuedFillbot
        {
            public double beat;
            public double holdLength;
            public BotSize size;
            public Color fuelColor;
            public Color lampColorOff;
            public Color lampColorOn;
            public EndAnim endAnim;
            public bool altOK;
            public bool stop;
            public bool color;
        }
        private static List<QueuedFillbot> queuedBots = new List<QueuedFillbot>();

        [Header("Components")]
        [SerializeField] private NtrFillbot smallBot;
        [SerializeField] private NtrFillbot mediumBot;
        [SerializeField] private NtrFillbot largeBot;
        public Animator filler;
        [System.NonSerialized] public bool fillerHolding;
        [SerializeField] private Transform[] gears;
        [SerializeField] private Animator[] meters;
        [SerializeField] private SpriteRenderer[] metersFuel;
        [SerializeField] private Material impactMaterial;
        [SerializeField] private Animator conveyerBelt;
        [SerializeField] private GameObject blackout;
        [SerializeField] private SpriteRenderer[] fillerRenderer;
        [SerializeField] private SpriteRenderer[] otherRenderer;
        [SerializeField] private SpriteRenderer BGPlane;

        [System.NonSerialized] public BotSize fillerPosition = BotSize.Medium;

        [NonSerialized] public List<NtrFillbot> currentBots = new List<NtrFillbot>();

        [NonSerialized] public double conveyerStartBeat = -1;

        [NonSerialized] public float conveyerNormalizedOffset;

        private ColorEase[] colorEases = new ColorEase[7];
        private int toggleGlobal = 0;
        private Color fuelColorGlobal = new Color(1f, 0.385f, 0.385f),
                      lampColorOffGlobal = new Color(0.635f, 0.635f, 0.185f),
                      lampColorOnGlobal = new Color(1f, 1f, 0.42f);

        public static Fillbots instance;

        private void Awake()
        {
            instance = this;
            SetupBopRegion("fillbots", "bop", "auto");

            colorEases = new ColorEase[] {
                new(Color.white),
                new(new Color(1f, 0.88f, 0.88f)),
                new(new Color(1f, 0.88f, 0.88f)),
                new(new Color(1f, 0.88f, 0.88f)),
                new(new Color(1f, 0.88f, 0.88f)),
                new(new Color(1f, 0.88f, 0.88f)),
                new(new Color(1f, 0.88f, 0.88f)),
            };
        }

        public override void OnBeatPulse(double beat)
        {
            if (BeatIsInBopRegion(beat)) {
                Bop(toggleGlobal);
                toggleGlobal ^= 1;
            }
        }

        public override void OnPlay(double beat)
        {
            if (queuedBots.Count > 0) queuedBots.Clear();
            foreach (var evt in scheduledInputs)
            {
                evt.Disable();
            }
        }

        private void OnDestroy()
        {
            if (queuedBots.Count > 0) queuedBots.Clear();
            foreach (var evt in scheduledInputs)
            {
                evt.Disable();
            }
        }

        public override void OnStop(double beat) => EntityPreCheck(beat);
        void EntityPreCheck(double beat)
        {
            if (gameManager == null) return;
            List<RiqEntity> prevEntities = gameManager.Beatmap.Entities.FindAll(c => c.datamodel.Split(0) == "fillbots");

            // init colors
            RiqEntity bg = prevEntities.FindLast(c => c.beat <= beat && c.datamodel == "fillbots/background appearance");
            RiqEntity obj = prevEntities.FindLast(c => c.beat <= beat && c.datamodel == "fillbots/object appearance");

            if (bg != null)
            {
                BackgroundColorSet(bg.beat, bg.length, bg["colorBGStart"], bg["colorBGEnd"],
                            bg["colorMetersStart"], bg["colorMeter1Start"], bg["colorMeter2Start"], bg["colorMeter3Start"], bg["colorMeter4Start"], bg["colorMeter5Start"], bg["colorMeter6Start"],
                            bg["colorMetersEnd"], bg["colorMeter1End"], bg["colorMeter2End"], bg["colorMeter3End"], bg["colorMeter4End"], bg["colorMeter5End"], bg["colorMeter6End"],
                            bg["separate"], bg["ease"]);
            }

            if (obj != null)
            {
                ObjectColorSet(obj["colorFuel"], obj["colorLampOff"], obj["colorLampOn"], obj["colorImpact"], obj["colorFiller"], obj["colorConveyer"]);
            }
            else
            {
                ObjectColorSet(new Color(1f, 0.385f, 0.385f), new Color(0.635f, 0.635f, 0.185f), new Color(1f, 1f, 0.42f), new Color(1f, 0.59f, 0.01f), Color.white, Color.white);
            }
        }

        private void Update()
        {
            var cond = Conductor.instance;
            if (cond.isPlaying && !cond.isPaused)
            {
                if (queuedBots.Count > 0)
                {
                    foreach (var queuedBot in queuedBots)
                    {
                        SpawnFillbot(queuedBot.beat, queuedBot.holdLength, queuedBot.size, queuedBot.fuelColor, queuedBot.lampColorOff, queuedBot.lampColorOn, queuedBot.endAnim, queuedBot.altOK, queuedBot.stop, queuedBot.color);
                    }
                    queuedBots.Clear();
                }
                if (PlayerInput.GetIsAction(InputAction_BasicPress) && !IsExpectingInputNow(InputAction_BasicPress))
                {
                    string sizeSuffix = fillerPosition switch
                    {
                        BotSize.Small => "Small",
                        BotSize.Medium => "Medium",
                        BotSize.Large => "Large",
                        _ => throw new System.NotImplementedException()
                    };
                    filler.DoScaledAnimationAsync("Hold" + sizeSuffix, 0.5f);
                    SoundByte.PlayOneShotGame("fillbots/armExtension");
                }
                if (PlayerInput.GetIsAction(InputAction_BasicRelease) && !IsExpectingInputNow(InputAction_BasicRelease))
                {
                    string sizeSuffix = fillerPosition switch
                    {
                        BotSize.Small => "Small",
                        BotSize.Medium => "Medium",
                        BotSize.Large => "Large",
                        _ => throw new System.NotImplementedException()
                    };
                    filler.DoScaledAnimationAsync("ReleaseWhiff" + sizeSuffix, 0.5f);
                    SoundByte.PlayOneShotGame("fillbots/armRetractionWhiff");
                    if (fillerHolding) SoundByte.PlayOneShotGame("fillbots/armRetractionPop");
                }

                UpdateConveyerBelt(conveyerStartBeat, conveyerNormalizedOffset);
                UpdateBackgroundColor();
            }
        }

        public static void PreSpawnFillbot(double beat, double holdLength, int size, Color fuelColor, Color lampColorOff, Color lampColorOn, int endAnim, bool altOK, bool stop, bool color)
        {
            if (GameManager.instance.currentGame == "fillbots")
            {
                instance.SpawnFillbot(beat, holdLength, (BotSize)size, fuelColor, lampColorOff, lampColorOn, (EndAnim)endAnim, altOK, stop, color);
            }
            else
            {
                queuedBots.Add(new QueuedFillbot
                {
                    beat = beat,
                    holdLength = holdLength,
                    size = (BotSize)size,
                    fuelColor = fuelColor,
                    lampColorOff = lampColorOff,
                    lampColorOn = lampColorOn,
                    endAnim = (EndAnim)endAnim,
                    altOK = altOK,
                    stop = stop,
                    color = color,
                });
            }
        }

        private void SpawnFillbot(double beat, double holdLength, BotSize size, Color fuelColor, Color lampColorOff, Color lampColorOn, EndAnim endAnim, bool altOK, bool stop, bool color)
        {
            NtrFillbot Bot = size switch
            {
                BotSize.Small => smallBot,
                BotSize.Medium => mediumBot,
                BotSize.Large => largeBot,
                _ => throw new System.NotImplementedException()
            };
            NtrFillbot spawnedBot = Instantiate(Bot, transform);

            if (holdLength > 0) spawnedBot.holdLength = holdLength;

            spawnedBot.startBeat = beat;
            spawnedBot.endAnim = endAnim;
            spawnedBot.altOK = altOK;

            spawnedBot.Init();
            
            var actions = new List<BeatAction.Action>();
            var fallingBots = currentBots.FindAll(x => x.startBeat < beat && x.startBeat + 3 >= beat);
            if (fallingBots.Count > 0) {
                actions.Add(new BeatAction.Action(beat - 0.25, delegate
                {
                    foreach (var bot in fallingBots)
                    {
                        bot.StackToLeft(beat, 0.25);
                    }
                    if (conveyerStartBeat is -2) conveyerStartBeat = beat - 0.25;
                }));
                actions.Add(new BeatAction.Action(beat, delegate
                {
                    RenewConveyerNormalizedOffset();
                    conveyerStartBeat = -2;
                }));
            } else {
                actions.Add(new BeatAction.Action(beat - 0.5, delegate
                {
                    RenewConveyerNormalizedOffset();
                    conveyerStartBeat = -2;
                }));
            }

            actions.Add(new BeatAction.Action(beat, delegate
            {
                if (!color)
                {
                    fuelColor = fuelColorGlobal;
                    lampColorOff = lampColorOffGlobal;
                    lampColorOn = lampColorOnGlobal;
                }
                spawnedBot.InitColor(fuelColor, lampColorOff, lampColorOn);
            }));

            actions.Add(new BeatAction.Action(beat + 3, delegate
            {
                if (!PlayerInput.GetIsAction(InputAction_BasicPressing) && !fillerHolding) filler.DoScaledAnimationAsync("FillerPrepare", 0.5f);
                conveyerStartBeat = beat + 3;
                fillerPosition = size;
            }));

            var remainingBots = currentBots.FindAll(x => x.conveyerRestartLength < 0);
            if (stop) spawnedBot.conveyerRestartLength = -1;
            actions.Add(new BeatAction.Action(beat + 3, delegate
            {
                foreach (var bot in remainingBots)
                {
                    if (bot.botState is BotState.Idle) bot.conveyerRestartLength = 0.5;
                    if (bot.conveyerStartBeat is -2) bot.conveyerStartBeat = beat + 3;
                }
            }));

            BeatAction.New(instance, actions);
        }
        public static void FillErUp(double beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("fillbots/fillErUp1", beat - 0.5),
                new MultiSound.Sound("fillbots/fillErUp2", beat - 0.25),
                new MultiSound.Sound("fillbots/fillErUp3", beat),
            }, forcePlay: true);
        }

        public void ToggleBop(double beat, float length, bool bopOrNah, bool autoBop)
        {
            if (bopOrNah)
            {
                for (int i = 0; i < length; i++)
                {
                    BeatAction.New(instance, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(beat + i, delegate {
                            Bop(toggleGlobal);
                            toggleGlobal ^= 1;
                        })
                    });
                }
            }
        }
        private void Bop(int toggle)
        {
            toggle = (toggle != 0) ? 1 : 0;
            foreach (var meter in meters)
            {
                meter.DoScaledAnimationAsync(toggle switch
                {
                    0 => "Up",
                    1 or _ => "Down"
                }, 0.5f);
                toggle ^= 1;
            }

            var danceBots = currentBots.FindAll(x => x.botState is BotState.Dance);
            foreach (var bot in danceBots)
            {
                bot.SuccessDance();
            }
        }

        private void UpdateConveyerBelt(double startBeat, float offset)
        {
            float normalizedBeat = Conductor.instance.GetPositionFromBeat(startBeat, 1);
            float playTime = ((startBeat >= 0 && normalizedBeat >= 0) ? (normalizedBeat + offset) : offset) % 1 / 4;

            conveyerBelt.Play("Move", -1, playTime);
            UpdateGears((startBeat >= 0 && normalizedBeat >= 0) ? normalizedBeat + offset : offset);
        }
        private void UpdateGears(float beat)
        {
            foreach (var gear in gears)
            {
                gear.localEulerAngles = new Vector3(0, 0, Mathf.LerpUnclamped(0, -90, beat));
            }
        }
        public void RenewConveyerNormalizedOffset()
        {
            if (conveyerStartBeat is not -1 or -2)
            {
                float normalizedBeat = Conductor.instance.GetPositionFromBeat(conveyerStartBeat, 1);
                if (normalizedBeat >= 0) conveyerNormalizedOffset = (conveyerNormalizedOffset + normalizedBeat) % 4;
            }
        }

        public void Blackout()
        {
            blackout.SetActive(!blackout.activeSelf);
        }

        public void BackgroundColorSet(double beat, float length, Color BGStart, Color BGEnd, 
            Color metersStart, Color meter1Start, Color meter2Start, Color meter3Start, Color meter4Start, Color meter5Start, Color meter6Start,
            Color metersEnd, Color meter1End, Color meter2End, Color meter3End, Color meter4End, Color meter5End, Color meter6End,
            bool separate, int colorEaseSet)
        {
            colorEases = new ColorEase[] {
                new(beat, length, BGStart, BGEnd, colorEaseSet),
                new(beat, length, meter1Start, meter1End, colorEaseSet),
                new(beat, length, meter2Start, meter2End, colorEaseSet),
                new(beat, length, meter3Start, meter3End, colorEaseSet),
                new(beat, length, meter4Start, meter4End, colorEaseSet),
                new(beat, length, meter5Start, meter5End, colorEaseSet),
                new(beat, length, meter6Start, meter6End, colorEaseSet),
            };
            if (!separate)
            {
                for (int i = 1; i < 7; i++)
                {
                    colorEases[i] = new(beat, length, metersStart, metersEnd, colorEaseSet);
                }
            }

            UpdateBackgroundColor();
        }
        public void ObjectColorSet(Color fuelColor, Color lampColorOff, Color lampColorOn, Color impact, Color filler, Color conveyer)
        {
            fuelColorGlobal = fuelColor; lampColorOffGlobal = lampColorOff; lampColorOnGlobal = lampColorOn;

            impactMaterial.SetColor("_ColorAlpha", impact);
            this.conveyerBelt.GetComponent<SpriteRenderer>().color = conveyer;
            foreach (var renderer in fillerRenderer)
            {
                renderer.color = filler;
            }
        }

        private void UpdateBackgroundColor()
        {
            BGPlane.color = colorEases[0].GetColor();
            foreach (var renderer in otherRenderer)
            {
                renderer.color = colorEases[0].GetColor();
            }

            for (int i = 0; i < metersFuel.Length; i++)
            {
                metersFuel[i].color = colorEases[i+1].GetColor();
            }
        }
    }
}

