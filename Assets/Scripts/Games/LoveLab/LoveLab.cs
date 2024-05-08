using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;
using UnityEngine.Sprites;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class NtrLabLoader
{
    public static Minigame AddGame(EventCaller e)
    {
        return new Minigame("loveLab", "Love Lab", "b5ffff", false, false, new List<GameAction>()
            {
                new GameAction("bop", "Bop")
                {
                    function = delegate { LoveLab.instance.Bop(e.currentEntity.beat, e.currentEntity.length, 
                        e.currentEntity["toggle"], e.currentEntity["toggle2"]); },
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("toggle", true, "Bop", "Whether both will bop to the beat or not"),
                        new Param("toggle2", false, "Bop (Auto)", "Whether both will bop automatically to the beat or not")
                    }
                },

                new GameAction("beat intervals", "Start Interval")
                {
                    preFunction = delegate { LoveLab.PreInterval(e.currentEntity.beat, e.currentEntity.length, e.currentEntity["auto"]);   },
                    parameters = new List<Param>()
                    {
                        new Param("auto", true, "Auto Pass Turn", "Toggle if the turn should be passed automatically at the end of the start interval.")
                    },
                    preFunctionLength = .9f,
                    defaultLength = 4f,
                    resizable = true,
                    priority = 2,
                },
                new GameAction("boy shakes", "Boy Shakes")
                {
                    parameters = new List<Param>()
                    {
                        new Param("speed", LoveLab.throwFlaskSpeedType.fastFlask, "Flask Speed", "How fast should the boy throw the flasks to the girl?")
                    },
                    defaultLength = 1f,
                    resizable = true,
                    priority = 1
                },

                new GameAction("girl blush", "Girl Blush")
                {
                    function = delegate { LoveLab.instance.girlBlush(e.currentEntity["toggle"]); },
                    defaultLength = .5f,
                    parameters = new List<Param>()
                    {
                        new Param("toggle", false, "Auto", "Blush automatically?")
                    }

                },

                new GameAction("boxGuy", "Box Guy")
                {
                    function = delegate { LoveLab.instance.mainBoxGuy(e.currentEntity.beat, e.currentEntity["type"]); },
                    defaultLength = 2f,
                    resizable = false,
                    parameters = new List<Param>()
                    {
                        new Param("type", LoveLab.boxGuyAction.takeAway, "Action", "Box Guy's Actions")
                    }
                },

                new GameAction("set object colors", "Object Colors")
                {
                    function = delegate {
                        LoveLab.instance.UpdateMaterialColor(e.currentEntity["colorA"], e.currentEntity["colorB"], e.currentEntity["colorC"]);
                    },
                    defaultLength = 0.5f,
                    parameters = new List<Param>()
                    {
                        new Param("colorA", new Color(0.02909997f, 0.4054601f, 0.97f), "1st Flask Color", "1st Flask Color to show"),
                        new Param("colorB", new Color(0.81f, 0.2984211f, 0f), "2nd Flask Color", "2nd Flask Color to show"),
                        new Param("colorC", new Color(0.8313726f, 0.2039216f, 0.5058824f), "3rd Flask Color", "3rd Flask Color to show")
                    },
                    priority = 2
                },

                new GameAction("set time of day", "Time of Day")
                {
                    function = delegate
                    {
                        LoveLab.instance.setTimeOfDay(e.currentEntity["type"]);
                    },
                    defaultLength = .5f,
                    parameters = new List<Param>()
                    {
                        new Param("type", LoveLab.timeOfDay.sunset, "Time", "Set the time of day")
                    }
                },

                new GameAction("clouds", "Clouds")
                {
                    function = delegate { LoveLab.instance.cloudMove(e.currentEntity["toggle"]); },
                    defaultLength = .5f,
                    parameters = new List<Param>()
                    {
                        new Param("toggle", true, "Move", "Move Clouds?")
                    }
                },

                new GameAction("spotlight", "Spotlight")
                {
                    function = delegate { LoveLab.instance.spotlight(e.currentEntity["toggle"], e.currentEntity["spotType"], e.currentEntity["posType"]); },
                    defaultLength = .5f,
                    parameters = new List<Param>()
                    {
                        new Param("toggle", false, "Spotlight", "Activate Spotlight?"),
                        new Param("spotType", LoveLab.spotlightType.normal, "Spotlight Type", "Normal or Coned?"),
                        new Param("posType", LoveLab.spotlightPos.boy, "Spotlight Position", "(For cone) under the Boy or the Girl?")
                    },
                    priority = 1
                }
            }
        //new List<string>() { "ntr", "repeat" },
        //"ntrlab", "en",
        //new List<string>() { "en" }
        //chronologicalSortKey: 19
        );
    }
}
}

namespace HeavenStudio.Games
{
    using DG.Tweening;
    using Jukebox;
    using Scripts_LoveLab;
    using UnityEngine.UIElements;

    public class LoveLab : Minigame
    {
        public static LoveLab instance { get; set; }

        #region varibles + components
        [Header("Lab Guy")]
        [SerializeField] Animator labGuy;
        [SerializeField] Animator labGuyHead;
        [SerializeField] Animator labGuyArm;

        [Header("Lab Girl")]
        [SerializeField]  Animator labGirl;
        [SerializeField] Animator labGirlHead;
        [SerializeField] Animator labGirlArm;

        [Header("Lab Weird")]
        [SerializeField] Animator labAssistant;
        [SerializeField] Animator labAssistantHead;
        [SerializeField] Animator labAssistantArm;

        [Header("Flask")]
        public GameObject flask;
        [SerializeField] GameObject labFlaskObj;
        public GameObject labGirlFlaskObj;
        [SerializeField] ParticleSystem boyFlaskBreak;
        [SerializeField] ParticleSystem girlFlaskBreak;
        [SerializeField] Material flaskMatForBoy;
        [SerializeField] Material flaskMatForGirl;
        [SerializeField] Material flaskMatForWeird;
        [SerializeField] Color boyLiquidColor = new Color(0.02909997f, 0.4054601f, 0.97f); //216, 97, 97   0868F8
        [SerializeField] Color girlLiquidColor = new Color(0.972549f, 0.3764706f, 0.03137255f); //F86008 
        [SerializeField] Color weirdLiquidColor = new Color(0.8313726f, 0.2039216f, 0.5058824f); //331, 75, 83   D43481
        [SerializeField] SpriteRenderer flaskSpriteRend;
        [SerializeField] SpriteRenderer girlFlaskSpriteRend;
        [SerializeField] SpriteRenderer weirdFlaskSpriteRend;
        [SerializeField] List<string> flaskArcToBoy = new List<string>();
        [SerializeField] List<string> flaskArcToGirl = new List<string>();    

