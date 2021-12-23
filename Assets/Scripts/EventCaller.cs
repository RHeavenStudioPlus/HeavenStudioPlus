using System;
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

            public GameAction(string actionName, EventCallback function)
            {
                this.actionName = actionName;
                this.function = function;
            }
        }

        public void Init()
        {
            minigames = new List<MiniGame>()
            {
                new MiniGame("forkLifter", new List<GameAction>() 
                { 
                    new GameAction("pea", delegate { ForkLifter.instance.Flick(currentBeat, 0); } ),
                    new GameAction("topbun", delegate { ForkLifter.instance.Flick(currentBeat, 1); } ),
                    new GameAction("burger", delegate { ForkLifter.instance.Flick(currentBeat, 2); } ),
                    new GameAction("bottombun", delegate { ForkLifter.instance.Flick(currentBeat, 3); } ),
                    new GameAction("prepare", delegate { ForkLifter.instance.ForkLifterHand.Prepare(); }),
                    new GameAction("gulp", delegate { ForkLifterPlayer.instance.Eat(); }),
                    new GameAction("sigh", delegate { Jukebox.PlayOneShot("sigh"); })
                })
            };

            for (int i = 0; i < minigames.Count; i++)
            {
                minigames[i].holder = GamesHolder.Find(minigames[i].name).gameObject;
            }
        }

        private void Update()
        {
            if (GameManager.instance.currentEvent > 0 && GameManager.instance.currentEvent < GameManager.instance.Beatmap.entities.Count)
            currentBeat = GameManager.instance.Beatmap.entities[GameManager.instance.currentEvent].beat;
        }

        public void CallEvent(string event_, float beat)
        {
            string[] details = event_.Split('/');
            MiniGame game = minigames.Find(c => c.name == details[0]);

            try
            {
                GameAction action = game.actions.Find(c => c.actionName == details[1]);
                action.function.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Event not found! May be spelled wrong or it is not implemented.");
            }
        }
    }
}