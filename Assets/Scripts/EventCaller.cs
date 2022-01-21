using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RhythmHeavenMania
{
    public class EventCaller : MonoBehaviour
    {
        public Transform GamesHolder;
        public float currentBeat;
        public float currentLength;
        public float currentValA;
        public string currentSwitchGame;
        public int currentType;

        public delegate void EventCallback();

        public static EventCaller instance { get; private set; }

        public List<Minigames.Minigame> minigames = new List<Minigames.Minigame>()
        {
        };

        public Minigames.Minigame GetMinigame(string gameName)
        {
            return minigames.Find(c => c.name == gameName);
        }

        public Minigames.GameAction GetGameAction(Minigames.Minigame game, string action)
        {
            return game.actions.Find(c => c.actionName == action);
        }

        public void Init()
        {
            instance = this;

            Minigames.Init(this);

            List<Minigames.Minigame> minigamesInBeatmap = new List<Minigames.Minigame>();
            for (int i = 0; i < GameManager.instance.Beatmap.entities.Count; i++)
            {
                if (!minigamesInBeatmap.Contains(minigames.Find(c => c.name == GameManager.instance.Beatmap.entities[i].datamodel.Split('/')[0])) && GameManager.instance.Beatmap.entities[i].datamodel.Split('/')[0] != "gameManager")
                {
                    minigamesInBeatmap.Add(minigames.Find(c => c.name == GameManager.instance.Beatmap.entities[i].datamodel.Split('/')[0]));
                }
            }

            for (int i = 0; i < minigamesInBeatmap.Count; i++)
            {
                minigames[minigames.FindIndex(c => c.name == minigamesInBeatmap[i].name)].holder = Resources.Load<GameObject>($"Games/{minigamesInBeatmap[i].name}");
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
            Minigames.Minigame game = minigames.Find(c => c.name == details[0]);

            try
            {
                currentLength = GameManager.instance.Beatmap.entities[GameManager.instance.currentEvent].length;
                currentType = GameManager.instance.Beatmap.entities[GameManager.instance.currentEvent].type;
                currentValA = GameManager.instance.Beatmap.entities[GameManager.instance.currentEvent].valA;

                if (details.Length > 2) currentSwitchGame = details[2];

                Minigames.GameAction action = game.actions.Find(c => c.actionName == details[1]);
                action.function.Invoke();

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