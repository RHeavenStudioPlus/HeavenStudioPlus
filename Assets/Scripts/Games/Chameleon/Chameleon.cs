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
    public static class RvlChameleonLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("chameleon", "Chameleon", "ffffff", false, false, new List<GameAction>()
            {
                new GameAction("far", "Far Fly")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        Chameleon.PreSpawnFly(e.beat, e.length - 4, (int)Scripts_Chameleon.FlyType.Far);
                        if (e["countIn"]) Chameleon.CountIn(e.beat);
                    },
                    defaultLength = 8f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("countIn", false, "Count-In"),
                    }
                },
                new GameAction("close", "Close Fly")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        Chameleon.PreSpawnFly(e.beat, e.length - 4, (int)Scripts_Chameleon.FlyType.Close);
                        if (e["countIn"]) Chameleon.CountIn(e.beat);
                    },
                    defaultLength = 8f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("countIn", false, "Count-In"),
                    }
                },
                new GameAction("background appearance", "Background Appearance")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        Chameleon.instance.BackgroundColorSet(e.beat, e.length, e["colorBG1Start"], e["colorBG1End"], e["colorBG2Start"], e["colorBG2End"], e["ease"]);
                    },
                    defaultLength = 0.5f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("colorBG1Start", new Color(0.204f, 0.385f, 0.064f), "Start BG Color", "Set top-most color of the background gradient at the start of the event."),
                        new Param("colorBG1End", new Color(0.204f, 0.385f, 0.064f), "End BG Color", "Set top-most color of the background gradient at the end of the event."),
                        new Param("colorBG2Start", new Color(0.07f, 0.133f, 0.02f), "Start BG Color", "Set bottom-most color of the background gradient at the start of the event."),
                        new Param("colorBG2End", new Color(0.07f, 0.133f, 0.02f), "End BG Color", "Set bottom-most color of the background gradient at the end of the event."),
                        new Param("ease", Util.EasingFunction.Ease.Instant, "Ease", "Set the easing of the action."),
                    }
                },
                new GameAction("modifiers", "Modifiers")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity; 
                        Chameleon.Modifiers(e["enableCrown"]); 
                    },
                    defaultLength = 0.5f,
                    parameters = new List<Param>()
                    {
                        new Param("enableCrown", true, "Enable Crown", "Toggle if the crown should appear or not."),
                    },
                },
            },
            new List<string>() { "rvl", "normal" },
            "rvlchameleon", "en",
            new List<string>() {},
            chronologicalSortKey: 107
            );
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_Chameleon;
    public class Chameleon : Minigame
    {
        public Transform baseFly;
        public Animator chameleonAnim;
        [SerializeField] Transform chameleonEye;
        [SerializeField] GameObject Crown;
        [System.NonSerialized] public Fly currentFly;

        [SerializeField] SpriteRenderer gradient;
        [SerializeField] SpriteRenderer bgHigh;
        [SerializeField] SpriteRenderer bgLow;

        private ColorEase[] colorEases = new ColorEase[2];
        
        static bool enableCrown;

        float moveEyeTimeThreshold = 0.01f;
        float moveEyeTime = 0;

        private struct QueuedFly
        {
            public double beat;
            public double length;
            public int type;
        }
        private static List<QueuedFly> queuedFlys = new();

        public static Chameleon instance;

        const int IAAltDownCat = IAMAXCAT;
        
        protected static bool IA_PadAltPress(out double dt)
        {
            return PlayerInput.GetPadDown(InputController.ActionsPad.South, out dt);
        }
        protected static bool IA_BatonAltPress(out double dt)
        {
            return PlayerInput.GetSqueezeDown(out dt);
        }
        
        public static PlayerInput.InputAction InputAction_Close =
            new("RvlChameleonClose", new int[] { IAPressCat, IAPressCat, IAPressCat },
            IA_PadBasicPress, IA_TouchFlick, IA_BatonBasicPress);

        public static PlayerInput.InputAction InputAction_Far =
            new("RvlChameleonFar", new int[] { IAAltDownCat, IAAltDownCat, IAAltDownCat },
            IA_PadAltPress, IA_TouchFlick, IA_BatonAltPress);
            

        private void Awake()
        {
            instance = this;

            Crown.SetActive(enableCrown);
            colorEases = new ColorEase[] {
                new(new Color(0.204f, 0.385f, 0.064f)),
                new(new Color(0.07f, 0.133f, 0.02f)),
            };
        }

        public override void OnPlay(double beat)
        {
            if (queuedFlys.Count > 0) queuedFlys.Clear();
            foreach (var evt in scheduledInputs)
            {
                evt.Disable();
            }
        }

        private void OnDestroy()
        {
            if (queuedFlys.Count > 0) queuedFlys.Clear();
            foreach (var evt in scheduledInputs)
            {
                evt.Disable();
            }
        }

        private void Update()
        {
            var cond = Conductor.instance;
            if (cond.isPlaying && !cond.isPaused)
            {
                if (queuedFlys.Count > 0)
                {
                    foreach (var queuedFly in queuedFlys)
                    {
                        SpawnFly(queuedFly.beat, queuedFly.length, queuedFly.type);
                    }
                    queuedFlys.Clear();
                }
                if (PlayerInput.GetIsAction(InputAction_Close) && !IsExpectingInputNow(InputAction_Close))
                {
                    chameleonAnim.DoScaledAnimationAsync("tongueClose", 0.5f);
                    SoundByte.PlayOneShotGame("chameleon/blankClose");
                }
                else if (PlayerInput.GetIsAction(InputAction_Far) && !IsExpectingInputNow(InputAction_Far))
                {
                    chameleonAnim.DoScaledAnimationAsync("tongueFar", 0.5f);
                    SoundByte.PlayOneShotGame("chameleon/blankFar");
                }

                moveEyeTime += Time.deltaTime;
                if (moveEyeTime >= moveEyeTimeThreshold)
                {
                    moveEyeTime = 0;
                    if (currentFly is not null)
                    {
                        if (cond.songPositionInBeatsAsDouble >= currentFly.startBeat + 1)
                        {
                            Vector3 relative = currentFly.gameObject.transform.InverseTransformPoint(chameleonEye.transform.position);
                            float currentAngle = chameleonEye.eulerAngles.z;
                            currentAngle = currentAngle > 180 ? currentAngle - 360 : currentAngle;
                            float nextAngle = 165 + Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
                            nextAngle = nextAngle > 180 ? nextAngle - 360 : nextAngle;
                            if (currentFly.isFall)
                            {
                                nextAngle = currentFly.flyType switch {
                                    FlyType.Far => (nextAngle < -70) ? -70 : nextAngle,
                                    FlyType.Close => (nextAngle < -100) ? -100 : nextAngle,
                                    _ => throw new System.NotImplementedException()
                                };
                            }
                            float angle = Mathf.Lerp(currentAngle, nextAngle, 0.05f);
                            chameleonEye.eulerAngles = new Vector3(0, 0, angle);
                        }
                    }
                    else
                    {
                        float currentAngle = chameleonEye.eulerAngles.z;
                        float angle = Mathf.LerpAngle(currentAngle, 15, 0.1f);
                        chameleonEye.eulerAngles = new Vector3(0, 0, angle);
                    }
                }
                
                UpdateBackgroundColor();
            }
        }

        public static void PreSpawnFly(double beat, double length, int type)
        {
            if (GameManager.instance.currentGame == "chameleon")
            {
                instance.SpawnFly(beat, length, type);
            }
            else
            {
                queuedFlys.Add(new QueuedFly
                {
                    beat = beat,
                    length = length,
                    type = type,
                });
            }
        }


        public void SpawnFly(double beat, double length, int type)
        {
            if (length < 0) length = 4;
            var newFly = Instantiate(baseFly, transform).GetComponent<Fly>();
            newFly.startBeat = beat;
            newFly.lengthBeat = length;
            newFly.flyType = (FlyType)type;

            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate {
                    currentFly = newFly;
                    currentFly.gameObject.SetActive(true);
                    currentFly.Init();
                }),
            });
        }

        public static void CountIn(double beat)
        {
            MultiSound.Play(new MultiSound.Sound[]{
                new MultiSound.Sound("count-ins/three2", beat + 4),
                new MultiSound.Sound("count-ins/two2", beat + 5),
                new MultiSound.Sound("count-ins/one2", beat + 6),
            }, forcePlay: true, game: false);
        }


        public void BackgroundColorSet(double beat, float length, Color BG1Start, Color BG1End, Color BG2Start, Color BG2End, int colorEaseSet)
        {
            colorEases = new ColorEase[] {
                new(beat, length, BG1Start, BG1End, colorEaseSet),
                new(beat, length, BG2Start, BG2End, colorEaseSet),
            };

            UpdateBackgroundColor();
        }
        private void UpdateBackgroundColor()
        {
            gradient.color = colorEases[0].GetColor();
            bgHigh.color = colorEases[0].GetColor();
            bgLow.color = colorEases[1].GetColor();
        }
                
        public static void Modifiers(bool enableCrown)
        {
            Chameleon.enableCrown = enableCrown;

            if (GameManager.instance.currentGame == "chameleon") 
                Chameleon.instance.Crown.SetActive(enableCrown);
        }
    }
}