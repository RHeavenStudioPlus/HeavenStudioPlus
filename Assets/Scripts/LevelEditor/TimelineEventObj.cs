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
        private int enemyIndex;
        public float length;
        private bool eligibleToMove = false;
        private bool lastVisible;

        [Header("Colors")]
        public Color NormalCol;
        public Color SelectedCol;
        public Color DeleteCol;

        private void Update()
        {
            // Optimizations

            bool visible = GetComponent<RectTransform>().IsVisibleFrom(Camera.main);

            if (visible != lastVisible)
            {
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    this.transform.GetChild(i).gameObject.SetActive(visible);
                }
            }

            lastVisible = visible;

            // -------------

            if (Conductor.instance.NotStopped())
            {
                Cancel();
                return;
            }

            enemyIndex = GameManager.instance.Beatmap.entities.FindIndex(a => a.eventObj == this);

            if (isDragging == true)
            {
                Vector3 mousePos;
                mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                this.transform.position = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY - 0.40f, 0);
                this.transform.localPosition = new Vector3(Mathf.Clamp(Mathp.Round2Nearest(this.transform.localPosition.x, 0.25f), 0, Mathf.Infinity), Mathf.Clamp(Mathp.Round2Nearest(this.transform.localPosition.y, 51.34f), -51.34f * 3, 0));

                if (lastPos != transform.localPosition)
                    OnMove();

                lastPos = this.transform.localPosition;

                SetColor(1);
            }
            else
            {
                SetColor(0);
            }

            if (Input.GetMouseButtonUp(0))
                OnUp();

            if (Input.GetKeyDown(KeyCode.Delete))
                Timeline.instance.DestroyEventObject(this);
        }

        private void OnMove()
        {
            if (GameManager.instance.Beatmap.entities.FindAll(c => c.beat == this.transform.localPosition.x && c.track == (int)(this.transform.localPosition.y / 51.34f * -1)).Count > 0)
            {
                // PosPreview.GetComponent<Image>().color = Color.red;
                eligibleToMove = false;
            }
            else
            {
                // PosPreview.GetComponent<Image>().color = Color.yellow;
                eligibleToMove = true;
            }
        }

        private void OnComplete()
        {
            var entity = GameManager.instance.Beatmap.entities[enemyIndex];
            entity.beat = this.transform.localPosition.x;
            GameManager.instance.SortEventsList();
            entity.track = (int)(this.transform.localPosition.y / 51.34f) * -1;

            // this.transform.localPosition = this.transform.localPosition;
            // transform.DOLocalMove(PosPreview.transform.localPosition, 0.15f).SetEase(Ease.OutExpo);
        }

        private void Cancel()
        {
            if (PosPreview) Destroy(PosPreview.gameObject);
            eligibleToMove = false;
        }

        public void OnDown()
        {
            Vector3 mousePos;

            /*PosPreview = Instantiate(PosPreviewRef, PosPreviewRef.transform.parent);
            PosPreview.sizeDelta = new Vector2(100 * transform.GetComponent<RectTransform>().sizeDelta.x, transform.GetComponent<RectTransform>().sizeDelta.y);
            PosPreview.transform.localPosition = this.transform.localPosition;
            PosPreview.GetComponent<Image>().enabled = true;
            PosPreview.GetComponent<Image>().color = Color.yellow;*/

            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            startPosX = mousePos.x - this.transform.position.x;
            startPosY = mousePos.y - this.transform.position.y;
            isDragging = true;
        }

        public void OnUp()
        {
            isDragging = false;

            if (eligibleToMove) OnComplete();
            Cancel();
        }

        public void SetColor(int type)
        {
            Color c = Color.white;
            switch (type)
            {
                case 0:
                    c = NormalCol;
                    break;
                case 1:
                    c = SelectedCol;
                    break;
                case 2:
                    c = DeleteCol;
                    break;
            }

            transform.GetChild(0).GetComponent<Image>().color = c;
        }
    }
}