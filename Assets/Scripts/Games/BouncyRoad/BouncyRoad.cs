using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NaughtyBezierCurves;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;

    public static class AgbBouncyRoadLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("bouncyRoad", "Bouncy Road", "0296FF", false, false, new List<GameAction>()
            {
                new GameAction("ball", "Ball")
                {
                    defaultLength = 1f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("goal", true, "Play Goal Sound"),
                        new Param("color", Color.white, "Color", "Choose the color of the ball."),
                    }
                },
                new GameAction("background appearance", "Background Appearance")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        BouncyRoad.instance.BackgroundColorSet(e.beat, e.length, e["colorBG1Start"], e["colorBG1End"], e["colorBG2Start"], e["colorBG2End"], e["ease"]);
                    },
                    defaultLength = 0.5f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("colorBG1Start", new Color(0.004f, 0.596f, 0.996f), "Start BG Color", "Set top-most color of the background gradient at the start of the event."),
                        new Param("colorBG1End", new Color(0.004f, 0.596f, 0.996f), "End BG Color", "Set top-most color of the background gradient at the end of the event."),
                        new Param("colorBG2Start", Color.black, "Start BG Color", "Set bottom-most color of the background gradient at the start of the event."),
                        new Param("colorBG2End",Color.black, "End BG Color", "Set bottom-most color of the background gradient at the end of the event."),
                        new Param("ease", Util.EasingFunction.Ease.Instant, "Ease", "Set the easing of the action."),
                    }
                },
                // new GameAction("object appearance", "Object Appearance")
                // {
                //     function = delegate {
                //         var e = eventCaller.currentEntity;
                //         BouncyRoad.instance.ObjectColorSet(e["color1"], e["color2"], e["color3"]);
                //     },
                //     defaultLength = 0.5f,
                //     parameters = new List<Param>()
                //     {
                //         new Param("color1", new Color(1, 1, 1), "Color 1"),
                //         new Param("color2", new Color(1, 1, 1), "Color 2"),
                //         new Param("color3", new Color(1, 1, 1), "Color 3"),
                //     }
                // },
            },
            new List<string>() { "agb", "normal" },
            "agbbouncy", "en",
            new List<string>() { },
            chronologicalSortKey: 24
            );
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_BouncyRoad;
    public class BouncyRoad : Minigame
    {
        [SerializeField] GameObject baseBall;
        [SerializeField] GameObject baseBounceCurve;
        [SerializeField] Transform CurveHolder;

        [SerializeField] Transform ThingsTrans;
        [System.NonSerialized] public Animator[] ThingsAnim;
        [System.NonSerialized] public Dictionary<float, BezierCurve3D[]> CurveCache;
        [SerializeField] BezierCurve3D PosCurve;

        [SerializeField] float fallY;

        [SerializeField] private SpriteRenderer BGGradient, BGHigh, BGLow;
        private ColorEase[] colorEases = new ColorEase[2];

        const double BALL_SEEK_TIME = 1.0;
        private struct ScheduledBall
        {
            public double beat;
            public double length;
            public bool goal;
            public Color color;
        }
        List<ScheduledBall> scheduledBalls = new();
        int ballIndex;

        public static BouncyRoad instance;

        const int IALeft = 0;
        const int IARight = 1;

        protected static bool IA_PadLeft(out double dt)
        {
            return PlayerInput.GetPadDown(InputController.ActionsPad.Up, out dt)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Down, out dt)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Left, out dt)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Right, out dt);
        }
        protected static bool IA_TouchLeft(out double dt)
        {
            return PlayerInput.GetTouchDown(InputController.ActionsTouch.Left, out dt);
        }

        protected static bool IA_PadRight(out double dt)
        {
            return PlayerInput.GetPadDown(InputController.ActionsPad.East, out dt);
        }
        protected static bool IA_TouchRight(out double dt)
        {
            return PlayerInput.GetTouchDown(InputController.ActionsTouch.Right, out dt);
        }

        public static PlayerInput.InputAction InputAction_Left =
            new("AgbBouncyLeft", new int[] { IALeft, IALeft, IALeft },
            IA_PadLeft, IA_TouchLeft, IA_BatonBasicPress);

        public static PlayerInput.InputAction InputAction_Right =
            new("AgbBouncyRight", new int[] { IARight, IARight, IAEmptyCat },
            IA_PadRight, IA_TouchRight, IA_Empty);

        void Awake()
        {
            instance = this;
            
            colorEases = new ColorEase[] {
                new(new Color(0.004f, 0.596f, 0.996f)),
                new(Color.black),
            };

            ThingsAnim = new Animator[ThingsTrans.childCount];
            int childIndex = 0;
            foreach (Transform child in ThingsTrans)
            {
                // var prog = (float)childIndex/(ThingsTrans.childCount-1);
                // child.transform.localPosition = PosCurve.GetPoint(prog);
                ThingsAnim[childIndex++] = child.GetComponent<Animator>();
            }

            var newCurves = new BezierCurve3D[ThingsTrans.childCount + 3];
            // for (var i = 0; i < ThingsAnim.Length + 1; ++i)
            // {
            //     var prog1 = (float)(i-1)/(ThingsTrans.childCount-1);
            //     var prog2 = (float)(i)/(ThingsTrans.childCount-1);
            //     var pos1 = PosCurve.GetPoint(prog1);
            //     var pos2 = PosCurve.GetPoint(prog2);

            //     var newCurve = GenerateInitCurve(pos1, pos2);

            //     newCurves[i] = newCurve.GetComponent<BezierCurve3D>();
            // }
            {
                Vector3 pos1, pos2;
                pos1 = PosCurve.GetPoint((float)(-1)/(ThingsAnim.Length-1));
                pos2 = PosCurve.GetPoint(0);
                newCurves[0] = GenerateInitCurve(pos1, pos2).GetComponent<BezierCurve3D>();

                for (var i = 0; i < ThingsAnim.Length-1; ++i)
                {
                    pos1 = ThingsTrans.GetChild(i).transform.localPosition;
                    pos2 = ThingsTrans.GetChild(i+1).transform.localPosition;

                    var newCurve = GenerateInitCurve(pos1, pos2);

                    newCurves[1+i] = newCurve.GetComponent<BezierCurve3D>();
                }

                pos1 = PosCurve.GetPoint(1);
                pos2 = PosCurve.GetPoint((float)(ThingsAnim.Length)/(ThingsAnim.Length-1));
                newCurves[ThingsAnim.Length] = GenerateInitCurve(pos1, pos2).GetComponent<BezierCurve3D>();
            }
            newCurves[^2] = GenerateMissCurve(13).GetComponent<BezierCurve3D>();
            newCurves[^1] = GenerateMissCurve(14).GetComponent<BezierCurve3D>();

            CurveCache = new Dictionary<float, BezierCurve3D[]>();
            CurveCache.Add(1, newCurves);
        }

        public override void OnGameSwitch(double beat)
        {

            PersistColor(beat);

            double gameStartBeat = beat, gameEndBeat = double.MaxValue;
            var firstEnd = EventCaller.GetAllInGameManagerList("gameManager", new string[] { "switchGame", "end" }).Find(x => x.beat > gameStartBeat);
            gameEndBeat = firstEnd?.beat ?? gameEndBeat;


            scheduledBalls.Clear();
            ballIndex = 0;
            var events = EventCaller.GetAllInGameManagerList("bouncyRoad", new string[] { "ball" }).FindAll(x => x.beat >= gameStartBeat && x.beat < gameEndBeat);
            foreach (var e in events)
            {
                if (e.length == 0) continue;
                var ball = new ScheduledBall
                {
                    beat = e.beat,
                    length = e.length,
                    goal = e["goal"],
                    color = e["color"],
                };
                scheduledBalls.Add(ball);
            }
            scheduledBalls.Sort((x, y) => (x.beat - x.length).CompareTo(y.beat - y.length));
        }
        public override void OnPlay(double beat)
        {
            OnGameSwitch(beat);
        }

        void Update()
        {
            var cond = Conductor.instance;
            if (!cond.isPlaying || cond.isPaused) return;

            if (PlayerInput.GetIsAction(InputAction_Right) && !IsExpectingInputNow(InputAction_Right))
            {
                ThingsAnim[12].Play("podium", 0, 0);
            }
            if (PlayerInput.GetIsAction(InputAction_Left) && !IsExpectingInputNow(InputAction_Left))
            {
                ThingsAnim[13].Play("podium", 0, 0);
            }

            UpdateBalls();
            UpdateBackgroundColor();
        }

        void UpdateBalls()
        {
            double beat = conductor.songPositionInBeatsAsDouble;
            while(ballIndex < scheduledBalls.Count)
            {
                var ball = scheduledBalls[ballIndex];
                if (ball.beat - ball.length < beat + BALL_SEEK_TIME)
                {
                    SpawnBall(ball.beat, ball.length, ball.goal, ball.color);
                    ballIndex++;
                }
                else
                {
                    break;
                }
            }
        }

        public void SpawnBall(double beat, double length, bool goal, Color color)
        {
            var newBall = Instantiate(baseBall, transform).GetComponent<Ball>();

            newBall.startBeat = beat;
            newBall.lengthBeat = length;
            newBall.goal = goal;
            newBall.color = color;

            newBall.curve = GetHeightCurve((float)length);
            
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat - length, delegate
                {
                    newBall.Init();
                    newBall.gameObject.SetActive(true);
                })
            });
        }

        List<double> bounceBeats = new();
        public void PlayBounceSound(double beat, double length)
        {
            var sounds = new List<MultiSound.Sound>();
            for (int i = 0; i < 12 ; i++)
            {
                var bounceBeat = beat + i * length;
                if (!bounceBeats.Contains(bounceBeat)) sounds.Add(new MultiSound.Sound("bouncyRoad/ballBounce", bounceBeat));
                bounceBeats.Add(bounceBeat);
            }
            MultiSound.Play(sounds.ToArray());
        }

        private Transform GenerateInitCurve(Vector3 pos1, Vector3 pos2)
        {
            float dist = Vector3.Distance(pos1, pos2);
            float angle = Mathf.Atan2(pos1.z - pos2.z, pos1.x - pos2.x) * Mathf.Rad2Deg;

            var newCurve = Instantiate(baseBounceCurve, CurveHolder).transform;

            var point0 = newCurve.GetChild(0);
            var point1 = newCurve.GetChild(1);

            point0.transform.localPosition = pos1;
            point0.transform.localEulerAngles = new Vector3(0, -angle, 0);
            point0.transform.localScale = new Vector3(dist, 1, 1);
            point1.transform.localPosition = pos2;
            point1.transform.localEulerAngles = new Vector3(0, -angle, 0);
            point1.transform.localScale = new Vector3(dist, 1, 1);

            return newCurve;
        }

        private Transform GenerateMissCurve(int number)
        {
            var curve = CurveHolder.GetChild(number);

            var newCurve = Instantiate(curve, CurveHolder).transform;

            var point0 = newCurve.GetChild(0);
            var point1 = newCurve.GetChild(1);

            Vector3 pos1 = point1.transform.localPosition;
            point1.transform.localPosition = pos1 + new Vector3(0, fallY, 0);

            return newCurve;
        }

        private BezierCurve3D[] GetHeightCurve(float length)
        {
            BezierCurve3D[] newCurves;
            CurveCache.TryGetValue(length, out newCurves);

            if (newCurves is null)
            {
                var newCurveHolder = Instantiate(CurveHolder, transform);
                newCurveHolder.name = $"CurveHolder_{length}";
                var newCurvesTrans = newCurveHolder.transform;

                newCurves = new BezierCurve3D[newCurvesTrans.childCount];
                int childIndex = 0;
                foreach (Transform child in newCurvesTrans)
                {
                    var point0 = child.GetChild(0);
                    var point1 = child.GetChild(1);

                    Vector3 scale = point0.transform.localScale;
                    point0.transform.localScale = new Vector3(scale.x, length * scale.y, scale.z);
                    point1.transform.localScale = new Vector3(scale.x, length * scale.y, scale.z);
                    newCurves[childIndex++] = child.GetComponent<BezierCurve3D>();
                }
                CurveCache.Add(length, newCurves);
            }

            return newCurves;
        }

        public void BackgroundColorSet(double beat, float length, Color BG1Start, Color BG1End, Color BG2Start, Color BG2End, int colorEaseSet)
        {
            colorEases = new ColorEase[] {
                new(beat, length, BG1Start, BG1End, colorEaseSet),
                new(beat, length, BG2Start, BG2End, colorEaseSet),
            };

            UpdateBackgroundColor();
        }
        public void ObjectColorSet(Color Color1, Color Color2, Color Color3)
        {
            
        }
        private void UpdateBackgroundColor()
        {
            BGGradient.material.SetColor("_ColorAlpha", colorEases[0].GetColor());
            BGGradient.material.SetColor("_ColorDelta", colorEases[1].GetColor());
            BGHigh.color = colorEases[0].GetColor();
            BGLow.color = colorEases[1].GetColor();
        }

        private void PersistColor(double beat)
        {
            var allEventsBeforeBeat = EventCaller.GetAllInGameManagerList("bouncyRoad", new string[] { "background appearance" }).FindAll(x => x.beat < beat);
            if (allEventsBeforeBeat.Count > 0)
            {
                allEventsBeforeBeat.Sort((x, y) => x.beat.CompareTo(y.beat)); //just in case
                var lastEvent = allEventsBeforeBeat[^1];
                BackgroundColorSet(lastEvent.beat, lastEvent.length, lastEvent["colorBG1Start"], lastEvent["colorBG1End"], lastEvent["colorBG2Start"], lastEvent["colorBG2End"], lastEvent["ease"]);
            }
        }
    }
}
