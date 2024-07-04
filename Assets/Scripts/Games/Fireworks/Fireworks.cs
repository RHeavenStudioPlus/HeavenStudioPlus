using HeavenStudio.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyBezierCurves;
using DG.Tweening;
using Jukebox;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class AgbFireworkLoader
    {
        public static Minigame AddGame(EventCaller eventCaller) {
			
			RiqEntity fwBGUpdater(string datamodel, RiqEntity e)
            {
                if (datamodel == "fireworks/altBG")
                {
					e.CreateProperty("stars", (!e["toggle"]));
					e.CreateProperty("faces", (e["toggle"]));
					e.CreateProperty("startTop", new Color(0f, 8/255f, 32/255f));
					e.CreateProperty("endTop", new Color(0f, 8/255f, 32/255f));
					e.CreateProperty("startBot", new Color(0f, 51/255f, 119/255f));
					e.CreateProperty("endBot", new Color(0f, 51/255f, 119/255f));
					e.CreateProperty("startCity", new Color(0f, 8/255f, 32/255f));
					e.CreateProperty("endCity", new Color(0f, 8/255f, 32/255f));
					e.CreateProperty("ease", 0);
					 
					e.dynamicData.Remove("toggle");
					
					e.datamodel = "fireworks/changeBG";
                    return e;
                }
                return null;
            }
            RiqBeatmap.OnUpdateEntity += fwBGUpdater;
			
            return new Minigame("fireworks", "Fireworks \n<color=#adadad>(Hanabi)</color>", "000820", true, false, new List<GameAction>()
            {
                new GameAction("firework", "Firework")
                {
                    preFunction = delegate {var e = eventCaller.currentEntity; Fireworks.PreSpawnFirework(e.beat, false, e["whereToSpawn"], e["toggle"], e["explosionType"], e["applause"], e["offSet"]); },
                    defaultLength = 4f,
                    parameters = new List<Param>()
                    {
                        new Param("whereToSpawn", Fireworks.WhereToSpawn.Middle, "Side", "Choose the side this firework should spawn on."),
                        new Param("explosionType", Fireworks.ExplosionType.MixedCircular, "Pattern", "Choose the pattern that this firework should explode into."),
                        new Param("applause", false, "Applause", "Toggle if applause should play if you successfully hit this cue."),
                        new Param("offSet", new EntityTypes.Float(0, 4, 0), "Vertical Offset", "Choose the verical offset for this firework."),
                        new Param("toggle", false, "Count-In", "Toggle if a count-in should automatically play for this cue, as shown in the practice of the original game.")
                    }
                },
                new GameAction("sparkler", "Sparkler")
                {
                    preFunction = delegate {var e = eventCaller.currentEntity; Fireworks.PreSpawnFirework(e.beat, true, e["whereToSpawn"], e["toggle"], e["explosionType"], e["applause"], e["offSet"]); },
                    defaultLength = 2f,
                    parameters = new List<Param>()
                    {
                        new Param("whereToSpawn", Fireworks.WhereToSpawn.Middle, "Side", "Choose the side this sparkler should spawn on."),
                        new Param("explosionType", Fireworks.ExplosionType.MixedCircular, "Pattern", "Choose the pattern that this sparkler should explode into."),
                        new Param("applause", false, "Applause", "Toggle if applause should play if you successfully hit this cue."),
                        new Param("offSet", new EntityTypes.Float(0, 4, 0), "Vertical Offset", "Choose the vertical offset for this sparkler."),
                        new Param("toggle", false, "Count-In", "Toggle if a count-in should automatically play for this cue, as shown in the practice of the original game.")
                    }
                },
                new GameAction("bomb", "Bomb")
                {
                    function = delegate {var e = eventCaller.currentEntity; Fireworks.instance.SpawnBomb(e.beat, e["toggle"], e["applause"]); },
                    defaultLength = 3f,
                    parameters = new List<Param>()
                    {
                        new Param("applause", false, "Applause", "Toggle if applause should play if you successfully hit this cue."),
                        new Param("toggle", false, "Count-In", "Toggle if a count-in should automatically play for this cue, as shown in the practice of the original game.")
                    }
                },
                new GameAction("countIn", "Count-In")
                {
                    preFunction = delegate {var e = eventCaller.currentEntity; Fireworks.CountIn(e.beat, e["count"]); },
                    defaultLength = 1f,
                    parameters = new List<Param>()
                    {
                        new Param("count", Fireworks.CountInType.CountOne, "Type", "Set the number to be said.")
                    }
                },
				new GameAction("changeBG", "Background Appearance")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        Fireworks.instance.BackgroundColor(e.beat, e.length, e["stars"], e["faces"], e["startTop"], e["endTop"], e["startBot"], e["endBot"], e["startCity"], e["endCity"], e["ease"]);
                    },
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("stars", true, "Stars", "Toggle if the stars should appear."),
						new Param("faces", false, "Remix 5", "Toggle if the faces from Remix 5 (GBA) should appear."),
						new Param("startTop", new Color(0f, 8/255f, 32/255f), "Gradient Top Start", "Set the color at the start of the event."),
                        new Param("endTop", new Color(0f, 8/255f, 32/255f), "Gradient Top End", "Set the color at the end of the event."),
						new Param("startBot", new Color(0f, 51/255f, 119/255f), "Gradient Bottom Start", "Set the color at the start of the event."),
                        new Param("endBot", new Color(0f, 51/255f, 119/255f), "Gradient Bottom End", "Set the color at the end of the event."),
						new Param("startCity", new Color(0f, 8/255f, 32/255f), "City Start", "Set the color at the start of the event."),
                        new Param("endCity", new Color(0f, 8/255f, 32/255f), "City End", "Set the color at the end of the event."),
						new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.")
                    }
                },

                new GameAction("altBG", "Background Appearance (OLD)")
                {
                    function = delegate {var e = eventCaller.currentEntity; Fireworks.instance.ChangeBackgroundAppearance(e["toggle"]); },
                    defaultLength = 0.5f,
					hidden = true,
                    parameters = new List<Param>()
                    {
                        new Param("toggle", true, "Remix 5", "Toggle fi ze bckgrond frum realix 5 (GFA) shool aper. (You should never see this.)")
                    }
                }
            },
            new List<string>() {"agb", "normal"},
            "agbexplode", "en",
            new List<string>() {},
            chronologicalSortKey: 28
            );
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_Fireworks;
    public class Fireworks : Minigame
    {
        public enum ExplosionType
        {
            UniformBig = 0,
            UniformDonut = 1,
            UniformSwirl = 2,
            UniformSmile = 3,
            MixedCircular = 4
        }
        public struct QueuedFirework
        {
            public double beat;
            public bool isSparkler;
            public int whereToSpawn;
            public bool practice;
            public int explosionType;
            public bool applause;
            public float verticalOffset;
        }
        public enum WhereToSpawn
        {
            Left = 0,
            Right = 1,
            Middle = 2
        }
        public enum CountInType
        {
            CountOne = 0,
            CountTwo = 1,
            CountThree = 2,
            CountHey = 3
        }
        [Header("Components")]
        [SerializeField] Transform spawnLeft;
        [SerializeField] Transform spawnRight;
        [SerializeField] Transform spawnMiddle;
        [SerializeField] Transform bombSpawn;
        [SerializeField] Rocket firework;
        [SerializeField] FireworksBomb bomb;
        [SerializeField] BezierCurve3D bombCurve;
        [SerializeField] SpriteRenderer flashWhite;
		[SerializeField] SpriteRenderer gradientTop;
        [SerializeField] SpriteRenderer gradientBottom;
		[SerializeField] SpriteRenderer city;
        [SerializeField] GameObject faces;
        [SerializeField] GameObject stars;
        [Header("Properties")]
        Tween flashTween;
        public static List<QueuedFirework> queuedFireworks = new List<QueuedFirework>();
		
		private ColorEase topColorEase = new(new Color(0f, 8/255f, 32/255f));
		private ColorEase botColorEase = new(new Color(0f, 51/255f, 119/255f));
		private ColorEase cityColorEase = new(new Color(0f, 8/255f, 32/255f));

        public static Fireworks instance;

        void OnDestroy()
        {
            if (queuedFireworks.Count > 0) queuedFireworks.Clear();
            foreach (var evt in scheduledInputs)
            {
                evt.Disable();
            }
        }

        void Awake()
        {
            instance = this;
        }
		
		public override void OnPlay(double beat)
        {
			PersistColor(beat);
        }

        void Update()
        {
            var cond = Conductor.instance;

            if (cond.isPlaying && !cond.isPaused)
            {
                if (queuedFireworks.Count > 0)
                {
                    foreach (var firework in queuedFireworks)
                    {
                        SpawnFirework(firework.beat, firework.isSparkler, firework.whereToSpawn, firework.practice, firework.explosionType, firework.applause, firework.verticalOffset);
                    }
                    queuedFireworks.Clear();
                }
            }
			
			BackgroundColorUpdate();
        }

        public void ChangeBackgroundAppearance(bool doIt)
        {
            //faces.SetActive(doIt);
            //stars.SetActive(!doIt);
        }

        public static void CountIn(double beat, int count)
        {
            switch (count)
            {
                case (int)CountInType.CountOne:
                    MultiSound.Play(new MultiSound.Sound[]
                    {
                        new MultiSound.Sound("fireworks/count1", beat)
                    }, forcePlay: true);
                    break;
                case (int)CountInType.CountTwo:
                    MultiSound.Play(new MultiSound.Sound[]
                    {
                        new MultiSound.Sound("fireworks/count2", beat)
                    }, forcePlay: true);
                    break;
                case (int)CountInType.CountThree:
                    MultiSound.Play(new MultiSound.Sound[]
                    {
                        new MultiSound.Sound("fireworks/count3", beat)
                    }, forcePlay: true);
                    break;
                case (int)CountInType.CountHey:
                    MultiSound.Play(new MultiSound.Sound[]
                    {
                        new MultiSound.Sound("fireworks/countHey", beat)
                    }, forcePlay: true);
                    break;
            }
        }

        public static void PreSpawnFirework(double beat, bool isSparkler, int whereToSpawn, bool practice, int explosionType, bool applause, float verticalOffset)
        {
            if (isSparkler)
            {
                MultiSound.Play(new MultiSound.Sound[]
                {
                    new MultiSound.Sound("fireworks/nuei", beat, 1, 1, false, 0.223f)
                }, forcePlay: true);
            }
            else
            {
                MultiSound.Play(new MultiSound.Sound[]
                {
                    new MultiSound.Sound("fireworks/rocket_2", beat)
                }, forcePlay: true);
            }
            if (GameManager.instance.currentGame == "fireworks")
            {
                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat, delegate
                    {
                        Fireworks.instance.SpawnFirework(beat, isSparkler, whereToSpawn, practice, explosionType, applause, verticalOffset);
                    })
                });
            }
            else
            {
                queuedFireworks.Add(new QueuedFirework { beat = beat, isSparkler = isSparkler, whereToSpawn = whereToSpawn, practice = practice, explosionType = explosionType, applause = applause, verticalOffset = verticalOffset });
            }

        }

        void SpawnFirework(double beat, bool isSparkler, int whereToSpawn, bool practice, int explosionType, bool applause, float verticalOffset)
        {
            if (isSparkler && practice)
            {
                MultiSound.Play(new MultiSound.Sound[]
                {
                    new MultiSound.Sound("fireworks/practiceHai", beat + 1),
                }, forcePlay: true);
            }
            else if (practice)
            {
                MultiSound.Play(new MultiSound.Sound[]
                {
                    new MultiSound.Sound("fireworks/practice1", beat),
                    new MultiSound.Sound("fireworks/practice2", beat + 1),
                    new MultiSound.Sound("fireworks/practice3", beat + 2),
                    new MultiSound.Sound("fireworks/practiceHai", beat + 3),
                }, forcePlay: true);
            }

            Transform spawnPoint = spawnMiddle;
            switch (whereToSpawn)
            {
                case (int)WhereToSpawn.Left:
                    spawnPoint = spawnLeft;
                    break;
                case (int)WhereToSpawn.Right:
                    spawnPoint = spawnRight;
                    break;
                default:
                    spawnPoint = spawnMiddle;
                    break;
            }
            Rocket spawnedRocket = Instantiate(firework, spawnPoint, false);
            spawnedRocket.isSparkler = isSparkler;
            spawnedRocket.offSet = verticalOffset;
            spawnedRocket.applause = applause;
            spawnedRocket.Init(beat, explosionType);
        }

        public void SpawnBomb(double beat, bool practice, bool applause)
        {
            SoundByte.PlayOneShotGame("fireworks/tamaya_4");
            if (practice)
            {
                MultiSound.Play(new MultiSound.Sound[]
                {
                    new MultiSound.Sound("fireworks/practiceHai", beat + 2),
                }, forcePlay: true);
            }
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 1, delegate
                {
                    FireworksBomb spawnedBomb = Instantiate(bomb, bombSpawn, false);
                    spawnedBomb.curve = bombCurve;
                    spawnedBomb.applause = applause;
                    spawnedBomb.Init(beat + 1);
                })
            });
        }

        public void ChangeFlashColor(Color color, float beats)
        {
            var seconds = Conductor.instance.secPerBeat * beats;

            if (flashTween != null)
                flashTween.Kill(true);

            if (seconds == 0)
            {
                flashWhite.color = color;
            }
            else
            {
                flashTween = flashWhite.DOColor(color, seconds);
            }
        }

        public void FadeFlashColor(Color start, Color end, float beats)
        {
            ChangeFlashColor(start, 0f);
            ChangeFlashColor(end, beats);
        }
		
		private void BackgroundColorUpdate()
        {
			gradientTop.color = topColorEase.GetColor();
			gradientBottom.color = botColorEase.GetColor();
			city.color = cityColorEase.GetColor();
        }
		
		public void BackgroundColor(double beat, float length, bool dostars, bool dormx5, Color topStart, Color topEnd, Color botStart, Color botEnd, Color cityStart, Color cityEnd, int ease)
        {
            topColorEase = new ColorEase(beat, length, topStart, topEnd, ease);
			botColorEase = new ColorEase(beat, length, botStart, botEnd, ease);
			cityColorEase = new ColorEase(beat, length, cityStart, cityEnd, ease);
			
			faces.SetActive(dormx5);
			stars.SetActive(dostars);
        }
		
		private void PersistColor(double beat)
        {
            var allEventsBeforeBeat = EventCaller.GetAllInGameManagerList("fireworks", new string[] { "changeBG" }).FindAll(x => x.beat < beat);
            if (allEventsBeforeBeat.Count > 0)
            {
                allEventsBeforeBeat.Sort((x, y) => x.beat.CompareTo(y.beat)); //just in case
                var lastEvent = allEventsBeforeBeat[^1];
                BackgroundColor(lastEvent.beat, lastEvent.length, lastEvent["stars"], lastEvent["faces"], lastEvent["startTop"], lastEvent["endTop"], lastEvent["startBot"], lastEvent["endBot"], lastEvent["startCity"], lastEvent["endCity"], lastEvent["ease"]);
            }
        }
		
		public override void OnGameSwitch(double beat)
        {
            PersistColor(beat);
        }
    }
}
