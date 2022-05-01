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
            return new Minigame("coinToss", "Coin Toss \n [One coin at a time!]", "B4E6F6", false, false, new List<GameAction>()
            {
                new GameAction("toss",                 delegate { CoinToss.instance.TossCoin(eventCaller.currentEntity.beat, eventCaller.currentEntity.toggle); }, 7, false, parameters: new List<Param>()
                {
                    new Param("toggle", false, "Audience Reaction", "Enable Audience Reaction"),
                }),
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

        private int nbCoinThrown; //Variable used if multiple coins are thrown. But it's pretty buggy at the moment and should not be used at the moment

        [Header("Animators")]
        public Animator handAnimator;

        private void Awake()
        {
            instance = this;
            nbCoinThrown = 0;
        }

        private void Update()
        {
           //pass
        }

        private void LateUpdate()
        {
          //pass
        }

        public void TossCoin(float beat, bool audienceReacting)
        {
            Jukebox.PlayOneShotGame("coinToss/throw");
            handAnimator.Play("Throw", 0, 0);

            isThrowing = true;

            GameObject coin = Instantiate(coin_cue);
            coin.SetActive(true);
            Coin c = coin.GetComponent<Coin>();
            c.startBeat = beat;
            c.audienceReacting = audienceReacting;

            nbCoinThrown++;
        }

        public void Catch_Success(bool audienceReacting)
        {
            Jukebox.PlayOneShotGame("coinToss/catch");
            if(audienceReacting) Jukebox.PlayOneShotGame("coinToss/applause");
            handAnimator.Play("Catch_success", 0, 0);

            if(nbCoinThrown > 0) nbCoinThrown--;
            if(nbCoinThrown == 0) isThrowing = false;
        }

        public void Catch_Miss(bool audienceReacting)
        {
            Jukebox.PlayOneShotGame("coinToss/miss");
            if(audienceReacting) Jukebox.PlayOneShotGame("coinToss/disappointed");
            handAnimator.Play("Pickup", 0, 0);

            if (nbCoinThrown > 0) nbCoinThrown--;
            if (nbCoinThrown == 0) isThrowing = false;
        }

        public void Catch_Empty()
        {
            handAnimator.Play("Catch_empty", 0, 0);
            isThrowing = false;
        }
    }
}
