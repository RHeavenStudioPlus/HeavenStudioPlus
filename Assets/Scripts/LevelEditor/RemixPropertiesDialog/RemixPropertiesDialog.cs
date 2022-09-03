using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Editor.Track;

using TMPro;

namespace HeavenStudio.Editor 
{
    public class RemixPropertiesDialog : Dialog
    {
        [Header("General References")]
        [SerializeField] TabsManager tabsManager;

        [Header("Editable Properties")]
        [SerializeField] PropertyTag[] infoTags;
        [SerializeField] PropertyTag[] flavourTags;

        [Header("Containers")]
        [SerializeField] ChartInfoProperties infoContainer;
        // [SerializeField] ChartFlavourProperties flavourContainer;

        public DynamicBeatmap chart;

        private void Start() { }

        public void SwitchPropertiesDialog()
        {
            if (dialog.activeSelf)
            {
                tabsManager.CloseContent();
                Editor.instance.canSelect = true;
                Editor.instance.inAuthorativeMenu = false;
                dialog.SetActive(false);
            }
            else
            {
                ResetAllDialogs();

                infoContainer.Init(this);
                //flavourContainer.Init(this);

                tabsManager.OpenContent();
                Editor.instance.canSelect = false;
                Editor.instance.inAuthorativeMenu = true;
                dialog.SetActive(true);
            }
        }

        public void SetupDialog()
        {
            chart = GameManager.instance.Beatmap;
            PropertyTag[] tags = infoTags;
            int i = 0;

            foreach (PropertyTag property in tags)
            {
                if (chart.properties.ContainsKey(property.tag))
                {
                    Debug.Log($"Found property: {property.tag} with label {property.label}");
                    infoContainer.AddParam(this, property.tag, chart.properties[property.tag], property.label, property.isReadOnly);
                }
                else
                {
                    if (property.tag == "divider")
                    {
                        infoContainer.AddDivider(this);
                    }
                    else
                    {
                        Debug.LogWarning("Property Menu generation Warning: Property " + property.tag + " not found, skipping...");
                    }
                }
                i++;
            }
        }

        private void CleanDialog() {}

        private void Update() {}

        [Serializable]
        public class PropertyTag
        {
            public string tag;
            public string label;
            public bool isReadOnly;
        }
    }
}