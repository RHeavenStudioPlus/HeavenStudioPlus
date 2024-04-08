using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using HeavenStudio.Common;
using Jukebox;

namespace HeavenStudio.Editor
{
    public class DropdownPropertyPrefab : EventPropertyPrefab
    {
        [Header("Dropdown")]
        [Space(10)]
        public LeftClickTMP_Dropdown dropdown;
        public Scrollbar scrollbar;

        public int[] values;
        private int defaultValue;
        private int lastValue = -1;
        private Array enumValues;
        private object type;
        
        private bool openedDropdown = false;
        private bool setup = false;
        
        public override void SetProperties(string propertyName, object type, string caption)
        {
            base.SetProperties(propertyName, type, caption);
            
            this.type = type;

            int selected = 0;

            switch (type)
            {
                case EntityTypes.Dropdown dropdownEntity:
                    // entity[propertyName].ChangeValues(dropdownEntity.Values);
                    defaultValue = dropdownEntity.defaultValue;
                    EntityTypes.DropdownObj dropdownObj = entity[propertyName];

                    int size = dropdownObj.Values.Count;
                    values = new int[size];

                    for (int i = 0; i < size; i++)
                    {
                        values[i] = i;
                    }

                    selected = dropdownObj.value;
                    dropdown.AddOptions(dropdownObj.Values);
                    dropdown.onValueChanged.AddListener(newVal => dropdownObj.value = newVal);
                    dropdownObj.onValueChanged = new Action<List<string>>(newValues =>
                    {
                        if (dropdown == null) return;
                        dropdown.ClearOptions();
                        dropdown.AddOptions(newValues);
                        dropdown.enabled = newValues.Count > 0;
                        dropdownObj.value = defaultValue;
                    });
                    break;
                case Enum enumEntity:
                    Type enumType = enumEntity.GetType();
                    defaultValue = (int)type;
                    enumValues = Enum.GetValues(enumType);
                    values = enumValues.Cast<int>().ToArray();
                    selected = Array.FindIndex(values, val => val == (int)entity[propertyName]);

                    dropdown.AddOptions(Enum.GetNames(enumType).ToList());
                    dropdown.onValueChanged.AddListener(val => entity[propertyName] = values[val]);
                    break;
                case EntityTypes.NoteSampleDropdown noteDropdown:
                    Type noteEnumType = noteDropdown.defaultValue.GetType();
                    enumValues = Enum.GetValues(noteEnumType);
                    values = enumValues.Cast<int>().ToArray();
                    selected = Array.FindIndex(values, val => val == (int)entity[propertyName]);
                    defaultValue = selected;
                    lastValue = selected;

                    dropdown.AddOptions(Enum.GetNames(noteEnumType).ToList());
                    dropdown.onValueChanged.AddListener(val =>
                    {
                        entity[propertyName] = values[val];
                        UpdateNoteProperty(noteDropdown, enumValues.GetValue(values[val]));
                        
                        lastValue = values[val];
                    });
                    break;
                default: break;
            }

            dropdown.value = selected;
            dropdown.enabled = dropdown.options.Count > 0;

            dropdown.onValueChanged.AddListener(newValue =>
            {
                this.caption.text = (newValue != defaultValue) ? (_captionText + "*") : _captionText;
            });
        }

        public void ResetValue()
        {
            dropdown.value = defaultValue;
        }

        public override void SetCollapses(object type)
        {
            dropdown.onValueChanged.AddListener(_ => UpdateCollapse(values[dropdown.value]));
            UpdateCollapse(values[dropdown.value]);
        }

        private void Update()
        {
            if (scrollbar != null)
            {
                if (openedDropdown == false)
                {
                    openedDropdown = true;

                    var valuePos = (float)dropdown.value / (dropdown.options.Count - 1);
                    var scrollVal = scrollbar.direction == Scrollbar.Direction.TopToBottom ? valuePos : 1.0f - valuePos;
                    scrollbar.value = Mathf.Max(0.001f, scrollVal);
                }
            }
            else
            {
                openedDropdown = false;
            }
        }

        #region Note Sample Dropdown
        private void OnEnable() { // Used for when the dropdown is uncollapsed
            if (setup && type is EntityTypes.NoteSampleDropdown sampleDropdown)
            {
                UpdateNoteProperty(sampleDropdown, enumValues.GetValue(values[entity[propertyName]]), true);
            }
        }

        public override void PostLoadProperties(object type)
        {
            base.PostLoadProperties(type);

            setup = true;

            if (type is EntityTypes.NoteSampleDropdown sampleDropdown && gameObject.activeSelf)
            {
                UpdateNoteProperty(sampleDropdown, enumValues.GetValue(values[entity[propertyName]]));
            }
        }
        
        private void UpdateNoteProperty(EntityTypes.NoteSampleDropdown noteDropdown, object newSampleEnum, bool forceSwitchCheck = false)
        {
            EventParameterManager.instance.currentProperties.TryGetValue(noteDropdown.semisProp, out var property);

            if (!property) return;

            NotePropertyPrefab noteProperty = (NotePropertyPrefab)property;
            NoteSample sample = noteDropdown.getNoteSample(newSampleEnum);
            
            bool switched = false;
            if ((int)newSampleEnum != lastValue || forceSwitchCheck) {
                // Keep the semitones value if the note is the same, otherwise reset it
                if(sample.note != noteProperty.note.sampleNote) parameterManager.entity[noteDropdown.semisProp] = 0;
                switched = true;
            }
            noteProperty.SetNote(new EntityTypes.Note(0, sample.note, sample.octave, sample.sample, offsetToC: false), switched);
        }
        #endregion
    }
}