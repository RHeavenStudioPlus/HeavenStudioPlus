using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RhythmHeavenMania
{
    [Serializable]
    public class Beatmap
    {
        public float bpm;
        public List<Entity> entities = new List<Entity>();

        [Serializable]
        public class Entity : ICloneable
        {
            public float beat;
            public int track;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public float length;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public float valA;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public int type;
            public string datamodel;
            [JsonIgnore] public Editor.TimelineEventObj eventObj;

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }
    }
}