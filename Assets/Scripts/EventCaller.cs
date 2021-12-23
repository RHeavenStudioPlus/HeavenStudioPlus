using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using RhythmHeavenMania.Games.ForkLifter;
using RhythmHeavenMania.Games.ClappyTrio;
using RhythmHeavenMania.Util;

namespace RhythmHeavenMania
{
    public class EventCaller : MonoBehaviour
    {
        public Transform GamesHolder;
        private float currentBeat;

        public delegate void EventCallback();

        public List<MiniGame> minigames = new List<MiniGame>()
        {
        };

        [Serializable]
        public class MiniGame
        {
            public string name;
            public GameObject holder;
            public List<GameAction> actions = new List<GameAction>();

            public MiniGame(string name, List<GameAction> actions)
            {
                this.name = name;
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
                new MiniGame("gameManager", new List<GameAction>()
                {
                    new GameAction("end", delegate { Debug.Log("end"); })
                }),
                new MiniGame("forkLifter", new List<GameAction>() 
                { 
                    new GameAction("pea", delegate { ForkLifter.instance.Flick(currentBeat, 0); }, true ),
                    new GameAction("topbun", delegate { ForkLifter.instance.Flick(currentBeat, 1); }, true ),
                    new GameAction("burger", delegate { ForkLifter.instance.Flick(currentBeat, 2); }, true ),
                    new GameAction("bottombun", delegate { ForkLifter.instance.Flick(currentBeat, 3); }, true ),
                    new GameAction("prepare", delegate { ForkLifter.instance.ForkLifterHand.Prepare(); }),
                    new GameAction("gulp", delegate { ForkLifterPlayer.instance.Eat(); }),
                    new GameAction("sigh", delegate { Jukebox.PlayOneShot("sigh"); })
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
            if (GameManager.instance.currentEvent > 0 && GameManager.instance.currentEvent < GameManager.instance.Beatmap.entities.Count)
            currentBeat = GameManager.instance.Beatmap.entities[GameManager.instance.currentEvent].beat;
        }

        public void CallEvent(string event_)
        {
            string[] details = event_.Split('/');
            MiniGame game = minigames.Find(c => c.name == details[0]);

            try
            {
                GameAction action = game.actions.Find(c => c.actionName == details[1]);
                action.function.Invoke();

                if (action.playerAction == true)
                    GameManager.instance.currentPlayerEvent++;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Event not found! May be spelled wrong or it is not implemented.");
            }
        }

        public static List<Beatmap.Entity> GetAllInGameManagerListExcept(string gameName, string[] exclude)
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
    }
}