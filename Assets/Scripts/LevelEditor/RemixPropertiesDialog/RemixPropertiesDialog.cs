using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Editor.Track;

using TMPro;

namespace HeavenStudio.Editor 
{
    public class RemixPropertiesDialog : Dialog
    {
        private void Start() {}

        public void SwitchPropertiesDialog()
        {
            if(dialog.activeSelf) {
                Editor.instance.canSelect = true;
                Editor.instance.inAuthorativeMenu = false;
                dialog.SetActive(false);
            } else {
                ResetAllDialogs();
                Editor.instance.canSelect = false;
                Editor.instance.inAuthorativeMenu = true;
                dialog.SetActive(true);
            }
        }

        private void Update() {}
    }
}