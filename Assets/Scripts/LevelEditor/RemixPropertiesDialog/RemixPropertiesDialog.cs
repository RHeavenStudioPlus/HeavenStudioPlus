using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Editor.Track;

using TMPro;

namespace HeavenStudio.Editor 
{
    public class RemixPropertiesDialog : MonoBehaviour
    {
        [SerializeField] private GameObject propertiesMenu;

        private void Start() {}

        public void SwitchSettingsDialog()
        {
            if(propertiesMenu.activeSelf) {
                propertiesMenu.SetActive(false);
            } else {
                propertiesMenu.SetActive(true);
            }
        }

        private void Update() {}
    }
}