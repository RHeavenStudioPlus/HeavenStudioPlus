using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;

using Jukebox;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    /// Minigame loaders handle the setup of your minigame.
    /// Here, you designate the game prefab, define entities, and mark what AssetBundle to load

    /// Names of minigame loaders follow a specific naming convention of `PlatformcodeNameLoader`, where:
    /// `Platformcode` is a three-leter platform code with the minigame's origin
    /// `Name` is a short internal name
    /// `Loader` is the string "Loader"

    /// Platform codes are as follows:
    /// Agb: Gameboy Advance    ("Advance Gameboy")
    /// Ntr: Nintendo DS        ("Nitro")
    /// Rvl: Nintendo Wii       ("Revolution")
    /// Ctr: Nintendo 3DS       ("Centrair")
    /// Mob: Mobile
    /// Pco: PC / Other

    /// Fill in the loader class label, "*prefab name*", and "*Display Name*" with the relevant information
    /// For help, feel free to reach out to us on our discord, in the #development channel.
    public static class AgbRhythmTestGBALoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("rhythmTestGBA", "Rhythm Test (GBA) \n<color=#adadad>(Rhythm-kan Check)</color>", "2DD816", false, false, new List<GameAction>()
            {

                new GameAction("countin", "Change Screen Beeping Properties")
                {
                    function = delegate { RhythmTestGBA.instance.KTBPrep(eventCaller.currentEntity.beat, eventCaller.currentEntity.length, eventCaller.currentEntity["toggle"], eventCaller.currentEntity["auto"],
                     eventCaller.currentEntity["image"],
                      eventCaller.currentEntity["textFlash"], eventCaller.currentEntity["textDisplay"], eventCaller.currentEntity["hasSound"]); },
                    defaultLength = 1f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("toggle", true, "Toggle", "Toggle the beeping on or off."),
                        new Param("auto", false, "Auto", "Toggle if the machine should beep automatically."),
                        new Param("image", RhythmTestGBA.PulseOption.Note, "Screen Image", "Set what appears on the machine's screen.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (int)x == (int)RhythmTestGBA.PulseOption.Text, new string[] { "textFlash", "textDisplay" }),
                        }),
                        new Param("textDisplay", "Get ready...", "Text to Display", "Changes the text displayed on the screen."),
                        new Param("textFlash", true, "Text Flash", "Toggle if the text on the screen pulses to the beat."),
                        new Param("hasSound", true, "Has Sound", "Toggle if the beeping plays sound or not.")
                        
                    },
                },

                new GameAction("button", "Start Keep-the-Beat")
                {
                    preFunction = delegate { var e = eventCaller.currentEntity; RhythmTestGBA.PreStartKeepbeat(e.beat, e.length); },
                    defaultLength = 1f,
                    resizable = false,

                },

                new GameAction("stopktb", "Stop Keep-the-Beat")
                {
                    preFunction = delegate { RhythmTestGBA.instance.PreStopKeepbeat(eventCaller.currentEntity.beat, eventCaller.currentEntity.length, eventCaller.currentEntity["mutecue"], eventCaller.currentEntity["finishText"], eventCaller.currentEntity["textDisplay"]); },
                    defaultLength = 4f,
                    resizable = false,
                    parameters = new List<Param>()
                    {
                        new Param("mutecue", false, "Mute Cue", "Mute the sound cue signifying the end of the keep-the-beat section."),
                        new Param("finishText", true, "Finish Text", "Set if text appears once the keep-the-bet section is finished.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "textDisplay" }),
                        }),
                        new Param("textDisplay", "Test complete!", "Text to Display", "Changes the text displayed on the screen."),
                    }
                },

                new GameAction("countdown", "Countdown")
                {
                    function = delegate {RhythmTestGBA.instance.PreCountDown(eventCaller.currentEntity.beat, eventCaller.currentEntity.length, eventCaller.currentEntity["val1"]);},
                    defaultLength = 1f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("val1", new EntityTypes.Integer(1, 9, 3), "Beats", "Set how many beats there will be before the player has to input.")                        
                    }
                },

                new GameAction("hidecount", "Toggle Countdown")
                {
                    function = delegate {RhythmTestGBA.instance.HideCountdown(eventCaller.currentEntity["togglecount"]);},
                    defaultLength = 0.5f,
                    resizable = false,
                    parameters = new List<Param>()
                    {
                        new Param("togglecount", true, "Toggle Countdown?", "Toggles whether the countdown appears or not."),
                    }

                },

            },
            new List<string>() {"agb", "aim"},
            "agbRhythmTestGBA", "en",
            new List<string>() { "en" },
            chronologicalSortKey: 0
            );
        }
    }
}

