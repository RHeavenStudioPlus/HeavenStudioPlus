using HeavenStudio.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class AgbSpaceDanceLoader
    {
        public static Minigame AddGame(EventCaller eventCaller) {
            return new Minigame("spaceDance", "Space Dance \n<color=#eb5454>[WIP don't use]</color>", "000000", false, false, new List<GameAction>()
            {
                
            });
        }
    }
}

namespace HeavenStudio.Games
{
    // using Scripts_RhythmSomen;
    public class SpaceDance : Minigame
    {
        public static SpaceDance instance;

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {

            if (PlayerInput.Pressed() && !IsExpectingInputNow())
            {
            
            }
        }
    }
}