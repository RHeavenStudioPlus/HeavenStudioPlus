using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
using Jukebox;
using HeavenStudio.Games.Scripts_ForkLifter;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class RvlForkLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
		{
			RiqEntity flGradientUpdater(string datamodel, RiqEntity e)
            {
                if (datamodel == "forkLifter/colorGrad" && e.version == 0)
                {
					e.CreateProperty("type", 2);
					e["type"] = 2;	// setting it in create property doesn't work for some reason? lol
					e.version = 1;
                    return e;
                }
                return null;
            }
            RiqBeatmap.OnUpdateEntity += flGradientUpdater;
			
            return new Minigame("forkLifter", "Fork Lifter", "f1f1f1", true, false, new List<GameAction>()
            {
                new GameAction("flick", "Flick Food")
                {
                    inactiveFunction = delegate {
                        var e = eventCaller.currentEntity;
                        ForkLifter.Flick(e.beat);
                    },
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        ForkLifter.Flick(e.beat);
                        ForkLifter.instance.FlickActive(e.beat, e["type"]);
                    },
                    defaultLength = 3,
                    parameters = new List<Param>()
                    {
                        new Param("type", ForkLifter.FlickType.Pea, "Object", "Choose the object to be flicked.")
                    },
                },
                new GameAction("prepare", "Prepare Hand")
                {
                    function = delegate { ForkLifter.instance.ForkLifterHand.Prepare(eventCaller.currentEntity["mute"]); }, 
                    defaultLength = 0.5f,
                    parameters = new List<Param>()
                    {
                        new Param("mute", false, "Mute", "Toggle if the prepare sound effect should play.")
                    }
                },
                new GameAction("gulp", "Swallow")
                {
                    function = delegate { ForkLifter.playerInstance.Eat(eventCaller.currentEntity["sfx"]); },
                    parameters = new List<Param>()
                    {
                        new Param("sfx", ForkLifterPlayer.EatType.Default, "SFX", "Choose the SFX to play.")
                    }
                },
                new GameAction("sigh", "Sigh")
                {
                    function = delegate { SoundByte.PlayOneShot("games/forkLifter/sigh"); }
                },
                new GameAction("color", "Background Appearance")
                {
                    function = delegate { var e = eventCaller.currentEntity; ForkLifter.instance.BackgroundColor(e.beat, e.length, e["start"], e["end"], e["ease"]); },
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("start", Color.white, "Start Color", "Set the color at the start of the event."),
                        new Param("end", Color.white, "End Color", "Set the color at the end of the event."),
                        new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.")
                    },
                },
                new GameAction("colorGrad", "Gradient Appearance")
                {
                    function = delegate { var e = eventCaller.currentEntity; ForkLifter.instance.BackgroundColorGrad(e.beat, e.length, (e.version = 1),
					e["type"], e["toggleVC"], e["toggleLines"],
					e["start"], e["end"], 
					e["startBG"], e["endBG"], 
					e["startLines"], e["endLines"],
					e["ease"]); },
                    resizable = true,
                    parameters = new List<Param>()
                    {
						new Param("type", ForkLifter.GradientType.Game, "Gradient Type", "Set the type of gradient.", new List<Param.CollapseParam>()
						{
                            new Param.CollapseParam((x, _) => (int)x != (int)ForkLifter.GradientType.Classic, new string[] { "startBG", "endBG", "toggleLines" })
                        }),
						
						new Param("toggleVC", false, "Viewcircle Uses Gradient Color", "If toggled, the viewcircle's background will use the gradient top color. Otherwise, it will use the background color."),
						new Param("toggleLines", false, "Megamix Lines", "If toggled, the lines from Megamix will be enabled.", new List<Param.CollapseParam>()
						{
                           new Param.CollapseParam((x, _) => (bool)x, new string[] { "startLines", "endLines" }),
                        }),
						
                        new Param("start", new Color(224/255f, 224/255f, 224/255f), "Gradient Top Start", "Set the color at the start of the event."),
                        new Param("end", new Color(224/255f, 224/255f, 224/255f), "Gradient Top End", "Set the color at the end of the event."),
						
						new Param("startBG", Color.white, "Gradient Bottom Start", "Set the color at the start of the event."),
                        new Param("endBG", Color.white, "Gradient Bottom End", "Set the color at the end of the event."),	
						
						new Param("startLines", new Color(243/255f, 243/255f, 243/255f), "Lines Start", "Set the color at the start of the event."),
                        new Param("endLines", new Color(243/255f, 243/255f, 243/255f), "Lines End", "Set the color at the end of the event."),	
						
                        new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.")
                    },
                },
            },
            new List<string>() {"rvl", "normal"},
            "rvlfork", "en",
            new List<string>() {},
            chronologicalSortKey: 6
            );
        }
    }
}

