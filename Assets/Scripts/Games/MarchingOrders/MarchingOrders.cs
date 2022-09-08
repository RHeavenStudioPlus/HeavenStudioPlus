//notes:
//  BEFORE NEW PROPS
// - minenice will also use this to test out randomly named parameters so coding has to rest until the new props update [DONE]
// - see fan club for separate prefabs (cadets) [DONE]
// - temporarily take sounds from rhre, wait until someone records the full code, including misses, or record it myself (unlikely) [IN PROGRESS]
//  AFTER NEW PROPS
// - testmod marching orders using speed
// - see space soccer, mr upbeat, tunnel for keep-the-beat codes
// - figure how to do custom bg changes when the upscaled textures are finished (see karate man, launch party once it releases)
// - will use a textbox without going through the visual options but i wonder how..?? (see first contact if ever textboxes are implemented in said minigame)
//  AFTER FEATURE COMPLETION
// - delete all notes once the minigame is considered feature-complete

using HeavenStudio.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class AgbMarcherLoader
    {
        public static Minigame AddGame(EventCaller eventCaller) {
            return new Minigame("marchingOrders", "Marching Orders \n<color=#eb5454>[WIP]</color>", "00A43B", false, false, new List<GameAction>()
                {
                    new GameAction("bop", "Bop")
					{
					    function = delegate { var e = eventCaller.currentEntity; MarchingOrders.instance.Bop(e.beat, e.length); },
                        defaultLength = 1f,
					    resizable = true
				    },
					
					new GameAction("marching", "Cadets March")
					{
					    function = delegate { var e = eventCaller.currentEntity; MarchingOrders.instance.CadetsMarch(e.beat, e.length); },
                        defaultLength = 4f,
					    resizable = true
				    },
                    
                    new GameAction("attention", "Attention...")
					{
					    function = delegate { var e = eventCaller.currentEntity; MarchingOrders.instance.SargeAttention(e.beat); },
					    defaultLength = 2.25f,
						inactiveFunction = delegate { var e = eventCaller.currentEntity; MarchingOrders.AttentionSound(e.beat);}
                    },
					
					new GameAction("march", "March!")
					{
						function = delegate { var e = eventCaller.currentEntity; MarchingOrders.instance.SargeMarch(e.beat); },
						defaultLength = 2f,
                        inactiveFunction = delegate { var e = eventCaller.currentEntity; MarchingOrders.MarchSound(e.beat);}
					},
					
					new GameAction("halt", "Halt!")
					{
						function = delegate { var e = eventCaller.currentEntity; MarchingOrders.instance.SargeHalt(e.beat); },
						defaultLength = 2f,
                        inactiveFunction = delegate { var e = eventCaller.currentEntity; MarchingOrders.HaltSound(e.beat);}
                    },
					
					//new GameAction("face turn", delegate {}, 4f, false, parameters: new List<Param>()
                    //{
                    //    new Param("type", MarchingOrders.DirectionFaceTurn.Right, "Direction", "The direction sarge wants the cadets to face"),
                    //    new Param("type2", MarchingOrders.FaceTurnLength.Normal, "Length", "How fast or slow the event lasts"),
                    //}),
                });
        }
    }
}

namespace HeavenStudio.Games
{
    //using Scripts_MarchingOrders;
    public class MarchingOrders : Minigame
    {
		//code is just copied from other minigame code, i will polish them later
		[Header("References")]
        public Animator Sarge;
        public Animator Cadet1;
        public Animator Cadet2;
        public Animator Cadet3;
		public Animator CadetPlayer;
        public GameObject Player;
		
		
		public GameEvent bop = new GameEvent();
		public GameEvent noBop = new GameEvent();
		public GameEvent marching = new GameEvent();
		
		private int marchCount;
		
        public static MarchingOrders instance;
        
        public enum DirectionFaceTurn
		{
            Right,
            Left,
        }
        public enum FaceTurnLength
		{
            Normal,
            Fast,
        }
        
        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {
			var cond = Conductor.instance;
            if (cond.ReportBeat(ref bop.lastReportedBeat, bop.startBeat % 1))
            {
                if (cond.songPositionInBeats >= bop.startBeat && cond.songPositionInBeats < bop.startBeat + bop.length)
                {
                    if (!(cond.songPositionInBeats >= noBop.startBeat && cond.songPositionInBeats < noBop.startBeat + noBop.length))
                        Cadet1.DoScaledAnimationAsync("Bop", 0.5f);
					    Cadet2.DoScaledAnimationAsync("Bop", 0.5f);
					    Cadet3.DoScaledAnimationAsync("Bop", 0.5f);
						CadetPlayer.DoScaledAnimationAsync("Bop", 0.5f);
                }
            }
			
			if (cond.ReportBeat(ref marching.lastReportedBeat, bop.startBeat % 1))
            {
                if (cond.songPositionInBeats >= marching.startBeat && cond.songPositionInBeats < marching.startBeat + marching.length)
                {
					marchCount += 1;
                    var marchAnim = (marchCount % 2 != 0 ? "MarchR" : "MarchL");

                    Jukebox.PlayOneShotGame("marchingOrders/step1");
					Cadet1.DoScaledAnimationAsync(marchAnim, 0.5f);
					Cadet2.DoScaledAnimationAsync(marchAnim, 0.5f);
					Cadet3.DoScaledAnimationAsync(marchAnim, 0.5f);
                }
            }
			
			if (PlayerInput.Pressed() && !IsExpectingInputNow())
            {
            Jukebox.PlayOneShot("miss");
            Sarge.DoScaledAnimationAsync("Anger", 0.5f);
            }
        }
        