        [Header("Misc")]
        [SerializeField] Animator heartBox;
        [SerializeField] GameObject spotlightShader;
        [SerializeField] GameObject spotlightShaderCone;
        [SerializeField] GameObject spotlightCone;
        [SerializeField] GameObject labHeartBox;
        [SerializeField] Animator boxPerson;
        [SerializeField] Animator boxPersonDay;
        [SerializeField] GameObject heartContainer;
        [SerializeField] GameObject labGuyFlask;
        [SerializeField] GameObject labGirlFlask;
        public static List<QueuedFlask> queuedFlask = new List<QueuedFlask>();
        //public List<GameObject> boyInstantiatedFlask = new List<GameObject>(); //blank to boy
        public List<LoveLabFlask> girlInstantiatedFlask = new List<LoveLabFlask>(); //boy to girl
        public List<LoveLabFlask> weirdInstantiatedFlask = new List<LoveLabFlask>(); //girl to weird

        [Header("Variables")]
        public bool hasMissed = false;
        bool canBop = false;
        public bool bopRight = false;
        public bool isHoldingWhiff = false;
        bool isHoldingWhiffAlt = false;
        bool isHoldingWhiffPressed = false;
        public bool canCallForFlask = true;
        bool isDay = false;
        bool hasStartedInterval;
        [SerializeField] bool isHolding;
        public int counter = 0;
        bool isLong = false;
        int isLongCount = 0;
        bool canCloudsMove = true;
        bool hasShakenUp = false;
        public double startBeat;

        [Header("Clouds")]
        [SerializeField] GameObject clouds;
        [SerializeField] float cloudSpeed;
        [SerializeField] float cloudDistance;

        [Header("Time Of Day")]
        [SerializeField] GameObject sunsetBG;
        [SerializeField] GameObject dayBG;
        [SerializeField] Material girlShader;
        [SerializeField] GameObject girlHeaderShader;
        [SerializeField] Material boyShader;
        [SerializeField] GameObject boyHeaderShader;
        [SerializeField] Material weirdShader;
        [SerializeField] GameObject weirdHeaderShader;

        [Header("Hearts")]
        //public List<Sprite> warningSprites = new List<Sprite>();
        public float speedForHearts = 0.00069f;
        public float speedForHeartsMultiplier = 3f;
        public float speedForHeartsMinimizer = .001f;
        public List<heartDetails> reqHeartsContainer = new List<heartDetails>();
        [SerializeField] Transform heartHolder;
        [SerializeField] GameObject guyHeartObj;
        [SerializeField] GameObject girlHeartObj;
        [SerializeField] GameObject completeHeartObj;
        public List<LoveLabHearts> guyHearts;
        public List<LoveLabHearts> girlHearts;
        public List<LoveLabHearts> completeHearts;
        List<PlayerActionEvent> currentEvents;
        [SerializeField] List<int> currentHearts;
        [SerializeField] Transform endPoint;
        [SerializeField] Vector3 spawnPoint;
        #endregion


        #region time of day
        public enum timeOfDay
        {
            sunset,
            day
        }

        public void defaultShaders()
        {
            sunsetBG.SetActive(true);
            dayBG.SetActive(false);

            getShaderColors(0);

            girlHeaderShader.SetActive(true);

            boyHeaderShader.SetActive(true);

            weirdHeaderShader.SetActive(true);

            isDay = false;
        }

        public void getShaderColors(int tod)
        {
            float[,] wantShader;

            wantShader = (int)timeOfDay.sunset == tod ? shaderColorsForSunset : shaderColorsForDay;

            for (int x = 0; x < 3; x++)
            {
                girlShader.SetColor(shaderColorsDebug[x], new Color(wantShader[x, 0], wantShader[x, 1], wantShader[x, 2]));
                boyShader.SetColor(shaderColorsDebug[x], new Color(wantShader[x + 3, 0], wantShader[x + 3, 1], wantShader[x + 3, 2]));
                weirdShader.SetColor(shaderColorsDebug[x], new Color(wantShader[x + 6, 0], wantShader[x + 6, 1], wantShader[x + 6, 2]));
            }
        }

        public float[,] shaderColorsForDay = new float[,]
        {
            //girl
            { 0.8470589f, 0.9725491f, 0.8784314f },
            { 0.01176471f, 0.7686275f, 0.2431373f },
            { 0.9686275f, 0.7529413f, 0.654902f },
            //boy
            { 1f, 1f, 1f },
            { 0.3921569f, 0.4039216f, 0.3921569f },
            { 0.9686275f, 0.7254902f, 0.5686275f },
            //weird
            { 1f, 1f, 1f },
            { 0.754717f, 0.5786163f, 0f },
            { 0.9686275f, 0.3803922f, 0.03137255f },
        };

        public float[,] shaderColorsForSunset = new float[,]
        {
            //girl
            { 1f, 0.937255f, 0.3529412f },
            { 0.01176471f, 0.7686275f, 0.2431373f },
            { 0.9960785f, 0.6588235f, 0.5176471f },
            //boy
            { 1f, 0.8392158f, 0.4196079f },
            { 0.6862745f, 0.4078432f, 0.4196079f },
            { 0.8901961f, 0.4627451f, 0.2039216f },
            //weird
            { 0.9725491f, 0.7843138f, 0.4078432f },
            { 0.972549f, 0.7529412f, 0.03137255f },
            { 0.8941177f, 0.1882353f, 0f },
        };

        public string[] shaderColorsDebug = new string[]
        {
            "_ColorAlpha",
            "_ColorBravo",
            "_ColorDelta",
        };

        public void setTimeOfDay(int tod)
        {
            if(tod == (int)timeOfDay.sunset)
            {
                defaultShaders();
                getShaderColors(tod);
            }
            else
            {
                sunsetBG.SetActive(false);
                dayBG.SetActive(true);

                getShaderColors(tod);

                girlHeaderShader.SetActive(false);

                boyHeaderShader.SetActive(false);

                weirdHeaderShader.SetActive(false);

                isDay = true;
            }
        }
        #endregion

        #region intervals
        float smthPreteval;
        bool smthAutoPass;

        public Transform getHeartHolder()
        {
            return heartHolder;
        }
        public void playFlaskBreak(int whichFlask)
        {
            if(whichFlask == 0) //guy
            {
                boyFlaskBreak.Play();
            }
            else
            {
                girlFlaskBreak.Play();
            }
        }

        public void labGuyIdleState()
        {
            labGuyHead.DoScaledAnimationAsync("GuyFaceIdle");

            if (labGuyArm.IsAnimationNotPlaying())
            {
                labGuyArm.Play("ArmIdle");
            }
        }

