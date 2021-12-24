using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.ClappyTrio
{
    public class ClappyTrio : MonoBehaviour
    {
        [SerializeField] private GameObject LionLeft;
        private GameObject LionMiddle;
        private GameObject LionPlayer;

        [SerializeField] private Sprite[] faces;
        public SpriteRenderer lionHeadLeft, lionHeadMiddle, lionHeadPlayer;

        private bool isClapping;
        private float currentClappingLength;
        private float lastClapStart;
        private int clapIndex;

        private ClappyTrioPlayer ClappyTrioPlayer;

        public bool playerHitLast = false;


        public static ClappyTrio instance { get; set; }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            LionMiddle = Instantiate(LionLeft, LionLeft.transform.parent);
            LionMiddle.transform.localPosition = new Vector3(3.1f, 0);

            LionPlayer = Instantiate(LionLeft, LionLeft.transform.parent);
            LionPlayer.transform.localPosition = new Vector3(6.2f, 0);
            ClappyTrioPlayer = LionPlayer.AddComponent<ClappyTrioPlayer>();


            lionHeadLeft = LionLeft.transform.GetChild(1).GetComponent<SpriteRenderer>();
            lionHeadMiddle = LionMiddle.transform.GetChild(1).GetComponent<SpriteRenderer>();
            lionHeadPlayer = LionPlayer.transform.GetChild(1).GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (isClapping)
            {
                float songPosBeat = Conductor.instance.songPositionInBeats;

                if (songPosBeat > lastClapStart && songPosBeat < lastClapStart + 1 && clapIndex == 0)
                {
                    SetFace(0, 4);
                    LionLeft.GetComponent<Animator>().Play("Clap", 0, 0);
                    Jukebox.PlayOneShotGame("clappyTrio/leftClap");

                    clapIndex++;
                }
                else if (songPosBeat > lastClapStart + currentClappingLength && songPosBeat < lastClapStart + (currentClappingLength * 2) && clapIndex == 1)
                {
                    SetFace(1, 4);
                    LionMiddle.GetComponent<Animator>().Play("Clap", 0, 0);
                    Jukebox.PlayOneShotGame("clappyTrio/middleClap");

                    clapIndex++;
                }
                else if (songPosBeat > lastClapStart + (currentClappingLength * 2 - 0.35f) && clapIndex == 2)
                {
                    ClappyTrioPlayer.SetClapAvailability(lastClapStart + (currentClappingLength * 2 - 0.35f));

                    clapIndex = 0;
                    isClapping = false;
                    currentClappingLength = 0;
                }
            }
        }

        public void Clap(float beat, float length)
        {
            playerHitLast = false;
            isClapping = true;
            lastClapStart = beat;
            currentClappingLength = length;
        }

        public void Prepare(int type)
        {
            SetFace(0, type);
            SetFace(1, type);
            SetFace(2, type);
            PlayAnimationAll("Prepare");
            Jukebox.PlayOneShotGame("clappyTrio/ready");
        }

        public void Bop()
        {
            if (playerHitLast)
            {
                SetFace(0, 1);
                SetFace(1, 1);
                SetFace(2, 1);
            }
            PlayAnimationAll("Bop");
        }

        private void PlayAnimationAll(string anim)
        {
            LionLeft.GetComponent<Animator>().Play(anim, 0, 0);
            LionMiddle.GetComponent<Animator>().Play(anim, 0, 0);
            LionPlayer.GetComponent<Animator>().Play(anim, 0, 0);
        }

        public void SetFace(int lion, int type)
        {
            if (lion == 0)
                lionHeadLeft.sprite = faces[type];
            if (lion == 1)
                lionHeadMiddle.sprite = faces[type];
            if (lion == 2)
                lionHeadPlayer.sprite = faces[type];
        }
    }
}