using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using NaughtyBezierCurves;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_CoinToss
{
    public class Arm : PlayerActionObject
    {

        private Boolean isThrowing;
        private Animator handAnimator;

        private CoinToss game;

        // Start is called before the first frame update
        private void Awake()
        {
            isThrowing = false;
            game = CoinToss.instance;
            handAnimator = game.handAnimator;
        }

        // Update is called once per frame
        private void Update()
        {
            if (PlayerInput.Pressed() && state.perfect)
            {
                if (isThrowing)
                {
                    Catch_Success();
                    isThrowing = false;
                }
            }
        }

        public override void OnAce()
        {
            if(isThrowing)
            {
                Catch_Success();
                isThrowing = false;
            }
        }

        public void Catch_Success()
        {
            Jukebox.PlayOneShotGame("coinToss/catch");
            handAnimator.Play("Catch_success", 0, 0);
        }
    }

}