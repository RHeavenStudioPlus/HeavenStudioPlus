// Hello my brothers, it is RaffyTaffy14 here in the code comments with another monologue.
// I still do not know much crap about Unity or C#.
// Despite that, I have managed to cobble together Sumo Brothers in a fairly functional state.
// A lot of the code may be very weird or just useless (especially the older stuff), but it works for now.
// I started making Sumo Brothers for Heaven Studio all the way back in January of 2023 (13 months ago!)
// Sumo Brothers at that time was VERY janky. Heck, I used blurry screenshots from videos as reference guides for everything.
// Progress on Sumo stopped a few weeks into the project, being as functional as how I left Lockstep for rasmus to pick up.
// In June, I finally learned how to cue inputs (I was doing something wrong for the longest time), so now Finishing Poses could be cued.
// Nothing happened until December, when I coded most of the functionality for the Slapping/Stomping/Posing
// switching, which is how it currently works now.

// From the end of January 2024 to the middle of February 2024, I spent dozens of hours meticulously animating
// every animation (there's *74* when you factor out duplicate animations for the Glasses Brother) for the game, from "SumoSlapPrepare" all
// the way to "SumoPoseGBopMiss2".
// No one thought that there would be anyone crazy enough to hecking animate Sumo Brothers.
// People said that it would be animator suicide to even attempt to animate Sumo Brothers.
// Yet, here I am, still alive and still too crazy.

// Sometime during the mass animation process, I managed to get back in contact with the upscaler for this game for a potential
// redo of the Inu Sensei sprites.
// Let's just say that the sprites got a MASSIVE glow up (thx UnbrokenMage)
// Animation for Sumo Brothers still isn't done, as I am yet to animate the 3rd and 4th poses which are (currently) absent.
// Despite the absense for those 20 animations (yes 2 poses takes up 20 animations), I have managed to do a whopping 54 animations
// during my ~2 week grind.

// I hope that whomever may be reading this shall enjoy the wonders of Sumo Brothers and have a great rest of your day. 
using HeavenStudio.Util;
using HeavenStudio.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class CtrSumouLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("sumoBrothers", "Sumo Brothers", "EDED15", false, false, new List<GameAction>()
            {
                
                new GameAction("bop", "Bop")
                {
                    function = delegate { var e = eventCaller.currentEntity; SumoBrothers.instance.Bop(e.beat, e.length, e["bopInu"], e["bopSumo"], e["bopInuAuto"], e["bopSumoAuto"]); },
                    parameters = new List<Param>()
                    {
                        new Param("bopInu", true, "Inu Sensei", "Whether Inu Sensei should bop."),
                        new Param("bopSumo", true, "Sumo Brothers", "Whether the Sumo Brothers should bop."),
                        new Param("bopInuAuto", false, "Inu Sensei (Auto)", "Whether Inu Sensei should bop automatically."),
                        new Param("bopSumoAuto", false, "Sumo Brothers (Auto)", "Whether the Sumo Brothers should bop automatically."),
                    },
                    defaultLength = 1f,
                    resizable = true
                },

                new GameAction("crouch", "Crouch")
                {
                    function = delegate { var e = eventCaller.currentEntity; SumoBrothers.instance.Crouch(e.beat, e.length, e["inuT"], e["sumoT"]); },
                    parameters = new List<Param>()
                    {
                        new Param("inuT", true, "Inu Sensei", "Whether Inu Sensei should crouch."),
                        new Param("sumoT", true, "Sumo Brothers", "Whether the Sumo Brothers should crouch.")
                    },
                    defaultLength = 1f,
                    resizable = true
                },

                new GameAction("stompSignal", "Stomp Signal")
                {
                    function = delegate { var e = eventCaller.currentEntity; SumoBrothers.instance.StompSignal(e.beat, e["mute"], !e["mute"], e["look"], e["direction"]); },
                    parameters = new List<Param>()
                    {
                        new Param("mute", false, "Mute", "Disables Inu Sensei's sound cues and animations."),
                        new Param("look", true, "Look at Camera", "The Sumo Brothers will look at the camera if transitioning from slapping."),
                        new Param("direction", SumoBrothers.StompDirection.Automatic, "Stomp Direction", "Which direction the Sumo Brothers will begin stomping in."),
                    },
                    inactiveFunction = delegate { var e = eventCaller.currentEntity; if (!e["mute"]) { SumoBrothers.StompSignalSound(e.beat);} },
                    defaultLength = 4f,
                    priority = 4
                },

                new GameAction("slapSignal", "Slap Signal")
                {
                    function = delegate { var e = eventCaller.currentEntity; SumoBrothers.instance.SlapSignal(e.beat, e["mute"], !e["mute"]); },
                    parameters = new List<Param>()
                    {
                        new Param("mute", false, "Mute", "Disables Inu Sensei's sound cues and animations.")
                    },
                    inactiveFunction = delegate { var e = eventCaller.currentEntity; if (!e["mute"]) { SumoBrothers.SlapSignalSound(e.beat);} },
                    defaultLength = 4f,
                    priority = 3
                },

                new GameAction("endPose", "Finishing Pose")
                {
                    function = delegate { var e = eventCaller.currentEntity; SumoBrothers.instance.EndPose(e.beat, e["random"], e["type"], e["bg"], e["confetti"], e["alternate"], e["throw"]); },
                    parameters = new List<Param>()
                    {   new Param("random", true, "Random Pose", "Picks a random pose that will play on a successful input. Does not include the finale pose.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => !(bool)x, new string[] { "type" })
                        }),
                        new Param("type", SumoBrothers.PoseType.Squat, "Pose", "The pose variant that the Sumo Brothers perform on a successful input.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (int)x == (int)SumoBrothers.PoseType.Finale, new string[] { "throw" })
                        }),
                        new Param("throw", true, "Throw Glasses", "If the Blue Sumo Brother will throw his glasses on a successful input."),
                        new Param("alternate", true, "Alternate Background", "Alternates between which of the backgrounds appear on a successful input.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => !(bool)x, new string[] { "bg" })
                        }),
                        new Param("bg", SumoBrothers.BGType.GreatWave, "Background", "The background that appears on a successful input."),
                        new Param("confetti", true, "Confetti (WIP)", "Confetti particles will fly everywhere on a successful input.")
                    },
                    defaultLength = 5f,
                    priority = 2
                },

                new GameAction("look", "Look at Camera")
                {
                    function = delegate { var e = eventCaller.currentEntity; SumoBrothers.instance.LookAtCamera(e.beat, e.length); },
                    /*parameters = new List<Param>()
                    {
                        new Param("look", true, "Look at Camera", "Whether the Sumo Brothers will look at the camera while slapping."),
                    },*/
                    defaultLength = 1f,
                    resizable = true
                },

            },
            new List<string>() { "ctr", "keep" },
            //"ctrsumou", "en",
            //new List<string>() { },
            chronologicalSortKey: 31
            );
        }
    }
}

