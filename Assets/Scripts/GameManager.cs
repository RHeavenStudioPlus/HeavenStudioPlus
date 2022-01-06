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
        [HideInInspector] public List<Beatmap.Entity> playerEntities = new List<Beatmap.Entity>();

        public int currentEvent, currentPlayerEvent;

        public TextAsset txt;

        public float startOffset;

        public Camera GameCamera, CursorCam;
        public CircleCursor CircleCursor;

        [Header("Games")]
        Coroutine currentGameSwitchIE;
        public string currentGame;

        public float startBeat;

        private GameObject currentGameO;

        private List<GameObject> preloadedGames = new List<GameObject>();

        [HideInInspector] public GameObject GamesHolder;

        public bool playOnStart;

        private void Awake()
        {
            instance = this;
        }

        // before Start() since this is very important
        private void Start()
        {
            this.transform.localScale = new Vector3(3000, 3000);
            SpriteRenderer sp = this.gameObject.AddComponent<SpriteRenderer>();
            sp.enabled = false;
            sp.color = Color.black;
            sp.sprite = Resources.Load<Sprite>("Sprites/GeneralPurpose/Square");
            sp.sortingOrder = 30000;
            this.gameObject.layer = 3;

            string json = txt.text;
            Beatmap = JsonConvert.DeserializeObject<Beatmap>(json);

            SortEventsList();

            GlobalGameManager.Init();

            eventCaller = this.gameObject.AddComponent<EventCaller>();
            eventCaller.GamesHolder = GamesHolder.transform;
            eventCaller.Init();
            Conductor.instance.SetBpm(Beatmap.bpm);

            if (playOnStart) StartCoroutine(Begin());

            // SetCurrentGame(eventCaller.GamesHolder.transform.GetComponentsInChildren<Transform>()[1].name);

            if (Beatmap.entities.Count >= 1)
            {
                SetCurrentGame(Beatmap.entities[0].datamodel.Split('/')[0]);
                SetGame(Beatmap.entities[0].datamodel.Split('/')[0]);
            }
        }

        private IEnumerator Begin()
        {
            yield return new WaitForSeconds(startOffset);
            Conductor.instance.Play(startBeat);
        }

        private void Update()
        {
            if (Beatmap.entities.Count < 1)
                return;
            if (!Conductor.instance.musicSource.isPlaying)
                return;

            if (Input.GetKeyDown(KeyCode.A))
            {
                Conductor.instance.SetTime(Conductor.instance.songPositionInBeats + 3);

                GetGame(currentGame).holder.GetComponent<Minigame>().OnTimeChange();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Conductor.instance.SetTime(Conductor.instance.songPositionInBeats - 3);

                GetGame(currentGame).holder.GetComponent<Minigame>().OnTimeChange();
            }

            List<float> entities = Beatmap.entities.Select(c => c.beat).ToList();

            if (currentEvent < Beatmap.entities.Count && currentEvent >= 0)
            {
                if (Conductor.instance.songPositionInBeats >= entities[currentEvent])
                {
                    // allows for multiple events on the same beat to be executed on the same frame, so no more 1-frame delay
                    var entitesAtSameBeat   = Beatmap.entities.FindAll(c => c.beat == Beatmap.entities[currentEvent].beat && c.datamodel.Split('/')[0] != "gameManager");
                    var gameManagerEntities = Beatmap.entities.FindAll(c => c.beat == Beatmap.entities[currentEvent].beat && c.datamodel.Split('/')[0] == "gameManager");

                    // GameManager entities should ALWAYS execute before gameplay entities
                    for (int i = 0; i < gameManagerEntities.Count; i++)
                    {
                        eventCaller.CallEvent(gameManagerEntities[i].datamodel);
                    }

                    for (int i = 0; i < entitesAtSameBeat.Count; i++)
                    {
                        // if game isn't loaded, preload game so whatever event that would be called will still run outside if needed
                        if (entitesAtSameBeat[i].datamodel.Split('/')[0] != currentGame && !preloadedGames.Contains(preloadedGames.Find(c => c.name == entitesAtSameBeat[i].datamodel.Split('/')[0])))
                        {
                            PreloadGame(entitesAtSameBeat[i].datamodel.Split('/')[0]);
                        }
                        eventCaller.CallEvent(entitesAtSameBeat[i].datamodel);
                    }

                    currentEvent += entitesAtSameBeat.Count + gameManagerEntities.Count;
                }
            }
        }

        public void SortEventsList()
        {
            Beatmap.entities.Sort((x, y) => x.beat.CompareTo(y.beat));
        }

        public void SetCurrentEventToClosest(float beat)
        {
            if (Beatmap.entities.Count > 0)
            {
                List<float> entities = Beatmap.entities.Select(c => c.beat).ToList();
                if (playerEntities != null)
                {
                    List<float> entities_p = playerEntities.Select(c => c.beat).ToList();
                    currentPlayerEvent = entities_p.IndexOf(Mathp.GetClosestInList(entities_p, beat));
                }
                currentEvent = entities.IndexOf(Mathp.GetClosestInList(entities, beat));

                print(currentEvent);

                string newGame = Beatmap.entities[currentEvent].datamodel.Split('/')[0];
                if (newGame != currentGame)
                {
                    SwitchGame(newGame);
                }
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

            SetGame(game);

            yield return new WaitForSeconds(0.1666f);

            this.GetComponent<SpriteRenderer>().enabled = false;
        }

        private void SetGame(string game, bool onGameSwitch = true)
        {
            Destroy(currentGameO);

            var instantiate = true;

            if (preloadedGames.Count > 0)
            {
                for (int i = 0; i < preloadedGames.Count; i++)
                {
                    if (preloadedGames[i].gameObject.name == game)
                    {
                        preloadedGames[i].SetActive(true);
                        currentGameO = preloadedGames[i];
                        preloadedGames.Remove(preloadedGames[i]);
                        instantiate = false;
                    }
                }
            }

            if (instantiate)
            {
                currentGameO = Instantiate(GetGame(game).holder);
                currentGameO.transform.parent = eventCaller.GamesHolder.transform;
                currentGameO.name = game;
            }

            GameCamera.orthographic = true;

            if (onGameSwitch)
            {
                if (GetGame(currentGame).holder.GetComponent<Minigame>() != null)
                    GetGame(game).holder.GetComponent<Minigame>().OnGameSwitch();
            }

            SetCurrentGame(game);
        }

        private void PreloadGame(string game)
        {
            if (preloadedGames.Contains(preloadedGames.Find(c => c.name == game)))
                return;

            var g = Instantiate(GetGame(game).holder);
            g.transform.parent = eventCaller.GamesHolder.transform;
            g.SetActive(false);
            g.name = game;
            preloadedGames.Add(g);
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