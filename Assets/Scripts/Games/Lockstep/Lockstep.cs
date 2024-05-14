/* I do not know crap about Unity or C#
Almost none of this code is mine, but it's all fair game when the game you're stealing from
borrowed from other games */
//Don't worry Raffy everyone starts somewhere - Rasmus

using HeavenStudio.Util;
using Jukebox;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class NtrBackbeatLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("lockstep", "Lockstep", "f0338d", false, false, new List<GameAction>()
            {
                new GameAction("bop", "Bop")
                {
                    function = delegate { var e = eventCaller.currentEntity; Lockstep.instance.Bop(e.beat, e.length, e["toggle"], e["toggle2"]); },
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("toggle", true, "Bop", "Toggle if the stepswitchers should bop for the duration of this event."),
                        new Param("toggle2", false, "Bop (Auto)", "Toggle if the stepswitchers should automatically bop until another Bop event is reached."),
                    },
                },
                new GameAction("stepping", "Stepping")
                {
                    preFunction = delegate {var e = eventCaller.currentEntity; Lockstep.Marching(e.beat, e["sound"], e["amount"], e["visual"]);},
                    parameters = new List<Param>()
                    {
                        new Param("sound", false, "Voice", "Toggle if voice sounds should play automatically when stepping starts. It will automatically switch between \"Hai!\" and \"Ho!\" if it is on or off beat.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "amount" })
                        }),
                        new Param("amount", new EntityTypes.Integer(1, 50, 1), "Amount", "Set how many sounds will play."),
                        new Param("visual", true, "Background Visual", "Toggle if the background will automatically flip depending on if it's on or off beat.")
                    },
                    preFunctionLength = 1
                },
                new GameAction("offbeatSwitch", "Switch to Offbeat")
                {
                    preFunction = delegate { var e = eventCaller.currentEntity; Lockstep.OffbeatSwitchSound(e.beat, e["ho"], e["sound"]); },
                    defaultLength = 4.5f,
                    parameters = new List<Param>()
                    {
                        new Param("sound", true, "Sound Cue", "Toggle if the \"Hai! Hai! Hai! Ha-Ha!\" sound should be played."),
                        new Param("ho", true, "End Sounds", "Toggle if the \"Ho! Ho! Ho! Ho!\" sound should be played."),
                        new Param("visual", true, "Background Visual", "Toggle if the background will automatically flip depending on if it's on or off beat.")
                    }
                },
                new GameAction("onbeatSwitch", "Switch to Onbeat")
                {
                    preFunction = delegate { var e = eventCaller.currentEntity; Lockstep.OnbeatSwitchSound(e.beat, e["hai"], e["sound"]); },
                    defaultLength = 3f,
                    parameters = new List<Param>()
                    {
                        new Param("sound", true, "Sound Cue", "Toggle if the \"Mm-ha! Mm-ha! Hai!\" sound should be played."),
                        new Param("hai", new EntityTypes.Integer(0, 100, 1), "Set how many \"Hai!\" sounds will play."),
                        new Param("visual", true, "Background Visual", "Toggle if the background will automatically flip depending on if it's on or off beat.")
                    }
                },
                new GameAction("hai", "Hai")
                {
                    preFunction = delegate { Lockstep.HaiSound(eventCaller.currentEntity.beat); }
                },
                new GameAction("ho", "Ho")
                {
                    preFunction = delegate { Lockstep.HoSound(eventCaller.currentEntity.beat); }
                },
                new GameAction("set colours", "Set Colors")
                {
                    function = delegate {var e = eventCaller.currentEntity; Lockstep.instance.SetBackgroundColours(e["colorA"], e["colorB"], e["objColA"], e["objColB"], e["objColC"]); },
                    parameters = new List<Param>()
                    {
                        new Param("colorA", Lockstep.defaultBGColorOn, "Background Onbeat", "Set the color that appears for the onbeat."),
                        new Param("colorB", Lockstep.defaultBGColorOff, "Background Offbeat", "Set the color that appears for the offbeat."),
                        new Param("objColA", Lockstep.stepperOut, "Stepper Outline", "Set the color used for the outline of the stepswitchers."),
                        new Param("objColB", Lockstep.stepperDark, "Stepper Dark", "Set the color that appears for the dark side of the stepwitchers."),
                        new Param("objColC", Lockstep.stepperLight, "Stepper Light", "Set the color that appears for the light side of the stepwitchers."),
                    },
                    defaultLength = 0.5f,
                },
                new GameAction("zoom", "Zoom Camera")
                {
                    function = delegate { Lockstep.instance.SetZoom(eventCaller.currentEntity["zoom"]); },
                    parameters = new List<Param>()
                    {
                        new Param("zoom", Lockstep.ZoomPresets.Regular, "Zoom Level", "Set the level to zoom to.")
                    }
                },
                new GameAction("bach", "Show Bach")
                {
                    defaultLength = 4,
                    resizable = true,
                },
                new GameAction("marching", "Force Stepping")
                {
                    preFunction = delegate {var e = eventCaller.currentEntity; Lockstep.Marching(e.beat, e["sound"], e["amount"], e["visual"], true, e.length);},
                    parameters = new List<Param>()
                    {
                        new Param("sound", false, "Voice", "Toggle if voice sounds should play automatically when stepping starts. It will automatically switch between \"Hai!\" and \"Ho!\" if it is on or off beat.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "amount" })
                        }),
                        new Param("amount", new EntityTypes.Integer(1, 50, 1), "Amount", "Set how many sounds will play."),
                        new Param("visual", true, "Background Visual", "Toggle if the background will automatically flip depending on if it's on or off beat."),
                    },
                    preFunctionLength = 1,
                    resizable = true,
                    defaultLength = 4
                }
            },
            new List<string>() { "ntr", "keep" },
            "ntrbackbeat", "en",
            new List<string>() { },
            chronologicalSortKey: 27
            );

        }
    }
}