namespace HeavenStudio.Games
{
    /// This class handles the minigame logic.
    /// Minigame inherits directly from MonoBehaviour, and adds Heaven Studio specific methods to override.
    using Scripts_RhythmTestGBA;
    public class RhythmTestGBA : Minigame
    {
        public static RhythmTestGBA instance;
        static List<double> queuedButton = new();
        
        bool goBeep;
        bool stopAutoBeep;
        bool keepPressing;
        bool shouldmute;
        bool disableCount;
        bool beatPulseTextFlash = false;
        bool beepHasSound;
        bool canBeep = false;
        //bool isBeeping = false;

        int screenFXType;

        private double numberSelect;
        private float countLength;

        [NonSerialized] public List<Interval> noBopIntervals = new(),
                                              noBeepIntervals = new();

        [Header("Objects")]
        [SerializeField] GameObject noteFlash;
        [SerializeField] TMP_Text screenText;

        [Header("Animators")]
        [SerializeField] Animator buttonAnimator;
        [SerializeField] Animator flashAnimator;
        [SerializeField] Animator numberBGAnimator;
        [SerializeField] Animator numberAnimator;
        [SerializeField] Animator textAnimator;

        [Header("Properties")]
        //private static double startBlippingBeat = double.MaxValue;    Unused value - Marc

        [Header("Variables")]
        
        //int pressPlayerCount;    Unused value - Marc
        public static double wantButton = double.MinValue;

        GameEvent button = new GameEvent();

        double lastButton = double.MaxValue;

        public enum PulseOption
        {
            Note,
            Text
        }

        public enum BeepNoise: int
        {
            defaultBeep = 0,
            highBeep = 1,
            dingBeep = 2
        }

        public int beepType = 0;
        //public struct QueuedButton
        //{
        //    public double beat;
        //    public float length;
        //}
        //static List<QueuedButton> queuedButton = new List<QueuedButton>();

        private void Awake()
        {
            instance = this;
            screenText.text = "";
            SetupBopRegion("rhythmTestGBA", "countin", "auto");
            var currentBeat = Conductor.instance.unswungSongPositionInBeatsAsDouble;
            KeepTheBeep(currentBeat, 1f, false, false, 0, true);
            HandleBeeps();
        }

        void OnDestroy()
        {
            if (queuedButton.Count > 0) queuedButton.Clear();
            foreach (var ktb in scheduledInputs)
            {
                ktb.Disable();
            }
        }
    

        private void Update()
        {
            var cond = Conductor.instance;

            if (PlayerInput.GetIsAction(InputAction_BasicPress) && !IsExpectingInputNow(InputAction_BasicPress))
            {
                PressButton();
                //print("unexpected input");
            }

            if (wantButton != double.MinValue)
            {
                queuedButton.Add(wantButton);
                keepPressing = true;
                //pressPlayerCount = 0;    Unused value - Marc
                wantButton = double.MinValue;
            }

            if (Conductor.instance.isPlaying && !Conductor.instance.isPaused)
            {            

                if (queuedButton.Count > 0)
                {
                     
                    foreach (var ktb in queuedButton)
                    {
                        BeatAction.New(instance, new List<BeatAction.Action>() {
                            new BeatAction.Action(ktb, delegate {
                                ScheduleInput(ktb, 1f, InputAction_BasicPress, ButtonSuccess, ButtonFailure, ButtonEmpty);

                            }),
                            new BeatAction.Action(ktb + 1, delegate {
                               if (keepPressing) queuedButton.Add(ktb + 1);
                            }),
                        });
                    }
                        queuedButton.Clear();
                }  
            }      

            if (lastButton + 1 <= cond.songPositionInBeatsAsDouble)
            {
            lastButton++;
                ScheduleInput(lastButton, 1, InputAction_BasicPress, ButtonSuccess, ButtonFailure, ButtonEmpty);

            }
        }

        //public override void OnGameSwitch (double beat)
        //{

        //    var actions = GameManager.instance.Beatmap.Entities.FindAll(e => e.datamodel.Split('/')[0] == "rhythmTestGBA");
        //    var tempStops = actions.FindAll(e => e.datamodel == "rhythmTestGBA/stopktb");

