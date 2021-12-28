using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using RhythmHeavenMania.Util;

using RhythmHeavenMania.Games.ForkLifter;
using RhythmHeavenMania.Games.ClappyTrio;
using RhythmHeavenMania.Games.Spaceball;

namespace RhythmHeavenMania
{
    public class EventCaller : MonoBehaviour
    {
        public Transform GamesHolder;
        private float currentBeat;
        private float currentLength;
        private float currentValA;
        private string currentSwitchGame;
        private int currentType;

        public delegate void EventCallback();

        public List<MiniGame> minigames = new List<MiniGame>()
        {
        };

        [Serializable]
        public class MiniGame
        {
            public string name;
            public string color;
            public GameObject holder;
            public List<GameAction> actions = new List<GameAction>();

            public MiniGame(string name, string color, List<GameAction> actions)
            {
                this.name = name;
                this.color = color;
                this.actions = actions;
            }
        }

        public class GameAction
        {
            public string actionName;
            public EventCallback function;
            public bool playerAction = false;

            public GameAction(string actionName, EventCallback function, bool playerAction = false)
            {
                this.actionName = actionName;
                this.function = function;
                this.playerAction = playerAction;
            }
        }

        public void Init()
        {
            minigames = new List<MiniGame>()
            {
                new MiniGame("gameManager", "", new List<GameAction>()
                {
                    new GameAction("end", delegate { Debug.Log("end"); }),
                    new GameAction("switchGame", delegate { GameManager.instance.SwitchGame(currentSwitchGame); })
                }),
                new MiniGame("forkLifter", "FFFFFF", new List<GameAction>()
                {
                    new GameAction("pea", delegate { ForkLifter.instance.Flick(currentBeat, 0); }, true ),
                    new GameAction("topbun", delegate { ForkLifter.instance.Flick(currentBeat, 1); }, true ),
                    new GameAction("burger", delegate { ForkLifter.instance.Flick(currentBeat, 2); }, true ),
                    new GameAction("bottombun", delegate { ForkLifter.instance.Flick(currentBeat, 3); }, true ),
                    new GameAction("prepare", delegate { ForkLifter.instance.ForkLifterHand.Prepare(); }),
                    new GameAction("gulp", delegate { ForkLifterPlayer.instance.Eat(); }),
                    new GameAction("sigh", delegate { Jukebox.PlayOneShot("sigh"); })
                }),
                new MiniGame("clappyTrio", "29E7FF", new List<GameAction>()
                {
                    // Claps
                    new GameAction("clap", delegate { ClappyTrio.instance.Clap(currentBeat, currentLength); }, true ),

                    new GameAction("bop", delegate { ClappyTrio.instance.Bop(currentBeat); } ),

                    new GameAction("prepare", delegate { ClappyTrio.instance.Prepare(0); } ),
                    new GameAction("prepare_alt", delegate { ClappyTrio.instance.Prepare(3); } ),
                }),
                new MiniGame("spaceball", "00A518", new List<GameAction>()
                {
                    new GameAction("shoot", delegate { Spaceball.instance.Shoot(currentBeat, false, currentType); }, true ),
                    new GameAction("shootHigh", delegate { Spaceball.instance.Shoot(currentBeat, true, currentType); }, true ),
                    new GameAction("costume", delegate { Spaceball.instance.Costume(currentType); }, true ),
                    // new GameAction("cameraZoom", delegate { Spaceball.instance.CameraZoom(currentBeat, currentLength, currentValA); } ),
                })
            };

            for (int i = 1; i < minigames.Count; i++)
            {
                minigames[i].holder = GamesHolder.Find(minigames[i].name).gameObject;
            }

            for (int i = 0; i < GameManager.instance.Beatmap.entities.Count; i++)
            {
                string[] e = GameManager.instance.Beatmap.entities[i].datamodel.Split('/');
                try
                {
                    if (minigames.Find(c => c.name == e[0]).actions.Find(c => c.actionName == e[1]).playerAction == true)
                    {
                        GameManager.instance.playerEntities.Add(GameManager.instance.Beatmap.entities[i]);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }
            }
        }

        private void Update()
        {
            if (GameManager.instance.currentEvent >= 0 && GameManager.instance.currentEvent < GameManager.instance.Beatmap.entities.Count)
            currentBeat = GameManager.instance.Beatmap.entities[GameManager.instance.currentEvent].beat;
        }

        public void CallEvent(string event_)
        {
            string[] details = event_.Split('/');
            MiniGame game = minigames.Find(c => c.name == details[0]);

            try
            {
                currentLength = GameManager.instance.Beatmap.entities[GameManager.instance.currentEvent].length;
                currentType = GameManager.instance.Beatmap.entities[GameManager.instance.currentEvent].type;
                currentValA = GameManager.instance.Beatmap.entities[GameManager.instance.currentEvent].valA;

                if (details.Length > 2) currentSwitchGame = details[2];

                GameAction action = game.actions.Find(c => c.actionName == details[1]);
                action.function.Invoke();

                if (action.playerAction == true)
                    GameManager.instance.currentPlayerEvent++;

            }
            catch (Exception ex)
            {
                Debug.LogWarning("Event not found! May be spelled wrong or it is not implemented." + ex);
            }
        }

        public static List<Beatmap.Entity> GetAllInGameManagerList(string gameName, string[] include)
        {
            List<Beatmap.Entity> temp1 = GameManager.instance.Beatmap.entities.FindAll(c => c.datamodel.Split('/')[0] == gameName);
            List<Beatmap.Entity> temp2 = new List<Beatmap.Entity>();
            for (int i = 0; i < temp1.Count; i++)
            {
                if (include.Any(temp1[i].datamodel.Split('/')[1].Contains))
                {
                    temp2.Add(temp1[i]);
                }
            }
            return temp2;
        }

        public static List<Beatmap.Entity> GetAllInGameManagerListExclude(string gameName, string[] exclude)
        {
            List<Beatmap.Entity> temp1 = GameManager.instance.Beatmap.entities.FindAll(c => c.datamodel.Split('/')[0] == gameName);
            List<Beatmap.Entity> temp2 = new List<Beatmap.Entity>();
            for (int i = 0; i < temp1.Count; i++)
            {
                if (!exclude.Any(temp1[i].datamodel.Split('/')[1].Contains))
                {
                    temp2.Add(temp1[i]);
                }
            }
            return temp2;
        }

        public static List<Beatmap.Entity> GetAllPlayerEntities(string gameName)
        {
            return GameManager.instance.playerEntities.FindAll(c => c.datamodel.Split('/')[0] == gameName);
        }

        public static List<Beatmap.Entity> GetAllPlayerEntitiesExcept(string gameName)
        {
            return GameManager.instance.playerEntities.FindAll(c => c.datamodel.Split('/')[0] != gameName);
        }

        // elaborate as fuck, boy
        public static List<Beatmap.Entity> GetAllPlayerEntitiesExceptBeforeBeat(string gameName, float beat)
        {
            return GameManager.instance.playerEntities.FindAll(c => c.datamodel.Split('/')[0] != gameName && c.beat < beat);
        }
    }
}