using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Starpelly;

namespace RhythmHeavenMania.Editor.Track
{
    public class TempoTimeline : MonoBehaviour
    {
        [Header("Components")]
        private RectTransform rectTransform;
        [SerializeField] private RectTransform RefTempoChange;

        private void Start()
        {
            rectTransform = this.GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    AddTempoChange();
                }
            }
        }

        private void AddTempoChange()
        {
            GameObject tempoChange = Instantiate(RefTempoChange.gameObject, this.transform);

            tempoChange.transform.GetChild(0).GetComponent<Image>().color = EditorTheme.theme.properties.TempoLayerCol.Hex2RGB();
            tempoChange.transform.GetChild(1).GetComponent<Image>().color = EditorTheme.theme.properties.TempoLayerCol.Hex2RGB();
            tempoChange.transform.GetChild(2).GetComponent<TMP_Text>().color = EditorTheme.theme.properties.TempoLayerCol.Hex2RGB();

            tempoChange.SetActive(true);
            tempoChange.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, tempoChange.transform.position.y);
            tempoChange.transform.localPosition = new Vector3(Starpelly.Mathp.Round2Nearest(tempoChange.transform.localPosition.x, 0.25f), tempoChange.transform.localPosition.y);

            TempoTimelineObj tempoTimelineObj = tempoChange.AddComponent<TempoTimelineObj>();

            Beatmap.TempoChange tempoC = new Beatmap.TempoChange();
            tempoC.beat = tempoChange.transform.localPosition.x;
            tempoC.tempo = GameManager.instance.Beatmap.bpm;

            tempoTimelineObj.tempoChange = tempoC;
            GameManager.instance.Beatmap.tempoChanges.Add(tempoC);
        }
    }

}