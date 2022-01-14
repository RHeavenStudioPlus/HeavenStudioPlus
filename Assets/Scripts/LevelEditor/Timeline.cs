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
        public List<TimelineEventObj> eventObjs = new List<TimelineEventObj>();
        private bool lastFrameDrag;
        public int LayerCount = 4;

        [Header("Timeline Components")]
        [SerializeField] private RectTransform TimelineSlider;
        [SerializeField] private TMP_Text TimelinePlaybackBeat;
        [SerializeField] private RectTransform TimelineContent;
        [SerializeField] private RectTransform TimelineSongPosLineRef;
        [SerializeField] private RectTransform TimelineEventObjRef;
        [SerializeField] private RectTransform LayersRect;
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

                AddEventObject(e.datamodel, false, new Vector3(e.beat, Mathp.Round2Nearest(Random.Range(0, -LayersRect.rect.height), LayerHeight())), i);
            }

            TimelineSlider.GetChild(0).GetComponent<Image>().color = EditorTheme.theme.properties.BeatMarkerCol.Hex2RGB();
            TimelineSlider.GetChild(1).GetComponent<Image>().color = EditorTheme.theme.properties.BeatMarkerCol.Hex2RGB();
            TimelineSlider.GetChild(2).GetComponent<TMP_Text>().color = EditorTheme.theme.properties.BeatMarkerCol.Hex2RGB();
            TimelineSlider.GetChild(3).GetComponent<TMP_Text>().color = EditorTheme.theme.properties.BeatMarkerCol.Hex2RGB();
            TimelineSongPosLineRef.GetComponent<Image>().color = EditorTheme.theme.properties.CurrentTimeMarkerCol.Hex2RGB();
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
                TimelineSongPosLine.transform.localPosition = new Vector3(Conductor.instance.songPositionInBeats, TimelineSongPosLine.transform.localPosition.y);
                TimelineSongPosLine.transform.localScale = new Vector3(1f / TimelineContent.transform.localScale.x, TimelineSongPosLine.transform.localScale.y, 1);
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

            GameManager.instance.Play(time);
        }

        public void Pause()
        {
            // isPaused = true;
            GameManager.instance.Pause();
        }

        public void Stop(float time)
        {
            // isPaused = true;
            // timelineSlider.value = 0;

            if (TimelineSongPosLine != null)
            Destroy(TimelineSongPosLine.gameObject);

            GameManager.instance.Stop(time);
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

        public void AddEventObject(string eventName, bool dragNDrop = false, Vector3 pos = new Vector3(), int entityId = 0)
        {
            GameObject g = Instantiate(TimelineEventObjRef.gameObject, TimelineEventObjRef.parent);
            g.transform.localPosition = pos;
            g.transform.GetChild(2).GetComponent<TMP_Text>().text = eventName.Split('/')[1];

            TimelineEventObj eventObj = g.GetComponent<TimelineEventObj>();
            eventObj.Icon.sprite = Editor.GameIcon(eventName.Split(0));

            EventCaller.GameAction gameAction = EventCaller.instance.GetGameAction(EventCaller.instance.GetMinigame(eventName.Split(0)), eventName.Split(1));

            if (gameAction != null)
            {
                g.GetComponent<RectTransform>().sizeDelta = new Vector2(gameAction.defaultLength, LayerHeight());
                float length = gameAction.defaultLength;
                eventObj.length = length;
            }

            g.SetActive(true);

            if (dragNDrop)
            {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                g.transform.position = new Vector3(mousePos.x, mousePos.y, 0);


                Beatmap.Entity en = new Beatmap.Entity();
                en.datamodel = eventName;
                en.eventObj = eventObj;

                GameManager.instance.Beatmap.entities.Add(en);
                GameManager.instance.SortEventsList();

                Selections.instance.ClickSelect(eventObj);
                eventObj.isDragging = true;
            }
            else
            {
                var entity = GameManager.instance.Beatmap.entities[entityId];
                var e = GameManager.instance.Beatmap.entities[entityId];

                entity.eventObj = g.GetComponent<TimelineEventObj>();
                entity.track = (int)(g.transform.localPosition.y / LayerHeight() * -1);
            }

            Editor.EventObjs.Add(eventObj);
            eventObjs.Add(eventObj);
        }

        public void DestroyEventObject(Beatmap.Entity entity)
        {
            Editor.EventObjs.Remove(entity.eventObj);
            GameManager.instance.Beatmap.entities.Remove(entity);

            Destroy(entity.eventObj.gameObject);
            GameManager.instance.SortEventsList();
        }

        public bool IsMouseAboveEvents()
        {
            return Timeline.instance.eventObjs.FindAll(c => c.mouseHovering == true).Count > 0;
        }

        public bool IsEventsDragging()
        {
            return Timeline.instance.eventObjs.FindAll(c => c.isDragging == true).Count > 0;
        }

        public float SnapToLayer(float y)
        {
            float size = LayerHeight();
            return Mathf.Clamp(Mathp.Round2Nearest(y, size), -size * 3, 0);
        }

        public float LayerHeight()
        {
            return LayersRect.rect.height / 4;
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