namespace HeavenStudio.Games
{
    //    using Scripts_SumoBrothers;
    public class SumoBrothers : Minigame
    {
        [Header("Animators")]
        [SerializeField] Animator inuSensei;
        [SerializeField] Animator sumoBrotherP;
        [SerializeField] Animator sumoBrotherG;
        [SerializeField] Animator sumoBrotherGHead;
        [SerializeField] Animator sumoBrotherPHead;
        [SerializeField] Animator impact;
        [SerializeField] Animator glasses;
        [SerializeField] Animator dust;
        [SerializeField] Animator bgMove;
        [SerializeField] Animator bgStatic;

        [Header("Properties")]
        /*static List<queuedSumoInputs> queuedInputs = new List<queuedSumoInputs>();
        public struct queuedSumoInputs
        {
            public float beat;
            public int cue;
            public int poseType;
            public int poseBG;
        }*/

        [SerializeField] private Transform camera;
        public float cameraX = 0f;
        public float cameraXNew = 0f;
        private double justStompBeat;
        private double stompShakeLength;
        public double stompShakeSpeed;

        public List<double> stompShakeTimings = new List<double>();
        public List<float> stompShakeValues = new List<float>();

        private bool goBopSumo;
        private bool goBopInu;

        private bool allowBopSumo;
        private bool allowBopInu;

        private bool sumoStompDir;
        private int sumoSlapDir;
        private int sumoPoseType;
        private string sumoPoseTypeCurrent = "1";
        private int sumoPoseTypeNext;
        private double nextGameswitchBeat = -1;



        private bool lookingAtCamera = false;

        //private double lastReportedBeat = 0f;    Unused value - Marc

        private bool cueCurrentlyActive; 
        private double cueCurrentlyActiveBeat;
        
        //private var stompInput;

        const int IAAltDownCat = IAMAXCAT;

        public static SumoBrothers instance;

        public enum PoseType
        {
            Squat = 1,
            Stance = 2,
            Pointing = 3,
            Finale = 4,
            // finale but without throwing glasses will just be = 5
            Dab = 6,
        }

        public enum BGType
        {
            None = 2,
            GreatWave = 0,
            OtaniOniji = 1,
            Nerd = 3,
        }

        private BGType bgType = BGType.None;
        private BGType bgTypeNext = BGType.None;

        public enum StompDirection
        {
            Automatic = 0,
            Left = 1,
            Right = 2,
        }

        private enum SumoState
        {
            Idle,
            Slap,
            Stomp,
            Pose
        }
        private SumoState sumoState = SumoState.Idle;
        private SumoState sumoStatePrevious = SumoState.Idle;

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

        public static PlayerInput.InputAction InputAction_Alt =
        new("CtrSumouAlt", new int[] { IAAltDownCat, IAFlickCat, IAAltDownCat },
        IA_PadAltPress, IA_TouchFlick, IA_BatonAltPress);


        // Start is called before the first frame update
        void Awake()
        {
            goBopInu = true;
            goBopSumo = true;
            allowBopInu = true;
            allowBopSumo = true;
            instance = this;
            cueCurrentlyActive = false;

            sumoStompDir = false;
            sumoSlapDir = 0;

            sumoPoseType = 0;
            sumoPoseTypeNext = 0;

            var beat = Conductor.instance.songPositionInBeatsAsDouble;

            
        }

        void OnDestroy()
        {
            /*if (!Conductor.instance.isPlaying || Conductor.instance.isPaused)
            {
                if (queuedInputs.Count > 0) queuedInputs.Clear();
            }*/
        }

        // Update is called once per frame
        void Update()
        {
            var cond = Conductor.instance;

            if (PlayerInput.GetIsAction(InputAction_BasicPress) && !IsExpectingInputNow(InputAction_BasicPress) && !IsExpectingInputNow(InputAction_Alt))
            {
                // Slap whiffs
                if (sumoStatePrevious == SumoState.Slap || sumoStatePrevious == SumoState.Idle)
                {

                    SoundByte.PlayOneShotGame("sumoBrothers/whiff");

                    if (lookingAtCamera) {
                        sumoBrotherPHead.DoScaledAnimationAsync("SumoPSlapLook");
                    } else {
                        sumoBrotherPHead.DoScaledAnimationAsync("SumoPSlap");
                    }

                
                    if (sumoSlapDir == 2) {
                    sumoBrotherP.DoScaledAnimationAsync("SumoSlapToStomp", 0.5f);
                    } else if (sumoSlapDir == 1) {
                    sumoBrotherP.DoScaledAnimationAsync("SumoSlapFront", 0.5f);
                    } else {
                    sumoBrotherP.DoScaledAnimationAsync("SumoSlapBack", 0.5f);
                    }
                }
                // Stomp whiffs
                if (sumoStatePrevious == SumoState.Stomp && !sumoBrotherP.IsPlayingAnimationNames("SumoStompMiss"))
                {
                    SoundByte.PlayOneShotGame("sumoBrothers/miss");

                    inuSensei.DoScaledAnimationAsync("InuFloatMiss", 0.5f);

                    sumoBrotherP.DoScaledAnimationAsync("SumoStompMiss", 0.5f);
                    sumoBrotherPHead.DoScaledAnimationAsync("SumoPMiss");
                }
            }

            StompShake();
        }

