using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Starpelly;
using DG.Tweening;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.RhythmTweezers
{
    // use PlayerActionObject for the actual tweezers but this isn't playable rn so IDC
    public class RhythmTweezers : Minigame
    {
        public Transform VegetableHolder;
        public GameObject Vegetable;
        public Animator VegetableAnimator;
        public Tweezers Tweezers;
        public GameObject hairBase;
        public GameObject longHairBase;

        public GameObject HairsHolder;
        [NonSerialized] public int hairsLeft = 0;

        public float beatInterval = 4f;
        float intervalStartBeat;
        bool intervalStarted;
        public float tweezerBeatOffset = 0f;

        Tween transitionTween;
        bool transitioning = false;

        public static RhythmTweezers instance { get; set; }

        private void Awake()
        {
            instance = this;
        }

        public void SpawnHair(float beat)
        {
            // End transition early if the next hair is a lil early.
            StopTransitionIfActive();

            // If interval hasn't started, assume this is the first hair of the interval.
            if (!intervalStarted)
            {
                SetIntervalStart(beat, beatInterval);
            }

            Jukebox.PlayOneShotGame("rhythmTweezers/shortAppear", beat);
            Hair hair = Instantiate(hairBase, HairsHolder.transform).GetComponent<Hair>();
            hair.gameObject.SetActive(true);

            float rot = -58f + 116 * Mathp.Normalize(beat, intervalStartBeat, intervalStartBeat + beatInterval - 1f);
            hair.transform.eulerAngles = new Vector3(0, 0, rot);
            hair.createBeat = beat;
            hairsLeft++;
        }

        public void SpawnLongHair(float beat)
        {
            StopTransitionIfActive();

            if (!intervalStarted)
            {
                SetIntervalStart(beat, beatInterval);
            }

            Jukebox.PlayOneShotGame("rhythmTweezers/longAppear", beat);
            LongHair hair = Instantiate(longHairBase, HairsHolder.transform).GetComponent<LongHair>();
            hair.gameObject.SetActive(true);

            float rot = -58f + 116 * Mathp.Normalize(beat, intervalStartBeat, intervalStartBeat + beatInterval - 1f);
            hair.transform.eulerAngles = new Vector3(0, 0, rot);
            hair.createBeat = beat;
            hairsLeft++;
        }

        public void SetIntervalStart(float beat, float interval = 4f)
        {
            // Don't do these things if the interval was already started.
            if (!intervalStarted)
            {
                // End transition early if the interval starts a lil early.
                StopTransitionIfActive();
                hairsLeft = 0;
                intervalStarted = true;
            }

            intervalStartBeat = beat;
            beatInterval = interval;
        }

        const float vegDupeOffset = 16.7f;
        public void NextVegetable(float beat)
        {
            transitioning = true;

            Jukebox.PlayOneShotGame("rhythmTweezers/register", beat);

            // Move both vegetables to the left by vegDupeOffset, then reset their positions.
            // On position reset, reset state of core vegetable.
            transitionTween = VegetableHolder.DOLocalMoveX(-vegDupeOffset, Conductor.instance.secPerBeat * 0.5f)
                .OnComplete(() => {

                var holderPos = VegetableHolder.localPosition;
                VegetableHolder.localPosition = new Vector3(0f, holderPos.y, holderPos.z);

                ResetVegetable();
                transitioning = false;
                intervalStarted = false;

            }).SetEase(Ease.InOutSine);
        }

        private void Update()
        {
            if (!Conductor.instance.isPlaying && !Conductor.instance.isPaused && intervalStarted)
            {
                StopTransitionIfActive();
                ResetVegetable();
                intervalStarted = false;
            }
        }

        private void LateUpdate()
        {
            // Set tweezer angle.
            var tweezerAngle = -180f;
            
            if (intervalStarted)
            {
                var tweezerTime = Conductor.instance.songPositionInBeats - beatInterval - tweezerBeatOffset;
                var unclampedAngle = -58f + 116 * Mathp.Normalize(tweezerTime, intervalStartBeat, intervalStartBeat + beatInterval - 1f);
                tweezerAngle = Mathf.Clamp(unclampedAngle, -180f, 180f);
            }

            Tweezers.transform.eulerAngles = new Vector3(0, 0, tweezerAngle);

            // Set tweezer to follow vegetable.
            var currentTweezerPos = Tweezers.transform.localPosition;
            var vegetablePos = Vegetable.transform.localPosition;
            var vegetableHolderPos = VegetableHolder.transform.localPosition;
            Tweezers.transform.localPosition = new Vector3(vegetableHolderPos.x, vegetablePos.y + 1f, currentTweezerPos.z);
        }

        private void ResetVegetable()
        {
            foreach (Transform t in HairsHolder.transform)
            {
                var go = t.gameObject;
                if (go != hairBase)
                {
                    GameObject.Destroy(go);
                }
            }

            VegetableAnimator.Play("Idle", 0, 0);
        }

        private void StopTransitionIfActive()
        {
            if (transitioning)
            {
                if (transitionTween != null)
                    transitionTween.Kill(true);
            }
        }
    }
}
