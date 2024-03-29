using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyBezierCurves;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;

using Jukebox;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class NtrPingpongLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("rhythmRally", "Rhythm Rally", "ffffff", false, false, new List<GameAction>()
            {
                new GameAction("bop", "Bop")
                {
                    function = delegate {var e = eventCaller.currentEntity; RhythmRally.instance.Bop(e.beat, e.length, e["bop"], e["bopAuto"]); },
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("bop", true, "Bop", "Toggle if the paddlers should bop for the duration of this event."),
                        new Param("bopAuto", false, "Bop (Auto)", "Toggle if the paddlers should automatically bop until another Bop event is reached.")
                    }
                },
                new GameAction("whistle", "Whistle")
                {
                    preFunction = delegate { RhythmRally.PlayWhistle(eventCaller.currentEntity.beat); },
                    defaultLength = 0.5f
                },
                new GameAction("toss ball", "Toss Ball")
                {
                    function = delegate { RhythmRally.instance.Toss(eventCaller.currentEntity.beat, eventCaller.currentEntity.length, 6f, true); },
                    defaultLength = 2f,
                    resizable = true
                },
                new GameAction("rally", "Rally")
                {
                    function = delegate { RhythmRally.instance.Serve(eventCaller.currentEntity.beat, RhythmRally.RallySpeed.Normal); },
                    defaultLength = 4f,
                    resizable = true
                },
                new GameAction("slow rally", "Slow Rally")
                {
                    function = delegate { RhythmRally.instance.Serve(eventCaller.currentEntity.beat, RhythmRally.RallySpeed.Slow); },
                    defaultLength = 8f,
                    resizable = true
                },
                new GameAction("fast rally", "Fast Rally")
                {
                    function = delegate { RhythmRally.instance.PrepareFastRally(eventCaller.currentEntity.beat, RhythmRally.RallySpeed.Fast, eventCaller.currentEntity["muteAudio"]); },
                    defaultLength = 6f,
                    parameters = new List<Param>()
                    {
                        new Param("muteAudio", false, "Mute Cowbell", "Toggle if the cowbell \"tonk-tink-tonk\" sound should be muted.")
                    }
                },
                new GameAction("superfast rally", "Superfast Rally")
                {
                    function = delegate { RhythmRally.instance.PrepareFastRally(eventCaller.currentEntity.beat, RhythmRally.RallySpeed.SuperFast, eventCaller.currentEntity["muteAudio"]); },
                    defaultLength = 12f,
                    parameters = new List<Param>()
                    {
                        new Param("muteAudio", false, "Mute Cowbell", "Toggle if the cowbell \"tonk-tink-tonk\" sound should be muted.")
                    }
                },
                new GameAction("tonktinktonk", "Tonk-Tink-Tonk (Stretchable)")
                {
                    preFunction = delegate {var e = eventCaller.currentEntity; RhythmRally.TonkTinkTonkStretchable(e.beat, e.length); },
                    defaultLength = 4f,
                    resizable = true
                },
                new GameAction("superfast stretchable", "Superfast Rally (Stretchable)")
                {
                    function = delegate { var e = eventCaller.currentEntity; RhythmRally.instance.SuperFastRallyStretchable(e.beat, e.length); },
                    defaultLength = 8f,
                    resizable = true
                },
                new GameAction("pose", "End Pose")
                {
                    function = delegate { RhythmRally.instance.Pose(); },
                    defaultLength = 0.5f
                },
                new GameAction("camera", "Camera Controls")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        // var rotation = new Vector3(0, e["valA"], 0);
                        RhythmRally.instance.ChangeCameraAngle(e.beat, e["valA"], e["valB"], e.length, (Util.EasingFunction.Ease)e["type"], e["additive"]);
                    },
                    defaultLength = 4,
                    resizable = true,
                    parameters = new List<Param>() {
                        new Param("valA", new EntityTypes.Integer(-360, 360, 0), "Rotation", "Set the rotation of the camera around the center of the table."),
                        new Param("valB", new EntityTypes.Float(0.5f, 4f, 1), "Zoom", "Set the camera's level of zoom."),
                        new Param("type", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action."),
                        new Param("additive", true, "Additive Rotation", "Toggle if the above rotation should be added to the current angle instead of setting the target angle to travel to.")
                    }
                },
                // todo: background recolouring
                // new GameAction("bg colour", "Background Colour")
                // {
                //     function = delegate {
                //         var e = eventCaller.currentEntity;
                //     },
                //     defaultLength = 1,
                //     resizable = true,
                //     parameters = new List<Param>() {
                //         new Param("bottomColour", new Color(0,0,0,0), "Bottom Colour", "The colour at the bottom of the skybox"),
                //         new Param("topColour", new Color(1,1,1,1), "Top Colour", "The colour at the top of the skybox"),
                //         new Param("type", Util.EasingFunction.Ease.Linear, "Ease", "The easing function to use"),
                //     }
                // },
            },
            new List<string>() { "ntr", "keep" },
            "ntrpingpong", "en",
            new List<string>() { },
            chronologicalSortKey: 6
            );
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_RhythmRally;

    public class RhythmRally : Minigame
    {
        public enum RallySpeed { Slow, Normal, Fast, SuperFast }

        [Header("Camera")]
        [SerializeField] Transform cameraPivot;
        [SerializeField] Transform cameraPos;
        [SerializeField] float cameraFOV;

        [Header("Ball and curve info")]
        public GameObject ball;
        public GameObject ballShadow;
        public ParticleSystem ballTrail;
        public BezierCurve3D serveCurve;
        public BezierCurve3D returnCurve;
        public BezierCurve3D tossCurve;
        public BezierCurve3D missCurve;
        public GameObject ballHitFX;


        [Header("Animators")]
        public Animator playerAnim;
        public Animator opponentAnim;

        [Header("Properties")]
        public RallySpeed rallySpeed = RallySpeed.Normal;
        public bool started;
        public bool missed;
        public bool served;
        public bool tossing;
        public double serveBeat;
        public double targetBeat;
        public double tossBeat;
        public double missBeat;
        public float tossLength;
        private bool inPose;

        public Paddlers paddlers;

        public static RhythmRally instance;

        private void Awake()
        {
            GameCamera.AdditionalPosition = cameraPos.position + (Quaternion.Euler(cameraPos.rotation.eulerAngles) * Vector3.forward * 10f);
            GameCamera.AdditionalRotEuler = cameraPos.rotation.eulerAngles;
            GameCamera.AdditionalFoV = cameraFOV;
            instance = this;
            paddlers.Init();

            playerAnim.Play("Idle", 0, 0);
            opponentAnim.Play("Idle", 0, 0);
            SetupBopRegion("rhythmRally", "bop", "bopAuto");
        }

        private void Start()
        {
            EntityPreCheck(Conductor.instance.songPositionInBeatsAsDouble);
        }

        const float tableHitTime = 0.58f;
        bool opponentServing = false; // Opponent serving this frame?
        double cameraRotateBeat = double.MaxValue;
        double cameraRotateLength;
        Util.EasingFunction.Ease cameraRotateEase;
        float cameraRotateLast = 0, cameraScaleLast = 1;
        float cameraRotateNext = 0, cameraScaleNext = 1;

        void Update()
        {
            var cond = Conductor.instance;
            var currentBeat = cond.songPositionInBeatsAsDouble;

            var hitBeat = serveBeat; // Beat when the last paddler hit the ball
            var beatDur1 = 1f; // From paddle to table
            var beatDur2 = 1f; // From table to other paddle

            var playerState = playerAnim.GetCurrentAnimatorStateInfo(0);
            var opponentState = opponentAnim.GetCurrentAnimatorStateInfo(0);

            if (started)
            {
                // Determine hitBeat and beatDurs.
                switch (rallySpeed)
                {
                    case RallySpeed.Normal:
                        if (!served)
                        {
                            hitBeat = serveBeat + 2f;
                        }
                        break;
                    case RallySpeed.Fast:
                        if (!served)
                        {
                            hitBeat = serveBeat + 1f;
                            beatDur1 = 1f;
                            beatDur2 = 2f;
                        }
                        else
                        {
                            beatDur1 = 0.5f;
                            beatDur2 = 0.5f;
                        }
                        break;
                    case RallySpeed.SuperFast:
                        if (!served)
                        {
                            hitBeat = serveBeat + 1f;
                        }

                        beatDur1 = 0.5f;
                        beatDur2 = 0.5f;
                        break;
                    case RallySpeed.Slow:
                        if (!served)
                        {
                            hitBeat = serveBeat + 4f;
                        }

                        beatDur1 = 2f;
                        beatDur2 = 2f;
                        break;
                }


                // Ball position.
                var curveToUse = served ? serveCurve : returnCurve;
                float curvePosition;

                var hitPosition1 = cond.GetPositionFromBeat(hitBeat, beatDur1);
                if (hitPosition1 >= 1f)
                {
                    var hitPosition2 = cond.GetPositionFromBeat(hitBeat + beatDur1, beatDur2);
                    curvePosition = tableHitTime + hitPosition2 * (1f - tableHitTime);
                }
                else
                {
                    curvePosition = hitPosition1 * tableHitTime;
                }

                if (!missed)
                {
                    float curveHeight = 1.25f;
                    if ((rallySpeed == RallySpeed.Fast && served) || rallySpeed == RallySpeed.SuperFast)
                        curveHeight = 0.75f;
                    else if (rallySpeed == RallySpeed.Fast && !served && hitPosition1 >= 1f)
                        curveHeight = 2f;
                    else if (rallySpeed == RallySpeed.Slow)
                        curveHeight = 3f;

                    curveToUse.transform.localScale = new Vector3(1f, curveHeight, 1f);
                    ball.transform.position = curveToUse.GetPoint(Mathf.Max(0, curvePosition));

                    // Make ball inactive before it passes through the floor.
                    if (curvePosition > 1.05f)
                        ball.SetActive(false);
                }
                else
                {
                    if (tossing)
                    {
                        TossUpdate(tossBeat, tossLength);
                    }
                    else
                    {
                        var missPosition = cond.GetPositionFromBeat(missBeat, 1f);
                        ball.transform.position = missCurve.GetPoint(Mathf.Max(0, missPosition));

                        if (missPosition > 1f)
                            ball.SetActive(false);
                    }
                }

                ballShadow.transform.position = new Vector3(ball.transform.position.x, -0.399f, ball.transform.position.z);

                var timeBeforeNextHit = hitBeat + beatDur1 + beatDur2 - currentBeat;

                // Check if the opponent should swing.
                if (!served && timeBeforeNextHit <= 0f)
                {
                    var rallies = GameManager.instance.Beatmap.Entities.FindAll(c => c.datamodel == "rhythmRally/rally" || c.datamodel == "rhythmRally/slow rally");
                    for (int i = 0; i < rallies.Count; i++)
                    {
                        var rally = rallies[i];
                        if (rally.beat - currentBeat <= 0f && rally.beat + rally.length - currentBeat > 0f)
                        {
                            Serve(hitBeat + beatDur1 + beatDur2, rallySpeed);
                            opponentServing = true;
                            break;
                        }
                    }
                }

                // Check if paddler should do ready animation.
                bool readyToPrep;
                switch (rallySpeed)
                {
                    case RallySpeed.Slow:
                    case RallySpeed.Fast:
                        readyToPrep = timeBeforeNextHit <= 2f;
                        break;
                    case RallySpeed.SuperFast:
                        readyToPrep = timeBeforeNextHit <= 0.5f;
                        break;
                    default:
                        readyToPrep = timeBeforeNextHit <= 1f;
                        break;
                }

                // Paddler ready animation.
                if (readyToPrep && !opponentServing && !inPose)
                {
                    if (served)
                    {
                        if (PlayerInput.CurrentControlStyle != InputController.ControlStyles.Touch || GameManager.instance.autoplay)
                        {
                            if ((playerState.IsName("Swing") && playerAnim.IsAnimationNotPlaying()) || (!playerState.IsName("Swing") && !playerState.IsName("Ready1")))
                                playerAnim.Play("Ready1");
                        }
                    }
                    else if (!opponentServing)
                    {
                        if ((opponentState.IsName("Swing") && opponentAnim.IsAnimationNotPlaying()) || (!opponentState.IsName("Swing") && !opponentState.IsName("Ready1")))
                        {
                            opponentAnim.Play("Ready1");

                            // Toss ball if it fell off the table.
                            if (missed && !tossing)
                            {
                                float tossHeight = 3f;

                                if (rallySpeed == RallySpeed.Slow || rallySpeed == RallySpeed.Fast)
                                    tossHeight = 6f;

                                Toss(hitBeat + beatDur1, beatDur2, tossHeight);
                            }
                        }

                        // If player never swung and is still in ready state, snap them out of it.
                        // only if they're not manually preparing via touch controls
                        if (missed && playerState.IsName("Ready1") && (!paddlers.PlayerDown))
                            playerAnim.Play("Beat");
                    }
                }
            }
            else
            {
                if (tossing)
                {
                    TossUpdate(tossBeat, tossLength);
                }
            }

            opponentServing = false;

            //update camera
            UpdateCamera(currentBeat);
        }

        public override void OnPlay(double beat)
        {
            EntityPreCheck(beat);
        }

        void EntityPreCheck(double beat)
        {
            cameraRotateBeat = double.MaxValue;
            cameraRotateLength = 0;
            cameraRotateEase = Util.EasingFunction.Ease.Linear;
            cameraRotateLast = 0; cameraScaleLast = 1;
            cameraRotateNext = 0; cameraScaleNext = 1;

            List<RiqEntity> prevEntities = GameManager.instance.Beatmap.Entities.FindAll(c => c.beat < beat && c.datamodel.Split(0) == "rhythmRally");
            RiqEntity lastGameSwitch = GameManager.instance.Beatmap.Entities.FindLast(c => c.beat <= beat && c.datamodel == "gameManager/switchGame/rhythmRally");

            if (lastGameSwitch == null) return;
            List<RiqEntity> cameraEntities = prevEntities.FindAll(c => c.beat >= lastGameSwitch.beat && c.datamodel == "rhythmRally/camera");

            foreach (var entity in cameraEntities)
            {
                ChangeCameraAngle(entity.beat, entity["valA"], entity["valB"], entity.length, (Util.EasingFunction.Ease)entity["type"], entity["additive"]);
            }

            UpdateCamera(beat);
        }

        void UpdateCamera(double beat)
        {
            if (beat >= cameraRotateBeat)
            {
                Util.EasingFunction.Function func = Util.EasingFunction.GetEasingFunction(cameraRotateEase);
                float rotProg = Conductor.instance.GetPositionFromBeat(cameraRotateBeat, cameraRotateLength, true);
                rotProg = Mathf.Clamp01(rotProg);
                float rot = func(cameraRotateLast, cameraRotateNext, rotProg);
                cameraPivot.rotation = Quaternion.Euler(0, rot, 0);
                cameraPivot.localScale = Vector3.one * func(cameraScaleLast, cameraScaleNext, rotProg);
            }

            GameCamera.AdditionalPosition = cameraPos.position + (Quaternion.Euler(cameraPos.rotation.eulerAngles) * Vector3.forward * 10f);
            GameCamera.AdditionalRotEuler = cameraPos.rotation.eulerAngles;
            GameCamera.AdditionalFoV = cameraFOV;
        }

        public override void OnBeatPulse(double beat)
        {
            if (BeatIsInBopRegion(beat) && !inPose)
            {
                BopSingle();
            }
        }

        public void Bop(double beat, float length, bool bop, bool bopAuto)
        {
            if (bop)
            {
                for (int i = 0; i < length; i++)
                {
                    BeatAction.New(instance, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(beat + i, delegate
                        {
                            BopSingle();
                        })
                    });
                }
            }
        }

        void BopSingle()
        {
            var playerState = playerAnim.GetCurrentAnimatorStateInfo(0);
            var opponentState = opponentAnim.GetCurrentAnimatorStateInfo(0);

            bool playerPrepping = false; // Player using prep animation?
            bool opponentPrepping = false; // Opponent using prep animation?
            if ((!playerPrepping) && (!paddlers.PlayerDown) && (playerAnim.IsAnimationNotPlaying() || playerState.IsName("Idle") || playerState.IsName("Beat")))
                playerAnim.DoScaledAnimationAsync("Beat", 0.5f);

            if ((!opponentPrepping) && (!opponentServing) && (!tossing) && (opponentAnim.IsAnimationNotPlaying() || opponentState.IsName("Idle") || opponentState.IsName("Beat")))
                opponentAnim.DoScaledAnimationAsync("Beat", 0.5f);
        }

        public void Serve(double beat, RallySpeed speed)
        {
            if (!ball.activeSelf)
                ball.SetActive(true);

            if (!ballTrail.gameObject.activeSelf)
                ballTrail.gameObject.SetActive(true);

            served = true;
            missed = false;
            started = true;
            opponentServing = true;
            tossing = false;

            serveBeat = beat;
            rallySpeed = speed;

            double bounceBeat = 0f;

            switch (rallySpeed)
            {
                case RallySpeed.Normal:
                    targetBeat = 2f;
                    bounceBeat = serveBeat + 1f;
                    break;
                case RallySpeed.Fast:
                case RallySpeed.SuperFast:
                    targetBeat = 1f;
                    bounceBeat = serveBeat + 0.5f;
                    break;
                case RallySpeed.Slow:
                    targetBeat = 4f;
                    bounceBeat = serveBeat + 2f;
                    break;
            }

            opponentAnim.DoScaledAnimationAsync("Swing", 0.5f);
            MultiSound.Play(new MultiSound.Sound[] { new MultiSound.Sound("rhythmRally/Serve", serveBeat), new MultiSound.Sound("rhythmRally/ServeBounce", bounceBeat) });
            paddlers.BounceFX(bounceBeat);

            ScheduleInput(serveBeat, targetBeat, InputAction_FlickPress, paddlers.Just, paddlers.Miss, paddlers.Out);
        }

        public void Toss(double beat, float length, float height, bool firstToss = false)
        {
            // Hide trail while tossing to prevent weirdness while teleporting ball.
            ballTrail.gameObject.SetActive(false);

            if (firstToss)
                height *= length / 2f;

            tossCurve.transform.localScale = new Vector3(1f, height, 1f);
            tossBeat = beat;
            tossLength = length;
            tossing = true;

            if (firstToss)
            {
                opponentAnim.Play("Ready1");
            }

            if (!ball.activeSelf)
                ball.SetActive(true);
        }

        private void TossUpdate(double beat, float duration)
        {
            var tossPosition = Conductor.instance.GetPositionFromBeat(beat, duration);
            ball.transform.position = tossCurve.GetPoint(Mathf.Clamp(tossPosition, 0, 1));

            if (tossPosition > 1.05f)
                ball.SetActive(false);
        }

        public static void PlayWhistle(double beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("rhythmRally/Whistle", beat),
            }, forcePlay: true);
        }

        public void Pose()
        {
            playerAnim.Play("Pose", 0, 0);
            opponentAnim.Play("Pose", 0, 0);
            ball.SetActive(false); // temporary solution, should realistically just fall down
            inPose = true;
        }

        public void ChangeCameraAngle(double beat, float rotation, float camZoom, double length, Util.EasingFunction.Ease ease, bool additive = true)
        {
            cameraRotateBeat = beat;
            cameraRotateLength = length;
            cameraRotateEase = ease;
            cameraRotateLast = cameraRotateNext % 360f;
            cameraScaleLast = cameraScaleNext;
            cameraScaleNext = camZoom;
            if (additive)
            {
                cameraRotateNext = cameraRotateLast + rotation;
            }
            else
            {
                cameraRotateNext = rotation;
            }
        }

        public void PrepareFastRally(double beat, RallySpeed speedChange, bool muteAudio = false)
        {
            if (speedChange == RallySpeed.Fast)
            {
                BeatAction.New(this, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat + 2f, delegate { Serve(beat + 2f, RallySpeed.Fast); })
                });

                if (muteAudio) return;
                TonkTinkTonkStretchable(beat, 1.5f);
            }
            else if (speedChange == RallySpeed.SuperFast)
            {
                SuperFastRallyStretchable(beat + 4f, 8f);

                if (muteAudio) return;
                TonkTinkTonkStretchable(beat, 4f);
            }
        }

        public static void TonkTinkTonkStretchable(double beat, float length)
        {
            List<MultiSound.Sound> soundsToPlay = new List<MultiSound.Sound>();
            bool tink = false;
            for (float i = 0; i < length; i += 0.5f)
            {
                soundsToPlay.Add(new MultiSound.Sound(tink ? "rhythmRally/Tink" : "rhythmRally/Tonk", beat + i));
                tink = !tink;
            }
            MultiSound.Play(soundsToPlay.ToArray(), forcePlay: true);
        }

        public void SuperFastRallyStretchable(double beat, float length)
        {
            List<BeatAction.Action> servesToPerform = new List<BeatAction.Action>();

            for (int i = 0; i < length; i += 2)
            {
                double beatToSpawn = beat + i;
                servesToPerform.Add(new BeatAction.Action(beatToSpawn, delegate { Serve(beatToSpawn, RallySpeed.SuperFast); }));
            }
            BeatAction.New(this, servesToPerform);
        }
    }
}