        //    foreach (var e in tempStops.FindAll(e => e.beat < beat && e.beat + 2 > beat)) {
        //        PreStopKeepbeat(e.beat, e.length, e["mutecue"], e["textFlash"]);
        //    }
        //}

        private void HandleBeeps()
        {
            List<RiqEntity> events = EventCaller.GetAllInGameManagerList("rhythmTestGBA", new string[] { "countin" });

            foreach (var e in events)
            {
                noBopIntervals.Add(new Interval(e.beat, e.beat + 1));
            }

            foreach (var v in events)
            {
                noBeepIntervals.Add(new Interval(v.beat, v.beat + v.length));
            }
        }

        public void KTBPrep(double beat, float length, bool shouldBeep, bool autoBeep, int type, bool textFlash, string textDisplay, bool hasSound)
        {
            canBeep = true;
            if (type == 1)
            {
                screenText.text = textDisplay;
            }
            beepType = 0;
            screenFXType = type;
            beepHasSound = hasSound;
            KeepTheBeep(beat, length, shouldBeep, autoBeep, type, textFlash);
        }

        public void KeepTheBeep(double beat, float length, bool shouldBeep, bool autoBeep, int type, bool textFlash)
        {
        
            beatPulseTextFlash = textFlash;
            if (beepHasSound)
            {
                switch (beepType)
                {
                    case (int)BeepNoise.defaultBeep:
                        SoundByte.PlayOneShotGame("rhythmTestGBA/blip");
                        break;
                    case (int)BeepNoise.highBeep:
                        SoundByte.PlayOneShotGame("rhythmTestGBA/blip2");
                        break;
                    case (int)BeepNoise.dingBeep:
                        SoundByte.PlayOneShotGame("rhythmTestGBA/end_ding");
                        break;
                }}
            
            if (shouldBeep)
            {
                PlayFakeFlashFX(type, textFlash);
                List<BeatAction.Action> beeps = new List<BeatAction.Action>();
                for (int i = 0; i < length; i++)
                {
                    beeps.Add(new BeatAction.Action(beat + i, delegate { 
                        PlayFlashFX();}
                
                ));
                }
                BeatAction.New(instance, beeps);
            }
        }
            
            
            //goBeep = autoBeep;
            //if (autoBeep)
            //{
           //     PlayFlashFX(beatPulseTextFlash, type);
             //   SoundByte.PlayOneShotGame("rhythmTestGBA/blip");
            //}
            
           // if (shouldBeep && !autoBeep)
           // {
                
                //if (!isBeeping)
                //    {
                //        PlayFlashFX(beatPulseTextFlash, type);
                //        SoundByte.PlayOneShotGame("rhythmTestGBA/blip");
                //        isBeeping = true;
                //    }
            //    for (int i = 0; i < length; i++)
            //    {
            //        BeatAction.New(instance, new List<BeatAction.Action>()
             //           {
            //                new BeatAction.Action(beat + i, delegate
            //            {
            //                PlayFlashFX(beatPulseTextFlash, type);

            //                SoundByte.PlayOneShotGame("rhythmTestGBA/blip");
            //            })
            //        });
                
            //    }
        //    }
    //    }

