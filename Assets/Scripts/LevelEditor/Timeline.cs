using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Starpelly;

namespace RhythmHeavenMania.Editor
{
    public class Timeline : MonoBehaviour
    {
        [Header("Song Positions")]
        [SerializeField] private TMP_Text SongBeat;
        [SerializeField] private TMP_Text SongPos;

        [Header("Timeline Properties")]
        private float lastBeatPos = 0;
        private Vector2 lastMousePos;

        [Header("Timeline Components")]
        [SerializeField] private RectTransform TimelineSlider;
        [SerializeField] private TMP_Text TimelinePlaybackBeat;
        [SerializeField] private RectTransform TimelineContent;
        [SerializeField] private RectTransform TimelineSongPosLineRef;
        [SerializeField] private RectTransform TimelineEventObjRef;
        private RectTransform TimelineSongPosLine;

        public static Timeline instance { get; private set; }

        #region Initializers

        public void Init()
        {
            instance = this;

            for (int i = 0; i < GameManager.instance.Beatmap.entities.Count; i++)
            {
                var entity = GameManager.instance.Beatmap.entities[i];
                var e = GameManager.instance.Beatmap.entities[i];

                EventCaller.GameAction gameAction = EventCaller.instance.GetGameAction(EventCaller.instance.GetMinigame(e.datamodel.Split(0)), e.datamodel.Split(1));

                GameObject g = Instantiate(TimelineEventObjRef.gameObject, TimelineEventObjRef.parent);
                g.transform.localPosition = new Vector3(e.beat, Mathp.Round2Nearest(Random.Range(0, -205.36f), 51.34f));
                g.transform.GetChild(1).GetComponent<TMP_Text>().text = e.datamodel.Split('/')[1];

                TimelineEventObj eventObj = g.GetComponent<TimelineEventObj>();
                eventObj.Icon.sprite = Editor.GameIcon(e.datamodel.Split(0));

                if (gameAction != null)
                {
                    g.GetComponent<RectTransform>().sizeDelta = new Vector2(gameAction.defaultLength, g.GetComponent<RectTransform>().sizeDelta.y);
                    float length = gameAction.defaultLength;
                    eventObj.length = length;
                }

                g.SetActive(true);
                entity.eventObj = g.GetComponent<TimelineEventObj>();
                entity.track = (int)(g.transform.localPosition.y / 51.34f * -1);
            }
        }

        #endregion

        private void Update()
        {
            if (!Conductor.instance.isPlaying && !Conductor.instance.isPaused)
            {
                SongBeat.text = $"Beat {string.Format("{0:0.000}", TimelineSlider.localPosition.x)}";
                SongPos.text = FormatTime(Conductor.instance.GetSongPosFromBeat(TimelineSlider.localPosition.x));
            }
            else
            {
                SongBeat.text = $"Beat {string.Format("{0:0.000}", Conductor.instance.songPositionInBeats)}";
                SongPos.text = FormatTime(Conductor.instance.songPosition);
            }

            SliderControl();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    PlayCheck(false);
                }
                else
                {
                    PlayCheck(true);
                }
            }


            lastBeatPos = Conductor.instance.songPositionInBeats;

            if (Input.GetMouseButton(1) && !Conductor.instance.isPlaying)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(TimelineContent, Input.mousePosition, Editor.instance.EditorCamera, out lastMousePos);
                TimelineSlider.localPosition = new Vector3(Mathp.Round2Nearest(lastMousePos.x + 0.12f, 0.25f), TimelineSlider.transform.localPosition.y);

                Conductor.instance.SetBeat(TimelineSlider.transform.localPosition.x);
            }
        }

        private void SliderControl()
        {
            TimelinePlaybackBeat.text = $"Beat {string.Format("{0:0.000}", TimelineSlider.localPosition.x)}";

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
                if (!Conductor.instance.isPlaying)
                {
                    Play(false, TimelineSlider.transform.localPosition.x);
                }
                else
                {
                    Stop(TimelineSlider.transform.localPosition.x);
                }
                    
            }
            else
            {
                if (!Conductor.instance.isPlaying)
                {
                    Play(false, TimelineSongPosLine.transform.localPosition.x);
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Play(bool fromStart, float time)
        {
            // if (fromStart) Stop();

            if (!Conductor.instance.isPaused)
            {
                TimelineSongPosLine = Instantiate(TimelineSongPosLineRef, TimelineSongPosLineRef.parent).GetComponent<RectTransform>();
                TimelineSongPosLine.gameObject.SetActive(true);
            }

            Conductor.instance.Play(time);
        }

        public void Pause()
        {
            // isPaused = true;
            Conductor.instance.Pause();
        }

        public void Stop(float time)
        {
            // isPaused = true;
            // timelineSlider.value = 0;

            if (TimelineSongPosLine != null)
            Destroy(TimelineSongPosLine.gameObject);

            Conductor.instance.Stop(time);
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

        public bool CheckIfMouseInTimeline()
        {
            return (this.gameObject.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(TimelineContent.transform.parent.gameObject.GetComponent<RectTransform>(), Input.mousePosition, Camera.main));
        }
        #endregion

        #region Functions

        public void AddEventObject(string eventName, bool dragNDrop = false)
        {
            GameObject g = Instantiate(TimelineEventObjRef.gameObject, TimelineEventObjRef.parent);
            g.transform.localPosition = new Vector3(0, 0);
            g.transform.GetChild(1).GetComponent<TMP_Text>().text = eventName.Split('/')[1];

            TimelineEventObj eventObj = g.GetComponent<TimelineEventObj>();
            eventObj.Icon.sprite = Editor.GameIcon(eventName.Split(0));

            EventCaller.GameAction gameAction = EventCaller.instance.GetGameAction(EventCaller.instance.GetMinigame(eventName.Split(0)), eventName.Split(1));

            if (gameAction != null)
            {
                g.GetComponent<RectTransform>().sizeDelta = new Vector2(gameAction.defaultLength, g.GetComponent<RectTransform>().sizeDelta.y);
                float length = gameAction.defaultLength;
                eventObj.length = length;
            }

            g.SetActive(true);

            Beatmap.Entity entity = new Beatmap.Entity();
            entity.datamodel = eventName;
            entity.eventObj = eventObj;

            GameManager.instance.Beatmap.entities.Add(entity);
            GameManager.instance.SortEventsList();

            g.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (dragNDrop)
            {
                eventObj.OnDown();
            }

            Editor.EventObjs.Add(eventObj);

            // entity.eventObj = g.GetComponent<TimelineEventObj>();
            // entity.track = (int)(g.transform.localPosition.y / 51.34f * -1);
        }

        public void DestroyEventObject(TimelineEventObj eventObj)
        {
            var e = GameManager.instance.Beatmap.entities.Find(c => c.eventObj == eventObj);
            GameManager.instance.Beatmap.entities.Remove(e);
            GameManager.instance.SortEventsList();
            Destroy(eventObj.gameObject);

            Editor.EventObjs.Remove(eventObj);
        }

        #endregion

        #region Commands

        public void Move()
        { 
        }

        public void Undo()
        {

        }

        #endregion
    }
}