using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using HeavenStudio.Util;
using Jukebox;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class AgbUpbeatLoader
    {
        public static Minigame AddGame(EventCaller eventCaller) {
            RiqEntity BackgroundUpdater(string datamodel, RiqEntity e)
            {
                if (datamodel == "mrUpbeat/changeBG" && e.dynamicData.ContainsKey("toggle") && !e.dynamicData.ContainsKey("ease"))
                {
                    e.CreateProperty("ease", (int)(e["toggle"] ? Util.EasingFunction.Ease.Instant : Util.EasingFunction.Ease.Linear));
                    e.dynamicData.Remove("toggle");
                    return e;
                }
                return null;
            }
            RiqBeatmap.OnUpdateEntity += BackgroundUpdater;

            return new Minigame("mrUpbeat", "Mr. Upbeat", "E0E0E0", false, false, new List<GameAction>()
            {
                new GameAction("prepare", "Prepare")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        MrUpbeat.PrePrepare(e.beat, e.length, e["forceOnbeat"]);
                    },
                    parameters = new List<Param>()
                    {
                        new Param("forceOnbeat", false, "Mr. Downbeat", "Toggle if Mr. Upbeat should step on the beat of the block instead of on the offbeat. Only use this if you know what you're doing."),
                    },
                    preFunctionLength = 0.5f,
                    defaultLength = 4f,
                    resizable = true,
                },
                new GameAction("ding", "Ding!")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        if (GameManager.instance.currentGame == "mrUpbeat") {
                            MrUpbeat.instance.Ding(e.beat, e["toggle"], e["stopBlipping"], e["playDing"]);
                        } else {
                            MrUpbeat.DingSfx(e.beat, e["toggle"], e["playDing"]);
                        }
                    },
                    defaultLength = 0.5f,
                    parameters = new List<Param>()
                    {
                        new Param("toggle", false, "Applause", "Toggle if applause should be played."),
                        new Param("stopBlipping", true, "Stop Blipping", "Toggle if the blipping should stop too."),
                        new Param("playDing", true, "Play Ding", "Toggle if this block should play a ding?"),
                    },
                    preFunctionLength = 0.5f,
                },
                new GameAction("changeBG", "Background Appearance")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        MrUpbeat.instance.BackgroundColor(e.beat, e.length, e["start"], e["end"], e["ease"]);
                    },
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("start", new Color(0.878f, 0.878f, 0.878f), "Start Color", "Set the color at the start of the event."),
                        new Param("end", new Color(0.878f, 0.878f, 0.878f), "End Color", "Set the color at the start of the event."),
                        new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the background.")
                    }
                },
                new GameAction("upbeatColors", "Upbeat Colors")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        MrUpbeat.instance.UpbeatColors(e["blipColor"], e["setShadow"], e["shadowColor"]);
                    },
                    defaultLength = 0.5f,
                    parameters = new List<Param>()
                    {
                        new Param("blipColor", new Color(0, 1f, 0), "Blip Color", "Set the blip color."),
                        new Param("setShadow", false, "Custom Shadow Color", "Toggle if Mr. Upbeat's shadow should be custom."),
                        new Param("shadowColor", new Color(1f, 1f, 1f, 0), "Shadow Color", "Set the shadow color."),
                    }
                },
                new GameAction("blipEvents", "Blip Events")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        MrUpbeat.instance.BlipEvents(e["letter"], e["shouldGrow"], e["resetBlip"], e["shouldBlip"], e["blipLength"]);
                    },
                    defaultLength = 0.5f,
                    parameters = new List<Param>()
                    {
                        new Param("letter", "", "Letter To Appear", "Set the letter to appear on the blip."),
                        new Param("shouldGrow", true, "Grow Antenna", "Toggle if Mr. Upbeat's antenna should grow on every blip"),
                        new Param("resetBlip", false, "Reset Antenna", "Toggle if Mr. Upbeat's antenna should reset"),
                        new Param("shouldBlip", true, "Should Blip", "Toggle if Mr. Upbeat's antenna should blip every offbeat."),
                        new Param("blipLength", new EntityTypes.Integer(0, 4, 4), "Text Blip Requirement", "Set how many blips it will take for the text to appear on Mr. Upbeat’s antenna."),
                    }
                },
                new GameAction("fourBeatCountInOffbeat", "4 Beat Count-In")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        MrUpbeat.CountIn(e.beat, e.length, e["a"]);
                    },
                    parameters = new List<Param>()
                    {
                        new Param("a", true, "A", "Toggle if numbers should be preceded with \"A-\"."),
                    },
                    defaultLength = 4f,
                    resizable = true,
                },
                new GameAction("countOffbeat", "Count")
                {
                    inactiveFunction = delegate { MrUpbeat.Count(eventCaller.currentEntity["number"]); },
                    parameters = new List<Param>()
                    {
                        new Param("number", MrUpbeat.Counts.One, "Type", "Set the number to say."),
                    },
                },
                new GameAction("forceStepping", "Force Stepping")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        MrUpbeat.instance.ForceStepping(e.beat, e.length);
                    },
                    defaultLength = 4f,
                    resizable = true,
                },
            },
            new List<string>() {"agb", "keep"},
            "agboffbeat", "en",
            new List<string>() {},
            chronologicalSortKey: 101
            );
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_MrUpbeat;
    using Jukebox;
    public class MrUpbeat : Minigame
    {
        public enum Counts
        {
            One,
            Two,
            Three,
            Four,
            A,
        }

        [Header("References")]
        [SerializeField] Animator metronomeAnim;
        [SerializeField] UpbeatMan man;
        [SerializeField] Material blipMaterial;
        [SerializeField] SpriteRenderer bg;
        [SerializeField] SpriteRenderer[] shadowSr;

        [Header("Properties")]
        public int stepIterate = 0;
        private static double startSteppingBeat = double.MaxValue;
        private static double startBlippingBeat = double.MaxValue;
        private string currentMetronomeDir = "Right";
        private static double metronomeBeat = double.MaxValue;
        private bool stopStepping;
        public bool stopBlipping;

        private ColorEase bgColorEase = new(new Color(0.878f, 0.878f, 0.878f));

        public static MrUpbeat instance;

        private void Awake()
        {
            instance = this;
        }

        void OnDestroy()
        {
            startSteppingBeat = double.MaxValue;
            startBlippingBeat = double.MaxValue;
            stepIterate = 0;
            foreach (var evt in scheduledInputs)
            {
                evt.Disable();
            }
        }

        public override void OnGameSwitch(double beat)
        {
            List<RiqEntity> prevEntities = GameManager.instance.Beatmap.Entities.FindAll(c => c.beat <= beat && c.datamodel.Split(0) == "mrUpbeat");

            if (beat >= startBlippingBeat) {
                double tempBeat = ((beat % 1 == 0.5) ? Mathf.Floor((float)beat) : Mathf.Round((float)beat)) + (startBlippingBeat % 1);
                BeatAction.New(instance, new List<BeatAction.Action>() {
                    new BeatAction.Action(tempBeat, delegate { man.RecursiveBlipping(tempBeat); })
                });
                startBlippingBeat = double.MaxValue;
            }

            // init background color/blip color stuff by getting the last of each of those blocks
            var bgColorEntity = prevEntities.FindLast(x => x.datamodel.Split(1) == "changeBG" && x.beat <= beat);
            var upbeatColorEntity = prevEntities.FindLast(x => x.datamodel.Split(1) == "upbeatColors" && x.beat <= beat);

            if (bgColorEntity != null) {
                bg.color = bgColorEntity["end"];
            }
            
            if (upbeatColorEntity != null) {
                blipMaterial.SetColor("_ColorBravo", upbeatColorEntity["blipColor"]);
                Color shadowColor = upbeatColorEntity["shadowColor"];
                if (upbeatColorEntity["setShadow"]) foreach (var shadow in shadowSr) {
                    shadow.color = new Color(shadowColor.r, shadowColor.g, shadowColor.b, 1);
                }
            } else {
                blipMaterial.SetColor("_ColorBravo", new Color(0, 1f, 0));
            }
        }

        public void Update()
        {
            bg.color = bgColorEase.GetColor();
            if (conductor.isPlaying && !conductor.isPaused) {
                var songPos = conductor.songPositionInBeatsAsDouble;

                if (songPos >= startSteppingBeat - 2) {
                    man.canStep = true;
                }

                if (songPos >= startSteppingBeat) {
                    RecursiveStepping(startSteppingBeat);
                    startSteppingBeat = double.MaxValue;
                }

                if (songPos >= startBlippingBeat) {
                    man.RecursiveBlipping(startBlippingBeat);
                    startBlippingBeat = double.MaxValue;
                }

                if (songPos > metronomeBeat + 1)
                {
                    metronomeAnim.Play("MetronomeGo" + currentMetronomeDir, -1, 1);
                    metronomeAnim.speed = 0;
                }
                else if (songPos >= metronomeBeat)
                {
                    metronomeAnim.DoScaledAnimation("MetronomeGo" + currentMetronomeDir, metronomeBeat, 1, ignoreSwing: false);
                }
            }
        }

        public void Ding(double beat, bool applause, bool stopBlipping, bool playDing)
        {
            BeatAction.New(instance, new List<BeatAction.Action>() {
                new BeatAction.Action(beat - 0.5, delegate {
                    stopStepping = true;
                    if (stopBlipping) this.stopBlipping = true;
                }),
                new BeatAction.Action(beat, delegate {
                    man.canStep = false;
                }),
                new BeatAction.Action(beat + 0.5, delegate {
                    stopStepping = false;
                }),
            });
            DingSfx(beat, applause, playDing);
        }

        public static void DingSfx(double beat, bool applause, bool playDing)
        {
            if (playDing) SoundByte.PlayOneShotGame("mrUpbeat/ding", beat: beat, forcePlay: true);
            if (applause) SoundByte.PlayOneShot("applause", beat: beat);
        }

        public static void PrePrepare(double beat, float length, bool mrDownbeat)
        {
            bool isGame = GameManager.instance.currentGame == "mrUpbeat";
            if (!mrDownbeat) {
                beat = Mathf.Floor((float)beat) + 0.5;
                length = Mathf.Round(length);
            }
            startBlippingBeat = beat;
            startSteppingBeat = beat + length - 0.5f;
            if (!isGame) Blipping(beat, length);
        }

        private void ScheduleStep(double beat)
        {
            PlayerActionEvent input = ScheduleInput(beat, 0.5f, InputAction_BasicPress, Success, Miss, Nothing);
            // input.IsHittable = () => man.canStep && man.canStepFromAnim && man.FacingCorrectly();
        }

        private void RecursiveStepping(double beat)
        {
            if (stopStepping) {
                stopStepping = false;
                return;
            }
            currentMetronomeDir = (stepIterate % 2 == 1) ? "Right" : "Left";
            SoundByte.PlayOneShotGame($"mrUpbeat/metronome{currentMetronomeDir}");
            metronomeBeat = beat;
            ScheduleStep(beat);
            BeatAction.New(this, new List<BeatAction.Action>() {
                new(beat + 1, delegate { RecursiveStepping(beat + 1); })
            });
            stepIterate++;
        }

        public void ForceStepping(double beat, float length)
        {
            var actions = new List<BeatAction.Action>();
            for (int i = 0; i < length; i++)
            {
                ScheduleStep(beat + i);
                actions.Add(new BeatAction.Action(beat + i, delegate { 
                    currentMetronomeDir = (stepIterate % 2 == 1) ? "Right" : "Left";
                    SoundByte.PlayOneShotGame($"mrUpbeat/metronome{currentMetronomeDir}");
                    metronomeBeat = beat + i;
                    stepIterate++;
                }));
            }
            BeatAction.New(this, actions);
        }

        public static void Blipping(double beat, float length)
        {
            RiqEntity gameSwitch = GameManager.instance.Beatmap.Entities.Find(c => c.beat > beat && c.datamodel == "gameManager/switchGame/mrUpbeat");
            if (gameSwitch.beat <= beat || gameSwitch.beat >= beat + length + 1) return;

            List<MultiSound.Sound> inactiveBlips = new();
            for (int i = 0; i < gameSwitch.beat - beat; i++) {
                inactiveBlips.Add(new MultiSound.Sound("mrUpbeat/blip", beat + i));
            }

            MultiSound.Play(inactiveBlips.ToArray(), forcePlay: true);
        }

        public void Success(PlayerActionEvent caller, float state)
        {
            man.Step();
            if (state is >= 1f or <= -1f) SoundByte.PlayOneShot("nearMiss");
        }

        public void Miss(PlayerActionEvent caller)
        {
            man.Fall();
        }

        public void BackgroundColor(double beat, float length, Color startColor, Color endColor, int ease)
        {
            bgColorEase = new(beat, length, startColor, endColor, ease);
        }

        public void UpbeatColors(Color blipColor, bool setShadow, Color shadowColor)
        {
            blipMaterial.SetColor("_ColorBravo", blipColor);

            if (setShadow) foreach (var shadow in shadowSr) {
                shadow.color = new Color(shadowColor.r, shadowColor.g, shadowColor.b, 1);
            }
        }

        public void BlipEvents(string inputLetter, bool shouldGrow, bool resetBlip, bool shouldBlip, int blipLength)
        {
            if (resetBlip) man.blipSize = 0;
            man.shouldGrow = shouldGrow;
            man.blipString = inputLetter;
            man.shouldBlip = shouldBlip;
            man.blipLength = blipLength;
        }

        public static void Count(int number)
        {
            SoundByte.PlayOneShotGame("mrUpbeat/" + (number < 4 ? number + 1 : "a"), forcePlay: true);
        }

        public static void CountIn(double beat, float length, bool a)
        {
            var sound = new List<MultiSound.Sound>();
            if (a) sound.Add(new MultiSound.Sound("mrUpbeat/a", beat - (0.5f * (length/4))));
            for (int i = 0; i < 4; i++) {
                sound.Add(new MultiSound.Sound("mrUpbeat/" + (i + 1), beat + (i * (length / 4)), offset: (i == 3) ? 0.05 : 0));
            }
            
            MultiSound.Play(sound.ToArray(), forcePlay: true);
        }

        public void Nothing(PlayerActionEvent caller) { }
    }
}