		public void Bop(float beat, float length)
        {
            bop.length = length;
            bop.startBeat = beat;
        }
		
        public void CadetsMarch(float beat, float length)
        {
            marching.length = length;
            marching.startBeat = beat;	
        }
        
        public void SargeAttention(float beat)
        {
            MultiSound.Play(new MultiSound.Sound[] {
            new MultiSound.Sound("marchingOrders/attention1", beat),
            new MultiSound.Sound("marchingOrders/attention2", beat + 0.25f),
            new MultiSound.Sound("marchingOrders/attention3", beat + 0.75f),
            }, forcePlay:true);
			
			BeatAction.New(Player, new List<BeatAction.Action>() 
                {
                new BeatAction.Action(beat + 0.25f,     delegate { Sarge.DoScaledAnimationAsync("Talk", 0.5f);}),
                });
        }
		
		public void SargeMarch(float beat)
        {
			MultiSound.Play(new MultiSound.Sound[] {
            new MultiSound.Sound("marchingOrders/march1", beat),
            new MultiSound.Sound("marchingOrders/march2", beat + 1f),
            }, forcePlay:true);
			
			BeatAction.New(Player, new List<BeatAction.Action>() 
                {
                new BeatAction.Action(beat,     delegate { Sarge.DoScaledAnimationAsync("Talk", 0.5f);}),
				new BeatAction.Action(beat + 1f,     delegate { Cadet1.DoScaledAnimationAsync("MarchL", 0.5f);}),
                new BeatAction.Action(beat + 1f,     delegate { Cadet2.DoScaledAnimationAsync("MarchL", 0.5f);}),
				new BeatAction.Action(beat + 1f,     delegate { Cadet3.DoScaledAnimationAsync("MarchL", 0.5f);}),
				new BeatAction.Action(beat + 1f,     delegate { CadetPlayer.DoScaledAnimationAsync("MarchL", 0.5f);}),
				});
        }
        
		public void SargeHalt(float beat)
        {
			MultiSound.Play(new MultiSound.Sound[] {
            new MultiSound.Sound("marchingOrders/halt1", beat),
            new MultiSound.Sound("marchingOrders/halt2", beat + 1f),
			new MultiSound.Sound("marchingOrders/step1", beat + 1f),
            }, forcePlay:true);
			
			BeatAction.New(Player, new List<BeatAction.Action>() 
                {
                new BeatAction.Action(beat,     delegate { Sarge.DoScaledAnimationAsync("Talk", 0.5f);}),
				new BeatAction.Action(beat + 1f,     delegate { Cadet1.DoScaledAnimationAsync("Halt", 0.5f);}),
                new BeatAction.Action(beat + 1f,     delegate { Cadet2.DoScaledAnimationAsync("Halt", 0.5f);}),
				new BeatAction.Action(beat + 1f,     delegate { Cadet3.DoScaledAnimationAsync("Halt", 0.5f);}),
				});
        }
		
		public static void AttentionSound(float beat)
        {
            MultiSound.Play(new MultiSound.Sound[] {
            new MultiSound.Sound("marchingOrders/attention1", beat),
            new MultiSound.Sound("marchingOrders/attention2", beat + 0.25f),
            new MultiSound.Sound("marchingOrders/attention3", beat + 0.75f),
            }, forcePlay:true);
        }
		
		public static void MarchSound(float beat)
        {
            MultiSound.Play(new MultiSound.Sound[] {
            new MultiSound.Sound("marchingOrders/march1", beat),
            new MultiSound.Sound("marchingOrders/march2", beat + 1f),
            }, forcePlay:true);
        }
		
		public static void HaltSound(float beat)
        {
            MultiSound.Play(new MultiSound.Sound[] {
            new MultiSound.Sound("marchingOrders/halt1", beat),
            new MultiSound.Sound("marchingOrders/halt2", beat + 1f),
            }, forcePlay:true);
        }
    }
}

