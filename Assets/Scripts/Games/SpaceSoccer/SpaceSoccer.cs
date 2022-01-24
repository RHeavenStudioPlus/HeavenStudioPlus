using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.SpaceSoccer
{
    public class SpaceSoccer : Minigame
    {
        [Header("Components")]
        [SerializeField] private GameObject ballRef;
        [SerializeField] private Kicker kicker;

        [Header("Properties")]
        [SerializeField] private bool ballDispensed;

        public static SpaceSoccer instance { get; private set; }

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (ballDispensed)
            {
            }
        }

        public void Dispense(float beat)
        {
            ballDispensed = true;

            GameObject ball = Instantiate(ballRef, this.transform);
            Ball ball_ = ball.GetComponent<Ball>();
            ball_.dispensedBeat = beat;
            ball_.dispensing = true;

            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("spaceSoccer/dispenseNoise", beat),
                new MultiSound.Sound("spaceSoccer/dispenseTumble1", beat + 0.25f),
                new MultiSound.Sound("spaceSoccer/dispenseTumble2", beat + 0.5f),
                new MultiSound.Sound("spaceSoccer/dispenseTumble2B", beat + 0.5f),
                new MultiSound.Sound("spaceSoccer/dispenseTumble3", beat + 0.75f),
                new MultiSound.Sound("spaceSoccer/dispenseTumble4", beat + 1f),
                new MultiSound.Sound("spaceSoccer/dispenseTumble5", beat + 1.25f),
                new MultiSound.Sound("spaceSoccer/dispenseTumble6", beat + 1.5f),
                new MultiSound.Sound("spaceSoccer/dispenseTumble6B", beat + 1.75f),
            });
        }

        public void KeepUp(float beat, float length)
        {
            kicker.KeepUp(beat, length);
        }

        public void HighKick(float beat)
        {
            kicker.HighKick(beat);
        }
    }

}