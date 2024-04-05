using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

using Jukebox;

using HeavenStudio.Games.Scripts_CatchOfTheDay;
using HeavenStudio.Common;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;

    public static class RvlCatchOfTheDayLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("catchOfTheDay", "Catch of the Day", "b5dede", false, false, new List<GameAction>()
            {
                new GameAction("fish1", "Quicknibble")
                {
                    function = delegate { var e = eventCaller.currentEntity; CatchOfTheDay.Cue_Fish01(e); CatchOfTheDay.Instance.NewLake(e); },
                    inactiveFunction = delegate { var e = eventCaller.currentEntity; CatchOfTheDay.Cue_Fish01(e); },
                    defaultLength = 3f,
                    parameters = new List<Param>()
                    {
                        new Param("layout", CatchOfTheDay.FishLayout.Random, "Layout", "Set the layout for the scene."),
                        new Param("useCustomColor", false, "Custom Color", "Set whether or not to use a custom color.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "colorTop", "colorBottom" })
                        }),
                        new Param("colorTop",    new Color(0.7098039f, 0.8705882f, 0.8705882f), "Top Color",    "The color for the top part of the background."),
                        new Param("colorBottom", new Color(0.4666667f, 0.7372549f, 0.8196079f), "Bottom Color", "The color for the bottom part of the background."),
                        new Param("sceneDelay", new EntityTypes.Float(0f, 32f, 2f), "Scene Change Delay", "Amount of beats to wait before changing to the next scene."),
                        new Param("fgManta", false, "Foreground Stingray", "Spawn a stingray in the foreground of the scene."),
                        new Param("bgManta", false, "Background Stingray", "Spawn a stingray in the background of the scene."),
                        new Param("schoolFish", false, "School of Fish", "Spawn a school of fish to as a distraction.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "fishDensity" })
                        }),
                        new Param("fishDensity", new EntityTypes.Float(0f, 1f, 1f), "Fish Density", "Set the density for the fish in the school."),
                    },
                },
                new GameAction("fish2", "Pausegill")
                {
                    function = delegate { var e = eventCaller.currentEntity; CatchOfTheDay.Cue_Fish02(e); CatchOfTheDay.Instance.NewLake(e); },
                    inactiveFunction = delegate { var e = eventCaller.currentEntity; CatchOfTheDay.Cue_Fish02(e); },
                    defaultLength = 4f,
                    parameters = new List<Param>()
                    {
                        new Param("countIn", false, "Count-In", "Play the \"And Go!\" sound effect as a count in to the cue."),
                        new Param("layout", CatchOfTheDay.FishLayout.Random, "Layout", "Set the layout for the scene."),
                        new Param("useCustomColor", false, "Custom Color", "Set whether or not to use a custom color.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "colorTop", "colorBottom" })
                        }),
                        new Param("colorTop",    new Color(0.7098039f, 0.8705882f, 0.8705882f), "Top Color",    "The color for the top part of the background."),
                        new Param("colorBottom", new Color(0.4666667f, 0.7372549f, 0.8196079f), "Bottom Color", "The color for the bottom part of the background."),
                        new Param("sceneDelay", new EntityTypes.Float(0f, 32f, 2f), "Scene Change Delay", "Amount of beats to wait before changing to the next scene."),
                        new Param("fgManta", false, "Foreground Stingray", "Spawn a stingray in the foreground of the scene."),
                        new Param("bgManta", false, "Background Stingray", "Spawn a stingray in the background of the scene."),
                        new Param("schoolFish", false, "School of Fish", "Spawn a school of fish to as a distraction.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "fishDensity" })
                        }),
                        new Param("fishDensity", new EntityTypes.Float(0f, 1f, 1f), "Fish Density", "Set the density for the fish in the school."),
                    },
                },
                new GameAction("fish3", "Threefish")
                {
                    function = delegate { var e = eventCaller.currentEntity; CatchOfTheDay.Cue_Fish03(e); CatchOfTheDay.Instance.NewLake(e); },
                    inactiveFunction = delegate { var e = eventCaller.currentEntity; CatchOfTheDay.Cue_Fish03(e); },
                    defaultLength = 5.5f,
                    parameters = new List<Param>()
                    {
                        new Param("countIn", false, "Count-In", "Play the \"One Two Three Go!\" sound effect as a count in to the cue."),
                        new Param("fakeOut", false, "Fake-Out", "If enabled, a quicknibble will be shown initially, before being chased away by the threefish."),
                        new Param("layout", CatchOfTheDay.FishLayout.Random, "Layout", "Set the layout for the scene."),
                        new Param("useCustomColor", false, "Custom Color", "Set whether or not to use a custom color.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "colorTop", "colorBottom" })
                        }),
                        new Param("colorTop",    new Color(0.7098039f, 0.8705882f, 0.8705882f), "Top Color",    "The color for the top part of the background."),
                        new Param("colorBottom", new Color(0.4666667f, 0.7372549f, 0.8196079f), "Bottom Color", "The color for the bottom part of the background."),
                        new Param("sceneDelay", new EntityTypes.Float(0f, 32f, 2f), "Scene Change Delay", "Amount of beats to wait before changing to the next scene."),
                        new Param("fgManta", false, "Foreground Stingray", "Spawn a stingray in the foreground of the scene."),
                        new Param("bgManta", false, "Background Stingray", "Spawn a stingray in the background of the scene."),
                        new Param("schoolFish", false, "School of Fish", "Spawn a school of fish to as a distraction.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "fishDensity" })
                        }),
                        new Param("fishDensity", new EntityTypes.Float(0f, 1f, 1f), "Fish Density", "Set the density for the fish in the school."),
                    },
                },
                new GameAction("moveAngler", "Move Angler")
                {
                    function = delegate { var e = eventCaller.currentEntity; CatchOfTheDay.Instance.SetAnglerMovement(e); },
                    defaultLength = 1f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("doMove", false, "Move", "Select this option if you want to move Ann.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "endMoveX", "endMoveY" } ),
                            new Param.CollapseParam((x, e) => (bool)x && (Util.EasingFunction.Ease)e["ease"] != Util.EasingFunction.Ease.Instant, new string[] { "startMoveX", "startMoveY" }),
                            new Param.CollapseParam((_, e) => (bool)e["doMove"] || (bool)e["doRotate"] || (bool)e["doScale"], new string[] { "ease" })
                        }),
                        new Param("startMoveX", new EntityTypes.Float(-20f, 20f, 0f), "Start X", "Set the X position from which to move."),
                        new Param("startMoveY", new EntityTypes.Float(-20f, 20f, 0f), "Start Y", "Set the Y position from which to move."),
                        new Param("endMoveX", new EntityTypes.Float(-20f, 20f, 0f), "End X", "Set the X position to which to move."),
                        new Param("endMoveY", new EntityTypes.Float(-20f, 20f, 0f), "End Y", "Set the Y position to which to move."),
                        new Param("doRotate", false, "Rotate", "Select this option if you want to rotate Ann.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "endRotDegrees" } ),
                            new Param.CollapseParam((x, e) => (bool)x && (Util.EasingFunction.Ease)e["ease"] != Util.EasingFunction.Ease.Instant, new string[] { "startRotDegrees" }),
                            new Param.CollapseParam((_, e) => (bool)e["doMove"] || (bool)e["doRotate"] || (bool)e["doScale"], new string[] { "ease" })
                        }),
                        new Param("startRotDegrees", new EntityTypes.Float(-360f, 360f, 0f), "Start Rotation", "Set the amount of degrees at which to begin rotating."),
                        new Param("endRotDegrees", new EntityTypes.Float(-360f, 360f, 0f), "End Rotation", "Set the amount of degrees at which to finish rotating."),
                        new Param("doScale", false, "Scale", "Select this option if you want to change Ann's scale.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "endScaleX", "endScaleY" } ),
                            new Param.CollapseParam((x, e) => (bool)x && (Util.EasingFunction.Ease)e["ease"] != Util.EasingFunction.Ease.Instant, new string[] { "startScaleX", "startScaleY" }),
                            new Param.CollapseParam((_, e) => (bool)e["doMove"] || (bool)e["doRotate"] || (bool)e["doScale"], new string[] { "ease" })
                        }),
                        new Param("startScaleX", new EntityTypes.Float(-5f, 5f, 1f), "Start Scale X", "Set the desired scale on the X axis at which to start."),
                        new Param("startScaleY", new EntityTypes.Float(-5f, 5f, 1f), "Start Scale Y", "Set the desired scale on the Y axis at which to start."),
                        new Param("endScaleX", new EntityTypes.Float(-5f, 5f, 1f), "End Scale X", "Set the desired scale on the X axis at which to end."),
                        new Param("endScaleY", new EntityTypes.Float(-5f, 5f, 1f), "End Scale Y", "Set the desired scale on the Y axis at which to end."),
                        new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing for the action.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, e) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant && (bool)e["doMove"], new string[] { "startMoveX", "startMoveY" }),
                            new Param.CollapseParam((x, e) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant && (bool)e["doRotate"], new string[] { "startRotDegrees" }),
                            new Param.CollapseParam((x, e) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant && (bool)e["doScale"], new string[] { "startScaleX", "startScaleY" }),
                        }),
                        new Param("sticky", false, "Follow Camera", "Select this to make Ann follow the camera."),
                    }
                }
            },
            new List<string>() {"rvl", "normal"},
            "rvlfishing", "en"
            , chronologicalSortKey: 21
            );
        }
    }
}