namespace HeavenStudio.Games
{
    // using Scripts_Lockstep;
    public class Lockstep : Minigame
    {
        private static Color _defaultBGColorOn;
        public static Color defaultBGColorOn
        {
            get
            {
                ColorUtility.TryParseHtmlString("#f0338d", out _defaultBGColorOn);
                return _defaultBGColorOn;
            }
        }

        private static Color _defaultBGColorOff;
        public static Color defaultBGColorOff
        {
            get
            {
                ColorUtility.TryParseHtmlString("#BC318B", out _defaultBGColorOff);
                return _defaultBGColorOff;
            }
        }

        private static Color _stepperDark;
        public static Color stepperDark
        {
            get
            {
                ColorUtility.TryParseHtmlString("#737373", out _stepperDark);
                return _stepperDark;
            }
        }

        private static Color _stepperLight;
        public static Color stepperLight
        {
            get
            {
                ColorUtility.TryParseHtmlString("#FFFFFF", out _stepperLight);
                return _stepperLight;
            }
        }

        private static Color _stepperOut;
        public static Color stepperOut
        {
            get
            {
                ColorUtility.TryParseHtmlString("#9A2760", out _stepperOut);
                return _stepperOut;
            }
        }

        public Color currentBGOnColor;
        public Color currentBGOffColor;

        [Header("Components")]
        [SerializeField] Animator stepswitcherPlayer;
        [SerializeField] Animator stepswitcherLeft;
        [SerializeField] Animator stepswitcherRight;
        [SerializeField] Animator bach;

        // master stepper dictates what sprite the slave steppers use
        [SerializeField] Animator masterStepperAnim;
        [SerializeField] SpriteRenderer masterStepperSprite;

        // slave steppers copy the sprite of the master stepper
        [SerializeField] SpriteRenderer[] slaveSteppers;

