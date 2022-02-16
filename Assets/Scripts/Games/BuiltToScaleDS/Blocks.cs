using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RhythmHeavenMania.Games.BuiltToScaleDS
{
    using RhythmHeavenMania.Util;
    public class Blocks : PlayerActionObject
    {
        public float createBeat;
        public float createLength;
        public Animator anim;

        private bool moving = true;
        private BuiltToScaleDS game;

        private void Awake()
        {
            game = BuiltToScaleDS.instance;
        }

        private void Update()
        {
            if (!moving) return;

            var windupBeat = createBeat + (createLength * 3.5f);
            var hitBeat = windupBeat + createLength;
            var currentBeat = Conductor.instance.songPositionInBeats;

            var shooterState = game.shooterAnim.GetCurrentAnimatorStateInfo(0);
            if (currentBeat > windupBeat && currentBeat < hitBeat
                && !shooterState.IsName("Windup")
                && game.shooterAnim.IsAnimationNotPlaying())
            {
                game.shooterAnim.Play("Windup", 0, 0);
            }

            float stateBeat = Conductor.instance.GetPositionFromMargin(createBeat + (createLength * 4.5f), 1f);
            StateCheck(stateBeat);

            if (PlayerInput.Pressed())
            {
                if (state.perfect)
                {
                    Ace();
                }
                else if (state.notPerfect())
                {
                    Miss();
                }
            }

            if (moving && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
            {
                game.SetBlockTime(this, createBeat, createLength);
            }
        }

        void Ace()
        {
            moving = false;
            game.shootingThisFrame = true;

            game.Shoot();
            game.SpawnObject(BuiltToScaleDS.BTSObject.HitPieces);
            Destroy(gameObject);

            Jukebox.PlayOneShotGame("builtToScaleDS/Hit");
        }

        void Miss()
        {
            moving = false;
            game.shootingThisFrame = true;

            game.Shoot();
            game.SpawnObject(BuiltToScaleDS.BTSObject.MissPieces);
            Destroy(gameObject);
        }

        public override void OnAce()
        {
            Ace();
        }
    }
}