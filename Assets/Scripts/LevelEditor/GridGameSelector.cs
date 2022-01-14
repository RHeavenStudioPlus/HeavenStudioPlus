using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using DG.Tweening;

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
                Timeline.instance.AddEventObject(mg.name + "/" + mg.actions[currentEventIndex].actionName, true, new Vector3(0, 0));
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
                currentEventIndex = mg.actions.Count - 1;
            }
            else if (currentEventIndex > mg.actions.Count - 1)
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

            for (int i = 0; i < mg.actions.Count; i++)
            {
                GameObject e = Instantiate(EventRef, EventRef.transform.parent);
                e.GetComponent<TMP_Text>().text = mg.actions[i].actionName;
                e.SetActive(true);
            }

            gameOpen = true;
            currentEventIndex = 0;
            SetColor(0);
        }

        private void SetColor(int ind)
        {
            for (int i = 0; i < EventRef.transform.parent.childCount; i++)
            {
                EventRef.transform.parent.GetChild(i).GetComponent<TMP_Text>().color = Color.white;
            }

            EventRef.transform.parent.GetChild(ind + 1).GetComponent<TMP_Text>().color = Color.cyan;
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