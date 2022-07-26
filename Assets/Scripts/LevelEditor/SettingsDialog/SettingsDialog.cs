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
<<<<<<< HEAD
=======
        //this may all be moved to a different script in the future
>>>>>>> d65cae24d2db1df6a0e5bb4d3bd4e86fe633985f

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