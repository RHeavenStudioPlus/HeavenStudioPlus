using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;

using Jukebox;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class RvlBasketLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("basketballGirls", "Basketball Girls", "ffffff", false, false, new List<GameAction>()
            {
                new("bop", "Bop")
                {
                    function = delegate { var e = eventCaller.currentEntity; BasketballGirls.instance.ToggleBop(e.beat, e.length, e["toggle"], e["auto"]);},
                    resizable = true,
                    parameters = new()
                    {
                        new("toggle", true, "Bop"),
                        new("auto", false, "Bop (Auto)")
                    }
                },
                new GameAction("ball", "Ball")
                {
                    function = delegate {var e = eventCaller.currentEntity; BasketballGirls.instance.SpawnBall(e.beat); },
                    defaultLength = 2f,
                },
                new GameAction("zoom", "Zoom In/Out")
                {
                    function = delegate { var e = eventCaller.currentEntity; BasketballGirls.instance.ToggleZoom(e.beat, e.length, e["ease"], e["toggle"]);},
                    defaultLength = 4f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("toggle", true, "Zoom In", "Toggle if the camera should zoom in."),
                        new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action."),
                    }
                },
                new GameAction("background appearance", "Background Appearance")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        BasketballGirls.instance.BackgroundColorSet(e.beat, e.length, e["colorBGStart"], e["colorBGEnd"], e["ease"]);
                    },
                    defaultLength = 0.5f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("colorBGStart", new Color(1f, 0.937f, 0.224f), "Start BG Color", "Set the color at the start of the event."),
                        new Param("colorBGEnd", new Color(1f, 0.937f, 0.224f), "End BG Color", "Set the color at the end of the event."),
                        new Param("ease", Util.EasingFunction.Ease.Instant, "Ease", "Set the easing of the action."),
                    }
                },
            },
            new List<string>() { "rvl", "normal" },
            "rvlbasket", "en",
            new List<string>() {},
            chronologicalSortKey: 105
            );
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_BasketballGirls;
    public class BasketballGirls : Minigame
    {
        public Transform baseBall;
        public Animator girlLeftAnim, girlRightAnim;
        [NonSerialized] public List<Interval> girlLeftNoBopIntervals = new(),
                                              girlRightNoBopIntervals = new();
        public Animator goalAnim;

        public Transform[] CameraPosition;
        double cameraMoveBeat = double.MaxValue;
        double cameraMoveLength;
        Util.EasingFunction.Ease cameraMoveEase;
        bool cameraMoveIn;

        [SerializeField] private SpriteRenderer BGPlane;

        private ColorEase bgColorEase = new(new Color(1f, 0.937f, 0.224f));

        public static BasketballGirls instance;

        const int IAAltDownCat = IAMAXCAT;
        
        protected static bool IA_BatonAltPress(out double dt)
        {
            return PlayerInput.GetSqueezeDown(out dt);
        }

        public static PlayerInput.InputAction InputAction_Catch =
            new("RvlBasketCatch", new int[] { IAPressCat, IAPressCat, IAAltDownCat },
            IA_PadBasicPress, IA_TouchBasicPress, IA_BatonAltPress);

        private void Awake()
        {
            instance = this;
            SetupBopRegion("basketballGirls", "bop", "auto");
            HandleBops();
        }

        public override void OnLateBeatPulse(double beat)
        {
            if (BeatIsInBopRegion(beat)) { Bop(beat);}
        }

        private void Update()
        {
            var cond = Conductor.instance;
            if (cond.isPlaying && !cond.isPaused)
            {
                if (PlayerInput.GetIsAction(InputAction_Catch) && !IsExpectingInputNow(InputAction_Catch))
                {
                    if (!girlRightAnim.IsPlayingAnimationNames("blank") && !girlRightNoBopIntervals.Any(x => x.Contains(cond.songPositionInBeatsAsDouble)))
                    {
                        SoundByte.PlayOneShotGame("basketballGirls/A");
                        girlRightAnim.DoScaledAnimationAsync("blank", 0.5f);
                    }
                }
                
                UpdateCamera(cond.songPositionInBeatsAsDouble);
                UpdateBackgroundColor();
            }
        }

        private void HandleBops()
        {
            List<RiqEntity> events = EventCaller.GetAllInGameManagerList("basketballGirls", new string[] { "ball" });

            foreach (var e in events)
            {
                girlLeftNoBopIntervals.Add(new Interval(e.beat, e.beat + 2));
            }
        }
        public void ToggleBop(double beat, float length, bool bopOrNah, bool autoBop)
        {
            if (bopOrNah)
            {
                for (int i = 0; i < length; i++)
                {
                    var currentBeat = beat + i;
                    BeatAction.New(instance, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(currentBeat, delegate { Bop(currentBeat);})
                    });
                }
            }
        }
        private void Bop(double beat)
        {
            if (!girlLeftNoBopIntervals.Any(x => x.Contains(beat)))
            {
                MultiSound.Play(
                    new MultiSound.Sound[] {
                        new MultiSound.Sound("basketballGirls/dribble" + UnityEngine.Random.Range(1,3).ToString(), beat),
                        new MultiSound.Sound("basketballGirls/dribbleEcho" + UnityEngine.Random.Range(1,4).ToString(), beat + 0.5),
                    }
                );
                girlLeftAnim.DoScaledAnimationAsync("dribble", 0.5f);
            }
            if (!girlRightNoBopIntervals.Any(x => x.Contains(beat)))
            {
                girlRightAnim.DoScaledAnimationAsync("bop", 0.5f);
            }
        }

        public void SpawnBall(double beat)
        {
            var newBall = Instantiate(baseBall, transform).GetComponent<Ball>();
            newBall.startBeat = beat;

            SoundByte.PlayOneShotGame("basketballGirls/voice");
            girlLeftAnim.DoScaledAnimationAsync("prepare", 0.5f);
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate {
                    newBall.gameObject.SetActive(true);
                    newBall.Init();
                }),
                new BeatAction.Action(beat+2, delegate {
                }),
            });
        }

        public void ToggleZoom(double beat, double length, int ease, bool toggle)
        {
            cameraMoveBeat = beat;
            cameraMoveLength = length;
            cameraMoveEase = (Util.EasingFunction.Ease)ease;
            cameraMoveIn = toggle;
        }
        private void UpdateCamera(double beat)
        {
            Vector3 cameraPosition;

            if (beat >= cameraMoveBeat)
            {
                Util.EasingFunction.Function func = Util.EasingFunction.GetEasingFunction(cameraMoveEase);
                float prog = conductor.GetPositionFromBeat(cameraMoveBeat, cameraMoveLength, true);
                prog = Mathf.Clamp01(prog);
                if (cameraMoveIn)
                {
                    cameraPosition.x = func(CameraPosition[0].position.x, CameraPosition[1].position.x, prog);
                    cameraPosition.y = func(CameraPosition[0].position.y, CameraPosition[1].position.y, prog);
                    cameraPosition.z = func(CameraPosition[0].position.z, CameraPosition[1].position.z, prog);
                }
                else
                {
                    cameraPosition.x = func(CameraPosition[1].position.x, CameraPosition[0].position.x, prog);
                    cameraPosition.y = func(CameraPosition[1].position.y, CameraPosition[0].position.y, prog);
                    cameraPosition.z = func(CameraPosition[1].position.z, CameraPosition[0].position.z, prog);
                }
            }
            else
            {
                if (cameraMoveIn) cameraPosition = CameraPosition[1].position;
                else cameraPosition = CameraPosition[0].position;
            }

            GameCamera.AdditionalPosition = cameraPosition - GameCamera.defaultPosition;
        }

        public void BackgroundColorSet(double beat, float length, Color BGStart, Color BGEnd, int colorEaseSet)
        {
            bgColorEase = new(beat, length, BGStart, BGEnd, colorEaseSet);

            UpdateBackgroundColor();
        }
        private void UpdateBackgroundColor()
        {
            BGPlane.color = bgColorEase.GetColor();
        }

        //call this in OnPlay(double beat) and OnGameSwitch(double beat)
        private void PersistColor(double beat)
        {
            var allEventsBeforeBeat = EventCaller.GetAllInGameManagerList("basketballGirls", new string[] { "background appearance" }).FindAll(x => x.beat < beat);
            if (allEventsBeforeBeat.Count > 0)
            {
                allEventsBeforeBeat.Sort((x, y) => x.beat.CompareTo(y.beat)); //just in case
                var lastEvent = allEventsBeforeBeat[^1];
                BackgroundColorSet(lastEvent.beat, lastEvent.length, lastEvent["colorBGStart"], lastEvent["colorBGEnd"], lastEvent["ease"]);
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
    }
}


// How should Bop's on/off be managed?
namespace HeavenStudio.Games.Scripts_BasketballGirls
{
    public class Interval
    {
        private readonly double _start;
        private readonly double _end;
        private readonly Func<double, double, bool> _leftComparer;
        private readonly Func<double, double, bool> _rightComparer;

        public double Start => _start;
        public double End => _end;

        public Interval(double start, double end, bool isLeftClosed = true, bool isRightClosed = false)
        {
            _start = start;
            _end = end;

            _leftComparer = isLeftClosed ? (value, boundary) => value >= boundary : (value, boundary) => value > boundary;
            _rightComparer = isRightClosed ? (value, boundary) => value <= boundary : (value, boundary) => value < boundary;
        }

        public bool Contains(double value) => _leftComparer(value, _start) && _rightComparer(value, _end);
    }
}
