using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

using Newtonsoft.Json;

using HeavenStudio.Util;

namespace HeavenStudio
{
    [Serializable]
    public class DynamicBeatmap
    {
        public float bpm;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(100)] public int musicVolume; // In percent (1-100)
        
        public Dictionary<string, object> properties = 
            new Dictionary<string, object>() {
                {"remixtitle", "New Remix"},
                {"remixauthor", "Your Name"},
        };
        public List<DynamicEntity> entities = new List<DynamicEntity>();
        public List<TempoChange> tempoChanges = new List<TempoChange>();
        public List<VolumeChange> volumeChanges = new List<VolumeChange>();
        public float firstBeatOffset;

        [Serializable]
        public class DynamicEntity : ICloneable
        {
            public float beat;
            public int track;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public float length;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public float swing;
            public Dictionary<string, object> DynamicData = new Dictionary<string, object>();

            public string datamodel;
            [JsonIgnore] public Editor.Track.TimelineEventObj eventObj;

            public object Clone()
            {
                return this.MemberwiseClone();
            }

            public DynamicEntity DeepCopy()
            {
                //lol the AI generated this
                return JsonConvert.DeserializeObject<DynamicEntity>(JsonConvert.SerializeObject(this));
            }

            public object this[string propertyName]
            {
                get
                {
                    return DynamicData[propertyName];
                }
                set
                {
                    if (DynamicData.ContainsKey(propertyName))
                    {
                        DynamicData[propertyName] = value;
                    }
                    else
                    {
                        UnityEngine.Debug.LogError($"This entity does not have a property named {propertyName}! Attempted to insert value of type {value.GetType()}");
                    }
                }
            }
        }

        [Serializable]
        public class TempoChange : ICloneable
        {
            public float beat;
            public float length;
            public float tempo;

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }

        [Serializable]
        public class VolumeChange : ICloneable
        {
            public float beat;
            public float length;
            public float volume;

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }

        public object this[string propertyName]
        {
            get
            {
                return properties[propertyName];
            }
            set
            {
                if (properties.ContainsKey(propertyName))
                {
                    properties[propertyName] = value;
                }
                else
                {
                    UnityEngine.Debug.LogError($"This beatmap does not have a property named {propertyName}! Attempted to insert value of type {value.GetType()}");
                }
            }
        }
    }
}