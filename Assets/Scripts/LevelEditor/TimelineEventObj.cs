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
        private RectTransform rectTransform;

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
        public bool resizable;
        public bool resizing;

        [Header("Colors")]
        public Color NormalCol;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();

            if (!resizable)
            {
                Destroy(transform.GetChild(5).gameObject);
                Destroy(transform.GetChild(6).gameObject);
            }
        }

        private void Update()
        {
            entity = GameManager.instance.Beatmap.entities.Find(a => a.eventObj == this);

            mouseHovering = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main);

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

                transform.GetChild(3).gameObject.SetActive(true);

                for (int i = 0; i < transform.GetChild(4).childCount; i++)
                {
                    transform.GetChild(4).GetChild(i).GetComponent<Image>().color = Color.cyan;
                }

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Vector3[] v = new Vector3[4];
                rectTransform.GetWorldCorners(v);

                if (mouseHovering)
                {
                    if (mousePos.x > transform.position.x && mousePos.x < transform.position.x + 0.1f)
                    {
                    }
                    else if (mousePos.x > v[3].x - 0.1f && mousePos.x < v[3].x)
                    {

                    }
                }
            }
            else
            {
                transform.GetChild(3).gameObject.SetActive(false);

                for (int i = 0; i < transform.GetChild(4).childCount; i++)
                    transform.GetChild(4).GetChild(i).GetComponent<Image>().color = new Color32(0, 0, 0, 51);
            }

            if (Conductor.instance.NotStopped())
            {
                Cancel();
                return;
            }

            if (!resizing)
            {
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

        }

        #region ResizeEvents

        public void DragEnter()
        {
            if (selected)
                Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/horizontal_resize"), new Vector2(8, 8), CursorMode.Auto);
        }

        public void DragExit()
        {
            if (!resizing)
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        public void OnLeftDown()
        {
            if (selected)
            {
                SetPivot(new Vector2(1, rectTransform.pivot.y));
                resizing = true;
            }
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
        }

        public void OnLeftUp()
        {
            SetPivot(new Vector2(0, rectTransform.pivot.y));
            resizing = false;
            OnComplete();
        }

        public void OnRightDown()
        {
            if (selected)
            {
                SetPivot(new Vector2(0, rectTransform.pivot.y));
                resizing = true;
            }
        }

        public void DragRight()
        {
            if (!resizing) return;
            // if (!mouseHovering) return;

            Vector2 sizeDelta = rectTransform.sizeDelta;

            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, Camera.main, out mousePos);

            sizeDelta = new Vector2(mousePos.x, sizeDelta.y);
            sizeDelta = new Vector2(Mathf.Clamp(sizeDelta.x, 0.25f, Mathf.Infinity), sizeDelta.y);

            rectTransform.sizeDelta = new Vector2(Mathp.Round2Nearest(sizeDelta.x, 0.25f), sizeDelta.y);
        }

        public void OnRightUp()
        {
            resizing = false;

            OnComplete();
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
        }

        private void OnComplete()
        {
            entity.length = rectTransform.sizeDelta.x;
            entity.beat = this.transform.localPosition.x;
            GameManager.instance.SortEventsList();
            entity.track = GetTrack();
        }

        #endregion

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