namespace HeavenStudio.Games
{
    public class CatchOfTheDay : Minigame
    {
        /*
        BIG LIST OF TODOS
        - wait for upscale
        */
        protected const int MAX_LAKES = 50;

        public static CatchOfTheDay Instance
        {
            get
            {
                if (GameManager.instance.minigame is CatchOfTheDay instance)
                    return instance;
                return null;
            }
        }

        [SerializeField] Animator Angler;
        [SerializeField] GameObject LakeScenePrefab;
        [SerializeField] Transform LakeSceneHolder;

        public int? LastLayout;
        public Dictionary<RiqEntity, LakeScene> ActiveLakes = new();

        public static Dictionary<RiqEntity, MultiSound> FishSounds = new();

        private List<RiqEntity> _AllFishes;
        
        [SerializeField] Transform AnglerTransform;
        private bool _AnglerIsMoving = false;
        private AnglerMoveArgs _CurrentAnglerMoveArgs;
        private bool _AnglerIsRotating = false;
        private AnglerRotateArgs _CurrentAnglerRotateArgs;
        private bool _AnglerIsScaling = false;
        private AnglerScaleArgs _CurrentAnglerScaleArgs;

        private Vector3 _AnglerBasePosition;
        private Vector3 _AnglerBaseEulerAngles;
        private Vector3 _AnglerBaseScale;

