using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using TMPro;
using Starpelly;

namespace HeavenStudio.Editor.Track
{
    public class VolumeTimeline : MonoBehaviour
    {
        [Header("Components")]
        private RectTransform rectTransform;
        public TMP_InputField StartingVolume;
        private RectTransform StartingVolumeRect;

        private bool firstUpdate;

        void Start()
        {
            rectTransform = this.GetComponent<RectTransform>();
            StartingVolumeRect = StartingVolume.GetComponent<RectTransform>();
        }

        void Update()
        {
            if (!firstUpdate)
            {
                UpdateStartingVolumeText();
                firstUpdate = true;
            }

            if (Timeline.instance.userIsEditingInputField)
                return;

            if (Timeline.instance.timelineState.musicVolume && !Conductor.instance.NotStopped())
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(StartingVolumeRect, Input.mousePosition, Editor.instance.EditorCamera))
                {
                    int increase = Mathf.RoundToInt(Input.mouseScrollDelta.y);
                    if (Input.GetKey(KeyCode.LeftShift))
                        increase *= 5;

                    if (increase != 0)
                    {
                        GameManager.instance.Beatmap.musicVolume += increase;
                        UpdateStartingVolumeText();
                        UpdateStartingVolumeFromText(); // In case the scrolled-to value is invalid.
                    }
                }
            }
        }

        public void UpdateStartingVolumeText()
        {
            StartingVolume.text = GameManager.instance.Beatmap.musicVolume.ToString();
        }

        public void UpdateStartingVolumeFromText()
        {
            // Failsafe against empty string.
            if (String.IsNullOrEmpty(StartingVolume.text))
                StartingVolume.text = "100";
            
            var newVol = Convert.ToInt32(StartingVolume.text);

            // Failsafe against invalid volume.
            if (newVol > 100)
            {
                StartingVolume.text = "100";
                newVol = 100;
            }
            else if (newVol < 0)
            {
                StartingVolume.text = "1";
                newVol = 1;
            }

            GameManager.instance.Beatmap.musicVolume = newVol;

            // In case the newVol ended up differing from the inputted string.
            UpdateStartingVolumeText();
        }
    }
}
