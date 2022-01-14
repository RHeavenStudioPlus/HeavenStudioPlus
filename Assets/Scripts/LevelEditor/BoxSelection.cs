using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Editor
{
    public class BoxSelection : MonoBehaviour
    {
        [SerializeField] private RectTransform boxVisual;
        private Rect selectionBox;

        private Vector2 startPosition = Vector2.zero;
        private Vector2 endPosition = Vector2.zero;

        public bool selecting = false;

        public static BoxSelection instance { get; private set; }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            DrawVisual();
        }

        private void Update()
        {
            if (Selections.instance.eventsSelected.Count > 0 && Timeline.instance.IsEventsDragging())
            {
                return;
            }

            // click
            if (Input.GetMouseButtonDown(0))
            {
                startPosition = Input.mousePosition;
                selectionBox = new Rect();
            }

            // dragging
            if (Input.GetMouseButton(0))
            {
                endPosition = Input.mousePosition;
                DrawVisual();
                DrawSelection();
            }

            // release click
            if (Input.GetMouseButtonUp(0))
            {

                startPosition = Vector2.zero;
                endPosition = Vector2.zero;
                DrawVisual();
                SelectEvents();
            }

            // selecting = (selectionBox.size != Vector2.zero); -- doesn't work really

            // for real time selection just move SelectEvents() to here, but that breaks some shit. might fix soon idk --pelly
        }

        private void DrawVisual()
        {
            Vector2 boxStart = startPosition;
            Vector2 boxEnd = endPosition;

            Vector2 boxCenter = (boxStart + boxEnd) / 2;
            boxVisual.position = boxCenter;

            Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));

            boxVisual.sizeDelta = boxSize;
        }

        private void DrawSelection()
        {
            // X
            if (Input.mousePosition.x < startPosition.x)
            {
                // dragging left
                selectionBox.xMin = Input.mousePosition.x;
                selectionBox.xMax = startPosition.x;
            }
            else
            {
                // dragging right
                selectionBox.xMin = startPosition.x;
                selectionBox.xMax = Input.mousePosition.x;
            }

            // Y
            if (Input.mousePosition.y < startPosition.y)
            {
                // dragging down
                selectionBox.yMin = Input.mousePosition.y;
                selectionBox.yMax = startPosition.y;
            }
            else
            {
                // dragging up
                selectionBox.yMin = startPosition.y;
                selectionBox.yMax = Input.mousePosition.y;
            }
        }

        private void SelectEvents()
        {
            int selected = 0;

            for (int i = 0; i < GameManager.instance.Beatmap.entities.Count; i++)
            {
                TimelineEventObj e = GameManager.instance.Beatmap.entities[i].eventObj;
                if (selectionBox.Contains(Camera.main.WorldToScreenPoint(e.transform.position)))
                {
                    Selections.instance.DragSelect(e);
                    selected++;
                }

                // I'm trying this fix this dammit
                /*if (selectionBox.Overlaps(RectTransformToScreenSpace(e.GetComponent<RectTransform>())))
                {
                    print(RectTransformToScreenSpace(e.GetComponent<RectTransform>()));
                    print(selectionBox);
                    Selections.instance.DragSelect(e);
                    selected++;
                }*/
            }

            selecting = selected > 0;
        }

        public Rect RectTransformToScreenSpace(RectTransform transform)
        {
            Vector2 sizeTemp = Vector2.Scale(transform.rect.size, transform.localScale);
            Vector2 size = new Vector2(sizeTemp.x * 100, sizeTemp.y);
            return new Rect((Vector2)Camera.main.WorldToScreenPoint(transform.position) - (size * 0.5f), size);
        }
    }
}