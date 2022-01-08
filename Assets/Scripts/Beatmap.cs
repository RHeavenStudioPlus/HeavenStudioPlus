using System;
using System.Collections.Generic;

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
            public float length;
            public float valA;
            public int type;
            public string datamodel;

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }
    }
}