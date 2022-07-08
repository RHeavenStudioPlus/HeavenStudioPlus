using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Editor.Track;

using TMPro;

namespace HeavenStudio.Editor 
{
    public class SettingsDialog : MonoBehaviour
    {
        [SerializeField] private GameObject settingsMenu;

        private void Start() {}

        public void SwitchSettingsDialog()
        {
            if(settingsMenu.activeSelf) {
                settingsMenu.SetActive(false);
            } else {
                settingsMenu.SetActive(true);
            }
        }

        private void Update() {}
    }
}