        [SerializeField] StickyCanvas _StickyCanvas;

        void Awake()
        {
            _AnglerBasePosition = AnglerTransform.localPosition;
            _AnglerBaseEulerAngles = AnglerTransform.localEulerAngles;
            _AnglerBaseScale = AnglerTransform.localScale;
        }
        private void Update()
        {
            if (!conductor.isPlaying && !conductor.isPaused && ActiveLakes.Count <= 0)
            {
                List<RiqEntity> activeFishes = GetActiveFishes(conductor.songPositionInBeatsAsDouble);
                if (activeFishes.Count > 0)
                    NewLake(activeFishes[0]);
                else
                    SpawnNextFish(conductor.songPositionInBeatsAsDouble);
            }

            // Moving Ann
            if (_AnglerIsMoving)
            {
                float normalizedBeat = Conductor.instance.GetPositionFromBeat(_CurrentAnglerMoveArgs.StartBeat, _CurrentAnglerMoveArgs.Length);
                Util.EasingFunction.Function func = Util.EasingFunction.GetEasingFunction(_CurrentAnglerMoveArgs.Ease);
                float newPos = func(0f, 1f, normalizedBeat);
                Vector3 diff = _CurrentAnglerMoveArgs.EndPosition - _CurrentAnglerMoveArgs.StartPosition;
                AnglerTransform.localPosition = _AnglerBasePosition + _CurrentAnglerMoveArgs.StartPosition + (diff * newPos);
                
                if (normalizedBeat >= 1f)
                {
                    AnglerTransform.localPosition = _AnglerBasePosition + _CurrentAnglerMoveArgs.EndPosition;
                    _AnglerIsMoving = false;
                }
            }
            if (_AnglerIsRotating)
            {
                float normalizedBeat = Conductor.instance.GetPositionFromBeat(_CurrentAnglerRotateArgs.StartBeat, _CurrentAnglerRotateArgs.Length);
                Util.EasingFunction.Function func = Util.EasingFunction.GetEasingFunction(_CurrentAnglerRotateArgs.Ease);
                float newPos = func(0f, 1f, normalizedBeat);
                float diff = _CurrentAnglerRotateArgs.EndRotation - _CurrentAnglerRotateArgs.StartRotation;
                AnglerTransform.localEulerAngles = _AnglerBaseEulerAngles + new Vector3(0, 0, _CurrentAnglerRotateArgs.StartRotation + (diff * newPos));

                if (normalizedBeat >= 1f)
                {
                    AnglerTransform.localEulerAngles = _AnglerBaseEulerAngles +  new Vector3(0, 0, _CurrentAnglerRotateArgs.EndRotation);
                    _AnglerIsRotating = false;
                }
            }
            if (_AnglerIsScaling)
            {
                float normalizedBeat = Conductor.instance.GetPositionFromBeat(_CurrentAnglerScaleArgs.StartBeat, _CurrentAnglerScaleArgs.Length);
                Util.EasingFunction.Function func = Util.EasingFunction.GetEasingFunction(_CurrentAnglerScaleArgs.Ease);
                float newPos = func(0f, 1f, normalizedBeat);
                Vector3 diff = _CurrentAnglerScaleArgs.EndScale - _CurrentAnglerScaleArgs.StartScale;
                AnglerTransform.localScale = new Vector3
                (
                    _AnglerBaseScale.x * (_CurrentAnglerScaleArgs.StartScale.x + (diff.x * newPos)),
                    _AnglerBaseScale.y * (_CurrentAnglerScaleArgs.StartScale.y + (diff.y * newPos)),
                    0
                );

                if (normalizedBeat >= 1f)
                {
                    AnglerTransform.localScale = new Vector3
                    (
                        _AnglerBaseScale.x * _CurrentAnglerScaleArgs.EndScale.x,
                        _AnglerBaseScale.y * _CurrentAnglerScaleArgs.EndScale.y,
                        0
                    );
                    _AnglerIsScaling = false;
                }
            }
        }
        public override void OnPlay(double beat)
        {
            OnGameSwitch(beat);
        }
        public override void OnGameSwitch(double beat)
        {
            DestroyOrphanedLakes();
            CleanupFishSounds();

            // set ann movement
            foreach (RiqEntity e in EventCaller.GetAllInGameManagerList("catchOfTheDay", new string[] { "moveAngler" }).Where(e => e.beat <= beat).OrderBy(e => e.beat))
            {
                SetAnglerMovement(e);
            }

            // get active fishes
            foreach (RiqEntity e in GetActiveFishes(beat))
            {
                NewLake(e);
            }
            if (ActiveLakes.Count <= 0)
            {
                SpawnNextFish(beat);
            }
        }

