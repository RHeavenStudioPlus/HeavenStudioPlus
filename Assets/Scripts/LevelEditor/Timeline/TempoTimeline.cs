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
        [SerializeField] private RectTransform StartingBPM;

        public List<TempoTimelineObj> tempoTimelineObjs = new List<TempoTimelineObj>();

        private void Start()
        {
            rectTransform = this.GetComponent<RectTransform>();

            for (int i = 0; i < GameManager.instance.Beatmap.tempoChanges.Count; i++)
            {
                Beatmap.TempoChange tempoChange = GameManager.instance.Beatmap.tempoChanges[i];
                AddTempoChange(false, tempoChange);
            }
        }

        private void Update()
        {
            StartingBPM.GetChild(0).GetComponent<TMP_Text>().text = GameManager.instance.Beatmap.bpm.ToString();

            if (Timeline.instance.timelineState.tempoChange && !Conductor.instance.NotStopped())
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Editor.instance.EditorCamera))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (tempoTimelineObjs.FindAll(c => c.hovering == true).Count == 0)
                        {
                            AddTempoChange(true);
                        }
                    }
                }

                if (RectTransformUtility.RectangleContainsScreenPoint(StartingBPM, Input.mousePosition, Editor.instance.EditorCamera))
                {
                    float increase = Input.mouseScrollDelta.y;
                    if (Input.GetKey(KeyCode.LeftControl))
                        increase /= 100f;
                    if (Input.GetKey(KeyCode.LeftShift))
                        increase *= 5f;

                    GameManager.instance.Beatmap.bpm += increase;
                    StartingBPM.transform.GetChild(0).GetComponent<TMP_Text>().text = GameManager.instance.Beatmap.bpm.ToString();
                }

                StartingBPM.GetComponent<Image>().enabled = true;
                StartingBPM.GetComponent<Button>().enabled = true;
            }
            else
            {
                StartingBPM.GetComponent<Image>().enabled = false;
                StartingBPM.GetComponent<Button>().enabled = false;
                StartingBPM.GetComponent<Button>().targetGraphic.color = Color.white;
            }
        }

        private void AddTempoChange(bool create, Beatmap.TempoChange tempoChange_ = null)
        {
            GameObject tempoChange = Instantiate(RefTempoChange.gameObject, this.transform);

            tempoChange.transform.GetChild(0).GetComponent<Image>().color = EditorTheme.theme.properties.TempoLayerCol.Hex2RGB();
            tempoChange.transform.GetChild(1).GetComponent<Image>().color = EditorTheme.theme.properties.TempoLayerCol.Hex2RGB();
            tempoChange.transform.GetChild(2).GetComponent<TMP_Text>().color = EditorTheme.theme.properties.TempoLayerCol.Hex2RGB();

            tempoChange.SetActive(true);

            TempoTimelineObj tempoTimelineObj = tempoChange.GetComponent<TempoTimelineObj>();

            if (create == true)
            {
                tempoChange.transform.position = new Vector3(Editor.instance.EditorCamera.ScreenToWorldPoint(Input.mousePosition).x + 0.08f, tempoChange.transform.position.y);
                tempoChange.transform.localPosition = new Vector3(Starpelly.Mathp.Round2Nearest(tempoChange.transform.localPosition.x, 0.25f), tempoChange.transform.localPosition.y);

                Beatmap.TempoChange tempoC = new Beatmap.TempoChange();
                tempoC.beat = tempoChange.transform.localPosition.x;
                tempoC.tempo = GameManager.instance.Beatmap.bpm;

                tempoTimelineObj.tempoChange = tempoC;
                GameManager.instance.Beatmap.tempoChanges.Add(tempoC);
            }
            else
            {
                tempoChange.transform.localPosition = new Vector3(tempoChange_.beat, tempoChange.transform.localPosition.y);

                tempoTimelineObj.tempoChange = tempoChange_;
            }

            tempoTimelineObjs.Add(tempoTimelineObj);
        }
    }

}