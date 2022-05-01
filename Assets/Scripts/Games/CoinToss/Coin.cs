using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_CoinToss
{
    public class Coin : PlayerActionObject
    {
        public float startBeat;

        public bool audienceReacting;

        void Awake()
        {
            PlayerActionInit(this.gameObject, startBeat);
        }

        public override void OnAce()
        {
            Hit();
        }

        void Update()
        {
            //Make sure there's no overlapping coin cues.
            if (CoinToss.instance.current_coin != this.gameObject) Destroy(this.gameObject);

            if (Conductor.instance.GetPositionFromBeat(startBeat, 1f) >= 6.3f)
                MissCoin();

            float normalizedBeat = Conductor.instance.GetPositionFromBeat(startBeat, 6f);
            StateCheck(normalizedBeat);

            if (PlayerInput.Pressed())
            {
                if (state.perfect)
                {
                    Hit();
                } else
                {
                    CoinToss.instance.Catch_Empty();
                }
            }
        }

        public void Hit()
        {
            if(CoinToss.instance.isThrowing)
            {
                CoinToss.instance.Catch_Success(audienceReacting);
                Destroy(this.gameObject);
            }
        }

        public void MissCoin()
        {
            CoinToss.instance.Catch_Miss(audienceReacting);
            Destroy(this.gameObject);
        }
    }
}