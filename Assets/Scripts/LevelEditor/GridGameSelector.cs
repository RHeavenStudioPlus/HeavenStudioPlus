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

            Editor.instance.SetGameEventTitle($"Select game event for {gameName}");
        }

        private void AddEvents()
        {

        }

        private void DestroyEvents()
        {

        }

        #endregion
    }
}