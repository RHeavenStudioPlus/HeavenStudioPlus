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

        public GameObject coin_cue;

        [Header("Animators")]
        public Animator handAnimator;

        private void Awake()
        {
            instance = this;
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

            GameObject coin = Instantiate(coin_cue);
            coin.SetActive(true);
            Coin c = coin.GetComponent<Coin>();
            c.startBeat = beat;
        }

        public void Catch_Success()
        {
            Jukebox.PlayOneShotGame("coinToss/catch");
            handAnimator.Play("Catch_success", 0, 0);

            isThrowing = false;
        }

        public void Catch_Miss()
        {
            Jukebox.PlayOneShotGame("coinToss/miss");
        }
    }
}