        // rendertextures update when the slave steppers change sprites
        [SerializeField] Vector2 rtSize;
        [SerializeField] Camera cameraNear1, cameraNear2, cameraDV;

        [SerializeField] SpriteRenderer background;
        [SerializeField] Material playerMaterial, stepperMaterial;
        [SerializeField] Material topNear, bottomNear, distantView;

        [Header("Properties")]
        static List<QueuedMarch> queuedInputs = new();
        RenderTexture[] renderTextures;
        Sprite masterSprite;
        HowMissed currentMissStage;
        bool lessSteppers = false;
        public enum HowMissed
        {
            NotMissed = 0,
            MissedOff = 1,
            MissedOn = 2
        }
        bool offColorActive;
        bool goBop;
        public GameEvent bop = new GameEvent();
        List<double> switches = new();
        private List<RiqEntity> bachEvents = new();

        public static Lockstep instance;

        public enum ZoomPresets
        {
            Regular,
            NotThatFar,
            Far,
            VeryFar,
            ExtremelyFar
        }

        void Awake()
        {
            instance = this;
            currentBGOnColor = defaultBGColorOn;
            currentBGOffColor = defaultBGColorOff;
            var switchEvents = EventCaller.GetAllInGameManagerList("lockstep", new string[] { "onbeatSwitch", "offbeatSwitch" });

            foreach (var switchEvent in switchEvents)
            {
                switches.Add(switchEvent.beat + switchEvent.length - 1);
            }

            bachEvents = EventCaller.GetAllInGameManagerList("lockstep", new string[] { "bach" });

            renderTextures = new RenderTexture[3];
            renderTextures[0] = new RenderTexture((int)rtSize.x, (int)rtSize.y, 24, RenderTextureFormat.ARGB32)
            {
                wrapMode = TextureWrapMode.Repeat,
            };
            renderTextures[1] = new RenderTexture((int)rtSize.x, (int)rtSize.y, 24, RenderTextureFormat.ARGB32)
            {
                wrapMode = TextureWrapMode.Repeat,
            };
            renderTextures[2] = new RenderTexture((int)rtSize.x, (int)rtSize.y, 24, RenderTextureFormat.ARGB32)
            {
                wrapMode = TextureWrapMode.Repeat,
            };

            cameraNear1.targetTexture = renderTextures[0];
            cameraNear2.targetTexture = renderTextures[1];
            cameraDV.targetTexture = renderTextures[2];

            // topT.texture = renderTextures[2];
            // topN.texture = renderTextures[0];
            // bottomL.texture = renderTextures[2];
            // bottomC.texture = renderTextures[2];
            // bottomR.texture = renderTextures[2];
            // bottomN.texture = renderTextures[1];

            topNear.SetTexture("_MainTex", renderTextures[0]);
            bottomNear.SetTexture("_MainTex", renderTextures[1]);
            distantView.SetTexture("_MainTex", renderTextures[2]);
        }

        void OnDestroy()
        {
            foreach (var rt in renderTextures)
            {
                rt.Release();
            }
            queuedInputs.Clear();
        }

        private static bool ForceStepOnBeat(double beat)
        {
            return EventCaller.GetAllInGameManagerList("lockstep", new string[] { "marching" }).Find(x => beat >= x.beat && beat < x.beat + x.length) != null;
        }

        private void PersistColors(double beat)
        {
            var allEventsBeforeBeat = EventCaller.GetAllInGameManagerList("lockstep", new string[] { "set colours" }).FindAll(x => x.beat < beat);
            if (allEventsBeforeBeat.Count > 0)
            {
                allEventsBeforeBeat.Sort((x, y) => x.beat.CompareTo(y.beat));
                var lastEvent = allEventsBeforeBeat[^1];
                SetBackgroundColours(lastEvent["colorA"], lastEvent["colorB"], lastEvent["objColA"], lastEvent["objColB"], lastEvent["objColC"]);
            }
        }

