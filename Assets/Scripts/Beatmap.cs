using System;
using System.Collections.Generic;

namespace RhythmHeavenMania
{
    [Serializable]
    public class Beatmap
    {
        public double bpm;
        public List<Entity> entities;

        [Serializable]
        public class Entity : ICloneable
        {
            public float beat;
            public int track;
            public string datamodel;

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }
    }
}