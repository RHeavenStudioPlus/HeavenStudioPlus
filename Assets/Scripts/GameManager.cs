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
        [Header("Lists")]
        public Beatmap Beatmap = new Beatmap();
        [HideInInspector] public List<Beatmap.Entity> playerEntities = new List<Beatmap.Entity>();
        private List<GameObject> preloadedGames = new List<GameObject>();
        public List<GameObject> SoundObjects = new List<GameObject>();

        [Header("Components")]
        public TextAsset txt;
        public Camera GameCamera, CursorCam;
        public CircleCursor CircleCursor;
        [HideInInspector] public GameObject GamesHolder;

        [Header("Games")]
        public string currentGame;
        Coroutine currentGameSwitchIE;

        [Header("Properties")]
        public int currentEvent, currentTempoEvent;
        public float startOffset;
        public bool playOnStart;
        public float startBeat;
        private GameObject currentGameO;
        public bool autoplay;
        public int BeatmapEntities()
        {
            return Beatmap.entities.Count + Beatmap.tempoChanges.Count;
        }

        public static GameManager instance { get; private set; }
        private EventCaller eventCaller;

        private void Awake()
        {
            // autoplay = true;
            instance = this;
        }

        public void Init()
        {
            this.transform.localScale = new Vector3(30000000, 30000000);
            SpriteRenderer sp = this.gameObject.AddComponent<SpriteRenderer>();
            sp.enabled = false;
            sp.color = Color.black;
            sp.sprite = Resources.Load<Sprite>("Sprites/GeneralPurpose/Square");
            sp.sortingOrder = 30000;
            // this.gameObject.layer = 3;

            if (txt != null)
            {
                string json = txt.text;
                Beatmap = JsonConvert.DeserializeObject<Beatmap>(json);
            }
            else
            {
                Beatmap = new Beatmap();
                Beatmap.bpm = 120f;
            }

            SortEventsList();

            GlobalGameManager.Init();

            eventCaller = this.gameObject.AddComponent<EventCaller>();
            eventCaller.GamesHolder = GamesHolder.transform;
            eventCaller.Init();
            Conductor.instance.SetBpm(Beatmap.bpm);

            if (playOnStart)
            {
                Play(startBeat);
            }

            // SetCurrentGame(eventCaller.GamesHolder.transform.GetComponentsInChildren<Transform>()[1].name);

            if (Beatmap.entities.Count >= 1)
            {
                SetCurrentGame(Beatmap.entities[0].datamodel.Split(0));
                SetGame(Beatmap.entities[0].datamodel.Split(0));
            }
            else
            {
                SetGame("noGame");
            }
        }

        public void LoadRemix(string json)
        {
            SortEventsList();

            Beatmap = JsonConvert.DeserializeObject<Beatmap>(json);
            Conductor.instance.SetBpm(Beatmap.bpm);
            Stop(0);
            SetCurrentEventToClosest(0);

            if (Beatmap.entities.Count >= 1)
            {
                SetCurrentGame(Beatmap.entities[0].datamodel.Split(0));
                SetGame(Beatmap.entities[0].datamodel.Split(0));
            }
            else
            {
                SetGame("noGame");
            }
        }

        // LateUpdate works a bit better but causes a bit of bugs, so remind me to fix those eventually
        private void Update()
        {
            if (Beatmap.entities.Count < 1)
                return;
            if (!Conductor.instance.isPlaying)
                return;

            List<float> entities = Beatmap.entities.Select(c => c.beat).ToList();
            List<float> tempoChanges = Beatmap.tempoChanges.Select(c => c.beat).ToList();

            if (currentEvent < Beatmap.entities.Count && currentEvent >= 0)
            {
                if (Conductor.instance.songPositionInBeats >= entities[currentEvent] /*&& SongPosLessThanClipLength(Conductor.instance.songPositionInBeats)*/)
                {
                    // allows for multiple events on the same beat to be executed on the same frame, so no more 1-frame delay
                    var entitesAtSameBeat   = Beatmap.entities.FindAll(c => c.beat == Beatmap.entities[currentEvent].beat && !EventCaller.FXOnlyGames().Contains(EventCaller.instance.GetMinigame(c.datamodel.Split('/')[0])));
                    var fxEntities = Beatmap.entities.FindAll(c => c.beat == Beatmap.entities[currentEvent].beat && EventCaller.FXOnlyGames().Contains(EventCaller.instance.GetMinigame(c.datamodel.Split('/')[0])));

                    // FX entities should ALWAYS execute before gameplay entities
                    for (int i = 0; i < fxEntities.Count; i++)
                    {
                        eventCaller.CallEvent(fxEntities[i].datamodel);
                        currentEvent++;
                    }

                    for (int i = 0; i < entitesAtSameBeat.Count; i++)
                    {
                        var entity = entitesAtSameBeat[i];
                        // if game isn't loaded, preload game so whatever event that would be called will still run outside if needed
                        if (entitesAtSameBeat[i].datamodel.Split('/')[0] != currentGame && !preloadedGames.Contains(preloadedGames.Find(c => c.name == entitesAtSameBeat[i].datamodel.Split('/')[0])))
                        {
                            PreloadGame(entitesAtSameBeat[i].datamodel.Split('/')[0]);
                        }
                        eventCaller.CallEvent(entitesAtSameBeat[i].datamodel);

                        // Thank you to @shshwdr for bring this to my attention
                        currentEvent++;
                    }

                    // currentEvent += gameManagerEntities.Count;
                }
            }

            if (currentTempoEvent < Beatmap.tempoChanges.Count && currentTempoEvent >= 0)
            {
                if (Conductor.instance.songPositionInBeats >= tempoChanges[currentTempoEvent])
                {
                    Conductor.instance.songBpm = Beatmap.tempoChanges[currentTempoEvent].tempo;
                    Conductor.instance.timeSinceLastTempoChange = Time.time;
                    currentTempoEvent++;
                }
            }
        }

        #region Play Events

        public void Play(float beat)
        {
            StartCoroutine(PlayCo(beat));
        }

        private IEnumerator PlayCo(float beat)
        {
            yield return null;
            bool paused = Conductor.instance.isPaused;

            Conductor.instance.SetBpm(Beatmap.bpm);

            Conductor.instance.Play(beat);
            if (!paused)
            {
                SetCurrentEventToClosest(beat);
            }

            for (int i = 0; i < SoundObjects.Count; i++) Destroy(SoundObjects[i].gameObject);
        }

        public void Pause()
        {
            Conductor.instance.Pause();
        }

        public void Stop(float beat)
        {
            Conductor.instance.Stop(beat);
            SetCurrentEventToClosest(beat);
        }

        #endregion

        #region List Functions

        public void SortEventsList()
        {
            Beatmap.entities.Sort((x, y) => x.beat.CompareTo(y.beat));
            Beatmap.tempoChanges.Sort((x, y) => x.beat.CompareTo(y.beat));
        }

        public void SetCurrentEventToClosest(float beat)
        {
            SortEventsList();
            if (Beatmap.entities.Count > 0)
            {
                List<float> entities = Beatmap.entities.Select(c => c.beat).ToList();

                currentEvent = entities.IndexOf(Mathp.GetClosestInList(entities, beat));

                var gameSwitchs = Beatmap.entities.FindAll(c => c.datamodel.Split(1) == "switchGame");

                string newGame = Beatmap.entities[currentEvent].datamodel.Split(0);

                if (gameSwitchs.Count > 0)
                {
                    int index = gameSwitchs.FindIndex(c => c.beat == Mathp.GetClosestInList(gameSwitchs.Select(c => c.beat).ToList(), beat));
                    var closestGameSwitch = gameSwitchs[index];
                    if (closestGameSwitch.beat <= beat)
                    {
                        newGame = closestGameSwitch.datamodel.Split(2);
                    }
                    else if (closestGameSwitch.beat > beat)
                    {
                        if (index == 0)
                        {
                            newGame = Beatmap.entities[0].datamodel.Split(0);
                        }
                        else
                        {
                            if (index - 1 >= 0)
                            {
                                newGame = gameSwitchs[index - 1].datamodel.Split(2);
                            }
                            else
                            {
                                newGame = Beatmap.entities[Beatmap.entities.IndexOf(closestGameSwitch) - 1].datamodel.Split(0);
                            }
                        }
                    }
                    // newGame = gameSwitchs[gameSwitchs.IndexOf(gameSwitchs.Find(c => c.beat == Mathp.GetClosestInList(gameSwitchs.Select(c => c.beat).ToList(), beat)))].datamodel.Split(2);
                }

                if (!GetGameInfo(newGame).fxOnly)
                {
                    SetGame(newGame);
                }
            }
            else
            {
                SetGame("noGame");
            }

            if (Beatmap.tempoChanges.Count > 0)
            {
                List<float> tempoChanges = Beatmap.tempoChanges.Select(c => c.beat).ToList();

                currentTempoEvent = tempoChanges.IndexOf(Mathp.GetClosestInList(tempoChanges, beat));
            }
        }

        #endregion

        public void SwitchGame(string game)
        {
            if (game != currentGame)
            {
                if (currentGameSwitchIE != null)
                    StopCoroutine(currentGameSwitchIE);
                currentGameSwitchIE = StartCoroutine(SwitchGameIE(game));
            }
        }

        IEnumerator SwitchGameIE(string game)
        {
            this.GetComponent<SpriteRenderer>().enabled = true;

            SetGame(game);
            GameCamera.transform.localPosition = new Vector3(GameCamera.transform.localPosition.x, GameCamera.transform.localPosition.y, -10);

            yield return new WaitForSeconds(0.1f);

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
                    if (preloadedGames[i].gameObject != null)
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
            }

            if (instantiate)
            {
                currentGameO = Instantiate(GetGame(game));
                currentGameO.transform.parent = eventCaller.GamesHolder.transform;
                currentGameO.name = game;
            }

            /*if (onGameSwitch)
            {
                if (GetGame(currentGame).GetComponent<Minigame>() != null)
                    GetGame(game).GetComponent<Minigame>().OnGameSwitch();
            }*/

            SetCurrentGame(game);
        }

        private void PreloadGame(string game)
        {
            if (preloadedGames.Contains(preloadedGames.Find(c => c.name == game)))
                return;

            var g = Instantiate(GetGame(game));
            g.transform.parent = eventCaller.GamesHolder.transform;
            g.SetActive(false);
            g.name = game;
            preloadedGames.Add(g);
        }

        public GameObject GetGame(string name)
        {
            var gameInfo = GetGameInfo(name);
            if (gameInfo != null)
            {
                if (gameInfo.fxOnly)
                {
                    name = Beatmap.entities.FindAll(c => {
                            var gameName = c.datamodel.Split(0);
                            var newGameInfo = GetGameInfo(gameName);
                            if (newGameInfo == null)
                                return false;
                            else
                                return !newGameInfo.fxOnly;
                        }).ToList()[0].datamodel.Split(0);
                }
            }
            return Resources.Load<GameObject>($"Games/{name}");
        }

        public Minigames.Minigame GetGameInfo(string name)
        {
            return EventCaller.instance.minigames.Find(c => c.name == name);
        }

        // never gonna use this
        public Minigames.Minigame GetCurrentGame()
        {
            return eventCaller.minigames.Find(c => c.name == transform.GetComponentsInChildren<Transform>()[1].name);
        }

        public void SetCurrentGame(string game)
        {
            currentGame = game;
            if (GetGameInfo(currentGame) != null) CircleCursor.InnerCircle.GetComponent<SpriteRenderer>().color = Colors.Hex2RGB(GetGameInfo(currentGame).color);
            else
                CircleCursor.InnerCircle.GetComponent<SpriteRenderer>().color = Color.white;
        }

        private bool SongPosLessThanClipLength(float t)
        {
            if (Conductor.instance.musicSource.clip != null)
                return Conductor.instance.GetSongPosFromBeat(t) < Conductor.instance.musicSource.clip.length;
            else
                return true;
        }
    }
}