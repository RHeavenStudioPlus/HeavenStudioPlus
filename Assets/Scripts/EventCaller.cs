using System;
using System.Linq;
using BurstLinq;
using System.Collections.Generic;
using UnityEngine;
using Jukebox;
using HeavenStudio.Games;

namespace HeavenStudio
{
    public class EventCaller : MonoBehaviour
    {
        public GameManager gameManager { get; private set; }
        public Transform GamesHolder;
        public RiqEntity currentEntity = new RiqEntity();
        public string currentSwitchGame;

        public delegate void EventCallback();

        public static EventCaller instance { get; private set; }

        public Dictionary<string, Minigames.Minigame> minigames = new();


        public Minigames.Minigame GetMinigame(string gameName)
        {
            minigames.TryGetValue(gameName, out var minigame);
            return minigame;
        }

        public Minigames.GameAction GetGameAction(Minigames.Minigame game, string action)
        {
            return game.actions.Find(c => c.actionName == action);
        }

        public Minigames.GameAction GetGameAction(string gameName, string action)
        {
            if (minigames.TryGetValue(gameName, out var minigame))
            {
                return minigame.actions.Find(c => c.actionName == action);
            }
            else
            {
                Debug.LogWarning($"Game {gameName} not found!");
                return null;
            }
        }

        public Minigames.Param GetGameParam(Minigames.Minigame game, string action, string param)
        {
            return GetGameAction(game, action).parameters.Find(c => c.propertyName == param);
        }

        public Minigames.Param GetGameParam(string gameName, string action, string param)
        {
            return GetGameAction(gameName, action).parameters.Find(c => c.propertyName == param);
        }

        public void Init(GameManager mgr)
        {
            gameManager = mgr;
            instance = this;

            currentEntity = new RiqEntity();

            Minigames.Init(this);
        }

        public void CallEvent(RiqEntity entity, bool gameActive)
        {
            string[] details = entity.datamodel.Split('/');
            Minigames.Minigame game = minigames[details[0]];
            try
            {
                currentEntity = entity;

                if (details.Length > 2) currentSwitchGame = details[2];

                Minigames.GameAction action = game.actions.Find(c => c.actionName == details[1]);
                if (gameActive)
                {
                    action.function.Invoke();
                }
                else
                {
                    action.inactiveFunction.Invoke();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Event not found! May be spelled wrong or it is not implemented.\n" + ex);
            }
        }

        public void CallPreEvent(RiqEntity entity)
        {
            string[] details = entity.datamodel.Split('/');
            Minigames.Minigame game = minigames[details[0]];
            try
            {
                currentEntity = entity;

                Minigames.GameAction action = game.actions.Find(c => c.actionName == details[1]);
                action.preFunction.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Event not found! May be spelled wrong or it is not implemented.\n" + ex);
            }
        }

        public static List<RiqEntity> GetAllInGameManagerList(string gameName, string[] include)
        {
            Predicate<RiqEntity> match = c =>
            {
                string[] details = c.datamodel.Split('/');
                return details[0] == gameName && include.Contains(details[1]);
            };
            return instance.gameManager.Beatmap.Entities.FindAll(match);
        }

        public static List<RiqEntity> GetAllInGameManagerListExclude(string gameName, string[] exclude)
        {
            Predicate<RiqEntity> match = c =>
            {
                string[] details = c.datamodel.Split('/');
                return details[0] == gameName && !exclude.Contains(details[1]);
            };
            return instance.gameManager.Beatmap.Entities.FindAll(match);
        }

        public static List<Minigames.Minigame> FXOnlyGames()
        {
            return instance.minigames.Values.ToList().FindAll(c => c.fxOnly);
        }
    }
}