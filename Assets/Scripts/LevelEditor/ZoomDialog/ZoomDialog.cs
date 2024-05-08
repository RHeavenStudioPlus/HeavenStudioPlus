using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Editor.Track;

using TMPro;

namespace HeavenStudio.Editor 
{
    public class ZoomDialog : Dialog
    {
        // [SerializeField] private TMP_Text snapText;
        [SerializeField] RectTransform btnRectTransform;

        public void SwitchZoomDialog()
        {
            if (dialog.activeSelf) {
                dialog.SetActive(false);
            } else {
                ResetAllDialogs();
                SetPosRelativeToButtonPos(btnRectTransform, new Vector2(146, 120));
                dialog.SetActive(true);
            }
        }
    }
}