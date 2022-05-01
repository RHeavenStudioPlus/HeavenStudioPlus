using DG.Tweening;
using NaughtyBezierCurves;
using HeavenStudio.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class NtrCoinLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("coinToss", "Coin Toss", "B4E6F6", false, false, new List<GameAction>()
            {
                new GameAction("toss",                 delegate { CoinToss.instance.TossCoin(eventCaller.currentEntity.beat); }, 7, false),
            });
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_CoinToss;
    public class CoinToss : Minigame
    {

        public static CoinToss instance { get; set; }

        public Boolean isThrowing;

        [Header("Animators")]
        public Animator handAnimator;

        private void Awake()
        {
            instance = this;
            isThrowing = false;
        }

        private void Update()
        {
           //pass
        }

        private void LateUpdate()
        {
          //pass
        }

        public void TossCoin(float beat)
        {
            Jukebox.PlayOneShotGame("coinToss/throw");
            handAnimator.Play("Throw", 0, 0);

            isThrowing = true;
        }

    }
}
