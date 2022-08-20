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
                // software version (MajorMinorPatch, revision)
                {"productversion", 000},
                {"productsubversion", 0},

                // general chart info
                {"remixtitle", "New Remix"},    // chart name
                {"remixauthor", "Your Name"},   // charter's name
                {"remixlevel", 1},              // chart difficulty
                {"remixtempo", 120f},           // avg. chart tempo
                {"remixtags", ""},              // chart tags
                {"icontype", 0},                // chart icon (presets, custom - future)
                {"iconurl", ""},                // custom icon location (future)

                // chart song info
                {"idolgenre", "Song Genre"},    // song genre
                {"idolsong", "Song Name"},      // song name
                {"idolcredit", "Artist"},       // song artist

                // chart prologue
                {"prologuetype", 0},            // prologue card animation (future)
                {"prologuecaption", "Remix"},   // prologue card sub-title (future)

                // chart results screen messages
                {"resultcaption", "Rhythm League Notes"},                       // result screen header
                {"resultcommon_hi", "Good rhythm."},                            // generic "Superb" message (one-liner, or second line for single-type)
                {"resultcommon_ok", "Eh. Passable."},                           // generic "OK" message (one-liner, or second line for single-type)
                {"resultcommon_ng", "Try harder next time."},                   // generic "Try Again" message (one-liner, or second line for single-type)

                    // the following are shown / hidden in-editor depending on the tags of the games used
                {"resultnormal_hi", "You show strong fundamentals."},           // "Superb" message for normal games (two-liner)
                {"resultnormal_ng", "Work on your fundamentals."},              // "Try Again" message for normal games (two-liner)

                {"resultkeep_hi", "You kept the beat well."},                   // "Superb" message for keep-the-beat games (two-liner)
                {"resultkeep_ng", "You had trouble keeping the beat."},         // "Try Again" message for keep-the-beat games (two-liner)

                {"resultaim_hi", "You had great aim."},                         // "Superb" message for aim games (two-liner)
                {"resultaim_ng", "Your aim was a little shaky."},               // "Try Again" message for aim games (two-liner)

                {"resultrepeat_hi", "You followed the example well."},          // "Superb" message for call-and-response games (two-liner)
                {"resultrepeat_ng", "Next time, follow the example better."},   // "Try Again" message for call-and-response games (two-liner)
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

            public void CreateProperty(string name, object defaultValue)
            {
                if (!DynamicData.ContainsKey(name))
                    DynamicData.Add(name, defaultValue);
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