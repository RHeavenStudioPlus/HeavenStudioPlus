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

        private Vector3 lastPos;
        private RectTransform rectTransform;

        [Header("Components")]
        [SerializeField] private RectTransform PosPreview;
        [SerializeField] private RectTransform PosPreviewRef;
        [SerializeField] public Image Icon;
        [SerializeField] private Image selectedImage;
        [SerializeField] private RectTransform outline;
        [SerializeField] private RectTransform resizeGraphic;

        [Header("Properties")]
        private Beatmap.Entity entity;
        public float length;
        private bool eligibleToMove = false;
        private bool lastVisible;
        public bool selected;
        public bool mouseHovering;
        public bool resizable;
        public bool resizing;
        public bool moving;

        [Header("Colors")]
        public Color NormalCol;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();

            if (!resizable)
            {
                Destroy(resizeGraphic.gameObject);
            }
        }

        private void Update()
        {
            selected = Selections.instance.eventsSelected.Contains(this);
            entity = GameManager.instance.Beatmap.entities.Find(a => a.eventObj == this);

            mouseHovering = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main);

            #region Optimizations

            // problem with long objects but im lazy right now
            bool visible = rectTransform.IsVisibleFrom(Camera.main);

            if (visible != lastVisible)
            {
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    // this.transform.GetChild(i).gameObject.SetActive(visible);
                }
            }

            lastVisible = visible;

            #endregion

            SetColor(GetTrack());

            if (Conductor.instance.NotStopped())
            {
                Cancel();
                return;
            }

            if (selected)
            {
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    Selections.instance.Deselect(this);
                    Timeline.instance.DestroyEventObject(entity);
                }

                selectedImage.gameObject.SetActive(true);
                for (int i = 0; i < outline.childCount; i++)
                {
                    outline.GetChild(i).GetComponent<Image>().color = Color.cyan;
                }
            }
            else
            {
                selectedImage.gameObject.SetActive(false);

                for (int i = 0; i < outline.childCount; i++)
                    outline.GetChild(i).GetComponent<Image>().color = new Color32(0, 0, 0, 51);
            }

            if (!resizing)
            {
                if (Input.GetMouseButtonUp(0) && Timeline.instance.CheckIfMouseInTimeline())
                {
                    if (!mouseHovering && !moving && !BoxSelection.instance.selecting)
                    {
                        if (!Input.GetKey(KeyCode.LeftShift))
                        {
                            Selections.instance.Deselect(this);
                        }
                    }

                    OnUp();
                }
                if (moving && selected)
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    this.transform.position = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY - 0.40f, 0);
                    this.transform.localPosition = new Vector3(Mathf.Clamp(Mathp.Round2Nearest(this.transform.localPosition.x, 0.25f), 0, Mathf.Infinity), Timeline.instance.SnapToLayer(this.transform.localPosition.y));

                    if (lastPos != transform.localPosition)
                        OnMove();

                    lastPos = this.transform.localPosition;
                }
            }

        }

        #region ClickEvents

        public void OnClick()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Selections.instance.ShiftClickSelect(this);
            }
            else
            {
                Selections.instance.ClickSelect(this);
            }
        }

        public void OnDown()
        {
            if (!selected) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPosX = mousePos.x - this.transform.position.x;
            startPosY = mousePos.y - this.transform.position.y;

            moving = true;

            OnComplete();
        }

        public void OnUp()
        {
            if (selected)
            {
                moving = false;

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

        #region ResizeEvents

        public void DragEnter()
        {
            Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/horizontal_resize"), new Vector2(8, 8), CursorMode.Auto);
        }

        public void DragExit()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        public void OnLeftDown()
        {
            SetPivot(new Vector2(1, rectTransform.pivot.y));
            resizing = true;
        }

        public void DragLeft()
        {
            if (!resizing) return;

            Vector2 sizeDelta = rectTransform.sizeDelta;

            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, Camera.main, out mousePos);

            sizeDelta = new Vector2(-mousePos.x + 0.1f, sizeDelta.y);
            sizeDelta = new Vector2(Mathf.Clamp(sizeDelta.x, 0.25f, rectTransform.localPosition.x), sizeDelta.y);

            rectTransform.sizeDelta = new Vector2(Mathp.Round2Nearest(sizeDelta.x, 0.25f), sizeDelta.y);

            OnComplete();
        }

        public void OnLeftUp()
        {
            SetPivot(new Vector2(0, rectTransform.pivot.y));
            resizing = false;
        }

        public void OnRightDown()
        {
            SetPivot(new Vector2(0, rectTransform.pivot.y));
            resizing = true;
        }

        public void DragRight()
        {
            if (!resizing) return;

            Vector2 sizeDelta = rectTransform.sizeDelta;

            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, Camera.main, out mousePos);

            sizeDelta = new Vector2(mousePos.x, sizeDelta.y);
            sizeDelta = new Vector2(Mathf.Clamp(sizeDelta.x, 0.25f, Mathf.Infinity), sizeDelta.y);

            rectTransform.sizeDelta = new Vector2(Mathp.Round2Nearest(sizeDelta.x, 0.25f), sizeDelta.y);

            OnComplete();
        }

        public void OnRightUp()
        {
            resizing = false;
        }

        private void SetPivot(Vector2 pivot)
        {
            if (rectTransform == null) return;

            Vector2 size = rectTransform.rect.size;
            Vector2 deltaPivot = rectTransform.pivot - pivot;
            Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }

        #endregion

        #region OnEvents

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

            OnComplete();
        }

        private void OnComplete()
        {
            entity.length = rectTransform.sizeDelta.x;
            entity.beat = this.transform.localPosition.x;
            GameManager.instance.SortEventsList();
            entity.track = GetTrack();
        }

        #endregion

        #region Selection

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

            if (resizable)
            {
                c = new Color(0, 0, 0, 0.35f);
                transform.GetChild(1).GetChild(0).GetComponent<Image>().color = c;
                transform.GetChild(1).GetChild(1).GetComponent<Image>().color = c;
                transform.GetChild(1).GetChild(2).GetComponent<Image>().color = c;
            }
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