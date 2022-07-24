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
        [SerializeField] private GameObject autoSearchLabel;

        private bool isAutoSearching = false;

        private void Start() {
            numConnectedLabel.text = "Connected: " + PlayerInput.GetNumControllersConnected();
            currentControllerLabel.text = "Current Controller: " + PlayerInput.GetInputController(1).GetDeviceName();
            PopulateControllersDropdown();
            PopulateSplitControllersDropdown();

            controllersDropdown.onValueChanged.AddListener(delegate 
            {
                InputController lastController = PlayerInput.GetInputController(1);
                InputController newController = PlayerInput.GetInputControllers()[controllersDropdown.value];
                lastController.SetPlayer(newController.GetPlayer() != null ? (int) newController.GetPlayer() : -1);
                newController.SetPlayer(1);
                currentControllerLabel.text = "Current Controller: " + newController.GetDeviceName();

                if (typeof(InputJoyshock) == newController.GetType()) {
                    StartCoroutine(SelectionVibrate((InputJoyshock) newController));
                }
            });
        }

        private void Update() {
            if (isAutoSearching) {
                var controllers = PlayerInput.GetInputControllers();
                foreach (var controller in controllers) {
                    if (controller.GetLastButtonDown() > 0 || controller.GetLastKeyDown() > 0) {
                        PlayerInput.GetInputController(1).SetPlayer(controller.GetPlayer() != null ? (int) controller.GetPlayer() : -1);
                        controller.SetPlayer(1);
                        isAutoSearching = false;
                        autoSearchLabel.SetActive(false);
                        controllersDropdown.value = PlayerInput.GetInputControllerId(1);
                        currentControllerLabel.text = "Current Controller: " + controller.GetDeviceName();

                        if (typeof(InputJoyshock) == controller.GetType()) {
                            StartCoroutine(SelectionVibrate((InputJoyshock) controller));
                        }
                    }
                }
            }
        }

        public void StartAutoSearch() {
            autoSearchLabel.SetActive(true);
            isAutoSearching = true;
        }

        public void SearchAndConnectControllers()
        {
            int connected = PlayerInput.InitInputControllers();
            numConnectedLabel.text = "Connected: " + connected;
            currentControllerLabel.text = "Current Controller: " + PlayerInput.GetInputController(1).GetDeviceName();
            PopulateControllersDropdown();
            PopulateSplitControllersDropdown();
        }

        public void PopulateControllersDropdown()
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

        public void PopulateSplitControllersDropdown()
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

        IEnumerator SelectionVibrate(InputJoyshock controller)
        {
            JslSetRumbleFrequency(controller.GetHandle(), 0.2f, 0.25f, 80f, 160f);
            yield return new WaitForSeconds(0.08f);
            JslSetRumbleFrequency(controller.GetHandle(), 0f, 0f, 0f, 0f);
            yield return new WaitForSeconds(0.04f);
            JslSetRumbleFrequency(controller.GetHandle(), 0.25f, 0f, 640f, 0f);
            yield return new WaitForSeconds(0.05f);
            JslSetRumbleFrequency(controller.GetHandle(), 0f, 0f, 0f, 0f);
        }
    }
}