using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.Spaceball
{
    public class Spaceball : Minigame
    {
        public GameObject Ball;

        public GameObject Dispenser;
        public GameObject Dust;

        public static Spaceball instance { get; set; }

        public override void OnGameSwitch()
        {
            GameManager.instance.GameCamera.orthographic = false;
        }

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            var allPlayerActions = EventCaller.GetAllPlayerEntities("spaceball");
            int currentPlayerEvent = GameManager.instance.currentPlayerEvent - EventCaller.GetAllPlayerEntitiesExceptBeforeBeat("spaceball", Conductor.instance.songPositionInBeats).Count;

            if (currentPlayerEvent < allPlayerActions.Count)
            {
                if (Conductor.instance.songPositionInBeats > allPlayerActions[currentPlayerEvent].beat - 1)
                {
                    Dispenser.GetComponent<Animator>().Play("DispenserPrepare", 0, 0);
                }
            }
        }

        public void Shoot(float beat)
        {
            Jukebox.PlayOneShotGame("spaceball/shoot");
            GameObject ball = Instantiate(Ball);
            ball.transform.parent = Ball.transform.parent;
            ball.SetActive(true);
            ball.GetComponent<SpaceballBall>().startBeat = beat;

            Dispenser.GetComponent<Animator>().Play("DispenserShoot", 0, 0);
        }
    }
}