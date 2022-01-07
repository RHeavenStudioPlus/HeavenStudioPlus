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
        [SerializeField] private RectTransform TimelineSlider;
        [SerializeField] private RectTransform TimelineContent;
        [SerializeField] private RectTransform TimelineSongPosLineRef;
        private RectTransform TimelineSongPosLine;

        #region Initializers

        public void Init()
        {
        }

        #endregion

        private void Update()
        {
            SongBeat.text = $"Beat {string.Format("{0:0.000}", Conductor.instance.songPositionInBeats)}";
            SongPos.text = FormatTime(Conductor.instance.songPosition);

            isPlaying = Conductor.instance.isPlaying;

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
            if (TimelineSongPosLine != null)
            {
                TimelineSongPosLine.transform.localPosition = new Vector3(Conductor.instance.songPositionInBeats, TimelineSlider.transform.localPosition.y);
                TimelineSongPosLine.transform.localScale = new Vector3(1f / TimelineContent.transform.localScale.x, TimelineSlider.transform.localScale.y, 1);
            }
        }

        #region PlayChecks
        public void PlayCheck(bool fromStart)
        {
            if (fromStart)
            {
                if (isPlaying)
                    Play(true);
                else
                    Stop();
            }
            else
            {
                if (!isPlaying)
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

            if (!Conductor.instance.isPaused)
            {
                TimelineSongPosLine = Instantiate(TimelineSongPosLineRef, TimelineSongPosLineRef.parent).GetComponent<RectTransform>();
                TimelineSongPosLine.gameObject.SetActive(true);
            }

            Conductor.instance.Play();
        }

        public void Pause()
        {
            // isPaused = true;
            Conductor.instance.Pause();
        }

        public void Stop()
        {
            // isPaused = true;
            // timelineSlider.value = 0;

            Destroy(TimelineSongPosLine.gameObject);

            Conductor.instance.Stop();
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