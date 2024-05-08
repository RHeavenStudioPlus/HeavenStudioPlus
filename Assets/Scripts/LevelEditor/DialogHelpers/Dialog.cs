using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Editor.Track;

using TMPro;

namespace HeavenStudio.Editor
{
    public class Dialog : MonoBehaviour
    {
        public bool IsOpen => dialog.activeSelf;
        [SerializeField] internal RectTransform rectTransform;
        [SerializeField] protected GameObject dialog;
        public void ForceState(bool onoff = false)
        {
            dialog.SetActive(onoff);
            if (Editor.instance == null) return;
            Editor.instance.canSelect = !onoff;
            Editor.instance.inAuthorativeMenu = onoff;
        }

        public static void ResetAllDialogs()
        {
            foreach (var dialog in FindObjectsOfType<Dialog>())
            {
                dialog.ForceState(false);
            }
        }

        public void SwitchDialogue()
        {
            if (dialog.activeSelf) {
                dialog.SetActive(false);
            } else {
                ResetAllDialogs();
                dialog.SetActive(true);
            }
        }

        public void SetPosRelativeToButtonPos(RectTransform buttonRect, Vector2? relativePos = null)
        {
            // janky? maybe. does it work? you bet it does
            rectTransform.SetParent(buttonRect);
            rectTransform.localPosition = relativePos ?? new Vector2(210, 120);
            // rectTransform.localPosition = new Vector2((rectTransform.rect.width - buttonRect.rect.width) / 2, (rectTransform.rect.height + buttonRect.rect.height) / 2);
            // rectTransform.offsetMin = Vector2.up * (buttonRect.rect.height + 7);

            rectTransform.SetParent(Editor.instance.MainCanvas.transform, true);
        }
    }
}