using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace RhythmHeavenMania.Editor
{
    public class Timeline : MonoBehaviour
    {
        [Header("Song Positions")]
        [SerializeField] private TMP_Text SongBeat;
        [SerializeField] private TMP_Text SongPos;

        [Header("Timeline Properties")]
        private bool isPlaying = false;
        private float lastBeatPos = 0;

        [Header("Timeline Components")]
        [SerializeField] private Slider TimelineSlider;

        #region Initializers

        public void Init()
        {
        }

        #endregion

        private void Update()
        {
            SongBeat.text = $"Beat {string.Format("{0:0.000}", Conductor.instance.songPositionInBeats)}";
            SongPos.text = FormatTime(Conductor.instance.songPosition);

            isPlaying = Conductor.instance.musicSource.isPlaying;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    PlayCheck(true);
                }
                else
                {
                    PlayCheck(false);
                }
            }

            SliderControl();

            lastBeatPos = Conductor.instance.songPositionInBeats;
        }

        private void SliderControl()
        {
            TimelineSlider.maxValue = Conductor.instance.SongLengthInBeats();

            if (isPlaying)
            {
                TimelineSlider.value = Conductor.instance.songPositionInBeats;
            }
        }

        #region PlayChecks
        public void PlayCheck(bool fromStart)
        {
            if (fromStart)
            {
                if (isPlaying && Conductor.instance.musicSource.clip)
                    Play(true);
                else
                    Stop();
            }
            else
            {
                if (!isPlaying && Conductor.instance.musicSource.clip)
                {
                    Play(false);
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Play(bool fromStart)
        {
            if (fromStart) Stop();
            // isPaused = false;
            Conductor.instance.musicSource.Play();
        }

        public void Pause()
        {
            // isPaused = true;
            Conductor.instance.musicSource.Pause();
        }

        public void Stop()
        {
            // isPaused = true;
            // timelineSlider.value = 0;
            Conductor.instance.musicSource.time = 0;
            Conductor.instance.musicSource.Stop();
        }
        #endregion


        #region Extras
        private string FormatTime(float time)
        {
            int minutes = (int)time / 60;
            int seconds = (int)time - 60 * minutes;
            int milliseconds = (int)(1000 * (time - minutes * 60 - seconds));
            return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
        #endregion
    }
}