using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BurstLinq;

using UnityEngine;
using UnityEngine.Pool;

using Jukebox;
using HeavenStudio.Util;
using HeavenStudio.Games;
using HeavenStudio.Common;
using Cysharp.Threading.Tasks;

namespace HeavenStudio
{
    public class GameManager : MonoBehaviour
    {
        const int SoundPoolSizeMin = 32;
        const int SoundPoolSizeMax = 32;

        [Header("Lists")]
        [NonSerialized] public RiqBeatmap Beatmap = new();
        [NonSerialized] public ObjectPool<Sound> SoundObjects;

        [Header("Components")]
        [NonSerialized] public Camera GameCamera, CursorCam, OverlayCamera, StaticCamera;
        [NonSerialized] public CircleCursor CircleCursor;
        [NonSerialized] public GameObject GamesHolder;
        [NonSerialized] public Games.Global.Flash fade;
        [NonSerialized] public Games.Global.Filter filter;

        [Header("Games")]
        Coroutine currentGameSwitchIE;

        [NonSerialized]
        public int currentEvent, currentTempoEvent, currentVolumeEvent, currentSectionEvent,
            currentPreEvent, currentPreSwitch, currentPreSequence;

        public string currentGame { get; private set; }
        public GameObject minigameObj { get; private set; }
        public Minigame minigame { get; private set; }
        public RiqEntity lastSection { get; private set; }
        public RiqEntity currentSection { get; private set; }

        public double endBeat { get; private set; }
        public double startBeat { get; private set; }

        public double nextSectionBeat { get; private set; }
        public double SectionProgress { get; private set; }
        public float MarkerWeight { get; private set; }

        public int MarkerCategory { get; private set; }

        public bool playMode { get; private set; }
        public bool autoplay { get; private set; }
        public bool canInput { get; private set; }

        public bool GameHasSplitColours
        {
            get
            {
                var inf = GetGameInfo(currentGame);
                if (inf == null) return false;
                return inf.splitColorL != null && inf.splitColorR != null;
            }
        }

        bool AudioLoadDone;
        bool ChartLoadError;
        //bool exiting;    Unused value - Marc

        List<double> eventBeats, preSequenceBeats, tempoBeats, volumeBeats, sectionBeats;
        List<RiqEntity> allGameSwitches;

        public event Action<double> onBeatChanged;
        public event Action<RiqEntity, RiqEntity> onSectionChange;
        public event Action<double> onBeatPulse;
        public event Action<double> onPlay;
        public event Action<double> onPause;
        public event Action<double> onUnPause;

        public int BeatmapEntities()
        {
            return Beatmap.Entities.Count + Beatmap.TempoChanges.Count + Beatmap.VolumeChanges.Count + Beatmap.SectionMarkers.Count;
        }

        public static GameManager instance { get; private set; }
        private EventCaller eventCaller;
        public Conductor conductor;

        // average input accuracy (msec)
        List<int> inputOffsetSamples = new List<int>();
        float averageInputOffset = 0;
        public float AvgInputOffset
        {
            get
            {
                return averageInputOffset;
            }
            set
            {
                inputOffsetSamples.Add((int)value);
                averageInputOffset = (float)inputOffsetSamples.Average();
            }
        }

        // input accuracy (%)
        bool skillStarCollected = false, noMiss = true;

        // cleared sections
        List<bool> clearedSections = new List<bool>();
        public bool ClearedSection
        {
            set
            {
                clearedSections.Add(value);
            }
        }

        JudgementManager.JudgementInfo judgementInfo;

        private void Awake()
        {
            instance = this;
            //exiting = false;    Unused value - Marc
        }

        public void Init(bool preLoaded = false)
        {
            AudioLoadDone = false;
            ChartLoadError = false;
            currentPreEvent = 0;
            currentPreSwitch = 0;
            currentPreSequence = 0;

            GameObject filter = new GameObject("filter");
            this.filter = filter.AddComponent<Games.Global.Filter>();

            eventCaller = this.gameObject.AddComponent<EventCaller>();
            eventCaller.GamesHolder = GamesHolder.transform;
            eventCaller.Init(this);

            canInput = true;

            // note: serialize this shit in the inspector //
            GameObject textbox = Instantiate(Resources.Load<GameObject>("Prefabs/Common/Textbox"));
            textbox.name = "Textbox";

            GameObject timingDisp = Instantiate(Resources.Load<GameObject>("Prefabs/Common/Overlays/TimingAccuracy"));
            timingDisp.name = "TimingDisplay";

            GameObject skillStarDisp = Instantiate(Resources.Load<GameObject>("Prefabs/Common/Overlays/SkillStar"));
            skillStarDisp.name = "SkillStar";

            GameObject overlays = Instantiate(Resources.Load<GameObject>("Prefabs/Common/Overlays"));
            overlays.name = "Overlays";

            GoForAPerfect.instance.Disable();
            /////

            SoundByte.BasicCheck();
            SoundObjects = new ObjectPool<Sound>(CreatePooledSound, OnTakePooledSound, OnReturnPooledSound, OnDestroyPooledSound, true, SoundPoolSizeMin, SoundPoolSizeMax);


            if (preLoaded)
            {
                LoadRemix(false);
            }
            else
            {
                RiqFileHandler.ClearCache();
                NewRemix();
            }

            SortEventsList();
            conductor.SetBpm(Beatmap.TempoChanges[0]["tempo"]);
            conductor.SetVolume(Beatmap.VolumeChanges[0]["volume"]);
            conductor.firstBeatOffset = Beatmap.data.offset;

            if (!preLoaded)
            {
                if (Beatmap.Entities.Count == 0)
                {
                    SetGame("noGame");
                }
                else
                {
                    SetCurrentEventToClosest(0, true);
                }
            }

            if (playMode)
            {
                StartCoroutine(WaitReadyAndPlayCo(startBeat, 1f));
            }
        }