        public void PlayFlashFX()
        {
            numberAnimator.Play("Idle");
            numberBGAnimator.Play("Idle");
            var currentBeat = Conductor.instance.songPositionInBeatsAsDouble;
            if (!noBopIntervals.Any(x => x.Contains(currentBeat)))
            
            if (beepHasSound){
            if(!noBeepIntervals.Any(x => x.Contains(currentBeat)))
            {
            {
                    switch (beepType)
                {
                    case (int)BeepNoise.defaultBeep:
                        SoundByte.PlayOneShotGame("rhythmTestGBA/blip");
                        break;
                    case (int)BeepNoise.highBeep:
                        SoundByte.PlayOneShotGame("rhythmTestGBA/blip2");
                        break;
                    case (int)BeepNoise.dingBeep:
                        SoundByte.PlayOneShotGame("rhythmTestGBA/end_ding");
                        break;
                }
            }
            }}
            if (screenFXType == 0)
            {
            
            noteFlash.SetActive(true);
            flashAnimator.DoScaledAnimationAsync("KTBPulse", 0.5f);
            screenText.text = "";
            }
            else if (beatPulseTextFlash){
            
            noteFlash.SetActive(false);
            textAnimator.DoScaledAnimationAsync("TextFlash", 0.5f);
            }
            else
            {
            
            noteFlash.SetActive(false);
            textAnimator.DoScaledAnimationAsync("TextIdle", 0.5f);
            }

            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(currentBeat+0.9f, delegate {noteFlash.SetActive(false);})
            });
        }

        public void PlayFakeFlashFX(int fakeFXType, bool textFlash)
        {
            numberAnimator.Play("Idle");
            numberBGAnimator.Play("Idle");
            
            var currentBeat = Conductor.instance.songPositionInBeatsAsDouble;
            if (fakeFXType == 0)
            {
            
            noteFlash.SetActive(true);
            flashAnimator.DoScaledAnimationAsync("KTBPulse", 0.5f);
            screenText.text = "";
            
            
            }
            else if (textFlash){
            
            noteFlash.SetActive(false);
            textAnimator.DoScaledAnimationAsync("TextFlash", 0.5f);
            }
            else
            {
            
            noteFlash.SetActive(false);
            textAnimator.DoScaledAnimationAsync("TextIdle", 0.5f);
            }

            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                //new BeatAction.Action(beat-1, delegate {KillBeeps(beat);}),
                new BeatAction.Action(currentBeat+0.9f, delegate {noteFlash.SetActive(false);})
            });
        }
        

        public void ChangeText(double beat, string newText, bool newTextFlash)
        {
            screenText.text = newText;
            beatPulseTextFlash = newTextFlash;
        }


        public void PressButton()
        {
            SoundByte.PlayOneShotGame("rhythmTestGBA/press");

            buttonAnimator.DoScaledAnimationAsync("Press", 0.5f);

        }

        public void PreStopKeepbeat(double beat, float length, bool muted, bool hasFinish, string finishText)
        {
            noBeepIntervals.Add(new Interval(beat, beat + length));
            

            shouldmute = muted;
            noBeepIntervals.Add(new Interval(beat, beat + length));
            
            noBeepIntervals.Add(new Interval(beat, beat + length));
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                
                new BeatAction.Action(beat, delegate {StopKeepbeat(beat, length, shouldmute, hasFinish, finishText);
                canBeep = false;
                })
                
            });
            
            

        }        


        public void StopKeepbeat(double beat, float length,  bool shouldmute, bool hasFinish, string finishText)
        {
            keepPressing = false;
            
            ScheduleInput(beat, 1f, InputAction_BasicPress, ButtonSuccess, ButtonFailure, ButtonEmpty);
            ScheduleInput(beat, 2f, InputAction_BasicPress, ButtonSuccess, ButtonFailure, ButtonEmpty);
            ScheduleInput(beat, 3f, InputAction_BasicPress, ButtonSuccess, ButtonFailure, ButtonEmpty);
            

            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                
                new BeatAction.Action(beat, delegate {PlayFlashFX();}),  
                
                new BeatAction.Action(beat+1, delegate {PlayFlashFX();}),                 
  
                new BeatAction.Action(beat+2, delegate {PlayFlashFX();}), 



                new BeatAction.Action(beat+3, delegate { SoundByte.PlayOneShotGame("rhythmTestGBA/end_ding", beat: beat, forcePlay: true);
                
                if (hasFinish)
                {
                    screenText.text = finishText;
                    textAnimator.DoScaledAnimationAsync("TextIdle", 0.5f);
                }
                
                })

            });

            if (!shouldmute)
            {
                MultiSound.Play(new MultiSound.Sound[] {
                        new MultiSound.Sound("rhythmTestGBA/blip2", beat),
                        new MultiSound.Sound("rhythmTestGBA/blip2", beat+1),
                        new MultiSound.Sound("rhythmTestGBA/blip2", beat+2),
                        
            });
            }
          
            
            
        }

        public override void OnBeatPulse(double beat)
        {
             if (BeatIsInBopRegion(beat) && canBeep)
            {
                PlayFlashFX();
            }
            
        }

        //public void KillBeeps(double beat)
        //{
        //    goBeep = false;
            //isBeeping = false;
        //}

        public static void PreStartKeepbeat(double beat, float length)
        {
            if (GameManager.instance.currentGame == "rhythmTestGBA")
            {
                StartKeepbeat(beat, length);
            }
            else
            {
                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                
                    new BeatAction.Action(beat, delegate {queuedButton.Add(wantButton);}),
                }
                );
            }
        }


        


        public static void StartKeepbeat(double beat, float length)
        {
            RhythmTestGBA.wantButton = beat-1;

            
        }



        public void PreCountDown(double startBeat, float length, int countdownNumber)
        {
            if (keepPressing) return;
            ScheduleInput(startBeat, length * countdownNumber, InputAction_BasicPress, ButtonSuccess, ButtonFailure, ButtonEmpty);
            countLength = length;
            switch (countdownNumber)
            {
                case 1:
                    CountOne(startBeat, length);
                    break;
                case 2:
                    CountTwo(startBeat, length);
                    break;
                case 3:
                    CountThree(startBeat, length);
                    break;
                case 4:
                    CountFour(startBeat, length);
                    break;
                case 5:
                    CountFive(startBeat, length);
                    break;
                case 6:
                    CountSix(startBeat, length);
                    break;
                case 7:
                    CountSeven(startBeat, length);
                    break;
                case 8:
                    CountEight(startBeat, length);
                    break;
                case 9:
                    CountNine(startBeat, length);
                    break;
            }
        }

