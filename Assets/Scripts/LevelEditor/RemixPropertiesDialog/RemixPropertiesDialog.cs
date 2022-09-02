using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Editor.Track;

using TMPro;

namespace HeavenStudio.Editor 
{
    public class RemixPropertiesDialog : Dialog
    {
        [Header("Editable Properties")]
        [SerializeField] string[] infoTags;
        [SerializeField] string[] infoLabels;
        [SerializeField] string[] flavourTags;
        [SerializeField] string[] flavourLabels;

        [Header("Containers")]
        [SerializeField] ChartInfoProperties infoContainer;
        // [SerializeField] ChartFlavourProperties flavourContainer;

        public DynamicBeatmap chart;

        private void Start() {}

        public void SwitchPropertiesDialog()
        {
            if(dialog.activeSelf) {
                Editor.instance.canSelect = true;
                Editor.instance.inAuthorativeMenu = false;
                dialog.SetActive(false);

                CleanDialog();
            } else {
                ResetAllDialogs();
                Editor.instance.canSelect = false;
                Editor.instance.inAuthorativeMenu = true;
                dialog.SetActive(true);

                SetupDialog();
            }
        }

        private void SetupDialog() {
            chart = GameManager.instance.Beatmap;
            string[] tags = infoTags;
            string[] labels = infoLabels;
            int i = 0;

            foreach (string property in tags) {
                if (chart.properties.ContainsKey(property)) {
                    infoContainer.AddParam(this, property, chart.properties[property], labels[i]);
                }
                else
                {
                    if (property == "divider")
                    {
                        //TODO: prefab that's just a dividing line
                    }
                    else
                    {
                        Debug.LogWarning("Property Menu generation Warning: Property " + property + " not found");
                    }
                }
                i++;
            }
        }

        private void CleanDialog() {
            foreach (Transform child in dialog.transform) {
                Destroy(child.gameObject);
            }
        }

        private void Update() {}
    }
}