using System;
using UnityEngine;
using HeavenStudio.Util;

namespace HeavenStudio.StudioDance
{
    public class Dancer : MonoBehaviour
    {
        [SerializeField] ChoreographyInfo debugChoreography;
        [SerializeField] ChoreographyInfo[] choreographies;
        Conductor cond;
        GameManager gm;
        private Animator animator;
        private double currentBeat = 0f;

        private bool isDance = false;

        private ChoreographyInfo currentChoreography;
        private double totalChoreographyLength = 0f;

        public ChoreographyInfo[] ChoreographyInfos { get => choreographies; }
        public ChoreographyInfo CurrentChoreography { get => currentChoreography; }

        public void SetChoreography(ChoreographyInfo choreography)
        {
            currentChoreography = choreography;
            totalChoreographyLength = 0f;
            foreach (var step in choreography.choreographySteps)
            {
                totalChoreographyLength += step.beatLength;
            }
        }

        public void SetChoreography(int index)
        {
            if (index < 0 || index >= choreographies.Length) return;
            var choreography = choreographies[index];
            currentChoreography = choreography;
            totalChoreographyLength = 0f;
            foreach (var step in choreography.choreographySteps)
            {
                totalChoreographyLength += step.beatLength;
            }

            if (cond is not null && animator is not null && !cond.isPlaying)
            {
                animator.Play(currentChoreography.introState, -1, 0f);
            }
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
            cond = Conductor.instance;
            gm = GameManager.instance;

            if (gm != null)
            {
                gm.onBeatPulse += OnBeatPulse;
            }

            if (debugChoreography != null)
            {
                SetChoreography(debugChoreography);
            }

            animator.Play(currentChoreography.introState, -1, 0f);
        }

        public void SetStartChoreography()
        {
            if (debugChoreography != null)
            {
                SetChoreography(debugChoreography);
            }
            else
            {
                SetChoreography(0);
            }
        }

        private void OnBeatPulse(double beat)
        {
            currentBeat = beat;
        }

        private void Update()
        {
            if (currentChoreography == null || cond == null) return;
            if (!cond.isPlaying)
            {
                if (!isDance) return;
                if (currentBeat % 2 != 0)
                {
                    animator.Play(currentChoreography.poseStateOdd, -1, 0f);
                }
                else
                {
                    animator.Play(currentChoreography.poseStateEven, -1, 0f);
                }
                isDance = false;
                return;
            }
            isDance = true;

            float speed = 1f;
            if (currentChoreography.halfSpeedBpm != currentChoreography.doubleSpeedBpm)
            {
                if (cond.songBpm < currentChoreography.halfSpeedBpm)
                {
                    speed = 0.5f;
                }
                else if (cond.songBpm > currentChoreography.doubleSpeedBpm)
                {
                    speed = 2f;
                }
            }

            double choreoBeat = cond.songPositionInBeatsAsDouble % (totalChoreographyLength * speed);
            double cycleStartBeat = Math.Floor(cond.songPositionInBeatsAsDouble / (totalChoreographyLength * speed)) * (totalChoreographyLength * speed);

            double beatSum = 0.0;
            double stepLength = 0.0;
            string stepState = "";
            foreach (ChoreographyInfo.ChoreographyStep s in currentChoreography.choreographySteps)
            {
                if (choreoBeat > beatSum && choreoBeat < beatSum + (s.beatLength * speed))
                {
                    stepLength = s.beatLength * speed;
                    stepState = s.stateName;
                    break;
                }
                beatSum += s.beatLength * speed;
            }
            if (stepState is not null or "")
            {
                animator.DoScaledAnimation(stepState, cycleStartBeat + beatSum, stepLength, animLayer: -1, clamp: true);
            }
        }
    }
}