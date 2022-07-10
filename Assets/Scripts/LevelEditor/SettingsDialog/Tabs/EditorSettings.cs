using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HeavenStudio.Editor 
{
    public class EditorSettings : MonoBehaviour
    {
        public Toggle cursorCheckbox;

        public void OnCursorCheckboxChanged()
        {
            Editor.instance.isCursorEnabled = cursorCheckbox.isOn;
            if (!Editor.instance.fullscreen)
            {
                GameManager.instance.CursorCam.enabled = Editor.instance.isCursorEnabled;
            }
        }
    }
}