        public void labGirlIdleState()
        {
            instance.labGirlHead.Play("GirlIdleFace");
            if (instance.labGirlArm.IsAnimationNotPlaying())
            {
                instance.labGirlArm.DoScaledAnimationAsync("ArmIdle");
            }
        }
        public void labWeirdEndState(double beat, float addBeat, GameObject flaskObj)
        {
            BeatAction.New(LoveLab.instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + addBeat, delegate
                {
                    Destroy(flaskObj);
                    weirdInstantiatedFlask.RemoveAt(0);

                    labAssistantArm.DoScaledAnimationAsync("MittenGrabStart", 1f);
                }),

                new BeatAction.Action(beat + (addBeat+1f), delegate
                {

                    labAssistantArm.DoScaledAnimationAsync("MittenGrab", 1f);
                }),
                

                new BeatAction.Action(beat + (addBeat + 1.5f), delegate
                {
                    labAssistantArm.DoScaledAnimationAsync("MittenLetGo", .5f);
                }),
            });
        }
        public enum throwFlaskSpeedType
        {
            fastFlask,
            slowFlask,
            midSlowFlask
        }
        private struct queuedInterval
        {
            public double beat;
            public float interval;
            public bool autoPassTurn;
        }
        private static List<queuedInterval> queuedIntervals = new List<queuedInterval>();

        public static void PreInterval(double beat, float interval, bool autoPassTurn)
        {
            instance.smthPreteval = interval;
            instance.smthAutoPass = autoPassTurn;

            if (GameManager.instance.currentGame == "loveLab")
            {
                instance.SetIntervalStart(beat, interval, beat, autoPassTurn);
            }
            else
            {
                queuedIntervals.Add(new queuedInterval
                {
                    beat = beat,
                    interval = interval,
                    autoPassTurn = autoPassTurn
                });    
            }
        }

        private void SetIntervalStart(double beat, float interval, double gameSwitchBeat, bool autoPassTurn)
        {
            //List<RiqEntity> shakeEvents = GetAllFlaskBetween(beat, beat + interval);
            ////Debug.LogWarning(shakeEvents.Count);
            //shakeEvents.Sort((x, y) => x.beat.CompareTo(y.beat));


            //for (int x = 0; x < shakeEvents.Count; x++)
            //{
            //    var a = shakeEvents[x];
            //    boyShake(a.beat, a.length, beat, beat + interval, interval, a["speed"]);
            //}


            //string reqArc = "";

            //reqArc = shakeEvents[shakeEvents.Count - 1]["speed"] == 0 ? flaskArcToBoy[0] : flaskArcToBoy[1];

            //spawnCustomFlask(shakeEvents[0].beat, reqArc);

            //this.counter = shakeEvents.Count;

            //List<BeatAction.Action> queuedFlasks = new()
            //{
            //    new BeatAction.Action(beat, delegate
            //    {
            //        instance.hasMissed = false;

            //        if (autoPassTurn)
            //        {
            //            BoyPassToGirl(beat + interval, beat, beat + interval, 1, 0);
            //        }
            //    })
            //};
            //BeatAction.New(this, queuedFlasks);


            #region stuff that worked
            List<RiqEntity> reqShakeEvents = GetAllFlaskBetween(beat, beat + interval);
            reqShakeEvents.Sort((x, y) => x.beat.CompareTo(y.beat));

            currentHearts.Add(reqShakeEvents.Count);
            //howManyShakes.Add(reqShakeEvents.Count);
            //howManyShakesCounter.Add(0);
            instance.hasMissed = false;

            string reqArc = "";

            reqArc = reqShakeEvents[reqShakeEvents.Count - 1]["speed"] == 0 ? flaskArcToBoy[0] : flaskArcToBoy[1];

            spawnCustomFlask(reqShakeEvents[0].beat, reqArc);

            this.counter = reqShakeEvents.Count;

            List<BeatAction.Action> queuedFlasks = new()
            {
                new BeatAction.Action(beat, delegate
                {
                    //initial shakes
                    instance.hasMissed = false;

                    if (autoPassTurn)
                    {
                        //pass to girl
                        BoyPassToGirl(beat + interval, beat, beat + interval, 1, 0);
                    }
                })
            };
            for (int i = 0; i < reqShakeEvents.Count; i++)
            {
                RiqEntity shakeEventToCheck = reqShakeEvents[i];
                if (shakeEventToCheck.beat >= gameSwitchBeat)
                {
                    queuedFlasks.Add(new BeatAction.Action(shakeEventToCheck.beat, delegate
                    {
                        //lab boy shake
                        boyShake(shakeEventToCheck.beat, shakeEventToCheck.length, beat, beat + interval, interval, shakeEventToCheck["speed"]);
                    }));
                }
            }
            BeatAction.New(this, queuedFlasks);
            #endregion
        }

        private void PersistObjects(double beat)
        {
            List<RiqEntity> allIntervals = EventCaller.GetAllInGameManagerList("loveLab", new string[] { "beat intervals" }).FindAll(x => x.beat < beat);
            //List<RiqEntity> shakeEvents = EventCaller.GetAllInGameManagerList("loveLab", new string[] { "boy shakes" }).FindAll(x => x.beat < beat && x.beat + x.length > beat);
            RiqEntity lastInterval = allIntervals[allIntervals.Count - 1];
            Debug.LogWarning("Last Interval Beat: " + lastInterval.beat);
            Debug.LogWarning("Last Interval Length: " + lastInterval.length);
            List<RiqEntity> shakeEvents = GetAllFlaskBetween(beat, lastInterval.beat + lastInterval.length);

            Debug.LogWarning("Shake Event Count: "+ shakeEvents.Count);


            if(allIntervals.Count != 0)
            {
                hasStartedInterval = true;
                Debug.LogWarning("pre existing intervals");
            }

            for (int x = 0; x < shakeEvents.Count; x++)
            {
                var a = shakeEvents[x];
                boyShake(a.beat, a.length, beat, beat + lastInterval.length, lastInterval.length, a["speed"]);
            }
        }

        private List<RiqEntity> GetAllFlaskBetween(double beat, double endBeat)
        {
            List<RiqEntity> speakEvents = EventCaller.GetAllInGameManagerList("loveLab", new string[] { "boy shakes" });
            List<RiqEntity> tempEvents = new();

            foreach (var entity in speakEvents)
            {
                if (entity.beat >= beat && entity.beat < endBeat)
                {
                    tempEvents.Add(entity);
                }
            }

            return tempEvents;
        }

        private void girlLook(double beat, double length)
        {
            float speedLook = 0;
            speedLook = length <= .5f ? 0 : .5f;

            labGuyHead.DoScaledAnimationAsync("GuyRightFace");

            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + speedLook, delegate
                {
                    labGirlHead.Play("GirlLeftFace");
                })
            });
        }

        private void boyShake(double beat, double length, double firstBeatOfInterval, double lastBeatOfInterval, double intervalLength, int speedType)
        {
            #region debug stuff
            //Debug.LogWarning("Beat: " + beat);
            //Debug.LogWarning("Length: " + (beat + length));
            //Debug.LogWarning("First Beat: " + firstBeatOfInterval);
            //Debug.LogWarning("Last Beat: " + lastBeatOfInterval);
            //Debug.LogWarning("Speed Type: " + speedType);
            #endregion

            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate {                
                    girlLook(beat, intervalLength);

                    if (!hasStartedInterval)
                    {
                        SoundByte.PlayOneShotGame("loveLab/LeftCatch", beat, forcePlay: true);
                        labGuyArm.Play("GrabFlask");
                        hasStartedInterval = true;
                        LoveLabHearts bh = Instantiate(guyHeartObj, instance.heartHolder).GetComponent<LoveLabHearts>();
                        bh.heartBeat = beat;
                        bh.length = length;
                        bh.heartCount = girlHearts.Count != 0 ? 0 : guyHearts.Count;
                        bh.intervalSpeed = intervalLength;
                        bh.addPos = length <= .5f && instance.currentHearts[instance.currentHearts.Count - 1] <= 1 ? 1.5f : 2.5f;    

                        guyHearts.Add(bh);
                        //spawnCustomFlask(beat);
                    }
                    else if(beat > firstBeatOfInterval)
                    {
                        SoundByte.PlayOneShotGame("loveLab/shakeDown");
                        labGuyArm.Play("ShakeFlaskDown");
                    }
                }),

                new BeatAction.Action(beat + length, delegate {
                    if(lastBeatOfInterval > beat + length)
                    {
                        SoundByte.PlayOneShotGame("loveLab/shakeUp");
                        labGuyArm.Play("ShakeFlaskUp");

                        LoveLabHearts bh = Instantiate(guyHeartObj, instance.heartHolder).GetComponent<LoveLabHearts>();
                        bh.heartBeat = (beat + length);
                        bh.length = length;
                        bh.heartCount = guyHearts.Count;
                        bh.intervalSpeed = intervalLength;

                        guyHearts.Add(bh);

                        heartUp(guyHearts);
                    }
                    else
                    {
                        SoundByte.PlayOneShotGame("loveLab/leftThrow");
                        labGuyArm.Play("ThrowFlask");
                        spawnFlaskForGirlCustom((beat + length) + 1f, speedType);

                        foreach(LoveLabHearts h in guyHearts)
                        {
                            h.stop = true;
                        }
                    }
                }),
            });
        }

        public List<int> howManyShakes = new List<int>();
        public List<int> howManyShakesCounter = new List<int>();
        public List<double> howManyShakesEndBeat = new List<double>();

        public void clearShakeList()
        {
            howManyShakes.RemoveAt(0);
            howManyShakesCounter.RemoveAt(0);
            howManyShakesEndBeat.RemoveAt(0);
        }



        private void BoyPassToGirl(double beat, double intervalBeat, double endBeat, float length, int counter)
        {
            var inputs = GetAllFlaskBetween(intervalBeat, endBeat);
            inputs.Sort((x, y) => x.beat.CompareTo(y.beat));

            howManyShakes.Add(inputs.Count);

            float addDelay = 0f;

            inputs[0]["speed"] = inputs[inputs.Count - 1]["speed"];

            #region debug stuff
            //Debug.LogWarning("Beat: " + beat);
            //Debug.LogWarning("Length: " + (beat + length));
            //Debug.LogWarning("First Beat: " + intervalBeat);
            //Debug.LogWarning("Last Beat: " + endBeat);
            //Debug.LogWarning("Speed Type: " + speedType);
            #endregion

            if (inputs[0]["speed"] == 0)
            {
                addDelay = 0f;
            }
            else if (inputs[0]["speed"] == 2)
            {
                addDelay = .5f;
            }
            else
            {
                addDelay = 1f;
            }

            for (int i = 0; i < inputs.Count; i++)
            {
                var input = inputs[i];
                double relativeBeat = input.beat - intervalBeat;
                double addForEndBeat = (input.beat + input.length) - intervalBeat;
                if(i == 0)
                {
                    ScheduleInput(beat, (length + relativeBeat) + addDelay, InputAction_BasicPressing, onCatch, onMiss, onEmpty);
                }
                else
                {
                    ScheduleAutoplayInput(beat, (length + relativeBeat) + addDelay, InputAction_Alt, onDownAuto, onMiss, onEmpty);
                    //ScheduleUserInput(beat, (length + relativeBeat) + addDelay, IA_DPadRelease, onDownAuto, onMiss, onEmpty);
                }

                if ((inputs[i].beat + inputs[i].length) >= endBeat)
                {
                    ScheduleInput(beat, (length + addForEndBeat) + addDelay, IA_FlickRelease, onRelease, onMiss, onEmpty);
                }
                else
                {
                    ScheduleInput(beat, (length + addForEndBeat) + addDelay, IA_DPadPressing, onUp, onMiss, onEmpty);
                }

            }

            BeatAction.New(this, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate
                {
                    hasMissed = false;

                }),
                new BeatAction.Action(endBeat + 1f, delegate
                {
                    labGuyHead.Play("GuyFaceIdle");
                    if (labGuyArm.IsAnimationNotPlaying())
                    {
                        labGuyArm.Play("ArmIdle");
                    }
                })
            });
        }


        public static void onCatch(PlayerActionEvent caller, float beat)
        {
            instance.labGirlHead.Play("GirlIdleFace");
            LoveLabHearts gh = Instantiate(instance.girlHeartObj, instance.heartHolder).GetComponent<LoveLabHearts>();
            gh.heartBeat = (caller.startBeat + caller.timer);
            gh.heartCount = instance.girlHearts.Count;
            gh.length = instance.guyHearts[0].length;
            gh.addPos = gh.length <= .5f && instance.currentHearts[instance.currentHearts.Count - 1] <= 1 ? 1.5f : 2.5f;
            gh.intervalSpeed = instance.guyHearts[0].intervalSpeed;
            instance.girlHearts.Add(gh);

            if (!instance.isHolding)
            {
                instance.isHolding = true;
                SoundByte.PlayOneShotGame("loveLab/RightCatch");
                instance.labGirlArm.DoScaledAnimationAsync("GrabFlask");
                LoveLab.instance.girlInstantiatedFlask[0].GetComponent<LoveLabFlask>().destroyThisObj();
            }

            else if (instance.currentHearts[0] > 1 && instance.isHolding)
            {
                SoundByte.PlayOneShotGame("loveLab/shakeDown");
                instance.labGirlArm.DoScaledAnimationAsync("ShakeFlaskDown");
            }
        }

        public void heartUp(List<LoveLabHearts> hearts)
        {
            var heartCount = hearts.Count;
            var constant = 1.3f; //1.5f

            if (hearts[heartCount - 1].getHeartType() == 1)
            {
                hearts[heartCount - 1].intervalSpeed = guyHearts[heartCount - 1].intervalSpeed;
            }

            for (int x = 0; x < heartCount - 1; x++)
            {
                hearts[x].nextHeartBeat = hearts[x + 1].heartBeat;

            }

            for (int x = 0; x < heartCount; x++)
            {
                if (x > 0)
                {
                    hearts[x].prevHeartBeat = hearts[x - 1].heartBeat;
                }
            }

            var max = hearts.Count - 1;
            //Debug.LogWarning(max);

            for (int x = 0; x < heartCount; x++)
            {
                if (hearts.Count > 1) //should only check if hearts is more than 1
                {
                    hearts[x].onlyOne = false;
                    if (x == 0)
                    {
                        //Debug.LogWarning((float)(guyHearts[max].nextHeartBeat));
                        //Debug.LogWarning((float)(guyHearts[max].heartBeat));
                        if (hearts[max].nextHeartBeat == 0)
                        {
                            hearts[x].addPos += (float)(hearts[max].heartBeat - hearts[max].prevHeartBeat) * constant;
                            //hearts[x]._step = (float)(hearts[max].heartBeat - hearts[max].prevHeartBeat);
                            //hearts[x].updateBeat();
                        }
                        else
                        {
                            hearts[0].addPos += (float)(hearts[max].nextHeartBeat - hearts[max].heartBeat) * constant;
                            //hearts[0]._step = (float)(hearts[max].nextHeartBeat - hearts[max].heartBeat);
                            //hearts[x].updateBeat();
                        }

                    }
                    else if (x == max)
                    {
                        continue;
                    }
                    else
                    {
                        hearts[x].addPos += (float)(hearts[max].heartBeat - hearts[max].prevHeartBeat) * constant;
                        //hearts[x]._step = (float)(hearts[max].heartBeat - hearts[max].prevHeartBeat);
                        //hearts[x].updateBeat();
                    }

                    /*  4.5 //only in 0 (next heart - heart)
                                4 (prev heart - heart)
                                3.5 (prev heart - heart)
                                2.5 (prev heart - heart)
                                2 (nothing)

                                should use the current heart spacing
                            */
                }
            }
        }

        public static void onUp(PlayerActionEvent caller, float beat)
        {
            instance.hasShakenUp = true;
            SoundByte.PlayOneShotGame("loveLab/shakeUp");
            instance.labGirlArm.DoScaledAnimationAsync("ShakeFlaskUp");
            LoveLabHearts gh = Instantiate(instance.girlHeartObj, instance.heartHolder).GetComponent<LoveLabHearts>();
            gh.heartBeat = (caller.startBeat + caller.timer);
            gh.heartCount = instance.girlHearts.Count;
            gh.length = instance.guyHearts[gh.heartCount].length;
            instance.girlHearts.Add(gh);

            instance.heartUp(instance.girlHearts);
        } 

        public static void onDownAuto(PlayerActionEvent caller, float beat)
        {
            instance.hasShakenUp = false;
            SoundByte.PlayOneShotGame("loveLab/shakeDown");
            instance.labGirlArm.DoScaledAnimationAsync("ShakeFlaskDown");
        }

        public static void onRelease(PlayerActionEvent caller, float beat)
        {
            instance.hasShakenUp = false;
            instance.labGirlHead.DoScaledAnimationAsync("GirlIdleFace");
            SoundByte.PlayOneShotGame("loveLab/rightThrowNoShake");
            instance.labGirlArm.DoScaledAnimationAsync("ThrowFlask");
            instance.isHolding = false;
            instance.hasStartedInterval = false;

            //instance.clearShakeList();

            foreach (LoveLabHearts h in instance.girlHearts)
            {
                h.stop = true;
            }   

            instance.spawnFlaskForWeird((caller.startBeat + caller.timer));

            var lastCounter = instance.currentHearts[0];

            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action((caller.startBeat + caller.timer) + 1f, delegate
                {
                    instance.labGirlIdleState();

                    SoundByte.PlayOneShotGame("loveLab/heartsCombine");

                     foreach(LoveLabHearts h in instance.girlHearts)
                    {
                        h.stop = true;
                    }

                    var additionalHearts = 0.063f;

                    for(int x = 0; x < instance.currentHearts[0]; x++)
                    {
                        instance.guyHearts[0].heartAnim.Play("HeartMerge");
                        instance.girlHearts[0].heartAnim.Play("HeartGirlMerge");

                        LoveLabHearts a = Instantiate(instance.completeHeartObj, instance.heartHolder).GetComponent<LoveLabHearts>();
                        a.transform.position = new Vector2(a.transform.position.x, instance.girlHearts[0].transform.position.y);
                        var additionalHeartLength = (0.063f * instance.girlHearts.Count);
                        a.heartBeat = caller.startBeat + caller.timer + 1f;
                        a.dropStart = a.transform.position;
                        a.end = instance.endPoint.position;
                        instance.completeHearts.Add(a);
                        a.timer = (2.25f + additionalHeartLength);
                        a.isWaiting = false;
                        //a.goDown();

                        instance.guyHearts.RemoveAt(0);
                        instance.girlHearts.RemoveAt(0);
                    }

                    instance.currentHearts.RemoveAt(0);



                    //for(int x = 0; x < lastCounter, x++)
                    //{
                        //Debug.LogWarning(2.25f + (additionalHearts * x));
                        //instance.completeHearts[x].dropStart = instance.completeHearts[x].transform.position;
                        
                        //instance.completeHearts[x].timer = (2.25f + (additionalHearts * x));
                        //instance.completeHearts[x].isWaiting = false;
                    //}
                    

                    
                    //instance.currentHearts.Clear();
                }),
            });

            for (int x = 0; x < lastCounter; x++)
            {
                #region debug stuff
                //Debug.LogWarning("Add: " + (2.75f + (additionalHearts * (x))));
                //Debug.LogWarning("Pitch: " + (1f + (pitchHearts * (x))));
                //Debug.LogWarning("x: " + x);
                #endregion
                var additionalHearts = 0.063f;
                var a = x;

                

                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action((caller.startBeat + caller.timer) + (2.25f + (additionalHearts * a)), delegate
                    {
                        if(lastCounter == 1)
                        {
                            SoundByte.PlayOneShotGame("loveLab/bagHeartLast");
                            instance.heartBox.DoScaledAnimationAsync("HeartBoxSquish");
                        }
                        else
                        {
                            instance.bagHeartSound(a);
                        }
                    })

                });
            }
        }

        public static void onMiss(PlayerActionEvent caller)
        {
            //SoundByte.PlayOneShot("miss");
            if (instance.hasMissed) return;

            instance.hasShakenUp = false;
            instance.isHolding = false;
            instance.hasMissed = true;
            instance.labGuyHead.DoScaledAnimationAsync("GuyIdleFace");
            //if (instance.labGuyArm.IsAnimationNotPlaying())
            //{
            //    instance.labGuyArm.Play("ArmIdle");
            //}

            //instance.reqHeartsContainer.RemoveAt(0);

            //Destroy(instance.girlInstantiatedFlask[0].gameObject);
            instance.boyFlaskBreak.Play();

            //if(instance.girlInstantiatedFlask[0] != null)
            //{
            //    instance.girlInstantiatedFlask.RemoveAt(0);
            //}

            if (instance.reqHeartsContainer.Count > 0)
            {
                instance.reqHeartsContainer.RemoveAt(0);
            }

            foreach(LoveLabHearts bh in instance.guyHearts)
            {
                bh.deadHeart();
            }

            if(instance.girlHearts.Count != 0)
            {
                foreach(LoveLabHearts gh in instance.girlHearts)
                {
                    gh.deadHeart();
                }
            }
        }

        public static void onMissWhenHold(PlayerActionEvent caller)
        {
            Debug.LogWarning(caller.timer);
            instance.reqHeartsContainer.RemoveAt(0);
            LoveLabFlask spawnedFlask = Instantiate(instance.labGirlFlaskObj, instance.transform).AddComponent<LoveLabFlask>();
            spawnedFlask.onMissWhenHold((caller.startBeat + caller.timer) + 1f);
        }

        public static void onEmpty(PlayerActionEvent caller)
        {
            //empty
        }

        public void bagHeartSound(int x)
        {
            var pitchHearts = 0.14f;
            SoundByte.PlayOneShotGame("loveLab/bagHeart", pitch: 1f + (pitchHearts * x));
            instance.heartBox.DoScaledAnimationAsync("HeartBoxSquish");
        }

        public void onDownFlaskCustom() //for the player not autoplay
        {
            SoundByte.PlayOneShotGame("loveLab/shakeDown");
            labGirlArm.DoScaledAnimationAsync("ShakeFlaskDown");
        }

        public void spawnFlaskForGirlCustom(double beat, int flaskHeart) //boy to girl
        {
            //flaskMatForBoy.SetColor("_ColorAlpha", instance.boyLiquidColor);
            LoveLabFlask spawnedFlask = Instantiate(labFlaskObj, instance.transform).AddComponent<LoveLabFlask>();

            switch (flaskHeart)
            {
                case 0:
                    spawnedFlask.girlArc(beat, flaskArcToGirl[0]);
                    break;
                case 1:
                    spawnedFlask.girlArc(beat, flaskArcToGirl[1]);
                    break;
                case 2:
                    spawnedFlask.girlArc(beat, flaskArcToGirl[2]);
                    break;
            }

            girlInstantiatedFlask.Add(spawnedFlask.GetComponent<LoveLabFlask>());
        }

        public void spawnCustomFlask(double beat, string reqArc) //wall to boy
        {
            LoveLabFlask spawnedFlask = Instantiate(labFlaskObj, instance.transform).AddComponent<LoveLabFlask>();
            spawnedFlask.customShakes(beat, reqArc);
        }

        public void cloudMove(bool canMove)
        {
            canCloudsMove = canMove;
        }
        #endregion

        //make a smooth transition to day to sunset and vice versa

        /*
         * would be nice if there were visual warnings for the next game
         * head of dj and student for the hearts
         * balls of munchy monk
         * fruits for catchy tune
         * balls for double date
         * pots for karate man
         * shuttlecock for air rally
         * farmer head for second contact
         * etc.
         * 
         */

        /* generic's girl blush logic
         * during shake down girl will look at guy
         * before the end of the shakedown (.5f) girl goes idle look 
         * 
         * original girl blush logic
         * random blushes and/or force a look blush
         */

        /*
         * particle effect
         * when someone grabs a flask do an arc with either 2/3/4 hearts popping both left and right
         * 
         */

        /*
         * box guy
         * sleeves are not tinted for sunset
         * 
         */

        /*
         * bops
         * check for offbeat bops (auto)
         * beat 0 weird bug
         */

        public void girlBlush(bool autoBlush)
        {
            if (!autoBlush)
            {
                labGirlHead.Play("GirlBlushFace");
            }
            else
            {
                labGirlHead.Play("GirlBlushFace");
            }
        }
        [SerializeField] SuperCurveObject.Path[] flaskBouncePath;

        public void spotlight(bool active, int whichType, int whichPos)
        {
            if(active == true)
            {
                if (whichType == (int)spotlightType.normal)
                {
                    spotlightShader.SetActive(true);
                    spotlightShaderCone.SetActive(false);
                }
                else
                {
                    spotlightShader.SetActive(false);
                    spotlightShaderCone.SetActive(true);

                    if (whichPos == (int)spotlightPos.boy)
                    {
                        spotlightCone.transform.position = new Vector2(0, 0);
                    }
                    else
                    {
                        spotlightCone.transform.position = new Vector2(5f, 0);
                    }
                }   
            }
            else
            {
                spotlightShader.SetActive(false);
                spotlightShaderCone.SetActive(false);
            }
        }

        public enum spotlightType
        {
            normal,
            cone
        }

        public enum spotlightPos
        {
            boy,
            girl
        }

        public void UpdateMaterialColor(Color liquidCol, Color liquid2Col, Color liquid3Col)
        {
            boyLiquidColor = liquidCol;
            girlLiquidColor = liquid2Col;
            weirdLiquidColor = liquid3Col;

            LoveLab.instance.flaskMatForBoy.SetColor("_ColorAlpha", boyLiquidColor);
            LoveLab.instance.flaskMatForGirl.SetColor("_ColorAlpha", girlLiquidColor);
            LoveLab.instance.flaskMatForWeird.SetColor("_ColorAlpha", weirdLiquidColor);
        }

        public override void OnGameSwitch(double beat)
        {
            preChecks(beat);
            //PersistObjects(beat);
        }

        public override void OnPlay(double beat)
        {
            preChecks(beat);
            queuedFlask.Clear();
        }

        private void OnDestroy()
        {
            //preChecks(0f);
            queuedFlask.Clear();
            //destroyAllFlasks();
            foreach (var evt in scheduledInputs)
            {
                evt.Disable();
            }
        }

        void preChecks(double beat)
        {
            List<RiqEntity> prevEntities = GameManager.instance.Beatmap.Entities.FindAll(c => c.datamodel.Split(0) == "loveLab");
            RiqEntity obj = prevEntities.FindLast(c => c.beat <= beat && c.datamodel == "loveLab/set object colors");

            if (obj != null)
            {
                UpdateMaterialColor(obj["colorA"], obj["colorB"], obj["colorC"]);
            }
            else
            {
                UpdateMaterialColor(new Color(0.02909997f, 0.4054601f, 0.97f), new Color(0.972549f, 0.3764706f, 0.03137255f), new Color(0.8313726f, 0.2039216f, 0.5058824f));
            }

            getShaderColors(0);

            try
            {
                RiqEntity obj2 = prevEntities.FindLast(c => c.beat <= beat && c.datamodel == "loveLab/bop");

                if (obj2 != null)
                {
                    Bop(obj2.beat, obj2.length, obj2["toggle"], obj2["toggle2"]);
                }
                else
                {
                    Bop(0, 0, false, false);
                }
            }
            catch { }
        }

        protected static bool IA_TouchDpadPress(out double dt)
        {
            
            return PlayerInput.GetSlide(out dt) &&  !instance.hasShakenUp;
        }
        protected static bool IA_TouchDpadRelease(out double dt)
        {
            
            return PlayerInput.GetSlide(out dt) &&  instance.hasShakenUp;
        }




        #region IA
        const int IA_AltPress = IAMAXCAT;

        public static PlayerInput.InputAction IA_FlickPress =
            new("NtrLabFlickPress", new int[] { IAPressCat, IAFlickCat, IAPressCat },
            IA_PadBasicPress, IA_TouchBasicPressing, IA_BatonBasicPress);
        public static PlayerInput.InputAction IA_FlickRelease =
            new("NtrLabFlickRelease", new int[] { IAReleaseCat, IAFlickCat, IAReleaseCat },
            IA_PadBasicRelease, IA_TouchFlick, IA_BatonBasicRelease);
        public static PlayerInput.InputAction IA_DPadRelease =
            new("NtrLabDPadRelease", new int[] { IAReleaseCat, IAFlickCat, IAReleaseCat },
                IA_PadAnyUp, IA_TouchDpadRelease, IA_BatonBasicRelease);
        public static PlayerInput.InputAction IA_DPadPressing =
            new("NtrLabDPadPress", new int[] { IAReleaseCat, IAFlickCat, IAReleaseCat },
                IA_PadAnyPressingDown, IA_TouchDpadPress, IA_BatonBasicPressing);

        protected static bool IA_TouchNrmPress(out double dt)
        {
            return PlayerInput.GetTouchDown(InputController.ActionsTouch.Tap, out dt)
                && !instance.IsExpectingInputNow(InputAction_Alt);
        }

        protected static bool IA_PadAnyDown(out double dt)
        {
            return PlayerInput.GetPadDown(InputController.ActionsPad.Up, out dt)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Down, out dt)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Left, out dt)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Right, out dt);
        }

        protected static bool IA_PadAnyUp(out double dt)
        {
            return PlayerInput.GetPadUp(InputController.ActionsPad.Up, out dt)
                    || PlayerInput.GetPadUp(InputController.ActionsPad.Down, out dt)
                    || PlayerInput.GetPadUp(InputController.ActionsPad.Left, out dt)
                    || PlayerInput.GetPadUp(InputController.ActionsPad.Right, out dt);
        }

        protected static bool IA_PadAnyPressingDown(out double dt)
        {
            dt = 0;

            return PlayerInput.GetPadDown(InputController.ActionsPad.Up)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Down)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Left)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Right);
        }


        protected static bool IA_PadNorthDown(out double dt)
        {
            dt = 0;
            return PlayerInput.GetPadDown(InputController.ActionsPad.Up);
        }

        protected static bool IA_PadSouthDown(out double dt)
        {
            dt = 0;
            return PlayerInput.GetPadDown(InputController.ActionsPad.Down);
        }

        //make a down and up stuff for shake

        protected static bool IA_PadAltPress(out double dt)
        {
            return PlayerInput.GetPadDown(InputController.ActionsPad.South, out dt);
        }
        protected static bool IA_BatonAltPress(out double dt)
        {
            return PlayerInput.GetSqueezeDown(out dt);
        }
        protected static bool IA_TouchAltPress(out double dt)
        {
            return PlayerInput.GetTouchDown(InputController.ActionsTouch.Tap, out dt)
                && instance.IsExpectingInputNow(InputAction_Alt);
        }

        protected static bool IA_PadAltPressing(out double dt)
        {
            dt = 0;
            return PlayerInput.GetPad(InputController.ActionsPad.South);
        }
        protected static bool IA_TouchAltPressing(out double dt)
        {
            dt = 0;
            return PlayerInput.GetTouch(InputController.ActionsTouch.Tap);
        }
        protected static bool IA_BatonAltPressing(out double dt)
        {
            dt = 0;
            return PlayerInput.GetBaton(InputController.ActionsBaton.Face);
        }

        public static PlayerInput.InputAction InputAction_TouchRelease =
            new("NtrLabRelease", new int[] { IAEmptyCat, IAReleaseCat, IAEmptyCat },
            IA_Empty, IA_TouchBasicRelease, IA_Empty);


        public static PlayerInput.InputAction InputAction_Nrm =
            new("NtrLabAlt", new int[] { IAPressCat, IAPressCat, IAPressCat },
            IA_PadBasicPress, IA_TouchNrmPress, IA_BatonBasicPress);
        public static PlayerInput.InputAction InputAction_Alt =
            new("NtrLabAlt", new int[] { IA_AltPress, IA_AltPress, IA_AltPress },
            IA_PadAltPress, IA_TouchAltPress, IA_BatonAltPress);
        public static PlayerInput.InputAction InputAction_AltPressing =
            new("NtrLabAlt", new int[] { IA_AltPress, IA_AltPress, IA_AltPress },
                IA_PadAltPressing, IA_TouchAltPressing, IA_BatonAltPressing);
        public static PlayerInput.InputAction InputAction_DPad =
            new("NtrLabAlt", new int[] { IAPressCat, IAPressCat, IAPressCat },
                IA_PadAnyDown, IA_TouchNrmPress, IA_BatonBasicPress);

        public static PlayerInput.InputAction InputAction_DPadNorth =
            new("NtrLabAlt", new int[] { IAPressCat, IAPressCat, IAPressCat },
                IA_PadNorthDown, IA_TouchNrmPress, IA_BatonBasicPress);
        public static PlayerInput.InputAction InputAction_DPadSouth =
            new("NtrLabAlt", new int[] { IAPressCat, IAPressCat, IAPressCat },
                IA_PadSouthDown, IA_TouchNrmPress, IA_BatonBasicPress);

        #endregion

        private void Awake()
        {
            instance = this;

            labGuyHead.DoScaledAnimation("GuyFaceIdle", 1f);
            labAssistantHead.DoScaledAnimation("WeirdFaceIdle", 1f);

            flaskSpriteRend.material = flaskMatForBoy;
            girlFlaskSpriteRend.material = flaskMatForGirl;
            weirdFlaskSpriteRend.material = flaskMatForWeird;
        }

        private void Start()
        {
            if (canCloudsMove) clouds.transform.position = Vector3.left * ((Time.realtimeSinceStartup * cloudSpeed) % cloudDistance);
            else clouds.transform.position = new Vector2(0, 0);
        }

        // Editor gizmo to draw trajectories
        new void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            foreach (SuperCurveObject.Path path in flaskBouncePath)
            {
                if (path.preview)
                {
                    try
                    {
                        labFlaskObj.GetComponent<LoveLabFlask>().DrawEditorGizmo(path);
                    }
                    catch { }
                }
            }
        }

        public SuperCurveObject.Path GetPath(string name)
        {
            foreach (SuperCurveObject.Path path in flaskBouncePath)
            {
                if (path.name == name)
                {
                    return path;
                }
            }
            return default(SuperCurveObject.Path);
        }

        #region Spawn Flasks
        public enum flaskHeart
        {
            oneHeart,
            oneOffHeart,
            twoHeart,
            threeHeart,
            fiveHeart
        }

        public struct QueuedFlask //for the offscreen stuff
        {
            public double beat;
            public flaskHeart type;
        }

        public struct heartDetails
        {
            public flaskHeart heartType;
            public int heartCounter;
        }

        public void spawnFlaskForWeird(double beat)
        {
            LoveLabFlask spawnedFlask = Instantiate(labGirlFlaskObj, instance.transform).AddComponent<LoveLabFlask>();
            spawnedFlask.ForWeirdInit(beat + 1f);
            weirdInstantiatedFlask.Add(spawnedFlask.GetComponent<LoveLabFlask>());
        }
        #endregion

        private void Update()
        {
            var cond = Conductor.instance;
            var songPos = cond.songPositionInBeatsAsDouble;

            float normalizedBeat = Conductor.instance.GetPositionFromBeat(startBeat, 5f);

            if (!cond.isPlaying)
            {
                preChecks(songPos);
            }

            if (cond.isPlaying && !cond.isPaused)
            {
                if (queuedFlask.Count != 0)
                {
                    foreach (QueuedFlask flask in queuedFlask)
                    {
                        //switch (flask.type)
                        //{
                        //
                        //}
                    }
                    queuedFlask.Clear();
                }
            }
            else
            {
                if ((!cond.isPaused) && queuedFlask.Count != 0)
                {
                    queuedFlask.Clear();
                }
            }

            #region whiff grab
            if (PlayerInput.GetIsAction(InputAction_BasicPressing) && !IsExpectingInputNow(InputAction_BasicPressing) && !isHolding)
            {
                Debug.LogWarning("whiff");
                labGirlArm.DoScaledAnimationAsync("WhiffGrab");
                
                isHolding = true;

                
            }
            else if (PlayerInput.GetIsAction(InputAction_BasicRelease) && !IsExpectingInputNow(InputAction_BasicRelease))
            {
                labGirlArm.DoScaledAnimationAsync("ArmIdle");
                instance.hasShakenUp = false;
                isHolding = false;
            }

            #endregion

                

            #region whiff up and down
            if (PlayerInput.GetIsAction(IA_DPadPressing) && PlayerInput.GetIsAction(InputAction_BasicPressing) && (!IsExpectingInputNow(IA_DPadPressing) && !IsExpectingInputNow(IA_DPadRelease)))
            {
                
                    onWhiffUp();

                    if (PlayerInput.CurrentControlStyle == InputController.ControlStyles.Touch)
                {
                    instance.hasShakenUp = true;
                }
            
            }
            else if (PlayerInput.GetIsAction(IA_DPadRelease) && PlayerInput.GetIsAction(InputAction_BasicPressing) && (!IsExpectingInputNow(IA_DPadRelease) && !IsExpectingInputNow(IA_DPadPressing)))
            {

                    onWhiffDown();

                    if (PlayerInput.CurrentControlStyle == InputController.ControlStyles.Touch)
                {
                    instance.hasShakenUp = false;
                }

            }
            #endregion

            if (canCloudsMove) clouds.transform.position = Vector3.left * ((Time.realtimeSinceStartup * cloudSpeed) % cloudDistance);
            else clouds.transform.position = new Vector2(0, 0);
        }

        #region Bops
        public void Bopping()
        {
            if (bopRight && labGuy.IsAnimationNotPlaying())
            {
                labGuy.DoScaledAnimationAsync("GuyBopRight", .5f);
                labGirl.DoScaledAnimationAsync("GirlBopRight", .5f);
                labAssistant.DoScaledAnimationAsync("AssistantBopRight", .5f);
            }
            else
            {
                labGuy.DoScaledAnimationAsync("GuyBopLeft", .5f);
                labGirl.DoScaledAnimationAsync("GirlBopLeft", .5f);
                labAssistant.DoScaledAnimationAsync("AssistantBopLeft", .5f);
            }
            bopRight = !bopRight;
        }
        public void Bop(double beat, float length, bool bop, bool autoBop)
        {
            if (bop)
            {
                for (int i = 0; i < length; i++)
                {
                    var a = i;
                    BeatAction.New(instance, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(beat + a, delegate
                        {
                            Bopping();
                        })
                    });
                }
            }
            if (autoBop)
            {
                ToggleBop(true);
            }
            else
            {
                ToggleBop(false);
            }
        }

        public override void OnLateBeatPulse(double beat)
        {
            if (BeatIsInBopRegion(beat)) SingleBop();
        }

        public void ToggleBop(bool go)
        {
            canBop = go;
        }

        void SingleBop()
        {
            if (canBop)
            {
                Bopping();
            }
        }

        #endregion

        #region Box Guy

        public enum boxGuyAction
        {
            takeAway,
            putBack,
            noBox,
            instaBox
        }

        public void mainBoxGuy(double beat, int reqAction)
        {
            Animator box;

            if (!isDay)
            {
                box = boxPerson;
            }
            else
            {
                box = boxPersonDay;
            }

            switch (reqAction)
            {
                case 0:
                    box.DoScaledAnimationAsync("BoxTakeAway", .5f);
                    break;
                case 1:
                    box.DoScaledAnimationAsync("BoxPutBack", .5f);
                    break;
                case 2:
                    box.DoScaledAnimationAsync("NoBox", 0f);
                    break;
                case 3:
                    box.DoScaledAnimationAsync("BoxIdle", 0f);
                    break;
                default:
                    box.DoScaledAnimationAsync("BoxTakeAway", .5f);
                    break;
            }   
        }


        #endregion

        #region Lab Girl Actions
        public void onWhiffUp()
        {
            if (hasShakenUp && PlayerInput.CurrentControlStyle == InputController.ControlStyles.Touch) return;
            labGirlArm.DoScaledAnimationAsync("WhiffUp", 0.75f);
        }

        public void onWhiffDown()
        {
            if (!hasShakenUp && PlayerInput.CurrentControlStyle == InputController.ControlStyles.Touch) return;
            labGirlArm.DoScaledAnimationAsync("WhiffDown", 0.75f);
        }

        #endregion
    }
}