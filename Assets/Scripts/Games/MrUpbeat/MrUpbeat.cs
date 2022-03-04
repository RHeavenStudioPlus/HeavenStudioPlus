using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Starpelly;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.MrUpbeat
{
    public class MrUpbeat : Minigame
    {
        [Header("References")]
        public GameObject metronome;
        public Animator animator;
        public Animator blipAnimator;
        public GameObject[] shadows;

        public static MrUpbeat instance;

        private void Awake()
        {
            instance = this;
        }

       

    }
}