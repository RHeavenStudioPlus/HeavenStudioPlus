using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using HeavenStudio;
using HeavenStudio.InputSystem;
using static JSL;

namespace HeavenStudio.Editor 
{
    public class ControllerSettings : MonoBehaviour
    {
        [SerializeField] private TMP_Text numConnectedLabel;
        [SerializeField] private TMP_Text currentControllerLabel;
        [SerializeField] private TMP_Dropdown controllersDropdown;
        [SerializeField] private TMP_Dropdown splitControllersDropdown;

        private void Start() {
            numConnectedLabel.text = "Connected: " + PlayerInput.GetNumControllersConnected();
            currentControllerLabel.text = "Current Controller: " + PlayerInput.GetInputController(1).GetDeviceName();
        }

        public void SearchAndConnectControllers()
        {
            int connected = PlayerInput.InitInputControllers();
            numConnectedLabel.text = "Connected: " + connected;
            currentControllerLabel.text = "Current Controller: " + PlayerInput.GetInputController(1).GetDeviceName();
        }

        public void populateControllersDropdown()
        {
            List<TMP_Dropdown.OptionData> dropDownData = new List<TMP_Dropdown.OptionData>();
            var vals = PlayerInput.GetInputControllers();
            for (int i = 0; i < vals.Count; i++)
            {
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
                optionData.text = vals[i].GetDeviceName();
                dropDownData.Add(optionData);
            }
            controllersDropdown.AddOptions(dropDownData);
            controllersDropdown.value = 0;
        }

        public void populateSplitControllersDropdown()
        {
            List<TMP_Dropdown.OptionData> dropDownData = new List<TMP_Dropdown.OptionData>();
            var vals = PlayerInput.GetInputControllers();
            InputController.InputFeatures features;
            for (int i = 0; i < vals.Count; i++)
            {
                features = vals[i].GetFeatures();
                if (features.HasFlag(InputController.InputFeatures.Extra_SplitControllerLeft) || features.HasFlag(InputController.InputFeatures.Extra_SplitControllerRight))
                {
                    TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
                    optionData.text = vals[i].GetDeviceName();
                    dropDownData.Add(optionData);
                }
            }
            splitControllersDropdown.AddOptions(dropDownData);
            splitControllersDropdown.value = 0;
        }
    }
}