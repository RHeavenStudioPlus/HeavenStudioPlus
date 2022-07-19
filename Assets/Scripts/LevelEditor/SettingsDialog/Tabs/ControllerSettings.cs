using UnityEngine;
using UnityEngine.UI;
using TMPro;

using HeavenStudio;
using static JSL;

namespace HeavenStudio.Editor 
{
    public class ControllerSettings : MonoBehaviour
    {
        [SerializeField] private TMP_Text numConnectedLabel;
        [SerializeField] private TMP_Text currentControllerLabel;

        public void SearchAndConnectControllers()
        {
            int connected = PlayerInput.InitJoyShocks();
            numConnectedLabel.text = "Connected: " + connected;
            //do this better
            currentControllerLabel.text = "Current Controller: " + PlayerInput.GetJoyShockName(0);
        }
    }
}