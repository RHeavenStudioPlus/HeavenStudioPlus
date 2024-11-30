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
    public static class PcoBalloonHunterLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("balloonHunter", "Balloon Hunter", "92cbe8", false, false, new List<GameAction>()
            {
                new GameAction("bop", "Bop")
                {
                    function = delegate {
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out BalloonHunter instance)) {
                            instance.Bop(eventCaller.currentEntity.beat, eventCaller.currentEntity.length, eventCaller.currentEntity["auto"], eventCaller.currentEntity["toggle"], eventCaller.currentEntity["emote"]);
                        }
                    },
                    defaultLength = 1f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("toggle", true, "Bop", "Toggle if the characters should bop for the duration of this event."),
                        new Param("auto", true, "Bop (Auto)", "Toggle if the characters should automatically bop until another Bop event is reached."),
                        new Param("emote", true, "Reactions", "Toggle if the characters should react to the player's performance."),
                    }
                },
                new GameAction("prepare", "Prepare")
                {
                    function = delegate {
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out BalloonHunter instance)) {
                            instance.Prepare();
                        }
                    },
                    defaultLength = 0.5f,
                },
                new GameAction("balloonSlow", "Normal Balloon")
                {
                    function = delegate {
                        SoundByte.PlayOneShotGame("balloonHunter/tweet_slow", eventCaller.currentEntity.beat, forcePlay: true);
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out BalloonHunter instance)) {
                            instance.QueueBalloonSlow(eventCaller.currentEntity.beat);
                        }
                    },
                    defaultLength = 3,
                },
                new GameAction("balloonFast", "Fast Balloon")
                {
                    function = delegate {
                        SoundByte.PlayOneShotGame("balloonHunter/tweet_fast", eventCaller.currentEntity.beat, forcePlay: true);
                        SoundByte.PlayOneShotGame("balloonHunter/tweet_fast", eventCaller.currentEntity.beat+0.75, forcePlay: true);
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out BalloonHunter instance)) {
                            instance.SendBalloonFast(eventCaller.currentEntity.beat);
                        }
                    },
                    defaultLength = 2.5f,
                },
                new GameAction("balloonBoth", "Double Balloons")
                {
                    function = delegate {
                        SoundByte.PlayOneShotGame("balloonHunter/tweet_both", eventCaller.currentEntity.beat, forcePlay: true);
                        SoundByte.PlayOneShotGame("balloonHunter/tweet_both", eventCaller.currentEntity.beat+0.75, forcePlay: true);
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out BalloonHunter instance)) {
                            instance.SendBalloonBoth(eventCaller.currentEntity.beat);
                        }
                    },
                    defaultLength = 3,
                },
                new GameAction("balloonFive", "Slow Balloon")
                {
                    function = delegate {
                        SoundByte.PlayOneShotGame("balloonHunter/tweet_five", eventCaller.currentEntity.beat, forcePlay: true);
                        SoundByte.PlayOneShotGame("balloonHunter/tweet_five", eventCaller.currentEntity.beat+1, forcePlay: true);
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out BalloonHunter instance)) {
                            instance.SendBalloonFive(eventCaller.currentEntity.beat);
                        }
                    },
                    defaultLength = 5,
                }
            }
            );
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_BalloonHunter;
    /// This class handles the minigame logic.
    /// Minigame inherits directly from MonoBehaviour, and adds Heaven Studio specific methods to override.
    public class BalloonHunter : Minigame
    {
        //public static BalloonHunter instance;
        public string bopExpression = "Neutral";

        [Header("Objects")]
        [SerializeField] Balloon slowBalloon;
        [SerializeField] Balloon fastBalloon;
        [SerializeField] Balloon balloonFive;

        [Header("Animators")]
        [SerializeField] Animator hunterAnim;
        [SerializeField] Animator birdAnim;

        private bool hunterBop = true;
        private bool birdBop = true;
        private bool preparing;
        private bool queueBopReset;
        public bool hunterHold;

        private void Awake()
        {
            //instance = this;
            slowBalloon.gameObject.SetActive(false);
            fastBalloon.gameObject.SetActive(false);
            balloonFive.gameObject.SetActive(false);
        }


        public override void OnLateBeatPulse(double beat)
        {
            if (hunterBop)
            {
                hunterAnim.DoScaledAnimationAsync("Bop", 0.5f, animLayer: 0);
                if (!queueBopReset) { hunterAnim.DoScaledAnimationAsync(bopExpression, 0.5f, animLayer: 1); }

            }
            if (birdBop)
            {
                birdAnim.DoScaledAnimationAsync("Bop", 0.5f, animLayer: 0);
                if (!queueBopReset) { birdAnim.DoScaledAnimationAsync(bopExpression, 0.5f, animLayer: 1); }
            }

        }

        public void Update()
        {
            if (PlayerInput.GetIsAction(InputAction_BasicPress) && (!preparing) && !IsExpectingInputNow(InputAction_BasicPress))
            {
                hunterAnim.DoScaledAnimationAsync("Bop", 0.5f);

            }
            if (PlayerInput.GetIsAction(InputAction_BasicPress) && (preparing) && !IsExpectingInputNow(InputAction_BasicPress))
            {
                hunterAnim.DoScaledAnimationAsync("Miss", 0.5f, animLayer: 0);
                hunterAnim.DoScaledAnimationAsync("Blow", 0.5f, animLayer: 1);
                SoundByte.PlayOneShotGame("balloonHunter/blow");
                hunterHold = false;
            }
        }

        public void Prepare()
        {
            if (preparing) return;
            hunterAnim.DoScaledAnimationAsync("Prepare", 0.5f, animLayer: 0);
            hunterAnim.DoScaledAnimationAsync("Hold", 0.5f, animLayer: 1);
            preparing = true;
            hunterHold = true;
            hunterBop = false;
        }

        public void Unprepare()
        {
            if ((!hunterHold) && (preparing))
            {
                hunterAnim.DoScaledAnimationAsync("Bop", 0.5f, animLayer: 0);
                preparing = false;
            }
        }

        public void BirdCall()
        {
            if (birdBop) birdBop = false;
            birdAnim.DoScaledAnimationAsync("Bop", 0.5f, animLayer: 0);
            birdAnim.DoScaledAnimationAsync("Call", 0.5f, animLayer: 1);
        }

        public void ResetBopExpression(double beat)
        {
            BeatAction.New(this, new() {
                    new(beat+1, delegate{
                        bopExpression = "Neutral";
                        queueBopReset = false;
                    }),
            });
        }

        public void Bop(double beat, float length, bool auto, bool bop, bool emote)
        {
            hunterBop = auto;
            birdBop = auto;
            preparing = false;
            if (bop) 
            { 
                List<BeatAction.Action> actions = new();
                for (int i = 0; i < length; i++)
                {
                    actions.Add(new(beat + i, delegate {
                        hunterAnim.DoScaledAnimationAsync("Bop", 0.5f, animLayer: 0);
                        hunterAnim.DoScaledAnimationAsync(emote ? bopExpression : "Neutral", 0.5f, animLayer: 1);
                        birdAnim.DoScaledAnimationAsync("Bop", 0.5f, animLayer: 0);
                        birdAnim.DoScaledAnimationAsync(emote ? bopExpression : "Neutral", 0.5f, animLayer: 1);
                    }));
                }
                if (actions.Count > 0) BeatAction.New(this, actions);
            }
            queueBopReset = true;
            ResetBopExpression(beat);
        }

        public void QueueBalloonSlow(double beat)
        {
            SendBalloonSlow(beat);
            Unprepare();

            BeatAction.New(this, new List<BeatAction.Action>(){
                new BeatAction.Action(beat, delegate {BirdCall();}),
                new BeatAction.Action(beat+1, delegate {Prepare();}),
            });
        }

        public void SendBalloonSlow(double beat)
        {
            Balloon newSlowB = Instantiate(slowBalloon, transform);
            newSlowB.balloonSpeed = 3f;
            newSlowB.startBeat = beat;
            newSlowB.gameObject.SetActive(true);
        }

        public void SendBalloonFast(double beat)
        {
            Balloon newFastB = Instantiate(fastBalloon, transform);
            newFastB.balloonSpeed = 2.5f;
            newFastB.startBeat = beat;
            newFastB.gameObject.SetActive(true);

            Unprepare();

            BeatAction.New(this, new List<BeatAction.Action>(){
                new BeatAction.Action(beat, delegate {BirdCall();}),
                new BeatAction.Action(beat+0.75, delegate {BirdCall();}),
                new BeatAction.Action(beat+0.75, delegate {Prepare();})
            });
        }

        public void SendBalloonBoth(double beat)
        {
            SendBalloonSlow(beat);
            SendBalloonFast(beat);
        }

        public void SendBalloonFive(double beat)
        {
            Balloon newFive = Instantiate(balloonFive, transform);
            newFive.balloonSpeed = 3f;
            newFive.startBeat = beat+2;
            newFive.gameObject.SetActive(true);

            BeatAction.New(this, new List<BeatAction.Action>(){
                new BeatAction.Action(beat, delegate {BirdCall();}),
                new BeatAction.Action(beat+1, delegate {BirdCall();}),
                new BeatAction.Action(beat+2, delegate {Unprepare();}),
                new BeatAction.Action(beat+3, delegate {Prepare();})
            });
        }
    }
}