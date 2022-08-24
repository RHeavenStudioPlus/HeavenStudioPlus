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
            return new Minigame("marchingOrders", "Marching Orders \n<color=#eb5454>[WIP]</color>", "000000", false, false, new List<GameAction>()
                {
					//from krispy:
					//i'm not that good at coding but i'll try my best to make a minigame
					//please don't take over... i'll get back into it once i know coding
					//thank you and have a nice day!
					
					
					
					
					//the cues do nothing at the moment, so i temporarily disabled them
					//new GameAction("marching", delegate {}, 4f, true),
					
					//new GameAction("attention", delegate {}, 2f, false),
					//new GameAction("march", delegate {}, 2f, false),
					//new GameAction("halt", delegate {}, 2f, false),
					//new GameAction("face, turn", delegate {}, 4f, false),
                });
        }
    }
}

namespace HeavenStudio.Games
{
    // using Scripts_MarchingOrders;
    public class MarchingOrders : Minigame
    {
        // Start is called before the first frame update
        void Awake()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}

