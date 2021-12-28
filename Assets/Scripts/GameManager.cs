using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Starpelly;
using Newtonsoft.Json;
using RhythmHeavenMania.Games;

namespace RhythmHeavenMania
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        private EventCaller eventCaller;

        public Beatmap Beatmap;
        [HideInInspector] public List<Beatmap.Entity> playerEntities;

        public int currentEvent, currentPlayerEvent;

        public TextAsset txt;

        public float startOffset;

        public Camera GameCamera, CursorCam;
        public CircleCursor CircleCursor;

        [Header("Games")]
        Coroutine currentGameSwitchIE;
        public string currentGame;

        public float startBeat;

        private void Awake()
        {
            instance = this;
        }

        // before Start() since this is very important
        private void OnEnable()
        {
            SortEventsList();

            string json = txt.text;
            Beatmap = JsonConvert.DeserializeObject<Beatmap>(json);

            SortEventsList();

            GlobalGameManager.Init();

            eventCaller = GetComponent<EventCaller>();
            eventCaller.Init();
            Conductor.instance.SetBpm(Beatmap.bpm);

            StartCoroutine(Begin());

            // SetCurrentGame(eventCaller.GamesHolder.transform.GetComponentsInChildren<Transform>()[1].name);

            if (Beatmap.entities.Count >= 1)
            {
                SetCurrentGame(Beatmap.entities[0].datamodel.Split('/')[0]);
            }
        }

        private IEnumerator Begin()
        {
            yield return new WaitForSeconds(startOffset);
            Conductor.instance.musicSource.Play();
        }

        private void Update()
        {
            if (Beatmap.entities.Count < 1)
                return;

            if (Input.GetKeyDown(KeyCode.A))
            {
                Conductor.instance.musicSource.time += 3;
                SetCurrentEventToClosest();
                GetGame(currentGame).holder.GetComponent<Minigame>().OnTimeChange();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Conductor.instance.musicSource.time -= 3;
                SetCurrentEventToClosest();
                GetGame(currentGame).holder.GetComponent<Minigame>().OnTimeChange();
            }

            List<float> entities = Beatmap.entities.Select(c => c.beat).ToList();

            if (currentEvent < Beatmap.entities.Count && currentEvent >= 0)
            {
                if (Conductor.instance.songPositionInBeats >= entities[currentEvent])
                {
                    // allows for multiple events on the same beat to be executed on the same frame, so no more 1-frame delay
                    List<Beatmap.Entity> entitesAtSameBeat = Beatmap.entities.FindAll(c => c.beat == Beatmap.entities[currentEvent].beat);

                    for (int i = 0; i < entitesAtSameBeat.Count; i++)
                    {
                        eventCaller.CallEvent(entitesAtSameBeat[i].datamodel);
                        currentEvent++;
                    }

                    // eventCaller.CallEvent(Beatmap.entities[currentEvent].datamodel);

                    // currentEvent++;
                }
            }
        }

        public void SortEventsList()
        {
            Beatmap.entities.Sort((x, y) => x.beat.CompareTo(y.beat));
        }

        public void SetCurrentEventToClosest()
        {
            if (Beatmap.entities.Count > 0)
            {
                List<float> entities = Beatmap.entities.Select(c => c.beat).ToList();
                List<float> entities_p = playerEntities.Select(c => c.beat).ToList();
                currentEvent = entities.IndexOf(Mathp.GetClosestInList(entities, Conductor.instance.songPositionInBeats));
                currentPlayerEvent = entities_p.IndexOf(Mathp.GetClosestInList(entities_p, Conductor.instance.songPositionInBeats));
            }
        }

        public void SwitchGame(string game)
        {
            if (currentGameSwitchIE != null) StopCoroutine(currentGameSwitchIE);
            currentGameSwitchIE = StartCoroutine(SwitchGameIE(game));
        }

        IEnumerator SwitchGameIE(string game)
        {
            this.GetComponent<SpriteRenderer>().enabled = true;

            GetGame(currentGame).holder.GetComponent<Minigame>().OnGameSwitch();

            GetGame(currentGame).holder.SetActive(false);
            GetGame(game).holder.SetActive(true);

            GameCamera.orthographic = true;

            GetGame(game).holder.GetComponent<Minigame>().OnGameSwitch();

            SetCurrentGame(game);

            yield return new WaitForSeconds(0.1666f);

            this.GetComponent<SpriteRenderer>().enabled = false;
        }

        public EventCaller.MiniGame GetGame(string name)
        {
            return eventCaller.minigames.Find(c => c.name == name);
        }

        // never gonna use this
        public EventCaller.MiniGame GetCurrentGame()
        {
            return eventCaller.minigames.Find(c => c.name == transform.GetComponentsInChildren<Transform>()[1].name);
        }

        public void SetCurrentGame(string game)
        {
            currentGame = game;
            CircleCursor.InnerCircle.GetComponent<SpriteRenderer>().color = Colors.Hex2RGB(GetGame(currentGame).color);
        }
    }
}