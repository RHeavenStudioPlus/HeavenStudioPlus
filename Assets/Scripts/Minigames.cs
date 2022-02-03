using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

using RhythmHeavenMania.Games.ForkLifter;
using RhythmHeavenMania.Games.ClappyTrio;
using RhythmHeavenMania.Games.Spaceball;
using RhythmHeavenMania.Games.KarateMan;
using RhythmHeavenMania.Games.SpaceSoccer;
using RhythmHeavenMania.Games.DJSchool;

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
            public bool threeD;
            public bool fxOnly;
            public List<GameAction> actions = new List<GameAction>();

            public Minigame(string name, string displayName, string color, bool threeD, bool fxOnly, List<GameAction> actions)
            {
                this.name = name;
                this.displayName = displayName;
                this.color = color;
                this.actions = actions;
                this.threeD = threeD;
                this.fxOnly = fxOnly;
            }
        }

        public class GameAction
        {
            public string actionName;
            public EventCallback function;
            public float defaultLength;
            public bool resizable;
            public List<Param> parameters;

            public GameAction(string actionName, EventCallback function, float defaultLength = 1, bool resizable = false, List<Param> parameters = null)
            {
                this.actionName = actionName;
                this.function = function;
                this.defaultLength = defaultLength;
                this.resizable = resizable;
                this.parameters = parameters;
            }
        }

        [System.Serializable]
        public class Param
        {
            public string propertyName;
            public object parameter;
            public string propertyCaption;

            public Param(string propertyName, object parameter, string propertyCaption)
            {
                this.propertyName = propertyName;
                this.parameter = parameter;
                this.propertyCaption = propertyCaption;
            }
        }

        public delegate void EventCallback();

        public static void Init(EventCaller eventCaller)
        {
            eventCaller.minigames = new List<Minigame>()
            {
                new Minigame("gameManager", "Game Manager", "", false, true, new List<GameAction>()
                {
                    new GameAction("switchGame",        delegate { GameManager.instance.SwitchGame(eventCaller.currentSwitchGame); }),
                    new GameAction("end",               delegate { Debug.Log("end"); }),
                    new GameAction("skill star",        delegate {  }, 1f, true),
                }),
                new Minigame("countIn", "Count-Ins", "", false, true, new List<GameAction>()
                {
                    new GameAction("cowbell",           delegate { Jukebox.PlayOneShot("count-ins/cowbell"); }, 1f),
                    new GameAction("one",           delegate { Jukebox.PlayOneShot("count-ins/one1"); }, 1f),
                    new GameAction("one (alt)",           delegate { Jukebox.PlayOneShot("count-ins/one2"); }, 1f),
                    new GameAction("two",           delegate { Jukebox.PlayOneShot("count-ins/two1"); }, 1f),
                    new GameAction("two (alt)",           delegate { Jukebox.PlayOneShot("count-ins/two2"); }, 1f),
                    new GameAction("three",           delegate { Jukebox.PlayOneShot("count-ins/three1"); }, 1f),
                    new GameAction("three (alt)",           delegate { Jukebox.PlayOneShot("count-ins/three2"); }, 1f),
                    new GameAction("four",           delegate { Jukebox.PlayOneShot("count-ins/four1"); }, 1f),
                    new GameAction("four (alt)",           delegate { Jukebox.PlayOneShot("count-ins/four2"); }, 1f),
                    new GameAction("and",           delegate { Jukebox.PlayOneShot("count-ins/and"); }, 0.5f),
                    new GameAction("go!",           delegate { Jukebox.PlayOneShot("count-ins/go1"); }, 1f),
                    new GameAction("go! (alt)",           delegate { Jukebox.PlayOneShot("count-ins/go2"); }, 1f),
                    new GameAction("ready!",           delegate 
                    {             
                        MultiSound.Play(new MultiSound.Sound[]
                        {
                            new MultiSound.Sound("count-ins/ready1", eventCaller.currentBeat),
                            new MultiSound.Sound("count-ins/ready2", eventCaller.currentBeat + 1f),
                        }, false);
                    }, 2f),
                }),
                new Minigame("forkLifter", "Fork Lifter", "FFFFFF", false, false, new List<GameAction>()
                {
                    new GameAction("pea",               delegate { ForkLifter.instance.Flick(eventCaller.currentBeat, 0); }, 3),
                    new GameAction("topbun",            delegate { ForkLifter.instance.Flick(eventCaller.currentBeat, 1); }, 3),
                    new GameAction("burger",            delegate { ForkLifter.instance.Flick(eventCaller.currentBeat, 2); }, 3),
                    new GameAction("bottombun",         delegate { ForkLifter.instance.Flick(eventCaller.currentBeat, 3); }, 3),
                    new GameAction("prepare",           delegate { ForkLifter.instance.ForkLifterHand.Prepare(); }, 0.5f),
                    new GameAction("gulp",              delegate { ForkLifterPlayer.instance.Eat(); }),
                    new GameAction("sigh",              delegate { Jukebox.PlayOneShot("sigh"); })
                }),
                new Minigame("clappyTrio", "The Clappy Trio", "29E7FF", false, false, new List<GameAction>()
                {
                    new GameAction("clap",              delegate { ClappyTrio.instance.Clap(eventCaller.currentBeat, eventCaller.currentLength); }, 3, true),
                    new GameAction("bop",               delegate { ClappyTrio.instance.Bop(eventCaller.currentBeat); } ),
                    new GameAction("prepare",           delegate { ClappyTrio.instance.Prepare(0); } ),
                    new GameAction("prepare_alt",       delegate { ClappyTrio.instance.Prepare(3); } ),
                }),
                new Minigame("spaceball", "Spaceball", "00A518", false, false, new List<GameAction>()
                {
                    new GameAction("shoot",             delegate { Spaceball.instance.Shoot(eventCaller.currentBeat, false, eventCaller.currentType); }, 2, false),
                    new GameAction("shootHigh",         delegate { Spaceball.instance.Shoot(eventCaller.currentBeat, true, eventCaller.currentType); }, 3),
                    new GameAction("costume",           delegate { Spaceball.instance.Costume(eventCaller.currentType); }, 1f, false, new List<Param>() { new Param("type", new EntityTypes.Integer(0, 2), "type") } ),
                    new GameAction("alien",             delegate { Spaceball.instance.alien.Show(eventCaller.currentBeat); } ),
                    new GameAction("camera",            delegate { }, 4, true ),
                    new GameAction("prepare dispenser", delegate { Spaceball.instance.PrepareDispenser(); }, 1 ),
                }),
                new Minigame("karateman", "Karate Man", "70A8D8", false, false, new List<GameAction>()
                {
                    new GameAction("bop",               delegate { KarateMan.instance.Bop(eventCaller.currentBeat, eventCaller.currentLength); }, 0.5f, true),
                    new GameAction("pot",               delegate { KarateMan.instance.Shoot(eventCaller.currentBeat, 0); }, 2),
                    new GameAction("bulb",              delegate { KarateMan.instance.Shoot(eventCaller.currentBeat, 1); }, 2),
                    new GameAction("rock",              delegate { KarateMan.instance.Shoot(eventCaller.currentBeat, 2); }, 2),
                    new GameAction("ball",              delegate { KarateMan.instance.Shoot(eventCaller.currentBeat, 3); }, 2),
                    new GameAction("kick",              delegate { KarateMan.instance.Shoot(eventCaller.currentBeat, 4); }, 4.5f),
                    new GameAction("combo",             delegate { KarateMan.instance.Combo(eventCaller.currentBeat); }, 4f),
                    new GameAction("hit3",              delegate { KarateMan.instance.Hit3(eventCaller.currentBeat); }),
                    new GameAction("hit4",              delegate { KarateMan.instance.Hit4(eventCaller.currentBeat); }),
                    new GameAction("prepare",           delegate { KarateMan.instance.Prepare(eventCaller.currentBeat, eventCaller.currentLength); }, 1f, true),
                    new GameAction("bgfxon",            delegate { KarateMan.instance.BGFXOn(); } ),
                    new GameAction("bgfxoff",           delegate { KarateMan.instance.BGFXOff(); }),
                    new GameAction("tacobell",          delegate { KarateMan.instance.Shoot(eventCaller.currentBeat, 6); }, 2),
                }),
                new Minigame("spaceSoccer", "Space Soccer", "B888F8", false, false, new List<GameAction>()
                {
                    new GameAction("ball dispense",     delegate { SpaceSoccer.instance.Dispense(eventCaller.currentBeat); }, 2f),
                    new GameAction("keep-up",           delegate { }, 4f, true),
                    new GameAction("high kick-toe!",    delegate { }, 3f),
                }),
                new Minigame("djSchool", "DJ School \n<color=#eb5454>[Non-Playable]</color>", "B888F8", false, false, new List<GameAction>()
                {
                    new GameAction("break c'mon ooh",   delegate { DJSchool.instance.BreakCmon(eventCaller.currentBeat);  }, 3f),
                    new GameAction("scratch-o hey",     delegate { DJSchool.instance.ScratchoHey(eventCaller.currentBeat);  }, 3f),
                }),
                /*new Minigame("rhythmRally", "Rhythm Rally", "B888F8", true, false, new List<GameAction>()
                {

                }),
                new Minigame("spaceDance", "Space Dance", "B888F8", new List<GameAction>()
                {
                }),
                new Minigame("tapTrial", "Tap Trial", "B888F8", new List<GameAction>()
                {
                }),
                new Minigame("sneakySpirits", "Sneaky Spirits", "B888F8", new List<GameAction>()
                {
                }),
                new Minigame("munchyMonk", "Munchy Monk", "B888F8", new List<GameAction>()
                {
                }),
                new Minigame("airRally", "Air Rally", "B888F8", new List<GameAction>()
                {
                }),
                new Minigame("ringside", "Ringside", "B888F8", new List<GameAction>()
                {
                }),
                new Minigame("workingDough", "Working Dough", "B888F8", new List<GameAction>()
                {
                })*/
            };
        }
    }
}