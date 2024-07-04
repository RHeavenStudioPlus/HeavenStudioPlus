using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;
using NaughtyBezierCurves;

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
    public static class PcoVenusLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("nipInTheBud", "Nip In the Bud", "ffffff", false, false, new List<GameAction>()
            {
                new GameAction("bop", "Bop")
                {
                    function = delegate {NipInTheBud.instance.BopToggle(eventCaller.currentEntity.beat, eventCaller.currentEntity.length, eventCaller.currentEntity["auto"], eventCaller.currentEntity["toggle"]);},
                    defaultLength = 1f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("toggle", true, "Bop", "Toggle if Leilani should bop for the duration of this event."),
                        new Param("auto", false, "Autobop", "Toggle if Leilani should bop automatically until another Bop event is reached."),
                    }
                },
                new GameAction("prepare", "Prepare")
                {
                    function = delegate {
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out NipInTheBud instance)) {
                            instance.DoPrepare();
                        }
                    },
                    defaultLength = 0.5f,
                },
                new GameAction("spawnMosquito", "Mosquito")
                {
                    function = delegate {NipInTheBud.instance.QueueMosquito(eventCaller.currentEntity.beat);},
                    defaultLength = 3f,
                    resizable = false,
                },
                new GameAction("spawnMayfly", "Mayfly")
                {
                    function = delegate {NipInTheBud.instance.QueueMayfly(eventCaller.currentEntity.beat);},
                    defaultLength = 5f,
                    resizable = false,
                }
            },

        new List<string>() {"pco", "normal"}
        //"pcovenus", "en",
            );

        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_NipInTheBud;
    using Jukebox;
    /// This class handles the minigame logic.
    /// Minigame inherits directly from MonoBehaviour, and adds Heaven Studio specific methods to override.
    public class NipInTheBud : Minigame
    {
        public static NipInTheBud instance;

        bool goBop;
        public bool noBop = false;
        public bool queuePrepare;
        public bool preparing;

        [Header("Animators")]
        public Animator Leilani;
        public Animator Bubble;

        [Header("Objects")]
        [SerializeField] Mosquito Mosquito;
        [SerializeField] Mayfly Mayfly;
        [SerializeField] GameObject mosquitoStart;
        [SerializeField] GameObject mayflyStart;


        private void Awake()
        {
            instance = this;
            SetupBopRegion("nipInTheBud", "bop", "auto");   

        }

        public override void OnBeatPulse(double beat)
        {
            if (BeatIsInBopRegion(beat))
            {
                Bop();
            }

        }

        public void Update()
        {
            if (queuePrepare && !preparing && (Leilani.IsAnimationNotPlaying() || Leilani.IsPlayingAnimationNames("Bop")))
            {
                Leilani.DoScaledAnimationAsync("Prepare", 0.5f);
                preparing = true;
                queuePrepare = false;
            }
            if (PlayerInput.GetIsAction(InputAction_BasicPress) && !GameManager.instance.autoplay && PlayerInput.CurrentControlStyle == InputController.ControlStyles.Touch)
            {
                // queuePrepare = true;
                Leilani.DoScaledAnimationAsync("Prepare", 0.5f);
                preparing = true;
            }
            if (PlayerInput.GetIsAction(InputAction_BasicRelease) && (!IsExpectingInputNow(InputAction_BasicRelease)) && (!GameManager.instance.autoplay)  && PlayerInput.CurrentControlStyle == InputController.ControlStyles.Touch)
            {
                StopPrepare();
                Leilani.DoScaledAnimationAsync("Unprepare", 0.5f);
            }
            if (PlayerInput.GetIsAction(InputAction_FlickPress) && !IsExpectingInputNow(InputAction_FlickPress))
            {
                Leilani.DoScaledAnimationAsync("Snap", 0.5f);
                SoundByte.PlayOneShotGame("nipInTheBud/whiff");
                }
        }

        public void DoPrepare()
        {
            if (PlayerInput.CurrentControlStyle == InputController.ControlStyles.Touch && PlayerInput.PlayerHasControl()) return;
            Leilani.DoScaledAnimationAsync("Prepare", 0.5f);
            preparing = true;
        }

        public void StopPrepare()
        {
            preparing = false;
            queuePrepare = false;
        }

        public void BopToggle(double beat, float length, bool toggle, bool autoBop)
        {
            
            if (toggle)
            {
                List<BeatAction.Action> bops = new List<BeatAction.Action>();
                for (int i = 0; i < length; i++)
                {
                    bops.Add(new BeatAction.Action(beat + i, delegate { Bop(); }));
                }
                BeatAction.New(instance, bops);
            }
        }

        public void Bop()
        {
            if (!noBop && !preparing && !queuePrepare && (Leilani.IsAnimationNotPlaying() || Leilani.IsPlayingAnimationNames("Idle"))) Leilani.DoScaledAnimationAsync("Bop", 0.5f, 0);
        }

        public void QueueMosquito (double beat)
        {
            SummonMosquito(beat);
            BeatAction.New(this, new() {
                    new(beat+1, () => queuePrepare = PlayerInput.CurrentControlStyle != InputController.ControlStyles.Touch || GameManager.instance.autoplay)
                });
        }

        public void SummonMosquito(double beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("nipInTheBud/mosquito1", beat),
                new MultiSound.Sound("nipInTheBud/mosquito2", beat+1f)
            });
            Mosquito newMosquito = Instantiate(Mosquito, mosquitoStart.transform);
            newMosquito.startBeat = beat;
            newMosquito.gameObject.SetActive(true);
        }

        public void QueueMayfly (double beat)
        {
            SummonMayfly(beat+2);
            
            MultiSound.Play(new MultiSound.Sound[]
            {
                
                new MultiSound.Sound("nipInTheBud/blink1", beat),
                new MultiSound.Sound("nipInTheBud/blink2", beat+1f)
            });
            BeatAction.New(this, new() {
                    new(beat+3, () => queuePrepare = PlayerInput.CurrentControlStyle != InputController.ControlStyles.Touch || GameManager.instance.autoplay)
                });
            BeatAction.New(instance, new List<BeatAction.Action>(){
                new BeatAction.Action(beat, delegate {Bubble.DoScaledAnimationAsync("alert1", 0.5f);}),
                new BeatAction.Action(beat+1, delegate {Bubble.DoScaledAnimationAsync("alert2", 0.5f);}),
                new BeatAction.Action(beat+2, delegate {Bubble.DoScaledAnimationAsync ("disable", 0.5f);})
            });
        }

        public void SummonMayfly(double beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("nipInTheBud/mayfly1", beat),
                new MultiSound.Sound("nipInTheBud/mayfly2", beat+1f)
            });
            Mayfly newMayfly = Instantiate(Mayfly, mayflyStart.transform);
            newMayfly.startBeat = beat;
            newMayfly.gameObject.SetActive(true);
        }

        
    }
}