        Sound CreatePooledSound()
        {
            GameObject oneShot = new GameObject($"Pooled Scheduled Sound");
            oneShot.transform.SetParent(transform);

            AudioSource audioSource = oneShot.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            Sound snd = oneShot.AddComponent<Sound>();

            oneShot.SetActive(false);

            return snd;
        }

        // Called when an item is returned to the pool using Release
        void OnReturnPooledSound(Sound snd)
        {
            snd.Stop();
        }

        // Called when an item is taken from the pool using Get
        void OnTakePooledSound(Sound snd)
        {
            snd.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        void OnDestroyPooledSound(Sound snd)
        {
            snd.Stop();
            Destroy(snd.gameObject);
        }

        public void NewRemix()
        {
            AudioLoadDone = false;
            Beatmap = new("1", "HeavenStudio");
            Beatmap.data.properties = new(Minigames.propertiesModel);

            RiqEntity t = Beatmap.AddNewTempoChange(0, 120f);
            t.CreateProperty("swingDivision", 1f);

            Beatmap.AddNewVolumeChange(0, 100f);

            Beatmap.data.offset = 0f;
            conductor.musicSource.clip = null;
            RiqFileHandler.WriteRiq(Beatmap);
            AudioLoadDone = true;
        }

        public IEnumerator LoadMusic()
        {
            ChartLoadError = false;
            IEnumerator load = RiqFileHandler.LoadSong();
            while (true)
            {
                object current = load.Current;
                try
                {
                    if (load.MoveNext() == false)
                    {
                        break;
                    }
                    current = load.Current;
                }
                catch (System.IO.FileNotFoundException f)
                {
                    Debug.LogWarning("chart has no music: " + f.Message);
                    conductor.musicSource.clip = null;
                    AudioLoadDone = true;
                    yield break;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load music: {e.Message}");
                    GlobalGameManager.ShowErrorMessage("Error Loading Music", e.Message + "\n\n" + e.StackTrace);
                    AudioLoadDone = true;
                    ChartLoadError = true;
                    yield break;
                }
                yield return current;
            }
            conductor.musicSource.clip = RiqFileHandler.StreamedAudioClip;
            AudioLoadDone = true;
        }

        public void LoadRemix(bool editor = false)
        {
            AudioLoadDone = false;
            ChartLoadError = false;
            try
            {
                Beatmap = RiqFileHandler.ReadRiq();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load remix: {e.Message}");
                GlobalGameManager.ShowErrorMessage("Error Loading RIQ", e.Message + "\n\n" + e.StackTrace);
                ChartLoadError = true;
                return;
            }
            if (!editor)
                StartCoroutine(LoadMusic());
            SortEventsList();
            conductor.SetBpm(Beatmap.TempoChanges[0]["tempo"]);
            conductor.SetVolume(Beatmap.VolumeChanges[0]["volume"]);
            conductor.firstBeatOffset = Beatmap.data.offset;
            if (!playMode)
            {
                Stop(0);
            }
            SetCurrentEventToClosest(0, true);

            if (editor)
            {
                if (Beatmap.data.riqOrigin != "HeavenStudio")
                {
                    string origin = Beatmap.data.riqOrigin?.DisplayName() ?? "Unknown Origin";
                    GlobalGameManager.ShowErrorMessage("Warning",
                        $"This chart came from\n<alpha=#AA>{origin}</color>\nand uses content not included in Heaven Studio.\n\n<color=\"yellow\">You may be able to edit this chart in Heaven Studio to be used in its original program.</color>");
                }
            }
        }

        public void ScoreInputAccuracy(double beat, double accuracy, bool late, double time, float pitch = 1, double margin = 0, float weight = 1, bool doDisplay = true)
        {
            // push the hit event to the timing display
            if (doDisplay)
                TimingAccuracyDisplay.instance.MakeAccuracyVfx(time, pitch, margin, late);

            if (weight > 0 && MarkerWeight > 0)
            {
                judgementInfo.inputs.Add(new JudgementManager.InputInfo
                {
                    beat = beat,
                    accuracyState = accuracy,
                    timeOffset = time,
                    weight = weight * MarkerWeight,
                    category = MarkerCategory
                });
                if (accuracy < Minigame.rankOkThreshold)
                {
                    SkillStarManager.instance.KillStar();
                    GoForAPerfect.instance.Miss();
                    SectionMedalsManager.instance.MakeIneligible();
                    noMiss = false;
                }
            }

            if (SkillStarManager.instance.IsEligible && !skillStarCollected && accuracy >= 1f)
            {
                if (SkillStarManager.instance.DoStarJust())
                    skillStarCollected = true;
            }
        }

        public void DoSectionCompletion(double beat, bool clear, string name, double score)
        {
            judgementInfo.medals.Add(new JudgementManager.MedalInfo
            {
                beat = beat,
                cleared = clear
            });
        }

        public List<Minigames.Minigame> SeekAheadAndPreload(double start, float seekTime = 8f)
        {
            List<Minigames.Minigame> gamesToPreload = new();
            List<RiqEntity> entitiesAtSameBeat = ListPool<RiqEntity>.Get();
            Minigames.Minigame inf;

            // seek ahead to preload games that have assetbundles
            string[] split;
            if (currentPreSwitch < allGameSwitches.Count && currentPreSwitch >= 0)
            {
                while (currentPreSwitch < allGameSwitches.Count && allGameSwitches[currentPreSwitch].beat <= start + seekTime)
                {
                    split = allGameSwitches[currentPreSwitch].datamodel.Split('/');
                    string gameName = split[2];
                    inf = GetGameInfo(gameName);
                    if (inf != null && !(inf.inferred || inf.fxOnly))
                    {
                        if (inf.UsesAssetBundle && !(inf.AssetsLoaded || inf.AlreadyLoading))
                        {
                            gamesToPreload.Add(inf);
                            Debug.Log($"ASYNC loading assetbundles for game {gameName}");
                            inf.LoadAssetsAsync().Forget();
                        }
                    }
                    currentPreSwitch++;
                }
            }
            //then check game entities
            if (currentPreEvent < Beatmap.Entities.Count && currentPreEvent >= 0)
            {
                if (start + seekTime >= eventBeats[currentPreEvent])
                {
                    foreach (RiqEntity entity in Beatmap.Entities)
                    {
                        if (entity.beat == Beatmap.Entities[currentPreEvent].beat && !EventCaller.FXOnlyGames().Contains(eventCaller.GetMinigame(entity.datamodel.Split('/')[0])))
                        {
                            entitiesAtSameBeat.Add(entity);
                        }
                    }
                    SortEventsByPriority(entitiesAtSameBeat);
                    foreach (RiqEntity entity in entitiesAtSameBeat)
                    {
                        string gameName = entity.datamodel.Split('/')[0];
                        inf = GetGameInfo(gameName);
                        if (inf != null && !(inf.inferred || inf.fxOnly))
                        {
                            if (inf.UsesAssetBundle && !inf.AssetsLoaded)
                            {
                                gamesToPreload.Add(inf);
                                Debug.Log($"ASYNC loading assetbundles for game {gameName}");
                                inf.LoadAssetsAsync().Forget();
                            }
                        }
                        currentPreEvent++;
                    }
                }
            }
            ListPool<RiqEntity>.Release(entitiesAtSameBeat);
            return gamesToPreload;
        }

        public void SeekAheadAndDoPreEvent(double start)
        {
            if (currentPreSequence < Beatmap.Entities.Count && currentPreSequence >= 0)
            {
                List<RiqEntity> entitiesInRange = ListPool<RiqEntity>.Get();
                while (currentPreSequence < preSequenceBeats.Count && start >= preSequenceBeats[currentPreSequence])
                {
                    entitiesInRange.Clear();
                    foreach (RiqEntity entity in Beatmap.Entities)
                    {
                        string[] entityDatamodel = entity.datamodel.Split('/');
                        double seekTime = eventCaller.GetGameAction(entityDatamodel[0], entityDatamodel[1]).preFunctionLength;
                        if (entity.beat - seekTime == preSequenceBeats[currentPreSequence])
                        {
                            entitiesInRange.Add(entity);
                        }
                    }
                    SortEventsByPriority(entitiesInRange);

                    foreach (RiqEntity entity in entitiesInRange)
                    {
                        // no, the second parameter doesn't matter here. i don't know a good way to make it work better, tho
                        eventCaller.CallEvent(entity, true, true);
                        currentPreSequence++;
                    }
                }
                ListPool<RiqEntity>.Release(entitiesInRange);
            }
        }

        private void Update()
        {
            if (BeatmapEntities() < 1)
                return;
            if (conductor.WaitingForDsp || !conductor.isPlaying)
                return;
            double clampedBeat = Math.Max(conductor.songPositionInBeatsAsDouble, 0);

            if (currentTempoEvent < Beatmap.TempoChanges.Count && currentTempoEvent >= 0)
            {
                if (conductor.songPositionInBeatsAsDouble >= tempoBeats[currentTempoEvent])
                {
                    conductor.SetBpm(Beatmap.TempoChanges[currentTempoEvent]["tempo"]);
                    currentTempoEvent++;
                }
            }

            if (currentVolumeEvent < Beatmap.VolumeChanges.Count && currentVolumeEvent >= 0)
            {
                if (conductor.songPositionInBeatsAsDouble >= volumeBeats[currentVolumeEvent])
                {
                    conductor.SetVolume(Beatmap.VolumeChanges[currentVolumeEvent]["volume"]);
                    currentVolumeEvent++;
                }
            }

            if (currentSectionEvent < Beatmap.SectionMarkers.Count && currentSectionEvent >= 0)
            {
                if (conductor.songPositionInBeatsAsDouble >= sectionBeats[currentSectionEvent])
                {
                    RiqEntity marker = Beatmap.SectionMarkers[currentSectionEvent];
                    if (!string.IsNullOrEmpty(marker["sectionName"]))
                    {
                        Debug.Log("Section " + marker["sectionName"] + " started");
                        lastSection = currentSection;
                        if (currentSectionEvent < Beatmap.SectionMarkers.Count)
                            currentSection = marker;
                        else
                            currentSection = null;
                        nextSectionBeat = endBeat;
                        foreach (RiqEntity futureSection in Beatmap.SectionMarkers)
                        {
                            if (futureSection.beat < marker.beat) continue;
                            if (futureSection == marker) continue;
                            if (!string.IsNullOrEmpty(futureSection["sectionName"]))
                            {
                                nextSectionBeat = futureSection.beat;
                                break;
                            }
                        }
                        onSectionChange?.Invoke(currentSection, lastSection);
                    }

                    if (OverlaysManager.OverlaysEnabled)
                    {
                        if (PersistentDataManager.gameSettings.perfectChallengeType != PersistentDataManager.PerfectChallengeType.Off)
                        {
                            if (marker["startPerfect"] && GoForAPerfect.instance != null && GoForAPerfect.instance.perfect && !GoForAPerfect.instance.gameObject.activeSelf)
                            {
                                GoForAPerfect.instance.Enable(marker.beat);
                            }
                        }
                    }

                    MarkerWeight = marker["weight"];
                    MarkerCategory = marker["category"];
                    currentSectionEvent++;
                }
            }

            if (conductor.songPositionInBeatsAsDouble >= Math.Ceiling(_playStartBeat) + _pulseTally)
            {
                if (minigame != null) minigame.OnBeatPulse(Math.Ceiling(_playStartBeat) + _pulseTally);
                onBeatPulse?.Invoke(Math.Ceiling(_playStartBeat) + _pulseTally);
                _pulseTally++;
            }

            float seekTime = 8f;
            //seek ahead to preload games that have assetbundles
            SeekAheadAndPreload(clampedBeat, seekTime);
            SeekAheadAndDoPreEvent(clampedBeat);

            if (currentEvent < Beatmap.Entities.Count && currentEvent >= 0)
            {
                List<RiqEntity> entitiesInRange = ListPool<RiqEntity>.Get();
                List<RiqEntity> fxEntities = ListPool<RiqEntity>.Get();
                // allows for multiple events on the same beat to be executed on the same frame, so no more 1-frame delay
                while (currentEvent < eventBeats.Count && clampedBeat >= eventBeats[currentEvent] && this.conductor.isPlaying)
                {
                    fxEntities.Clear();
                    entitiesInRange.Clear();
                    using (PooledObject<List<RiqEntity>> pool = ListPool<RiqEntity>.Get(out List<RiqEntity> currentBeatEntities))
                    {
                        currentBeatEntities = Beatmap.Entities.FindAll(c => c.beat == eventBeats[currentEvent]);
                        foreach (RiqEntity entity in currentBeatEntities)
                        {
                            if (EventCaller.FXOnlyGames().Contains(eventCaller.GetMinigame(entity.datamodel.Split('/')[0])))
                            {
                                fxEntities.Add(entity);
                            }
                            else
                            {
                                entitiesInRange.Add(entity);
                            }
                        }
                    }

                    SortEventsByPriority(fxEntities);
                    SortEventsByPriority(entitiesInRange);

                    // FX entities should ALWAYS execute before gameplay entities
                    foreach (RiqEntity entity in fxEntities)
                    {
                        eventCaller.CallEvent(entity, true);
                        currentEvent++;
                    }

                    foreach (RiqEntity entity in entitiesInRange)
                    {
                        // if game isn't loaded, run inactive event
                        if (entity.datamodel.Split('/')[0] != currentGame)
                        {
                            eventCaller.CallEvent(entity, false);
                        }
                        else
                        {
                            eventCaller.CallEvent(entity, true);
                        }

                        // Thank you to @shshwdr for bring this to my attention
                        currentEvent++;
                    }
                }
                ListPool<RiqEntity>.Release(entitiesInRange);
                ListPool<RiqEntity>.Release(fxEntities);
            }

            if (currentSection == null)
            {
                SectionProgress = 0;
            }
            else
            {
                double currectSectionStart = conductor.GetSongPosFromBeat(currentSection.beat);

                SectionProgress = (conductor.songPosition - currectSectionStart) / (conductor.GetSongPosFromBeat(nextSectionBeat) - currectSectionStart);
            }
        }

        private void LateUpdate()
        {
            OverlaysManager.instance.TogleOverlaysVisibility(Editor.Editor.instance == null || Editor.Editor.instance.fullscreen || ((PersistentDataManager.gameSettings.overlaysInEditor) && (!Editor.Editor.instance.fullscreen)) || HeavenStudio.Editor.GameSettings.InPreview);

            if (!conductor.isPlaying)
                return;

            if (conductor.songPositionInBeatsAsDouble >= Math.Ceiling(_playStartBeat) + _latePulseTally)
            {
                if (minigame != null) minigame.OnLateBeatPulse(Math.Ceiling(_playStartBeat) + _latePulseTally);
                onBeatPulse?.Invoke(Math.Ceiling(_playStartBeat) + _latePulseTally);
                _latePulseTally++;
            }
        }

        public static void PlaySFXArbitrary(double beat, float length, string game, string name, float pitch, float volume, bool looping, int offset)
        {
            if (string.IsNullOrEmpty(name)) return;
            Sound sound;
            if (game == "common")
            {
                sound = SoundByte.PlayOneShot(name, beat, pitch, volume, looping, null, (offset / 1000f));
            }
            else
            {
                SoundByte.PreloadGameAudioClips(game);
                sound = SoundByte.PlayOneShotGame(game + "/" + name, beat, pitch, volume, looping, true, (offset / 1000f));
            }
            if (looping)
            {
                BeatAction.New(null, new() {
                    new(beat + length, () => sound.KillLoop(0)),
                });
            }
        }

        public void PlayAnimationArbitrary(string animator, string animation, float scale)
        {
            Transform animTrans = minigameObj.transform.Find(animator);
            if (animTrans != null && animTrans.TryGetComponent(out Animator anim))
            {
                anim.DoScaledAnimationAsync(animation, scale);
            }
        }

        public void ToggleInputs(bool inputs)
        {
            canInput = inputs;
        }

        public void ToggleAutoplay(bool auto)
        {
            autoplay = auto;
        }

        public void TogglePlayMode(bool mode)
        {
            playMode = mode;
        }

        #region Play Events

        private double _playStartBeat = 0;
        private int _pulseTally = 0;
        private int _latePulseTally = 0;

        public void Play(double beat, float delay = 0f)
        {
            bool paused = conductor.isPaused;
            _playStartBeat = beat;
            _pulseTally = 0;
            _latePulseTally = 0;
            canInput = true;
            if (!paused)
            {
                inputOffsetSamples.Clear();
                averageInputOffset = 0;

                TimingAccuracyDisplay.instance.ResetArrow();
                SkillStarManager.instance.Reset();
                skillStarCollected = false;
                noMiss = true;

                GoForAPerfect.instance.perfect = true;
                GoForAPerfect.instance.Disable();

                SectionMedalsManager.instance.Reset();
                clearedSections.Clear();

                judgementInfo = new JudgementManager.JudgementInfo
                {
                    inputs = new List<JudgementManager.InputInfo>(),
                    medals = new List<JudgementManager.MedalInfo>()
                };

                MarkerWeight = 1;
                MarkerCategory = 0;

                if (playMode && delay > 0)
                {
                    GlobalGameManager.ForceFade(0, delay * 0.5f, delay * 0.5f);
                }
            }

            StartCoroutine(PlayCo(beat, delay));
            //onBeatChanged?.Invoke(beat);
        }

        private IEnumerator PlayCo(double beat, float delay = 0f)
        {
            bool paused = conductor.isPaused;

            if (!paused)
            {
                conductor.SetBpm(Beatmap.TempoChanges[0]["tempo"]);
                conductor.SetVolume(Beatmap.VolumeChanges[0]["volume"]);
                conductor.firstBeatOffset = Beatmap.data.offset;
                conductor.PlaySetup(beat);
                SetCurrentEventToClosest(beat, true);
                Debug.Log("Playing at " + beat);
                KillAllSounds();

                if (delay > 0)
                {
                    yield return new WaitForSeconds(delay);
                }

                Minigame miniGame = null;
                if (minigameObj != null && minigameObj.TryGetComponent<Minigame>(out miniGame))
                {
                    if (miniGame != null)
                    {
                        miniGame.OnPlay(beat);
                    }
                }
                onPlay?.Invoke(beat);

                bool hasStartPerfect = false;
                foreach (RiqEntity marker in Beatmap.SectionMarkers)
                {
                    if (marker["startPerfect"])
                    {
                        hasStartPerfect = true;
                        break;
                    }
                }

                if (OverlaysManager.OverlaysEnabled && !hasStartPerfect)
                {
                    if (PersistentDataManager.gameSettings.perfectChallengeType != PersistentDataManager.PerfectChallengeType.Off)
                    {
                        GoForAPerfect.instance.Enable(0);
                    }
                }
            }
            else
            {
                onUnPause?.Invoke(beat);
            }

            if (playMode)
            {
                CircleCursor.LockCursor(true);
            }
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            conductor.Play(beat);
        }

        public void Pause()
        {
            conductor.Pause();
            Util.SoundByte.PauseOneShots();
            onPause?.Invoke(conductor.songPositionInBeatsAsDouble);
            canInput = false;
        }

        public void Stop(double beat, bool restart = false, float restartDelay = 0f)
        {
            // I feel like I should standardize the names
            if (conductor.isPlaying)
            {
                SkillStarManager.instance.KillStar();
                TimingAccuracyDisplay.instance.StopStarFlash();
                GoForAPerfect.instance.Disable();
                SectionMedalsManager.instance.OnRemixEnd(endBeat, currentSection);
            }

            Minigame miniGame;
            if (minigameObj != null && minigameObj.TryGetComponent<Minigame>(out miniGame))
            {
                if (miniGame != null)
                {
                    miniGame.OnStop(beat);
                }
            }

            conductor.Stop(beat);
            conductor.SetBeat(beat);

            KillAllSounds();
            if (restart)
            {
                Play(0, restartDelay);
            }
            else if (playMode)
            {
                //exiting = true;    Unused value - Marc
                judgementInfo.star = skillStarCollected;
                judgementInfo.perfect = GoForAPerfect.instance.perfect;
                judgementInfo.noMiss = noMiss;
                judgementInfo.time = DateTime.Now;

                JudgementManager.SetPlayInfo(judgementInfo, Beatmap);
                GlobalGameManager.LoadScene("Judgement", 0.35f, 0f, DestroyGame);
                CircleCursor.LockCursor(false);
            }
            else
            {
                conductor.SetBeat(beat);
            }
            Application.backgroundLoadingPriority = ThreadPriority.Normal;
        }

        public void SafePlay(double beat, float delay, bool discord)
        {
            StartCoroutine(WaitReadyAndPlayCo(beat, delay, discord));
        }

        private IEnumerator WaitReadyAndPlayCo(double beat, float delay = 1f, bool discord = true)
        {
            SoundByte.UnloadAudioClips();
            SoundByte.PreloadAudioClipAsync("skillStar");
            SoundByte.PreloadAudioClipAsync("perfectMiss");

            conductor.SetBeat(beat);

            WaitUntil yieldOverlays = new WaitUntil(() => OverlaysManager.OverlaysReady);
            WaitUntil yieldBeatmap = new WaitUntil(() => Beatmap != null && BeatmapEntities() > 0);
            WaitUntil yieldAudio = new WaitUntil(() => AudioLoadDone || (ChartLoadError && !GlobalGameManager.IsShowingDialog));
            WaitUntil yieldGame = null;
            List<Minigames.Minigame> gamesToPreload = SetCurrentEventToClosest(beat, true);
            Debug.Log($"Preloading {gamesToPreload.Count} games");
            if (gamesToPreload.Count > 0)
            {
                yieldGame = new WaitUntil(() => gamesToPreload.All(x => x.AssetsLoaded));
            }

            // wait for overlays to be ready
            // Debug.Log("waiting for overlays");
            yield return yieldOverlays;
            // wait for beatmap to be loaded
            // Debug.Log("waiting for beatmap");
            yield return yieldBeatmap;
            //wait for audio clip to be loaded
            // Debug.Log("waiting for audio");
            yield return yieldAudio;
            //wait for games to be loaded
            // Debug.Log("waiting for minigames");
            if (yieldGame != null)
                yield return yieldGame;

            SkillStarManager.instance.KillStar();
            TimingAccuracyDisplay.instance.StopStarFlash();
            GoForAPerfect.instance.Disable();
            SectionMedalsManager.instance?.Reset();

            if (discord)
            {
                GlobalGameManager.UpdateDiscordStatus(Beatmap["remixtitle"].ToString(), false, true);
            }

            Play(beat, delay);
            yield break;
        }

        public void KillAllSounds()
        {
            // Debug.Log("Killing all sounds");
            SoundObjects.Clear();
            SoundByte.KillOneShots();
        }

        #endregion

        #region List Functions

        public void SortEventsList()
        {
            Beatmap.Entities.Sort((x, y) => x.beat.CompareTo(y.beat));
            Beatmap.TempoChanges.Sort((x, y) => x.beat.CompareTo(y.beat));
            Beatmap.VolumeChanges.Sort((x, y) => x.beat.CompareTo(y.beat));
            Beatmap.SectionMarkers.Sort((x, y) => x.beat.CompareTo(y.beat));

            eventBeats = Beatmap.Entities.Select(c => c.beat).ToList();
            tempoBeats = Beatmap.TempoChanges.Select(c => c.beat).ToList();
            volumeBeats = Beatmap.VolumeChanges.Select(c => c.beat).ToList();
            sectionBeats = Beatmap.SectionMarkers.Select(c => c.beat).ToList();

            allGameSwitches = EventCaller.GetAllInGameManagerList("gameManager", new string[] { "switchGame" });

            preSequenceBeats = new List<double>();
            string[] seekEntityDatamodel;
            foreach (RiqEntity entity in Beatmap.Entities)
            {
                seekEntityDatamodel = entity.datamodel.Split('/');
                double seekTime = eventCaller.GetGameAction(seekEntityDatamodel[0], seekEntityDatamodel[1]).preFunctionLength;
                preSequenceBeats.Add(entity.beat - seekTime);
            }
            preSequenceBeats.Sort();
        }

        void SortEventsByPriority(List<RiqEntity> entities)
        {
            string[] xDatamodel;
            string[] yDatamodel;
            entities.Sort((x, y) =>
            {
                xDatamodel = x.datamodel.Split('/');
                yDatamodel = y.datamodel.Split('/');

                Minigames.GameAction xAction = eventCaller.GetGameAction(xDatamodel[0], xDatamodel[1]);
                Minigames.GameAction yAction = eventCaller.GetGameAction(yDatamodel[0], yDatamodel[1]);

                return yAction.priority.CompareTo(xAction.priority);
            });
        }

        public static double GetClosestInList(List<double> list, double compareTo)
        {
            if (list.Count > 0)
                return list.Aggregate((x, y) => Math.Abs(x - compareTo) < Math.Abs(y - compareTo) ? x : y);
            else
                return double.MinValue;
        }

        public static int GetIndexAfter(List<double> list, double compareTo)
        {
            list.Sort();
            if (list.Count > 0)
            {
                foreach (double item in list)
                {
                    if (item >= compareTo)
                    {
                        return Math.Max(list.IndexOf(item), 0);
                    }
                }
                return list.Count;
            }
            return 0;
        }

        public static int GetIndexBefore(List<double> list, double compareTo)
        {
            list.Sort();
            if (list.Count > 0)
            {
                foreach (double item in list)
                {
                    if (item >= compareTo)
                    {
                        return Math.Max(list.IndexOf(item) - 1, 0);
                    }
                }
                return list.Count - 1;
            }
            return 0;
        }

        public List<Minigames.Minigame> SetCurrentEventToClosest(double beat, bool canPreload = false)
        {
            SortEventsList();
            List<Minigames.Minigame> preload = new();
            onBeatChanged?.Invoke(beat);
            if (Beatmap.Entities.Count > 0)
            {
                currentEvent = GetIndexAfter(eventBeats, beat);
                currentPreEvent = GetIndexAfter(eventBeats, beat);
                currentPreSequence = GetIndexAfter(eventBeats, beat);

                string newGame = Beatmap.Entities[Math.Min(currentEvent, eventBeats.Count - 1)].datamodel.Split(0);

                if (allGameSwitches.Count > 0)
                {
                    int index = GetIndexBefore(allGameSwitches.Select(c => c.beat).ToList(), beat);
                    currentPreSwitch = index;
                    var closestGameSwitch = allGameSwitches[index];
                    if (closestGameSwitch.beat <= beat)
                    {
                        newGame = closestGameSwitch.datamodel.Split(2);
                    }
                    else if (closestGameSwitch.beat > beat)
                    {
                        if (index == 0)
                        {
                            newGame = Beatmap.Entities[0].datamodel.Split(0);
                        }
                        else
                        {
                            if (index - 1 >= 0)
                            {
                                newGame = allGameSwitches[index - 1].datamodel.Split(2);
                            }
                            else
                            {
                                newGame = Beatmap.Entities[Beatmap.Entities.IndexOf(closestGameSwitch) - 1].datamodel.Split(0);
                            }
                        }
                    }
                }

                if (!GetGameInfo(newGame).fxOnly)
                {
                    if (canPreload)
                    {
                        Minigames.Minigame inf = GetGameInfo(newGame);
                        if (inf != null && inf.UsesAssetBundle && !inf.AssetsLoaded)
                        {
                            preload.Add(inf);
                        }
                    }
                    StartCoroutine(WaitAndSetGame(newGame));
                }

                List<RiqEntity> allEnds = EventCaller.GetAllInGameManagerList("gameManager", new string[] { "end" });
                if (allEnds.Count > 0)
                    endBeat = allEnds.Select(c => c.beat).Min();
                else
                    endBeat = conductor.SongLengthInBeatsAsDouble();
            }
            else
            {
                SetGame("noGame");
                endBeat = conductor.SongLengthInBeatsAsDouble();
            }

            if (Beatmap.TempoChanges.Count > 0)
            {
                currentTempoEvent = 0;

                //for tempo changes, just go over all of em until the last one we pass
                for (int t = 0; t < tempoBeats.Count; t++)
                {
                    // Debug.Log("checking tempo event " + t + " against beat " + beat + "( tc beat " + tempoChanges[t] + ")");
                    if (tempoBeats[t] > beat)
                    {
                        break;
                    }
                    currentTempoEvent = t;
                }
                // Debug.Log("currentTempoEvent is now " + currentTempoEvent);
            }

            if (Beatmap.VolumeChanges.Count > 0)
            {
                currentVolumeEvent = 0;

                for (int t = 0; t < volumeBeats.Count; t++)
                {
                    if (volumeBeats[t] > beat)
                    {
                        break;
                    }
                    currentVolumeEvent = t;
                }
            }

            lastSection = null;
            currentSection = null;
            if (Beatmap.SectionMarkers.Count > 0)
            {
                currentSectionEvent = 0;

                for (int t = 0; t < sectionBeats.Count; t++)
                {
                    if (sectionBeats[t] > beat)
                    {
                        break;
                    }
                    currentSectionEvent = t;
                }
            }
            onSectionChange?.Invoke(currentSection, lastSection);

            preload.AddRange(SeekAheadAndPreload(beat));
            return preload;
        }

        #endregion

        /// <summary>
        /// While playing a chart, switches the currently active game
        /// Should only be called by chart entities
        /// </summary>
        /// <param name="game">name of the game to switch to</param>
        /// <param name="beat">beat of the chart entity calling the switch</param>
        /// <param name="flash">hide the screen during the switch</param>
        public void SwitchGame(string game, double beat, bool flash)
        {
            if (game != currentGame)
            {
                if (currentGameSwitchIE != null)
                    StopCoroutine(currentGameSwitchIE);
                currentGameSwitchIE = StartCoroutine(SwitchGameIE(game, beat, flash));
            }
        }

        IEnumerator SwitchGameIE(string game, double beat, bool flash)
        {
            if (flash)
            {
                HeavenStudio.StaticCamera.instance.ToggleCanvasVisibility(false);
            }

            SetGame(game, false);

            Minigame miniGame;
            if (minigameObj != null && minigameObj.TryGetComponent<Minigame>(out miniGame))
            {
                if (miniGame != null)
                {
                    miniGame.OnGameSwitch(beat);
                }
            }

            while (conductor.GetUnSwungBeat(beat + 0.25) > Math.Max(conductor.unswungSongPositionInBeatsAsDouble, 0))
            {
                if (!conductor.isPlaying)
                {
                    HeavenStudio.StaticCamera.instance.ToggleCanvasVisibility(true);
                    SetAmbientGlowToCurrentMinigameColor();
                    StopCoroutine(currentGameSwitchIE);
                }
                yield return null;
            }

            HeavenStudio.StaticCamera.instance.ToggleCanvasVisibility(true);
            SetAmbientGlowToCurrentMinigameColor();
        }

        /// <summary>
        /// Immediately sets the current minigame to the specified game
        /// </summary>
        /// <param name="game"></param>
        /// <param name="useMinigameColor"></param>
        private void SetGame(string game, bool useMinigameColor = true)
        {
            ResetCamera(); // resetting camera before setting new minigame so minigames can set camera values in their awake call - Rasmus

            GameObject prefab = GetGamePrefab(game);
            if (prefab == null) return;

            Destroy(minigameObj);
            minigameObj = Instantiate(prefab);
            if (minigameObj.TryGetComponent<Minigame>(out var minigame))
            {
                this.minigame = minigame;
                minigame.minigameName = game;
                minigame.gameManager = this;
                minigame.conductor = conductor;
            }
            Vector3 originalScale = minigameObj.transform.localScale;
            minigameObj.transform.parent = eventCaller.GamesHolder.transform;
            minigameObj.transform.localScale = originalScale;
            minigameObj.name = game;

            SetCurrentGame(game, useMinigameColor);
        }

        string currentGameRequest = null;
        /// <summary>
        /// Waits for a given game to preload, then sets it as the current game
        /// </summary>
        /// <param name="game"></param>
        /// <param name="useMinigameColor"></param>
        /// <returns></returns>
        private IEnumerator WaitAndSetGame(string game, bool useMinigameColor = true)
        {
            if (game == currentGameRequest)
            {
                yield break;
            }
            currentGameRequest = game;
            var inf = GetGameInfo(game);
            if (inf != null && inf.UsesAssetBundle)
            {
                if (!(inf.AssetsLoaded || inf.AlreadyLoading))
                {
                    // Debug.Log($"ASYNC loading assetbundles for game {game}");
                    inf.LoadAssetsAsync().Forget();
                    yield return new WaitUntil(() => inf.AssetsLoaded);
                }
                SetGame(game, useMinigameColor);
                currentGameRequest = null;
            }
            else
            {
                SetGame(game, useMinigameColor);
                currentGameRequest = null;
            }
        }

        public void DestroyGame()
        {
            SoundByte.UnloadAudioClips();
            SetGame("noGame");
        }

        /// <summary>
        /// Get the game prefab for a given game name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject GetGamePrefab(string name)
        {
            if (name is null or "" or "noGame")
            {
                return Resources.Load<GameObject>($"Games/noGame");
            }

            Minigames.Minigame gameInfo = GetGameInfo(name);
            if (gameInfo != null)
            {
                if (gameInfo.inferred)
                {
                    return Resources.Load<GameObject>($"Games/noGame");
                }
                if (gameInfo.fxOnly)
                {
                    var gameInfos = Beatmap.Entities
                        .Select(x => x.datamodel.Split(0))
                        .Select(x => GetGameInfo(x))
                        .Where(x => x != null)
                        .Where(x => !x.fxOnly)
                        .Where(x => x.LoadableName is not "noGame" or "" or null);
                    if (gameInfos.Count() > 0)
                    {
                        gameInfo = gameInfos.FirstOrDefault();
                        if (gameInfo == null) return Resources.Load<GameObject>($"Games/noGame");
                    }
                    else
                    {
                        return Resources.Load<GameObject>($"Games/noGame");
                    }
                }

                GameObject prefab;
                if (gameInfo.UsesAssetBundle)
                {
                    //game is packed in an assetbundle, load from that instead
                    if (gameInfo.AssetsLoaded && gameInfo.LoadedPrefab != null) return gameInfo.LoadedPrefab;
                    // couldn't load cached prefab, try loading from resources (usually indev games with mispacked assetbundles)
                    Debug.LogWarning($"Failed to load prefab for game {name} from assetbundle, trying Resources...");
                    prefab = Resources.Load<GameObject>($"Games/{name}");
                    if (prefab != null)
                    {
                        return prefab;
                    }
                    else
                    {
                        Debug.LogWarning($"Game {name} not found, using noGame");
                        return Resources.Load<GameObject>($"Games/noGame");
                    }
                }
                // games with no assetbundle (usually indev games)
                prefab = Resources.Load<GameObject>($"Games/{name}");
                if (prefab != null)
                {
                    return prefab;
                }
                else
                {
                    Debug.LogWarning($"Game {name} not found, using noGame");
                    return Resources.Load<GameObject>($"Games/noGame");
                }
            }
            else
            {
                Debug.LogWarning($"Game {name} not found, using noGame");
                return Resources.Load<GameObject>($"Games/noGame");
            }
        }

        public Minigames.Minigame GetGameInfo(string name)
        {
            return eventCaller.GetMinigame(name);
        }

        public bool TryGetMinigame<T>(out T mg) where T : Minigame
        {
            if (minigame is T tempMinigame)
            {
                mg = tempMinigame;
                return true;
            }
            else
            {
                mg = null;
                return false;
            }
        }

        Color colMain;
        public void SetCurrentGame(string game, bool useMinigameColor = true)
        {
            currentGame = game;
            if (GetGameInfo(currentGame) != null)
            {
                colMain = StringUtils.Hex2RGB(GetGameInfo(currentGame).color);
                CircleCursor.SetCursorColors(colMain, StringUtils.Hex2RGB(GetGameInfo(currentGame).splitColorL), StringUtils.Hex2RGB(GetGameInfo(currentGame).splitColorR));
                if (useMinigameColor) HeavenStudio.StaticCamera.instance.SetAmbientGlowColour(colMain, true);
                else HeavenStudio.StaticCamera.instance.SetAmbientGlowColour(Color.black, false);
            }
            else
            {
                CircleCursor.SetCursorColors(Color.white, Color.white, Color.white);
                HeavenStudio.StaticCamera.instance.SetAmbientGlowColour(Color.black, false);
            }
            CircleCursor.ClearTrail(false);
        }

        private void SetAmbientGlowToCurrentMinigameColor()
        {
            if (GetGameInfo(currentGame) != null)
                HeavenStudio.StaticCamera.instance.SetAmbientGlowColour(StringUtils.Hex2RGB(GetGameInfo(currentGame).color), true);
        }

        private bool SongPosLessThanClipLength(float t)
        {
            if (conductor.musicSource.clip != null)
                return conductor.GetSongPosFromBeat(t) < conductor.musicSource.clip.length;
            else
                return true;
        }

        public void ResetCamera()
        {
            HeavenStudio.GameCamera.ResetAdditionalTransforms();
        }
    }
}