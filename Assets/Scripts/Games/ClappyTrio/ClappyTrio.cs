using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.ClappyTrio
{
    public class ClappyTrio : Minigame
    {
        public int lionCount = 3;

        public List<GameObject> Lion;

        [SerializeField] private Sprite[] faces;

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

        public override void OnGameSwitch()
        {
            for (int i = 0; i < Lion.Count; i++)
            {
                SetFace(i, 0);
            }
            PlayAnimationAll("Idle");
        }

        private void Start()
        {
            float maxWidth = 9.2f;
            float minus = 0;

            float newSpacing = maxWidth / lionCount;

            if (lionCount > 3)
            {
                Lion[0].transform.localPosition = new Vector3(-1.5f, 0);
                maxWidth = 6.2f;
                minus = 1.5f;
            }

            for (int i = 1; i < lionCount; i++)
            {
                GameObject lion = Instantiate(Lion[0], Lion[0].transform.parent);

                // lion.transform.localPosition = new Vector3(Lion[0].transform.localPosition.x + (1.0333f * lionCount) - i, 0);
                lion.transform.localPosition = new Vector3((newSpacing) * (i) - minus, 0);
                Lion.Add(lion);

                if (i == lionCount - 1)
                {
                    ClappyTrioPlayer = lion.AddComponent<ClappyTrioPlayer>();
                }
            }

            /*LionMiddle = Instantiate(LionLeft, LionLeft.transform.parent);
            LionMiddle.transform.localPosition = new Vector3(3.1f, 0);

            LionPlayer = Instantiate(LionLeft, LionLeft.transform.parent);
            LionPlayer.transform.localPosition = new Vector3(6.2f, 0);
            ClappyTrioPlayer = LionPlayer.AddComponent<ClappyTrioPlayer>();


            lionHeadLeft = LionLeft.transform.GetChild(1).GetComponent<SpriteRenderer>();
            lionHeadMiddle = LionMiddle.transform.GetChild(1).GetComponent<SpriteRenderer>();
            lionHeadPlayer = LionPlayer.transform.GetChild(1).GetComponent<SpriteRenderer>();*/
        }

        private void Update()
        {
            if (isClapping)
            {
                float songPosBeat = Conductor.instance.songPositionInBeats;

                for (int i = 0; i < Lion.Count; i++)
                {
                    float minus = 0;

                    // i spent like 25 minutes trying to figure out what was wrong with this when i forgot to subtract the currentClapLength :(
                    if (i == Lion.Count - 1)
                        minus = 0.35f;

                    if (songPosBeat > lastClapStart + (currentClappingLength * (i) - minus) && songPosBeat < lastClapStart + (currentClappingLength * (i + 1)) && clapIndex == i)
                    {
                        if (i == Lion.Count - 1)
                        {
                            ClappyTrioPlayer.SetClapAvailability(lastClapStart + (currentClappingLength * i - 0.35f));

                            clapIndex = 0;
                            isClapping = false;
                            currentClappingLength = 0;
                            ClappyTrioPlayer.clapStarted = false;
                        }
                        else
                        {
                            SetFace(i, 4);
                            Lion[i].GetComponent<Animator>().Play("Clap", 0, 0);
                            Jukebox.PlayOneShotGame("clappyTrio/leftClap");
                            clapIndex++;
                        }
                        break;
                    }
                }

                /*if (songPosBeat > lastClapStart && songPosBeat < lastClapStart + 1 && clapIndex == 0)
                {
                    Debug.Log(Conductor.instance.songPositionInBeats);
                    SetFace(0, 4);
                    Lion[0].GetComponent<Animator>().Play("Clap", 0, 0);
                    Jukebox.PlayOneShotGame("clappyTrio/leftClap");

                    clapIndex++;
                }
                else if (songPosBeat > lastClapStart + currentClappingLength && songPosBeat < lastClapStart + (currentClappingLength * 2) && clapIndex == 1)
                {
                    Debug.Log(Conductor.instance.songPositionInBeats);
                    SetFace(1, 4);
                    Lion[1].GetComponent<Animator>().Play("Clap", 0, 0);
                    Jukebox.PlayOneShotGame("clappyTrio/middleClap");

                    clapIndex++;
                }
                else if (songPosBeat > lastClapStart + (currentClappingLength * 2 - 0.35f) && clapIndex == 2)
                {
                    Debug.Log(Conductor.instance.songPositionInBeats);
                    ClappyTrioPlayer.SetClapAvailability(lastClapStart + (currentClappingLength * 2 - 0.35f));

                    clapIndex = 0;
                    isClapping = false;
                    currentClappingLength = 0;
                    ClappyTrioPlayer.clapStarted = false;
                }*/
            }
        }

        public void Clap(float beat, float length)
        {
            ClappyTrioPlayer.clapStarted = true;
            ClappyTrioPlayer.canHit = true; // this is technically a lie, this just restores the ability to hit

            playerHitLast = false;
            isClapping = true;
            lastClapStart = beat;
            currentClappingLength = length;
        }

        public void Prepare(int type)
        {
            for (int i = 0; i < Lion.Count; i++)
            {
                SetFace(i, type);
            }
            PlayAnimationAll("Prepare");
            Jukebox.PlayOneShotGame("clappyTrio/ready");
        }

        public void Bop()
        {
            if (playerHitLast)
            {
                for (int i = 0; i < Lion.Count; i++)
                {
                    SetFace(i, 1);
                }
            }
            else
            {
                for (int i = 0; i < Lion.Count; i++)
                {
                    if (i == Lion.Count - 1)
                    {
                        SetFace(i, 0);
                    }
                    else
                    {
                        SetFace(i, 2);
                    }
                }
            }
            PlayAnimationAll("Bop");
        }

        private void PlayAnimationAll(string anim)
        {
            for (int i = 0; i < Lion.Count; i++)
            {
                Lion[i].GetComponent<Animator>().Play(anim, -1, 0);
            }
        }

        public void SetFace(int lion, int type)
        {
            Lion[lion].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = faces[type];
        }
    }
}