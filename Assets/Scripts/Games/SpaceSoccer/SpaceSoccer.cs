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
        [SerializeField] private GameObject Background;
        [SerializeField] private Sprite[] backgroundSprite;

        [Header("Properties")]
        [SerializeField] private bool ballDispensed;

        public static SpaceSoccer instance { get; private set; }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            for (int x = 0; x < Random.Range(9, 12); x++)
            {
                for (int y = 0; y < Random.Range(6, 9); y++)
                {
                    GameObject test = new GameObject("test");
                    test.transform.parent = Background.transform;
                    test.AddComponent<SpriteRenderer>().sprite = backgroundSprite[Random.Range(0, 2)];
                    test.GetComponent<SpriteRenderer>().sortingOrder = -50;
                    test.transform.localPosition = new Vector3(Random.Range(-15f, 15f), Random.Range(-15f, 15f));
                    test.transform.localScale = new Vector3(0.52f, 0.52f);
                }
            }
        }

        private void Update()
        {
            if (ballDispensed)
            {
            }

            
        }

        public void Dispense(float beat)
        {
            if (kicker.ball != null) return;
            ballDispensed = true;

            GameObject ball = Instantiate(ballRef, this.transform);
            Ball ball_ = ball.GetComponent<Ball>();
            ball_.dispensedBeat = beat;
            ball_.dispensing = true;
            kicker.ball = ball_;
            kicker.dispenserBeat = beat;
            kicker.kickTimes = 0;

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
    }

}