// Countdown playing functions

        public void CountNine(double startBeat, float length)
        {
            
            BeatAction.New(instance, new List<BeatAction.Action>()
            {

                new BeatAction.Action(startBeat, delegate {FlashNine(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length, delegate {FlashEight(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*2, delegate {FlashSeven(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*3, delegate {FlashSix(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*4, delegate {FlashFive(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*5, delegate {FlashFour(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*6, delegate {FlashThree(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*7, delegate {FlashTwo(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*8, delegate {FlashOne(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*9, delegate {FlashZero(startBeat);})
            });
        }


        public void CountEight(double startBeat, float length)
        {
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat, delegate {FlashEight(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length, delegate {FlashSeven(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*2, delegate {FlashSix(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*3, delegate {FlashFive(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*4, delegate {FlashFour(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*5, delegate {FlashThree(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*6, delegate {FlashTwo(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*7, delegate {FlashOne(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*8, delegate {FlashZero(startBeat);})
            });

        }        
        public void CountSeven(double startBeat, float length)
        {
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat, delegate {FlashSeven(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length, delegate {FlashSix(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*2, delegate {FlashFive(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*3, delegate {FlashFour(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*4, delegate {FlashThree(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*5, delegate {FlashTwo(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*6, delegate {FlashOne(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*7, delegate {FlashZero(startBeat);})
            });
        }
        public void CountSix(double startBeat, float length)
        {
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat, delegate {FlashSix(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length, delegate {FlashFive(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*2, delegate {FlashFour(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*3, delegate {FlashThree(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*4, delegate {FlashTwo(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*5, delegate {FlashOne(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*6, delegate {FlashZero(startBeat);})
            });
        }
        public void CountFive(double startBeat, float length)
        {
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat, delegate {FlashFive(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length, delegate {FlashFour(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*2, delegate {FlashThree(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*3, delegate {FlashTwo(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*4, delegate {FlashOne(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*5, delegate {FlashZero(startBeat);})
            });
        }
        public void CountFour(double startBeat, float length)
        {
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat, delegate {FlashFour(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length, delegate {FlashThree(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*2, delegate {FlashTwo(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*3, delegate {FlashOne(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*4, delegate {FlashZero(startBeat);})
            });
        }
        public void CountThree(double startBeat, float length)
        {
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat, delegate {FlashThree(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length, delegate {FlashTwo(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*2, delegate {FlashOne(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*3, delegate {FlashZero(startBeat);})
            });
        }
        public void CountTwo(double startBeat, float length)
        {
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat, delegate {FlashTwo(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length, delegate {FlashOne(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length*2, delegate {FlashZero(startBeat);})
            });
        }
        public void CountOne(double startBeat, float length)
        {
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat, delegate {FlashOne(startBeat, disableCount);}),
                new BeatAction.Action(startBeat + length, delegate {FlashZero(startBeat);})
            });
        }
// Number Call Functions

        public void HideCountdown(bool toggleCount)
        {
            if (toggleCount)
            {
                disableCount = false;
            }
            else
            {
                disableCount = true;
            }
        }

        public void FlashNine(double beat, bool disableCount)
        {
            if (disableCount != true)
            {
                numberBGAnimator.DoScaledAnimationAsync("FlashBG", 0.5f);
                numberAnimator.DoScaledAnimationAsync("Nine", 0.5f);
                SoundByte.PlayOneShotGame("rhythmTestGBA/blip2");
            }
            else
            {
                numberBGAnimator.Play("Idle");
                numberAnimator.Play("Idle");                
            }
        }

        public void FlashEight(double beat, bool disableCount)
        {
            if (!disableCount)
            {
                numberBGAnimator.DoScaledAnimationAsync("FlashBG", 0.5f);
                numberAnimator.DoScaledAnimationAsync("Eight", 0.5f);
                SoundByte.PlayOneShotGame("rhythmTestGBA/blip2");
            }
            else
            {
                numberBGAnimator.Play("Idle");
                numberAnimator.Play("Idle");                
            }
        }

        public void FlashSeven(double beat, bool disableCount)
        {
            if (!disableCount)
            {
                numberBGAnimator.DoScaledAnimationAsync("FlashBG", 0.5f);
                numberAnimator.DoScaledAnimationAsync("Seven", 0.5f);
                SoundByte.PlayOneShotGame("rhythmTestGBA/blip2");
            }
            else
            {
                numberBGAnimator.Play("Idle");
                numberAnimator.Play("Idle");                
            }
        }

        public void FlashSix(double beat, bool disableCount)
        {
            if (!disableCount)
            {
                numberBGAnimator.DoScaledAnimationAsync("FlashBG", 0.5f);
                numberAnimator.DoScaledAnimationAsync("Six", 0.5f);
                SoundByte.PlayOneShotGame("rhythmTestGBA/blip2");
            }
            else
            {
                numberBGAnimator.Play("Idle");
                numberAnimator.Play("Idle");                
            }
        }

        public void FlashFive(double beat, bool disableCount)
        {
            if (!disableCount)
            {
                numberBGAnimator.DoScaledAnimationAsync("FlashBG", 0.5f);
                numberAnimator.DoScaledAnimationAsync("Five", 0.5f);
                SoundByte.PlayOneShotGame("rhythmTestGBA/blip2");
            }
            else
            {
                numberBGAnimator.Play("Idle");
                numberAnimator.Play("Idle");                
            }
        }

        public void FlashFour(double beat, bool disableCount)
        {
            if (!disableCount)
            {
                numberBGAnimator.DoScaledAnimationAsync("FlashBG", 0.5f);
                numberAnimator.DoScaledAnimationAsync("Four", 0.5f);
                SoundByte.PlayOneShotGame("rhythmTestGBA/blip2");
            }
            else
            {
                numberBGAnimator.Play("Idle");
                numberAnimator.Play("Idle");                
            }
        }        

        public void FlashThree(double beat, bool disableCount)
        {
            if (!disableCount)
            {
                numberBGAnimator.DoScaledAnimationAsync("FlashBG", 0.5f);
                numberAnimator.DoScaledAnimationAsync("Three", 0.5f);
                SoundByte.PlayOneShotGame("rhythmTestGBA/blip2");
            }
            else
            {
                numberBGAnimator.Play("Idle");
                numberAnimator.Play("Idle");                
            }
        }

        public void FlashTwo(double beat, bool disableCount)
        {
            if (!disableCount)
            {
                numberBGAnimator.DoScaledAnimationAsync("FlashBG", 0.5f);
                numberAnimator.DoScaledAnimationAsync("Two", 0.5f);
                SoundByte.PlayOneShotGame("rhythmTestGBA/blip2");
            }
            else
            {
                numberBGAnimator.Play("Idle");
                numberAnimator.Play("Idle");                
            }
        }

        public void FlashOne(double beat, bool disableCount)
        {
            if (!disableCount)
            {
                numberBGAnimator.DoScaledAnimationAsync("FlashBG", 0.5f);
                numberAnimator.DoScaledAnimationAsync("One", 0.5f);
                SoundByte.PlayOneShotGame("rhythmTestGBA/blip2");
            }
            else
            {
                numberBGAnimator.Play("Idle");
                numberAnimator.Play("Idle");                
            }
        }

        public void FlashZero(double beat)
        {

            numberBGAnimator.DoScaledAnimationAsync("FlashHit", 0.5f);
            numberAnimator.DoScaledAnimationAsync("Zero", 0.5f);
            SoundByte.PlayOneShotGame("rhythmTestGBA/blip3");

        }




        public void HideCountdown(double beat, bool showcount)
        {

        }


        public void ButtonSuccess(PlayerActionEvent caller, float state)
        {
            PressButton();
       

        }

        public void ButtonFailure(PlayerActionEvent caller)
        {

        }
        public void ButtonEmpty(PlayerActionEvent caller) {



         }


    }
}

namespace HeavenStudio.Games.Scripts_RhythmTestGBA
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

