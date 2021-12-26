using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.Spaceball
{
    public class Spaceball : Minigame
    {
        public GameObject Ball;
        public GameObject BallsHolder;

        public GameObject Dispenser;
        public GameObject Dust;

        private float lastCamDistance;
        private bool zoomingCamera = false;
        private float lastZoomCamBeat;
        private float lastZoomCamLength;
        private float lastZoomCamDistance;

        public Sprite[] Balls;

        public static Spaceball instance { get; set; }

        public override void OnGameSwitch()
        {
            for (int i = 1; i < BallsHolder.transform.childCount; i++)
                Destroy(BallsHolder.transform.GetChild(i).gameObject);
            GameManager.instance.GameCamera.orthographic = false;
            SpaceballPlayer.instance.EligibleHits.RemoveRange(0, SpaceballPlayer.instance.EligibleHits.Count);
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

            if (zoomingCamera)
            {
                float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(lastZoomCamBeat, lastZoomCamLength);
                float newPosZ = Mathf.Lerp(lastCamDistance, lastZoomCamDistance, normalizedBeat);
                GameManager.instance.GameCamera.transform.localPosition = new Vector3(0, 0, newPosZ);
            }
        }


        public void CameraZoom(float beat, float length, float distance)
        {
            lastZoomCamBeat = beat;
            lastZoomCamLength = length;

            float dist = distance;
            dist = dist * -1;

            if (dist > 0)
                lastZoomCamDistance = 0;
            else
                lastZoomCamDistance = dist;

            zoomingCamera = true;
            lastCamDistance = GameManager.instance.GameCamera.transform.localPosition.z;
        }

        public void Shoot(float beat, bool high, string type)
        {
            GameObject ball = Instantiate(Ball);
            ball.transform.parent = Ball.transform.parent;
            ball.SetActive(true);
            ball.GetComponent<SpaceballBall>().startBeat = beat;
            if (high)
            {
                ball.GetComponent<SpaceballBall>().high = true;
                Jukebox.PlayOneShotGame("spaceball/longShoot");
            }
            else
            {
                Jukebox.PlayOneShotGame("spaceball/shoot");
            }

            if (type == "riceball")
            {
                ball.GetComponent<SpaceballBall>().Sprite.sprite = Balls[1];
            }

            Dispenser.GetComponent<Animator>().Play("DispenserShoot", 0, 0);
        }
    }
}