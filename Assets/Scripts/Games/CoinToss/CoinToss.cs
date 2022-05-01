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

        //Right now, you can only throw one coin at a time.
        //..Which makes sense, you only have one coin in the original game

        //Though it would need a bit of code rewrite to make it work with multiple coins

        public static CoinToss instance { get; set; }
        public Boolean isThrowing;

        public GameObject coin_cue;

        public GameObject current_coin;

        [Header("Animators")]
        public Animator handAnimator;

        private void Awake()
        {
            instance = this;
            isThrowing = false;
            current_coin = null;
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
            //Play sound and animations
            Jukebox.PlayOneShotGame("coinToss/throw");
            handAnimator.Play("Throw", 0, 0);
            //Game state says the hand is throwing the coin
            isThrowing = true;

            //Delete the current coin to clean up overlapping instances
            if(current_coin != null)
            {
                Destroy(current_coin);
                current_coin = null;
            }

            //Create a new coin to throw
            GameObject coin = Instantiate(coin_cue);
            coin.SetActive(true);
            Coin c = coin.GetComponent<Coin>();
            c.startBeat = beat;
            c.audienceReacting = audienceReacting;

            current_coin = coin;
        }

        public void Catch_Success(bool audienceReacting)
        {
            Jukebox.PlayOneShotGame("coinToss/catch");
            if(audienceReacting) Jukebox.PlayOneShotGame("coinToss/applause");
            handAnimator.Play("Catch_success", 0, 0);

            isThrowing = false;
        }

        public void Catch_Miss(bool audienceReacting)
        {
            Jukebox.PlayOneShotGame("coinToss/miss");
            if(audienceReacting) Jukebox.PlayOneShotGame("coinToss/disappointed");
            handAnimator.Play("Pickup", 0, 0);

            isThrowing = false;
        }

        public void Catch_Empty()
        {
            handAnimator.Play("Catch_empty", 0, 0);
            isThrowing = false;
        }
    }
}
