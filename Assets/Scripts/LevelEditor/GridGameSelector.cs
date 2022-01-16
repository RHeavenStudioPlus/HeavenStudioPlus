using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using DG.Tweening;
using Starpelly;

namespace RhythmHeavenMania.Editor
{
    public class GridGameSelector : MonoBehaviour
    {
        public string SelectedMinigame;

        [Header("Components")]
        public GameObject GameEventSelector;
        public GameObject EventRef;
        public GameObject CurrentSelected;

        [Header("Properties")]
        private EventCaller.MiniGame mg;
        private bool gameOpen;
        [SerializeField] private int currentEventIndex;
        private int dragTimes;
        public float posDif;
        public int ignoreSelectCount;

        private void Update()
        {
            if (gameOpen)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    UpdateIndex(currentEventIndex + 1);
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    UpdateIndex(currentEventIndex - 1);
                }
            }

            if (Input.mouseScrollDelta.y != 0)
            {
                UpdateIndex(currentEventIndex - Mathf.RoundToInt(Input.mouseScrollDelta.y));
            }
        }

        #region Events

        public void Drag()
        {
            if (Conductor.instance.NotStopped()) return;

            if (Timeline.instance.CheckIfMouseInTimeline() && dragTimes < 1)
            {
                dragTimes++;
                if (currentEventIndex == 0)
                {
                    Timeline.instance.AddEventObject($"gameManager/switchGame/{mg.name}", true, new Vector3(0, 0));
                }
                else
                {
                    Timeline.instance.AddEventObject(mg.name + "/" + mg.actions[currentEventIndex - 1].actionName, true, new Vector3(0, 0));
                }
            }
        }

        public void Drop()
        {
            if (Conductor.instance.NotStopped()) return;

            dragTimes = 0;
        }

        #endregion

        #region Functions

        public void UpdateIndex(int amount)
        {
            currentEventIndex = amount;

            EventRef.transform.parent.parent.DOKill();
            CurrentSelected.transform.DOKill();

            if (currentEventIndex < 0)
            {
                currentEventIndex = EventRef.transform.parent.childCount - (ignoreSelectCount + 1);
            }
            else if (currentEventIndex > EventRef.transform.parent.childCount - (ignoreSelectCount + 1))
            {
                currentEventIndex = 0;
            }

            if (currentEventIndex > 2)
            {
                if (EventRef.transform.parent.childCount - ignoreSelectCount - 4 > currentEventIndex)
                {
                    EventRef.transform.parent.parent.DOLocalMoveY((EventRef.GetComponent<RectTransform>().sizeDelta.y + 5) * (currentEventIndex - 2), 0.35f).SetEase(Ease.OutExpo);
                }
                else
                {
                    EventRef.transform.parent.parent.DOLocalMoveY((EventRef.GetComponent<RectTransform>().sizeDelta.y + 5) * (EventRef.transform.parent.childCount - 9), 0.35f).SetEase(Ease.OutExpo);
                }
            }
            else
            {
                EventRef.transform.parent.parent.transform.DOLocalMoveY(0, 0.35f).SetEase(Ease.OutExpo);
            }

            for (int i = ignoreSelectCount; i < EventRef.transform.parent.childCount; i++)
            {
                float easeTime = 0.35f;
                Ease ease = Ease.OutCirc;
                int curIndex = currentEventIndex + ignoreSelectCount;

                EventRef.transform.parent.GetChild(i).DOKill();

                EventRef.transform.parent.GetChild(i).localPosition = new Vector3(EventRef.transform.parent.GetChild(i).localPosition.x, EventRef.transform.localPosition.y - ((i - ignoreSelectCount) * EventRef.GetComponent<RectTransform>().sizeDelta.y));

                if (i < curIndex)
                {
                    EventRef.transform.parent.GetChild(i).transform.DOLocalMove(new Vector3
                    (EventRef.transform.localPosition.x + (posDif),
                    EventRef.transform.parent.GetChild(i).transform.localPosition.y), easeTime).SetEase(ease);
                }
                else if (i > curIndex)
                {
                    EventRef.transform.parent.GetChild(i).transform.DOLocalMove(new Vector3
                    (EventRef.transform.localPosition.x + (posDif),
                    EventRef.transform.parent.GetChild(i).transform.localPosition.y), easeTime).SetEase(ease);
                }
                else if (i == curIndex)
                {
                    EventRef.transform.parent.GetChild(i).transform.DOLocalMove(new Vector3
                    (EventRef.transform.localPosition.x,
                    EventRef.transform.parent.GetChild(i).transform.localPosition.y), easeTime).SetEase(ease);
                }
            }

            CurrentSelected.transform.DOLocalMoveY(EventRef.transform.parent.GetChild(currentEventIndex + ignoreSelectCount).transform.localPosition.y, 0.35f).SetEase(Ease.OutExpo);

            SetColor(currentEventIndex);
        }

        public void SelectGame(string gameName, int index)
        {
            DestroyEvents();

            transform.GetChild(index).GetChild(0).gameObject.SetActive(true);

            SelectedMinigame = gameName;

            mg = EventCaller.instance.minigames.Find(c => c.displayName == gameName);

            AddEvents();

            gameOpen = true;

            currentEventIndex = 0;

            UpdateIndex(0);
        }

        private void AddEvents()
        {
            if (mg.name != "gameManager")
            {
                GameObject sg = Instantiate(EventRef, EventRef.transform.parent);
                sg.GetComponent<TMP_Text>().text = "switchGame";
                sg.SetActive(true);
            }

            for (int i = 0; i < mg.actions.Count; i++)
            {
                if (mg.actions[i].actionName != "switchGame")
                {
                    GameObject e = Instantiate(EventRef, EventRef.transform.parent);
                    e.GetComponent<TMP_Text>().text = mg.actions[i].actionName;
                    e.SetActive(true);
                }
            }
        }

        private void SetColor(int ind)
        {
            for (int i = ignoreSelectCount; i < EventRef.transform.parent.childCount; i++)
            {
                EventRef.transform.parent.GetChild(i).GetComponent<TMP_Text>().color = EditorTheme.theme.properties.EventNormalCol.Hex2RGB();
            }

            EventRef.transform.parent.GetChild(ind + ignoreSelectCount).GetComponent<TMP_Text>().color = EditorTheme.theme.properties.EventSelectedCol.Hex2RGB();
            CurrentSelected.GetComponent<Image>().color = EditorTheme.theme.properties.EventSelectedCol.Hex2RGB();
        }

        private void DestroyEvents()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }

            for (int i = ignoreSelectCount; i < EventRef.transform.parent.childCount; i++)
            {
                Destroy(EventRef.transform.parent.GetChild(i).gameObject);
            }

            gameOpen = false;
        }

        #endregion
    }
}