        public static void Cue_Fish01(RiqEntity e)
        {
            CleanupFishSounds();

            double beat = e.beat;

            FishSounds.Add(e, MultiSound.Play(new MultiSound.Sound[]{
                new MultiSound.Sound("catchOfTheDay/quick1", beat),
                new MultiSound.Sound("catchOfTheDay/quick2", beat + 1),
            }, forcePlay: true));

            if (Instance != null && Instance.ActiveLakes.ContainsKey(e))
                Instance.ActiveLakes[e]._MultiSound = FishSounds[e];
        }
        public static void Cue_Fish02(RiqEntity e)
        {
            CleanupFishSounds();

            double beat = e.beat;
            bool countIn = e["countIn"];

            FishSounds.Add(e, MultiSound.Play(new MultiSound.Sound[]{
                new MultiSound.Sound("catchOfTheDay/pausegill1", beat),
                new MultiSound.Sound("catchOfTheDay/pausegill2", beat + 0.5),
                new MultiSound.Sound("catchOfTheDay/pausegill3", beat + 1),
            }, forcePlay: true));

            if (countIn)
            {
                MultiSound.Play(new MultiSound.Sound[]{
                    new MultiSound.Sound("count-ins/and", beat + 2),
                    new MultiSound.Sound(UnityEngine.Random.Range(0.0f, 1.0f) > 0.5 ? "count-ins/go1" : "count-ins/go2", beat + 3),
                }, forcePlay: true, game: false);
            }

            if (Instance != null && Instance.ActiveLakes.ContainsKey(e))
                Instance.ActiveLakes[e]._MultiSound = FishSounds[e];
        }
        public static void Cue_Fish03(RiqEntity e)
        {
            CleanupFishSounds();

            double beat = e.beat;
            bool countIn = e["countIn"];

            FishSounds.Add(e, MultiSound.Play(new MultiSound.Sound[]{
                new MultiSound.Sound("catchOfTheDay/threefish1", beat),
                new MultiSound.Sound("catchOfTheDay/threefish2", beat + 0.25),
                new MultiSound.Sound("catchOfTheDay/threefish3", beat + 0.5),
                new MultiSound.Sound("catchOfTheDay/threefish4", beat + 1)
            }, forcePlay: true));
            if (countIn)
            {
                MultiSound.Play(new MultiSound.Sound[]{
                    new MultiSound.Sound("count-ins/one1", beat + 2),
                    new MultiSound.Sound("count-ins/two1", beat + 3),
                    new MultiSound.Sound("count-ins/three1", beat + 4),
                    new MultiSound.Sound(UnityEngine.Random.Range(0.0f, 1.0f) > 0.5 ? "count-ins/go1" : "count-ins/go2", beat + 4.5),
                }, forcePlay: true, game: false);
            }
            
            if (Instance != null && Instance.ActiveLakes.ContainsKey(e))
                Instance.ActiveLakes[e]._MultiSound = FishSounds[e];
        }

