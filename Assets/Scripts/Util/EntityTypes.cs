using System;
using System.Collections.Generic;
using System.Linq;
using Jukebox;
using Newtonsoft.Json;
using UnityEngine;

namespace HeavenStudio
{
    public class EntityTypes
    {
        public struct Integer
        {
            public int min;
            public int val;
            public int max;

            public Integer(int min, int max, int val = 0)
            {
                this.min = min;
                this.val = val;
                this.max = max;
            }
        }
        
        public struct Note
        {
            public static int maxSemitones = 36;
            public int min;
            public int val;
            public int max;
            
            public int sampleNote;
            public int sampleOctave;
            public string sampleName;
            public bool offsetToC;
            
            public Note(int val = 0, int sampleNote = 0, int sampleOctave = 4, string sampleName = null, bool offsetToC = true)
            {
                min = -maxSemitones;
                max = maxSemitones;
                
                this.val = val;
                this.sampleNote = sampleNote;
                this.sampleOctave = sampleOctave;
                this.sampleName = sampleName;
                this.offsetToC = offsetToC;
            }

            public Note(int min, int max, int val = 0, int sampleNote = 0, int sampleOctave = 4, string sampleName = null, bool offsetToC = true)
            {
                this.min = min;
                this.val = val;
                this.max = max;
                this.sampleNote = sampleNote;
                this.sampleOctave = sampleOctave;
                this.sampleName = sampleName;
                this.offsetToC = offsetToC;
            }
        }
        
        public struct Float
        {
            public float min;
            public float val;
            public float max;

            public Float(float min, float max, float val = 0)
            {
                this.min = min;
                this.val = val;
                this.max = max;
            }
        }

        // this will eventually replace Float and Integer
        public struct Number
        {
            public float snap;
            public float min;
            public float val;
            public float max;

            public Number(float snap, float min, float max, float val = 0)
            {
                this.snap = snap;
                this.min = min;
                this.val = val;
                this.max = max;
            }

            public Number(float min, float max, float val = 0)
            {
                this.snap = 0.001f;
                this.min = min;
                this.val = val;
                this.max = max;
            }
        }

        public struct Button
        {
            public string defaultLabel;
            public Func<RiqEntity, string> onClick;

            public Button(string defaultLabel, Func<RiqEntity, string> onClick)
            {
                this.defaultLabel = defaultLabel;
                this.onClick = onClick;
            }
        }

        public struct Dropdown
        {
            public int defaultValue;
            public List<string> values;

            public Dropdown(int defaultValue = 0, params string[] values)
            {
                this.defaultValue = defaultValue;
                this.values = values.ToList();
            }
        }
        
        public struct NoteSampleDropdown
        {
            public object defaultValue;
            public Func<object, NoteSample> getNoteSample;
            public string semisProp;
            
            public NoteSampleDropdown(object defaultValue, Func<object, NoteSample> getNoteSample, string semisProp)
            {
                this.defaultValue = defaultValue;
                this.getNoteSample = getNoteSample;
                this.semisProp = semisProp;
            }
        }
        
        public class DropdownObj
        {
            public void SetValues(List<string> values)
            {
                Values = values ?? new();
                onValueChanged?.Invoke(values);
            }
            public int value;
            public List<string> Values { get; private set; }
            [JsonIgnore] public string CurrentValue => value < Values?.Count ? Values?[value] : null;
            [JsonIgnore] public Action<List<string>> onValueChanged;

            public DropdownObj(int defaultValue = 0, List<string> values = null)
            {
                value = defaultValue;
                Values = values ?? new();

                onValueChanged = null;
            }

            public DropdownObj(Dropdown dd)
            {
                value = dd.defaultValue;
                Values = dd.values ?? new();

                onValueChanged = null;
            }
        }

        public struct Resource
        {
            public enum ResourceType
            {
                Image,
                Audio,
                MSMD,
                AssetBundle
            }

            public string path;
            public string name;
            public ResourceType type;

            public Resource(ResourceType type, string path, string name)
            {
                this.type = type;
                this.path = path;
                this.name = name;
            }
        }
    }
}