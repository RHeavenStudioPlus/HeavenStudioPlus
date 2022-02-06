using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Editor
{
    public class EventPropertyPrefab : MonoBehaviour
    {
        public TMP_Text caption;
        [SerializeField] private EventParameterManager parameterManager;

        [Header("Integer and Float")]
        [Space(10)]
        public Slider slider;
        public TMP_InputField inputField;

        [Header("Dropdown")]
        [Space(10)]
        public TMP_Dropdown dropdown;

        private string propertyName;


        public void SetProperties(string propertyName, object type, string caption)
        {
            this.propertyName = propertyName;
            this.caption.text = caption;

            var objType = type.GetType();

            if (objType == typeof(EntityTypes.Integer))
            {
                var integer = ((EntityTypes.Integer)type);

                slider.minValue = integer.min;
                slider.maxValue = integer.max;

                slider.value = Mathf.RoundToInt(System.Convert.ToSingle(parameterManager.entity[propertyName]));
                inputField.text = slider.value.ToString();

                slider.onValueChanged.AddListener(delegate 
                {
                    inputField.text = slider.value.ToString();
                    parameterManager.entity[propertyName] = (int)slider.value;
                });

                inputField.onEndEdit.AddListener(delegate
                {
                    slider.value = Mathf.RoundToInt(System.Convert.ToSingle(System.Convert.ToSingle(inputField.text)));
                    parameterManager.entity[propertyName] = (int)slider.value;
                });
            }
            else if (objType == typeof(EntityTypes.Float))
            {
                var fl = ((EntityTypes.Float)type);

                slider.minValue = fl.min;
                slider.maxValue = fl.max;

                slider.value = System.Convert.ToSingle(parameterManager.entity[propertyName]);
                inputField.text = slider.value.ToString("G4"); // G4 = Display no more than 4 decimal places.

                slider.onValueChanged.AddListener(delegate 
                {
                    inputField.text = slider.value.ToString("G4");
                    parameterManager.entity[propertyName] = (float)System.Math.Round(slider.value, 4);
                });

                inputField.onEndEdit.AddListener(delegate 
                {
                    slider.value = (float)System.Math.Round(System.Convert.ToSingle(inputField.text), 4);
                    parameterManager.entity[propertyName] = slider.value;
                });
            }
            else if (objType == typeof(EasingFunction.Ease))
            {
                List<TMP_Dropdown.OptionData> dropDownData = new List<TMP_Dropdown.OptionData>();
                for (int i = 0; i < System.Enum.GetValues(typeof(EasingFunction.Ease)).Length; i++)
                {
                    string name = System.Enum.GetNames(typeof(EasingFunction.Ease))[i];
                    TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();

                    optionData.text = name;

                    dropDownData.Add(optionData);
                }
                dropdown.AddOptions(dropDownData);
                dropdown.value = ((int)(EasingFunction.Ease)parameterManager.entity[propertyName]);

                dropdown.onValueChanged.AddListener(delegate 
                {
                    parameterManager.entity[propertyName] = (EasingFunction.Ease)dropdown.value;
                });
            }
        }
    }
}