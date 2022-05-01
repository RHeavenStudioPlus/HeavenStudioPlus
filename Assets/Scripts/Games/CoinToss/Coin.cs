using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_CoinToss
{
    public class Coin : PlayerActionObject
    {
        public float startBeat;

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
            if (Conductor.instance.GetPositionFromBeat(startBeat, 1f) >= 6.3f)
                MissCoin();

            float normalizedBeat = Conductor.instance.GetPositionFromBeat(startBeat, 6f);
            StateCheck(normalizedBeat);

            if (PlayerInput.Pressed())
            {
                if (state.perfect)
                {
                    Hit();
                }
        }

        public void Hit()
        {
            CoinToss.instance.Catch_Success();
            Destroy(this.gameObject);
        }

        public void MissCoin()
        {
            CoinToss.instance.Catch_Miss();
            Destroy(this.gameObject);
        }
    }
}