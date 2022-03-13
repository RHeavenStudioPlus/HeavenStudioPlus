using RhythmHeavenMania.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Games
{
    // none yet
    //using Scripts_FanClub;

    public class FanClub : Minigame
    {
        // userdata here

        // end userdata

        public static FanClub instance;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {

        }
    }
}