        public override void OnGameSwitch(double beat) // stole code from manzai
            {
            foreach(var entity in GameManager.instance.Beatmap.Entities)
                {
                if(entity.beat > beat) //the list is sorted based on the beat of the entity, so this should work fine.
                {
                    break;
                }
                if((entity.datamodel != "sumoBrothers/stompSignal" && entity.datamodel != "sumoBrothers/slapSignal") || entity.beat + entity.length < beat)
                {
                    continue;
                }
                bool isOnGameSwitchBeat = entity.beat == beat;
                if(entity.datamodel == "sumoBrothers/stompSignal")   {StompSignal(entity.beat, true, true, entity["look"], entity["StompDirection"]);}
                if(entity.datamodel == "sumoBrothers/slapSignal")   {SlapSignal(entity.beat, true, true);}
                }

                FindNextGameswitchBeat(beat);
            }

            public override void OnPlay(double beat)
        {
            FindNextGameswitchBeat(beat);
        }

            private void FindNextGameswitchBeat(double beat)
            {   // some of this code was made by astrl thanks
                var nextGameswitch = gameManager.Beatmap.Entities.Find(e => e.beat > beat && e.datamodel != "gameManager/switchGame/sumoBrothers" && e.datamodel.Length >= "gameManager/switchGame".Length && e.datamodel[..("gameManager/switchGame".Length)] == "gameManager/switchGame");
                if ( nextGameswitch != null) { nextGameswitchBeat = nextGameswitch.beat; } else { nextGameswitchBeat = double.MaxValue;}
                //print(nextGameswitchBeat);
            }
        
        public override void OnLateBeatPulse(double beat)
        {
                if (allowBopInu)
                {
                    if (goBopInu)
                    {
                        inuSensei.DoScaledAnimationAsync("InuBop", 0.5f);
                    }
                    else
                    {
                    //    inuSensei.DoScaledAnimationAsync("InuIdle", 0.5f);
                    }

                }

                if (allowBopSumo)
                {
                    if(goBopSumo)
                    {
                        BrosBop();
                    }
                    else
                    {
                    //    sumoBrotherP.DoScaledAnimationAsync("SumoIdle", 0.5f);
                    //    sumoBrotherG.DoScaledAnimationAsync("SumoIdle", 0.5f);
                    }
                    
                }

                //print("current sumo state: " + sumoState + " and previous sumo state: " + sumoStatePrevious);
                //print("sumo pose type: " + sumoPoseType);
        }

        public void Bop(double beat, float length, bool inu, bool sumo, bool inuAuto, bool sumoAuto)
        {
            goBopInu = inuAuto;
            goBopSumo = sumoAuto;

            if (inu || sumo)
            {
                List<BeatAction.Action> bops = new List<BeatAction.Action>();
                for (int i = 0; i < length; i++)
                {
                    bops.Add(new BeatAction.Action(beat + i, delegate
                    {
                        if (inu)
                        {
                            inuSensei.DoScaledAnimationAsync("InuBop", 0.5f);
                        }
                        if (sumo)
                        {
                            BrosBop();
                        }
                    }));
                }
                BeatAction.New(instance, bops);
            }

        }

        private void BrosBop()
        {
            if (sumoStatePrevious == SumoState.Idle) {
                sumoBrotherP.DoScaledAnimationAsync("SumoBop", 0.5f);
                sumoBrotherG.DoScaledAnimationAsync("SumoBop", 0.5f);
                sumoBrotherPHead.DoScaledAnimationAsync("SumoPIdle", 0.5f);
                sumoBrotherGHead.DoScaledAnimationAsync("SumoGIdle", 0.5f);
            } else if (sumoStatePrevious == SumoState.Pose) {
                sumoBrotherP.DoScaledAnimationAsync("SumoPosePBop" + sumoPoseTypeCurrent, 0.5f);
                sumoBrotherG.DoScaledAnimationAsync("SumoPoseGBop" + sumoPoseTypeCurrent, 0.5f);
            }

        }