        public void SetAnglerMovement(RiqEntity e)
        {
            if (e["doMove"])
            {
                _AnglerIsMoving = true;
                _CurrentAnglerMoveArgs = new AnglerMoveArgs(
                    e.beat, e.length,
                    new Vector3(e["startMoveX"], e["startMoveY"], 0),
                    new Vector3(e["endMoveX"], e["endMoveY"], 0),
                    e["ease"]
                );
            }
            if (e["doRotate"])
            {
                _AnglerIsRotating = true;
                _CurrentAnglerRotateArgs = new AnglerRotateArgs(
                    e.beat, e.length,
                    e["startRotDegrees"],
                    e["endRotDegrees"],
                    e["ease"]
                );
            }
            if (e["doScale"])
            {
                _AnglerIsScaling = true;
                _CurrentAnglerScaleArgs = new AnglerScaleArgs(
                    e.beat, e.length,
                    new Vector3(e["startScaleX"], e["startScaleY"], 1),
                    new Vector3(e["endScaleX"], e["endScaleY"], 1),
                    e["ease"]
                );
            }
            _StickyCanvas.Sticky = (bool)e["sticky"];
        }

        public void DoPickAnim()
        {
            Angler.DoScaledAnimationAsync("Pick", 0.5f);
        }
        public void DoJustAnim()
        {
            Angler.DoScaledAnimationAsync("Just", 0.5f);
        }
        public void DoMissAnim()
        {
            Angler.DoScaledAnimationAsync("Miss", 0.5f);
        }
        public void DoThroughAnim()
        {
            Angler.DoScaledAnimationAsync("Through", 0.5f);
        }
        public void DoOutAnim()
        {
            Angler.DoScaledAnimationAsync("Through", 0.5f);
        }