        private bool BachOnBeat(double beat)
        {
            return bachEvents.Find(x => beat >= x.beat && beat < x.beat + x.length) != null;
        }
        public override void OnGameSwitch(double beat)
        {
            QueueSwitchBGs(beat);

        }

        public override void OnPlay(double beat)
        {

            queuedInputs.Clear();
            QueueSwitchBGs(beat);

        }

        private void QueueSwitchBGs(double beat)
        {
            double nextGameSwitchBeat = double.MaxValue;
            List<RiqEntity> allEnds = EventCaller.GetAllInGameManagerList("gameManager", new string[] { "switchGame", "end" }).FindAll(x => x.beat > beat);
            if (allEnds.Count > 0)
            {
                nextGameSwitchBeat = allEnds[0].beat;
            }

            var switchEventsOn = EventCaller.GetAllInGameManagerList("lockstep", new string[] { "onbeatSwitch" });
            foreach (var on in switchEventsOn)
            {
                if (on.beat >= nextGameSwitchBeat) continue;
                OnbeatSwitch(on.beat, beat, on["visual"]);
            }

            var switchEventsOff = EventCaller.GetAllInGameManagerList("lockstep", new string[] { "offbeatSwitch" });
            foreach (var off in switchEventsOff)
            {
                if (off.beat >= nextGameSwitchBeat) continue;
                OffbeatSwitch(off.beat, beat, off["visual"]);
            }
        }

        void Start()
        {
            stepperMaterial.SetColor("_ColorAlpha", stepperOut);
            stepperMaterial.SetColor("_ColorBravo", stepperDark);
            stepperMaterial.SetColor("_ColorDelta", stepperLight);

            playerMaterial.SetColor("_ColorAlpha", stepperOut);
            playerMaterial.SetColor("_ColorBravo", stepperDark);
            playerMaterial.SetColor("_ColorDelta", stepperLight);

            EntityPreCheck(Conductor.instance.songPositionInBeatsAsDouble);


            masterSprite = masterStepperSprite.sprite;
            stepswitcherLeft.gameObject.SetActive(lessSteppers);
            stepswitcherRight.gameObject.SetActive(lessSteppers);
            masterStepperAnim.gameObject.SetActive(!lessSteppers);

            UpdateAndRenderSlaves();

            cameraNear1.Render();
            cameraNear2.Render();
            cameraDV.Render();
        }

        void EntityPreCheck(double beat)
        {
            PersistColors(beat);
        }

        void UpdateAndRenderSlaves()
        {
            foreach (var stepper in slaveSteppers)
            {
                stepper.sprite = masterSprite;
            }
        }

        void PlayStepperAnim(string animName, bool player, float timescale = 1f, float startpos = 0f, int layer = -1)
        {
            if (player) stepswitcherPlayer.DoScaledAnimationAsync(animName, timescale, startpos, layer);
            if (lessSteppers)
            {
                stepswitcherLeft.DoScaledAnimationAsync(animName, timescale, startpos, layer);
                stepswitcherRight.DoScaledAnimationAsync(animName, timescale, startpos, layer);
            }
            else
            {
                masterStepperAnim.DoScaledAnimationAsync(animName, timescale, startpos, layer);
            }
        }

        public override void OnBeatPulse(double beat)
        {
            if (goBop)
            {
                PlayStepperAnim("Bop", true, 0.5f);
            }
        }

