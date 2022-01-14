using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Starpelly;
using DG.Tweening;

namespace RhythmHeavenMania.Editor
{
    public class TimelineEventObj : MonoBehaviour
    {
        private float startPosX;
        private float startPosY;
        public bool isDragging;

        private Vector3 lastPos;

        [Header("Components")]
        [SerializeField] private RectTransform PosPreview;
        [SerializeField] private RectTransform PosPreviewRef;
        [SerializeField] public Image Icon;

        [Header("Properties")]
        private Beatmap.Entity entity;
        public float length;
        private bool eligibleToMove = false;
        private bool lastVisible;
        public bool selected;
        public bool mouseHovering;

        [Header("Colors")]
        public Color NormalCol;

        private void Update()
        {
            entity = GameManager.instance.Beatmap.entities.Find(a => a.eventObj == this);

            mouseHovering = RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition, Camera.main);

            #region Optimizations

            bool visible = GetComponent<RectTransform>().IsVisibleFrom(Camera.main);

            if (visible != lastVisible)
            {
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    this.transform.GetChild(i).gameObject.SetActive(visible);
                }
            }

            lastVisible = visible;

            #endregion

            SetColor(GetTrack());

            if (selected)
            {
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    Selections.instance.Deselect(this);
                    Timeline.instance.DestroyEventObject(entity);
                }
            }

            if (Conductor.instance.NotStopped())
            {
                Cancel();
                return;
            }


            if (Input.GetMouseButtonDown(0) && Timeline.instance.IsMouseAboveEvents())
            {
                if (selected)
                {
                    Vector3 mousePos;
                    mousePos = Input.mousePosition;
                    mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                    startPosX = mousePos.x - this.transform.position.x;
                    startPosY = mousePos.y - this.transform.position.y;

                    isDragging = true;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!mouseHovering && !isDragging && !BoxSelection.instance.selecting)
                {
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        Selections.instance.Deselect(this);
                    }
                }

                OnUp();
            }

            if (isDragging && selected)
            {
                Vector3 mousePos;
                mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                this.transform.position = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY - 0.40f, 0);
                this.transform.localPosition = new Vector3(Mathf.Clamp(Mathp.Round2Nearest(this.transform.localPosition.x, 0.25f), 0, Mathf.Infinity), Timeline.instance.SnapToLayer(this.transform.localPosition.y));

                if (lastPos != transform.localPosition)
                    OnMove();

                lastPos = this.transform.localPosition;
            }

        }

        private void OnMove()
        {
            if (GameManager.instance.Beatmap.entities.FindAll(c => c.beat == this.transform.localPosition.x && c.track == GetTrack()).Count > 0)
            {
                eligibleToMove = false;
            }
            else
            {
                eligibleToMove = true;
            }
        }

        private void OnComplete()
        {
            entity.beat = this.transform.localPosition.x;
            GameManager.instance.SortEventsList();
            entity.track = GetTrack();
        }

        #region ClickEvents

        public void OnDown()
        {
            if (!selected)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Selections.instance.ShiftClickSelect(this);
                }
                else
                {
                    Selections.instance.ClickSelect(this);
                }

                // Selector.instance.Click(this);
            }
        }

        public void OnUp()
        {
            if (selected)
            {
                isDragging = false;

                if (eligibleToMove)
                {
                    OnComplete();
                }

                Cancel();
            }
        }

        private void Cancel()
        {
            eligibleToMove = false;
        }

        #endregion

        #region Selection

        public void Select()
        {
            selected = true;
        }

        public void DeSelect()
        {
            selected = false;
        }

        #endregion

        #region Extra

        public void SetColor(int type)
        {
            Color c = Color.white;
            switch (type)
            {
                case 0:
                    c = EditorTheme.theme.properties.Layer1Col.Hex2RGB();
                    break;
                case 1:
                    c = EditorTheme.theme.properties.Layer2Col.Hex2RGB();
                    break;
                case 2:
                    c = EditorTheme.theme.properties.Layer3Col.Hex2RGB();
                    break;
                case 3:
                    c = EditorTheme.theme.properties.Layer4Col.Hex2RGB();
                    break;
            }

            // c = new Color(c.r, c.g, c.b, 0.85f);
            transform.GetChild(0).GetComponent<Image>().color = c;
        }

        public int GetTrack()
        {
            return (int)(this.transform.localPosition.y / Timeline.instance.LayerHeight()) * -1;
        }

        private void OnDestroy()
        {
            // better safety net than canada's healthcare system
            // GameManager.instance.Beatmap.entities.Remove(GameManager.instance.Beatmap.entities.Find(c => c.eventObj = this));
        }

        #endregion
    }
}