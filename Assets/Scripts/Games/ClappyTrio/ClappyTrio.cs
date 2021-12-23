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

        private bool isClapping;
        private float currentClappingLength;
        private float lastClapStart;
        private int clapIndex;

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
        }

        private void Update()
        {
            if (isClapping)
            {
                float songPosBeat = Conductor.instance.songPositionInBeats;

                if (songPosBeat > lastClapStart && songPosBeat < lastClapStart + 1 && clapIndex == 0)
                {
                    LionLeft.GetComponent<Animator>().Play("Clap", 0, 0);
                    Jukebox.PlayOneShotGame("clappyTrio/leftClap");

                    clapIndex++;
                }
                else if (songPosBeat > lastClapStart + currentClappingLength && songPosBeat < lastClapStart + (currentClappingLength * 2) && clapIndex == 1)
                {
                    LionMiddle.GetComponent<Animator>().Play("Clap", 0, 0);
                    Jukebox.PlayOneShotGame("clappyTrio/middleClap");

                    clapIndex++;
                }
                else if (songPosBeat > lastClapStart + (currentClappingLength * 2) && clapIndex == 2)
                {
                    LionPlayer.GetComponent<Animator>().Play("Clap", 0, 0);
                    Jukebox.PlayOneShotGame("clappyTrio/rightClap");

                    clapIndex++;
                }
            }
        }

        public void Clap(float beat, float length)
        {
            isClapping = true;
            lastClapStart = beat;
            currentClappingLength = length;
        }

        public void Prepare(int type)
        {
            PlayAnimationAll("Prepare");
            Jukebox.PlayOneShotGame("clappyTrio/ready");
            SetFace(0, type);
            SetFace(1, type);
            SetFace(2, type);
        }

        private void PlayAnimationAll(string anim)
        {
            LionLeft.GetComponent<Animator>().Play(anim, 0, 0);
            LionMiddle.GetComponent<Animator>().Play(anim, 0, 0);
            LionPlayer.GetComponent<Animator>().Play(anim, 0, 0);
        }

        private void SetFace(int lion, int type)
        {
            if (lion == 0)
                LionLeft.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = faces[type];
            else if (lion == 1)
                LionMiddle.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = faces[type];
            else if (lion == 3)
                LionPlayer.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = faces[type];
        }
    }
}