        private void Update()
        {
            if (conductor.isPlaying && !conductor.isPaused)
            {
                if (queuedInputs.Count > 0)
                {
                    foreach (var input in queuedInputs)
                    {
                        if (input.force)
                        {
                            ForceMarching(input.beat, input.length, input.sound, input.amount, input.visual);
                        }
                        else
                        {
                            StartMarching(input.beat, input.sound, input.amount, input.visual);
                        }
                    }
                    queuedInputs.Clear();
                }
                if (PlayerInput.GetIsAction(InputAction_BasicPress) && !IsExpectingInputNow(InputAction_BasicPress))
                {
                    currentMissStage = HowMissed.NotMissed;
                    double beatAnimCheck = conductor.songPositionInBeatsAsDouble - 0.25;
                    if (beatAnimCheck % 1.0 >= 0.5)
                    {
                        stepswitcherPlayer.DoScaledAnimationAsync("OnbeatMarch", 0.5f);
                    }
                    else
                    {
                        stepswitcherPlayer.DoScaledAnimationAsync("OffbeatMarch", 0.5f);
                    }
                    SoundByte.PlayOneShot("miss");
                    ScoreMiss();
                }
            }
        }

        private void LateUpdate()
        {
            if (masterSprite != masterStepperSprite.sprite)
            {
                masterSprite = masterStepperSprite.sprite;
                UpdateAndRenderSlaves();
            }
        }

        public void SetZoom(int zoom)
        {
            GameCamera.AdditionalPosition = new Vector3(0, 0, (ZoomPresets)zoom switch
            {
                ZoomPresets.Regular => 0,
                ZoomPresets.NotThatFar => -4.5f,
                ZoomPresets.Far => -11,
                ZoomPresets.VeryFar => -26,
                ZoomPresets.ExtremelyFar => -63,
                _ => throw new System.NotImplementedException()
            });
        }

        public void Bop(double beat, float length, bool shouldBop, bool autoBop)
        {
            goBop = autoBop;
            if (shouldBop)
            {
                for (int i = 0; i < length; i++)
                {
                    BeatAction.New(instance, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(beat + i, delegate
                        {
                            PlayStepperAnim("Bop", true, 0.5f);
                        })
                    });
                }
            }
        }

        public static void HaiSound(double beat)
        {
            SoundByte.PlayOneShot("games/lockstep/hai", beat, 1, 1, false, null, 0.018);
        }

        public static void HoSound(double beat)
        {
            SoundByte.PlayOneShot("games/lockstep/ho", beat, 1, 1, false, null, 0.015);
        }

        public static void OnbeatSwitchSound(double beat, int hais, bool sound)
        {
            if (sound)
            {
                MultiSound.Play(new MultiSound.Sound[]
                {
                new MultiSound.Sound("lockstep/nha1", beat, 1, 1, false, 0),
                new MultiSound.Sound("lockstep/nha2", beat + 0.5f, 1, 1, false, 0.01),
                new MultiSound.Sound("lockstep/nha1", beat + 1f, 1, 1, false, 0),
                new MultiSound.Sound("lockstep/nha2", beat + 1.5f, 1, 1, false, 0.01)
                }, forcePlay: true);
            }

            if (hais > 0)
            {
                List<MultiSound.Sound> haisList = new();

                for (int i = 0; i < hais; i++)
                {
                    haisList.Add(new MultiSound.Sound("lockstep/hai", beat + 2 + i, 1, 1, false, 0.018));
                }

                double nextOffBeat = double.MaxValue;
                var switchEventsOn = EventCaller.GetAllInGameManagerList("lockstep", new string[] { "offbeatSwitch" });
                switchEventsOn.Sort((x, y) => x.beat.CompareTo(y.beat));
                for (int i = 0; i < switchEventsOn.Count; i++)
                {
                    if (switchEventsOn[i].beat > beat)
                    {
                        nextOffBeat = switchEventsOn[i].beat;
                        break;
                    }
                }

                var haisActual = haisList.FindAll(x => x.beat < nextOffBeat);

                MultiSound.Play(haisActual.ToArray(), true, true);
            }
        }

