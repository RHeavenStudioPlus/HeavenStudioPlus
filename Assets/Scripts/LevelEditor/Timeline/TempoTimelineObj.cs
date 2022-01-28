using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace RhythmHeavenMania.Editor.Track
{
    public class TempoTimelineObj : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private TMP_Text tempoTXT;

        public Beatmap.TempoChange tempoChange;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            tempoTXT = transform.GetChild(2).GetComponent<TMP_Text>();
            UpdateTempo();
        }

        private void Update()
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main))
            {
                float newTempo = Input.mouseScrollDelta.y;

                if (Input.GetKey(KeyCode.LeftShift))
                    newTempo *= 5f;
                if (Input.GetKey(KeyCode.LeftControl))
                    newTempo /= 100f;

                tempoChange.tempo += newTempo;
            }
            UpdateTempo();
        }

        private void UpdateTempo()
        {
            tempoTXT.text = $"{tempoChange.tempo} BPM";
        }
    }
}