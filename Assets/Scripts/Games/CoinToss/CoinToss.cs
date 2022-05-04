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
    //using Scripts_CoinToss;
    public class CoinToss : Minigame
    {

        //Right now, you can only throw one coin at a time.
        //..Which makes sense, you only have one coin in the original game

        //Though it would need a bit of code rewrite to make it work with multiple coins

        public static CoinToss instance { get; set; }
        public Boolean isThrowing;

        public bool audienceReacting;

        [Header("Animators")]
        public Animator handAnimator;

        public PlayerActionEvent coin;

        private void Awake()
        {
            instance = this;
            isThrowing = false;

            coin = null;
        }

        private void Update()
        {
           //nothing
        }

        private void LateUpdate()
        {
            //nothing
        }

        public void TossCoin(float beat, bool audienceReacting)
        {
            if (coin != null) return;

            //Play sound and animations
            Jukebox.PlayOneShotGame("coinToss/throw");
            handAnimator.Play("Throw", 0, 0);
            //Game state says the hand is throwing the coin
            isThrowing = true;

            this.audienceReacting = audienceReacting;

            coin = ScheduleInput(beat, 6f, InputType.DIRECTION_DOWN, CatchSuccess, CatchMiss, CatchEmpty);
        }

        public void CatchSuccess(int state)
        {
            if (state != 1)
            {
                CatchMiss(); 
                return;
            }

            Jukebox.PlayOneShotGame("coinToss/catch");
            if(this.audienceReacting) Jukebox.PlayOneShotGame("coinToss/applause");
            handAnimator.Play("Catch_success", 0, 0);

            isThrowing = false; 
        }

        public void CatchMiss()
        {
            Jukebox.PlayOneShotGame("coinToss/miss");
            if(this.audienceReacting) Jukebox.PlayOneShotGame("coinToss/disappointed");
            handAnimator.Play("Pickup", 0, 0);

            isThrowing = false;
        }

        public void CatchEmpty()
        {
            handAnimator.Play("Catch_empty", 0, 0);
            isThrowing = false;

            coin.CanHit(false);
        }
    }
}
