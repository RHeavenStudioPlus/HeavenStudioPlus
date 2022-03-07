using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Starpelly;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.DrummingPractice
{
    public class DrummingPractice : Minigame
    {
        [Header("References")]
        public SpriteRenderer backgroundGradient;
        public Drummer player;
        public Drummer leftDrummer;
        public Drummer rightDrummer;


        public static DrummingPractice instance;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            
        }

    }
}