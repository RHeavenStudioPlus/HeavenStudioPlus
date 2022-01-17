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
        private RectTransform eventsParent;

        [Header("Properties")]
        private EventCaller.MiniGame mg;
        private bool gameOpen;
        [SerializeField] private int currentEventIndex;
        private int dragTimes;
        public float posDif;
        public int ignoreSelectCount;

        private void Start()
        {
            eventsParent = EventRef.transform.parent.GetChild(2).GetComponent<RectTransform>();
            SelectGame("Game Manager", 1);

            SetColors();
        }

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

        #region Functions

        public void UpdateIndex(int amount, bool updateCol = true)
        {
            currentEventIndex = amount;

            EventRef.transform.parent.DOKill();
            CurrentSelected.transform.DOKill();

            if (currentEventIndex < 0)
                currentEventIndex = eventsParent.childCount - 1;
            else if (currentEventIndex > eventsParent.childCount - 1)
                currentEventIndex = 0;

            if (currentEventIndex > 2 && eventsParent.childCount >= 8)
            {
                if (eventsParent.childCount - 4 > currentEventIndex)
                {
                    EventRef.transform.parent.DOLocalMoveY((EventRef.GetComponent<RectTransform>().sizeDelta.y) * (currentEventIndex - 2), 0.35f).SetEase(Ease.OutExpo);
                }    
                else
                {
                    EventRef.transform.parent.DOLocalMoveY((EventRef.GetComponent<RectTransform>().sizeDelta.y) * (eventsParent.childCount - 7), 0.35f).SetEase(Ease.OutExpo);
                }
            }
            else
                EventRef.transform.parent.DOLocalMoveY(0, 0.35f).SetEase(Ease.OutExpo);

            CurrentSelected.transform.DOLocalMoveY(eventsParent.transform.GetChild(currentEventIndex).localPosition.y + eventsParent.transform.localPosition.y, 0.35f).SetEase(Ease.OutExpo);

            if (updateCol)
            SetColors(currentEventIndex);
        }

        public void SelectGame(string gameName, int index)
        {
            mg = EventCaller.instance.minigames.Find(c => c.displayName == gameName);
            SelectedMinigame = gameName;
            gameOpen = true;

            DestroyEvents();
            AddEvents();

            transform.GetChild(index).GetChild(0).gameObject.SetActive(true);

            currentEventIndex = 0;
            UpdateIndex(0, false);

            Editor.instance.SetGameEventTitle($"Select game event for {gameName}");
        }

        private void AddEvents()
        {
            if (mg.name != "gameManager")
            {
                GameObject sg = Instantiate(EventRef, eventsParent);
                sg.GetComponent<TMP_Text>().text = "switchGame";
                sg.SetActive(true);
                sg.GetComponent<TMP_Text>().color = EditorTheme.theme.properties.EventSelectedCol.Hex2RGB();
            }

            for (int i = 0; i < mg.actions.Count; i++)
            {
                if (mg.actions[i].actionName == "switchGame") continue;
                GameObject g = Instantiate(EventRef, eventsParent);
                g.GetComponent<TMP_Text>().text = mg.actions[i].actionName;
                g.SetActive(true);
            }
        }

        private void DestroyEvents()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }

            for (int i = 0; i < eventsParent.childCount; i++)
            {
                Destroy(eventsParent.GetChild(i).gameObject);
            }
        }

        private void SetColors(int index = 0)
        {
            CurrentSelected.GetComponent<Image>().color = EditorTheme.theme.properties.EventSelectedCol.Hex2RGB();

            for (int i = 0; i < eventsParent.transform.childCount; i++)
                eventsParent.GetChild(i).GetComponent<TMP_Text>().color = EditorTheme.theme.properties.EventNormalCol.Hex2RGB();

            eventsParent.GetChild(index).GetComponent<TMP_Text>().color = EditorTheme.theme.properties.EventSelectedCol.Hex2RGB();
        }

        #endregion

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
    }
}