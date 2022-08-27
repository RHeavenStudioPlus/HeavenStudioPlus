//notes:
//	BEFORE NEW PROPS
//1. minenice will also use this to test out randomly named parameters so coding has to rest until the new props update
//2. see fan club for separate prefabs (cadets) [DONE]
//3. temporarily take sounds from rhre, wait until someone records the full code, including misses, or record it myself (unlikely) [IN PROGRESS]
//	AFTER NEW PROPS
//4. see space soccer, mr upbeat, tunnel for keep-the-beat codes
//5. figure how to do custom bg changes when the upscaled textures are finished (see karate man, launch party once it releases)
//6. will use a textbox without going through the visual options but i wonder how..?? (see first contact if ever textboxes are implemented in said minigame)
//	AFTER FEATURE COMPLETION
//7. delete all notes once the minigame is considered feature-complete

using HeavenStudio.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class AbgMarcherLoader
    {
        public static Minigame AddGame(EventCaller eventCaller) {
            return new Minigame("marchingOrders", "Marching Orders \n<color=#eb5454>[WIP]</color>", "00A43B", false, false, new List<GameAction>()
                {
					//from krispy:
					//i'm not that good at coding but i'll try my best to make a minigame
					//please don't take over... i'll get back into it once i know coding
					//thank you and have a nice day!
					
					
					
					
					//the cues do nothing at the moment, so i temporarily disabled them
					//new GameAction("marching", delegate { MarchingOrders.instance.CadetsMarch(eventCaller.currentEntity.beat); }, 4f, true),
					
					new GameAction("attention", delegate { MarchingOrders.instance.SargeAttention(eventCaller.currentEntity.beat); }, 2f, false),
					//new GameAction("march", delegate {}, 2f, false),
					//new GameAction("halt", delegate {}, 2f, false),
					//new GameAction("face turn", delegate {}, 4f, false, parameters: new List<Param>()
					//{
                    //    new Param("type", MarchingOrders.DirectionFaceTurn.Right, "Direction", "The direction sarge wants the cadets to face"),
					//  	new Param("type2", MarchingOrders.FaceTurnLength.Normal, "Length", "How fast or slow the event lasts"),
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
		public enum DirectionFaceTurn {
            Right,
            Left,
		}
		public enum FaceTurnLength {
            Normal,
            Fast,
		}
		
		public static MarchingOrders instance;
		
        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            
        }
		
		public void CadetsMarch(float beat)
		{
			
		}
		
		public void SargeAttention(float beat)
		{
			//ScheduleInput(beat, 1f, InputType.DIRECTION_DOWN_DOWN, SitSuccess, SitMiss, SitEmpty);
            MultiSound.Play(new MultiSound.Sound[] {
            new MultiSound.Sound("marchingOrders/attention1", beat - 0.25f),
            new MultiSound.Sound("marchingOrders/attention2", beat),
            new MultiSound.Sound("marchingOrders/attention3", beat + 0.5f),
            });
			
		}
    }
}