namespace HeavenStudio.Games
{
   // using Jukebox;
    using Scripts_ForkLifter;

    public class ForkLifter : Minigame
    {
		
        public enum FlickType
        {
            Pea,
            TopBun,
            Burger,
            BottomBun
        }
		
		public enum GradientType
		{
			Game,
			Remix,
			Classic
		}

        public static ForkLifter instance;
        public static ForkLifterPlayer playerInstance => ForkLifterPlayer.instance;

        [Header("References")]
        public ForkLifterHand ForkLifterHand;

        [Header("Objects")]
        public Animator handAnim;
        public GameObject flickedObject;
        public SpriteRenderer peaPreview;
        [SerializeField] SpriteRenderer bg;
		
		//public Material gradientMaterial;
        public SpriteRenderer[] Gradients;
		[SerializeField] SpriteRenderer gradientFiller;
		[SerializeField] SpriteRenderer mmLines;
		
        [SerializeField] SpriteRenderer viewerCircle;
		[SerializeField] SpriteRenderer viewerCircleBg;
        [SerializeField] SpriteRenderer playerShadow;
        [SerializeField] SpriteRenderer handShadow;
		public SpriteRenderer[] forkEffects;

        public Sprite[] peaSprites;
        public Sprite[] peaHitSprites;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            BackgroundColorUpdate();
        }

        public override void OnPlay(double beat)
        {
            OnGameSwitch(beat);
        }

        public override void OnGameSwitch(double beat)
        {
            var actions = GameManager.instance.Beatmap.Entities.FindAll(e => e.datamodel.Split('/')[0] == "forkLifter");

            var actionsBefore = actions.FindAll(e => e.beat < beat);

            var lastColor = actionsBefore.FindLast(e => e.datamodel == "forkLifter/color");
            if (lastColor != null) {
                BackgroundColor(lastColor.beat, lastColor.length, lastColor["start"], lastColor["end"], lastColor["ease"]);
            }

            var lastColorGrad = actionsBefore.FindLast(e => e.datamodel == "forkLifter/colorGrad");
            if (lastColorGrad != null) {
                BackgroundColorGrad(lastColorGrad.beat, lastColorGrad.length, lastColorGrad.version, lastColorGrad["type"], lastColorGrad["toggleVC"], lastColorGrad["toggleLines"], lastColorGrad["start"], lastColorGrad["end"], lastColorGrad["startBG"], lastColorGrad["endBG"], lastColorGrad["startLines"], lastColorGrad["endLines"], lastColorGrad["ease"]);
            }

            var tempFlicks = actions.FindAll(e => e.datamodel == "forkLifter/flick");

            foreach (var e in tempFlicks.FindAll(e => e.beat < beat && e.beat + 2 > beat)) {
                FlickActive(e.beat, e["type"]);
            }

            ForkLifterHand.allFlickEntities = tempFlicks.FindAll(e => e.beat >= beat);
            ForkLifterHand.CheckNextFlick();
        }

        public static void Flick(double beat)
        {
            var offset = SoundByte.GetClipLengthGame("forkLifter/zoomFast") - 0.03;
            SoundByte.PlayOneShotGame("forkLifter/zoomFast", beat + 2, offset: offset, forcePlay: true);

            SoundByte.PlayOneShotGame("forkLifter/flick", forcePlay: true);
        }

        public void FlickActive(double beat, int type)
        {

            handAnim.DoScaledAnimationFromBeatAsync("Hand_Flick", 0.5f, beat);
            ForkLifterHand.currentFlickIndex++;
            GameObject fo = Instantiate(flickedObject);
            fo.transform.parent = flickedObject.transform.parent;
            Pea pea = fo.GetComponent<Pea>();
            pea.startBeat = beat;
            pea.type = type;
            fo.SetActive(true);
        }