        public void StompSignal(double beat, bool mute, bool inu, bool lookatcam, int startingDirection)
        {     
            if (sumoState == SumoState.Stomp || cueCurrentlyActive)
            {
                return;
            }

            CueRunning(beat + 3);
            // true = left, false = right
            // Automatic = 0, Left = 1, Right = 2
            /*if (startingDirection != 2)
            {
                sumoStompDir = true;
            } else {
                sumoStompDir = false;
            }*/
            

            if (lookatcam && sumoState == SumoState.Slap) {
                lookingAtCamera = true;
                sumoBrotherPHead.DoScaledAnimationAsync("SumoPSlapLook", 0.5f);
                sumoBrotherGHead.DoScaledAnimationAsync("SumoGSlapLook", 0.5f);

            }

            BeatAction.New(instance, new List<BeatAction.Action>()
                {
                new BeatAction.Action(beat + 3, delegate { allowBopSumo = false; })
                });

            if (inu && conductor.songPosBeat <= beat + 2)
            {
                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat, delegate { allowBopInu = false; }),
                    new BeatAction.Action(beat, delegate { inuSensei.DoScaledAnimationAsync("InuBeatChange", 0.5f); }),
                    new BeatAction.Action(beat + 2, delegate { inuSensei.DoScaledAnimationAsync("InuBeatChange", 0.5f); })
                });
            }

            if (mute == false) { StompSignalSound(beat); }

            sumoStatePrevious = sumoState;
            sumoState = SumoState.Stomp;

            int stompType = 1;
            bool startingLeftAfterTransition = false;

            if (startingDirection == 1)
            {
                startingLeftAfterTransition = true;
            }

            if (startingDirection == 2)
            {
                stompType = 2;
            }

            if (sumoStatePrevious == SumoState.Slap) {
                stompType = 3;
            } else if (sumoStatePrevious == SumoState.Pose) {
                stompType = 4;
            }

            StompRecursive(beat + 3, 1, stompType, startingLeftAfterTransition);
            
        }

        public static void StompSignalSound(double beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
                {
                    new MultiSound.Sound("sumoBrothers/stompsignal", beat),
                    new MultiSound.Sound("sumoBrothers/stompsignal", beat + 2f)
                }, forcePlay: true);
        }

        private void StompRecursive(double beat, double remaining, int type, bool startingLeftAfterTransition)
        {

            if (sumoState != SumoState.Stomp) { remaining -= 1; }
            if (beat >= nextGameswitchBeat - 1) { remaining = 0; }

            if (remaining <= 0) { return; }

            if (type == 3) { // Stomp Animation - Transition from Slapping to Stomping
                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat, delegate { sumoStompDir = true; }),
                    new BeatAction.Action(beat + 1, delegate { sumoStatePrevious = SumoState.Stomp; }),
                    new BeatAction.Action(beat + 1, delegate { lookingAtCamera = false; }),
                    new BeatAction.Action(beat + 1, delegate { sumoBrotherG.DoScaledAnimationAsync("SumoStompL", 0.5f); }),
                    new BeatAction.Action(beat + 1, delegate { sumoBrotherGHead.DoScaledAnimationAsync("SumoGStomp", 0.5f); }),
                    new BeatAction.Action(beat + 1, delegate { if (sumoState == SumoState.Stomp && !inuSensei.IsPlayingAnimationNames("InuFloatMiss")) {inuSensei.DoScaledAnimationAsync("InuFloat", 0.5f);} })                 
                });
            } else if (type == 4) { // Stomp Animation - Transition from Posing to Stomping
                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat, delegate { sumoStompDir = true; }),
                    new BeatAction.Action(beat, delegate { sumoBrotherP.DoScaledAnimationAsync("SumoPoseSwitch",0.5f); }),
                    new BeatAction.Action(beat, delegate { sumoBrotherG.DoScaledAnimationAsync("SumoPoseSwitch",0.5f); }),
                    new BeatAction.Action(beat, delegate { sumoBrotherPHead.DoScaledAnimationAsync("SumoPStomp",0.5f); }),
                    new BeatAction.Action(beat, delegate { sumoBrotherGHead.DoScaledAnimationAsync("SumoGStomp",0.5f); }),
                    new BeatAction.Action(beat, delegate { bgStatic.DoScaledAnimationAsync("empty", 0.5f); }),
                    new BeatAction.Action(beat, delegate { glasses.DoScaledAnimationAsync("glassesGone", 0.5f); }),
                    new BeatAction.Action(beat, delegate { bgType = BGType.None; }),
                    new BeatAction.Action(beat + 1, delegate { sumoStatePrevious = SumoState.Stomp; }),
                    new BeatAction.Action(beat + 1, delegate { sumoBrotherG.DoScaledAnimationAsync("SumoStompR", 0.5f); }),
                    new BeatAction.Action(beat + 1, delegate { sumoBrotherGHead.DoScaledAnimationAsync("SumoGStomp", 0.5f); }),
                    new BeatAction.Action(beat + 1, delegate { if (sumoState == SumoState.Stomp && !inuSensei.IsPlayingAnimationNames("InuFloatMiss")) {inuSensei.DoScaledAnimationAsync("InuFloat", 0.5f);} })                 
                });
            } else if (type == 1) { // Stomp Animation - Left Stomp
                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat, delegate { sumoStatePrevious = SumoState.Stomp; }),
                    new BeatAction.Action(beat, delegate { sumoStompDir = true; }),
                    new BeatAction.Action(beat, delegate { sumoBrotherP.DoScaledAnimationAsync("SumoStompPrepareL", 0.5f); }),
                    new BeatAction.Action(beat, delegate { sumoBrotherG.DoScaledAnimationAsync("SumoStompPrepareR", 0.5f); }),
                    new BeatAction.Action(beat, delegate { sumoBrotherPHead.DoScaledAnimationAsync("SumoPStomp", 0.5f); }),
                    new BeatAction.Action(beat, delegate { sumoBrotherGHead.DoScaledAnimationAsync("SumoGStomp", 0.5f); }),
                    new BeatAction.Action(beat + 1, delegate { sumoBrotherG.DoScaledAnimationAsync("SumoStompR", 0.5f); }),   
                    new BeatAction.Action(beat + 1, delegate { if (sumoState == SumoState.Stomp && !inuSensei.IsPlayingAnimationNames("InuFloatMiss")) {inuSensei.DoScaledAnimationAsync("InuFloat", 0.5f);} })             
                });
            } else if (type == 2) { // Stomp Animation - Right Stomp
                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat, delegate { sumoStatePrevious = SumoState.Stomp; }),
                    new BeatAction.Action(beat, delegate { sumoStompDir = false; }),
                    new BeatAction.Action(beat, delegate { sumoBrotherP.DoScaledAnimationAsync("SumoStompPrepareR", 0.5f); }),
                    new BeatAction.Action(beat, delegate { sumoBrotherG.DoScaledAnimationAsync("SumoStompPrepareL", 0.5f); }),
                    new BeatAction.Action(beat, delegate { sumoBrotherPHead.DoScaledAnimationAsync("SumoPStomp", 0.5f); }),
                    new BeatAction.Action(beat, delegate { sumoBrotherGHead.DoScaledAnimationAsync("SumoGStomp", 0.5f); }),
                    new BeatAction.Action(beat + 1, delegate { sumoBrotherG.DoScaledAnimationAsync("SumoStompL", 0.5f); }),
                    new BeatAction.Action(beat + 1, delegate { if (sumoState == SumoState.Stomp && !inuSensei.IsPlayingAnimationNames("InuFloatMiss")) {inuSensei.DoScaledAnimationAsync("InuFloat", 0.5f);} })                       
                });
            }

            if (type == 2 || startingLeftAfterTransition == true) {type = 1;} else { type = 2; }
        

        var stompInput = ScheduleInput(beat , 1, InputAction_BasicPress, StompHit, StompMiss, Nothing);
            BeatAction.New(instance, new List<BeatAction.Action>()
                {
                new BeatAction.Action(beat, delegate { StompRecursive(beat + 2, remaining, type, false); })
                });

            stompInput.IsHittable = () => {
                        return !sumoBrotherP.IsPlayingAnimationNames("SumoStompMiss");
                    };
                


            //print("sumo stomp dir: " + sumoStompDir);
            
        }

        public void SlapSignal(double beat, bool mute, bool inu)
        {
            if (sumoState == SumoState.Slap || cueCurrentlyActive)
            {
                return;
            }

            CueRunning(beat + 3);

            sumoSlapDir = 0;

            BeatAction.New(instance, new List<BeatAction.Action>()
                {
                new BeatAction.Action(beat + 3, delegate { allowBopSumo = false; }),
                new BeatAction.Action(beat + 3, delegate { sumoBrotherP.DoScaledAnimationAsync("SumoSlapPrepare",0.5f); }),
                new BeatAction.Action(beat + 3, delegate { sumoBrotherG.DoScaledAnimationAsync("SumoSlapPrepare", 0.5f); }),
                new BeatAction.Action(beat + 3, delegate { sumoBrotherPHead.DoScaledAnimationAsync("SumoPSlap", 0.5f); }),
                new BeatAction.Action(beat + 3, delegate { sumoBrotherGHead.DoScaledAnimationAsync("SumoGSlap", 0.5f); }),
                new BeatAction.Action(beat + 3, delegate { bgStatic.DoScaledAnimationAsync("empty", 0.5f); }),
                new BeatAction.Action(beat + 3, delegate { glasses.DoScaledAnimationAsync("glassesGone", 0.5f); }),
                new BeatAction.Action(beat + 3, delegate { bgType = BGType.None; }),
                new BeatAction.Action(beat + 3, delegate { if (sumoStatePrevious == SumoState.Pose) sumoBrotherP.DoScaledAnimationAsync("SumoPoseSwitch",0.5f); }),
                new BeatAction.Action(beat + 3, delegate { if (sumoStatePrevious == SumoState.Pose) sumoBrotherG.DoScaledAnimationAsync("SumoPoseSwitch",0.5f); }),
                new BeatAction.Action(beat + 3, delegate { if (sumoStatePrevious == SumoState.Pose) sumoBrotherPHead.DoScaledAnimationAsync("SumoPStomp",0.5f); }),
                new BeatAction.Action(beat + 3, delegate { if (sumoStatePrevious == SumoState.Pose) sumoBrotherGHead.DoScaledAnimationAsync("SumoGStomp",0.5f); })
                });

            if (inu  && conductor.songPosBeat <= beat + 3)
            {
                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                new BeatAction.Action(beat, delegate { allowBopInu = false; }),
                new BeatAction.Action(beat, delegate { inuSensei.DoScaledAnimationAsync("InuBeatChange", 0.5f); }),
                new BeatAction.Action(beat + 1, delegate { inuSensei.DoScaledAnimationAsync("InuBeatChange", 0.5f); }),
                new BeatAction.Action(beat + 2, delegate { inuSensei.DoScaledAnimationAsync("InuBeatChange", 0.5f); }),
                new BeatAction.Action(beat + 3, delegate { inuSensei.DoScaledAnimationAsync("InuBeatChange", 0.5f); }),
                new BeatAction.Action(beat + 3, delegate { allowBopInu = true; })
                });
            }

            if (mute == false) { SlapSignalSound(beat); }

            sumoStatePrevious = sumoState;
            sumoState = SumoState.Slap;
            SlapRecursive(beat + 4, 4);

            BeatAction.New(instance, new List<BeatAction.Action>()
                {
                new BeatAction.Action(beat + 4, delegate { sumoStatePrevious = SumoState.Slap; })
                });

        }

        public static void SlapSignalSound(double beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
                {
                new MultiSound.Sound("sumoBrothers/slapsignal", beat),
                new MultiSound.Sound("sumoBrothers/slapsignal", beat + 1f),
                new MultiSound.Sound("sumoBrothers/slapsignal", beat + 2f),
                new MultiSound.Sound("sumoBrothers/slapsignal", beat + 3f)
                }, forcePlay: true);
        }

        private void SlapRecursive(double beat, double remaining)
        {

            if (sumoState != SumoState.Slap) {remaining -= 1; }

            if (remaining <= 0) { return; }

            if (remaining == 1 && sumoState == SumoState.Stomp) {
                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                new BeatAction.Action(beat - 0.5, delegate { sumoSlapDir = 2; })
                });
                }

            ScheduleInput(beat - 1, 1, InputAction_BasicPress, SlapHit, SlapMiss, Nothing);
            if (beat >= nextGameswitchBeat - 1) { remaining = 0; }
            BeatAction.New(instance, new List<BeatAction.Action>()
                {
                new BeatAction.Action(beat, delegate { SlapRecursive(beat + 1, remaining); })
                });
            
            
            
            //print("the sumo slap direction:" + sumoSlapDir);
        }

        public void Crouch(double beat, float length, bool inu, bool sumo)
        {
            if (sumoStatePrevious == SumoState.Idle) {
                
                if (inu) { allowBopInu = false; inuSensei.DoScaledAnimationAsync("InuCrouch", 0.5f); }
                if (sumo) { sumoBrotherP.DoScaledAnimationAsync("SumoCrouch", 0.5f); sumoBrotherG.DoScaledAnimationAsync("SumoCrouch", 0.5f); 
                            allowBopSumo = false;}
                            BeatAction.New(instance, new List<BeatAction.Action>() {
            new BeatAction.Action(beat + length, delegate { if (sumoStatePrevious == SumoState.Idle) allowBopInu = true; }),
            new BeatAction.Action(beat + length, delegate { if (sumoStatePrevious == SumoState.Idle) allowBopSumo = true; }),
            new BeatAction.Action(beat + length, delegate { if (goBopSumo && sumoStatePrevious == SumoState.Idle) BrosBop(); }),
            new BeatAction.Action(beat + length, delegate { if (goBopInu && sumoStatePrevious == SumoState.Idle) inuSensei.DoScaledAnimationAsync("InuBop", 0.5f); })
            });
            }
        }

        public void LookAtCamera(double beat, float length)
        {
            if (sumoState == SumoState.Slap) {
            BeatAction.New(instance, new List<BeatAction.Action>() {
            new BeatAction.Action(beat, delegate { lookingAtCamera = true; }),
            new BeatAction.Action(beat, delegate { sumoBrotherPHead.DoScaledAnimationAsync("SumoPSlapLook", 0.5f); }),
            new BeatAction.Action(beat, delegate { sumoBrotherGHead.DoScaledAnimationAsync("SumoGSlapLook", 0.5f); }),
            //new BeatAction.Action(beat, delegate { print("look"); }),
            new BeatAction.Action(beat + length, delegate { lookingAtCamera = false; }),
            new BeatAction.Action(beat + length, delegate { if (sumoState == SumoState.Slap) sumoBrotherPHead.DoScaledAnimationAsync("SumoPSlap", 0.5f); }),
            new BeatAction.Action(beat + length, delegate { if (sumoState == SumoState.Slap) sumoBrotherGHead.DoScaledAnimationAsync("SumoGSlap", 0.5f); }),
            //new BeatAction.Action(beat + length, delegate { print("lookun"); })

            });
            }
        }

        public void EndPose(double beat, bool randomPose, int poseType, int backgroundType, bool confetti, bool alternateBG, bool throwGlasses)
        {
            if (cueCurrentlyActive)
            { return; }

            CueRunning(beat + 3.5);
            sumoStatePrevious = sumoState;
            sumoState = SumoState.Pose;

            if (sumoPoseTypeNext > 0 & sumoPoseTypeNext < 4 & randomPose) {
                poseType = UnityEngine.Random.Range(1, 3);
                if (poseType >= sumoPoseTypeNext) poseType++;
            } else if (randomPose) {
                poseType = UnityEngine.Random.Range(1, 4);
            }

            if (alternateBG) {
                if (bgTypeNext != BGType.GreatWave) { backgroundType = 0; } else {
                backgroundType = 1; }
                
            }

            if (!throwGlasses & poseType == 4) {
                poseType = 5;
            }

            var cond = Conductor.instance;

            ScheduleInput(beat, 4f, InputAction_Alt, PoseHit, PoseMiss, Nothing);

            var tweet = SoundByte.PlayOneShotGame("sumoBrothers/posesignal", -1, 1f, 1f, true);
            tweet.SetLoopParams(beat + 3, 0.05f);

            BeatAction.New(instance, new List<BeatAction.Action>() {
                new BeatAction.Action(beat, delegate { allowBopInu = false; }),
                new BeatAction.Action(beat, delegate { inuSensei.DoScaledAnimationAsync("InuAlarm", 0.5f); }),
                new BeatAction.Action(beat + 3, delegate { allowBopInu = true; }),
                new BeatAction.Action(beat + 3, delegate { inuSensei.DoScaledAnimationAsync("InuIdle", 0.5f);; }),
                new BeatAction.Action(beat + 3, delegate { if (goBopInu == true) inuSensei.DoScaledAnimationAsync("InuBop", 0.5f); }),
                new BeatAction.Action(beat + 3.5, delegate { allowBopSumo = false; }),
                new BeatAction.Action(beat + 3.5, delegate { sumoPoseTypeNext = poseType; }),
                new BeatAction.Action(beat + 3.5, delegate { bgTypeNext = (BGType)backgroundType; }),
                new BeatAction.Action(beat + 4, delegate { sumoStatePrevious = SumoState.Pose; }),
                new BeatAction.Action(beat + 4.5, delegate { allowBopSumo = true; })
            });

        }

        public void CueRunning(double beat)
        {
            cueCurrentlyActive = true;
            cueCurrentlyActiveBeat = beat;

            BeatAction.New(instance, new List<BeatAction.Action>()
                {
                new BeatAction.Action(beat, delegate { cueCurrentlyActive = false; })
                });
        }


        void PoseHit(PlayerActionEvent caller, float state)
        {
            sumoPoseTypeCurrent = sumoPoseTypeNext.ToString();
            sumoPoseType = sumoPoseTypeNext;

            if (sumoPoseType == 4) { glasses.DoScaledAnimationAsync("glassesThrow", 0.5f); }

            if (sumoPoseType == 5)
            {
                sumoBrotherG.DoScaledAnimationAsync("SumoPoseG4Alt", 0.5f);
                sumoBrotherGHead.DoScaledAnimationAsync("SumoGPoseAlt4", 0.5f);
                sumoPoseType = 4;
                sumoPoseTypeNext = 4;
                sumoPoseTypeCurrent = "4";

            } else {
            sumoBrotherG.DoScaledAnimationAsync("SumoPoseG" + sumoPoseTypeCurrent, 0.5f);
            sumoBrotherGHead.DoScaledAnimationAsync("SumoGPose" + sumoPoseTypeCurrent, 0.5f);
            }

            sumoBrotherP.DoScaledAnimationAsync("SumoPoseP" + sumoPoseTypeCurrent, 0.5f);

            bgStatic.DoScaledAnimationAsync($"{bgType.ToString()}Dark", 0.5f);


            bgType = bgTypeNext;

            if (bgType == BGType.Nerd)
            {
                SoundByte.PlayOneShotGame("sumoBrothers/Goofy");
            }

            var beat = conductor.songPosBeat;

            BeatAction.New(instance, new List<BeatAction.Action>()
            {
            new BeatAction.Action(beat, delegate { bgMove.DoScaledAnimationAsync(bgType.ToString(), 0.5f); }),
            new BeatAction.Action(beat + 2, delegate { bgMove.DoScaledAnimationAsync("empty", 0.5f); }),
            new BeatAction.Action(beat + 2, delegate { bgStatic.DoScaledAnimationAsync(bgType.ToString(), 0.5f); }),
            });
            

            if (state >= 1f || state <= -1f)
            {
                SoundByte.PlayOneShotGame("sumoBrothers/tink");
                sumoBrotherPHead.DoScaledAnimationAsync("SumoPPoseBarely" + sumoPoseTypeCurrent, 0.5f);
                // Dust is not meant to show up on a barely, need to somehow code that eventually
                dust.DoScaledAnimationAsync("dustGone", 0.5f);
            }
            else
            {
                sumoBrotherPHead.DoScaledAnimationAsync("SumoPPose" + sumoPoseTypeCurrent, 0.5f);
            }
            SoundByte.PlayOneShotGame("sumoBrothers/pose");

        }

        void PoseMiss(PlayerActionEvent caller)
        {
            SoundByte.PlayOneShotGame("sumoBrothers/miss");

            if (sumoPoseTypeNext == 5) { sumoPoseTypeNext = 4; }

            sumoPoseType = sumoPoseTypeNext;
            sumoPoseTypeCurrent = "Miss" + sumoPoseTypeNext.ToString();

            bgStatic.DoScaledAnimationAsync("empty", 0.5f);

            sumoBrotherPHead.DoScaledAnimationAsync("SumoPPose" + sumoPoseType.ToString(), 0.5f);
            sumoBrotherGHead.DoScaledAnimationAsync("SumoGPose" + sumoPoseType.ToString(), 0.5f);
            sumoBrotherP.DoScaledAnimationAsync("SumoPoseP" + sumoPoseTypeCurrent, 0.5f);
            sumoBrotherG.DoScaledAnimationAsync("SumoPoseG" + sumoPoseTypeCurrent, 0.5f);

            if (sumoPoseType == 4) { sumoBrotherGHead.DoScaledAnimationAsync("SumoGPoseAlt4", 0.5f); }
        }

        void SlapHit(PlayerActionEvent caller, float state)
        {
            if (state >= 1f || state <= -1f)
            {
                SoundByte.PlayOneShotGame("sumoBrothers/tink");

                if (lookingAtCamera) {
                    sumoBrotherPHead.DoScaledAnimationAsync("SumoPSlapLookBarely");
                    sumoBrotherGHead.DoScaledAnimationAsync("SumoGSlapLook", 0.5f);
                } else {
                    sumoBrotherPHead.DoScaledAnimationAsync("SumoPSlapBarely");
                    sumoBrotherGHead.DoScaledAnimationAsync("SumoGSlap", 0.5f);
                }

            }
            else
            {

                if (lookingAtCamera) {
                    sumoBrotherPHead.DoScaledAnimationAsync("SumoPSlapLook");
                    sumoBrotherGHead.DoScaledAnimationAsync("SumoGSlapLook", 0.5f);
                } else {
                    sumoBrotherPHead.DoScaledAnimationAsync("SumoPSlap");
                    sumoBrotherGHead.DoScaledAnimationAsync("SumoGSlap", 0.5f);
                }

            }

            if (sumoSlapDir == 1) { sumoSlapDir = 0;}
            else if (sumoSlapDir == 0) { sumoSlapDir = 1;}

            SoundByte.PlayOneShotGame("sumoBrothers/slap");
            impact.DoScaledAnimationAsync("impact", 0.5f);
            
            if (sumoSlapDir == 2) {
                sumoBrotherP.DoScaledAnimationAsync("SumoSlapToStomp", 0.5f);
                sumoBrotherG.DoScaledAnimationAsync("SumoSlapToStomp", 0.5f);
            } else if (sumoSlapDir == 1) {
                sumoBrotherP.DoScaledAnimationAsync("SumoSlapFront", 0.5f);
                sumoBrotherG.DoScaledAnimationAsync("SumoSlapFront", 0.5f);
            } else {
                sumoBrotherP.DoScaledAnimationAsync("SumoSlapBack", 0.5f);
                sumoBrotherG.DoScaledAnimationAsync("SumoSlapBack", 0.5f);
            }


        }

        void SlapMiss(PlayerActionEvent caller)
        {

            if (sumoSlapDir == 1) { sumoSlapDir = 0;}
            else if (sumoSlapDir == 0) { sumoSlapDir = 1;}

            SoundByte.PlayOneShotGame("sumoBrothers/miss");
            if (sumoSlapDir == 2) {
                sumoBrotherG.DoScaledAnimationAsync("SumoSlapToStomp", 0.5f);
            } else if (sumoSlapDir == 1) {
                sumoBrotherG.DoScaledAnimationAsync("SumoSlapFront", 0.5f);
            } else {
                sumoBrotherG.DoScaledAnimationAsync("SumoSlapBack", 0.5f);
            }

            sumoBrotherP.DoScaledAnimationAsync("SumoSlapMiss", 0.5f);
            sumoBrotherPHead.DoScaledAnimationAsync("SumoPMiss");

            if (lookingAtCamera) {
                    sumoBrotherGHead.DoScaledAnimationAsync("SumoGSlapLook", 0.5f);
                } else {
                    sumoBrotherGHead.DoScaledAnimationAsync("SumoGSlap", 0.5f);
                }

            if (sumoState == SumoState.Slap) {
            inuSensei.DoScaledAnimationAsync("InuBopMiss", 0.5f);
            }


        }

        void StompHit(PlayerActionEvent caller, float state)
        {
            if (state >= 1f || state <= -1f)
            {
                SoundByte.PlayOneShotGame("sumoBrothers/tink");
                sumoBrotherPHead.DoScaledAnimationAsync("SumoPStompBarely", 0.5f);
            }
            else
            {
                sumoBrotherPHead.DoScaledAnimationAsync("SumoPStomp", 0.5f);
            }
            SoundByte.PlayOneShotGame("sumoBrothers/stomp");

            if (sumoStompDir) 
            {
                sumoBrotherP.DoScaledAnimationAsync("SumoStompL", 0.5f);
            } else {
                sumoBrotherP.DoScaledAnimationAsync("SumoStompR", 0.5f);
            }

            justStompBeat = Conductor.instance.songPositionInBeatsAsDouble;

            // Attempt at making the code better, abandoned because the damn lists didn't cooperate

            /*stompShakeTimings.Clear();
            stompShakeValues.Clear();

            for (int i = 0; i < 9; i++) 
            {
                double currBeat = stompShakeSpeed * i + justStompBeat;
                stompShakeTimings.Add(currBeat);
            }

            stompShakeValues.Add(-0.3f); // -0.3 0.3 -0.2 0.2 -0.1 0.1 -0.05 0
            stompShakeValues.Add(0.3f);
            stompShakeValues.Add(-0.2f);
            stompShakeValues.Add(0.2f);
            stompShakeValues.Add(-0.1f);
            stompShakeValues.Add(0.1f);
            stompShakeValues.Add(-0.05f);
            stompShakeValues.Add(0f);*/

            // yaaaay old crusty code

            BeatAction.New(instance, new List<BeatAction.Action>() {
                //new BeatAction.Action(justStompBeat, delegate { stompShakeLength = 0.13; }),
                new BeatAction.Action(justStompBeat, delegate { cameraXNew = -0.3f; }),
                //new BeatAction.Action(justStompBeat + 0.2, delegate { stompShakeLength = 0.25; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 1), delegate { justStompBeat += stompShakeSpeed; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 1), delegate { cameraX = cameraXNew; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 1), delegate { cameraXNew = 0.3f; }),
                //new BeatAction.Action(justStompBeat + 0.4, delegate { stompShakeLength = 0.25; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 2), delegate { justStompBeat += stompShakeSpeed; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 2), delegate { cameraX = cameraXNew; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 2), delegate { cameraXNew = -0.2f; }),
                //new BeatAction.Action(justStompBeat + 0.6, delegate { stompShakeLength = 0.25; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 3), delegate { justStompBeat += stompShakeSpeed; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 3), delegate { cameraX = cameraXNew; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 3), delegate { cameraXNew = 0.2f; }),
                //new BeatAction.Action(justStompBeat + 0.8, delegate { stompShakeLength = 0.12; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 4), delegate { justStompBeat += stompShakeSpeed; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 4), delegate { cameraX = cameraXNew; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 4), delegate { cameraXNew = -0.1f; }),
                //
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 5), delegate { justStompBeat += stompShakeSpeed; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 5), delegate { cameraX = cameraXNew; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 5), delegate { cameraXNew = 0.1f; }),
                //
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 6), delegate { justStompBeat += stompShakeSpeed; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 6), delegate { cameraX = cameraXNew; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 6), delegate { cameraXNew = -0.1f; }),
                //
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 7), delegate { justStompBeat += stompShakeSpeed; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 7), delegate { cameraX = cameraXNew; }),
                new BeatAction.Action(justStompBeat + (stompShakeSpeed * 7), delegate { cameraXNew = 0f; }),
            }); 
            
            /*BeatAction.New(instance, new List<BeatAction.Action>() {
                new BeatAction.Action(beat, delegate { allowBopInu = false; }),
                new BeatAction.Action(beat, delegate { inuSensei.DoScaledAnimationAsync("InuAlarm", 0.5f); }),
                new BeatAction.Action(beat + 3, delegate { allowBopInu = true; }),
                new BeatAction.Action(beat + 3, delegate { inuSensei.DoScaledAnimationAsync("InuIdle", 0.5f);; }),
                new BeatAction.Action(beat + 3, delegate { if (goBopInu == true) inuSensei.DoScaledAnimationAsync("InuBop", 0.5f); }),
                new BeatAction.Action(beat + 5, delegate { allowBopSumo = true; })
            });*/

            

        }

        void StompMiss(PlayerActionEvent caller)
        {
            if (!sumoBrotherP.IsPlayingAnimationNames("SumoStompMiss"))
            {
            SoundByte.PlayOneShotGame("sumoBrothers/miss");

            inuSensei.DoScaledAnimationAsync("InuFloatMiss", 0.5f);

            sumoBrotherP.DoScaledAnimationAsync("SumoStompMiss", 0.5f);
            sumoBrotherPHead.DoScaledAnimationAsync("SumoPMiss");
            }


        }

        void Nothing(PlayerActionEvent caller) { }

        void StompShake()
        {
            // Attempt at making the code better, abandoned because the damn lists didn't cooperate

            //var timings = stompShakeTimings.ToList();
            //var values = stompShakeValues.ToList();
            /*if (stompShakeTimings.Count == 0) { return; }

            if (justStompBeat >= stompShakeTimings[0] && -1f != stompShakeValues[0])
            {
                justStompBeat = stompShakeTimings[0];
                cameraX = cameraXNew;
                cameraXNew = stompShakeValues[0];
                stompShakeValues.RemoveAt(0);
                stompShakeTimings.RemoveAt(0);
                print(stompShakeTimings.Count + " " + stompShakeValues.Count);
                print("cX: " + cameraX + "  cXN: " + cameraXNew + "jSB: " + justStompBeat + "sST: " + stompShakeTimings + "sSV: " + stompShakeValues);
            }*/

            float normalizedBeat = Conductor.instance.GetPositionFromBeat(justStompBeat, stompShakeSpeed);

            if (1f >= normalizedBeat)
            {
                EasingFunction.Function func = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseInOutQuad);
                float newPosX = func(cameraX, cameraXNew, normalizedBeat);
                camera.position = new Vector3(newPosX, 0, 0);
                //print("cX: " + cameraX + "  cXN: " + cameraXNew + "nPX: " + newPosX + "sSL: " + stompShakeLength);
                //print("cX: " + cameraX + "  cXN: " + cameraXNew + "  jSB: " + justStompBeat + "  sST: " + stompShakeTimings + "  sSV: " + stompShakeValues);
            }
        }


    }
}
