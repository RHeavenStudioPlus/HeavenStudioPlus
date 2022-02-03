using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace RhythmHeavenMania.Editor
{
    public class EventPropertyPrefab : MonoBehaviour
    {
        public TMP_Text caption;
        public Slider slider;
        public TMP_InputField inputField;

        private string propertyName;

        [SerializeField] private EventParameterManager parameterManager;

        public void SetProperties(string propertyName, object type, string caption)
        {
            this.propertyName = propertyName;
            this.caption.text = caption;

            var integer = ((EntityTypes.Integer)type);

            slider.minValue = integer.min;
            slider.maxValue = integer.max;
        }

        public void TestChange()
        {
            inputField.text = slider.value.ToString();
            parameterManager.entity[propertyName] = (int)slider.value;
        }
    }
}