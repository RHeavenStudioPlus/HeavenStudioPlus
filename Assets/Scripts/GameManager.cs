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

        [Header("Games")]
        Coroutine currentGameSwitchIE;
        private string currentGame;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
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

            currentGame = eventCaller.GamesHolder.GetComponentsInChildren<Transform>()[1].name;
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
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Conductor.instance.musicSource.time -= 3;
                GameManager.instance.SetCurrentEventToClosest();
            }

            List<float> entities = Beatmap.entities.Select(c => c.beat).ToList();

            if (currentEvent < Beatmap.entities.Count && currentEvent >= 0)
            {
                if (Conductor.instance.songPositionInBeats >= entities[currentEvent])
                {
                    eventCaller.CallEvent(Beatmap.entities[currentEvent].datamodel);

                    currentEvent++;
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

            eventCaller.minigames.Find(c => c.name == currentGame).holder.SetActive(false);
            eventCaller.minigames.Find(c => c.name == game).holder.SetActive(true);
            eventCaller.minigames.Find(c => c.name == game).holder.GetComponent<Minigame>().OnGameSwitch();
            currentGame = game;

            yield return new WaitForSeconds(0.1666f);


            this.GetComponent<SpriteRenderer>().enabled = false;
        }

        private void OnGUI()
        {
            // GUI.Box(new Rect(0, 0, 300, 50), $"SongPosInBeats: {Conductor.instance.songPositionInBeats}");
        }
    }
}