        private ColorEase bgColorEase = new(Color.white);
        private ColorEase gradColorEase = new(new Color(224/255f, 224/255f, 224/255f));
		private ColorEase linesColorEase = new(new Color (243/255f, 243/255f, 243/255f));
		private ColorEase gradBgEase = new(Color.white);
		private bool vCircleToggle = false;

        //call this in update
        private void BackgroundColorUpdate()
        {
			bg.color =
			viewerCircle.color = bgColorEase.GetColor();
			
			mmLines.color = linesColorEase.GetColor();
			
			for (int i = 0; i < Gradients.Length; i++)
            {
                Gradients[i].color = gradColorEase.GetColor();
            }
			
			
			if (Gradients[2].gameObject.activeSelf)
			{
				gradientFiller.color = 
				playerShadow.color = gradColorEase.GetColor();
				
				for (int i = 0; i < forkEffects.Length; i++)
				{
					forkEffects[i].color = gradColorEase.GetColor();
				}
			}
			else
			{
				gradientFiller.color = 
				playerShadow.color = gradBgEase.GetColor();
				
				for (int i = 0; i < forkEffects.Length; i++)
				{
					forkEffects[i].color = gradBgEase.GetColor();
				}
			}
			
			
			if (vCircleToggle)
			{
				viewerCircleBg.color = 
				handShadow.color = gradColorEase.GetColor();
			}
			else
			{
				viewerCircleBg.color = 
				handShadow.color = bgColorEase.GetColor();
			}
			
        }

        public void BackgroundColor(double beat, float length, Color startColor, Color endColor, int ease)
        {
            bgColorEase = new ColorEase(beat, length, startColor, endColor, ease);
        }

        public void BackgroundColorGrad(double beat, float length, int version, int gradType, bool vCircle, bool lines, Color startColor, Color endColor, Color startBottom, Color endBottom, Color startLines, Color endLines, int ease)
        {
            gradColorEase = new ColorEase(beat, length, startColor, endColor, ease);
			gradBgEase = new ColorEase(beat, length, startBottom, endBottom, ease);
			linesColorEase = new ColorEase(beat, length, startLines, endLines, ease);
			
			for (int i = 0; i < Gradients.Length; i++)
            {
                Gradients[i].gameObject.SetActive(gradType == (i));
            }
			
			if (gradType != 2)
			{
				mmLines.gameObject.SetActive(lines);
			}
			else
			{
				mmLines.gameObject.SetActive(false);
			}

			
			vCircleToggle = (vCircle);
        }

        //call this in OnPlay(double beat) and OnGameSwitch(double beat)
        private void PersistColor(double beat)
        {
            var allEventsBeforeBeat = EventCaller.GetAllInGameManagerList("forkLifter", new string[] { "color" }).FindAll(x => x.beat < beat);
            if (allEventsBeforeBeat.Count > 0)
            {
                allEventsBeforeBeat.Sort((x, y) => x.beat.CompareTo(y.beat)); //just in case
                var lastEvent = allEventsBeforeBeat[^1];
                BackgroundColor(lastEvent.beat, lastEvent.length, lastEvent["start"], lastEvent["end"], lastEvent["ease"]);
            }

            var allEventsBeforeBeatGrad = EventCaller.GetAllInGameManagerList("forkLifter", new string[] { "colorGrad" }).FindAll(x => x.beat < beat);
            if (allEventsBeforeBeatGrad.Count > 0)
            {
                allEventsBeforeBeatGrad.Sort((x, y) => x.beat.CompareTo(y.beat)); //just in case
                var lastEventGrad = allEventsBeforeBeatGrad[^1];
                BackgroundColorGrad(lastEventGrad.beat, lastEventGrad.length, lastEventGrad.version, lastEventGrad["type"], lastEventGrad["toggleVC"], lastEventGrad["toggleLines"], lastEventGrad["start"], lastEventGrad["end"], lastEventGrad["startBG"], lastEventGrad["endBG"], lastEventGrad["startLines"], lastEventGrad["endLines"], lastEventGrad["ease"]);
            }
        }
    }
}