        private void OnbeatSwitch(double beat, double gameswitchBeat, bool visual)
        {
            List<BeatAction.Action> allActions = new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { if(visual) ChangeBeatBackGroundColour(false); }),
                new BeatAction.Action(beat + 0.5f, delegate { if (visual) ChangeBeatBackGroundColour(true); }),
                new BeatAction.Action(beat + 1f, delegate
                {
                    if(visual) ChangeBeatBackGroundColour(false);
                }),
                new BeatAction.Action(beat + 1.5f, delegate
                {
                    if (visual) ChangeBeatBackGroundColour(true);
                }),
                new BeatAction.Action(beat + 1.75f, delegate { if (!marchRecursing && !ForceStepOnBeat(beat + 2f)) MarchRecursive(beat + 2f); }),
                new BeatAction.Action(beat + 2f, delegate { if (visual) ChangeBeatBackGroundColour(false); }),
            };
            List<BeatAction.Action> actions = new();
            foreach (var action in allActions)
            {
                if (action.beat >= gameswitchBeat) actions.Add(action);
            }
            if (actions.Count > 0) BeatAction.New(instance, actions);
        }

        public static void OffbeatSwitchSound(double beat, bool hoSound, bool sound)
        {
            if (sound)
            {
                MultiSound.Play(new MultiSound.Sound[]
                {
                    new MultiSound.Sound("lockstep/hai", beat, 1, 1, false, 0.018),
                    new MultiSound.Sound("lockstep/hai", beat + 1f, 1, 1, false, 0.018),
                    new MultiSound.Sound("lockstep/hai", beat + 2f, 1, 1, false, 0.018),
                    new MultiSound.Sound("lockstep/hahai1", beat + 3f, 1, 1, false, 0),
                    new MultiSound.Sound("lockstep/hahai2", beat + 3.5f, 1, 1, false, 0.014),
                }, forcePlay: true);
            }
            if (hoSound)
            {
                List<MultiSound.Sound> hos = new List<MultiSound.Sound>
                {
                    new MultiSound.Sound("lockstep/ho", beat + 4.5f, 1, 1, false, 0.015),
                    new MultiSound.Sound("lockstep/ho", beat + 5.5f, 1, 0.6835514f, false, 0.015),
                    new MultiSound.Sound("lockstep/ho", beat + 6.5f, 1, 0.3395127f, false, 0.015),
                    new MultiSound.Sound("lockstep/ho", beat + 7.5f, 1, 0.1200322f, false, 0.015),
                };

                double nextOnBeat = double.MaxValue;
                var switchEventsOn = EventCaller.GetAllInGameManagerList("lockstep", new string[] { "onbeatSwitch" });
                switchEventsOn.Sort((x, y) => x.beat.CompareTo(y.beat));
                for (int i = 0; i < switchEventsOn.Count; i++)
                {
                    if (switchEventsOn[i].beat > beat)
                    {
                        nextOnBeat = switchEventsOn[i].beat;
                        break;
                    }
                }

                var hosActual = hos.FindAll(x => x.beat < nextOnBeat);

                MultiSound.Play(hosActual.ToArray(), true, true);
            }
        }

        private void OffbeatSwitch(double beat, double gameswitchBeat, bool visual)
        {
            List<BeatAction.Action> allActions = new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { if (visual) ChangeBeatBackGroundColour(true); }),
                new BeatAction.Action(beat + 1f, delegate { if (visual) ChangeBeatBackGroundColour(false); }),
                new BeatAction.Action(beat + 2f, delegate { if (visual) ChangeBeatBackGroundColour(true); }),
                new BeatAction.Action(beat + 3f, delegate
                {
                    if (visual) ChangeBeatBackGroundColour(false);
                }),
                new BeatAction.Action(beat + 3.25f, delegate { if (!marchRecursing && !ForceStepOnBeat(beat + 3.5)) MarchRecursive(beat + 3.5f); }),
                new BeatAction.Action(beat + 3.5f, delegate { if (visual) ChangeBeatBackGroundColour(true); }),
            };
            List<BeatAction.Action> actions = new();
            foreach (var action in allActions)
            {
                if (action.beat >= gameswitchBeat) actions.Add(action);
            }
            if (actions.Count > 0) BeatAction.New(instance, actions);
        }

        private struct QueuedMarch
        {
            public double beat;
            public float length;
            public bool sound;
            public int amount;
            public bool visual;
            public bool force;
        }
        public static void Marching(double beat, bool sound, int amount, bool visual, bool force = false, float length = 0)
        {
            if (GameManager.instance.currentGame == "lockstep")
            {
                if (force)
                {
                    instance.ForceMarching(beat, length, sound, amount, visual);
                }
                else
                {
                    instance.StartMarching(beat, sound, amount, visual);
                }
            }
            else
            {
                queuedInputs.Add(new QueuedMarch
                {
                    amount = amount,
                    beat = beat,
                    sound = sound,
                    visual = visual,
                    length = length,
                    force = force
                });
            }
        }

        private void ForceMarching(double beat, float length, bool sound, int amount, bool visual)
        {
            bool offBeat = beat % 1 != 0;
            if (sound)
            {
                MultiSound.Sound[] sounds = new MultiSound.Sound[amount];
                for (int i = 0; i < amount; i++)
                {
                    sounds[i] = new MultiSound.Sound($"lockstep/" + (offBeat ? "ho" : "hai"), beat + i, 1, 1, false, offBeat ? 0.015 : 0.018);
                }
                MultiSound.Play(sounds, true, true);
            }
            List<BeatAction.Action> steps = new()
            {
                new BeatAction.Action(beat, delegate
                {
                    if (visual) ChangeBeatBackGroundColour(offBeat);
                    if (BachOnBeat(beat)) bach.DoScaledAnimationAsync(offBeat ? "BachOff" : "BachOn", 0.5f);
                    EvaluateMarch(offBeat);
                })
            };
            ScheduleInput(beat - 1, 1, InputAction_BasicPress, offBeat ? JustOff : JustOn, offBeat ? MissOff : MissOn, Nothing);
            for (int i = 1; i < length; i++)
            {
                double stepBeat = beat + i;
                steps.Add(new BeatAction.Action(stepBeat, delegate
                {
                    if (BachOnBeat(stepBeat)) bach.DoScaledAnimationAsync(offBeat ? "BachOff" : "BachOn", 0.5f);
                    EvaluateMarch(offBeat);
                }));
                ScheduleInput(stepBeat - 1, 1, InputAction_BasicPress, offBeat ? JustOff : JustOn, offBeat ? MissOff : MissOn, Nothing);
            }
            BeatAction.New(this, steps);
        }

        private void StartMarching(double beat, bool sound, int amount, bool visual)
        {
            if (marchRecursing) return;
            bool offBeat = beat % 1 != 0;
            if (visual)
            {
                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat, delegate { ChangeBeatBackGroundColour(offBeat); })
                });
            }
            if (sound)
            {
                MultiSound.Sound[] sounds = new MultiSound.Sound[amount];
                for (int i = 0; i < amount; i++)
                {
                    sounds[i] = new MultiSound.Sound($"lockstep/" + (offBeat ? "ho" : "hai"), beat + i, 1, 1, false, offBeat ? 0.015 : 0.018);
                }
                MultiSound.Play(sounds, true, true);
            }
            MarchRecursive(beat);
        }

        private bool marchRecursing;
        private void MarchRecursive(double beat)
        {
            marchRecursing = true;
            if (NextStepIsSwitch(beat)) beat -= 0.5;
            bool offBeat = beat % 1 != 0;
            bool bachOnBeat = BachOnBeat(beat);
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate
                {
                    if (gameManager.currentGame != "lockstep") return;
                    ScheduleInput(beat - 1, 1, InputAction_BasicPress, offBeat ? JustOff : JustOn, offBeat ? MissOff : MissOn, Nothing);
                    EvaluateMarch(offBeat);
                    MarchRecursive(beat + 1);
                    if (bachOnBeat) bach.DoScaledAnimationAsync(offBeat ? "BachOff" : "BachOn", 0.5f);
                }),
            });
        }

        private bool NextStepIsSwitch(double beat)
        {
            return switches.Contains(beat - 0.5);
        }

        public void EvaluateMarch(bool offBeat)
        {
            if (offBeat)
            {
                PlayStepperAnim("OffbeatMarch", false, 0.5f);
            }
            else
            {
                PlayStepperAnim("OnbeatMarch", false, 0.5f);
            }
        }

        private void JustOn(PlayerActionEvent caller, float state)
        {
            currentMissStage = HowMissed.NotMissed;
            stepswitcherPlayer?.DoScaledAnimationAsync("OnbeatMarch", 0.5f);
            if (state >= 1f || state <= -1f)
            {
                SoundByte.PlayOneShot("nearMiss");
                return;
            }
            SoundByte.PlayOneShotGame($"lockstep/foot{UnityEngine.Random.Range(1, 3)}");
            SoundByte.PlayOneShotGame("lockstep/drumOn");
        }

        private void JustOff(PlayerActionEvent caller, float state)
        {
            currentMissStage = HowMissed.NotMissed;
            stepswitcherPlayer?.DoScaledAnimationAsync("OffbeatMarch", 0.5f);
            if (state >= 1f || state <= -1f)
            {
                SoundByte.PlayOneShot("nearMiss");
                return;
            }
            SoundByte.PlayOneShotGame($"lockstep/foot{UnityEngine.Random.Range(1, 3)}");
            SoundByte.PlayOneShotGame("lockstep/drumOff");
        }

        private void MissOn(PlayerActionEvent caller)
        {
            if (gameManager.currentGame != "lockstep") return;
            if (currentMissStage == HowMissed.MissedOn) return;
            if (stepswitcherPlayer is not null && stepswitcherPlayer.isActiveAndEnabled)
            {
                stepswitcherPlayer?.Play("OnbeatMiss", 0, 0);
            }
            SoundByte.PlayOneShotGame("lockstep/wayOff");
            currentMissStage = HowMissed.MissedOn;
        }

        private void MissOff(PlayerActionEvent caller)
        {
            if (gameManager.currentGame != "lockstep") return;
            if (currentMissStage == HowMissed.MissedOff) return;
            if (stepswitcherPlayer is not null && stepswitcherPlayer.isActiveAndEnabled)
            {
                stepswitcherPlayer?.Play("OffbeatMiss", 0, 0);
            }
            SoundByte.PlayOneShotGame("lockstep/wayOff");
            currentMissStage = HowMissed.MissedOff;
        }

        public void ChangeBeatBackGroundColour(bool off)
        {
            if (off)
            {
                background.color = currentBGOffColor;
                offColorActive = true;
            }
            else
            {
                background.color = currentBGOnColor;
                offColorActive = false;
            }
        }

        public void SetBackgroundColours(Color onColor, Color offColor, Color outlineColor, Color darkColor, Color lightColor)
        {
            currentBGOnColor = onColor;
            currentBGOffColor = offColor;

            if (offColorActive)
            {
                background.color = currentBGOffColor;
            }
            else
            {
                background.color = currentBGOnColor;
            }

            stepperMaterial.SetColor("_ColorAlpha", outlineColor);
            stepperMaterial.SetColor("_ColorBravo", darkColor);
            stepperMaterial.SetColor("_ColorDelta", lightColor);

            playerMaterial.SetColor("_ColorAlpha", outlineColor);
            playerMaterial.SetColor("_ColorBravo", darkColor);
            playerMaterial.SetColor("_ColorDelta", lightColor);
        }

        public void Nothing(PlayerActionEvent caller) { }
    }
}
