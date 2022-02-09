using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.RhythmTweezers
{
    // use PlayerActionObject for the actual tweezers but this isn't playable rn so IDC
    public class RhythmTweezers : Minigame
    {
        public GameObject Vegetable;
        public Animator VegetableAnimator;
        public Tweezers Tweezers;

        [SerializeField] private GameObject HairsHolder;

        public float tweezersBeatOffset;
        public Vector2 tweezersRotOffset;
        public float rotSpd;
        public float offset;

        public static RhythmTweezers instance { get; set; }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            float beat = 0;
            float offset = 0f;
            BeatAction.New(HairsHolder, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + offset,      delegate { SpawnHair(beat + offset); }),
                new BeatAction.Action(beat + 1f + offset, delegate { SpawnHair(beat + 1f + offset); }),
                new BeatAction.Action(beat + 2f + offset, delegate 
                {
                    SpawnHair(beat + 2f + offset); 
                }),
                new BeatAction.Action(beat + 3f + offset, delegate { SpawnHair(beat + 3f + offset); }),
            });
        }

        private void SpawnHair(float beat)
        {
            Jukebox.PlayOneShotGame("rhythmTweezers/shortAppear", beat);
            GameObject hair = Instantiate(HairsHolder.transform.GetChild(0).gameObject, HairsHolder.transform);
            hair.SetActive(true);

            float rot = ((offset / 3f) * (beat * 2f)) - offset;

            hair.transform.eulerAngles = new Vector3(0, 0, rot);
            hair.GetComponent<Hair>().createBeat = beat;
        }

        private void Update()
        {
            float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(tweezersBeatOffset, rotSpd);
            float rot = Mathf.Lerp(tweezersRotOffset.x, tweezersRotOffset.y, normalizedBeat);
            Tweezers.transform.eulerAngles = new Vector3(0, 0, rot);
        }
    }
}
