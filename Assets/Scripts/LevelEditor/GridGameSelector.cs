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

        private void Update()
        {
            if (gameOpen)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    UpdateIndex(1);
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    UpdateIndex(-1);
                }
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
            currentEventIndex += amount;

            if (currentEventIndex < 0)
            {
                currentEventIndex = EventRef.transform.parent.childCount - 2;
            }
            else if (currentEventIndex > EventRef.transform.parent.childCount - 2)
            {
                currentEventIndex = 0;
            }

            if (currentEventIndex > 2)
            {
                EventRef.transform.parent.parent.transform.DOLocalMoveY((EventRef.GetComponent<RectTransform>().sizeDelta.y + 5) * (currentEventIndex - 2), 0.35f).SetEase(Ease.OutExpo);
            }
            else
            {
                EventRef.transform.parent.parent.transform.DOLocalMoveY(0, 0.35f).SetEase(Ease.OutExpo);
            }

            CurrentSelected.transform.DOLocalMoveY(EventRef.transform.parent.GetChild(currentEventIndex + 1).transform.localPosition.y, 0.35f).SetEase(Ease.OutExpo);

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
            SetColor(0);
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
            for (int i = 0; i < EventRef.transform.parent.childCount; i++)
            {
                EventRef.transform.parent.GetChild(i).GetComponent<TMP_Text>().color = EditorTheme.theme.properties.EventNormalCol.Hex2RGB();
            }

            EventRef.transform.parent.GetChild(ind + 1).GetComponent<TMP_Text>().color = EditorTheme.theme.properties.EventSelectedCol.Hex2RGB();
            CurrentSelected.GetComponent<Image>().color = EditorTheme.theme.properties.EventSelectedCol.Hex2RGB();
        }

        private void DestroyEvents()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }

            for (int i = 1; i < EventRef.transform.parent.childCount; i++)
            {
                Destroy(EventRef.transform.parent.GetChild(i).gameObject);
            }

            gameOpen = false;
        }

        #endregion
    }
}