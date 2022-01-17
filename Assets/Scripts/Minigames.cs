using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

using RhythmHeavenMania.Games.ForkLifter;
using RhythmHeavenMania.Games.ClappyTrio;
using RhythmHeavenMania.Games.Spaceball;
using RhythmHeavenMania.Games.KarateMan;

namespace RhythmHeavenMania
{
    public class Minigames
    {
        public class Minigame
        {
            public string name;
            public string displayName;
            public string color;
            public GameObject holder;
            public List<GameAction> actions = new List<GameAction>();

            public Minigame(string name, string displayName, string color, List<GameAction> actions)
            {
                this.name = name;
                this.displayName = displayName;
                this.color = color;
                this.actions = actions;
            }
        }

        public class GameAction
        {
            public string actionName;
            public EventCallback function;
            public bool playerAction = false;
            public float defaultLength;
            public bool resizable;

            public GameAction(string actionName, EventCallback function, float defaultLength = 1, bool playerAction = false, bool resizable = false)
            {
                this.actionName = actionName;
                this.function = function;
                this.playerAction = playerAction;
                this.defaultLength = defaultLength;
                this.resizable = resizable;
            }
        }

        public delegate void EventCallback();

        public static void Init(EventCaller eventCaller)
        {
            eventCaller.minigames = new List<Minigame>()
            {
                new Minigame("gameManager", "Game Manager", "", new List<GameAction>()
                {
                    new GameAction("end",           delegate { Debug.Log("end"); }),
                    new GameAction("switchGame",    delegate { GameManager.instance.SwitchGame(eventCaller.currentSwitchGame); })
                }),
                new Minigame("forkLifter", "Fork Lifter", "FFFFFF", new List<GameAction>()
                {
                    new GameAction("pea",           delegate { ForkLifter.instance.Flick(eventCaller.currentBeat, 0); }, 3, true),
                    new GameAction("topbun",        delegate { ForkLifter.instance.Flick(eventCaller.currentBeat, 1); }, 3, true),
                    new GameAction("burger",        delegate { ForkLifter.instance.Flick(eventCaller.currentBeat, 2); }, 3, true),
                    new GameAction("bottombun",     delegate { ForkLifter.instance.Flick(eventCaller.currentBeat, 3); }, 3, true),
                    new GameAction("prepare",       delegate { ForkLifter.instance.ForkLifterHand.Prepare(); }, 0.5f, true),
                    new GameAction("gulp",          delegate { ForkLifterPlayer.instance.Eat(); }),
                    new GameAction("sigh",          delegate { Jukebox.PlayOneShot("sigh"); })
                }),
                new Minigame("clappyTrio", "The Clappy Trio", "29E7FF", new List<GameAction>()
                {
                    new GameAction("clap",          delegate { ClappyTrio.instance.Clap(eventCaller.currentBeat, eventCaller.currentLength); }, 3, true),
                    new GameAction("bop",           delegate { ClappyTrio.instance.Bop(eventCaller.currentBeat); } ),
                    new GameAction("prepare",       delegate { ClappyTrio.instance.Prepare(0); } ),
                    new GameAction("prepare_alt",   delegate { ClappyTrio.instance.Prepare(3); } ),
                }),
                new Minigame("spaceball", "Spaceball", "00A518", new List<GameAction>()
                {
                    new GameAction("shoot",         delegate { Spaceball.instance.Shoot(eventCaller.currentBeat, false, eventCaller.currentType); }, 2, true),
                    new GameAction("shootHigh",     delegate { Spaceball.instance.Shoot(eventCaller.currentBeat, true, eventCaller.currentType); }, 3, true),
                    new GameAction("costume",       delegate { Spaceball.instance.Costume(eventCaller.currentType); } ),
                    new GameAction("alien",         delegate { Spaceball.instance.alien.Show(eventCaller.currentBeat); } ),
                    new GameAction("cameraZoom",    delegate { }, 4, false, true ),
                }),
                new Minigame("karateman", "Karate Man", "70A8D8", new List<GameAction>()
                {
                    new GameAction("bop",           delegate { KarateMan.instance.Bop(eventCaller.currentBeat, eventCaller.currentLength); }, 0.5f, true, true),
                    new GameAction("pot",           delegate { KarateMan.instance.Shoot(eventCaller.currentBeat, 0); }, 2, true),
                    new GameAction("bulb",          delegate { KarateMan.instance.Shoot(eventCaller.currentBeat, 1); }, 2, true),
                    new GameAction("rock",          delegate { KarateMan.instance.Shoot(eventCaller.currentBeat, 2); }, 2, true),
                    new GameAction("ball",          delegate { KarateMan.instance.Shoot(eventCaller.currentBeat, 3); }, 2, true),
                    new GameAction("kick",          delegate { KarateMan.instance.Shoot(eventCaller.currentBeat, 4); }, 4.5f, true),
                    new GameAction("bgfxon",        delegate { KarateMan.instance.BGFXOn(); } ),
                    new GameAction("bgfxoff",       delegate { KarateMan.instance.BGFXOff(); }),
                })
            };
        }
    }
}