        public void DestroyOrphanedLakes()
        {
            List<GameObject> toDestroy = new();
            for (int i = 0; i < LakeSceneHolder.childCount; i++)
            {
                LakeScene lake = LakeSceneHolder.GetChild(i).gameObject.GetComponent<LakeScene>();
                if (lake == null || (!ActiveLakes.ContainsValue(lake) && !lake.IsDummy))
                    toDestroy.Add(LakeSceneHolder.GetChild(i).gameObject);
            }
            foreach (GameObject obj in toDestroy)
            {
                Destroy(obj);
            }
        }
        public static void CleanupFishSounds()
        {
            List<RiqEntity> expiredKeys = new();
            foreach (KeyValuePair<RiqEntity, MultiSound> kv in FishSounds)
            {
                if (kv.Value == null)
                    expiredKeys.Add(kv.Key);
            }
            foreach (RiqEntity key in expiredKeys)
                FishSounds.Remove(key);
        }
        public List<RiqEntity> GetActiveFishes(double beat)
        {
            return CacheFishes().FindAll(e => e.beat <= beat && e.beat + e.length - 1 + e["sceneDelay"] >= beat);
        }
        public RiqEntity GetNextFish(double beat)
        {
            RiqEntity gameSwitch = GetNextGameSwitch(beat);
            return CacheFishes().FirstOrDefault(e => e.beat >= beat && (gameSwitch is null || e.beat < gameSwitch.beat));
        }
        public RiqEntity GetNextGameSwitch(double beat)
        {
            return EventCaller.GetAllInGameManagerList("gameManager", new string[] { "switchGame" }).OrderBy(e => e.beat).FirstOrDefault(e => e.beat > beat && e.datamodel != "gameManager/switchGame/catchOfTheDay");
        }

        public LakeScene NewLake(RiqEntity e)
        {
            if (ActiveLakes.ContainsKey(e))
                return null;
            
            if (ActiveLakes.Count >= MAX_LAKES)
                return null;
            
            int sort = CacheFishes().FindIndex(x => e == x);
            if (sort < 0)
                return null;
            
            CleanupFishSounds();

            Debug.Log($"Spawning Lake {sort}");

            LakeScene lake = Instantiate(LakeScenePrefab, LakeSceneHolder).GetComponent<LakeScene>();
            LastLayout = lake.Setup(e, this, LastLayout, 0 - sort);
            ActiveLakes.Add(e, lake);
            if (FishSounds.ContainsKey(e))
                lake._MultiSound = FishSounds[e];
            return lake;
        }
        public bool SpawnNextFish(double beat)
        {
            RiqEntity nextFish = GetNextFish(beat);
            if (nextFish is not null)
            {
                NewLake(nextFish);
                return true;
            }
            return false;
        }
        public void DisposeLake(LakeScene lake, double beat)
        {
            ActiveLakes.Remove(lake.Entity);

            if (ActiveLakes.Count <= 0)
            {
                if (SpawnNextFish(conductor.songPositionInBeatsAsDouble))
                    lake.Crossfade(beat);
            }
            else
                lake.Crossfade(beat);
        }

        public List<RiqEntity> CacheFishes()
        {
            return _AllFishes ??= EventCaller.GetAllInGameManagerList("catchOfTheDay", new string[] { "fish1", "fish2", "fish3" }).OrderBy(e => e.beat).ToList();
        }

        public enum FishLayout : int
        {
            Random = -1,
            LayoutA = 0,
            LayoutB = 1,
            LayoutC = 2
        }
    
        private struct AnglerMoveArgs
        {
            public Vector3 StartPosition;
            public Vector3 EndPosition;
            public double StartBeat;
            public double Length;
            public Util.EasingFunction.Ease Ease;

            public AnglerMoveArgs(double startBeat, double length, Vector3 startPosition, Vector3 endPosition, int ease)
            {
                StartPosition = startPosition;
                EndPosition = endPosition;
                StartBeat = startBeat;
                Length = length;
                Ease = (Util.EasingFunction.Ease)ease;
            }
        }
        private struct AnglerRotateArgs
        {
            public float StartRotation;
            public float EndRotation;
            public double StartBeat;
            public double Length;
            public Util.EasingFunction.Ease Ease;

            public AnglerRotateArgs(double startBeat, double length, float startRotation, float endRotation, int ease)
            {
                StartRotation = startRotation;
                EndRotation = endRotation;
                StartBeat = startBeat;
                Length = length;
                Ease = (Util.EasingFunction.Ease)ease;
            }
        }
        private struct AnglerScaleArgs
        {
            public Vector3 StartScale;
            public Vector3 EndScale;
            public double StartBeat;
            public double Length;
            public Util.EasingFunction.Ease Ease;

            public AnglerScaleArgs(double startBeat, double length, Vector3 startScale, Vector3 endScale, int ease)
            {
                StartScale = startScale;
                EndScale = endScale;
                StartBeat = startBeat;
                Length = length;
                Ease = (Util.EasingFunction.Ease)ease;
            }
        }
    }
}

// This minigame ported by Yin. ☆